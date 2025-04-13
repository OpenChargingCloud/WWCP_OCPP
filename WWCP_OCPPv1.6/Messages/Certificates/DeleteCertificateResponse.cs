/*
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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A DeleteCertificate response.
    /// </summary>
    [SecurityExtensions]
    public class DeleteCertificateResponse : AResponse<DeleteCertificateRequest,
                                                       DeleteCertificateResponse>,
                                             IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/deleteCertificateResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the DeleteCertificate request.
        /// </summary>
        [Mandatory]
        public DeleteCertificateStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DeleteCertificate response.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the DeleteCertificate request.</param>
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public DeleteCertificateResponse(DeleteCertificateRequest  Request,
                                         DeleteCertificateStatus   Status,

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
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:DeleteCertificate.conf",
        //   "definitions": {
        //     "DeleteCertificateStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed",
        //         "NotFound"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/DeleteCertificateStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DeleteCertificate response.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomDeleteCertificateResponseParser">An optional delegate to parse custom DeleteCertificate responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static DeleteCertificateResponse Parse(DeleteCertificateRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var deleteCertificateResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomDeleteCertificateResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return deleteCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of a DeleteCertificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out DeleteCertificateResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteCertificate response.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="DeleteCertificateResponse">The parsed DeleteCertificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomDeleteCertificateResponseParser">An optional delegate to parse custom DeleteCertificate responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(DeleteCertificateRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out DeleteCertificateResponse?      DeleteCertificateResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                DeleteCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "DeleteCertificate status",
                                       DeleteCertificateStatusExtensions.Parse,
                                       out DeleteCertificateStatus Status,
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


                DeleteCertificateResponse = new DeleteCertificateResponse(

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

                if (CustomDeleteCertificateResponseParser is not null)
                    DeleteCertificateResponse = CustomDeleteCertificateResponseParser(JSON,
                                                                                      DeleteCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                DeleteCertificateResponse  = null;
                ErrorResponse              = "The given JSON representation of a DeleteCertificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteCertificateResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateResponseSerializer">A delegate to serialize custom DeleteCertificate responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseSerializer   = null,
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

            return CustomDeleteCertificateResponseSerializer is not null
                       ? CustomDeleteCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The DeleteCertificate failed because of a request error.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request.</param>
        public static DeleteCertificateResponse RequestError(DeleteCertificateRequest  Request,
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
                   DeleteCertificateStatus.Failed,
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
        /// The DeleteCertificate failed.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteCertificateResponse FormationViolation(DeleteCertificateRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    DeleteCertificateStatus.Failed,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteCertificate failed.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteCertificateResponse SignatureError(DeleteCertificateRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    DeleteCertificateStatus.Failed,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteCertificate failed.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request.</param>
        /// <param name="Description">An optional error description.</param>
        public static DeleteCertificateResponse Failed(DeleteCertificateRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    DeleteCertificateStatus.Failed,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The DeleteCertificate failed because of an exception.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request.</param>
        /// <param name="Exception">The exception.</param>
        public static DeleteCertificateResponse ExceptionOccurred(DeleteCertificateRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    DeleteCertificateStatus.Failed,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (DeleteCertificateResponse1, DeleteCertificateResponse2)

        /// <summary>
        /// Compares two DeleteCertificate responses for equality.
        /// </summary>
        /// <param name="DeleteCertificateResponse1">A DeleteCertificate response.</param>
        /// <param name="DeleteCertificateResponse2">Another DeleteCertificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteCertificateResponse? DeleteCertificateResponse1,
                                           DeleteCertificateResponse? DeleteCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteCertificateResponse1, DeleteCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteCertificateResponse1 is null || DeleteCertificateResponse2 is null)
                return false;

            return DeleteCertificateResponse1.Equals(DeleteCertificateResponse2);

        }

        #endregion

        #region Operator != (DeleteCertificateResponse1, DeleteCertificateResponse2)

        /// <summary>
        /// Compares two DeleteCertificate responses for inequality.
        /// </summary>
        /// <param name="DeleteCertificateResponse1">A DeleteCertificate response.</param>
        /// <param name="DeleteCertificateResponse2">Another DeleteCertificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteCertificateResponse? DeleteCertificateResponse1,
                                           DeleteCertificateResponse? DeleteCertificateResponse2)

            => !(DeleteCertificateResponse1 == DeleteCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteCertificate responses for equality.
        /// </summary>
        /// <param name="Object">A DeleteCertificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteCertificateResponse deleteCertificateResponse &&
                   Equals(deleteCertificateResponse);

        #endregion

        #region Equals(DeleteCertificateResponse)

        /// <summary>
        /// Compares two DeleteCertificate responses for equality.
        /// </summary>
        /// <param name="DeleteCertificateResponse">A DeleteCertificate response to compare with.</param>
        public override Boolean Equals(DeleteCertificateResponse? DeleteCertificateResponse)

            => DeleteCertificateResponse is not null &&
                   Status.Equals(DeleteCertificateResponse.Status);

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
