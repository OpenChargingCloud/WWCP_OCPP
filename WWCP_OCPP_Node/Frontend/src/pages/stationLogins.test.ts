/**
 * The logins and groups drawn, in a document of happy-dom, against a
 * stand-in controller: a station half typed - and its focus - outlives
 * another being switched off, a switch or a chooser the controller refused
 * says what it has, the settings of one group do not keep what was typed
 * into another's, and a station added empties the form and says its password
 * once.
 */

import { asked, change, field, open, refused, said, settled, submit, until, type Asked } from '../../test/stationServer.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { I18NString, LoginGroup, WebSocketLogin, WebSocketLogins } from '../api/stationServer.ts';

const { stationLoginsPage } = await import('./stationLogins.ts');


function aGroup(id: string, builtIn = false): LoginGroup {
    return { id, name: `Group ${id}`, enabled: true, builtIn, addedAt: '2026-10-01T00:00:00Z',
             authMethods: [ 'basic', 'totp', 'certificate' ], clientAuthentication: [ 'httpAuthWithoutTLS', 'httpAuthOverTLS', 'tlsClientCertificate' ], members: 0 };
}

function aStation(id: string, group = 'default'): WebSocketLogin {
    return { id, group, enabled: true, addedAt: '2026-10-01T00:00:00Z', hasPassword: true, hasTOTP: false };
}

let held: WebSocketLogins;

/** Refuses every change to a station where told to. */
let refuseChanges = false;

function controller({ method, path, body }: Asked): unknown {

    if (path === '/configuration/ocpp-server/logins' && method === 'GET')
        return held;

    if (path === '/configuration/ocpp-server/logins' && method === 'POST') {
        const added = body as { id: string; group: string; description?: I18NString };
        held = { ...held, logins: [ ...held.logins, { ...aStation(added.id, added.group), description: added.description } ] };
        return { id: added.id, password: 'made up, shown once', logins: held };
    }

    const keys = /^\/configuration\/ocpp-server\/logins\/([^/]+)\/sshKeys(?:\/([^/]+))?$/.exec(path);

    if (keys && method === 'POST') {
        const given = body as { line: string; label: string };
        const key   = { fingerprint: 'SHA256:ab+cd/ef', line: given.line, label: given.label, created: '2026-11-07T00:00:00Z' };
        held = { ...held, logins: held.logins.some(station => station.id === keys[1])
                                        ? held.logins.map(station => station.id === keys[1] ? { ...station, hasSSHKey: true, sshKeys: [ key ] } : station)
                                        : [ ...held.logins, { ...aStation(keys[1]!), hasPassword: false, hasSSHKey: true, sshKeys: [ key ] } ] };
        return { id: keys[1], fingerprint: key.fingerprint, logins: held };
    }

    if (keys && method === 'DELETE') {
        held = { ...held, logins: held.logins.map(station => station.id === keys[1] ? { ...station, hasSSHKey: false, sshKeys: [] } : station) };
        return held;
    }

    const pins = /^\/configuration\/ocpp-server\/logins\/([^/]+)\/keyPins(?:\/([^/]+))?$/.exec(path);

    if (pins && method === 'POST') {
        const given  = body as { pin: string; label: string };
        const keyPin = { pin: given.pin.replace(/:/g, '').toLowerCase(), label: given.label, added: '2026-11-11T00:00:00Z' };
        held = { ...held, logins: held.logins.some(station => station.id === pins[1])
                                        ? held.logins.map(station => station.id === pins[1] ? { ...station, hasKeyPin: true, keyPins: [ keyPin ] } : station)
                                        : [ ...held.logins, { ...aStation(pins[1]!), hasPassword: false, hasKeyPin: true, keyPins: [ keyPin ] } ] };
        return { id: pins[1], keyPin: keyPin.pin, logins: held };
    }

    if (pins && method === 'DELETE') {
        held = { ...held, logins: held.logins.map(station => station.id === pins[1] ? { ...station, hasKeyPin: false, keyPins: [] } : station) };
        return held;
    }

    const binding = /^\/configuration\/ocpp-server\/logins\/([^/]+)\/totp\/binding$/.exec(path);

    if (binding && method === 'PUT') {
        const bound = (body as { tlsChannelBinding: boolean }).tlsChannelBinding;
        held = { ...held, logins: held.logins.map(station => station.id === binding[1]
                                                                     ? { ...station, totp: { ...station.totp!, tlsChannelBinding: bound } }
                                                                     : station) };
        return held;
    }

    const one = /^\/configuration\/ocpp-server\/logins\/([^/]+)$/.exec(path);

    if (one && method === 'PUT') {
        if (refuseChanges)
            return refused(409, 'the group is full');
        held = { ...held, logins: held.logins.map(station => station.id === one[1] ? { ...station, ...(body as object) } : station) };
        return held;
    }

    return undefined;

}

async function opened(): Promise<HTMLElement> {
    held = { file: 'stations.json', enabled: 2, maxLogins: 100, maxGroups: 8, minPasswordLength: 16, minSecretLength: 20,
             defaultGroup: 'default', groups: [ aGroup('default', true), aGroup('field-test') ],
             logins: [ aStation('cs001'), aStation('cs002') ] };
    refuseChanges = false;
    return open(stationLoginsPage, '/configuration/ocpp-server/logins', [ 'stations:read', 'stations:edit' ],
                controller, root => root.querySelector('#station-form') !== null);
}

const switchOf  = (root: HTMLElement, id: string) => root.querySelector<HTMLInputElement>(`.station-enabled[data-id="${id}"]`)!;
const chooserOf = (root: HTMLElement, id: string) => root.querySelector<HTMLSelectElement>(`.station-group[data-id="${id}"]`)!;
const putsSent  = () => asked.filter(one => one.method === 'PUT').length;


describe('the logins and groups', () => {

    it('keep a station half typed, and its focus, while another is switched off', async () => {

        const root = await opened();
        const id   = field(root, '#station-form', 'id');
        const description = field(root, '#station-form', 'description');

        id.value   = 'cs003';
        description.value = 'Ladepunkt 3';
        description.focus();

        change(switchOf(root, 'cs001'), false);
        await until(() => held.logins[0]!.enabled === false && root.querySelector('tr.dimmed') !== null,
                    'the station switched off was not drawn so');

        assert.ok(field(root, '#station-form', 'description') === description, 'the field was made anew');
        assert.equal(id.value,                'cs003');
        assert.equal(description.value,       'Ladepunkt 3');
        assert.ok(document.activeElement === description, 'the focus went');

    });

    it('put a switch and a chooser the controller refused back to what it has', async () => {

        const root = await opened();

        refuseChanges = true;

        change(switchOf(root, 'cs001'), false);
        await until(() => said.length === 1 && asked.filter(one => one.method === 'GET').length === 2, 'the refused switch was not loaded again');
        await settled();

        change(chooserOf(root, 'cs002'), 'field-test');
        await until(() => said.length === 2 && asked.filter(one => one.method === 'GET').length === 3, 'the refused move was not loaded again');
        await settled();

        assert.equal(putsSent(), 2);
        assert.match(said[0]!, /the group is full/);
        assert.equal(switchOf(root, 'cs001').checked,  true,       'the switch says what was clicked');
        assert.equal(chooserOf(root, 'cs002').value,   'default',  'the chooser says what was chosen');

    });

    it('open the settings of another group without what was typed into the first one\'s', async () => {

        const root = await opened();

        root.querySelector<HTMLButtonElement>('.group-edit[data-id="default"]')!.click();
        await until(() => root.querySelector('#group-form[data-id="default"]') !== null, 'the settings did not open');

        field(root, '#group-form', 'name').value = 'typed for default';

        root.querySelector<HTMLButtonElement>('.group-edit[data-id="field-test"]')!.click();
        await until(() => root.querySelector('#group-form[data-id="field-test"]') !== null, 'the other settings did not open');

        assert.equal(field(root, '#group-form', 'name').value, 'Group field-test', 'the other group\'s settings say what was typed into the first');

    });

    it('empty the form once a station is added, and say its password', async () => {

        const root = await opened();

        field(root, '#station-form', 'id').value   = 'cs003';
        field(root, '#station-form', 'description').value = 'Ladepunkt 3';
        change(field<HTMLSelectElement>(root, '#station-form', 'group'), 'field-test');

        submit(root, '#station-form');
        await until(() => root.querySelector('.station-enabled[data-id="cs003"]') !== null, 'the station added was not drawn');

        assert.deepEqual(asked.find(one => one.method === 'POST')?.body, { id: 'cs003', password: '', group: 'field-test', description: { en: 'Ladepunkt 3' } });
        assert.equal(root.querySelector('code.password')?.textContent, 'made up, shown once');
        assert.equal(field(root, '#station-form', 'id').value,                          '', 'the station added is still in the form');
        assert.equal(field(root, '#station-form', 'description').value,                 '');
        assert.equal(field<HTMLSelectElement>(root, '#station-form', 'group').value,    'default');

    });

    it('ask for the token of a station to be bound to its connection, say so, and stop asking', async () => {

        const root = await opened();

        assert.equal(root.querySelector('.station-totp-binding[data-id="cs001"]'), null, 'a station without a token is offered its binding');

        held = { ...held, logins: held.logins.map(station => station.id === 'cs002'
                                                                     ? { ...station, hasTOTP: true,
                                                                         totp: { validitySeconds: 30, length: 12, alphabet: 'ab', hashAlgorithm: 'SHA256' as const } }
                                                                     : station) };

        // Drawn anew from what the CSMS holds now.
        change(switchOf(root, 'cs001'), false);
        await until(() => root.querySelector('.station-totp-binding[data-id="cs002"]') !== null, 'the binding of a token was not offered');

        const asks = window.confirm;
        let   told = '';
        window.confirm = (text?: string) => { told = text ?? ''; return true; };

        try {

            const button = () => root.querySelector<HTMLButtonElement>('.station-totp-binding[data-id="cs002"]')!;

            assert.equal(button().textContent!.trim(), 'Bind token');
            assert.equal(root.querySelector('.station-totp-bound'), null);

            button().click();
            await until(() => root.querySelector('.station-totp-bound') !== null, 'that the token is bound was not drawn');

            assert.match(told, /over HTTP\/3 only/, 'what binding costs was not said before it was done');
            const sent = asked.find(one => one.path.endsWith('/totp/binding'));

            assert.equal(sent?.method, 'PUT');
            assert.equal(sent?.path,   '/configuration/ocpp-server/logins/cs002/totp/binding');
            assert.deepEqual(sent?.body, { tlsChannelBinding: true });
            assert.match(root.querySelector('.station-totp-bound')!.textContent!, /over HTTP\/3 only/);
            assert.equal(button().textContent!.trim(), 'Unbind token');

            told = '';
            button().click();
            await until(() => root.querySelector('.station-totp-bound') === null, 'that the token is no longer bound was not drawn');

            assert.equal(told, '', 'taking the binding away was asked about');
            assert.deepEqual(asked.filter(one => one.path.endsWith('/totp/binding')).at(-1)?.body, { tlsChannelBinding: false });

        }
        finally {
            window.confirm = asks;
        }

    });

    it('give a station an SSH key, say when its group does not take keys, and take the key away by a fingerprint fit for a URL', async () => {

        const root = await opened();

        field(root, '#sshkey-form', 'id').value    = 'cs003';
        field(root, '#sshkey-form', 'line').value  = 'ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIA cs003';
        field(root, '#sshkey-form', 'label').value = 'bay 3';

        submit(root, '#sshkey-form');
        await until(() => root.querySelector('.station-sshkey[data-id="cs003"]') !== null, 'the key given was not drawn');

        assert.deepEqual(asked.find(one => one.method === 'POST')?.body,
                         { line: 'ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIA cs003', label: 'bay 3', group: '' });
        assert.match(root.querySelector('.station-sshkey[data-id="cs003"]')!.textContent!, /SHA256:ab\+cd\/ef - bay 3/);
        assert.equal(field(root, '#sshkey-form', 'line').value, '', 'the key given is still in the form');
        assert.match(root.querySelector('.station-sshkey[data-id="cs003"]')!.closest('td')!.textContent!, /does not accept SSH keys/,
                     'a key its group does not take was not said to be of no use');

        root.querySelector<HTMLButtonElement>('.station-sshkey-remove[data-id="cs003"]')!.click();
        await until(() => root.querySelector('.station-sshkey[data-id="cs003"]') === null, 'the key taken away was still drawn');

        assert.equal(asked.find(one => one.method === 'DELETE')?.path, '/configuration/ocpp-server/logins/cs003/sshKeys/ab-cd_ef');

    });

    it('let a station in by a key pin, say when its group does not take pins, and take the pin away by its 64 digits', async () => {

        const root = await opened();
        const pin  = '0a'.repeat(32);

        field(root, '#keypin-form', 'id').value    = 'cs003';
        field(root, '#keypin-form', 'pin').value   = '0A:'.repeat(31) + '0A';
        field(root, '#keypin-form', 'label').value = 'bay 3';

        submit(root, '#keypin-form');
        await until(() => root.querySelector('.station-keypin[data-id="cs003"]') !== null, 'the key pin given was not drawn');

        assert.deepEqual(asked.find(one => one.method === 'POST')?.body,
                         { pin: '0A:'.repeat(31) + '0A', label: 'bay 3', group: '' });
        assert.match(root.querySelector('.station-keypin[data-id="cs003"]')!.textContent!, new RegExp(`${pin} - bay 3`));
        assert.equal(field(root, '#keypin-form', 'pin').value, '', 'the key pin given is still in the form');
        assert.match(root.querySelector('.station-keypin[data-id="cs003"]')!.closest('td')!.textContent!, /key pin/);
        assert.match(root.querySelector('.station-keypin[data-id="cs003"]')!.closest('td')!.textContent!, /does not accept key pins/,
                     'a pin its group does not take was not said to be of no use');

        root.querySelector<HTMLButtonElement>('.station-keypin-remove[data-id="cs003"]')!.click();
        await until(() => root.querySelector('.station-keypin[data-id="cs003"]') === null, 'the key pin taken away was still drawn');

        assert.equal(asked.find(one => one.method === 'DELETE')?.path, `/configuration/ocpp-server/logins/cs003/keyPins/${pin}`);

    });

    it('offer key pins among the ways in a group accepts', async () => {

        const root = await opened();

        root.querySelector<HTMLButtonElement>('.group-edit[data-id="field-test"]')!.click();
        await until(() => root.querySelector('#group-form[data-id="field-test"]') !== null, 'the settings did not open');

        assert.equal(field<HTMLInputElement>(root, '#group-form', 'keypin').checked, false, 'a group without key pins is drawn with them');

    });

});
