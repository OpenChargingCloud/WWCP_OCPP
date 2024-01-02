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
    /// Extensions methods for preconditioning status.
    /// </summary>
    public static class PreconditioningStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a preconditioning status.
        /// </summary>
        /// <param name="Text">A text representation of a preconditioning status.</param>
        public static PreconditioningStatus Parse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return PreconditioningStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a preconditioning status.
        /// </summary>
        /// <param name="Text">A text representation of a preconditioning status.</param>
        public static PreconditioningStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return null;

        }

        #endregion

        #region TryParse(Text, out PreconditioningStatus)

        /// <summary>
        /// Try to parse the given text as a preconditioning status.
        /// </summary>
        /// <param name="Text">A text representation of a preconditioning status.</param>
        /// <param name="PreconditioningStatus">The parsed preconditioning status.</param>
        public static Boolean TryParse(String Text, out PreconditioningStatus PreconditioningStatus)
        {
            switch (Text.Trim())
            {

                case "Ready":
                    PreconditioningStatus = PreconditioningStatus.Ready;
                    return true;

                case "Preconditioning":
                    PreconditioningStatus = PreconditioningStatus.Preconditioning;
                    return true;

                case "NotReady":
                    PreconditioningStatus = PreconditioningStatus.NotReady;
                    return true;

                default:
                    PreconditioningStatus = PreconditioningStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this PreconditioningStatus)

        /// <summary>
        /// Return a string representation of the given preconditioning status.
        /// </summary>
        /// <param name="PreconditioningStatus">A preconditioning status.</param>
        public static String AsText(this PreconditioningStatus PreconditioningStatus)

            => PreconditioningStatus switch {
                   PreconditioningStatus.Ready            => "Ready",
                   PreconditioningStatus.Preconditioning  => "Preconditioning",
                   PreconditioningStatus.NotReady         => "NotReady",
                   _                                      => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Pricing types.
    /// </summary>
    public enum PreconditioningStatus
    {

        /// <summary>
        /// No information available on the status of preconditioning.
        /// </summary>
        Unknown,

        /// <summary>
        /// The battery is preconditioned and ready to react directly on a
        /// given setpoint for charging (and discharging when available).
        /// </summary>
        Ready,

        /// <summary>
        /// Busy with preconditioning the BMS. When done will move to status Ready.
        /// </summary>
        Preconditioning,

        /// <summary>
        /// The battery is not preconditioned and not able to directly react to given setpoint.
        /// </summary>
        NotReady

    }

}
