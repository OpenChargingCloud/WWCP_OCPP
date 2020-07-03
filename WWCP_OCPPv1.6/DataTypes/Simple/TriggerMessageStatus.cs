/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the trigger message status.
    /// </summary>
    public static class TriggerMessageStatusExtentions
    {

        #region AsTriggerMessageStatus(Text)

        public static TriggerMessageStatus AsTriggerMessageStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return TriggerMessageStatus.Accepted;

                case "Rejected":
                    return TriggerMessageStatus.Rejected;


                default:
                    return TriggerMessageStatus.NotImplemented;

            }

        }

        #endregion

        #region AsText(this TriggerMessageStatus)

        public static String AsText(this TriggerMessageStatus TriggerMessageStatus)
        {

            switch (TriggerMessageStatus)
            {

                case TriggerMessageStatus.Accepted:
                    return "Accepted";

                case TriggerMessageStatus.Rejected:
                    return "Rejected";


                default:
                    return "NotImplemented";

            }

        }

        #endregion

    }


    /// <summary>
    /// Defines the trigger-message-status-values.
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
