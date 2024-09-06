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
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The install certificate request.
    /// </summary>
    [SecurityExtensions]
    public class InstallCertificateRequest : ARequest<InstallCertificateRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/installCertificateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The type of the certificate.
        /// </summary>
        public CertificateUse  CertificateType    { get; }

        /// <summary>
        /// The PEM encoded X.509 certificate.
        /// </summary>
        public Certificate     Certificate        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new install certificate request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
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
        public InstallCertificateRequest(NetworkingNode_Id            NetworkingNodeId,
                                         CertificateUse               CertificateType,
                                         Certificate                  Certificate,

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
                   nameof(InstallCertificateRequest)[..^7],

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

            this.CertificateType  = CertificateType;
            this.Certificate      = Certificate;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:InstallCertificate.req",
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
        //     },
        //     "certificate": {
        //         "type": "string",
        //       "maxLength": 5500
        //     }
        // },
        //   "required": [
        //     "certificateType",
        //     "certificate"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomInstallCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an install certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomInstallCertificateRequestParser">An optional delegate to parse custom install certificate requests.</param>
        public static InstallCertificateRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      NetworkingNode_Id                                        NetworkingNodeId,
                                                      NetworkPath                                              NetworkPath,
                                                      CustomJObjectParserDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var installCertificateRequest,
                         out var errorResponse,
                         CustomInstallCertificateRequestParser) &&
                installCertificateRequest is not null)
            {
                return installCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of an install certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out InstallCertificateRequest, out ErrorResponse, CustomInstallCertificateRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an install certificate request.
        /// </summary>
        /// <param name="InstallCertificateRequestJSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="InstallCertificateRequest">The parsed install certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         InstallCertificateRequestJSON,
                                       Request_Id                      RequestId,
                                       NetworkingNode_Id               NetworkingNodeId,
                                       NetworkPath                     NetworkPath,
                                       out InstallCertificateRequest?  InstallCertificateRequest,
                                       out String?                     ErrorResponse)

            => TryParse(InstallCertificateRequestJSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out InstallCertificateRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an install certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="InstallCertificateRequest">The parsed install certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomInstallCertificateRequestParser">An optional delegate to parse custom install certificate requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       NetworkingNode_Id                                        NetworkingNodeId,
                                       NetworkPath                                              NetworkPath,
                                       out InstallCertificateRequest?                           InstallCertificateRequest,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestParser)
        {

            try
            {

                InstallCertificateRequest = null;

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

                #region Certificate        [mandatory]

                if (!JSON.ParseMandatoryText("certificate",
                                             "certificate",
                                             out String certificateText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!OCPPv1_6.Certificate.TryParse(certificateText,
                                                   out var Certificate,
                                                   out ErrorResponse))
                {
                    return false;
                }

                if (Certificate is null)
                    return false;

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


                InstallCertificateRequest = new InstallCertificateRequest(

                                                NetworkingNodeId,
                                                CertificateType,
                                                Certificate,

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

                if (CustomInstallCertificateRequestParser is not null)
                    InstallCertificateRequest = CustomInstallCertificateRequestParser(JSON,
                                                                                      InstallCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                InstallCertificateRequest  = null;
                ErrorResponse              = "The given JSON representation of an install certificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomInstallCertificateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomInstallCertificateRequestSerializer">A delegate to serialize custom install certificate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateType",   CertificateType.AsText()),
                                 new JProperty("certificate",       Certificate.    ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomInstallCertificateRequestSerializer is not null
                       ? CustomInstallCertificateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (InstallCertificateRequest1, InstallCertificateRequest2)

        /// <summary>
        /// Compares two install certificate requests for equality.
        /// </summary>
        /// <param name="InstallCertificateRequest1">An install certificate request.</param>
        /// <param name="InstallCertificateRequest2">Another install certificate request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (InstallCertificateRequest? InstallCertificateRequest1,
                                           InstallCertificateRequest? InstallCertificateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(InstallCertificateRequest1, InstallCertificateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (InstallCertificateRequest1 is null || InstallCertificateRequest2 is null)
                return false;

            return InstallCertificateRequest1.Equals(InstallCertificateRequest2);

        }

        #endregion

        #region Operator != (InstallCertificateRequest1, InstallCertificateRequest2)

        /// <summary>
        /// Compares two install certificate requests for inequality.
        /// </summary>
        /// <param name="InstallCertificateRequest1">An install certificate request.</param>
        /// <param name="InstallCertificateRequest2">Another install certificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (InstallCertificateRequest? InstallCertificateRequest1,
                                           InstallCertificateRequest? InstallCertificateRequest2)

            => !(InstallCertificateRequest1 == InstallCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<InstallCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two install certificate requests for equality.
        /// </summary>
        /// <param name="Object">An install certificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InstallCertificateRequest installCertificateRequest &&
                   Equals(installCertificateRequest);

        #endregion

        #region Equals(InstallCertificateRequest)

        /// <summary>
        /// Compares two install certificate requests for equality.
        /// </summary>
        /// <param name="InstallCertificateRequest">An install certificate request to compare with.</param>
        public override Boolean Equals(InstallCertificateRequest? InstallCertificateRequest)

            => InstallCertificateRequest is not null &&

               CertificateType.Equals(InstallCertificateRequest.CertificateType) &&
               Certificate.    Equals(InstallCertificateRequest.Certificate)     &&

               base.    GenericEquals(InstallCertificateRequest);

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

                return CertificateType.GetHashCode() * 5 ^
                       Certificate.    GetHashCode() * 3 ^

                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{CertificateType}': {Certificate.ToString().SubstringMax(40)}";

        #endregion

    }

}
