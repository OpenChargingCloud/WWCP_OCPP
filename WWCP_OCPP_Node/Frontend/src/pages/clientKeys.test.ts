/**
 * The Client keys page drawn, in a document of happy-dom, against a stand-in
 * station: a subject half typed, and its focus, outlive a certificate being
 * brought in, as a browser takes the focus away while the page is held still;
 * a key made shows its request and empties the form; the remark beside the
 * kinds of key follows the one chosen; keys keep their card while another
 * goes; a certificate refused stays pasted, with why.
 *
 * Run with `npm test`.
 */

import { change, field, open, refused, submit, until, type Asked } from '@node/../test/node.ts';
import { chromeTakesTheFocus } from '@node/../test/dom.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { StationCertificates, StationKey } from '../api/connections.ts';

const { clientKeysPage } = await import('./clientKeys.ts');


let held: StationCertificates;

/** Refuses every certificate brought in where told to. */
let refuseCertificates = false;

const refusal = 'None of the certificates in this file belongs to a key of this charging station. ' +
                'A certificate is only usable here if it answers a signing request made here.';

function aKey(id: string, more: Partial<StationKey> = {}): StationKey {
    return {
        id,
        algorithm:    'ECDSA P-256',
        createdAt:    '2026-10-01T10:00:00Z',
        subject:      `CN=${id}`,
        inUse:        false,
        canBeHeldUp:  true,
        csr:          `-----BEGIN CERTIFICATE REQUEST-----\n${id}\n-----END CERTIFICATE REQUEST-----`,
        ...more
    };
}

function aStation(): StationCertificates {
    return {
        directory:             'certificates',
        now:                   '2026-10-04T12:00:00Z',
        inUseId:               null,
        entries:               [ aKey('aaaa'), aKey('bbbb') ],
        algorithms:            [ { id: 'ecdsa-p256', name: 'ECDSA P-256', remark: 'What every back end takes.' },
                                 { id: 'ed448',      name: 'Ed448',       remark: 'No key object in .NET today.' } ],
        defaultAlgorithm:      'ecdsa-p256',
        maxSubjectLength:      256,
        canImportPrivateKeys:  false
    };
}

function station({ method, path, body }: Asked): unknown {

    if (path === '/configuration/certificates' && method === 'POST') {
        const { subject } = body as { subject: string };
        const made = aKey('cccc', { subject: `CN=${subject}` });
        held = { ...held, entries: [ made, ...held.entries ] };
        return { id: made.id, csr: made.csr, certificates: held };
    }

    if (path === '/configuration/certificates/import') {
        if (refuseCertificates)
            return refused(400, refusal);
        return { id: 'aaaa', warnings: [], certificates: held };
    }

    if (path === '/configuration/certificates/remove') {
        const { id } = body as { id: string };
        held = { ...held, entries: held.entries.filter(entry => entry.id !== id) };
        return held;
    }

    if (path === '/configuration/certificates')
        return held;

    return undefined;

}

async function opened(): Promise<HTMLElement> {
    held               = aStation();
    refuseCertificates = false;
    return open(clientKeysPage, '/configuration/client-keys', [ 'connections:read', 'connections:edit' ],
                station, root => root.querySelector('#create-form') !== null);
}

const subject  = (root: HTMLElement) => field(root, '#create-form', 'subject');
const pasted   = (root: HTMLElement) => field<HTMLTextAreaElement>(root, '#import-form', 'pem');
const kind     = (root: HTMLElement) => field<HTMLSelectElement>(root, '#create-form', 'algorithm');
const remark   = (root: HTMLElement) => root.querySelector('#algorithm-remark')?.textContent?.trim();
const card     = (root: HTMLElement, id: string) => root.querySelector<HTMLButtonElement>(`[data-remove="${id}"]`)?.closest('.card');


describe('the Client keys page', () => {

    it('keeps a subject half typed, and its focus, while a certificate is brought in', async () => {

        const root    = await opened();
        const browser = chromeTakesTheFocus(root);
        const typed   = subject(root);

        typed.value = 'cs001.exam';
        typed.focus();

        pasted(root).value = '-----BEGIN CERTIFICATE-----\nMIIB\n-----END CERTIFICATE-----';
        submit(root, '#import-form');
        await until(() => root.querySelector('#import-note')?.textContent === 'Taken in.', 'the certificate was not taken in');
        browser.disconnect();

        assert.ok(subject(root) === typed,           'the field was made anew');
        assert.equal(typed.value,                    'cs001.exam');
        assert.ok(document.activeElement === typed,  'the focus went');
        assert.equal(pasted(root).value,             '', 'the certificate taken in is still pasted');

    });

    it('shows the request of a key made, with the form empty and the kind of key back at the suggested one', async () => {

        const root = await opened();

        subject(root).value = 'cs001.example.org';
        change(kind(root), 'ed448');

        submit(root, '#create-form');
        await until(() => root.querySelector('#just-made') !== null, 'the request is not shown');

        assert.equal(root.querySelector<HTMLTextAreaElement>('#just-made')!.value, held.entries[0].csr);
        assert.equal(subject(root).value,  '',            'the form still holds what was made');
        assert.equal(kind(root).value,     'ecdsa-p256',  'the kind of key is still the one chosen');
        assert.equal(remark(root),         'What every back end takes.');

    });

    it('says beside the kinds of key what is worth knowing about the one chosen', async () => {

        const root = await opened();

        assert.equal(remark(root), 'What every back end takes.');

        change(kind(root), 'ed448');
        await until(() => remark(root) === 'No key object in .NET today.', 'the remark is about another kind of key');

        assert.equal(kind(root).value, 'ed448', 'the kind chosen went');

    });

    it('keeps the card of a key while the one before it is removed', async () => {

        const root   = await opened();
        const second = card(root, 'bbbb');

        root.querySelector<HTMLButtonElement>('[data-remove="aaaa"]')!.click();
        await until(() => !card(root, 'aaaa'), 'the key was not removed');

        assert.ok(card(root, 'bbbb') === second, 'the card was made anew');

    });

    it('keeps a certificate the station refused pasted, and says why', async () => {

        const root = await opened();

        refuseCertificates = true;
        pasted(root).value = '-----BEGIN CERTIFICATE-----\nMIIC\n-----END CERTIFICATE-----';

        submit(root, '#import-form');
        await until(() => root.querySelector('#import-error')?.textContent !== '', 'the refusal was not said');

        assert.equal(root.querySelector('#import-error')!.textContent, refusal);
        assert.equal(pasted(root).value, '-----BEGIN CERTIFICATE-----\nMIIC\n-----END CERTIFICATE-----', 'what was pasted went');

    });

});
