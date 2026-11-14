/*
 * What the pages of the connections are told by the kind of node they are
 * drawn for: a charging station and a local controller keep the same
 * connections and credentials, and serve them by the same
 * routes, but each under a resource of its own, and each is something else.
 *
 * Said once, in a kind's main.ts, before the web interface starts - the pages
 * ask when they are drawn. Untold, they are a charging station's.
 *
 *   setConnectionsResource('csms');
 *   setNodeNoun('local controller');
 *   setNodeTraits({ oneNodePerOCPPVersion: false, isDialled: true });
 */

/** What a kind of node is, where the pages say something that is not true of every kind. */
export interface NodeTraits {
    /** Whether it is one OCPP node for each version of OCPP it speaks, so that a connection says which of them dials. */
    oneNodePerOCPPVersion:  boolean;
    /** Whether it is dialled itself, as a server, beside dialling its back ends. */
    isDialled:              boolean;
}

let resource  = 'connections';
let noun      = 'station';
let inFull    = 'charging station';
let traits: NodeTraits = { oneNodePerOCPPVersion: true, isDialled: false };


/** Name the resource a right to the connections is a right to: "connections" on a charging station, "csms" on a local controller. */
export function setConnectionsResource(Name: string): void {
    resource = Name;
}

/** The resource a right to the connections is a right to. */
export function connectionsResource(): string {
    return resource;
}

/**
 * Say what the node is called where a page speaks of it: "this station",
 * "this local controller" - and in full, where it is first named: "this
 * charging station". In full it is the same, unless it is said.
 */
export function setNodeNoun(Noun: string, InFull: string = Noun): void {
    noun    = Noun;
    inFull  = InFull;
}

/** What the node is called: "station". */
export function nodeNoun(): string {
    return noun;
}

/** What the node is called in full: "charging station". */
export function nodeNounInFull(): string {
    return inFull;
}

/** Say what is true of this kind of node; what is left out stays as it is. */
export function setNodeTraits(Traits: Partial<NodeTraits>): void {
    traits = { ...traits, ...Traits };
}

/** What is true of this kind of node. */
export function nodeTraits(): NodeTraits {
    return traits;
}
