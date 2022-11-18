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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    // ToDo: Should not be an enum, but a struct to allow extensibility!

    /// <summary>
    /// Extentions methods for the message triggers.
    /// </summary>
    public static class MessageTriggersExtentions
    {

        #region Parse(Text)

        public static MessageTriggers Parse(String Text)

            => Text.Trim() switch {
                   "BootNotification"               => MessageTriggers.BootNotification,
                   "LogStatusNotification"          => MessageTriggers.LogStatusNotification,
                   "DiagnosticsStatusNotification"  => MessageTriggers.DiagnosticsStatusNotification,
                   "FirmwareStatusNotification"     => MessageTriggers.FirmwareStatusNotification,
                   "Heartbeat"                      => MessageTriggers.Heartbeat,
                   "MeterValues"                    => MessageTriggers.MeterValues,
                   "SignChargePointCertificate"     => MessageTriggers.SignChargePointCertificate,
                   "StatusNotification"             => MessageTriggers.StatusNotification,
                   _                                => MessageTriggers.Unknown
               };

        #endregion

        #region AsText(this MessageTrigger)

        public static String AsText(this MessageTriggers MessageTrigger)

            => MessageTrigger switch {
                   MessageTriggers.BootNotification               => "BootNotification",
                   MessageTriggers.LogStatusNotification          => "LogStatusNotification",
                   MessageTriggers.DiagnosticsStatusNotification  => "DiagnosticsStatusNotification",
                   MessageTriggers.FirmwareStatusNotification     => "FirmwareStatusNotification",
                   MessageTriggers.Heartbeat                      => "Heartbeat",
                   MessageTriggers.MeterValues                    => "MeterValues",
                   MessageTriggers.SignChargePointCertificate     => "SignChargePointCertificate",
                   MessageTriggers.StatusNotification             => "StatusNotification",
                   _                                              => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the message-trigger-values.
    /// </summary>
    public enum MessageTriggers
    {

        /// <summary>
        /// Unknown message-trigger status.
        /// </summary>
        Unknown,

        /// <summary>
        /// To trigger a BootNotification request.
        /// </summary>
        BootNotification,

        /// <summary>
        /// To trigger LogStatusNotification.req.
        /// </summary>
        LogStatusNotification,

        /// <summary>
        /// To trigger a DiagnosticsStatusNotification request.
        /// </summary>
        DiagnosticsStatusNotification,

        /// <summary>
        /// To trigger a FirmwareStatusNotification request.
        /// </summary>
        FirmwareStatusNotification,

        /// <summary>
        /// To trigger a Heartbeat request.
        /// </summary>
        Heartbeat,

        /// <summary>
        /// To trigger a MeterValues request.
        /// </summary>
        MeterValues,

        /// <summary>
        /// To trigger a SignCertificate.req with certificateType: ChargePointCertificate.
        /// </summary>
        SignChargePointCertificate,

        /// <summary>
        /// To trigger a StatusNotification request.
        /// </summary>
        StatusNotification

    }

}
