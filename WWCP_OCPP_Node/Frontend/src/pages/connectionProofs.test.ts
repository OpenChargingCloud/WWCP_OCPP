/**
 * What a connection proves itself with, on the Connections page, drawn in a
 * document of happy-dom against a stand-in station: what there is to choose
 * follows the scheme of the URL as it is typed - nothing, a login or client
 * certificates over wss://, no certificate over ws://, and over ssh:// the
 * SSH keys and nothing else; certificates and keys are ticked, several, each
 * from a time on, and go to the station in the order ticked; a login typed
 * into the form of a connection is written down before the connection is,
 * and once; a connection says on its card what it names; and the credentials
 * are a tab of the page, told of a login written down beside it.
 *
 * Run with `npm test`.
 */

import { asked, change, field, leave, open, refused, submit, type, until, type Asked } from '@node/../test/node.ts';

import { strict as assert }     from 'node:assert';
import { after, describe, it }  from 'node:test';

import type { ConnectionToSave, LoginToSave, StationConnection, StationConnections } from '../api/connections.ts';

const { connectionsPage } = await import('./connections.ts');


let held: StationConnections;

/** What the page sent the last time it wrote a connection down, or saved one. */
let lastAdded: ConnectionToSave | null = null;
let lastSaved: ConnectionToSave | null = null;

/** The logins the page wrote down. */
let logins: LoginToSave[] = [];

/** Whether the station refuses the next connection written down. */
let refuseTheConnection = false;

function aConnection(id: string, more: Partial<StationConnection> = {}): StationConnection {
    return { id, description: `CSMS ${id}`, url: `wss://csms.example.org/${id}`, connectionType: 'CSMS', ocppVersion: 'OCPP2.1',
             autoConnect: true, secure: true, createdAt: '2026-10-01T10:00:00Z', ...more };
}

function aStation(more: Partial<StationConnections> = {}): StationConnections {
    return {
        directory:              'connections',
        authentications:        [ { id: 'l1', description: 'CSMS login', kind: 'basic', login: 'cs001', createdAt: '2026-10-01T10:00:00Z', hasSecret: true } ],
        connections:            [ aConnection('c1', { identities: [ { id: 'aaaa' }, { id: 'bbbb', from: '2027-01-31T00:00:00Z' } ], certificateId: 'aaaa' }),
                                  aConnection('c2', { url: 'ssh://cs001@csms.example.org:2222', secure: false, transport: 'ssh',
                                                      identities: [ { id: 'k1' } ] }) ],
        certificates:           [ { id: 'aaaa', label: 'This year',  subject: 'CN=cs001', algorithm: 'ECDSA P-256', hasCertificate: true, canBeHeldUp: true },
                                  { id: 'bbbb', label: 'Next year',  subject: 'CN=cs001', algorithm: 'ECDSA P-256', hasCertificate: true, canBeHeldUp: true } ],
        sshKeys:                [ { id: 'own', label: "This charging station's own key", own: true, made: true,
                                    publicKey: 'ssh-ed25519 AAAAOwn chargingstation@bench', fingerprint: 'SHA256:own', path: 'connections/ssh/id_ed25519', createdAt: null },
                                  { id: 'k1',  label: 'CSMS over SSH', own: false, made: true,
                                    publicKey: 'ssh-ed25519 AAAAK1 chargingstation@bench',  fingerprint: 'SHA256:k1',  path: 'connections/ssh/keys/k1', createdAt: '2026-11-01T10:00:00Z' } ],
        connectionTypes:        [ 'CSMS', 'CSMSBackup', 'LocalController' ],
        ocppVersions:           [ 'OCPP2.1', 'OCPP1.6' ],
        maxDescriptionLength:   120,
        minSharedSecretLength:  16,
        secretsAreReadable:     false,
        totpDefaults:           { validitySeconds: 30, length: 12, alphabet: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ234567', hashAlgorithm: 'SHA256' },
        timestamp:              '2026-10-04T12:00:00Z',
        states:                 {},
        ...more
    };
}

function station({ method, path, body }: Asked): unknown {

    if (path === '/configuration/authentications' && method === 'POST') {
        const login = body as LoginToSave;
        logins.push(login);
        const id = `l${held.authentications.length + 1}`;
        held = { ...held, authentications: [ { id, description: login.description, kind: login.kind, login: login.login,
                                               createdAt: '2026-11-15T10:00:00Z', hasSecret: login.secret !== '' },
                                             ...held.authentications ] };
        return { id, connections: held };
    }

    if (path === '/configuration/connections' && method === 'POST') {
        lastAdded = body as ConnectionToSave;
        if (refuseTheConnection)
            return refused(400, 'A connection needs somewhere to go.');
        const made = aConnection('c9', { description: lastAdded.description, url: lastAdded.url });
        held = { ...held, connections: [ made, ...held.connections ] };
        return { id: made.id, connections: held };
    }

    if (path === '/configuration/connections/update') {
        lastSaved = body as ConnectionToSave;
        return held;
    }

    if (path === '/status/connections')
        return { timestamp: '2026-10-04T12:00:05Z', states: {} };

    if (path === '/configuration/connections')
        return held;

    return undefined;

}

async function opened(path = '/configuration/connections', make: () => StationConnections = aStation): Promise<HTMLElement> {
    held                 = make();
    lastAdded            = null;
    lastSaved            = null;
    logins               = [];
    refuseTheConnection  = false;
    return open(connectionsPage, path, [ 'connections:read', 'connections:edit', 'connections:run' ],
                station, root => root.querySelector('#panel-new #add-form') !== null);
}

const add     = (root: HTMLElement) => root.querySelector<HTMLFormElement>('#panel-new #add-form')!;
const editOf  = (root: HTMLElement, id: string) => root.querySelector<HTMLFormElement>(`form[data-edit="${id}"]`)!;
const ways    = (form: HTMLFormElement) => [ ...form.querySelectorAll<HTMLOptionElement>('select[name="proves"] option') ].map(option => option.value);
const ticked  = (form: HTMLFormElement) => [ ...form.querySelectorAll<HTMLInputElement>('input[name="identity"]') ].filter(box => box.checked).map(box => box.value);
const boxOf   = (form: HTMLFormElement, id: string) => form.querySelector<HTMLInputElement>(`input[name="identity"][value="${id}"]`)!;
const fromOf  = (form: HTMLFormElement, id: string) => form.querySelector<HTMLInputElement>(`input[name="from:${id}"]`)!;
const typeURL = (form: HTMLFormElement, url: string) => type(form.querySelector<HTMLInputElement>('[name="url"]')!, url);


describe('what a connection proves itself with', () => {

    after(leave);

    it('offers what the scheme of its URL allows, as it is typed: a login always, certificates with TLS, and over SSH the keys and nothing else', async () => {

        const root = await opened();
        const form = add(root);

        assert.deepEqual(ways(form), [ '', 'auth:l1', 'new:basic', 'new:totp' ], 'a certificate is offered before anybody said where it goes');

        typeURL(form, 'wss://csms.example.org/cs001');
        await until(() => ways(form).includes('tls'), 'a wss:// connection is not offered its client certificates');

        typeURL(form, 'ws://bench.local/cs001');
        await until(() => !ways(form).includes('tls'), 'a ws:// connection is offered a certificate it has no handshake to show in');
        assert.match(form.querySelector('[data-proves-hint]')!.textContent!, /no TLS handshake/);

        typeURL(form, 'ssh://cs001@csms.example.org:2222');
        await until(() => form.querySelector('[data-identities="ssh"]') !== null, 'an ssh:// connection is not shown the keys it signs in with');

        assert.deepEqual(ways(form), [ '', 'auth:l1', 'new:basic', 'new:totp' ], 'an ssh:// connection is offered something beside its keys and a password');
        assert.deepEqual([ ...form.querySelectorAll<HTMLInputElement>('input[name="identity"]') ].map(box => box.value), [ 'own', 'k1' ]);
        assert.deepEqual(ticked(form), []);
        assert.ok(add(root) === form, 'the form was made anew, and what was typed into it with it');
        assert.equal(form.querySelector<HTMLInputElement>('[name="url"]')!.value, 'ssh://cs001@csms.example.org:2222');

    });

    it('says what there is to do where nothing is written down that it could prove itself with', async () => {

        const root = await opened(undefined, () => aStation({ authentications: [], certificates: [], connections: [] }));

        assert.match(add(root).querySelector('[data-proves-hint]')!.textContent!, /Nothing is written down yet/);
        assert.deepEqual(ways(add(root)), [ '', 'new:basic', 'new:totp' ]);

    });

    it('writes down the certificates ticked, each from the time beside it', async () => {

        const root = await opened();
        const form = add(root);

        type(field(root, '#panel-new #add-form', 'description'), 'CSMS, main');
        typeURL(form, 'wss://csms.example.org/cs001');
        await until(() => ways(form).includes('tls'), 'the certificates are not offered');

        change(form.querySelector<HTMLSelectElement>('select[name="proves"]')!, 'tls');
        await until(() => form.querySelector('[data-identities="tls"]') !== null, 'the certificates are not shown');

        change(boxOf(form, 'aaaa'), true);
        change(boxOf(form, 'bbbb'), true);
        type(fromOf(form, 'bbbb'), '2027-01-31T00:00');

        submit(root, '#panel-new #add-form');
        await until(() => lastAdded !== null, 'the connection was not written down');

        assert.deepEqual(lastAdded!.identities, [ { id: 'aaaa' }, { id: 'bbbb', from: new Date('2027-01-31T00:00').toISOString() } ]);
        assert.equal(lastAdded!.authenticationId, null);
        assert.equal(lastAdded!.certificateId,    null);

    });

    it('shows what a connection names, ticked and with its time, and says it on its card - and saves the keys of an ssh:// one', async () => {

        const root = await opened();

        assert.match(root.querySelector('[data-proves="c1"]')!.textContent!.replace(/\s+/g, ' '), /TLS identities This year, Next year from /);
        assert.match(root.querySelector('[data-proves="c2"]')!.textContent!.replace(/\s+/g, ' '), /The SSH key CSMS over SSH/);

        const tls = editOf(root, 'c1');

        assert.equal(tls.querySelector<HTMLSelectElement>('select[name="proves"]')!.value, 'tls');
        assert.deepEqual(ticked(tls), [ 'aaaa', 'bbbb' ]);
        assert.notEqual(fromOf(tls, 'bbbb').value, '', 'the time a certificate is shown from is not in its field');
        assert.equal(fromOf(tls, 'aaaa').value, '');

        const ssh = editOf(root, 'c2');

        assert.deepEqual(ticked(ssh), [ 'k1' ]);

        change(boxOf(ssh, 'own'), true);
        submit(root, 'form[data-edit="c2"]');
        await until(() => lastSaved !== null, 'the connection was not saved');

        assert.deepEqual(lastSaved!.identities, [ { id: 'own' }, { id: 'k1' } ]);
        assert.equal(lastSaved!.authenticationId, null);

        // None ticked is the node's own key, and said as nothing.
        lastSaved = null;
        change(boxOf(ssh, 'own'), false);
        change(boxOf(ssh, 'k1'),  false);
        submit(root, 'form[data-edit="c2"]');
        await until(() => lastSaved !== null, 'the connection was not saved');

        assert.deepEqual(lastSaved!.identities, []);

    });

    it('writes down the password an ssh:// connection signs in with, one that is there or one typed with it, and takes it away again', async () => {

        const root = await opened(undefined, () => aStation({
                                      authentications: [ { id: 'l1', description: 'CSMS login', kind: 'basic', login: 'cs001', createdAt: '2026-10-01T10:00:00Z', hasSecret: true },
                                                         { id: 't1', description: 'One-time',   kind: 'totp',  login: 'cs001', createdAt: '2026-10-01T10:00:00Z', hasSecret: true } ]
                                  }));
        const ssh  = editOf(root, 'c2');

        assert.deepEqual(ways(ssh), [ '', 'auth:l1', 'auth:t1', 'new:basic', 'new:totp' ], 'a one-time password is not offered over SSH');
        assert.match(ssh.querySelector('option[value="auth:t1"]')!.textContent!, /one-time password/);
        assert.equal(ssh.querySelector<HTMLSelectElement>('select[name="proves"]')!.value, '');

        change(ssh.querySelector<HTMLSelectElement>('select[name="proves"]')!, 'auth:l1');
        submit(root, 'form[data-edit="c2"]');
        await until(() => lastSaved !== null, 'the connection was not saved');

        assert.equal(lastSaved!.authenticationId, 'l1');
        assert.deepEqual(lastSaved!.identities, [ { id: 'k1' } ], 'the keys went with the password');

        // One with a password says so, and has it chosen; "None" takes it away.
        const withOne = await opened(undefined, () => aStation({
                                         connections: [ aConnection('c2', { url: 'ssh://cs001@csms.example.org:2222', secure: false, transport: 'ssh', authenticationId: 'l1' }) ]
                                     }));
        const form    = editOf(withOne, 'c2');

        assert.match(withOne.querySelector('[data-proves="c2"]')!.textContent!.replace(/\s+/g, ' '), /SSH key, and the password of CSMS login/);
        assert.equal(form.querySelector<HTMLSelectElement>('select[name="proves"]')!.value, 'auth:l1');

        change(form.querySelector<HTMLSelectElement>('select[name="proves"]')!, '');
        submit(withOne, 'form[data-edit="c2"]');
        await until(() => lastSaved !== null, 'the connection was not saved');

        assert.equal(lastSaved!.authenticationId, null);

        // And a new one, typed with a new connection.
        const fresh = add(withOne);

        type(field(withOne, '#panel-new #add-form', 'description'), 'CSMS over SSH');
        typeURL(fresh, 'ssh://cs002@csms.example.org:2222');
        await until(() => fresh.querySelector('[data-identities="ssh"]') !== null, 'the keys are not shown');

        change(fresh.querySelector<HTMLSelectElement>('select[name="proves"]')!, 'new:basic');
        await until(() => fresh.querySelector('[data-new-login]') !== null, 'the fields of a new login are not shown');

        type(fresh.querySelector<HTMLInputElement>('[name="newLogin"]')!,  'cs002');
        type(fresh.querySelector<HTMLInputElement>('[name="newSecret"]')!, 'a password');

        submit(withOne, '#panel-new #add-form');
        await until(() => lastAdded !== null, 'the connection was not written down');

        assert.deepEqual(logins, [ { description: 'CSMS over SSH', kind: 'basic', login: 'cs002', secret: 'a password' } ]);
        assert.equal(lastAdded!.authenticationId, 'l2');
        assert.deepEqual(lastAdded!.identities, []);

    });

    it('writes down that an ssh:// connection offers no key, and a one-time password as what it signs in with', async () => {

        const root = await opened(undefined, () => aStation({
                                      authentications: [ { id: 't1', description: 'One-time', kind: 'totp', login: 'cs001', createdAt: '2026-10-01T10:00:00Z',
                                                           hasSecret: true, tlsChannelBinding: true } ],
                                      connections:     [ aConnection('c2', { url: 'ssh://cs001@csms.example.org:2222', secure: false, transport: 'ssh',
                                                                             identities: [ { id: 'k1' } ] }),
                                                         aConnection('c3', { url: 'ssh://cs001@csms.example.org:2222', secure: false, transport: 'ssh',
                                                                             identities: [ { id: 'none' } ], authenticationId: 't1' }) ]
                                  }));

        // One that offers none says so, has it ticked, and shows no key as gone.
        const none = editOf(root, 'c3');

        assert.match(root.querySelector('[data-proves="c3"]')!.textContent!.replace(/\s+/g, ' '), /No SSH key, and the one-time password of One-time/);
        assert.equal(none.querySelector<HTMLInputElement>('[name="noKey"]')!.checked, true);
        assert.deepEqual([ ...none.querySelectorAll<HTMLInputElement>('input[name="identity"]') ].map(box => box.value), [ 'own', 'k1' ]);
        assert.match(none.querySelector('option[value="auth:t1"]')!.textContent!, /bound to the session/);

        // And one that offers a key is told to offer none: whatever is ticked.
        const ssh = editOf(root, 'c2');

        assert.equal(ssh.querySelector<HTMLInputElement>('[name="noKey"]')!.checked, false);

        change(ssh.querySelector<HTMLInputElement>('[name="noKey"]')!, true);
        change(ssh.querySelector<HTMLSelectElement>('select[name="proves"]')!, 'auth:t1');
        submit(root, 'form[data-edit="c2"]');
        await until(() => lastSaved !== null, 'the connection was not saved');

        assert.deepEqual(lastSaved!.identities, [ { id: 'none' } ]);
        assert.equal(lastSaved!.authenticationId, 't1');

    });

    it('writes a login typed into the form of a connection down before the connection, and once', async () => {

        const root = await opened();
        const form = add(root);

        type(field(root, '#panel-new #add-form', 'description'), 'The spare');
        typeURL(form, 'wss://spare.example.org/cs001');

        change(form.querySelector<HTMLSelectElement>('select[name="proves"]')!, 'new:basic');
        await until(() => form.querySelector('[data-new-login]') !== null, 'the fields of a new login are not shown');

        type(form.querySelector<HTMLInputElement>('[name="newLogin"]')!,  'cs001');
        type(form.querySelector<HTMLInputElement>('[name="newSecret"]')!, 'a password');

        refuseTheConnection = true;
        submit(root, '#panel-new #add-form');
        await until(() => root.querySelector('#panel-new #add-error')?.textContent !== '', 'why the connection was refused is not said');

        assert.deepEqual(logins, [ { description: 'The spare', kind: 'basic', login: 'cs001', secret: 'a password' } ],
                         'the login was not written down as it was typed, called what the connection is');
        assert.equal(lastAdded!.authenticationId, 'l2');
        assert.equal(form.querySelector<HTMLSelectElement>('select[name="proves"]')!.value, 'auth:l2', 'the login written down is not the one chosen now');
        assert.equal(form.querySelector('[data-new-login]'), null);
        assert.equal(form.querySelector<HTMLInputElement>('[name="url"]')!.value, 'wss://spare.example.org/cs001', 'what was typed went with the refusal');

        // Again, with what the station refused put right: the login that is there, and no second one.
        refuseTheConnection = false;
        lastAdded = null;
        submit(root, '#panel-new #add-form');
        await until(() => root.querySelector('#panel-new #add-note')?.textContent === 'Written down.', 'the connection was not written down');

        assert.equal(logins.length, 1, 'a second login was written down');
        assert.equal(lastAdded!.authenticationId, 'l2');

        // And the Credentials tab knows it.
        assert.match(root.querySelector('#panel-credentials')!.textContent!, /The spare/);

    });

    it('keeps the credentials as a tab of the page, opened by the address', async () => {

        const root = await opened();

        assert.deepEqual([ ...root.querySelectorAll('[role="tab"]') ].map(tab => tab.id), [ 'tab-connections', 'tab-new', 'tab-credentials' ]);
        assert.equal(root.querySelector<HTMLElement>('#panel-connections')!.hidden, false, 'the page does not open on the connections there are');
        assert.equal(root.querySelector<HTMLElement>('#panel-new')!.hidden,         true);
        assert.equal(root.querySelector<HTMLElement>('#panel-credentials')!.hidden, true);
        assert.equal(root.querySelector('#panel-connections #add-form'), null, 'a new connection is written down among the ones there are');
        assert.ok(root.querySelector('#panel-connections [data-proves="c1"]') !== null);
        assert.match(root.querySelector('#panel-credentials')!.textContent!, /CSMS login/, 'the credentials are not drawn while their tab is not shown');
        assert.equal(asked.some(one => one.path === '/configuration/authentications'), false, 'the credentials were asked for a second time');

        root.querySelector<HTMLButtonElement>('#tab-credentials')!.click();
        await until(() => !root.querySelector<HTMLElement>('#panel-credentials')!.hidden, 'the tab of the credentials is not shown');

        assert.ok(add(root) !== null, 'the form of the connections went with the look at the other tab');

        // A connection written down is shown where it stands: among the others.
        root.querySelector<HTMLButtonElement>('#tab-new')!.click();
        await until(() => !root.querySelector<HTMLElement>('#panel-new')!.hidden, 'the tab of a new connection is not shown');

        type(field(root, '#panel-new #add-form', 'description'), 'The spare');
        typeURL(add(root), 'wss://spare.example.org/cs001');
        submit(root, '#panel-new #add-form');
        await until(() => !root.querySelector<HTMLElement>('#panel-connections')!.hidden, 'the page did not turn to the connection written down');

        assert.match(root.querySelector('#panel-connections')!.textContent!, /The spare/);

        const there = await opened('/configuration/connections?tab=credentials');

        assert.equal(there.querySelector<HTMLElement>('#panel-credentials')!.hidden, false);
        assert.equal(there.querySelector<HTMLElement>('#panel-connections')!.hidden, true);

    });

});
