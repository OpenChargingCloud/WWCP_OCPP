/**
 * The Authentication page drawn, in a document of happy-dom, against a
 * stand-in station: credentials half written, and their focus, outlive
 * another set being saved, as a browser takes the focus away while the page
 * is held still; the kind picked shows its fields at once and keeps them
 * across a draw; a set saved says what the station took, its secret field
 * empty again; the card after one removed stays; credentials refused stay
 * typed, with why.
 *
 * Run with `npm test`.
 */

import { change, field, open, refused, submit, until, type Asked } from '@node/../test/node.ts';
import { chromeTakesTheFocus } from '@node/../test/dom.ts';

import { strict as assert }  from 'node:assert';
import { describe, it }      from 'node:test';

import type { LoginToSave, StationConnections, StationLogin } from '../api/connections.ts';

const { authenticationPage } = await import('./authentication.ts');


let held: StationConnections;

/** What the page last asked the station to write down anew. */
let written: LoginToSave | undefined;

function aLogin(id: string, more: Partial<StationLogin> = {}): StationLogin {
    return { id, description: `login ${id}`, kind: 'basic', login: `cs-${id}`, createdAt: '2026-10-01T10:00:00Z', hasSecret: true, ...more };
}

function aStation(): StationConnections {
    return {
        directory:              'connections',
        authentications:        [ aLogin('a1'), aLogin('a2') ],
        connections:            [],
        certificates:           [],
        connectionTypes:        [ 'CSMS', 'LocalController' ],
        ocppVersions:           [ 'OCPP2.1', 'OCPP1.6' ],
        maxDescriptionLength:   120,
        minSharedSecretLength:  16,
        secretsAreReadable:     false,
        totpDefaults:           { validitySeconds: 30, length: 12, alphabet: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ234567', hashAlgorithm: 'SHA256' },
        timestamp:              '2026-10-04T12:00:00Z',
        states:                 {}
    };
}

function station({ method, path, body }: Asked): unknown {

    if (path === '/configuration/authentications' && method === 'POST') {
        const entry = body as LoginToSave;
        written = entry;
        // AuthenticationEntry.TryParse
        if (entry.login.trim() === '')
            return refused(400, 'A login is needed: it is the name the other end knows this station by.');
        const made = aLogin('a3', { description: entry.description, kind: entry.kind, login: entry.login });
        held = { ...held, authentications: [ made, ...held.authentications ] };
        return { id: made.id, connections: held };
    }

    if (path === '/configuration/authentications/update') {
        const entry = body as LoginToSave;
        // A station takes what it takes: a description as it writes it down.
        held = { ...held, authentications: held.authentications.map(one => one.id === entry.id
                                                                            ? { ...one, description: entry.description.replace(/\s+/g, ' '), login: entry.login, kind: entry.kind }
                                                                            : one) };
        return held;
    }

    if (path === '/configuration/authentications/remove') {
        const { id } = body as { id: string };
        held = { ...held, authentications: held.authentications.filter(one => one.id !== id) };
        return held;
    }

    if (path === '/configuration/authentications')
        return held;

    return undefined;

}

async function opened(): Promise<HTMLElement> {
    held = aStation();
    return open(authenticationPage, '/configuration/authentication', [ 'connections:read', 'connections:edit' ],
                station, root => root.querySelector('#add-form') !== null);
}

const editOf = (root: HTMLElement, id: string) => root.querySelector<HTMLFormElement>(`form[data-edit="${id}"]`)!;
const cardOf = (root: HTMLElement, id: string) => editOf(root, id)?.closest('.card');
const totpOf = (root: HTMLElement, id: string) => root.querySelector<HTMLElement>(`[data-totp="${id}"]`)!;
const titleOf = (root: HTMLElement, id: string) => root.querySelector(`[data-secret-title="${id}"]`)?.textContent?.trim();


describe('the Authentication page', () => {

    it('keeps credentials half written, and their focus, while another set is saved', async () => {

        const root    = await opened();
        const browser = chromeTakesTheFocus(root);
        const typed   = field(root, '#add-form', 'login');

        typed.value = 'cs-ne';
        typed.focus();

        field(root, 'form[data-edit="a2"]', 'description').value = 'the local controller';
        submit(root, 'form[data-edit="a2"]');
        await until(() => root.querySelector('[data-note="a2"]')?.textContent === 'Saved.', 'the credentials were not saved');
        browser.disconnect();

        assert.ok(field(root, '#add-form', 'login') === typed,  'the field was made anew');
        assert.equal(typed.value,                               'cs-ne');
        assert.ok(document.activeElement === typed,             'the focus went');

    });

    it('shows the fields of the kind picked at once, and keeps them while another set is saved', async () => {

        const root = await opened();

        assert.equal(totpOf(root, 'add').hidden, true);

        change(field<HTMLSelectElement>(root, '#add-form', 'kind'), 'totp');
        await until(() => totpOf(root, 'add').hidden === false, 'the TOTP fields are not shown');

        assert.equal(titleOf(root, 'add'), 'Shared secret');

        submit(root, 'form[data-edit="a2"]');
        await until(() => root.querySelector('[data-note="a2"]')?.textContent === 'Saved.', 'the credentials were not saved');

        assert.equal(totpOf(root, 'add').hidden,                                 false, 'the kind picked went with a draw');
        assert.equal(field<HTMLSelectElement>(root, '#add-form', 'kind').value,  'totp');
        assert.equal(titleOf(root, 'add'),                                        'Shared secret');

    });

    it('offers a new one-time password unbound, and writes it down so', async () => {

        // Bound, it could not be sent at all: the station's TLS exports no
        // keying material to bind it with, and it is not dialled.
        const root = await opened();

        written = undefined;

        change(field<HTMLSelectElement>(root, '#add-form', 'kind'), 'totp');
        await until(() => totpOf(root, 'add').hidden === false, 'the TOTP fields are not shown');

        assert.equal(field<HTMLInputElement>(root, '#add-form', 'tlsChannelBinding').checked, false,
                     'a new one-time password is offered bound to the TLS session');

        field(root, '#add-form', 'description').value  = 'one-time';
        field(root, '#add-form', 'login').value        = 'cs-otp';
        field(root, '#add-form', 'secret').value       = 'a-shared-secret-of-sixteen';

        submit(root, '#add-form');
        await until(() => written !== undefined, 'nothing was written down');

        assert.equal(written!.tlsChannelBinding, false, 'the one-time password was written down bound');

    });

    it('shows a set as the station took it once it is saved, its secret field empty again', async () => {

        const root = await opened();
        const form = editOf(root, 'a1');

        field(root, 'form[data-edit="a1"]', 'description').value = 'the   back end';
        field(root, 'form[data-edit="a1"]', 'secret').value      = 'not shown again';

        submit(root, 'form[data-edit="a1"]');
        await until(() => root.querySelector('[data-note="a1"]')?.textContent === 'Saved.', 'the credentials were not saved');

        assert.ok(editOf(root, 'a1') === form, 'the form was made anew');
        assert.equal(field(root, 'form[data-edit="a1"]', 'description').value,         'the back end', 'the field says what was typed, not what the station took');
        assert.equal(field(root, 'form[data-edit="a1"]', 'description').defaultValue,  'the back end');
        assert.equal(field(root, 'form[data-edit="a1"]', 'secret').value,              '', 'the secret is still in the field');
        assert.equal(form.closest('details')!.open,                                     true, 'the set saved closed');

    });

    it('keeps the card of a set while the one before it is removed', async () => {

        const root   = await opened();
        const second = cardOf(root, 'a2');

        editOf(root, 'a1').querySelector<HTMLButtonElement>('[data-remove]')!.click();
        await until(() => root.querySelector('form[data-edit="a1"]') === null, 'the credentials were not removed');

        assert.ok(cardOf(root, 'a2') === second, 'the card was made anew');

    });

    it('keeps credentials the station refused typed, and says why', async () => {

        const root = await opened();

        field(root, '#add-form', 'description').value = 'without a login';

        submit(root, '#add-form');
        await until(() => root.querySelector('#add-error')?.textContent !== '', 'the refusal was not said');

        assert.equal(root.querySelector('#add-error')!.textContent, 'A login is needed: it is the name the other end knows this station by.');
        assert.equal(field(root, '#add-form', 'description').value, 'without a login', 'what was typed went');

    });

});
