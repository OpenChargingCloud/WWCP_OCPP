/*
 * What the tests of the pages of a station server share: every kind's stand-in
 * for fetch and document of happy-dom - WWCP_Node's @node/../test/node.ts -
 * and the station server as a node that has just been set up has it. The
 * pages speak of a local controller here, as they did while they were its.
 *
 * Imported first, before a page: lit-html looks for the document as it is
 * loaded, and the pages load it.
 *
 *   import { anOCPPServer, open, ... } from '../../test/stationServer.ts';
 *   const { ocppServerPage } = await import('./ocppServer.ts');
 */

import { setNodeNoun } from '../src/settings.ts';

import type { OCPPServerConfiguration } from '../src/api/stationServer.ts';

export * from '@node/../test/node.ts';

setNodeNoun('local controller');


/** The charging station server, as a node that has just been set up has it. */
export function anOCPPServer(): OCPPServerConfiguration {
    return {
        enabled:                     false,
        address:                     null,
        port:                        8443,
        clientAuthentication:        [ 'httpAuthWithoutTLS', 'httpAuthOverTLS', 'tlsClientCertificate' ],
        subprotocols:                [ 'ocpp2.1' ],
        reachableAs:                 [ 'lc001.example.org' ],
        minTLSVersion:               '1.2',
        checkCertificateRevocation:  false,
        maxConnections:              100,
        pingEverySeconds:            60,
        http3:                       false,
        logging:                     { connections: true, authentication: true, messages: false, payloads: false,
                                       payloadsUntil: null, payloadsNow: false, pings: false },
        ssh:                         { enabled: false, subprotocols: null, maxMessageSize: 67108864, pingEverySeconds: 30,
                                       subsystem: 'ocpp@open.charging.cloud',
                                       state: { served: false, sshServer: true, url: 'ssh://lc001.example.org:22',
                                                hostKey: 'ssh-ed25519 SHA256:bm90IGEgcmVhbCBrZXkgYXQgYWxsLCBqdXN0IGZvcg',
                                                connections: 0, stationsWithAKey: 0 } },
        state:                       { running: false, tls: false, url: 'wss://lc001.example.org:8443', http3Port: null, http3Problem: null, connections: 0,
                                       webSocketLogins: 0, trustedChains: 1, hasCertificate: false, waitingForARestart: [] },
        limits:                      { subprotocols: [ 'ocpp1.6', 'ocpp2.0.1', 'ocpp2.1' ], clientAuthentication: [ 'httpAuthWithoutTLS', 'httpAuthOverTLS', 'tlsClientCertificate' ],
                                       tlsVersions: [ '1.2', '1.3' ], maxConnections: 1000, maxReachableAs: 8,
                                       suggestedPayloadWindowSeconds: 3600 },
        file:                        'ocppServer.json'
    };
}
