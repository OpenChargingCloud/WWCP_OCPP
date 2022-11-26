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
    /// Extention methods for monitoring bases.
    /// </summary>
    public static class MonitoringBasesExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a monitoring base.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring base.</param>
        public static MonitoringBases Parse(String Text)
        {

            if (TryParse(Text, out var monitoringBase))
                return monitoringBase;

            return MonitoringBases.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a monitoring base.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring base.</param>
        public static MonitoringBases? TryParse(String Text)
        {

            if (TryParse(Text, out var monitoringBase))
                return monitoringBase;

            return null;

        }

        #endregion

        #region TryParse(Text, out MonitoringBases)

        /// <summary>
        /// Try to parse the given text as a monitoring base.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring base.</param>
        /// <param name="MonitoringBases">The parsed monitoring bases.</param>
        public static Boolean TryParse(String Text, out MonitoringBases MonitoringBases)
        {
            switch (Text.Trim())
            {

                case "All":
                    MonitoringBases = MonitoringBases.All;
                    return true;

                case "FactoryDefault":
                    MonitoringBases = MonitoringBases.FactoryDefault;
                    return true;

                case "HardWiredOnly":
                    MonitoringBases = MonitoringBases.HardWiredOnly;
                    return true;

                default:
                    MonitoringBases = MonitoringBases.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MonitoringBases)

        public static String AsText(this MonitoringBases MonitoringBases)

            => MonitoringBases switch {
                   MonitoringBases.All             => "All",
                   MonitoringBases.FactoryDefault  => "FactoryDefault",
                   MonitoringBases.HardWiredOnly   => "HardWiredOnly",
                   _                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Monitoring bases.
    /// </summary>
    public enum MonitoringBases
    {

        /// <summary>
        /// Unknown monitoring base.
        /// </summary>
        Unknown,

        /// <summary>
        /// Activate all pre-configured monitors.
        /// </summary>
        All,

        /// <summary>
        /// Activate the default monitoring settings as recommended by the manufacturer.
        /// This is a subset of all preconfigured monitors.
        /// </summary>
        FactoryDefault,

        /// <summary>
        /// Clears all custom monitors and disables all pre-configured monitors.
        /// </summary>
        HardWiredOnly

    }

}
