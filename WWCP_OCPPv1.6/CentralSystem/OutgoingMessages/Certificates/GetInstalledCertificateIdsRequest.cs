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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The get installed certificate ids request.
    /// </summary>
    [SecurityExtensions]
    public class GetInstalledCertificateIdsRequest : ARequest<GetInstalledCertificateIdsRequest>,
                                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/getInstalledCertificateIdsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The type of the certificates requested.
        /// </summary>
        public CertificateUse  CertificateType    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get installed certificate ids request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="CertificateType">The type of the certificates requested.</param>
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
        public GetInstalledCertificateIdsRequest(NetworkingNode_Id             NetworkingNodeId,
                                                 CertificateUse                CertificateType,

                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                 CustomData?                   CustomData          = null,

                                                 Request_Id?                   RequestId           = null,
                                                 DateTime?                     RequestTimestamp    = null,
                                                 TimeSpan?                     RequestTimeout      = null,
                                                 EventTracking_Id?             EventTrackingId     = null,
                                                 NetworkPath?                  NetworkPath         = null,
                                                 CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(GetInstalledCertificateIdsRequest)[..^7],

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

            this.CertificateType = CertificateType;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:GetInstalledCertificateIds.req",
        //   "definitions": {
        //     "CertificateUseEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "CentralSystemRootCertificate",
        //         "ManufacturerRootCertificate"
        //       ]
        //     }
        // },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "certificateType": {
        //         "$ref": "#/definitions/CertificateUseEnumType"
        //     }
        // },
        //   "required": [
        //     "certificateType"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomGetInstalledCertificateIdsRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get installed certificate ids request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomGetInstalledCertificateIdsRequestParser">An optional delegate to parse custom get installed certificate ids requests.</param>
        public static GetInstalledCertificateIdsRequest Parse(JObject                                                          JSON,
                                                              Request_Id                                                       RequestId,
                                                              NetworkingNode_Id                                                NetworkingNodeId,
                                                              NetworkPath                                                      NetworkPath,
                                                              CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?  CustomGetInstalledCertificateIdsRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var getInstalledCertificateIdsRequest,
                         out var errorResponse,
                         CustomGetInstalledCertificateIdsRequestParser) &&
                getInstalledCertificateIdsRequest is not null)
            {
                return getInstalledCertificateIdsRequest;
            }

            throw new ArgumentException("The given JSON representation of a get installed certificate ids request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out GetInstalledCertificateIdsRequest, out ErrorResponse, CustomGetInstalledCertificateIdsRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get installed certificate ids request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetInstalledCertificateIdsRequest">The parsed get installed certificate ids request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       Request_Id                              RequestId,
                                       NetworkingNode_Id                       NetworkingNodeId,
                                       NetworkPath                             NetworkPath,
                                       out GetInstalledCertificateIdsRequest?  GetInstalledCertificateIdsRequest,
                                       out String?                             ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out GetInstalledCertificateIdsRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get installed certificate ids request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetInstalledCertificateIdsRequest">The parsed get installed certificate ids request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetInstalledCertificateIdsRequestParser">An optional delegate to parse custom get installed certificate ids requests.</param>
        public static Boolean TryParse(JObject                                                          JSON,
                                       Request_Id                                                       RequestId,
                                       NetworkingNode_Id                                                NetworkingNodeId,
                                       NetworkPath                                                      NetworkPath,
                                       out GetInstalledCertificateIdsRequest?                           GetInstalledCertificateIdsRequest,
                                       out String?                                                      ErrorResponse,
                                       CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?  CustomGetInstalledCertificateIdsRequestParser)
        {

            try
            {

                GetInstalledCertificateIdsRequest = null;

                #region CertificateType    [mandatory]

                if (!JSON.MapMandatory("certificateType",
                                       "certificate type",
                                       CertificateUseExtensions.Parse,
                                       out CertificateUse CertificateType,
                                       out ErrorResponse))
                {
                    return false;
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


                GetInstalledCertificateIdsRequest = new GetInstalledCertificateIdsRequest(

                                                        NetworkingNodeId,
                                                        CertificateType,

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

                if (CustomGetInstalledCertificateIdsRequestParser is not null)
                    GetInstalledCertificateIdsRequest = CustomGetInstalledCertificateIdsRequestParser(JSON,
                                                                                                      GetInstalledCertificateIdsRequest);

                return true;

            }
            catch (Exception e)
            {
                GetInstalledCertificateIdsRequest  = null;
                ErrorResponse                      = "The given JSON representation of a get installed certificate ids request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetInstalledCertificateIdsRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetInstalledCertificateIdsRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?  CustomGetInstalledCertificateIdsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                     CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateType",   CertificateType.AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetInstalledCertificateIdsRequestSerializer is not null
                       ? CustomGetInstalledCertificateIdsRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetInstalledCertificateIdsRequest1, GetInstalledCertificateIdsRequest2)

        /// <summary>
        /// Compares two get installed certificate ids requests for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsRequest1">A get installed certificate ids request.</param>
        /// <param name="GetInstalledCertificateIdsRequest2">Another get installed certificate ids request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest1,
                                           GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetInstalledCertificateIdsRequest1, GetInstalledCertificateIdsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetInstalledCertificateIdsRequest1 is null || GetInstalledCertificateIdsRequest2 is null)
                return false;

            return GetInstalledCertificateIdsRequest1.Equals(GetInstalledCertificateIdsRequest2);

        }

        #endregion

        #region Operator != (GetInstalledCertificateIdsRequest1, GetInstalledCertificateIdsRequest2)

        /// <summary>
        /// Compares two get installed certificate ids requests for inequality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsRequest1">A get installed certificate ids request.</param>
        /// <param name="GetInstalledCertificateIdsRequest2">Another get installed certificate ids request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest1,
                                           GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest2)

            => !(GetInstalledCertificateIdsRequest1 == GetInstalledCertificateIdsRequest2);

        #endregion

        #endregion

        #region IEquatable<GetInstalledCertificateIdsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get installed certificate ids requests for equality.
        /// </summary>
        /// <param name="Object">A get installed certificate ids request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetInstalledCertificateIdsRequest getInstalledCertificateIdsRequest &&
                   Equals(getInstalledCertificateIdsRequest);

        #endregion

        #region Equals(GetInstalledCertificateIdsRequest)

        /// <summary>
        /// Compares two get installed certificate ids requests for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsRequest">A get installed certificate ids request to compare with.</param>
        public override Boolean Equals(GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest)

            => GetInstalledCertificateIdsRequest is not null &&

               CertificateType.Equals(GetInstalledCertificateIdsRequest.CertificateType) &&

               base.    GenericEquals(GetInstalledCertificateIdsRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return CertificateType.GetHashCode() * 3 ^
                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CertificateType.AsText();

        #endregion

    }

}
