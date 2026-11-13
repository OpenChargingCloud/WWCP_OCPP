import { connectionsAPI as api, type LoginToSave, type StationConnections, type StationLogin } from '../api/connections';
import { auth } from '../auth';
import { connectionsResource, nodeNoun, nodeNounInFull } from '../settings';
import { must } from '@node/html';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { errorMessage, field, formatTimestamp, whileSaving } from '@node/ui';
import { anyFormTypedSinceDrawn, unsaved } from '@node/unsaved';
import { html, nothing, render, repeat, type TemplateResult } from '@node/view';

/**
 * The credentials this station proves itself with.
 *
 * Several, because a station in a fleet normally has more than one: a login for
 * the back end and another for the local controller in the same cabinet, and
 * the two are not interchangeable. Which is which is written down rather than
 * guessed at from a URL a year later - that is what the description is for, and
 * why it is the one field that cannot be left empty.
 *
 * A secret goes in and never comes back out. The station is told a password, it
 * writes it where only its owner may read it, and what this page is handed
 * afterwards is whether one is set. So the secret field on an existing record
 * is empty and means "leave it alone" - which is what makes it possible to
 * correct a description without having to find the password again.
 */
export const authenticationPage: Page = {

    title: 'Authentication',

    render({ root }) {

        const content = shell(root, {
            active:    '/configuration/authentication',
            title:     'Authentication',
            subtitle:  `How this ${nodeNounInFull()} proves who it is when it dials a back end.`,
            actions:   reloadButton(() => reload())
        });

        render(content, html`<div class="loading">Loading ...</div>`);

        const mayManage = auth.can(connectionsResource(), 'edit');

        let cancelled = false;
        let store: StationConnections | null = null;

        /** Which records have their details open, so a redraw does not close them. */
        const opened = new Set<string>();

        /**
         * The kind picked in a form and not saved yet, by the form - "add", or
         * the id of the record. Which fields are shown follows it, and it
         * follows it straight away rather than after a save: somebody who
         * picks TOTP and sees no shared secret field concludes the page is
         * broken.
         */
        const picked = new Map<string, StationLogin['kind']>();


        function draw(): void {

            if (store === null)
                return;

            const state = store;

            render(content, html`

                ${mayManage ? nothing : html`
                    <div class="notice">${mayButNot('look at the credentials', 'change them')}</div>
                `}

                <section class="card">

                    <h2><i class="fa-solid fa-user-plus"></i> Write down a set of credentials</h2>

                    <form id="add-form" class="form-stack" data-kind-form="add" @submit=${add}>

                        <label>What they are for
                            <input type="text" name="description" maxlength="${state.maxDescriptionLength}"
                                   placeholder="CSMS login" ?disabled=${!mayManage} />
                            <span class="hint">
                                The one field worth taking seriously. Six logins on a page a year from now are
                                told apart by this and nothing else.
                            </span>
                        </label>

                        <label>How
                            <select name="kind" data-kind ?disabled=${!mayManage}
                                    @change=${(event: Event) => pick('add', (event.target as HTMLSelectElement).value)}>
                                <option value="basic">HTTP Basic - a name and a password</option>
                                <option value="totp">HTTP TOTP - a secret that never travels</option>
                            </select>
                        </label>

                        <label>Login
                            <input type="text" name="login" placeholder="cs001"
                                   ?disabled=${!mayManage} />
                            <span class="hint">
                                A role rather than a person: it is what the other end calls this ${nodeNoun()}.
                            </span>
                        </label>

                        ${secretField('add', null)}

                        ${totpFields('add', null, state)}

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayManage}>
                                Write them down
                            </button>
                            <span id="add-note"  class="form-notice" role="status"></span>
                            <span id="add-error" class="form-error"  role="alert"></span>
                        </div>

                    </form>

                </section>

                <section class="card">

                    <h2><i class="fa-solid fa-list"></i> What is configured</h2>

                    <p class="hint">
                        Newest first. Written to ${state.directory}, the secrets readable by nobody else.
                    </p>

                    ${state.authentications.length === 0
                          ? html`
                                <p class="hint">
                                    Nothing yet. This ${nodeNoun()} can only dial a back end that asks it for
                                    nothing, or one it shows a client certificate to.
                                </p>
                            `
                          : html`<div class="cards">${repeat(state.authentications, entry => entry.id, entry => entryCard(entry, state))}</div>`}

                </section>

            `);

        }

        /** The kind a form shows the fields of: the one picked in it, or the one the station has. */
        function kindIn(id: string, entry: StationLogin | null): StationLogin['kind'] {
            return picked.get(id) ?? entry?.kind ?? 'basic';
        }

        function pick(id: string, kind: string): void {
            picked.set(id, kind === 'totp' ? 'totp' : 'basic');
            draw();
        }

        /**
         * The secret, which is write-only.
         *
         * On an existing record it is empty and stays empty: this station
         * cannot show it back, and pretending otherwise with a row of dots
         * would suggest that it could.
         */
        function secretField(id: string, entry: StationLogin | null): TemplateResult {

            const isTOTP = kindIn(id, entry) === 'totp';

            return html`
                <label data-secret-label="${id}">
                    <span data-secret-title="${id}">${isTOTP ? 'Shared secret' : 'Password'}</span>
                    <input type="password" name="secret" autocomplete="new-password"
                           placeholder="${entry === null ? '' : 'unchanged'}"
                           ?disabled=${!mayManage} />
                    <span class="hint">
                        ${entry === null
                              ? html`
                                    It is written to this ${nodeNoun()} and never read back out - not by this page
                                    and not by anything else. Keep your own copy.
                                `
                              : entry.hasSecret
                                    ? html`
                                          One is set. Leave this empty to keep it; type here only to replace it.
                                          It cannot be shown back.
                                      `
                                    : html`
                                          <strong>None is set</strong>, so nothing using these credentials can
                                          connect. Type one here.
                                      `}
                    </span>
                </label>
            `;

        }

        /** What a time-based password is made of - shown only when that is what it is. */
        function totpFields(id: string, entry: StationLogin | null, state: StationConnections): TemplateResult {

            const on = kindIn(id, entry) === 'totp';

            return html`
                <div data-totp="${id}" class="form-stack" ?hidden=${!on}>

                    <label class="checkbox">
                        <input type="checkbox" name="tlsChannelBinding"
                               ?checked=${entry?.tlsChannelBinding ?? false}
                               ?disabled=${!mayManage} />
                        Bind the password to the TLS session
                    </label>

                    <span class="hint">
                        A bound password is no use to anybody who copies it off one connection and tries it on
                        another. It is bound on a connection that names HTTP/3 and nothing else: over HTTP/1.1
                        and HTTP/2 this ${nodeNoun()}'s TLS exports no keying material to bind one with, so any other
                        connection whose credentials ask for it is not dialled at all, rather than sent less
                        than it was told. The Connections page says so where it applies. The back end has to
                        verify it as bound, which is not every back end's to do.
                    </span>

                    <div class="form-row">

                        <label>Good for
                            <input type="number" name="validitySeconds" min="5" max="3600"
                                   placeholder="${state.totpDefaults.validitySeconds}"
                                   value="${entry?.validitySeconds ?? ''}"
                                   ?disabled=${!mayManage} />
                            <span class="hint">seconds</span>
                        </label>

                        <label>Length
                            <input type="number" name="length" min="4" max="64"
                                   placeholder="${state.totpDefaults.length}"
                                   value="${entry?.length ?? ''}"
                                   ?disabled=${!mayManage} />
                            <span class="hint">characters</span>
                        </label>

                        <label>HMAC
                            <select name="hashAlgorithm" ?disabled=${!mayManage}>
                                ${['SHA256', 'SHA384', 'SHA512'].map(one => html`
                                    <option value="${one}" ?selected=${one === (entry?.hashAlgorithm ?? 'SHA256')}>
                                        ${one}
                                    </option>
                                `)}
                            </select>
                        </label>

                    </div>

                    <label>Alphabet
                        <input type="text" name="alphabet" class="mono"
                               placeholder="${state.totpDefaults.alphabet}"
                               value="${entry?.alphabet ?? ''}"
                               ?disabled=${!mayManage} />
                        <span class="hint">
                            Left empty, the default. Both ends have to agree on every one of these, so
                            changing any of them here means changing them at the back end too.
                        </span>
                    </label>

                </div>
            `;

        }

        function entryCard(entry: StationLogin, state: StationConnections): TemplateResult {

            const usedBy = state.connections.filter(one => one.authenticationId === entry.id);

            return html`
                <div class="card">

                    <div class="key-head">
                        <strong>${entry.description}</strong>
                        <span class="chip">${entry.kind === 'basic' ? 'HTTP Basic' : 'HTTP TOTP'}</span>
                        ${entry.hasSecret ? nothing : html`<span class="chip warn">no secret</span>`}
                    </div>

                    <dl class="kv">
                        <dt>Login</dt>   <dd><code>${entry.login}</code></dd>
                        <dt>Written</dt> <dd>${formatTimestamp(entry.createdAt)}</dd>
                        <dt>Used by</dt> <dd>
                            ${usedBy.length === 0
                                  ? html`<span class="hint">nothing yet</span>`
                                  : usedBy.map(one => one.description).join(', ')}
                        </dd>
                    </dl>

                    <details ?open=${opened.has(entry.id)} data-details="${entry.id}"
                             @toggle=${(event: Event) => toggled(entry.id, (event.target as HTMLDetailsElement).open)}>

                        <summary>Change them</summary>

                        <form class="form-stack" data-id="${entry.id}" data-edit="${entry.id}" data-kind-form="${entry.id}"
                              @submit=${(event: SubmitEvent) => { event.preventDefault(); void save(entry.id, event.currentTarget as HTMLFormElement); }}>

                            <label>What they are for
                                <input type="text" name="description" maxlength="${state.maxDescriptionLength}"
                                       value="${entry.description}" ?disabled=${!mayManage} />
                            </label>

                            <label>How
                                <select name="kind" data-kind ?disabled=${!mayManage}
                                        @change=${(event: Event) => pick(entry.id, (event.target as HTMLSelectElement).value)}>
                                    <option value="basic" ?selected=${entry.kind === 'basic'}>
                                        HTTP Basic - a name and a password
                                    </option>
                                    <option value="totp"  ?selected=${entry.kind === 'totp'}>
                                        HTTP TOTP - a secret that never travels
                                    </option>
                                </select>
                            </label>

                            <label>Login
                                <input type="text" name="login" value="${entry.login}"
                                       ?disabled=${!mayManage} />
                            </label>

                            ${secretField(entry.id, entry)}

                            ${totpFields(entry.id, entry, state)}

                            <div class="form-actions">
                                <button type="submit" class="btn primary" ?disabled=${!mayManage}>
                                    Save
                                </button>
                                <button type="button" class="btn small" data-remove="${entry.id}"
                                        ?disabled=${!mayManage}
                                        @click=${() => void remove(entry.id)}>
                                    Remove
                                </button>
                                <span class="form-notice" data-note="${entry.id}"  role="status"></span>
                                <span class="form-error"  data-error="${entry.id}" role="alert"></span>
                            </div>

                            ${usedBy.length > 0 ? html`
                                <span class="hint">
                                    ${usedBy.length === 1 ? 'One connection uses' : `${usedBy.length} connections use`}
                                    these, so they cannot be removed until those point somewhere else.
                                </span>
                            ` : nothing}

                        </form>

                    </details>

                </div>
            `;

        }

        /** A details element that was open stays open across a redraw. */
        function toggled(id: string, open: boolean): void {
            if (open)
                opened.add(id);
            else
                opened.delete(id);
        }

        /** What a form says, in the shape the station takes. */
        function readFrom(form: HTMLFormElement, id?: string): LoginToSave {

            const kind    = field(form, 'kind') === 'totp' ? 'totp' : 'basic';
            const number  = (name: string): number | undefined => {
                const written = field(form, name);
                return written === '' ? undefined : Number(written);
            };

            const written: LoginToSave = {
                id,
                description:  field(form, 'description'),
                kind,
                login:        field(form, 'login'),
                // Not trimmed: a password may legitimately start or end with a
                // space, and silently taking it off would leave somebody with
                // credentials that are refused for no visible reason.
                secret:       field(form, 'secret', false)
            };

            if (kind === 'totp') {
                written.validitySeconds    = number('validitySeconds');
                written.length             = number('length');
                written.alphabet           = field(form, 'alphabet');
                written.hashAlgorithm      = field(form, 'hashAlgorithm');
                written.tlsChannelBinding  = form.querySelector<HTMLInputElement>('[name="tlsChannelBinding"]')?.checked ?? false;
            }

            return written;

        }


        async function add(event: SubmitEvent): Promise<void> {

            event.preventDefault();

            const form = event.currentTarget as HTMLFormElement;
            const note = must<HTMLElement>(content, '#add-note');

            note.textContent = '';
            must<HTMLElement>(content, '#add-error').textContent = '';

            const written = readFrom(form);

            try
            {
                const made = await whileSaving(content, note, () => api.authentications.add(written));

                if (cancelled)
                    return;

                store = made.connections;
                picked.delete('add');

                draw();

                // Written down, so no longer typed: the form is empty again.
                form.reset();

                note.textContent = 'Written down.';
            }
            catch (problem)
            {
                if (!cancelled)
                    must<HTMLElement>(content, '#add-error').textContent = errorMessage(problem);
            }

        }

        async function save(id: string, form: HTMLFormElement): Promise<void> {

            const note  = must<HTMLElement>(content, `[data-note="${id}"]`);
            const error = must<HTMLElement>(content, `[data-error="${id}"]`);

            note. textContent = '';
            error.textContent = '';

            const written = readFrom(form, id);

            try
            {
                store = await whileSaving(content, note, () => api.authentications.update(written));

                if (cancelled)
                    return;

                // Stays open, because somebody who just saved is frequently
                // about to change something else here.
                opened.add(id);
                picked.delete(id);

                draw();

                // A draw leaves a form as it is typed into; the one saved goes
                // back to what it says now - the station's answer, and no
                // secret, which is never shown back.
                form.reset();

                note.textContent = 'Saved.';
            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }

        async function remove(id: string): Promise<void> {

            const error = must<HTMLElement>(content, `[data-error="${id}"]`);
            const note  = must<HTMLElement>(content, `[data-note="${id}"]`);

            error.textContent = '';

            try
            {
                store = await whileSaving(content, note, () => api.authentications.remove(id));

                if (cancelled)
                    return;

                opened.delete(id);
                picked.delete(id);

                draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }

        /**
         * The page as the station has it now, drawn over the page as it is -
         * what is typed into its forms kept, as a draw keeps it. Reload empties
         * them itself.
         */
        async function load(): Promise<void> {

            try
            {
                const loaded = await api.authentications.get();

                if (cancelled)
                    return;

                store = loaded;
                draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    render(content, html`
                        <div class="error-box">The credentials could not be loaded: ${errorMessage(problem)}</div>
                    `);
            }

        }

        /**
         * Loaded anew - Reload - is what the station has, the forms too, which
         * a draw on its own would leave as typed: the kinds picked and not
         * saved go with them.
         */
        async function reload(): Promise<void> {
            picked.clear();
            await load();
            if (!cancelled)
                content.querySelectorAll('form').forEach(form => form.reset());
        }

        // A half-typed password is work like any other, and losing one is worse
        // than losing most: it cannot be read back off the page to try again.
        const release = unsaved.heldBy(() => anyFormTypedSinceDrawn(content));

        void load();

        return () => { cancelled = true; release(); };

    }

};
