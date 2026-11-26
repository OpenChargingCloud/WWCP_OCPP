import { deviceModelAPI, type DeviceModelPage } from '../api/deviceModel';
import { auth } from '../auth';
import { must } from '@node/html';
import { collapseAll, expandAll, filterTree, jsonTree } from '@node/jsonTree';
import type { Page } from '@node/router';
import { shell, type MenuEntry } from '@node/shell';
import { stationsResource } from '@node/connecting/settings';
import { errorMessage } from '@node/ui';
import { html, render } from '@node/view';


/**
 * What a folded row says of a variable or a key: what it says now - or that
 * it is hidden -, so that a value is read without unfolding it; nothing of
 * its own for a component or a version, which say how many entries they have.
 */
export function summaryOf(Value: object): string | null {

    const entry = Value as Record<string, unknown>;

    if (entry.hidden === true)
        return '= hidden';

    if ('error' in entry)
        return `= ? (${String(entry.error)})`;

    if ('value' in entry)
        return entry.value === null || entry.value === undefined
                   ? '= null'
                   : `= ${JSON.stringify(entry.value)}`;

    return null;

}

/**
 * The entry of the menu: for whoever may read the OCPP log - every
 * GetVariables and its answer is in it, so whoever may read it may read what
 * they would say (Achim 2026-11-26).
 */
export const deviceModelMenuEntry: MenuEntry = {
    path:        '/ocpp-device-model',
    label:       'OCPP Device Model',
    icon:        'fa-sitemap',
    permission:  () => auth.can(stationsResource(), 'edit')
};

/**
 * The Device Model of this node as a tree to browse: its components of OCPP
 * 2.1, each with its variables and what they say now, and its configuration
 * keys of OCPP 1.6 - every level folded at first, unfolded a click at a time,
 * and searched by names and values. A way to see what a change of the
 * configuration came to in OCPP, before a CSMS asks.
 *
 * A value OCPP keeps WriteOnly - a password, a key - is hidden from all but a
 * systemadmin, who is hidden nothing (Achim 2026-11-26).
 */
export const deviceModelPage: Page = {

    title: 'OCPP Device Model',

    render({ root }) {

        let cancelled = false;

        const content = shell(root, {
            active:    '/ocpp-device-model',
            title:     'OCPP Device Model',
            subtitle:  'What this node shows OCPP of itself, as it is now: its components and variables, and its configuration keys.',
            actions:   html`
                <button type="button" id="reload" class="btn small" title="Read the Device Model again"
                        @click=${() => void load()}><i class="fa-solid fa-rotate"></i> Reload</button>
            `
        });

        render(content, html`

            <div class="log-filters">

                <label class="filter-search">
                    <i class="fa-solid fa-magnifying-glass"></i>
                    <input type="search" id="search" placeholder="Search the names and the values ..." autocomplete="off" />
                </label>

                <button type="button" id="expand"   class="btn small">Unfold all</button>
                <button type="button" id="collapse" class="btn small">Fold all</button>

            </div>

            <p id="write-only" class="hint"></p>

            <section class="card">
                <div id="tree"><p class="hint">Reading the Device Model ...</p></div>
            </section>

            <p id="matches" class="small muted"></p>

        `);

        const holder    = must<HTMLElement>      (content, '#tree');
        const search    = must<HTMLInputElement> (content, '#search');
        const writeOnly = must<HTMLElement>      (content, '#write-only');
        const matches   = must<HTMLElement>      (content, '#matches');

        let tree: HTMLElement | null = null;

        function filter(): void {

            if (tree === null)
                return;

            const needle = search.value.trim();
            const found  = filterTree(tree, needle);

            matches.textContent = needle === ''        ? ''
                                : found === 0          ? 'Nothing found.'
                                : `${found} ${found === 1 ? 'row' : 'rows'} found.`;

        }

        function show(Answer: DeviceModelPage): void {

            writeOnly.textContent = Answer.writeOnlyShown
                                        ? 'You are a systemadmin: the values OCPP keeps write-only - passwords, keys - are shown too.'
                                        : 'The values OCPP keeps write-only - passwords, keys - are hidden; a systemadmin sees them.';

            if (Object.keys(Answer.deviceModel).length === 0) {
                tree = null;
                holder.replaceChildren(Object.assign(document.createElement('p'), {
                                           className:    'hint',
                                           textContent:  'This node has no OCPP node yet, and so no Device Model.'
                                       }));
                return;
            }

            tree = jsonTree(Answer.deviceModel, { summary: summaryOf });
            holder.replaceChildren(tree);

            filter();

        }

        async function load(): Promise<void> {

            try
            {
                const answer = await deviceModelAPI.get();
                if (!cancelled)
                    show(answer);
            }
            catch (problem)
            {
                if (!cancelled)
                    holder.replaceChildren(Object.assign(document.createElement('p'), {
                                               className:    'form-error',
                                               textContent:  errorMessage(problem)
                                           }));
            }

        }

        search.addEventListener('input', () => filter());

        // Both show everything again: a search hides what it did not find.
        must<HTMLButtonElement>(content, '#expand')  .addEventListener('click', () => { if (tree !== null) { search.value = ''; filter(); expandAll(tree);   } });
        must<HTMLButtonElement>(content, '#collapse').addEventListener('click', () => { if (tree !== null) { search.value = ''; filter(); collapseAll(tree); } });

        void load();

        return () => { cancelled = true; };

    }

};
