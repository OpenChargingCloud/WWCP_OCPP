﻿/*
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

using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for EV driver charging tariffs (B2C).
    /// </summary>
    public static class ChargingTariffExtensions
    {

        #region GenerateWithHashId(ProviderId, ProviderName, Currency, TariffElements, ...)

        /// <summary>
        /// Generate a new read-only signable EV driver charging tariff (B2C) having an identification based on the SHA256 of its information.
        /// </summary>
        public static ChargingTariff GenerateWithHashId(Provider_Id                                            ProviderId,
                                                        DisplayTexts                                           ProviderName,
                                                        Currency                                               Currency,
                                                        IEnumerable<TariffElement>                             TariffElements,

                                                        ImageLinks?                                            ProviderLogos                         = null,
                                                        DateTime?                                              Created                               = null,
                                                        IEnumerable<ChargingTariff_Id>?                        Replaces                              = null,
                                                        IEnumerable<ChargingTariff_Id>?                        References                            = null,
                                                        TariffType?                                            TariffType                            = null,
                                                        DisplayTexts?                                          Description                           = null,
                                                        URL?                                                   URL                                   = null,
                                                        EnergyMix?                                             EnergyMix                             = null,

                                                        IEnumerable<IdToken>?                                  IdTokens                              = null,

                                                        Price?                                                 MinPrice                              = null,
                                                        Price?                                                 MaxPrice                              = null,
                                                        DateTime?                                              NotBefore                             = null,
                                                        DateTime?                                              NotAfter                              = null,

                                                        CustomData?                                            CustomData                            = null,

                                                        CustomJObjectSerializerDelegate<ChargingTariff>?       CustomTariffSerializer                = null,
                                                        CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                                                        CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                                                        CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                                                        CustomJObjectSerializerDelegate<TaxRate>?              CustomTaxRateSerializer               = null,
                                                        CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                                                        CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                                                        CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                                                        CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                                                        CustomJObjectSerializerDelegate<IdToken>?              CustomIdTokenSerializer               = null,
                                                        CustomJObjectSerializerDelegate<AdditionalInfo>?       CustomAdditionalInfoSerializer        = null,
                                                        CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                                                        CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)

        {

            var creationTimestamp  = Created ?? Timestamp.Now;

            var chargingTariff     = new ChargingTariff(

                                         ChargingTariff_Id.NewRandom(ProviderId),
                                         ProviderId,
                                         ProviderName,
                                         Currency,
                                         TariffElements,

                                         ProviderLogos,
                                         creationTimestamp,
                                         Replaces,
                                         References,
                                         TariffType,
                                         Description,
                                         URL,
                                         EnergyMix,

                                         IdTokens,

                                         MinPrice,
                                         MaxPrice,
                                         NotBefore,
                                         NotAfter,

                                         null,
                                         null,
                                         null,

                                         CustomData

                                     );


            return new (

                   ChargingTariff_Id.Generate(
                       ProviderId,
                       SHA256.HashData(

                           chargingTariff.ToJSON  (CustomTariffSerializer,
                                                   CustomPriceSerializer,
                                                   CustomTariffElementSerializer,
                                                   CustomPriceComponentSerializer,
                                                   CustomTaxRateSerializer,
                                                   CustomTariffRestrictionsSerializer,
                                                   CustomEnergyMixSerializer,
                                                   CustomEnergySourceSerializer,
                                                   CustomEnvironmentalImpactSerializer,
                                                   CustomIdTokenSerializer,
                                                   CustomAdditionalInfoSerializer,
                                                   CustomSignatureSerializer,
                                                   CustomCustomDataSerializer).

                                          ToString(Formatting.None,
                                                   SignableMessage.DefaultJSONConverters).

                                          ToUTF8Bytes()

                       ).ToBase64(),
                       creationTimestamp
                   ),

                   chargingTariff.ProviderId,
                   chargingTariff.ProviderName,
                   chargingTariff.Currency,
                   chargingTariff.TariffElements,

                   chargingTariff.ProviderLogos,
                   chargingTariff.Created,
                   chargingTariff.Replaces,
                   chargingTariff.References,
                   chargingTariff.TariffType,
                   chargingTariff.Description,
                   chargingTariff.URL,
                   chargingTariff.EnergyMix,

                   chargingTariff.IdTokens,

                   chargingTariff.MinPrice,
                   chargingTariff.MaxPrice,
                   chargingTariff.NotBefore,
                   chargingTariff.NotAfter,

                   null,
                   null,
                   chargingTariff.Signatures,

                   chargingTariff.CustomData

               );

        }

        #endregion

    }


    /// <summary>
    /// A read-only signable EV driver charging tariff (B2C).
    /// </summary>
    public class ChargingTariff : ACustomSignableData,
                                  ISignableMessage,
                                  IHasId<ChargingTariff_Id>,
                                  IEquatable<ChargingTariff>,
                                  IComparable<ChargingTariff>,
                                  IComparable,
                                  INotBeforeNotAfter
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/chargingTariff");

        #endregion

        #region Properties

        /// <summary>
        /// The global unique and unique in time identification of the charging tariff.
        /// </summary>
        [Mandatory]
        public   ChargingTariff_Id              Id                    { get; }

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        [Mandatory]
        public JSONLDContext                    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The timestamp when this tariff was created.
        /// </summary>
        [Mandatory] //, NonStandard("Pagination")]
        public   DateTime                       Created               { get; }

        /// <summary>
        /// Optional references to other tariffs, which will be replaced by this charging tariff.
        /// </summary>
        [Optional]
        public  IEnumerable<ChargingTariff_Id>  Replaces              { get; }

        /// <summary>
        /// Optional references to other tariffs, e.g. because some local adaption of a charging tariff was required.
        /// </summary>
        [Optional]
        public  IEnumerable<ChargingTariff_Id>  References            { get; }

        /// <summary>
        /// The unique identification of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public   Provider_Id                    ProviderId            { get; }

        /// <summary>
        /// The multi-language name of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public   DisplayTexts                   ProviderName          { get; }

        /// <summary>
        /// The optional collection of logo URLs of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Optional]
        public   ImageLinks                     ProviderLogos         { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public   Currency                       Currency              { get; }

        /// <summary>
        /// The optional tariff type allows to distinguish between charging preferences.
        /// When omitted, this tariff is valid for all charging sessions.
        /// </summary>
        [Optional]
        public   TariffType?                    TariffType            { get; }

        /// <summary>
        /// The optional multi-language tariff description.
        /// </summary>
        [Optional]
        public   DisplayTexts                   Description           { get; }

        /// <summary>
        /// The optional informative (not legally binding) URL to a web page that contains an
        /// explanation of the tariff information in human readable form.
        /// </summary>
        [Optional]
        public   URL?                           URL                   { get; }

        /// <summary>
        /// Optional details on the energy supplied with this tariff.
        /// </summary>
        [Optional]
        public   EnergyMix?                     EnergyMix             { get;  }


        /// <summary>
        /// The optional enumeration of IdTokens this charging tariff is intended for.
        /// </summary>
        [Optional]
        public   IEnumerable<IdToken>           IdTokens              { get;  }


        /// <summary>
        /// When this optional field is set, a charging session with this tariff will at least cost
        /// this amount. This is different from a FLAT fee (Start Tariff, Transaction Fee),
        /// as a FLAT fee is a fixed amount that has to be paid for any Charging Session.
        /// A minimum price indicates that when the cost of a charging session is lower
        /// than this amount, the cost of the charging session will be equal to this amount.
        /// </summary>
        [Optional]
        public   Price?                         MinPrice              { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will NOT
        /// cost more than this amount.
        /// </summary>
        [Optional]
        public   Price?                         MaxPrice              { get; }

        /// <summary>
        /// The enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public   IEnumerable<TariffElement>     TariffElements        { get; }

        /// <summary>
        /// The optional timestamp when this charging tariff becomes active/valid.
        /// Typically used for a new charging tariff that already exists within a
        /// charging station, CSMS or EMP system, before it becomes active/valid.
        /// </summary>
        [Optional]
        public   DateTime?                      NotBefore             { get; }

        /// <summary>
        /// The optional timestamp after which this charging tariff is no longer active/valid.
        /// Typically used when a charging tariff is replaced automatically with a new version
        /// of the tariff after this timestamp has passed.
        /// </summary>
        [Optional]
        public   DateTime?                      NotAfter              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new read-only signable EV driver charging tariff (B2C).
        /// </summary>
        /// <param name="Id">A global unique and unique in time identification of the charging tariff.</param>
        /// <param name="ProviderId">An unique identification of the e-mobility provider responsible for this tariff.</param>
        /// <param name="ProviderName">An multi-language name of the e-mobility provider responsible for this tariff.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="TariffElements">An enumeration of tariff elements.</param>
        /// 
        /// <param name="ProviderLogos">An optional collection of logo URLs of the e-mobility provider responsible for this tariff.</param>
        /// <param name="Created">An optional timestamp when this tariff was created.</param>
        /// <param name="Replaces">Optional references to other tariffs, which will be replaced by this charging tariff.</param>
        /// <param name="References">Optional references to other tariffs, e.g. because some local adaption of a charging tariff was required.</param>
        /// <param name="TariffType">An optional tariff type, that allows to distinguish between charging preferences. When omitted, this tariff is valid for all charging sessions.</param>
        /// <param name="Description">An optional multi-language tariff description.</param>
        /// <param name="URL">An optional informative (not legally binding) URL to a web page that contains an explanation of the tariff information in human readable form.</param>
        /// 
        /// <param name="IdTokens">An optional enumeration of IdTokens for which this charging tariff is intended for.</param>
        /// 
        /// <param name="MinPrice">When this optional field is set, a charging session with this tariff will at least cost this amount.</param>
        /// <param name="MaxPrice">When this optional field is set, a charging session with this tariff will NOT cost more than this amount.</param>
        /// <param name="NotBefore">An optional timestamp when this object becomes active/valid. Typically used for a new object that already exists, before it becomes active/valid.</param>
        /// <param name="NotAfter">An optional timestamp after which this object is no longer active/valid. Typically used when an object is is replaced automatically with a new version of the object after this timestamp has passed.</param>
        /// <param name="EnergyMix">Optional details on the energy supplied with this tariff.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this charging tariff.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this charging tariff.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingTariff(ChargingTariff_Id                Id,
                              Provider_Id                      ProviderId,
                              DisplayTexts                     ProviderName,
                              Currency                         Currency,
                              IEnumerable<TariffElement>       TariffElements,

                              ImageLinks?                      ProviderLogos   = null,
                              DateTime?                        Created         = null,
                              IEnumerable<ChargingTariff_Id>?  Replaces        = null,
                              IEnumerable<ChargingTariff_Id>?  References      = null,
                              TariffType?                      TariffType      = null,
                              DisplayTexts?                    Description     = null,
                              URL?                             URL             = null,
                              EnergyMix?                       EnergyMix       = null,

                              IEnumerable<IdToken>?            IdTokens        = null,

                              Price?                           MinPrice        = null,
                              Price?                           MaxPrice        = null,
                              DateTime?                        NotBefore       = null,
                              DateTime?                        NotAfter        = null,

                              IEnumerable<KeyPair>?            SignKeys        = null,
                              IEnumerable<SignInfo>?           SignInfos       = null,
                              IEnumerable<Signature>?          Signatures      = null,

                              CustomData?                      CustomData      = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            if (!TariffElements.Any())
                throw new ArgumentNullException(nameof(TariffElements), "The given enumeration of tariff elements must not be null or empty!");

            this.Id              = Id;
            this.ProviderId      = ProviderId;
            this.ProviderName    = ProviderName;
            this.Currency        = Currency;
            this.TariffElements  = TariffElements.Distinct();

            this.ProviderLogos   = ProviderLogos ?? ImageLinks.  Empty;
            this.Created         = Created       ?? Timestamp.   Now;
            this.Replaces        = Replaces?.     Distinct() ?? Array.Empty<ChargingTariff_Id>();
            this.References      = References?.   Distinct() ?? Array.Empty<ChargingTariff_Id>();
            this.TariffType      = TariffType;
            this.Description     = Description   ?? DisplayTexts.Empty;
            this.URL             = URL;
            this.EnergyMix       = EnergyMix;

            this.IdTokens        = IdTokens?.     Distinct() ?? Array.Empty<IdToken>();

            this.MinPrice        = MinPrice;
            this.MaxPrice        = MaxPrice;
            this.NotBefore       = NotBefore;
            this.NotAfter        = NotAfter;


            unchecked
            {

                hashCode = this.Id.            GetHashCode()        * 67 ^
                           this.ProviderId.    GetHashCode()        * 61 ^
                           this.ProviderName.  GetHashCode()        * 59 ^
                           this.Currency.      GetHashCode()        * 53 ^
                           this.TariffElements.CalcHashCode()       * 47 ^

                           this.ProviderLogos. GetHashCode()        * 43 ^
                           this.Created.       GetHashCode()        * 41 ^
                           this.Replaces.      CalcHashCode()       * 37 ^
                           this.References.    CalcHashCode()       * 31 ^
                          (this.TariffType?.   GetHashCode()  ?? 0) * 29 ^
                           this.Description.   CalcHashCode()       * 23 ^
                          (this.URL?.          GetHashCode()  ?? 0) * 19 ^
                           this.EnergyMix?.    GetHashCode()  ?? 0  * 17 ^

                           this.IdTokens.      CalcHashCode()       * 13 ^

                          (this.MinPrice?.     GetHashCode()  ?? 0) * 11 ^
                          (this.MaxPrice?.     GetHashCode()  ?? 0) *  7 ^
                           this.NotBefore.     GetHashCode()        *  5 ^
                          (this.NotAfter?.     GetHashCode()  ?? 0) *  3 ^

                           base.               GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, ChargingTariffIdURL = null, CustomChargingTariffParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingTariffIdURL">An optional charging tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomChargingTariffParser">A delegate to parse custom charging tariff JSON objects.</param>
        public static ChargingTariff Parse(JObject                                       JSON,
                                           ChargingTariff_Id?                            ChargingTariffIdURL   = null,
                                           CustomJObjectParserDelegate<ChargingTariff>?  CustomChargingTariffParser    = null)
        {

            if (TryParse(JSON,
                         out var chargingTariff,
                         out var errorResponse,
                         ChargingTariffIdURL,
                         CustomChargingTariffParser) &&
                chargingTariff is not null)
            {
                return chargingTariff;
            }

            throw new ArgumentException("The given JSON representation of a charging tariff is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingTariff, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingTariff">The parsed charging tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out ChargingTariff?  ChargingTariff,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out ChargingTariff,
                        out ErrorResponse,
                        null,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingTariff">The parsed charging tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="TariffIdURL">An optional charging tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomChargingTariffParser">A delegate to parse custom charging tariff JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out ChargingTariff?                           ChargingTariff,
                                       out String?                                   ErrorResponse,
                                       ChargingTariff_Id?                            TariffIdURL                  = null,
                                       CustomJObjectParserDelegate<ChargingTariff>?  CustomChargingTariffParser   = null)
        {

            try
            {

                ChargingTariff = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                    [optional]

                if (JSON.ParseOptional("id",
                                       "tariff identification",
                                       ChargingTariff_Id.TryParse,
                                       out ChargingTariff_Id? TariffIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!TariffIdURL.HasValue && !TariffIdBody.HasValue)
                {
                    ErrorResponse = "The tariff identification is missing!";
                    return false;
                }

                if (TariffIdURL.HasValue && TariffIdBody.HasValue && TariffIdURL.Value != TariffIdBody.Value)
                {
                    ErrorResponse = "The optional tariff identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse ProviderId            [mandatory]

                if (!JSON.ParseMandatory("providerId",
                                         "provider identification",
                                         Provider_Id.TryParse,
                                         out Provider_Id ProviderId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ProviderName          [mandatory]

                if (!JSON.ParseMandatoryJSON("providerName",
                                             "provider name",
                                             DisplayTexts.TryParse,
                                             out DisplayTexts? ProviderName,
                                             out ErrorResponse) ||
                     ProviderName is null)
                {
                    return false;
                }

                #endregion

                #region Parse Currency              [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffElements        [mandatory]

                if (!JSON.ParseMandatoryHashSet("elements",
                                                "tariff elements",
                                                TariffElement.TryParse,
                                                out HashSet<TariffElement> TariffElements,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse ProviderLogos         [optional]

                if (JSON.ParseOptionalJSONArray("providerLogos",
                                                "provider logo URLs",
                                                ImageLinks.TryParse,
                                                out ImageLinks LogoURLs,
                                                out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Created               [mandatory]

                if (!JSON.ParseMandatory("created",
                                         "created",
                                         out DateTime Created,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Replaces              [optional]

                if (JSON.ParseOptionalHashSet("replaces",
                                              "replaces tariff",
                                              ChargingTariff_Id.TryParse,
                                              out HashSet<ChargingTariff_Id> Replaces,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse References            [optional]

                if (JSON.ParseOptionalHashSet("references",
                                              "references tariff",
                                              ChargingTariff_Id.TryParse,
                                              out HashSet<ChargingTariff_Id> References,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TariffType            [optional]

                if (JSON.ParseOptional("type",
                                       "tariff type",
                                       OCPPv2_1.TariffType.TryParse,
                                       out TariffType? TariffType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Description           [optional]

                if (JSON.ParseOptionalJSON("description",
                                           "tariff description",
                                           DisplayTexts.TryParse,
                                           out DisplayTexts? Description,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse URL                   [optional]

                if (JSON.ParseOptional("url",
                                       "tariff URL",
                                       org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                       out URL? URL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMix             [optional]

                if (JSON.ParseOptionalJSON("energyMix",
                                           "energy mix",
                                           OCPPv2_1.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse IdTokens              [optional]

                if (!JSON.ParseOptionalHashSet("idTokens",
                                               "id tokens",
                                               IdToken.TryParse,
                                               out HashSet<IdToken> IdTokens,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse MinPrice              [optional]

                if (JSON.ParseOptionalJSON("minPrice",
                                           "minimum price",
                                           Price.TryParse,
                                           out Price? MinPrice,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxPrice              [optional]

                if (JSON.ParseOptionalJSON("maxPrice",
                                           "maximum price",
                                           Price.TryParse,
                                           out Price? MaxPrice,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotBefore             [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTime? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter              [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures                  [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                  [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingTariff = new ChargingTariff(

                                     TariffIdBody ?? TariffIdURL!.Value,
                                     ProviderId,
                                     ProviderName,
                                     Currency,
                                     TariffElements,

                                     LogoURLs,
                                     Created,
                                     Replaces,
                                     References,
                                     TariffType,
                                     Description,
                                     URL,
                                     EnergyMix,

                                     IdTokens,

                                     MinPrice,
                                     MaxPrice,
                                     NotBefore,
                                     NotAfter,

                                     null,
                                     null,
                                     Signatures,
                                     CustomData

                                 );

                if (CustomChargingTariffParser is not null)
                    ChargingTariff = CustomChargingTariffParser(JSON,
                                                                ChargingTariff);

                return true;

            }
            catch (Exception e)
            {
                ChargingTariff  = default;
                ErrorResponse   = "The given JSON representation of a charging tariff is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingTariffSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingTariff>?       CustomChargingTariffSerializer        = null,
                              CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                              CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<TaxRate>?              CustomTaxRateSerializer               = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?              CustomIdTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?       CustomAdditionalInfoSerializer        = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",             Id.              ToString()),
                                 new JProperty("providerId",     ProviderId.      ToString()),
                                 new JProperty("providerName",   ProviderName.    ToJSON()),

                           ProviderLogos.     Any()
                               ? new JProperty("providerLogos",  ProviderLogos.   ToJSON())
                               : null,

                                 new JProperty("currency",       Currency.        ISOCode),
                                 new JProperty("created",        Created.         ToIso8601()),

                           Replaces.          Any()
                               ? new JProperty("replaces",       new JArray(Replaces.      Select(chargingTariffId => chargingTariffId.ToString())))
                               : null,

                           References.        Any()
                               ? new JProperty("references",     new JArray(References.    Select(chargingTariffId => chargingTariffId.ToString())))
                               : null,

                           TariffType.HasValue
                               ? new JProperty("type",           TariffType.Value.ToString())
                               : null,

                           Description.       Any()
                               ? new JProperty("description",    Description.     ToJSON())
                               : null,

                           URL.     HasValue
                               ? new JProperty("url",            URL.             ToString())
                               : null,

                           EnergyMix is not null
                               ? new JProperty("energyMix",      EnergyMix.       ToJSON(CustomEnergyMixSerializer,
                                                                                         CustomEnergySourceSerializer,
                                                                                         CustomEnvironmentalImpactSerializer))
                               : null,


                           IdTokens.          Any()
                               ? new JProperty("idTokens",       new JArray(IdTokens.Select(idToken                => idToken.         ToJSON(CustomIdTokenSerializer,
                                                                                                                                              CustomAdditionalInfoSerializer,
                                                                                                                                              CustomCustomDataSerializer))))
                               : null,


                           MinPrice.HasValue
                               ? new JProperty("minPrice",       MinPrice.  Value.ToJSON(CustomPriceSerializer))
                               : null,

                           MaxPrice.HasValue
                               ? new JProperty("maxPrice",       MaxPrice.  Value.ToJSON(CustomPriceSerializer))
                               : null,

                           NotBefore.HasValue
                               ? new JProperty("notBefore",      NotBefore. Value.ToIso8601())
                               : null,

                           NotAfter.HasValue
                               ? new JProperty("notAftere",      NotAfter.  Value.ToIso8601())
                               : null,


                           TariffElements.    Any()
                               ? new JProperty("elements",       new JArray(TariffElements.Select(tariffElement    => tariffElement.   ToJSON(CustomTariffElementSerializer,
                                                                                                                                              CustomPriceComponentSerializer,
                                                                                                                                              CustomTaxRateSerializer,
                                                                                                                                              CustomTariffRestrictionsSerializer))))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",     new JArray(Signatures.    Select(signature        => signature.       ToJSON(CustomSignatureSerializer,
                                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomChargingTariffSerializer is not null
                       ? CustomChargingTariffSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charging tariff.
        /// </summary>
        public ChargingTariff Clone()

            => new (

                   Id.            Clone,
                   ProviderId.    Clone,
                   ProviderName.  Clone(),
                   Currency.      Clone,
                   TariffElements.Select(tariffElement    => tariffElement.   Clone()).ToArray(),

                   ProviderLogos. Clone(),
                   Created,
                   Replaces.      Select(chargingTariffId => chargingTariffId.Clone).  ToArray(),
                   References.    Select(chargingTariffId => chargingTariffId.Clone).  ToArray(),
                   TariffType?.   Clone,
                   Description.   Clone(),
                   URL?.          Clone,
                   EnergyMix?.    Clone(),

                   IdTokens.      Select(idToken          => idToken.Clone()).         ToArray(),

                   MinPrice?.     Clone(),
                   MaxPrice?.     Clone(),
                   NotBefore,
                   NotAfter,

                   SignKeys,
                   SignInfos,
                   Signatures.    Select(signature        => signature.       Clone()).ToArray(),

                   CustomData

               );

        #endregion


        #region Sign(KeyPair, out ErrorResponse, SignerName = null, Description = null, Timestamp = null, ...)

        public Boolean Sign(KeyPair                                                KeyPair,
                            out String?                                            ErrorResponse,
                            String?                                                SignerName                            = null,
                            I18NString?                                            Description                           = null,
                            DateTime?                                              Timestamp                             = null,
                            CustomJObjectSerializerDelegate<ChargingTariff>?       CustomChargingTariffSerializer        = null,
                            CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                            CustomJObjectSerializerDelegate<TaxRate>?              CustomTaxRateSerializer               = null,
                            CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                            CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                            CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                            CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                            CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                            CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                            CustomJObjectSerializerDelegate<IdToken>?              CustomIdTokenSerializer               = null,
                            CustomJObjectSerializerDelegate<AdditionalInfo>?       CustomAdditionalInfoSerializer        = null,
                            CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                            CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)

            => Sign(ToJSON(CustomChargingTariffSerializer,
                           CustomPriceSerializer,
                           CustomTariffElementSerializer,
                           CustomPriceComponentSerializer,
                           CustomTaxRateSerializer,
                           CustomTariffRestrictionsSerializer,
                           CustomEnergyMixSerializer,
                           CustomEnergySourceSerializer,
                           CustomEnvironmentalImpactSerializer,
                           CustomIdTokenSerializer,
                           CustomAdditionalInfoSerializer,
                           CustomSignatureSerializer,
                           CustomCustomDataSerializer),
                    Context,
                    KeyPair,
                    out ErrorResponse,
                    SignerName,
                    Description,
                    Timestamp);

        #endregion

        #region Verify(out ErrorResponse, VerificationRuleAction = null, ...)

        public Boolean Verify(out String?                                            ErrorResponse,
                              VerificationRuleActions?                               VerificationRuleAction                = null,
                              CustomJObjectSerializerDelegate<ChargingTariff>?       CustomChargingTariffSerializer        = null,
                              CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                              CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<TaxRate>?              CustomTaxRateSerializer               = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?              CustomIdTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?       CustomAdditionalInfoSerializer        = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)

            => Verify(ToJSON(CustomChargingTariffSerializer,
                             CustomPriceSerializer,
                             CustomTariffElementSerializer,
                             CustomPriceComponentSerializer,
                             CustomTaxRateSerializer,
                             CustomTariffRestrictionsSerializer,
                             CustomEnergyMixSerializer,
                             CustomEnergySourceSerializer,
                             CustomEnvironmentalImpactSerializer,
                             CustomIdTokenSerializer,
                             CustomAdditionalInfoSerializer,
                             CustomSignatureSerializer,
                             CustomCustomDataSerializer),
                      Context,
                      out ErrorResponse,
                      VerificationRuleAction);

        #endregion


        #region Operator overloading

        #region Operator == (ChargingTariff1, ChargingTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariff1">A charging tariff.</param>
        /// <param name="ChargingTariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingTariff? ChargingTariff1,
                                           ChargingTariff? ChargingTariff2)
        {

            if (Object.ReferenceEquals(ChargingTariff1, ChargingTariff2))
                return true;

            if (ChargingTariff1 is null || ChargingTariff2 is null)
                return false;

            return ChargingTariff1.Equals(ChargingTariff2);

        }

        #endregion

        #region Operator != (ChargingTariff1, ChargingTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariff1">A charging tariff.</param>
        /// <param name="ChargingTariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingTariff? ChargingTariff1,
                                           ChargingTariff? ChargingTariff2)

            => !(ChargingTariff1 == ChargingTariff2);

        #endregion

        #region Operator <  (ChargingTariff1, ChargingTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariff1">A charging tariff.</param>
        /// <param name="ChargingTariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingTariff? ChargingTariff1,
                                          ChargingTariff? ChargingTariff2)

            => ChargingTariff1 is null
                   ? throw new ArgumentNullException(nameof(ChargingTariff1), "The given tariff must not be null!")
                   : ChargingTariff1.CompareTo(ChargingTariff2) < 0;

        #endregion

        #region Operator <= (ChargingTariff1, ChargingTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariff1">A charging tariff.</param>
        /// <param name="ChargingTariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingTariff? ChargingTariff1,
                                           ChargingTariff? ChargingTariff2)

            => !(ChargingTariff1 > ChargingTariff2);

        #endregion

        #region Operator >  (ChargingTariff1, ChargingTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariff1">A charging tariff.</param>
        /// <param name="ChargingTariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingTariff? ChargingTariff1,
                                          ChargingTariff? ChargingTariff2)

            => ChargingTariff1 is null
                   ? throw new ArgumentNullException(nameof(ChargingTariff1), "The given tariff must not be null!")
                   : ChargingTariff1.CompareTo(ChargingTariff2) > 0;

        #endregion

        #region Operator >= (ChargingTariff1, ChargingTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTariff1">A charging tariff.</param>
        /// <param name="ChargingTariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingTariff? ChargingTariff1,
                                           ChargingTariff? ChargingTariff2)

            => !(ChargingTariff1 < ChargingTariff2);

        #endregion

        #endregion

        #region IComparable<ChargingTariff> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging tariffs.
        /// </summary>
        /// <param name="Object">A charging tariff to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingTariff chargingTariff
                   ? CompareTo(chargingTariff)
                   : throw new ArgumentException("The given object is not a charging tariff!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingTariff)

        /// <summary>
        /// Compares two charging tariffs.
        /// </summary>
        /// <param name="ChargingTariff">A charging tariff to compare with.</param>
        public Int32 CompareTo(ChargingTariff? ChargingTariff)
        {

            if (ChargingTariff is null)
                throw new ArgumentNullException(nameof(ChargingTariff), "The given charging tariff must not be null!");

            var c = Id.         CompareTo(ChargingTariff.Id);

            if (c == 0)
                c = Currency.   CompareTo(ChargingTariff.Currency);

            //if (c == 0)
            //    c = Created.    CompareTo(Tariff.Created);

            //if (c == 0)
            //    c = LastUpdated.CompareTo(Tariff.LastUpdated);

            // TariffElements
            // 
            // TariffType
            // TariffAltText
            // TariffAltURL
            // MinPrice
            // MaxPrice
            // Start
            // End
            // EnergyMix

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingTariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging tariffs for equality.
        /// </summary>
        /// <param name="Object">A charging tariff to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingTariff chargingTariff &&
                   Equals(chargingTariff);

        #endregion

        #region Equals(ChargingTariff)

        /// <summary>
        /// Compares two charging tariffs for equality.
        /// </summary>
        /// <param name="ChargingTariff">A charging tariff to compare with.</param>
        public Boolean Equals(ChargingTariff? ChargingTariff)

            => ChargingTariff is not null &&

               Id.                     Equals(ChargingTariff.Id)           &&
               ProviderId.             Equals(ChargingTariff.ProviderId)   &&
               ProviderName.           Equals(ChargingTariff.ProviderName) &&
               Currency.               Equals(ChargingTariff.Currency)     &&
               Created.                Equals(ChargingTariff.Created)      &&
               NotBefore.              Equals(ChargingTariff.NotBefore)    &&
               Description.            Equals(ChargingTariff.Description)  &&

            ((!TariffType.HasValue    && !ChargingTariff.TariffType.HasValue) ||
              (TariffType.HasValue    &&  ChargingTariff.TariffType.HasValue    && TariffType.Value.Equals(ChargingTariff.TariffType.Value))) &&

            ((!URL.       HasValue    && !ChargingTariff.URL.       HasValue) ||
              (URL.       HasValue    &&  ChargingTariff.URL.       HasValue    && URL.       Value.Equals(ChargingTariff.URL.       Value))) &&

             ((EnergyMix  is     null &&  ChargingTariff.EnergyMix  is null)  ||
              (EnergyMix  is not null &&  ChargingTariff.EnergyMix  is not null && EnergyMix.       Equals(ChargingTariff.EnergyMix)))        &&

            ((!MinPrice.  HasValue    && !ChargingTariff.MinPrice.  HasValue) ||
              (MinPrice.  HasValue    &&  ChargingTariff.MinPrice.  HasValue    && MinPrice.  Value.Equals(ChargingTariff.MinPrice.  Value))) &&

            ((!MaxPrice.  HasValue    && !ChargingTariff.MaxPrice.  HasValue) ||
              (MaxPrice.  HasValue    &&  ChargingTariff.MaxPrice.  HasValue    && MaxPrice.  Value.Equals(ChargingTariff.MaxPrice.  Value))) &&

            ((!NotAfter.  HasValue    && !ChargingTariff.NotAfter.  HasValue) ||
              (NotAfter.  HasValue    &&  ChargingTariff.NotAfter.  HasValue    && NotAfter.  Value.Equals(ChargingTariff.NotAfter.  Value))) &&

               Replaces.      Count().Equals(ChargingTariff.Replaces.Count())                                   &&
               Replaces.      All(chargingTariffId => ChargingTariff.Replaces.      Contains(chargingTariffId)) &&

               References.    Count().Equals(ChargingTariff.Replaces.Count())                                   &&
               References.    All(chargingTariffId => ChargingTariff.Replaces.      Contains(chargingTariffId)) &&

               TariffElements.Count().Equals(ChargingTariff.TariffElements.Count())                             &&
               TariffElements.All(tariffElement    => ChargingTariff.TariffElements.Contains(tariffElement))    &&

               Description.   Count().Equals(ChargingTariff.Description.Count())                                &&
               Description.   All(displayText      => ChargingTariff.Description.   Contains(displayText))      &&

               base.Equals(ChargingTariff);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => hashCode ^
               Signatures.CalcHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,          " (",
                   //CountryCode, "-",
                   //PartyId,     ") ",
                   Currency,    ", ",

                   TariffElements.Count(), " tariff element(s), ",

                   Description.Any()
                       ? "text: " + Description.First().Text + ", "
                       : "",

                   URL.HasValue
                       ? "url: " + URL.Value + ", "
                       : ""

                   //EnergyMix is not null
                   //    ? "energy mix: " + EnergyMix + ", "
                   //    : ""

                 //  "last updated: " + LastUpdated.ToIso8601()

               );

        #endregion


    }

}
