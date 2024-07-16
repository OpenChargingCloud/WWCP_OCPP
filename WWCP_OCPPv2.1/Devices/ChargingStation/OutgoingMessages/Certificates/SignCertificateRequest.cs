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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The sign certificate request.
    /// </summary>
    public class SignCertificateRequest : ARequest<SignCertificateRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/signCertificateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The sign certificate request identification.
        /// </summary>
        [Mandatory]
        public String                  CSR                         { get; }

        /// <summary>
        /// The PEM encoded RFC 2986 certificate signing request (CSR)
        /// [max 5500].
        /// </summary>
        [Mandatory]
        public Int32                   SignCertificateRequestId    { get; }

        /// <summary>
        /// Whether the certificate is to be used for both the 15118 connection (if implemented)
        /// and the charging station to central system (CSMS) connection.
        /// </summary>
        [Optional]
        public CertificateSigningUse?  CertificateType             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new sign certificate request.
        /// </summary>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
        /// <param name="SignCertificateRequestId">A sign certificate request identification.</param>
        /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
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
        public SignCertificateRequest(NetworkingNode_Id             DestinationId,
                                      String                        CSR,
                                      Int32                         SignCertificateRequestId,
                                      CertificateSigningUse?        CertificateType     = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<Signature>?       Signatures          = null,

                                      CustomData?                   CustomData          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      NetworkPath?                  NetworkPath         = null,
                                      CancellationToken             CancellationToken   = default)

            : base(DestinationId,
                   nameof(SignCertificateRequest)[..^7],

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

            this.CSR                       = CSR;
            this.SignCertificateRequestId  = SignCertificateRequestId;
            this.CertificateType           = CertificateType;

            unchecked
            {

                hashCode = this.CSR.                     GetHashCode()       * 7 ^
                           this.SignCertificateRequestId.GetHashCode()       * 5 ^
                          (this.CertificateType?.        GetHashCode() ?? 0) * 3 ^
                           base.                         GetHashCode();

            }

        }

        #endregion


        //ToDo: This request has a breaking change in OCPP v2.1!
        //      Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SignCertificateRequest",
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
        //     },
        //     "CertificateSigningUseEnumType": {
        //       "description": "Indicates the type of certificate that is to be signed. When omitted the certificate is to be used for both the 15118 connection (if implemented) and the Charging Station to CSMS connection.\r\n\r\n",
        //       "javaType": "CertificateSigningUseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ChargingStationCertificate",
        //         "V2GCertificate"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "csr": {
        //       "description": "The Charging Station SHALL send the public key in form of a Certificate Signing Request (CSR) as described in RFC 2986 [22] and then PEM encoded, using the &lt;&lt;signcertificaterequest,SignCertificateRequest&gt;&gt; message.\r\n",
        //       "type": "string",
        //       "maxLength": 5500
        //     },
        //     "certificateType": {
        //       "$ref": "#/definitions/CertificateSigningUseEnumType"
        //     }
        //   },
        //   "required": [
        //     "csr"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomSignCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a sign certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSignCertificateRequestParser">A delegate to parse custom SignCertificate requests.</param>
        public static SignCertificateRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   NetworkingNode_Id                                     DestinationId,
                                                   NetworkPath                                           NetworkPath,
                                                   CustomJObjectParserDelegate<SignCertificateRequest>?  CustomSignCertificateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var signCertificateRequest,
                         out var errorResponse,
                         CustomSignCertificateRequestParser))
            {
                return signCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of a sign certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out SignCertificateRequest, out ErrorResponse, CustomSignCertificateRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a sign certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SignCertificateRequest">The parsed sign certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignCertificateRequestParser">A delegate to parse custom sign certificate requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       NetworkingNode_Id                                     DestinationId,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out SignCertificateRequest?      SignCertificateRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       CustomJObjectParserDelegate<SignCertificateRequest>?  CustomSignCertificateRequestParser)
        {

            try
            {

                SignCertificateRequest = null;

                #region CSR                         [mandatory]

                if (!JSON.ParseMandatoryText("csr",
                                             "certificate signing request",
                                             out String? CSR,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SignCertificateRequestId    [mandatory]  // FixMe!!!

                //ToDo: In OCPP v2.0.1 is does not exist!

                //    if (!JSON.ParseMandatory("requestId",
                //                             "sign certificate request identification",
                //                             out Int32 SignCertificateRequestId,
                //                             out ErrorResponse))
                //    {
                //        return false;
                //    }

                #endregion

                #region CertificateType             [optional]

                if (JSON.ParseOptional("certificateType",
                                       "certificate type",
                                       CertificateSigningUse.TryParse,
                                       out CertificateSigningUse CertificateType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                  [optional, OCPP_CSE]

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

                #region CustomData                  [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SignCertificateRequest = new SignCertificateRequest(

                                             DestinationId,
                                             CSR,
                                             1, //SignCertificateRequestId,
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

                if (CustomSignCertificateRequestParser is not null)
                    SignCertificateRequest = CustomSignCertificateRequestParser(JSON,
                                                                                SignCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                SignCertificateRequest  = null;
                ErrorResponse           = "The given JSON representation of a sign certificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignCertificateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignCertificateRequestSerializer">A delegate to serialize custom sign certificate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignCertificateRequest>?  CustomSignCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("csr",               CSR),
                                 new JProperty("requestId",         SignCertificateRequestId),

                           CertificateType.HasValue
                               ? new JProperty("certificateType",   CertificateType.Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null


                       );

            return CustomSignCertificateRequestSerializer is not null
                       ? CustomSignCertificateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignCertificateRequest1, SignCertificateRequest2)

        /// <summary>
        /// Compares two sign certificate requests for equality.
        /// </summary>
        /// <param name="SignCertificateRequest1">A sign certificate request.</param>
        /// <param name="SignCertificateRequest2">Another sign certificate request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignCertificateRequest? SignCertificateRequest1,
                                           SignCertificateRequest? SignCertificateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignCertificateRequest1, SignCertificateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SignCertificateRequest1 is null || SignCertificateRequest2 is null)
                return false;

            return SignCertificateRequest1.Equals(SignCertificateRequest2);

        }

        #endregion

        #region Operator != (SignCertificateRequest1, SignCertificateRequest2)

        /// <summary>
        /// Compares two sign certificate requests for inequality.
        /// </summary>
        /// <param name="SignCertificateRequest1">A sign certificate request.</param>
        /// <param name="SignCertificateRequest2">Another sign certificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignCertificateRequest? SignCertificateRequest1,
                                           SignCertificateRequest? SignCertificateRequest2)

            => !(SignCertificateRequest1 == SignCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<SignCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two sign certificate requests for equality.
        /// </summary>
        /// <param name="Object">A sign certificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignCertificateRequest signCertificateRequest &&
                   Equals(signCertificateRequest);

        #endregion

        #region Equals(SignCertificateRequest)

        /// <summary>
        /// Compares two sign certificate requests for equality.
        /// </summary>
        /// <param name="SignCertificateRequest">A sign certificate request to compare with.</param>
        public override Boolean Equals(SignCertificateRequest? SignCertificateRequest)

            => SignCertificateRequest is not null &&

               CSR.                     Equals(SignCertificateRequest.CSR)                      &&
               SignCertificateRequestId.Equals(SignCertificateRequest.SignCertificateRequestId) &&

               base.GenericEquals(SignCertificateRequest);

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

            => String.Concat(

                   $"{CSR.SubstringMax(20)},",

                   CertificateType.HasValue
                       ? $" ({CertificateType})"
                       : ""

               );

        #endregion

    }

}
