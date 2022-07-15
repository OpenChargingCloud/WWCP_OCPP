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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extention methods for charge point error codes.
    /// </summary>
    public static class ChargePointErrorCodeExtentions
    {

        #region Parse(Text)

        public static ChargePointErrorCodes Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "ConnectorLockFailure":
                    return ChargePointErrorCodes.ConnectorLockFailure;

                case "EVCommunicationError":
                    return ChargePointErrorCodes.EVCommunicationError;

                case "GroundFailure":
                    return ChargePointErrorCodes.GroundFailure;

                case "HighTemperature":
                    return ChargePointErrorCodes.HighTemperature;

                case "InternalError":
                    return ChargePointErrorCodes.InternalError;

                case "LocalListConflict":
                    return ChargePointErrorCodes.LocalListConflict;

                case "NoError":
                    return ChargePointErrorCodes.NoError;

                case "OtherError":
                    return ChargePointErrorCodes.OtherError;

                case "OverCurrentFailure":
                    return ChargePointErrorCodes.OverCurrentFailure;

                case "OverVoltage":
                    return ChargePointErrorCodes.OverVoltage;

                case "PowerMeterFailure":
                    return ChargePointErrorCodes.PowerMeterFailure;

                case "PowerSwitchFailure":
                    return ChargePointErrorCodes.PowerSwitchFailure;

                case "ReaderFailure":
                    return ChargePointErrorCodes.ReaderFailure;

                case "ResetFailure":
                    return ChargePointErrorCodes.ResetFailure;

                case "UnderVoltage":
                    return ChargePointErrorCodes.UnderVoltage;

                case "WeakSignal":
                    return ChargePointErrorCodes.WeakSignal;


                default:
                    return ChargePointErrorCodes.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargePointErrorCode)

        public static String AsText(this ChargePointErrorCodes ChargePointErrorCode)
        {

            switch (ChargePointErrorCode)
            {

                case ChargePointErrorCodes.ConnectorLockFailure:
                    return "ConnectorLockFailure";

                case ChargePointErrorCodes.EVCommunicationError:
                    return "EVCommunicationError";

                case ChargePointErrorCodes.GroundFailure:
                    return "GroundFailure";

                case ChargePointErrorCodes.HighTemperature:
                    return "HighTemperature";

                case ChargePointErrorCodes.InternalError:
                    return "InternalError";

                case ChargePointErrorCodes.LocalListConflict:
                    return "LocalListConflict";

                case ChargePointErrorCodes.NoError:
                    return "NoError";

                case ChargePointErrorCodes.OtherError:
                    return "OtherError";

                case ChargePointErrorCodes.OverCurrentFailure:
                    return "OverCurrentFailure";

                case ChargePointErrorCodes.OverVoltage:
                    return "OverVoltage";

                case ChargePointErrorCodes.PowerMeterFailure:
                    return "PowerMeterFailure";

                case ChargePointErrorCodes.PowerSwitchFailure:
                    return "PowerSwitchFailure";

                case ChargePointErrorCodes.ReaderFailure:
                    return "ReaderFailure";

                case ChargePointErrorCodes.ResetFailure:
                    return "ResetFailure";

                case ChargePointErrorCodes.UnderVoltage:
                    return "UnderVoltage";

                case ChargePointErrorCodes.WeakSignal:
                    return "WeakSignal";


                default:
                    return "unknown";

            }

        }

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
