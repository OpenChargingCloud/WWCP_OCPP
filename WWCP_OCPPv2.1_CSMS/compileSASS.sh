#!/bin/bash
#
# Compile the stylesheets of this project, by hand.
#
# The build does this by itself now, so this is here for the times somebody
# wants only the CSS without waiting for a build.
#
# It used to do the work itself and needed "sass" and "jq" installed
# globally - which nobody had, on machines where these projects then failed
# to build at all, because the CSS they produce is gitignored and embedded
# as a resource. One implementation now, in ../compileSASS.mjs, with the
# compiler pinned in package.json at the repository root.

set -e
cd "$(dirname "$0")"

if [ ! -d ../node_modules ]; then
    npm --prefix .. ci
fi

node ../compileSASS.mjs
