﻿/*
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
    /// A read-only signable charging ticket enabling secure and privacy-aware offline charging.
    /// </summary>
    public class ChargingTicket : ACustomSignableData,
                                  IHasId<ChargingTicket_Id>,
                                  IEquatable<ChargingTicket>,
                                  IComparable<ChargingTicket>,
                                  IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/ticket");

        #endregion

        #region Properties

        /// <summary>
        /// The global unique and unique in time identification of the charging ticket.
        /// </summary>
        [Mandatory]
        public ChargingTicket_Id                Id                         { get; }

        /// <summary>
        /// The timestamp when this charging ticket was created.
        /// </summary>
        [Mandatory] //, NonStandard("Pagination")]
        public DateTime                         Created                    { get; }

        /// <summary>
        /// The unique identification of the e-mobility provider responsible for this charging ticket.
        /// </summary>
        [Mandatory]
        public Provider_Id                      ProviderId                 { get; }

        /// <summary>
        /// The multi-language name of the e-mobility provider responsible for this charging ticket.
        /// </summary>
        [Mandatory]
        public DisplayTexts                     ProviderName               { get; }

        /// <summary>
        /// The optional URL of the e-mobility provider responsible for this charging ticket.
        /// </summary>
        [Optional]
        public URL?                             ProviderURL                { get; }

        /// <summary>
        /// The optional multi-language charging ticket description.
        /// </summary>
        [Optional]
        public DisplayTexts                     Description                { get; }


        /// <summary>
        /// The timestamp when this charging ticket becomes active (UTC).
        /// </summary>
        [Mandatory]
        public DateTime                         NotBefore                  { get; }

        /// <summary>
        /// The timestamp after which this charging ticket is no longer valid (UTC).
        /// </summary>
        [Mandatory]
        public DateTime                         NotAfter                   { get; }


        /// <summary>
        /// The public key of the EV driver.
        /// </summary>
        [Mandatory]
        public PublicKey                        DriverPublicKey            { get; }

        /// <summary>
        /// The enumeration of tariffs that can be used with this charging ticket.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingTariff>      ChargingTariffs            { get; }


        /// <summary>
        /// An optional enumeration of charging station operators, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<Operator_Id>         ValidOperators             { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPool_Id>     ValidChargingPools         { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingStation_Id>  ValidChargingStations      { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<GlobalEVSE_Id>       ValidEVSEIds               { get; }


        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPool_Id>     InvalidChargingPools       { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingStation_Id>  InvalidChargingStations    { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<GlobalEVSE_Id>       InvalidEVSEIds             { get; }


        /// <summary>
        /// The allowed current type: AC, DC, or both.
        /// </summary>
        [Mandatory]
        public CurrentTypes                     AllowedCurrentType         { get; }

        /// <summary>
        /// The maximum allowed consumed energy during a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public WattHour?                        MaxKWh                     { get; }

        /// <summary>
        /// The maximum allowed charging power during a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public Watt?                            MaxKW                      { get; }

        /// <summary>
        /// The maximum allowed current of a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public Ampere?                          MaxCurrent                 { get; }

        /// <summary>
        /// The maximum allowed duration of a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public TimeSpan?                        MaxDuration                { get; }

        /// <summary>
        /// The maximum allowed price of a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public Price?                           MaxPrice                   { get; }

        /// <summary>
        /// The day(s) of the week when this charging ticket allows charging.
        /// </summary>
        [Optional]
        public IEnumerable<DayOfWeek>           DaysOfWeek                 { get; }


        /// <summary>
        /// How the charging ticket can be used for multiple charging sessions.
        /// </summary>
        [Mandatory]
        public ChargingTicketMultiUsages        MultiUsage                 { get; }

        /// <summary>
        /// The day(s) of the week when this charging ticket allows charging.
        /// </summary>
        [Mandatory]
        public ChargingTicketValidationMethods  ValidationMethod           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging ticket.
        /// </summary>
        /// <param name="Id">A global unique and unique in time identification of the charging ticket.</param>
        /// <param name="ProviderId">An unique identification of the e-mobility provider responsible for this ticket.</param>
        /// <param name="ProviderName">An multi-language name of the e-mobility provider responsible for this ticket.</param>
        /// 
        /// <param name="Created">An optional timestamp when this ticket was created.</param>
        /// <param name="Description">An optional multi-language ticket description.</param>
        /// <param name="NotBefore">An optional timestamp when this ticket becomes active (UTC).</param>
        /// <param name="NotAfter">An optional timestamp after which this ticket is no longer valid (UTC).</param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingTicket(ChargingTicket_Id                 Id,
                              Provider_Id                       ProviderId,
                              DisplayTexts                      ProviderName,
                              PublicKey                         DriverPublicKey,
                              IEnumerable<ChargingTariff>       ChargingTariffs,

                              URL?                              ProviderURL               = null,
                              DisplayTexts?                     Description               = null,
                              DateTime?                         Created                   = null,
                              DateTime?                         NotBefore                 = null,
                              DateTime?                         NotAfter                  = null,

                              IEnumerable<Operator_Id>?         ValidOperators            = null,
                              IEnumerable<ChargingPool_Id>?     ValidChargingPools        = null,
                              IEnumerable<ChargingStation_Id>?  ValidChargingStations     = null,
                              IEnumerable<GlobalEVSE_Id>?       ValidEVSEIds              = null,

                              IEnumerable<ChargingPool_Id>?     InvalidChargingPools      = null,
                              IEnumerable<ChargingStation_Id>?  InvalidChargingStations   = null,
                              IEnumerable<GlobalEVSE_Id>?       InvalidEVSEIds            = null,

                              CurrentTypes?                     AllowedCurrentType        = null,
                              TimeSpan?                         MaxDuration               = null,
                              WattHour?                         MaxKWh                    = null,
                              Watt?                             MaxKW                     = null,
                              Ampere?                           MaxCurrent                = null,
                              Price?                            MaxPrice                  = null,
                              IEnumerable<DayOfWeek>?           DaysOfWeek                = null,

                              ChargingTicketMultiUsages?        MultiUsage                = null,
                              ChargingTicketValidationMethods?  ValidationMethod          = null,

                              IEnumerable<KeyPair>?             SignKeys                  = null,
                              IEnumerable<SignInfo>?            SignInfos                 = null,
                              IEnumerable<Signature>?           Signatures                = null,

                              CustomData?                       CustomData                = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            if (!ChargingTariffs.Any())
                throw new ArgumentNullException(nameof(ChargingTariffs), "The given enumeration of tariffs must not be null or empty!");

            this.Id                       = Id;
            this.ProviderId               = ProviderId;
            this.ProviderName             = ProviderName;
            this.DriverPublicKey          = DriverPublicKey;
            this.ChargingTariffs          = ChargingTariffs.         Distinct();

            this.ProviderURL              = ProviderURL;
            this.Description              = Description                         ?? DisplayTexts.Empty;
            this.Created                  = Created                             ?? Timestamp.Now;
            this.NotBefore                = NotBefore                           ?? this.Created;
            this.NotAfter                 = NotAfter                            ?? this.Created + TimeSpan.FromDays(3);

            this.ValidOperators           = ValidOperators?.         Distinct() ?? Array.Empty<Operator_Id>();
            this.ValidChargingPools       = ValidChargingPools?.     Distinct() ?? Array.Empty<ChargingPool_Id>();
            this.ValidChargingStations    = ValidChargingStations?.  Distinct() ?? Array.Empty<ChargingStation_Id>();
            this.ValidEVSEIds             = ValidEVSEIds?.           Distinct() ?? Array.Empty<GlobalEVSE_Id>();

            this.InvalidChargingPools     = InvalidChargingPools?.   Distinct() ?? Array.Empty<ChargingPool_Id>();
            this.InvalidChargingStations  = InvalidChargingStations?.Distinct() ?? Array.Empty<ChargingStation_Id>();
            this.InvalidEVSEIds           = InvalidEVSEIds?.         Distinct() ?? Array.Empty<GlobalEVSE_Id>();

            this.AllowedCurrentType       = AllowedCurrentType                  ?? CurrentTypes.ACDC;
            this.MaxDuration              = MaxDuration;
            this.MaxKWh                   = MaxKWh;
            this.MaxKW                    = MaxKW;
            this.MaxCurrent               = MaxCurrent;
            this.MaxPrice                 = MaxPrice;
            this.DaysOfWeek               = DaysOfWeek?.             Distinct() ?? Array.Empty<DayOfWeek>();

            this.MultiUsage               = MultiUsage                          ?? ChargingTicketMultiUsages.      Allowed;
            this.ValidationMethod         = ValidationMethod                    ?? ChargingTicketValidationMethods.OfflineChargingAllowed;

            unchecked
            {

                hashCode = this.Id.                     GetHashCode()       * 89 ^
                           this.ProviderId.             GetHashCode()       * 83 ^
                           this.ProviderName.           GetHashCode()       * 79 ^
                           this.DriverPublicKey.        GetHashCode()       * 73 ^
                           this.ChargingTariffs.                CalcHashCode()      * 71 ^

                          (this.ProviderURL?.           GetHashCode() ?? 0) * 67 ^
                           this.Description.            CalcHashCode()      * 61 ^
                           this.Created.                GetHashCode()       * 59 ^
                           this.NotBefore.              GetHashCode()       * 53 ^
                           this.NotAfter.               GetHashCode()       * 47 ^

                           this.ValidOperators.         CalcHashCode()      * 43 ^
                           this.ValidChargingPools.     CalcHashCode()      * 37 ^
                           this.ValidChargingStations.  CalcHashCode()      * 31 ^
                           this.ValidEVSEIds.           CalcHashCode()      * 29 ^

                           this.InvalidChargingPools.   CalcHashCode()      * 23 ^
                           this.InvalidChargingStations.CalcHashCode()      * 19 ^
                           this.InvalidEVSEIds.         CalcHashCode()      * 17 ^

                          (this.MaxDuration?.           GetHashCode() ?? 0) * 17 ^
                          (this.MaxKWh?.                GetHashCode() ?? 0) * 13 ^
                          (this.MaxKW?.                 GetHashCode() ?? 0) * 11 ^
                          (this.MaxCurrent?.            GetHashCode() ?? 0) *  7 ^
                          (this.MaxPrice?.              GetHashCode() ?? 0) *  5 ^
                           this.DaysOfWeek.             CalcHashCode()      *  3 ^

                           this.MultiUsage.             GetHashCode()       *  3 ^
                           this.ValidationMethod.       GetHashCode()       *  3 ^

                           this.Signatures.             CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, TicketIdURL = null, CustomTicketParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging ticket.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingTicketIdURL">An optional charging ticket identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTicketParser">A delegate to parse custom charging ticket JSON objects.</param>
        public static ChargingTicket Parse(JObject                                       JSON,
                                           ChargingTicket_Id?                            ChargingTicketIdURL   = null,
                                           CustomJObjectParserDelegate<ChargingTicket>?  CustomTicketParser    = null)
        {

            if (TryParse(JSON,
                         out var ticket,
                         out var errorResponse,
                         ChargingTicketIdURL,
                         CustomTicketParser))
            {
                return ticket!;
            }

            throw new ArgumentException("The given JSON representation of a charging ticket is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingTicket, out ErrorResponse, TicketIdURL = null, CustomTicketParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging ticket.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingTicket">The parsed charging ticket.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out ChargingTicket?  ChargingTicket,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out ChargingTicket,
                        out ErrorResponse,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging ticket.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingTicket">The parsed charging ticket.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="TicketIdURL">An optional ticket identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTicketParser">A delegate to parse custom ticket JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out ChargingTicket?                           ChargingTicket,
                                       out String?                                   ErrorResponse,
                                       ChargingTicket_Id?                            TicketIdURL          = null,
                                       CustomJObjectParserDelegate<ChargingTicket>?  CustomTicketParser   = null)
        {

            try
            {

                ChargingTicket = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                 [optional]

                if (JSON.ParseOptional("id",
                                       "ticket identification",
                                       ChargingTicket_Id.TryParse,
                                       out ChargingTicket_Id? TicketIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!TicketIdURL.HasValue && !TicketIdBody.HasValue)
                {
                    ErrorResponse = "The ticket identification is missing!";
                    return false;
                }

                if (TicketIdURL.HasValue && TicketIdBody.HasValue && TicketIdURL.Value != TicketIdBody.Value)
                {
                    ErrorResponse = "The optional ticket identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse ProviderId         [mandatory]

                if (!JSON.ParseMandatory("providerId",
                                         "provider identification",
                                         Provider_Id.TryParse,
                                         out Provider_Id ProviderId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ProviderName       [mandatory]

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

                #region Parse DriverPublicKey    [mandatory]

                if (!JSON.ParseMandatoryJSON("driverPublicKey",
                                             "driver public key",
                                             PublicKey.TryParse,
                                             out PublicKey? DriverPublicKey,
                                             out ErrorResponse) ||
                     DriverPublicKey is null)
                {
                    return false;
                }

                #endregion

                #region Parse ChargingTariffs    [mandatory]

                if (!JSON.ParseMandatoryHashSet("chargingTariffs",
                                                "charging tariffs",
                                                ChargingTariff.TryParse,
                                                out HashSet<ChargingTariff> ChargingTariffs,
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
                                        "replaces ticket",
                                        ChargingTicket_Id.TryParse,
                                        out ChargingTicket_Id? Replaces,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse References        [optional]

                if (!JSON.ParseOptional("references",
                                        "references ticket",
                                        ChargingTicket_Id.TryParse,
                                        out ChargingTicket_Id? References,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TicketType        [optional]

                //if (JSON.ParseOptionalEnum("type",
                //                           "ticket type",
                //                           out TicketType? TicketType,
                //                           out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                #endregion

                #region Parse Description       [optional]

                if (JSON.ParseOptionalJSON("description",
                                           "ticket description",
                                           DisplayTexts.TryParse,
                                           out DisplayTexts? Description,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse URL               [optional]

                if (JSON.ParseOptional("url",
                                       "ticket URL",
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


                ChargingTicket = new ChargingTicket(

                                     TicketIdBody ?? TicketIdURL!.Value,
                                     ProviderId,
                                     ProviderName,
                                     DriverPublicKey,
                                     ChargingTariffs,

                                     null, //ProviderURL,
                                     Description,
                                     Created,
                                     NotBefore,
                                     NotAfter,

                                     null, //ValidOperators,
                                     null, //ValidChargingPools,
                                     null, //ValidChargingStations,
                                     null, //ValidEVSEIds,

                                     null, //InvalidChargingPools,
                                     null, //InvalidChargingStations,
                                     null, //InvalidEVSEIds,

                                     null, //AllowedCurrentType,
                                     null, //MaxDuration,
                                     null, //MaxKWh,
                                     null, //MaxKW,
                                     null, //MaxCurrent,
                                     null, //MaxPrice,
                                     null, //DaysOfWeek,

                                     null, //MultiUsage,
                                     null, //ValidationMethod,

                                     null,
                                     null,
                                     Signatures,
                                     CustomData

                                 );

                if (CustomTicketParser is not null)
                    ChargingTicket = CustomTicketParser(JSON,
                                                        ChargingTicket);

                return true;

            }
            catch (Exception e)
            {
                ChargingTicket  = default;
                ErrorResponse   = "The given JSON representation of a charging ticket is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingTicketSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingTicketSerializer">A delegate to serialize custom ticket JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPublicKeySerializer">A delegate to serialize cryptographic public keys.</param>
        /// 
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingTicket>?       CustomChargingTicketSerializer        = null,
                              CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                              CustomJObjectSerializerDelegate<PublicKey>?            CustomPublicKeySerializer             = null,
                              CustomJObjectSerializerDelegate<ChargingTariff>?               CustomTariffSerializer                = null,
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

                                 new JProperty("id",                        Id.              ToString()),

                                 new JProperty("providerId",                                 ProviderId),
                                 new JProperty("providerName",              new JArray(ProviderName.           Select(providerName      => providerName.     ToJSON(CustomDisplayTextSerializer)))),

                           ProviderURL.HasValue
                               ? new JProperty("providerURL",               ProviderURL.Value.ToString())
                               : null,

                                 new JProperty("driverPublicKey",           DriverPublicKey.  ToJSON(CustomPublicKeySerializer,
                                                                                                     CustomCustomDataSerializer)),

                                 new JProperty("tariffs",                   new JArray(ChargingTariffs.                Select(chargingTariff    => chargingTariff.   ToJSON(CustomTariffSerializer,
                                                                                                                                                                    CustomDisplayTextSerializer,
                                                                                                                                                                    CustomPriceSerializer,
                                                                                                                                                                    CustomTariffElementSerializer,
                                                                                                                                                                    CustomPriceComponentSerializer,
                                                                                                                                                                    CustomTariffRestrictionsSerializer,
                                                                                                                                                                    CustomEnergyMixSerializer,
                                                                                                                                                                    CustomEnergySourceSerializer,
                                                                                                                                                                    CustomEnvironmentalImpactSerializer,
                                                                                                                                                                    CustomSignatureSerializer,
                                                                                                                                                                    CustomCustomDataSerializer)))),

                           Description.            Any()
                               ? new JProperty("description",               new JArray(Description.            Select(description       => description.      ToJSON(CustomDisplayTextSerializer))))
                               : null,

                                 new JProperty("created",                   Created.         ToIso8601()),
                                 new JProperty("notBefore",                 NotBefore.       ToIso8601()),
                                 new JProperty("notAftere",                 NotAfter.        ToIso8601()),

                           ValidOperators.         Any()
                               ? new JProperty("validOperators",            new JArray(ValidOperators.         Select(operatorId        => operatorId.       ToString())))
                               : null,

                           ValidChargingPools.     Any()
                               ? new JProperty("validChargingPools",        new JArray(ValidChargingPools.     Select(chargingPoolId    => chargingPoolId.   ToString())))
                               : null,

                           ValidChargingStations.  Any()
                               ? new JProperty("validChargingStations",     new JArray(ValidChargingStations.  Select(chargingStationId => chargingStationId.ToString())))
                               : null,

                           ValidEVSEIds.           Any()
                               ? new JProperty("validEVSEIds",              new JArray(ValidEVSEIds.           Select(evseId            => evseId.           ToString())))
                               : null,


                           InvalidChargingPools.   Any()
                               ? new JProperty("invalidChargingPools",      new JArray(InvalidChargingPools.   Select(chargingPoolId    => chargingPoolId.   ToString())))
                               : null,

                           InvalidChargingStations.Any()
                               ? new JProperty("invalidChargingStations",   new JArray(InvalidChargingStations.Select(chargingStationId => chargingStationId.ToString())))
                               : null,

                           InvalidEVSEIds.         Any()
                               ? new JProperty("invalidEVSEIds",            new JArray(InvalidEVSEIds.         Select(evseId            => evseId.           ToString())))
                               : null,


                                 new JProperty("allowedCurrentType",        AllowedCurrentType.AsText()),

                           MaxDuration.HasValue
                               ? new JProperty("maxDuration",               MaxDuration.Value.TotalMinutes)
                               : null,

                           MaxKWh.     HasValue
                               ? new JProperty("maxKWh",                    MaxKWh.     Value.Value)
                               : null,

                           MaxKW.      HasValue
                               ? new JProperty("maxKW",                     MaxKW.      Value.Value)
                               : null,

                           MaxCurrent. HasValue
                               ? new JProperty("maxCurrent",                MaxCurrent. Value.Value)
                               : null,

                           MaxPrice.HasValue
                               ? new JProperty("maxPrice",                  MaxPrice.   Value.ToJSON(CustomPriceSerializer))
                               : null,

                           DaysOfWeek.Any()
                               ? new JProperty("daysOfWeek",                new JArray(DaysOfWeek.             Select(dayOfWeek         => dayOfWeek.    ToString())))
                               : null,

                                 new JProperty("multiUsage",                MultiUsage.      AsText()),
                                 new JProperty("validationMethod",          ValidationMethod.AsText()),


                           Signatures.Any()
                               ? new JProperty("signatures",                new JArray(Signatures.             Select(signature         => signature.    ToJSON(CustomSignatureSerializer,
                                                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.ToJSON(CustomCustomDataSerializer))
                               : null);


            return CustomChargingTicketSerializer is not null
                       ? CustomChargingTicketSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ChargingTicket Clone()

            => new (Id,
                    ProviderId.             Clone,
                    ProviderName.           Clone(),
                    DriverPublicKey.        Clone(),
                    ChargingTariffs,

                    ProviderURL,
                    Description.            Clone(),
                    Created,
                    NotBefore,
                    NotAfter,

                    ValidOperators.         Select(operatorId        => operatorId.       Clone).ToArray(),
                    ValidChargingPools.     Select(chargingPoolId    => chargingPoolId.   Clone).ToArray(),
                    ValidChargingStations.  Select(chargingStationId => chargingStationId.Clone).ToArray(),
                    ValidEVSEIds.           Select(evseId            => evseId.           Clone).ToArray(),

                    InvalidChargingPools.   Select(chargingPoolId    => chargingPoolId.   Clone).ToArray(),
                    InvalidChargingStations.Select(chargingStationId => chargingStationId.Clone).ToArray(),
                    InvalidEVSEIds.         Select(evseId            => evseId.           Clone).ToArray(),

                    AllowedCurrentType,
                    MaxDuration,
                    MaxKWh,
                    MaxKW,
                    MaxCurrent,
                    MaxPrice,
                    DaysOfWeek.             ToArray(),

                    MultiUsage,
                    ValidationMethod,

                    SignKeys,
                    SignInfos,
                    Signatures.             Select(signature     => signature.    Clone()).ToArray(),

                    CustomData);

        #endregion


        #region Operator overloading

        #region Operator == (ChrgingTicket1, ChrgingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChrgingTicket1">A charging ticket.</param>
        /// <param name="ChrgingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingTicket? ChrgingTicket1,
                                           ChargingTicket? ChrgingTicket2)
        {

            if (Object.ReferenceEquals(ChrgingTicket1, ChrgingTicket2))
                return true;

            if (ChrgingTicket1 is null || ChrgingTicket2 is null)
                return false;

            return ChrgingTicket1.Equals(ChrgingTicket2);

        }

        #endregion

        #region Operator != (ChrgingTicket1, ChrgingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChrgingTicket1">A charging ticket.</param>
        /// <param name="ChrgingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingTicket? ChrgingTicket1,
                                           ChargingTicket? ChrgingTicket2)

            => !(ChrgingTicket1 == ChrgingTicket2);

        #endregion

        #region Operator <  (ChrgingTicket1, ChrgingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChrgingTicket1">A charging ticket.</param>
        /// <param name="ChrgingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingTicket? ChrgingTicket1,
                                          ChargingTicket? ChrgingTicket2)

            => ChrgingTicket1 is null
                   ? throw new ArgumentNullException(nameof(ChrgingTicket1), "The given ticket must not be null!")
                   : ChrgingTicket1.CompareTo(ChrgingTicket2) < 0;

        #endregion

        #region Operator <= (ChrgingTicket1, ChrgingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChrgingTicket1">A charging ticket.</param>
        /// <param name="ChrgingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingTicket? ChrgingTicket1,
                                           ChargingTicket? ChrgingTicket2)

            => !(ChrgingTicket1 > ChrgingTicket2);

        #endregion

        #region Operator >  (ChrgingTicket1, ChrgingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChrgingTicket1">A charging ticket.</param>
        /// <param name="ChrgingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingTicket? ChrgingTicket1,
                                          ChargingTicket? ChrgingTicket2)

            => ChrgingTicket1 is null
                   ? throw new ArgumentNullException(nameof(ChrgingTicket1), "The given ticket must not be null!")
                   : ChrgingTicket1.CompareTo(ChrgingTicket2) > 0;

        #endregion

        #region Operator >= (ChrgingTicket1, ChrgingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChrgingTicket1">A charging ticket.</param>
        /// <param name="ChrgingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingTicket? ChrgingTicket1,
                                           ChargingTicket? ChrgingTicket2)

            => !(ChrgingTicket1 < ChrgingTicket2);

        #endregion

        #endregion

        #region IComparable<ChargingTicket> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging tickets.
        /// </summary>
        /// <param name="Object">A charging ticket to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingTicket chargingTicket
                   ? CompareTo(chargingTicket)
                   : throw new ArgumentException("The given object is not a charging ticket!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingTicket)

        /// <summary>
        /// Compares two charging tickets.
        /// </summary>
        /// <param name="ChargingTicket">A charging ticket to compare with.</param>
        public Int32 CompareTo(ChargingTicket? ChargingTicket)
        {

            if (ChargingTicket is null)
                throw new ArgumentNullException(nameof(ChargingTicket), "The given charging ticket must not be null!");

            var c =         Id.        CompareTo(ChargingTicket.Id);

            if (c == 0) c = ProviderId.CompareTo(ChargingTicket.ProviderId);
            if (c == 0) c = Created.   CompareTo(ChargingTicket.Created);
            if (c == 0) c = NotBefore. CompareTo(ChargingTicket.NotBefore);
            if (c == 0) c = NotAfter.  CompareTo(ChargingTicket.NotAfter);

            //ProviderId,
            //ProviderName,
            //DriverPublicKey,
            //Tariffs,

            //ProviderURL
            //Description
            //Created
            //NotBefore
            //NotAfter

            //ValidOperators
            //ValidChargingPools
            //ValidChargingStations
            //ValidEVSEIds

            //InvalidChargingPools
            //InvalidChargingStations
            //InvalidEVSEIds

            //AllowedCurrentType
            //MaxDuration
            //MaxKWh
            //MaxKW
            //MaxCurrent
            //MaxPrice
            //DaysOfWeek

            //MultiUsage
            //ValidationMethod

            //SignKeys
            //SignInfos
            //Signatures

            //CustomData

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Ticket> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging tickets for equality.
        /// </summary>
        /// <param name="Object">A charging ticket to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingTicket chargingTicket &&
                   Equals(chargingTicket);

        #endregion

        #region Equals(ChargingTicket)

        /// <summary>
        /// Compares two charging tickets for equality.
        /// </summary>
        /// <param name="ChargingTicket">A charging ticket to compare with.</param>
        public Boolean Equals(ChargingTicket? ChargingTicket)

            => ChargingTicket is not null &&

               Id.                     Equals(ChargingTicket.Id)                 &&
               ProviderId.             Equals(ChargingTicket.ProviderId)         &&
               ProviderName.           Equals(ChargingTicket.ProviderName)       &&
               DriverPublicKey.        Equals(ChargingTicket.DriverPublicKey)    &&

               ProviderId.             Equals(ChargingTicket.ProviderId)         &&
               NotBefore.              Equals(ChargingTicket.NotBefore)          &&
               NotAfter.               Equals(ChargingTicket.NotAfter)           &&
               AllowedCurrentType.     Equals(ChargingTicket.AllowedCurrentType) &&
               MultiUsage.             Equals(ChargingTicket.MultiUsage)         &&
               ValidationMethod.       Equals(ChargingTicket.ValidationMethod)   &&

               ChargingTariffs.Count().Equals(ChargingTicket.ChargingTariffs.Count()) &&
               ChargingTariffs.All(displayText => ChargingTicket.ChargingTariffs.Contains(displayText)) &&

            ((!ProviderURL.HasValue     && !ChargingTicket.ProviderURL.HasValue) ||
              (ProviderURL.HasValue     &&  ChargingTicket.ProviderURL.HasValue    && ProviderURL.Value.Equals(ChargingTicket.ProviderURL.Value))) &&

             ((Description  is     null &&  ChargingTicket.Description is null)  ||
              (Description  is not null &&  ChargingTicket.Description is not null && Description.      Equals(ChargingTicket.Description)))       &&



            ((!MaxDuration.HasValue    && !ChargingTicket.MaxDuration. HasValue) ||
              (MaxDuration.HasValue    &&  ChargingTicket.MaxDuration. HasValue    && MaxDuration.Value.Equals(ChargingTicket.MaxDuration.Value))) &&

            ((!MaxKWh.     HasValue    && !ChargingTicket.MaxKWh.      HasValue) ||
              (MaxKWh.     HasValue    &&  ChargingTicket.MaxKWh.      HasValue    && MaxKWh.     Value.Equals(ChargingTicket.MaxKWh.     Value))) &&

            ((!MaxKW.      HasValue    && !ChargingTicket.MaxKW.       HasValue) ||
              (MaxKW.      HasValue    &&  ChargingTicket.MaxKW.       HasValue    && MaxKW.      Value.Equals(ChargingTicket.MaxKW.      Value))) &&

            ((!MaxCurrent. HasValue    && !ChargingTicket.MaxCurrent.  HasValue) ||
              (MaxCurrent. HasValue    &&  ChargingTicket.MaxCurrent.  HasValue    && MaxCurrent. Value.Equals(ChargingTicket.MaxCurrent. Value))) &&

            ((!MaxPrice.   HasValue    && !ChargingTicket.MaxPrice.    HasValue) ||
              (MaxPrice.   HasValue    &&  ChargingTicket.MaxPrice.    HasValue    && MaxPrice.   Value.Equals(ChargingTicket.MaxPrice.   Value))) &&

               base.Equals(ChargingTicket);


        //ValidOperators
        //ValidChargingPools
        //ValidChargingStations
        //ValidEVSEIds

        //InvalidChargingPools
        //InvalidChargingStations
        //InvalidEVSEIds

        //DaysOfWeek

        //SignKeys
        //SignInfos
        //Signatures

        //CustomData

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

            => $"{Id} ({ProviderId})";

        #endregion


    }

}