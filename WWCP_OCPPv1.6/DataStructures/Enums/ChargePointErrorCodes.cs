/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extention methods for charge point error codes.
    /// </summary>
    public static class ChargePointErrorCodeExtensions
    {

        #region Parse(Text)

        public static ChargePointErrorCodes Parse(String Text)

            => Text.Trim() switch {
                   "ConnectorLockFailure"  => ChargePointErrorCodes.ConnectorLockFailure,
                   "EVCommunicationError"  => ChargePointErrorCodes.EVCommunicationError,
                   "GroundFailure"         => ChargePointErrorCodes.GroundFailure,
                   "HighTemperature"       => ChargePointErrorCodes.HighTemperature,
                   "InternalError"         => ChargePointErrorCodes.InternalError,
                   "LocalListConflict"     => ChargePointErrorCodes.LocalListConflict,
                   "NoError"               => ChargePointErrorCodes.NoError,
                   "OtherError"            => ChargePointErrorCodes.OtherError,
                   "OverCurrentFailure"    => ChargePointErrorCodes.OverCurrentFailure,
                   "OverVoltage"           => ChargePointErrorCodes.OverVoltage,
                   "PowerMeterFailure"     => ChargePointErrorCodes.PowerMeterFailure,
                   "PowerSwitchFailure"    => ChargePointErrorCodes.PowerSwitchFailure,
                   "ReaderFailure"         => ChargePointErrorCodes.ReaderFailure,
                   "ResetFailure"          => ChargePointErrorCodes.ResetFailure,
                   "UnderVoltage"          => ChargePointErrorCodes.UnderVoltage,
                   "WeakSignal"            => ChargePointErrorCodes.WeakSignal,
                   _                       => ChargePointErrorCodes.Unknown
               };

        #endregion

        #region AsText(this ChargePointErrorCode)

        public static String AsText(this ChargePointErrorCodes ChargePointErrorCode)

            => ChargePointErrorCode switch {
                   ChargePointErrorCodes.ConnectorLockFailure  => "ConnectorLockFailure",
                   ChargePointErrorCodes.EVCommunicationError  => "EVCommunicationError",
                   ChargePointErrorCodes.GroundFailure         => "GroundFailure",
                   ChargePointErrorCodes.HighTemperature       => "HighTemperature",
                   ChargePointErrorCodes.InternalError         => "InternalError",
                   ChargePointErrorCodes.LocalListConflict     => "LocalListConflict",
                   ChargePointErrorCodes.NoError               => "NoError",
                   ChargePointErrorCodes.OtherError            => "OtherError",
                   ChargePointErrorCodes.OverCurrentFailure    => "OverCurrentFailure",
                   ChargePointErrorCodes.OverVoltage           => "OverVoltage",
                   ChargePointErrorCodes.PowerMeterFailure     => "PowerMeterFailure",
                   ChargePointErrorCodes.PowerSwitchFailure    => "PowerSwitchFailure",
                   ChargePointErrorCodes.ReaderFailure         => "ReaderFailure",
                   ChargePointErrorCodes.ResetFailure          => "ResetFailure",
                   ChargePointErrorCodes.UnderVoltage          => "UnderVoltage",
                   ChargePointErrorCodes.WeakSignal            => "WeakSignal",
                   _                                           => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// The charge point status reported in a StatusNotification request.
    /// </summary>
    public enum ChargePointErrorCodes
    {

        /// <summary>
        /// Unknown charge point error code.
        /// </summary>
        Unknown,


        /// <summary>
        /// Failure to lock or unlock connector.
        /// </summary>
        ConnectorLockFailure,

        /// <summary>
        /// Communication failure with the vehicle, might be Mode 3 or other
        /// communication protocol problem. This is not a real error in the
        /// sense that the Charge Point doesn’t need to go to the faulted
        /// state. Instead, it should go to the SuspendedEVSE state.
        /// </summary>
        EVCommunicationError,

        /// <summary>
        /// Ground fault circuit interrupter has been activated.
        /// </summary>
        GroundFailure,

        /// <summary>
        /// Temperature inside Charge Point is too high.
        /// </summary>
        HighTemperature,

        /// <summary>
        /// Error in internal hard- or software component.
        /// </summary>
        InternalError,

        /// <summary>
        /// The authorization information received from the central system
        /// is in conflict with the LocalAuthorizationList.
        /// </summary>
        LocalListConflict,

        /// <summary>
        /// No error to report.
        /// </summary>
        NoError,

        /// <summary>
        /// Other type of error. More information in vendorErrorCode.
        /// </summary>
        OtherError,

        /// <summary>
        /// Over current protection device has tripped.
        /// </summary>
        OverCurrentFailure,

        /// <summary>
        /// Voltage has risen above an acceptable level.
        /// </summary>
        OverVoltage,

        /// <summary>
        /// Failure to read power meter.
        /// </summary>
        PowerMeterFailure,

        /// <summary>
        /// Failure to control power switch.
        /// </summary>
        PowerSwitchFailure,

        /// <summary>
        /// Failure with idTag reader.
        /// </summary>
        ReaderFailure,

        /// <summary>
        /// Unable to perform a reset.
        /// </summary>
        ResetFailure,

        /// <summary>
        /// Voltage has dropped below an acceptable level.
        /// </summary>
        UnderVoltage,

        /// <summary>
        /// Wireless communication device reports a weak signal.
        /// </summary>
        WeakSignal


    }

}
