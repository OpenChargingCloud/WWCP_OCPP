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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The certificate signed request.
    /// </summary>
    [SecurityExtensions]
    public class CertificateSignedRequest : ARequest<CertificateSignedRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/certificateSignedRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The signed PEM encoded X.509 certificates.
        /// This can also contain the necessary sub CA certificates.
        /// </summary>
        public CertificateChain  CertificateChain    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a certificate signed request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="CertificateChain">The signed PEM encoded X.509 certificates. This can also contain the necessary sub CA certificates.</param>
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
        public CertificateSignedRequest(NetworkingNode_Id             NetworkingNodeId,
                                        CertificateChain              CertificateChain,

                                        IEnumerable<WWCP.KeyPair>?    SignKeys            = null,
                                        IEnumerable<WWCP.SignInfo>?   SignInfos           = null,
                                        IEnumerable<Signature>?  Signatures          = null,

                                        CustomData?                   CustomData          = null,

                                        Request_Id?                   RequestId           = null,
                                        DateTime?                     RequestTimestamp    = null,
                                        TimeSpan?                     RequestTimeout      = null,
                                        EventTracking_Id?             EventTrackingId     = null,
                                        NetworkPath?                  NetworkPath         = null,
                                        CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(CertificateSignedRequest)[..^7],

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

            this.CertificateChain = CertificateChain;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:CertificateSigned.req",
        //     "type": "object",
        //     "properties": {
        //         "certificateChain": {
        //             "type": "string",
        //             "maxLength": 10000
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "certificateChain"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomCertificateSignedRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a certificate signed request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomCertificateSignedRequestParser">An optional delegate to parse custom certificate signed requests.</param>
        public static CertificateSignedRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     NetworkingNode_Id                                       NetworkingNodeId,
                                                     NetworkPath                                             NetworkPath,
                                                     CustomJObjectParserDelegate<CertificateSignedRequest>?  CustomCertificateSignedRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var certificateSignedRequest,
                         out var errorResponse,
                         CustomCertificateSignedRequestParser) &&
                certificateSignedRequest is not null)
            {
                return certificateSignedRequest;
            }

            throw new ArgumentException("The given JSON representation of a certificate signed request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out CertificateSignedRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a certificate signed request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CertificateSignedRequest">The parsed CertificateSigned request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       NetworkingNode_Id              NetworkingNodeId,
                                       NetworkPath                    NetworkPath,
                                       out CertificateSignedRequest?  CertificateSignedRequest,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out CertificateSignedRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a certificate signed request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CertificateSignedRequest">The parsed certificate signed request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateSignedRequestParser">An optional delegate to parse custom certificate signed requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       NetworkingNode_Id                                       NetworkingNodeId,
                                       NetworkPath                                             NetworkPath,
                                       out CertificateSignedRequest?                           CertificateSignedRequest,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateSignedRequest>?  CustomCertificateSignedRequestParser)
        {

            try
            {

                CertificateSignedRequest = null;

                #region CertificateChain    [mandatory]

                if (!JSON.ParseMandatoryText("certificateChain",
                                             "certificate chain",
                                             out String certificateChainText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!OCPPv1_6.CertificateChain.TryParse(certificateChainText,
                                                        out var CertificateChain,
                                                        out ErrorResponse))
                {
                    return false;
                }

                if (CertificateChain is null)
                    return false;

                #endregion

                #region Signatures          [optional, OCPP_CSE]

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

                #region CustomData          [optional]

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


                CertificateSignedRequest = new CertificateSignedRequest(

                                               NetworkingNodeId,
                                               CertificateChain,

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

                if (CustomCertificateSignedRequestParser is not null)
                    CertificateSignedRequest = CustomCertificateSignedRequestParser(JSON,
                                                                                    CertificateSignedRequest);

                return true;

            }
            catch (Exception e)
            {
                CertificateSignedRequest  = null;
                ErrorResponse             = "The given JSON representation of a certificate signed request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateSignedRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateSignedRequestSerializer">A delegate to serialize custom certificate signed requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateSignedRequest>?  CustomCertificateSignedRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateChain",   CertificateChain.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCertificateSignedRequestSerializer is not null
                       ? CustomCertificateSignedRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CertificateSignedRequest1, CertificateSignedRequest2)

        /// <summary>
        /// Compares two certificate signed requests for equality.
        /// </summary>
        /// <param name="CertificateSignedRequest1">A certificate signed request.</param>
        /// <param name="CertificateSignedRequest2">Another certificate signed request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CertificateSignedRequest? CertificateSignedRequest1,
                                           CertificateSignedRequest? CertificateSignedRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CertificateSignedRequest1, CertificateSignedRequest2))
                return true;

            // If one is null, but not both, return false.
            if (CertificateSignedRequest1 is null || CertificateSignedRequest2 is null)
                return false;

            return CertificateSignedRequest1.Equals(CertificateSignedRequest2);

        }

        #endregion

        #region Operator != (CertificateSignedRequest1, CertificateSignedRequest2)

        /// <summary>
        /// Compares two certificate signed requests for inequality.
        /// </summary>
        /// <param name="CertificateSignedRequest1">A certificate signed request.</param>
        /// <param name="CertificateSignedRequest2">Another certificate signed request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CertificateSignedRequest? CertificateSignedRequest1,
                                           CertificateSignedRequest? CertificateSignedRequest2)

            => !(CertificateSignedRequest1 == CertificateSignedRequest2);

        #endregion

        #endregion

        #region IEquatable<CertificateSignedRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate signed requests for equality.
        /// </summary>
        /// <param name="Object">A certificate signed request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateSignedRequest certificateSignedRequest &&
                   Equals(certificateSignedRequest);

        #endregion

        #region Equals(CertificateSignedRequest)

        /// <summary>
        /// Compares two certificate signed requests for equality.
        /// </summary>
        /// <param name="CertificateSignedRequest">A certificate signed request to compare with.</param>
        public override Boolean Equals(CertificateSignedRequest? CertificateSignedRequest)

            => CertificateSignedRequest is not null &&

               CertificateChain.Equals(CertificateSignedRequest.CertificateChain) &&

               base.     GenericEquals(CertificateSignedRequest);

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

                return CertificateChain.GetHashCode() * 3 ^
                       base.            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CertificateChain.ToString();

        #endregion

    }

}
