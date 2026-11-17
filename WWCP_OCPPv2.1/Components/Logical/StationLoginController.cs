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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where StationLoginController and StationLoginGroupController read the
    /// charging stations that may sign in to a node's OCPP server, and the
    /// login groups that decide what they may sign in with.
    /// </summary>
    /// <remarks>
    /// The controllers keep no value of their own: whatever holds the logins
    /// answers each read, so that a station added on a page is what the next
    /// read sees. A source that is an <see cref="ISetVariablesSource"/> as well
    /// takes the writes of a SetVariables request, all of the request's at once.
    /// </remarks>
    public interface IStationLoginsSource
    {

        /// <summary>
        /// The charging stations that may sign in: the instances of
        /// StationLoginController.
        /// </summary>
        IEnumerable<String>  Stations                  { get; }

        /// <summary>
        /// The login groups: the instances of StationLoginGroupController.
        /// </summary>
        IEnumerable<String>  Groups                    { get; }

        /// <summary>
        /// Whether this source writes what a CSMS may write.
        /// </summary>
        Boolean              Writes
            => false;

        /// <summary>
        /// The value of a variable of a charging station; null where there is
        /// no such station, and for a variable that is only written.
        /// </summary>
        /// <param name="Station">The charging station: an instance of StationLoginController.</param>
        /// <param name="Variable">The variable, as the controller spells it.</param>
        String? StationValue(String Station, String Variable);

        /// <summary>
        /// The value of a variable of a login group; null where there is no
        /// such group.
        /// </summary>
        /// <param name="Group">The login group: an instance of StationLoginGroupController.</param>
        /// <param name="Variable">The variable, as the controller spells it.</param>
        String? GroupValue(String Group, String Variable);

    }


    /// <summary>
    /// One charging station that may sign in to the OCPP server of a node - a
    /// local controller's station port - and what it signs in with: an instance
    /// for every station, named by what the station calls itself in OCPP.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OCPP 2.1 has no component for this: a charging station signs in, and
    /// keeps no list of others that do. So this is a controller of its own, as
    /// the node's page lists the stations: its group, whether it may sign in,
    /// and each way it may - a password, a one-time token (TOTP), SSH keys, the
    /// key pins of its TLS client keys.
    /// </para>
    /// <para>
    /// The secrets are written and never read out: Password and TOTPSecret are
    /// write-only, and "" takes them away. HasPassword and HasTOTP say whether
    /// there is one. A CSMS brings each secret itself - the answer to a
    /// SetVariables request carries no value, so nothing is made up here as the
    /// page makes one up. The parameters of the token are set together with a
    /// new secret only; whether it is bound to the TLS session of its connection
    /// by itself.
    /// </para>
    /// <para>
    /// An SSH key is added by its authorized_keys line (SSHKeyAdd) and taken
    /// away by its fingerprint (SSHKeyRemove); SSHKeys lists the fingerprints.
    /// KeyPins is the whole list: what it no longer names is taken away.
    /// </para>
    /// <para>
    /// A station that is not listed yet is listed by a request that names it and
    /// gives it a first way in - a Password, a TOTPSecret, an SSHKeyAdd or
    /// KeyPins - and its Group and Enabled in the same request as well. Listed
    /// set to false takes it off the list.
    /// </para>
    /// </remarks>
    public class StationLoginController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of a charging station, in the order of the node's page.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("Group",               DataTypes.String,      "The login group that decides what the station may sign in with.",                                                                MaxLimit: 32,                          Writable: true),
            new ("Enabled",             DataTypes.Boolean,     "Whether the station may sign in at the moment.",                                                                                                                         Writable: true),

            new ("Password",            DataTypes.String,      "Its password for HTTP Basic authentication, at least 16 characters; empty takes it away. Written, never read.",                   MaxLimit: 128,                         Writable: true, MayBeEmpty: true, WriteOnly: true),
            new ("HasPassword",         DataTypes.Boolean,     "Whether it has a password."),

            new ("TOTPSecret",          DataTypes.String,      "The shared secret of its one-time tokens (TOTP); empty takes them away. Written, never read.",                                    MaxLimit: 128,                         Writable: true, MayBeEmpty: true, WriteOnly: true),
            new ("TOTPValidity",        DataTypes.Integer,     "How long one token stands. Set together with a new TOTPSecret only.",                                                            MinLimit: 10, MaxLimit: 300, InSeconds: true, Writable: true),
            new ("TOTPLength",          DataTypes.Integer,     "How many characters a token has. Set together with a new TOTPSecret only.",                                                      MinLimit: 6,  MaxLimit: 128,           Writable: true),
            new ("TOTPAlphabet",        DataTypes.String,      "Which characters a token is made of. Set together with a new TOTPSecret only.",                                                  MaxLimit: 256,                         Writable: true),
            new ("TOTPHashAlgorithm",   DataTypes.OptionList,  "The HMAC a token is derived with. Set together with a new TOTPSecret only.",                                                     ValuesList: [ "SHA256", "SHA384", "SHA512" ], Writable: true),
            new ("TOTPChannelBinding",  DataTypes.Boolean,     "Whether a token has to be bound to the TLS session of its connection.",                                                                                                  Writable: true),
            new ("HasTOTP",             DataTypes.Boolean,     "Whether it may sign in with a one-time token."),

            new ("SSHKeys",             DataTypes.String,      "The SHA-256 fingerprints of its SSH keys for OCPP over SSH, comma separated."),
            new ("SSHKeyAdd",           DataTypes.String,      "An SSH public key to add, as one line of an authorized_keys file. Written, never read.",                                         MaxLimit: 8000,                        Writable: true, WriteOnly: true),
            new ("SSHKeyRemove",        DataTypes.String,      "The SHA-256 fingerprint of an SSH key to take away, \"SHA256:...\" as OpenSSH writes it. Written, never read.",                  MaxLimit: 100,                         Writable: true, WriteOnly: true),

            new ("KeyPins",             DataTypes.String,      "The key pins of the TLS client keys it signs in with, comma separated - SHA-256 over the SubjectPublicKeyInfo, 64 hexadecimal digits. What the list no longer names is taken away.", MaxItems: 4, Writable: true, MayBeEmpty: true),

            new ("Note",                DataTypes.String,      "What the station is, for whoever reads the list later."),
            new ("AddedAt",             DataTypes.DateTime,    "When it was listed."),
            new ("Listed",              DataTypes.Boolean,     "Whether it is listed; false takes it off the list, with all it signs in with.",                                                                                          Writable: true)

        ];

        /// <summary>
        /// The variables that give a station a first way in: one of them, set,
        /// lists a station that is not listed yet.
        /// </summary>
        public static readonly IReadOnlyList<String> FirstWaysIn = [ "Password", "TOTPSecret", "SSHKeyAdd", "KeyPins" ];

        /// <summary>
        /// The parameters of the one-time token, set together with a new
        /// TOTPSecret only.
        /// </summary>
        public static readonly IReadOnlyList<String> TOTPParameters = [ "TOTPValidity", "TOTPLength", "TOTPAlphabet", "TOTPHashAlgorithm" ];

        #endregion

        #region Properties

        /// <summary>
        /// The charging station: the instance.
        /// </summary>
        public String   Station    { get; }

        /// <summary>
        /// Whether this stands for a station a SetVariables request lists, which
        /// the source does not have yet - see <see cref="NewInstance"/>.
        /// </summary>
        public Boolean  IsNew      { get; }

        /// <summary>
        /// The names of the variables of a station.
        /// </summary>
        public static IEnumerable<String> VariableNames
            => Variables.Select(variable => variable.Name);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The controller of the given charging station of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Station">The charging station; the instance.</param>
        public StationLoginController(IStationLoginsSource  Source,
                                      String                Station)

            : this(Source, Station, false)

        { }

        private StationLoginController(IStationLoginsSource  Source,
                                       String                Station,
                                       Boolean               IsNew)

            : base(nameof(StationLoginController),
                   Station,
                   I18NString.Create($"The charging station '{Station}', and what it may sign in with."),
                   null)

        {

            if (!IsNew && !Source.Stations.Contains(Station, StringComparer.Ordinal))
                throw new ArgumentException($"The source has no charging station '{Station}'!", nameof(Station));

            this.Station  = Station;
            this.IsNew    = IsNew;

            var setter    = Source.Writes ? Source as ISetVariablesSource : null;

            // Each may be monitored but the secrets, which are never read: a
            // station is let in and shut out on the page as well.
            foreach (var variable in Variables)
                variableConfigs.Add(variable.ToVariableConfig(variable.WriteOnly
                                                                  ? () => null
                                                                  : () => Source.StationValue(Station, variable.Name),
                                                              setter is not null,
                                                              Monitored: !variable.WriteOnly));

            if (setter is not null)
                SetVariablesSource = setter;

        }

        #endregion


        #region (static) NewInstance(Source, Station)

        /// <summary>
        /// The controller of a charging station the source does not have yet,
        /// standing in for it while a SetVariables request that lists it is set.
        /// </summary>
        /// <param name="Source">Where the values of the station will come from.</param>
        /// <param name="Station">What the new station calls itself; the instance.</param>
        public static StationLoginController NewInstance(IStationLoginsSource  Source,
                                                         String                Station)

            => new (Source, Station, true);

        #endregion

        #region (static) TryCheck(Station, Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable of a station can be set to the given value
        /// by its type, its limits and its values alone - or why not, as a
        /// SetVariables result says it. Whether it fits the rest - a password
        /// long enough, a group that is there - is for the source to ask.
        /// </summary>
        /// <param name="Station">The charging station, to name it in a refusal.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String                                Station,
                                       String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)

            => LoginVariables.TryCheck(Variables, $"{nameof(StationLoginController)}[{Station}]", Variable, Value, out Refusal);

        #endregion

    }


    /// <summary>
    /// One login group of the OCPP server of a node: what its charging stations
    /// may sign in with and over which security profiles. An instance for every
    /// group, named by its id.
    /// </summary>
    /// <remarks>
    /// A group that is not there yet is made by a request that names it and sets
    /// its AuthMethods and SecurityProfiles. Listed set to false takes it away -
    /// not the default group, and not one with members: they are moved first,
    /// and whoever moves them sees where they land.
    /// </remarks>
    public class StationLoginGroupController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of a login group, in the order of the node's page.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("Name",              DataTypes.String,      "What the group is called on screen.",                                                                                                      MaxLimit: 60,                         Writable: true),
            new ("Enabled",           DataTypes.Boolean,     "Whether its members may sign in at the moment.",                                                                                                                                  Writable: true),
            new ("AuthMethods",       DataTypes.MemberList,  "The ways its members may prove who they are.",                                                                                             ValuesList: [ "basic", "totp", "certificate", "sshkey", "keypin" ], Writable: true),
            new ("SecurityProfiles",  DataTypes.MemberList,  "The OCPP security profiles its members may come in on.",                                                                                   ValuesList: [ "1", "2", "3" ],        Writable: true),
            new ("SSHChannels",       DataTypes.MemberList,  "What its members have over their SSH connection besides OCPP: their time, their names; empty for OCPP alone.",                                     ValuesList: [ "ntske/1@open.charging.cloud", "dns@open.charging.cloud" ], Writable: true, MayBeEmpty: true),
            new ("Note",              DataTypes.String,      "What the group is for, for whoever reads the list later.",                                                                                 MaxLimit: 200,                        Writable: true, MayBeEmpty: true),
            new ("Members",           DataTypes.Integer,     "How many charging stations it has."),
            new ("AddedAt",           DataTypes.DateTime,    "When it was made."),
            new ("Listed",            DataTypes.Boolean,     "Whether it is there; false takes it away - not the default group, and not one with members.",                                                                                     Writable: true)

        ];

        /// <summary>
        /// The variables a new group is made with: both, set.
        /// </summary>
        public static readonly IReadOnlyList<String> MadeWith = [ "AuthMethods", "SecurityProfiles" ];

        #endregion

        #region Properties

        /// <summary>
        /// The login group: the instance.
        /// </summary>
        public String   Group    { get; }

        /// <summary>
        /// Whether this stands for a group a SetVariables request makes, which
        /// the source does not have yet - see <see cref="NewInstance"/>.
        /// </summary>
        public Boolean  IsNew    { get; }

        /// <summary>
        /// The names of the variables of a group.
        /// </summary>
        public static IEnumerable<String> VariableNames
            => Variables.Select(variable => variable.Name);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The controller of the given login group of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Group">The login group; the instance.</param>
        public StationLoginGroupController(IStationLoginsSource  Source,
                                           String                Group)

            : this(Source, Group, false)

        { }

        private StationLoginGroupController(IStationLoginsSource  Source,
                                            String                Group,
                                            Boolean               IsNew)

            : base(nameof(StationLoginGroupController),
                   Group,
                   I18NString.Create($"The login group '{Group}' of the charging stations."),
                   null)

        {

            if (!IsNew && !Source.Groups.Contains(Group, StringComparer.Ordinal))
                throw new ArgumentException($"The source has no login group '{Group}'!", nameof(Group));

            this.Group  = Group;
            this.IsNew  = IsNew;

            var setter  = Source.Writes ? Source as ISetVariablesSource : null;

            foreach (var variable in Variables)
                variableConfigs.Add(variable.ToVariableConfig(() => Source.GroupValue(Group, variable.Name),
                                                              setter is not null,
                                                              Monitored: true));

            if (setter is not null)
                SetVariablesSource = setter;

        }

        #endregion


        #region (static) NewInstance(Source, Group)

        /// <summary>
        /// The controller of a login group the source does not have yet,
        /// standing in for it while a SetVariables request that makes it is set.
        /// </summary>
        /// <param name="Source">Where the values of the group will come from.</param>
        /// <param name="Group">The id of the new group; the instance.</param>
        public static StationLoginGroupController NewInstance(IStationLoginsSource  Source,
                                                              String                Group)

            => new (Source, Group, true);

        #endregion

        #region (static) TryCheck(Group, Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable of a group can be set to the given value by
        /// its type, its limits and its values alone - or why not, as a
        /// SetVariables result says it.
        /// </summary>
        /// <param name="Group">The login group, to name it in a refusal.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String                                Group,
                                       String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)

            => LoginVariables.TryCheck(Variables, $"{nameof(StationLoginGroupController)}[{Group}]", Variable, Value, out Refusal);

        #endregion

    }


    /// <summary>
    /// What both controllers of the logins check alike.
    /// </summary>
    internal static class LoginVariables
    {

        /// <summary>
        /// Whether the named variable of the given ones can be set to the given
        /// value: "NotFound" where there is none, "ReadOnly" where it is only
        /// read, "InvalidValue" where it is empty and may not be, and else what
        /// its type, its limits and its values say.
        /// </summary>
        public static Boolean TryCheck(IEnumerable<ControllerVariable>       Variables,
                                       String                                Component,
                                       String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            var variable = Variables.FirstOrDefault(variable => String.Equals(variable.Name, Variable, StringComparison.OrdinalIgnoreCase));

            if (variable is null)
            {
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{Component}'!");
                return false;
            }

            var where = $"{Component}.{variable.Name}";

            if (!variable.Writable)
            {
                Refusal = new StatusInfo("ReadOnly", $"{where} is read-only!");
                return false;
            }

            if (String.IsNullOrWhiteSpace(Value) && !variable.MayBeEmpty)
            {
                Refusal = new StatusInfo("InvalidValue", $"'{where}' must not be empty!");
                return false;
            }

            return variable.TryCheck(Value, where, out Refusal);

        }

    }

}
