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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A sign certificate response.
    /// </summary>
    [SecurityExtensions]
    public class SignCertificateResponse : AResponse<CP.SignCertificateRequest,
                                                        SignCertificateResponse>,
                                           IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/signCertificateResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure status of the sign certificate request.
        /// </summary>
        public GenericStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region SignCertificateResponse(Request, Status)

        /// <summary>
        /// Create a sign certificate response.
        /// </summary>
        /// <param name="Request">The sign certificate request leading to this response.</param>
        /// <param name="Status">The success or failure status of the certificate signing request.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignCertificateResponse(CP.SignCertificateRequest     Request,
                                       GenericStatus                 Status,

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

        #region SignCertificateResponse(Request, Result)

        /// <summary>
        /// Create a sign certificate response.
        /// </summary>
        /// <param name="Request">The sign certificate request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SignCertificateResponse(CP.SignCertificateRequest  Request,
                                       Result                     Result)

            : base(Request,
                   Result)

        {

            this.Status = GenericStatus.Unknown;

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignCertificate.conf",
        //   "definitions": {
        //     "GenericStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/GenericStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomSignCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a sign certificate response.
        /// </summary>
        /// <param name="Request">The sign certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignCertificateResponseParser">An optional delegate to parse custom sign certificate responses.</param>
        public static SignCertificateResponse Parse(CP.SignCertificateRequest                              Request,
                                                    JObject                                                JSON,
                                                    CustomJObjectParserDelegate<SignCertificateResponse>?  CustomSignCertificateResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var signCertificateResponse,
                         out var errorResponse,
                         CustomSignCertificateResponseParser) &&
                signCertificateResponse is not null)
            {
                return signCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of a sign certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SignCertificateResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a sign certificate response.
        /// </summary>
        /// <param name="Request">The sign certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignCertificateResponse">The parsed sign certificate response.</param>
        /// <param name="CustomSignCertificateResponseParser">An optional delegate to parse custom sign certificate responses.</param>
        public static Boolean TryParse(CP.SignCertificateRequest                              Request,
                                       JObject                                                JSON,
                                       out SignCertificateResponse?                           SignCertificateResponse,
                                       out String?                                            ErrorResponse,
                                       CustomJObjectParserDelegate<SignCertificateResponse>?  CustomSignCertificateResponseParser   = null)
        {

            try
            {

                SignCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "identification tag information",
                                       GenericStatusExtensions.Parse,
                                       out GenericStatus Status,
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


                SignCertificateResponse = new SignCertificateResponse(

                                              Request,
                                              Status,
                                              null,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData

                                          );

                if (CustomSignCertificateResponseParser is not null)
                    SignCertificateResponse = CustomSignCertificateResponseParser(JSON,
                                                                                  SignCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                SignCertificateResponse  = null;
                ErrorResponse            = "The given JSON representation of a sign certificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignCertificateResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignCertificateResponseSerializer">A delegate to serialize custom sign certificate responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignCertificateResponse>?  CustomSignCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?           CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
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

            return CustomSignCertificateResponseSerializer is not null
                       ? CustomSignCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The sign certificate request failed.
        /// </summary>
        /// <param name="Request">The sign certificate request leading to this response.</param>
        public static SignCertificateResponse Failed(CP.SignCertificateRequest Request)

            => new (Request,
                    GenericStatus.Unknown);

        #endregion


        #region Operator overloading

        #region Operator == (SignCertificateResponse1, SignCertificateResponse2)

        /// <summary>
        /// Compares two sign certificate responses for equality.
        /// </summary>
        /// <param name="SignCertificateResponse1">A sign certificate response.</param>
        /// <param name="SignCertificateResponse2">Another sign certificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignCertificateResponse? SignCertificateResponse1,
                                           SignCertificateResponse? SignCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignCertificateResponse1, SignCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SignCertificateResponse1 is null || SignCertificateResponse2 is null)
                return false;

            return SignCertificateResponse1.Equals(SignCertificateResponse2);

        }

        #endregion

        #region Operator != (SignCertificateResponse1, SignCertificateResponse2)

        /// <summary>
        /// Compares two sign certificate responses for inequality.
        /// </summary>
        /// <param name="SignCertificateResponse1">A sign certificate response.</param>
        /// <param name="SignCertificateResponse2">Another sign certificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignCertificateResponse? SignCertificateResponse1,
                                           SignCertificateResponse? SignCertificateResponse2)

            => !(SignCertificateResponse1 == SignCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<SignCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two sign certificate responses for equality.
        /// </summary>
        /// <param name="Object">A sign certificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignCertificateResponse signCertificateResponse &&
                   Equals(signCertificateResponse);

        #endregion

        #region Equals(SignCertificateResponse)

        /// <summary>
        /// Compares two sign certificate responses for equality.
        /// </summary>
        /// <param name="SignCertificateResponse">A sign certificate response to compare with.</param>
        public override Boolean Equals(SignCertificateResponse? SignCertificateResponse)

            => SignCertificateResponse is not null &&
                   Status.Equals(SignCertificateResponse.Status);

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

            => Status.AsText();

        #endregion

    }

}
