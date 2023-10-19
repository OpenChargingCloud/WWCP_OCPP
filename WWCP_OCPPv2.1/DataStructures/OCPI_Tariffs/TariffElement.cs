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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A charging tariff element.
    /// </summary>
    public class TariffElement : IEquatable<TariffElement>
    {

        #region Properties

        /// <summary>
        /// Enumeration of price components that make up the pricing of this tariff.
        /// </summary>
        [Mandatory]
        public IEnumerable<PriceComponent>  PriceComponents       { get; }

        /// <summary>
        /// Enumeration of tariff restrictions.
        /// </summary>
        [Optional]
        public TariffRestrictions?          TariffRestrictions    { get;  }

        #endregion

        #region Constructor(s)

        #region TariffElement(params PriceComponents)

        /// <summary>
        /// Create a new charging tariff element.
        /// </summary>
        /// <param name="PriceComponents">An enumeration of price components that make up the pricing of this tariff.</param>
        public TariffElement(params PriceComponent[]  PriceComponents)

            : this(PriceComponents,
                   null)

        { }

        #endregion

        #region TariffElement(PriceComponents, TariffRestrictions = null)

        /// <summary>
        /// Create a new charging tariff element.
        /// </summary>
        /// <param name="PriceComponents">An enumeration of price components that make up the pricing of this tariff.</param>
        /// <param name="TariffRestrictions">An enumeration of tariff restrictions.</param>
        public TariffElement(IEnumerable<PriceComponent>  PriceComponents,
                             TariffRestrictions?          TariffRestrictions   = null)
        {

            if (!PriceComponents.Any())
                throw new ArgumentNullException(nameof(PriceComponents), "The given enumeration of price components must not be empty!");

            this.PriceComponents     = PriceComponents.Distinct();
            this.TariffRestrictions  = TariffRestrictions;

        }

        #endregion

        #region TariffElement(PriceComponent,  TariffRestriction)

        /// <summary>
        /// Create a new charging tariff element.
        /// </summary>
        /// <param name="PriceComponent">A price component that makes up the pricing of this tariff.</param>
        /// <param name="TariffRestriction">A charging tariff restriction.</param>
        public TariffElement(PriceComponent      PriceComponent,
                             TariffRestrictions  TariffRestriction)
        {

            this.PriceComponents     = new[] { PriceComponent };
            this.TariffRestrictions  = TariffRestriction;

        }

        #endregion

        #endregion


        #region (static) Parse   (JSON, CustomTariffElementParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffElementParser">A delegate to parse custom tariff element JSON objects.</param>
        public static TariffElement Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<TariffElement>?  CustomTariffElementParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffElement,
                         out var errorResponse,
                         CustomTariffElementParser))
            {
                return tariffElement;
            }

            throw new ArgumentException("The given JSON representation of a tariff element is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomTariffElementParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffElementParser">A delegate to parse custom tariff element JSON objects.</param>
        public static TariffElement? TryParse(JObject                                      JSON,
                                              CustomJObjectParserDelegate<TariffElement>?  CustomTariffElementParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffElement,
                         out var errorResponse,
                         CustomTariffElementParser))
            {
                return tariffElement;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out TariffElement, out ErrorResponse, CustomTariffElementParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffElement">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject            JSON,
                                       out TariffElement  TariffElement,
                                       out String?        ErrorResponse)

            => TryParse(JSON,
                        out TariffElement,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffElement">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffElementParser">A delegate to parse custom tariff element JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out TariffElement                            TariffElement,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<TariffElement>?  CustomTariffElementParser   = null)
        {

            try
            {

                TariffElement = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PriceComponents       [mandatory]

                if (!JSON.ParseMandatoryHashSet("price_components",
                                                "price components",
                                                PriceComponent.TryParse,
                                                out HashSet<PriceComponent> PriceComponents,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffRestrictions    [optional]

                if (JSON.ParseOptionalJSON("restrictions",
                                           "tariff restrictions",
                                           OCPPv2_1.TariffRestrictions.TryParse,
                                           out TariffRestrictions? TariffRestrictions,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TariffElement = new TariffElement(PriceComponents,
                                                  TariffRestrictions);


                if (CustomTariffElementParser is not null)
                    TariffElement = CustomTariffElementParser(JSON,
                                                              TariffElement);

                return true;

            }
            catch (Exception e)
            {
                TariffElement  = default;
                ErrorResponse  = "The given JSON representation of a tariff element is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffElementSerializer = null, CustomPriceComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffElement>?       CustomTariffElementSerializer        = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?      CustomPriceComponentSerializer       = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?  CustomTariffRestrictionsSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("price_components",  new JArray(PriceComponents.Select(priceComponent => priceComponent.ToJSON(CustomPriceComponentSerializer)))),

                           TariffRestrictions is not null
                               ? new JProperty("restrictions",      TariffRestrictions.ToJSON(CustomTariffRestrictionsSerializer))
                               : null

                       );

            return CustomTariffElementSerializer is not null
                       ? CustomTariffElementSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TariffElement Clone()

            => new (PriceComponents.Select(priceComponent => priceComponent.Clone()).ToArray(),
                    TariffRestrictions?.Clone());

        #endregion


        #region Operator overloading

        #region Operator == (TariffElement1, TariffElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffElement1">A charging tariff element.</param>
        /// <param name="TariffElement2">Another charging tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffElement? TariffElement1,
                                           TariffElement? TariffElement2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffElement1, TariffElement2))
                return true;

            // If one is null, but not both, return false.
            if (TariffElement1 is null || TariffElement2 is null)
                return false;

            return TariffElement1.Equals(TariffElement2);

        }

        #endregion

        #region Operator != (TariffElement1, TariffElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffElement1">A charging tariff element.</param>
        /// <param name="TariffElement2">Another charging tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffElement? TariffElement1,
                                           TariffElement? TariffElement2)

            => !(TariffElement1 == TariffElement2);

        #endregion

        #endregion

        #region IEquatable<TariffElement> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging tariff elements for equality.
        /// </summary>
        /// <param name="Object">A charging tariff element to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffElement tariffElement &&
                   Equals(tariffElement);

        #endregion

        #region Equals(TariffElement)

        /// <summary>
        /// Compares two charging tariff elements for equality.
        /// </summary>
        /// <param name="TariffElement">A charging tariff element to compare with.</param>
        public Boolean Equals(TariffElement? TariffElement)

            => TariffElement is not null &&

               PriceComponents.Count().Equals(TariffElement.PriceComponents.Count()) &&
               PriceComponents.All(priceComponents => TariffElement.PriceComponents.Contains(priceComponents)) &&

             ((TariffRestrictions is     null && TariffElement.TariffRestrictions is     null) ||
              (TariffRestrictions is not null && TariffElement.TariffRestrictions is not null && TariffRestrictions == TariffElement.TariffRestrictions));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return PriceComponents.    CalcHashCode() * 3 ^
                      (TariffRestrictions?.GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{PriceComponents.Count()} price component(s){(TariffRestrictions is not null ? " with tariff restrictions" : "")}!";

        #endregion

    }

}
