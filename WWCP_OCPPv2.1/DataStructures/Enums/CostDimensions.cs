/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for the dimensions of the cost of a charging period.
    /// </summary>
    public static class CostDimensionsExtensions
    {

        #region TryParse(Text, out CostDimension)

        /// <summary>
        /// Try to parse the given text as a cost dimension - "IdleTIme" as the
        /// schema spells it, and "IdleTime".
        /// </summary>
        /// <param name="Text">A text representation of a cost dimension.</param>
        /// <param name="CostDimension">The parsed cost dimension.</param>
        public static Boolean TryParse(String Text, out CostDimensions CostDimension)
        {

            switch (Text.Trim())
            {

                case "Energy":        CostDimension = CostDimensions.Energy;        return true;
                case "MaxCurrent":    CostDimension = CostDimensions.MaxCurrent;    return true;
                case "MinCurrent":    CostDimension = CostDimensions.MinCurrent;    return true;
                case "MaxPower":      CostDimension = CostDimensions.MaxPower;      return true;
                case "MinPower":      CostDimension = CostDimensions.MinPower;      return true;
                case "IdleTIme":
                case "IdleTime":      CostDimension = CostDimensions.IdleTime;      return true;
                case "ChargingTime":  CostDimension = CostDimensions.ChargingTime;  return true;

                default:
                    CostDimension = default;
                    return false;

            }

        }

        #endregion

        #region AsText(this CostDimension)

        /// <summary>
        /// The text of the given cost dimension - "IdleTIme" as the schema spells it.
        /// </summary>
        /// <param name="CostDimension">A cost dimension.</param>
        public static String AsText(this CostDimensions CostDimension)

            => CostDimension switch {
                   CostDimensions.Energy        => "Energy",
                   CostDimensions.MaxCurrent    => "MaxCurrent",
                   CostDimensions.MinCurrent    => "MinCurrent",
                   CostDimensions.MaxPower      => "MaxPower",
                   CostDimensions.MinPower      => "MinPower",
                   CostDimensions.IdleTime      => "IdleTIme",
                   _                            => "ChargingTime"
               };

        #endregion

        #region Unit(this CostDimension)

        /// <summary>
        /// The unit the volume of the given cost dimension is in: Wh, A, W or seconds.
        /// </summary>
        /// <param name="CostDimension">A cost dimension.</param>
        public static org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure Unit(this CostDimensions CostDimension)

            => CostDimension switch {
                   CostDimensions.Energy        => org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.WattHour,
                   CostDimensions.MaxCurrent    => org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Ampere,
                   CostDimensions.MinCurrent    => org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Ampere,
                   CostDimensions.MaxPower      => org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Watt,
                   CostDimensions.MinPower      => org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Watt,
                   _                            => org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Second
               };

        #endregion

    }


    /// <summary>
    /// The dimensions of the cost of a charging period.
    /// </summary>
    public enum CostDimensions
    {

        /// <summary>
        /// The energy (dis-)charged during the charging period, in Wh - negative
        /// when more energy was fed into the grid than charged into the EV.
        /// </summary>
        Energy,

        /// <summary>
        /// The sum of the maximum current over all phases during the charging period, in A.
        /// </summary>
        MaxCurrent,

        /// <summary>
        /// The sum of the minimum current over all phases during the charging period, in A.
        /// </summary>
        MinCurrent,

        /// <summary>
        /// The maximum power during the charging period, in W.
        /// </summary>
        MaxPower,

        /// <summary>
        /// The minimum power during the charging period, in W.
        /// </summary>
        MinPower,

        /// <summary>
        /// The time not charging during the charging period, in seconds.
        /// </summary>
        IdleTime,

        /// <summary>
        /// The time charging during the charging period, in seconds.
        /// </summary>
        ChargingTime

    }

}
