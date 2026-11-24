import { apiURL, request } from '@node/api/client';


// The OCPP messages of a node, as OCPPServingHTTPAPI hands them out at
// "/ocpp/messages": every one it sent and received, over every connection,
// in either OCPP version - a page of them, and a stream to follow. A charging
// station, a local controller, a gateway and a CSMS serve them alike.


/** Which of the kinds of OCPP message it was - the first element of its array, by name. */
export type OCPPMessageKind = 'request' | 'response' | 'requestError' | 'responseError' | 'send';

/** One OCPP message, as it went over a connection of this node. */
export interface OCPPMessage {
    /** A number that only ever grows, so a reader can catch up with `after`. */
    id:           number;
    timestamp:    string;
    /** "in": the node at the other end sent it; "out": this node did. */
    direction:    'in' | 'out';
    kind:         OCPPMessageKind;
    /** "1.6" or "2.1". */
    ocppVersion:  string;
    /** The node at the other end, or where its connection comes from; null where neither is known. */
    peer:         string | null;
    /** The identification of the message: an answer's is its request's. */
    messageId:    string;
    /** The action - an answer's that of its request, where this node saw it; null where it did not. */
    action:       string | null;
    /** The message as it went over the wire: a JSON array. */
    message:      unknown[];
}

/** A page of OCPP messages, and what the node keeps of them. */
export interface OCPPMessagePage {
    messages:  OCPPMessage[];
    /** The newest id of the whole log, whatever this page was filtered by. */
    lastId:    number;
    count:     number;
    /** How many messages the node keeps, in memory. */
    capacity:  number;
    /** Every node at the other end of a message kept. */
    peers:     string[];
}


export const ocppLogAPI = {

    /** The newest `Limit` messages - only those after `After`, where it is given. */
    messages: (Limit: number, After?: number) =>
        request<OCPPMessagePage>('GET', `/ocpp/messages?limit=${Limit}${After === undefined ? '' : `&after=${After}`}`),

    /** Where the stream of the messages is - asked when it is opened, not when this module is loaded. */
    eventsURL: () => apiURL('/ocpp/messages/events')

};


/** Whether a message is an error: a CALLERROR, or a CALLRESULTERROR of OCPP 2.1. */
export function isError(Message: OCPPMessage): boolean {
    return Message.kind === 'requestError' || Message.kind === 'responseError';
}
