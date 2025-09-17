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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The RemoveDefaultChargingTariff response.
    /// </summary>
    public class RemoveDefaultChargingTariffResponse : AResponse<RemoveDefaultChargingTariffRequest,
                                                                 RemoveDefaultChargingTariffResponse>,
                                                       IResponse<Result>
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

        /// <summary>
        /// Create a new RemoveDefaultChargingTariff response.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="EVSEStatusInfos">An optional enumeration of status infos for individual EVSEs.</param>
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
        public RemoveDefaultChargingTariffResponse(RemoveDefaultChargingTariffRequest                               Request,
                                                   RemoveDefaultChargingTariffStatus                                Status,
                                                   StatusInfo?                                                      StatusInfo            = null,
                                                   IEnumerable<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?  EVSEStatusInfos       = null,

                                                   Result?                                                          Result                = null,
                                                   DateTimeOffset?                                                  ResponseTimestamp     = null,

                                                   SourceRouting?                                                   Destination           = null,
                                                   NetworkPath?                                                     NetworkPath           = null,

                                                   IEnumerable<KeyPair>?                                            SignKeys              = null,
                                                   IEnumerable<SignInfo>?                                           SignInfos             = null,
                                                   IEnumerable<Signature>?                                          Signatures            = null,

                                                   CustomData?                                                      CustomData            = null,

                                                   SerializationFormats?                                            SerializationFormat   = null,
                                                   CancellationToken                                                CancellationToken     = default)

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


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomRemoveDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a RemoveDefaultChargingTariff response.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRemoveDefaultChargingTariffResponseParser">A delegate to parse custom RemoveDefaultChargingTariff responses.</param>
        public static RemoveDefaultChargingTariffResponse Parse(RemoveDefaultChargingTariffRequest                                               Request,
                                                                JObject                                                                          JSON,
                                                                SourceRouting                                                                Destination,
                                                                NetworkPath                                                                      NetworkPath,
                                                                DateTimeOffset?                                                                  ResponseTimestamp                                 = null,
                                                                CustomJObjectParserDelegate<RemoveDefaultChargingTariffResponse>?                CustomRemoveDefaultChargingTariffResponseParser   = null,
                                                                CustomJObjectParserDelegate<StatusInfo>?                                         CustomStatusInfoParser                            = null,
                                                                CustomJObjectParserDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?  CustomEVSEStatusInfoParser                        = null,
                                                                CustomJObjectParserDelegate<Signature>?                                          CustomSignatureParser                             = null,
                                                                CustomJObjectParserDelegate<CustomData>?                                         CustomCustomDataParser                            = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var removeDefaultChargingTariffResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomRemoveDefaultChargingTariffResponseParser,
                         CustomStatusInfoParser,
                         CustomEVSEStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return removeDefaultChargingTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a RemoveDefaultChargingTariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out RemoveDefaultChargingTariffResponse, out ErrorResponse, CustomRemoveDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a RemoveDefaultChargingTariff response.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RemoveDefaultChargingTariffResponse">The parsed RemoveDefaultChargingTariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoveDefaultChargingTariffResponseParser">A delegate to parse custom RemoveDefaultChargingTariff responses.</param>
        public static Boolean TryParse(RemoveDefaultChargingTariffRequest                                               Request,
                                       JObject                                                                          JSON,
                                       SourceRouting                                                                Destination,
                                       NetworkPath                                                                      NetworkPath,
                                       [NotNullWhen(true)]  out RemoveDefaultChargingTariffResponse?                    RemoveDefaultChargingTariffResponse,
                                       [NotNullWhen(false)] out String?                                                 ErrorResponse,
                                       DateTimeOffset?                                                                  ResponseTimestamp                                 = null,
                                       CustomJObjectParserDelegate<RemoveDefaultChargingTariffResponse>?                CustomRemoveDefaultChargingTariffResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                                         CustomStatusInfoParser                            = null,
                                       CustomJObjectParserDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?  CustomEVSEStatusInfoParser                        = null,
                                       CustomJObjectParserDelegate<Signature>?                                          CustomSignatureParser                             = null,
                                       CustomJObjectParserDelegate<CustomData>?                                         CustomCustomDataParser                            = null)
        {

            try
            {

                RemoveDefaultChargingTariffResponse = null;

                #region Status             [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "RemoveDefaultChargingTariff status",
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
                                           OCPPv2_1.StatusInfo.TryParse,
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
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData         [optional]

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


                RemoveDefaultChargingTariffResponse = new RemoveDefaultChargingTariffResponse(

                                                          Request,
                                                          Status,
                                                          StatusInfo,
                                                          EVSEStatusInfos,

                                                          null,
                                                          ResponseTimestamp,

                                                          Destination,
                                                          NetworkPath,

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
                ErrorResponse                        = "The given JSON representation of a RemoveDefaultChargingTariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRemoveDefaultChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoveDefaultChargingTariffResponseSerializer">A delegate to serialize custom RemoveDefaultChargingTariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomEVSEStatusInfoSerializer">A delegate to serialize custom EVSE status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffResponse>?                CustomRemoveDefaultChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                                         CustomStatusInfoSerializer                            = null,
                              CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?  CustomEVSEStatusInfoSerializer                        = null,
                              CustomJObjectSerializerDelegate<Signature>?                                          CustomSignatureSerializer                             = null,
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
        /// The RemoveDefaultChargingTariff failed because of a request error.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
        public static RemoveDefaultChargingTariffResponse RequestError(RemoveDefaultChargingTariffRequest  Request,
                                                                       EventTracking_Id                    EventTrackingId,
                                                                       ResultCode                          ErrorCode,
                                                                       String?                             ErrorDescription    = null,
                                                                       JObject?                            ErrorDetails        = null,
                                                                       DateTimeOffset?                     ResponseTimestamp   = null,

                                                                       SourceRouting?                      Destination         = null,
                                                                       NetworkPath?                        NetworkPath         = null,

                                                                       IEnumerable<KeyPair>?               SignKeys            = null,
                                                                       IEnumerable<SignInfo>?              SignInfos           = null,
                                                                       IEnumerable<Signature>?             Signatures          = null,

                                                                       CustomData?                         CustomData          = null)

            => new (

                   Request,
                   RemoveDefaultChargingTariffStatus.Rejected,
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
        /// The RemoveDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RemoveDefaultChargingTariffResponse FormationViolation(RemoveDefaultChargingTariffRequest  Request,
                                                                             String                              ErrorDescription)

            => new (Request,
                    RemoveDefaultChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The RemoveDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RemoveDefaultChargingTariffResponse SignatureError(RemoveDefaultChargingTariffRequest  Request,
                                                                         String                              ErrorDescription)

            => new (Request,
                    RemoveDefaultChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The RemoveDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
        /// <param name="Description">An optional error description.</param>
        public static RemoveDefaultChargingTariffResponse Failed(RemoveDefaultChargingTariffRequest  Request,
                                                                 String?                             Description   = null)

            => new (Request,
                    RemoveDefaultChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The RemoveDefaultChargingTariff failed because of an exception.
        /// </summary>
        /// <param name="Request">The RemoveDefaultChargingTariff request.</param>
        /// <param name="Exception">The exception.</param>
        public static RemoveDefaultChargingTariffResponse ExceptionOccurred(RemoveDefaultChargingTariffRequest  Request,
                                                                           Exception                           Exception)

            => new (Request,
                    RemoveDefaultChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (RemoveDefaultChargingTariffResponse1, RemoveDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two RemoveDefaultChargingTariff responses for equality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffResponse1">A RemoveDefaultChargingTariff response.</param>
        /// <param name="RemoveDefaultChargingTariffResponse2">Another RemoveDefaultChargingTariff response.</param>
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
        /// Compares two RemoveDefaultChargingTariff responses for inequality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffResponse1">A RemoveDefaultChargingTariff response.</param>
        /// <param name="RemoveDefaultChargingTariffResponse2">Another RemoveDefaultChargingTariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoveDefaultChargingTariffResponse? RemoveDefaultChargingTariffResponse1,
                                           RemoveDefaultChargingTariffResponse? RemoveDefaultChargingTariffResponse2)

            => !(RemoveDefaultChargingTariffResponse1 == RemoveDefaultChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoveDefaultChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RemoveDefaultChargingTariff responses for equality.
        /// </summary>
        /// <param name="Object">A RemoveDefaultChargingTariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoveDefaultChargingTariffResponse removeDefaultChargingTariffResponse &&
                   Equals(removeDefaultChargingTariffResponse);

        #endregion

        #region Equals(RemoveDefaultChargingTariffResponse)

        /// <summary>
        /// Compares two RemoveDefaultChargingTariff responses for equality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffResponse">A RemoveDefaultChargingTariff response to compare with.</param>
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
