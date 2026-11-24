import { ApiError, nodeAPI } from '@node/api/client';
import { stationsResource } from '@node/connecting/settings';
import { ocppLogAPI, type OCPPMessage } from '../api/ocppLog';

// The browser's copy of the OCPP messages of this node, fed as the event log
// and a roaming hub's traffic are: a snapshot from the JSON API and a
// Server-Sent Events stream. Both carry ids from the same counter on the node,
// and the counter only ever grows - so a message is taken only when it is
// newer than the snapshot, and a replayed stream (Hermod repeats its cached
// events to every new client) does no harm.
//
// The stream is not filtered: filtering by peer, by direction, by kind and by
// what a message says happens on this side, over what is already here, and is
// therefore instant.
//
// It starts and stops with its page, as the traffic of a hub does: it is
// behind a permission of its own, and a busy CSMS sends and receives far more
// than it writes to its log. See RoamingHub's traffic/store.ts, which this is
// modelled on, for why pause() and resume() are there.

export type StoreEvent =
    | { type: 'messages'; added: OCPPMessage[] }
    | { type: 'reloaded' }
    | { type: 'stream' }
    | { type: 'error';    text: string };

type Listener = (event: StoreEvent) => void;

/** How many messages the page keeps: the ceiling for a page that has been open all day. */
const MAX_MESSAGES = 5_000;

/** How much a fresh page loads before it starts following along. */
const SNAPSHOT_SIZE = 1_000;

/** How long after the stream has given up before the node is asked why, and before asking again. */
const ASK_WHY_AFTER   =  3_000;
const ASK_AGAIN_AFTER = 10_000;

/** The routes every node has: who is signed in, here. */
const api = nodeAPI();


export class OCPPLogStore {

    /** What is known, oldest first. */
    readonly messages: OCPPMessage[] = [];

    /** Every node seen at the other end of a message, for the filter. */
    readonly peers = new Set<string>();

    /** Whether the event stream is up. */
    streamConnected = false;

    /** The newest id this page knows about. */
    lastId = 0;

    /** How many messages the node keeps. */
    capacity = 0;

    /** Whether the node stopped sending the messages to this account, because it may no longer read them. */
    refused = false;

    private source:              EventSource | null = null;
    private readonly listeners = new Set<Listener>();

    private askingWhy = false;
    private askAgain: ReturnType<typeof setTimeout> | null = null;
    private paused    = false;


    onChange(listener: Listener): () => void {
        this.listeners.add(listener);
        return () => this.listeners.delete(listener);
    }


    /** Open the stream; every 'open' - the first and every reconnect - reloads. */
    start(): void {

        if (this.source !== null)
            return;

        this.refused = false;

        const source = new EventSource(ocppLogAPI.eventsURL());
        this.source  = source;

        source.addEventListener('open', () => {
            this.streamConnected = true;
            this.emit({ type: 'stream' });
            void this.reload();
        });

        source.addEventListener('error', () => {

            if (this.streamConnected) {
                this.streamConnected = false;
                this.emit({ type: 'stream' });
            }

            // CLOSED is the browser having given up - and also what a stream
            // looks like that the node ended because its reader may no longer
            // read it.
            if (source.readyState === EventSource.CLOSED)
                this.findOutWhy(source, ASK_WHY_AFTER);

        });

        source.addEventListener('message', event => {

            try
            {
                this.apply([JSON.parse((event as MessageEvent<string>).data) as OCPPMessage]);
            }
            catch (error)
            {
                console.warn('An OCPP message could not be read:', error);
            }

        });

    }

    /**
     * Why the stream stopped, asked of the node rather than guessed: a session
     * that is gone (the router's business), an account that may no longer
     * read the messages (said, and not tried again every few seconds), or a
     * stream that was cut (opened again).
     */
    private findOutWhy(Source: EventSource, In: number): void {

        if (this.askingWhy || this.source !== Source || this.askAgain !== null)
            return;

        this.askAgain = setTimeout(() => {

            this.askAgain = null;

            if (this.source !== Source)
                return;

            this.askingWhy = true;

            api.auth.me().then(
                me => {

                    this.askingWhy = false;

                    if (this.source !== Source)
                        return;

                    Source.close();
                    this.source = null;

                    if (me.permissions.includes(`${stationsResource()}:edit`)) {
                        this.start();
                        return;
                    }

                    this.refused = true;
                    this.emit({ type: 'stream' });
                    this.emit({
                        type: 'error',
                        text: 'This account may no longer read the OCPP messages, so the node has stopped sending them. ' +
                              'What is shown is what came before.'
                    });

                },
                (problem: unknown) => {

                    this.askingWhy = false;

                    if (problem instanceof ApiError && problem.isUnauthorized)
                        return;

                    this.findOutWhy(Source, ASK_AGAIN_AFTER);

                }
            );

        }, In);

    }


    /** Close the stream and forget everything: when the page goes. */
    stop(): void {

        if (this.askAgain !== null) {
            clearTimeout(this.askAgain);
            this.askAgain = null;
        }

        this.source?.close();
        this.source = null;

        this.streamConnected = false;
        this.paused          = false;
        this.lastId          = 0;

        this.messages.length = 0;
        this.peers.clear();

    }

    /** Close the stream while the page waits in the browser's back/forward cache, and keep what is known. */
    pause(): void {

        this.paused = this.source !== null;

        if (this.askAgain !== null) {
            clearTimeout(this.askAgain);
            this.askAgain = null;
        }

        this.source?.close();
        this.source = null;

        if (this.streamConnected) {
            this.streamConnected = false;
            this.emit({ type: 'stream' });
        }

    }

    /** Open the stream again where pause() closed one; the 'open' reloads what was missed. */
    resume(): void {

        if (!this.paused)
            return;

        this.paused = false;

        this.start();

    }

    /** Throw away what this page shows, without touching the node's record. */
    clear(): void {
        this.messages.length = 0;
        this.emit({ type: 'reloaded' });
    }


    /** The snapshot: the messages up to now, as far back as the node keeps them. */
    async reload(): Promise<void> {

        try
        {

            const page = await ocppLogAPI.messages(SNAPSHOT_SIZE);

            this.messages.length = 0;
            this.messages.push(...page.messages);

            this.lastId   = Math.max(page.lastId, ...page.messages.map(message => message.id), 0);
            this.capacity = page.capacity;

            for (const peer of page.peers)
                this.peers.add(peer);

            for (const message of page.messages)
                this.remember(message);

            this.emit({ type: 'reloaded' });

        }
        catch (error)
        {
            this.emit({
                type: 'error',
                text: `Could not load the OCPP messages: ${error instanceof Error ? error.message : String(error)}`
            });
        }

    }


    /** Everything the stream delivered that the snapshot did not already hold. */
    apply(incoming: OCPPMessage[]): void {

        const added = incoming.filter(message => message.id > this.lastId).
                               sort((a, b) => a.id - b.id);

        if (added.length === 0)
            return;

        for (const message of added) {
            this.messages.push(message);
            this.lastId = message.id;
            this.remember(message);
        }

        if (this.messages.length > MAX_MESSAGES)
            this.messages.splice(0, this.messages.length - MAX_MESSAGES);

        this.emit({ type: 'messages', added });

    }

    private remember(message: OCPPMessage): void {
        if (message.peer)
            this.peers.add(message.peer);
    }

    private emit(event: StoreEvent): void {

        for (const listener of this.listeners)
        {
            try
            {
                listener(event);
            }
            catch (error)
            {
                console.error('An OCPP log listener failed:', error);
            }
        }

    }

}


export const ocppLog = new OCPPLogStore();
