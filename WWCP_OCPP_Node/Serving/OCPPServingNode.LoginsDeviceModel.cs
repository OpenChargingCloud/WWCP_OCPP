/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Globalization;

using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Logins;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The charging stations that may sign in, and their login groups, as the
    /// device model of the node's OCPP 2.1 node shows them to its CSMS:
    /// StationLoginController and StationLoginGroupController, read and written
    /// as the node's page reads and writes them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// What may read and write them is not decided here: a kind says which
    /// resource of its role model the components are - "stations" of a local
    /// controller - as it does for its other components.
    /// </para>
    /// <para>
    /// The entries of a station are put into effect one after the other, in
    /// this order: Listed false; the password; the one-time token and its
    /// binding; the SSH keys; the key pins; the group; whether it may sign in;
    /// Listed true. Each through what the page changes it with, so each is
    /// written to the file by itself: one that is refused keeps what was set
    /// before it, whatever the request's data consistency model says. A login
    /// group is one change of the file. The groups of a request come before
    /// its stations, so that a new station can be put into a new group.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region Data

        private          StationLoginsSource?  stationLoginsSource;
        private readonly Object                loginComponentsLock  = new ();

        #endregion


        #region AttachStationLoginControllers(Node)

        /// <summary>
        /// Give the given OCPP 2.1 node a StationLoginController for every
        /// charging station that may sign in and a StationLoginGroupController
        /// for every login group, for its CSMS to read and to write - and the
        /// instances a request makes of both, and keep them following the list.
        /// </summary>
        /// <param name="Node">The OCPP 2.1 node of this node.</param>
        public void AttachStationLoginControllers(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)
        {

            if (Node is not OCPPv2_1.IDeviceModelNode deviceModel)
                return;

            stationLoginsSource ??= new StationLoginsSource(this);

            deviceModel.AddNewInstancesSource(stationLoginsSource);

            FollowStationLogins(deviceModel);

            // The page, the command line and OCPP itself change the list: the
            // instances follow every change, before the next read.
            WebSocketLogins.OnChanged += () => FollowStationLogins(deviceModel);

        }

        #endregion

        #region (private) FollowStationLogins(Node)

        /// <summary>
        /// Give the given node an instance of each component for every station
        /// and group there is now, and none for one there is not.
        /// </summary>
        private void FollowStationLogins(OCPPv2_1.IDeviceModelNode Node)
        {

            if (stationLoginsSource is not StationLoginsSource source)
                return;

            try
            {

                lock (loginComponentsLock)
                {

                    var stations  = WebSocketLogins.Logins.Select(login => login.Id).ToHashSet(StringComparer.Ordinal);
                    var groups    = WebSocketLogins.Groups.Select(group => group.Id).ToHashSet(StringComparer.Ordinal);

                    var shownStations = Node.GetComponentConfigs(nameof(OCPPv2_1.StationLoginController)).     OfType<OCPPv2_1.StationLoginController>().     ToArray();
                    var shownGroups   = Node.GetComponentConfigs(nameof(OCPPv2_1.StationLoginGroupController)).OfType<OCPPv2_1.StationLoginGroupController>().ToArray();

                    foreach (var shown in shownStations.Where(shown => !stations.Contains(shown.Station)))
                        Node.RemoveComponent(shown.Name, shown.Instance);

                    foreach (var shown in shownGroups.Where(shown => !groups.Contains(shown.Group)))
                        Node.RemoveComponent(shown.Name, shown.Instance);

                    foreach (var station in stations.Where(station => !shownStations.Any(shown => shown.Station == station)))
                        Node.AddComponent(new OCPPv2_1.StationLoginController(source, station));

                    foreach (var group in groups.Where(group => !shownGroups.Any(shown => shown.Group == group)))
                        Node.AddComponent(new OCPPv2_1.StationLoginGroupController(source, group));

                }

            }
            catch (Exception e)
            {
                Log.Warning($"The device model could not follow the charging stations that may sign in: {e.Message}", "ocpp", "station", "auth");
            }

        }

        #endregion


        #region (private class) StationLoginsSource

        /// <summary>
        /// What StationLoginController and StationLoginGroupController read and
        /// write of a node: its list of charging stations and login groups,
        /// through what the node's page changes them with.
        /// </summary>
        /// <param name="Node">The node.</param>
        private sealed class StationLoginsSource(OCPPServingNode Node) : OCPPv2_1.IStationLoginsSource,
                                                                         OCPPv2_1.ISetVariablesSource,
                                                                         OCPPv2_1.INewInstancesSource
        {

            private WebSocketLogins Logins
                => Node.WebSocketLogins;

            #region IStationLoginsSource

            public IEnumerable<String> Stations
                => [.. Logins.Logins.Select(login => login.Id)];

            public IEnumerable<String> Groups
                => [.. Logins.Groups.Select(group => group.Id)];

            public Boolean Writes
                => true;

            public String? StationValue(String Station, String Variable)
            {

                if (!Logins.TryGet(Station, out var login))
                    return null;

                return Variable switch {
                           "Group"               => login.GroupId,
                           "Enabled"             => Bool(login.Enabled),
                           "HasPassword"         => Bool(login.HasPassword),
                           "TOTPValidity"        => login.TOTP is TOTPSettings totp1 ? ((Int64) totp1.ValidityTime.TotalSeconds).ToString(CultureInfo.InvariantCulture) : "",
                           "TOTPLength"          => login.TOTP?.Length.ToString(CultureInfo.InvariantCulture) ?? "",
                           "TOTPAlphabet"        => login.TOTP?.Alphabet ?? "",
                           "TOTPHashAlgorithm"   => login.TOTP?.HashAlgorithm.ToString() ?? "",
                           "TOTPChannelBinding"  => Bool(login.TOTP?.TLSChannelBinding == true),
                           "HasTOTP"             => Bool(login.HasTOTP),
                           "SSHKeys"             => String.Join(",", login.Keys.Select(key => key.Fingerprint)),
                           "KeyPins"             => String.Join(",", login.Pins.Select(pin => pin.Pin.ToHex())),
                           "Note"                => login.Note ?? "",
                           "AddedAt"             => login.AddedAt.ToUniversalTime().ToString("o"),
                           "Listed"              => "true",
                           _                     => null
                       };

            }

            public String? GroupValue(String Group, String Variable)
            {

                if (Logins.GetGroup(Group) is not WebSocketLoginGroup group)
                    return null;

                return Variable switch {
                           "Name"              => group.Name,
                           "Enabled"           => Bool(group.Enabled),
                           "AuthMethods"       => String.Join(",", group.AuthMethods.Order().Select(method => method.AsText())),
                           // OCPP's words for how its members may authenticate.
                           "SecurityProfiles"  => String.Join(",", ClientAuthenticationProfiles.SecurityProfiles(group.ClientAuthentication)),
                           "SSHChannels"       => String.Join(",", (group.SSHChannels ?? new HashSet<String>()).Order(StringComparer.Ordinal)),
                           "Note"              => group.Note ?? "",
                           "Members"           => Logins.Logins.Count(login => login.GroupId == group.Id).ToString(CultureInfo.InvariantCulture),
                           "AddedAt"           => group.AddedAt.ToUniversalTime().ToString("o"),
                           "Listed"            => "true",
                           _                   => null
                       };

            }

            private static String Bool(Boolean Value)
                => Value ? "true" : "false";

            #endregion

            #region INewInstancesSource

            public IEnumerable<String> NewInstanceComponents
                => [ nameof(OCPPv2_1.StationLoginController), nameof(OCPPv2_1.StationLoginGroupController) ];

            /// <summary>
            /// A station the request gives a first way in, or a group the request
            /// sets AuthMethods and SecurityProfiles of - and nothing else.
            /// </summary>
            public OCPPv2_1.ComponentConfig? NewInstance(OCPPv2_1.Component            Component,
                                                         OCPPv2_1.CSMS.SetVariablesRequest  Request)
            {

                if (Component.Instance is not String instance)
                    return null;

                var named = Request.VariableData.Where(entry => String.Equals(entry.Component.Name, Component.Name, StringComparison.OrdinalIgnoreCase) &&
                                                                entry.Component.Instance == instance).
                                                 ToArray();

                if (String.Equals(Component.Name, nameof(OCPPv2_1.StationLoginController), StringComparison.OrdinalIgnoreCase))
                    return !Logins.TryGet(instance, out _) &&
                           named.Any(entry => OCPPv2_1.StationLoginController.FirstWaysIn.Contains(entry.Variable.Name, StringComparer.OrdinalIgnoreCase) &&
                                              !String.IsNullOrWhiteSpace(entry.AttributeValue))
                               ? OCPPv2_1.StationLoginController.NewInstance(this, instance)
                               : null;

                if (String.Equals(Component.Name, nameof(OCPPv2_1.StationLoginGroupController), StringComparison.OrdinalIgnoreCase))
                    return Logins.GetGroup(instance) is null &&
                           OCPPv2_1.StationLoginGroupController.MadeWith.All(variable => named.Any(entry => String.Equals(entry.Variable.Name, variable, StringComparison.OrdinalIgnoreCase)))
                               ? OCPPv2_1.StationLoginGroupController.NewInstance(this, instance)
                               : null;

                return null;

            }

            #endregion

            #region ISetVariablesSource

            public async Task<IReadOnlyList<OCPPv2_1.SetVariableResult>> SetVariablesAsync(IReadOnlyList<OCPPv2_1.SetVariableEntry>  Entries,
                                                                                           OCPPv2_1.DataConsistencyModel             DataConsistencyModel,
                                                                                           CancellationToken                         CancellationToken)
            {

                var results = new OCPPv2_1.SetVariableResult[Entries.Count];

                OCPPv2_1.SetVariableResult Result(Int32 Index, OCPPv2_1.SetVariableStatus Status, String? ReasonCode = null, String? AdditionalInfo = null)

                    => results[Index] = new (
                                            AttributeStatus:      Status,
                                            Component:            Entries[Index].Data.Component,
                                            Variable:             Entries[Index].Data.Variable,
                                            AttributeType:        Entries[Index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual,
                                            AttributeStatusInfo:  ReasonCode is not null ? new OCPPv2_1.StatusInfo(ReasonCode, AdditionalInfo) : null
                                        );

                void Done(Int32 Index, Boolean Taken, String? Error, Boolean NotSaved)
                {
                    if (Taken)
                        Result(Index, OCPPv2_1.SetVariableStatus.Accepted);
                    else
                        Result(Index, OCPPv2_1.SetVariableStatus.Rejected, NotSaved ? "InternalError" : "InvalidValue", Error);
                }

                // The groups first, so that a station can go into a new one.
                foreach (var group in Enumerable.Range(0, Entries.Count).
                                                 Where  (index => Entries[index].Component is OCPPv2_1.StationLoginGroupController).
                                                 GroupBy(index => ((OCPPv2_1.StationLoginGroupController) Entries[index].Component).Group))
                {
                    SetGroup(group.Key, [.. group]);
                }

                foreach (var station in Enumerable.Range(0, Entries.Count).
                                                   Where  (index => Entries[index].Component is OCPPv2_1.StationLoginController).
                                                   GroupBy(index => ((OCPPv2_1.StationLoginController) Entries[index].Component).Station))
                {
                    SetStation(station.Key, [.. station]);
                }

                // Anything else is not of these components: said so, should it
                // come all the same.
                for (var index = 0; index < Entries.Count; index++)
                    results[index] ??= new (OCPPv2_1.SetVariableStatus.UnknownComponent, Entries[index].Data.Component, Entries[index].Data.Variable,
                                            Entries[index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual,
                                            new OCPPv2_1.StatusInfo("NotFound", "Not a variable of the charging stations that may sign in."));

                await Task.CompletedTask;

                return results;


                #region (local) Taken(Indices)

                // The entries that pass their own check, by their variable's
                // name; those that do not are answered here.
                Dictionary<String, (Int32 Index, String Value)> Taken(IEnumerable<Int32> Indices, Func<String, String, (Boolean, OCPPv2_1.StatusInfo?)> Check)
                {

                    var taken = new Dictionary<String, (Int32, String)>(StringComparer.OrdinalIgnoreCase);

                    foreach (var index in Indices)
                    {

                        var (ok, refusal) = Check(Entries[index].Variable.Name, Entries[index].Data.AttributeValue ?? "");

                        if (!ok)
                            results[index] = new (OCPPv2_1.SetVariableStatus.Rejected, Entries[index].Data.Component, Entries[index].Data.Variable,
                                                  Entries[index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual, refusal);

                        else if (taken.ContainsKey(Entries[index].Variable.Name))
                            Result(index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", $"{Entries[index].Variable.Name} is set twice in one request.");

                        else
                            taken[Entries[index].Variable.Name] = (index, Entries[index].Data.AttributeValue ?? "");

                    }

                    return taken;

                }

                #endregion

                #region (local) SetGroup(Group, Indices)

                void SetGroup(String Group, IReadOnlyList<Int32> Indices)
                {

                    var taken = Taken(Indices, (variable, value) => (OCPPv2_1.StationLoginGroupController.TryCheck(Group, variable, value, out var refusal), refusal));

                    if (taken.Count == 0)
                        return;

                    #region Listed false: taken away, and nothing else of it set

                    if (taken.TryGetValue("Listed", out var listed) && !Boolean.Parse(listed.Value))
                    {

                        var removed = Logins.TryRemoveGroup(Group, out var removeError, out var removeNotSaved);

                        Done(listed.Index, removed, removeError, removeNotSaved);

                        foreach (var other in taken.Where(other => other.Key != "Listed"))
                            Result(other.Value.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue",
                                   $"The login group '{Group}' is taken away by the same request.");

                        return;

                    }

                    #endregion

                    var existing  = Logins.GetGroup(Group);
                    var changes   = taken.Where(change => change.Key != "Listed").ToArray();

                    if (changes.Length > 0)
                    {

                        var authMethods = taken.TryGetValue("AuthMethods", out var auth)
                                              ? Members(auth.Value).Select(member => AuthMethodExtensions.TryParseAuthMethod(member)!.Value).ToArray()
                                              : existing?.AuthMethods.ToArray();

                        var profiles    = taken.TryGetValue("SecurityProfiles", out var profile)
                                              ? Members(profile.Value).Select(member => ClientAuthenticationProfiles.OfProfile(Byte.Parse(member, CultureInfo.InvariantCulture))).ToArray()
                                              : existing?.ClientAuthentication.ToArray();

                        var channels    = taken.TryGetValue("SSHChannels", out var channel)
                                              ? Members(channel.Value)
                                              : null;

                        var updated     = Logins.TryAddOrUpdateGroup(Group,
                                                                     taken.TryGetValue("Name",    out var name)    ? name.Value                  : existing?.Name ?? Group,
                                                                     taken.TryGetValue("Enabled", out var enabled) ? Boolean.Parse(enabled.Value) : existing?.Enabled ?? true,
                                                                     authMethods,
                                                                     profiles,
                                                                     taken.TryGetValue("Note",    out var note)    ? (note.Value.Length > 0 ? note.Value : null) : existing?.Note,
                                                                     channels,
                                                                     out var error,
                                                                     out var notSaved);

                        foreach (var change in changes)
                            Done(change.Value.Index, updated, error, notSaved);

                        if (updated)
                            Node.Log.Notice($"Over OCPP, the login group '{Group}' was {(existing is null ? "made" : "changed")}: {String.Join(", ", changes.Select(change => change.Key))}.",
                                            "ocpp", "station", "auth");

                    }

                    if (taken.TryGetValue("Listed", out var stays))
                        Result(stays.Index, Logins.GetGroup(Group) is not null
                                                ? OCPPv2_1.SetVariableStatus.Accepted
                                                : OCPPv2_1.SetVariableStatus.Rejected,
                                            Logins.GetGroup(Group) is not null ? null : "InvalidValue",
                                            Logins.GetGroup(Group) is not null ? null : $"There is no login group '{Group}' to keep.");

                }

                #endregion

                #region (local) SetStation(Station, Indices)

                void SetStation(String Station, IReadOnlyList<Int32> Indices)
                {

                    var taken = Taken(Indices, (variable, value) => (OCPPv2_1.StationLoginController.TryCheck(Station, variable, value, out var refusal), refusal));

                    if (taken.Count == 0)
                        return;

                    var said = new List<String>();

                    #region Listed false: taken off the list, and nothing else of it set

                    if (taken.TryGetValue("Listed", out var listed) && !Boolean.Parse(listed.Value))
                    {

                        var removed = Logins.TryRemove(Station, out var removeError, out var removeNotSaved);

                        Done(listed.Index, removed, removeError, removeNotSaved);

                        foreach (var other in taken.Where(other => other.Key != "Listed"))
                            Result(other.Value.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue",
                                   $"The charging station '{Station}' is taken off the list by the same request.");

                        if (removed)
                            Node.Log.Notice($"Over OCPP, the charging station '{Station}' was taken off the list.", "ocpp", "station", "auth");

                        return;

                    }

                    #endregion

                    // A station not listed yet goes into the group the request
                    // names with the first way in that lists it.
                    var group = taken.TryGetValue("Group", out var groupEntry) ? groupEntry.Value : null;

                    String? GroupWhileNew()
                        => Logins.TryGet(Station, out _) ? null : group;

                    #region The password

                    if (taken.TryGetValue("Password", out var password))
                    {

                        String? error; Boolean notSaved; Boolean done;

                        if (password.Value.Length == 0)
                            done = Logins.TryClearPassword(Station, out error, out notSaved);
                        else
                            done = Logins.TrySetPassword(Station, password.Value, GroupWhileNew(), null, out error, out notSaved);

                        Done(password.Index, done, error, notSaved);
                        if (done) said.Add(password.Value.Length == 0 ? "its password taken away" : "a password");

                    }

                    #endregion

                    #region The one-time token: its secret with its parameters, and its binding

                    var parameters = OCPPv2_1.StationLoginController.TOTPParameters.Where(taken.ContainsKey).ToArray();

                    if (taken.TryGetValue("TOTPSecret", out var secret) && secret.Value.Length > 0)
                    {

                        TOTPHashAlgorithm? hash = taken.TryGetValue("TOTPHashAlgorithm", out var algorithm)
                                                      ? Enum.Parse<TOTPHashAlgorithm>(algorithm.Value, ignoreCase: true)
                                                      : null;

                        var done = Logins.TrySetTOTP(Station,
                                                     secret.Value,
                                                     taken.TryGetValue("TOTPValidity", out var validity) ? TimeSpan.FromSeconds(Int64.Parse(validity.Value, CultureInfo.InvariantCulture)) : null,
                                                     taken.TryGetValue("TOTPLength",   out var length)   ? UInt32.Parse(length.Value, CultureInfo.InvariantCulture)                     : null,
                                                     taken.TryGetValue("TOTPAlphabet", out var alphabet) ? alphabet.Value                                                                : null,
                                                     hash,
                                                     GroupWhileNew(),
                                                     null,
                                                     out _,
                                                     out var error,
                                                     out var notSaved);

                        Done(secret.Index, done, error, notSaved);

                        foreach (var parameter in parameters)
                            Done(taken[parameter].Index, done, error, notSaved);

                        if (done) said.Add("a one-time token");

                    }

                    else
                    {

                        if (taken.TryGetValue("TOTPSecret", out var cleared))
                        {
                            var done = Logins.TryClearTOTP(Station, out var error, out var notSaved);
                            Done(cleared.Index, done, error, notSaved);
                            if (done) said.Add("its one-time token taken away");
                        }

                        // Set without a secret, a parameter would have a secret
                        // made up here that no answer can carry back.
                        foreach (var parameter in parameters)
                            Result(taken[parameter].Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue",
                                   $"{parameter} is set together with a new TOTPSecret only: the token is made anew, and its secret has to come with it.");

                    }

                    if (taken.TryGetValue("TOTPChannelBinding", out var binding))
                    {
                        var done = Logins.TrySetTOTPBinding(Station, Boolean.Parse(binding.Value), out var error, out var notSaved);
                        Done(binding.Index, done, error, notSaved);
                        if (done) said.Add($"its one-time token {(Boolean.Parse(binding.Value) ? "bound" : "not bound")} to its connection");
                    }

                    #endregion

                    #region The SSH keys

                    if (taken.TryGetValue("SSHKeyAdd", out var keyToAdd))
                    {
                        var done = Node.TryAddStationSSHKey(Station, keyToAdd.Value, null, GroupWhileNew(), null, "OCPP", out var key, out var error, out var notSaved);
                        Done(keyToAdd.Index, done, error, notSaved);
                        if (done) said.Add($"the SSH key {key!.Fingerprint}");
                    }

                    if (taken.TryGetValue("SSHKeyRemove", out var keyToRemove))
                    {
                        var done = Logins.TryRemoveSSHKey(Station, keyToRemove.Value.Trim(), out var error, out var notSaved);
                        Done(keyToRemove.Index, done, error, notSaved);
                        if (done) said.Add($"the SSH key {keyToRemove.Value.Trim()} taken away");
                    }

                    #endregion

                    #region The key pins: the whole list

                    if (taken.TryGetValue("KeyPins", out var pins))
                    {

                        var wanted   = new List<KeyPin>();
                        String? why  = null;

                        foreach (var member in Members(pins.Value))
                        {
                            if (KeyPin.TryParse(member, out var pin))
                            {
                                if (!wanted.Contains(pin))
                                    wanted.Add(pin);
                            }
                            else
                                why = $"\"{member}\" is no key pin - 64 hexadecimal digits, with or without colons between the bytes.";
                        }

                        if (why is not null)
                            Result(pins.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", why);

                        else
                        {

                            var held     = Logins.TryGet(Station, out var login) ? login.Pins.Select(pin => pin.Pin).ToArray() : [];
                            var done     = true;
                            String? error = null;
                            var notSaved = false;

                            // The new ones first: a station whose last pin is
                            // replaced keeps a way in throughout.
                            foreach (var pin in wanted.Where(pin => !held.Contains(pin)))
                                if (done)
                                    done = Logins.TryAddKeyPin(Station, pin.ToHex(), null, GroupWhileNew(), null, "OCPP", out _, out error, out notSaved);

                            foreach (var pin in held.Where(pin => !wanted.Contains(pin)))
                                if (done)
                                    done = Logins.TryRemoveKeyPin(Station, pin.ToHex(), out error, out notSaved);

                            Done(pins.Index, done, error, notSaved);
                            if (done) said.Add("its key pins");

                        }

                    }

                    #endregion

                    #region The group, whether it may sign in, and that it stays listed

                    if (groupEntry.Value is not null && results[groupEntry.Index] is null)
                    {

                        var login = Logins.TryGet(Station, out var listedLogin) ? listedLogin : null;

                        // Put into it already when it was listed by this request.
                        if (login is not null && String.Equals(login.GroupId, group, StringComparison.OrdinalIgnoreCase))
                            Result(groupEntry.Index, OCPPv2_1.SetVariableStatus.Accepted);

                        else
                        {
                            var done = Logins.TrySetGroup(Station, group, out var error, out var notSaved);
                            Done(groupEntry.Index, done, error, notSaved);
                            if (done) said.Add($"the group '{group}'");
                        }

                    }

                    if (taken.TryGetValue("Enabled", out var enabled))
                    {
                        var done = Logins.TrySetEnabled(Station, Boolean.Parse(enabled.Value), out var error, out var notSaved);
                        Done(enabled.Index, done, error, notSaved);
                        if (done) said.Add(Boolean.Parse(enabled.Value) ? "let in" : "shut out");
                    }

                    if (taken.TryGetValue("Listed", out var stays))
                    {
                        var isListed = Logins.TryGet(Station, out _);
                        if (isListed)
                            Result(stays.Index, OCPPv2_1.SetVariableStatus.Accepted);
                        else
                            Result(stays.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", $"The charging station '{Station}' is not listed: it is listed by a first way in.");
                    }

                    #endregion

                    if (said.Count > 0)
                        Node.Log.Notice($"Over OCPP, the charging station '{Station}' was given: {String.Join("; ", said)}.", "ocpp", "station", "auth");

                }

                #endregion

            }

            #endregion


            #region (private static) Members(Value)

            private static String[] Members(String Value)

                => Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            #endregion

        }

        #endregion

    }

}
