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

using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for EV driver tariffs (B2C).
    /// </summary>
    public static class TariffExtensions
    {

        #region GenerateWithHashId(ProviderId, ProviderName, Currency, TariffElements, ...)

        /// <summary>
        /// Generate a new read-only signable EV driver tariff (B2C) having an identification based on the SHA256 of its information.
        /// </summary>
        public static Tariff GenerateWithHashId(Provider_Id                                          ProviderId,
                                                DisplayTexts                                         ProviderName,
                                                Currency                                             Currency,
                                                MessageContents?                                     Description                         = null,
                                                DateTime?                                            ValidFrom                           = null,
                                                DateTime?                                            ValidTo                             = null,
                                                Price?                                               MinCost                             = null,
                                                Price?                                               MaxCost                             = null,
                                                TariffFixed?                                         FixedFee                            = null,
                                                TariffFixed?                                         ReservationFixed                    = null,
                                                TariffTime?                                          ReservationTime                     = null,
                                                TariffEnergy?                                        Energy                              = null,
                                                TariffTime?                                          ChargingTime                        = null,
                                                TariffTime?                                          IdleTime                            = null,
                                                CustomData?                                          CustomData                          = null,

                                                String?                                              HMACSecret                          = null,

                                                CustomJObjectSerializerDelegate<Tariff>?             CustomTariffSerializer              = null,
                                                CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                                                CustomJObjectSerializerDelegate<Price>?              CustomPriceSerializer               = null,
                                                CustomJObjectSerializerDelegate<TaxRate>?            CustomTaxRateSerializer             = null,
                                                CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffConditionsSerializer    = null,
                                                CustomJObjectSerializerDelegate<TariffEnergy>?       CustomTariffEnergySerializer        = null,
                                                CustomJObjectSerializerDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceSerializer   = null,
                                                CustomJObjectSerializerDelegate<TariffTime>?         CustomTariffTimeSerializer          = null,
                                                CustomJObjectSerializerDelegate<TariffTimePrice>?    CustomTariffTimePriceSerializer     = null,
                                                CustomJObjectSerializerDelegate<TariffFixed>?        CustomTariffFixedSerializer         = null,
                                                CustomJObjectSerializerDelegate<TariffFixedPrice>?   CustomTariffFixedPriceSerializer    = null,
                                                CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                                                CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)

        {

            var tariff = new Tariff(

                             Tariff_Id.New(ProviderId),
                             Currency,
                             Description,
                             ValidFrom,
                             ValidTo,
                             MinCost,
                             MaxCost,
                             FixedFee,
                             ReservationFixed,
                             ReservationTime,
                             Energy,
                             ChargingTime,
                             IdleTime,

                             null,
                             null,
                             null,

                             CustomData

                         );


            return new (

                   Tariff_Id.New(
                       ProviderId,
                       SHA256.HashData(

                           (tariff.ToJSON(CustomTariffSerializer,
                                          CustomMessageContentSerializer,
                                          CustomPriceSerializer,
                                          CustomTaxRateSerializer,
                                          CustomTariffConditionsSerializer,
                                          CustomTariffEnergySerializer,
                                          CustomTariffEnergyPriceSerializer,
                                          CustomTariffTimeSerializer,
                                          CustomTariffTimePriceSerializer,
                                          CustomTariffFixedSerializer,
                                          CustomTariffFixedPriceSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer).

                                  ToString(Formatting.None,
                                           SignableMessage.DefaultJSONConverters) + (HMACSecret ?? "")).

                                  ToUTF8Bytes()

                       ).ToBase64()
                   ),

                   tariff.Currency,
                   tariff.Description,
                   tariff.ValidFrom,
                   tariff.ValidTo,
                   tariff.MinCost,
                   tariff.MaxCost,
                   tariff.FixedFee,
                   tariff.ReservationFixed,
                   tariff.ReservationTime,
                   tariff.Energy,
                   tariff.ChargingTime,
                   tariff.IdleTime,

                   null,
                   null,
                   tariff.Signatures,

                   tariff.CustomData

               );

        }

        #endregion

    }


    /// <summary>
    /// A tariff.
    /// </summary>
    public class Tariff : ACustomSignableData,
                          ISignableMessage,
                          IHasId<Tariff_Id>,
                          IEquatable<Tariff>,
                          IComparable<Tariff>,
                          IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/tariff");

        #endregion

        #region Properties

        /// <summary>
        /// The global unique and unique in time identification of the tariff.
        /// [Max: 36]
        /// </summary>
        [Mandatory]
        public   Tariff_Id        Id                  { get; }

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        [Mandatory]
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The ISO 4217 code of the currency used for this tariff.
        /// </summary>
        [Mandatory]
        public   Currency         Currency            { get; }

        /// <summary>
        /// The optional multi-language tariff description.
        /// </summary>
        [Optional]
        public   MessageContents  Description         { get; }

        /// <summary>
        /// Time when this tariff becomes active. When absent, it is immediately active.
        /// </summary>
        [Optional]
        public   DateTime?        ValidFrom           { get; }

        /// <summary>
        /// Time when this tariff is no longer valid. When absent, it is valid indefinitely.
        /// </summary>
        [Optional]
        public   DateTime?        ValidTo             { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will at least cost
        /// this amount. This is different from a FLAT fee (Start Tariff, Transaction Fee),
        /// as a FLAT fee is a fixed amount that has to be paid for any Charging Session.
        /// A minimum price indicates that when the cost of a charging session is lower
        /// than this amount, the cost of the charging session will be equal to this amount.
        /// </summary>
        [Optional]
        public   Price?           MinCost            { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will NOT
        /// cost more than this amount.
        /// </summary>
        [Optional]
        public   Price?           MaxCost            { get; }

        /// <summary>
        /// The optional fixed fee tariff elements.
        /// </summary>
        [Optional]
        public   TariffFixed?     FixedFee            { get; }

        /// <summary>
        /// The optional reservation fixed fee tariff elements.
        /// </summary>
        [Optional]
        public   TariffFixed?     ReservationFixed    { get; }

        /// <summary>
        /// The optional reservation time tariff elements.
        /// </summary>
        [Optional]
        public   TariffTime?      ReservationTime     { get; }

        /// <summary>
        /// The optional energy tariff elements.
        /// </summary>
        [Optional]
        public   TariffEnergy?    Energy              { get; }

        /// <summary>
        /// The optional charging time tariff elements.
        /// </summary>
        [Optional]
        public   TariffTime?      ChargingTime        { get; }

        /// <summary>
        /// The optional idle time tariff elements.
        /// </summary>
        [Optional]
        public   TariffTime?      IdleTime            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a tariff.
        /// </summary>
        /// <param name="Id">A global unique and unique in time identification of the tariff.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="Description">An optional multi-language tariff description.</param>
        /// <param name="ValidFrom">Time when this tariff becomes active. When absent, it is immediately active.</param>
        /// <param name="ValidTo">Time when this tariff is no longer valid. When absent, it is valid indefinitely.</param>
        /// <param name="ReservationFixed">The optional reservation fixed fee tariff elements.</param>
        /// <param name="ReservationTime">The optional reservation time tariff elements.</param>
        /// <param name="MinCost">When this optional field is set, a charging session with this tariff will at least cost this amount.</param>
        /// <param name="MaxCost">When this optional field is set, a charging session with this tariff will NOT cost more than this amount.</param>
        /// <param name="Energy">The optional energy tariff elements.</param>
        /// <param name="ChargingTime">The optional charging time tariff elements.</param>
        /// <param name="IdleTime">The optional idle time tariff elements.</param>
        /// <param name="FixedFee">The optional fixed fee tariff elements.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this tariff.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this tariff.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Tariff(Tariff_Id                Id,
                      Currency                 Currency,
                      MessageContents?         Description        = null,
                      DateTime?                ValidFrom          = null,
                      DateTime?                ValidTo            = null,
                      Price?                   MinCost            = null,
                      Price?                   MaxCost            = null,
                      TariffFixed?             FixedFee           = null,
                      TariffFixed?             ReservationFixed   = null,
                      TariffTime?              ReservationTime    = null,
                      TariffEnergy?            Energy             = null,
                      TariffTime?              ChargingTime       = null,
                      TariffTime?              IdleTime           = null,

                      IEnumerable<KeyPair>?    SignKeys           = null,
                      IEnumerable<SignInfo>?   SignInfos          = null,
                      IEnumerable<Signature>?  Signatures         = null,

                      CustomData?              CustomData         = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            this.Id                = Id;
            this.Currency          = Currency;
            this.Description       = Description ?? MessageContents.Empty;
            this.ValidFrom         = ValidFrom;
            this.ValidTo           = ValidTo;
            this.MinCost           = MinCost;
            this.MaxCost           = MaxCost;
            this.FixedFee          = FixedFee;
            this.ReservationFixed  = ReservationFixed;
            this.ReservationTime   = ReservationTime;
            this.Energy            = Energy;
            this.ChargingTime      = ChargingTime;
            this.IdleTime          = IdleTime;

            unchecked
            {

                hashCode = this.Id.               GetHashCode()       * 43 ^
                           this.Currency.         GetHashCode()       * 41 ^
                           this.Description.      GetHashCode()       * 37 ^
                          (this.ValidFrom?.       GetHashCode() ?? 0) * 31 ^
                          (this.ValidTo?.         GetHashCode() ?? 0) * 29 ^
                          (this.MinCost?.         GetHashCode() ?? 0) * 23 ^
                          (this.MaxCost?.         GetHashCode() ?? 0) * 19 ^
                          (this.FixedFee?.        GetHashCode() ?? 0) * 17 ^
                          (this.ReservationFixed?.GetHashCode() ?? 0) * 13 ^
                          (this.ReservationTime?. GetHashCode() ?? 0) * 11 ^
                          (this.Energy?.          GetHashCode() ?? 0) *  7 ^
                          (this.ChargingTime?.    GetHashCode() ?? 0) *  5 ^
                          (this.IdleTime?.        GetHashCode() ?? 0) *  3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "A tariff is described by fields with prices for:\r\nenergy,\r\ncharging time,\r\nidle time,\r\nfixed fee,\r\nreservation time,\r\nreservation fixed fee. +\r\nEach of these fields may have (optional) conditions that specify when a price is applicable. +\r\nThe _description_ contains a human-readable explanation of the tariff to be shown to the user. +\r\nThe other fields are parameters that define the tariff. These are used by the charging station to calculate the price.",
        //     "javaType": "Tariff",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "tariffId": {
        //             "description": "Unique id of tariff",
        //             "type": "string",
        //             "maxLength": 60
        //         },
        //         "description": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/MessageContentType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 10
        //         },
        //         "currency": {
        //             "description": "Currency code according to ISO 4217",
        //             "type": "string",
        //             "maxLength": 3
        //         },
        //         "energy": {
        //             "$ref": "#/definitions/TariffEnergyType"
        //         },
        //         "validFrom": {
        //             "description": "Time when this tariff becomes active. When absent, it is immediately active.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "chargingTime": {
        //             "$ref": "#/definitions/TariffTimeType"
        //         },
        //         "idleTime": {
        //             "$ref": "#/definitions/TariffTimeType"
        //         },
        //         "fixedFee": {
        //             "$ref": "#/definitions/TariffFixedType"
        //         },
        //         "reservationTime": {
        //             "$ref": "#/definitions/TariffTimeType"
        //         },
        //         "reservationFixed": {
        //             "$ref": "#/definitions/TariffFixedType"
        //         },
        //         "minCost": {
        //             "$ref": "#/definitions/PriceType"
        //         },
        //         "maxCost": {
        //             "$ref": "#/definitions/PriceType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "tariffId",
        //         "currency"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, TariffIdURL = null, CustomTariffParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffIdURL">An optional tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom tariff JSON objects.</param>
        public static Tariff Parse(JObject                               JSON,
                                   Tariff_Id?                            TariffIdURL          = null,
                                   CustomJObjectParserDelegate<Tariff>?  CustomTariffParser   = null)
        {

            if (TryParse(JSON,
                         out var tariff,
                         out var errorResponse,
                         TariffIdURL,
                         CustomTariffParser))
            {
                return tariff;
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
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out Tariff?  Tariff,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

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
                                       [NotNullWhen(true)]  out Tariff?      Tariff,
                                       [NotNullWhen(false)] out String?      ErrorResponse,
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

                #region Parse Id                  [optional]

                if (JSON.ParseOptional("tariffId",
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

                #region Parse Currency            [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description         [optional]

                if (JSON.ParseOptionalJSONArray("description",
                                                "tariff description",
                                                MessageContents.TryParse,
                                                out MessageContents? Description,
                                                out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidFrom           [optional]

                if (JSON.ParseOptional("validFrom",
                                       "valid from",
                                       out DateTime? ValidFrom,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ValidTo             [optional]

                if (JSON.ParseOptional("validTo",
                                       "valid to",
                                       out DateTime? ValidTo,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MinCost             [optional]

                if (JSON.ParseOptionalJSON("minCost",
                                           "minimum cost/price",
                                           Price.TryParse,
                                           out Price? MinCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse MaxCost             [optional]

                if (JSON.ParseOptionalJSON("maxCost",
                                           "maximum cost/price",
                                           Price.TryParse,
                                           out Price? MaxCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse FixedFee            [optional]

                if (JSON.ParseOptionalJSON("fixedFee",
                                           "fixed fee tariff element",
                                           TariffFixed.TryParse,
                                           out TariffFixed? FixedFee,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ReservationFixed    [optional]

                if (JSON.ParseOptionalJSON("reservationFixed",
                                           "reservation fixed fee tariff element",
                                           TariffFixed.TryParse,
                                           out TariffFixed? ReservationFixed,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ReservationTime     [optional]

                if (JSON.ParseOptionalJSON("reservationTime",
                                           "reservation time tariff element",
                                           TariffTime.TryParse,
                                           out TariffTime? ReservationTime,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Energy              [optional]

                if (JSON.ParseOptionalJSON("energy",
                                           "energy tariff element",
                                           TariffEnergy.TryParse,
                                           out TariffEnergy? Energy,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingTime        [optional]

                if (JSON.ParseOptionalJSON("chargingTime",
                                           "charging time tariff element",
                                           TariffTime.TryParse,
                                           out TariffTime? ChargingTime,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse IdleTime            [optional]

                if (JSON.ParseOptionalJSON("idleTime",
                                           "idle time tariff element",
                                           TariffTime.TryParse,
                                           out TariffTime? IdleTime,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Tariff = new Tariff(

                             TariffIdBody ?? TariffIdURL!.Value,
                             Currency,
                             Description,
                             ValidFrom,
                             ValidTo,
                             MinCost,
                             MaxCost,
                             FixedFee,
                             ReservationFixed,
                             ReservationTime,
                             Energy,
                             ChargingTime,
                             IdleTime,

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
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        /// <param name="CustomTariffEnergySerializer">A delegate to serialize custom TariffEnergy JSON objects.</param>
        /// <param name="CustomTariffEnergyPriceSerializer">A delegate to serialize custom TariffEnergyPrice JSON objects.</param>
        /// <param name="CustomTariffTimeSerializer">A delegate to serialize custom TariffTime JSON objects.</param>
        /// <param name="CustomTariffTimePriceSerializer">A delegate to serialize custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffFixedSerializer">A delegate to serialize custom TariffFixed JSON objects.</param>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Tariff>?             CustomTariffSerializer              = null,
                              CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                              CustomJObjectSerializerDelegate<Price>?              CustomPriceSerializer               = null,
                              CustomJObjectSerializerDelegate<TaxRate>?            CustomTaxRateSerializer             = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffConditionsSerializer    = null,
                              CustomJObjectSerializerDelegate<TariffEnergy>?       CustomTariffEnergySerializer        = null,
                              CustomJObjectSerializerDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffTime>?         CustomTariffTimeSerializer          = null,
                              CustomJObjectSerializerDelegate<TariffTimePrice>?    CustomTariffTimePriceSerializer     = null,
                              CustomJObjectSerializerDelegate<TariffFixed>?        CustomTariffFixedSerializer         = null,
                              CustomJObjectSerializerDelegate<TariffFixedPrice>?   CustomTariffFixedPriceSerializer    = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("tariffId",           Id.              ToString()),
                                 new JProperty("currency",           Currency.        ISOCode),

                           Description.Count > 0
                               ? new JProperty("description",        Description.     ToJSON(CustomMessageContentSerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           ValidFrom.HasValue
                               ? new JProperty("validFrom",          ValidFrom.Value. ToISO8601())
                               : null,

                           ValidTo.  HasValue
                               ? new JProperty("validTo",            ValidTo.  Value. ToISO8601())
                               : null,

                           MaxCost.HasValue
                               ? new JProperty("maxCost",            MaxCost.  Value. ToJSON(CustomPriceSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           MinCost.HasValue
                               ? new JProperty("minCost",            MinCost.  Value. ToJSON(CustomPriceSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           MaxCost.HasValue
                               ? new JProperty("maxCost",            MaxCost.  Value. ToJSON(CustomPriceSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           FixedFee is not null
                               ? new JProperty("fixedFee",           FixedFee.        ToJSON(CustomTariffFixedSerializer,
                                                                                             CustomTariffFixedPriceSerializer,
                                                                                             CustomTariffConditionsSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           ReservationFixed is not null
                               ? new JProperty("reservationFixed",   ReservationFixed.ToJSON(CustomTariffFixedSerializer,
                                                                                             CustomTariffFixedPriceSerializer,
                                                                                             CustomTariffConditionsSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           ReservationTime is not null
                               ? new JProperty("reservationTime",    ReservationTime. ToJSON(CustomTariffTimeSerializer,
                                                                                             CustomTariffTimePriceSerializer,
                                                                                             CustomTariffConditionsSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           Energy is not null
                               ? new JProperty("energy",             Energy.          ToJSON(CustomTariffEnergySerializer,
                                                                                             CustomTariffEnergyPriceSerializer,
                                                                                             CustomTariffConditionsSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           ChargingTime is not null
                               ? new JProperty("chargingTIme",       ChargingTime.    ToJSON(CustomTariffTimeSerializer,
                                                                                             CustomTariffTimePriceSerializer,
                                                                                             CustomTariffConditionsSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,

                           IdleTime is not null
                               ? new JProperty("idleTime",           IdleTime.        ToJSON(CustomTariffTimeSerializer,
                                                                                             CustomTariffTimePriceSerializer,
                                                                                             CustomTariffConditionsSerializer,
                                                                                             CustomTaxRateSerializer))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomTariffSerializer is not null
                       ? CustomTariffSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tariff.
        /// </summary>
        public Tariff Clone()

            => new (

                   Id.               Clone(),
                   Currency.         Clone(),
                   Description.      Clone(),
                   ValidFrom,
                   ValidTo,
                   MinCost?.         Clone(),
                   MaxCost?.         Clone(),
                   FixedFee?.        Clone(),
                   ReservationFixed?.Clone(),
                   ReservationTime?. Clone(),
                   Energy?.          Clone(),
                   ChargingTime?.    Clone(),
                   IdleTime?.        Clone(),

                   SignKeys,
                   SignInfos,
                   Signatures.       Select(signature => signature.Clone()).ToArray(),

                   CustomData

               );

        #endregion


        #region Sign(KeyPair, out ErrorResponse, SignerName = null, Description = null, Timestamp = null, ...)

        public Boolean Sign(ECCKeyPair                                           KeyPair,
                            out String?                                          ErrorResponse,
                            String?                                              SignerName                          = null,
                            I18NString?                                          Description                         = null,
                            DateTime?                                            Timestamp                           = null,

                            CustomJObjectSerializerDelegate<Tariff>?             CustomTariffSerializer              = null,
                            CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                            CustomJObjectSerializerDelegate<Price>?              CustomPriceSerializer               = null,
                            CustomJObjectSerializerDelegate<TaxRate>?            CustomTaxRateSerializer             = null,
                            CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffConditionsSerializer    = null,
                            CustomJObjectSerializerDelegate<TariffEnergy>?       CustomTariffEnergySerializer        = null,
                            CustomJObjectSerializerDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceSerializer   = null,
                            CustomJObjectSerializerDelegate<TariffTime>?         CustomTariffTimeSerializer          = null,
                            CustomJObjectSerializerDelegate<TariffTimePrice>?    CustomTariffTimePriceSerializer     = null,
                            CustomJObjectSerializerDelegate<TariffFixed>?        CustomTariffFixedSerializer         = null,
                            CustomJObjectSerializerDelegate<TariffFixedPrice>?   CustomTariffFixedPriceSerializer    = null,
                            CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                            CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)

            => Sign(ToJSON(CustomTariffSerializer,
                           CustomMessageContentSerializer,
                           CustomPriceSerializer,
                           CustomTaxRateSerializer,
                           CustomTariffConditionsSerializer,
                           CustomTariffEnergySerializer,
                           CustomTariffEnergyPriceSerializer,
                           CustomTariffTimeSerializer,
                           CustomTariffTimePriceSerializer,
                           CustomTariffFixedSerializer,
                           CustomTariffFixedPriceSerializer,
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

        public Boolean Verify(out String?                                          ErrorResponse,
                              VerificationRuleActions?                             VerificationRuleAction              = null,

                              CustomJObjectSerializerDelegate<Tariff>?             CustomTariffSerializer              = null,
                              CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                              CustomJObjectSerializerDelegate<Price>?              CustomPriceSerializer               = null,
                              CustomJObjectSerializerDelegate<TaxRate>?            CustomTaxRateSerializer             = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffConditionsSerializer    = null,
                              CustomJObjectSerializerDelegate<TariffEnergy>?       CustomTariffEnergySerializer        = null,
                              CustomJObjectSerializerDelegate<TariffEnergyPrice>?  CustomTariffEnergyPriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffTime>?         CustomTariffTimeSerializer          = null,
                              CustomJObjectSerializerDelegate<TariffTimePrice>?    CustomTariffTimePriceSerializer     = null,
                              CustomJObjectSerializerDelegate<TariffFixed>?        CustomTariffFixedSerializer         = null,
                              CustomJObjectSerializerDelegate<TariffFixedPrice>?   CustomTariffFixedPriceSerializer    = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)

            => Verify(ToJSON(CustomTariffSerializer,
                             CustomMessageContentSerializer,
                             CustomPriceSerializer,
                             CustomTaxRateSerializer,
                             CustomTariffConditionsSerializer,
                             CustomTariffEnergySerializer,
                             CustomTariffEnergyPriceSerializer,
                             CustomTariffTimeSerializer,
                             CustomTariffTimePriceSerializer,
                             CustomTariffFixedSerializer,
                             CustomTariffFixedPriceSerializer,
                             CustomSignatureSerializer,
                             CustomCustomDataSerializer),
                      Context,
                      out ErrorResponse,
                      VerificationRuleAction);

        #endregion


        #region Operator overloading

        #region Operator == (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
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
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff? Tariff1,
                                           Tariff? Tariff2)

            => !(Tariff1 == Tariff2);

        #endregion

        #region Operator <  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
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
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff? Tariff1,
                                           Tariff? Tariff2)

            => !(Tariff1 > Tariff2);

        #endregion

        #region Operator >  (Tariff1, Tariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
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
        /// <param name="Tariff1">A tariff.</param>
        /// <param name="Tariff2">Another tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff? Tariff1,
                                           Tariff? Tariff2)

            => !(Tariff1 < Tariff2);

        #endregion

        #endregion

        #region IComparable<Tariff> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariffs.
        /// </summary>
        /// <param name="Object">A tariff to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Tariff tariff
                   ? CompareTo(tariff)
                   : throw new ArgumentException("The given object is not a tariff!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Tariff)

        /// <summary>
        /// Compares two tariffs.
        /// </summary>
        /// <param name="Tariff">A tariff to compare with.</param>
        public Int32 CompareTo(Tariff? Tariff)
        {

            if (Tariff is null)
                throw new ArgumentNullException(nameof(Tariff), "The given tariff must not be null!");

            var c = Id.             CompareTo(Tariff.Id);

            if (c == 0)
                c = Currency.       CompareTo(Tariff.Currency);

            if (c == 0 && ValidFrom.HasValue && Tariff.ValidFrom.HasValue)
                c = ValidFrom.Value.CompareTo(Tariff.ValidFrom.Value);

            if (c == 0 && ValidTo.  HasValue && Tariff.ValidTo.  HasValue)
                c = ValidTo.Value.  CompareTo(Tariff.ValidTo.Value);

            if (c == 0 && MinCost.  HasValue && Tariff.MinCost.  HasValue)
                c = MinCost.Value.  CompareTo(Tariff.MinCost.Value);

            if (c == 0 && MaxCost.  HasValue && Tariff.MaxCost.  HasValue)
                c = MaxCost.Value.  CompareTo(Tariff.MaxCost.Value);

            //ToDo: Compare tariff elements!

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Tariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariffs for equality.
        /// </summary>
        /// <param name="Object">A tariff to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Tariff tariff &&
                   Equals(tariff);

        #endregion

        #region Equals(Tariff)

        /// <summary>
        /// Compares two tariffs for equality.
        /// </summary>
        /// <param name="Tariff">A tariff to compare with.</param>
        public Boolean Equals(Tariff? Tariff)

            => Tariff is not null &&

               Id.                     Equals(Tariff.Id)           &&
               Currency.               Equals(Tariff.Currency)     &&
               Description.            Equals(Tariff.Description)  &&

            ((!ValidFrom.      HasValue    && !Tariff.ValidFrom.       HasValue) ||
              (ValidFrom.      HasValue    &&  Tariff.ValidFrom.       HasValue    && ValidFrom.Value. Equals(Tariff.ValidFrom.Value)))  &&

            ((!ValidTo.        HasValue    && !Tariff.ValidTo.         HasValue) ||
              (ValidTo.        HasValue    &&  Tariff.ValidTo.         HasValue    && ValidTo.  Value. Equals(Tariff.ValidTo.  Value)))  &&

            ((!MinCost.        HasValue    && !Tariff.MinCost.         HasValue) ||
              (MinCost.        HasValue    &&  Tariff.MinCost.         HasValue    && MinCost.  Value. Equals(Tariff.MinCost.  Value)))  &&

            ((!MaxCost.        HasValue    && !Tariff.MaxCost.         HasValue) ||
              (MaxCost.        HasValue    &&  Tariff.MaxCost.         HasValue    && MaxCost.  Value. Equals(Tariff.MaxCost.  Value)))  &&

            ((FixedFee         is     null &&  Tariff.FixedFee         is null)  ||
             (FixedFee         is not null &&  Tariff.FixedFee         is not null && FixedFee.        Equals(Tariff.FixedFee)))         &&

            ((ReservationFixed is     null &&  Tariff.ReservationFixed is null)  ||
             (ReservationFixed is not null &&  Tariff.ReservationFixed is not null && ReservationFixed.Equals(Tariff.ReservationFixed))) &&

            ((ReservationTime  is     null &&  Tariff.ReservationTime  is null)  ||
             (ReservationTime  is not null &&  Tariff.ReservationTime  is not null && ReservationTime. Equals(Tariff.ReservationTime)))  &&

             ((Energy       is     null &&  Tariff.Energy       is null)  ||
              (Energy       is not null &&  Tariff.Energy       is not null && Energy.         Equals(Tariff.Energy)))            &&

             ((ChargingTime is     null &&  Tariff.ChargingTime is null)  ||
              (ChargingTime is not null &&  Tariff.ChargingTime is not null && ChargingTime.   Equals(Tariff.ChargingTime)))      &&

             ((IdleTime     is     null &&  Tariff.IdleTime     is null)  ||
              (IdleTime     is not null &&  Tariff.IdleTime     is not null && IdleTime.       Equals(Tariff.IdleTime)))          &&

               base.Equals(Tariff);

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

                   $"{Id}: {Currency}",

                   Description.Count > 0
                       ? $", description: {Description.First()}"
                       : "",

                   ValidFrom.HasValue
                       ? $", valid from: {ValidFrom.Value}"
                       : "",

                   ValidTo.  HasValue
                       ? $", valid to: {ValidTo.Value}"
                       : "",

                   MaxCost.HasValue
                       ? $", max price: {MaxCost.Value} {Currency}"
                       : "",

                   MinCost.HasValue
                       ? $", min price: {MinCost.Value} {Currency}"
                       : "",

                   MaxCost.HasValue
                       ? $", max price: {MaxCost.Value} {Currency}"
                       : "",

                   FixedFee is not null
                       ? $", fixed fee: {FixedFee.Prices.Count()} prices and {FixedFee.TaxRates.Count()} tax rates"
                       : "",

                   ReservationFixed is not null
                       ? $", reservation fixed fee: {ReservationFixed.Prices.Count()} prices and {ReservationFixed.TaxRates.Count()} tax rates"
                       : "",

                   ReservationTime is not null
                       ? $", reservation time: {ReservationTime.Prices.Count()} prices and {ReservationTime.TaxRates.Count()} tax rates"
                       : "",

                   Energy is not null
                       ? $", energy: {Energy.Prices.Count()} prices and {Energy.TaxRates.Count()} tax rates"
                       : "",

                   ChargingTime is not null
                       ? $", charging time: {ChargingTime.Prices.Count()} prices and {ChargingTime.TaxRates.Count()} tax rates"
                       : "",

                   IdleTime is not null
                       ? $", idle time: {IdleTime.Prices.Count()} prices and {IdleTime.TaxRates.Count()} tax rates"
                       : ""

               );

        #endregion


    }

}
