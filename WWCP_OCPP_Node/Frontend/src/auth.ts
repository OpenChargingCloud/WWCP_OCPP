import { auth as nodeAuth, type AuthState } from '@node/auth';
import type { NodeMe } from '@node/api/client';

/**
 * Who is signed in to the node these pages are drawn for: every node's
 * AuthState - the one the router's guard and the shell read too - typed with
 * no more than every kind's "me" says, and with a resource that is a name:
 * which resource the connections are is the kind's to say, see settings.ts.
 */
export const auth = nodeAuth as unknown as AuthState<NodeMe<string>, string>;
