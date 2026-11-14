import { afterAsking,
         request,
         type PinKeys,
         type ServerJudgement }  from '@node/api/client';


// What a node that dials its back ends keeps for it, and how it is asked: the
// credentials it proves itself with, the connections it dials and where each
// of them stands, and the client keys it holds up. A charging station and a
// local controller keep them alike, and serve them by the same routes -
// OCPPDiallingHTTPAPI's - so the types and the calls are here, for the pages
// of both. A kind of node names them in its own api/client.ts as well, and
// spreads the calls into its own api.


/**
 * One set of credentials this station can prove itself with.
 *
 * The secret half is never in here. `hasSecret` is what takes its place, so a
 * page can tell "not configured yet" from "configured, and you are not being
 * shown it".
 */
export interface StationLogin {
    id:                  string;
    /** What somebody wrote down that it is for, e.g. "CSMS login". */
    description:         string;
    kind:                'basic' | 'totp';
    /** The name the other end knows this station by. */
    login:               string;
    createdAt:           string;
    hasSecret:           boolean;
    /** Whether the one-time password is bound to the TLS session. TOTP only. */
    tlsChannelBinding?:  boolean;
    validitySeconds?:    number;
    length?:             number;
    alphabet?:           string;
    hashAlgorithm?:      string;
}

/** What goes in when credentials are written down or changed. */
export interface LoginToSave {
    id?:                 string;
    description:         string;
    kind:                'basic' | 'totp';
    login:               string;
    /** Empty means "keep whatever is already there". */
    secret:              string;
    validitySeconds?:    number;
    length?:             number;
    alphabet?:           string;
    hashAlgorithm?:      string;
    tlsChannelBinding?:  boolean;
}

/** One place this station dials. */
export interface StationConnection {
    id:                  string;
    description:         string;
    url:                 string;
    connectionType:      string;
    /** Which OCPP is spoken here, and so which of this station's two nodes dials. */
    ocppVersion:         string;
    autoConnect:  boolean;
    /** Whether the URL makes a TLS connection, which decides what the rest can mean. */
    secure:              boolean;
    /** Over an HTTP WebSocket, or over the SSH subsystem ocpp@open.charging.cloud. */
    transport?:          'websocket' | 'ssh';
    createdAt:           string;
    authenticationId?:   string;
    certificateId?:      string;
    /** The host keys the server of an ssh:// connection may show, separated by commas; none where nothing is pinned. */
    sshHostKey?:         string;
    /** Whether an ssh:// connection that pins nothing learns the first host key shown, on probation. */
    sshTrustOnFirstUse?: 'None' | 'HostKey';
    /** What a host key that matches no pin leads to. */
    sshOnPinMismatch?:   'Refuse' | 'Record' | 'Accept';
    /** How long a learned host key waits for a signed pin set, in seconds. */
    sshProbationTime?:   number;
    /** The version of the signed pin set the pins came from. */
    sshHostKeysVersion?: number;
    /** The host key learned on the first connection, while it is on probation. */
    sshLearnedHostKey?:  string;
    sshLearnedAt?:       string;
    /**
     * Whether the server of a TLS connection is known by the key pin of its key
     * rather than by a CA: it has key pins, or learns one on first use. Only
     * for wss:// and https://.
     */
    keyPinned?:                  boolean;
    /** The key pins of the keys its server may show: 64 lowercase hexadecimal digits each, at most four. */
    serverKeyPins?:              string[];
    /** The version of the signed pin set the key pins came from. */
    serverKeyPinsVersion?:       number;
    /** Where the key pins come from: written down, learned and on probation, or learned and confirmed since. */
    serverKeyPinState?:          'Configured' | 'Learned' | 'Confirmed';
    /** Whether raw public keys (RFC 7250) are offered; this station's TLS stack cannot offer them yet. */
    rawPublicKeys?:              'Off' | 'Prefer' | 'Require';
    /** Whether a wss:// connection that pins no key learns the key pin of the first one shown, on probation. */
    trustOnFirstUse?:            'None' | 'Key';
    /** How long a learned key pin waits for a signed pin set, in seconds. */
    serverKeyPinProbationTime?:  number;
    /** The key pin learned on the first connection, while it is on probation. */
    serverKeyPinLearned?:        string;
    serverKeyPinLearnedAt?:      string;
    /**
     * What the server of a wss:// connection checked by its CA is held to
     * beside that, in the keys a time server, a name server and a meter are
     * held to something with. Never beside key pins.
     */
    pins?:                       PinKeys;
    /** The version of the signed pin set the roots came from. */
    pinsVersion?:                number;
    /** What the station made of the certificate its server showed last, where it is checked by its CA. */
    judgement?:                  ServerJudgement | null;
    /**
     * The version of HTTP its WebSocket is opened over, where it names one:
     * "1.1", "2" or "3", and that alone. Left out, it is "auto": the station
     * offers HTTP/2 and HTTP/1.1 and the server chooses, and goes over
     * HTTP/3 once the server said it is reached over it.
     */
    httpVersion?:                HTTPVersionOfAConnection;
    /** The version of HTTP the WebSocket it has now was opened over. */
    httpVersionInUse?:           '1.1' | '2' | '3';
    /**
     * How its OCPP messages are written, where it is not JSON: "cbor" offers
     * the CBOR subprotocol of its OCPP version and nothing else, "auto"
     * offers it first and takes what the back end chooses. Left out, it is "json".
     */
    encoding?:                   EncodingOfAConnection;
    /** The subprotocol the connection it has now agreed on, over a WebSocket or over ssh://: "ocpp2.1", "ocpp2.1cbor". */
    subprotocolInUse?:           string;
    /**
     * Which signatures the messages the station signs for it get: "jws",
     * "cose" or "both". Left out, the signature policy of the station decides.
     */
    signatureFormats?:           SignatureFormatsOfAConnection;
    warnings?:           string[];
}

/** The version of HTTP a ws:// or wss:// connection opens its WebSocket over. */
export type HTTPVersionOfAConnection = 'auto' | '1.1' | '2' | '3';

/** How the OCPP messages of a connection are written. */
export type EncodingOfAConnection = 'json' | 'cbor' | 'auto';

/** Which signatures a connection writes; "policy" is what the signature policy of the station says. */
export type SignatureFormatsOfAConnection = 'policy' | 'jws' | 'cose' | 'both';

/** What goes in when a connection is written down or changed. */
export interface ConnectionToSave {
    /** The version of HTTP its WebSocket is opened over; left out, the station keeps the one it has. */
    httpVersion?:        HTTPVersionOfAConnection;
    /** How its OCPP messages are written; left out, the station keeps what it says. */
    encoding?:           EncodingOfAConnection;
    /** Which signatures it writes; left out, the station keeps what it says, and "policy" is saying nothing. */
    signatureFormats?:   SignatureFormatsOfAConnection;
    id?:                 string;
    description:         string;
    url:                 string;
    connectionType:      string;
    ocppVersion:         string;
    autoConnect:  boolean;
    authenticationId:    string | null;
    certificateId:       string | null;
    /** The host keys typed, separated by commas: "" for none. */
    sshHostKey?:         string;
    sshTrustOnFirstUse?: 'None' | 'HostKey';
    sshOnPinMismatch?:   'Refuse' | 'Record' | 'Accept';
    sshProbationTime?:   number;
    /**
     * The host keys the page showed when it was drawn: where those typed are
     * still those, the station keeps the ones it has now - taken from a signed
     * pin set meanwhile, perhaps.
     */
    sshHostKeyAsShown?:  string;
    /** The key pins typed, one each: none for none. */
    serverKeyPins?:              string[];
    /** The key pins the page showed when it was drawn, as for the host keys. */
    serverKeyPinsAsShown?:       string[];
    rawPublicKeys?:              'Off' | 'Prefer' | 'Require';
    trustOnFirstUse?:            'None' | 'Key';
    serverKeyPinProbationTime?:  number;
    /** What its server is to be held to; nothing in it for nothing. */
    pins?:                       PinKeys;
    /** What the page showed it held to when it was drawn: what was learned since, or taken from a signed pin set, stays. */
    pinsAsShown?:                PinKeys;
}

/** A key a signed pin set is believed of: the point of an elliptic curve, in base64, as an OCPP signature's keyId carries it. */
export interface PinSetSigner {
    publicKey:    string;
    description:  string;
    createdAt:    string;
}

/** The key this station signs in with over SSH. */
export interface StationSshKey {
    /** Its public half, as a line of authorized_keys. */
    publicKey:    string;
    fingerprint:  string;
    path:         string;
}

/** One line of what happened while a connection was being tested. */
export interface ConnectionTestStep {
    /** Milliseconds since the test started. */
    at_ms:  number;
    level:  'info' | 'notice' | 'warning' | 'error';
    text:   string;
}

/**
 * What a connection test asks its back end in OCPP once it is connected: a
 * Heartbeat, or nothing - everything up to the handshake and no further.
 */
export type ConnectionTestAsks = 'heartbeat' | 'none';

/** How long the station waits for the answer to what a connection test asked, in seconds. */
export const connectionTestAnswerWithin = 10;

/** What came of testing one connection. */
export interface ConnectionTest {
    description:  string;
    url:          string;
    ok:           boolean;
    runtime_ms:   number;
    steps:        ConnectionTestStep[];
    /** The key pin of the key the server of a wss:// connection showed, believed or not: what it would be pinned to. */
    serverKeyPin?: string;
    /** What the test asked in OCPP once it was connected, and what came of it; absent where it did not come that far, or asked nothing. */
    ocpp?: {
        request:        string;
        sent:           unknown[];
        sentAt_ms:      number;
        answered:       boolean;
        answer?:        unknown[];
        /** Whether the answer is a CALLERROR: an answer in OCPP all the same. */
        error?:         boolean;
        roundTrip_ms?:  number;
    };
}

/**
 * Where a connection this station dialled stands.
 *
 * Three kinds of "not connected", because they ask three different things of
 * whoever is looking: lost and trying are waited out, the station comes back by
 * itself; refused needs the other end fixed; not dialled and failed need this
 * end fixed. And one that is not connected on purpose: standby, an endpoint of
 * a back end that has more than one, dialled when the one dialled now cannot be
 * reached.
 */
export type ConnectionStatus = 'connected' | 'lost' | 'trying' | 'refused' | 'notDialled' | 'failed' | 'standby';

/** What became of one connection this station dialled, and since when. */
export interface ConnectionState {
    status:          ConnectionStatus;
    since:           string;
    /** The sentence the station logged when it came to stand there. */
    said:            string;
    /** What was dialled - which is what is written down now only until something is changed. */
    description:     string;
    url:             string;
    ocppVersion:     string;
    /** While it is tried again: which attempt comes next, and when. */
    attempt?:        number;
    nextAttemptAt?:  string;
}

/** Where every connection this station dialled stands, and what time it is there. */
export interface ConnectionStates {
    /** The station's own clock, which is what "since" and "next" are counted from. */
    timestamp:  string;
    states:     Record<string, ConnectionState>;
}

/** A client certificate, as far as a connection is concerned. */
export interface ConnectionCertificate {
    id:                  string;
    subject:             string;
    algorithm:           string;
    hasCertificate:      boolean;
    canBeHeldUp:         boolean;
}

/**
 * Everything the two pages work from.
 *
 * One shape for both, from one handler on the station: the Connections page
 * needs the credentials in order to offer them, and the Authentication page
 * needs the connections in order to say which ones a removal would break.
 */
export interface StationConnections extends ConnectionStates {
    directory:             string;
    authentications:       StationLogin[];
    connections:           StationConnection[];
    certificates:          ConnectionCertificate[];
    /** Null until the first ssh:// connection made it. */
    sshClientKey?:         StationSshKey | null;
    /** The keys a pin set for a connection over SSH is believed of. */
    pinSetSigners?:        PinSetSigner[];
    connectionTypes:       string[];
    ocppVersions:          string[];
    maxDescriptionLength:  number;
    minSharedSecretLength: number;
    /** Always false, and said out loud: a secret is written here and never read back. */
    secretsAreReadable:    boolean;
    totpDefaults: {
        validitySeconds:   number;
        length:            number;
        alphabet:          string;
        hashAlgorithm:     string;
    };
}

/** One kind of key this station will make for itself. */
export interface KeyAlgorithm {
    /** How it is written in the API, e.g. "ed448". */
    id:       string;
    /** How it is written on a page, e.g. "Ed448". */
    name:     string;
    /** What somebody choosing it should know. */
    remark:   string;
}

/** One key usage a signing request may ask for beside the ones the kind of key brings by itself. */
export interface KeyUsage {
    /** How it is written in the API and in X.509, e.g. "keyAgreement". */
    id:          string;
    name:        string;
    remark:      string;
    /** The kinds of key that can be used that way, by their id. */
    algorithms:  string[];
}

/** One extended key usage a signing request may ask for beside clientAuth. */
export interface ExtendedKeyUsage {
    /** How it is written in the API and in X.509, e.g. "serverAuth". */
    id:      string;
    name:    string;
    remark:  string;
    oid:     string;
}

/** A profile a signing request is made to: "ocpp", "modbus-tls". */
export interface KeyTemplate {
    id:                 string;
    name:               string;
    remark:             string;
    /** What it asks for by itself, beside what every request asks for. */
    keyUsages:          string[];
    extendedKeyUsages:  string[];
    /** The role it asks for, where it asks for one: what the profile names, though any may be typed. */
    role?: {
        label:      string;
        roles:      string[];
        maxLength:  number;
    };
}

/** What a signing request asks for beyond a subject and a kind of key; everything may be left out. */
export interface KeyAsks {
    template?:           string;
    role?:               string;
    /** Beside a signature, and enciphering a key where the key can. */
    keyUsages?:          string[];
    /** Beside clientAuth; a name, or an object identifier in dots. */
    extendedKeyUsages?:  string[];
}

/** One key of this station, its signing request, and its certificate if it has one. */
export interface StationKey {
    id:            string;
    algorithm:     string;
    createdAt:     string;
    subject:       string;
    /** Whether this is the one the station would hold up when it dials. */
    inUse:         boolean;
    /**
     * Whether this station can load the certificate together with its key at
     * all. False is about the platform and not about the certificate: .NET has
     * no key object for an Ed448 or an ML-DSA key today.
     */
    canBeHeldUp:   boolean;
    cannotBeHeldUp?: string;
    /** The request waiting to be collected, while there is no certificate yet. */
    csr?:          string;
    /** What its request asked for; all of its key usages, where they were written down. */
    asksFor?: {
        template:           string;
        templateName:       string;
        role:               string | null;
        keyUsages:          string[];
        extendedKeyUsages:  string[];
    };
    /** When its request was sent to the CSMS, while its certificate has not come. */
    sentToCSMSAt?: string;
    certificate?: {
        subject:           string;
        issuer:            string;
        serialNumber:      string;
        notBefore:         string;
        notAfter:          string;
        thumbprintSHA256:  string;
        /** How many were sent along between it and a root. */
        intermediates:     number;
        /** Negative once it has run out. */
        daysLeft:          number;
        expired:           boolean;
        notYetValid:       boolean;
    };
    warnings?:     string[];
}

/** The keys and certificates this station dials a back end with. */
export interface StationCertificates {
    directory:             string;
    /** What the station thinks the time is, so a page does not guess which clock the days are counted by. */
    now:                   string;
    inUseId:               string | null;
    entries:               StationKey[];
    algorithms:            KeyAlgorithm[];
    defaultAlgorithm:      string;
    maxSubjectLength:      number;
    /** Always false, and said out loud: a key that arrived from elsewhere is one somebody else has a copy of. */
    canImportPrivateKeys:  boolean;
    /** What a request may be made to, and may ask for beside the usual; left out by a node that asks for a client certificate and nothing else. */
    templates?:            KeyTemplate[];
    defaultTemplate?:      string;
    keyUsages?:            KeyUsage[];
    extendedKeyUsages?:    ExtendedKeyUsage[];
    /** Whether a request can be sent to the CSMS from here, in SignCertificate: a charging station's. */
    canBeSentToCSMS?:      boolean;
}


/**
 * The routes of the credentials, the connections and the client keys, as a
 * kind of node has them in its own api: api.authentications, api.connections
 * and api.clientKeys.
 */
export const connectionsAPI = {

    authentications: {

        get:     ()                  => request<StationConnections>('GET', '/configuration/authentications'),

        add:     (entry: LoginToSave) =>
                     request<{ id: string; connections: StationConnections }>(
                         'POST', '/configuration/authentications', entry),

        /** A secret left empty keeps the one already there. */
        update:  (entry: LoginToSave) =>
                     request<StationConnections>('POST', '/configuration/authentications/update', entry),

        /** Refused while a connection is using it, and the refusal names it. */
        remove:  (id: string) =>
                     request<StationConnections>('POST', '/configuration/authentications/remove', { id })

    },

    connections: {

        get:     ()                       => request<StationConnections>('GET', '/configuration/connections'),

        add:     (entry: ConnectionToSave) =>
                     request<{ id: string; connections: StationConnections }>(
                         'POST', '/configuration/connections', entry),

        update:  (entry: ConnectionToSave) =>
                     request<StationConnections>('POST', '/configuration/connections/update', entry),

        remove:  (id: string) =>
                     request<StationConnections>('POST', '/configuration/connections/remove', { id }),

        /** A key a pin set for a connection over SSH is believed of - the CSMS's signing anchor. */
        addPinSetSigner:     (publicKey: string, description: string) =>
                                 request<StationConnections>('POST', '/configuration/connections/pinSetSigners', { publicKey, description }),

        removePinSetSigner:  (publicKey: string) =>
                                 request<StationConnections>('POST', '/configuration/connections/pinSetSigners/remove', { publicKey }),

        /**
         * Where each connection this station dialled stands - asked again and
         * again while the page is open, and therefore only that, rather than
         * everything the page was drawn from.
         */
        states:  ()  => request<ConnectionStates>('GET', '/status/connections'),

        /**
         * Make this connection once, and say everything that happened.
         *
         * Takes the fields rather than an identification, because the page
         * offers this beside a connection being written down for the first
         * time as well as beside one that exists - and both times what
         * somebody means is "test what is on the screen". Nothing is stored.
         *
         * Slower than the other writes on purpose: the station stays connected
         * for about two seconds to see whether anything is said, so the
         * deadline has to cover the connection plus that - and, where the
         * back end is asked something, the time the station waits for an
         * answer that may never come.
         */
        test:    (entry: ConnectionToSave, asks: ConnectionTestAsks = 'heartbeat') =>
                     request<ConnectionTest>('POST', '/configuration/connections/test', { ...entry, ocppRequest: asks },
                                             afterAsking(asks === 'none' ? [ 2 ] : [ connectionTestAnswerWithin, 2 ]))

    },

    /** The keys this station dials its back ends with, made here and never imported. */
    clientKeys: {

        get:     ()  => request<StationCertificates>('GET', '/configuration/certificates'),

        /**
         * A new key and the signing request to be handed to whoever issues
         * certificates for this station.
         *
         * Slower than the other writes by a wide margin - an RSA 4096 or an
         * SLH-DSA key takes seconds to generate, and the deadline for a write
         * covers it.
         */
        create:  (subject: string, algorithm: string, asks: KeyAsks = {}) =>
                     request<{ id: string; csr: string; certificates: StationCertificates }>(
                         'POST', '/configuration/certificates', { subject, algorithm, ...asks }),

        /**
         * Send the request of a key that waits for its certificate to the
         * CSMS, in SignCertificate - where the node says it can, see
         * canBeSentToCSMS. Answered once the CSMS has, which is why the
         * deadline covers the time a node waits for an answer over OCPP.
         */
        sendToCSMS:  (id: string) =>
                         request<{ said: string; certificates: StationCertificates }>(
                             'POST', '/configuration/certificates/sign', { id }, afterAsking([ 30 ])),

        /** The certificate that came back, and whatever intermediates came with it. */
        add:     (pem: string) =>
                     request<{ id: string; warnings: string[]; certificates: StationCertificates }>(
                         'POST', '/configuration/certificates/import', { pem }),

        /** The identification travels in the body, as it does everywhere else in this API. */
        remove:  (id: string) =>
                     request<StationCertificates>('POST', '/configuration/certificates/remove', { id })

    }

};
