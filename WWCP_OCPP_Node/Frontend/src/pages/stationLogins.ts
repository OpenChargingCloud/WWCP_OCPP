import { stationServerAPI as api, type AuthMethod, type ClientAuthentication, type LoginGroup, type WebSocketLogin, type WebSocketLogins } from '../api/stationServer';
import { auth } from '../auth';
import { nodeNoun, stationsResource } from '../settings';
import { must } from '@node/html';
import { toURL } from '@node/basePath';
import type { Page } from '@node/router';
import { mayButNot, reloadButton, shell } from '@node/shell';
import { errorMessage, field, formatTimestamp } from '@node/ui';
import { anyFormTypedSinceDrawn, unsaved } from '@node/unsaved';
import { html, keyed, live, nothing, render, repeat, type TemplateResult } from '@node/view';


/** How a client may authenticate, by OCPP's security profile 1, 2 and 3. */
const BY_PROFILE: ClientAuthentication[] = [ 'httpAuthWithoutTLS', 'httpAuthOverTLS', 'tlsClientCertificate' ];

/** OCPP's security profile of each way a client may authenticate. */
const PROFILE_OF: Record<ClientAuthentication, number> = { httpAuthWithoutTLS: 1, httpAuthOverTLS: 2, tlsClientCertificate: 3 };

/** NTS key establishment over SSH: a station's time over the connection it has anyway. */
const channelNTSKE = 'ntske/1@open.charging.cloud';

/** DNS over SSH: a station's names over the connection it has anyway. */
const channelDNS   = 'dns@open.charging.cloud';

/** What a channel is called where there is little room. */
function channelName(channel: string): string {
    return channel === channelNTSKE ? 'time (NTS-KE)'
         : channel === channelDNS   ? 'names (DNS)'
         :                            channel;
}

/**
 * Who may sign in to the charging station server, with what, and under which
 * group's rules.
 *
 * The page is built around one thing being true: a credential can be shown
 * exactly once. A password is kept as a hash and cannot be read back at all; a
 * TOTP shared secret has to stay readable for the controller to compute tokens
 * from, but it is still never put into the list this page reads. So both are
 * handed out at the moment they are set and never again, and the page says so
 * where somebody would otherwise go looking for them.
 *
 * Drawn by view.ts: a draw changes only what differs, so that a station half
 * typed into the form below the list outlives a group being saved, or a
 * station switched off, moved or removed - and so does its focus.
 */
export const stationLoginsPage: Page = {

    title: 'WebSocket logins',

    render({ root }) {

        const content = shell(root, {
            active:    '/configuration/ocpp-server/logins',
            title:     'WebSocket logins',
            subtitle:  'Who may sign in at the WebSocket server - a charging station, a local controller, a gateway - with what, and what their group allows them.',
            actions:   reloadButton(() => reload())
        });

        render(content, html`<div class="loading">Loading ...</div>`);

        const mayChange = auth.can(stationsResource(), 'edit');

        let cancelled = false;
        let store: WebSocketLogins | null = null;

        /** The credential that was just set, offered once and then gone. */
        let justMade: { id: string; what: string; secret: string } | null = null;

        /** The group whose settings are open, or null while none is. */
        let editing: string | null = null;


        function draw(): void {

            if (store === null)
                return;

            const logins = store;

            render(content, html`

                ${mayChange ? nothing : html`
                    <div class="notice">
                        ${mayButNot('look at the logins', 'change them')}
                    </div>
                `}

                ${justMade === null ? nothing : html`
                    <div class="notice ok">
                        <strong>The ${justMade.what} of '${justMade.id}' is</strong>
                        <code class="password">${justMade.secret}</code><br />
                        Write it down or type it into the charging station now. This is the only place it is
                        handed out - the list below never carries it.
                    </div>
                `}

                <div class="cards">
                    ${groupsCard(logins)}
                    ${loginsCard(logins)}
                </div>
            `);

        }


        // What a group allows its members, said once for a whole site.
        function groupsCard(logins: WebSocketLogins): TemplateResult {

            return html`
                <section class="card wide">

                    <h2><i class="fa-solid fa-layer-group"></i> Login groups</h2>

                    <p class="hint">
                        A group says how its members may prove who they are and how they may authenticate at the
                        server - OCPP's security profiles. It can only narrow what
                        <a href="${toURL('/configuration/ocpp-server')}">the server itself</a> allows, never widen it - and a
                        group with nothing ticked lets nobody in, which is how a whole site is stopped in one move.
                    </p>

                    <div class="table-scroll">
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Group</th>
                                    <th>Accepts</th>
                                    <th>Profiles</th>
                                    <th>Members</th>
                                    <th>On</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                ${repeat(logins.groups, group => group.id, group => groupRow(group))}
                            </tbody>
                        </table>
                    </div>

                    ${editing === null
                          ? nothing
                          : keyed(editing, groupForm(logins.groups.find(group => group.id === editing) ?? null))}

                    ${editing !== null || !mayChange ? nothing : html`
                        <div class="form-actions">
                            <button type="button" id="group-new" class="btn"
                                    ?disabled=${logins.groups.length >= logins.maxGroups} @click=${() => openGroup('')}>
                                Make a group
                            </button>
                        </div>
                    `}

                </section>
            `;

        }


        function groupRow(group: LoginGroup): TemplateResult {

            return html`
                <tr class="${group.enabled ? '' : 'dimmed'}">

                    <td>
                        <code>${group.id}</code>
                        ${group.builtIn ? html`<span class="badge">built in</span>` : nothing}
                        <div class="small muted">${group.name}${group.note ? ` - ${group.note}` : ''}</div>
                    </td>

                    <td>
                        ${group.authMethods.length === 0
                              ? html`<span class="badge warn">nothing</span>`
                              : group.authMethods.map(method => html`<span class="badge">${method}</span> `)}
                        ${(group.sshChannels ?? []).length === 0 ? nothing : html`
                            <div class="small muted">over SSH besides OCPP: ${(group.sshChannels ?? []).map(channelName).join(', ')}</div>
                        `}
                    </td>

                    <td>
                        ${group.clientAuthentication.length === 0
                              ? html`<span class="badge warn">none</span>`
                              : group.clientAuthentication.map(authentication => html`<span class="badge" title="${authentication}">${PROFILE_OF[authentication]}</span> `)}
                    </td>

                    <td>${group.members}</td>

                    <td>${group.enabled ? 'yes' : html`<span class="warn">no</span>`}</td>

                    <td class="right">
                        <button type="button" class="btn small group-edit" data-id="${group.id}"
                                ?disabled=${!mayChange} @click=${() => openGroup(group.id)}>
                            Settings
                        </button>
                        <button type="button" class="btn small danger group-remove" data-id="${group.id}"
                                ?disabled=${!mayChange || group.builtIn || group.members !== 0}
                                @click=${() => void removeGroup(group.id)}>
                            Remove
                        </button>
                    </td>

                </tr>
            `;

        }


        // Making a group and changing one are the same form; a new one simply
        // starts empty and lets the identification be typed.
        function groupForm(group: LoginGroup | null): TemplateResult {

            const ticked = (method: AuthMethod) => group?.authMethods.includes(method) ?? true;

            // A grant of its own: a new group gives neither until somebody says so.
            const given  = (channel: string) => group?.sshChannels?.includes(channel) ?? false;
            const onProfile = (profile: number) => group?.clientAuthentication.includes(BY_PROFILE[profile - 1]!) ?? true;

            return html`
                <form id="group-form" class="form-stack" data-id="${group?.id ?? ''}" @submit=${saveGroup}>

                    <h3>${group === null ? 'A new group' : `The group '${group.id}'`}</h3>

                    ${group !== null ? nothing : html`
                        <label>Identification
                            <input type="text" name="id" placeholder="field-test" maxlength="32" required
                                   pattern="[a-z0-9\\-]+" />
                            <span class="hint">Lower-case letters, digits and hyphens.</span>
                        </label>
                    `}

                    <label>Name
                        <input type="text" name="name" maxlength="60" value="${group?.name ?? ''}"
                               placeholder="Field test" />
                    </label>

                    <label>What it is
                        <input type="text" name="note" maxlength="200" value="${group?.note ?? ''}" />
                    </label>

                    <fieldset>
                        <legend>Ways in it accepts</legend>
                        <label class="check">
                            <input type="checkbox" name="basic" ?checked=${ticked('basic')} />
                            <span>A password (HTTP Basic Authentication)</span>
                        </label>
                        <label class="check">
                            <input type="checkbox" name="totp" ?checked=${ticked('totp')} />
                            <span>A one-time token (HTTP TOTP Authentication)</span>
                        </label>
                        <label class="check">
                            <input type="checkbox" name="certificate" ?checked=${ticked('certificate')} />
                            <span>A TLS client certificate</span>
                        </label>
                        <label class="check">
                            <input type="checkbox" name="sshkey" ?checked=${ticked('sshkey')} />
                            <span>An SSH key (OCPP over SSH - no security profile, so the profiles below are not asked)</span>
                        </label>
                        <label class="check">
                            <input type="checkbox" name="keypin" ?checked=${ticked('keypin')} />
                            <span>A key pin of a TLS client key (security profile 3, without a certificate authority)</span>
                        </label>
                    </fieldset>

                    <fieldset>
                        <legend>What its stations have over their SSH connection besides OCPP</legend>
                        <label class="check">
                            <input type="checkbox" name="channelNTSKE" ?checked=${given(channelNTSKE)} />
                            <span>Their time (NTS key establishment, <code>${channelNTSKE}</code>)</span>
                        </label>
                        <label class="check">
                            <input type="checkbox" name="channelDNS" ?checked=${given(channelDNS)} />
                            <span>Their names (DNS, <code>${channelDNS}</code>)</span>
                        </label>
                        <span class="hint">For stations that sign in with an SSH key, and only where this ${nodeNoun()} serves it. A key for OCPP has neither by itself.</span>
                    </fieldset>

                    <fieldset>
                        <legend>How its members may authenticate (OCPP's security profiles)</legend>
                        <label class="check">
                            <input type="checkbox" name="profile1" ?checked=${onProfile(1)} />
                            <span>1 - HTTP authentication without TLS: a password on an unencrypted port</span>
                        </label>
                        <label class="check">
                            <input type="checkbox" name="profile2" ?checked=${onProfile(2)} />
                            <span>2 - HTTP authentication over TLS: a password over TLS</span>
                        </label>
                        <label class="check">
                            <input type="checkbox" name="profile3" ?checked=${onProfile(3)} />
                            <span>3 - a TLS client certificate</span>
                        </label>
                    </fieldset>

                    <label class="switch">
                        <input type="checkbox" name="enabled" ?checked=${group?.enabled ?? true} />
                        <span>Its members may sign in</span>
                    </label>

                    <div class="form-actions">
                        <button type="submit" class="btn primary">Save the group</button>
                        <button type="button" id="group-cancel" class="btn" @click=${() => { editing = null; draw(); }}>Cancel</button>
                        <span id="group-error" class="form-error" role="alert"></span>
                    </div>

                </form>
            `;

        }


        // Who may sign in, and with what.
        function loginsCard(logins: WebSocketLogins): TemplateResult {

            return html`
                <section class="card wide">

                    <h2><i class="fa-solid fa-charging-station"></i> WebSocket logins</h2>

                    <p class="hint">
                        A charging station that is not in this list cannot sign in with a password or a token,
                        whatever it calls itself. A station that comes in on a client certificate does not need to
                        be listed - but listing it puts it into a group, which is the only way to say anything
                        about it. Kept in ${logins.file}.
                    </p>

                    ${logins.logins.length === 0
                          ? html`<p class="muted">No charging station may sign in yet.</p>`
                          : html`
                              <div class="table-scroll">
                                  <table class="table">
                                      <thead>
                                          <tr>
                                              <th>Identification</th>
                                              <th>What it is</th>
                                              <th>Group</th>
                                              <th>Signs in with</th>
                                              <th>Added</th>
                                              <th>May sign in</th>
                                              <th></th>
                                          </tr>
                                      </thead>
                                      <tbody>
                                          ${repeat(logins.logins, station => station.id, station => loginRow(station, logins))}
                                      </tbody>
                                  </table>
                              </div>
                          `}

                    <form id="station-form" class="form-row" @submit=${addStation}>

                        <label>Identification
                            <input type="text" name="id" placeholder="cs001" maxlength="48"
                                   ?disabled=${!mayChange} required />
                        </label>

                        <label>What it is
                            <input type="text" name="note" placeholder="Ladepunkt 1" maxlength="200"
                                   ?disabled=${!mayChange} />
                        </label>

                        <label>Group
                            <select name="group" ?disabled=${!mayChange}>
                                ${logins.groups.map(group => html`
                                    <option value="${group.id}"
                                            ?selected=${group.id === logins.defaultGroup}>
                                        ${group.name}
                                    </option>
                                `)}
                            </select>
                        </label>

                        <label>Password
                            <input type="text" name="password" placeholder="leave empty to make one up"
                                   minlength="${logins.minPasswordLength}" maxlength="64"
                                   ?disabled=${!mayChange} />
                        </label>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayChange}>
                                Add or change
                            </button>
                            <span id="station-error" class="form-error" role="alert"></span>
                        </div>

                    </form>

                    <h3>An SSH key, for OCPP over SSH</h3>

                    <p class="hint">
                        The station's public key, as one line of an authorized_keys file - options such as
                        <code>from="10.10.0.0/16"</code> in front where wanted. It signs in to this ${nodeNoun()}'s SSH
                        server under its identification, and gets OCPP and nothing else. A station that is not listed
                        yet is added with it. One key is one charging station.
                    </p>

                    <form id="sshkey-form" class="form-row" @submit=${addSSHKey}>

                        <label>Identification
                            <input type="text" name="id" placeholder="cs001" maxlength="48"
                                   ?disabled=${!mayChange} required />
                        </label>

                        <label>Its public key
                            <input type="text" name="line" placeholder="ssh-ed25519 AAAA... cs001"
                                   ?disabled=${!mayChange} required />
                        </label>

                        <label>Called
                            <input type="text" name="label" placeholder="bay 1" maxlength="200"
                                   ?disabled=${!mayChange} />
                        </label>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayChange}>
                                Give it the key
                            </button>
                            <span id="sshkey-error" class="form-error" role="alert"></span>
                        </div>

                    </form>

                    <h3>A key pin, for a TLS client key</h3>

                    <p class="hint">
                        SHA-256 over the station's public key, as the station shows it at commissioning: 64 hexadecimal
                        digits, with or without colons, or <code>sha256//</code> and base64. The station is let in by
                        that key alone, in whatever certificate it wraps it - only on its own path, and as nobody the
                        certificate names. A station with key pins is not let in by a certificate authority's
                        certificate in its name. A station that is not listed yet is added with it. One key is one
                        charging station.
                    </p>

                    <form id="keypin-form" class="form-row" @submit=${addKeyPin}>

                        <label>Identification
                            <input type="text" name="id" placeholder="cs001" maxlength="48"
                                   ?disabled=${!mayChange} required />
                        </label>

                        <label>Its key pin
                            <input type="text" name="pin" placeholder="64 hexadecimal digits" maxlength="200"
                                   ?disabled=${!mayChange} required />
                        </label>

                        <label>Called
                            <input type="text" name="label" placeholder="bay 1" maxlength="200"
                                   ?disabled=${!mayChange} />
                        </label>

                        <div class="form-actions">
                            <button type="submit" class="btn primary" ?disabled=${!mayChange}>
                                Let it in by the pin
                            </button>
                            <span id="keypin-error" class="form-error" role="alert"></span>
                        </div>

                    </form>

                </section>
            `;

        }


        function loginRow(station: WebSocketLogin, logins: WebSocketLogins): TemplateResult {

            const group = logins.groups.find(one => one.id === station.group);

            // What the station carries is one thing; what its group still
            // accepts is another. Showing only the first would be a page that
            // says "password" beside a station that cannot use one.
            const usable = (method: AuthMethod) => group?.enabled === true && group.authMethods.includes(method);

            return html`
                <tr class="${station.enabled && group?.enabled !== false ? '' : 'dimmed'}">

                    <td><code>${station.id}</code></td>

                    <td>${station.note ?? '-'}</td>

                    <td>
                        <select class="station-group" data-id="${station.id}" ?disabled=${!mayChange}
                                .value=${live(station.group)}
                                @change=${(event: Event) => void move(station.id, (event.target as HTMLSelectElement).value)}>
                            ${repeat(logins.groups, one => one.id, one => html`
                                <option value="${one.id}" ?selected=${one.id === station.group}>
                                    ${one.name}
                                </option>
                            `)}
                        </select>
                        ${group?.enabled === false ? html`<div class="small warn">the group is switched off</div>` : nothing}
                    </td>

                    <td>
                        ${station.hasPassword
                              ? html`<span class="badge ${usable('basic') ? '' : 'warn'}">password</span> `
                              : nothing}
                        ${station.hasTOTP
                              ? html`<span class="badge ${usable('totp') ? '' : 'warn'}">token</span> `
                              : nothing}
                        ${station.hasSSHKey
                              ? html`<span class="badge ${usable('sshkey') ? '' : 'warn'}">SSH key</span> `
                              : nothing}
                        ${station.hasKeyPin
                              ? html`<span class="badge ${usable('keypin') ? '' : 'warn'}">key pin</span> `
                              : nothing}
                        ${!station.hasPassword && !station.hasTOTP && !station.hasSSHKey && !station.hasKeyPin
                              ? html`<span class="badge">certificate only</span>`
                              : nothing}
                        ${station.totp
                              ? html`<div class="small muted">
                                         ${station.totp.length} characters of ${station.totp.hashAlgorithm},
                                         every ${station.totp.validitySeconds}s${station.totp.tlsChannelBinding
                                             ? html`, <span class="station-totp-bound">bound to its connection: over HTTP/3 only</span>`
                                             : nothing}
                                     </div>`
                              : nothing}
                        ${(station.sshKeys ?? []).map(key => html`
                              <div class="small muted station-sshkey" data-id="${station.id}" data-fingerprint="${key.fingerprint}">
                                  <code>${key.fingerprint}</code>${key.label ? ` - ${key.label}` : ''}
                                  ${key.isDisabled ? html`<span class="warn">switched off</span>` : nothing}
                                  <button type="button" class="btn small station-sshkey-remove"
                                          data-id="${station.id}" data-fingerprint="${key.fingerprint}"
                                          ?disabled=${!mayChange} @click=${() => void dropSSHKey(station.id, key.fingerprint)}>
                                      Take away
                                  </button>
                              </div>
                          `)}
                        ${station.hasSSHKey && !usable('sshkey')
                              ? html`<div class="small warn">its group does not accept SSH keys</div>`
                              : nothing}
                        ${(station.keyPins ?? []).map(keyPin => html`
                              <div class="small muted station-keypin" data-id="${station.id}" data-pin="${keyPin.pin}">
                                  <code class="keypin">${keyPin.pin}</code>${keyPin.label ? ` - ${keyPin.label}` : ''}
                                  <button type="button" class="btn small station-keypin-remove"
                                          data-id="${station.id}" data-pin="${keyPin.pin}"
                                          ?disabled=${!mayChange} @click=${() => void dropKeyPin(station.id, keyPin.pin)}>
                                      Take away
                                  </button>
                              </div>
                          `)}
                        ${station.hasKeyPin && !usable('keypin')
                              ? html`<div class="small warn">its group does not accept key pins</div>`
                              : nothing}
                        ${station.hasKeyPin && usable('keypin') && group?.clientAuthentication.includes('tlsClientCertificate') === false
                              ? html`<div class="small warn">its group does not accept security profile 3, which a client key is</div>`
                              : nothing}
                    </td>

                    <td class="small muted">${formatTimestamp(station.addedAt)}</td>

                    <td>
                        <label class="switch small">
                            <input type="checkbox" class="station-enabled" data-id="${station.id}"
                                   .checked=${live(station.enabled)}
                                   ?disabled=${!mayChange}
                                   @change=${(event: Event) => void enable(station.id, (event.target as HTMLInputElement).checked)} />
                            <span>${station.enabled ? 'yes' : 'no'}</span>
                        </label>
                    </td>

                    <td class="right">
                        <button type="button" class="btn small station-totp" data-id="${station.id}"
                                ?disabled=${!mayChange} @click=${() => void giveAToken(station.id)}>
                            ${station.hasTOTP ? 'New token secret' : 'Give it a token'}
                        </button>
                        ${station.hasTOTP ? html`
                            <button type="button" class="btn small station-totp-binding" data-id="${station.id}"
                                    title=${station.totp?.tlsChannelBinding
                                                ? 'Take a raw token again, which can be replayed while it stands.'
                                                : 'Take only a token bound to the TLS session of its connection. The station then has to come over HTTP/3, and a token read off its connection is no use on another.'}
                                    ?disabled=${!mayChange}
                                    @click=${() => void bindToken(station.id, !station.totp?.tlsChannelBinding)}>
                                ${station.totp?.tlsChannelBinding ? 'Unbind token' : 'Bind token'}
                            </button>
                            <button type="button" class="btn small station-totp-remove" data-id="${station.id}"
                                    ?disabled=${!mayChange} @click=${() => void dropToken(station.id)}>
                                Drop token
                            </button>
                        ` : nothing}
                        ${station.hasPassword ? html`
                            <button type="button" class="btn small station-password-remove" data-id="${station.id}"
                                    ?disabled=${!mayChange} @click=${() => void dropPassword(station.id)}>
                                Drop password
                            </button>
                        ` : nothing}
                        <button type="button" class="btn small danger station-remove" data-id="${station.id}"
                                ?disabled=${!mayChange} @click=${() => void removeStation(station.id)}>
                            Remove
                        </button>
                    </td>

                </tr>
            `;

        }


        /**
         * Open a group's settings - or a new group's, for ''. Drawn with
         * keyed(), the form for another group is another form, and does not
         * keep what was typed into the one open before.
         */
        function openGroup(id: string): void {
            editing = id;
            draw();
        }


        function saveGroup(event: SubmitEvent): void {
            event.preventDefault();
            void tellGroup(event.currentTarget as HTMLFormElement);
        }


        async function tellGroup(form: HTMLFormElement): Promise<void> {

            const error = content.querySelector<HTMLElement>('#group-error');

            if (error)
                error.textContent = '';

            const existing = form.dataset.id ?? '';
            const ticked   = (name: string) => form.querySelector<HTMLInputElement>(`[name="${name}"]`)?.checked === true;

            const methods: AuthMethod[] = [];

            if (ticked('basic'))        methods.push('basic');
            if (ticked('totp'))         methods.push('totp');
            if (ticked('certificate'))  methods.push('certificate');
            if (ticked('sshkey'))       methods.push('sshkey');
            if (ticked('keypin'))       methods.push('keypin');

            const channels: string[] = [];

            if (ticked('channelNTSKE'))  channels.push(channelNTSKE);
            if (ticked('channelDNS'))    channels.push(channelDNS);

            const profiles: number[] = [];

            if (ticked('profile1'))  profiles.push(1);
            if (ticked('profile2'))  profiles.push(2);
            if (ticked('profile3'))  profiles.push(3);

            try
            {

                store = await api.ocppServer.groups.save({
                    id:                existing.length > 0 ? existing : field(form, 'id'),
                    name:              field(form, 'name', false),
                    enabled:           ticked('enabled'),
                    authMethods:       methods,
                    clientAuthentication:  profiles.map(profile => BY_PROFILE[profile - 1]!),
                    sshChannels:       channels,
                    note:              field(form, 'note', false)
                });

                if (cancelled)
                    return;

                editing = null;
                draw();

            }
            catch (problem)
            {
                if (error && !cancelled)
                    error.textContent = errorMessage(problem);
            }

        }


        async function removeGroup(id: string): Promise<void> {

            if (!window.confirm(`Remove the login group '${id}'?`))
                return;

            await change(() => api.ocppServer.groups.remove(id));

        }


        function addStation(event: SubmitEvent): void {

            event.preventDefault();

            const form = event.currentTarget as HTMLFormElement;

            void tellStation(form, field(form, 'id'), field(form, 'password', false),
                             field(form, 'group'), field(form, 'note', false));

        }


        async function tellStation(form: HTMLFormElement, id: string, password: string, group: string, note: string): Promise<void> {

            const error = must<HTMLElement>(content, '#station-error');
            error.textContent = '';

            try
            {

                const answer = await api.ocppServer.logins.save(id, password, group, note);

                if (cancelled)
                    return;

                store     = answer.logins;
                justMade  = answer.password === undefined
                                ? null
                                : { id, what: 'password', secret: answer.password };

                draw();

                // A draw leaves a form as it is typed into; this station was
                // added, so the form is emptied for the next.
                form.reset();

            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }


        function addSSHKey(event: SubmitEvent): void {

            event.preventDefault();

            void tellSSHKey(event.currentTarget as HTMLFormElement);

        }


        async function tellSSHKey(form: HTMLFormElement): Promise<void> {

            const error = must<HTMLElement>(content, '#sshkey-error');
            error.textContent = '';

            try
            {

                // The group and the note are the station form's business; a key
                // is given to a station as it is.
                const answer = await api.ocppServer.logins.addSSHKey(
                                   field(form, 'id'), field(form, 'line'), field(form, 'label', false), '', '');

                if (cancelled)
                    return;

                store     = answer.logins;
                justMade  = null;

                draw();

                form.reset();

            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }


        function addKeyPin(event: SubmitEvent): void {

            event.preventDefault();

            void tellKeyPin(event.currentTarget as HTMLFormElement);

        }


        async function tellKeyPin(form: HTMLFormElement): Promise<void> {

            const error = must<HTMLElement>(content, '#keypin-error');
            error.textContent = '';

            try
            {

                // As for an SSH key: the group and the note are the station
                // form's business.
                const answer = await api.ocppServer.logins.addKeyPin(
                                   field(form, 'id'), field(form, 'pin'), field(form, 'label', false), '', '');

                if (cancelled)
                    return;

                store     = answer.logins;
                justMade  = null;

                draw();

                form.reset();

            }
            catch (problem)
            {
                if (!cancelled)
                    error.textContent = errorMessage(problem);
            }

        }


        async function dropKeyPin(id: string, pin: string): Promise<void> {

            if (!window.confirm(`Take the key pin ${pin} away from '${id}'?\n\nA connection it let in stays open; the next one is refused.`))
                return;

            await change(() => api.ocppServer.logins.removeKeyPin(id, pin));

        }


        async function dropSSHKey(id: string, fingerprint: string): Promise<void> {

            if (!window.confirm(`Take the SSH key ${fingerprint} away from '${id}'?\n\nA session it opened ends now.`))
                return;

            await change(() => api.ocppServer.logins.removeSSHKey(id, fingerprint));

        }


        async function giveAToken(id: string): Promise<void> {

            if (!window.confirm(
                    `Give '${id}' a new shared secret for one-time tokens?\n\n` +
                    'Any secret it has now stops working the moment this is saved.'))
                return;

            try
            {

                // Everything but the secret is left at what both ends already
                // agree on. A page that asked for the alphabet before it asked
                // for anything else would be a page nobody gets through.
                const answer = await api.ocppServer.logins.saveTOTP(id, {});

                if (cancelled)
                    return;

                store     = answer.logins;
                justMade  = answer.sharedSecret === undefined
                                ? null
                                : { id, what: 'TOTP shared secret', secret: answer.sharedSecret };

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                    window.alert(errorMessage(problem));
            }

        }


        async function bindToken(id: string, bound: boolean): Promise<void> {

            if (bound && !window.confirm(`Take only a one-time token bound to its connection from '${id}'?\n\n` +
                                         'The station then signs in over HTTP/3 only, and has to bind its token. ' +
                                         'One that does not is turned away.'))
                return;

            await change(() => api.ocppServer.logins.bindTOTP(id, bound));

        }

        async function dropToken(id: string): Promise<void> {

            if (!window.confirm(`Stop letting '${id}' in with a one-time token?`))
                return;

            await change(() => api.ocppServer.logins.removeTOTP(id));

        }


        async function dropPassword(id: string): Promise<void> {

            if (!window.confirm(`Stop letting '${id}' in with a password?`))
                return;

            await change(() => api.ocppServer.logins.removePassword(id));

        }


        async function removeStation(id: string): Promise<void> {

            if (!window.confirm(`Remove the charging station '${id}'? It will no longer be able to sign in.`))
                return;

            await change(() => api.ocppServer.logins.remove(id));

        }


        const enable = (id: string, wanted: boolean) =>
            change(() => api.ocppServer.logins.enable(id, wanted));

        const move = (id: string, group: string) =>
            change(() => api.ocppServer.logins.move(id, group));


        /** Every change that answers with the whole list and has nothing to hand out. */
        async function change(what: () => Promise<WebSocketLogins>): Promise<void> {

            try
            {

                const answer = await what();

                if (cancelled)
                    return;

                store     = answer;
                justMade  = null;

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                {
                    window.alert(errorMessage(problem));
                    // A switch or a chooser goes back to what the controller
                    // has, which a draw puts right.
                    void load();
                }
            }

        }


        /**
         * The logins as the local controller has them now, drawn over the page
         * as it is - what is typed into its forms kept, as a draw keeps it.
         * Reload empties them itself.
         */
        async function load(): Promise<void> {

            try
            {

                const logins = await api.ocppServer.logins.get();

                if (cancelled)
                    return;

                store = logins;

                draw();

            }
            catch (problem)
            {
                if (!cancelled)
                    render(content, html`<div class="error-box">${errorMessage(problem)}</div>`);
            }

        }


        /**
         * Loaded anew - Reload - is what the controller has, the forms too,
         * which a draw on its own would leave as typed.
         */
        async function reload(): Promise<void> {

            await load();

            if (!cancelled)
                content.querySelectorAll('form').forEach(form => form.reset());

        }


        // The group being edited, the station and the key being added are drafts. The
        // switches and choosers in the list are in no form: they take effect
        // the moment they are touched.
        const release = unsaved.heldBy(() => anyFormTypedSinceDrawn(content));

        void load();

        return () => { cancelled = true; release(); };

    }

};
