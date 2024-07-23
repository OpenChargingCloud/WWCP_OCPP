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
    /// The DeleteCertificate response.
    /// </summary>
    public class DeleteCertificateResponse : AResponse<CSMS.DeleteCertificateRequest,
                                                       DeleteCertificateResponse>,
                                             IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/deleteCertificateResponse");

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
        public DeleteCertificateStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?              StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region DeleteCertificateResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new DeleteCertificate response.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the DeleteCertificate request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DeleteCertificateResponse(CSMS.DeleteCertificateRequest  Request,
                                         DeleteCertificateStatus        Status,
                                         StatusInfo?                    StatusInfo          = null,
                                         DateTime?                      ResponseTimestamp   = null,

                                         IEnumerable<KeyPair>?          SignKeys            = null,
                                         IEnumerable<SignInfo>?         SignInfos           = null,
                                         IEnumerable<Signature>?        Signatures          = null,

                                         CustomData?                    CustomData          = null)

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

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region DeleteCertificateResponse(Request, Result)

        /// <summary>
        /// Create a new DeleteCertificate response.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public DeleteCertificateResponse(CSMS.DeleteCertificateRequest  Request,
                                         Result                         Result,
                                         DateTime?                      ResponseTimestamp   = null,

                                         NetworkingNode_Id?             DestinationId       = null,
                                         NetworkPath?                   NetworkPath         = null,

                                         IEnumerable<KeyPair>?          SignKeys            = null,
                                         IEnumerable<SignInfo>?         SignInfos           = null,
                                         IEnumerable<Signature>?        Signatures          = null,

                                         CustomData?                    CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:DeleteCertificateResponse",
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
        //     "DeleteCertificateStatusEnumType": {
        //       "description": "Charging Station indicates if it can process the request.\r\n",
        //       "javaType": "DeleteCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed",
        //         "NotFound"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.\r\n",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "reasonCode"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/DeleteCertificateStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomDeleteCertificateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a DeleteCertificate response.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteCertificateResponseParser">A delegate to parse custom DeleteCertificate responses.</param>
        public static DeleteCertificateResponse Parse(CSMS.DeleteCertificateRequest                            Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var deleteCertificateResponse,
                         out var errorResponse,
                         CustomDeleteCertificateResponseParser))
            {
                return deleteCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of a DeleteCertificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteCertificateResponse, out ErrorResponse, CustomDeleteCertificateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteCertificate response.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteCertificateResponse">The parsed DeleteCertificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteCertificateResponseParser">A delegate to parse custom DeleteCertificate responses.</param>
        public static Boolean TryParse(CSMS.DeleteCertificateRequest                            Request,
                                       JObject                                                  JSON,
                                       [NotNullWhen(true)]  out DeleteCertificateResponse?      DeleteCertificateResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null)
        {

            try
            {

                DeleteCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "DeleteCertificate status",
                                         DeleteCertificateStatusExtensions.TryParse,
                                         out DeleteCertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                           OCPPv2_1.CustomData.TryParse,
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
                                                StatusInfo,
                                                null,
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

        #region ToJSON(CustomDeleteCertificateResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateResponseSerializer">A delegate to serialize custom DeleteCertificate responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

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
        public static DeleteCertificateResponse RequestError(CSMS.DeleteCertificateRequest  Request,
                                                             EventTracking_Id               EventTrackingId,
                                                             ResultCode                     ErrorCode,
                                                             String?                        ErrorDescription    = null,
                                                             JObject?                       ErrorDetails        = null,
                                                             DateTime?                      ResponseTimestamp   = null,

                                                             NetworkingNode_Id?             DestinationId       = null,
                                                             NetworkPath?                   NetworkPath         = null,

                                                             IEnumerable<KeyPair>?          SignKeys            = null,
                                                             IEnumerable<SignInfo>?         SignInfos           = null,
                                                             IEnumerable<Signature>?        Signatures          = null,

                                                             CustomData?                    CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
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
        /// <param name="ErrorDescription">An optional error decription.</param>
        public static DeleteCertificateResponse SignatureError(CSMS.DeleteCertificateRequest  Request,
                                                               String                         ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The DeleteCertificate failed.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request.</param>
        /// <param name="Description">An optional error decription.</param>
        public static DeleteCertificateResponse Failed(CSMS.DeleteCertificateRequest  Request,
                                                       String?                        Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The DeleteCertificate failed because of an exception.
        /// </summary>
        /// <param name="Request">The DeleteCertificate request.</param>
        /// <param name="Exception">The exception.</param>
        public static DeleteCertificateResponse ExceptionOccured(CSMS.DeleteCertificateRequest  Request,
                                                                 Exception                      Exception)

            => new (Request,
                    Result.FromException(Exception));

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

               Status.     Equals(DeleteCertificateResponse.Status) &&

             ((StatusInfo is     null && DeleteCertificateResponse.StatusInfo is     null) ||
               StatusInfo is not null && DeleteCertificateResponse.StatusInfo is not null && StatusInfo.Equals(DeleteCertificateResponse.StatusInfo)) &&

               base.GenericEquals(DeleteCertificateResponse);

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

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

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
