/**
 * The pages of the connections drawn for a kind of node that is no charging
 * station, in a document of happy-dom: they ask the rights of the resource
 * the kind names, and speak of the node as the kind calls it. Untold, they
 * are a charging station's - which is what their own tests draw.
 *
 * Run with `npm test` of a kind of node that has these pages.
 */

import { leave, open, type Asked } from '@node/../test/node.ts';

import { strict as assert }     from 'node:assert';
import { after, describe, it }  from 'node:test';

import type { StationConnections } from './api/connections.ts';
import { connectionsResource, farEnd, nodeNoun, nodeNounInFull, nodeTraits,
         setConnectionsResource, setNodeNoun, setNodeTraits } from './settings.ts';

const { connectionsPage }     = await import('./pages/connections.ts');
const { authenticationPage }  = await import('./pages/authentication.ts');


/** A node that dials nowhere yet, and has nothing to prove itself with. */
function nothingYet(): StationConnections {
    return {
        directory:              'connections',
        authentications:        [],
        connections:            [],
        certificates:           [],
        connectionTypes:        [ 'CSMS', 'CSMSBackup', 'LocalController' ],
        ocppVersions:           [ 'OCPP2.1', 'OCPP1.6' ],
        maxDescriptionLength:   120,
        minSharedSecretLength:  16,
        secretsAreReadable:     false,
        totpDefaults:           { validitySeconds: 30, length: 12, alphabet: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ234567', hashAlgorithm: 'SHA256' },
        timestamp:              '2026-10-04T12:00:00Z',
        states:                 {}
    };
}

function node({ path }: Asked): unknown {

    if (path === '/configuration/connections' || path === '/configuration/authentications')
        return nothingYet();

    if (path === '/status/connections')
        return { timestamp: '2026-10-04T12:00:05Z', states: {} };

    return undefined;

}

const written = (root: HTMLElement) => (root.textContent ?? '').replace(/\s+/g, ' ');


describe('the pages of the connections, untold', () => {

    it('are a charging station\'s, under the resource "connections"', () => {

        assert.equal(connectionsResource(), 'connections');
        assert.equal(nodeNoun(),            'station');
        assert.equal(nodeNounInFull(),      'charging station');
        assert.deepEqual(nodeTraits(),      { oneNodePerOCPPVersion: true, isDialled: false, dialsDown: false });
        assert.equal(farEnd(),              'back end');

    });

});


describe('the pages of the connections, told they are a local controller\'s', () => {

    // The Connections page asks where the connections stand every five
    // seconds, for as long as it is open.
    after(leave);

    const told = () => {
        setConnectionsResource('csms');
        setNodeNoun('local controller');
        setNodeTraits({ oneNodePerOCPPVersion: false, isDialled: true });
    };

    it('let whoever may change the resource it names change the connections, and nobody for the right to "connections"', async () => {

        told();

        const may = await open(connectionsPage, '/configuration/connections', [ 'csms:read', 'csms:edit', 'csms:run' ],
                               node, root => root.querySelector('#add-form') !== null);

        assert.equal(may.querySelector<HTMLButtonElement>('#add-form button[type="submit"]')!.disabled, false);
        assert.equal(may.querySelector<HTMLButtonElement>('#add-form [data-test="add"]')!.disabled,    false);

        // Somebody who may neither write a connection down nor test one is not shown where one is written down.
        const mayNot = await open(connectionsPage, '/configuration/connections', [ 'connections:read', 'connections:edit', 'connections:run' ],
                                  node, root => root.querySelector('#panel-credentials #add-form') !== null);

        assert.equal(mayNot.querySelector('#tab-new'),   null);
        assert.equal(mayNot.querySelector('#panel-new'), null);

    });

    it('speak of a local controller on the Connections page, and of no station that it is', async () => {

        told();

        const root = await open(connectionsPage, '/configuration/connections', [ 'csms:read', 'csms:edit' ],
                                node, root => root.querySelector('#add-form') !== null);
        const text = written(root);

        assert.match(text, /Where this local controller dials, and what it proves itself with\./);
        assert.match(text, /Nothing yet\. This local controller dials nowhere and waits to be dialled\./);
        assert.match(text, /A URL does not say which OCPP is spoken over it, so it is said here\./);
        assert.doesNotMatch(text, /[Tt]his (charging )?station/);
        assert.doesNotMatch(text, /is two nodes/);

    });

    it('speak of a local controller on the Authentication page', async () => {

        told();

        const root = await open(authenticationPage, '/configuration/authentication', [ 'csms:read', 'csms:edit' ],
                                node, root => root.querySelector('#add-form') !== null);
        const text = written(root);

        assert.match(text, /How this local controller proves who it is when it dials a back end\./);
        assert.match(text, /it is what the other end calls this local controller\./);
        assert.doesNotMatch(text, /[Tt]his (charging )?station/);

    });

});


describe("the pages of the connections, told they are a CSMS's", () => {

    after(() => {
        leave();
        // What the tests after these draw is a charging station's again.
        setConnectionsResource('connections');
        setNodeNoun('station', 'charging station');
        setNodeTraits({ oneNodePerOCPPVersion: true, isDialled: false, dialsDown: false });
    });

    const told = () => {
        setConnectionsResource('stations');
        setNodeNoun('CSMS');
        setNodeTraits({ oneNodePerOCPPVersion: false, isDialled: true, dialsDown: true });
    };

    /** What a CSMS writes down: what is below it, by its name, over ws:// and wss:// alone, and nothing that dials it. */
    const below = ({ path }: Asked): unknown => {

        if (path === '/configuration/connections' || path === '/configuration/authentications')
            return {
                ...nothingYet(),
                connectionTypes:  [ 'ChargingStation', 'LocalController' ],
                ocppVersions:     [ 'OCPP2.1' ],
                rules:            { inbound: false, ssh: false, namesTheNodeItLeadsTo: true },
                connections:      [ {
                    id:              'c1',
                    description:     'Station 7',
                    url:             'wss://station7.example.org:9443/csms-01',
                    connectionType:  'ChargingStation',
                    ocppVersion:     'OCPP2.1',
                    autoConnect:     true,
                    secure:          true,
                    transport:       'websocket',
                    createdAt:       '2026-10-04T12:00:00Z',
                    remoteNodeId:    'cs007'
                } ]
            };

        return node({ path } as Asked);

    };

    it('offer what is below a CSMS, ask for the node at the other end, and offer nothing that dials it', async () => {

        told();

        const root  = await open(connectionsPage, '/configuration/connections', [ 'stations:read', 'stations:edit', 'stations:run' ],
                                 below, root => root.querySelector('#add-form') !== null);
        const form  = root.querySelector<HTMLFormElement>('#add-form')!;
        const types = [ ...form.querySelectorAll<HTMLOptionElement>('[name="connectionType"] option') ];

        assert.deepEqual(types.map(option => option.value),               [ 'ChargingStation', 'LocalController' ]);
        assert.deepEqual(types.map(option => option.textContent!.trim()), [ 'Charging station', 'Local controller' ]);
        assert.equal(types[0]!.selected, true);

        assert.notEqual(form.querySelector('[name="remoteNodeId"]'), null);
        assert.equal(form.querySelector('[name="inbound"]'),         null);

        assert.equal(farEnd(), 'node');
        assert.doesNotMatch(written(form), /ssh:\/\/cs001/);
        assert.doesNotMatch(written(form), /back end/);

    });

    it('say which node a connection leads to', async () => {

        told();

        const root = await open(connectionsPage, '/configuration/connections', [ 'stations:read' ],
                                below, root => root.querySelector('[data-remote-node="c1"]') !== null);

        assert.equal(root.querySelector('[data-remote-node="c1"]')!.textContent!.trim(), 'cs007');

    });

});
