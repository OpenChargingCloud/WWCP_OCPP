/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A limit at a state-of-charge.
    /// </summary>
    public class LimitAtSoC : ACustomData,
                              IEquatable<LimitAtSoC>
    {

        #region Properties

        /// <summary>
        /// The state-of-charge value beyond which the charging rate limit should be applied.
        /// </summary>
        [Mandatory]
        public PercentageByte     SoC      { get; }

        /// <summary>
        /// The charging rate limit beyond the state-of-charge value.
        /// The unit is defined by _chargingSchedule.chargingRateUnit_ (Watt or Ampere).
        /// </summary>
        [Mandatory]
        public ChargingRateValue  Limit    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new limit at a state-of-charge.
        /// </summary>
        /// <param name="SoC">An state-of-charge value beyond which the charging rate limit should be applied.</param>
        /// <param name="Limit">The charging rate limit beyond the state-of-charge. The unit is defined by _chargingSchedule.chargingRateUnit_ (Watt or Ampere).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public LimitAtSoC(PercentageByte     SoC,
                          ChargingRateValue  Limit,
                          CustomData?        CustomData   = null)

            : base(CustomData)

        {

            this.SoC    = SoC;
            this.Limit  = Limit;

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "LimitAtSoC",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "soc": {
        //             "description": "The SoC value beyond which the charging rate limit should be applied.",
        //             "type": "integer",
        //             "minimum": 0.0,
        //             "maximum": 100.0
        //         },
        //         "limit": {
        //             "description": "Charging rate limit beyond the SoC value.\r\nThe unit is defined by _chargingSchedule.chargingRateUnit_.",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "soc",
        //         "limit"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomLimitBeyondSoCParser = null)

        /// <summary>
        /// Parse the given JSON representation of a limit beyond state-of-charge.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomLimitBeyondSoCParser">An optional delegate to parse custom sales tariff entries.</param>
        public static LimitAtSoC Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<LimitAtSoC>?  CustomLimitBeyondSoCParser   = null)
        {

            if (TryParse(JSON,
                         out var limitBeyondSoC,
                         out var errorResponse,
                         CustomLimitBeyondSoCParser))
            {
                return limitBeyondSoC;
            }

            throw new ArgumentException("The given JSON representation of a limit beyond state-of-charge is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(LimitBeyondSoCJSON, out LimitBeyondSoC, out ErrorResponse, CustomLimitBeyondSoCParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a limit beyond state-of-charge.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LimitBeyondSoC">The parsed connector type.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out LimitAtSoC?  LimitBeyondSoC,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

            => TryParse(JSON,
                        out LimitBeyondSoC,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a limit beyond state-of-charge.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LimitBeyondSoC">The parsed connector type.</param>
        /// <param name="CustomLimitBeyondSoCParser">An optional delegate to parse custom sales tariff entries.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out LimitAtSoC?      LimitBeyondSoC,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<LimitAtSoC>?  CustomLimitBeyondSoCParser   = null)
        {

            try
            {

                LimitBeyondSoC = default;

                #region StateOfCharge        [mandatory]

                if (!JSON.ParseMandatory("soc",
                                         "state-of-charge",
                                         out PercentageByte StateOfCharge,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingRateLimit    [mandatory]

                if (!JSON.ParseMandatory("limit",
                                         "charging rate limit",
                                         out ChargingRateValue ChargingRateLimit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                LimitBeyondSoC = new LimitAtSoC(
                                     StateOfCharge,
                                     ChargingRateLimit,
                                     CustomData
                                 );

                if (CustomLimitBeyondSoCParser is not null)
                    LimitBeyondSoC = CustomLimitBeyondSoCParser(JSON,
                                                                LimitBeyondSoC);

                return true;

            }
            catch (Exception e)
            {
                LimitBeyondSoC  = default;
                ErrorResponse   = "The given JSON representation of a limit beyond state-of-charge is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLimitAtSoCSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLimitAtSoCSerializer">A delegate to serialize custom limitBeyondSoCs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LimitAtSoC>?  CustomLimitAtSoCSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("soc",          SoC.  Value),
                                 new JProperty("limit",        Limit.Value),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomLimitAtSoCSerializer is not null
                       ? CustomLimitAtSoCSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (LimitBeyondSoC1, LimitBeyondSoC2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LimitBeyondSoC1">A limit beyond state-of-charge.</param>
        /// <param name="LimitBeyondSoC2">Another limit beyond state-of-charge.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LimitAtSoC? LimitBeyondSoC1,
                                           LimitAtSoC? LimitBeyondSoC2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(LimitBeyondSoC1, LimitBeyondSoC2))
                return true;

            // If one is null, but not both, return false.
            if (LimitBeyondSoC1 is null || LimitBeyondSoC2 is null)
                return false;

            return LimitBeyondSoC1.Equals(LimitBeyondSoC2);

        }

        #endregion

        #region Operator != (LimitBeyondSoC1, LimitBeyondSoC2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LimitBeyondSoC1">A limit beyond state-of-charge.</param>
        /// <param name="LimitBeyondSoC2">Another limit beyond state-of-charge.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LimitAtSoC? LimitBeyondSoC1,
                                           LimitAtSoC? LimitBeyondSoC2)

            => !(LimitBeyondSoC1 == LimitBeyondSoC2);

        #endregion

        #endregion

        #region IEquatable<LimitBeyondSoC> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two limit beyond a state-of-charge values for equality.
        /// </summary>
        /// <param name="Object">A limit beyond state-of-charge to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LimitAtSoC limitBeyondSoC &&
                   Equals(limitBeyondSoC);

        #endregion

        #region Equals(LimitBeyondSoC)

        /// <summary>
        /// Compares two limit beyond a state-of-charge values for equality.
        /// </summary>
        /// <param name="LimitBeyondSoC">A limit beyond state-of-charge to compare with.</param>
        public Boolean Equals(LimitAtSoC? LimitBeyondSoC)

            => LimitBeyondSoC is not null &&

               SoC.    Equals(LimitBeyondSoC.SoC)     &&
               Limit.Equals(LimitBeyondSoC.Limit) &&

               base.             Equals(LimitBeyondSoC);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return SoC.  GetHashCode() * 5 ^
                       Limit.GetHashCode() * 3 ^
                       base. GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Limit} after {SoC} state-of-charge";

        #endregion

    }

}
