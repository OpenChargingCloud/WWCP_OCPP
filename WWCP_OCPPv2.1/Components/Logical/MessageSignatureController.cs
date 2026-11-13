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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where MessageSignatureController and SignerRoleController read how a
    /// charging station holds signed messages to its signature policy, and
    /// which keys sign as which of its roles.
    /// </summary>
    /// <remarks>
    /// The controllers keep no value of their own: whatever holds the policy
    /// and the signer roles answers each read, so that a change made
    /// elsewhere - a role assignment taken, a policy set, a mode changed at
    /// the station - is what the next read sees. A source that is an
    /// <see cref="ISetVariablesSource"/> as well takes the writes of a
    /// SetVariables request, all of the request's at once.
    /// </remarks>
    public interface IMessageSignatureSource
    {

        /// <summary>
        /// The variables of MessageSignatureController this source answers.
        /// </summary>
        IEnumerable<String>  Variables            { get; }

        /// <summary>
        /// The variables of MessageSignatureController this source writes.
        /// </summary>
        IEnumerable<String>  WritableVariables    { get; }

        /// <summary>
        /// The roles that are instances of SignerRoleController: those of the
        /// station's role model, whether they have keys or not.
        /// </summary>
        IEnumerable<String>  Roles                { get; }

        /// <summary>
        /// The variables of a role this source answers.
        /// </summary>
        IEnumerable<String>  RoleVariables        { get; }

        /// <summary>
        /// The value of a variable of MessageSignatureController, in the words of the white paper; null for none.
        /// </summary>
        /// <param name="Variable">The variable, as the paper spells it.</param>
        String? Value(String Variable);

        /// <summary>
        /// The value of a variable of a role; null for none.
        /// </summary>
        /// <param name="Role">The role: an instance of SignerRoleController.</param>
        /// <param name="Variable">The variable, as the paper spells it.</param>
        String? RoleValue(String Role, String Variable);

    }


    /// <summary>
    /// The message signature controller of the white paper "Signed OCPP
    /// Messages and Signer Roles" (section 6.3.1): whether and how a charging
    /// station holds what it receives to its signature policy.
    /// </summary>
    /// <remarks>
    /// The signature policy counts it as "signatures": reading it needs
    /// "signatures:read", writing it "signatures:edit", whatever an entry of
    /// the policy table says of it.
    /// </remarks>
    public class MessageSignatureController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The modes of the policy.
        /// </summary>
        public static readonly IReadOnlyList<String> Modes = [ "Off", "Observe", "Enforce" ];

        /// <summary>
        /// The variables, in the order of the paper.
        /// </summary>
        internal static readonly ControllerVariable[] ControllerVariables = [

            new ("Enabled",                DataTypes.Boolean,     "Whether signatures are produced and verified at all.",                                                                                               Writable: true),
            new ("Mode",                   DataTypes.OptionList,  "Off: signatures are ignored. Observe: they are verified and the policy is evaluated, and what it would refuse is only reported. Enforce: the policy applies.", ValuesList: Modes, Writable: true),
            new ("Formats",                DataTypes.MemberList,  "The encodings of the signatures this station writes, where a connection does not say otherwise.",                                                    ValuesList: [ "Embedded", "COSE" ], Writable: true),
            new ("AcceptFormat0",          DataTypes.Boolean,     "Whether a signature of the format before the protected header is accepted, as unverified.",                                                          Writable: true),
            new ("Algorithms",             DataTypes.MemberList,  "The algorithms of the signatures this station verifies."),
            new ("SignResponses",          DataTypes.Boolean,     "Whether every response to a signed request is signed.",                                                                                              Writable: true),
            new ("SignOwnMessages",        DataTypes.MemberList,  "The requests this station signs by itself.",                                                                                                         Writable: true, MayBeEmpty: true),
            new ("MaxClockSkew",           DataTypes.Integer,     "How far in the future a signature may say it was made.",                                                                                             MinLimit: 0, MaxLimit: 86400, InSeconds: true, Writable: true),
            new ("MaxSignatureAge",        DataTypes.Integer,     "How long a signature that does not say when it expires is valid.",                                                                                   MinLimit: 1, MaxLimit: 86400, InSeconds: true, Writable: true),
            new ("ReplayCacheSize",        DataTypes.Integer,     "How many signatures this station remembers to have seen."),
            new ("Roles",                  DataTypes.String,      "The roles of this station a key may sign as, comma separated: the instances of SignerRoleController.",                                               MayBeEmpty: true),
            new ("PolicyVersion",          DataTypes.Integer,     "The version of the policy table this station was given; 0 while it was given none."),
            new ("OwnKeyId",               DataTypes.String,      "The identifier of the key this station signs with.",                                                                                                 MayBeEmpty: true),

            // Written at the station alone: whoever may write it over the network could make itself the owner.
            new ("AllowFirstOwnerByCSMS",  DataTypes.Boolean,     "Whether this station, while it has no owner, takes its first owner from the first role assignment a key of its own signed.")

        ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The message signature controller of a station.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public MessageSignatureController(IMessageSignatureSource Source)

            : base(nameof(MessageSignatureController),
                   null,
                   I18NString.Create("How this charging station holds signed messages to its signature policy."),
                   null)

        {

            var setter    = Source as ISetVariablesSource;
            var answered  = new HashSet<String>(Source.Variables,                                       StringComparer.OrdinalIgnoreCase);
            var written   = new HashSet<String>(setter is not null ? Source.WritableVariables : [],     StringComparer.OrdinalIgnoreCase);

            // Each may be monitored: a CSMS is told when the mode changes, or the version of the policy.
            foreach (var variable in ControllerVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(variable.ToVariableConfig(() => Source.Value(variable.Name),
                                                              written.Contains(variable.Name),
                                                              Monitored: true));

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        #endregion

        #region (static) TryCheck(Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable can be set to the given value by its
        /// type, its limits and its values alone; or why not, as a
        /// SetVariables result says it.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            var variable = ControllerVariables.FirstOrDefault(variable => String.Equals(variable.Name, Variable, StringComparison.OrdinalIgnoreCase));

            if (variable is null)
            {
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{nameof(MessageSignatureController)}'!");
                return false;
            }

            var where = $"{nameof(MessageSignatureController)}.{variable.Name}";

            if (!variable.Writable)
            {
                Refusal = new StatusInfo("ReadOnly", $"{where} is read-only!");
                return false;
            }

            return variable.TryCheck(Value, where, out Refusal);

        }

        #endregion

    }


    /// <summary>
    /// The signer role controller of the white paper "Signed OCPP Messages
    /// and Signer Roles" (section 6.3.2): one instance for each role of the
    /// station a key may sign as.
    /// </summary>
    /// <remarks>
    /// Its variables are read-only. Keys are given to a role by a signed
    /// role assignment (AddSignerRole, UpdateSignerRole), and not by a
    /// SetVariables request: an assignment carries the owner's signature
    /// over what it says, and is stored as the evidence it is.
    /// </remarks>
    public class SignerRoleController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of a role, in the order of the paper.
        /// </summary>
        internal static readonly ControllerVariable[] RoleVariables = [
            new ("Description",       DataTypes.String,    "What the role is for.",                                                                         MayBeEmpty: true),
            new ("Permissions",       DataTypes.String,    "What the role may do on this station, comma separated: \"resource:operation\".",                MayBeEmpty: true),
            new ("KeyIds",            DataTypes.String,    "The identifiers of the keys that sign as the role, comma separated.",                           MayBeEmpty: true),
            new ("CertificateGroup",  DataTypes.String,    "The group of the root certificates whose certificates may carry the role.",                     MayBeEmpty: true),
            new ("Version",           DataTypes.Integer,   "The version of the role assignment that is stored; 0 for a role that never had keys."),
            new ("LastUsed",          DataTypes.DateTime,  "When a signature of the role was last accepted.",                                               MayBeEmpty: true)
        ];

        #endregion

        #region Properties

        /// <summary>
        /// The role this instance is of.
        /// </summary>
        public String  Role    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The instance of one role.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Role">The role; the instance.</param>
        public SignerRoleController(IMessageSignatureSource  Source,
                                    String                   Role)

            : base(nameof(SignerRoleController),
                   Role,
                   I18NString.Create($"The keys that sign as the role '{Role}' of this charging station."),
                   null)

        {

            if (!Source.Roles.Contains(Role, StringComparer.Ordinal))
                throw new ArgumentException($"The source has no role '{Role}'!", nameof(Role));

            this.Role = Role;

            var answered = new HashSet<String>(Source.RoleVariables, StringComparer.OrdinalIgnoreCase);

            // Each may be monitored: a CSMS is told when a role gets other keys.
            foreach (var variable in RoleVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(variable.ToVariableConfig(() => Source.RoleValue(Role, variable.Name),
                                                              false,
                                                              Monitored: true));

        }

        #endregion

        #region (static) AllOf(Source)

        /// <summary>
        /// One instance for each role of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public static IEnumerable<SignerRoleController> AllOf(IMessageSignatureSource Source)
            => [ .. Source.Roles.Select(role => new SignerRoleController(Source, role)) ];

        #endregion

    }

}
