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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// What each OCPP 2.1 action a CSMS sends needs on a charging station, as
    /// permissions of the node's role model: the table a station ships (white
    /// paper "Signed OCPP Messages and Signer Roles", section 5.3, Q12).
    /// </summary>
    /// <remarks>
    /// The policy table of a station can say otherwise for any action. The
    /// entries of SetVariables and GetVariables are not here: they are decided
    /// one by one, by the resource of their component.
    /// </remarks>
    public static class ChargingStationActionPermissions
    {

        private const String Purpose           = "chargingProfile.chargingProfilePurpose";
        private const String CriteriaPurpose   = "chargingProfileCriteria.chargingProfilePurpose";
        private const String RequestedMessage  = "requestedMessage";

        /// <summary>
        /// Charging profiles that are the operator's everyday load management.
        /// </summary>
        private static readonly String[] ofASession    = [ "TxProfile", "TxDefaultProfile", "PriorityCharging" ];

        /// <summary>
        /// Charging profiles that say what the station may draw and deliver.
        /// </summary>
        private static readonly String[] ofTheStation  = [ "ChargingStationMaxProfile", "LocalGeneration" ];

        private const String ExternalConstraints       = "External constraints come from an energy management system or the grid operator, not from a CSMS";

        /// <summary>
        /// The table.
        /// </summary>
        /// <param name="IsMetrologicallyRelevant">Whether the station holds a calibration certificate or signs sessions over a meter it reads. The station says so itself: a firmware update then needs "calibration:edit" too.</param>
        public static IEnumerable<ActionPermission> Default(Func<Boolean>? IsMetrologicallyRelevant = null)

            => [

                   // Restarting: once idle, or at once.
                   ActionPermission.Needs  ("Reset",                   "reset:run",            FieldCondition.Equal("type", "OnIdle")),
                   ActionPermission.Needs  ("Reset",                   "reset-immediate:run"),

                   // A message asked for: the EVSE status, a new certificate, anything else.
                   ActionPermission.Needs  ("TriggerMessage",          "state:run",            FieldCondition.Equal(RequestedMessage, "StatusNotification")),
                   ActionPermission.Needs  ("TriggerMessage",          "certificates:edit",    FieldCondition.In   (RequestedMessage, "SignChargingStationCertificate",
                                                                                                                                      "SignV2GCertificate",
                                                                                                                                      "SignV2G20Certificate",
                                                                                                                                      "SignCombinedCertificate")),
                   ActionPermission.Needs  ("TriggerMessage",          "node:run"),

                   ActionPermission.Needs  ("InstallCertificate",      "certificates:edit"),
                   ActionPermission.Needs  ("DeleteCertificate",       "certificates:edit"),
                   ActionPermission.Needs  ("CertificateSigned",       "certificates:edit"),

                   ActionPermission.Needs  ("GetLog",                  "log:read"),

                   ActionPermission.Needs  ("SyncNetworkTime",         "nts:run"),
                   ActionPermission.Needs  ("TestTimeServer",          "nts:run"),

                   ActionPermission.Needs  ("RequestStartTransaction", "session:run"),
                   ActionPermission.Needs  ("RequestStopTransaction",  "session:run"),
                   ActionPermission.Needs  ("UnlockConnector",         "unlock:run"),
                   ActionPermission.Needs  ("ChangeAvailability",      "availability:edit"),

                   // Charging profiles, by their purpose. One whose purpose is none of these
                   // needs both permissions.
                   ActionPermission.Needs  ("SetChargingProfile",      "session:run",          FieldCondition.In   (Purpose, ofASession)),
                   ActionPermission.Needs  ("SetChargingProfile",      "power:edit",           FieldCondition.In   (Purpose, ofTheStation)),
                   ActionPermission.Refuses("SetChargingProfile",      ExternalConstraints,    FieldCondition.Equal(Purpose, "ChargingStationExternalConstraints")),
                   new ActionPermission    ("SetChargingProfile",      [ "session:run", "power:edit" ]),

                   // Clearing needs what the profiles it would clear need: without a purpose
                   // among its criteria that is all of them.
                   ActionPermission.Needs  ("ClearChargingProfile",    "session:run",          FieldCondition.In   (CriteriaPurpose, ofASession)),
                   ActionPermission.Needs  ("ClearChargingProfile",    "power:edit",           FieldCondition.In   (CriteriaPurpose, ofTheStation)),
                   ActionPermission.Refuses("ClearChargingProfile",    ExternalConstraints,    FieldCondition.Equal(CriteriaPurpose, "ChargingStationExternalConstraints")),
                   new ActionPermission    ("ClearChargingProfile",    [ "session:run", "power:edit" ]),

                   new ActionPermission    ("UpdateFirmware",
                                            [ "firmware:edit" ],
                                            AdditionalPermissions: () => IsMetrologicallyRelevant?.Invoke() == true
                                                                             ? [ "calibration:edit" ]
                                                                             : []),
                   ActionPermission.Needs  ("PublishFirmware",         "firmware:edit"),

                   ActionPermission.Needs  ("SetNetworkProfile",       "connections:edit"),

                   // The roles and the signature policy: phase 4 renames these messages.
                   ActionPermission.Needs  ("AddUserRole",             "signatures:edit"),
                   ActionPermission.Needs  ("UpdateUserRole",          "signatures:edit"),
                   ActionPermission.Needs  ("DeleteUserRole",          "signatures:edit"),
                   ActionPermission.Needs  ("AddSignaturePolicy",      "signatures:edit"),
                   ActionPermission.Needs  ("UpdateSignaturePolicy",   "signatures:edit"),
                   ActionPermission.Needs  ("DeleteSignaturePolicy",   "signatures:edit")

               ];

    }

}
