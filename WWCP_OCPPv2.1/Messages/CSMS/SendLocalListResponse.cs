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
    /// A send local list response.
    /// </summary>
    public class SendLocalListResponse : AResponse<CSMS.SendLocalListRequest,
                                                   SendLocalListResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the send local list command.
        /// </summary>
        [Mandatory]
        public SendLocalListStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?          StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region SendLocalListResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="Status">The success or failure of the send local list command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public SendLocalListResponse(CSMS.SendLocalListRequest  Request,
                                     SendLocalListStatus        Status,
                                     StatusInfo?                StatusInfo   = null,
                                     CustomData?                CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region SendLocalListResponse(Request, Result)

        /// <summary>
        /// Create a new send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SendLocalListResponse(CSMS.SendLocalListRequest  Request,
                                     Result                     Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SendLocalListResponse",
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
        //     "SendLocalListStatusEnumType": {
        //       "description": "This indicates whether the Charging Station has successfully received and applied the update of the Local Authorization List.\r\n",
        //       "javaType": "SendLocalListStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed",
        //         "VersionMismatch"
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
        //       "$ref": "#/definitions/SendLocalListStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSendLocalListResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSendLocalListResponseParser">A delegate to parse custom send local list responses.</param>
        public static SendLocalListResponse Parse(CSMS.SendLocalListRequest                            Request,
                                                  JObject                                              JSON,
                                                  CustomJObjectParserDelegate<SendLocalListResponse>?  CustomSendLocalListResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var sendLocalListResponse,
                         out var errorResponse,
                         CustomSendLocalListResponseParser))
            {
                return sendLocalListResponse!;
            }

            throw new ArgumentException("The given JSON representation of a send local list response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SendLocalListResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendLocalListResponseParser">A delegate to parse custom send local list responses.</param>
        public static Boolean TryParse(CSMS.SendLocalListRequest                            Request,
                                       JObject                                              JSON,
                                       out SendLocalListResponse?                           SendLocalListResponse,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<SendLocalListResponse>?  CustomSendLocalListResponseParser   = null)
        {

            try
            {

                SendLocalListResponse = null;

                #region SendLocalListStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "update status",
                                         SendLocalListStatusExtensions.TryParse,
                                         out SendLocalListStatus SendLocalListStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo             [optional]

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

                #region CustomData             [optional]

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


                SendLocalListResponse = new SendLocalListResponse(Request,
                                                                  SendLocalListStatus,
                                                                  StatusInfo,
                                                                  CustomData);

                if (CustomSendLocalListResponseParser is not null)
                    SendLocalListResponse = CustomSendLocalListResponseParser(JSON,
                                                                              SendLocalListResponse);

                return true;

            }
            catch (Exception e)
            {
                SendLocalListResponse  = null;
                ErrorResponse          = "The given JSON representation of a send local list response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSendLocalListResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendLocalListResponseSerializer">A delegate to serialize custom send local list responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SendLocalListResponse>?  CustomSendLocalListResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?             CustomStatusInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSendLocalListResponseSerializer is not null
                       ? CustomSendLocalListResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The send local list command failed.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        public static SendLocalListResponse Failed(CSMS.SendLocalListRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A send local list response.</param>
        /// <param name="SendLocalListResponse2">Another send local list response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendLocalListResponse? SendLocalListResponse1,
                                           SendLocalListResponse? SendLocalListResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendLocalListResponse1, SendLocalListResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SendLocalListResponse1 is null || SendLocalListResponse2 is null)
                return false;

            return SendLocalListResponse1.Equals(SendLocalListResponse2);

        }

        #endregion

        #region Operator != (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two send local list responses for inequality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A send local list response.</param>
        /// <param name="SendLocalListResponse2">Another send local list response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListResponse? SendLocalListResponse1,
                                           SendLocalListResponse? SendLocalListResponse2)

            => !(SendLocalListResponse1 == SendLocalListResponse2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="Object">A send local list response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendLocalListResponse sendLocalListResponse &&
                   Equals(sendLocalListResponse);

        #endregion

        #region Equals(SendLocalListResponse)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse">A send local list response to compare with.</param>
        public override Boolean Equals(SendLocalListResponse? SendLocalListResponse)

            => SendLocalListResponse is not null &&

               Status.     Equals(SendLocalListResponse.Status) &&

             ((StatusInfo is     null && SendLocalListResponse.StatusInfo is     null) ||
               StatusInfo is not null && SendLocalListResponse.StatusInfo is not null && StatusInfo.Equals(SendLocalListResponse.StatusInfo)) &&

               base.GenericEquals(SendLocalListResponse);

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
