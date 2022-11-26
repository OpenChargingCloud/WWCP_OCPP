/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for the charging profile purposes.
    /// </summary>
    public static class ChargingProfilePurposesExtentions
    {

        #region Parse(Text)

        public static ChargingProfilePurposes Parse(String Text)

            => Text.Trim() switch {
                   "ChargingStationExternalConstraints"  => ChargingProfilePurposes.ChargingStationExternalConstraints,
                   "ChargePointMaxProfile"               => ChargingProfilePurposes.ChargePointMaxProfile,
                   "TxProfile"                           => ChargingProfilePurposes.TxProfile,
                   _                                     => ChargingProfilePurposes.TxDefaultProfile
               };

        #endregion

        #region AsText(this ChargingProfilePurpose)

        public static String AsText(this ChargingProfilePurposes ChargingProfilePurpose)

            => ChargingProfilePurpose switch {
                   ChargingProfilePurposes.ChargingStationExternalConstraints  => "ChargingStationExternalConstraints",
                   ChargingProfilePurposes.ChargePointMaxProfile               => "ChargePointMaxProfile",
                   ChargingProfilePurposes.TxProfile                           => "TxProfile",
                   _                                                           => "TxDefaultProfile"
               };

        #endregion

    }


    /// <summary>
    /// Defines the charging-profile-purpose-values.
    /// </summary>
    public enum ChargingProfilePurposes
    {

        /// <summary>
        /// Additional constraints that will be incorporated into a local power schedule. Only valid for a charging station.
        /// Therefore the EVSE identification MUST be 0 in the SetChargingProfileRequest message.
        /// </summary>
        ChargingStationExternalConstraints,

        /// <summary>
        /// Configuration for the maximum power or current available
        /// for an entire charging station.
        /// </summary>
        ChargePointMaxProfile,

        /// <summary>
        /// Default profile to be used for new transactions.
        /// </summary>
        TxDefaultProfile,

        /// <summary>
        /// Profile with constraints to be imposed by the charging station
        /// on the current transaction. A profile with this purpose
        /// SHALL cease to be valid when the transaction terminates.
        /// </summary>
        TxProfile

    }

}
