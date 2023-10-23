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
        public ChargingTicket_Id                       Id                          { get; }

        /// <summary>
        /// The timestamp when this charging ticket was created.
        /// </summary>
        [Mandatory] //, NonStandard("Pagination")]
        public DateTime                                Created                     { get; }

        /// <summary>
        /// The unique identification of the e-mobility provider responsible for this charging ticket.
        /// </summary>
        [Mandatory]
        public Provider_Id                             ProviderId                  { get; }

        /// <summary>
        /// The multi-language name of the e-mobility provider responsible for this charging ticket.
        /// </summary>
        [Mandatory]
        public DisplayTexts                            ProviderName                { get; }

        /// <summary>
        /// The optional URL of the e-mobility provider responsible for this charging ticket.
        /// </summary>
        [Optional]
        public URL?                                    ProviderURL                 { get; }

        /// <summary>
        /// The optional multi-language charging ticket description.
        /// </summary>
        [Optional]
        public DisplayTexts                            Description                 { get; }


        /// <summary>
        /// The timestamp when this charging ticket becomes active (UTC).
        /// </summary>
        [Mandatory]
        public DateTime                                NotBefore                   { get; }

        /// <summary>
        /// The timestamp after which this charging ticket is no longer valid (UTC).
        /// </summary>
        [Mandatory]
        public DateTime                                NotAfter                    { get; }


        /// <summary>
        /// The public key of the EV driver.
        /// </summary>
        [Mandatory]
        public PublicKey                               DriverPublicKey             { get; }

        /// <summary>
        /// The enumeration of tariffs that can be used with this charging ticket.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingTariff>             ChargingTariffs             { get; }


        /// <summary>
        /// An optional enumeration of charging station operators, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<Operator_Id>                ValidOperators              { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPool_Id>            ValidChargingPools          { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingStation_Id>         ValidChargingStations       { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<GlobalEVSE_Id>              ValidEVSEs                  { get; }


        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPool_Id>            InvalidChargingPools        { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingStation_Id>         InvalidChargingStations     { get; }

        /// <summary>
        /// An optional enumeration of EVSE identifications, this charging ticket is valid for.
        /// </summary>
        [Optional]
        public IEnumerable<GlobalEVSE_Id>              InvalidEVSEs                { get; }


        /// <summary>
        /// The allowed current type: AC, DC, or both.
        /// </summary>
        [Mandatory]
        public CurrentTypes                            AllowedCurrentType          { get; }

        /// <summary>
        /// The maximum allowed consumed energy during a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public WattHour?                               MaxKWh                      { get; }

        /// <summary>
        /// The maximum allowed charging power during a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public Watt?                                   MaxKW                       { get; }

        /// <summary>
        /// The maximum allowed current of a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public Ampere?                                 MaxCurrent                  { get; }

        /// <summary>
        /// The maximum allowed duration of a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public TimeSpan?                               MaxDuration                 { get; }

        /// <summary>
        /// The maximum allowed price of a charging session authorized by this charging ticket.
        /// </summary>
        [Optional]
        public Price?                                  MaxPrice                    { get; }

        /// <summary>
        /// The day(s) of the week when this charging ticket allows charging.
        /// </summary>
        [Optional]
        public IEnumerable<DayOfWeek>                  DaysOfWeek                  { get; }


        /// <summary>
        /// How the charging ticket can be used for multiple charging sessions.
        /// </summary>
        [Mandatory]
        public ChargingTicketMultipleSessions          MultipleSessions            { get; }

        /// <summary>
        /// The day(s) of the week when this charging ticket allows charging.
        /// </summary>
        [Mandatory]
        public ChargingTicketValidationMethods         ValidationMethod            { get; }

        /// <summary>
        /// Whether smart (bidirectional) charging is allowed.
        /// </summary>
        public ChargingTicketSmartChargingModes        SmartChargingMode           { get; }

        /// <summary>
        /// Whether signed meter values are expected.
        /// </summary>
        public ChargingTicketMeterValueSignatureModes  MeterValueSignatureMode     { get; }

        /// <summary>
        /// Whether the communication betweem the charging station and the EV driver should be encrypted or not.
        /// </summary>
        public ChargingTicketE2ECommunicationSecurity  E2ECommunicationSecurity    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging ticket.
        /// </summary>
        /// <param name="Id">A global unique and unique in time identification of the charging ticket.</param>
        /// <param name="ProviderId">An unique identification of the e-mobility provider responsible for this charging ticket.</param>
        /// <param name="ProviderName">An multi-language name of the e-mobility provider responsible for this charging ticket.</param>
        /// <param name="DriverPublicKey">An public key of the EV driver.</param>
        /// <param name="ChargingTariffs">An enumeration of tariffs that can be used with this charging ticket.</param>
        /// <param name="ProviderURL">An optional URL of the e-mobility provider responsible for this charging ticket.</param>
        /// <param name="Description">An optional multi-language charging ticket description.</param>
        /// <param name="Created">An optional timestamp when this charging ticket was created.</param>
        /// <param name="NotBefore">An optional timestamp when this charging ticket becomes active (UTC).</param>
        /// <param name="NotAfter">An optional timestamp after which this charging ticket is no longer valid (UTC).</param>
        /// <param name="Validity">The validity (life time) of this charging ticket (only when no 'NotAfter' parameter was given!).</param>
        /// 
        /// <param name="ValidOperators">An enumeration of charging station operators where this charging ticket can be used.</param>
        /// <param name="ValidChargingPools">An enumeration of charging pools where this charging ticket can be used.</param>
        /// <param name="ValidChargingStations">An enumeration of charging stations where this charging ticket can be used.</param>
        /// <param name="ValidEVSEs">An enumeration of EVSEs where this charging ticket can be used.</param>
        /// <param name="InvalidChargingPools">An enumeration of charging pools where this charging ticket can NOT be used.</param>
        /// <param name="InvalidChargingStations">An enumeration of charging stations where this charging ticket can NOT be used.</param>
        /// <param name="InvalidEVSEs">An enumeration of EVSEs where this charging ticket can NOT be used.</param>
        /// 
        /// <param name="AllowedCurrentType">The allowed current type: AC, DC, or both.</param>
        /// <param name="MaxKWh">The maximum allowed charging power during a charging session authorized by this charging ticket.</param>
        /// <param name="MaxKW">The maximum allowed charging power during a charging session authorized by this charging ticket.</param>
        /// <param name="MaxCurrent">The maximum allowed current of a charging session authorized by this charging ticket.</param>
        /// <param name="MaxDuration">The maximum allowed duration of a charging session authorized by this charging ticket.</param>
        /// <param name="MaxPrice">The maximum allowed price of a charging session authorized by this charging ticket.</param>
        /// <param name="DaysOfWeek">The day(s) of the week when this charging ticket allows charging.</param>
        /// <param name="MultipleSessions">How the charging ticket can be used for multiple charging sessions.</param>
        /// <param name="ValidationMethod">The day(s) of the week when this charging ticket allows charging.</param>
        /// <param name="SmartChargingMode">Whether smart (bidirectional) charging is allowed.</param>
        /// <param name="MeterValueSignatureMode">Whether signed meter values are expected.</param>
        /// <param name="E2ECommunicationSecurity">Whether the communication betweem the charging station and the EV driver should be encrypted or not.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this charging ticket.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this charging ticket.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingTicket(ChargingTicket_Id                        Id,
                              Provider_Id                              ProviderId,
                              DisplayTexts                             ProviderName,
                              PublicKey                                DriverPublicKey,
                              IEnumerable<ChargingTariff>              ChargingTariffs,

                              URL?                                     ProviderURL                = null,
                              DisplayTexts?                            Description                = null,
                              DateTime?                                Created                    = null,
                              DateTime?                                NotBefore                  = null,
                              DateTime?                                NotAfter                   = null,
                              TimeSpan?                                Validity                   = null,

                              IEnumerable<Operator_Id>?                ValidOperators             = null,
                              IEnumerable<ChargingPool_Id>?            ValidChargingPools         = null,
                              IEnumerable<ChargingStation_Id>?         ValidChargingStations      = null,
                              IEnumerable<GlobalEVSE_Id>?              ValidEVSEs                 = null,

                              IEnumerable<ChargingPool_Id>?            InvalidChargingPools       = null,
                              IEnumerable<ChargingStation_Id>?         InvalidChargingStations    = null,
                              IEnumerable<GlobalEVSE_Id>?              InvalidEVSEs               = null,

                              CurrentTypes?                            AllowedCurrentType         = null,
                              WattHour?                                MaxKWh                     = null,
                              Watt?                                    MaxKW                      = null,
                              Ampere?                                  MaxCurrent                 = null,
                              TimeSpan?                                MaxDuration                = null,
                              Price?                                   MaxPrice                   = null,
                              IEnumerable<DayOfWeek>?                  DaysOfWeek                 = null,

                              ChargingTicketMultipleSessions?          MultipleSessions           = null,
                              ChargingTicketValidationMethods?         ValidationMethod           = null,
                              ChargingTicketSmartChargingModes?        SmartChargingMode          = null,
                              ChargingTicketMeterValueSignatureModes?  MeterValueSignatureMode    = null,
                              ChargingTicketE2ECommunicationSecurity?  E2ECommunicationSecurity   = null,

                              IEnumerable<KeyPair>?                    SignKeys                   = null,
                              IEnumerable<SignInfo>?                   SignInfos                  = null,
                              IEnumerable<Signature>?                  Signatures                 = null,

                              CustomData?                              CustomData                 = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            if (!ChargingTariffs.Any())
                throw new ArgumentNullException(nameof(ChargingTariffs), "The given enumeration of tariffs must not be null or empty!");

            this.Id                        = Id;
            this.ProviderId                = ProviderId;
            this.ProviderName              = ProviderName;
            this.DriverPublicKey           = DriverPublicKey;
            this.ChargingTariffs           = ChargingTariffs.         Distinct();

            this.ProviderURL               = ProviderURL;
            this.Description               = Description                         ?? DisplayTexts.Empty;
            this.Created                   = Created                             ?? Timestamp.Now;
            this.NotBefore                 = NotBefore                           ?? this.Created;
            this.NotAfter                  = NotAfter                            ?? this.NotBefore + (Validity ?? TimeSpan.FromDays(3));

            this.ValidOperators            = ValidOperators?.         Distinct() ?? Array.Empty<Operator_Id>();
            this.ValidChargingPools        = ValidChargingPools?.     Distinct() ?? Array.Empty<ChargingPool_Id>();
            this.ValidChargingStations     = ValidChargingStations?.  Distinct() ?? Array.Empty<ChargingStation_Id>();
            this.ValidEVSEs                = ValidEVSEs?.             Distinct() ?? Array.Empty<GlobalEVSE_Id>();

            this.InvalidChargingPools      = InvalidChargingPools?.   Distinct() ?? Array.Empty<ChargingPool_Id>();
            this.InvalidChargingStations   = InvalidChargingStations?.Distinct() ?? Array.Empty<ChargingStation_Id>();
            this.InvalidEVSEs              = InvalidEVSEs?.           Distinct() ?? Array.Empty<GlobalEVSE_Id>();

            this.AllowedCurrentType        = AllowedCurrentType                  ?? CurrentTypes.ACDC;
            this.MaxKWh                    = MaxKWh;
            this.MaxKW                     = MaxKW;
            this.MaxCurrent                = MaxCurrent;
            this.MaxDuration               = MaxDuration;
            this.MaxPrice                  = MaxPrice;
            this.DaysOfWeek                = DaysOfWeek?.             Distinct() ?? Array.Empty<DayOfWeek>();

            this.MultipleSessions          = MultipleSessions                    ?? ChargingTicketMultipleSessions.        Allowed;
            this.ValidationMethod          = ValidationMethod                    ?? ChargingTicketValidationMethods.       OfflineChargingAllowed;
            this.SmartChargingMode         = SmartChargingMode                   ?? ChargingTicketSmartChargingModes.      NotAllowed;
            this.MeterValueSignatureMode   = MeterValueSignatureMode             ?? ChargingTicketMeterValueSignatureModes.WhenAvailable;
            this.E2ECommunicationSecurity  = E2ECommunicationSecurity            ?? ChargingTicketE2ECommunicationSecurity.EphemeralKeyAgreement;

            unchecked
            {

                hashCode = this.Id.                      GetHashCode()       * 127 ^
                           this.ProviderId.              GetHashCode()       * 113 ^
                           this.ProviderName.            GetHashCode()       * 109 ^
                           this.DriverPublicKey.         GetHashCode()       * 107 ^
                           this.ChargingTariffs.         CalcHashCode()      * 103 ^

                          (this.ProviderURL?.            GetHashCode() ?? 0) * 101 ^
                           this.Description.             CalcHashCode()      *  97 ^
                           this.Created.                 GetHashCode()       *  89 ^
                           this.NotBefore.               GetHashCode()       *  83 ^
                           this.NotAfter.                GetHashCode()       *  79 ^

                           this.ValidOperators.          CalcHashCode()      *  73 ^
                           this.ValidChargingPools.      CalcHashCode()      *  71 ^
                           this.ValidChargingStations.   CalcHashCode()      *  67 ^
                           this.ValidEVSEs.              CalcHashCode()      *  61 ^

                           this.InvalidChargingPools.    CalcHashCode()      *  59 ^
                           this.InvalidChargingStations. CalcHashCode()      *  53 ^
                           this.InvalidEVSEs.            CalcHashCode()      *  47 ^

                           this.AllowedCurrentType.      GetHashCode()       *  43 ^
                          (this.MaxKWh?.                 GetHashCode() ?? 0) *  37 ^
                          (this.MaxKW?.                  GetHashCode() ?? 0) *  31 ^
                          (this.MaxCurrent?.             GetHashCode() ?? 0) *  29 ^
                          (this.MaxDuration?.            GetHashCode() ?? 0) *  23 ^
                          (this.MaxPrice?.               GetHashCode() ?? 0) *  19 ^
                           this.DaysOfWeek.              CalcHashCode()      *  17 ^

                           this.MultipleSessions.        GetHashCode()       *  13 ^
                           this.ValidationMethod.        GetHashCode()       *  11 ^
                           this.SmartChargingMode.       GetHashCode()       *   7 ^
                           this.MeterValueSignatureMode. GetHashCode()       *   6 ^
                           this.E2ECommunicationSecurity.GetHashCode()       *   3 ^

                           this.Signatures.              CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ChargingTicketIdURL = null, CustomTicketParser = null)

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

        #region (static) TryParse(JSON, out ChargingTicket, out ErrorResponse, ChargingTicketIdURL = null, CustomChargingTicketParser = null)

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
        /// <param name="ChargingTicketIdURL">An optional charging ticket identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomChargingTicketParser">A delegate to parse custom charging ticket JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out ChargingTicket?                           ChargingTicket,
                                       out String?                                   ErrorResponse,
                                       ChargingTicket_Id?                            ChargingTicketIdURL          = null,
                                       CustomJObjectParserDelegate<ChargingTicket>?  CustomChargingTicketParser   = null)
        {

            try
            {

                ChargingTicket = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                          [optional]

                if (JSON.ParseOptional("id",
                                       "ticket identification",
                                       ChargingTicket_Id.TryParse,
                                       out ChargingTicket_Id? TicketIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!ChargingTicketIdURL.HasValue && !TicketIdBody.HasValue)
                {
                    ErrorResponse = "The ticket identification is missing!";
                    return false;
                }

                if (ChargingTicketIdURL.HasValue && TicketIdBody.HasValue && ChargingTicketIdURL.Value != TicketIdBody.Value)
                {
                    ErrorResponse = "The optional ticket identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse ProviderId                  [mandatory]

                if (!JSON.ParseMandatory("providerId",
                                         "provider identification",
                                         Provider_Id.TryParse,
                                         out Provider_Id ProviderId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ProviderName                [mandatory]

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

                #region Parse DriverPublicKey             [mandatory]

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

                #region Parse ChargingTariffs             [mandatory]

                if (!JSON.ParseMandatoryHashSet("chargingTariffs",
                                                "charging tariffs",
                                                ChargingTariff.TryParse,
                                                out HashSet<ChargingTariff> ChargingTariffs,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse ProviderURL                 [optional]

                if (!JSON.ParseOptional("providerURL",
                                        "provider URL",
                                        URL.TryParse,
                                        out URL? ProviderURL,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Description                 [optional]

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

                #region Parse Created                     [mandatory]

                if (!JSON.ParseMandatory("created",
                                         "created",
                                         out DateTime Created,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse NotBefore                   [mandatory]

                if (!JSON.ParseMandatory("notBefore",
                                         "not before",
                                         out DateTime NotBefore,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse NotAfter                    [mandatory]

                if (!JSON.ParseMandatory("notAfter",
                                         "not after",
                                         out DateTime NotAfter,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse ValidOperators              [optional]

                if (JSON.ParseOptionalHashSet("validOperators",
                                              "valid operator identifications",
                                              Operator_Id.TryParse,
                                              out HashSet<Operator_Id> ValidOperators,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidChargingPools          [optional]

                if (JSON.ParseOptionalHashSet("validChargingPools",
                                              "valid charging pool identifications",
                                              ChargingPool_Id.TryParse,
                                              out HashSet<ChargingPool_Id> ValidChargingPools,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidChargingStations       [optional]

                if (JSON.ParseOptionalHashSet("validChargingStations",
                                              "valid charging station identifications",
                                              ChargingStation_Id.TryParse,
                                              out HashSet<ChargingStation_Id> ValidChargingStations,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidEVSEs                  [optional]

                if (JSON.ParseOptionalHashSet("validEVSEs",
                                              "valid EVSE identifications",
                                              GlobalEVSE_Id.TryParse,
                                              out HashSet<GlobalEVSE_Id> ValidEVSEs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse InvalidChargingPools        [optional]

                if (JSON.ParseOptionalHashSet("invalidChargingPools",
                                              "invalid charging pool identifications",
                                              ChargingPool_Id.TryParse,
                                              out HashSet<ChargingPool_Id> InvalidChargingPools,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse InvalidChargingStations     [optional]

                if (JSON.ParseOptionalHashSet("invalidChargingStations",
                                              "invalid charging station identifications",
                                              ChargingStation_Id.TryParse,
                                              out HashSet<ChargingStation_Id> InvalidChargingStations,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse InvalidEVSEs                [optional]

                if (JSON.ParseOptionalHashSet("invalidEVSEs",
                                              "invalid EVSE identifications",
                                              GlobalEVSE_Id.TryParse,
                                              out HashSet<GlobalEVSE_Id> InvalidEVSEs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse AllowedCurrentType          [mandatory]

                if (!JSON.ParseMandatory("allowedCurrentType",
                                         "allowed current type",
                                         CurrentTypesExtensions.TryParse,
                                         out CurrentTypes AllowedCurrentType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxKWh                      [optional]

                if (JSON.ParseOptional("maxKWh",
                                       "max KWh",
                                       out WattHour? MaxKWh,
                                       out ErrorResponse,
                                       Multiplicator: 1000))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxKW                       [optional]

                if (JSON.ParseOptional("MaxKW",
                                       "max KW",
                                       out Watt? MaxKW,
                                       out ErrorResponse,
                                       Multiplicator: 1000))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxCurrent                  [optional]

                if (JSON.ParseOptional("MaxCurrent",
                                       "max current",
                                       out Ampere? MaxCurrent,
                                       out ErrorResponse,
                                       Multiplicator: 1000))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxDuration                 [optional]

                if (JSON.ParseOptional("maxDuration",
                                       "max duration",
                                       out TimeSpan? MaxDuration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxPrice                    [optional]

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

                #region Parse DaysOfWeek                  [optional]

                if (JSON.ParseOptionalEnums("daysOfWeek",
                                            "days of week",
                                            out HashSet<DayOfWeek> DaysOfWeek,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse MultipleSessions            [mandatory]

                if (!JSON.ParseMandatory("multipleSessions",
                                         "multiple sessions",
                                         ChargingTicketMultipleSessionsExtensions.TryParse,
                                         out ChargingTicketMultipleSessions MultipleSessions,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ValidationMethod            [mandatory]

                if (!JSON.ParseMandatory("validationMethod",
                                         "validation method",
                                         ChargingTicketValidationMethodsExtensions.TryParse,
                                         out ChargingTicketValidationMethods ValidationMethod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SmartChargingMode           [mandatory]

                if (!JSON.ParseMandatory("smartChargingMode",
                                         "smart charging method",
                                         ChargingTicketSmartChargingModesExtensions.TryParse,
                                         out ChargingTicketSmartChargingModes SmartChargingMode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MeterValueSignatureMode     [mandatory]

                if (!JSON.ParseMandatory("MeterValueSignatureMode",
                                         "meter value signature mode",
                                         ChargingTicketMeterValueSignatureModesExtensions.TryParse,
                                         out ChargingTicketMeterValueSignatureModes MeterValueSignatureMode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse E2ECommunicationSecurity    [mandatory]

                if (!JSON.ParseMandatory("evDriverCommunication",
                                         "ev driver communication",
                                         ChargingTicketEVDriverCommunicationsExtensions.TryParse,
                                         out ChargingTicketE2ECommunicationSecurity E2ECommunicationSecurity,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Signatures                        [optional, OCPP_CSE]

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

                #region CustomData                        [optional]

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

                                     TicketIdBody ?? ChargingTicketIdURL!.Value,
                                     ProviderId,
                                     ProviderName,
                                     DriverPublicKey,
                                     ChargingTariffs,

                                     ProviderURL,
                                     Description,
                                     Created,
                                     NotBefore,
                                     NotAfter,
                                     null, // Validity (just a helper within the constructor!)

                                     ValidOperators,
                                     ValidChargingPools,
                                     ValidChargingStations,
                                     ValidEVSEs,
                                     InvalidChargingPools,
                                     InvalidChargingStations,
                                     InvalidEVSEs,

                                     AllowedCurrentType,
                                     MaxKWh,
                                     MaxKW,
                                     MaxCurrent,
                                     MaxDuration,
                                     MaxPrice,
                                     DaysOfWeek,

                                     MultipleSessions,
                                     ValidationMethod,
                                     SmartChargingMode,
                                     MeterValueSignatureMode,
                                     E2ECommunicationSecurity,

                                     null,
                                     null,
                                     Signatures,
                                     CustomData

                                 );

                if (CustomChargingTicketParser is not null)
                    ChargingTicket = CustomChargingTicketParser(JSON,
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
                              CustomJObjectSerializerDelegate<ChargingTariff>?       CustomTariffSerializer                = null,
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

                                 new JProperty("id",                         Id.                      ToString()),

                                 new JProperty("providerId",                                          ProviderId),
                                 new JProperty("providerName",               new JArray(ProviderName.           Select(providerName      => providerName.     ToJSON(CustomDisplayTextSerializer)))),

                           ProviderURL.HasValue
                               ? new JProperty("providerURL",                ProviderURL.Value.       ToString())
                               : null,

                                 new JProperty("driverPublicKey",            DriverPublicKey.         ToJSON(CustomPublicKeySerializer,
                                                                                                             CustomCustomDataSerializer)),

                                 new JProperty("tariffs",                    new JArray(ChargingTariffs.                Select(chargingTariff    => chargingTariff.   ToJSON(CustomTariffSerializer,
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
                               ? new JProperty("description",                new JArray(Description.            Select(description       => description.      ToJSON(CustomDisplayTextSerializer))))
                               : null,

                                 new JProperty("created",                    Created.                 ToIso8601()),
                                 new JProperty("notBefore",                  NotBefore.               ToIso8601()),
                                 new JProperty("notAftere",                  NotAfter.                ToIso8601()),

                           ValidOperators.         Any()
                               ? new JProperty("validOperators",             new JArray(ValidOperators.         Select(operatorId        => operatorId.       ToString())))
                               : null,

                           ValidChargingPools.     Any()
                               ? new JProperty("validChargingPools",         new JArray(ValidChargingPools.     Select(chargingPoolId    => chargingPoolId.   ToString())))
                               : null,

                           ValidChargingStations.  Any()
                               ? new JProperty("validChargingStations",      new JArray(ValidChargingStations.  Select(chargingStationId => chargingStationId.ToString())))
                               : null,

                           ValidEVSEs.           Any()
                               ? new JProperty("validEVSEIds",               new JArray(ValidEVSEs.             Select(evseId            => evseId.           ToString())))
                               : null,


                           InvalidChargingPools.   Any()
                               ? new JProperty("invalidChargingPools",       new JArray(InvalidChargingPools.   Select(chargingPoolId    => chargingPoolId.   ToString())))
                               : null,

                           InvalidChargingStations.Any()
                               ? new JProperty("invalidChargingStations",    new JArray(InvalidChargingStations.Select(chargingStationId => chargingStationId.ToString())))
                               : null,

                           InvalidEVSEs.         Any()
                               ? new JProperty("invalidEVSEIds",             new JArray(InvalidEVSEs.           Select(evseId            => evseId.           ToString())))
                               : null,


                                 new JProperty("allowedCurrentType",         AllowedCurrentType.      AsText()),

                           MaxKWh.     HasValue
                               ? new JProperty("maxKWh",                     MaxKWh.            Value.Value)
                               : null,

                           MaxKW.      HasValue
                               ? new JProperty("maxKW",                      MaxKW.             Value.Value)
                               : null,

                           MaxCurrent. HasValue
                               ? new JProperty("maxCurrent",                 MaxCurrent.        Value.Value)
                               : null,

                           MaxDuration.HasValue
                               ? new JProperty("maxDuration",                MaxDuration.       Value.TotalMinutes)
                               : null,

                           MaxPrice.HasValue
                               ? new JProperty("maxPrice",                   MaxPrice.          Value.ToJSON(CustomPriceSerializer))
                               : null,

                           DaysOfWeek.Any()
                               ? new JProperty("daysOfWeek",                 new JArray(DaysOfWeek.             Select(dayOfWeek         => dayOfWeek.    ToString())))
                               : null,

                                 new JProperty("multiUsage",                 MultipleSessions.        AsText()),
                                 new JProperty("validationMethod",           ValidationMethod.        AsText()),
                                 new JProperty("smartChargingMode",          SmartChargingMode.       AsText()),
                                 new JProperty("meterValueSignatures",       MeterValueSignatureMode. AsText()),
                                 new JProperty("e2eCommunicationSecurity",   E2ECommunicationSecurity.AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",                 new JArray(Signatures.             Select(signature         => signature.    ToJSON(CustomSignatureSerializer,
                                                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                 CustomData.              ToJSON(CustomCustomDataSerializer))
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
                    null,

                    ValidOperators.         Select(operatorId        => operatorId.       Clone).ToArray(),
                    ValidChargingPools.     Select(chargingPoolId    => chargingPoolId.   Clone).ToArray(),
                    ValidChargingStations.  Select(chargingStationId => chargingStationId.Clone).ToArray(),
                    ValidEVSEs.             Select(evseId            => evseId.           Clone).ToArray(),

                    InvalidChargingPools.   Select(chargingPoolId    => chargingPoolId.   Clone).ToArray(),
                    InvalidChargingStations.Select(chargingStationId => chargingStationId.Clone).ToArray(),
                    InvalidEVSEs.           Select(evseId            => evseId.           Clone).ToArray(),

                    AllowedCurrentType,
                    MaxKWh,
                    MaxKW,
                    MaxCurrent,
                    MaxDuration,
                    MaxPrice,
                    DaysOfWeek.             ToArray(),

                    MultipleSessions,
                    ValidationMethod,
                    SmartChargingMode,
                    MeterValueSignatureMode,
                    E2ECommunicationSecurity,

                    SignKeys,
                    SignInfos,
                    Signatures.             Select(signature     => signature.    Clone()).ToArray(),

                    CustomData);

        #endregion


        #region Operator overloading

        #region Operator == (ChargingTicket1, ChargingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTicket1">A charging ticket.</param>
        /// <param name="ChargingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingTicket? ChargingTicket1,
                                           ChargingTicket? ChargingTicket2)
        {

            if (Object.ReferenceEquals(ChargingTicket1, ChargingTicket2))
                return true;

            if (ChargingTicket1 is null || ChargingTicket2 is null)
                return false;

            return ChargingTicket1.Equals(ChargingTicket2);

        }

        #endregion

        #region Operator != (ChargingTicket1, ChargingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTicket1">A charging ticket.</param>
        /// <param name="ChargingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingTicket? ChargingTicket1,
                                           ChargingTicket? ChargingTicket2)

            => !(ChargingTicket1 == ChargingTicket2);

        #endregion

        #region Operator <  (ChargingTicket1, ChargingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTicket1">A charging ticket.</param>
        /// <param name="ChargingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingTicket? ChargingTicket1,
                                          ChargingTicket? ChargingTicket2)

            => ChargingTicket1 is null
                   ? throw new ArgumentNullException(nameof(ChargingTicket1), "The given ticket must not be null!")
                   : ChargingTicket1.CompareTo(ChargingTicket2) < 0;

        #endregion

        #region Operator <= (ChargingTicket1, ChargingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTicket1">A charging ticket.</param>
        /// <param name="ChargingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingTicket? ChargingTicket1,
                                           ChargingTicket? ChargingTicket2)

            => !(ChargingTicket1 > ChargingTicket2);

        #endregion

        #region Operator >  (ChargingTicket1, ChargingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTicket1">A charging ticket.</param>
        /// <param name="ChargingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingTicket? ChargingTicket1,
                                          ChargingTicket? ChargingTicket2)

            => ChargingTicket1 is null
                   ? throw new ArgumentNullException(nameof(ChargingTicket1), "The given ticket must not be null!")
                   : ChargingTicket1.CompareTo(ChargingTicket2) > 0;

        #endregion

        #region Operator >= (ChargingTicket1, ChargingTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingTicket1">A charging ticket.</param>
        /// <param name="ChargingTicket2">Another charging ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingTicket? ChargingTicket1,
                                           ChargingTicket? ChargingTicket2)

            => !(ChargingTicket1 < ChargingTicket2);

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

               Id.                      Equals(ChargingTicket.Id)                       &&
               ProviderId.              Equals(ChargingTicket.ProviderId)               &&
               ProviderName.            Equals(ChargingTicket.ProviderName)             &&
               DriverPublicKey.         Equals(ChargingTicket.DriverPublicKey)          &&

               ProviderId.              Equals(ChargingTicket.ProviderId)               &&
               NotBefore.               Equals(ChargingTicket.NotBefore)                &&
               NotAfter.                Equals(ChargingTicket.NotAfter)                 &&
               AllowedCurrentType.      Equals(ChargingTicket.AllowedCurrentType)       &&
               MultipleSessions.        Equals(ChargingTicket.MultipleSessions)         &&
               ValidationMethod.        Equals(ChargingTicket.ValidationMethod)         &&
               SmartChargingMode.       Equals(ChargingTicket.SmartChargingMode)        &&
               MeterValueSignatureMode. Equals(ChargingTicket.MeterValueSignatureMode)  &&
               E2ECommunicationSecurity.Equals(ChargingTicket.E2ECommunicationSecurity) &&


            ((!ProviderURL.HasValue    && !ChargingTicket.ProviderURL. HasValue) ||
              (ProviderURL.HasValue    &&  ChargingTicket.ProviderURL. HasValue    && ProviderURL.Value.Equals(ChargingTicket.ProviderURL.Value))) &&

             ((Description is     null &&  ChargingTicket.Description  is null)  ||
              (Description is not null &&  ChargingTicket.Description  is not null && Description.      Equals(ChargingTicket.Description)))       &&


            ((!MaxKWh.     HasValue    && !ChargingTicket.MaxKWh.      HasValue) ||
              (MaxKWh.     HasValue    &&  ChargingTicket.MaxKWh.      HasValue    && MaxKWh.     Value.Equals(ChargingTicket.MaxKWh.     Value))) &&

            ((!MaxKW.      HasValue    && !ChargingTicket.MaxKW.       HasValue) ||
              (MaxKW.      HasValue    &&  ChargingTicket.MaxKW.       HasValue    && MaxKW.      Value.Equals(ChargingTicket.MaxKW.      Value))) &&

            ((!MaxCurrent. HasValue    && !ChargingTicket.MaxCurrent.  HasValue) ||
              (MaxCurrent. HasValue    &&  ChargingTicket.MaxCurrent.  HasValue    && MaxCurrent. Value.Equals(ChargingTicket.MaxCurrent. Value))) &&

            ((!MaxDuration.HasValue    && !ChargingTicket.MaxDuration. HasValue) ||
              (MaxDuration.HasValue    &&  ChargingTicket.MaxDuration. HasValue    && MaxDuration.Value.Equals(ChargingTicket.MaxDuration.Value))) &&

            ((!MaxPrice.   HasValue    && !ChargingTicket.MaxPrice.    HasValue) ||
              (MaxPrice.   HasValue    &&  ChargingTicket.MaxPrice.    HasValue    && MaxPrice.   Value.Equals(ChargingTicket.MaxPrice.   Value))) &&


               DaysOfWeek.             Count().          Equals(ChargingTicket.DaysOfWeek.             Count())                     &&
               DaysOfWeek.             All(dayOfWeek         => ChargingTicket.DaysOfWeek.             Contains(dayOfWeek))         &&


               ValidOperators.         Count().          Equals(ChargingTicket.ValidOperators.         Count())                     &&
               ValidOperators.         All(operatorId        => ChargingTicket.ValidOperators.         Contains(operatorId))        &&

               ValidChargingPools.     Count().          Equals(ChargingTicket.ValidChargingPools.     Count())                     &&
               ValidChargingPools.     All(chargingPoolId    => ChargingTicket.ValidChargingPools.     Contains(chargingPoolId))    &&

               ValidChargingStations.  Count().          Equals(ChargingTicket.ValidChargingStations.  Count())                     &&
               ValidChargingStations.  All(chargingStationId => ChargingTicket.ValidChargingStations.  Contains(chargingStationId)) &&

               ValidEVSEs.             Count().          Equals(ChargingTicket.ValidEVSEs.             Count())                     &&
               ValidEVSEs.             All(evseId            => ChargingTicket.ValidEVSEs.             Contains(evseId))            &&


               InvalidChargingPools.   Count().          Equals(ChargingTicket.InvalidChargingPools.   Count())                     &&
               InvalidChargingPools.   All(chargingPoolId    => ChargingTicket.InvalidChargingPools.   Contains(chargingPoolId))    &&

               InvalidChargingStations.Count().          Equals(ChargingTicket.InvalidChargingStations.Count())                     &&
               InvalidChargingStations.All(chargingStationId => ChargingTicket.InvalidChargingStations.Contains(chargingStationId)) &&

               InvalidEVSEs.           Count().          Equals(ChargingTicket.InvalidEVSEs.           Count())                     &&
               InvalidEVSEs.           All(evseId            => ChargingTicket.InvalidEVSEs.           Contains(evseId))            &&


               ChargingTariffs.        Count().          Equals(ChargingTicket.ChargingTariffs.        Count())                     &&
               ChargingTariffs.        All(chargingTariff    => ChargingTicket.ChargingTariffs.        Contains(chargingTariff))    &&


               base.Equals(ChargingTicket);

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
