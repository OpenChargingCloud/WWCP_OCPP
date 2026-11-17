import { stationServerAPI as api, type OCPPServerConfiguration, type ServerCertificate, type ServerCertificateFound,
         type ServerCertificates, type ServerCertificatesUploaded } from '../api/stationServer';
import { auth } from '../auth';
import { nodeNoun } from '../settings';
import { ApiError } from '@node/api/client';
import { must } from '@node/html';
import { toURL } from '@node/basePath';
import { appendText, pemBox } from '@node/pemBox';
import { base64Of, fingerprintOf } from '@node/pages/certificates';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { rememberTab, tabFromURL, tabsView, type Tab } from '@node/tabs';
import { errorMessage, field, formatTimestamp } from '@node/ui';
import { anyFormTypedSinceDrawn, unsaved } from '@node/unsaved';
import { html, nothing, render, repeat, type TemplateResult } from '@node/view';

/** The largest file the upload offers to read: a certificate chain is a few kilobytes. */
const largestFile = 1024 * 1024;

/** How long the upload waits after the last key before it asks what the box holds. */
const inspectAfter_ms = 350;

/** The tabs: the keys and their requests, every certificate by name, and the upload. */
const allTabs: readonly Tab[] = [
    { id: 'keys',    label: 'Keys and requests',  icon: 'fa-key'         },
    { id: 'all',     label: 'All certificates',   icon: 'fa-list'        },
    { id: 'upload',  label: 'Upload',             icon: 'fa-file-import' }
];

/**
 * The certificates this local controller presents to the charging stations.
 *
 * The page is built around the one thing that is hard about certificates:
 * replacing one before it runs out, without a window in which nothing works.
 * So more than one lives here at a time, the replacement is uploaded the day it
 * arrives even if it only becomes valid in two days, and the controller
 * switches over by itself at the moment it may.
 *
 * Three tabs. **Keys and requests** makes a key and its signing request, and
 * shows each key with the certificate it was given. **All certificates** is
 * every certificate once, by name or by fingerprint, with the key it is for.
 * **Upload** is a box for certificates as text - typed, pasted, or dropped
 * files on - every one of which goes in under the key it belongs to, which the
 * controller works out from the certificate itself: renewals for several keys
 * are pasted at once, and nobody has to know which is whose.
 *
 * There is no way to upload a private key, and saying so on the page is
 * deliberate: the key is made here and never leaves, and somebody looking for
 * the button should find the reason instead. A key that comes along in a file
 * is not put into the box, and its certificate is said to be refused for it.
 *
 * Drawn by view.ts: a draw changes only what differs, so that a certificate
 * pasted under one key outlives another key being made or removed - and the
 * upload, half filled in, outlives a look at another tab.
 */
export const serverCertificatesPage: Page = {

    title: 'Server certificates',

    render({ root, url }) {

        const content = shell(root, {
            active:    '/configuration/ocpp-server/certificates',
            title:     'Server certificates',
            subtitle:  `What this ${nodeNoun()} presents to the charging stations, and what takes over when it runs out.`,
            actions:   reloadButton(() => load())
        });

        render(content, html`<div class="loading">Loading ...</div>`);

        const mayManage = auth.can('certificates', 'edit');
        const tabs      = mayManage ? allTabs : allTabs.filter(tab => tab.id !== 'upload');

        let cancelled = false;
        let store:  ServerCertificates      | null = null;
        let server: OCPPServerConfiguration | null = null;

        /** The request that was just made, offered for download straight away. */
        let justMade: { id: string; csr: string } | null = null;

        /**
         * The kind of key chosen, whose remark the form shows - or undefined
         * while it is the one drawn as chosen.
         */
        let algorithmChosen: string | undefined;

        /** The tab shown; the order of the list of all certificates, and what it is narrowed to. */
        let shown   = tabFromURL(tabs, url);
        let order:  'name' | 'fingerprint' = 'name';
        let narrow  = '';

        // What the upload has found and done, for as long as the page is open:
        // the box itself holds its text.
        let found:         ServerCertificateFound[]          = [];
        let foundProblem:  string | null                     = null;
        let uploaded:      ServerCertificatesUploaded | null = null;
        let inspection     = 0;
        let inspectTimer:  ReturnType<typeof setTimeout> | undefined;


        /** Show another tab, and keep it in the address. */
        function show(id: string): void {
            shown = id;
            rememberTab(tabs, id);
            draw();
        }


        function draw(): void {

            if (store === null || server === null)
                return;

            const certificates   = store;
            const configuration  = server;

            render(content, html`

                ${mayManage ? nothing : html`
                    <div class="notice">
                        ${mayButNot('look at the certificates', 'make or replace them')}
                    </div>
                `}

                ${configuration.reachableAs.length > 0 ? nothing : html`
                    <div class="notice warn">
                        This ${nodeNoun()} has not been told what it is
                        <a href="${toURL('/configuration/ocpp-server')}">reachable as</a>. A signing request without those names
                        produces a certificate that no charging station will accept, so none can be made yet.
                    </div>
                `}

                ${tabsView(tabs, shown, show, 'The server certificates')}

                <section class="tab-panel" role="tabpanel" id="panel-keys" aria-labelledby="tab-keys" ?hidden=${shown !== 'keys'}>
                    ${keysView(certificates, configuration)}
                </section>

                <section class="tab-panel" role="tabpanel" id="panel-all" aria-labelledby="tab-all" ?hidden=${shown !== 'all'}>
                    ${allView(certificates)}
                </section>

                ${mayManage ? html`
                    <section class="tab-panel" role="tabpanel" id="panel-upload" aria-labelledby="tab-upload" ?hidden=${shown !== 'upload'}>
                        ${uploadView()}
                    </section>` : nothing}

            `);

        }


        // ---------------------------------------------------------------------
        // Keys and requests
        // ---------------------------------------------------------------------

        function keysView(certificates: ServerCertificates, configuration: OCPPServerConfiguration): TemplateResult {

            const kindOfKey = algorithmChosen ?? 'ecdsa-p256';

            return html`
                <div class="cards">

                    ${keyPinsCard(certificates)}

                    <section class="card">

                        <h2><i class="fa-solid fa-key"></i> Make a signing request</h2>

                        <form id="create-form" class="form-stack" @submit=${create}>

                            <label>Subject
                                <input type="text" name="subject" maxlength="200"
                                       value="${configuration.reachableAs[0] ?? ''}"
                                       placeholder="lc001.example.org"
                                       ?disabled=${!mayManage} />
                            </label>

                            <label>Key
                                <select name="algorithm" id="algorithm" ?disabled=${!mayManage}
                                        @change=${(event: Event) => { algorithmChosen = (event.target as HTMLSelectElement).value; draw(); }}>
                                    ${certificates.algorithms.map(algorithm => html`
                                        <option value="${algorithm.id}"
                                                ?selected=${algorithm.id === 'ecdsa-p256'}>
                                            ${algorithm.name}${algorithm.presentable === false ? ' - not servable here' : ''}
                                        </option>
                                    `)}
                                </select>
                            </label>

                            <span class="hint" id="algorithm-remark">
                                ${certificates.algorithms.find(one => one.id === kindOfKey)?.remark ?? ''}
                            </span>

                            <div class="notice">
                                <strong>Making a key and serving it are two questions.</strong> Every kind here can be
                                generated and made into a signing request. Whether this machine can then hold the
                                certificate up to a charging station depends on its TLS stack - Ed25519, Ed448, P-521
                                and the post-quantum kinds are refused by some platforms and served by others. This
                                ${nodeNoun()} finds out by trying, and says so beside the certificate rather than
                                letting a handshake fail.
                            </div>

                            <p class="hint">
                                The request will be made out for
                                ${configuration.reachableAs.length > 0
                                      ? html`<strong>${configuration.reachableAs.join(', ')}</strong>`
                                      : html`<em>nothing</em>`}.
                            </p>

                            <div class="form-actions">
                                <button type="submit" class="btn primary"
                                        ?disabled=${!mayManage || configuration.reachableAs.length === 0}>
                                    Generate a key and a request
                                </button>
                                <span id="create-error" class="form-error" role="alert"></span>
                            </div>

                            <div class="notice">
                                The private key is generated here and never leaves: there is no import, and no
                                route that would hand one out. What leaves is the signing request, which is a
                                public document.
                            </div>

                        </form>

                        ${justMade === null ? nothing : html`
                            <div class="notice ok">
                                <strong>The request for '${justMade.id}' is ready.</strong>
                                <a class="btn small" href="${api.ocppServer.certificates.csrURL(justMade.id)}" download>
                                    Download it
                                </a>
                                <pre class="pem">${justMade.csr}</pre>
                            </div>
                        `}

                    </section>

                    <section class="card wide">

                        <h2><i class="fa-solid fa-certificate"></i> Keys and certificates</h2>

                        <p class="hint">
                            In ${certificates.directory}. By this ${nodeNoun()}'s clock it is
                            ${formatTimestamp(certificates.now)} - which is what decides which of these is in its
                            window, so a ${nodeNoun()} whose clock is wrong presents the wrong one.
                        </p>

                        ${certificates.entries.length === 0
                              ? html`<p class="muted">No key has been made yet.</p>`
                              : repeat(certificates.entries, entry => entry.id, entry => entryCard(entry))}

                    </section>

                </div>
            `;

        }


        /**
         * The key pins a charging station pins to reach this controller - the
         * current key's, and the next one's ahead of a rollover - and the three
         * moves that make them: the server key where nothing is presented yet,
         * a next key, and the switch to it.
         */
        function keyPinsCard(certificates: ServerCertificates): TemplateResult {

            const current = certificates.keyPins?.current ?? null;
            const next    = certificates.keyPins?.next    ?? null;

            return html`
                <section class="card" id="keypins">

                    <h2><i class="fa-solid fa-fingerprint"></i> Key pins</h2>

                    <p class="hint">
                        A charging station that pins this ${nodeNoun()}'s key checks SHA-256 over its public key and
                        nothing else - no certificate authority, no name, no dates. The pin is the same whether the
                        key is presented in a certificate an authority issued for it or in the carrier certificate
                        made from it here. Hand it to the station at commissioning.
                    </p>

                    ${current === null
                          ? html`<p class="muted">No key is presented to the charging stations yet.</p>`
                          : html`
                              <dl class="kv">
                                  <dt>Key pin</dt>
                                  <dd><code class="keypin" id="current-keypin">${current.keyPin}</code></dd>
                                  <dt>Of the key</dt>
                                  <dd>
                                      <code>${current.id}</code>, made ${formatTimestamp(current.createdAt)},
                                      presented ${current.presented === 'carrier'
                                                      ? 'in its carrier certificate'
                                                      : 'in the certificate an authority issued for it'}
                                  </dd>
                              </dl>
                          `}

                    ${next === null ? nothing : html`
                        <dl class="kv">
                            <dt>Next key pin</dt>
                            <dd><code class="keypin" id="next-keypin">${next.keyPin}</code></dd>
                            <dt>Of the key</dt>
                            <dd><code>${next.id}</code>, made ${formatTimestamp(next.createdAt)} - presented once it is switched to</dd>
                        </dl>
                    `}

                    <div class="form-actions">
                        ${current === null ? html`
                            <button type="button" class="btn primary" id="serverkey-create" ?disabled=${!mayManage}
                                    @click=${() => void makeServerKey(false)}>
                                Make the server key
                            </button>
                        ` : next === null ? html`
                            <button type="button" class="btn" id="serverkey-next" ?disabled=${!mayManage}
                                    @click=${() => void makeServerKey(true)}>
                                Announce a next key
                            </button>
                        ` : html`
                            <button type="button" class="btn danger" id="serverkey-switch" ?disabled=${!mayManage}
                                    @click=${() => void switchKey(next.id)}>
                                Switch to the next key
                            </button>
                        `}
                        <span id="serverkey-error" class="form-error" role="alert"></span>
                    </div>

                    <p class="hint">
                        ${current === null
                              ? 'The server key is made here, without a signing request, and presented in a carrier certificate made from it. The charging station port encrypts from the next start.'
                              : next === null
                                    ? 'Ahead of a rollover: the next key is made now and only presented once somebody switches to it. Hand its pin to every station first.'
                                    : 'Once every station has the next pin, switch: stations that pinned only the current key are refused from their next handshake on.'}
                    </p>

                </section>
            `;

        }


        function entryCard(entry: ServerCertificate): TemplateResult {

            const certificate = entry.certificate;

            return html`
                <div class="entry ${entry.inUse ? 'in-use' : ''}">

                    <div class="entry-head">
                        <div>
                            <code>${entry.id}</code>
                            <span class="badge">${entry.algorithm}</span>
                            ${entry.inUse ? html`<span class="badge ok">being presented</span>` : nothing}
                            ${entry.next ? html`<span class="badge">next key</span>` : nothing}
                            ${entry.hasCarrier ? html`<span class="badge">server key</span>` : nothing}
                            ${certificate ? html`<span class="badge ${stateClass(certificate.state)}">${certificate.state}</span>` : nothing}
                            ${certificate || entry.hasCarrier ? nothing : html`<span class="badge warn">waiting for a certificate</span>`}
                            ${entry.canBePresented ? nothing : html`<span class="badge warn">not servable here</span>`}
                        </div>
                        <div class="entry-actions">
                            ${entry.hasCarrier ? nothing : html`
                                <a class="btn small" href="${api.ocppServer.certificates.csrURL(entry.id)}" download>
                                    Signing request
                                </a>
                            `}
                            <button type="button" class="btn small danger remove" data-id="${entry.id}"
                                    ?disabled=${!mayManage} @click=${() => void remove(entry.id)}>
                                Remove
                            </button>
                        </div>
                    </div>

                    <dl class="kv">
                        <dt>${entry.hasCarrier ? 'Carrier' : 'Asked for'}</dt>
                        <dd>${entry.carrier?.subject ?? entry.subject}</dd>

                        <dt>Key made</dt>
                        <dd>${formatTimestamp(entry.createdAt)}</dd>

                        ${entry.keyPin === undefined ? nothing : html`
                            <dt>Key pin</dt>
                            <dd><code class="small keypin" data-id="${entry.id}">${entry.keyPin}</code></dd>
                        `}

                        ${certificate ? html`
                            <dt>Issued to</dt>
                            <dd>${certificate.subject}</dd>

                            <dt>Issued by</dt>
                            <dd>${certificate.issuer}</dd>

                            <dt>Valid for</dt>
                            <dd>${certificate.subjectAltNames.length > 0 ? certificate.subjectAltNames.join(', ') : '-'}</dd>

                            <dt>Valid from</dt>
                            <dd>${formatTimestamp(certificate.notBefore)}</dd>

                            <dt>Valid until</dt>
                            <dd>
                                ${formatTimestamp(certificate.notAfter)}
                                <span class="${certificate.daysRemaining < 30 ? 'warn' : 'muted'}">
                                    (${certificate.daysRemaining} day(s))
                                </span>
                            </dd>

                            <dt>Intermediates</dt>
                            <dd>${certificate.intermediates}</dd>

                            <dt>Thumbprint</dt>
                            <dd><code class="small">${certificate.thumbprint}</code></dd>
                        ` : nothing}
                    </dl>

                    ${entry.warnings.length === 0 ? nothing : html`
                        <div class="notice warn">
                            <ul>${entry.warnings.map(warning => html`<li>${warning}</li>`)}</ul>
                        </div>
                    `}

                    <form class="form-stack upload-form" data-id="${entry.id}" @submit=${(event: SubmitEvent) => void upload(entry.id, event)}>

                        <label>${certificate
                                     ? 'Replace this certificate'
                                     : entry.hasCarrier
                                           ? 'A certificate an authority issued for this key - its carrier stays'
                                           : 'The certificate that answers this request'}
                            <textarea name="pem" rows="5" placeholder="-----BEGIN CERTIFICATE-----&#10;..."
                                      ?disabled=${!mayManage}></textarea>
                        </label>

                        <span class="hint">
                            The certificate first, then the intermediates that lead to it. The root is not needed:
                            a charging station either already trusts it or does not, and sending it changes
                            neither. A certificate that is already expired, or that belongs to a key that is not
                            here, is refused now rather than at the hour it would have taken over.
                        </span>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayManage}>Take it in</button>
                            <span class="form-error upload-error" role="alert"></span>
                        </div>

                    </form>

                </div>
            `;

        }


        // ---------------------------------------------------------------------
        // All certificates
        // ---------------------------------------------------------------------

        /** Every key that has a certificate, by the certificate's name or fingerprint, narrowed down as asked. */
        function certificatesListed(certificates: ServerCertificates): ServerCertificate[] {

            const wanted = narrow.trim().toLowerCase();

            return certificates.entries.
                       filter(entry => entry.certificate !== undefined).
                       filter(entry => wanted.length === 0 ||
                                       [ entry.id, entry.certificate!.subject, entry.certificate!.issuer, entry.certificate!.thumbprint,
                                         ...entry.certificate!.subjectAltNames ].some(said => said.toLowerCase().includes(wanted))).
                       sort((one, other) => order === 'fingerprint'
                                                ? one.certificate!.thumbprint.localeCompare(other.certificate!.thumbprint)
                                                : one.certificate!.subject.localeCompare(other.certificate!.subject, undefined, { sensitivity: 'base' }) ||
                                                  one.certificate!.thumbprint.localeCompare(other.certificate!.thumbprint));

        }


        function allView(certificates: ServerCertificates): TemplateResult {

            const listed = certificatesListed(certificates);
            const total  = certificates.entries.filter(entry => entry.certificate !== undefined).length;

            return html`

                <div class="list-controls">
                    <span class="segmented" role="radiogroup" aria-label="Order">
                        <label class="checkbox"><input type="radio" name="order" value="name"        .checked=${order === 'name'}
                                                       @change=${() => { order = 'name';        draw(); }} /> by name</label>
                        <label class="checkbox"><input type="radio" name="order" value="fingerprint" .checked=${order === 'fingerprint'}
                                                       @change=${() => { order = 'fingerprint'; draw(); }} /> by fingerprint</label>
                    </span>
                    <input type="search" id="all-filter" placeholder="Narrow down: a name, an issuer, a fingerprint, a key"
                           .value=${narrow} @input=${(event: Event) => { narrow = (event.target as HTMLInputElement).value; draw(); }} />
                    <span class="muted">${listed.length === total ? `${total} certificate(s)` : `${listed.length} of ${total} certificate(s)`}</span>
                </div>

                ${listed.length === 0
                      ? html`<p class="hint">${total === 0 ? 'No key has a certificate yet.' : 'None that matches.'}</p>`
                      : repeat(listed, entry => entry.id, entry => {
                            const certificate = entry.certificate!;
                            return html`
                                <article class="card certificate-card" data-certificate="${entry.id}">
                                    <h3>
                                        ${certificate.subject}
                                        ${entry.inUse ? html`<span class="badge ok">being presented</span>` : nothing}
                                        <span class="badge ${stateClass(certificate.state)}">${certificate.state}</span>
                                    </h3>
                                    <div class="certificate-facts">
                                        <span>issued by ${certificate.issuer}</span>
                                        <span class="muted">${[ `for key ${entry.id} (${entry.algorithm})`,
                                                                ...(certificate.intermediates > 0 ? [ `+${certificate.intermediates} intermediate(s)` ] : []),
                                                                `valid ${certificate.notBefore.slice(0, 10)} to ${certificate.notAfter.slice(0, 10)}`,
                                                                `${certificate.daysRemaining} day(s) left` ].join(', ')}</span>
                                        ${certificate.subjectAltNames.length > 0 ? html`<span class="muted">valid for ${certificate.subjectAltNames.join(', ')}</span>` : nothing}
                                        <code class="fingerprint" title="SHA-256">${fingerprintOf(certificate.thumbprint)}</code>
                                    </div>
                                </article>
                            `;
                        })}

            `;

        }


        // ---------------------------------------------------------------------
        // Upload
        // ---------------------------------------------------------------------

        function uploadView(): TemplateResult {

            return html`
                <section class="card">

                    <h2><i class="fa-solid fa-file-import"></i> Upload certificates</h2>

                    <form id="upload-all-form" class="form-stack" @submit=${(event: SubmitEvent) => { event.preventDefault(); void uploadAll(event.currentTarget as HTMLFormElement); }}>

                        <label for="upload-pem">Certificates, as text</label>
                        ${pemBox({
                            id:         'upload-pem',
                            name:       'pem',
                            rows:       12,
                            accept:     '.pem,.crt,.cer,.der,.p7b',
                            onFiles:    (files, box) => void readFiles(files, box),
                            onChanged:  () => inspectSoon()
                        })}
                        <p class="hint">
                            The certificates that answer this ${nodeNoun()}'s signing requests, each with the
                            intermediates that lead to it - for any number of keys at once. Which key each one is
                            for is read from the certificate itself. A file dropped on the box or chosen is read by
                            this ${nodeNoun()} and put into the box as PEM; a private key in it is not: the key is made
                            here and never arrives.
                        </p>
                        <span id="upload-files-note" class="form-error" role="alert"></span>

                        <div id="upload-found" aria-live="polite">${foundView()}</div>

                        <div class="form-actions">
                            <button type="submit" class="btn primary">Take them in</button>
                            <span id="upload-note"  class="form-notice" role="status"></span>
                            <span id="upload-error" class="form-error"  role="alert"></span>
                        </div>

                        <div id="upload-result">${resultView()}</div>

                    </form>

                </section>
            `;

        }


        function foundView(): TemplateResult | typeof nothing {

            if (foundProblem !== null)
                return html`<p class="form-error">${foundProblem}</p>`;

            if (found.length === 0)
                return nothing;

            return html`
                <p class="hint">The box holds ${found.length} certificate(s):</p>
                <ul class="found-certificates">
                    ${repeat(found, one => one.id, one => html`
                        <li data-found="${one.id}">
                            <strong>${one.label}</strong>
                            <code class="muted" title="SHA-256: ${one.thumbprint}">${one.id}</code>
                            ${one.chainLength > 0 ? html`<span class="chip">+${one.chainLength} intermediate(s)</span>` : nothing}
                            ${one.keyId !== null && one.refusal === null ? html`<span class="chip on">for key ${one.keyId}</span>` : nothing}
                            <span class="muted">valid until ${one.notAfter.slice(0, 10)}</span>
                            ${one.refusal === null ? nothing : html`<span class="form-error refusal">${one.refusal}</span>`}
                        </li>
                    `)}
                </ul>
            `;

        }


        function resultView(): TemplateResult | typeof nothing {

            if (uploaded === null)
                return nothing;

            return html`
                <ul class="upload-result">
                    ${uploaded.taken.map(one => html`
                        <li class="went-in">${one.label}: taken in for key ${one.id}${one.warnings.length > 0 ? html` - ${one.warnings.join(' ')}` : nothing}</li>
                    `)}
                    ${uploaded.refused.map(one => html`
                        <li class="refused"><strong>${one.label}</strong>: ${one.error}</li>
                    `)}
                </ul>
            `;

        }


        /** Ask what the box holds, once the typing has stopped. */
        function inspectSoon(): void {
            clearTimeout(inspectTimer);
            inspectTimer = setTimeout(() => void inspectNow(), inspectAfter_ms);
        }

        /** Ask what the box holds now - and forget an answer to an earlier question. */
        async function inspectNow(): Promise<void> {

            const text  = content.querySelector<HTMLTextAreaElement>('#upload-pem')?.value.trim() ?? '';
            const asked = ++inspection;

            if (text.length === 0) {
                found        = [];
                foundProblem = null;
                draw();
                return;
            }

            try
            {
                const answer = await api.ocppServer.certificates.inspect({ pem: text });

                if (cancelled || asked !== inspection)
                    return;

                found        = answer.certificates;
                foundProblem = null;
            }
            catch (problem)
            {
                if (cancelled || asked !== inspection)
                    return;

                found        = [];
                foundProblem = errorMessage(problem);
            }

            draw();

        }


        /** Files chosen or dropped: each read by the controller and put into the box as PEM, without a key - or why not. */
        async function readFiles(files: File[], box: HTMLTextAreaElement): Promise<void> {

            const problems: string[] = [];

            must<HTMLElement>(content, '#upload-files-note').textContent = '';

            for (const file of files)
            {

                if (file.size > largestFile) {
                    problems.push(`'${file.name}' is ${Math.round(file.size / 1024)} kB, and a certificate is a few - almost certainly not the file you meant.`);
                    continue;
                }

                try
                {
                    const answer = await api.ocppServer.certificates.inspect({ content: await base64Of(file) });

                    if (cancelled)
                        return;

                    appendText(box, answer.pem);

                    for (const one of answer.certificates.filter(certificate => certificate.hasPrivateKey))
                        problems.push(`'${file.name}' brought the private key of ${one.label} along, which was left out: the key is made here and never arrives.`);
                }
                catch (problem)
                {
                    problems.push(problem instanceof ApiError && (problem.body as { passwordWanted?: boolean } | undefined)?.passwordWanted === true
                                      ? `'${file.name}' opens only with a password - which a file of certificates alone does not need.`
                                      : `'${file.name}': ${errorMessage(problem)}`);
                }

            }

            if (!cancelled)
                must<HTMLElement>(content, '#upload-files-note').textContent = problems.join(' ');

        }


        /** Every certificate in the box taken in, each under the key it is for. */
        async function uploadAll(form: HTMLFormElement): Promise<void> {

            const note  = must<HTMLElement>(content, '#upload-note');
            const error = must<HTMLElement>(content, '#upload-error');
            const text  = must<HTMLTextAreaElement>(content, '#upload-pem').value.trim();

            note.textContent  = '';
            error.textContent = '';

            if (text.length === 0) {
                error.textContent = 'Paste a certificate into the box, or drop a file on it, first.';
                return;
            }

            let done: ServerCertificatesUploaded;

            try
            {
                done = await api.ocppServer.certificates.uploadAll({ pem: text });
            }
            catch (problem)
            {
                // What was pasted stays in the box, to be corrected.
                const said = problem instanceof ApiError ? problem.body as Partial<ServerCertificatesUploaded> | undefined : undefined;

                if (said?.refused !== undefined) {
                    uploaded = { taken: said.taken ?? [], refused: said.refused };
                    draw();
                }

                must<HTMLElement>(content, '#upload-error').textContent = errorMessage(problem);
                return;
            }

            if (cancelled)
                return;

            uploaded = done;
            justMade = null;
            store    = await api.ocppServer.certificates.get();
            server   = await api.ocppServer.get();

            if (done.refused.length === 0) {
                found        = [];
                foundProblem = null;
                draw();
                // Taken in, every one of them: the box goes back to an empty one.
                form.reset();
            }
            else
                draw();

            must<HTMLElement>(content, '#upload-note').textContent =
                done.refused.length === 0
                    ? `Taken in: ${done.taken.length} certificate(s).`
                    : `Taken in: ${done.taken.length} certificate(s); ${done.refused.length} not.`;

        }


        // ---------------------------------------------------------------------
        // One key
        // ---------------------------------------------------------------------

        function create(event: SubmitEvent): void {

            event.preventDefault();

            const form = event.currentTarget as HTMLFormElement;

            void makeKey(form, field(form, 'subject'), field(form, 'algorithm'));

        }


        async function makeKey(form: HTMLFormElement, subject: string, algorithm: string): Promise<void> {

            const error = must<HTMLElement>(content, '#create-error');
            error.textContent = '';

            try
            {

                const made = await api.ocppServer.certificates.create(subject, algorithm);

                if (cancelled)
                    return;

                justMade         = made;
                store            = await api.ocppServer.certificates.get();
                algorithmChosen  = undefined;

                draw();

                // A draw leaves a form as it is typed into; this one was made
                // into a request, so it goes back to what it starts with.
                form.reset();

            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }


        /** The server key, or the next one: made here, and its pin shown on the card. */
        async function makeServerKey(next: boolean): Promise<void> {

            const error = must<HTMLElement>(content, '#serverkey-error');
            error.textContent = '';

            try
            {

                const made = await api.ocppServer.serverKeys.create('ecdsa-p256', next);

                if (cancelled)
                    return;

                justMade  = null;
                store     = made.certificates;
                server    = await api.ocppServer.get();

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                    must<HTMLElement>(content, '#serverkey-error').textContent = errorMessage(problem);
            }

        }


        /** Present the next key from now on - asked first, because stations without its pin are locked out. */
        async function switchKey(id: string): Promise<void> {

            if (!window.confirm(`Present the next key '${id}' from now on?\n\n` +
                                'Charging stations that pinned only the current key are refused from their next handshake on.'))
                return;

            const error = must<HTMLElement>(content, '#serverkey-error');
            error.textContent = '';

            try
            {

                store = await api.ocppServer.serverKeys.switch();

                if (cancelled)
                    return;

                justMade = null;
                server   = await api.ocppServer.get();

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                    must<HTMLElement>(content, '#serverkey-error').textContent = errorMessage(problem);
            }

        }


        async function upload(id: string, event: SubmitEvent): Promise<void> {

            event.preventDefault();

            const form   = event.currentTarget as HTMLFormElement;
            const pem    = field(form, 'pem', false);
            const error  = form.querySelector<HTMLElement>('.upload-error');

            if (error)
                error.textContent = '';

            try
            {

                const answer = await api.ocppServer.certificates.upload(id, pem);

                if (cancelled)
                    return;

                justMade = null;
                store    = await api.ocppServer.certificates.get();
                server   = await api.ocppServer.get();

                draw();

                // Taken in: this key's form is emptied, and every other one
                // keeps what is pasted into it.
                form.reset();

                if (answer.warnings.length > 0)
                    window.alert(`The certificate was taken in, with something to say about it:\n\n${answer.warnings.join('\n\n')}`);

            }
            catch (problem)
            {
                if (error && !cancelled)
                    error.textContent = errorMessage(problem);
            }

        }


        async function remove(id: string): Promise<void> {

            if (!window.confirm(`Remove the key '${id}' and everything belonging to it? This cannot be undone.`))
                return;

            try
            {

                store = await api.ocppServer.certificates.remove(id);

                if (cancelled)
                    return;

                justMade = null;
                server   = await api.ocppServer.get();

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                    window.alert(errorMessage(problem));
            }

        }


        async function load(): Promise<void> {

            render(content, html`<div class="loading">Loading ...</div>`);

            try
            {

                const [certificates, configuration] = await Promise.all([
                    api.ocppServer.certificates.get(),
                    api.ocppServer.get()
                ]);

                if (cancelled)
                    return;

                store            = certificates;
                server           = configuration;
                algorithmChosen  = undefined;
                found            = [];
                foundProblem     = null;
                uploaded         = null;

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                    render(content, html`<div class="error-box">${errorMessage(problem)}</div>`);
            }

        }


        // A half-typed subject, or a certificate pasted but not yet taken in,
        // is work like any other.
        const release = unsaved.heldBy(() => anyFormTypedSinceDrawn(content));

        void load();

        return () => { cancelled = true; clearTimeout(inspectTimer); release(); };

    }

};


function stateClass(state: string): string {
    return state === 'valid' ? 'ok' : state === 'pending' ? '' : 'warn';
}
