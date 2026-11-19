import { commissioningAPI as api, type CommissioningConfiguration } from '../api/commissioning';
import { auth } from '../auth';
import { nodeNoun } from '../settings';
import { toURL } from '@node/basePath';
import { must } from '@node/html';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { errorMessage, whileSaving } from '@node/ui';
import { anyFormTypedSinceDrawn, unsaved } from '@node/unsaved';
import { html, nothing, render } from '@node/view';

/**
 * Commissioning over SSH: a tool speaking OCPP to this node - a charging
 * station, a local controller, a gateway or a CSMS - on the node's own SSH
 * server, as an account that may run it.
 *
 * The tool connects under its account's name and never as the CSMS, so the
 * node's line up keeps its CSMS: what the node has to say goes there, and the
 * tool gets the answers to its own requests. Switched on and off at once; off
 * lets go of every tool connected.
 */
export const commissioningPage: Page = {

    title: 'Commissioning',

    render({ root }) {

        const content = shell(root, {
            active:    '/configuration/commissioning',
            title:     'Commissioning',
            subtitle:  `A tool speaking OCPP to this ${nodeNoun()} over its SSH server.`,
            actions:   reloadButton(() => reload())
        });

        render(content, html`<div class="loading">Loading ...</div>`);

        const mayChange = auth.can('commissioning', 'edit');

        let cancelled = false;
        let current: CommissioningConfiguration | null = null;


        function draw(): void {

            if (current === null)
                return;

            const commissioning = current;
            const where         = commissioning.url === null
                                      ? null
                                      : commissioning.url.replace(/^ssh:\/\//, 'ssh://<account>@');

            render(content, html`

                ${mayChange ? nothing : html`
                    <div class="notice">${mayButNot('look at commissioning', 'switch it on or off')}</div>
                `}

                <div class="cards">

                    <section class="card">

                        <h2><i class="fa-solid fa-screwdriver-wrench"></i> OCPP over SSH</h2>

                        <p class="hint" id="commissioning-state">
                            ${commissioning.served
                                  ? html`<strong>Served</strong>: a commissioning tool may speak OCPP to this ${nodeNoun()}.`
                                  : html`<strong>Not served</strong>: no tool may speak OCPP to this ${nodeNoun()}.`}
                        </p>

                        ${commissioning.served && !commissioning.sshServer ? html`
                            <div class="error-box" id="no-ssh-server">
                                The SSH server is off, so no tool can reach this ${nodeNoun()}. Switch it on under
                                <a href="${toURL('/configuration/ssh')}">SSH</a>.
                            </div>
                        ` : nothing}

                        <form id="commissioning-form" class="form-stack" @submit=${(event: SubmitEvent) => { event.preventDefault(); void save(); }}>

                            <label class="checkbox">
                                <input type="checkbox" name="enabled" ?checked=${commissioning.enabled} ?disabled=${!mayChange} />
                                Serve commissioning over SSH
                            </label>

                            <div class="form-actions">
                                <button type="submit" class="btn primary" ?disabled=${!mayChange}>Save</button>
                                <span id="form-note"  class="form-notice" role="status"></span>
                                <span id="form-error" class="form-error"  role="alert"></span>
                            </div>

                            <span class="hint">
                                Saved to ${commissioning.file}, and served or not from that moment, without a restart.
                                Switched off, every tool connected is let go of.
                            </span>

                        </form>

                    </section>

                    <section class="card">

                        <h2><i class="fa-solid fa-plug-circle-bolt"></i> Where a tool connects</h2>

                        <dl class="kv" id="commissioning-facts">
                            <dt>URL</dt>           <dd>${where ?? 'none - the SSH server is off'}</dd>
                            <dt>Subsystem</dt>     <dd>${commissioning.subsystem}</dd>
                            <dt>OCPP</dt>          <dd>${commissioning.subprotocols.join(', ')}</dd>
                            <dt>OCPP node</dt>     <dd>${commissioning.node}</dd>
                            <dt>Host key</dt>      <dd>${commissioning.hostKey ?? 'not known yet'}</dd>
                            <dt>Who may</dt>       <dd>an account that may ${commissioning.permission}, with its own SSH key</dd>
                        </dl>

                        <p class="hint">
                            The tool is a networking node of its own, named after its account, and never the CSMS:
                            the ${nodeNoun()}'s own connections keep their CSMS, and the tool is answered what it asks.
                        </p>

                        <div class="table-scroll">
                            <table class="records" id="commissioning-tools">
                                <thead>
                                    <tr><th>Connected now</th></tr>
                                </thead>
                                <tbody>
                                    ${commissioning.tools.length === 0
                                          ? html`<tr><td>No tool is connected.</td></tr>`
                                          : commissioning.tools.map(tool => html`<tr><td>${tool}</td></tr>`)}
                                </tbody>
                            </table>
                        </div>

                    </section>

                </div>

            `);

        }

        /**
         * The switch saved, and drawn again as the node took it.
         */
        async function save(): Promise<void> {

            const form   = must<HTMLFormElement>(content, '#commissioning-form');
            const note   = must<HTMLElement>(content, '#form-note');
            const error  = must<HTMLElement>(content, '#form-error');

            note.textContent   = '';
            error.textContent  = '';

            // Read before the page is held still: a disabled field is left out
            // of a FormData.
            const enabled = new FormData(form).get('enabled') !== null;

            try
            {
                current = await whileSaving(content, note, () => api.commissioning.save({ enabled }));

                if (cancelled)
                    return;

                draw();

                form.reset();

                note.textContent = current.served ? 'Saved. Commissioning over SSH is served.' : 'Saved. Commissioning over SSH is not served.';
            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }

        async function load(): Promise<void> {

            try
            {
                const loaded = await api.commissioning.get();

                if (cancelled)
                    return;

                current = loaded;
                draw();
            }
            catch (problem)
            {
                if (!cancelled)
                    render(content, html`
                        <div class="error-box">Commissioning could not be loaded: ${errorMessage(problem)}</div>
                    `);
            }

        }

        /**
         * Loaded anew - Reload - is what the node has, the form too, which a
         * draw on its own would leave as typed.
         */
        async function reload(): Promise<void> {
            await load();
            if (!cancelled)
                content.querySelectorAll('form').forEach(form => form.reset());
        }

        const release = unsaved.heldBy(() => anyFormTypedSinceDrawn(content));

        void load();

        return () => { cancelled = true; release(); };

    }

};
