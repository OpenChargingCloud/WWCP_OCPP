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
    /// Extention methods for component criteria.
    /// </summary>
    public static class ComponentCriteriaExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a component criterium.
        /// </summary>
        /// <param name="Text">A text representation of a component criterium.</param>
        public static ComponentCriteria Parse(String Text)
        {

            if (TryParse(Text, out var criterium))
                return criterium;

            return ComponentCriteria.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a component criterium.
        /// </summary>
        /// <param name="Text">A text representation of a component criterium.</param>
        public static ComponentCriteria? TryParse(String Text)
        {

            if (TryParse(Text, out var criterium))
                return criterium;

            return null;

        }

        #endregion

        #region TryParse(Text, out Criterium)

        /// <summary>
        /// Try to parse the given text as a component criterium.
        /// </summary>
        /// <param name="Text">A text representation of a component criterium.</param>
        /// <param name="Criterium">The parsed component criterium.</param>
        public static Boolean TryParse(String Text, out ComponentCriteria Criterium)
        {
            switch (Text.Trim())
            {

                case "Active":
                    Criterium = ComponentCriteria.Active;
                    return true;

                case "Available":
                    Criterium = ComponentCriteria.Available;
                    return true;

                case "Enabled":
                    Criterium = ComponentCriteria.Enabled;
                    return true;

                case "Problem":
                    Criterium = ComponentCriteria.Problem;
                    return true;

                default:
                    Criterium = ComponentCriteria.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this ComponentCriteria Criterium)

            => Criterium switch {
                   ComponentCriteria.Active     => "Active",
                   ComponentCriteria.Available  => "Available",
                   ComponentCriteria.Enabled    => "Enabled",
                   ComponentCriteria.Problem    => "Problem",
                   _                            => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Component criteria.
    /// </summary>
    public enum ComponentCriteria
    {

        /// <summary>
        /// Unknown component criteria.
        /// </summary>
        Unknown,

        /// <summary>
        /// Components that are active, i.e. having Active = 1.
        /// </summary>
        Active,

        /// <summary>
        /// Components that are available, i.e. having Available = 1.
        /// </summary>
        Available,

        /// <summary>
        /// Components that are enabled, i.e. having Enabled = 1.
        /// </summary>
        Enabled,

        /// <summary>
        /// Components that reported a problem, i.e. having Problem = 1.
        /// </summary>
        Problem

    }

}
