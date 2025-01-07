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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The prices within an fixed tariff element.
    /// </summary>
    public class TariffFixedPrice
    {

        #region Properties

        /// <summary>
        /// The fixed price (excl. tax) for this tariff element.
        /// </summary>
        [Mandatory]
        public Decimal            PriceFixed    { get; }

        /// <summary>
        /// The optional enumeration of tariff conditions
        /// </summary>
        [Optional]
        public TariffConditions?  Conditions    { get;  }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new prices for an fixed tariff element.
        /// </summary>
        /// <param name="PriceFixed">A fixed price (excl. tax) for this TariffFixedPrice.</param>
        /// <param name="Conditions">Optional tariff conditions.</param>
        public TariffFixedPrice(Decimal            PriceFixed,
                                TariffConditions?  Conditions   = null)
        {

            this.PriceFixed  = PriceFixed;
            this.Conditions  = Conditions;

            unchecked
            {

                hashCode = this.PriceFixed. GetHashCode() * 3 ^
                          (this.Conditions?.GetHashCode() ?? 0);

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description":          "Tariff with optional conditions for a fixed price.",
        //     "javaType":             "TariffFixedPrice",
        //     "type":                 "object",
        //     "additionalProperties":  false,
        //     "properties": {
        //         "conditions": {
        //             "$ref":         "#/definitions/TariffConditionsType"
        //         },
        //         "priceFixed": {
        //             "description":  "Fixed price for this element e.g. a start fee.",
        //             "type":         "number"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffFixedPricesParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TariffFixedPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffFixedPriceParser">An optional delegate to parse custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static TariffFixedPrice Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<TariffFixedPrice>?  CustomTariffFixedPriceParser   = null,
                                             CustomJObjectParserDelegate<TariffConditions>?  CustomTariffConditionsParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffFixedPrice,
                         out var errorResponse,
                         CustomTariffFixedPriceParser,
                         CustomTariffConditionsParser) &&
                tariffFixedPrice is not null)
            {
                return tariffFixedPrice;
            }

            throw new ArgumentException("The given JSON representation of a tariff fixed price is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffFixedPrices, out ErrorResponse, CustomTariffFixedPricesParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a TariffFixedPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffFixedPrices">The parsed TariffFixedPrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out TariffFixedPrice?  TariffFixedPrices,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out TariffFixedPrices,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a TariffFixedPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffFixedPrice">The parsed TariffFixedPrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffFixedPriceParser">An optional delegate to parse custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out TariffFixedPrice?      TariffFixedPrice,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<TariffFixedPrice>?  CustomTariffFixedPriceParser   = null,
                                       CustomJObjectParserDelegate<TariffConditions>?  CustomTariffConditionsParser    = null)
        {

            try
            {

                TariffFixedPrice = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PriceFixed    [mandatory]

                if (!JSON.ParseMandatory("priceFixed",
                                         "fixed price",
                                         out Decimal PriceFixed,
                                         out ErrorResponse))
                {
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


                TariffFixedPrice = new TariffFixedPrice(
                                       PriceFixed,
                                       Conditions
                                   );


                if (CustomTariffFixedPriceParser is not null)
                    TariffFixedPrice = CustomTariffFixedPriceParser(JSON,
                                                                    TariffFixedPrice);

                return true;

            }
            catch (Exception e)
            {
                TariffFixedPrice  = default;
                ErrorResponse     = "The given JSON representation of a tariff element is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffFixedPriceSerializer = null, CustomTariffConditionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffFixedPrice>?  CustomTariffFixedPriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?  CustomTariffConditionsSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priceFixed",   PriceFixed),

                           Conditions is not null
                               ? new JProperty("conditions",   Conditions.ToJSON(CustomTariffConditionsSerializer))
                               : null

                       );

            return CustomTariffFixedPriceSerializer is not null
                       ? CustomTariffFixedPriceSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TariffFixedPrice Clone()

            => new (
                   PriceFixed,
                   Conditions?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffFixedPrices1, TariffFixedPrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffFixedPrices1">A tariff fixed price.</param>
        /// <param name="TariffFixedPrices2">Another tariff fixed price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffFixedPrice? TariffFixedPrices1,
                                           TariffFixedPrice? TariffFixedPrices2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffFixedPrices1, TariffFixedPrices2))
                return true;

            // If one is null, but not both, return false.
            if (TariffFixedPrices1 is null || TariffFixedPrices2 is null)
                return false;

            return TariffFixedPrices1.Equals(TariffFixedPrices2);

        }

        #endregion

        #region Operator != (TariffFixedPrices1, TariffFixedPrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffFixedPrices1">A tariff fixed price.</param>
        /// <param name="TariffFixedPrices2">Another tariff fixed price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffFixedPrice? TariffFixedPrices1,
                                           TariffFixedPrice? TariffFixedPrices2)

            => !(TariffFixedPrices1 == TariffFixedPrices2);

        #endregion

        #endregion

        #region IEquatable<TariffFixedPrices> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff fixed prices for equality.
        /// </summary>
        /// <param name="Object">A tariff fixed price to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffFixedPrice tariffFixedPrice &&
                   Equals(tariffFixedPrice);

        #endregion

        #region Equals(TariffFixedPrices)

        /// <summary>
        /// Compares two tariff fixed prices for equality.
        /// </summary>
        /// <param name="TariffFixedPrice">A tariff fixed price to compare with.</param>
        public Boolean Equals(TariffFixedPrice? TariffFixedPrice)

            => TariffFixedPrice is not null &&

               PriceFixed.Equals(TariffFixedPrice.PriceFixed) &&

             ((Conditions is     null && TariffFixedPrice.Conditions is     null) ||
              (Conditions is not null && TariffFixedPrice.Conditions is not null && Conditions.Equals(TariffFixedPrice.Conditions)));

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

            => $"{PriceFixed} €{(Conditions is not null ? $", with '{Conditions}'" : "")}!";

        #endregion

    }

}
