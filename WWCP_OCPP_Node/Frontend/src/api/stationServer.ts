import { request }  from '@node/api/client';
import type { I18NString } from './i18n';


// What a node that serves the charging stations below it keeps for them, and
// how it is asked: its section "ocppServer", the keys and certificates it
// presents, the chains a station's certificate may lead to, and the charging
// stations that may sign in with their login groups. A local controller and a
// gateway keep them alike, and serve them by the same routes -
// OCPPServingHTTPAPI's - so the types and the calls are here, for the pages of
// both. A kind of node spreads the calls into its own api where it wants them
// there as well.
//
// One of its names is the name of something else elsewhere, and stays so here:
// a WebSocketLogin is a charging station that may sign in to this node - not the
// credentials a connection of "@ocpp/api/connections" signs in with. A module
// that exports both modules' names names one of them under another name.
//
// What the station port presents is not here: it is the node's certificate
// store's - TLS server identities told the listener "ocpp", and their server
// keys - see "@node/api/client" and the Server identities page.


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

/**
 * How a client may authenticate at a WebSocket server: by HTTP authentication
 * without TLS or over it, or by its TLS client certificate - OCPP's security
 * profiles 1, 2 and 3.
 */
export type ClientAuthentication = 'httpAuthWithoutTLS' | 'httpAuthOverTLS' | 'tlsClientCertificate';

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

/** An SSH public key a charging station signs in with for OCPP-over-SSH. Nothing about it is secret. */
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

export { inEnglish, textOf, type I18NString } from './i18n';

/** One charging station that may sign in. */
export interface WebSocketLogin {
    login:        string;
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
    description?: I18NString;
}

/** A group of logins, and what its members are allowed to do. */
export interface LoginGroup {
    id:                string;
    name:              string;
    enabled:           boolean;
    builtIn:           boolean;
    addedAt:           string;
    authMethods:       AuthMethod[];
    /** How its members may authenticate - OCPP's security profiles 1, 2 and 3. */
    clientAuthentication:  ClientAuthentication[];
    /** What its stations have over their SSH connection besides OCPP: the names of SSH subsystems. */
    sshChannels?:      string[];
    members:           number;
    description?:      I18NString;
}

/** Which charging stations may sign in, and with what. */
export interface WebSocketLogins {
    file:               string;
    enabled:            number;
    maxLogins:        number;
    maxGroups:          number;
    minPasswordLength:  number;
    minSecretLength:    number;
    defaultGroup:       string;
    groups:             LoginGroup[];
    logins:             WebSocketLogin[];
}

/** What a group may be changed to. */
export interface LoginGroupUpdate {
    id?:               string;
    name:              string;
    enabled:           boolean;
    authMethods:       AuthMethod[];
    /** How its members may authenticate - OCPP's security profiles 1, 2 and 3. */
    clientAuthentication:  ClientAuthentication[];
    /** Replaced like everything else about a group: left out, its stations have OCPP alone. */
    sshChannels?:      string[];
    description?:      I18NString;
}

/** What a one-time token may be set to. */
export interface TOTPUpdate {
    sharedSecret?:     string;
    validitySeconds?:  number;
    length?:           number;
    alphabet?:         string;
    hashAlgorithm?:    string;
    group?:            string;
    description?:      I18NString;
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

        trust: {

            get:      ()  => request<ClientTrust>('GET', '/configuration/ocpp-server/trust'),

            add:      (pem: string, name: string) =>
                          request<{ id: string; warnings: string[] }>('POST', '/configuration/ocpp-server/trust', { pem, name }),

            update:   (id: string, change: { enabled?: boolean; name?: string }) =>
                          request<ClientTrust>('PUT', `/configuration/ocpp-server/trust/${encodeURIComponent(id)}`, change),

            remove:   (id: string) =>
                          request<ClientTrust>('DELETE', `/configuration/ocpp-server/trust/${encodeURIComponent(id)}`)

        },

        logins: {

            get:      ()  => request<WebSocketLogins>('GET', '/configuration/ocpp-server/logins'),

            /**
             * Add a station or give one a new password. An empty password means
             * "make one up", and the made-up one comes back here and nowhere
             * else: it is kept only as a hash.
             */
            save:     (login: string, password: string, group: string, description?: I18NString) =>
                          request<{ login: string; password?: string; logins: WebSocketLogins }>(
                              'POST', '/configuration/ocpp-server/logins', { login, password, group, description }),

            enable:   (login: string, enabled: boolean) =>
                          request<WebSocketLogins>('PUT', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}`, { enabled }),

            move:     (login: string, group: string) =>
                          request<WebSocketLogins>('PUT', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}`, { group }),

            remove:   (login: string) =>
                          request<WebSocketLogins>('DELETE', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}`),

            /**
             * Give a station what it needs to be let in with a one-time token.
             * An empty shared secret means "make one up", and it comes back
             * here - the only place it is ever handed out.
             */
            saveTOTP: (login: string, update: TOTPUpdate) =>
                          request<{ login: string; sharedSecret?: string; logins: WebSocketLogins }>(
                              'PUT', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/totp`, update),

            /**
             * Whether a station's token has to be bound to the TLS session of
             * its connection. Its secret stays as it is.
             */
            bindTOTP:       (login: string, bound: boolean) =>
                          request<WebSocketLogins>(
                              'PUT', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/totp/binding`,
                              { tlsChannelBinding: bound }),

            removeTOTP:     (login: string) =>
                          request<WebSocketLogins>('DELETE', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/totp`),

            removePassword: (login: string) =>
                          request<WebSocketLogins>('DELETE', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/password`),

            /**
             * Give a station an SSH key for OCPP-over-SSH - one line of an
             * authorized_keys file - adding the station where it is not listed
             * yet. A public key: nothing secret goes either way.
             */
            addSSHKey: (login: string, line: string, label: string, group: string, description?: I18NString) =>
                          request<{ login: string; fingerprint: string; logins: WebSocketLogins }>(
                              'POST', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/sshKeys`, { line, label, group, description }),

            /** Take a key away; a session it opened ends. */
            removeSSHKey: (login: string, fingerprint: string) =>
                          request<WebSocketLogins>('DELETE',
                              `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/sshKeys/${fingerprintInURL(fingerprint)}`),

            /**
             * Let a station in by the key pin of its TLS client key - 64
             * hexadecimal digits, with or without colons, or "sha256//" and
             * base64 - adding the station where it is not listed yet.
             */
            addKeyPin: (login: string, pin: string, label: string, group: string, description?: I18NString) =>
                          request<{ login: string; keyPin: string; logins: WebSocketLogins }>(
                              'POST', `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/keyPins`, { pin, label, group, description }),

            /** Stop letting a station in by a key pin; the pin as the list has it, 64 hexadecimal digits. */
            removeKeyPin: (login: string, pin: string) =>
                          request<WebSocketLogins>('DELETE',
                              `/configuration/ocpp-server/logins/${encodeURIComponent(login)}/keyPins/${encodeURIComponent(pin)}`)

        },

        groups: {

            /** Everything about the group is replaced, not merged. */
            save:     (update: LoginGroupUpdate) =>
                          update.id === undefined
                              ? request<WebSocketLogins>('POST', '/configuration/ocpp-server/groups', update)
                              : request<WebSocketLogins>('PUT',  `/configuration/ocpp-server/groups/${encodeURIComponent(update.id)}`, update),

            remove:   (id: string) =>
                          request<WebSocketLogins>('DELETE', `/configuration/ocpp-server/groups/${encodeURIComponent(id)}`)

        }

    }

};
