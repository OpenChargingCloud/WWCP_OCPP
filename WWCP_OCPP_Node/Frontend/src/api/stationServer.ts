import { apiURL, request }  from '@node/api/client';


// What a node that serves the charging stations below it keeps for them, and
// how it is asked: its section "ocppServer", the keys and certificates it
// presents, the chains a station's certificate may lead to, and the charging
// stations that may sign in with their login groups. A local controller and a
// gateway keep them alike, and serve them by the same routes -
// OCPPServingHTTPAPI's - so the types and the calls are here, for the pages of
// both. A kind of node spreads the calls into its own api where it wants them
// there as well.
//
// Two of its names are names of something else elsewhere, and stay so here: a
// StationLogin is a charging station that may sign in to this node - not the
// credentials a connection of "@ocpp/api/connections" signs in with - and a
// KeyAlgorithm is a kind of key the node presents as a server. A module that
// exports both modules' names names one of them under another name.


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
    /** 1, 2 and 3 - the OCPP security profiles a station may connect with. */
    securityProfiles:            number[];
    subprotocols:                string[];
    /** The names and addresses the stations dial; what a certificate must cover. */
    reachableAs:                 string[];
    minTLSVersion:               string;
    checkCertificateRevocation:  boolean;
    maxConnections:              number;
    pingEverySeconds:            number;
    /** Whether WebSockets are taken over HTTP/3 as well, on the UDP port of the same number. */
    http3:                       boolean;
    logging:                     OCPPServerLogging;
    /** OCPP over SSH, on this controller's SSH server, beside the WebSocket port. */
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
        stationLogins:      number;
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
        securityProfiles:               number[];
        tlsVersions:                    string[];
        maxConnections:                 number;
        maxReachableAs:                 number;
        minMaxMessageSize?:             number;
        maxMaxMessageSize?:             number;
        suggestedPayloadWindowSeconds:  number;
    };
    file:  string;
}

/** OCPP over SSH: the subsystem the stations ask for on this controller's SSH server. */
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
    securityProfiles?:            number[];
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

/** A kind of key this local controller will make for itself. */
export interface KeyAlgorithm {
    id:           string;
    name:         string;
    /** What somebody choosing it should know. */
    remark:       string;
    /**
     * Whether this platform is known to be able to present a certificate with
     * such a key, or absent while nobody has tried. Found out by doing a TLS
     * handshake, not from a list - it depends on the operating system, the
     * runtime and the year.
     */
    presentable?: boolean;
}

/** One key of this local controller, and the certificate it was given. */
export interface ServerCertificate {
    /** Where the public key hashes to; what the signing request is filed under. */
    id:              string;
    algorithm:       string;
    createdAt:       string;
    subject:         string;
    hasCertificate:  boolean;
    /**
     * Whether this machine can hold this certificate up to a charging station
     * during a TLS handshake. A different question from whether the certificate
     * is any good: an Ed448 or an ML-DSA key makes a perfectly valid one that
     * this platform's TLS stack will not serve.
     */
    canBePresented:  boolean;
    /** Whether this is the one being presented to the charging stations. */
    inUse:           boolean;
    warnings:        string[];
    /** SHA-256 over the key's SubjectPublicKeyInfo, 64 lower-case hexadecimal digits: the same in every wrapper. */
    keyPin?:         string;
    /** Whether it is a server key, presented in a self-signed carrier certificate where no authority issued one. */
    hasCarrier?:     boolean;
    /** Whether it is the announced next key, presented only once somebody switches to it. */
    next?:           boolean;
    /** Since when it is the current server key, if it ever was. */
    currentSince?:   string | null;
    /** What it is presented in at the next handshake - or null where it is not. */
    presented?:      'certificate' | 'carrier' | null;
    carrier?: {
        subject:     string;
        thumbprint:  string;
    };
    certificate?: {
        subject:          string;
        issuer:           string;
        serialNumber:     string;
        thumbprint:       string;
        notBefore:        string;
        notAfter:         string;
        subjectAltNames:  string[];
        intermediates:    number;
        /** "pending" before its window, "valid" inside it, "expired" after. */
        state:            'pending' | 'valid' | 'expired';
        daysRemaining:    number;
    };
}

/** The keys and certificates this local controller presents. */
export interface ServerCertificates {
    directory:             string;
    /** By the controller's own clock, which is what decides the windows below. */
    now:                   string;
    servedId:              string | null;
    entries:               ServerCertificate[];
    algorithms:            KeyAlgorithm[];
    /** Always false, and said out loud: a private key is made here and never arrives. */
    canImportPrivateKeys:  boolean;
    /** The key pins a charging station pins: the current key's first, and the announced next one's. */
    keyPins?: {
        current:  ServerKeyPin & { presented: 'certificate' | 'carrier' | null } | null;
        next:     ServerKeyPin | null;
    };
}

/** The key pin of one key of this local controller, and the key it is of. */
export interface ServerKeyPin {
    id:         string;
    keyPin:     string;
    createdAt:  string;
}

/** One certificate found in a text or a file, and the key of this controller it is for. */
export interface ServerCertificateFound {
    id:             string;
    thumbprint:     string;
    label:          string;
    subject:        string;
    issuer:         string;
    notBefore:      string;
    notAfter:       string;
    chainLength:    number;
    /** Whether it came with its private key - which is refused: none arrives here. */
    hasPrivateKey:  boolean;
    /** The key of this controller it belongs to, by its public key; null for none. */
    keyId:          string | null;
    /** Why it would not be taken in, as far as can be said before trying; null where nothing speaks against it. */
    refusal:        string | null;
}

/** What an upload of several certificates did, one by one. */
export interface ServerCertificatesUploaded {
    taken:    { id: string; label: string; warnings: string[] }[];
    refused:  { label: string; error: string }[];
    error?:   string;
}

/** One chain a charging station's certificate may lead to. */
export interface TrustedChain {
    id:             string;
    name:           string;
    addedAt:        string;
    enabled:        boolean;
    subject:        string;
    issuer:         string;
    serialNumber:   string;
    thumbprint:     string;
    notBefore:      string;
    notAfter:       string;
    /** Whether it can vouch for others at all, or only for itself. */
    isCA:           boolean;
    intermediates:  number;
    warnings:       string[];
    state:          'pending' | 'valid' | 'expired';
    daysRemaining:  number;
}

/** Which chains a charging station's own certificate may lead to. */
export interface ClientTrust {
    directory:   string;
    now:         string;
    enabled:     number;
    maxEntries:  number;
    entries:     TrustedChain[];
}

/** A way a charging station can prove who it is. */
export type AuthMethod = 'basic' | 'totp' | 'certificate' | 'sshkey' | 'keypin';

/**
 * The key pin of a TLS client key a charging station is let in by: SHA-256
 * over its SubjectPublicKeyInfo. Nothing about it is secret.
 */
export interface StationKeyPin {
    /** 64 lower-case hexadecimal digits. */
    pin:       string;
    label?:    string;
    added:     string;
    addedBy?:  string;
}

/** An SSH public key a charging station signs in with for OCPP over SSH. Nothing about it is secret. */
export interface StationSSHKey {
    /** "SHA256:...", as OpenSSH writes it: what the key is known by. */
    fingerprint:  string;
    /** The authorized_keys line it was given as, its options in front. */
    line:         string;
    label?:       string;
    created:      string;
    createdBy?:   string;
    isDisabled?:  boolean;
}

/**
 * What a charging station needs in order to be let in with a one-time token.
 *
 * The shared secret is never in here: it is the one credential the controller
 * has to keep readable, so it is handed out once when it is set and never put
 * into the list this page reads.
 */
export interface TOTPSettings {
    validitySeconds:  number;
    length:           number;
    alphabet:         string;
    hashAlgorithm:    'SHA256' | 'SHA384' | 'SHA512';
    /** There, and true, where the token has to be bound to the TLS session of its connection. */
    tlsChannelBinding?: boolean;
}

/** One charging station that may sign in. */
export interface StationLogin {
    id:           string;
    group:        string;
    enabled:      boolean;
    addedAt:      string;
    hasPassword:  boolean;
    hasTOTP:      boolean;
    totp?:        TOTPSettings;
    hasSSHKey?:   boolean;
    sshKeys?:     StationSSHKey[];
    hasKeyPin?:   boolean;
    keyPins?:     StationKeyPin[];
    note?:        string;
}

/** A group of logins, and what its members are allowed to do. */
export interface LoginGroup {
    id:                string;
    name:              string;
    enabled:           boolean;
    builtIn:           boolean;
    addedAt:           string;
    authMethods:       AuthMethod[];
    securityProfiles:  number[];
    /** What its stations have over their SSH connection besides OCPP: the names of SSH subsystems. */
    sshChannels?:      string[];
    members:           number;
    note?:             string;
}

/** Which charging stations may sign in, and with what. */
export interface StationLogins {
    file:               string;
    enabled:            number;
    maxStations:        number;
    maxGroups:          number;
    minPasswordLength:  number;
    minSecretLength:    number;
    defaultGroup:       string;
    groups:             LoginGroup[];
    stations:           StationLogin[];
}

/** What a group may be changed to. */
export interface LoginGroupUpdate {
    id?:               string;
    name:              string;
    enabled:           boolean;
    authMethods:       AuthMethod[];
    securityProfiles:  number[];
    /** Replaced like everything else about a group: left out, its stations have OCPP alone. */
    sshChannels?:      string[];
    note?:             string;
}

/** What a one-time token may be set to. */
export interface TOTPUpdate {
    sharedSecret?:     string;
    validitySeconds?:  number;
    length?:           number;
    alphabet?:         string;
    hashAlgorithm?:    string;
    group?:            string;
    note?:             string;
}


/**
 * An SSH key's fingerprint as it goes into a URL: without "SHA256:", and in
 * the URL-safe Base64 alphabet, where '/' would be read as a path - as the
 * controller reads it back.
 */
export function fingerprintInURL(fingerprint: string): string {
    return fingerprint.replace(/^SHA256:/, '').replace(/\+/g, '-').replace(/\//g, '_');
}


/** How the station server of a node is asked: what a node that serves those below it has. */
export const stationServerAPI = {

    ocppServer: {

        get:   ()                          => request<OCPPServerConfiguration>('GET', '/configuration/ocpp-server'),
        /** Only the fields given are changed; the answer is the whole thing as it now stands. */
        save:  (update: OCPPServerUpdate)  => request<OCPPServerConfiguration>('PUT', '/configuration/ocpp-server', update),

        certificates: {

            get:     ()  => request<ServerCertificates>('GET', '/configuration/ocpp-server/certificates'),

            /**
             * Generate a key and the signing request that goes with it; the key
             * never leaves.
             *
             * Given three minutes rather than the half of one any other write
             * gets, because an RSA key is a search for two primes that takes as
             * long as it takes: measured here, an RSA 4096 key took between 0.4
             * and 7 seconds on a desktop, and a local controller may well be a
             * board many times slower than that.
             */
            create:  (subject: string, algorithm: string) =>
                         request<{ id: string; csr: string }>('POST', '/configuration/ocpp-server/certificates',
                                                              { subject, algorithm }, 3 * 60_000),

            /** Where the signing request can be downloaded; a plain file, not JSON. */
            csrURL:  (id: string) => apiURL(`/configuration/ocpp-server/certificates/${encodeURIComponent(id)}/csr`),

            /** Take in the certificate that answers a request, with its intermediates. */
            upload:  (id: string, pem: string) =>
                         request<{ id: string; warnings: string[] }>('PUT',
                             `/configuration/ocpp-server/certificates/${encodeURIComponent(id)}`, { pem }),

            remove:  (id: string) =>
                         request<ServerCertificates>('DELETE',
                             `/configuration/ocpp-server/certificates/${encodeURIComponent(id)}`),

            /** What a text or a file holds, and which key of this controller each certificate is for - nothing taken in. */
            inspect: (what: { pem?: string; content?: string; password?: string }) =>
                         request<{ certificates: ServerCertificateFound[]; pem: string }>('POST',
                             '/configuration/ocpp-server/certificates/inspect', what),

            /** Every certificate of a text or a file taken in under the key it is for. */
            uploadAll: (what: { pem?: string; content?: string; password?: string }) =>
                         request<ServerCertificatesUploaded>('POST',
                             '/configuration/ocpp-server/certificates/upload', what)

        },

        /**
         * The server key the charging stations pin, presented in a carrier
         * certificate made from it: the first one where nothing is presented
         * yet, or the next one, announced ahead of a rollover. Given the time
         * a key takes to make, as above.
         */
        serverKeys: {

            create:  (algorithm: string, next: boolean) =>
                         request<{ id: string; keyPin: string; next: boolean; certificates: ServerCertificates }>(
                             'POST', '/configuration/ocpp-server/serverKeys', { algorithm, next }, 3 * 60_000),

            /** Present the next key from now on; stations that pinned only the key before are refused. */
            switch:  () =>
                         request<ServerCertificates>('POST', '/configuration/ocpp-server/serverKeys/switch', {})

        },

        trust: {

            get:      ()  => request<ClientTrust>('GET', '/configuration/ocpp-server/trust'),

            add:      (pem: string, name: string) =>
                          request<{ id: string; warnings: string[] }>('POST', '/configuration/ocpp-server/trust', { pem, name }),

            update:   (id: string, change: { enabled?: boolean; name?: string }) =>
                          request<ClientTrust>('PUT', `/configuration/ocpp-server/trust/${encodeURIComponent(id)}`, change),

            remove:   (id: string) =>
                          request<ClientTrust>('DELETE', `/configuration/ocpp-server/trust/${encodeURIComponent(id)}`)

        },

        stations: {

            get:      ()  => request<StationLogins>('GET', '/configuration/ocpp-server/stations'),

            /**
             * Add a station or give one a new password. An empty password means
             * "make one up", and the made-up one comes back here and nowhere
             * else: it is kept only as a hash.
             */
            save:     (id: string, password: string, group: string, note: string) =>
                          request<{ id: string; password?: string; stations: StationLogins }>(
                              'POST', '/configuration/ocpp-server/stations', { id, password, group, note }),

            enable:   (id: string, enabled: boolean) =>
                          request<StationLogins>('PUT', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}`, { enabled }),

            move:     (id: string, group: string) =>
                          request<StationLogins>('PUT', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}`, { group }),

            remove:   (id: string) =>
                          request<StationLogins>('DELETE', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}`),

            /**
             * Give a station what it needs to be let in with a one-time token.
             * An empty shared secret means "make one up", and it comes back
             * here - the only place it is ever handed out.
             */
            saveTOTP: (id: string, update: TOTPUpdate) =>
                          request<{ id: string; sharedSecret?: string; stations: StationLogins }>(
                              'PUT', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/totp`, update),

            /**
             * Whether a station's token has to be bound to the TLS session of
             * its connection. Its secret stays as it is.
             */
            bindTOTP:       (id: string, bound: boolean) =>
                          request<StationLogins>(
                              'PUT', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/totp/binding`,
                              { tlsChannelBinding: bound }),

            removeTOTP:     (id: string) =>
                          request<StationLogins>('DELETE', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/totp`),

            removePassword: (id: string) =>
                          request<StationLogins>('DELETE', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/password`),

            /**
             * Give a station an SSH key for OCPP over SSH - one line of an
             * authorized_keys file - adding the station where it is not listed
             * yet. A public key: nothing secret goes either way.
             */
            addSSHKey: (id: string, line: string, label: string, group: string, note: string) =>
                          request<{ id: string; fingerprint: string; stations: StationLogins }>(
                              'POST', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/sshKeys`, { line, label, group, note }),

            /** Take a key away; a session it opened ends. */
            removeSSHKey: (id: string, fingerprint: string) =>
                          request<StationLogins>('DELETE',
                              `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/sshKeys/${fingerprintInURL(fingerprint)}`),

            /**
             * Let a station in by the key pin of its TLS client key - 64
             * hexadecimal digits, with or without colons, or "sha256//" and
             * base64 - adding the station where it is not listed yet.
             */
            addKeyPin: (id: string, pin: string, label: string, group: string, note: string) =>
                          request<{ id: string; keyPin: string; stations: StationLogins }>(
                              'POST', `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/keyPins`, { pin, label, group, note }),

            /** Stop letting a station in by a key pin; the pin as the list has it, 64 hexadecimal digits. */
            removeKeyPin: (id: string, pin: string) =>
                          request<StationLogins>('DELETE',
                              `/configuration/ocpp-server/stations/${encodeURIComponent(id)}/keyPins/${encodeURIComponent(pin)}`)

        },

        groups: {

            /** Everything about the group is replaced, not merged. */
            save:     (update: LoginGroupUpdate) =>
                          update.id === undefined
                              ? request<StationLogins>('POST', '/configuration/ocpp-server/groups', update)
                              : request<StationLogins>('PUT',  `/configuration/ocpp-server/groups/${encodeURIComponent(update.id)}`, update),

            remove:   (id: string) =>
                          request<StationLogins>('DELETE', `/configuration/ocpp-server/groups/${encodeURIComponent(id)}`)

        }

    }

};
