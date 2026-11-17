/**
 * The server certificates drawn, in a document of happy-dom, against a
 * stand-in controller: a certificate pasted under one key outlives another
 * key being removed or taking its own in, the remark on the kind of key
 * follows the one chosen, and a request made empties its form.
 */

import { anOCPPServer, asked, change, field, open, refused, submit, type, until, type Asked } from '../../test/stationServer.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { ServerCertificate, ServerCertificates } from '../api/stationServer.ts';

const { serverCertificatesPage } = await import('./serverCertificates.ts');


function aKey(id: string): ServerCertificate {
    return { id, algorithm: 'ecdsa-p256', createdAt: '2026-10-01T00:00:00Z', subject: 'CN=lc001.example.org',
             hasCertificate: false, canBePresented: true, inUse: false, warnings: [] };
}

let held: ServerCertificates;

/** A certificate a key was given, for the list of all of them. */
function withCertificate(id: string, subject: string, thumbprint: string, inUse = false): ServerCertificate {
    return { ...aKey(id), hasCertificate: true, inUse,
             certificate: { subject, issuer: 'CN=Fleet CA', serialNumber: '01', thumbprint, notBefore: '2026-09-01T00:00:00Z',
                            notAfter: '2027-09-01T00:00:00Z', subjectAltNames: [ 'lc001.example.org' ], intermediates: 1,
                            state: 'valid', daysRemaining: 330 } };
}

/** What the stand-in finds in a text: k1's certificate, and one of nobody's key. */
const foundInText = [
    { id: '1111111111111111', thumbprint: '11'.repeat(32), label: 'lc001.example.org', subject: 'CN=lc001.example.org', issuer: 'CN=Fleet CA',
      notBefore: '2026-09-01T00:00:00Z', notAfter: '2027-09-01T00:00:00Z', chainLength: 1, hasPrivateKey: false, keyId: 'k1', refusal: null },
    { id: '2222222222222222', thumbprint: '22'.repeat(32), label: 'Somebody Else', subject: 'CN=Somebody Else', issuer: 'CN=Somebody Else',
      notBefore: '2026-09-01T00:00:00Z', notAfter: '2027-09-01T00:00:00Z', chainLength: 0, hasPrivateKey: false, keyId: null,
      refusal: 'It belongs to no key of this local controller.' }
];

let refuseTheUpload = false;

function controller({ method, path, body }: Asked): unknown {

    if (path === '/configuration/ocpp-server')
        return anOCPPServer();

    if (path === '/configuration/ocpp-server/certificates/inspect') {
        const { content } = body as { content?: string };
        if (content !== undefined)
            return { certificates: [ { ...foundInText[0]!, hasPrivateKey: true, refusal: 'That certificate came with its private key.' } ],
                     pem: '-----BEGIN CERTIFICATE-----\nFROMFILE\n-----END CERTIFICATE-----\n' };
        return { certificates: foundInText, pem: '' };
    }

    if (path === '/configuration/ocpp-server/certificates/upload') {
        if (refuseTheUpload)
            return refused(400, 'None of the 2 certificates was taken in.', { taken: [], refused: [ { label: 'Somebody Else', error: 'Not ours.' } ] });
        held = { ...held, entries: held.entries.map(entry => entry.id === 'k1' ? withCertificate('k1', 'CN=lc001.example.org', '11'.repeat(32), true) : entry) };
        return { taken: [ { id: 'k1', label: 'lc001.example.org', warnings: [] } ], refused: [] };
    }

    if (path === '/configuration/ocpp-server/certificates' && method === 'GET')
        return held;

    if (path === '/configuration/ocpp-server/serverKeys' && method === 'POST') {
        const { next } = body as { algorithm: string; next: boolean };
        const id       = next ? 'k5' : 'k4';
        const keyPin   = (next ? 'b2' : 'a1').repeat(32);
        const made     = { ...aKey(id), subject: `CN=Key pin carrier ${id}`, hasCarrier: true, next, keyPin };
        held = { ...held, entries: [ made, ...held.entries ],
                 keyPins: next ? { current: held.keyPins!.current, next: { id, keyPin, createdAt: made.createdAt } }
                               : { current: { id, keyPin, createdAt: made.createdAt, presented: 'carrier' }, next: null } };
        return { id, keyPin, next, certificates: held };
    }

    if (path === '/configuration/ocpp-server/serverKeys/switch' && method === 'POST') {
        held = { ...held, keyPins: { current: { ...held.keyPins!.next!, presented: 'carrier' }, next: null },
                 entries: held.entries.map(entry => ({ ...entry, next: false })) };
        return held;
    }

    if (path === '/configuration/ocpp-server/certificates' && method === 'POST') {
        held = { ...held, entries: [ ...held.entries, aKey('k3') ] };
        return { id: 'k3', csr: '-----BEGIN CERTIFICATE REQUEST-----' };
    }

    const one = /^\/configuration\/ocpp-server\/certificates\/([^/]+)$/.exec(path);

    if (one && method === 'DELETE') {
        held = { ...held, entries: held.entries.filter(entry => entry.id !== one[1]) };
        return held;
    }

    if (one && method === 'PUT') {
        held = { ...held, entries: held.entries.map(entry => entry.id === one[1] ? { ...entry, hasCertificate: true, inUse: true } : entry) };
        return { id: one[1], warnings: [] };
    }

    return undefined;

}

async function opened(path = '/configuration/ocpp-server/certificates', permissions = [ 'certificates:read', 'certificates:edit' ]): Promise<HTMLElement> {
    refuseTheUpload = false;
    held = { directory: 'certificates', now: '2026-10-04T12:00:00Z', servedId: null, canImportPrivateKeys: false,
             entries:    [ aKey('k1'), aKey('k2') ],
             algorithms: [ { id: 'ecdsa-p256', name: 'ECDSA P-256', remark: 'What every station takes.' },
                           { id: 'rsa-4096',   name: 'RSA 4096',    remark: 'Takes a while to make.' } ] } as ServerCertificates;
    return open(serverCertificatesPage, path, permissions,
                controller, root => root.querySelector('.upload-form[data-id="k2"]') !== null);
}

const pemOf = (root: HTMLElement, id: string) => field<HTMLTextAreaElement>(root, `.upload-form[data-id="${id}"]`, 'pem');


describe('the server certificates', () => {

    it('keep a certificate pasted under one key, and its focus, while another key is removed', async () => {

        const root = await opened();
        const pem  = pemOf(root, 'k2');

        pem.value = '-----BEGIN CERTIFICATE-----\nk2';
        pem.focus();

        root.querySelector<HTMLButtonElement>('.remove[data-id="k1"]')!.click();
        await until(() => root.querySelector('.upload-form[data-id="k1"]') === null, 'the key removed is still drawn');

        assert.ok(pemOf(root, 'k2') === pem, 'the field was made anew');
        assert.equal(pem.value,              '-----BEGIN CERTIFICATE-----\nk2');
        assert.ok(document.activeElement === pem, 'the focus went');

    });

    it('empty the form of the key that took its certificate in, and only that one', async () => {

        const root = await opened();

        pemOf(root, 'k1').value = '-----BEGIN CERTIFICATE-----\nk1';
        pemOf(root, 'k2').value = '-----BEGIN CERTIFICATE-----\nk2';

        submit(root, '.upload-form[data-id="k1"]');
        await until(() => root.querySelector('.entry.in-use') !== null, 'the certificate taken in was not drawn');

        assert.deepEqual(asked.find(one => one.method === 'PUT')?.body, { pem: '-----BEGIN CERTIFICATE-----\nk1' });
        assert.equal(pemOf(root, 'k1').value, '',                                 'what was taken in is still in its form');
        assert.equal(pemOf(root, 'k2').value, '-----BEGIN CERTIFICATE-----\nk2', 'what was pasted under the other key went');

    });

    it('say the remark on the kind of key chosen, after a draw too', async () => {

        const root   = await opened();
        const remark = () => root.querySelector('#algorithm-remark')!.textContent!.trim();

        assert.equal(remark(), 'What every station takes.');

        change(field<HTMLSelectElement>(root, '#create-form', 'algorithm'), 'rsa-4096');
        assert.equal(remark(), 'Takes a while to make.');

        root.querySelector<HTMLButtonElement>('.remove[data-id="k1"]')!.click();
        await until(() => root.querySelector('.upload-form[data-id="k1"]') === null, 'the key removed is still drawn');

        assert.equal(field<HTMLSelectElement>(root, '#create-form', 'algorithm').value, 'rsa-4096');
        assert.equal(remark(), 'Takes a while to make.');

    });

    it('put the request\'s form back to what it starts with once a request is made', async () => {

        const root = await opened();

        field(root, '#create-form', 'subject').value = 'lc002.example.org';
        change(field<HTMLSelectElement>(root, '#create-form', 'algorithm'), 'rsa-4096');

        submit(root, '#create-form');
        await until(() => root.querySelector('.upload-form[data-id="k3"]') !== null, 'the key made was not drawn');

        assert.deepEqual(asked.find(one => one.method === 'POST')?.body, { subject: 'lc002.example.org', algorithm: 'rsa-4096' });
        assert.equal(field(root, '#create-form', 'subject').value,                        'lc001.example.org');
        assert.equal(field<HTMLSelectElement>(root, '#create-form', 'algorithm').value,   'ecdsa-p256');
        assert.equal(root.querySelector('#algorithm-remark')!.textContent!.trim(),        'What every station takes.');
        assert.match(root.querySelector('pre.pem')?.textContent ?? '', /BEGIN CERTIFICATE REQUEST/);

    });

    it('make the server key where nothing is presented, show its pin, announce a next key and switch to it', async () => {

        const root   = await opened();
        const pinOf  = (selector: string) => root.querySelector(selector)?.textContent?.trim();

        assert.ok(root.querySelector('#current-keypin') === null, 'a key pin is shown where nothing is presented');

        root.querySelector<HTMLButtonElement>('#serverkey-create')!.click();
        await until(() => root.querySelector('#current-keypin') !== null, 'the key pin of the server key made is not shown');

        assert.deepEqual(asked.find(one => one.path.endsWith('/serverKeys'))?.body, { algorithm: 'ecdsa-p256', next: false });
        assert.equal(pinOf('#current-keypin'), 'a1'.repeat(32));
        assert.match(root.querySelector('#keypins')!.textContent!, /in its carrier certificate/);
        assert.equal(pinOf('.keypin[data-id="k4"]'), 'a1'.repeat(32), 'the key does not say its pin');
        assert.ok(root.querySelector('#serverkey-create') === null, 'a second server key is offered beside the first');

        root.querySelector<HTMLButtonElement>('#serverkey-next')!.click();
        await until(() => root.querySelector('#next-keypin') !== null, 'the next key pin is not shown');

        assert.deepEqual(asked.filter(one => one.path.endsWith('/serverKeys')).at(-1)?.body, { algorithm: 'ecdsa-p256', next: true });
        assert.equal(pinOf('#current-keypin'), 'a1'.repeat(32), 'the next key took over before the switch');
        assert.equal(pinOf('#next-keypin'),    'b2'.repeat(32));

        root.querySelector<HTMLButtonElement>('#serverkey-switch')!.click();
        await until(() => root.querySelector('#next-keypin') === null, 'the switch is not drawn');

        assert.ok(asked.some(one => one.method === 'POST' && one.path.endsWith('/serverKeys/switch')), 'no switch was asked for');
        assert.equal(pinOf('#current-keypin'), 'b2'.repeat(32));

    });

});


describe('the server certificates, in their tabs', () => {

    const box  = (root: HTMLElement) => root.querySelector<HTMLTextAreaElement>('#upload-pem')!;
    const text = '-----BEGIN CERTIFICATE-----\nA\n-----END CERTIFICATE-----\n-----BEGIN CERTIFICATE-----\nB\n-----END CERTIFICATE-----\n';

    it('list every certificate once, by name or by fingerprint, narrowed down as typed', async () => {

        const root = await opened('/configuration/ocpp-server/certificates?tab=all');

        held = { ...held, entries: [ withCertificate('k1', 'CN=zulu.example.org', 'aa'.repeat(32)),
                                     withCertificate('k2', 'CN=Alpha.example.org', 'bb'.repeat(32), true), aKey('k3') ] };
        root.querySelector<HTMLButtonElement>('#reload')?.click();
        await until(() => root.querySelectorAll('#panel-all [data-certificate]').length === 2, 'the certificates are not listed');

        const listed = () => [ ...root.querySelectorAll<HTMLElement>('#panel-all [data-certificate]') ].map(card => card.dataset['certificate']);

        assert.equal(root.querySelector<HTMLElement>('#panel-all')!.hidden, false, 'the tab the address names is not shown');
        assert.deepEqual(listed(), [ 'k2', 'k1' ], 'not by name, in any case, or a key without a certificate is listed');
        assert.match(root.querySelector('#panel-all [data-certificate="k2"]')!.textContent!, /being presented/);

        const byFingerprint = root.querySelector<HTMLInputElement>('#panel-all input[name="order"][value="fingerprint"]')!;
        byFingerprint.checked = true;
        byFingerprint.dispatchEvent(new Event('change', { bubbles: true }));
        assert.deepEqual(listed(), [ 'k1', 'k2' ]);

        type(root.querySelector<HTMLInputElement>('#all-filter')!, 'ALPHA');
        assert.deepEqual(listed(), [ 'k2' ]);

    });

    it('say which key each certificate in the box is for, and which is for none', async () => {

        const root = await opened('/configuration/ocpp-server/certificates?tab=upload');

        type(box(root), text);
        await until(() => root.querySelectorAll('#upload-found [data-found]').length === 2, 'what the box holds is not said');

        assert.match(root.querySelector('[data-found="1111111111111111"]')!.textContent!, /for key k1/);
        assert.match(root.querySelector('[data-found="2222222222222222"] .refusal')!.textContent!, /belongs to no key/);

    });

    it('take every certificate in the box in, each under its key, and empty the box', async () => {

        const root = await opened('/configuration/ocpp-server/certificates?tab=upload');

        type(box(root), text);
        submit(root, '#upload-all-form');
        await until(() => (root.querySelector('#upload-note')?.textContent ?? '').startsWith('Taken in'), 'nothing was taken in');

        assert.deepEqual(asked.find(one => one.path.endsWith('/certificates/upload'))!.body, { pem: text.trim() });
        assert.match(root.querySelector('#upload-result .went-in')!.textContent!, /lc001\.example\.org: taken in for key k1/);
        assert.equal(box(root).value, '', 'what was taken in is still in the box');
        assert.ok(root.querySelector('#panel-keys .entry.in-use') !== null, 'the key that took its certificate in is not drawn as such');

    });

    it('keep the box, and say why, where nothing was taken in', async () => {

        const root = await opened('/configuration/ocpp-server/certificates?tab=upload');

        type(box(root), text);
        refuseTheUpload = true;
        submit(root, '#upload-all-form');
        await until(() => (root.querySelector('#upload-error')?.textContent ?? '') !== '', 'the refusal is not said');

        assert.equal(box(root).value, text);
        assert.match(root.querySelector('#upload-result .refused')!.textContent!, /Somebody Else: Not ours\./);

    });

    it('put a file dropped on the box into it without its key, and say that the key was left out', async () => {

        const root = await opened('/configuration/ocpp-server/certificates?tab=upload');
        const drop = new Event('drop', { bubbles: true, cancelable: true });

        Object.defineProperty(drop, 'dataTransfer', { value: { files: [ new File([ 'p12' ], 'lc001.p12') ], getData: () => '' } });
        root.querySelector('.pem-box')!.dispatchEvent(drop);

        await until(() => box(root).value.includes('FROMFILE'), 'the file is not in the box');
        await until(() => /left out/.test(root.querySelector('#upload-files-note')?.textContent ?? ''), 'the key left out is not said');

        assert.doesNotMatch(box(root).value, /PRIVATE KEY/);

    });

    it('offer no upload to somebody who may only look', async () => {

        const root = await opened('/configuration/ocpp-server/certificates', [ 'certificates:read' ]);

        assert.ok(root.querySelector('#tab-upload') === null && root.querySelector('#upload-all-form') === null, 'a reader is offered the upload');

    });

});
