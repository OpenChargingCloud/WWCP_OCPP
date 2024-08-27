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

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetDefaultChargingTariff response.
    /// </summary>
    public class GetDefaultChargingTariffResponse : AResponse<GetDefaultChargingTariffRequest,
                                                                   GetDefaultChargingTariffResponse>,
                                                    IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getDefaultChargingTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                                                 Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The GetDefaultChargingTariff status.
        /// </summary>
        [Mandatory]
        public GenericStatus                                                 Status               { get; }

        /// <summary>
        /// An optional element providing more information about the GetDefaultChargingTariff status.
        /// </summary>
        [Optional]
        public StatusInfo?                                                   StatusInfo           { get; }

        /// <summary>
        /// An optional enuemration of default charging tariffs.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff>                                   ChargingTariffs      { get; }

        /// <summary>
        /// The optional map of charging tariffs to EVSE identifications.
        /// </summary>
        [Optional]
        public IReadOnlyDictionary<Tariff_Id, IEnumerable<EVSE_Id>>  ChargingTariffMap    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetDefaultChargingTariff response.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetDefaultChargingTariffResponse(GetDefaultChargingTariffRequest                                Request,
                                                GenericStatus                                                  Status,
                                                StatusInfo?                                                    StatusInfo            = null,
                                                IEnumerable<Tariff>?                                   ChargingTariffs       = null,
                                                IReadOnlyDictionary<Tariff_Id, IEnumerable<EVSE_Id>>?  ChargingTariffMap     = null,

                                                Result?                                                        Result                = null,
                                                DateTime?                                                      ResponseTimestamp     = null,

                                                SourceRouting?                                                 Destination           = null,
                                                NetworkPath?                                                   NetworkPath           = null,

                                                IEnumerable<KeyPair>?                                          SignKeys              = null,
                                                IEnumerable<SignInfo>?                                         SignInfos             = null,
                                                IEnumerable<Signature>?                                        Signatures            = null,

                                                CustomData?                                                    CustomData            = null,

                                                SerializationFormats?                                          SerializationFormat   = null,
                                                CancellationToken                                              CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status             = Status;
            this.StatusInfo         = StatusInfo;
            this.ChargingTariffs    = ChargingTariffs?.Distinct() ?? [];
            this.ChargingTariffMap  = ChargingTariffMap ?? new ReadOnlyDictionary<Tariff_Id, IEnumerable<EVSE_Id>>(
                                                               new Dictionary<Tariff_Id, IEnumerable<EVSE_Id>>()
                                                           );

            unchecked
            {

                hashCode = this.Status.           GetHashCode()       * 7 ^
                          (this.StatusInfo?.      GetHashCode() ?? 0) * 5 ^
                           this.ChargingTariffMap.CalcHashCode()      * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetDefaultChargingTariff response.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetDefaultChargingTariffResponseParser">A delegate to parse custom GetDefaultChargingTariff responses.</param>
        public static GetDefaultChargingTariffResponse Parse(GetDefaultChargingTariffRequest                                 Request,
                                                             JObject                                                         JSON,
                                                             SourceRouting                                                   Destination,
                                                             NetworkPath                                                     NetworkPath,
                                                             DateTime?                                                       ResponseTimestamp                              = null,
                                                             CustomJObjectParserDelegate<GetDefaultChargingTariffResponse>?  CustomGetDefaultChargingTariffResponseParser   = null
                                                             //CustomJObjectParserDelegate<StatusInfo>?                        CustomStatusInfoParser                         = null,
                                                             //CustomJObjectParserDelegate<Tariff>?                    CustomChargingTariffParser                     = null,
                                                             //CustomJObjectParserDelegate<Price>?                             CustomPriceParser                              = null,
                                                             ////CustomJObjectParserDelegate<TariffElement>?                     CustomTariffElementParser                      = null,
                                                             ////CustomJObjectParserDelegate<PriceComponent>?                    CustomPriceComponentParser                     = null,
                                                             //CustomJObjectParserDelegate<TaxRate>?                           CustomTaxRateParser                            = null,
                                                             //CustomJObjectParserDelegate<TariffConditions>?                CustomTariffRestrictionsParser                 = null,
                                                             //CustomJObjectParserDelegate<EnergyMix>?                         CustomEnergyMixParser                          = null,
                                                             //CustomJObjectParserDelegate<EnergySource>?                      CustomEnergySourceParser                       = null,
                                                             //CustomJObjectParserDelegate<EnvironmentalImpact>?               CustomEnvironmentalImpactParser                = null,
                                                             //CustomJObjectParserDelegate<IdToken>?                           CustomIdTokenParser                            = null,
                                                             //CustomJObjectParserDelegate<AdditionalInfo>?                    CustomAdditionalInfoParser                     = null,
                                                             //CustomJObjectParserDelegate<Signature>?                         CustomSignatureParser                          = null,
                                                             //CustomJObjectParserDelegate<CustomData>?                        CustomCustomDataParser                         = null
                                                             )
        {


            if (TryParse(Request,
                         JSON,
                     Destination,
                         NetworkPath,
                         out var getDefaultChargingTariffResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetDefaultChargingTariffResponseParser
                         //CustomStatusInfoParser,
                         //CustomChargingTariffParser,
                         //CustomPriceParser,
                         ////CustomTariffElementParser,
                         ////CustomPriceComponentParser,
                         //CustomTaxRateParser,
                         //CustomTariffRestrictionsParser,
                         //CustomEnergyMixParser,
                         //CustomEnergySourceParser,
                         //CustomEnvironmentalImpactParser,
                         //CustomIdTokenParser,
                         //CustomAdditionalInfoParser,
                         //CustomSignatureParser,
                         //CustomCustomDataParser
                         ))
            {
                return getDefaultChargingTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetDefaultChargingTariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetDefaultChargingTariffResponse, out ErrorResponse, CustomGetDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetDefaultChargingTariff response.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetDefaultChargingTariffResponse">The parsed GetDefaultChargingTariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetDefaultChargingTariffResponseParser">A delegate to parse custom GetDefaultChargingTariff responses.</param>
        public static Boolean TryParse(GetDefaultChargingTariffRequest                                 Request,
                                       JObject                                                         JSON,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out GetDefaultChargingTariffResponse?      GetDefaultChargingTariffResponse,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       DateTime?                                                       ResponseTimestamp                              = null,
                                       CustomJObjectParserDelegate<GetDefaultChargingTariffResponse>?  CustomGetDefaultChargingTariffResponseParser   = null
                                       //CustomJObjectParserDelegate<StatusInfo>?                        CustomStatusInfoParser                         = null,
                                       //CustomJObjectParserDelegate<Tariff>?                    CustomChargingTariffParser                     = null,
                                       //CustomJObjectParserDelegate<Price>?                             CustomPriceParser                              = null,
                                       //CustomJObjectParserDelegate<TariffElement>?                     CustomTariffElementParser                      = null,
                                       //CustomJObjectParserDelegate<PriceComponent>?                    CustomPriceComponentParser                     = null,
                                       //CustomJObjectParserDelegate<TaxRate>?                           CustomTaxRateParser                            = null,
                                       //CustomJObjectParserDelegate<TariffConditions>?                CustomTariffRestrictionsParser                 = null,
                                       //CustomJObjectParserDelegate<EnergyMix>?                         CustomEnergyMixParser                          = null,
                                       //CustomJObjectParserDelegate<EnergySource>?                      CustomEnergySourceParser                       = null,
                                       //CustomJObjectParserDelegate<EnvironmentalImpact>?               CustomEnvironmentalImpactParser                = null,
                                       //CustomJObjectParserDelegate<IdToken>?                           CustomIdTokenParser                            = null,
                                       //CustomJObjectParserDelegate<AdditionalInfo>?                    CustomAdditionalInfoParser                     = null,
                                       //CustomJObjectParserDelegate<Signature>?                         CustomSignatureParser                          = null,
                                       //CustomJObjectParserDelegate<CustomData>?                        CustomCustomDataParser                         = null
            )
        {

            try
            {

                GetDefaultChargingTariffResponse = null;

                #region Status               [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "GetDefaultChargingTariff status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo           [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingTariffs      [optional]

                if (JSON.ParseOptionalHashSet("chargingTariffs",
                                              "charging tariffs",
                                              Tariff.TryParse,
                                              out HashSet<Tariff> ChargingTariffs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingTariffMap    [optional]

                var ChargingTariffMap = new Dictionary<Tariff_Id, IEnumerable<EVSE_Id>>();

                if (JSON.ParseOptional("chargingTariffMap",
                                       "charging tariff map",
                                       out JObject ChargingTariffMapJSON,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    foreach (var chargingTariffProperty in ChargingTariffMapJSON.Properties())
                    {

                        var chargingTariffId = Tariff_Id.TryParse(chargingTariffProperty.Name);

                        if (!chargingTariffId.HasValue)
                            continue;

                        if (chargingTariffProperty.Value is not JArray evseIdArrayProperty)
                            continue;

                        var evseIds = new HashSet<EVSE_Id>();

                        foreach (var evseIdProperty in evseIdArrayProperty)
                        {

                            var evseIdString  = evseIdProperty?.Value<String>();
                            if (evseIdString is null)
                                continue;

                            var evseId        = EVSE_Id.TryParse(evseIdString);
                            if (!evseId.HasValue)
                                continue;

                            evseIds.Add(evseId.Value);

                        }

                        if (evseIds.Any())
                            ChargingTariffMap.Add(chargingTariffId.Value, evseIds);

                    }

                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                GetDefaultChargingTariffResponse = new GetDefaultChargingTariffResponse(

                                                       Request,
                                                       Status,
                                                       StatusInfo,
                                                       ChargingTariffs,
                                                       new ReadOnlyDictionary<Tariff_Id, IEnumerable<EVSE_Id>>(ChargingTariffMap),

                                                       null,
                                                       ResponseTimestamp,

                                                       Destination,
                                                       NetworkPath,

                                                       null,
                                                       null,
                                                       Signatures,

                                                       CustomData

                                                   );

                if (CustomGetDefaultChargingTariffResponseParser is not null)
                    GetDefaultChargingTariffResponse = CustomGetDefaultChargingTariffResponseParser(JSON,
                                                                                                    GetDefaultChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                GetDefaultChargingTariffResponse  = null;
                ErrorResponse                     = "The given JSON representation of a GetDefaultChargingTariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDefaultChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDefaultChargingTariffResponseSerializer">A delegate to serialize custom GetDefaultChargingTariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomChargingTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rate JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDefaultChargingTariffResponse>?  CustomGetDefaultChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<Tariff>?                    CustomChargingTariffSerializer                     = null,
                              //CustomJObjectSerializerDelegate<Price>?                             CustomPriceSerializer                              = null,
                              //CustomJObjectSerializerDelegate<TariffElement>?                     CustomTariffElementSerializer                      = null,
                              //CustomJObjectSerializerDelegate<PriceComponent>?                    CustomPriceComponentSerializer                     = null,
                              //CustomJObjectSerializerDelegate<TaxRate>?                           CustomTaxRateSerializer                            = null,
                              //CustomJObjectSerializerDelegate<TariffConditions>?                CustomTariffRestrictionsSerializer                 = null,
                              //CustomJObjectSerializerDelegate<EnergyMix>?                         CustomEnergyMixSerializer                          = null,
                              //CustomJObjectSerializerDelegate<EnergySource>?                      CustomEnergySourceSerializer                       = null,
                              //CustomJObjectSerializerDelegate<EnvironmentalImpact>?               CustomEnvironmentalImpactSerializer                = null,
                              //CustomJObjectSerializerDelegate<IdToken>?                           CustomIdTokenSerializer                            = null,
                              //CustomJObjectSerializerDelegate<AdditionalInfo>?                    CustomAdditionalInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null
            )
        {

            var json = JSONObject.Create(

                                 new JProperty("status",              Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",          StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer))
                               : null,

                           ChargingTariffs.Any()
                               ? new JProperty("chargingTariffs",     new JArray (ChargingTariffs.  Select(chargingTariff => chargingTariff.ToJSON(CustomChargingTariffSerializer
                                                                                                                                                   //CustomPriceSerializer,
                                                                                                                                                   //CustomTariffElementSerializer,
                                                                                                                                                   //CustomPriceComponentSerializer,
                                                                                                                                                   //CustomTaxRateSerializer,
                                                                                                                                                   //CustomTariffRestrictionsSerializer,
                                                                                                                                                   //CustomEnergyMixSerializer,
                                                                                                                                                   //CustomEnergySourceSerializer,
                                                                                                                                                   //CustomEnvironmentalImpactSerializer,
                                                                                                                                                   //CustomIdTokenSerializer,
                                                                                                                                                   //CustomAdditionalInfoSerializer,
                                                                                                                                                   //CustomSignatureSerializer,
                                                                                                                                                   //CustomCustomDataSerializer
                                                                                                                                                   ))))
                               : null,

                           ChargingTariffMap.Any()
                               ? new JProperty("chargingTariffMap",   new JObject(ChargingTariffMap.Select(kvp => new JProperty(kvp.Key.ToString(),
                                                                                                                  new JArray   (kvp.Value.Select(evseId => evseId.ToString()))))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray (Signatures.       Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                         CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetDefaultChargingTariffResponseSerializer is not null
                       ? CustomGetDefaultChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetDefaultChargingTariff failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        public static GetDefaultChargingTariffResponse RequestError(GetDefaultChargingTariffRequest  Request,
                                                                    EventTracking_Id                 EventTrackingId,
                                                                    ResultCode                       ErrorCode,
                                                                    String?                          ErrorDescription    = null,
                                                                    JObject?                         ErrorDetails        = null,
                                                                    DateTime?                        ResponseTimestamp   = null,

                                                                    SourceRouting?                   Destination         = null,
                                                                    NetworkPath?                     NetworkPath         = null,

                                                                    IEnumerable<KeyPair>?            SignKeys            = null,
                                                                    IEnumerable<SignInfo>?           SignInfos           = null,
                                                                    IEnumerable<Signature>?          Signatures          = null,

                                                                    CustomData?                      CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,
                   null,
                   null,
                  OCPPv2_1.Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The GetDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetDefaultChargingTariffResponse FormationViolation(GetDefaultChargingTariffRequest  Request,
                                                                          String                           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetDefaultChargingTariffResponse SignatureError(GetDefaultChargingTariffRequest  Request,
                                                                      String                           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetDefaultChargingTariffResponse Failed(GetDefaultChargingTariffRequest  Request,
                                                              String?                          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetDefaultChargingTariff failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetDefaultChargingTariffResponse ExceptionOccured(GetDefaultChargingTariffRequest  Request,
                                                                        Exception                        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetDefaultChargingTariffResponse1, GetDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two GetDefaultChargingTariff responses for equality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffResponse1">A GetDefaultChargingTariff response.</param>
        /// <param name="GetDefaultChargingTariffResponse2">Another GetDefaultChargingTariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse1,
                                           GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDefaultChargingTariffResponse1, GetDefaultChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetDefaultChargingTariffResponse1 is null || GetDefaultChargingTariffResponse2 is null)
                return false;

            return GetDefaultChargingTariffResponse1.Equals(GetDefaultChargingTariffResponse2);

        }

        #endregion

        #region Operator != (GetDefaultChargingTariffResponse1, GetDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two GetDefaultChargingTariff responses for inequality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffResponse1">A GetDefaultChargingTariff response.</param>
        /// <param name="GetDefaultChargingTariffResponse2">Another GetDefaultChargingTariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse1,
                                           GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse2)

            => !(GetDefaultChargingTariffResponse1 == GetDefaultChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDefaultChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetDefaultChargingTariff responses for equality.
        /// </summary>
        /// <param name="Object">A GetDefaultChargingTariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDefaultChargingTariffResponse getDefaultChargingTariffResponse &&
                   Equals(getDefaultChargingTariffResponse);

        #endregion

        #region Equals(GetDefaultChargingTariffResponse)

        /// <summary>
        /// Compares two GetDefaultChargingTariff responses for equality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffResponse">A GetDefaultChargingTariff response to compare with.</param>
        public override Boolean Equals(GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse)

            => GetDefaultChargingTariffResponse is not null &&

               Status.Equals(GetDefaultChargingTariffResponse.Status) &&

             ((StatusInfo is     null && GetDefaultChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && GetDefaultChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(GetDefaultChargingTariffResponse.StatusInfo))) &&

               ChargingTariffs.  Count().Equals(GetDefaultChargingTariffResponse.ChargingTariffs.  Count())       &&
               ChargingTariffs.  All(kvp =>     GetDefaultChargingTariffResponse.ChargingTariffs.  Contains(kvp)) &&

               ChargingTariffMap.Count().Equals(GetDefaultChargingTariffResponse.ChargingTariffMap.Count())       &&
               ChargingTariffMap.All(kvp => GetDefaultChargingTariffResponse.    ChargingTariffMap.Contains(kvp)) &&

               base.GenericEquals(GetDefaultChargingTariffResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}{(ChargingTariffMap.Any() ? $", {ChargingTariffMap.Select(kvp => $"'{kvp.Key}' => '{kvp.Value.AggregateWith(",")}'").AggregateWith("; ")}" : "")}";

        #endregion

    }

}
