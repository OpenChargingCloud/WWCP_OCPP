/**
 * The Commissioning page drawn, in a document of happy-dom, against a stand-in
 * node: the switch sends what it says and the page says what the node took; a refusal keeps the switch as set and says why; somebody who may only
 * read it is told so and can switch nothing; where a tool connects is said as
 * an account's URL, and who is connected is listed; and an SSH server that is
 * off is said to be in the way.
 *
 * Run with `npm test`.
 */

import { open, refused, submit, until, type Asked } from '../../test/stationServer.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { CommissioningConfiguration, CommissioningUpdate } from '../api/commissioning.ts';

const { commissioningPage } = await import('./commissioning.ts');


let held: CommissioningConfiguration;

/** What the last save sent. */
let sent: CommissioningUpdate | undefined;

/** Refuses every change where told to. */
let refuseChanges = false;

function aStation(): CommissioningConfiguration {
    return {
        enabled:       false,
        served:        false,
        sshServer:     true,
        url:           'ssh://127.0.0.1:22348',
        hostKey:       'ssh-ed25519 SHA256:B8eSW5zm2JdBDTSLAkEYA5Ot2N/YU8UhvsLmCvLq0ao',
        subsystem:     'ocpp@open.charging.cloud',
        subprotocols:  [ 'ocpp2.1', 'ocpp2.0.1' ],
        node:          'test02',
        permission:    'commissioning:run',
        tools:         [],
        file:          'configuration.json'
    };
}

function station({ method, path, body }: Asked): unknown {

    if (path === '/configuration/commissioning' && method === 'PUT') {
        if (refuseChanges)
            return refused(400, "The configuration file could not be written.");
        const update = body as CommissioningUpdate;
        sent = update;
        held = { ...held, enabled: update.enabled, served: update.enabled };
        return held;
    }

    if (path === '/configuration/commissioning')
        return held;

    return undefined;

}

async function opened(Station: Partial<CommissioningConfiguration> = {},
                      Permissions = [ 'commissioning:read', 'commissioning:edit' ]): Promise<HTMLElement> {
    held          = { ...aStation(), ...Station };
    sent          = undefined;
    refuseChanges = false;
    return open(commissioningPage, '/configuration/commissioning', Permissions,
                station, root => root.querySelector('#commissioning-form') !== null);
}

const theSwitch = (root: HTMLElement) => root.querySelector<HTMLInputElement>('#commissioning-form input[name="enabled"]')!;
const said      = (root: HTMLElement, selector: string) => root.querySelector(selector)?.textContent?.replace(/\s+/g, ' ').trim() ?? '';


describe('the Commissioning page', () => {

    it('switches it on, and says that it is served', async () => {

        const root = await opened();

        assert.match(said(root, '#commissioning-state'), /^Not served/);

        theSwitch(root).checked = true;

        submit(root, '#commissioning-form');
        await until(() => /^Saved/.test(root.querySelector('#form-note')?.textContent ?? ''), 'the switch was not saved');

        assert.deepEqual(sent, { enabled: true });
        assert.match(said(root, '#commissioning-state'), /^Served/);
        assert.equal(theSwitch(root).checked, true, 'the switch does not say what the station took');

    });

    it('switches it off', async () => {

        const root = await opened({ enabled: true, served: true });

        theSwitch(root).checked = false;

        submit(root, '#commissioning-form');
        await until(() => /^Saved/.test(root.querySelector('#form-note')?.textContent ?? ''), 'the switch was not saved');

        assert.deepEqual(sent, { enabled: false });
        assert.match(said(root, '#form-note'), /not served/);

    });

    it('keeps the switch as set where the station refused, and says why', async () => {

        const root = await opened();

        refuseChanges = true;
        theSwitch(root).checked = true;

        submit(root, '#commissioning-form');
        await until(() => root.querySelector('#form-error')?.textContent !== '', 'the refusal was not said');

        assert.match(root.querySelector('#form-error')!.textContent!, /could not be written/);
        assert.equal(theSwitch(root).checked, true, 'the switch went back');

    });

    it('tells somebody who may only read it so, and lets them switch nothing', async () => {

        const root = await opened({}, [ 'commissioning:read' ]);

        assert.match(root.querySelector('.notice')!.textContent!, /may look at commissioning but not switch it on or off/);
        assert.equal(theSwitch(root).disabled, true);
        assert.equal(root.querySelector<HTMLButtonElement>('#commissioning-form button[type="submit"]')!.disabled, true);

    });

    it('says where a tool connects, as an account, and who is connected', async () => {

        const root = await opened({ enabled: true, served: true, tools: [ 'installer', 'root' ] });

        const facts = said(root, '#commissioning-facts');

        assert.match(facts, /URL ssh:\/\/<account>@127\.0\.0\.1:22348/);
        assert.match(facts, /Subsystem ocpp@open\.charging\.cloud/);
        assert.match(facts, /OCPP ocpp2\.1, ocpp2\.0\.1/);
        assert.match(facts, /OCPP node test02/);
        assert.match(facts, /Who may an account that may commissioning:run/);

        const rows = [...root.querySelectorAll('.table-scroll #commissioning-tools tbody tr')].map(row => row.textContent!.trim());

        assert.deepEqual(rows, [ 'installer', 'root' ]);
        assert.equal(root.querySelector('#no-ssh-server'), null, 'an SSH server that is on was said to be off');

    });

    it('says that an SSH server that is off is in the way', async () => {

        const root = await opened({ enabled: true, served: true, sshServer: false, url: null });

        assert.match(said(root, '#no-ssh-server'), /The SSH server is off/);
        assert.match(said(root, '#commissioning-facts'), /URL none - the SSH server is off/);

    });

});
