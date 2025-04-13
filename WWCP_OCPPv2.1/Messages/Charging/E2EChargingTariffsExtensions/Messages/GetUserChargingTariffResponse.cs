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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    //Note: This command is a draft version of the OCPP 2.1 specification
    //       and might be subject to change in future versions of the specification!

    /// <summary>
    /// A get user charging tariff response.
    /// </summary>
    public class GetUserChargingTariffResponse : AResponse<GetUserChargingTariffRequest,
                                                           GetUserChargingTariffResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getUserChargingTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The generic status.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the status.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get user charging tariff response.
        /// </summary>
        /// <param name="Request">The get user charging tariff request leading to this response.</param>
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public GetUserChargingTariffResponse(CS.GetUserChargingTariffRequest  Request,
                                             GenericStatus                    Status,
                                             StatusInfo?                      StatusInfo            = null,

                                             Result?                          Result                = null,
                                             DateTime?                        ResponseTimestamp     = null,

                                             SourceRouting?                   Destination           = null,
                                             NetworkPath?                     NetworkPath           = null,

                                             IEnumerable<KeyPair>?            SignKeys              = null,
                                             IEnumerable<SignInfo>?           SignInfos             = null,
                                             IEnumerable<Signature>?          Signatures            = null,

                                             CustomData?                      CustomData            = null,

                                             SerializationFormats?            SerializationFormat   = null,
                                             CancellationToken                CancellationToken     = default)

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

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetUserChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get user charging tariff response.
        /// </summary>
        /// <param name="Request">The get user charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetUserChargingTariffResponseParser">A delegate to parse custom get user charging tariff responses.</param>
        public static GetUserChargingTariffResponse Parse(CS.GetUserChargingTariffRequest                              Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<GetUserChargingTariffResponse>?  CustomGetUserChargingTariffResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Tariff>?                 CustomChargingTariffParser                  = null,
                                                          //CustomJObjectParserDelegate<Price>?                          CustomPriceParser                           = null,
                                                          //CustomJObjectParserDelegate<TariffElement>?                  CustomTariffElementParser                   = null,
                                                          //CustomJObjectParserDelegate<PriceComponent>?                 CustomPriceComponentParser                  = null,
                                                          //CustomJObjectParserDelegate<TaxRate>?                        CustomTaxRateParser                         = null,
                                                          //CustomJObjectParserDelegate<TariffConditions>?             CustomTariffRestrictionsParser              = null,
                                                          //CustomJObjectParserDelegate<EnergyMix>?                      CustomEnergyMixParser                       = null,
                                                          //CustomJObjectParserDelegate<EnergySource>?                   CustomEnergySourceParser                    = null,
                                                          //CustomJObjectParserDelegate<EnvironmentalImpact>?            CustomEnvironmentalImpactParser             = null,
                                                          //CustomJObjectParserDelegate<IdToken>?                        CustomIdTokenParser                         = null,
                                                          //CustomJObjectParserDelegate<AdditionalInfo>?                 CustomAdditionalInfoParser                  = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {


            if (TryParse(Request,
                         JSON,
                     Destination,
                         NetworkPath,
                         out var getUserChargingTariffResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetUserChargingTariffResponseParser,
                         CustomStatusInfoParser,
                         CustomChargingTariffParser,
                         //CustomPriceParser,
                         //CustomTariffElementParser,
                         //CustomPriceComponentParser,
                         //CustomTaxRateParser,
                         //CustomTariffRestrictionsParser,
                         //CustomEnergyMixParser,
                         //CustomEnergySourceParser,
                         //CustomEnvironmentalImpactParser,
                         //CustomIdTokenParser,
                         //CustomAdditionalInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getUserChargingTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a get user charging tariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetUserChargingTariffResponse, out ErrorResponse, CustomGetUserChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get user charging tariff response.
        /// </summary>
        /// <param name="Request">The get user charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetUserChargingTariffResponse">The parsed get user charging tariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetUserChargingTariffResponseParser">A delegate to parse custom get user charging tariff responses.</param>
        public static Boolean TryParse(CS.GetUserChargingTariffRequest                              Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out GetUserChargingTariffResponse?      GetUserChargingTariffResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<GetUserChargingTariffResponse>?  CustomGetUserChargingTariffResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Tariff>?                 CustomChargingTariffParser                  = null,
                                       //CustomJObjectParserDelegate<Price>?                          CustomPriceParser                           = null,
                                       //CustomJObjectParserDelegate<TariffElement>?                  CustomTariffElementParser                   = null,
                                       //CustomJObjectParserDelegate<PriceComponent>?                 CustomPriceComponentParser                  = null,
                                       //CustomJObjectParserDelegate<TaxRate>?                        CustomTaxRateParser                         = null,
                                       //CustomJObjectParserDelegate<TariffConditions>?             CustomTariffRestrictionsParser              = null,
                                       //CustomJObjectParserDelegate<EnergyMix>?                      CustomEnergyMixParser                       = null,
                                       //CustomJObjectParserDelegate<EnergySource>?                   CustomEnergySourceParser                    = null,
                                       //CustomJObjectParserDelegate<EnvironmentalImpact>?            CustomEnvironmentalImpactParser             = null,
                                       //CustomJObjectParserDelegate<IdToken>?                        CustomIdTokenParser                         = null,
                                       //CustomJObjectParserDelegate<AdditionalInfo>?                 CustomAdditionalInfoParser                  = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                GetUserChargingTariffResponse = null;

                #region Status               [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "get user charging tariff status",
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
                                           out StatusInfo? StatusInfo,
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


                GetUserChargingTariffResponse = new GetUserChargingTariffResponse(

                                                    Request,
                                                    Status,
                                                    StatusInfo,
                                                    //ChargingTariffs,
                                                    //new ReadOnlyDictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>(ChargingTariffMap),

                                                    null,
                                                    ResponseTimestamp,

                                                Destination,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomGetUserChargingTariffResponseParser is not null)
                    GetUserChargingTariffResponse = CustomGetUserChargingTariffResponseParser(JSON,
                                                                                      GetUserChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                GetUserChargingTariffResponse  = null;
                ErrorResponse                     = "The given JSON representation of a get user charging tariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetUserChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetUserChargingTariffResponseSerializer">A delegate to serialize custom get user charging tariff responses.</param>
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetUserChargingTariffResponse>?  CustomGetUserChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Tariff>?                 CustomChargingTariffSerializer                  = null,
                              //CustomJObjectSerializerDelegate<Price>?                          CustomPriceSerializer                           = null,
                              //CustomJObjectSerializerDelegate<TariffElement>?                  CustomTariffElementSerializer                   = null,
                              //CustomJObjectSerializerDelegate<PriceComponent>?                 CustomPriceComponentSerializer                  = null,
                              //CustomJObjectSerializerDelegate<TaxRate>?                        CustomTaxRateSerializer                         = null,
                              //CustomJObjectSerializerDelegate<TariffConditions>?             CustomTariffRestrictionsSerializer              = null,
                              //CustomJObjectSerializerDelegate<EnergyMix>?                      CustomEnergyMixSerializer                       = null,
                              //CustomJObjectSerializerDelegate<EnergySource>?                   CustomEnergySourceSerializer                    = null,
                              //CustomJObjectSerializerDelegate<EnvironmentalImpact>?            CustomEnvironmentalImpactSerializer             = null,
                              //CustomJObjectSerializerDelegate<IdToken>?                        CustomIdTokenSerializer                         = null,
                              //CustomJObjectSerializerDelegate<AdditionalInfo>?                 CustomAdditionalInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",              Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",          StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer))
                               : null,

                           //ChargingTariffs.Any()
                           //    ? new JProperty("chargingTariffs",     new JArray (ChargingTariffs.  Select(chargingTariff => chargingTariff.ToJSON(CustomChargingTariffSerializer,
                           //                                                                                                                        CustomPriceSerializer,
                           //                                                                                                                        CustomTariffElementSerializer,
                           //                                                                                                                        CustomPriceComponentSerializer,
                           //                                                                                                                        CustomTaxRateSerializer,
                           //                                                                                                                        CustomTariffRestrictionsSerializer,
                           //                                                                                                                        CustomEnergyMixSerializer,
                           //                                                                                                                        CustomEnergySourceSerializer,
                           //                                                                                                                        CustomEnvironmentalImpactSerializer,
                           //                                                                                                                        CustomIdTokenSerializer,
                           //                                                                                                                        CustomAdditionalInfoSerializer,
                           //                                                                                                                        CustomSignatureSerializer,
                           //                                                                                                                        CustomCustomDataSerializer))))
                           //    : null,

                           //ChargingTariffMap.Any()
                           //    ? new JProperty("chargingTariffMap",   new JObject(ChargingTariffMap.Select(kvp => new JProperty(kvp.Key.ToString(),
                           //                                                                                       new JArray   (kvp.Value.Select(evseId => evseId.ToString()))))))
                           //    : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray (Signatures.       Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                         CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetUserChargingTariffResponseSerializer is not null
                       ? CustomGetUserChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetDefaultChargingTariff failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        public static GetUserChargingTariffResponse RequestError(CS.GetUserChargingTariffRequest  Request,
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
        public static GetUserChargingTariffResponse FormationViolation(CS.GetUserChargingTariffRequest  Request,
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
        public static GetUserChargingTariffResponse SignatureError(CS.GetUserChargingTariffRequest  Request,
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
        public static GetUserChargingTariffResponse Failed(CS.GetUserChargingTariffRequest  Request,
                                                           String?                          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetDefaultChargingTariff failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetUserChargingTariffResponse ExceptionOccurred(CS.GetUserChargingTariffRequest  Request,
                                                                     Exception                        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetUserChargingTariffResponse1, GetUserChargingTariffResponse2)

        /// <summary>
        /// Compares two get user charging tariff responses for equality.
        /// </summary>
        /// <param name="GetUserChargingTariffResponse1">A get user charging tariff response.</param>
        /// <param name="GetUserChargingTariffResponse2">Another get user charging tariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetUserChargingTariffResponse? GetUserChargingTariffResponse1,
                                           GetUserChargingTariffResponse? GetUserChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetUserChargingTariffResponse1, GetUserChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetUserChargingTariffResponse1 is null || GetUserChargingTariffResponse2 is null)
                return false;

            return GetUserChargingTariffResponse1.Equals(GetUserChargingTariffResponse2);

        }

        #endregion

        #region Operator != (GetUserChargingTariffResponse1, GetUserChargingTariffResponse2)

        /// <summary>
        /// Compares two get user charging tariff responses for inequality.
        /// </summary>
        /// <param name="GetUserChargingTariffResponse1">A get user charging tariff response.</param>
        /// <param name="GetUserChargingTariffResponse2">Another get user charging tariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetUserChargingTariffResponse? GetUserChargingTariffResponse1,
                                           GetUserChargingTariffResponse? GetUserChargingTariffResponse2)

            => !(GetUserChargingTariffResponse1 == GetUserChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<GetUserChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get user charging tariff responses for equality.
        /// </summary>
        /// <param name="Object">A get user charging tariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetUserChargingTariffResponse getUserChargingTariffResponse &&
                   Equals(getUserChargingTariffResponse);

        #endregion

        #region Equals(GetUserChargingTariffResponse)

        /// <summary>
        /// Compares two get user charging tariff responses for equality.
        /// </summary>
        /// <param name="GetUserChargingTariffResponse">A get user charging tariff response to compare with.</param>
        public override Boolean Equals(GetUserChargingTariffResponse? GetUserChargingTariffResponse)

            => GetUserChargingTariffResponse is not null &&

               Status.Equals(GetUserChargingTariffResponse.Status) &&

             ((StatusInfo is     null && GetUserChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && GetUserChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(GetUserChargingTariffResponse.StatusInfo))) &&

               //ChargingTariffs.  Count().Equals(GetUserChargingTariffResponse.ChargingTariffs.  Count())       &&
               //ChargingTariffs.  All(kvp =>     GetUserChargingTariffResponse.ChargingTariffs.  Contains(kvp)) &&

               //ChargingTariffMap.Count().Equals(GetUserChargingTariffResponse.ChargingTariffMap.Count())       &&
               //ChargingTariffMap.All(kvp => GetUserChargingTariffResponse.    ChargingTariffMap.Contains(kvp)) &&

               base.GenericEquals(GetUserChargingTariffResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}";

        #endregion

    }

}
