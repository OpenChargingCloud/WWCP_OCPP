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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The SignCertificate request.
    /// </summary>
    public class SignCertificateRequest : ARequest<SignCertificateRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/signCertificateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The PEM encoded certificate signing request (CSR) [max 5500].
        /// </summary>
        [Mandatory]
        public String         CSR    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SignCertificate request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="CSR">The PEM encoded certificate signing request (CSR) [max 5500].</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SignCertificateRequest(SourceRouting            Destination,
                                      String                   CSR,

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
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.CSR = CSR;

            unchecked
            {

                hashCode = this.CSR.GetHashCode() * 3 ^
                           base.    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignCertificate.req",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "csr": {
        //       "type": "string",
        //       "maxLength": 5500
        //     }
        //   },
        //   "required": [
        //     "csr"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SignCertificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSignCertificateRequestParser">An optional delegate to parse custom SignCertificate requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static SignCertificateRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             RequestTimestamp                     = null,
                                                   TimeSpan?                                             RequestTimeout                       = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<SignCertificateRequest>?  CustomSignCertificateRequestParser   = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var signCertificateRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSignCertificateRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return signCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of a SignCertificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SignCertificateRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SignCertificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SignCertificateRequest">The parsed SignCertificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSignCertificateRequestParser">A delegate to parse custom SignCertificate requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out SignCertificateRequest?      SignCertificateRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             RequestTimestamp                     = null,
                                       TimeSpan?                                             RequestTimeout                       = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<SignCertificateRequest>?  CustomSignCertificateRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                SignCertificateRequest = null;

                #region CSR           [mandatory]

                if (!JSON.ParseMandatoryText("csr",
                                             "certificate signing request",
                                             out String? CSR,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                SignCertificateRequest = new SignCertificateRequest(

                                             Destination,
                                             CSR,

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

                if (CustomSignCertificateRequestParser is not null)
                    SignCertificateRequest = CustomSignCertificateRequestParser(JSON,
                                                                                SignCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                SignCertificateRequest  = null;
                ErrorResponse           = "The given JSON representation of a SignCertificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignCertificateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignCertificateRequestSerializer">A delegate to serialize custom SignCertificate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignCertificateRequest>?  CustomSignCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("csr",          CSR),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
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
        /// Compares two SignCertificate requests for equality.
        /// </summary>
        /// <param name="SignCertificateRequest1">A SignCertificate request.</param>
        /// <param name="SignCertificateRequest2">Another SignCertificate request.</param>
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
        /// Compares two SignCertificate requests for inequality.
        /// </summary>
        /// <param name="SignCertificateRequest1">A SignCertificate request.</param>
        /// <param name="SignCertificateRequest2">Another SignCertificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignCertificateRequest? SignCertificateRequest1,
                                           SignCertificateRequest? SignCertificateRequest2)

            => !(SignCertificateRequest1 == SignCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<SignCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SignCertificate requests for equality.
        /// </summary>
        /// <param name="Object">A SignCertificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignCertificateRequest signCertificateRequest &&
                   Equals(signCertificateRequest);

        #endregion

        #region Equals(SignCertificateRequest)

        /// <summary>
        /// Compares two SignCertificate requests for equality.
        /// </summary>
        /// <param name="SignCertificateRequest">A SignCertificate request to compare with.</param>
        public override Boolean Equals(SignCertificateRequest? SignCertificateRequest)

            => SignCertificateRequest is not null &&

               CSR.        Equals(SignCertificateRequest.CSR) &&

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

            => CSR.SubstringMax(20);

        #endregion

    }

}
