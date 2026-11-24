import { isError, type OCPPMessage } from '../api/ocppLog';
import { auth } from '../auth';
import { ocppLog } from '../ocppLog/store';
import { must } from '@node/html';
import type { Page } from '@node/router';
import { shell, type MenuEntry } from '@node/shell';
import { stationsResource } from '@node/connecting/settings';
import { followAcrossTheCache } from '@node/start';
import { formatTime, formatTimestamp } from '@node/ui';
import { html, render } from '@node/view';

/** An element of the given kind and class, with the given text - as text, which needs no escaping. */
function element<K extends keyof HTMLElementTagNameMap>(Tag: K, Class = '', Text?: string): HTMLElementTagNameMap[K] {
    const made = document.createElement(Tag);
    if (Class !== '')
        made.className = Class;
    if (Text !== undefined)
        made.textContent = Text;
    return made;
}

/** A Font Awesome icon. */
function icon(Name: string): HTMLElement {
    return element('i', `fa-solid ${Name}`);
}

/** What the first element of the array says, in words. */
const kindNames: Record<OCPPMessage['kind'], string> = {
    request:        'CALL',
    response:       'RESULT',
    requestError:   'ERROR',
    responseError:  'RESULT ERROR',
    send:           'SEND'
};

/**
 * The action as the line shows it: a request's, and an answer's that of its
 * request - or a question mark where this node never saw the request.
 */
function actionOf(Message: OCPPMessage): string {
    return Message.action ?? '?';
}

/** The array as it went over the wire, on one line. */
export function compactJSON(Message: OCPPMessage): string {
    return JSON.stringify(Message.message);
}

/** The array with its payload laid out, for reading. */
export function prettyJSON(Message: OCPPMessage): string {
    return JSON.stringify(Message.message, null, 2);
}

/**
 * One line, made once - and made anew only when it is unfolded or folded
 * again, in place of the one it was.
 *
 * Made as elements, not drawn by view.ts, as a hub's traffic is: a line is
 * never drawn again, and what a message says goes in as text, which needs no
 * escaping - a charging station writes most of what is shown here.
 */
export function lineElement(Message: OCPPMessage, Open = false): HTMLElement {

    const error = isError(Message);

    const line = element('div', `line ocpp-message ${Message.kind} ${error ? 'failed' : ''} ${Open ? 'open' : ''}`);
    line.dataset.id = String(Message.id);

    const head = element('div', 'ocpp-head');
    head.setAttribute('role', 'button');
    head.tabIndex = 0;
    head.setAttribute('aria-expanded', String(Open));

    const time = element('time', '', formatTime(Message.timestamp));
    time.dateTime = Message.timestamp;
    time.title    = formatTimestamp(Message.timestamp);

    const direction = element('span', `chip dir ${Message.direction}`);
    direction.title = Message.direction === 'in' ? 'Received by this node' : 'Sent by this node';
    direction.append(icon(Message.direction === 'in' ? 'fa-arrow-left-long' : 'fa-arrow-right-long'));

    const peer = element('span', 'peer', Message.peer ?? '-');
    peer.title = Message.direction === 'in' ? `From ${Message.peer ?? 'an unknown node'}` : `To ${Message.peer ?? 'an unknown node'}`;

    const kind = element('span', `chip kind ${Message.kind}`, kindNames[Message.kind]);
    kind.title = `OCPP ${Message.ocppVersion}, message ${Message.messageId}`;

    const action = element('span', 'action', actionOf(Message));

    if (Message.action === null)
        action.title = 'The request this answers was not seen by this node';

    head.append(time,
                direction,
                peer,
                kind,
                action,
                element('code', 'json', compactJSON(Message)));

    line.append(head);

    if (Open)
        line.append(detailElement(Message));

    return line;

}

/** The array laid out, and what the line had no room for. */
function detailElement(Message: OCPPMessage): HTMLElement {

    const detail = element('div', 'ocpp-detail');

    const facts = element('div', 'small muted');
    facts.textContent = [
        formatTimestamp(Message.timestamp),
        `OCPP ${Message.ocppVersion}`,
        `message ${Message.messageId}`,
        Message.direction === 'in' ? `from ${Message.peer ?? 'an unknown node'}` : `to ${Message.peer ?? 'an unknown node'}`
    ].join(' · ');

    const copy = element('button', 'btn small', 'Copy');
    copy.type  = 'button';
    copy.dataset.copy = '';
    copy.title = 'Copy the JSON array';

    const head = element('div', 'ocpp-detail-head');
    head.append(facts, copy);

    detail.append(head, element('pre', '', prettyJSON(Message)));

    return detail;

}

/** An option of a chooser. */
function option(Value: string, Text: string): HTMLOptionElement {
    const made = element('option', '', Text);
    made.value = Value;
    return made;
}

/**
 * The entry of the menu: for whoever may change the station server - the
 * resource is the kind's to say, and asked when the menu is drawn, after the
 * kind has said it.
 */
export const ocppLogMenuEntry: MenuEntry = {
    path:        '/ocpp-log',
    label:       'OCPP log',
    icon:        'fa-right-left',
    permission:  () => auth.can(stationsResource(), 'edit')
};

/** Whether pagehide and pageshow already pause and resume the stream. */
let followingAcrossTheCache = false;

/**
 * Every OCPP message of this node, as it goes: the time, which way, the node
 * at the other end, the action, and the array that went over the wire.
 *
 * Built like a roaming hub's traffic page - one stream, filters that work on
 * what is already in the browser, a list that follows along until somebody
 * scrolls up, and a line that unfolds to the array laid out. An answer
 * carries no action of its own: it is shown with its request's, which the
 * node remembers until the answer comes.
 *
 * Behind the permission to change the station server, which is what switching
 * the contents of the messages into the event log asks for: a message carries
 * identification tokens and meter readings.
 */
export const ocppLogPage: Page = {

    title: 'OCPP log',

    render({ root }) {

        if (!followingAcrossTheCache) {
            followingAcrossTheCache = true;
            followAcrossTheCache(window, ocppLog);
        }

        const content = shell(root, {
            active:    '/ocpp-log',
            title:     'OCPP log',
            subtitle:  'Every OCPP message this node sent and received, over every connection.',
            actions:   html`
                <span id="stream-state" class="stream-state"></span>
                <button type="button" id="clear" class="btn small" title="Clear what this page shows; the node keeps its record"
                        @click=${() => ocppLog.clear()}>Clear view</button>
            `
        });

        render(content, html`

            <div class="log-filters">

                <label class="filter-search">
                    <i class="fa-solid fa-magnifying-glass"></i>
                    <input type="search" id="search" placeholder="Search the actions and the messages ..." autocomplete="off" />
                </label>

                <label class="filter-level">
                    Direction
                    <select id="direction">
                        <option value="">both ways</option>
                        <option value="in">received</option>
                        <option value="out">sent</option>
                    </select>
                </label>

                <label class="filter-level">
                    Node
                    <select id="peer"><option value="">every node</option></select>
                </label>

                <label class="filter-follow">
                    <input type="checkbox" id="errors" />
                    Only errors
                </label>

                <label class="filter-follow">
                    <input type="checkbox" id="follow" checked />
                    Follow
                </label>

            </div>

            <div id="messages" class="log ocpp-log" role="log" aria-live="polite" tabindex="0"></div>

            <div class="log-foot small muted">
                <span id="counts"></span>
                <button type="button" id="to-bottom" class="btn small" hidden>Jump to the newest</button>
            </div>

        `);

        const list        = must<HTMLElement>       (content, '#messages');
        const counts      = must<HTMLElement>       (content, '#counts');
        const toBottom    = must<HTMLButtonElement> (content, '#to-bottom');
        const search      = must<HTMLInputElement>  (content, '#search');
        const direction   = must<HTMLSelectElement> (content, '#direction');
        const peer        = must<HTMLSelectElement> (content, '#peer');
        const errors      = must<HTMLInputElement>  (content, '#errors');
        const follow      = must<HTMLInputElement>  (content, '#follow');
        const streamState = must<HTMLElement>       (root,    '#stream-state');

        /** The ids whose details are unfolded. */
        const opened = new Set<number>();

        let renderedPeers = '';


        function matches(message: OCPPMessage): boolean {

            if (direction.value !== '' && message.direction !== direction.value)
                return false;

            if (errors.checked && !isError(message))
                return false;

            if (peer.value !== '' && message.peer !== peer.value)
                return false;

            const needle = search.value.trim().toLowerCase();

            return needle.length === 0 ||
                   (message.action ?? '').toLowerCase().includes(needle) ||
                   compactJSON(message).toLowerCase().includes(needle);

        }

        function linesOf(messages: readonly OCPPMessage[]): DocumentFragment {
            const lines = document.createDocumentFragment();
            for (const message of messages)
                lines.append(lineElement(message, opened.has(message.id)));
            return lines;
        }

        function atBottom(): boolean {
            return list.scrollTop + list.clientHeight >= list.scrollHeight - 24;
        }

        function scrollToBottom(): void {
            list.scrollTop  = list.scrollHeight;
            toBottom.hidden = true;
        }

        /** Everything again: after a reload, or when a filter changed. */
        function redraw(): void {

            const shown = ocppLog.messages.filter(matches);

            list.replaceChildren(shown.length > 0
                                     ? linesOf(shown)
                                     : element('div', 'log-empty', 'Nothing to show. No OCPP message has gone over a connection of this node yet, or the filters are too narrow.'));

            if (follow.checked)
                scrollToBottom();

            updateCounts();
            drawPeers();

        }

        /** Only what is new: the usual case, and the cheap one. */
        function append(added: OCPPMessage[]): void {

            const wanted = added.filter(matches);

            if (wanted.length > 0) {

                const stick = follow.checked && atBottom();

                list.querySelector('.log-empty')?.remove();
                list.append(linesOf(wanted));

                // The node keeps a bounded record and so does this page; what
                // fell out of the store has to leave the list as well.
                while (list.childElementCount > ocppLog.messages.length)
                    list.firstElementChild?.remove();

                if (stick)
                    scrollToBottom();
                else
                    toBottom.hidden = false;

            }

            updateCounts();
            drawPeers();

        }

        function updateCounts(): void {

            const shown = list.querySelectorAll('.line').length;

            counts.textContent = `${shown} of ${ocppLog.messages.length} messages` +
                                 (ocppLog.capacity === 0
                                      ? ''
                                      : ` (the node keeps the last ${ocppLog.capacity}, in memory only)`);

        }

        /** The node filter, made anew only when a new node has been seen. */
        function drawPeers(): void {

            const all = [...ocppLog.peers].sort();
            const key = all.join('\0');

            if (key === renderedPeers)
                return;

            renderedPeers = key;

            const chosen = peer.value;

            peer.replaceChildren(option('', 'every node'),
                                 ...all.map(one => option(one, one)));

            peer.value = all.includes(chosen) ? chosen : '';

        }

        function showStream(): void {
            streamState.className   = `stream-state ${ocppLog.streamConnected ? 'live' : 'down'}`;
            streamState.textContent = ocppLog.streamConnected ? 'live'
                                    : ocppLog.refused         ? 'ended'
                                    :                           'reconnecting ...';
        }


        // Events

        list.addEventListener('click', event => {

            const target = event.target as Element | null;

            if (target?.closest('[data-copy]')) {
                copyOf(target);
                return;
            }

            // Selecting a part of the unfolded array is reading it, not folding it.
            if (target?.closest('.ocpp-detail'))
                return;

            toggle(target);

        });

        list.addEventListener('keydown', event => {
            if (event.key === 'Enter' || event.key === ' ') {
                const head = (event.target as Element | null)?.closest('.ocpp-head');
                if (head) {
                    event.preventDefault();
                    toggle(head);
                }
            }
        });

        /** The message of a line, by the id the line carries. */
        function messageOf(line: HTMLElement): OCPPMessage | undefined {
            const id = Number(line.dataset.id);
            return ocppLog.messages.find(one => one.id === id);
        }

        /** The array of a line onto the clipboard, as it went over the wire. */
        function copyOf(target: Element): void {

            const line    = target.closest<HTMLElement>('.line.ocpp-message');
            const message = line ? messageOf(line) : undefined;

            if (message === undefined)
                return;

            void navigator.clipboard?.writeText(compactJSON(message)).then(
                () => { target.textContent = 'Copied'; },
                () => { target.textContent = 'Could not copy'; }
            );

        }

        /**
         * Unfold a line, or fold it again: that line made anew, in its place,
         * with the focus kept where it was for somebody at the keyboard.
         */
        function toggle(target: Element | null): void {

            const line = target?.closest<HTMLElement>('.line.ocpp-message');

            if (!line)
                return;

            const message = messageOf(line);

            if (message === undefined)
                return;

            if (opened.has(message.id))
                opened.delete(message.id);
            else
                opened.add(message.id);

            const hadFocus = line.contains(document.activeElement);
            const made     = lineElement(message, opened.has(message.id));

            line.replaceWith(made);

            if (hadFocus)
                made.querySelector<HTMLElement>('.ocpp-head')?.focus();

        }

        search   .addEventListener('input',  () => redraw());
        direction.addEventListener('change', () => redraw());
        peer     .addEventListener('change', () => redraw());
        errors   .addEventListener('change', () => redraw());
        follow   .addEventListener('change', () => { if (follow.checked) scrollToBottom(); });
        toBottom .addEventListener('click',  () => scrollToBottom());

        list.addEventListener('scroll', () => {
            if (atBottom())
                toBottom.hidden = true;
        });

        const stopListening = ocppLog.onChange(event => {

            switch (event.type) {

                case 'messages':
                    append(event.added);
                    break;

                case 'reloaded':
                    redraw();
                    break;

                case 'stream':
                    showStream();

                    // No longer allowed to read this: what the menu offers is
                    // asked again, so that the next page drawn no longer
                    // offers the OCPP log.
                    if (ocppLog.refused)
                        void auth.refresh();

                    break;

                case 'error': {
                    const line = element('div', 'line error');
                    line.append(element('span', 'message', event.text));
                    list.append(line);
                    break;
                }

            }

        });

        ocppLog.start();

        showStream();
        redraw();

        // The stream goes when the page goes: it is behind its own
        // permission, it can run hot, and nothing outside this page shows it.
        return () => {
            stopListening();
            ocppLog.stop();
        };

    }

};
