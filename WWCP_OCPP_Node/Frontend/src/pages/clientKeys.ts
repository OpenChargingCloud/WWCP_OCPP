import { connectionsAPI as api, type KeyAsks, type StationCertificates, type StationKey } from '../api/connections';
import { auth } from '../auth';
import { connectionsResource, nodeNoun, nodeNounInFull, nodeTraits } from '../settings';
import { must } from '@node/html';
import type { CertificateAnswers, CertificatesSection, CertificatesTab, SectionContext } from '@node/pages/certificates';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { errorMessage, field, formatTimestamp, whileSaving } from '@node/ui';
import { typedSinceDrawn, unsaved } from '@node/unsaved';
import { html, nothing, render, repeat, type TemplateResult } from '@node/view';

/**
 * The keys this station makes for itself, and the certificates that come back
 * for them: what it dials a back end with.
 *
 * A key made here is who the station is before anybody has said so: it has a
 * private half that never leaves, a signing request to be handed to whoever
 * issues certificates, and no certificate yet. So the keys stand with the
 * identities - as a tab of that page, where one is made and all of them are
 * listed, and as a section among what the station presents, where the ones
 * still waiting are shown with the two things there are to do with them: copy
 * the request, or send it to the CSMS.
 *
 * Everything is built around the one thing that is hard about certificates:
 * replacing one before it runs out, without a window in which the station
 * cannot connect. So more than one lives here at a time, the replacement is
 * brought in the day it arrives even if it only becomes valid in two days, and
 * the station switches over by itself at the moment it may.
 *
 * There is no way to upload a private key, and saying so on the page is
 * deliberate: the key is made here and never leaves, and somebody looking for
 * the button should find the reason instead.
 *
 * One panel draws all of it, for whoever hosts it: the identities page of a
 * node that has one - clientKeysTab(), waitingKeysSection() and
 * clientKeyAnswers() - or a page of its own, clientKeysPage, for a node that
 * has none.
 */


/** What the tab is called in the address of the identities page: "?tab=client-keys". */
export const clientKeysTabId = 'client-keys';


/** What a panel of the keys is drawn into, and by. */
export interface ClientKeysHost {
    /** The element the page draws into: where the panel finds its own fields, and what is held still while something is saved. */
    page:       HTMLElement;
    /** Whether whoever is signed in may make, send and remove keys. */
    mayManage:  boolean;
    /** Draw the page the panel is a part of again. */
    draw():     void;
    /** Whether the page is still open: nothing is said to a page that was left. */
    alive():    boolean;
    /**
     * Whether a certificate is brought in on the panel itself: on a page of
     * its own. On the identities page the upload takes it, with every other
     * certificate.
     */
    bringsIn:   boolean;
}

/** The keys, loaded once and drawn in as many places as a page has for them. */
export interface ClientKeysPanel {
    /** Ask the node for its keys. Asked twice at once, it is asked once. */
    load():     Promise<void>;
    /** Forget what was chosen and made on the page, as it is loaded anew. */
    reset():    void;
    /** Making a key, and every key there is. */
    view():     TemplateResult;
    /** The keys that wait for their certificate, with what there is to do with them - or nothing, where none waits. */
    waiting(makeOne?: () => void):  TemplateResult | typeof nothing;
    /** Bring in a certificate that answers a request made here, and say what there is to say about it. */
    take(pem: string):  Promise<string>;
}


/** The keys of a node, for a page to draw - see ClientKeysHost. */
export function clientKeysPanel(Host: ClientKeysHost): ClientKeysPanel {

    const content   = Host.page;
    const mayManage = Host.mayManage;

    let store: StationCertificates | null = null;
    let loading: Promise<void> | null     = null;

    /** The request that was just made, put in front of somebody straight away. */
    let justMade: { id: string; csr: string } | null = null;

    /**
     * The kind of key and the template chosen in the form, while they are not
     * the ones the station suggests: what the remarks beside the lists, the
     * role and the usages on offer are about.
     */
    let picked:         string | null = null;
    let pickedTemplate: string | null = null;


    /** Say something in a place of the page - if the place is still there. */
    function say(selector: string, text: string): void {
        const place = content.querySelector<HTMLElement>(selector);
        if (place !== null)
            place.textContent = text;
    }

    /** Say something beside a key, wherever the page shows it. */
    function sayOf(id: string, what: 'note' | 'error', text: string): void {
        content.querySelectorAll<HTMLElement>(`[data-key="${id}"] [data-key-${what}]`).
                forEach(place => { place.textContent = text; });
    }


    function view(): TemplateResult {

        if (store === null)
            return html`<div class="loading">Loading ...</div>`;

        const certificates  = store;
        const algorithmId   = picked ?? certificates.defaultAlgorithm;
        const chosen        = certificates.algorithms.find(one => one.id === algorithmId);
        const templates     = certificates.templates ?? [];
        const templateId    = pickedTemplate ?? certificates.defaultTemplate ?? templates[0]?.id;
        const template      = templates.find(one => one.id === templateId);
        const keyUsages     = certificates.keyUsages         ?? [];
        const purposes      = certificates.extendedKeyUsages ?? [];

        return html`

            ${mayManage ? nothing : html`
                <div class="notice">${mayButNot('look at the keys', 'make or replace them')}</div>
            `}

            <div class="cards">

                <section class="card">

                    <h2><i class="fa-solid fa-key"></i> Make a signing request</h2>

                    <form id="create-form" class="form-stack" @submit=${create}>

                        <label>Subject
                            <input type="text" name="subject" maxlength="${certificates.maxSubjectLength}"
                                   placeholder="cs001.example.org"
                                   ?disabled=${!mayManage} />
                            <span class="hint">
                                What a back end recognises this ${nodeNoun()} by. Plain text is read as a common
                                name; a full distinguished name is taken as written. ${nodeTraits().isDialled
                                    ? 'It dials with this key and is not dialled by it, so nothing else is asked for.'
                                    : `A ${nodeNoun()} is not something anybody dials, so nothing else is asked for.`}
                            </span>
                        </label>

                        <label>Key
                            <select name="algorithm" id="algorithm" ?disabled=${!mayManage}
                                    @change=${(event: Event) => pick((event.target as HTMLSelectElement).value)}>
                                ${certificates.algorithms.map(algorithm => html`
                                    <option value="${algorithm.id}"
                                            ?selected=${algorithm.id === certificates.defaultAlgorithm}>
                                        ${algorithm.name}
                                    </option>
                                `)}
                            </select>
                            <span class="hint" id="algorithm-remark">${chosen?.remark ?? nothing}</span>
                        </label>

                        ${templates.length === 0 ? nothing : html`
                            <label>Template
                                <select name="template" id="template" ?disabled=${!mayManage}
                                        @change=${(event: Event) => pickTemplate((event.target as HTMLSelectElement).value)}>
                                    ${templates.map(one => html`
                                        <option value="${one.id}" ?selected=${one.id === certificates.defaultTemplate}>
                                            ${one.name}
                                        </option>
                                    `)}
                                </select>
                                <span class="hint" id="template-remark">${template?.remark ?? nothing}</span>
                            </label>
                        `}

                        ${template?.role === undefined ? nothing : html`
                            <label>${template.role.label}
                                <input type="text" name="role" id="role" list="role-list" autocomplete="off"
                                       maxlength="${template.role.maxLength}"
                                       placeholder="${template.role.roles[0] ?? ''}"
                                       ?disabled=${!mayManage} />
                                <datalist id="role-list">
                                    ${template.role.roles.map(role => html`<option value="${role}"></option>`)}
                                </datalist>
                                <span class="hint">
                                    Which role the holder of the certificate may play: one of
                                    ${template.role.roles.join(', ')} - or whatever else its server knows.
                                </span>
                            </label>
                        `}

                        ${keyUsages.length + purposes.length === 0 ? nothing : html`
                            <details id="asks-for">

                                <summary>What else the request asks for</summary>

                                ${keyUsages.length === 0 ? nothing : html`
                                    <fieldset class="usages" id="key-usages">
                                        <legend>Key usage</legend>
                                        <span class="hint">
                                            A signature is always asked for - that is how a client proves itself -
                                            and enciphering a key where the key is an RSA one. Beside them:
                                        </span>
                                        ${keyUsages.map(usage => {
                                            const fits = usage.algorithms.includes(algorithmId);
                                            return html`
                                                <label class="checkbox" title="${usage.remark}">
                                                    <input type="checkbox" name="keyUsage" value="${usage.id}"
                                                           ?disabled=${!(mayManage && fits)} />
                                                    ${usage.name} <code>${usage.id}</code>
                                                    ${fits ? nothing : html`<span class="hint">not with ${chosen?.name ?? 'this key'}</span>`}
                                                </label>
                                            `;
                                        })}
                                    </fieldset>
                                `}

                                ${purposes.length === 0 ? nothing : html`
                                    <fieldset class="usages" id="extended-key-usages">
                                        <legend>Extended key usage</legend>
                                        <span class="hint">
                                            Signing in as a client (<code>clientAuth</code>) is always asked for: that is
                                            what a key here is for. Beside it:
                                        </span>
                                        ${purposes.map(purpose => html`
                                            <label class="checkbox" title="${purpose.remark}">
                                                <input type="checkbox" name="extendedKeyUsage" value="${purpose.id}"
                                                       ?disabled=${!mayManage} />
                                                ${purpose.name} <code>${purpose.id}</code>
                                            </label>
                                        `)}
                                        <label>Others, as object identifiers
                                            <input type="text" name="otherExtendedKeyUsages" autocomplete="off"
                                                   placeholder="1.3.6.1.5.5.7.3.9"
                                                   ?disabled=${!mayManage} />
                                            <span class="hint">Any number of them, with a blank or a comma between.</span>
                                        </label>
                                    </fieldset>
                                `}

                                <span class="hint">
                                    Asked for is not granted: what goes into the certificate is the certificate
                                    authority's to decide, and what came back says so beside the key.
                                </span>

                            </details>
                        `}

                        <div class="notice">
                            <strong>Making a key and holding it up are two questions.</strong> Every kind here
                            can be generated and made into a signing request that a certificate authority will
                            accept. Whether this ${nodeNoun()} can then load the certificate that comes back together
                            with its key depends on the runtime underneath - Ed25519, Ed448 and the
                            post-quantum kinds have no key object in .NET today. Such a certificate is kept and
                            passed over rather than refused, and this page says so beside it.
                        </div>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayManage}>
                                Make a key and a request
                            </button>
                            <span id="create-note"  class="form-notice" role="status"></span>
                            <span id="create-error" class="form-error"  role="alert"></span>
                        </div>

                        <span class="hint">
                            An RSA 4096 or a post-quantum key takes a few seconds to generate.
                            Written to ${certificates.directory}, the private key readable by nobody else.
                        </span>

                    </form>

                </section>

                ${justMade !== null ? html`
                    <section class="card">
                        <h2><i class="fa-solid fa-file-export"></i> The request, waiting to be collected</h2>
                        <p class="hint">
                            Hand this to whoever issues certificates for this ${nodeNoun()}${certificates.canBeSentToCSMS
                                ? ', or send it to the CSMS from the key below'
                                : ''}. It is not a secret -
                            it holds the public half of the key and nothing else - and it stays here until
                            the answer comes back.
                        </p>
                        <textarea id="just-made" class="mono" rows="10" readonly .defaultValue=${justMade.csr}></textarea>
                        <div class="form-actions">
                            <button type="button" id="copy-csr" class="btn small" @click=${copyJustMade}>Copy</button>
                            <span id="copy-note" class="form-notice" role="status"></span>
                        </div>
                    </section>
                ` : nothing}

                ${Host.bringsIn ? html`
                    <section class="card">

                        <h2><i class="fa-solid fa-file-import"></i> Bring a certificate in</h2>

                        <form id="import-form" class="form-stack" @submit=${bringIn}>

                            <label>The certificate, and any intermediates
                                <textarea name="pem" class="mono" rows="8"
                                          placeholder="-----BEGIN CERTIFICATE-----"
                                          ?disabled=${!mayManage}></textarea>
                                <span class="hint">
                                    PEM, this ${nodeNoun()}'s own certificate first. It is matched to the key whose
                                    request it answers, so nothing has to be said about which one it is for.
                                </span>
                            </label>

                            <div class="form-actions">
                                <button type="submit" class="btn primary" ?disabled=${!mayManage}>
                                    Bring it in
                                </button>
                                <span id="import-note"  class="form-notice" role="status"></span>
                                <span id="import-error" class="form-error"  role="alert"></span>
                            </div>

                            <span class="hint">${noKeysComeIn(certificates)}</span>

                        </form>

                    </section>
                ` : nothing}

            </div>

            <section class="card">

                <h2><i class="fa-solid fa-list"></i> What is here</h2>

                <p class="hint">
                    Newest first. By this ${nodeNoun()}'s clock it is
                    ${formatTimestamp(certificates.now)}, which is what the days below are counted against.
                    ${Host.bringsIn ? nothing : html`
                        The certificate that comes back for a key goes in on the Upload tab, without a key:
                        it is matched to the key whose request it answers. ${noKeysComeIn(certificates)}
                    `}
                </p>

                ${certificates.entries.length === 0
                      ? html`
                            <p class="hint">
                                Nothing yet. This ${nodeNoun()} can only dial a back end that does not ask it
                                for a certificate.
                            </p>
                        `
                      : html`<div class="cards">${repeat(certificates.entries, entry => entry.id, entry => entryCard(entry, true))}</div>`}

            </section>

        `;

    }

    /** Why there is no way to bring a private key in - said where a node says there is none. */
    function noKeysComeIn(certificates: StationCertificates): TemplateResult | typeof nothing {
        return certificates.canImportPrivateKeys
                   ? nothing
                   : html`
                         A private key cannot be brought in, only a certificate. The key is
                         made on this ${nodeNoun()} and never leaves it - one that arrived from
                         somewhere else is one somebody else has a copy of.
                     `;
    }

    /** One key: what it is, where it stands, and what there is to do with it. */
    function entryCard(entry: StationKey, removable: boolean): TemplateResult {

        const asks = entry.asksFor;

        return html`
            <div class="card ${entry.inUse ? 'in-use' : ''}">

                <div class="key-head">
                    <strong>${entry.subject || entry.id}</strong>
                    ${entry.inUse ? html`<span class="chip on">in use</span>` : nothing}
                    ${entry.certificate === undefined
                          ? html`<span class="chip">${entry.sentToCSMSAt !== undefined ? 'request sent to the CSMS' : 'request out'}</span>`
                          : entry.certificate.expired
                                ? html`<span class="chip warn">expired</span>`
                                : entry.certificate.notYetValid
                                      ? html`<span class="chip">not yet valid</span>`
                                      : html`<span class="chip">${entry.certificate.daysLeft} days left</span>`}
                </div>

                <dl class="kv">
                    <dt>Key</dt>          <dd>${entry.algorithm}</dd>
                    <dt>Identification</dt> <dd><code>${entry.id}</code></dd>
                    <dt>Made</dt>         <dd>${formatTimestamp(entry.createdAt)}</dd>
                    ${asks === undefined ? nothing : html`
                        <dt>Asks for</dt>
                        <dd>
                            ${asks.templateName}${asks.role ? html`, as <code>${asks.role}</code>` : nothing}:
                            ${[ ...asks.keyUsages, ...asks.extendedKeyUsages ].join(', ')}
                        </dd>
                    `}
                    ${entry.sentToCSMSAt !== undefined && entry.certificate === undefined ? html`
                        <dt>Sent to the CSMS</dt> <dd>${formatTimestamp(entry.sentToCSMSAt)}</dd>
                    ` : nothing}
                    ${entry.certificate !== undefined ? html`
                        <dt>Issued by</dt>  <dd>${entry.certificate.issuer}</dd>
                        <dt>Valid</dt>      <dd>${formatTimestamp(entry.certificate.notBefore)}
                                                &ndash; ${formatTimestamp(entry.certificate.notAfter)}</dd>
                        <dt>Fingerprint</dt><dd><code>${entry.certificate.thumbprintSHA256}</code></dd>
                        <dt>Sent along</dt> <dd>${entry.certificate.intermediates} intermediate(s)</dd>
                    ` : nothing}
                </dl>

                ${(entry.warnings ?? []).map(warning => html`<div class="notice">${warning}</div>`)}

                ${entry.csr !== undefined ? html`
                    <details>
                        <summary>The signing request, waiting to be collected</summary>
                        <textarea class="mono" rows="8" readonly data-csr="${entry.id}" .defaultValue=${entry.csr}></textarea>
                    </details>
                ` : nothing}

                <div class="form-actions key-actions" data-key="${entry.id}">
                    ${entry.csr === undefined ? nothing : html`
                        <button type="button" class="btn small" data-copy-csr="${entry.id}"
                                @click=${() => copyRequest(entry)}>
                            Copy CSR
                        </button>
                        ${store?.canBeSentToCSMS ? html`
                            <button type="button" class="btn small" data-send-csr="${entry.id}"
                                    title="Send the signing request to the CSMS, in SignCertificate"
                                    ?disabled=${!mayManage}
                                    @click=${() => void sendToCSMS(entry.id)}>
                                Send to CSMS
                            </button>
                        ` : nothing}
                    `}
                    ${removable ? html`
                        <button type="button" class="btn small" data-remove="${entry.id}"
                                ?disabled=${!(mayManage && !entry.inUse)}
                                @click=${() => void remove(entry.id)}>
                            Remove
                        </button>
                    ` : nothing}
                    <span class="form-notice" role="status" data-key-note></span>
                    <span class="form-error"  role="alert"  data-key-error></span>
                    ${removable && entry.inUse
                          ? html`<span class="hint">
                                     The one this ${nodeNoun()} dials with. Bring another in before removing it,
                                     or it comes back from its next restart unable to connect.
                                 </span>`
                          : nothing}
                </div>

            </div>
        `;

    }

    /**
     * The keys that have no certificate yet, among what the node presents:
     * not something it can show anybody, and who it is all the same.
     */
    function waiting(makeOne?: () => void): TemplateResult | typeof nothing {

        const keys = (store?.entries ?? []).filter(entry => entry.certificate === undefined);

        if (keys.length === 0)
            return nothing;

        return html`
            <section class="card" id="waiting-keys">

                <h3><i class="fa-solid fa-hourglass-half"></i> Keys waiting for their certificate</h3>

                <p class="hint">
                    Made on this ${nodeNoun()}, each with a private key that never leaves it, and shown to
                    nobody yet: a key becomes an identity when the certificate for it comes back.
                    Copy its signing request for whoever issues certificates${store?.canBeSentToCSMS
                        ? ', or send it to the CSMS, which answers with the certificate by itself'
                        : ''}.
                    ${makeOne === undefined ? nothing : html`
                        <button type="button" class="btn small" id="make-a-key" @click=${makeOne}>Make a key</button>
                    `}
                </p>

                <div class="cards">${repeat(keys, entry => entry.id, entry => entryCard(entry, false))}</div>

            </section>
        `;

    }


    /**
     * Say beside the list what is worth knowing about the key chosen from
     * it: the remark is drawn for the kind chosen, not for the one the
     * station suggests - and so are the usages a key of that kind can have.
     */
    function pick(algorithm: string): void {
        picked = store !== null && algorithm === store.defaultAlgorithm ? null : algorithm;
        Host.draw();
    }

    /** The same for the template: its remark, and the role it asks for. */
    function pickTemplate(template: string): void {
        pickedTemplate = store !== null && template === store.defaultTemplate ? null : template;
        Host.draw();
    }

    function copyJustMade(): void {
        const box = content.querySelector<HTMLTextAreaElement>('#just-made');
        if (box === null)
            return;
        box.select();
        void navigator.clipboard?.writeText(box.value).then(
            () => say('#copy-note', 'Copied.'),
            () => say('#copy-note', 'Select it and copy it by hand.')
        );
    }

    /** The request of a key, on the clipboard - or said where to find it, where the browser has none to offer. */
    function copyRequest(entry: StationKey): void {

        if (entry.csr === undefined)
            return;

        sayOf(entry.id, 'error', '');

        const byHand = 'Open the signing request below, select it and copy it by hand.';

        if (navigator.clipboard === undefined) {
            sayOf(entry.id, 'note', byHand);
            return;
        }

        void navigator.clipboard.writeText(entry.csr).then(
            () => sayOf(entry.id, 'note', 'Copied.'),
            () => sayOf(entry.id, 'note', byHand)
        );

    }


    async function create(event: SubmitEvent): Promise<void> {

        event.preventDefault();

        const form  = event.currentTarget as HTMLFormElement;
        const note  = must<HTMLElement>(content, '#create-note');

        note.textContent = '';
        say('#create-error', '');

        // Read before the page is held still: a disabled field is left out
        // of a FormData - which is also how a usage the kind of key chosen
        // cannot have is left out, ticked or not.
        const data      = new FormData(form);
        const subject   = field(form, 'subject');
        const algorithm = field(form, 'algorithm');
        const role      = field(form, 'role');
        const others    = field(form, 'otherExtendedKeyUsages').split(/[\s,;]+/).filter(one => one.length > 0);

        const asks: KeyAsks = {
            template:           data.has('template') ? field(form, 'template') : undefined,
            role:               role.length > 0 ? role : undefined,
            keyUsages:          data.getAll('keyUsage').map(String),
            extendedKeyUsages:  [ ...data.getAll('extendedKeyUsage').map(String), ...others ]
        };

        try
        {
            const made = await whileSaving(content, note, () => api.clientKeys.create(subject, algorithm, asks));

            if (!Host.alive())
                return;

            justMade        = { id: made.id, csr: made.csr };
            store           = made.certificates;
            picked          = null;
            pickedTemplate  = null;

            Host.draw();

            // Made, so no longer typed: the form is empty again, the kind
            // of key and the template back at the ones the station suggests.
            form.reset();

            say('#create-note', 'Made. The request is below, waiting to be collected.');
        }
        catch (problem)
        {
            if (Host.alive())
                say('#create-error', errorMessage(problem));
        }

    }

    /** A certificate that answers a request made here, and what there is to say about it. */
    async function take(pem: string): Promise<string> {

        const taken = await api.clientKeys.add(pem);

        store     = taken.certificates;
        justMade  = null;

        return taken.warnings.length > 0
                   ? `Taken in. ${taken.warnings.join(' ')}`
                   : 'Taken in.';

    }

    async function bringIn(event: SubmitEvent): Promise<void> {

        event.preventDefault();

        const form = event.currentTarget as HTMLFormElement;
        const note = must<HTMLElement>(content, '#import-note');

        note.textContent = '';
        say('#import-error', '');

        const pem = field(form, 'pem', false);

        try
        {
            const said = await whileSaving(content, note, () => take(pem));

            if (!Host.alive())
                return;

            Host.draw();

            // Taken in, so no longer typed.
            form.reset();

            say('#import-note', said);
        }
        catch (problem)
        {
            if (Host.alive())
                say('#import-error', errorMessage(problem));
        }

    }

    /** The request of a key to the CSMS, which answers whether it will sign it - the certificate comes later, by itself. */
    async function sendToCSMS(id: string): Promise<void> {

        sayOf(id, 'note',  '');
        sayOf(id, 'error', '');

        const note = content.querySelector<HTMLElement>(`[data-key="${id}"] [data-key-note]`);

        try
        {
            const sent = await whileSaving(content, note, () => api.clientKeys.sendToCSMS(id));

            if (!Host.alive())
                return;

            store = sent.certificates;

            Host.draw();

            sayOf(id, 'note', sent.said);
        }
        catch (problem)
        {
            if (Host.alive())
                sayOf(id, 'error', errorMessage(problem));
        }

    }

    async function remove(id: string): Promise<void> {

        if (id === '')
            return;

        const note = content.querySelector<HTMLElement>('#create-note');

        say('#create-note', '');

        try
        {
            store = await whileSaving(content, note, () => api.clientKeys.remove(id));

            if (!Host.alive())
                return;

            if (justMade?.id === id)
                justMade = null;

            Host.draw();
        }
        catch (problem)
        {
            if (Host.alive())
                say('#create-error', errorMessage(problem));
        }

    }

    function load(): Promise<void> {

        // A section and a tab of one page both ask, at once: one question.
        loading ??= api.clientKeys.get().
                        then   (loaded => { store = loaded; }).
                        finally(()     => { loading = null; });

        return loading;

    }

    function reset(): void {
        picked          = null;
        pickedTemplate  = null;
        justMade        = null;
    }

    return { load, reset, view, waiting, take };

}


// ---------------------------------------------------------------------------
// On the identities page
// ---------------------------------------------------------------------------

/** One panel for a page that is open, whoever asks for it: its tab and its section draw the same keys. */
const panels = new WeakMap<object, ClientKeysPanel | null>();

/** The panel of an identities page - or null for whoever may not read what a node dials with. */
function panelOf(Context: SectionContext<any>): ClientKeysPanel | null {

    if (!panels.has(Context))
        panels.set(Context, auth.can(connectionsResource(), 'read')
                                ? clientKeysPanel({
                                      page:       Context.page,
                                      mayManage:  auth.can(connectionsResource(), 'edit'),
                                      draw:       () => Context.draw(),
                                      alive:      () => Context.page.isConnected,
                                      bringsIn:   false
                                  })
                                : null);

    return panels.get(Context) ?? null;

}

/**
 * The keys as a tab of the identities page: where one is made, and every key
 * there is. None for whoever may not read what the node dials with.
 *
 *   certificates: { tabs: clientKeysTab, sections: waitingKeysSection, answers: clientKeyAnswers() }
 */
export function clientKeysTab(Context: SectionContext<any>): CertificatesTab<any>[] {

    const panel = panelOf(Context);

    return panel === null ? [] : [{
        page:   'identities',
        tab:    { id: clientKeysTabId, label: 'Client keys', icon: 'fa-key' },
        load:   () => panel.load(),
        draw:   () => panel.view(),
        reset:  () => panel.reset()
    }];

}

/**
 * The keys that wait for their certificate, among what the node presents on
 * its identities page: each with its request to copy, or to send to the CSMS.
 */
export function waitingKeysSection(Context: SectionContext<any>): CertificatesSection<any>[] {

    const panel = panelOf(Context);

    return panel === null ? [] : [{
        below:  'presents',
        page:   'identities',
        load:   () => panel.load(),
        draw:   () => html`${panel.waiting(() => Context.show(clientKeysTabId))}`
    }];

}

/**
 * Where a certificate uploaded on the identities without its key goes: to the
 * key that was made here and waits for it - which is why the keys need no
 * upload of their own.
 */
export function clientKeyAnswers(): CertificateAnswers {
    return {

        hint:   html`
                    A certificate that answers a signing request made on the Client keys tab comes
                    without a key: it is matched to the key whose request it answers.
                `,

        found:  html`
                    No private key in the box: this is taken as the answer to a signing request made here,
                    and goes to the key that waits for it - whatever is ticked below.
                `,

        take:   async pem => {
                    const taken = await api.clientKeys.add(pem);
                    return taken.warnings.length > 0
                               ? `Taken in, for the key ${taken.id}. ${taken.warnings.join(' ')}`
                               : `Taken in, for the key ${taken.id}.`;
                }

    };
}


// ---------------------------------------------------------------------------
// A page of its own
// ---------------------------------------------------------------------------

/**
 * The keys on a page of their own, for a kind of node that has no identities
 * page to show them on: making one, bringing its certificate in, and every
 * key there is.
 */
export const clientKeysPage: Page = {

    title: 'Client keys',

    render({ root }) {

        const content = shell(root, {
            active:    '/configuration/client-keys',
            title:     'Client keys',
            subtitle:  `What this ${nodeNounInFull()} says it is when it dials a back end.`,
            actions:   reloadButton(() => reload())
        });

        render(content, html`<div class="loading">Loading ...</div>`);

        let cancelled = false;
        let loaded    = false;

        const panel = clientKeysPanel({
            page:       content,
            mayManage:  auth.can(connectionsResource(), 'edit'),
            draw:       () => { if (loaded) render(content, panel.view()); },
            alive:      () => !cancelled,
            bringsIn:   true
        });

        async function load(): Promise<void> {

            try
            {
                await panel.load();

                if (cancelled)
                    return;

                loaded = true;
                render(content, panel.view());
            }
            catch (problem)
            {
                if (!cancelled)
                    render(content, html`
                        <div class="error-box">The certificates could not be loaded: ${errorMessage(problem)}</div>
                    `);
            }

        }

        /**
         * Loaded anew - Reload - is what the station has, the forms too, which
         * a draw on its own would leave as typed.
         */
        async function reload(): Promise<void> {
            panel.reset();
            await load();
            if (!cancelled)
                content.querySelectorAll('form').forEach(form => form.reset());
        }

        // A half-typed subject or a certificate pasted but not yet brought in
        // is work like any other.
        const release = unsaved.heldBy(() => typedSinceDrawn(content.querySelector('#create-form')) ||
                                             typedSinceDrawn(content.querySelector('#import-form')));

        void load();

        return () => { cancelled = true; release(); };

    }

};
