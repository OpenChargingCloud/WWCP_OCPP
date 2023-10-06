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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A request stop transaction response.
    /// </summary>
    public class RequestStopTransactionResponse : AResponse<CSMS.RequestStopTransactionRequest,
                                                            RequestStopTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// The status indicating whether the charging station accepts the request to stop the charging transaction.
        /// </summary>
        [Mandatory]
        public RequestStartStopStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?             StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region RequestStopTransactionResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new request stop transaction response.
        /// </summary>
        /// <param name="Request">The request stop transaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charging station accepts the request to stop the charging transaction.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public RequestStopTransactionResponse(CSMS.RequestStopTransactionRequest  Request,
                                              RequestStartStopStatus              Status,
                                              StatusInfo?                         StatusInfo        = null,

                                              IEnumerable<KeyPair>?               SignKeys          = null,
                                              IEnumerable<SignInfo>?              SignInfos         = null,
                                              SignaturePolicy?                    SignaturePolicy   = null,
                                              IEnumerable<Signature>?             Signatures        = null,

                                              DateTime?                           Timestamp         = null,
                                              CustomData?                         CustomData        = null)

            : base(Request,
                   Result.OK(),
                   SignKeys,
                   SignInfos,
                   SignaturePolicy,
                   Signatures,
                   Timestamp,
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region RequestStopTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new request stop transaction response.
        /// </summary>
        /// <param name="Request">The request stop transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public RequestStopTransactionResponse(CSMS.RequestStopTransactionRequest  Request,
                                              Result                              Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:RequestStopTransactionResponse",
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
        //     "RequestStartStopStatusEnumType": {
        //       "description": "Status indicating whether Charging Station accepts the request to stop a transaction.\r\n",
        //       "javaType": "RequestStartStopStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected"
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
        //       "$ref": "#/definitions/RequestStartStopStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomRequestStopTransactionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a request stop transaction response.
        /// </summary>
        /// <param name="Request">The request stop transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRequestStopTransactionResponseParser">A delegate to parse custom request stop transaction responses.</param>
        public static RequestStopTransactionResponse Parse(CSMS.RequestStopTransactionRequest                            Request,
                                                           JObject                                                       JSON,
                                                           CustomJObjectParserDelegate<RequestStopTransactionResponse>?  CustomRequestStopTransactionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var requestStopTransactionResponse,
                         out var errorResponse,
                         CustomRequestStopTransactionResponseParser))
            {
                return requestStopTransactionResponse!;
            }

            throw new ArgumentException("The given JSON representation of a request stop transaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out RequestStopTransactionResponse, out ErrorResponse, CustomRequestStopTransactionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a request stop transaction response.
        /// </summary>
        /// <param name="Request">The request stop transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestStopTransactionResponse">The parsed request stop transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRequestStopTransactionResponseParser">A delegate to parse custom request stop transaction responses.</param>
        public static Boolean TryParse(CSMS.RequestStopTransactionRequest                            Request,
                                       JObject                                                       JSON,
                                       out RequestStopTransactionResponse?                           RequestStopTransactionResponse,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<RequestStopTransactionResponse>?  CustomRequestStopTransactionResponseParser   = null)
        {

            try
            {

                RequestStopTransactionResponse = null;

                #region RequestStartStopStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "request start stop status",
                                         RequestStartStopStatusExtensions.TryParse,
                                         out RequestStartStopStatus RequestStartStopStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo                [optional]

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

                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                RequestStopTransactionResponse = new RequestStopTransactionResponse(
                                                     Request,
                                                     RequestStartStopStatus,
                                                     StatusInfo,
                                                     null,
                                                     null,
                                                     null,
                                                     Signatures,
                                                     null,
                                                     CustomData
                                                 );

                if (CustomRequestStopTransactionResponseParser is not null)
                    RequestStopTransactionResponse = CustomRequestStopTransactionResponseParser(JSON,
                                                                                                RequestStopTransactionResponse);

                return true;

            }
            catch (Exception e)
            {
                RequestStopTransactionResponse  = null;
                ErrorResponse                   = "The given JSON representation of a request stop transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRequestStopTransactionResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRequestStopTransactionResponseSerializer">A delegate to serialize custom request stop transaction responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RequestStopTransactionResponse>?  CustomRequestStopTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                      CustomStatusInfoSerializer                       = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
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

            return CustomRequestStopTransactionResponseSerializer is not null
                       ? CustomRequestStopTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The request stop transaction request leading to this response.</param>
        public static RequestStopTransactionResponse Failed(CSMS.RequestStopTransactionRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (RequestStopTransactionResponse1, RequestStopTransactionResponse2)

        /// <summary>
        /// Compares two request stop transaction responses for equality.
        /// </summary>
        /// <param name="RequestStopTransactionResponse1">A request stop transaction response.</param>
        /// <param name="RequestStopTransactionResponse2">Another request stop transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RequestStopTransactionResponse? RequestStopTransactionResponse1,
                                           RequestStopTransactionResponse? RequestStopTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RequestStopTransactionResponse1, RequestStopTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (RequestStopTransactionResponse1 is null || RequestStopTransactionResponse2 is null)
                return false;

            return RequestStopTransactionResponse1.Equals(RequestStopTransactionResponse2);

        }

        #endregion

        #region Operator != (RequestStopTransactionResponse1, RequestStopTransactionResponse2)

        /// <summary>
        /// Compares two request stop transaction responses for inequality.
        /// </summary>
        /// <param name="RequestStopTransactionResponse1">A request stop transaction response.</param>
        /// <param name="RequestStopTransactionResponse2">Another request stop transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RequestStopTransactionResponse? RequestStopTransactionResponse1,
                                           RequestStopTransactionResponse? RequestStopTransactionResponse2)

            => !(RequestStopTransactionResponse1 == RequestStopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RequestStopTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two request stop transaction responses for equality.
        /// </summary>
        /// <param name="Object">A request stop transaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestStopTransactionResponse requestStopTransactionResponse &&
                   Equals(requestStopTransactionResponse);

        #endregion

        #region Equals(RequestStopTransactionResponse)

        /// <summary>
        /// Compares two request stop transaction responses for equality.
        /// </summary>
        /// <param name="RequestStopTransactionResponse">A request stop transaction response to compare with.</param>
        public override Boolean Equals(RequestStopTransactionResponse? RequestStopTransactionResponse)

            => RequestStopTransactionResponse is not null &&

               Status.     Equals(RequestStopTransactionResponse.Status) &&

             ((StatusInfo is     null && RequestStopTransactionResponse.StatusInfo is     null) ||
               StatusInfo is not null && RequestStopTransactionResponse.StatusInfo is not null && StatusInfo.Equals(RequestStopTransactionResponse.StatusInfo)) &&

               base.GenericEquals(RequestStopTransactionResponse);

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
