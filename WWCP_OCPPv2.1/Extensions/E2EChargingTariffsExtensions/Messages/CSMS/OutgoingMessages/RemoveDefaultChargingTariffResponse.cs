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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A remove default charging tariff response.
    /// </summary>
    public class RemoveDefaultChargingTariffResponse : AResponse<CSMS.RemoveDefaultChargingTariffRequest,
                                                                      RemoveDefaultChargingTariffResponse>,
                                                       IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/removeDefaultChargingTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                                                   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The RemoveDefaultChargingTariff status.
        /// </summary>
        [Mandatory]
        public RemoveDefaultChargingTariffStatus                               Status             { get; }

        /// <summary>
        /// An optional element providing more information about the RemoveDefaultChargingTariff status.
        /// </summary>
        [Optional]
        public StatusInfo?                                                     StatusInfo         { get; }

        /// <summary>
        /// The optional enumeration of status infos for individual EVSEs.
        /// </summary>
        [Optional]
        public IEnumerable<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>  EVSEStatusInfos    { get; }

        #endregion

        #region Constructor(s)

        #region RemoveDefaultChargingTariffResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new remove default charging tariff response.
        /// </summary>
        /// <param name="Request">The remove default charging tariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="EVSEStatusInfos">An optional enumeration of status infos for individual EVSEs.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public RemoveDefaultChargingTariffResponse(CSMS.RemoveDefaultChargingTariffRequest                          Request,
                                                   RemoveDefaultChargingTariffStatus                                Status,
                                                   StatusInfo?                                                      StatusInfo          = null,
                                                   IEnumerable<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?  EVSEStatusInfos     = null,
                                                   DateTime?                                                        ResponseTimestamp   = null,

                                                   IEnumerable<KeyPair>?                                            SignKeys            = null,
                                                   IEnumerable<SignInfo>?                                           SignInfos           = null,
                                                   IEnumerable<OCPP.Signature>?                                     Signatures          = null,

                                                   CustomData?                                                      CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status           = Status;
            this.StatusInfo       = StatusInfo;
            this.EVSEStatusInfos  = EVSEStatusInfos ?? Array.Empty<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>();

            unchecked
            {

                hashCode = this.Status.         GetHashCode()       * 7 ^
                          (this.StatusInfo?.    GetHashCode() ?? 0) * 5 ^
                           this.EVSEStatusInfos.CalcHashCode()      * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion

        #region RemoveDefaultChargingTariffResponse(Request, Result)

        /// <summary>
        /// Create a new remove default charging tariff response.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
        /// <param name="Result">A result.</param>
        public RemoveDefaultChargingTariffResponse(CSMS.RemoveDefaultChargingTariffRequest  Request,
                                                   Result                                   Result)

            : base(Request,
                   Result)

        {

            this.Status           = RemoveDefaultChargingTariffStatus.Rejected;
            this.EVSEStatusInfos  = Request.EVSEIds.Select(evseId => new EVSEStatusInfo<RemoveDefaultChargingTariffStatus>(
                                                                         evseId,
                                                                         RemoveDefaultChargingTariffStatus.Rejected
                                                                     ));

            unchecked
            {

                hashCode = this.Status.         GetHashCode()       * 7 ^
                          (this.StatusInfo?.    GetHashCode() ?? 0) * 5 ^
                           this.EVSEStatusInfos.CalcHashCode()      * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomRemoveDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a remove default charging tariff response.
        /// </summary>
        /// <param name="Request">The remove default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRemoveDefaultChargingTariffResponseParser">A delegate to parse custom remove default charging tariff responses.</param>
        public static RemoveDefaultChargingTariffResponse Parse(CSMS.RemoveDefaultChargingTariffRequest                               Request,
                                                                JObject                                                            JSON,
                                                                CustomJObjectParserDelegate<RemoveDefaultChargingTariffResponse>?  CustomRemoveDefaultChargingTariffResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var removeDefaultChargingTariffResponse,
                         out var errorResponse,
                         CustomRemoveDefaultChargingTariffResponseParser) &&
                removeDefaultChargingTariffResponse is not null)
            {
                return removeDefaultChargingTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a remove default charging tariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out RemoveDefaultChargingTariffResponse, out ErrorResponse, CustomRemoveDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a remove default charging tariff response.
        /// </summary>
        /// <param name="Request">The remove default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RemoveDefaultChargingTariffResponse">The parsed remove default charging tariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoveDefaultChargingTariffResponseParser">A delegate to parse custom remove default charging tariff responses.</param>
        public static Boolean TryParse(CSMS.RemoveDefaultChargingTariffRequest                            Request,
                                       JObject                                                            JSON,
                                       out RemoveDefaultChargingTariffResponse?                           RemoveDefaultChargingTariffResponse,
                                       out String?                                                        ErrorResponse,
                                       CustomJObjectParserDelegate<RemoveDefaultChargingTariffResponse>?  CustomRemoveDefaultChargingTariffResponseParser   = null)
        {

            try
            {

                RemoveDefaultChargingTariffResponse = null;

                #region Status             [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "remove default charging tariff status",
                                         RemoveDefaultChargingTariffStatusExtensions.TryParse,
                                         out RemoveDefaultChargingTariffStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo         [optional]

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

                #region EVSEStatusInfos    [mandatory]

                if (!JSON.ParseMandatory("evseStatusInfos",
                                         "EVSE status infos",
                                         out JArray EVSEStatusInfosJSON,
                                         out ErrorResponse))
                {
                    return false;
                }

                var EVSEStatusInfos = new List<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>();

                foreach (var evseStatusInfoProperty in EVSEStatusInfosJSON)
                {

                    if (evseStatusInfoProperty is not JObject evseStatusInfoJObject)
                    {
                        ErrorResponse = "Invalid evseStatusInfo JSON object!";
                        return false;
                    }

                    if (!EVSEStatusInfo<RemoveDefaultChargingTariffStatus>.TryParse(evseStatusInfoJObject,
                                                                                    out var evseStatusInfo,
                                                                                    out ErrorResponse,
                                                                                    RemoveDefaultChargingTariffStatusExtensions.TryParse,
                                                                                    null) ||
                        evseStatusInfo is null)
                    {
                        return false;
                    }

                    EVSEStatusInfos.Add(evseStatusInfo);

                }

                #endregion

                #region Signatures         [optional, OCPP_CSE]

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

                #region CustomData         [optional]

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


                RemoveDefaultChargingTariffResponse = new RemoveDefaultChargingTariffResponse(

                                                          Request,
                                                          Status,
                                                          StatusInfo,
                                                          EVSEStatusInfos,
                                                          null,

                                                          null,
                                                          null,
                                                          Signatures,

                                                          CustomData

                                                      );

                if (CustomRemoveDefaultChargingTariffResponseParser is not null)
                    RemoveDefaultChargingTariffResponse = CustomRemoveDefaultChargingTariffResponseParser(JSON,
                                                                                                          RemoveDefaultChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                RemoveDefaultChargingTariffResponse  = null;
                ErrorResponse                        = "The given JSON representation of a remove default charging tariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRemoveDefaultChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoveDefaultChargingTariffResponseSerializer">A delegate to serialize custom remove default charging tariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomEVSEStatusInfoSerializer">A delegate to serialize custom EVSE status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffResponse>?                CustomRemoveDefaultChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                                         CustomStatusInfoSerializer                            = null,
                              CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?  CustomEVSEStatusInfoSerializer                        = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                                     CustomSignatureSerializer                             = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                         CustomCustomDataSerializer                            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",            Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",        StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                      CustomCustomDataSerializer))
                               : null,

                                 new JProperty("evseStatusInfos",   new JArray(EVSEStatusInfos.Select(evseStatusInfo => evseStatusInfo.ToJSON(status => status.AsText(),
                                                                                                                                              CustomEVSEStatusInfoSerializer,
                                                                                                                                              CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray (Signatures.    Select(signature      => signature.     ToJSON(CustomSignatureSerializer,
                                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRemoveDefaultChargingTariffResponseSerializer is not null
                       ? CustomRemoveDefaultChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The remove default charging tariff failed.
        /// </summary>
        public static RemoveDefaultChargingTariffResponse Failed(CSMS.RemoveDefaultChargingTariffRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (RemoveDefaultChargingTariffResponse1, RemoveDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two remove default charging tariff responses for equality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffResponse1">A remove default charging tariff response.</param>
        /// <param name="RemoveDefaultChargingTariffResponse2">Another remove default charging tariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoveDefaultChargingTariffResponse? RemoveDefaultChargingTariffResponse1,
                                           RemoveDefaultChargingTariffResponse? RemoveDefaultChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoveDefaultChargingTariffResponse1, RemoveDefaultChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (RemoveDefaultChargingTariffResponse1 is null || RemoveDefaultChargingTariffResponse2 is null)
                return false;

            return RemoveDefaultChargingTariffResponse1.Equals(RemoveDefaultChargingTariffResponse2);

        }

        #endregion

        #region Operator != (RemoveDefaultChargingTariffResponse1, RemoveDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two remove default charging tariff responses for inequality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffResponse1">A remove default charging tariff response.</param>
        /// <param name="RemoveDefaultChargingTariffResponse2">Another remove default charging tariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoveDefaultChargingTariffResponse? RemoveDefaultChargingTariffResponse1,
                                           RemoveDefaultChargingTariffResponse? RemoveDefaultChargingTariffResponse2)

            => !(RemoveDefaultChargingTariffResponse1 == RemoveDefaultChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoveDefaultChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remove default charging tariff responses for equality.
        /// </summary>
        /// <param name="Object">A remove default charging tariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoveDefaultChargingTariffResponse removeDefaultChargingTariffResponse &&
                   Equals(removeDefaultChargingTariffResponse);

        #endregion

        #region Equals(RemoveDefaultChargingTariffResponse)

        /// <summary>
        /// Compares two remove default charging tariff responses for equality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffResponse">A remove default charging tariff response to compare with.</param>
        public override Boolean Equals(RemoveDefaultChargingTariffResponse? RemoveDefaultChargingTariffResponse)

            => RemoveDefaultChargingTariffResponse is not null &&

               Status.Equals(RemoveDefaultChargingTariffResponse.Status) &&

             ((StatusInfo is     null && RemoveDefaultChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && RemoveDefaultChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(RemoveDefaultChargingTariffResponse.StatusInfo))) &&

               EVSEStatusInfos.Count().Equals(RemoveDefaultChargingTariffResponse.EVSEStatusInfos.Count()) &&
               EVSEStatusInfos.All(kvp => RemoveDefaultChargingTariffResponse.EVSEStatusInfos.Contains(kvp)) &&

               base.GenericEquals(RemoveDefaultChargingTariffResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}{(EVSEStatusInfos.Any() ? $", {EVSEStatusInfos.Select(evseStatusInfo => $"{evseStatusInfo.EVSEId} => '{evseStatusInfo.Status.AsText()}'").AggregateWith(", ")}" : "")}";

        #endregion

    }

}
