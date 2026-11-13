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

import type { StationCertificates, StationConnections } from './api/connections.ts';
import { connectionsResource, nodeNoun, nodeNounInFull, nodeTraits,
         setConnectionsResource, setNodeNoun, setNodeTraits } from './settings.ts';

const { connectionsPage }     = await import('./pages/connections.ts');
const { authenticationPage }  = await import('./pages/authentication.ts');
const { clientKeysPage }      = await import('./pages/clientKeys.ts');


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

/** And no key to dial with. */
function noKeys(): StationCertificates {
    return {
        directory:             'certificates',
        now:                   '2026-10-04T12:00:00Z',
        inUseId:               null,
        entries:               [],
        algorithms:            [ { id: 'ecdsa-p256', name: 'ECDSA P-256', remark: 'What every back end takes.' } ],
        defaultAlgorithm:      'ecdsa-p256',
        maxSubjectLength:      200,
        canImportPrivateKeys:  false
    };
}

function node({ path }: Asked): unknown {

    if (path === '/configuration/connections' || path === '/configuration/authentications')
        return nothingYet();

    if (path === '/configuration/certificates')
        return noKeys();

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
        assert.deepEqual(nodeTraits(),      { oneNodePerOCPPVersion: true, isDialled: false });

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

        const mayNot = await open(connectionsPage, '/configuration/connections', [ 'connections:read', 'connections:edit', 'connections:run' ],
                                  node, root => root.querySelector('#add-form') !== null);

        assert.equal(mayNot.querySelector<HTMLButtonElement>('#add-form button[type="submit"]')!.disabled, true);
        assert.equal(mayNot.querySelector<HTMLButtonElement>('#add-form [data-test="add"]')!.disabled,    true);

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

    it('speak of a local controller on the Client keys page, which is dialled itself', async () => {

        told();

        const root = await open(clientKeysPage, '/configuration/client-keys', [ 'csms:read', 'csms:edit' ],
                                node, root => root.querySelector('#create-form') !== null);
        const text = written(root);

        assert.match(text, /What this local controller says it is when it dials a back end\./);
        assert.match(text, /What a back end recognises this local controller by\./);
        assert.match(text, /It dials with this key and is not dialled by it, so nothing else is asked for\./);
        assert.doesNotMatch(text, /[Tt]his (charging )?station/);
        assert.doesNotMatch(text, /is not something anybody dials/);

    });

});
