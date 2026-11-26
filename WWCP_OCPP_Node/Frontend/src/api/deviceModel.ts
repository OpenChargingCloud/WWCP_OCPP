import { request } from '@node/api/client';


// The Device Model of a node, as OCPPServingHTTPAPI hands it out at
// "/ocpp/deviceModel": its components of OCPP 2.1, each with its variables
// and what they say now, and its configuration keys of OCPP 1.6 - each
// version only where the node has an OCPP node of it.


/** A variable of a component of OCPP 2.1, as it is now. */
export interface DeviceModelVariable {
    /** What it says now; not there where it is hidden. */
    value?:            string | null;
    /** A value OCPP keeps WriteOnly, left out for all but a systemadmin. */
    hidden?:           true;
    /** What its getter said where it threw. */
    error?:            string;
    type:              string;
    mutability:        'ReadOnly' | 'ReadWrite' | 'WriteOnly' | string;
    persistent?:       true;
    constant?:         true;
    secret?:           true;
    characteristics?:  Record<string, unknown>;
    description?:      string;
}

/** A configuration key of OCPP 1.6, as it is now. */
export interface DeviceModelKey {
    value?:        string | null;
    hidden?:       true;
    accessRights:  string;
    readonly:      boolean;
    valueType:     string;
    unit:          string;
}

/** The Device Model of a node. */
export interface DeviceModel {
    /** Components by the name OCPP gives them - "Name[instance] @ EVSE 1/2" -, each with its variables by name. */
    'ocpp2.1'?:  Record<string, Record<string, DeviceModelVariable | string>>;
    /** Configuration keys by name. */
    'ocpp1.6'?:  Record<string, DeviceModelKey>;
}

/** What the node answers. */
export interface DeviceModelPage {
    /** Whether the values OCPP keeps WriteOnly are in it: for a systemadmin. */
    writeOnlyShown:  boolean;
    deviceModel:     DeviceModel;
}


export const deviceModelAPI = {

    /** The Device Model of this node, as it is now. */
    get: () => request<DeviceModelPage>('GET', '/ocpp/deviceModel')

};
