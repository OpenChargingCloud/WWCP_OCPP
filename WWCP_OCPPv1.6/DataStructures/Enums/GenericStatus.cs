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

    /// <summary>
    /// Extentions methods for the generic status.
    /// </summary>
    public static class GenericStatusExtentions
    {

        #region Parse(Text)

        public static GenericStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => GenericStatus.Accepted,
                   "Rejected"  => GenericStatus.Rejected,
                   _           => GenericStatus.Unknown
               };

        #endregion

        #region AsText(this GenericStatus)

        public static String AsText(this GenericStatus GenericStatus)

            => GenericStatus switch {
                   GenericStatus.Accepted  => "Accepted",
                   GenericStatus.Rejected  => "Rejected",
                   _                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Generic message response status.
    /// </summary>
    public enum GenericStatus
    {

        /// <summary>
        /// Unknown generic status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Request has not been accepted and will not be executed.
        /// </summary>
        Rejected

    }

}
