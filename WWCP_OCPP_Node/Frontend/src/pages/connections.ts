import { connectionsAPI as api, type ConnectionIdentity, type ConnectionState, type ConnectionTest, type ConnectionTestAsks, type ConnectionToSave, type LoginToSave, type EncodingOfAConnection, type HTTPVersionOfAConnection, type SignatureFormatsOfAConnection, type StationConnection, type StationConnections } from '../api/connections';
import { auth } from '../auth';
import { credentialsPanel } from './authentication';
import { connectionsResource, nodeNoun, nodeNounInFull, nodeTraits } from '../settings';
import { toURL } from '@node/basePath';
import { must } from '@node/html';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { rememberTab, tabFromURL, tabsView, type Tab } from '@node/tabs';
import { errorMessage, field, formatTime, formatTimestamp, whileSaving } from '@node/ui';
import { anyFormTypedSinceDrawn, unsaved } from '@node/unsaved';
import { html, nothing, render, repeat, type TemplateResult } from '@node/view';
import { keysOf, pinsIn, saysAnything, type StoreOffers } from '@node/pages/pins';
import { heldToView, judgementView, pinsFieldset, readPinsFieldset, storeOffers } from '@node/pages/pinViews';
import { noLongerWrittenDown, stateLine, type StateLine } from './connectionStates';

/** The versions of HTTP the WebSocket of a connection may be opened over, as the station names them. */
const httpVersions: ReadonlyArray<readonly [HTTPVersionOfAConnection, string]> = [
    [ 'auto', 'As the server chooses' ],
    [ '1.1',  'HTTP/1.1 and nothing else' ],
    [ '2',    'HTTP/2 and nothing else' ],
    [ '3',    'HTTP/3 and nothing else' ]
];

/** How the OCPP messages of a connection may be written, as the station names it. */
const encodings: ReadonlyArray<readonly [EncodingOfAConnection, string]> = [
    [ 'json', 'JSON, as OCPP defines it' ],
    [ 'cbor', 'CBOR and nothing else' ],
    [ 'auto', 'CBOR where the back end speaks it' ]
];

/** Which signatures a connection may write, as the station names them. */
const signatureFormats: ReadonlyArray<readonly [SignatureFormatsOfAConnection, string]> = [
    [ 'policy', 'As the signature policy says' ],
    [ 'jws',    'JWS, over the JSON of a message' ],
    [ 'cose',   'COSE, over the CBOR of a message' ],
    [ 'both',   'Both, next to each other' ]
];

/**
 * The places this charging station dials.
 *
 * A connection names the credentials it uses rather than carrying them: a
 * station that reaches its management system twice - normally and through a
 * spare address - proves itself the same way both times, and a password written
 * down twice is a password that only gets changed once.
 *
 * How it proves itself is one decision, and what there is to decide follows
 * where it goes: over ws:// and wss:// nothing, a set of credentials - one
 * written down already, or written down here with the connection - or, where
 * there is a TLS handshake, the client certificates it shows; over ssh:// the
 * SSH keys it signs in with, and nothing else. Certificates and keys are
 * ticked, several where one follows another: each from a time on, so that a
 * key is changed at the moment its server was told and not when somebody
 * happens to stand beside the station.
 *
 * The credentials are a tab of this page, because this is where they are
 * used: a login is written down for a connection, and looked up from one.
 *
 * What is configured but cannot work is said here rather than discovered at the
 * next connection - a password going out over a connection without TLS, a
 * certificate on a URL that makes no TLS handshake, credentials that were
 * removed next door. None of it is refused, because a station on a bench
 * talking to a test back end over ws:// is a real thing to want.
 *
 * An ssh:// connection signs in with this station's own SSH key, whose public
 * half is shown here to be handed to the server, and checks the server by the
 * host key written beside it - or, where none is, believes the first one and
 * writes it in.
 *
 * A wss:// connection is checked by its CA, or - with key pins, typically for a
 * local controller that no CA issues a certificate for - by the key of its
 * server alone: one or the other, never both. It learns the key pin of the
 * first key shown only where that is configured, and on probation; the card
 * says which, and the test says which key pin the server showed.
 *
 * And where each connection stands - connected, lost and coming back, turned
 * away - asked again every few seconds while the page is in view, because this
 * is where somebody looks to find out whether the station is on its back end,
 * and a page that was right when it was opened is not the answer to that.
 */
export const connectionsPage: Page = {

    title: 'Connections',

    render({ root }) {

        const page = shell(root, {
            active:    '/configuration/connections',
            title:     'Connections',
            subtitle:  `Where this ${nodeNounInFull()} dials, and what it proves itself with.`,
            actions:   reloadButton(() => reload())
        });

        /** The connections, and the credentials they prove themselves with - which are used here, and so are kept here. */
        const tabs: readonly Tab[] = [
            { id: 'connections', label: 'Connections', icon: 'fa-plug' },
            { id: 'credentials', label: 'Credentials', icon: 'fa-user-lock' }
        ];

        let shown = tabFromURL(tabs);

        function show(id: string): void {
            shown = id;
            rememberTab(tabs, id);
            frame();
        }

        /** The row of tabs and their two panels: each panel drawn by its own, so that what is typed into one outlives a look at the other. */
        function frame(): void {
            render(page, html`
                ${tabsView(tabs, shown, show, 'Connections')}
                <section class="tab-panel" role="tabpanel" id="panel-connections" aria-labelledby="tab-connections" ?hidden=${shown !== 'connections'}></section>
                <section class="tab-panel" role="tabpanel" id="panel-credentials" aria-labelledby="tab-credentials" ?hidden=${shown !== 'credentials'}></section>
            `);
        }

        frame();

        const content = must<HTMLElement>(page, '#panel-connections');

        render(content, html`<div class="loading">Loading ...</div>`);

        // Handed what the connections were loaded with, and telling them of
        // every change: a login written down on its tab is on offer in the
        // forms of the connections at once.
        const credentials = credentialsPanel(must<HTMLElement>(page, '#panel-credentials'), {
            handed:   true,
            changed:  state => { took(state, false); draw(); }
        });

        const mayManage = auth.can(connectionsResource(), 'edit');

        // Its own permission: a test makes this station open a connection to a
        // host and show it a credential, which is more than reading a page and
        // less than changing what the station does.
        const mayTest   = auth.can(connectionsResource(), 'run');

        let cancelled = false;
        let store: StationConnections | null = null;

        // Where the connections stand, and the station's clock when it said
        // so - kept apart from the store, because they are asked again on
        // their own. Drawn with the rest of the page: a draw leaves what is
        // typed into the forms where it is.
        let states:     Record<string, ConnectionState> = {};
        let timestamp   = '';
        let notAnswered: string | null = null;

        const opened = new Set<string>();

        /**
         * What is typed into the URL of a form and not saved yet, by the form
         * - "add", or the id of the connection. What a connection can prove
         * itself with follows its scheme, and follows it as it is typed.
         */
        const typedURLs = new Map<string, string>();

        /** The way picked in a form and not saved yet, by the form: what is shown below the choice follows it at once. */
        const ways      = new Map<string, string>();

        // What the certificate store keeps for the servers of the OCPP
        // connections: the roots and certificates a pin can be picked from.
        let offers: StoreOffers | null = null;

        /**
         * The connections as the station has them now. Whatever the store
         * came with is the newest there is of where they stand: it was
         * loaded, or written, a moment ago.
         */
        function took(State: StationConnections, tellTheCredentials = true): void {
            store        = State;
            states       = State.states ?? {};
            timestamp    = State.timestamp ?? timestamp;
            notAnswered  = null;
            if (tellTheCredentials)
                credentials.took(State);
        }

        /** The identities a connection names: as a list, or the one certificate it named before it could name several. */
        function identitiesOf(entry: StationConnection | null): ConnectionIdentity[] {
            return entry?.identities
                ?? (entry?.certificateId !== undefined && entry.certificateId !== null ? [ { id: entry.certificateId } ] : []);
        }

        /** What the choice of a ws:// or wss:// connection calls what it has: nothing, credentials, or client certificates. */
        function chosenOf(entry: StationConnection | null): string {
            if (entry?.authenticationId !== undefined && entry.authenticationId !== null)
                return `auth:${entry.authenticationId}`;
            if (entry?.transport !== 'ssh' && identitiesOf(entry).length > 0)
                return 'tls';
            return '';
        }

        /** How the URL of a form goes, as it is typed: over SSH, with TLS, without - or not said yet. */
        function schemeIn(id: string, entry: StationConnection | null): 'ssh' | 'wss' | 'ws' | '' {
            const url = (typedURLs.get(id) ?? entry?.url ?? '').trim().toLowerCase();
            return url.startsWith('ssh://')                                 ? 'ssh'
                 : url.startsWith('wss://') || url.startsWith('https://')   ? 'wss'
                 : url.startsWith('ws://')  || url.startsWith('http://')    ? 'ws'
                 :                                                            '';
        }

        /** A time as an input of the browser shows it: its own time, to the minute. */
        function localOf(iso: string | undefined): string {
            if (iso === undefined)
                return '';
            const at = new Date(iso);
            if (Number.isNaN(at.getTime()))
                return '';
            const two = (n: number) => String(n).padStart(2, '0');
            return `${at.getFullYear()}-${two(at.getMonth() + 1)}-${two(at.getDate())}T${two(at.getHours())}:${two(at.getMinutes())}`;
        }


        function draw(): void {

            if (store === null)
                return;

            const state = store;

            render(content, html`

                ${mayManage ? nothing : html`
                    <div class="notice">${mayButNot('look at the connections', 'change them')}</div>
                `}

                <section class="card">

                    <h2><i class="fa-solid fa-plus"></i> Write down a connection</h2>

                    <form id="add-form" class="form-stack" @submit=${add}>
                        ${theFields('add', null, state)}
                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayManage}>
                                Write it down
                            </button>
                            <button type="button" class="btn small" data-test="add"
                                    ?disabled=${!mayTest} @click=${testThisForm}>
                                <i class="fa-solid fa-plug-circle-check"></i> Test it
                            </button>
                            ${whatATestAsks(mayTest)}
                            <span id="add-note"  class="form-notice" role="status"></span>
                            <span id="add-error" class="form-error"  role="alert"></span>
                        </div>
                        <span class="hint">
                            Testing does not write anything down: it makes the connection once, asks its back
                            end for a Heartbeat, says what happened and when, and lets go. Worth doing before
                            saving rather than after. A back end that is not to be asked anything - one that
                            takes a Heartbeat for a station having come - is tested by asking nothing: then the
                            test ends with the handshake.
                        </span>
                    </form>

                </section>

                ${sshKeyView(state)}
                ${signersView(state)}

                <section class="card">

                    <h2><i class="fa-solid fa-list"></i> What is configured</h2>

                    <p class="hint">Newest first. Written to ${state.directory}.</p>

                    <p class="hint" id="states-note" role="status">${notAnswered === null
                        ? nothing
                        : `Where the connections stand is as it was at ${formatTime(timestamp)}: ` +
                          `asking again did not work (${notAnswered}).`}</p>

                    ${state.connections.length === 0
                          ? Object.keys(states).length === 0
                                ? html`
                                      <p class="hint">
                                          Nothing yet. This ${nodeNoun()} dials nowhere and waits to be dialled.
                                      </p>
                                  `
                                : html`<p class="hint">Nothing written down any more.</p>`
                          : html`<div class="cards">${repeat(state.connections, entry => entry.id, entry => entryCard(entry, state))}</div>`}

                    <div id="no-longer-written-down">${noLongerWrittenDownView()}</div>

                </section>

            `);

        }

        /** The same fields whether it is new or being changed, so the two cannot drift apart. */
        function theFields(id: string, entry: StationConnection | null, state: StationConnections): TemplateResult {

            const chosen = chosenOf(entry);

            return html`

                <label>What it is
                    <input type="text" name="description" maxlength="${state.maxDescriptionLength}"
                           placeholder="CSMS, main" value="${entry?.description ?? ''}"
                           ?disabled=${!mayManage} />
                </label>

                <label>Where it goes
                    <input type="text" name="url" class="mono"
                           placeholder="wss://csms.example.org/cs001" value="${entry?.url ?? ''}"
                           ?disabled=${!mayManage}
                           @input=${(event: Event) => typedURL(id, entry, (event.target as HTMLInputElement).value)} />
                    <span class="hint">
                        Write the scheme. <code>wss://</code> is encrypted and <code>ws://</code> is not, and
                        this ${nodeNoun()} will not decide that for you - everything below depends on which it is.
                        <code>ssh://cs001@csms.example.org:2222</code> speaks the same OCPP over SSH, signed in
                        with this ${nodeNoun()}'s own key.
                    </span>
                </label>

                <label>Host keys of its server
                    <input type="text" name="sshHostKey" class="mono"
                           placeholder="SHA256:... or ssh-ed25519 AAAA..." value="${entry?.sshHostKey ?? ''}"
                           ?disabled=${!mayManage} />
                    <span class="hint">
                        For an <code>ssh://</code> URL only: the SHA-256 fingerprints or public-key lines its server
                        may show, separated by commas - two while it changes its key. Testing says which key it shows.
                        MD5 fingerprints are not taken.
                    </span>
                </label>

                <div class="form-row">

                    <label>Where nothing is pinned
                        <select name="sshTrustOnFirstUse" ?disabled=${!mayManage}>
                            <option value="None"    ?selected=${(entry?.sshTrustOnFirstUse ?? 'None') === 'None'}>Do not dial</option>
                            <option value="HostKey" ?selected=${entry?.sshTrustOnFirstUse === 'HostKey'}>Learn the first key, on probation</option>
                        </select>
                        <span class="hint">
                            A learned key is on probation until a signed pin set confirms it; until then nothing but such a
                            pin set is executed, and a key nobody confirms in time is forgotten and learned again.
                        </span>
                    </label>

                    <label>Probation, in seconds
                        <input type="number" name="sshProbationTime" min="1" max="604800"
                               value="${entry?.sshProbationTime ?? 600}" ?disabled=${!mayManage} />
                    </label>

                    <label>Where the key matches no pin
                        <select name="sshOnPinMismatch" ?disabled=${!mayManage}>
                            <option value="Refuse" ?selected=${(entry?.sshOnPinMismatch ?? 'Refuse') === 'Refuse'}>Refuse the server</option>
                            <option value="Record" ?selected=${entry?.sshOnPinMismatch === 'Record'}>Believe it, and report it</option>
                            <option value="Accept" ?selected=${entry?.sshOnPinMismatch === 'Accept'}>Believe it, and log it</option>
                        </select>
                        <span class="hint">
                            A host key pin is the only check of the server: believing another key believes whoever answers.
                            For test benches and the move to a new server only; never where a password is sent.
                        </span>
                    </label>

                </div>

                <label>Key pins of its server
                    <input type="text" name="serverKeyPins" class="mono"
                           placeholder="64 hexadecimal digits, or sha256//..." value="${(entry?.serverKeyPins ?? []).join(', ')}"
                           ?disabled=${!mayManage} />
                    <span class="hint">
                        For a <code>wss://</code> URL only, typically a local controller that no CA issues a certificate for:
                        SHA-256 over the key of its server, at most four, separated by commas - two while it changes its key.
                        With one, the key is the whole check: no CA, no name and no validity period, so the ${nodeNoun()} needs no
                        clock to believe it. Without one, the certificate is checked by its CA. Testing says which key pin it shows.
                    </span>
                </label>

                <div class="form-row">

                    <label>Where no key is pinned
                        <select name="trustOnFirstUse" ?disabled=${!mayManage}>
                            <option value="None" ?selected=${(entry?.trustOnFirstUse ?? 'None') === 'None'}>Check its certificate by its CA</option>
                            <option value="Key"  ?selected=${entry?.trustOnFirstUse === 'Key'}>Learn the first key, on probation</option>
                        </select>
                        <span class="hint">
                            A learned key pin is on probation until a signed pin set confirms it; until then nothing but such a
                            pin set is executed, and a key pin nobody confirms in time is forgotten and learned again.
                        </span>
                    </label>

                    <label>Probation, in seconds
                        <input type="number" name="serverKeyPinProbationTime" min="1" max="604800"
                               value="${entry?.serverKeyPinProbationTime ?? 600}" ?disabled=${!mayManage} />
                    </label>

                    <label>Raw public keys
                        <select name="rawPublicKeys" ?disabled=${!mayManage}>
                            <option value="Off"     ?selected=${(entry?.rawPublicKeys ?? 'Off') === 'Off'}>Off - the key in a certificate</option>
                            <option value="Prefer"  ?selected=${entry?.rawPublicKeys === 'Prefer'}>Prefer them</option>
                            <option value="Require" ?selected=${entry?.rawPublicKeys === 'Require'}>Require them</option>
                        </select>
                        <span class="hint">
                            Only where the key is pinned or learned. This ${nodeNoun()}'s TLS stack cannot offer raw public keys yet:
                            Prefer checks the key in a certificate, and Require is not dialled.
                        </span>
                    </label>

                </div>

                <div data-server-pins>

                    <p class="hint">
                        The other way to know the server of a <code>wss://</code> URL: by its CA, and held to more than that -
                        as a time server, a name server and a meter are. Its certificate must be issued for its name and chain
                        to a root this machine trusts, or to a TLS root of this ${nodeNoun()}'s certificate store kept for
                        <code>ocpp</code>; a CA of your own goes there, or into a group of roots, and not into a pin. One way
                        or the other: none of this goes with key pins above, which ask no CA.
                    </p>

                    ${pinsFieldset(pinsIn(entry?.pins ?? {}), {
                          service:   'ocpp',
                          what:      'server',
                          evidence:  false,
                          shown:     entry?.judgement
                                         ? { name:         entry.judgement.server,
                                             certificate:  entry.judgement.certificate ?? null,
                                             root:         entry.judgement.root        ?? null }
                                         : null,
                          offers
                      })}

                </div>

                <div class="form-row">

                    <label>What is at the other end
                        <select name="connectionType" ?disabled=${!mayManage}>
                            ${state.connectionTypes.map(one => html`
                                <option value="${one}" ?selected=${one === (entry?.connectionType ?? 'CSMS')}>
                                    ${one === 'CSMS'       ? 'Charging station management system'
                                      : one === 'CSMSBackup' ? 'Management system, spare'
                                      : 'Local controller'}
                                </option>
                            `)}
                        </select>
                        <span class="hint">
                            What it is, for whoever reads this page. Every connection is dialled on its own;
                            nothing here waits for anything else.
                        </span>
                    </label>

                    <label>Which OCPP
                        <select name="ocppVersion" ?disabled=${!mayManage}>
                            ${state.ocppVersions.map(one => html`
                                <option value="${one}" ?selected=${one === (entry?.ocppVersion ?? 'OCPP2.1')}>
                                    ${one}
                                </option>
                            `)}
                        </select>
                        <span class="hint">
                            ${nodeTraits().oneNodePerOCPPVersion
                                  ? `This ${nodeNoun()} is two nodes and a URL does not say which one should dial, so it is said here.`
                                  : 'A URL does not say which OCPP is spoken over it, so it is said here.'}
                        </span>
                    </label>

                    <label>Which HTTP
                        <select name="httpVersion" ?disabled=${!mayManage}>
                            ${httpVersions.map(([value, text]) => html`
                                <option value="${value}" ?selected=${value === (entry?.httpVersion ?? 'auto')}>
                                    ${text}
                                </option>
                            `)}
                        </select>
                        <span class="hint">
                            What the WebSocket of a ws:// or wss:// connection is opened over. Left to the
                            server, this ${nodeNoun()} offers HTTP/2 and HTTP/1.1 and takes what the server
                            chooses; without TLS there is nothing to choose with, and it is HTTP/1.1. A
                            version that is named is that one and nothing else.
                        </span>
                    </label>

                    <label>Messages as
                        <select name="encoding" ?disabled=${!mayManage}>
                            ${encodings.map(([value, text]) => html`
                                <option value="${value}" ?selected=${value === (entry?.encoding ?? 'json')}>
                                    ${text}
                                </option>
                            `)}
                        </select>
                        <span class="hint">
                            How the OCPP messages of this connection are written. CBOR is the same
                            messages in binary frames, agreed on as the subprotocol "ocpp2.1cbor" or
                            "ocpp1.6cbor": a back end that does not speak it is not connected to with
                            "CBOR and nothing else", and is spoken to in JSON with "CBOR where the back
                            end speaks it".
                        </span>
                    </label>

                    <label>Signatures as
                        <select name="signatureFormats" ?disabled=${!mayManage}>
                            ${signatureFormats.map(([value, text]) => html`
                                <option value="${value}" ?selected=${value === (entry?.signatureFormats ?? 'policy')}>
                                    ${text}
                                </option>
                            `)}
                        </select>
                        <span class="hint">
                            Which signatures the messages this ${nodeNoun()} signs for this connection get.
                            A COSE signature signs the CBOR of a message whatever the connection
                            carries, and is read by a back end that knows the message. What the
                            ${nodeNoun()} verifies is not a matter of this: every signature it can read.
                        </span>
                    </label>

                </div>

                <label class="checkbox">
                    <input type="checkbox" name="autoConnect"
                           ?checked=${entry?.autoConnect ?? false}
                           ?disabled=${!mayManage} />
                    Connect by itself, and again after a drop
                    <span class="hint">
                        This is what decides whether the connection is used at all. Off, it stays written
                        down and nothing happens - an address kept ready for the day somebody needs it,
                        which can still be tried by hand with the button below.
                    </span>
                </label>

                ${provesView(id, entry, state)}

            `;

        }

        /** The URL of a form was typed into: where what it can prove itself with changes, the form is drawn again. */
        function typedURL(id: string, entry: StationConnection | null, url: string): void {

            const before = schemeIn(id, entry);

            typedURLs.set(id, url);

            if (schemeIn(id, entry) !== before)
                draw();

        }

        /** A way was picked: what is asked below it follows. */
        function pickWay(id: string, way: string): void {
            ways.set(id, way);
            draw();
        }

        /**
         * What a connection proves itself with, as its URL allows: over SSH
         * the keys it signs in with, and otherwise one choice - nothing,
         * credentials, or the client certificates it shows.
         */
        function provesView(id: string, entry: StationConnection | null, state: StationConnections): TemplateResult {

            const scheme = schemeIn(id, entry);

            if (scheme === 'ssh')
                return sshProvesView(id, entry, state);

            const had     = chosenOf(entry);
            const way     = ways.get(id) ?? had;
            const nothingToChoose = state.authentications.length === 0 && state.certificates.length === 0;

            return html`

                <label>How it proves itself
                    <select name="proves" ?disabled=${!mayManage}
                            @change=${(event: Event) => pickWay(id, (event.target as HTMLSelectElement).value)}>

                        <option value="" ?selected=${way === ''}>
                            Nothing - the back end must not ask
                        </option>

                        <optgroup label="A login">
                            ${state.authentications.map(one => html`
                                <option value="auth:${one.id}" ?selected=${way === `auth:${one.id}`}>
                                    ${one.description} (${one.kind === 'basic' ? 'HTTP Basic' : 'HTTP TOTP'})${one.hasSecret ? '' : ' - no secret set'}
                                </option>
                            `)}
                            <option value="new:basic" ?selected=${way === 'new:basic'}>New login and password ...</option>
                            <option value="new:totp"  ?selected=${way === 'new:totp'}>New TOTP secret ...</option>
                        </optgroup>

                        ${scheme === 'wss' || way === 'tls' ? html`
                            <option value="tls" ?selected=${way === 'tls'}>TLS client certificates</option>
                        ` : nothing}

                    </select>
                    <span class="hint" data-proves-hint>
                        One of them.
                        ${scheme === 'ws'
                              ? html`A <code>ws://</code> connection makes no TLS handshake, so it shows no client certificate
                                     - and a password goes over it for everybody on the way to read.`
                              : scheme === ''
                                    ? html`Over <code>wss://</code> a client certificate as well; over <code>ssh://</code> the SSH keys it signs in with instead.`
                                    : nothing}
                        ${nothingToChoose ? html`
                            Nothing is written down yet that it could prove itself with: choose a new login here, or make a
                            key on the <a href="${toURL('/configuration/identities')}?tab=request">Client identities</a> page.
                        ` : nothing}
                    </span>
                </label>

                ${way === 'new:basic' || way === 'new:totp' ? html`
                    <fieldset class="proves-with" data-new-login>
                        <legend>${way === 'new:basic' ? 'A new login and password' : 'A new TOTP secret'}</legend>
                        <label>What they are for
                            <input type="text" name="newDescription" maxlength="${state.maxDescriptionLength}"
                                   placeholder="CSMS login" ?disabled=${!mayManage} />
                            <span class="hint">Left empty, they are called what the connection is.</span>
                        </label>
                        <label>Login
                            <input type="text" name="newLogin" placeholder="cs001" ?disabled=${!mayManage} />
                        </label>
                        <label>${way === 'new:basic' ? 'Password' : 'Shared secret'}
                            <input type="password" name="newSecret" autocomplete="new-password" ?disabled=${!mayManage} />
                            <span class="hint">
                                ${way === 'new:totp' ? `At least ${state.minSharedSecretLength} characters. ` : ''}Written
                                down with the connection, on the Credentials tab, where another connection may use them as
                                well and where a TOTP's validity and length are set. Never shown again. A test before that
                                goes without them.
                            </span>
                        </label>
                    </fieldset>
                ` : nothing}

                ${way === 'tls' ? html`
                    <fieldset class="proves-with" data-identities="tls">
                        <legend>The client certificates it shows</legend>
                        ${identityRows(entry,
                                       state.certificates.map(one => ({
                                           id:      one.id,
                                           name:    one.label || one.subject || one.id,
                                           detail:  `${one.algorithm}${one.notAfter !== undefined ? `, until ${formatTimestamp(one.notAfter)}` : ''}${one.canBeHeldUp ? '' : ' - cannot be shown right now'}`
                                       })))}
                        <span class="hint">
                            ${state.certificates.length === 0 ? html`This ${nodeNoun()} has no TLS identity yet. ` : nothing}
                            One certificate is shown in a handshake: of those ticked whose time has come, the one that came
                            last - and the one before it while that one cannot be shown. Tick the certificate that follows
                            beside the one in use, with the time its server takes it from, in this browser's time. A key is
                            made, and its certificate brought, on the
                            <a href="${toURL('/configuration/identities')}?tab=request">Client identities</a> page.
                        </span>
                    </fieldset>
                ` : nothing}

            `;

        }

        /** Over SSH: the keys it signs in with, and nothing else to choose. */
        function sshProvesView(_id: string, entry: StationConnection | null, state: StationConnections): TemplateResult {

            const password = state.authentications.find(one => one.id === entry?.authenticationId);

            return html`
                <fieldset class="proves-with" data-identities="ssh">
                    <legend>The SSH keys it signs in with</legend>
                    ${identityRows(entry?.transport === 'ssh' ? entry : null,
                                   (state.sshKeys ?? []).map(one => ({
                                       id:      one.id,
                                       name:    one.label,
                                       detail:  one.made ? (one.fingerprint ?? '') : 'made when it is first needed'
                                   })))}
                    <span class="hint">
                        None ticked, it signs in with this ${nodeNoun()}'s own key. Several are offered to the server one
                        after another, the one whose time came last first: tick the key that follows beside the one in
                        use, with the time its server takes it from, in this browser's time. Keys are made on the
                        <a href="${toURL('/configuration/identities')}">Client identities</a> page, where the line for the
                        server's <code>authorized_keys</code> is.
                    </span>
                </fieldset>
                ${password !== undefined ? html`
                    <label class="checkbox">
                        <input type="checkbox" name="keepPassword" checked ?disabled=${!mayManage} />
                        And the password of ${password.description}, where the server asks for one
                        <span class="hint">Written down before a connection over SSH signed in with keys alone. Unticked, it is not sent any more.</span>
                    </label>
                ` : nothing}
            `;

        }

        /** What a connection may name, each to tick and each from a time on - and what it names that is no longer here. */
        function identityRows(entry:    StationConnection | null,
                              offered:  { id: string; name: string; detail: string }[]): TemplateResult {

            const named = new Map(identitiesOf(entry).map(one => [ one.id, one ]));
            const gone  = [ ...named.keys() ].filter(id => !offered.some(one => one.id === id)).
                                              map(id => ({ id, name: id, detail: 'no longer here' }));

            return html`
                <div class="identity-rows">
                    ${[ ...offered, ...gone ].map(one => html`
                        <div class="identity-row" data-identity="${one.id}">
                            <label class="checkbox">
                                <input type="checkbox" name="identity" value="${one.id}"
                                       ?checked=${named.has(one.id)} ?disabled=${!mayManage} />
                                ${one.name} <span class="hint">${one.detail}</span>
                            </label>
                            <label class="identity-from">from
                                <input type="datetime-local" name="from:${one.id}"
                                       value="${localOf(named.get(one.id)?.from)}" ?disabled=${!mayManage} />
                            </label>
                        </div>
                    `)}
                </div>
            `;

        }

        /** What a card says a connection proves itself with. */
        function provesSaid(entry: StationConnection, state: StationConnections): TemplateResult {

            const credentials = state.authentications.find(one => one.id === entry.authenticationId);
            const identities  = identitiesOf(entry);

            const from = (one: ConnectionIdentity) => one.from !== undefined ? html` <span class="hint">from ${formatTimestamp(one.from)}</span>` : nothing;
            const list = (names: (one: ConnectionIdentity) => string) =>
                             identities.map((one, at) => html`${at > 0 ? ', ' : ''}${names(one)}${from(one)}`);

            if (entry.transport === 'ssh') {

                const keys = state.sshKeys ?? [];

                return html`
                    ${identities.length === 0
                          ? html`This ${nodeNoun()}'s SSH key`
                          : html`${identities.length === 1 ? 'The SSH key ' : 'The SSH keys '}${list(one => keys.find(key => key.id === one.id)?.label ?? `${one.id} (no longer here)`)}`}${credentials !== undefined
                          ? html`, and the password of ${credentials.description}`
                          : entry.authenticationId !== undefined ? html`, and a password that is no longer here` : nothing}
                `;

            }

            if (credentials !== undefined)
                return html`${credentials.description} (${credentials.kind === 'basic' ? 'HTTP Basic' : 'HTTP TOTP'})`;

            if (identities.length > 0)
                return html`${identities.length === 1 ? 'TLS identity ' : 'TLS identities '}${list(one => {
                    const certificate = state.certificates.find(known => known.id === one.id);
                    return certificate !== undefined ? (certificate.label || certificate.subject || certificate.id) : `${one.id} (no longer here)`;
                })}`;

            return entry.authenticationId !== undefined
                       ? html`<span class="hint">something that is no longer here</span>`
                       : html`<span class="hint">nothing</span>`;

        }

        function entryCard(entry: StationConnection, state: StationConnections): TemplateResult {

            return html`
                <div class="card">

                    <div class="key-head">
                        <strong>${entry.description}</strong>
                        <span class="chip">${entry.connectionType}</span>
                        <span class="chip">${entry.ocppVersion}</span>
                        ${entry.httpVersionInUse ? html`
                            <span class="chip" data-http="${entry.id}"
                                  title="${(entry.httpVersion ?? 'auto') === 'auto'
                                              ? 'Its WebSocket was opened over this version of HTTP, which its server chose.'
                                              : 'Its WebSocket was opened over this version of HTTP, and is over no other.'}">HTTP/${entry.httpVersionInUse}</span>
                        ` : nothing}
                        ${entry.subprotocolInUse?.endsWith('cbor') ? html`
                            <span class="chip" data-encoding="${entry.id}"
                                  title="${(entry.encoding ?? 'json') === 'auto'
                                              ? `It agreed on ${entry.subprotocolInUse}: its messages are CBOR, which its back end chose.`
                                              : `It agreed on ${entry.subprotocolInUse}: its messages are CBOR.`}">CBOR</span>
                        ` : nothing}
                        ${entry.transport === 'ssh'
                              ? html`<span class="chip on">SSH</span>`
                              : entry.secure
                                    ? html`<span class="chip on">TLS</span>`
                                    : html`<span class="chip warn">no TLS</span>`}
                        ${entry.keyPinned === true ? html`<span class="chip on">key pin</span>` : nothing}
                        ${entry.autoConnect
                              ? html`<span class="chip on">connects by itself</span>`
                              : html`<span class="chip">configured only</span>`}
                    </div>

                    <div class="connection-state" data-state="${entry.id}">${stateView(entry)}</div>

                    <dl class="kv">
                        <dt>Where</dt>   <dd><code>${entry.url}</code></dd>
                        <dt>Proves itself</dt>
                        <dd data-proves="${entry.id}">${provesSaid(entry, state)}</dd>
                        ${entry.transport === 'ssh' ? html`
                            <dt>Host keys</dt>
                            <dd>${entry.sshHostKey !== undefined
                                      ? html`<code class="wrap" data-host-key="${entry.id}">${entry.sshHostKey}</code>${(entry.sshHostKeysVersion ?? 0) > 0
                                            ? html` <span class="hint">from the signed pin set of version ${entry.sshHostKeysVersion}</span>`
                                            : nothing}`
                                      : entry.sshLearnedHostKey !== undefined
                                            ? html`<code class="wrap" data-host-key="${entry.id}">${entry.sshLearnedHostKey}</code>
                                                   <span class="hint">learned ${formatTimestamp(entry.sshLearnedAt ?? '')}, on probation</span>`
                                            : entry.sshTrustOnFirstUse === 'HostKey'
                                                  ? html`<span class="hint" data-host-key="${entry.id}">none pinned yet - the first one its server shows is learned, on probation</span>`
                                                  : html`<span class="hint" data-host-key="${entry.id}">none pinned - not dialled</span>`}</dd>
                        ` : nothing}
                        ${entry.secure ? html`
                            <dt>Its server</dt>
                            <dd data-key-pins="${entry.id}">${keyPinsView(entry)}</dd>
                        ` : nothing}
                        ${entry.secure && entry.keyPinned !== true && saysAnything(pinsIn(entry.pins ?? {})) ? html`
                            <dt>Held to</dt>
                            <dd data-pins="${entry.id}">${heldToView(pinsIn(entry.pins ?? {}), offers?.nameOf)}${(entry.pinsVersion ?? 0) > 0
                                ? html` <span class="hint">its roots from the signed pin set of version ${entry.pinsVersion}</span>`
                                : nothing}</dd>
                        ` : nothing}
                        ${entry.judgement ? html`
                            <dt>Its certificate</dt>
                            <dd data-judgement="${entry.id}">${judgementView(entry.judgement)}</dd>
                        ` : nothing}
                        <dt>Written</dt> <dd>${formatTimestamp(entry.createdAt)}</dd>
                    </dl>

                    ${(entry.warnings ?? []).map(warning => html`<div class="notice">${warning}</div>`)}

                    <details ?open=${opened.has(entry.id)} data-details="${entry.id}"
                             @toggle=${(event: Event) => toggled(entry.id, (event.target as HTMLDetailsElement).open)}>

                        <summary>Change it</summary>

                        <form class="form-stack" data-id="${entry.id}" data-edit="${entry.id}"
                              @submit=${(event: SubmitEvent) => { event.preventDefault(); void save(entry.id, event.currentTarget as HTMLFormElement); }}>

                            ${theFields(entry.id, entry, state)}

                            <div class="form-actions">
                                <button type="submit" class="btn primary" ?disabled=${!mayManage}>
                                    Save
                                </button>
                                <button type="button" class="btn small" data-remove="${entry.id}"
                                        ?disabled=${!mayManage} @click=${() => void remove(entry.id)}>
                                    Remove
                                </button>
                                <button type="button" class="btn small" data-test="${entry.id}"
                                        ?disabled=${!mayTest} @click=${testThisForm}>
                                    <i class="fa-solid fa-plug-circle-check"></i> Test it
                                </button>
                                ${whatATestAsks(mayTest)}
                                <span class="form-notice" data-note="${entry.id}"  role="status"></span>
                                <span class="form-error"  data-error="${entry.id}" role="alert"></span>
                            </div>

                        </form>

                    </details>

                </div>
            `;

        }

        /**
         * How the server of a TLS connection is known: by the key pins of its
         * key - written down, from a signed pin set, or learned on the first
         * connection and on probation - or by its CA. A learned key pin is shown
         * here and not in the form: it is no pin until it is confirmed.
         */
        function keyPinsView(entry: StationConnection): TemplateResult {

            const pins     = entry.serverKeyPins ?? [];
            const version  = entry.serverKeyPinsVersion ?? 0;

            if (pins.length > 0)
                return html`${pins.map(pin => html`<code class="wrap">${pin}</code> `)}<span class="hint">${
                    entry.serverKeyPinState === 'Confirmed'
                        ? `learned on its first connection, and confirmed by the signed pin set of version ${version}`
                        : version > 0
                              ? `by key pin, from the signed pin set of version ${version}`
                              : 'by key pin - no CA, no name, no validity period'}</span>`;

            if (entry.serverKeyPinLearned !== undefined)
                return html`<code class="wrap">${entry.serverKeyPinLearned}</code>
                            <span class="hint">learned ${formatTimestamp(entry.serverKeyPinLearnedAt ?? '')}, on probation</span>`;

            if (entry.trustOnFirstUse === 'Key')
                return html`<span class="hint">no key pinned yet - the key pin of the first key its server shows is learned, on probation</span>`;

            return html`<span class="hint">by its certificate, which a CA this ${nodeNoun()} trusts must have issued for its name</span>`;

        }

        /**
         * The key this station signs in with over SSH: its public half, for the
         * authorized_keys of the servers it dials. Only where there is one, or
         * an ssh:// connection that will make it.
         */
        function sshKeyView(state: StationConnections): TemplateResult | typeof nothing {

            const key = state.sshClientKey ?? null;

            if (key === null && !state.connections.some(one => one.transport === 'ssh'))
                return nothing;

            return html`
                <section class="card" id="ssh-key">

                    <h2><i class="fa-solid fa-key"></i> This ${nodeNoun()}'s SSH key</h2>

                    ${key === null ? html`
                        <p class="hint">
                            None yet. It is made when an <code>ssh://</code> connection is written down, tested or
                            dialled.
                        </p>
                    ` : html`
                        <p class="hint">
                            What this ${nodeNoun()} signs in with over <code>ssh://</code>. This line goes into the
                            <code>authorized_keys</code> of every server it dials that way. It is not a secret: the
                            private half stays in <code>${key.path}</code>.
                        </p>
                        <textarea id="ssh-public-key" class="mono" rows="3" readonly .defaultValue=${key.publicKey}></textarea>
                        <div class="form-actions">
                            <button type="button" id="copy-ssh-key" class="btn small" @click=${copySshKey}>Copy</button>
                            <span class="hint">${key.fingerprint}</span>
                            <span id="copy-ssh-key-note" class="form-notice" role="status"></span>
                        </div>
                    `}

                </section>
            `;

        }

        /**
         * The keys a pin set for a connection over SSH is believed of: a key
         * learned on first use is confirmed, and the pins of a connection are
         * changed over OCPP, only by a SetVariables one of these signed.
         */
        function signersView(state: StationConnections): TemplateResult | typeof nothing {

            const signers = state.pinSetSigners ?? [];

            if (signers.length === 0 && !state.connections.some(one => one.transport === 'ssh'))
                return nothing;

            return html`
                <section class="card" id="pin-set-signers">

                    <h2><i class="fa-solid fa-signature"></i> Signers of pin sets</h2>

                    <p class="hint">
                        A host key learned on first use is confirmed, and the host keys of a connection over SSH are changed over
                        OCPP, only by a pin set one of these keys signed - the CSMS's, from commissioning.
                    </p>

                    ${signers.length === 0
                         ? html`<p class="hint" id="no-signers">None yet: no pin set is believed, and a key learned on first use is never confirmed.</p>`
                         : html`
                            <div class="table-scroll">
                                <table class="data">
                                    <thead><tr><th>Whose</th><th>Public key</th><th>Written</th><th></th></tr></thead>
                                    <tbody>
                                        ${signers.map(signer => html`
                                            <tr data-signer="${signer.publicKey}">
                                                <td>${signer.description}</td>
                                                <td><code class="wrap">${signer.publicKey}</code></td>
                                                <td>${formatTimestamp(signer.createdAt)}</td>
                                                <td>${mayManage ? html`<button type="button" class="btn small danger" @click=${() => removeSigner(signer.publicKey)}>Remove</button>` : nothing}</td>
                                            </tr>
                                        `)}
                                    </tbody>
                                </table>
                            </div>
                         `}

                    ${mayManage ? html`
                        <form id="signer-form" @submit=${addSigner}>
                            <div class="form-row">
                                <label>Whose
                                    <input type="text" name="signerDescription" placeholder="The CSMS" />
                                </label>
                                <label>Public key
                                    <input type="text" name="signerPublicKey" class="mono" placeholder="base64 or hexadecimal, as an OCPP signature's keyId" />
                                </label>
                            </div>
                            <div class="form-actions">
                                <button type="submit" class="btn primary">Believe it</button>
                                <span id="signer-note"  class="form-notice" role="status"></span>
                                <span id="signer-error" class="form-error"  role="alert"></span>
                            </div>
                        </form>
                    ` : nothing}

                </section>
            `;

        }

        async function addSigner(event: SubmitEvent): Promise<void> {

            event.preventDefault();

            const form   = event.currentTarget as HTMLFormElement;
            const error  = must<HTMLElement>(content, '#signer-error');
            const note   = must<HTMLElement>(content, '#signer-note');

            error.textContent = '';

            try
            {
                // Read before the form is held still: a field held still is no longer in its form data.
                const publicKey    = field(form, 'signerPublicKey');
                const description  = field(form, 'signerDescription');

                took(await whileSaving(content, note, () => api.connections.addPinSetSigner(publicKey, description)));
                if (!cancelled)
                    draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }

        async function removeSigner(publicKey: string): Promise<void> {

            if (!confirm('No pin set signed by this key is believed any more. Go on?'))
                return;

            try
            {
                took(await api.connections.removePinSetSigner(publicKey));
                if (!cancelled)
                    draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    must<HTMLElement>(content, '#signer-error').textContent = errorMessage(problem);
            }

        }

        function copySshKey(): void {
            const box = content.querySelector<HTMLTextAreaElement>('#ssh-public-key');
            if (box === null)
                return;
            box.select();
            void navigator.clipboard?.writeText(box.value).then(
                () => { must<HTMLElement>(content, '#copy-ssh-key-note').textContent = 'Copied.'; },
                () => { must<HTMLElement>(content, '#copy-ssh-key-note').textContent = 'Select it and copy it by hand.'; }
            );
        }

        /** Where one connection stands, as a chip, since when, and what the station said. */
        function stateView(entry: StationConnection): TemplateResult | typeof nothing {

            const line = stateLine(entry, states[entry.id], timestamp);

            return line === null ? nothing : lineView(line);

        }

        function lineView(line: StateLine): TemplateResult {

            return html`
                <div class="state-head">
                    <span class="chip ${line.tone}">${line.chip}</span>
                    <span class="hint">${line.when}</span>
                </div>
                <div class="hint">${line.said}</div>
                ${line.notes.map(note => html`<div class="notice small">${note}</div>`)}
            `;

        }

        /**
         * The connections removed here that the station still dials: it hangs
         * them up as soon as it has taken the change, and until then it may
         * well be on a back end this page no longer mentions.
         */
        function noLongerWrittenDownView(): TemplateResult | typeof nothing {

            const left = noLongerWrittenDown(store?.connections ?? [], states);

            if (left.length === 0)
                return nothing;

            return html`
                <div class="notice">
                    Removed here, and dialled as before until this ${nodeNoun()} has hung them up:
                    ${left.map(([ , state ]) => {
                        const line = stateLine({ url: state.url, ocppVersion: state.ocppVersion, autoConnect: true }, state, timestamp);
                        return html`
                            <div class="removed-state">
                                <strong>${state.description}</strong> <code>${state.url}</code>
                                ${line === null ? nothing : lineView(line)}
                            </div>
                        `;
                    })}
                </div>
            `;

        }

        let asking = false;

        /**
         * Ask where the connections stand, while the page is in view.
         *
         * Not while it is hidden: nobody is looking, and a tab left open in the
         * background has no business asking the station something every few
         * seconds for as long as the browser runs. Not twice at once either -
         * a station slow to answer is not helped by being asked again.
         */
        async function askAgain(): Promise<void> {

            if (asking || store === null || document.hidden)
                return;

            asking = true;

            try
            {
                const answer = await api.connections.states();

                if (cancelled)
                    return;

                states       = answer.states;
                timestamp    = answer.timestamp;
                notAnswered  = null;
            }
            catch (problem)
            {
                notAnswered  = errorMessage(problem);
            }
            finally
            {
                asking = false;
            }

            // The whole page, which leaves what is typed into its forms where
            // it is: only what differs is drawn.
            if (!cancelled)
                draw();

        }

        /** A details element that was open stays open across a redraw. */
        function toggled(id: string, open: boolean): void {
            if (open)
                opened.add(id);
            else
                opened.delete(id);
        }

        /**
         * What the test beside it asks the back end in OCPP once it is
         * connected.
         *
         * Beside the button rather than among the fields, and not one of
         * them: it says how to test, not what the connection is, and is
         * neither saved nor read with the rest of the form.
         */
        function whatATestAsks(MayTest: boolean): TemplateResult {
            return html`
                <select name="testAsks" class="test-asks" ?disabled=${!MayTest}
                        aria-label="What the test asks the back end in OCPP">
                    <option value="heartbeat" selected>and ask for a Heartbeat</option>
                    <option value="none">and ask nothing</option>
                </select>
            `;
        }

        /** Test what the form the button is in says. */
        function testThisForm(event: Event): void {
            void test((event.currentTarget as HTMLElement).closest('form'));
        }

        /**
         * What a form says, in the shape the station takes.
         *
         * The one control that picks how a connection proves itself becomes the
         * two fields the station stores, here and nowhere else - which is what
         * keeps "one of them" true by construction rather than by a check
         * somebody has to remember.
         */
        function readFrom(form: HTMLFormElement, id?: string): ConnectionToSave {

            const entry    = id !== undefined ? store?.connections.find(one => one.id === id) ?? null : null;
            const overSSH  = schemeIn(id ?? 'add', entry) === 'ssh';
            const proves   = overSSH ? '' : field(form, 'proves');

            // The certificates or the keys ticked, each from the time beside
            // it: read where the form shows them, and nowhere else.
            const identities: ConnectionIdentity[] = overSSH || proves === 'tls'
                ? [ ...form.querySelectorAll<HTMLInputElement>('input[name="identity"]:checked') ].map(box => {
                      const typed = form.querySelector<HTMLInputElement>(`input[name="from:${box.value}"]`)?.value ?? '';
                      const from  = typed === '' ? null : new Date(typed);
                      return from !== null && !Number.isNaN(from.getTime())
                                 ? { id: box.value, from: from.toISOString() }
                                 : { id: box.value };
                  })
                : [];

            // A password over SSH is one that was there before: kept while
            // its box stays ticked, and never added here.
            const password = overSSH && (form.querySelector<HTMLInputElement>('[name="keepPassword"]')?.checked ?? false)
                                 ? entry?.authenticationId ?? null
                                 : null;

            // As drawn, beside as typed: left as it was, the station keeps the
            // one it has, which the first connection may have written down
            // since this page last asked.
            const hostKey  = form.querySelector<HTMLInputElement>('[name="sshHostKey"]');

            const probation = Number(form.querySelector<HTMLInputElement>('[name="sshProbationTime"]')?.value ?? '600');

            // The same for the key pins of a wss:// connection, which a signed
            // pin set may have replaced since the page last asked.
            const keyPins        = form.querySelector<HTMLInputElement>('[name="serverKeyPins"]');
            const keyProbation   = Number(form.querySelector<HTMLInputElement>('[name="serverKeyPinProbationTime"]')?.value ?? '600');

            // What its server is held to. A line that is no pin is sent as it
            // was typed: the station refuses it in the sentence that names it,
            // which is the one a time server's dialog says.
            const read           = readPinsFieldset(form);

            const pins           = read.draft !== undefined
                                       ? keysOf(read.draft)
                                       : { certificateFingerprints:  linesOf(form, 'pinCertificates'),
                                           rootFingerprints:         linesOf(form, 'pinRoots') };

            // And what the page showed it held to, so that what the first
            // connection learned since, or a signed pin set brought, stays.
            const shown          = id !== undefined
                                       ? store?.connections.find(one => one.id === id)?.pins ?? {}
                                       : undefined;

            return {
                pins,
                ...(shown !== undefined ? { pinsAsShown: keysOf(pinsIn(shown)) } : {}),
                sshHostKey:          hostKey?.value.trim() ?? '',
                ...(id !== undefined && hostKey !== null ? { sshHostKeyAsShown: hostKey.defaultValue.trim() } : {}),
                sshTrustOnFirstUse:  (form.querySelector<HTMLSelectElement>('[name="sshTrustOnFirstUse"]')?.value ?? 'None') as 'None' | 'HostKey',
                sshOnPinMismatch:    (form.querySelector<HTMLSelectElement>('[name="sshOnPinMismatch"]')?.value ?? 'Refuse') as 'Refuse' | 'Record' | 'Accept',
                sshProbationTime:    Number.isFinite(probation) ? probation : 600,
                serverKeyPins:       pinsOf(keyPins?.value ?? ''),
                ...(id !== undefined && keyPins !== null ? { serverKeyPinsAsShown: pinsOf(keyPins.defaultValue) } : {}),
                trustOnFirstUse:     (form.querySelector<HTMLSelectElement>('[name="trustOnFirstUse"]')?.value ?? 'None') as 'None' | 'Key',
                rawPublicKeys:       (form.querySelector<HTMLSelectElement>('[name="rawPublicKeys"]')?.value ?? 'Off') as 'Off' | 'Prefer' | 'Require',
                serverKeyPinProbationTime: Number.isFinite(keyProbation) ? keyProbation : 600,
                id,
                description:         field(form, 'description'),
                url:                 field(form, 'url'),
                connectionType:      field(form, 'connectionType'),
                ocppVersion:         field(form, 'ocppVersion'),
                httpVersion:         (form.querySelector<HTMLSelectElement>('[name="httpVersion"]')?.value ?? 'auto') as HTTPVersionOfAConnection,
                encoding:            (form.querySelector<HTMLSelectElement>('[name="encoding"]')?.value ?? 'json') as EncodingOfAConnection,
                signatureFormats:    (form.querySelector<HTMLSelectElement>('[name="signatureFormats"]')?.value ?? 'policy') as SignatureFormatsOfAConnection,
                autoConnect:  form.querySelector<HTMLInputElement>('[name="autoConnect"]')?.checked ?? false,
                authenticationId:    overSSH ? password : proves.startsWith('auth:') ? proves.slice(5) : null,
                certificateId:       null,
                identities
            };

        }


        /** The lines of one of the pin fields, as they were typed. */
        function linesOf(form: HTMLFormElement, name: string): string[] {
            return (form.querySelector<HTMLTextAreaElement>(`textarea[name="${name}"]`)?.value ?? '').
                       split(/[\r\n,;]+/).map(line => line.trim()).filter(line => line !== '');
        }

        /** The key pins of a field, one each, as they are typed: separated by commas, semicolons or spaces. */
        function pinsOf(text: string): string[] {
            return text.split(/[\s,;]+/).filter(pin => pin !== '');
        }

        /**
         * The login typed into a form of a connection, written down before
         * the connection is: what it names is there then. Written down once -
         * a connection its node then refuses is saved again with the login
         * that is there by now, and not with a second one.
         */
        async function withItsNewLogin(id: string, login: LoginToSave | null, written: ConnectionToSave): Promise<ConnectionToSave> {

            if (login === null)
                return written;

            const made = await api.authentications.add(login);

            took(made.connections);
            ways.set(id, `auth:${made.id}`);

            return { ...written, authenticationId: made.id };

        }

        /**
         * The login typed into a form of a connection, or null where it
         * names one that is there, or none. Read before anything is sent:
         * a form is held still while it is, and says nothing then.
         */
        function newLoginIn(form: HTMLFormElement, written: ConnectionToSave): LoginToSave | null {

            const way = form.querySelector<HTMLSelectElement>('[name="proves"]')?.value ?? '';

            return way.startsWith('new:')
                       ? { description:  field(form, 'newDescription') || written.description,
                           kind:         way === 'new:totp' ? 'totp' : 'basic',
                           login:        field(form, 'newLogin'),
                           secret:       field(form, 'newSecret', false) }
                       : null;

        }

        /** A form was saved, or emptied: nothing of it is typed any more. */
        function settled(id: string): void {
            typedURLs.delete(id);
            ways.     delete(id);
        }

        async function add(event: SubmitEvent): Promise<void> {

            event.preventDefault();

            const form = event.currentTarget as HTMLFormElement;
            const note = must<HTMLElement>(content, '#add-note');

            note.textContent = '';
            must<HTMLElement>(content, '#add-error').textContent = '';

            const written = readFrom(form);
            const login   = newLoginIn(form, written);

            try
            {
                const made = await whileSaving(content, note, async () => api.connections.add(await withItsNewLogin('add', login, written)));

                if (cancelled)
                    return;

                took(made.connections);
                settled('add');

                draw();

                // Written down, so no longer typed: the form is empty again.
                form.reset();

                note.textContent = 'Written down.';
            }
            catch (problem)
            {
                if (!cancelled) {
                    // A login written down before its connection was refused
                    // is the one chosen now.
                    draw();
                    must<HTMLElement>(content, '#add-error').textContent = errorMessage(problem);
                }
            }

        }

        async function save(id: string, form: HTMLFormElement): Promise<void> {

            const note  = must<HTMLElement>(content, `[data-note="${id}"]`);
            const error = must<HTMLElement>(content, `[data-error="${id}"]`);

            note. textContent = '';
            error.textContent = '';

            const written = readFrom(form, id);
            const login   = newLoginIn(form, written);

            try
            {
                took(await whileSaving(content, note, async () => api.connections.update(await withItsNewLogin(id, login, written))));

                if (cancelled)
                    return;

                opened.add(id);
                settled(id);

                draw();

                // A draw leaves a form as it is typed into; the one saved goes
                // back to what it says now - the station's answer.
                form.reset();

                note.textContent = 'Saved.';
            }
            catch (problem)
            {
                if (!cancelled) {
                    draw();
                    error.textContent = errorMessage(problem);
                }
            }

        }

        async function remove(id: string): Promise<void> {

            const error = must<HTMLElement>(content, `[data-error="${id}"]`);
            const note  = must<HTMLElement>(content, `[data-note="${id}"]`);

            error.textContent = '';

            try
            {
                took(await whileSaving(content, note, () => api.connections.remove(id)));

                if (cancelled)
                    return;

                opened.delete(id);

                draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }

        /**
         * Test one connection, in a dialog that stays put until it is closed.
         *
         * The dialog is opened before the request rather than after it comes
         * back, because the whole thing takes some seconds and a button that
         * does nothing visible for that long is a button somebody presses
         * again. What is shown while waiting says what is being tried, so the
         * wait is legible rather than merely long.
         */
        async function test(form: HTMLFormElement | null): Promise<void> {

            if (form === null)
                return;

            // What is on the screen, not what is on disk. Beside the add form
            // there is nothing on disk yet, and beside an entry being edited
            // the two are frequently different - and the one somebody is
            // looking at is the one they mean.
            const entry  = readFrom(form);

            // And asked what the choice beside the button says; a Heartbeat
            // where there is none, which is what the station asks untold.
            const asks: ConnectionTestAsks
                         = form.querySelector<HTMLSelectElement>('[name="testAsks"]')?.value === 'none'
                               ? 'none'
                               : 'heartbeat';

            const dialog = document.createElement('dialog');

            dialog.className = 'test-dialog';

            /**
             * The dialog, saying what is being tried and then what came of it:
             * drawn as a whole each time, so that what it said while waiting
             * goes when the answer comes.
             */
            const drawDialog = (Steps: TemplateResult, Waiting: boolean): void => render(dialog, html`
                <h2>Testing ${entry.description || 'this connection'}</h2>
                <p class="hint"><code>${entry.url || 'no URL given'}</code></p>
                <div class="test-steps" id="test-steps">${Steps}</div>
                <div class="form-actions">
                    <button type="button" class="btn" id="test-close" ?disabled=${Waiting} @click=${dismiss}>Close</button>
                </div>
            `);

            drawDialog(html`<div class="loading">${asks === 'none'
                                                        ? 'Connecting, and staying connected for two seconds ...'
                                                        : 'Connecting, asking for a Heartbeat, and staying connected for two seconds ...'}</div>`, true);

            document.body.appendChild(dialog);
            dialog.showModal();

            /**
             * Shut it and take it away.
             *
             * Both halves explicitly, rather than removing it when the dialog
             * reports that it closed. Measured: the browser this station's
             * pages were tried in does not fire `close` at all - not on
             * `dialog.close()` and not on Escape - so a dialog cleaned up that
             * way stays in the page, closed and invisible, one more of them
             * after every test. Calling this twice is harmless, which is what
             * lets it be wired to everything that might end the dialog.
             */
            function dismiss(): void {
                dialog.close();
                dialog.remove();
            }

            // Whichever of these the browser actually delivers.
            dialog.addEventListener('close',  dismiss);
            dialog.addEventListener('cancel', dismiss);

            let result: ConnectionTest;

            try
            {
                result = await api.connections.test(entry, asks);
            }
            catch (problem)
            {
                drawDialog(html`
                    <div class="error-box">The test could not be run: ${errorMessage(problem)}</div>
                `, false);
                must<HTMLButtonElement>(dialog, '#test-close').focus();
                return;
            }

            drawDialog(html`
                <div class="${result.ok ? 'notice' : 'error-box'}">
                    ${result.ok
                          ? html`The connection can be made. ${result.runtime_ms} ms altogether.`
                          : result.ocpp !== undefined && !result.ocpp.answered
                                ? html`The connection can be made, and what was asked on it in OCPP was not answered. ${result.runtime_ms} ms altogether.`
                                : html`The connection could not be made. ${result.runtime_ms} ms altogether.`}
                </div>
                ${result.ocpp === undefined ? nothing : html`
                    <p class="hint" id="test-ocpp">
                        ${result.ocpp.answered
                              ? result.ocpp.error === true
                                    ? html`Its back end answered the ${result.ocpp.request} with an error, within ${result.ocpp.roundTrip_ms} ms: it speaks OCPP on this connection.`
                                    : html`Its back end answered the ${result.ocpp.request} within ${result.ocpp.roundTrip_ms} ms.`
                              : html`Its back end did not answer the ${result.ocpp.request}: something took the connection, and nothing there speaks OCPP on it.`}
                    </p>
                `}
                ${result.serverKeyPin === undefined ? nothing : html`
                    <p class="hint">
                        The key pin of the key its server showed, believed or not - what it would be pinned to:
                        <code class="wrap" id="test-key-pin">${result.serverKeyPin}</code>
                    </p>
                `}
                <ol class="test-log">
                    ${result.steps.map(step => html`
                        <li class="level-${step.level}">
                            <span class="at">+${step.at_ms} ms</span>
                            <span class="text">${step.text}</span>
                        </li>
                    `)}
                </ol>
            `, false);

            must<HTMLButtonElement>(dialog, '#test-close').focus();

        }

        /**
         * The page as the station has it now, drawn over the page as it is -
         * what is typed into its forms kept, as a draw keeps it. Reload empties
         * them itself.
         */
        async function load(): Promise<void> {

            try
            {
                const [ loaded, kept ] = await Promise.all([ api.connections.get(), storeOffers('ocpp') ]);

                if (cancelled)
                    return;

                offers = kept;

                took(loaded);
                draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    render(content, html`
                        <div class="error-box">The connections could not be loaded: ${errorMessage(problem)}</div>
                    `);
            }

        }

        /**
         * Loaded anew - Reload - is what the station has, the forms too, which
         * a draw on its own would leave as typed.
         */
        async function reload(): Promise<void> {
            typedURLs.clear();
            ways.     clear();
            await load();
            if (!cancelled) {
                content.querySelectorAll('form').forEach(form => form.reset());
                await credentials.reload();
            }
        }

        const release = unsaved.heldBy(() => anyFormTypedSinceDrawn(content));

        void load();

        // Every five seconds: a connection that is lost is tried again after a
        // second or so, so this is about as late as the page can be told of it
        // without asking more often than a person could read.
        const askingAgain = setInterval(() => void askAgain(), 5_000);

        return () => { cancelled = true; clearInterval(askingAgain); release(); credentials.release(); };

    }

};
