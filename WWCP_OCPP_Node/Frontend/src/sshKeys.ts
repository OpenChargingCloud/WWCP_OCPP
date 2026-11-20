import { connectionsAPI as api, type SshKey, type StationConnections, textOf } from './api/connections';
import { auth } from './auth';
import { nodeNoun } from './settings';
import { toURL } from '@node/basePath';
import { requestTabId } from '@node/certificateRequests';
import type { CertificatesSection, SectionContext } from '@node/pages/certificates';
import { errorMessage, formatTimestamp, whileSaving } from '@node/ui';
import { html, nothing, repeat, type TemplateResult } from '@node/view';


/**
 * The keys a node that dials signs in with over SSH, among what it presents
 * on the page of its client identities.
 *
 * They are no certificates and the certificate store does not keep them, and
 * they stand here all the same: a key a node signs in with at an ssh://
 * server is who it is there, as a TLS identity is who it is at a wss:// one,
 * and somebody who looks for "who is this node to its back ends" looks in one
 * place. One of them is the node's own - made when an ssh:// connection first
 * needs it, signed in with by every connection that names no key, and changed
 * by its back end over OCPP. The others are made here: a key for one server
 * and not the next, or the key that follows another at a time its server was
 * told - which the Connections page then ticks beside the one in use.
 *
 * What is shown is the public half, the line for a server's authorized_keys.
 * The private half never leaves its file.
 *
 * The keys stand with what the node presents; one is made where every key of
 * the page is, on its "New key" tab - and the page turns to the key once it
 * is made, for its line is what is wanted next.
 *
 * Outside pages/, where every file is a page and is held to what a page is:
 * this is a part of one, drawn where the page has room for it. And it has no
 * form: one field, sent by its button.
 */
export function sshKeysSection(context: SectionContext): CertificatesSection {

    /** What the node dials with, as it last said - null where this person may not see the connections. */
    let state: StationConnections | null = null;

    /** Making a key is what making one in the certificate store is: who the node is, not where it dials. */
    const mayMake = auth.can('certificates', 'run') || auth.can('certificates', 'edit');

    const place = (selector: string) => context.page.querySelector<HTMLElement>(selector);

    function say(selector: string, text: string): void {
        const where = place(selector);
        if (where !== null)
            where.textContent = text;
    }

    /** The connections that sign in with a key: by name, and the node's own by naming none. */
    function usedBy(key: SshKey): string[] {
        return (state?.connections ?? []).
                   filter(connection => connection.transport === 'ssh' &&
                                        ((connection.identities ?? []).some(one => one.id === key.id) ||
                                         (key.own && (connection.identities ?? []).length === 0))).
                   map   (connection => textOf(connection.description) ?? '');
    }

    function copy(key: SshKey): void {

        if (key.publicKey === null)
            return;

        void navigator.clipboard?.writeText(key.publicKey).then(
            () => say(`[data-ssh-key-note="${key.id}"]`, 'Copied.'),
            () => say(`[data-ssh-key-note="${key.id}"]`, 'Could not be copied: select the line and copy it by hand.')
        );

    }

    async function make(): Promise<void> {

        const label = context.page.querySelector<HTMLInputElement>('#ssh-key-label');
        const note  = place('#ssh-key-note');

        say('#ssh-key-note',  '');
        say('#ssh-key-error', '');

        try
        {
            const made = await whileSaving(context.page, note, () => api.sshKeys.make(label?.value.trim() ?? ''));

            state = made.connections;

            if (label !== null)
                label.value = '';

            // To the key, among the others: its line is what is wanted next.
            context.show('usage');
            context.draw();

            say(`[data-ssh-key-note="${made.id}"]`, 'Made. Its line goes into the authorized_keys of the server it is to sign in at.');
            place(`[data-ssh-key="${made.id}"]`)?.scrollIntoView?.({ block: 'nearest' });
        }
        catch (problem)
        {
            say('#ssh-key-error', errorMessage(problem));
        }

    }

    async function remove(key: SshKey): Promise<void> {

        say(`[data-ssh-key-error="${key.id}"]`, '');

        // Whatever server was given its line turns the node away afterwards:
        // asked, as for everything that cannot be undone.
        if (!confirm(`Remove the SSH key "${key.label}"? A server that was given its line does not let this ${nodeNoun()} in with it any more.`))
            return;

        try
        {
            state = await whileSaving(context.page, place(`[data-ssh-key-note="${key.id}"]`), () => api.sshKeys.remove(key.id));
            context.draw();
        }
        catch (problem)
        {
            say(`[data-ssh-key-error="${key.id}"]`, errorMessage(problem));
        }

    }

    function card(key: SshKey): TemplateResult {

        const used = usedBy(key);

        return html`
            <div class="card" data-ssh-key="${key.id}">

                <div class="key-head">
                    <strong>${key.label}</strong>
                    ${key.own ? html`<span class="chip on">this ${nodeNoun()}'s own</span>` : nothing}
                    ${used.length > 0
                          ? html`<span class="chip on" title="${used.join(', ')}">${used.length === 1 ? `signs in at ${used[0]}` : `signs in at ${used.length} connections`}</span>`
                          : html`<span class="chip">signs in nowhere</span>`}
                </div>

                ${key.made && key.publicKey !== null ? html`
                    <textarea class="mono" rows="3" readonly data-ssh-public-key="${key.id}" .defaultValue=${key.publicKey}></textarea>
                    <dl class="kv">
                        <dt>Fingerprint</dt> <dd><code>${key.fingerprint}</code></dd>
                        ${key.createdAt !== null ? html`<dt>Made</dt> <dd>${formatTimestamp(key.createdAt)}</dd>` : nothing}
                        <dt>Kept in</dt>     <dd><code>${key.path}</code></dd>
                    </dl>
                ` : html`
                    <p class="hint">
                        Not made yet: it is, the first time an <code>ssh://</code> connection that names no key is written
                        down, tested or dialled.
                    </p>
                `}

                <div class="form-actions">
                    ${key.made ? html`
                        <button type="button" class="btn small" data-copy-ssh-key="${key.id}" @click=${() => copy(key)}>Copy the line</button>
                    ` : nothing}
                    ${key.own ? nothing : html`
                        <button type="button" class="btn small danger" data-remove-ssh-key="${key.id}"
                                title="${used.length > 0 ? 'A connection signs in with it: choose another key for it first' : 'Take the key away for good'}"
                                ?disabled=${!mayMake || used.length > 0}
                                @click=${() => void remove(key)}>
                            Remove
                        </button>
                    `}
                    <span class="form-notice" role="status" data-ssh-key-note="${key.id}"></span>
                    <span class="form-error"  role="alert"  data-ssh-key-error="${key.id}"></span>
                </div>

            </div>
        `;

    }

    return {

        below:  'presents',

        // Somebody who may not see the connections is shown no keys, and
        // the rest of the page all the same.
        load:   async () => {
                    try         { state = await api.connections.get(); }
                    catch (_)   { state = null; }
                },

        draw:   () => {

                    if (state === null)
                        return html``;

                    const keys = state.sshKeys ?? [];

                    return html`
                        <section class="card" id="ssh-keys">

                            <h3><i class="fa-solid fa-terminal"></i> SSH keys</h3>

                            <p class="hint">
                                What this ${nodeNoun()} signs in with where it dials over <code>ssh://</code>: its own key,
                                which every connection that names none signs in with, and the ones made here - for one
                                server and not the next, or to follow another at a time its server was told. The line of a
                                key goes into the <code>authorized_keys</code> of the server; it is no secret, and the
                                private half never leaves this ${nodeNoun()}. Which connection signs in with which, and
                                from when, is ticked on the <a href="${toURL('/configuration/connections')}">Connections</a>
                                page${mayMake ? html`, and a key is made on the tab
                                <a href="#" @click=${(event: Event) => { event.preventDefault(); context.show(requestTabId); }}>New key</a>` : nothing}.
                            </p>

                            <div class="cards">${repeat(keys, key => key.id, key => card(key))}</div>

                        </section>
                    `;

                },

        makesKeys:  () => mayMake && state !== null,

        newKey: () => html`
                    <section class="card" id="make-ssh-key">

                        <h3><i class="fa-solid fa-terminal"></i> A new SSH key</h3>

                        <p class="hint">
                            A key this ${nodeNoun()} signs in with where it dials over <code>ssh://</code>, beside its own:
                            made here, and its private half never leaves. No certificate comes back for it - its line
                            goes into the <code>authorized_keys</code> of the server, and a connection is told to sign
                            in with it on the <a href="${toURL('/configuration/connections')}">Connections</a> page.
                        </p>

                        <div class="form-stack">
                            <label>What it is for
                                <input type="text" id="ssh-key-label" maxlength="${state?.maxDescriptionLength ?? 120}"
                                       placeholder="CSMS over SSH, from January"
                                       @keydown=${(event: KeyboardEvent) => { if (event.key === 'Enter') { event.preventDefault(); void make(); } }} />
                            </label>
                        </div>
                        <div class="form-actions">
                            <button type="button" class="btn primary" id="make-an-ssh-key" @click=${() => void make()}>
                                <i class="fa-solid fa-key"></i> Make an SSH key
                            </button>
                            <span class="form-notice" role="status" id="ssh-key-note"></span>
                            <span class="form-error"  role="alert"  id="ssh-key-error"></span>
                        </div>

                    </section>
                `

    };

}
