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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A CertificateSigned response.
    /// </summary>
    [SecurityExtensions]
    public class CertificateSignedResponse : AResponse<CertificateSignedRequest,
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

        /// <summary>
        /// Create a new CertificateSigned response.
        /// </summary>
        /// <param name="Request">The CertificateSigned request leading to this response.</param>
        /// <param name="Status">The success or failure of the certificate sign request.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public CertificateSignedResponse(CertificateSignedRequest  Request,
                                         CertificateSignedStatus   Status,

                                         Result?                   Result                = null,
                                         DateTime?                 ResponseTimestamp     = null,

                                         SourceRouting?            Destination           = null,
                                         NetworkPath?              NetworkPath           = null,

                                         IEnumerable<KeyPair>?     SignKeys              = null,
                                         IEnumerable<SignInfo>?    SignInfos             = null,
                                         IEnumerable<Signature>?   Signatures            = null,

                                         CustomData?               CustomData            = null,

                                         SerializationFormats?     SerializationFormat   = null,
                                         CancellationToken         CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.GetHashCode();

            }

        }

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
        /// Parse the given JSON representation of a CertificateSigned response.
        /// </summary>
        /// <param name="Request">The CertificateSigned request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomCertificateSignedResponseParser">An optional delegate to parse custom CertificateSigned responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static CertificateSignedResponse Parse(CertificateSignedRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseParser   = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var certificateSignedResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomCertificateSignedResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return certificateSignedResponse;
            }

            throw new ArgumentException("The given JSON representation of a CertificateSigned response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CertificateSignedResponse, out ErrorResponse, CustomCertificateSignedResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a CertificateSigned response.
        /// </summary>
        /// <param name="Request">The CertificateSigned request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="CertificateSignedResponse">The parsed CertificateSigned response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomCertificateSignedResponseParser">An optional delegate to parse custom CertificateSigned responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(CertificateSignedRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)] out CertificateSignedResponse?       CertificateSignedResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                CertificateSignedResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "CertificateSigned status",
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


                CertificateSignedResponse = new CertificateSignedResponse(

                                                Request,
                                                Status,

                                                null,
                                                ResponseTimestamp,

                                                Destination,
                                                NetworkPath,

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
                ErrorResponse              = "The given JSON representation of a CertificateSigned response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateSignedResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateSignedResponseSerializer">A delegate to serialize custom CertificateSigned responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
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
        /// The CertificateSigned failed because of a request error.
        /// </summary>
        /// <param name="Request">The CertificateSigned request.</param>
        public static CertificateSignedResponse RequestError(CertificateSignedRequest  Request,
                                                             EventTracking_Id          EventTrackingId,
                                                             ResultCode                ErrorCode,
                                                             String?                   ErrorDescription    = null,
                                                             JObject?                  ErrorDetails        = null,
                                                             DateTime?                 ResponseTimestamp   = null,

                                                             SourceRouting?            Destination         = null,
                                                             NetworkPath?              NetworkPath         = null,

                                                             IEnumerable<KeyPair>?     SignKeys            = null,
                                                             IEnumerable<SignInfo>?    SignInfos           = null,
                                                             IEnumerable<Signature>?   Signatures          = null,

                                                             CustomData?               CustomData          = null)

            => new (

                   Request,
                   CertificateSignedStatus.Rejected,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The CertificateSigned failed.
        /// </summary>
        /// <param name="Request">The CertificateSigned request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CertificateSignedResponse FormationViolation(CertificateSignedRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    CertificateSignedStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The CertificateSigned failed.
        /// </summary>
        /// <param name="Request">The CertificateSigned request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CertificateSignedResponse SignatureError(CertificateSignedRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    CertificateSignedStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The CertificateSigned failed.
        /// </summary>
        /// <param name="Request">The CertificateSigned request.</param>
        /// <param name="Description">An optional error description.</param>
        public static CertificateSignedResponse Failed(CertificateSignedRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    CertificateSignedStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The CertificateSigned failed because of an exception.
        /// </summary>
        /// <param name="Request">The CertificateSigned request.</param>
        /// <param name="Exception">The exception.</param>
        public static CertificateSignedResponse ExceptionOccured(CertificateSignedRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    CertificateSignedStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (CertificateSignedResponse1, CertificateSignedResponse2)

        /// <summary>
        /// Compares two CertificateSigned responses for equality.
        /// </summary>
        /// <param name="CertificateSignedResponse1">A CertificateSigned response.</param>
        /// <param name="CertificateSignedResponse2">Another CertificateSigned response.</param>
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
        /// Compares two CertificateSigned responses for inequality.
        /// </summary>
        /// <param name="CertificateSignedResponse1">A CertificateSigned response.</param>
        /// <param name="CertificateSignedResponse2">Another CertificateSigned response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CertificateSignedResponse? CertificateSignedResponse1,
                                           CertificateSignedResponse? CertificateSignedResponse2)

            => !(CertificateSignedResponse1 == CertificateSignedResponse2);

        #endregion

        #endregion

        #region IEquatable<CertificateSignedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CertificateSigned responses for equality.
        /// </summary>
        /// <param name="Object">A CertificateSigned response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateSignedResponse certificateSignedResponse &&
                   Equals(certificateSignedResponse);

        #endregion

        #region Equals(CertificateSignedResponse)

        /// <summary>
        /// Compares two CertificateSigned responses for equality.
        /// </summary>
        /// <param name="CertificateSignedResponse">A CertificateSigned response to compare with.</param>
        public override Boolean Equals(CertificateSignedResponse? CertificateSignedResponse)

            => CertificateSignedResponse is not null &&
                   Status.Equals(CertificateSignedResponse.Status);

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

            => Status.ToString();

        #endregion

    }

}
