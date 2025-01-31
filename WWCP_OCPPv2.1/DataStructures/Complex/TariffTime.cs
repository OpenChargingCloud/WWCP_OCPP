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
    /// A TariffTime tariff element.
    /// </summary>
    public class TariffTime : IEquatable<TariffTime>
    {

        #region Properties

        /// <summary>
        /// The enumeration of tariff prices and conditions.
        /// </summary>
        [Mandatory]
        public IEnumerable<TariffTimePrice>  Prices      { get;  }

        /// <summary>
        /// The optional enumeration of applicable tax percentages for this tariff dimension.
        /// If omitted, no tax is applicable.
        /// Not providing a tax is different from 0% tax, which would be a value of 0.0 here.
        /// </summary>
        [Optional]
        public IEnumerable<TaxRate>          TaxRates    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TariffTime tariff element.
        /// </summary>
        /// <param name="Prices">An enumeration of tariff prices and conditions.</param>
        /// <param name="TaxRates">An optional enumeration of applicable tax percentages for this tariff dimension.</param>
        public TariffTime(IEnumerable<TariffTimePrice>  Prices,
                          IEnumerable<TaxRate>?         TaxRates   = null)
        {

            if (!Prices.Any())
                throw new ArgumentNullException(nameof(Prices), "The given enumeration of prices must not be empty!");

            this.Prices    = Prices.   Distinct();
            this.TaxRates  = TaxRates?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.Prices.  CalcHashCode() * 3 ^
                           this.TaxRates.CalcHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Price elements and tax for time",
        //     "javaType": "TariffTime",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "prices": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/TariffTimePriceType"
        //             },
        //             "minItems": 1
        //         },
        //         "taxRates": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/TaxRateType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 5
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "prices"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffTimeParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TariffTime tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffTimeParser">An optional delegate to parse custom TariffTime tariff element JSON objects.</param>
        public static TariffTime Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<TariffTime>?  CustomTariffTimeParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffTime,
                         out var errorResponse,
                         CustomTariffTimeParser))
            {
                return tariffTime;
            }

            throw new ArgumentException("The given JSON representation of a TariffTime tariff element is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffTime, out ErrorResponse, CustomTariffTimeParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffTime">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out TariffTime?  TariffTime,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

            => TryParse(JSON,
                        out TariffTime,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffTime">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffTimeParser">An optional delegate to parse custom tariff element JSON objects.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out TariffTime?      TariffTime,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<TariffTime>?  CustomTariffTimeParser   = null)
        {

            try
            {

                TariffTime = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Prices      [mandatory]

                if (!JSON.ParseMandatoryHashSet("prices",
                                                "time prices",
                                                TariffTimePrice.TryParse,
                                                out HashSet<TariffTimePrice> Prices,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TaxRates    [optional]

                if (JSON.ParseOptionalHashSet("taxRate",
                                              "tax rates",
                                              TaxRate.TryParse,
                                              out HashSet<TaxRate> TaxRates,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TariffTime = new TariffTime(
                                 Prices,
                                 TaxRates
                             );


                if (CustomTariffTimeParser is not null)
                    TariffTime = CustomTariffTimeParser(JSON,
                                                        TariffTime);

                return true;

            }
            catch (Exception e)
            {
                TariffTime     = default;
                ErrorResponse  = "The given JSON representation of a TariffTime tariff element is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffTimeSerializer = null, CustomPriceComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffTimeSerializer">A delegate to serialize custom TariffTime JSON objects.</param>
        /// <param name="CustomTariffTimePriceSerializer">A delegate to serialize custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffTime>?        CustomTariffTimeSerializer         = null,
                              CustomJObjectSerializerDelegate<TariffTimePrice>?   CustomTariffTimePriceSerializer    = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?  CustomTariffConditionsSerializer   = null,
                              CustomJObjectSerializerDelegate<TaxRate>?           CustomTaxRateSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("prices",   new JArray(Prices.  Select(tariffTimePrice => tariffTimePrice.ToJSON(CustomTariffTimePriceSerializer,
                                                                                                                                CustomTariffConditionsSerializer)))),

                           TaxRates.Any()
                               ? new JProperty("taxRate",  new JArray(TaxRates.Select(taxRate         => taxRate.        ToJSON(CustomTaxRateSerializer))))
                               : null

                       );

            return CustomTariffTimeSerializer is not null
                       ? CustomTariffTimeSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffTime tariff element.
        /// </summary>
        public TariffTime Clone()

            => new (
                   Prices.  Select(tariffTimePrice => tariffTimePrice.Clone()),
                   TaxRates.Select(taxRate         => taxRate.        Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffTime1, TariffTime2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffTime1">A TariffTime tariff element.</param>
        /// <param name="TariffTime2">Another TariffTime tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffTime? TariffTime1,
                                           TariffTime? TariffTime2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffTime1, TariffTime2))
                return true;

            // If one is null, but not both, return false.
            if (TariffTime1 is null || TariffTime2 is null)
                return false;

            return TariffTime1.Equals(TariffTime2);

        }

        #endregion

        #region Operator != (TariffTime1, TariffTime2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffTime1">A TariffTime tariff element.</param>
        /// <param name="TariffTime2">Another TariffTime tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffTime? TariffTime1,
                                           TariffTime? TariffTime2)

            => !(TariffTime1 == TariffTime2);

        #endregion

        #endregion

        #region IEquatable<TariffTime> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffTime tariff elements for equality.
        /// </summary>
        /// <param name="Object">A TariffTime tariff element to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffTime tariffTime &&
                   Equals(tariffTime);

        #endregion

        #region Equals(TariffTime)

        /// <summary>
        /// Compares two TariffTime tariff elements for equality.
        /// </summary>
        /// <param name="TariffTime">A TariffTime tariff element to compare with.</param>
        public Boolean Equals(TariffTime? TariffTime)

            => TariffTime is not null &&

               Prices.  Count().Equals(TariffTime.Prices.  Count()) &&
               TaxRates.Count().Equals(TariffTime.TaxRates.Count()) &&

               Prices.  All(tariffTimePrice => TariffTime.Prices.  Contains(tariffTimePrice)) &&
               TaxRates.All(taxRate         => TariffTime.TaxRates.Contains(taxRate));

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

            => $"{Prices.Count()} prices, {TaxRates.Count()} tax rates";

        #endregion

    }

}
