/**
 * The OCPP Device Model page drawn, in a document of happy-dom, against a
 * stand-in node: the tree folded at first, a variable that says its value
 * while folded, a value hidden or shown as the node says, a search that finds
 * a value deep down, and a node that has no OCPP node yet.
 *
 * Run with `npm test`.
 */

import { open, until, type Asked } from '../../test/stationServer.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { DeviceModelPage } from '../api/deviceModel.ts';

const { deviceModelPage, deviceModelMenuEntry, summaryOf } = await import('./deviceModel.ts');


function aDeviceModel(WriteOnlyShown: boolean): DeviceModelPage {
    return {
        writeOnlyShown: WriteOnlyShown,
        deviceModel: {
            'ocpp2.1': {
                SecurityCtrlr: {
                    OrganizationName:   { value: 'GraphDefined', type: 'Actual', mutability: 'ReadWrite' },
                    BasicAuthPassword:  WriteOnlyShown
                                            ? { value: 's3cr3t-s3cr3t-s3cr3t', type: 'Actual', mutability: 'WriteOnly', secret: true }
                                            : { hidden: true,                  type: 'Actual', mutability: 'WriteOnly', secret: true }
                },
                'DNSClientCtrlr[ptb1]': {
                    Enabled:            { value: 'true', type: 'Actual', mutability: 'ReadOnly' }
                }
            },
            'ocpp1.6': {
                HeartbeatInterval:      { value: '300', accessRights: 'ReadWrite', readonly: false, valueType: 'Integer', unit: 'Seconds' }
            }
        }
    };
}

let answer: DeviceModelPage = aDeviceModel(false);

function node({ path }: Asked): unknown {
    return path === '/ocpp/deviceModel' ? answer : undefined;
}

async function opened(Answer: DeviceModelPage): Promise<HTMLElement> {
    answer = Answer;
    return open(deviceModelPage, '/ocpp-device-model', [ 'stations:read', 'stations:edit' ],
                node, root => !(root.querySelector('#tree')?.textContent ?? 'Reading').includes('Reading'));
}

/** The row of the given path. */
function rowAt(Root: HTMLElement, ...Path: string[]): HTMLElement | null {
    return Root.querySelector<HTMLElement>(`.jt-node[data-path='${JSON.stringify(Path)}'] > .jt-row`);
}


describe('the OCPP Device Model page', () => {

    it('is in the menu for whoever may read the OCPP log', () => {
        assert.equal(deviceModelMenuEntry.path,  '/ocpp-device-model');
        assert.equal(deviceModelMenuEntry.label, 'OCPP Device Model');
    });

    it('shows both versions folded, and a variable says its value while it is folded', async () => {

        const root = await opened(aDeviceModel(false));

        assert.ok(rowAt(root, 'ocpp2.1'));
        assert.ok(rowAt(root, 'ocpp1.6'));
        assert.ok(rowAt(root, 'ocpp2.1', 'SecurityCtrlr') === null, 'A level was unfolded at first.');

        rowAt(root, 'ocpp2.1')!.click();
        rowAt(root, 'ocpp2.1', 'SecurityCtrlr')!.click();

        assert.equal(rowAt(root, 'ocpp2.1', 'SecurityCtrlr', 'OrganizationName')?.querySelector('.jt-summary')?.textContent,  '= "GraphDefined"');
        assert.equal(rowAt(root, 'ocpp2.1', 'SecurityCtrlr', 'BasicAuthPassword')?.querySelector('.jt-summary')?.textContent, '= hidden');
        assert.match(root.querySelector('#write-only')?.textContent ?? '', /hidden; a systemadmin sees them/);
        assert.doesNotMatch(root.textContent ?? '', /s3cr3t/);

    });

    it('shows a systemadmin the values OCPP keeps write-only, and says so', async () => {

        const root = await opened(aDeviceModel(true));

        rowAt(root, 'ocpp2.1')!.click();
        rowAt(root, 'ocpp2.1', 'SecurityCtrlr')!.click();

        assert.equal(rowAt(root, 'ocpp2.1', 'SecurityCtrlr', 'BasicAuthPassword')?.querySelector('.jt-summary')?.textContent, '= "s3cr3t-s3cr3t-s3cr3t"');
        assert.match(root.querySelector('#write-only')?.textContent ?? '', /systemadmin: the values OCPP keeps write-only/);

    });

    it('finds a value deep down, and unfolds the way to it', async () => {

        const root   = await opened(aDeviceModel(false));
        const search = root.querySelector<HTMLInputElement>('#search')!;

        search.value = 'heartbeat';
        search.dispatchEvent(new Event('input'));

        await until(() => rowAt(root, 'ocpp1.6', 'HeartbeatInterval') !== null, 'the key was not found');

        assert.equal(rowAt(root, 'ocpp1.6', 'HeartbeatInterval')!.parentElement!.hidden, false);
        assert.equal(rowAt(root, 'ocpp2.1')!.parentElement!.hidden, true);
        assert.match(root.querySelector('#matches')?.textContent ?? '', /1 row found/);

        root.querySelector<HTMLButtonElement>('#collapse')!.click();

        assert.equal(search.value, '');
        assert.equal(rowAt(root, 'ocpp2.1')!.parentElement!.hidden, false);

    });

    it('says so where the node has no OCPP node yet', async () => {

        const root = await opened({ writeOnlyShown: false, deviceModel: {} });

        assert.match(root.querySelector('#tree')?.textContent ?? '', /no OCPP node yet/);

    });

    it('says what a variable is in a folded row: its value, hidden, or why its value could not be read', () => {
        assert.equal(summaryOf({ value: 'x' }),                    '= "x"');
        assert.equal(summaryOf({ value: null }),                   '= null');
        assert.equal(summaryOf({ hidden: true }),                  '= hidden');
        assert.equal(summaryOf({ value: null, error: 'gone' }),    '= ? (gone)');
        assert.equal(summaryOf({ OrganizationName: {} }),          null);
    });

});
