/*
 * Install the node tools of this repository - "npm ci" here - once, and never
 * twice at the same time.
 *
 * Every project that compiles TypeScript or SCSS asks for this before it does,
 * and MSBuild builds those projects in parallel. Left to themselves, two of
 * them found the install due at the same moment and both ran "npm ci" in this
 * one directory. npm ci begins by deleting node_modules, so the second one
 * pulled the compiler out from under the first one's tsc, which then reported
 * dozens of
 *
 *   error TS2584: Cannot find name 'document'
 *
 * against a tsconfig that plainly lists DOM - lib.dom.d.ts was simply gone
 * for a moment. On Windows the installs also fought over files the other one
 * held open and died with EPERM (-4048). It only happened when an install was
 * due - a fresh clone, or a changed package-lock.json - which is exactly what
 * every CI runner is and what a developer's machine almost never is. Seen in
 * CSMSCLI run 35817016929 and again on a changed lock file here.
 *
 * So the install happens under a lock, and whoever gets the lock asks again
 * whether it is still due: the first project installs, the others wait for it
 * and then find nothing left to do.
 *
 * The lock is a directory, because creating one is atomic on every file system
 * this is built on and fails for everybody but the first - that is the whole
 * primitive, and MSBuild has none of its own. It lives in the temporary
 * directory rather than here, so that it can never show up in "git status":
 * the build stamps every assembly with what git says about this tree, and a
 * stray lock would make that "-dirty". It is named after this directory, so
 * two clones do not wait for each other.
 *
 * "Due" is decided by content, not by time: the stamp inside node_modules holds
 * the hash of the package-lock.json that was installed. A checkout that
 * touches the lock file without changing it therefore costs one hash and no
 * install. MSBuild still asks the cheap question first - lock file newer than
 * stamp - so a build that installs nothing does not even start Node for it.
 */

import { createHash }                                         from "node:crypto";
import { mkdirSync, readFileSync, rmSync, statSync,
         utimesSync, writeFileSync }                          from "node:fs";
import { tmpdir }                                             from "node:os";
import { dirname, join }                                      from "node:path";
import { spawnSync }                                          from "node:child_process";
import { fileURLToPath }                                      from "node:url";

const root      = dirname(fileURLToPath(import.meta.url));
const lockFile  = join(root, "package-lock.json");
const stamp     = join(root, "node_modules", ".npm-ci-stamp");

// Case-folded on Windows, where D:\X and d:\x are the same directory and must
// therefore be the same lock.
const rootKey   = process.platform === "win32" ? root.toLowerCase() : root;
const lock      = join(tmpdir(), "node-tools-" + createHash("sha256").update(rootKey).digest("hex").slice(0, 16) + ".lock");

// A lock older than this belongs to a build that was killed while it held it.
// An install takes seconds to a minute; ten minutes is not a slow one.
const staleAfterMs  = 10 * 60 * 1000;
const giveUpAfterMs = 15 * 60 * 1000;


function wanted() {
    return createHash("sha256").update(readFileSync(lockFile)).digest("hex");
}

function installed() {
    try {
        return readFileSync(stamp, "utf8").trim();
    }
    catch {
        return null;
    }
}

function sleep(ms) {
    Atomics.wait(new Int32Array(new SharedArrayBuffer(4)), 0, 0, ms);
}

function acquire() {

    const started = Date.now();

    for (;;) {

        try {
            mkdirSync(lock);
            return;
        }
        catch (e) {
            if (e.code !== "EEXIST")
                throw e;
        }

        try {
            if (Date.now() - statSync(lock).mtimeMs > staleAfterMs) {
                console.log(`restoreNodeTools: removing the stale lock ${lock}`);
                rmSync(lock, { recursive: true, force: true });
                continue;
            }
        }
        catch {
            // Released between the mkdir and the stat: try again at once.
            continue;
        }

        if (Date.now() - started > giveUpAfterMs) {
            console.error(`restoreNodeTools: gave up waiting for ${lock} - remove it if no build is running`);
            process.exit(1);
        }

        sleep(250);

    }

}

function release() {
    rmSync(lock, { recursive: true, force: true });
}


acquire();

// Ctrl+C in the middle of an install would otherwise leave the lock behind,
// and every build after it would wait ten minutes to call it stale.
for (const signal of [ "SIGINT", "SIGTERM" ])
    process.on(signal, () => { release(); process.exit(130); });

try {

    const hash = wanted();

    if (installed() === hash) {

        // Somebody else installed it while this one waited, or the lock file
        // was touched without being changed. Bring the stamp's time forward,
        // so that MSBuild's own check stops asking.
        const now = new Date();
        utimesSync(stamp, now, now);

    }

    else {

        // shell on Windows, where npm is npm.cmd and a .cmd cannot be spawned
        // without one.
        const npm = spawnSync("npm", [ "ci" ], {
                        cwd:    root,
                        stdio:  "inherit",
                        shell:  process.platform === "win32"
                    });

        if (npm.status !== 0) {
            console.error(`restoreNodeTools: "npm ci" in ${root} ended with ${npm.status ?? npm.signal ?? npm.error}`);
            process.exitCode = 1;
        }
        else
            writeFileSync(stamp, hash + "\n");

    }

}
finally {
    release();
}
