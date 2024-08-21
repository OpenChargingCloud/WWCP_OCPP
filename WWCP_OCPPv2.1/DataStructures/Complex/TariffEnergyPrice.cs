/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The prices within an energy tariff element.
    /// </summary>
    public class TariffEnergyPrice
    {

        #region Properties

        /// <summary>
        /// The price per kWh (excl. tax) for this tariff element.
        /// </summary>
        [Mandatory]
        public Decimal            PriceKWh      { get; }

        /// <summary>
        /// The optional step size.
        /// When absent, the exact amount is billed.
        /// When present, this type is billed in blocks of stepSize of the base unit: Wh.
        /// Amounts are rounded up to a multiple of stepSize.
        /// </summary>
        /// <example>
        /// When the step_size is 1000, then energy will be billed in blocks of 1 kWh.
        /// So if 1005 Wh was charged 2 kWh (2 blocks of step_size) will be billed.
        /// </example>
        [Optional]
        public WattHour?          StepSize      { get; }

        /// <summary>
        /// The optional enumeration of tariff conditions
        /// </summary>
        [Optional]
        public TariffConditions?  Conditions    { get;  }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new prices for an energy tariff element.
        /// </summary>
        /// <param name="PriceKWh">A price per kWh (excl. tax) for this TariffEnergyPrice.</param>
        /// <param name="StepSize">An optional Wh step size.</param>
        /// <param name="Conditions">Optional tariff conditions.</param>
        public TariffEnergyPrice(Decimal            PriceKWh,
                                 WattHour?          StepSize     = null,
                                 TariffConditions?  Conditions   = null)
        {

            this.PriceKWh    = PriceKWh;
            this.StepSize    = StepSize;
            this.Conditions  = Conditions;

            unchecked
            {

                hashCode = this.PriceKWh.   GetHashCode()       * 5 ^
                          (this.StepSize?.  GetHashCode() ?? 0) * 3 ^
                          (this.Conditions?.GetHashCode() ?? 0);

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description":          "Tariff with optional conditions for an energy price.",
        //     "javaType":             "TariffEnergyPrice",
        //     "type":                 "object",
        //     "additionalProperties":  false,
        //     "properties": {
        //         "priceKwh": {
        //             "description":  "Price per kWh (excl. tax) for this element.",
        //             "type":         "number"
        //         },
        //         "stepSize": {
        //             "description":  "When absent, the exact amount is billed.
        //                              When present, this type is billed in blocks of _stepSize_ of the base unit: Wh.
        //                              Amounts are rounded up to a multiple of _stepSize_.",
        //             "type":         "integer"
        //         },
        //         "conditions": {
        //             "$ref":         "#/definitions/TariffConditionsType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffEnergyPricesParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TariffEnergyPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffEnergyPriceParser">An optional delegate to parse custom TariffEnergyPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static TariffEnergyPrice Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceParser   = null,
                                              CustomJObjectParserDelegate<TariffConditions>?   CustomTariffConditionsParser    = null)
        {

            if (TryParse(JSON,
                         out var tariffEnergyPrice,
                         out var errorResponse,
                         CustomTariffEnergyPriceParser,
                         CustomTariffConditionsParser) &&
                tariffEnergyPrice is not null)
            {
                return tariffEnergyPrice;
            }

            throw new ArgumentException("The given JSON representation of a tariff energy price is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffEnergyPrices, out ErrorResponse, CustomTariffEnergyPricesParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a TariffEnergyPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffEnergyPrice">The parsed TariffEnergyPrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out TariffEnergyPrice?  TariffEnergyPrice,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out TariffEnergyPrice,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a TariffEnergyPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffEnergyPrice">The parsed TariffEnergyPrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffEnergyPriceParser">An optional delegate to parse custom TariffEnergyPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out TariffEnergyPrice?      TariffEnergyPrice,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceParser   = null,
                                       CustomJObjectParserDelegate<TariffConditions>?   CustomTariffConditionsParser    = null)
        {

            try
            {

                TariffEnergyPrice = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PriceKWh      [mandatory]

                if (!JSON.ParseMandatory("priceKwh",
                                         "price per KWh",
                                         out Decimal PriceKWh,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse StepSize      [optional]

                if (!JSON.ParseOptional("stepSize",
                                        "price components",
                                        WattHour.TryParse,
                                        out WattHour? StepSize,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Conditions    [optional]

                if (JSON.ParseOptionalJSON("conditions",
                                           "tariff conditions",
                                           TariffConditions.TryParse,
                                           out TariffConditions? Conditions,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TariffEnergyPrice = new TariffEnergyPrice(
                                        PriceKWh,
                                        StepSize,
                                        Conditions
                                    );


                if (CustomTariffEnergyPriceParser is not null)
                    TariffEnergyPrice = CustomTariffEnergyPriceParser(JSON,
                                                                      TariffEnergyPrice);

                return true;

            }
            catch (Exception e)
            {
                TariffEnergyPrice  = default;
                ErrorResponse       = "The given JSON representation of a tariff element is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffEnergyPriceSerializer = null, CustomTariffConditionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffEnergyPriceSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffConditionsSerializer    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priceKwh",     PriceKWh),

                           StepSize.HasValue
                               ? new JProperty("stepSize",     Convert.ToInt32(StepSize.Value.Value))
                               : null,

                           Conditions is not null
                               ? new JProperty("conditions",   Conditions.ToJSON(CustomTariffConditionsSerializer))
                               : null

                       );

            return CustomTariffEnergyPriceSerializer is not null
                       ? CustomTariffEnergyPriceSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TariffEnergyPrice Clone()

            => new (
                   PriceKWh,
                   StepSize,
                   Conditions?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffEnergyPrices1, TariffEnergyPrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffEnergyPrices1">A tariff energy price.</param>
        /// <param name="TariffEnergyPrices2">Another tariff energy price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffEnergyPrice? TariffEnergyPrices1,
                                           TariffEnergyPrice? TariffEnergyPrices2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffEnergyPrices1, TariffEnergyPrices2))
                return true;

            // If one is null, but not both, return false.
            if (TariffEnergyPrices1 is null || TariffEnergyPrices2 is null)
                return false;

            return TariffEnergyPrices1.Equals(TariffEnergyPrices2);

        }

        #endregion

        #region Operator != (TariffEnergyPrices1, TariffEnergyPrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffEnergyPrices1">A tariff energy price.</param>
        /// <param name="TariffEnergyPrices2">Another tariff energy price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffEnergyPrice? TariffEnergyPrices1,
                                           TariffEnergyPrice? TariffEnergyPrices2)

            => !(TariffEnergyPrices1 == TariffEnergyPrices2);

        #endregion

        #endregion

        #region IEquatable<TariffEnergyPrices> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff energy prices for equality.
        /// </summary>
        /// <param name="Object">A tariff energy price to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffEnergyPrice tariffEnergyPrice &&
                   Equals(tariffEnergyPrice);

        #endregion

        #region Equals(TariffEnergyPrices)

        /// <summary>
        /// Compares two tariff energy prices for equality.
        /// </summary>
        /// <param name="TariffEnergyPrice">A tariff energy price to compare with.</param>
        public Boolean Equals(TariffEnergyPrice? TariffEnergyPrice)

            => TariffEnergyPrice is not null &&

               PriceKWh.Equals(TariffEnergyPrice.PriceKWh) &&

            ((!StepSize.HasValue      && !TariffEnergyPrice.StepSize.HasValue)      ||
              (StepSize.HasValue      &&  TariffEnergyPrice.StepSize.HasValue      && StepSize.Value.Equals(TariffEnergyPrice.StepSize.Value))) &&

             ((Conditions is     null &&  TariffEnergyPrice.Conditions is     null) ||
              (Conditions is not null &&  TariffEnergyPrice.Conditions is not null && Conditions.    Equals(TariffEnergyPrice.Conditions)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{PriceKWh} €/kWh{(StepSize is not null ? $", per {StepSize.Value} Wh" : "")}{(Conditions is not null ? $", with '{Conditions}'" : "")}!";

        #endregion

    }

}
