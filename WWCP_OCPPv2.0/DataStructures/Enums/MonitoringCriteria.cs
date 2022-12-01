/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for monitoring criteria.
    /// </summary>
    public static class MonitoringCriteriaExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a monitoring criterium.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring criterium.</param>
        public static MonitoringCriteria Parse(String Text)
        {

            if (TryParse(Text, out var criterium))
                return criterium;

            return MonitoringCriteria.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a monitoring criterium.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring criterium.</param>
        public static MonitoringCriteria? TryParse(String Text)
        {

            if (TryParse(Text, out var criterium))
                return criterium;

            return null;

        }

        #endregion

        #region TryParse(Text, out MonitoringCriterium)

        /// <summary>
        /// Try to parse the given text as a monitoring criterium.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring criterium.</param>
        /// <param name="MonitoringCriterium">The parsed monitoring criterium.</param>
        public static Boolean TryParse(String Text, out MonitoringCriteria MonitoringCriterium)
        {
            switch (Text.Trim())
            {

                case "ThresholdMonitoring":
                    MonitoringCriterium = MonitoringCriteria.ThresholdMonitoring;
                    return true;

                case "DeltaMonitoring":
                    MonitoringCriterium = MonitoringCriteria.DeltaMonitoring;
                    return true;

                case "PeriodicMonitoring":
                    MonitoringCriterium = MonitoringCriteria.PeriodicMonitoring;
                    return true;

                default:
                    MonitoringCriterium = MonitoringCriteria.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MonitoringCriterium)

        public static String AsText(this MonitoringCriteria MonitoringCriterium)

            => MonitoringCriterium switch {
                   MonitoringCriteria.ThresholdMonitoring  => "ThresholdMonitoring",
                   MonitoringCriteria.DeltaMonitoring      => "DeltaMonitoring",
                   MonitoringCriteria.PeriodicMonitoring   => "PeriodicMonitoring",
                   _                                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Monitoring criteria.
    /// </summary>
    public enum MonitoringCriteria
    {

        /// <summary>
        /// Unknown monitoring criteria.
        /// </summary>
        Unknown,

        /// <summary>
        /// Report variables and components with a monitor of type UpperThreshold or LowerThreshold.
        /// </summary>
        ThresholdMonitoring,

        /// <summary>
        /// Report variables and components with a monitor of type Delta.
        /// </summary>
        DeltaMonitoring,

        /// <summary>
        /// Report variables and components with a monitor of type Periodic or PeriodicClockAligned.
        /// </summary>
        PeriodicMonitoring

    }

}
