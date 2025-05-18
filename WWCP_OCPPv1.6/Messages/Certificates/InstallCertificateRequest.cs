﻿/*
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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The InstallCertificate request.
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
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The type of the certificate.
        /// </summary>
        [Mandatory]
        public CertificateUse     CertificateType     { get; }

        /// <summary>
        /// The optional X.509 certificate group.
        /// </summary>
        [Optional, VendorExtension("OCPP_CSE")]
        public CertificateGroup?  CertificateGroup    { get; }

        /// <summary>
        /// The PEM encoded X.509 certificate.
        /// </summary>
        [Mandatory]
        public Certificate        Certificate         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new InstallCertificate request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
        /// <param name="CertificateGroup">An optional X.509 certificate group.</param>
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
        public InstallCertificateRequest(SourceRouting            Destination,
                                         CertificateUse           CertificateType,
                                         Certificate              Certificate,
                                         CertificateGroup?        CertificateGroup      = null,

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
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.CertificateType   = CertificateType;
            this.CertificateGroup  = CertificateGroup;
            this.Certificate       = Certificate;

            unchecked
            {

                hashCode = this.CertificateType.  GetHashCode()       * 7 ^
                          (this.CertificateGroup?.GetHashCode() ?? 0) * 5 ^
                           this.Certificate.      GetHashCode()       * 3 ^
                           base.                  GetHashCode();

            }

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


        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:InstallCertificate.req",
        //   "definitions": {
        //     "CertificateUseEnumType": {
        //       "description": "Indicates the certificate type that is sent.",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "CentralSystemRootCertificate",
        //         "ManufacturerRootCertificate",
        //         "NetworkTimeRootCertificate"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "certificateType": {
        //       "$ref": "#/definitions/CertificateUseEnumType"
        //     },
        //     "certificateGroup": {
        //       "description": "A X.509 certificate group.",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "certificate": {
        //       "description": "A PEM encoded X.509 certificate.",
        //       "type": "string",
        //       "maxLength": 11000
        //     }
        //   },
        //   "required": [
        //     "certificateType",
        //     "certificate"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an InstallCertificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomInstallCertificateRequestParser">An optional delegate to parse custom InstallCertificate requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static InstallCertificateRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestParser   = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var installCertificateRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomInstallCertificateRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return installCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of an InstallCertificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out InstallCertificateRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an InstallCertificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="InstallCertificateRequest">The parsed InstallCertificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomInstallCertificateRequestParser">An optional delegate to parse custom InstallCertificate requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out InstallCertificateRequest?      InstallCertificateRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                InstallCertificateRequest = null;

                #region CertificateType     [mandatory]

                if (!JSON.ParseMandatory("certificateType",
                                         "certificate type",
                                         CertificateUse.TryParse,
                                         out CertificateUse certificateType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Certificate         [mandatory]

                if (!JSON.ParseMandatory("certificate",
                                         "certificate",
                                         Certificate.TryParse,
                                         out Certificate? certificate,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateGroup    [optional, OCPP_CSE]

                if (JSON.ParseOptional("certificateGroup",
                                       "certificate group",
                                       OCPPv1_6.CertificateGroup.TryParse,
                                       out CertificateGroup? certificateGroup,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

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


                InstallCertificateRequest = new InstallCertificateRequest(

                                                Destination,
                                                certificateType,
                                                certificate,
                                                certificateGroup,

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

                if (CustomInstallCertificateRequestParser is not null)
                    InstallCertificateRequest = CustomInstallCertificateRequestParser(JSON,
                                                                                      InstallCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                InstallCertificateRequest  = null;
                ErrorResponse              = "The given JSON representation of an InstallCertificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomInstallCertificateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomInstallCertificateRequestSerializer">A delegate to serialize custom InstallCertificate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                      IncludeJSONLDContext                        = false,
                              CustomJObjectSerializerDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",           DefaultJSONLDContext.  ToString())
                               : null,

                                 new JProperty("certificateType",    CertificateType.       ToString()),

                           CertificateGroup.HasValue
                               ? new JProperty("certificateGroup",   CertificateGroup.Value.ToString())
                               : null,

                                 new JProperty("certificate",        Certificate.           ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.            ToJSON(CustomCustomDataSerializer))
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
        /// Compares two InstallCertificate requests for equality.
        /// </summary>
        /// <param name="InstallCertificateRequest1">An InstallCertificate request.</param>
        /// <param name="InstallCertificateRequest2">Another InstallCertificate request.</param>
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
        /// Compares two InstallCertificate requests for inequality.
        /// </summary>
        /// <param name="InstallCertificateRequest1">An InstallCertificate request.</param>
        /// <param name="InstallCertificateRequest2">Another InstallCertificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (InstallCertificateRequest? InstallCertificateRequest1,
                                           InstallCertificateRequest? InstallCertificateRequest2)

            => !(InstallCertificateRequest1 == InstallCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<InstallCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two InstallCertificate requests for equality.
        /// </summary>
        /// <param name="Object">An InstallCertificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InstallCertificateRequest installCertificateRequest &&
                   Equals(installCertificateRequest);

        #endregion

        #region Equals(InstallCertificateRequest)

        /// <summary>
        /// Compares two InstallCertificate requests for equality.
        /// </summary>
        /// <param name="InstallCertificateRequest">An InstallCertificate request to compare with.</param>
        public override Boolean Equals(InstallCertificateRequest? InstallCertificateRequest)

            => InstallCertificateRequest is not null &&

               CertificateType.Equals(InstallCertificateRequest.CertificateType) &&
               Certificate.    Equals(InstallCertificateRequest.Certificate)     &&

            ((!CertificateGroup.HasValue && !InstallCertificateRequest.CertificateGroup.HasValue) ||
               CertificateGroup.HasValue &&  InstallCertificateRequest.CertificateGroup.HasValue && CertificateGroup.Value.Equals(InstallCertificateRequest.CertificateGroup.Value)) &&

               base.    GenericEquals(InstallCertificateRequest);

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

                   CertificateGroup.HasValue
                       ? $"'{CertificateGroup}' of "
                       : "",

                   $"'{CertificateType}': {Certificate.ToString().SubstringMax(40)}"

               );

        #endregion

    }

}
