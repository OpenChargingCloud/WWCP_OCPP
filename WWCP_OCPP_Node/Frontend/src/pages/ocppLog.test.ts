/**
 * The OCPP log: a line says the time, which way the message went, the node at
 * the other end, the action and the array as it went over the wire, and
 * unfolds to the array laid out; an error is marked as one; an answer whose
 * request was not seen says so; and the store takes a message once, however
 * often the stream repeats it.
 *
 * Run with `npm test`.
 */

// The document of happy-dom every page test is drawn in.
import '../../test/stationServer.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { OCPPMessage } from '../api/ocppLog.ts';

const { lineElement, compactJSON } = await import('./ocppLog.ts');
const { OCPPLogStore }             = await import('../ocppLog/store.ts');


function aMessage(More: Partial<OCPPMessage> = {}): OCPPMessage {
    return {
        id:           1,
        timestamp:    '2026-11-24T10:00:00.000Z',
        direction:    'out',
        kind:         'request',
        ocppVersion:  '2.1',
        peer:         'CSMS01',
        messageId:    '42',
        action:       'BootNotification',
        message:      [ 2, '42', 'BootNotification', { reason: 'PowerUp', chargingStation: { model: 'X', vendorName: 'Y' } } ],
        ...More
    };
}


describe('A line of the OCPP log', () => {

    it('says the action, the node and the array as it went over the wire', () => {

        const line = lineElement(aMessage());

        assert.equal(line.querySelector('.action')?.textContent, 'BootNotification');
        assert.equal(line.querySelector('.peer')?.textContent,   'CSMS01');
        assert.equal(line.querySelector('.chip.kind')?.textContent, 'CALL');
        assert.ok   (line.querySelector('.chip.dir.out'));
        assert.equal(line.querySelector('.json')?.textContent,   compactJSON(aMessage()));
        assert.equal(line.querySelector('.ocpp-detail'), null);

    });

    it('unfolds to the array laid out', () => {

        const line = lineElement(aMessage(), true);
        const pre  = line.querySelector('.ocpp-detail pre')?.textContent ?? '';

        assert.deepEqual(JSON.parse(pre), aMessage().message);
        assert.ok(pre.includes('\n'), 'laid out over several lines');

    });

    it('marks an error, and an answer whose request was not seen', () => {

        const line = lineElement(aMessage({ kind: 'requestError', direction: 'in', action: null,
                                            message: [ 4, '42', 'NotSupported', 'No', {} ] }));

        assert.ok   (line.classList.contains('failed'));
        assert.equal(line.querySelector('.action')?.textContent, '?');
        assert.equal(line.querySelector('.chip.kind')?.textContent, 'ERROR');

    });

    it('writes what a station sent as text, not as markup', () => {

        const line = lineElement(aMessage({ action: '<img src=x onerror=alert(1)>' }));

        assert.equal(line.querySelector('img'), null);

    });

});


describe('The OCPP log store', () => {

    it('takes a message once, however often the stream repeats it, and remembers its node', () => {

        const store = new OCPPLogStore();
        const added: number[] = [];

        store.onChange(event => { if (event.type === 'messages') added.push(...event.added.map(one => one.id)); });

        store.apply([ aMessage({ id: 2 }), aMessage({ id: 1 }) ]);
        store.apply([ aMessage({ id: 2 }), aMessage({ id: 3, peer: 'LC001' }) ]);

        assert.deepEqual(store.messages.map(one => one.id), [ 1, 2, 3 ]);
        assert.deepEqual(added,                             [ 1, 2, 3 ]);
        assert.deepEqual([...store.peers].sort(),           [ 'CSMS01', 'LC001' ]);

    });

});
