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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The get local list version request.
    /// </summary>
    public class GetLocalListVersionRequest : ARequest<GetLocalListVersionRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/getLocalListVersionRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get local list version request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetLocalListVersionRequest(NetworkingNode_Id        NetworkingNodeId,

                                          IEnumerable<KeyPair>?    SignKeys            = null,
                                          IEnumerable<SignInfo>?   SignInfos           = null,
                                          IEnumerable<Signature>?  Signatures          = null,

                                          CustomData?              CustomData          = null,

                                          Request_Id?              RequestId           = null,
                                          DateTime?                RequestTimestamp    = null,
                                          TimeSpan?                RequestTimeout      = null,
                                          EventTracking_Id?        EventTrackingId     = null,
                                          NetworkPath?             NetworkPath         = null,
                                          CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(GetLocalListVersionRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            unchecked
            {
                hashCode = base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetLocalListVersionRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomGetLocalListVersionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get local list version request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static GetLocalListVersionRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       NetworkingNode_Id                                         NetworkingNodeId,
                                                       NetworkPath                                               NetworkPath,
                                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var getLocalListVersionRequest,
                         out var errorResponse,
                         CustomGetLocalListVersionRequestParser) &&
                getLocalListVersionRequest is not null)
            {
                return getLocalListVersionRequest;
            }

            throw new ArgumentException("The given JSON representation of a get local list version request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out GetLocalListVersionRequest, out ErrorResponse, CustomGetLocalListVersionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get local list version request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       Request_Id                       RequestId,
                                       NetworkingNode_Id                NetworkingNodeId,
                                       NetworkPath                      NetworkPath,
                                       out GetLocalListVersionRequest?  GetLocalListVersionRequest,
                                       out String?                      ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out GetLocalListVersionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get local list version request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       NetworkingNode_Id                                         NetworkingNodeId,
                                       NetworkPath                                               NetworkPath,
                                       out GetLocalListVersionRequest?                           GetLocalListVersionRequest,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestParser)
        {

            try
            {

                GetLocalListVersionRequest = default;

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetLocalListVersionRequest  = new GetLocalListVersionRequest(

                                                  NetworkingNodeId,

                                                  null,
                                                  null,
                                                  Signatures,

                                                  CustomData,

                                                  RequestId,
                                                  null,
                                                  null,
                                                  null,
                                                  NetworkPath

                                              );

                if (CustomGetLocalListVersionRequestParser is not null)
                    GetLocalListVersionRequest = CustomGetLocalListVersionRequestParser(JSON,
                                                                                        GetLocalListVersionRequest);

                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionRequest  = null;
                ErrorResponse               = "The given JSON representation of a get local list version request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetLocalListVersionRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionRequestSerializer">A delegate to serialize custom GetLocalListVersion requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetLocalListVersionRequestSerializer is not null
                       ? CustomGetLocalListVersionRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two GetLocalListVersion requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A GetLocalListVersion request.</param>
        /// <param name="GetLocalListVersionRequest2">Another GetLocalListVersion request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLocalListVersionRequest? GetLocalListVersionRequest1,
                                           GetLocalListVersionRequest? GetLocalListVersionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionRequest1, GetLocalListVersionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetLocalListVersionRequest1 is null || GetLocalListVersionRequest2 is null)
                return false;

            return GetLocalListVersionRequest1.Equals(GetLocalListVersionRequest2);

        }

        #endregion

        #region Operator != (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two GetLocalListVersion requests for inequality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A GetLocalListVersion request.</param>
        /// <param name="GetLocalListVersionRequest2">Another GetLocalListVersion request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLocalListVersionRequest? GetLocalListVersionRequest1,
                                           GetLocalListVersionRequest? GetLocalListVersionRequest2)

            => !(GetLocalListVersionRequest1 == GetLocalListVersionRequest2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get local list version requests for equality.
        /// </summary>
        /// <param name="Object">A get local list version request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLocalListVersionRequest getLocalListVersionRequest &&
                   Equals(getLocalListVersionRequest);

        #endregion

        #region Equals(GetLocalListVersionRequest)

        /// <summary>
        /// Compares two get local list version requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest">A get local list version request to compare with.</param>
        public override Boolean Equals(GetLocalListVersionRequest? GetLocalListVersionRequest)

            => GetLocalListVersionRequest is not null &&

               base.GenericEquals(GetLocalListVersionRequest);

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

            => "GetLocalListVersionRequest";

        #endregion

    }

}
