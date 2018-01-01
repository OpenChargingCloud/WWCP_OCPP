/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// Allowable values of the optional "unit" field of a Value element,
    /// as used in the MeterValues request and StopTransaction request
    /// messages. Default value of "unit" is always "Wh".
    /// </summary>
    public enum UnitsOfMeasure
    {

        /// <summary>
        /// Degrees (temperature).
        /// </summary>
        Celsius,

        /// <summary>
        /// Degrees (temperature).
        /// </summary>
        Fahrenheit,

        /// <summary>
        /// Watt-hours (energy).
        /// </summary>
        Wh,

        /// <summary>
        /// kiloWatt-hours (energy).
        /// </summary>
        kWh,

        /// <summary>
        /// Var-hours (reactive energy).
        /// </summary>
        varh,

        /// <summary>
        /// kilovar-hours (reactive energy).
        /// </summary>
        kvarh,

        /// <summary>
        /// Watts (power).
        /// </summary>
        Watts,

        /// <summary>
        /// kiloWatts (power).
        /// </summary>
        kW,

        /// <summary>
        /// VoltAmpere (apparent power).
        /// </summary>
        VoltAmpere,

        /// <summary>
        /// kiloVolt Ampere (apparent power).
        /// </summary>
        kVA,

        /// <summary>
        /// Vars (reactive power).
        /// </summary>
        var,

        /// <summary>
        /// kilovars (reactive power).
        /// </summary>
        kvar,

        /// <summary>
        /// Amperes (current).
        /// </summary>
        Amperes,

        /// <summary>
        /// Voltage (r.m.s. AC).
        /// </summary>
        Voltage,

        /// <summary>
        /// Degrees Kelvin (temperature).
        /// </summary>
        Kelvin,

        /// <summary>
        /// Percentage.
        /// </summary>
        Percent


    }

}
