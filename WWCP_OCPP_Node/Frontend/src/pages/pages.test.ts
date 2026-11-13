/**
 * What the pages of the connections are held to: what every page of every
 * kind of node is, by the rules of WWCP_Node's test/pages.ts.
 *
 * Run with `npm test` of a kind of node that has these pages. "@node/.." is
 * WWCP_Node/Frontend, where the rules are.
 */

import { everyPageIn } from '@node/../test/pages.ts';


everyPageIn(new URL('./', import.meta.url), {
    withForms: [ 'authentication.ts', 'clientKeys.ts', 'connections.ts' ]
});
