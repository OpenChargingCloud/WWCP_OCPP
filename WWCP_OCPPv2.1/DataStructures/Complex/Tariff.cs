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
    /// Extension methods for EV driver charging tariffs (B2C).
    /// </summary>
    public static class TariffExtensions
    {

        #region GenerateWithHashId(ProviderId, ProviderName, Currency, TariffElements, ...)

        /// <summary>
        /// Generate a new read-only signable EV driver charging tariff (B2C) having an identification based on the SHA256 of its information.
        /// </summary>
        public static Tariff GenerateWithHashId(Provider_Id                                          ProviderId,
                                                DisplayTexts                                         ProviderName,
                                                Currency                                             Currency,
                                                MessageContents?                                     Description                         = null,
                                                Price?                                               MinPrice                            = null,
                                                Price?                                               MaxPrice                            = null,
                                                TariffEnergy?                                        Energy                              = null,
                                                TariffTime?                                          ChargingTime                        = null,
                                                TariffTime?                                          IdleTime                            = null,
                                                TariffFixed?                                         FixedFee                            = null,
                                                CustomData?                                          CustomData                          = null,

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
                             MinPrice,
                             MaxPrice,
                             Energy,
                             ChargingTime,
                             IdleTime,
                             FixedFee,

                             null,
                             null,
                             null,

                             CustomData

                         );


            return new (

                   Tariff_Id.New(
                       ProviderId,
                       SHA256.HashData(

                           tariff.ToJSON(CustomTariffSerializer,
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
                                           SignableMessage.DefaultJSONConverters).

                                  ToUTF8Bytes()

                       ).ToBase64()
                   ),

                   tariff.Currency,
                   tariff.Description,
                   tariff.MinPrice,
                   tariff.MaxPrice,
                   tariff.Energy,
                   tariff.ChargingTime,
                   tariff.IdleTime,
                   tariff.FixedFee,

                   null,
                   null,
                   tariff.Signatures,

                   tariff.CustomData

               );

        }

        #endregion

    }


    /// <summary>
    /// A read-only signable EV driver charging tariff (B2C).
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
        /// The global unique and unique in time identification of the charging tariff.
        /// [Max: 36]
        /// </summary>
        [Mandatory]
        public   Tariff_Id        Id              { get; }

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
        public   Currency         Currency        { get; }

        /// <summary>
        /// The optional multi-language tariff description.
        /// </summary>
        [Optional]
        public   MessageContents  Description     { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will at least cost
        /// this amount. This is different from a FLAT fee (Start Tariff, Transaction Fee),
        /// as a FLAT fee is a fixed amount that has to be paid for any Charging Session.
        /// A minimum price indicates that when the cost of a charging session is lower
        /// than this amount, the cost of the charging session will be equal to this amount.
        /// </summary>
        [Optional]
        public   Price?           MinPrice        { get; }

        /// <summary>
        /// When this optional field is set, a charging session with this tariff will NOT
        /// cost more than this amount.
        /// </summary>
        [Optional]
        public   Price?           MaxPrice        { get; }

        /// <summary>
        /// 
        /// </summary>
        [Optional]
        public   TariffEnergy?    Energy          { get; }

        /// <summary>
        /// 
        /// </summary>
        [Optional]
        public   TariffTime?      ChargingTime    { get; }

        /// <summary>
        /// 
        /// </summary>
        [Optional]
        public   TariffTime?      IdleTime        { get; }

        /// <summary>
        /// 
        /// </summary>
        [Optional]
        public   TariffFixed?     FixedFee        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new read-only signable EV driver charging tariff (B2C).
        /// </summary>
        /// <param name="Id">A global unique and unique in time identification of the charging tariff.</param>
        /// <param name="Currency">An ISO 4217 code of the currency used for this tariff.</param>
        /// <param name="Description">An optional multi-language tariff description.</param>
        /// <param name="MinPrice">When this optional field is set, a charging session with this tariff will at least cost this amount.</param>
        /// <param name="MaxPrice">When this optional field is set, a charging session with this tariff will NOT cost more than this amount.</param>
        /// <param name="Energy"></param>
        /// <param name="ChargingTime"></param>
        /// <param name="IdleTime"></param>
        /// <param name="FixedFee"></param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this charging tariff.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this charging tariff.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Tariff(Tariff_Id                Id,
                      Currency                 Currency,
                      MessageContents?         Description    = null,
                      Price?                   MinPrice       = null,
                      Price?                   MaxPrice       = null,
                      TariffEnergy?            Energy         = null,
                      TariffTime?              ChargingTime   = null,
                      TariffTime?              IdleTime       = null,
                      TariffFixed?             FixedFee       = null,

                      IEnumerable<KeyPair>?    SignKeys       = null,
                      IEnumerable<SignInfo>?   SignInfos      = null,
                      IEnumerable<Signature>?  Signatures     = null,

                      CustomData?              CustomData     = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            this.Id            = Id;
            this.Currency      = Currency;
            this.Description   = Description ?? MessageContents.Empty;
            this.MinPrice      = MinPrice;
            this.MaxPrice      = MaxPrice;
            this.Energy        = Energy;
            this.ChargingTime  = ChargingTime;
            this.IdleTime      = IdleTime;
            this.FixedFee      = FixedFee;

            unchecked
            {

                hashCode = this.Id.           GetHashCode()       * 29 ^
                           this.Currency.     GetHashCode()       * 23 ^
                           this.Description.  GetHashCode()       * 19 ^
                          (this.MinPrice?.    GetHashCode() ?? 0) * 17 ^
                          (this.MaxPrice?.    GetHashCode() ?? 0) * 13 ^
                          (this.Energy?.      GetHashCode() ?? 0) * 11 ^
                          (this.ChargingTime?.GetHashCode() ?? 0) *  7 ^
                          (this.IdleTime?.    GetHashCode() ?? 0) *  5 ^
                          (this.FixedFee?.    GetHashCode() ?? 0) *  3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, TariffIdURL = null, CustomTariffParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffIdURL">An optional charging tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom charging tariff JSON objects.</param>
        public static Tariff Parse(JObject                                       JSON,
                                           Tariff_Id?                            TariffIdURL   = null,
                                           CustomJObjectParserDelegate<Tariff>?  CustomTariffParser    = null)
        {

            if (TryParse(JSON,
                         out var tariff,
                         out var errorResponse,
                         TariffIdURL,
                         CustomTariffParser) &&
                tariff is not null)
            {
                return tariff;
            }

            throw new ArgumentException("The given JSON representation of a charging tariff is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Tariff, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed charging tariff.</param>
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
        /// Try to parse the given JSON representation of a charging tariff.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Tariff">The parsed charging tariff.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="TariffIdURL">An optional charging tariff identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomTariffParser">A delegate to parse custom charging tariff JSON objects.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out Tariff?      Tariff,
                                       [NotNullWhen(false)] out String?      ErrorResponse,
                                       Tariff_Id?                            TariffIdURL                  = null,
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

                #region Parse Id               [optional]

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

                #region Parse Currency        [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description     [optional]

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

                #region Parse MinPrice        [optional]

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

                #region Parse MaxPrice        [optional]

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

                #region Parse Energy          [optional]

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

                #region Parse ChargingTime    [optional]

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

                #region Parse IdleTime        [optional]

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

                #region Parse FixedFee        [optional]

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

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

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
                                     MinPrice,
                                     MaxPrice,
                                     Energy,
                                     ChargingTime,
                                     IdleTime,
                                     FixedFee,

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
                Tariff  = default;
                ErrorResponse   = "The given JSON representation of a charging tariff is invalid: " + e.Message;
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

                                 new JProperty("id",             Id.              ToString()),
                                 new JProperty("currency",       Currency.        ISOCode),

                           Description.       Any()
                               ? new JProperty("description",    Description.     ToJSON(CustomMessageContentSerializer,
                                                                                         CustomCustomDataSerializer))
                               : null,

                           MinPrice.HasValue
                               ? new JProperty("minPrice",       MinPrice.  Value.ToJSON(CustomPriceSerializer,
                                                                                         CustomTaxRateSerializer))
                               : null,

                           MaxPrice.HasValue
                               ? new JProperty("maxPrice",       MaxPrice.  Value.ToJSON(CustomPriceSerializer,
                                                                                         CustomTaxRateSerializer))
                               : null,

                           Energy is not null
                               ? new JProperty("energy",         Energy.          ToJSON(CustomTariffEnergySerializer,
                                                                                         CustomTariffEnergyPriceSerializer,
                                                                                         CustomTariffConditionsSerializer,
                                                                                         CustomTaxRateSerializer))
                               : null,

                           ChargingTime is not null
                               ? new JProperty("chargingTIme",   ChargingTime.    ToJSON(CustomTariffTimeSerializer,
                                                                                         CustomTariffTimePriceSerializer,
                                                                                         CustomTariffConditionsSerializer,
                                                                                         CustomTaxRateSerializer))
                               : null,

                           IdleTime is not null
                               ? new JProperty("idleTime",       IdleTime.        ToJSON(CustomTariffTimeSerializer,
                                                                                         CustomTariffTimePriceSerializer,
                                                                                         CustomTariffConditionsSerializer,
                                                                                         CustomTaxRateSerializer))
                               : null,

                           FixedFee is not null
                               ? new JProperty("fixedFee",       FixedFee.        ToJSON(CustomTariffFixedSerializer,
                                                                                         CustomTariffFixedPriceSerializer,
                                                                                         CustomTariffConditionsSerializer,
                                                                                         CustomTaxRateSerializer))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",     new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                            CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomTariffSerializer is not null
                       ? CustomTariffSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charging tariff.
        /// </summary>
        public Tariff Clone()

            => new (

                   Id.           Clone,
                   Currency.     Clone,
                   Description.  Clone(),
                   MinPrice?.    Clone(),
                   MaxPrice?.    Clone(),
                   Energy?.      Clone(),
                   ChargingTime?.Clone(),
                   IdleTime?.    Clone(),
                   FixedFee?.    Clone(),

                   SignKeys,
                   SignInfos,
                   Signatures.   Select(signature => signature.Clone()).ToArray(),

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

               Id.                     Equals(Tariff.Id)           &&
               Currency.               Equals(Tariff.Currency)     &&
               Description.            Equals(Tariff.Description)  &&

            ((!MinPrice.    HasValue    && !Tariff.MinPrice.    HasValue) ||
              (MinPrice.    HasValue    &&  Tariff.MinPrice.    HasValue    && MinPrice.Value.Equals(Tariff.MinPrice.  Value))) &&

            ((!MaxPrice.    HasValue    && !Tariff.MaxPrice.    HasValue) ||
              (MaxPrice.    HasValue    &&  Tariff.MaxPrice.    HasValue    && MaxPrice.Value.Equals(Tariff.MaxPrice.  Value))) &&

             ((Energy       is     null &&  Tariff.Energy       is null)  ||
              (Energy       is not null &&  Tariff.Energy       is not null && Energy.        Equals(Tariff.Energy)))           &&

             ((ChargingTime is     null &&  Tariff.ChargingTime is null)  ||
              (ChargingTime is not null &&  Tariff.ChargingTime is not null && ChargingTime.  Equals(Tariff.ChargingTime)))     &&

             ((IdleTime     is     null &&  Tariff.IdleTime     is null)  ||
              (IdleTime     is not null &&  Tariff.IdleTime     is not null && IdleTime.      Equals(Tariff.IdleTime)))         &&

             ((FixedFee     is     null &&  Tariff.FixedFee     is null)  ||
              (FixedFee     is not null &&  Tariff.FixedFee     is not null && FixedFee.      Equals(Tariff.FixedFee)))         &&

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

                   MinPrice.HasValue
                       ? $", min price: {MinPrice.Value} {Currency}"
                       : "",

                   MaxPrice.HasValue
                       ? $", max price: {MaxPrice.Value} {Currency}"
                       : "",

                   Energy is not null
                       ? $", energy: {Energy.Prices.Count()} prices and {Energy.TaxRates.Count()} tax rates"
                       : "",

                   ChargingTime is not null
                       ? $", charging time: {ChargingTime.Prices.Count()} prices and {ChargingTime.TaxRates.Count()} tax rates"
                       : "",

                   IdleTime is not null
                       ? $", idle time: {IdleTime.Prices.Count()} prices and {IdleTime.TaxRates.Count()} tax rates"
                       : "",

                   FixedFee is not null
                       ? $", fixed fee: {FixedFee.Prices.Count()} prices and {FixedFee.TaxRates.Count()} tax rates"
                       : ""

               );

        #endregion


    }

}
