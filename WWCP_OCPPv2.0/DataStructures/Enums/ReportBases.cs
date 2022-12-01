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
    /// Extensions methods for report bases.
    /// </summary>
    public static class ReportBasesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a report base.
        /// </summary>
        /// <param name="Text">A text representation of a report base.</param>
        public static ReportBases Parse(String Text)
        {

            if (TryParse(Text, out var reportBase))
                return reportBase;

            return ReportBases.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a report base.
        /// </summary>
        /// <param name="Text">A text representation of a report base.</param>
        public static ReportBases? TryParse(String Text)
        {

            if (TryParse(Text, out var reportBase))
                return reportBase;

            return null;

        }

        #endregion

        #region TryParse(Text, out ReportBase)

        /// <summary>
        /// Try to parse the given text as a report base.
        /// </summary>
        /// <param name="Text">A text representation of a report base.</param>
        /// <param name="ReportBase">The parsed report base.</param>
        public static Boolean TryParse(String Text, out ReportBases ReportBase)
        {
            switch (Text.Trim())
            {

                case "ConfigurationInventory":
                    ReportBase = ReportBases.ConfigurationInventory;
                    return true;

                case "FullInventory":
                    ReportBase = ReportBases.FullInventory;
                    return true;

                case "SummaryInventory":
                    ReportBase = ReportBases.SummaryInventory;
                    return true;

                default:
                    ReportBase = ReportBases.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ReportBase)

        public static String AsText(this ReportBases ReportBase)

            => ReportBase switch {
                   ReportBases.ConfigurationInventory  => "ConfigurationInventory",
                   ReportBases.FullInventory           => "FullInventory",
                   ReportBases.SummaryInventory        => "SummaryInventory",
                   _                                   => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Report bases.
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
