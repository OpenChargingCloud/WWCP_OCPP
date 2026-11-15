/**
 * The SSH keys a node that dials signs in with, among what it presents on
 * the page of its client identities, drawn in a document of happy-dom
 * against a stand-in node: its own key and the ones somebody made, each with
 * the line for a server and with where it signs in; a key is made by whoever
 * may make keys, and is on the page at once; one a connection signs in with
 * is not removed, one nothing signs in with is - after a yes; the node's own
 * stays; and somebody who may not see the connections is shown the rest of
 * the page all the same.
 *
 * Run with `npm test`.
 */

import { asked, open, refused, type, until, type Asked } from '@node/../test/node.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { CertificateStore } from '@node/api/client.ts';
import type { SshKey, StationConnection, StationConnections } from './api/connections.ts';

const { identitiesPage }  = await import('@node/pages/certificates.ts');
const { sshKeysSection }  = await import('./sshKeys.ts');


let held:              StationConnections;
let mayNotSeeThem      = false;
let confirmed          = true;

globalThis.confirm = () => confirmed;

const aStore = (): CertificateStore => ({
    directory:           'certs',
    trustAnchors:        [],
    credentials:         [ 'tlsIdentity' ],
    recognised:          [],
    kinds:               { tlsIdentity: { description: 'TLS identity', withArticle: 'a TLS identity', group: 'credential', page: 'identities',
                                          trustAnchor: false, needsPrivateKey: true, hasUsages: false, usages: [] } },
    usages:              [],
    certificates:        { tlsIdentity: [] },
    keysAreUnencrypted:  false
} as unknown as CertificateStore);

const aKey = (id: string, label: string, more: Partial<SshKey> = {}): SshKey => ({
    id, label, own: false, made: true, publicKey: `ssh-ed25519 AAAA${id} chargingstation@bench`, fingerprint: `SHA256:${id}`,
    path: `connections/ssh/keys/${id}`, createdAt: '2026-11-01T10:00:00Z', ...more
});

const overSSH = (id: string, more: Partial<StationConnection> = {}): StationConnection => ({
    id, description: `CSMS ${id}`, url: 'ssh://cs001@csms.example.org:2222', connectionType: 'CSMS', ocppVersion: 'OCPP2.1',
    autoConnect: true, secure: false, transport: 'ssh', createdAt: '2026-10-01T10:00:00Z', ...more
});

function aStation(): StationConnections {
    return {
        directory:              'connections',
        authentications:        [],
        connections:            [ overSSH('c1'), overSSH('c2', { identities: [ { id: 'k1' } ] }) ],
        certificates:           [],
        sshKeys:                [ aKey('own', "This charging station's own key", { own: true, made: false, publicKey: null, fingerprint: null, createdAt: null }),
                                  aKey('k1', 'CSMS over SSH'),
                                  aKey('k2', 'The spare') ],
        connectionTypes:        [ 'CSMS' ],
        ocppVersions:           [ 'OCPP2.1' ],
        maxDescriptionLength:   120,
        minSharedSecretLength:  16,
        secretsAreReadable:     false,
        totpDefaults:           { validitySeconds: 30, length: 12, alphabet: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ234567', hashAlgorithm: 'SHA256' },
        timestamp:              '2026-10-04T12:00:00Z',
        states:                 {}
    };
}

function node({ method, path, body }: Asked): unknown {

    if (path === '/certificates')
        return aStore();

    if (path === '/configuration/connections' && method === 'GET')
        return mayNotSeeThem ? refused(403, 'That needs a role that may read the connections.') : held;

    if (path === '/configuration/sshKeys') {
        const label = (body as { label: string }).label;
        if (label === '')
            return refused(400, 'Say what the key is for: it is told from the others by that and nothing else.');
        held = { ...held, sshKeys: [ held.sshKeys![0]!, aKey('k3', label), ...held.sshKeys!.slice(1) ] };
        return { id: 'k3', connections: held };
    }

    if (path === '/configuration/sshKeys/remove') {
        const id = (body as { id: string }).id;
        held = { ...held, sshKeys: held.sshKeys!.filter(key => key.id !== id) };
        return held;
    }

    return undefined;

}

async function opened(permissions = [ 'certificates:read', 'certificates:run', 'connections:read' ]): Promise<HTMLElement> {
    held       = aStation();
    confirmed  = true;
    return open(identitiesPage({ sections: context => [ sshKeysSection(context) ] }), '/configuration/identities', permissions, node,
                root => root.querySelector('#panel-usage') !== null);
}

const keys    = (root: HTMLElement) => [ ...root.querySelectorAll<HTMLElement>('#ssh-keys [data-ssh-key]') ].map(card => card.dataset['sshKey']);
const cardOf  = (root: HTMLElement, id: string) => root.querySelector<HTMLElement>(`#ssh-keys [data-ssh-key="${id}"]`)!;


describe('the SSH keys of a node that dials, among its client identities', () => {

    it('shows its own key and the ones somebody made, each with its line and where it signs in', async () => {

        const root = await opened();

        assert.deepEqual(keys(root), [ 'own', 'k1', 'k2' ]);

        assert.match(cardOf(root, 'own').textContent!, /own/);
        assert.match(cardOf(root, 'own').textContent!, /Not made yet/);
        assert.match(cardOf(root, 'own').textContent!.replace(/\s+/g, ' '), /signs in at CSMS c1/, 'a connection that names no key does not sign in with the own one');
        assert.equal(cardOf(root, 'own').querySelector('[data-remove-ssh-key]'), null, 'the own key is offered to be removed');

        assert.equal(cardOf(root, 'k1').querySelector<HTMLTextAreaElement>('[data-ssh-public-key="k1"]')!.value, 'ssh-ed25519 AAAAk1 chargingstation@bench');
        assert.match(cardOf(root, 'k1').textContent!.replace(/\s+/g, ' '), /signs in at CSMS c2/);
        assert.equal(cardOf(root, 'k1').querySelector<HTMLButtonElement>('[data-remove-ssh-key="k1"]')!.disabled, true, 'a key a connection signs in with can be removed');

        assert.match(cardOf(root, 'k2').textContent!, /signs in nowhere/);
        assert.equal(cardOf(root, 'k2').querySelector<HTMLButtonElement>('[data-remove-ssh-key="k2"]')!.disabled, false);

    });

    it('makes a key that is on the page at once, says why not where the node refuses, and removes one only after a yes', async () => {

        const root   = await opened();
        const label  = () => root.querySelector<HTMLInputElement>('#ssh-key-label')!;
        const make   = () => root.querySelector<HTMLButtonElement>('#make-an-ssh-key')!.click();

        make();
        await until(() => /what the key is for/.test(root.querySelector('#ssh-key-error')!.textContent!), 'why no key was made is not said');

        type(label(), 'CSMS over SSH, from January');
        make();
        await until(() => keys(root).includes('k3'), 'the key that was made is not on the page');

        assert.deepEqual(asked.filter(one => one.path === '/configuration/sshKeys').at(-1)!.body, { label: 'CSMS over SSH, from January' });
        assert.equal(label().value, '', 'what the key was called is still typed');
        assert.match(cardOf(root, 'k3').textContent!, /CSMS over SSH, from January/);

        confirmed = false;
        cardOf(root, 'k2').querySelector<HTMLButtonElement>('[data-remove-ssh-key="k2"]')!.click();

        assert.equal(asked.some(one => one.path === '/configuration/sshKeys/remove'), false, 'a key was removed without a yes');

        confirmed = true;
        cardOf(root, 'k2').querySelector<HTMLButtonElement>('[data-remove-ssh-key="k2"]')!.click();
        await until(() => !keys(root).includes('k2'), 'the key was not removed');

        assert.deepEqual(keys(root), [ 'own', 'k3', 'k1' ]);

    });

    it('offers nobody to make a key who may not, and shows the page without the keys to somebody who may not see the connections', async () => {

        const reader = await opened([ 'certificates:read', 'connections:read' ]);

        assert.deepEqual(keys(reader), [ 'own', 'k1', 'k2' ]);
        assert.equal(reader.querySelector('#make-an-ssh-key'), null, 'somebody who may not make keys is offered to');
        assert.equal(cardOf(reader, 'k2').querySelector<HTMLButtonElement>('[data-remove-ssh-key="k2"]')!.disabled, true);

        mayNotSeeThem = true;

        const without = await opened([ 'certificates:read' ]);

        mayNotSeeThem = false;

        assert.equal(without.querySelector('#ssh-keys'), null);
        assert.ok(without.querySelector('#panel-usage') !== null, 'the page went with the keys it could not show');
        assert.equal(without.querySelector('.error-box'), null);

    });

});
