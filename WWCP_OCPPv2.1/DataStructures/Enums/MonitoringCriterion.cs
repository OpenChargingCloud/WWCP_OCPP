/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for monitoring criterions.
    /// </summary>
    public static class MonitoringCriteriaExtensions
    {

        public static IEnumerable<MonitoringCriterion> All
            => [
                   MonitoringCriterion.ThresholdMonitoring,
                   MonitoringCriterion.DeltaMonitoring,
                   MonitoringCriterion.PeriodicMonitoring
               ];

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a monitoring criterion.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring criterion.</param>
        public static MonitoringCriterion Parse(String Text)
        {

            if (TryParse(Text, out var criterion))
                return criterion;

            return MonitoringCriterion.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a monitoring criterion.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring criterion.</param>
        public static MonitoringCriterion? TryParse(String Text)
        {

            if (TryParse(Text, out var criterion))
                return criterion;

            return null;

        }

        #endregion

        #region TryParse(Text, out MonitoringCriterion)

        /// <summary>
        /// Try to parse the given text as a monitoring criterion.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring criterion.</param>
        /// <param name="MonitoringCriterion">The parsed monitoring criterion.</param>
        public static Boolean TryParse(String Text, out MonitoringCriterion MonitoringCriterion)
        {
            switch (Text.Trim().ToLower())
            {

                case "thresholdmonitoring":
                    MonitoringCriterion = MonitoringCriterion.ThresholdMonitoring;
                    return true;

                case "deltamonitoring":
                    MonitoringCriterion = MonitoringCriterion.DeltaMonitoring;
                    return true;

                case "periodicmonitoring":
                    MonitoringCriterion = MonitoringCriterion.PeriodicMonitoring;
                    return true;

                default:
                    MonitoringCriterion = MonitoringCriterion.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MonitoringCriterion)

        public static String AsText(this MonitoringCriterion MonitoringCriterion)

            => MonitoringCriterion switch {
                   MonitoringCriterion.ThresholdMonitoring  => "ThresholdMonitoring",
                   MonitoringCriterion.DeltaMonitoring      => "DeltaMonitoring",
                   MonitoringCriterion.PeriodicMonitoring   => "PeriodicMonitoring",
                   _                                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Monitoring criterion.
    /// </summary>
    public enum MonitoringCriterion
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
