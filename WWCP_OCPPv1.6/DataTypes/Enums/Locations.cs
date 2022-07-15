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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extention methods for locations.
    /// </summary>
    public static class LocationExtentions
    {

        #region Parse(Text)

        public static Locations Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Body":
                    return Locations.Body;

                case "Cable":
                    return Locations.Cable;

                case "EV":
                    return Locations.EV;

                case "Inlet":
                    return Locations.Inlet;


                default:
                    return Locations.Outlet;

            }

        }

        #endregion

        #region AsText(this Location)

        public static String AsText(this Locations Location)
        {

            switch (Location)
            {

                case Locations.Body:
                    return "Body";

                case Locations.Cable:
                    return "Cable";

                case Locations.EV:
                    return "EV";

                case Locations.Inlet:
                    return "Inlet";


                default:
                    return "Outlet";

            }

        }

        #endregion

    }


    /// <summary>
    /// Allowable values of the optional "location" field of
    /// a value element in SampledValue.
    /// </summary>
    public enum Locations
    {

        /// <summary>
        /// Body.
        /// </summary>
        Body,

        /// <summary>
        /// Cable.
        /// </summary>
        Cable,

        /// <summary>
        /// EV.
        /// </summary>
        EV,

        /// <summary>
        /// Inlet.
        /// </summary>
        Inlet,

        /// <summary>
        /// Outlet.
        /// </summary>
        Outlet


    }

}
