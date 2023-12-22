/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A limit beyond a state-of-charge value.
    /// </summary>
    public class LimitBeyondSoC : ACustomData,
                                  IEquatable<LimitBeyondSoC>
    {

        #region Properties

        /// <summary>
        /// The state-of-charge value beyond which the charging rate limit should be applied.
        /// </summary>
        [Mandatory]
        public PercentageByte      StateOfCharge        { get; }

        /// <summary>
        /// The charging rate limit beyond the state-of-charge value (Watt or Ampere).
        /// </summary>
        [Mandatory]
        public ChargingRateValue  ChargingRateLimit    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new limit beyond state-of-charge.
        /// </summary>
        /// <param name="StateOfCharge">An state-of-charge value beyond which the charging rate limit should be applied.</param>
        /// <param name="ChargingRateLimit">The charging rate limit beyond the state-of-charge value (in chargingRateUnit).</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public LimitBeyondSoC(PercentageByte      StateOfCharge,
                              ChargingRateValue  ChargingRateLimit,
                              CustomData?        CustomData   = null)

            : base(CustomData)

        {

            this.StateOfCharge      = StateOfCharge;
            this.ChargingRateLimit  = ChargingRateLimit;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomLimitBeyondSoCParser = null)

        /// <summary>
        /// Parse the given JSON representation of a limit beyond state-of-charge.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomLimitBeyondSoCParser">A delegate to parse custom sales tariff entries.</param>
        public static LimitBeyondSoC Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<LimitBeyondSoC>?  CustomLimitBeyondSoCParser   = null)
        {

            if (TryParse(JSON,
                         out var limitBeyondSoC,
                         out var errorResponse,
                         CustomLimitBeyondSoCParser) &&
                limitBeyondSoC is not null)
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
        public static Boolean TryParse(JObject              JSON,
                                       out LimitBeyondSoC?  LimitBeyondSoC,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out LimitBeyondSoC,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a limit beyond state-of-charge.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LimitBeyondSoC">The parsed connector type.</param>
        /// <param name="CustomLimitBeyondSoCParser">A delegate to parse custom sales tariff entries.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out LimitBeyondSoC?                           LimitBeyondSoC,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<LimitBeyondSoC>?  CustomLimitBeyondSoCParser   = null)
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
                                         out Decimal chargingRateLimit,
                                         out ErrorResponse))
                {
                    return false;
                }

                var ChargingRateLimit = ChargingRateValue.Parse(chargingRateLimit,
                                                                ChargingRateUnits.Unknown);

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                LimitBeyondSoC = new LimitBeyondSoC(
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

        #region ToJSON(CustomLimitBeyondSoCSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLimitBeyondSoCSerializer">A delegate to serialize custom limitBeyondSoCs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LimitBeyondSoC>?  CustomLimitBeyondSoCSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("soc",          StateOfCharge.    Value),
                                 new JProperty("limit",        ChargingRateLimit.Value),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomLimitBeyondSoCSerializer is not null
                       ? CustomLimitBeyondSoCSerializer(this, json)
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
        public static Boolean operator == (LimitBeyondSoC? LimitBeyondSoC1,
                                           LimitBeyondSoC? LimitBeyondSoC2)
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
        public static Boolean operator != (LimitBeyondSoC? LimitBeyondSoC1,
                                           LimitBeyondSoC? LimitBeyondSoC2)

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

            => Object is LimitBeyondSoC limitBeyondSoC &&
                   Equals(limitBeyondSoC);

        #endregion

        #region Equals(LimitBeyondSoC)

        /// <summary>
        /// Compares two limit beyond a state-of-charge values for equality.
        /// </summary>
        /// <param name="LimitBeyondSoC">A limit beyond state-of-charge to compare with.</param>
        public Boolean Equals(LimitBeyondSoC? LimitBeyondSoC)

            => LimitBeyondSoC is not null &&

               StateOfCharge.    Equals(LimitBeyondSoC.StateOfCharge)     &&
               ChargingRateLimit.Equals(LimitBeyondSoC.ChargingRateLimit) &&

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

                return StateOfCharge.    GetHashCode() * 5 ^
                       ChargingRateLimit.GetHashCode() * 3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{ChargingRateLimit} after {StateOfCharge} state-of-charge";

        #endregion

    }

}
