/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    public static class UnitsOfMeasureExtentions
    {

        #region AsUnitsOfMeasure(Text)

        public static UnitsOfMeasure Parse(this String Text)
        {

            switch (Text)
            {

                case "Celsius":
                    return UnitsOfMeasure.Celsius;

                case "Fahrenheit":
                    return UnitsOfMeasure.Fahrenheit;

                case "kWh":
                    return UnitsOfMeasure.kWh;

                case "varh":
                    return UnitsOfMeasure.varh;

                case "kvarh":
                    return UnitsOfMeasure.kvarh;

                case "W":
                    return UnitsOfMeasure.Watts;

                case "kW":
                    return UnitsOfMeasure.kW;

                case "VA":
                    return UnitsOfMeasure.VoltAmpere;

                case "kVA":
                    return UnitsOfMeasure.kVA;

                case "var":
                    return UnitsOfMeasure.var;

                case "kvar":
                    return UnitsOfMeasure.kvar;

                case "A":
                    return UnitsOfMeasure.Amperes;

                case "V":
                    return UnitsOfMeasure.Voltage;

                case "K":
                    return UnitsOfMeasure.Kelvin;

                case "Percent":
                    return UnitsOfMeasure.Percent;


                default:
                    return UnitsOfMeasure.Wh;

            }

        }

        #endregion

        #region AsText(this UnitOfMeasure)

        public static String AsText(this UnitsOfMeasure UnitOfMeasure)
        {

            switch (UnitOfMeasure)
            {

                case UnitsOfMeasure.Celsius:
                    return "Celsius";

                case UnitsOfMeasure.Fahrenheit:
                    return "Fahrenheit";

                case UnitsOfMeasure.kWh:
                    return "kWh";

                case UnitsOfMeasure.varh:
                    return "varh";

                case UnitsOfMeasure.kvarh:
                    return "kvarh";

                case UnitsOfMeasure.Watts:
                    return "W";

                case UnitsOfMeasure.kW:
                    return "kW";

                case UnitsOfMeasure.VoltAmpere:
                    return "VA";

                case UnitsOfMeasure.kVA:
                    return "kVA";

                case UnitsOfMeasure.var:
                    return "var";

                case UnitsOfMeasure.kvar:
                    return "kvar";

                case UnitsOfMeasure.Amperes:
                    return "A";

                case UnitsOfMeasure.Voltage:
                    return "V";

                case UnitsOfMeasure.Kelvin:
                    return "K";

                case UnitsOfMeasure.Percent:
                    return "Percent";


                default:
                    return "Wh";

            }

        }

        #endregion

    }


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
