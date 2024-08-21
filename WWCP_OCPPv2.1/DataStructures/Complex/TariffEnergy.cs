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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An energy tariff element.
    /// </summary>
    public class TariffEnergy : IEquatable<TariffEnergy>
    {

        #region Properties

        /// <summary>
        /// The enumeration of tariff prices and conditions.
        /// </summary>
        [Mandatory]
        public IEnumerable<TariffEnergyPrice>  Prices      { get;  }

        /// <summary>
        /// The optional enumeration of applicable tax percentages for this tariff dimension.
        /// If omitted, no tax is applicable.
        /// Not providing a tax is different from 0% tax, which would be a value of 0.0 here.
        /// </summary>
        [Optional]
        public IEnumerable<TaxRate>            TaxRates    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new energy tariff element.
        /// </summary>
        /// <param name="Prices">An enumeration of tariff prices and conditions.</param>
        /// <param name="TaxRates">An optional enumeration of applicable tax percentages for this tariff dimension.</param>
        public TariffEnergy(IEnumerable<TariffEnergyPrice>  Prices,
                            IEnumerable<TaxRate>?           TaxRates   = null)
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
        //     "description":          "Price elements and tax for energy",
        //     "javaType":             "TariffEnergy",
        //     "type":                 "object",
        //     "additionalProperties":  false,
        //     "properties": {
        //         "prices": {
        //             "type":            "array",
        //             "additionalItems":  false,
        //             "items": {
        //                 "$ref": "#/definitions/TariffEnergyPriceType"
        //             },
        //             "minItems": 1
        //         },
        //         "taxRate": {
        //             "type":            "array",
        //             "additionalItems":  false,
        //             "items": {
        //                 "$ref": "#/definitions/TaxRateType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 5
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffEnergyParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffEnergyParser">An optional delegate to parse custom tariff element JSON objects.</param>
        public static TariffEnergy Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<TariffEnergy>?  CustomTariffEnergyParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffEnergy,
                         out var errorResponse,
                         CustomTariffEnergyParser))
            {
                return tariffEnergy;
            }

            throw new ArgumentException("The given JSON representation of a tariff element is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffEnergy, out ErrorResponse, CustomTariffEnergyParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffEnergy">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out TariffEnergy?  TariffEnergy,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out TariffEnergy,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffEnergy">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffEnergyParser">An optional delegate to parse custom tariff element JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out TariffEnergy?      TariffEnergy,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<TariffEnergy>?  CustomTariffEnergyParser   = null)
        {

            try
            {

                TariffEnergy = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Prices      [mandatory]

                if (!JSON.ParseMandatoryHashSet("prices",
                                                "energy prices",
                                                TariffEnergyPrice.TryParse,
                                                out HashSet<TariffEnergyPrice> Prices,
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


                TariffEnergy = new TariffEnergy(
                                   Prices,
                                   TaxRates
                               );


                if (CustomTariffEnergyParser is not null)
                    TariffEnergy = CustomTariffEnergyParser(JSON,
                                                            TariffEnergy);

                return true;

            }
            catch (Exception e)
            {
                TariffEnergy   = default;
                ErrorResponse  = "The given JSON representation of a tariff energy is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffEnergySerializer = null, CustomPriceComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffEnergySerializer">A delegate to serialize custom TariffEnergy JSON objects.</param>
        /// <param name="CustomTariffEnergyPriceSerializer">A delegate to serialize custom TariffEnergyPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffEnergy>?       CustomTariffEnergySerializer        = null,
                              CustomJObjectSerializerDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffConditionsSerializer    = null,
                              CustomJObjectSerializerDelegate<TaxRate>?            CustomTaxRateSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("prices",   new JArray(Prices.  Select(tariffEnergyPrice => tariffEnergyPrice.ToJSON(CustomTariffEnergyPriceSerializer,
                                                                                                                                    CustomTariffConditionsSerializer)))),

                           TaxRates.Any()
                               ? new JProperty("taxRate",  new JArray(TaxRates.Select(taxRate           => taxRate.          ToJSON(CustomTaxRateSerializer))))
                               : null

                       );

            return CustomTariffEnergySerializer is not null
                       ? CustomTariffEnergySerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TariffEnergy Clone()

            => new (
                   Prices.  Select(tariffEnergyPrice => tariffEnergyPrice.Clone()),
                   TaxRates.Select(taxRate           => taxRate.          Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffEnergy1, TariffEnergy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffEnergy1">A tariff energy.</param>
        /// <param name="TariffEnergy2">Another tariff energy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffEnergy? TariffEnergy1,
                                           TariffEnergy? TariffEnergy2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffEnergy1, TariffEnergy2))
                return true;

            // If one is null, but not both, return false.
            if (TariffEnergy1 is null || TariffEnergy2 is null)
                return false;

            return TariffEnergy1.Equals(TariffEnergy2);

        }

        #endregion

        #region Operator != (TariffEnergy1, TariffEnergy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffEnergy1">A tariff energy.</param>
        /// <param name="TariffEnergy2">Another tariff energy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffEnergy? TariffEnergy1,
                                           TariffEnergy? TariffEnergy2)

            => !(TariffEnergy1 == TariffEnergy2);

        #endregion

        #endregion

        #region IEquatable<TariffEnergy> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff energies for equality.
        /// </summary>
        /// <param name="Object">A tariff energy to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffEnergy tariffEnergy &&
                   Equals(tariffEnergy);

        #endregion

        #region Equals(TariffEnergy)

        /// <summary>
        /// Compares two tariff energies for equality.
        /// </summary>
        /// <param name="TariffEnergy">A tariff energy to compare with.</param>
        public Boolean Equals(TariffEnergy? TariffEnergy)

            => TariffEnergy is not null &&

               Prices.  Count().Equals(TariffEnergy.Prices.  Count()) &&
               Prices.  All(tariffEnergyPrice => TariffEnergy.Prices.  Contains(tariffEnergyPrice)) &&

               TaxRates.Count().Equals(TariffEnergy.TaxRates.Count()) &&
               TaxRates.All(taxRate           => TariffEnergy.TaxRates.Contains(taxRate));

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
