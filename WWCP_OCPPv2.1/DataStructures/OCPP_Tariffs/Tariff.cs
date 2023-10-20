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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A read-only signable charging tariff.
    /// </summary>
    /// 
    /// <remarks>
    /// This is based on OICP v2.2.1 as this might become future part of OCPP v2.1 (draft 3++).
    ///  - Removed property "CountryCode" and "PartyId", as both are just an OCPI implementation detail.
    ///  - Removed property "LastUpdated", as this data structure must be immutable for legal reasons!
    ///  - Renamed property "Start" to a _mandatory_ "NotBefore", to be more aligned with certificates.
    ///  - Renamed property "End" to "NotAfter", to be more aligned with certificates.
    ///  - Renamed property "TariffAltText" to "Description".
    ///  - Renamed property "TariffAltURL" to "URL"
    ///  
    ///  - Added property "Created" to track when the tariff was created.
    ///  - Added property "ProviderId" as the unique identification of the e-mobility provider responsible for this tariff.
    ///  - Added property "ProviderName" as the multi-language name of the e-mobility provider responsible for this tariff.
    ///  - Added property "EVSEIds" as a list of EVSE identifications, this tariff is valid for.
    /// </remarks>
    public class Tariff : ACustomSignableData,
                          IHasId<Tariff_Id>,
                          IEquatable<Tariff>,
                          IComparable<Tariff>,
                          IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/tariff");

        #endregion

        #region Properties

        /// <summary>
        /// The global unique and unique in time identification of the charging tariff.
        /// </summary>
        [Mandatory]
        public   Tariff_Id                   Id                   { get; }

        /// <summary>
        /// The timestamp when this tariff was created.
        /// </summary>
        [Mandatory] //, NonStandard("Pagination")]
        public   DateTime                    Created              { get; }

        /// <summary>
        /// This tariff replaces the tariff specificed by this tariff identification.
        /// </summary>
        [Optional]
        public   Tariff_Id?                  Replaces             { get; }

        /// <summary>
        /// The unique identification of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public   String                      ProviderId           { get; }

        /// <summary>
        /// The multi-language name of the e-mobility provider responsible for this tariff.
        /// </summary>
        [Mandatory]
        public   IEnumerable<DisplayText>    ProviderName         { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public   Currency                    Currency             { get; }

        /// <summary>
        /// The optional tariff type allows to distinguish between charging preferences.
        /// When omitted, this tariff is valid for all charging sessions.
        /// </summary>
        [Optional]
        public   TariffType?                 TariffType           { get; }

        /// <summary>
        /// The optional multi-language tariff description.
        /// </summary>
        [Optional]
        public   IEnumerable<DisplayText>    Description          { get; }

        /// <summary>
        /// The optional informative (not legally binding) URL to a web page that contains an
        /// explanation of the tariff information in human readable form.
        /// </summary>
        [Optional]
        public   URL?                        URL                  { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this tariff is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<GlobalEVSE_Id>    EVSEIds              { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will at least cost
        /// this amount. This is different from a FLAT fee (Start Tariff, Transaction Fee),
        /// as a FLAT fee is a fixed amount that has to be paid for any Charging Session.
        /// A minimum price indicates that when the cost of a charging session is lower
        /// than this amount, the cost of the charging session will be equal to this amount.
        /// </summary>
        [Optional]
        public   Price?                      MinPrice             { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will NOT
        /// cost more than this amount.
        /// </summary>
        [Optional]
        public   Price?                      MaxPrice             { get; }

        /// <summary>
        /// The enumeration of tariff elements.
        /// </summary>
        [Mandatory]
        public   IEnumerable<TariffElement>  TariffElements       { get; }

        /// <summary>
        /// The timestamp when this tariff becomes active (UTC).
        /// Typically used for a new tariff that is already given with the charging location,
        /// before it becomes active.
        /// </summary>
        [Mandatory]
        public   DateTime                    NotBefore            { get; }

        /// <summary>
        /// The optional timestamp after which this tariff is no longer valid (UTC).
        /// Typically used when this tariff is going to be replaced with a different tariff
        /// in the near future.
        /// </summary>
        [Optional]
        public   DateTime?                   NotAfter             { get; }

        /// <summary>
        /// Optional details on the energy supplied with this tariff.
        /// </summary>
        [Optional]
        public   EnergyMix?                  EnergyMix            { get;  }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging tariff.
        /// </summary>
        /// <param name="Id">A global unique and unique in time identification of the charging tariff.</param>
        /// <param name="ProviderId">An unique identification of the e-mobility provider responsible for this tariff.</param>
        /// <param name="ProviderName">An multi-language name of the e-mobility provider responsible for this tariff.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="TariffElements">An enumeration of tariff elements.</param>
        /// 
        /// <param name="Created">An optional timestamp when this tariff was created.</param>
        /// <param name="Replaces">This tariff replaces the tariff specificed by this tariff identification.</param>
        /// <param name="TariffType">An optional tariff type, that allows to distinguish between charging preferences. When omitted, this tariff is valid for all charging sessions.</param>
        /// <param name="Description">An optional multi-language tariff description.</param>
        /// <param name="URL">An optional informative (not legally binding) URL to a web page that contains an explanation of the tariff information in human readable form.</param>
        /// <param name="MinPrice">When this optional field is set, a charging session with this tariff will at least cost this amount.</param>
        /// <param name="MaxPrice">When this optional field is set, a charging session with this tariff will NOT cost more than this amount.</param>
        /// <param name="NotBefore">An optional timestamp when this tariff becomes active (UTC).</param>
        /// <param name="NotAfter">An optional timestamp after which this tariff is no longer valid (UTC).</param>
        /// <param name="EnergyMix">Optional details on the energy supplied with this tariff.</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Tariff(Tariff_Id                    Id,
                      String                       ProviderId,
                      IEnumerable<DisplayText>     ProviderName,
                      Currency                     Currency,
                      IEnumerable<TariffElement>   TariffElements,

                      DateTime?                    Created       = null,
                      Tariff_Id?                   Replaces      = null,
                      TariffType?                  TariffType    = null,
                      IEnumerable<DisplayText>?    Description   = null,
                      URL?                         URL           = null,
                      IEnumerable<GlobalEVSE_Id>?  EVSEIds       = null,
                      Price?                       MinPrice      = null,
                      Price?                       MaxPrice      = null,
                      DateTime?                    NotBefore     = null,
                      DateTime?                    NotAfter      = null,
                      EnergyMix?                   EnergyMix     = null,

                      IEnumerable<KeyPair>?        SignKeys      = null,
                      IEnumerable<SignInfo>?       SignInfos     = null,
                      IEnumerable<Signature>?      Signatures    = null,

                      CustomData?                  CustomData    = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            if (!TariffElements.Any())
                throw new ArgumentNullException(nameof(TariffElements), "The given enumeration of tariff elements must not be null or empty!");

            this.Id              = Id;
            this.ProviderId      = ProviderId;
            this.ProviderName    = ProviderName.  Distinct();
            this.Currency        = Currency;
            this.TariffElements  = TariffElements.Distinct();

            this.Created         = Created   ?? Timestamp.Now;
            this.Replaces        = Replaces;
            this.TariffType      = TariffType;
            this.Description     = Description?.  Distinct() ?? Array.Empty<DisplayText>();
            this.URL             = URL;
            this.EVSEIds         = EVSEIds?.      Distinct() ?? Array.Empty<GlobalEVSE_Id>();
            this.MinPrice        = MinPrice;
            this.MaxPrice        = MaxPrice;
            this.NotBefore       = NotBefore ?? this.Created;
            this.NotAfter        = NotAfter;
            this.EnergyMix       = EnergyMix;

            unchecked
            {

                hashCode = this.Id.            GetHashCode()       * 59 ^
                           this.ProviderId.    GetHashCode()       * 53 ^
                           this.ProviderName.  GetHashCode()       * 47 ^
                           this.Currency.      GetHashCode()       * 43 ^
                           this.TariffElements.CalcHashCode()      * 41 ^
                           this.Created.       GetHashCode()       * 37 ^
                          (this.Replaces?.     GetHashCode() ?? 0) * 31 ^
                          (this.TariffType?.   GetHashCode() ?? 0) * 29 ^
                           this.Description.   CalcHashCode()      * 23 ^
                          (this.URL?.          GetHashCode() ?? 0) * 19 ^
                           this.EVSEIds.       CalcHashCode()      * 17 ^
                          (this.MinPrice?.     GetHashCode() ?? 0) * 13 ^
                          (this.MaxPrice?.     GetHashCode() ?? 0) * 11 ^
                           this.NotBefore.     GetHashCode()       *  7 ^
                          (this.NotAfter?.     GetHashCode() ?? 0) *  5 ^
                           this.EnergyMix?.    GetHashCode() ?? 0  *  3 ^
                           this.Signatures.    CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, TariffIdURL = null, CustomTariffParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Tariff Parse(JObject                               JSON,
                                   //CountryCode?                          CountryCodeURL       = null,
                                   //Party_Id?                             PartyIdURL           = null,
                                   Tariff_Id?                            TariffIdURL          = null,
                                   CustomJObjectParserDelegate<Tariff>?  CustomTariffParser   = null)
        {

            if (TryParse(JSON,
                         out var tariff,
                         out var errorResponse,
                         //CountryCodeURL,
                         //PartyIdURL,
                         TariffIdURL,
                         CustomTariffParser))
            {
                return tariff!;
            }

            throw new ArgumentException("The given JSON representation of a tariff is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Tariff, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out Tariff?  Tariff,
                                       out String?  ErrorResponse)

            => TryParse(JSON,
                        out Tariff,
                        out ErrorResponse,
                        null,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       out Tariff?                           Tariff,
                                       out String?                           ErrorResponse,
                                       Tariff_Id?                            TariffIdURL          = null,
                                       CustomJObjectParserDelegate<Tariff>?  CustomTariffParser   = null)
        {

            try
            {

                Tariff = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                [optional]

                if (JSON.ParseOptional("id",
                                       "tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? TariffIdBody,
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

                #region Parse ProviderId        [mandatory]

                if (!JSON.ParseMandatoryText("providerId",
                                             "provider identification",
                                             out String ProviderId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ProviderName      [mandatory]

                if (!JSON.ParseMandatoryJSON("provider_name",
                                             "provider name",
                                             DisplayText.TryParse,
                                             out IEnumerable<DisplayText> ProviderName,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Currency          [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TariffElements    [mandatory]

                if (!JSON.ParseMandatoryHashSet("elements",
                                                "tariff elements",
                                                TariffElement.TryParse,
                                                out HashSet<TariffElement> TariffElements,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse Created           [mandatory]

                if (!JSON.ParseMandatory("created",
                                         "created",
                                         out DateTime Created,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Replaces          [optional]

                if (!JSON.ParseOptional("replaces",
                                        "replaces tariff",
                                        Tariff_Id.TryParse,
                                        out Tariff_Id? Replaces,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TariffType        [optional]

                if (JSON.ParseOptionalEnum("type",
                                           "tariff type",
                                           out TariffType? TariffType,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Description       [optional]

                if (JSON.ParseOptionalJSON("description",
                                           "tariff description",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Description,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse URL               [optional]

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

                #region Parse EVSEIds           [optional]

                if (JSON.ParseOptionalHashSet("evseIds",
                                              "EVSE identifications",
                                              GlobalEVSE_Id.TryParse,
                                              out HashSet<GlobalEVSE_Id> EVSEIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinPrice          [optional]

                if (JSON.ParseOptionalJSON("min_price",
                                           "minimum price",
                                           Price.TryParse,
                                           out Price? MinPrice,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxPrice          [optional]

                if (JSON.ParseOptionalJSON("max_price",
                                           "maximum price",
                                           Price.TryParse,
                                           out Price? MaxPrice,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotBefore         [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTime? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter          [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMix         [optional]

                if (JSON.ParseOptionalJSON("energy_mix",
                                           "energy mix",
                                           OCPPv2_1.EnergyMix.TryParse,
                                           out EnergyMix EnergyMix,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures              [optional, OCPP_CSE]

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

                #region CustomData              [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Tariff = new Tariff(
                             TariffIdBody ?? TariffIdURL!.Value,
                             ProviderId,
                             ProviderName,
                             Currency,
                             TariffElements,

                             Created,
                             Replaces,
                             TariffType,
                             Description,
                             URL,
                             EVSEIds,
                             MinPrice,
                             MaxPrice,
                             NotBefore,
                             NotAfter,
                             EnergyMix,

                             null,
                             null,
                             Signatures,
                             CustomData
                         );

                if (CustomTariffParser is not null)
                    Tariff = CustomTariffParser(JSON,
                                                Tariff);

                return true;

            }
            catch (Exception e)
            {
                Tariff         = default;
                ErrorResponse  = "The given JSON representation of a tariff is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Tariff>?               CustomTariffSerializer                = null,
                              CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                              CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                              CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer    = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",             Id.              ToString()),
                                 new JProperty("providerId",                      ProviderId),
                                 new JProperty("providerName",   new JArray(ProviderName.  Select(providerName  => providerName. ToJSON(CustomDisplayTextSerializer)))),
                                 new JProperty("currency",       Currency.        ToString()),

                           TariffElements.Any()
                               ? new JProperty("elements",       new JArray(TariffElements.Select(tariffElement => tariffElement.ToJSON(CustomTariffElementSerializer,
                                                                                                                                        CustomPriceComponentSerializer,
                                                                                                                                        CustomTariffRestrictionsSerializer))))
                               : null,

                                 new JProperty("notBefore",      NotBefore.       ToIso8601()),


                                 new JProperty("created",        Created.         ToIso8601()),

                           Replaces.  HasValue
                               ? new JProperty("replaces",       Replaces.        ToString())
                               : null,

                           TariffType.HasValue
                               ? new JProperty("type",           TariffType.Value.ToString())
                               : null,

                           Description.Any()
                               ? new JProperty("description",    new JArray(Description.Select(description      => description.  ToJSON(CustomDisplayTextSerializer))))
                               : null,

                           URL.     HasValue
                               ? new JProperty("url",            URL.  ToString())
                               : null,

                           EVSEIds.Any()
                               ? new JProperty("evseIds",        new JArray(EVSEIds.    Select(evseId           => evseId.       ToString())))
                               : null,

                           MinPrice.HasValue
                               ? new JProperty("minPrice",       MinPrice.Value.ToJSON(CustomPriceSerializer))
                               : null,

                           MaxPrice.HasValue
                               ? new JProperty("maxPrice",       MaxPrice.Value.ToJSON(CustomPriceSerializer))
                               : null,

                           NotAfter.HasValue
                               ? new JProperty("notAftere",      NotAfter.Value.ToIso8601())
                               : null,

                           EnergyMix is not null
                               ? new JProperty("energyMix",      EnergyMix.     ToJSON(CustomEnergyMixSerializer,
                                                                                       CustomEnergySourceSerializer,
                                                                                       CustomEnvironmentalImpactSerializer))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",     new JArray(Signatures. Select(signature        => signature.    ToJSON(CustomSignatureSerializer,
                                                                                                                                        CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomTariffSerializer is not null
                       ? CustomTariffSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Tariff Clone()

            => new (Id.            Clone,
                    new String(ProviderId.ToCharArray()),
                    ProviderName.  Select(displayText   => displayText.  Clone()).ToArray(),
                    Currency.      Clone,
                    TariffElements.Select(tariffElement => tariffElement.Clone()).ToArray(),

                    Created,
                    Replaces?.     Clone,
                    TariffType?.   Clone,
                    Description.   Select(displayText   => displayText.  Clone()).ToArray(),
                    URL?.          Clone,
                    EVSEIds.       Select(evseId        => evseId.       Clone  ).ToArray(),
                    MinPrice?.     Clone(),
                    MaxPrice?.     Clone(),
                    NotBefore,
                    NotAfter,
                    EnergyMix?.    Clone(),

                    SignKeys,
                    SignInfos,
                    Signatures.    Select(signature     => signature.    Clone()).ToArray(),

                    CustomData);

        #endregion


        #region Operator overloading

        #region Operator == (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tariff? Tariff1,
                                           Tariff? Tariff2)
        {

            if (Object.ReferenceEquals(Tariff1, Tariff2))
                return true;

            if (Tariff1 is null || Tariff2 is null)
                return false;

            return Tariff1.Equals(Tariff2);

        }

        #endregion

        #region Operator != (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff? Tariff1,
                                           Tariff? Tariff2)

            => !(Tariff1 == Tariff2);

        #endregion

        #region Operator <  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tariff? Tariff1,
                                          Tariff? Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) < 0;

        #endregion

        #region Operator <= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff? Tariff1,
                                           Tariff? Tariff2)

            => !(Tariff1 > Tariff2);

        #endregion

        #region Operator >  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tariff? Tariff1,
                                          Tariff? Tariff2)

            => Tariff1 is null
                   ? throw new ArgumentNullException(nameof(Tariff1), "The given tariff must not be null!")
                   : Tariff1.CompareTo(Tariff2) > 0;

        #endregion

        #region Operator >= (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A charging tariff.</param>
        /// <param name="Tariff2">Another charging tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff? Tariff1,
                                           Tariff? Tariff2)

            => !(Tariff1 < Tariff2);

        #endregion

        #endregion

        #region IComparable<Tariff> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging tariffs.
        /// </summary>
        /// <param name="Object">A charging tariff to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Tariff tariff
                   ? CompareTo(tariff)
                   : throw new ArgumentException("The given object is not a charging tariff!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Tariff)

        /// <summary>
        /// Compares two charging tariffs.
        /// </summary>
        /// <param name="Tariff">A charging tariff to compare with.</param>
        public Int32 CompareTo(Tariff? Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given charging tariff must not be null!");

            var c = Id.         CompareTo(Tariff.Id);

            if (c == 0)
                c = Currency.   CompareTo(Tariff.Currency);

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

        #region IEquatable<Tariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging tariffs for equality.
        /// </summary>
        /// <param name="Object">A charging tariff to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Tariff tariff &&
                   Equals(tariff);

        #endregion

        #region Equals(Tariff)

        /// <summary>
        /// Compares two charging tariffs for equality.
        /// </summary>
        /// <param name="Tariff">A charging tariff to compare with.</param>
        public Boolean Equals(Tariff? Tariff)

            => Tariff is not null &&

               Id.                     Equals(Tariff.Id)          &&
               Currency.               Equals(Tariff.Currency)    &&

            ((!TariffType.HasValue    && !Tariff.TariffType.HasValue) ||
              (TariffType.HasValue    &&  Tariff.TariffType.HasValue    && TariffType.Value.Equals(Tariff.TariffType.Value))) &&

            ((!TariffType.HasValue    && !Tariff.TariffType.HasValue) ||
              (TariffType.HasValue    &&  Tariff.TariffType.HasValue    && TariffType.Value.Equals(Tariff.TariffType.Value))) &&

            ((!MinPrice.  HasValue    && !Tariff.MinPrice.  HasValue) ||
              (MinPrice.  HasValue    &&  Tariff.MinPrice.  HasValue    && MinPrice.  Value.Equals(Tariff.MinPrice.  Value))) &&

            ((!MaxPrice.  HasValue    && !Tariff.MaxPrice.  HasValue) ||
              (MaxPrice.  HasValue    &&  Tariff.MaxPrice.  HasValue    && MaxPrice.  Value.Equals(Tariff.MaxPrice.  Value))) &&

            NotBefore.     Equals(Tariff.NotBefore) &&

            ((!NotAfter.       HasValue    && !Tariff.NotAfter.       HasValue) ||
              (NotAfter.       HasValue    &&  Tariff.NotAfter.       HasValue    && NotAfter.       Value.Equals(Tariff.NotAfter.       Value))) &&

             ((EnergyMix  is     null &&  Tariff.EnergyMix  is null)  ||
              (EnergyMix  is not null &&  Tariff.EnergyMix  is not null && EnergyMix.       Equals(Tariff.EnergyMix)))        &&

               TariffElements.Count().Equals(Tariff.TariffElements.Count())     &&
               TariffElements.All(tariffElement => Tariff.TariffElements.Contains(tariffElement)) &&

               Description.Count().Equals(Tariff.Description.Count())     &&
               Description.All(displayText => Tariff.Description.Contains(displayText));

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
