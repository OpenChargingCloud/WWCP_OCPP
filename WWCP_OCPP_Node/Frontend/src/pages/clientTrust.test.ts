/**
 * The accepted chains drawn, in a document of happy-dom, against a stand-in
 * controller: an authority half pasted outlives a chain switched off, a
 * switch the controller refused says what it has, and an authority accepted
 * empties the form.
 */

import { anOCPPServer, asked, change, field, open, refused, said, submit, until, type Asked } from '../../test/stationServer.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { ClientTrust, TrustedChain } from '../api/stationServer.ts';

const { clientTrustPage } = await import('./clientTrust.ts');


function aChain(id: string, enabled = true): TrustedChain {
    return {
        id, name: `Network ${id}`, addedAt: '2026-10-01T00:00:00Z', enabled,
        subject: `CN=${id}`, issuer: `CN=${id}`, serialNumber: '01', thumbprint: id.repeat(8),
        notBefore: '2026-01-01T00:00:00Z', notAfter: '2030-01-01T00:00:00Z',
        isCA: true, intermediates: 0, warnings: [], state: 'valid', daysRemaining: 1200
    };
}

let held: ClientTrust;

/** Refuses a switch where told to. */
let refuseSwitches = false;

function controller({ method, path, body }: Asked): unknown {

    if (path === '/configuration/ocpp-server')
        return anOCPPServer();

    if (path === '/configuration/ocpp-server/trust' && method === 'GET')
        return held;

    if (path === '/configuration/ocpp-server/trust' && method === 'POST') {
        held = { ...held, entries: [ ...held.entries, aChain('c3') ] };
        return { id: 'c3', warnings: [] };
    }

    const one = /^\/configuration\/ocpp-server\/trust\/(.+)$/.exec(path);

    if (one && method === 'PUT') {
        if (refuseSwitches)
            return refused(409, 'the last chain stays switched on');
        held = { ...held, entries: held.entries.map(entry => entry.id === one[1] ? { ...entry, ...(body as object) } : entry) };
        return held;
    }

    return undefined;

}

async function opened(): Promise<HTMLElement> {
    held           = { directory: 'trust', now: '2026-10-04T12:00:00Z', enabled: 2, maxEntries: 8,
                       entries: [ aChain('c1'), aChain('c2') ] };
    refuseSwitches = false;
    return open(clientTrustPage, '/configuration/ocpp-server/trust', [ 'certificates:read', 'certificates:edit' ],
                controller, root => root.querySelector('#add-form') !== null && root.querySelector('.trust-enabled') !== null);
}

const switchOf = (root: HTMLElement, id: string) => root.querySelector<HTMLInputElement>(`.trust-enabled[data-id="${id}"]`)!;


describe('the accepted chains', () => {

    it('keep an authority half pasted, and its focus, while a chain is switched off', async () => {

        const root = await opened();
        const pem  = field<HTMLTextAreaElement>(root, '#add-form', 'pem');

        pem.value = '-----BEGIN CERTIFICATE-----\nMIIB';
        pem.focus();

        change(switchOf(root, 'c1'), false);
        await until(() => root.querySelector('.entry.dimmed') !== null, 'the chain switched off was not drawn so');

        assert.ok(field<HTMLTextAreaElement>(root, '#add-form', 'pem') === pem, 'the field was made anew');
        assert.equal(pem.value,              '-----BEGIN CERTIFICATE-----\nMIIB');
        assert.ok(document.activeElement === pem, 'the focus went');

    });

    it('put a switch the controller refused back to what it has', async () => {

        const root = await opened();

        refuseSwitches = true;
        change(switchOf(root, 'c2'), false);

        await until(() => said.length === 1, 'the refusal was not said');

        assert.match(said[0]!, /the last chain stays switched on/);
        assert.equal(switchOf(root, 'c2').checked, true, 'the switch says what was clicked, not what the controller has');

    });

    it('empty the form once the authority is accepted', async () => {

        const root = await opened();

        field(root, '#add-form', 'name').value                       = 'Network c3';
        field<HTMLTextAreaElement>(root, '#add-form', 'pem').value   = '-----BEGIN CERTIFICATE-----\nMIIC';

        submit(root, '#add-form');
        await until(() => root.querySelector('.trust-enabled[data-id="c3"]') !== null, 'the authority accepted was not drawn');

        assert.deepEqual(asked.find(one => one.method === 'POST')?.body, { pem: '-----BEGIN CERTIFICATE-----\nMIIC', name: 'Network c3' });
        assert.equal(field<HTMLTextAreaElement>(root, '#add-form', 'pem').value, '', 'what was accepted is still in the form');
        assert.equal(field(root, '#add-form', 'name').value,                       '');

    });

});
