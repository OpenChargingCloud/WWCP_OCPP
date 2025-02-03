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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The RequestBatterySwap request.
    /// </summary>
    public class RequestBatterySwapRequest : ARequest<RequestBatterySwapRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/aFRRSignalRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The request identification to match with BatterySwapRequest.
        /// </summary>
        [Mandatory]
        public Int32          RequestBatterySwapRequestId    { get; }

        /// <summary>
        /// The identification token of the user that requested the battery swap.
        /// </summary>
        [Mandatory]
        public IdToken        IdToken                        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new automatic frequency restoration reserve (AFRR) signal request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="RequestBatterySwapRequestId">A request identification to match with BatterySwapRequest.</param>
        /// <param name="IdToken">An identification token of the user that requested the battery swap.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public RequestBatterySwapRequest(SourceRouting            Destination,
                                         Int32                    RequestBatterySwapRequestId,
                                         IdToken                  IdToken,

                                         IEnumerable<KeyPair>?    SignKeys              = null,
                                         IEnumerable<SignInfo>?   SignInfos             = null,
                                         IEnumerable<Signature>?  Signatures            = null,

                                         CustomData?              CustomData            = null,

                                         Request_Id?              RequestId             = null,
                                         DateTime?                RequestTimestamp      = null,
                                         TimeSpan?                RequestTimeout        = null,
                                         EventTracking_Id?        EventTrackingId       = null,
                                         NetworkPath?             NetworkPath           = null,
                                         SerializationFormats?    SerializationFormat   = null,
                                         CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(RequestBatterySwapRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.RequestBatterySwapRequestId  = RequestBatterySwapRequestId;
            this.IdToken                      = IdToken;

            unchecked
            {

                hashCode = this.RequestBatterySwapRequestId.GetHashCode() * 5 ^
                           this.IdToken.                    GetHashCode() * 3 ^
                           base.                            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:RequestBatterySwapRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "AdditionalInfoType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "AdditionalInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalIdToken": {
        //                     "description": "*(2.1)* This field specifies the additional IdToken.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "_additionalInfo_ can be used to send extra information to CSMS in addition to the regular authorization with _IdToken_. _AdditionalInfo_ contains one or more custom _types_, which need to be agreed upon by all parties involved. When the _type_ is not supported, the CSMS/Charging Station MAY ignore the _additionalInfo_.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "additionalIdToken",
        //                 "type"
        //             ]
        //         },
        //         "IdTokenType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "IdToken",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalInfo": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/AdditionalInfoType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "idToken": {
        //                     "description": "*(2.1)* IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "*(2.1)* Enumeration of possible idToken types. Values defined in Appendix as IdTokenEnumStringType.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "idToken",
        //                 "type"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "idToken": {
        //             "$ref": "#/definitions/IdTokenType"
        //         },
        //         "requestId": {
        //             "description": "Request id to match with BatterySwapRequest.",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "requestId",
        //         "idToken"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a RequestBatterySwap request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRequestBatterySwapRequestParser">A delegate to parse custom RequestBatterySwap requests.</param>
        public static RequestBatterySwapRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<RequestBatterySwapRequest>?  CustomRequestBatterySwapRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var requestBatterySwapRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomRequestBatterySwapRequestParser))
            {
                return requestBatterySwapRequest;
            }

            throw new ArgumentException("The given JSON representation of a RequestBatterySwap request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out RequestBatterySwapRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a RequestBatterySwap request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestBatterySwapRequest">The parsed RequestBatterySwap request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRequestBatterySwapRequestParser">A delegate to parse custom RequestBatterySwap requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out RequestBatterySwapRequest?      RequestBatterySwapRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<RequestBatterySwapRequest>?  CustomRequestBatterySwapRequestParser   = null)
        {

            try
            {

                RequestBatterySwapRequest = null;

                #region RequestBatterySwapRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "battery swap request identification",
                                         out Int32 RequestBatterySwapRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdToken                        [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures                     [optional, OCPP_CSE]

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

                #region CustomData                     [optional]

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


                RequestBatterySwapRequest = new RequestBatterySwapRequest(

                                                Destination,

                                                RequestBatterySwapRequestId,
                                                IdToken,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData,

                                                RequestId,
                                                RequestTimestamp,
                                                RequestTimeout,
                                                EventTrackingId,
                                                NetworkPath

                                            );

                if (CustomRequestBatterySwapRequestParser is not null)
                    RequestBatterySwapRequest = CustomRequestBatterySwapRequestParser(JSON,
                                                                                      RequestBatterySwapRequest);

                return true;

            }
            catch (Exception e)
            {
                RequestBatterySwapRequest  = null;
                ErrorResponse              = "The given JSON representation of a RequestBatterySwap request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRequestBatterySwapRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRequestBatterySwapRequestSerializer">A delegate to serialize custom RequestBatterySwap requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomBatteryDataSerializer">A delegate to serialize custom battery data.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                      IncludeJSONLDContext                        = false,
                              CustomJObjectSerializerDelegate<RequestBatterySwapRequest>?  CustomRequestBatterySwapRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?                    CustomIdTokenSerializer                     = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?             CustomAdditionalInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<BatteryData>?                CustomBatteryDataSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",    RequestBatterySwapRequestId),

                                 new JProperty("idToken",      IdToken.             ToJSON(CustomIdTokenSerializer,
                                                                                           CustomAdditionalInfoSerializer,
                                                                                           CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRequestBatterySwapRequestSerializer is not null
                       ? CustomRequestBatterySwapRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RequestBatterySwapRequest1, RequestBatterySwapRequest2)

        /// <summary>
        /// Compares two RequestBatterySwap requests for equality.
        /// </summary>
        /// <param name="RequestBatterySwapRequest1">A RequestBatterySwap request.</param>
        /// <param name="RequestBatterySwapRequest2">Another RequestBatterySwap request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RequestBatterySwapRequest? RequestBatterySwapRequest1,
                                           RequestBatterySwapRequest? RequestBatterySwapRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RequestBatterySwapRequest1, RequestBatterySwapRequest2))
                return true;

            // If one is null, but not both, return false.
            if (RequestBatterySwapRequest1 is null || RequestBatterySwapRequest2 is null)
                return false;

            return RequestBatterySwapRequest1.Equals(RequestBatterySwapRequest2);

        }

        #endregion

        #region Operator != (RequestBatterySwapRequest1, RequestBatterySwapRequest2)

        /// <summary>
        /// Compares two RequestBatterySwap requests for inequality.
        /// </summary>
        /// <param name="RequestBatterySwapRequest1">A RequestBatterySwap request.</param>
        /// <param name="RequestBatterySwapRequest2">Another RequestBatterySwap request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RequestBatterySwapRequest? RequestBatterySwapRequest1,
                                           RequestBatterySwapRequest? RequestBatterySwapRequest2)

            => !(RequestBatterySwapRequest1 == RequestBatterySwapRequest2);

        #endregion

        #endregion

        #region IEquatable<RequestBatterySwapRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RequestBatterySwap requests for equality.
        /// </summary>
        /// <param name="Object">A RequestBatterySwap request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestBatterySwapRequest requestBatterySwapRequest &&
                   Equals(requestBatterySwapRequest);

        #endregion

        #region Equals(RequestBatterySwapRequest)

        /// <summary>
        /// Compares two RequestBatterySwap requests for equality.
        /// </summary>
        /// <param name="RequestBatterySwapRequest">A RequestBatterySwap request to compare with.</param>
        public override Boolean Equals(RequestBatterySwapRequest? RequestBatterySwapRequest)

            => RequestBatterySwapRequest is not null &&

               RequestBatterySwapRequestId.Equals(RequestBatterySwapRequest.RequestBatterySwapRequestId) &&
               IdToken.                    Equals(RequestBatterySwapRequest.IdToken)                     &&

               base.                GenericEquals(RequestBatterySwapRequest);

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

            => $"'{IdToken}': {RequestId}";

        #endregion

    }

}
