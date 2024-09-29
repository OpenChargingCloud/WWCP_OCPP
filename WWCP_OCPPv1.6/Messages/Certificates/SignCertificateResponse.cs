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
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A SignCertificate response.
    /// </summary>
    [SecurityExtensions]
    public class SignCertificateResponse : AResponse<SignCertificateRequest,
                                                     SignCertificateResponse>,
                                           IResponse<Result>
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
        /// The success or failure status of the SignCertificate request.
        /// </summary>
        public GenericStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a SignCertificate response.
        /// </summary>
        /// <param name="Request">The SignCertificate request leading to this response.</param>
        /// <param name="Status">The success or failure status of the SignCertificate request.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SignCertificateResponse(SignCertificateRequest   Request,
                                       GenericStatus            Status,

                                       Result?                  Result                = null,
                                       DateTime?                ResponseTimestamp     = null,

                                       SourceRouting?           Destination           = null,
                                       NetworkPath?             NetworkPath           = null,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

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

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SignCertificate response.
        /// </summary>
        /// <param name="Request">The SignCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSignCertificateResponseParser">An optional delegate to parse custom SignCertificate responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static SignCertificateResponse Parse(SignCertificateRequest                                 Request,
                                                    JObject                                                JSON,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              ResponseTimestamp                     = null,
                                                    CustomJObjectParserDelegate<SignCertificateResponse>?  CustomSignCertificateResponseParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var signCertificateResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSignCertificateResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return signCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of a SignCertificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SignCertificateResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SignCertificate response.
        /// </summary>
        /// <param name="Request">The SignCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="SignCertificateResponse">The parsed SignCertificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSignCertificateResponseParser">An optional delegate to parse custom SignCertificate responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(SignCertificateRequest                                 Request,
                                       JObject                                                JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out SignCertificateResponse?      SignCertificateResponse,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              ResponseTimestamp                     = null,
                                       CustomJObjectParserDelegate<SignCertificateResponse>?  CustomSignCertificateResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                SignCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "generic status",
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


                SignCertificateResponse = new SignCertificateResponse(

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

                if (CustomSignCertificateResponseParser is not null)
                    SignCertificateResponse = CustomSignCertificateResponseParser(JSON,
                                                                                  SignCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                SignCertificateResponse  = null;
                ErrorResponse            = "The given JSON representation of a SignCertificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignCertificateResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignCertificateResponseSerializer">A delegate to serialize custom SignCertificate responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignCertificateResponse>?  CustomSignCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
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
        /// The SignCertificate failed because of a request error.
        /// </summary>
        /// <param name="Request">The SignCertificate request.</param>
        public static SignCertificateResponse RequestError(SignCertificateRequest   Request,
                                                           EventTracking_Id         EventTrackingId,
                                                           ResultCode               ErrorCode,
                                                           String?                  ErrorDescription    = null,
                                                           JObject?                 ErrorDetails        = null,
                                                           DateTime?                ResponseTimestamp   = null,

                                                           SourceRouting?           Destination         = null,
                                                           NetworkPath?             NetworkPath         = null,

                                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                                           IEnumerable<Signature>?  Signatures          = null,

                                                           CustomData?              CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
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
        /// The SignCertificate failed.
        /// </summary>
        /// <param name="Request">The SignCertificate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SignCertificateResponse FormationViolation(SignCertificateRequest  Request,
                                                                 String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SignCertificate failed.
        /// </summary>
        /// <param name="Request">The SignCertificate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SignCertificateResponse SignatureError(SignCertificateRequest  Request,
                                                             String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SignCertificate failed.
        /// </summary>
        /// <param name="Request">The SignCertificate request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SignCertificateResponse Failed(SignCertificateRequest  Request,
                                                     String?                 Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The SignCertificate failed because of an exception.
        /// </summary>
        /// <param name="Request">The SignCertificate request.</param>
        /// <param name="Exception">The exception.</param>
        public static SignCertificateResponse ExceptionOccured(SignCertificateRequest  Request,
                                                               Exception               Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SignCertificateResponse1, SignCertificateResponse2)

        /// <summary>
        /// Compares two SignCertificate responses for equality.
        /// </summary>
        /// <param name="SignCertificateResponse1">A SignCertificate response.</param>
        /// <param name="SignCertificateResponse2">Another SignCertificate response.</param>
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
        /// Compares two SignCertificate responses for inequality.
        /// </summary>
        /// <param name="SignCertificateResponse1">A SignCertificate response.</param>
        /// <param name="SignCertificateResponse2">Another SignCertificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignCertificateResponse? SignCertificateResponse1,
                                           SignCertificateResponse? SignCertificateResponse2)

            => !(SignCertificateResponse1 == SignCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<SignCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SignCertificate responses for equality.
        /// </summary>
        /// <param name="Object">A SignCertificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignCertificateResponse signCertificateResponse &&
                   Equals(signCertificateResponse);

        #endregion

        #region Equals(SignCertificateResponse)

        /// <summary>
        /// Compares two SignCertificate responses for equality.
        /// </summary>
        /// <param name="SignCertificateResponse">A SignCertificate response to compare with.</param>
        public override Boolean Equals(SignCertificateResponse? SignCertificateResponse)

            => SignCertificateResponse is not null &&
                   Status.Equals(SignCertificateResponse.Status);

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

            => Status.AsText();

        #endregion

    }

}
