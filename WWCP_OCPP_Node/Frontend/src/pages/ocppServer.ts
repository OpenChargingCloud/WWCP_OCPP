import { stationServerAPI as api, type ClientAuthentication, type OCPPServerConfiguration, type OCPPServerUpdate, type WebSocketLogins } from '../api/stationServer';
import { auth } from '../auth';
import { nodeNoun, stationsResource } from '../settings';
import { toURL } from '@node/basePath';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { errorMessage, field, formatTimestamp, isChecked, numberField } from '@node/ui';
import { anyFormTypedSinceDrawn, unsaved } from '@node/unsaved';
import { html, live, nothing, render, type TemplateResult } from '@node/view';

/**
 * The server the charging stations connect to, and which of them may.
 *
 * Two things on one page because they are one question: a port nobody can come
 * through is the same as no port. The certificates it presents and the chains
 * it accepts are their own pages - those are about who this controller is and
 * whom it believes, and they are a different permission.
 *
 * What is decided when the socket opens cannot be changed under a running
 * server, so the page says which of the fields it just saved are waiting for
 * the next start rather than letting somebody find out from a charging station.
 *
 * Drawn by view.ts: a draw changes only what differs, so that what is typed
 * into one card - and its focus - outlives another card being saved or the
 * server being switched on or off.
 */
export const ocppServerPage: Page = {

    title: 'Charging stations',

    render({ root }) {

        const content = shell(root, {
            active:    '/configuration/ocpp-server',
            title:     'Charging stations',
            subtitle:  `The HTTP WebSocket server the charging stations below this ${nodeNoun()} connect to.`,
            actions:   reloadButton(() => reload())
        });

        render(content, html`<div class="loading">Loading ...</div>`);

        const mayChange = auth.can(stationsResource(), 'edit');

        let cancelled = false;
        let server:   OCPPServerConfiguration | null = null;
        let stations: WebSocketLogins           | null = null;

        function draw(): void {

            if (server === null || stations === null)
                return;

            const configuration = server;
            const logins        = stations;
            const waiting       = configuration.state.waitingForARestart;

            render(content, html`

                ${mayChange ? nothing : html`
                    <div class="notice">
                        ${mayButNot('look at the charging station server', 'change it')}
                    </div>
                `}

                ${waiting.length === 0 ? nothing : html`
                    <div class="notice warn">
                        <strong>Saved, and waiting for the next start:</strong> ${waiting.join(', ')}.
                        The socket is opened once, when the server is built - so these are in the configuration
                        file but not in effect. Everything else on this page took effect at once.
                    </div>
                `}

                <div class="cards">

                    ${stateCard(configuration)}

                    <section class="card">

                        <h2><i class="fa-solid fa-power-off"></i> The server</h2>

                        <label class="switch">
                            <input type="checkbox" id="enabled"
                                   .checked=${live(configuration.enabled)}
                                   ?disabled=${!mayChange}
                                   @change=${(event: Event) => void save({ enabled: (event.target as HTMLInputElement).checked }, 'socket', null)} />
                            <span>${configuration.enabled ? 'switched on' : 'switched off'}</span>
                        </label>

                        <p class="hint">
                            Switched off, no charging station can connect at all. This is the one setting of this
                            ${nodeNoun()} that opens a port to a network rather than reaching out to one, so it
                            starts switched off and stays that way until somebody says otherwise.
                        </p>

                        <label class="switch">
                            <input type="checkbox" id="http3"
                                   .checked=${live(configuration.http3)}
                                   ?disabled=${!mayChange}
                                   @change=${(event: Event) => void save({ http3: (event.target as HTMLInputElement).checked }, 'http3', null)} />
                            <span>Take WebSockets over HTTP/3 as well</span>
                        </label>

                        <p class="hint">
                            HTTP/3 opens a UDP port of the same number beside the TCP port, and needs TLS. It is what
                            a one-time password bound to its TLS session is checked over; without it, such a connection
                            is not let in. Taken or not from the moment it is saved, without a restart.
                        </p>

                        <div class="form-actions">
                            <span id="http3-note"  class="form-notice" role="status"></span>
                            <span id="http3-error" class="form-error"  role="alert"></span>
                        </div>

                    </section>

                    ${sshCard(configuration)}

                    <section class="card">

                        <h2><i class="fa-solid fa-network-wired"></i> Socket</h2>

                        <form id="socket-form" class="form-stack" @submit=${saveSocket}>

                            <label>Listen on
                                <input type="text" name="address" value="${configuration.address ?? ''}"
                                       placeholder="0.0.0.0" ?disabled=${!mayChange} />
                            </label>

                            <label>TCP port
                                <input type="number" name="port" min="1" max="65535"
                                       value="${configuration.port}" ?disabled=${!mayChange} />
                            </label>

                            <fieldset class="checks">
                                <legend>OCPP versions offered</legend>
                                ${configuration.limits.subprotocols.map(subprotocol => html`
                                    <label class="check">
                                        <input type="checkbox" name="subprotocol" value="${subprotocol}"
                                               ?checked=${configuration.subprotocols.includes(subprotocol)}
                                               ?disabled=${!mayChange} />
                                        <span>${subprotocol}</span>
                                    </label>
                                `)}
                            </fieldset>

                            <label>Oldest TLS version accepted
                                <select name="minTLSVersion" ?disabled=${!mayChange}>
                                    ${configuration.limits.tlsVersions.map(version => html`
                                        <option value="${version}"
                                                ?selected=${version === configuration.minTLSVersion}>
                                            TLS ${version}
                                        </option>
                                    `)}
                                </select>
                            </label>

                            <label>At most this many connected at once
                                <input type="number" name="maxConnections" min="1" max="${configuration.limits.maxConnections}"
                                       value="${configuration.maxConnections}" ?disabled=${!mayChange} />
                            </label>

                            <label>Ping a silent connection every ... seconds
                                <input type="number" name="pingEverySeconds" min="5" max="3600"
                                       value="${configuration.pingEverySeconds}" ?disabled=${!mayChange} />
                            </label>

                            <span class="hint">
                                This is how a charging station that went away without closing its connection is
                                noticed, so it has to be shorter than the idle timeout of whatever is in between -
                                over mobile networks and behind NAT, frequently a minute or two.
                            </span>

                            <div class="form-actions">
                                <button type="submit" class="btn primary" ?disabled=${!mayChange}>Save</button>
                                <span id="socket-note"  class="form-notice" role="status"></span>
                                <span id="socket-error" class="form-error"  role="alert"></span>
                            </div>

                            <span class="hint">
                                Everything here is decided when the socket opens, so saving it writes the file and
                                the next start puts it into effect. Every charging station reconnects then.
                            </span>

                        </form>

                    </section>

                    ${profilesCard(configuration)}
                    ${reachableCard(configuration)}
                    ${loggingCard(configuration)}
                    ${loginsCard(logins)}

                </div>
            `);

        }


        // How a client may authenticate - OCPP's security profiles.
        function profilesCard(configuration: OCPPServerConfiguration): TemplateResult {

            const descriptions: Record<string, [ string, string ]> = {
                httpAuthWithoutTLS:    [ 'HTTP authentication without TLS', 'security profile 1: a password over an unencrypted connection. Only usable on a network nobody else is on.' ],
                httpAuthOverTLS:       [ 'HTTP authentication over TLS',    'security profile 2: a password over TLS. Needs a server certificate; without one this port does not encrypt at all.' ],
                tlsClientCertificate:  [ 'TLS client certificate',          'security profile 3: a certificate on both sides, and no password. Needs the station to hold a certificate from an authority named under Accepted chains.' ]
            };

            return html`
                <section class="card">

                    <h2><i class="fa-solid fa-shield-halved"></i> Client authentication</h2>

                    <form id="profiles-form" class="form-stack" @submit=${saveProfiles}>

                        <fieldset class="checks">
                            <legend>A charging station may authenticate by</legend>
                            ${configuration.limits.clientAuthentication.map(authentication => html`
                                <label class="check">
                                    <input type="checkbox" name="authentication" value="${authentication}"
                                           ?checked=${configuration.clientAuthentication.includes(authentication)}
                                           ?disabled=${!mayChange} />
                                    <span><strong>${descriptions[authentication]?.[0] ?? authentication}</strong> &mdash; ${descriptions[authentication]?.[1] ?? ''}</span>
                                </label>
                            `)}
                        </fieldset>

                        <label class="check">
                            <input type="checkbox" name="checkCertificateRevocation"
                                   ?checked=${configuration.checkCertificateRevocation}
                                   ?disabled=${!mayChange} />
                            <span>Check a station's certificate against its issuer's revocation list</span>
                        </label>

                        <span class="hint">
                            Off by default: a ${nodeNoun()} in a car park frequently has no route to the list, and a
                            check that cannot be made turns every station away on the day the network changes.
                        </span>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayChange}>Save</button>
                            <span id="profiles-note"  class="form-notice" role="status"></span>
                            <span id="profiles-error" class="form-error"  role="alert"></span>
                        </div>

                        ${configuration.clientAuthentication.some(authentication => authentication !== 'httpAuthWithoutTLS') && !configuration.state.hasCertificate
                              ? html`
                                  <div class="notice warn">
                                      HTTP authentication over TLS or a TLS client certificate is allowed, but there is no
                                      server certificate - so this port is <strong>not encrypted</strong> and only HTTP
                                      authentication without TLS can be used on it.
                                      <a href="${toURL('/configuration/server-identities')}">Make a TLS server identity for the listener
                                      "ocpp", or a server key of it for the stations to pin</a>.
                                  </div>
                              `
                              : nothing}

                        ${configuration.clientAuthentication.includes('tlsClientCertificate') && configuration.state.trustedChains === 0
                              ? html`
                                  <div class="notice warn">
                                      A TLS client certificate is allowed, but no certificate authority has been named to accept
                                      charging stations from, so none can connect with a certificate - unless it is
                                      let in by a key pin listed for it. An empty list is "nobody", not "everybody" &mdash;
                                      <a href="${toURL('/configuration/ocpp-server/trust')}">name one</a>.
                                  </div>
                              `
                              : nothing}

                    </form>

                </section>
            `;

        }


        // The names a certificate has to carry.
        function reachableCard(configuration: OCPPServerConfiguration): TemplateResult {

            return html`
                <section class="card">

                    <h2><i class="fa-solid fa-location-dot"></i> Reachable as</h2>

                    <form id="reachable-form" class="form-stack" @submit=${saveReachable}>

                        <label>One name or address per line
                            <textarea name="reachableAs" rows="4"
                                      placeholder="lc001.example.org&#10;192.168.1.10"
                                      .defaultValue=${configuration.reachableAs.join('\n')}
                                      ?disabled=${!mayChange}></textarea>
                        </label>

                        <span class="hint">
                            What the charging stations dial. A certificate that does not carry these names is a
                            certificate they refuse, whatever else is right about it - so this list is what goes
                            into a signing request, and what an uploaded certificate is held against.
                        </span>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayChange}>Save</button>
                            <span id="reachable-note"  class="form-notice" role="status"></span>
                            <span id="reachable-error" class="form-error"  role="alert"></span>
                        </div>

                    </form>

                </section>
            `;

        }


        // What of this server ends up in the log.
        function loggingCard(configuration: OCPPServerConfiguration): TemplateResult {

            const logging = configuration.logging;
            const window  = Math.round(configuration.limits.suggestedPayloadWindowSeconds / 60);

            return html`
                <section class="card">

                    <h2><i class="fa-solid fa-list-check"></i> What goes into the log</h2>

                    <form id="logging-form" class="form-stack" @submit=${saveLogging}>

                        <label class="check">
                            <input type="checkbox" name="connections" ?checked=${logging.connections}
                                   ?disabled=${!mayChange} />
                            <span>Charging stations connecting and disconnecting</span>
                        </label>

                        <label class="check">
                            <input type="checkbox" name="authentication" ?checked=${logging.authentication}
                                   ?disabled=${!mayChange} />
                            <span>Refused sign-ins and failed TLS handshakes</span>
                        </label>

                        <span class="hint">
                            The switch to leave alone: it is the only record of somebody trying.
                        </span>

                        <label class="check">
                            <input type="checkbox" name="messages" ?checked=${logging.messages}
                                   ?disabled=${!mayChange} />
                            <span>Every OCPP message, by name and identification</span>
                        </label>

                        <label class="check">
                            <input type="checkbox" name="pings" ?checked=${logging.pings}
                                   ?disabled=${!mayChange} />
                            <span>The WebSocket pings that keep a connection open</span>
                        </label>

                        <hr />

                        <label class="check">
                            <input type="checkbox" name="payloads" ?checked=${logging.payloads}
                                   ?disabled=${!mayChange} />
                            <span>...and what those messages carry, for the next ${window} minutes</span>
                        </label>

                        <div class="notice">
                            An OCPP message carries identification tokens, RFID card numbers and meter readings:
                            who charged, where and how much. This is a decision about personal data and not a
                            verbosity setting, so it cannot be switched on without saying when it goes off again -
                            and it goes off by itself at that moment, whether or not anybody remembered.
                            ${logging.payloadsUntil !== null
                                  ? html`<br /><strong>${logging.payloadsNow ? 'On until' : 'The window ended at'}
                                         ${formatTimestamp(logging.payloadsUntil)}.</strong>`
                                  : nothing}
                        </div>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayChange}>Save</button>
                            <span id="logging-note"  class="form-notice" role="status"></span>
                            <span id="logging-error" class="form-error"  role="alert"></span>
                        </div>

                    </form>

                </section>
            `;

        }


        // OCPP over SSH: on this controller's SSH server, beside the socket
        // below, and switched on and off without a restart.
        function sshCard(configuration: OCPPServerConfiguration): TemplateResult | typeof nothing {

            const ssh = configuration.ssh;

            if (ssh === undefined)
                return nothing;

            return html`
                <section class="card" id="ssh-card">

                    <h2><i class="fa-solid fa-terminal"></i> OCPP over SSH</h2>

                    <label class="switch">
                        <input type="checkbox" id="ssh-enabled"
                               .checked=${live(ssh.enabled)}
                               ?disabled=${!mayChange}
                               @change=${(event: Event) => void save({ ssh: { enabled: (event.target as HTMLInputElement).checked } }, 'ssh', null)} />
                        <span>${ssh.enabled ? 'switched on' : 'switched off'}</span>
                    </label>

                    ${ssh.enabled && !ssh.state.sshServer ? html`
                        <p class="warn" id="ssh-no-server">
                            The SSH server of this ${nodeNoun()} is off, so no charging station can reach it over
                            SSH. Switch SSH on under Configuration.
                        </p>
                    ` : nothing}

                    <dl class="kv">
                        <dt>Served</dt>
                        <dd class="${ssh.state.served ? 'ok' : 'muted'}">${ssh.state.served ? 'yes' : 'no'}</dd>

                        ${ssh.state.url === null ? nothing : html`
                            <dt>Stations connect to</dt>
                            <dd><code>${ssh.state.url}</code>, subsystem <code>${ssh.subsystem}</code></dd>
                        `}

                        ${ssh.state.hostKey === null ? nothing : html`
                            <dt>Host key</dt>
                            <dd><code id="ssh-host-key">${ssh.state.hostKey}</code></dd>
                        `}

                        <dt>Connected over SSH</dt>
                        <dd>${ssh.state.connections}</dd>

                        <dt>Stations with an SSH key</dt>
                        <dd>${ssh.state.stationsWithAKey}</dd>
                    </dl>

                    <p class="hint">
                        A charging station signs in to the SSH server under its identification with an SSH key it was
                        given under Logins and groups, and gets OCPP and nothing else. It pins the host key above,
                        as it would a certificate. Switching this off ends every OCPP session over SSH.
                    </p>

                    <span id="ssh-error" class="form-error" role="alert"></span>

                </section>
            `;

        }


        // Who may sign in now lives on its own page: it grew groups, one-time
        // tokens and two kinds of credential per login, none of which belongs
        // beside the socket settings.
        function loginsCard(logins: WebSocketLogins): TemplateResult {

            return html`
                <section class="card">

                    <h2><i class="fa-solid fa-users-gear"></i> Who may sign in</h2>

                    <p>
                        <strong>${logins.enabled}</strong> of ${logins.logins.length} charging station(s) could
                        sign in right now, in ${logins.groups.length} group(s).
                    </p>

                    <p class="hint">
                        Which stations those are, what each of them signs in with, and what their group allows
                        them is managed on its own page.
                    </p>

                    <div class="form-actions">
                        <a class="btn" href="${toURL('/configuration/ocpp-server/logins')}">Logins and groups</a>
                    </div>

                </section>
            `;

        }


        function stateCard(configuration: OCPPServerConfiguration): TemplateResult {

            return html`
                <section class="card">

                    <h2><i class="fa-solid fa-circle-info"></i> Right now</h2>

                    ${configuration.state.http3Problem ? html`
                        <div class="notice warn" id="http3-problem">
                            No WebSockets over HTTP/3: ${configuration.state.http3Problem}
                        </div>
                    ` : nothing}

                    <dl class="kv">
                        <dt>Listening</dt>
                        <dd class="${configuration.state.running ? 'ok' : 'muted'}">
                            ${configuration.state.running ? 'yes' : 'no'}
                        </dd>

                        <dt>Encrypted</dt>
                        <dd class="${configuration.state.tls ? 'ok' : 'warn'}">
                            ${configuration.state.tls ? 'TLS' : 'no - security profile 1 only'}
                        </dd>

                        <dt>Stations connect to</dt>
                        <dd><code>${configuration.state.url}</code></dd>

                        <dt>HTTP/3</dt>
                        <dd id="state-http3">${!configuration.http3
                                ? 'not taken'
                                : typeof configuration.state.http3Port === 'number'
                                      ? `on UDP port ${configuration.state.http3Port}`
                                      : 'asked for, and not taken now'}</dd>

                        ${configuration.state.keyPin ? html`
                            <dt>Key pin</dt>
                            <dd><code class="keypin" id="state-keypin">${configuration.state.keyPin}</code></dd>
                        ` : nothing}

                        ${configuration.state.keyPin && configuration.state.nextKeyPin ? html`
                            <dt>Next key pin</dt>
                            <dd><code class="keypin" id="state-next-keypin">${configuration.state.nextKeyPin}</code></dd>
                        ` : nothing}

                        <dt>Connected</dt>
                        <dd>${configuration.state.connections}</dd>

                        <dt>Stations that may sign in</dt>
                        <dd>${configuration.state.webSocketLogins}</dd>

                        <dt>Accepted chains</dt>
                        <dd>${configuration.state.trustedChains}</dd>
                    </dl>

                </section>
            `;

        }


        function saveSocket(event: SubmitEvent): void {

            event.preventDefault();

            const form = event.currentTarget as HTMLFormElement;

            void save({
                address:           field(form, 'address') || undefined,
                port:              numberField(form, 'port'),
                subprotocols:      checked(form, 'subprotocol'),
                minTLSVersion:     field(form, 'minTLSVersion'),
                maxConnections:    numberField(form, 'maxConnections'),
                pingEverySeconds:  numberField(form, 'pingEverySeconds')
            }, 'socket', form);

        }


        function saveProfiles(event: SubmitEvent): void {

            event.preventDefault();

            const form = event.currentTarget as HTMLFormElement;

            void save({
                clientAuthentication:        checked(form, 'authentication') as ClientAuthentication[],
                checkCertificateRevocation:  isChecked(form, 'checkCertificateRevocation')
            }, 'profiles', form);

        }


        function saveReachable(event: SubmitEvent): void {

            event.preventDefault();

            const form  = event.currentTarget as HTMLFormElement;
            const lines = field(form, 'reachableAs', false).
                              split('\n').
                              map(line => line.trim()).
                              filter(line => line.length > 0);

            void save({ reachableAs: lines }, 'reachable', form);

        }


        function saveLogging(event: SubmitEvent): void {

            event.preventDefault();

            const form     = event.currentTarget as HTMLFormElement;
            const payloads = isChecked(form, 'payloads');

            // The window is computed here rather than typed: what somebody
            // is agreeing to is "for the next hour", and a date field would
            // invite "until 2099" without anybody meaning it.
            const until = payloads
                              ? new Date(Date.now() + (server?.limits.suggestedPayloadWindowSeconds ?? 3600) * 1000).toISOString()
                              : null;

            void save({
                logging: {
                    connections:     isChecked(form, 'connections'),
                    authentication:  isChecked(form, 'authentication'),
                    messages:        isChecked(form, 'messages'),
                    pings:           isChecked(form, 'pings'),
                    payloads,
                    payloadsUntil:   until
                }
            }, 'logging', form);

        }


        /**
         * Tell the controller what one card says, and show what it took.
         *
         * @param form  the form saved, which then says what the controller
         *              took - or null for the switch, which is in none.
         */
        async function save(update: OCPPServerUpdate, where: string, form: HTMLFormElement | null): Promise<void> {

            const note  = content.querySelector<HTMLElement>(`#${where}-note`);
            const error = content.querySelector<HTMLElement>(`#${where}-error`);

            if (note)   note.textContent  = 'Saving ...';
            if (error)  error.textContent = '';

            try
            {

                server = await api.ocppServer.save(update);

                if (cancelled)
                    return;

                if (note)
                    note.textContent = '';

                draw();

                // A draw leaves a form as it is typed into; the one saved goes
                // back to what it says now - the controller's answer.
                form?.reset();

            }
            catch (problem)
            {

                if (cancelled)
                    return;

                if (note)   note.textContent  = '';
                if (error)  error.textContent = errorMessage(problem);

                // What the controller refused is not what it is running, so the
                // page goes back to saying what is true - a switch flipped back
                // with it, what is typed into the forms kept, to be put right,
                // and why it was refused as well.
                await load();

            }

        }


        /**
         * The page as the controller has it now, drawn over the page as it
         * is - what is typed into its forms kept, as a draw keeps it. Reload
         * empties them itself.
         */
        async function load(): Promise<void> {

            try
            {

                const [configuration, logins] = await Promise.all([
                    api.ocppServer.get(),
                    api.ocppServer.logins.get()
                ]);

                if (cancelled)
                    return;

                server   = configuration;
                stations = logins;

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                    render(content, html`<div class="error-box">${errorMessage(problem)}</div>`);
            }

        }


        /**
         * Loaded anew - Reload - is what the controller has, the forms too,
         * which a draw on its own would leave as typed.
         */
        async function reload(): Promise<void> {

            await load();

            if (!cancelled)
                content.querySelectorAll('form').forEach(form => form.reset());

        }


        // Every card with a Save button is a draft until it is saved. The switch
        // at the top is in no form: it takes effect the moment it is flipped.
        const release = unsaved.heldBy(() => anyFormTypedSinceDrawn(content));

        void load();

        return () => { cancelled = true; release(); };

    }

};


/** The values of every checked box of one name. */
function checked(form: HTMLFormElement, name: string): string[] {
    return new FormData(form).getAll(name).map(String);
}
