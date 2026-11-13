import type { ConnectionState, StationConnection } from '../api/connections';
import { nodeNoun } from '../settings';

/**
 * What the Connections page says beside a connection about where it stands.
 *
 * Apart from the page, because this is the part that has to be right when
 * nobody is looking: the page is where somebody finds out whether this station
 * is on its back end, and it is believed. So what is said here is what the
 * station said, and nothing it did not - "since" and "next" are counted on the
 * station's clock, which the browser's need not agree with, and a connection
 * changed on the page is said to be dialled as it was, which it is for the
 * moment it takes the station to hang it up and dial it as it is now.
 */


/** How a state is marked: the colours of the chips on the other pages. */
export type Tone = 'on' | 'warn' | 'bad' | '';

/** One connection's line, ready to be drawn. */
export interface StateLine {
    /** The word in the chip. */
    chip:   string;
    tone:   Tone;
    /** Since when, and while it is tried again, when next - in words. */
    when:   string;
    /** What the station said when it came to stand there, as it said it. */
    said:   string;
    /** What somebody looking at a connection changed since the start needs to know. */
    notes:  string[];
}


/**
 * How long, in the words somebody would use, from the station's clock.
 *
 * @param from  the earlier moment.
 * @param to    the later one.
 */
export function between(from: string, to: string): string {

    const seconds = Math.max(0, Math.round((Date.parse(to) - Date.parse(from)) / 1000));

    if (Number.isNaN(seconds))
        return '';

    if (seconds <     60)  return `${seconds} s`;
    if (seconds <   3600)  return `${Math.floor(seconds /    60)} min`;
    if (seconds <  86400)  return `${Math.floor(seconds /  3600)} h ${Math.floor(seconds % 3600 / 60)} min`;

    return `${Math.floor(seconds / 86400)} d ${Math.floor(seconds % 86400 / 3600)} h`;

}


/**
 * Where a connection stands, or null where there is nothing to say: one that
 * is only written down was never meant to be dialled, and the page says that
 * already.
 *
 * @param entry  the connection as it is written down now.
 * @param state  what the station says became of it, if it dialled it.
 * @param now    the station's clock when it said so.
 */
export function stateLine(entry:  Pick<StationConnection, 'url' | 'ocppVersion' | 'autoConnect'>,
                          state:  ConnectionState | undefined,
                          now:    string): StateLine | null {

    if (state === undefined)
        return entry.autoConnect
                   ? {
                         chip:   'not dialled',
                         tone:   '',
                         when:   '',
                         said:   'Not dialled yet: it was written down, or set to connect by itself, a moment ago, ' +
                                 `and is dialled as soon as this ${nodeNoun()} has taken the change.`,
                         notes:  []
                     }
                   : null;

    const since  = `for ${between(state.since, now)}`;

    const next   = state.nextAttemptAt === undefined
                       ? ''
                       : Date.parse(state.nextAttemptAt) > Date.parse(now)
                             ? `; attempt ${state.attempt ?? '?'} in ${between(now, state.nextAttemptAt)}`
                             : `; attempt ${state.attempt ?? '?'} is being made`;

    const [ chip, tone, when ] = ((): [ string, Tone, string ] => {
        switch (state.status) {
            case 'connected':   return [ 'connected',    'on',   since ];
            case 'lost':        return [ 'lost',         'warn', `${since}, and tried again by itself${next}` ];
            case 'trying':      return [ 'not reached',  'warn', `${since}, and tried again by itself${next}` ];
            case 'refused':     return [ 'refused',      'bad',  `${since}, and not tried again` ];
            case 'notDialled':  return [ 'not dialled',  'bad',  since ];
            case 'standby':     return [ 'in reserve',   '',     since ];
            default:            return [ 'failed',       'bad',  `${since}, and not tried again` ];
        }
    })();

    const notes: string[] = [];

    if (state.url !== entry.url || state.ocppVersion !== entry.ocppVersion)
        notes.push(`Dialled as ${state.url} (${state.ocppVersion}). ` +
                   `It is hung up and dialled as written here now, as soon as this ${nodeNoun()} has taken the change.`);

    if (!entry.autoConnect && (state.status === 'connected' || state.status === 'lost' || state.status === 'trying'))
        notes.push(`No longer set to connect by itself. It is hung up as soon as this ${nodeNoun()} has taken the change.`);

    return { chip, tone, when, said: state.said, notes };

}


/**
 * The connections this station dialled that are no longer written down.
 *
 * A connection removed on the page is hung up at once, but not in the same
 * moment, so for the time in between the station may still be on a back end
 * that the page no longer mentions. They are listed rather than left out.
 *
 * @param connections  what is written down now.
 * @param states       what the station says of what it dialled.
 */
export function noLongerWrittenDown(connections:  Pick<StationConnection, 'id'>[],
                                    states:       Record<string, ConnectionState>): [ string, ConnectionState ][] {

    const written = new Set(connections.map(connection => connection.id));

    return Object.entries(states).filter(([ id ]) => !written.has(id));

}
