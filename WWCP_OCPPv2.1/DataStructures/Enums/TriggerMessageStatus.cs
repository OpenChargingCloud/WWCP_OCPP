/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for trigger message status.
    /// </summary>
    public static class TriggerMessageStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a trigger message status.
        /// </summary>
        /// <param name="Text">A text representation of a trigger message status.</param>
        public static TriggerMessageStatus Parse(String Text)
        {

            if (TryParse(Text, out var transportProtocol))
                return transportProtocol;

            return TriggerMessageStatus.NotImplemented;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a trigger message status.
        /// </summary>
        /// <param name="Text">A text representation of a trigger message status.</param>
        public static TriggerMessageStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var transportProtocol))
                return transportProtocol;

            return null;

        }

        #endregion

        #region TryParse(Text, out TriggerMessageStatus)

        /// <summary>
        /// Try to parse the given text as a trigger message status.
        /// </summary>
        /// <param name="Text">A text representation of a trigger message status.</param>
        /// <param name="TriggerMessageStatus">The parsed trigger message status.</param>
        public static Boolean TryParse(String Text, out TriggerMessageStatus TriggerMessageStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    TriggerMessageStatus = TriggerMessageStatus.Accepted;
                    return true;

                case "Rejected":
                    TriggerMessageStatus = TriggerMessageStatus.Rejected;
                    return true;

                default:
                    TriggerMessageStatus = TriggerMessageStatus.NotImplemented;
                    return false;

            }
        }

        #endregion


        #region AsText(this TriggerMessageStatus)

        public static String AsText(this TriggerMessageStatus TriggerMessageStatus)

            => TriggerMessageStatus switch {
                   TriggerMessageStatus.Accepted  => "Accepted",
                   TriggerMessageStatus.Rejected  => "Rejected",
                   _                              => "NotImplemented"
               };

        #endregion

    }


    /// <summary>
    /// Trigger message status.
    /// </summary>
    public enum TriggerMessageStatus
    {

        /// <summary>
        /// Requested notification will be sent.
        /// </summary>
        Accepted,

        /// <summary>
        /// Requested notification will not be sent.
        /// </summary>
        Rejected,

        /// <summary>
        /// Requested notification cannot be sent because
        /// it is either not implemented or unknown.
        /// </summary>
        NotImplemented

    }

}
