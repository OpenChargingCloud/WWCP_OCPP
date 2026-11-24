import { request }  from '@node/api/client';
import { webSocketLoginsAPI, type ClientAuthentication } from '@node/connecting/api/webSocketLogins';


// What a node that serves the charging stations below it keeps for them, and
// how it is asked: its section "ocppServer", and - every WebSocket server's,
// see WWCP_Node's connecting/api/webSocketLogins - the chains a station's
// certificate may lead to and the charging stations that may sign in with
// their login groups, at "/configuration/ocpp-server". A local controller and
// a gateway keep them alike, and serve them by the same routes -
// OCPPServingHTTPAPI's - so the types and the calls are here, for the pages of
// both. A kind of node spreads the calls into its own api where it wants them
// there as well.
//
// What the station port presents is not here: it is the node's certificate
// store's - TLS server identities told the listener "ocpp", and their server
// keys - see "@node/api/client" and the Server identities page.

export type { AuthMethod, ClientAuthentication, ClientTrust, LoginGroup, LoginGroupUpdate, StationKeyPin, StationSSHKey,
              TOTPSettings, TOTPUpdate, TrustedChain, WebSocketLogin, WebSocketLogins } from '@node/connecting/api/webSocketLogins';
export { fingerprintInURL, inEnglish, textOf, type I18NString } from '@node/connecting/api/webSocketLogins';


/** What of the charging station server ends up in the event log. */
export interface OCPPServerLogging {
    connections:     boolean;
    authentication:  boolean;
    messages:        boolean;
    payloads:        boolean;
    /** When the contents stop being logged again; null while they are not. */
    payloadsUntil:   string | null;
    /** Whether they are being logged at this moment - the switch and the window together. */
    payloadsNow:     boolean;
    pings:           boolean;
}


/** The server the charging stations below this local controller connect to. */
export interface OCPPServerConfiguration {
    enabled:                     boolean;
    address:                     string | null;
    port:                        number;
    /** How a client may authenticate: what OCPP calls its security profiles 1, 2 and 3. */
    clientAuthentication:        ClientAuthentication[];
    subprotocols:                string[];
    /** The names and addresses the stations connect to; what a certificate must cover. */
    reachableAs:                 string[];
    minTLSVersion:               string;
    checkCertificateRevocation:  boolean;
    maxConnections:              number;
    pingEverySeconds:            number;
    /** Whether WebSockets are taken over HTTP/3 as well, on the UDP port of the same number. */
    http3:                       boolean;
    logging:                     OCPPServerLogging;
    /** OCPP-over-SSH, on this controller's SSH server, beside the WebSocket port. */
    ssh:                         OCPPServerSSH;
    state: {
        running:            boolean;
        tls:                boolean;
        url:                string;
        /** The UDP port WebSockets are taken over HTTP/3 on, or null while none are. */
        http3Port?:         number | null;
        /** Why none are, although they were asked for. */
        http3Problem?:      string | null;
        connections:        number;
        webSocketLogins:      number;
        trustedChains:      number;
        hasCertificate:     boolean;
        /** What a station pins to reach this port: 64 hexadecimal digits, or null on an unencrypted one. */
        keyPin?:            string | null;
        /** The key pin of the announced next key, to be handed to the stations before the switch. */
        nextKeyPin?:        string | null;
        /**
         * The fields that were changed but are not in effect: the socket is
         * decided when the server is built. Empty when everything saved is
         * already doing something.
         */
        waitingForARestart: string[];
    };
    limits: {
        subprotocols:                   string[];
        clientAuthentication:           ClientAuthentication[];
        tlsVersions:                    string[];
        maxConnections:                 number;
        maxReachableAs:                 number;
        minMaxMessageSize?:             number;
        maxMaxMessageSize?:             number;
        suggestedPayloadWindowSeconds:  number;
    };
    file:  string;
}

/** OCPP-over-SSH: the subsystem the stations ask for on this controller's SSH server. */
export interface OCPPServerSSH {
    enabled:           boolean;
    /** Null where the WebSocket port's are offered. */
    subprotocols:      string[] | null;
    maxMessageSize:    number;
    pingEverySeconds:  number;
    subsystem:         string;
    state: {
        /** Whether a station can connect over SSH right now. */
        served:            boolean;
        /** Whether this controller's SSH server is on at all. */
        sshServer:         boolean;
        url:               string | null;
        /** "ssh-ed25519 SHA256:...", what a station pins. */
        hostKey:           string | null;
        connections:       number;
        stationsWithAKey:  number;
    };
}

/** What a PUT to the charging station server may carry; everything is optional. */
export interface OCPPServerUpdate {
    enabled?:                     boolean;
    address?:                     string;
    port?:                        number;
    clientAuthentication?:        ClientAuthentication[];
    subprotocols?:                string[];
    reachableAs?:                 string[];
    minTLSVersion?:               string;
    checkCertificateRevocation?:  boolean;
    maxConnections?:              number;
    pingEverySeconds?:            number;
    http3?:                       boolean;
    ssh?: {
        enabled?:  boolean;
    };
    logging?: {
        connections?:     boolean;
        authentication?:  boolean;
        messages?:        boolean;
        payloads?:        boolean;
        payloadsUntil?:   string | null;
        pings?:           boolean;
    };
}


/** How the station server of a node is asked: what a node that serves those below it has. */
export const stationServerAPI = {

    ocppServer: {

        get:   ()                          => request<OCPPServerConfiguration>('GET', '/configuration/ocpp-server'),
        /** Only the fields given are changed; the answer is the whole thing as it now stands. */
        save:  (update: OCPPServerUpdate)  => request<OCPPServerConfiguration>('PUT', '/configuration/ocpp-server', update),

        trust:   webSocketLoginsAPI.trust,
        logins:  webSocketLoginsAPI.logins,
        groups:  webSocketLoginsAPI.groups

    }

};
