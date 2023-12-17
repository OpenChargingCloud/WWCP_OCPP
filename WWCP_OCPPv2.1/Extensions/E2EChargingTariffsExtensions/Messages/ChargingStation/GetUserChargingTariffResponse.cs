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

using System.Collections.ObjectModel;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    //Note: This command is a draft version of the OCPP 2.1 specification
    //       and might be subject to change in future versions of the specification!

    /// <summary>
    /// A get user charging tariff response.
    /// </summary>
    public class GetUserChargingTariffResponse : AResponse<CS.GetUserChargingTariffRequest,
                                                              GetUserChargingTariffResponse>,
                                                 IResponse
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

        #region GetUserChargingTariffResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get user charging tariff response.
        /// </summary>
        /// <param name="Request">The get user charging tariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetUserChargingTariffResponse(CS.GetUserChargingTariffRequest  Request,
                                             GenericStatus                    Status,
                                             StatusInfo?                      StatusInfo          = null,
                                             DateTime?                        ResponseTimestamp   = null,

                                             IEnumerable<KeyPair>?            SignKeys            = null,
                                             IEnumerable<SignInfo>?           SignInfos           = null,
                                             IEnumerable<OCPP.Signature>?     Signatures          = null,

                                             CustomData?                      CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

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

        #region GetUserChargingTariffResponse(Request, Result)

        /// <summary>
        /// Create a new get user charging tariff response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public GetUserChargingTariffResponse(CS.GetUserChargingTariffRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        {

            this.Status  = GenericStatus.Rejected;


            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion

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
                                                          CustomJObjectParserDelegate<GetUserChargingTariffResponse>?  CustomGetUserChargingTariffResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var getUserChargingTariffResponse,
                         out var errorResponse,
                         CustomGetUserChargingTariffResponseParser) &&
                getUserChargingTariffResponse is not null)
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
                                       out GetUserChargingTariffResponse?                           GetUserChargingTariffResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<GetUserChargingTariffResponse>?  CustomGetUserChargingTariffResponseParser   = null)
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
                                           OCPP.StatusInfo.TryParse,
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
                                              ChargingTariff.TryParse,
                                              out HashSet<ChargingTariff> ChargingTariffs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingTariffMap    [optional]

                var ChargingTariffMap = new Dictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>();

                if (JSON.ParseOptional("chargingTariffMap",
                                       "charging tariff map",
                                       out JObject ChargingTariffMapJSON,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    foreach (var chargingTariffProperty in ChargingTariffMapJSON.Properties())
                    {

                        var chargingTariffId = ChargingTariff_Id.TryParse(chargingTariffProperty.Name);

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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
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
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<ChargingTariff>?                    CustomChargingTariffSerializer                     = null,
                              CustomJObjectSerializerDelegate<Price>?                             CustomPriceSerializer                              = null,
                              CustomJObjectSerializerDelegate<TariffElement>?                     CustomTariffElementSerializer                      = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?                    CustomPriceComponentSerializer                     = null,
                              CustomJObjectSerializerDelegate<TaxRate>?                           CustomTaxRateSerializer                            = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?                CustomTariffRestrictionsSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                         CustomEnergyMixSerializer                          = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                      CustomEnergySourceSerializer                       = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?               CustomEnvironmentalImpactSerializer                = null,
                              CustomJObjectSerializerDelegate<IdToken>?                           CustomIdTokenSerializer                            = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?                    CustomAdditionalInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                    CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
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
        /// The get user charging tariff failed.
        /// </summary>
        public static GetUserChargingTariffResponse Failed(CS.GetUserChargingTariffRequest Request)

            => new (Request,
                    Result.Server());

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
