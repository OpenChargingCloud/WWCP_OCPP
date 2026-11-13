import { connectionsAPI as api, type StationCertificates, type StationKey } from '../api/connections';
import { auth } from '../auth';
import { connectionsResource, nodeNoun, nodeNounInFull, nodeTraits } from '../settings';
import { must } from '@node/html';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { errorMessage, field, formatTimestamp, whileSaving } from '@node/ui';
import { typedSinceDrawn, unsaved } from '@node/unsaved';
import { html, nothing, render, repeat, type TemplateResult } from '@node/view';

/**
 * The keys and certificates this station dials a back end with - its client
 * keys, apart from the certificate store of the Certificates page because they
 * are made here rather than imported.
 *
 * The page is built around the one thing that is hard about certificates:
 * replacing one before it runs out, without a window in which the station
 * cannot connect. So more than one lives here at a time, the replacement is
 * brought in the day it arrives even if it only becomes valid in two days, and
 * the station switches over by itself at the moment it may.
 *
 * There is no way to upload a private key, and saying so on the page is
 * deliberate: the key is made here and never leaves, and somebody looking for
 * the button should find the reason instead.
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

        const mayManage = auth.can(connectionsResource(), 'edit');

        let cancelled = false;
        let store: StationCertificates | null = null;

        /** The request that was just made, put in front of somebody straight away. */
        let justMade: { id: string; csr: string } | null = null;

        /**
         * The kind of key chosen in the form, while it is not the one the
         * station suggests: what the remark beside the list is about.
         */
        let picked: string | null = null;


        function draw(): void {

            if (store === null)
                return;

            const certificates = store;
            const chosen       = certificates.algorithms.find(one => one.id === (picked ?? certificates.defaultAlgorithm));

            render(content, html`

                ${mayManage ? nothing : html`
                    <div class="notice">${mayButNot('look at the certificates', 'make or replace them')}</div>
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
                                Hand this to whoever issues certificates for this ${nodeNoun()}. It is not a secret -
                                it holds the public half of the key and nothing else - and it stays here until
                                the answer comes back.
                            </p>
                            <textarea id="just-made" class="mono" rows="10" readonly .defaultValue=${justMade.csr}></textarea>
                            <div class="form-actions">
                                <button type="button" id="copy-csr" class="btn small" @click=${copy}>Copy</button>
                                <span id="copy-note" class="form-notice" role="status"></span>
                            </div>
                        </section>
                    ` : nothing}

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

                            <span class="hint">
                                ${certificates.canImportPrivateKeys
                                      ? nothing
                                      : html`
                                            A private key cannot be brought in, only a certificate. The key is
                                            made on this ${nodeNoun()} and never leaves it - one that arrived from
                                            somewhere else is one somebody else has a copy of.
                                        `}
                            </span>

                        </form>

                    </section>

                </div>

                <section class="card">

                    <h2><i class="fa-solid fa-list"></i> What is here</h2>

                    <p class="hint">
                        Newest first. By this ${nodeNoun()}'s clock it is
                        ${formatTimestamp(certificates.now)}, which is what the days below are counted against.
                    </p>

                    ${certificates.entries.length === 0
                          ? html`
                                <p class="hint">
                                    Nothing yet. This ${nodeNoun()} can only dial a back end that does not ask it
                                    for a certificate.
                                </p>
                            `
                          : html`<div class="cards">${repeat(certificates.entries, entry => entry.id, entryCard)}</div>`}

                </section>

            `);

        }

        function entryCard(entry: StationKey): TemplateResult {

            return html`
                <div class="card ${entry.inUse ? 'in-use' : ''}">

                    <div class="key-head">
                        <strong>${entry.subject || entry.id}</strong>
                        ${entry.inUse ? html`<span class="chip on">in use</span>` : nothing}
                        ${entry.certificate === undefined
                              ? html`<span class="chip">request out</span>`
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

                    <div class="form-actions">
                        <button type="button" class="btn small" data-remove="${entry.id}"
                                ?disabled=${!(mayManage && !entry.inUse)}
                                @click=${() => void remove(entry.id)}>
                            Remove
                        </button>
                        ${entry.inUse
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
         * Say beside the list what is worth knowing about the key chosen from
         * it: the remark is drawn for the kind chosen, not for the one the
         * station suggests.
         */
        function pick(algorithm: string): void {
            picked = store !== null && algorithm === store.defaultAlgorithm ? null : algorithm;
            draw();
        }

        function copy(): void {
            const box = content.querySelector<HTMLTextAreaElement>('#just-made');
            if (box === null)
                return;
            box.select();
            void navigator.clipboard?.writeText(box.value).then(
                () => { must<HTMLElement>(content, '#copy-note').textContent = 'Copied.'; },
                () => { must<HTMLElement>(content, '#copy-note').textContent = 'Select it and copy it by hand.'; }
            );
        }


        async function create(event: SubmitEvent): Promise<void> {

            event.preventDefault();

            const form  = event.currentTarget as HTMLFormElement;
            const note  = must<HTMLElement>(content, '#create-note');

            note.textContent = '';
            must<HTMLElement>(content, '#create-error').textContent = '';

            // Read before the page is held still: a disabled field is left out
            // of a FormData.
            const subject   = field(form, 'subject');
            const algorithm = field(form, 'algorithm');

            try
            {
                const made = await whileSaving(content, note, () => api.clientKeys.create(subject, algorithm));

                if (cancelled)
                    return;

                justMade  = { id: made.id, csr: made.csr };
                store     = made.certificates;
                picked    = null;

                draw();

                // Made, so no longer typed: the form is empty again, the kind
                // of key back at the one the station suggests.
                form.reset();

                note.textContent = 'Made. The request is below, waiting to be collected.';
            }
            catch (problem)
            {
                if (!cancelled)
                    must<HTMLElement>(content, '#create-error').textContent = errorMessage(problem);
            }

        }

        async function bringIn(event: SubmitEvent): Promise<void> {

            event.preventDefault();

            const form = event.currentTarget as HTMLFormElement;
            const note = must<HTMLElement>(content, '#import-note');

            note.textContent = '';
            must<HTMLElement>(content, '#import-error').textContent = '';

            const pem = field(form, 'pem', false);

            try
            {
                const taken = await whileSaving(content, note, () => api.clientKeys.add(pem));

                if (cancelled)
                    return;

                store     = taken.certificates;
                justMade  = null;

                draw();

                // Taken in, so no longer typed.
                form.reset();

                note.textContent = taken.warnings.length > 0
                                       ? `Taken in. ${taken.warnings.join(' ')}`
                                       : 'Taken in.';
            }
            catch (problem)
            {
                if (!cancelled)
                    must<HTMLElement>(content, '#import-error').textContent = errorMessage(problem);
            }

        }

        async function remove(id: string): Promise<void> {

            if (id === '')
                return;

            const note = must<HTMLElement>(content, '#create-note');

            note.textContent = '';

            try
            {
                store = await whileSaving(content, note, () => api.clientKeys.remove(id));

                if (cancelled)
                    return;

                if (justMade?.id === id)
                    justMade = null;

                draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    must<HTMLElement>(content, '#create-error').textContent = errorMessage(problem);
            }

        }

        async function load(): Promise<void> {

            try
            {
                const loaded = await api.clientKeys.get();

                if (cancelled)
                    return;

                store = loaded;
                draw();
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
            picked = null;
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
