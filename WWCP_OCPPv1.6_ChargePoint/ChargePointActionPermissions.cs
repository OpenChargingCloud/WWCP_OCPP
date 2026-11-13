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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// What each action a central system sends needs of whoever signed it, on
    /// every OCPP 1.6 charge point: the table of section 5.3 of the white paper
    /// "Signed OCPP Messages and Signer Roles" (Q12), in the words of OCPP 1.6.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The resources are those of the node's role model, so that a role may
    /// sign what it may do on the web page and at the command line, and
    /// nothing else. A permission names a resource and an operation, as in
    /// "reset:run".
    /// </para>
    /// <para>
    /// A soft reset is the restart once idle, and a hard one the restart at
    /// once. A charge point that holds a soft reset to "reset:run" waits with
    /// it until nothing charges, which OCPP 1.6 does not ask of it.
    /// </para>
    /// <para>
    /// ChangeConfiguration and GetConfiguration are not in the table: they
    /// are decided key by key, by the resource <see cref="ComponentOfKey"/>
    /// leads to. An action that is neither in the table nor decided by key
    /// falls to the default of the policy.
    /// </para>
    /// </remarks>
    public static class ChargePointActionPermissions
    {

        private const String Purpose           = "csChargingProfiles.chargingProfilePurpose";
        private const String ClearedPurpose    = "chargingProfilePurpose";
        private const String RequestedMessage  = "requestedMessage";

        /// <summary>
        /// Charging profiles that are the operator's everyday load management.
        /// </summary>
        private static readonly String[] ofASession  = [ "TxProfile", "TxDefaultProfile" ];

        #region Default(IsMetrologicallyRelevant = null)

        /// <summary>
        /// The table.
        /// </summary>
        /// <param name="IsMetrologicallyRelevant">Whether the charge point holds a calibration certificate or signs sessions over a meter it reads. The charge point says so itself: a firmware update then needs "calibration:edit" too.</param>
        /// <param name="TakesItsFirstOwner">Whether the charge point has no owner yet and takes its first one over the network (section 6.1, option 3). The charge point says so itself: an AddSignerRole then needs nobody's permission as a request, since there is nobody yet who could hold one, and is held to its role assignment alone.</param>
        public static IEnumerable<ActionPermission> Default(Func<Boolean>? IsMetrologicallyRelevant   = null,
                                                            Func<Boolean>? TakesItsFirstOwner         = null)
        {

            IEnumerable<String> ofAMetrologicallyRelevantOne()
                => IsMetrologicallyRelevant?.Invoke() == true
                       ? [ "calibration:edit" ]
                       : [];

            return [

                   // Restarting: once idle, or at once.
                   ActionPermission.Needs  ("Reset",                   "reset:run",            FieldCondition.Equal("type", "Soft")),
                   ActionPermission.Needs  ("Reset",                   "reset-immediate:run"),

                   // A message asked for: the connector status, a new certificate, anything else.
                   ActionPermission.Needs  ("TriggerMessage",          "state:run",            FieldCondition.Equal(RequestedMessage, "StatusNotification")),
                   ActionPermission.Needs  ("TriggerMessage",          "node:run"),
                   ActionPermission.Needs  ("ExtendedTriggerMessage",  "state:run",            FieldCondition.Equal(RequestedMessage, "StatusNotification")),
                   ActionPermission.Needs  ("ExtendedTriggerMessage",  "certificates:edit",    FieldCondition.Equal(RequestedMessage, "SignChargePointCertificate")),
                   ActionPermission.Needs  ("ExtendedTriggerMessage",  "node:run"),

                   ActionPermission.Needs  ("InstallCertificate",      "certificates:edit"),
                   ActionPermission.Needs  ("DeleteCertificate",       "certificates:edit"),
                   ActionPermission.Needs  ("CertificateSigned",       "certificates:edit"),

                   ActionPermission.Needs  ("GetLog",                  "log:read"),
                   ActionPermission.Needs  ("GetDiagnostics",          "log:read"),

                   ActionPermission.Needs  ("SyncNetworkTime",         "nts:run"),
                   ActionPermission.Needs  ("TestTimeServer",          "nts:run"),

                   ActionPermission.Needs  ("RemoteStartTransaction",  "session:run"),
                   ActionPermission.Needs  ("RemoteStopTransaction",   "session:run"),
                   ActionPermission.Needs  ("UnlockConnector",         "unlock:run"),
                   ActionPermission.Needs  ("ChangeAvailability",      "availability:edit"),

                   // An outlet held for somebody, or let go again: what the sessions are about.
                   ActionPermission.Needs  ("ReserveNow",              "session:run"),
                   ActionPermission.Needs  ("CancelReservation",       "session:run"),

                   // Who may charge without the CSMS being asked: the local authorization
                   // list and the authorization cache.
                   ActionPermission.Needs  ("GetLocalListVersion",     "authorization:read"),
                   ActionPermission.Needs  ("SendLocalList",           "authorization:edit"),
                   ActionPermission.Needs  ("ClearCache",              "authorization:edit"),

                   // Whatever a vendor made of it: the policy table can ask for more, by
                   // its "vendorId" and "messageId".
                   ActionPermission.Needs  ("DataTransfer",            "node:run"),

                   // Charging profiles, by their purpose. OCPP 1.6 has neither external
                   // constraints nor local generation.
                   ActionPermission.Needs  ("SetChargingProfile",      "session:run",          FieldCondition.In   (Purpose, ofASession)),
                   ActionPermission.Needs  ("SetChargingProfile",      "power:edit",           FieldCondition.Equal(Purpose, "ChargePointMaxProfile")),
                   new ActionPermission    ("SetChargingProfile",      [ "session:run", "power:edit" ]),

                   // Clearing needs what the profiles it would clear need: without a purpose
                   // that is all of them.
                   ActionPermission.Needs  ("ClearChargingProfile",    "session:run",          FieldCondition.In   (ClearedPurpose, ofASession)),
                   ActionPermission.Needs  ("ClearChargingProfile",    "power:edit",           FieldCondition.Equal(ClearedPurpose, "ChargePointMaxProfile")),
                   new ActionPermission    ("ClearChargingProfile",    [ "session:run", "power:edit" ]),

                   new ActionPermission    ("UpdateFirmware",          [ "firmware:edit" ],    AdditionalPermissions: ofAMetrologicallyRelevantOne),
                   new ActionPermission    ("SignedUpdateFirmware",    [ "firmware:edit" ],    AdditionalPermissions: ofAMetrologicallyRelevantOne),

                   // The signer roles and the signature policy (section 6.4). What writes
                   // them needs the permission to, whatever a policy entry says.
                   new ActionPermission    ("AddSignerRole",
                                            [],
                                            AdditionalPermissions: () => TakesItsFirstOwner?.Invoke() == true
                                                                             ? []
                                                                             : [ "signatures:edit" ]),
                   ActionPermission.Needs  ("UpdateSignerRole",        "signatures:edit"),
                   ActionPermission.Needs  ("DeleteSignerRole",        "signatures:edit"),
                   ActionPermission.Needs  ("GetSignerRoles",          "signatures:read"),
                   ActionPermission.Needs  ("SetSignaturePolicy",      "signatures:edit"),
                   ActionPermission.Needs  ("GetSignaturePolicy",      "signatures:read")

               ];

        }

        #endregion

        #region ActionsDecidedPerKey

        /// <summary>
        /// The actions a charge point decides key by key, and not as a whole.
        /// </summary>
        public static readonly IReadOnlyList<String> ActionsDecidedPerKey = [ "ChangeConfiguration", "GetConfiguration" ];

        #endregion

        #region ComponentOfKey(Key)

        /// <summary>
        /// The device model component a configuration key is the OCPP 1.6 name
        /// of a variable of, by its first part: "dns.rootPins.3" belongs to the
        /// DNS client, "ntske.legal.aead" to the network time. The signature
        /// policy maps the component to a resource, as it does for OCPP 2.1.
        /// </summary>
        /// <param name="Key">A configuration key, whatever its case.</param>
        public static String ComponentOfKey(String Key)
        {

            var dot    = Key.IndexOf('.');
            var first  = (dot > 0 ? Key[..dot] : Key).ToLowerInvariant();

            return first switch {
                       "dns"    => "DNSClientController",
                       "ntp"    => "NTPClientController",
                       "nts"    => "NTSClientController",
                       "ntske"  => "NTSClientController",

                       // "signatures.roles.support.version" is a variable of the role's
                       // component, the other keys are the signature controller's: both
                       // are "signatures" to the policy.
                       "signatures"  => "MessageSignatureController",

                       // "sshPeers", "sshPeers.clientPublicKey" and "sshPeer.<name>.*":
                       // who a host is believed by, and where the node's files may go.
                       "sshpeers"  => "SSHPeerController",
                       "sshpeer"   => "SSHPeerController",

                       _        => "ConfigurationKeys"
                   };

        }

        #endregion

    }

}
