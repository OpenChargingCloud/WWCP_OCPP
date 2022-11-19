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
    /// Extentions methods for report bases.
    /// </summary>
    public static class ReportBasesExtentions
    {

        #region Parse(Text)

        public static ReportBases Parse(String Text)

            => Text.Trim() switch {
                   "ConfigurationInventory"  => ReportBases.ConfigurationInventory,
                   "FullInventory"           => ReportBases.FullInventory,
                   "SummaryInventory"        => ReportBases.SummaryInventory,
                   _                         => ReportBases.Unknown
               };

        public static Boolean TryParse(String Text, out ReportBases Action)
        {
            switch (Text.Trim())
            {

                case "ConfigurationInventory":
                    Action = ReportBases.ConfigurationInventory;
                    return true;

                case "FullInventory":
                    Action = ReportBases.FullInventory;
                    return true;

                case "SummaryInventory":
                    Action = ReportBases.SummaryInventory;
                    return true;

                default:
                    Action = ReportBases.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText(this ReportBases)

        public static String AsText(this ReportBases ReportBases)

            => ReportBases switch {
                   ReportBases.ConfigurationInventory  => "ConfigurationInventory",
                   ReportBases.FullInventory           => "FullInventory",
                   ReportBases.SummaryInventory        => "SummaryInventory",
                   _                                   => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// A report base.
    /// </summary>
    public enum ReportBases
    {

        /// <summary>
        /// Unknown report bases.
        /// </summary>
        Unknown,

        /// <summary>
        /// A (configuration) report that lists all components/variables that can be set by the operator.
        /// </summary>
        ConfigurationInventory,

        /// <summary>
        /// A (full) report that lists everything except monitoring settings.
        /// </summary>
        FullInventory,

        /// <summary>
        /// A (summary) report that lists components/variables relating to the charging station’s current charging availability, and to any existing problem conditions.
        /// </summary>
        SummaryInventory

    }

}
