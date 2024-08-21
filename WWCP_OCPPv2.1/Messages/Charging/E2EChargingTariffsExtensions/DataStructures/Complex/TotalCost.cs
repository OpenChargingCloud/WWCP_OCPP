///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;
//using System.Diagnostics.CodeAnalysis;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1
//{

//    /// <summary>
//    /// A charging tariff element.
//    /// </summary>
//    public class TariffEnergy : IEquatable<TariffEnergy>
//    {

//        #region Properties

//        /// <summary>
//        /// Enumeration of price components that make up the pricing of this tariff.
//        /// </summary>
//        [Mandatory]
//        public Decimal priceKwh { get; }


//        public UInt32? StepSize { get; }

//        /// <summary>
//        /// Enumeration of tariff restrictions.
//        /// </summary>
//        [Optional]
//        public TariffConditions?          TariffRestrictions    { get;  }

//        #endregion

//        #region Constructor(s)

//        #region TariffEnergy(params PriceComponents)

//        /// <summary>
//        /// Create a new charging tariff element.
//        /// </summary>
//        /// <param name="PriceComponents">An enumeration of price components that make up the pricing of this tariff.</param>
//        public TariffEnergy(params PriceComponent[]  PriceComponents)

//            : this(PriceComponents,
//                   null)

//        { }

//        #endregion

//        #region TariffEnergy(PriceComponents, TariffRestrictions = null)

//        /// <summary>
//        /// Create a new charging tariff element.
//        /// </summary>
//        /// <param name="PriceComponents">An enumeration of price components that make up the pricing of this tariff.</param>
//        /// <param name="TariffRestrictions">An enumeration of tariff restrictions.</param>
//        public TariffEnergy(IEnumerable<PriceComponent>  PriceComponents,
//                             TariffConditions?          TariffRestrictions   = null)
//        {

//            if (!PriceComponents.Any())
//                throw new ArgumentNullException(nameof(PriceComponents), "The given enumeration of price components must not be empty!");

//            this.PriceComponents     = PriceComponents.Distinct();
//            this.TariffRestrictions  = TariffRestrictions;

//        }

//        #endregion

//        #region TariffEnergy(PriceComponent,  TariffRestriction)

//        /// <summary>
//        /// Create a new charging tariff element.
//        /// </summary>
//        /// <param name="PriceComponent">A price component that makes up the pricing of this tariff.</param>
//        /// <param name="TariffRestriction">A charging tariff restriction.</param>
//        public TariffEnergy(PriceComponent      PriceComponent,
//                             TariffConditions  TariffRestriction)
//        {

//            this.PriceComponents     = new[] { PriceComponent };
//            this.TariffRestrictions  = TariffRestriction;

//        }

//        #endregion

//        #endregion


//        #region (static) Parse   (JSON, CustomTariffEnergyParser = null)

//        /// <summary>
//        /// Parse the given JSON representation of a tariff element.
//        /// </summary>
//        /// <param name="JSON">The JSON to parse.</param>
//        /// <param name="CustomTariffEnergyParser">An optional delegate to parse custom tariff element JSON objects.</param>
//        public static TariffEnergy Parse(JObject                                      JSON,
//                                          CustomJObjectParserDelegate<TariffEnergy>?  CustomTariffEnergyParser   = null)
//        {

//            if (TryParse(JSON,
//                         out var tariffEnergy,
//                         out var errorResponse,
//                         CustomTariffEnergyParser) &&
//                tariffEnergy is not null)
//            {
//                return tariffEnergy;
//            }

//            throw new ArgumentException("The given JSON representation of a tariff element is invalid: " + errorResponse,
//                                        nameof(JSON));

//        }

//        #endregion

//        #region (static) TryParse(JSON, CustomTariffEnergyParser = null)

//        ///// <summary>
//        ///// Try to parse the given JSON representation of a tariff element.
//        ///// </summary>
//        ///// <param name="JSON">The JSON to parse.</param>
//        ///// <param name="CustomTariffEnergyParser">An optional delegate to parse custom tariff element JSON objects.</param>
//        //public static TariffEnergy? TryParse(JObject                                      JSON,
//        //                                      CustomJObjectParserDelegate<TariffEnergy>?  CustomTariffEnergyParser   = null)
//        //{

//        //    if (TryParse(JSON,
//        //                 out var tariffEnergy,
//        //                 out var errorResponse,
//        //                 CustomTariffEnergyParser))
//        //    {
//        //        return tariffEnergy;
//        //    }

//        //    return default;

//        //}

//        #endregion

//        #region (static) TryParse(JSON, out TariffEnergy, out ErrorResponse, CustomTariffEnergyParser = null)

//        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

//        /// <summary>
//        /// Try to parse the given JSON representation of a tariff element.
//        /// </summary>
//        /// <param name="JSON">The JSON to parse.</param>
//        /// <param name="TariffEnergy">The parsed tariff element.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        public static Boolean TryParse(JObject                                 JSON,
//                                       [NotNullWhen(true)]  out TariffEnergy?  TariffEnergy,
//                                       [NotNullWhen(false)] out String?        ErrorResponse)

//            => TryParse(JSON,
//                        out TariffEnergy,
//                        out ErrorResponse,
//                        null);


//        /// <summary>
//        /// Try to parse the given JSON representation of a tariff element.
//        /// </summary>
//        /// <param name="JSON">The JSON to parse.</param>
//        /// <param name="TariffEnergy">The parsed tariff element.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        /// <param name="CustomTariffEnergyParser">An optional delegate to parse custom tariff element JSON objects.</param>
//        public static Boolean TryParse(JObject                                     JSON,
//                                       [NotNullWhen(true)]  out TariffEnergy?      TariffEnergy,
//                                       [NotNullWhen(false)] out String?            ErrorResponse,
//                                       CustomJObjectParserDelegate<TariffEnergy>?  CustomTariffEnergyParser   = null)
//        {

//            try
//            {

//                TariffEnergy = default;

//                if (JSON?.HasValues != true)
//                {
//                    ErrorResponse = "The given JSON object must not be null or empty!";
//                    return false;
//                }

//                #region Parse PriceComponents       [mandatory]

//                if (!JSON.ParseMandatoryHashSet("price_components",
//                                                "price components",
//                                                PriceComponent.TryParse,
//                                                out HashSet<PriceComponent> PriceComponents,
//                                                out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region Parse TariffRestrictions    [optional]

//                if (JSON.ParseOptionalJSON("restrictions",
//                                           "tariff restrictions",
//                                           OCPPv2_1.TariffConditions.TryParse,
//                                           out TariffConditions? TariffRestrictions,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion


//                TariffEnergy = new TariffEnergy(
//                                   PriceComponents,
//                                   TariffRestrictions
//                               );


//                if (CustomTariffEnergyParser is not null)
//                    TariffEnergy = CustomTariffEnergyParser(JSON,
//                                                              TariffEnergy);

//                return true;

//            }
//            catch (Exception e)
//            {
//                TariffEnergy  = default;
//                ErrorResponse  = "The given JSON representation of a tariff element is invalid: " + e.Message;
//                return false;
//            }

//        }

//        #endregion

//        #region ToJSON(CustomTariffEnergySerializer = null, CustomPriceComponentSerializer = null, ...)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomTariffEnergySerializer">A delegate to serialize custom tariff element JSON objects.</param>
//        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
//        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rate JSON objects.</param>
//        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
//        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffEnergy>?       CustomTariffEnergySerializer        = null,
//                              CustomJObjectSerializerDelegate<PriceComponent>?      CustomPriceComponentSerializer       = null,
//                              CustomJObjectSerializerDelegate<TaxRate>?             CustomTaxRateSerializer              = null,
//                              CustomJObjectSerializerDelegate<TariffConditions>?  CustomTariffRestrictionsSerializer   = null)
//        {

//            var json = JSONObject.Create(

//                                 new JProperty("priceComponents",   new JArray(PriceComponents.Select(priceComponent => priceComponent.ToJSON(CustomPriceComponentSerializer,
//                                                                                                                                              CustomTaxRateSerializer)))),

//                           TariffRestrictions is not null
//                               ? new JProperty("restrictions",      TariffRestrictions.ToJSON(CustomTariffRestrictionsSerializer))
//                               : null

//                       );

//            return CustomTariffEnergySerializer is not null
//                       ? CustomTariffEnergySerializer(this, json)
//                       : json;

//        }

//        #endregion

//        #region Clone()

//        /// <summary>
//        /// Clone this object.
//        /// </summary>
//        public TariffEnergy Clone()

//            => new (
//                   PriceComponents.Select(priceComponent => priceComponent.Clone()).ToArray(),
//                   TariffRestrictions?.Clone()
//               );

//        #endregion


//        #region Operator overloading

//        #region Operator == (TariffEnergy1, TariffEnergy2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="TariffEnergy1">A charging tariff element.</param>
//        /// <param name="TariffEnergy2">Another charging tariff element.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator == (TariffEnergy? TariffEnergy1,
//                                           TariffEnergy? TariffEnergy2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(TariffEnergy1, TariffEnergy2))
//                return true;

//            // If one is null, but not both, return false.
//            if (TariffEnergy1 is null || TariffEnergy2 is null)
//                return false;

//            return TariffEnergy1.Equals(TariffEnergy2);

//        }

//        #endregion

//        #region Operator != (TariffEnergy1, TariffEnergy2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="TariffEnergy1">A charging tariff element.</param>
//        /// <param name="TariffEnergy2">Another charging tariff element.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator != (TariffEnergy? TariffEnergy1,
//                                           TariffEnergy? TariffEnergy2)

//            => !(TariffEnergy1 == TariffEnergy2);

//        #endregion

//        #endregion

//        #region IEquatable<TariffEnergy> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two charging tariff elements for equality.
//        /// </summary>
//        /// <param name="Object">A charging tariff element to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is TariffEnergy tariffEnergy &&
//                   Equals(tariffEnergy);

//        #endregion

//        #region Equals(TariffEnergy)

//        /// <summary>
//        /// Compares two charging tariff elements for equality.
//        /// </summary>
//        /// <param name="TariffEnergy">A charging tariff element to compare with.</param>
//        public Boolean Equals(TariffEnergy? TariffEnergy)

//            => TariffEnergy is not null &&

//               PriceComponents.Count().Equals(TariffEnergy.PriceComponents.Count()) &&
//               PriceComponents.All(priceComponents => TariffEnergy.PriceComponents.Contains(priceComponents)) &&

//             ((TariffRestrictions is     null && TariffEnergy.TariffRestrictions is     null) ||
//              (TariffRestrictions is not null && TariffEnergy.TariffRestrictions is not null && TariffRestrictions == TariffEnergy.TariffRestrictions));

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        /// <returns>The hash code of this object.</returns>
//        public override Int32 GetHashCode()
//        {
//            unchecked
//            {

//                return PriceComponents.    CalcHashCode() * 3 ^
//                      (TariffRestrictions?.GetHashCode() ?? 0);

//            }
//        }

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => $"{PriceComponents.Count()} price component(s){(TariffRestrictions is not null ? " with tariff restrictions" : "")}!";

//        #endregion

//    }

//}
