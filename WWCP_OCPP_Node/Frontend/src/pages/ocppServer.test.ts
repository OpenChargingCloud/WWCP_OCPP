/**
 * The charging station server's page drawn, in a document of happy-dom,
 * against a stand-in controller: what is typed into one card - and its focus
 * - outlives another being saved and the server being switched, a switch the
 * controller refused says what it has, and a card saved says what it took.
 */

import { anOCPPServer, asked, change, field, open, refused, settled, submit, until, type Asked } from '../../test/stationServer.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { OCPPServerConfiguration, OCPPServerUpdate, WebSocketLogins } from '../api/stationServer.ts';

const { ocppServerPage } = await import('./ocppServer.ts');


let held: OCPPServerConfiguration;

/** Refuses every change where told to. */
let refuseChanges = false;

/** Whether the stand-in's SSH server is on. */
let sshServerOn = true;

function controller({ method, path, body }: Asked): unknown {

    if (path === '/configuration/ocpp-server/logins')
        return { file: 'stations.json', enabled: 0, maxLogins: 100, maxGroups: 8, minPasswordLength: 16, minSecretLength: 20,
                 defaultGroup: 'default', groups: [], logins: [] } satisfies WebSocketLogins;

    if (path === '/configuration/ocpp-server' && method === 'PUT') {
        if (refuseChanges)
            return refused(409, 'no server certificate yet');
        const update = body as OCPPServerUpdate;
        held = { ...held, ...update } as OCPPServerConfiguration;
        // OCPP-over-SSH is a field of its own, merged as the controller merges it.
        if (update.ssh !== undefined)
            held.ssh = { ...anOCPPServer().ssh, ...held.ssh, ...update.ssh,
                         state: sshServerOn
                                    ? { ...anOCPPServer().ssh.state, served: update.ssh.enabled === true }
                                    : { ...anOCPPServer().ssh.state, served: false, sshServer: false, url: null, hostKey: null } };
        // A controller takes what it takes: names as it writes them, once each.
        if (update.reachableAs !== undefined)
            held.reachableAs = [ ...new Set(update.reachableAs.map(name => name.toLowerCase())) ];
        return held;
    }

    if (path === '/configuration/ocpp-server')
        return held;

    return undefined;

}

async function opened(): Promise<HTMLElement> {
    held          = anOCPPServer();
    refuseChanges = false;
    if (!sshServerOn)
        held.ssh  = { ...held.ssh, state: { ...held.ssh.state, sshServer: false, url: null, hostKey: null } };
    return open(ocppServerPage, '/configuration/ocpp-server', [ 'stations:read', 'stations:edit' ],
                controller, root => root.querySelector('#reachable-form') !== null);
}

const reachable = (root: HTMLElement) => field<HTMLTextAreaElement>(root, '#reachable-form', 'reachableAs');
const saved     = (root: HTMLElement, card: string) => root.querySelector(`#${card}-note`)?.textContent === '' &&
                                                       asked.some(one => one.method === 'PUT');


describe('the charging station server\'s page', () => {

    it('keeps what is typed into one card, and its focus, while another card is saved', async () => {

        const root  = await opened();
        const names = reachable(root);

        names.value = 'lc001.example.org\n192.168.1.10';
        names.focus();

        field(root, '#socket-form', 'port').value = '9443';
        submit(root, '#socket-form');
        await until(() => saved(root, 'socket') && held.port === 9443, 'the socket was not saved');

        assert.ok(reachable(root) === names, 'the field was made anew');
        assert.equal(names.value,            'lc001.example.org\n192.168.1.10');
        assert.ok(document.activeElement === names, 'the focus went');

    });

    it('keeps what is typed, and its focus, while the server is switched on', async () => {

        const root  = await opened();
        const names = reachable(root);

        names.value = 'lc001.example.org\n192.168.1.10';
        names.focus();

        change(root.querySelector<HTMLInputElement>('#enabled')!, true);
        await until(() => saved(root, 'socket') && held.enabled, 'the switch was not saved');

        assert.ok(reachable(root) === names, 'the field was made anew');
        assert.equal(names.value,            'lc001.example.org\n192.168.1.10');
        assert.ok(document.activeElement === names, 'the focus went');

    });

    it('puts a switch the controller refused back to what it has, and says why', async () => {

        const root = await opened();

        refuseChanges = true;
        change(root.querySelector<HTMLInputElement>('#enabled')!, true);

        await until(() => root.querySelector('#socket-error')?.textContent !== '', 'the refusal was not said');
        await until(() => asked.filter(one => one.method === 'GET' && one.path === '/configuration/ocpp-server').length === 2,
                    'the page did not ask what the controller has after the refusal');
        await settled();

        assert.equal(root.querySelector('#socket-error')!.textContent,           'no server certificate yet');
        assert.equal(root.querySelector<HTMLInputElement>('#enabled')!.checked,   false, 'the switch says what was clicked');

    });

    it('takes HTTP/3 when it is switched on, at once, and says where - and why not, where not', async () => {

        const root = await opened();

        assert.equal(root.querySelector('#state-http3')!.textContent, 'not taken');

        change(root.querySelector<HTMLInputElement>('#http3')!, true);
        await until(() => saved(root, 'http3') && held.http3, 'the switch was not saved');

        assert.deepEqual(asked.filter(one => one.method === 'PUT').map(one => one.body), [ { http3: true } ],
                         'more than the switch was sent');
        await until(() => root.querySelector('#state-http3')!.textContent === 'asked for, and not taken now', 'it was not said that none are taken');

        held = { ...held, state: { ...held.state, http3Port: 8443 } };
        change(root.querySelector<HTMLInputElement>('#http3')!, true);
        await until(() => root.querySelector('#state-http3')!.textContent === 'on UDP port 8443', 'the UDP port was not said');
        assert.equal(root.querySelector('#http3-problem'), null);

        held = { ...held, state: { ...held.state, http3Port: null, http3Problem: 'the UDP port 8443 cannot be listened on' } };
        change(root.querySelector<HTMLInputElement>('#http3')!, true);
        await until(() => root.querySelector('#http3-problem') !== null, 'why none are taken was not said');
        assert.match(root.querySelector('#http3-problem')!.textContent!, /the UDP port 8443 cannot be listened on/);

    });

    it('shows the names as the controller took them once they are saved, with nothing left to save', async () => {

        const root = await opened();

        reachable(root).value = 'LC001.example.org\nlc001.example.org\n192.168.1.10';

        submit(root, '#reachable-form');
        await until(() => saved(root, 'reachable') && held.reachableAs.length === 2, 'the names were not saved');

        assert.equal(reachable(root).value,         'lc001.example.org\n192.168.1.10', 'the field says what was typed, not what the controller took');
        assert.equal(reachable(root).defaultValue,  'lc001.example.org\n192.168.1.10');

    });

    it('keeps what is typed into a card the controller refused, and says why', async () => {

        const root = await opened();
        const port = field(root, '#socket-form', 'port');

        refuseChanges = true;
        port.value = '443';

        submit(root, '#socket-form');
        await until(() => root.querySelector('#socket-error')?.textContent !== '', 'the refusal was not said');
        await until(() => asked.filter(one => one.method === 'GET' && one.path === '/configuration/ocpp-server').length === 2,
                    'the page did not ask what the controller has after the refusal');
        await settled();

        assert.equal(root.querySelector('#socket-error')!.textContent, 'no server certificate yet');
        assert.equal(field(root, '#socket-form', 'port').value, '443', 'what was typed went');

    });

    it('switches OCPP-over-SSH on, and says where the stations connect and which host key they pin', async () => {

        const root = await opened();

        assert.equal(root.querySelector('#ssh-no-server'), null, 'an SSH server that is on was said to be off');

        change(root.querySelector<HTMLInputElement>('#ssh-enabled')!, true);
        await until(() => held.ssh.enabled && root.querySelector('#ssh-card .ok') !== null, 'OCPP-over-SSH was not switched on');

        assert.deepEqual(asked.find(one => one.method === 'PUT')?.body, { ssh: { enabled: true } }, 'more than the switch was sent');
        assert.match(root.querySelector('#ssh-card')!.textContent!, /ssh:\/\/lc001\.example\.org:22/);
        assert.equal(root.querySelector('#ssh-host-key')?.textContent, held.ssh.state.hostKey);

    });

    it('says so where OCPP-over-SSH is on and the SSH server is not', async () => {

        sshServerOn = false;

        try
        {

            const root = await opened();

            change(root.querySelector<HTMLInputElement>('#ssh-enabled')!, true);
            await until(() => held.ssh.enabled && root.querySelector('#ssh-no-server') !== null,
                        'OCPP-over-SSH switched on without an SSH server was not said to reach nobody');

            assert.equal(root.querySelector('#ssh-host-key'), null, 'a host key was shown for an SSH server that is off');

        }
        finally
        {
            sshServerOn = true;
        }

    });

});
