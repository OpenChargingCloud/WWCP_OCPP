﻿/*
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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A certificate signed response.
    /// </summary>
    [SecurityExtensions]
    public class CertificateSignedResponse : AResponse<CS.CertificateSignedRequest,
                                                          CertificateSignedResponse>,
                                             IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/certificateSignedResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the certificate sign request.
        /// </summary>
        public CertificateSignedStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region CertificateSignedResponse(Request, Status)

        /// <summary>
        /// Create a new certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="Status">The success or failure of the certificate sign request.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public CertificateSignedResponse(CS.CertificateSignedRequest   Request,
                                         CertificateSignedStatus       Status,

                                         DateTime?                     ResponseTimestamp   = null,

                                         IEnumerable<KeyPair>?         SignKeys            = null,
                                         IEnumerable<SignInfo>?        SignInfos           = null,
                                         IEnumerable<Signature>?       Signatures          = null,

                                         CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status = Status;

        }

        #endregion

        #region CertificateSignedResponse(Request, Result)

        /// <summary>
        /// Create a new certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public CertificateSignedResponse(CS.CertificateSignedRequest  Request,
                                         Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:CertificateSigned.conf",
        //   "definitions": {
        //     "CertificateSignedStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected"
        //       ]
        //     }
        // },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //         "$ref": "#/definitions/CertificateSignedStatusEnumType"
        //     }
        // },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomCertificateSignedResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCertificateSignedResponseParser">An optional delegate to parse custom certificate signed responses.</param>
        public static CertificateSignedResponse Parse(CS.CertificateSignedRequest                              Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var certificateSignedResponse,
                         out var errorResponse,
                         CustomCertificateSignedResponseParser) &&
                certificateSignedResponse is not null)
            {
                return certificateSignedResponse;
            }

            throw new ArgumentException("The given JSON representation of a certificate signed response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CertificateSignedResponse, out ErrorResponse, CustomCertificateSignedResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateSignedResponse">The parsed certificate signed response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateSignedResponseParser">An optional delegate to parse custom certificate signed responses.</param>
        public static Boolean TryParse(CS.CertificateSignedRequest                              Request,
                                       JObject                                                  JSON,
                                       out CertificateSignedResponse?                           CertificateSignedResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseParser   = null)
        {

            try
            {

                CertificateSignedResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "certificate signed status",
                                       CertificateSignedStatusExtensions.Parse,
                                       out CertificateSignedStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                CertificateSignedResponse = new CertificateSignedResponse(

                                                Request,
                                                Status,
                                                null,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData

                                            );

                if (CustomCertificateSignedResponseParser is not null)
                    CertificateSignedResponse = CustomCertificateSignedResponseParser(JSON,
                                                                                      CertificateSignedResponse);

                return true;

            }
            catch (Exception e)
            {
                CertificateSignedResponse  = null;
                ErrorResponse              = "The given JSON representation of a certificate signed response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateSignedResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateSignedResponseSerializer">A delegate to serialize custom certificate signed responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?             CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCertificateSignedResponseSerializer is not null
                       ? CustomCertificateSignedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The certificate signed failed.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        public static CertificateSignedResponse Failed(CS.CertificateSignedRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (CertificateSignedResponse1, CertificateSignedResponse2)

        /// <summary>
        /// Compares two certificate signed responses for equality.
        /// </summary>
        /// <param name="CertificateSignedResponse1">A certificate signed response.</param>
        /// <param name="CertificateSignedResponse2">Another certificate signed response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CertificateSignedResponse? CertificateSignedResponse1,
                                           CertificateSignedResponse? CertificateSignedResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CertificateSignedResponse1, CertificateSignedResponse2))
                return true;

            // If one is null, but not both, return false.
            if (CertificateSignedResponse1 is null || CertificateSignedResponse2 is null)
                return false;

            return CertificateSignedResponse1.Equals(CertificateSignedResponse2);

        }

        #endregion

        #region Operator != (CertificateSignedResponse1, CertificateSignedResponse2)

        /// <summary>
        /// Compares two certificate signed responses for inequality.
        /// </summary>
        /// <param name="CertificateSignedResponse1">A certificate signed response.</param>
        /// <param name="CertificateSignedResponse2">Another certificate signed response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CertificateSignedResponse? CertificateSignedResponse1,
                                           CertificateSignedResponse? CertificateSignedResponse2)

            => !(CertificateSignedResponse1 == CertificateSignedResponse2);

        #endregion

        #endregion

        #region IEquatable<CertificateSignedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate signed responses for equality.
        /// </summary>
        /// <param name="Object">A certificate signed response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateSignedResponse certificateSignedResponse &&
                   Equals(certificateSignedResponse);

        #endregion

        #region Equals(CertificateSignedResponse)

        /// <summary>
        /// Compares two certificate signed responses for equality.
        /// </summary>
        /// <param name="CertificateSignedResponse">A certificate signed response to compare with.</param>
        public override Boolean Equals(CertificateSignedResponse? CertificateSignedResponse)

            => CertificateSignedResponse is not null &&
                   Status.Equals(CertificateSignedResponse.Status);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Status.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
