/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The delete certificate request.
    /// </summary>
    public class DeleteCertificateRequest : ARequest<DeleteCertificateRequest>
    {

        #region Properties

        /// <summary>
        /// Indicates the certificate which should be deleted.
        /// </summary>
        public CertificateHashData  CertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a delete certificate request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CertificateHashData">Indicates the certificate which should be deleted.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public DeleteCertificateRequest(ChargeBox_Id         ChargeBoxId,
                                        CertificateHashData  CertificateHashData,

                                        Request_Id?          RequestId           = null,
                                        DateTime?            RequestTimestamp    = null,
                                        TimeSpan?            RequestTimeout      = null,
                                        EventTracking_Id?    EventTrackingId     = null,
                                        CancellationToken?   CancellationToken   = null)

            : base(ChargeBoxId,
                   "DeleteCertificate",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.CertificateHashData = CertificateHashData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:DeleteCertificate.req",
        //   "definitions": {
        //     "HashAlgorithmEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        //     },
        //     "CertificateHashDataType": {
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "hashAlgorithm": {
        //           "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //     "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //     "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //     "type": "string",
        //           "maxLength": 40
        //         }
        //       },
        //       "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "certificateHashData": {
        //         "$ref": "#/definitions/CertificateHashDataType"
        //     }
        // },
        //   "required": [
        //     "certificateHashData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomDeleteCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a delete certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomDeleteCertificateRequestParser">A delegate to parse custom delete certificate requests.</param>
        public static DeleteCertificateRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     ChargeBox_Id                                            ChargeBoxId,
                                                     CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var deleteCertificateRequest,
                         out var errorResponse,
                         CustomDeleteCertificateRequestParser))
            {
                return deleteCertificateRequest!;
            }

            throw new ArgumentException("The given JSON representation of a delete certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out DeleteCertificateRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a delete certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DeleteCertificateRequest">The parsed DeleteCertificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       ChargeBox_Id                   ChargeBoxId,
                                       out DeleteCertificateRequest?  DeleteCertificateRequest,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out DeleteCertificateRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a delete certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DeleteCertificateRequest">The parsed delete certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteCertificateRequestParser">A delegate to parse custom delete certificate requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       ChargeBox_Id                                            ChargeBoxId,
                                       out DeleteCertificateRequest?                           DeleteCertificateRequest,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<DeleteCertificateRequest>?  CustomDeleteCertificateRequestParser)
        {

            try
            {

                DeleteCertificateRequest = null;

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "certificate hash data",
                                             OCPPv1_6.CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (CertificateHashData is null)
                    return false;

                #endregion

                #region ChargeBoxId            [optional, OCPP_CSE]

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


                DeleteCertificateRequest = new DeleteCertificateRequest(ChargeBoxId,
                                                                        CertificateHashData,
                                                                        RequestId);

                if (CustomDeleteCertificateRequestParser is not null)
                    DeleteCertificateRequest = CustomDeleteCertificateRequestParser(JSON,
                                                                                    DeleteCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                DeleteCertificateRequest  = null;
                ErrorResponse             = "The given JSON representation of a delete certificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteCertificateRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public override JObject ToJSON()
            => ToJSON(null);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateRequestSerializer">A delegate to serialize custom delete certificate requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteCertificateRequest>? CustomDeleteCertificateRequestSerializer)
        {

            var json = JSONObject.Create(
                           new JProperty("certificateHashData",  CertificateHashData)
                       );

            return CustomDeleteCertificateRequestSerializer is not null
                       ? CustomDeleteCertificateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DeleteCertificateRequest1, DeleteCertificateRequest2)

        /// <summary>
        /// Compares two delete certificate requests for equality.
        /// </summary>
        /// <param name="DeleteCertificateRequest1">A delete certificate request.</param>
        /// <param name="DeleteCertificateRequest2">Another delete certificate request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteCertificateRequest DeleteCertificateRequest1,
                                           DeleteCertificateRequest DeleteCertificateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteCertificateRequest1, DeleteCertificateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteCertificateRequest1 is null || DeleteCertificateRequest2 is null)
                return false;

            return DeleteCertificateRequest1.Equals(DeleteCertificateRequest2);

        }

        #endregion

        #region Operator != (DeleteCertificateRequest1, DeleteCertificateRequest2)

        /// <summary>
        /// Compares two delete certificate requests for inequality.
        /// </summary>
        /// <param name="DeleteCertificateRequest1">A delete certificate request.</param>
        /// <param name="DeleteCertificateRequest2">Another delete certificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteCertificateRequest DeleteCertificateRequest1,
                                           DeleteCertificateRequest DeleteCertificateRequest2)

            => !(DeleteCertificateRequest1 == DeleteCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<DeleteCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two delete certificate requests for equality.
        /// </summary>
        /// <param name="Object">A delete certificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteCertificateRequest deleteCertificateRequest &&
                   Equals(deleteCertificateRequest);

        #endregion

        #region Equals(DeleteCertificateRequest)

        /// <summary>
        /// Compares two delete certificate requests for equality.
        /// </summary>
        /// <param name="DeleteCertificateRequest">A delete certificate request to compare with.</param>
        public override Boolean Equals(DeleteCertificateRequest? DeleteCertificateRequest)

            => DeleteCertificateRequest is not null &&
                   CertificateHashData.Equals(DeleteCertificateRequest.CertificateHashData);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => CertificateHashData.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CertificateHashData.ToString();

        #endregion

    }

}
