/**
 * The Client keys page drawn, in a document of happy-dom, against a stand-in
 * station: a subject half typed, and its focus, outlive a certificate being
 * brought in, as a browser takes the focus away while the page is held still;
 * a key made shows its request and empties the form; the remark beside the
 * kinds of key follows the one chosen; keys keep their card while another
 * goes; a certificate refused stays pasted, with why; a template that has a
 * role asks for it, and what was ticked is sent with it; a request goes to the
 * CSMS where the node can send it - and, on the identities page of a node
 * that has one, the keys are a tab of it, the ones still waiting stand among
 * what the node presents, and the upload takes their certificates.
 *
 * Run with `npm test`.
 */

import { asked, change, field, open, refused, submit, type, until, type Asked } from '@node/../test/node.ts';
import { chromeTakesTheFocus } from '@node/../test/dom.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { StationCertificates, StationKey } from '../api/connections.ts';

const { clientKeyAnswers, clientKeysPage, clientKeysTab, waitingKeysSection } = await import('./clientKeys.ts');
const { identitiesPage } = await import('@node/pages/certificates.ts');


let held: StationCertificates;

/** Refuses every certificate brought in where told to. */
let refuseCertificates = false;

/** What the CSMS says to a request sent to it: accepted, unless it is told what to refuse it with. */
let csmsRefuses: string | null = null;

/** What the last key was made with. */
let madeWith: Record<string, unknown> | null = null;

/** What a node that makes requests to templates says beside its keys. */
const profiles: Partial<StationCertificates> = {
    templates:          [ { id: 'ocpp',       name: 'OCPP client',       remark: 'A client certificate and nothing else.', keyUsages: [], extendedKeyUsages: [] },
                          { id: 'modbus-tls', name: 'Modbus/TLS client', remark: 'With the role its holder may play.',     keyUsages: [], extendedKeyUsages: [],
                            role: { label: 'Role', roles: [ 'ReadOnlySunSpec', 'GridServiceSunSpec' ], maxLength: 64 } } ],
    defaultTemplate:    'ocpp',
    keyUsages:          [ { id: 'nonRepudiation', name: 'Non-repudiation', remark: '', algorithms: [ 'ecdsa-p256', 'ed448' ] },
                          { id: 'keyAgreement',   name: 'Key agreement',   remark: '', algorithms: [ 'ecdsa-p256' ] } ],
    extendedKeyUsages:  [ { id: 'serverAuth', name: 'TLS server authentication', remark: '', oid: '1.3.6.1.5.5.7.3.1' } ]
};

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

    if (path === '/configuration/certificates/sign') {
        if (csmsRefuses !== null)
            return refused(502, csmsRefuses);
        const { id } = body as { id: string };
        held = { ...held, entries: held.entries.map(entry => entry.id === id ? { ...entry, sentToCSMSAt: '2026-10-04T12:00:01Z' } : entry) };
        return { said: 'The CSMS accepted the request.', certificates: held };
    }

    if (path === '/configuration/certificates' && method === 'POST') {
        madeWith = body as Record<string, unknown>;
        const { subject } = body as { subject: string };
        const made = aKey('cccc', { subject: `CN=${subject}` });
        held = { ...held, entries: [ made, ...held.entries ] };
        return { id: made.id, csr: made.csr, certificates: held };
    }

    if (path === '/configuration/certificates/import') {
        if (refuseCertificates)
            return refused(400, refusal);
        held = { ...held, entries: held.entries.map(entry => entry.id !== 'aaaa' ? entry : aKey('aaaa', {
                     csr:          undefined,
                     certificate:  { subject: 'CN=aaaa', issuer: 'CN=Fleet CA', serialNumber: '01', notBefore: '2026-10-01T00:00:00Z',
                                     notAfter: '2027-10-01T00:00:00Z', thumbprintSHA256: 'aa'.repeat(32), intermediates: 0,
                                     daysLeft: 361, expired: false, notYetValid: false }
                 })) };
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

async function opened(more: Partial<StationCertificates> = {}): Promise<HTMLElement> {
    held               = { ...aStation(), ...more };
    refuseCertificates = false;
    csmsRefuses        = null;
    madeWith           = null;
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

    it('asks for nothing but a subject and a kind of key of a node that makes requests to no template', async () => {

        const root = await opened();

        assert.equal(root.querySelector('#template'), null);
        assert.equal(root.querySelector('#asks-for'), null);

        subject(root).value = 'cs001.example.org';
        submit(root, '#create-form');
        await until(() => root.querySelector('#just-made') !== null, 'the request is not shown');

        assert.deepEqual(madeWith, { subject: 'cs001.example.org', algorithm: 'ecdsa-p256', keyUsages: [], extendedKeyUsages: [] });

    });

    it('asks for a role where the template has one, and sends the template, the role and what was ticked - but no usage the kind of key cannot have', async () => {

        const root     = await opened(profiles);
        const template = () => field<HTMLSelectElement>(root, '#create-form', 'template');
        const ticked   = (name: string, value: string) => root.querySelector<HTMLInputElement>(`#create-form [name="${name}"][value="${value}"]`)!;

        assert.equal(root.querySelector('#role'), null, 'a role is asked for where the template has none');
        assert.equal(root.querySelector('#template-remark')!.textContent!.trim(), 'A client certificate and nothing else.');

        const typed = subject(root);
        typed.value = 'cs001.example.org';

        change(template(), 'modbus-tls');
        await until(() => root.querySelector('#role') !== null, 'the role the template has is not asked for');

        assert.equal(root.querySelector('#template-remark')!.textContent!.trim(), 'With the role its holder may play.');
        assert.deepEqual([ ...root.querySelectorAll<HTMLOptionElement>('#role-list option') ].map(option => option.value),
                         [ 'ReadOnlySunSpec', 'GridServiceSunSpec' ]);
        assert.ok(subject(root) === typed, 'the subject was made anew');
        assert.equal(typed.value, 'cs001.example.org', 'the subject typed went with the template chosen');

        field(root, '#create-form', 'role').value = 'ReadOnlySunSpec';

        change(ticked('keyUsage', 'nonRepudiation'),      true);
        change(ticked('keyUsage', 'keyAgreement'),        true);
        change(ticked('extendedKeyUsage', 'serverAuth'),  true);
        field(root, '#create-form', 'otherExtendedKeyUsages').value = '1.2.3.4, 1.2.3.5';

        // An Ed448 key agrees on nothing: the usage is off, ticked or not.
        change(kind(root), 'ed448');
        await until(() => ticked('keyUsage', 'keyAgreement').disabled, 'a usage the kind of key cannot have is still on offer');

        assert.equal(ticked('keyUsage', 'nonRepudiation').disabled, false);
        assert.equal(template().value, 'modbus-tls', 'the template chosen went with the kind of key');

        submit(root, '#create-form');
        await until(() => root.querySelector('#just-made') !== null, 'the request is not shown');

        assert.deepEqual(madeWith, {
            subject:            'cs001.example.org',
            algorithm:          'ed448',
            template:           'modbus-tls',
            role:               'ReadOnlySunSpec',
            keyUsages:          [ 'nonRepudiation' ],
            extendedKeyUsages:  [ 'serverAuth', '1.2.3.4', '1.2.3.5' ]
        });

        assert.equal(template().value, 'ocpp', 'the template is still the one chosen');
        assert.equal(root.querySelector('#role'), null, 'the role of the template before is still asked for');

    });

    it('offers to send a request to the CSMS only where the node can, and says beside the key what the CSMS said', async () => {

        const without = await opened();

        assert.ok(without.querySelector('[data-copy-csr="aaaa"]') !== null, 'the request of a key that waits cannot be copied');
        assert.equal(without.querySelector('[data-send-csr]'), null, 'a node that cannot send a request offers to');

        const root   = await opened({ canBeSentToCSMS: true });
        const note   = () => root.querySelector(`[data-key="aaaa"] [data-key-note]`)!.textContent;
        const error  = () => root.querySelector(`[data-key="aaaa"] [data-key-error]`)!.textContent;
        const first  = card(root, 'aaaa')!;
        const second = card(root, 'bbbb');

        csmsRefuses = 'The CSMS answered Rejected to SignCertificate.';

        root.querySelector<HTMLButtonElement>('[data-send-csr="aaaa"]')!.click();
        await until(() => error() === csmsRefuses, 'what the CSMS refused it with is not said beside the key');

        assert.match(first.querySelector('.key-head')!.textContent!, /request out/);

        csmsRefuses = null;

        root.querySelector<HTMLButtonElement>('[data-send-csr="aaaa"]')!.click();
        await until(() => note() === 'The CSMS accepted the request.', 'what the CSMS said is not said beside the key');

        assert.equal(error(), '', 'the refusal before is still said');
        assert.ok(card(root, 'aaaa') === first && card(root, 'bbbb') === second, 'a card was made anew');
        assert.match(first.querySelector('.key-head')!.textContent!, /request sent to the CSMS/);
        assert.match(second!.querySelector('.key-head')!.textContent!, /request out/);

    });

});


describe('the keys on the identities page', () => {

    const store = () => ({
        directory:           'certs',
        trustAnchors:        [],
        credentials:         [ 'tlsIdentity' ],
        recognised:          [],
        kinds:               { tlsIdentity: { description: 'TLS identity', withArticle: 'a TLS identity', group: 'credential',
                                              page: 'identities', usages: [], needsPrivateKey: true } },
        usages:              [],
        certificates:        { tlsIdentity: [] },
        keysAreUnencrypted:  false
    });

    /** A node that keeps identities, and makes keys as well. */
    function node(asking: Asked): unknown {

        const { method, path } = asking;

        if (path === '/certificates')
            return store();

        if (method === 'POST' && path === '/certificates/inspect')
            return {
                certificates: [ { pem: '', id: '1'.repeat(16), thumbprint: '1'.repeat(64), label: 'CN=aaaa', subject: 'CN=aaaa', issuer: 'CN=Fleet CA',
                                  notBefore: '2026-10-01T00:00:00Z', notAfter: '2027-10-01T00:00:00Z', keyAlgorithm: 'ECDSA P-256',
                                  hasPrivateKey: false, chainLength: 0, isCA: false, selfSigned: false, inStoreAs: [],
                                  unsuitable: { tlsIdentity: 'A TLS identity has to carry its private key.' } } ],
                pem: (asking.body as { pem: string }).pem
            };

        return station(asking);

    }

    const options = { tabs: clientKeysTab, sections: waitingKeysSection, answers: clientKeyAnswers() };

    async function identities(permissions = [ 'certificates:read', 'certificates:edit', 'connections:read', 'connections:edit' ],
                              path        = '/configuration/identities'): Promise<HTMLElement> {

        held               = { ...aStation(), canBeSentToCSMS: true };
        refuseCertificates = false;
        csmsRefuses        = null;

        return open(identitiesPage(options), path, permissions, node,
                    root => root.querySelector('#panel-usage') !== null);

    }

    const panel   = (root: HTMLElement, id: string) => root.querySelector<HTMLElement>(`#panel-${id}`)!;
    const waiting = (root: HTMLElement) => [ ...root.querySelectorAll('#waiting-keys [data-key]') ].map(key => key.getAttribute('data-key'));


    it('are a tab of it, with the ones that wait for their certificate among what the node presents - to copy, or to send to the CSMS', async () => {

        const root = await identities();

        assert.deepEqual([ ...root.querySelectorAll('[role="tab"]') ].map(tab => tab.id),
                         [ 'tab-usage', 'tab-all', 'tab-client-keys', 'tab-upload' ]);

        assert.ok(panel(root, 'usage').querySelector('#waiting-keys') !== null, 'the keys that wait are not among what the node presents');
        assert.deepEqual(waiting(root), [ 'aaaa', 'bbbb' ]);

        const actions = [ ...root.querySelectorAll('#waiting-keys [data-key="aaaa"] button') ].map(button => button.textContent!.trim());

        assert.deepEqual(actions, [ 'Copy CSR', 'Send to CSMS' ], 'a key that waits has more, or less, to do with it than its request');

        assert.ok(panel(root, 'client-keys').querySelector('#create-form') !== null, 'no key can be made on the tab');
        assert.equal(panel(root, 'client-keys').querySelectorAll('[data-remove]').length, 2, 'the keys are not all on their tab');
        assert.equal(root.querySelectorAll('#import-form').length, 1, 'a certificate is brought in in two places');

        // Sent from among the identities, it says so there - and on the tab.
        root.querySelector<HTMLButtonElement>('#waiting-keys [data-send-csr="aaaa"]')!.click();
        await until(() => root.querySelector('#waiting-keys [data-key="aaaa"] [data-key-note]')?.textContent === 'The CSMS accepted the request.',
                    'what the CSMS said is not said beside the key');

        assert.match(panel(root, 'client-keys').querySelector('[data-key="aaaa"]')!.closest('.card')!.querySelector('.key-head')!.textContent!,
                     /request sent to the CSMS/);

        // And from there to where one is made.
        assert.equal(panel(root, 'client-keys').hidden, true);

        root.querySelector<HTMLButtonElement>('#make-a-key')!.click();
        await until(() => !panel(root, 'client-keys').hidden, 'the tab where a key is made is not shown');

    });

    it('take the certificate uploaded without a key, which is then no key that waits', async () => {

        const root = await identities(undefined, '/configuration/identities?tab=upload');
        const box  = root.querySelector<HTMLTextAreaElement>('#upload-pem')!;

        type(box, '-----BEGIN CERTIFICATE-----\nMIIB\n-----END CERTIFICATE-----');
        await until(() => root.querySelector('#upload-found .an-answer') !== null, 'the upload does not say the box holds an answer');

        submit(root, '#import-form');
        await until(() => root.querySelector('#import-note')?.textContent === 'Taken in, for the key aaaa.', 'the certificate was not taken in');

        assert.equal(box.value, '', 'the certificate taken in is still pasted');
        assert.deepEqual(waiting(root), [ 'bbbb' ], 'the key whose certificate came still waits');
        assert.match(panel(root, 'client-keys').querySelector('[data-key="aaaa"]')!.closest('.card')!.querySelector('.key-head')!.textContent!,
                     /361 days left/);

    });

    it('are not shown to, nor asked of the node for, somebody who may not read what it dials with', async () => {

        const root = await identities([ 'certificates:read', 'certificates:edit' ]);

        assert.deepEqual([ ...root.querySelectorAll('[role="tab"]') ].map(tab => tab.id), [ 'tab-usage', 'tab-all', 'tab-upload' ]);
        assert.equal(root.querySelector('#waiting-keys'), null);
        assert.equal(asked.some(one => one.path.startsWith('/configuration/certificates')), false, 'the node was asked for keys nobody may see');

    });

});
