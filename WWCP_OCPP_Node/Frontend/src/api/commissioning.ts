import { request }  from '@node/api/client';

/*
 * Commissioning over SSH, as every node that serves OCPP has it: a tool
 * speaking OCPP to the node on its own SSH server, as an account that may run
 * it - under the account's name, never as the CSMS.
 */

/** Commissioning over SSH, as GET /configuration/commissioning reads it. */
export interface CommissioningConfiguration {
    /** What the file says: switched on or not. */
    enabled:       boolean;
    /** Whether a tool is served now. */
    served:        boolean;
    /** Whether the node's SSH server is on at all. */
    sshServer:     boolean;
    /** Where a tool connects, as its account; null without an SSH server. */
    url:           string | null;
    /** The SSH server's host key, "ssh-ed25519 SHA256:...", for the tool to pin. */
    hostKey:       string | null;
    /** "ocpp@open.charging.cloud". */
    subsystem:     string;
    /** The OCPP versions spoken, "ocpp2.1" first. */
    subprotocols:  string[];
    /** The OCPP node a tool speaks to, "test02". */
    node:          string;
    /** What an account needs, "commissioning:run". */
    permission:    string;
    /** The tools connected now, by the names of their accounts. */
    tools:         string[];
    file:          string;
}

/** What a PUT to commissioning carries. */
export interface CommissioningUpdate {
    enabled:  boolean;
}


export const commissioningAPI = {

    commissioning: {
        get:   ()                              => request<CommissioningConfiguration>('GET', '/configuration/commissioning'),
        save:  (update: CommissioningUpdate)   => request<CommissioningConfiguration>('PUT', '/configuration/commissioning', update)
    }

};
