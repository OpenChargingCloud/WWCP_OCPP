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

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// The install certificate request.
    /// </summary>
    public class InstallCertificateRequest : ARequest<InstallCertificateRequest>
    {

        #region Properties

        /// <summary>
        /// The type of the certificate.
        /// </summary>
        [Mandatory]
        public CertificateUse  CertificateType    { get; }

        /// <summary>
        /// The PEM encoded X.509 certificate.
        /// [max 5500]
        /// </summary>
        [Mandatory]
        public Certificate     Certificate        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new install certificate request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateType">The type of the certificate.</param>
        /// <param name="Certificate">The PEM encoded X.509 certificate.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public InstallCertificateRequest(ChargeBox_Id       ChargeBoxId,
                                         CertificateUse     CertificateType,
                                         Certificate        Certificate,
                                         CustomData?        CustomData          = null,

                                         Request_Id?        RequestId           = null,
                                         DateTime?          RequestTimestamp    = null,
                                         TimeSpan?          RequestTimeout      = null,
                                         EventTracking_Id?  EventTrackingId     = null,
                                         CancellationToken  CancellationToken   = default)


            : base(ChargeBoxId,
                   "InstallCertificate",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.CertificateType  = CertificateType;
            this.Certificate      = Certificate;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:InstallCertificateRequest",
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
        //     "InstallCertificateUseEnumType": {
        //       "description": "Indicates the certificate type that is sent.\r\n",
        //       "javaType": "InstallCertificateUseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "V2GRootCertificate",
        //         "MORootCertificate",
        //         "CSMSRootCertificate",
        //         "ManufacturerRootCertificate"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "certificateType": {
        //       "$ref": "#/definitions/InstallCertificateUseEnumType"
        //     },
        //     "certificate": {
        //       "description": "A PEM encoded X.509 certificate.\r\n",
        //       "type": "string",
        //       "maxLength": 5500
        //     }
        //   },
        //   "required": [
        //     "certificateType",
        //     "certificate"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomInstallCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an install certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomInstallCertificateRequestParser">A delegate to parse custom install certificate requests.</param>
        public static InstallCertificateRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      ChargeBox_Id                                             ChargeBoxId,
                                                      CustomJObjectParserDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var installCertificateRequest,
                         out var errorResponse,
                         CustomInstallCertificateRequestParser))
            {
                return installCertificateRequest!;
            }

            throw new ArgumentException("The given JSON representation of an install certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out InstallCertificateRequest, out ErrorResponse, CustomInstallCertificateRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an install certificate request.
        /// </summary>
        /// <param name="InstallCertificateRequestJSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="InstallCertificateRequest">The parsed install certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         InstallCertificateRequestJSON,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out InstallCertificateRequest?  InstallCertificateRequest,
                                       out String?                     ErrorResponse)

            => TryParse(InstallCertificateRequestJSON,
                        RequestId,
                        ChargeBoxId,
                        out InstallCertificateRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an install certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="InstallCertificateRequest">The parsed install certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomInstallCertificateRequestParser">A delegate to parse custom install certificate requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       ChargeBox_Id                                             ChargeBoxId,
                                       out InstallCertificateRequest?                           InstallCertificateRequest,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestParser)
        {

            try
            {

                InstallCertificateRequest = null;

                #region CertificateType      [mandatory]

                if (!JSON.ParseMandatory("certificateType",
                                         "certificate type",
                                         CertificateUseExtensions.TryParse,
                                         out CertificateUse CertificateType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Certificate          [mandatory]

                if (!JSON.ParseMandatoryText("certificate",
                                             "certificate",
                                             out String certificateText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!OCPPv2_0_1.Certificate.TryParse(certificateText,
                                                   out var Certificate,
                                                   out ErrorResponse))
                {
                    return false;
                }

                if (Certificate is null)
                    return false;

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId          [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                InstallCertificateRequest = new InstallCertificateRequest(ChargeBoxId,
                                                                          CertificateType,
                                                                          Certificate,
                                                                          CustomData,
                                                                          RequestId);

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

        #region ToJSON(CustomInstallCertificateRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomInstallCertificateRequestSerializer">A delegate to serialize custom install certificate requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<InstallCertificateRequest>?  CustomInstallCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                           new JProperty("certificateType",   CertificateType.AsText()),
                           new JProperty("certificate",       Certificate.    ToString()),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.     ToJSON(CustomCustomDataSerializer))
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

            => String.Concat(
                   CertificateType.ToString(),
                   ", ",
                   Certificate.ToString().SubstringMax(10)
               );

        #endregion

    }

}
