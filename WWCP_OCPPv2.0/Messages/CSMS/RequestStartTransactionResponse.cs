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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// A request start transaction response.
    /// </summary>
    public class RequestStartTransactionResponse : AResponse<CSMS.RequestStartTransactionRequest,
                                                             RequestStartTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// The status indicating whether the charging station accepts the request
        /// to start a charging transaction.
        /// </summary>
        [Mandatory]
        public RequestStartStopStatus  Status           { get; }

        /// <summary>
        /// The optional transaction identification of an already started transaction, when
        /// the transaction was already started by the charging station before the
        /// RequestStartTransactionRequest was received.
        /// For example when the cable was plugged in first.
        /// </summary>
        [Optional]
        public Transaction_Id?         TransactionId    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?             StatusInfo       { get; }

        #endregion

        #region Constructor(s)

        #region RequestStartTransactionResponse(Request, Status, TransactionId = null, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new request start transaction response.
        /// </summary>
        /// <param name="Request">The request start transaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charging station accepts the request to start a charging transaction.</param>
        /// <param name="TransactionId">An optional transaction identification of an already started transaction, when the transaction was already started by the charging station before the RequestStartTransactionRequest was received. For example when the cable was plugged in first.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public RequestStartTransactionResponse(CSMS.RequestStartTransactionRequest  Request,
                                               RequestStartStopStatus               Status,
                                               Transaction_Id?                      TransactionId   = null,
                                               StatusInfo?                          StatusInfo      = null,
                                               CustomData?                          CustomData      = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status         = Status;
            this.TransactionId  = TransactionId;
            this.StatusInfo     = StatusInfo;

        }

        #endregion

        #region RequestStartTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new request start transaction response.
        /// </summary>
        /// <param name="Request">The request start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public RequestStartTransactionResponse(CSMS.RequestStartTransactionRequest  Request,
                                               Result                               Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:RequestStartTransactionResponse",
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
        //       "description": "Status indicating whether the Charging Station accepts the request to start a transaction.\r\n",
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
        //     },
        //     "transactionId": {
        //       "description": "When the transaction was already started by the Charging Station before the RequestStartTransactionRequest was received, for example: cable plugged in first. This contains the transactionId of the already started transaction.\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomRequestStartTransactionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a request start transaction response.
        /// </summary>
        /// <param name="Request">The request start transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRequestStartTransactionResponseParser">A delegate to parse custom request start transaction responses.</param>
        public static RequestStartTransactionResponse Parse(CSMS.RequestStartTransactionRequest                            Request,
                                                            JObject                                                        JSON,
                                                            CustomJObjectParserDelegate<RequestStartTransactionResponse>?  CustomRequestStartTransactionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var requestStartTransactionResponse,
                         out var errorResponse,
                         CustomRequestStartTransactionResponseParser))
            {
                return requestStartTransactionResponse!;
            }

            throw new ArgumentException("The given JSON representation of a request start transaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out RequestStartTransactionResponse, out ErrorResponse, CustomRequestStartTransactionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a request start transaction response.
        /// </summary>
        /// <param name="Request">The request start transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestStartTransactionResponse">The parsed request start transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRequestStartTransactionResponseParser">A delegate to parse custom request start transaction responses.</param>
        public static Boolean TryParse(CSMS.RequestStartTransactionRequest                            Request,
                                       JObject                                                        JSON,
                                       out RequestStartTransactionResponse?                           RequestStartTransactionResponse,
                                       out String?                                                    ErrorResponse,
                                       CustomJObjectParserDelegate<RequestStartTransactionResponse>?  CustomRequestStartTransactionResponseParser   = null)
        {

            try
            {

                RequestStartTransactionResponse = null;

                #region Status           [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "request start stop status",
                                         RequestStartStopStatusExtensions.TryParse,
                                         out RequestStartStopStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionId    [optional]

                if (JSON.ParseOptional("transactionId",
                                       "transaction identification",
                                       Transaction_Id.TryParse,
                                       out Transaction_Id? TransactionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo       [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_0.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData       [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                RequestStartTransactionResponse = new RequestStartTransactionResponse(Request,
                                                                                      Status,
                                                                                      TransactionId,
                                                                                      StatusInfo,
                                                                                      CustomData);

                if (CustomRequestStartTransactionResponseParser is not null)
                    RequestStartTransactionResponse = CustomRequestStartTransactionResponseParser(JSON,
                                                                                                  RequestStartTransactionResponse);

                return true;

            }
            catch (Exception e)
            {
                RequestStartTransactionResponse  = null;
                ErrorResponse                    = "The given JSON representation of a request start transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRequestStartTransactionResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRequestStartTransactionResponseSerializer">A delegate to serialize custom request start transaction responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RequestStartTransactionResponse>?  CustomRequestStartTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                       CustomStatusInfoSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",          Status.       AsText()),

                           TransactionId.HasValue
                               ? new JProperty("transactionId",   TransactionId.ToString())
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",      StatusInfo.   ToJSON(CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRequestStartTransactionResponseSerializer is not null
                       ? CustomRequestStartTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The request start transaction request leading to this response.</param>
        public static RequestStartTransactionResponse Failed(CSMS.RequestStartTransactionRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (RequestStartTransactionResponse1, RequestStartTransactionResponse2)

        /// <summary>
        /// Compares two request start transaction responses for equality.
        /// </summary>
        /// <param name="RequestStartTransactionResponse1">A request start transaction response.</param>
        /// <param name="RequestStartTransactionResponse2">Another request start transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RequestStartTransactionResponse? RequestStartTransactionResponse1,
                                           RequestStartTransactionResponse? RequestStartTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RequestStartTransactionResponse1, RequestStartTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (RequestStartTransactionResponse1 is null || RequestStartTransactionResponse2 is null)
                return false;

            return RequestStartTransactionResponse1.Equals(RequestStartTransactionResponse2);

        }

        #endregion

        #region Operator != (RequestStartTransactionResponse1, RequestStartTransactionResponse2)

        /// <summary>
        /// Compares two request start transaction responses for inequality.
        /// </summary>
        /// <param name="RequestStartTransactionResponse1">A request start transaction response.</param>
        /// <param name="RequestStartTransactionResponse2">Another request start transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RequestStartTransactionResponse? RequestStartTransactionResponse1,
                                           RequestStartTransactionResponse? RequestStartTransactionResponse2)

            => !(RequestStartTransactionResponse1 == RequestStartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RequestStartTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two request start transaction responses for equality.
        /// </summary>
        /// <param name="Object">A request start transaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestStartTransactionResponse requestStartTransactionResponse &&
                   Equals(requestStartTransactionResponse);

        #endregion

        #region Equals(RequestStartTransactionResponse)

        /// <summary>
        /// Compares two request start transaction responses for equality.
        /// </summary>
        /// <param name="RequestStartTransactionResponse">A request start transaction response to compare with.</param>
        public override Boolean Equals(RequestStartTransactionResponse? RequestStartTransactionResponse)

            => RequestStartTransactionResponse is not null &&

               Status.Equals(RequestStartTransactionResponse.Status) &&

             ((!TransactionId.HasValue    && !RequestStartTransactionResponse.TransactionId.HasValue) ||
                TransactionId.HasValue    &&  RequestStartTransactionResponse.TransactionId.HasValue    && TransactionId.Value.Equals(RequestStartTransactionResponse.TransactionId.Value)) &&

             ((StatusInfo     is     null &&  RequestStartTransactionResponse.StatusInfo    is null)  ||
               StatusInfo     is not null &&  RequestStartTransactionResponse.StatusInfo    is not null && StatusInfo.         Equals(RequestStartTransactionResponse.StatusInfo))          &&

               base.GenericEquals(RequestStartTransactionResponse);

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

                return Status.        GetHashCode()       * 7 ^
                      (TransactionId?.GetHashCode() ?? 0) * 5 ^
                      (StatusInfo?.   GetHashCode() ?? 0) * 3 ^

                       base.          GetHashCode();

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
