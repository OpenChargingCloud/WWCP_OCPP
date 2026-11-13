/*
 * How Node finds "@ocpp/...".
 *
 * What the nodes that dial a CSMS share - the pages of their connections,
 * credentials and client keys - is imported as "@ocpp/...", which webpack's
 * alias and the tsconfig's paths turn into WWCP_OCPP_Node/Frontend/src. The
 * tests are run by Node, which is told so here, as WWCP_Node's
 * test/resolve.ts tells it "@node/...".
 *
 * Loaded after that one, with a second --import: hooks registered later run
 * first, so this one names the file and hands it on, and everything else goes
 * on as it came - a relative import without its extension, "@node/...", a
 * package - for WWCP_Node's to find.
 *
 * A kind of node finds it where it finds the shared files, from its own
 * Frontend directory: ../../../WWCP_OCPP/WWCP_OCPP_Node/Frontend/test/resolve.ts.
 *
 * The file is named with its extension: handed on as a URL without one, it
 * would be taken for a package by the hook it is handed to.
 */

import { registerHooks } from 'node:module';

const shared = new URL('../src/', import.meta.url);

registerHooks({
    resolve(specifier, context, next) {

        if (!specifier.startsWith('@ocpp/'))
            return next(specifier, context);

        const file = new URL(specifier.slice('@ocpp/'.length), shared).href;

        return next(/\.(ts|(m|c)?js|json)$/.test(file) ? file : `${file}.ts`, context);

    }
});
