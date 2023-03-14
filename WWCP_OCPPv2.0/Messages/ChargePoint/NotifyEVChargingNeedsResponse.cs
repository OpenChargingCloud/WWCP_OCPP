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
    /// A notify EV charging needs response.
    /// </summary>
    public class NotifyEVChargingNeedsResponse : AResponse<CP.NotifyEVChargingNeedsRequest,
                                                              NotifyEVChargingNeedsResponse>
    {

        #region Properties

        /// <summary>
        /// Whether the CSMS has been able to process the message successfully.
        /// It does not imply that the EV charging needs can be met with the current charging profile.
        /// </summary>
        [Mandatory]
        public NotifyEVChargingNeedsStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                  StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region NotifyEVChargingNeedsResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new notify EV charging needs response.
        /// </summary>
        /// <param name="Request">The notify EV charging needs request leading to this response.</param>
        /// <param name="Status">Whether the CSMS has been able to process the message successfully. It does not imply that the EV charging needs can be met with the current charging profile.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public NotifyEVChargingNeedsResponse(CP.NotifyEVChargingNeedsRequest  Request,
                                             NotifyEVChargingNeedsStatus      Status,
                                             StatusInfo?                      StatusInfo   = null,
                                             CustomData?                      CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region NotifyEVChargingNeedsResponse(Request, Result)

        /// <summary>
        /// Create a new notify EV charging needs response.
        /// </summary>
        /// <param name="Request">The notify EV charging needs request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyEVChargingNeedsResponse(CP.NotifyEVChargingNeedsRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyEVChargingNeedsResponse",
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
        //     "NotifyEVChargingNeedsStatusEnumType": {
        //       "description": "Returns whether the CSMS has been able to process the message successfully. It does not imply that the evChargingNeeds can be met with the current charging profile.\r\n",
        //       "javaType": "NotifyEVChargingNeedsStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "Processing"
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
        //       "$ref": "#/definitions/NotifyEVChargingNeedsStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomNotifyEVChargingNeedsResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify EV charging needs response.
        /// </summary>
        /// <param name="Request">The notify EV charging needs request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyEVChargingNeedsResponseParser">A delegate to parse custom notify EV charging needs responses.</param>
        public static NotifyEVChargingNeedsResponse Parse(CP.NotifyEVChargingNeedsRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<NotifyEVChargingNeedsResponse>?  CustomNotifyEVChargingNeedsResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyEVChargingNeedsResponse,
                         out var errorResponse,
                         CustomNotifyEVChargingNeedsResponseParser))
            {
                return notifyEVChargingNeedsResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify EV charging needs response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyEVChargingNeedsResponse, out ErrorResponse, CustomNotifyEVChargingNeedsResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging needs response.
        /// </summary>
        /// <param name="Request">The notify EV charging needs request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyEVChargingNeedsResponse">The parsed notify EV charging needs response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingNeedsResponseParser">A delegate to parse custom notify EV charging needs responses.</param>
        public static Boolean TryParse(CP.NotifyEVChargingNeedsRequest                              Request,
                                       JObject                                                      JSON,
                                       out NotifyEVChargingNeedsResponse?                           NotifyEVChargingNeedsResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEVChargingNeedsResponse>?  CustomNotifyEVChargingNeedsResponseParser   = null)
        {

            try
            {

                NotifyEVChargingNeedsResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "notify EV charging needs status",
                                         NotifyEVChargingNeedsStatusExtensions.TryParse,
                                         out NotifyEVChargingNeedsStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

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

                #region CustomData    [optional]

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


                NotifyEVChargingNeedsResponse = new NotifyEVChargingNeedsResponse(Request,
                                                                                  Status,
                                                                                  StatusInfo,
                                                                                  CustomData);

                if (CustomNotifyEVChargingNeedsResponseParser is not null)
                    NotifyEVChargingNeedsResponse = CustomNotifyEVChargingNeedsResponseParser(JSON,
                                                                                              NotifyEVChargingNeedsResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingNeedsResponse  = null;
                ErrorResponse                  = "The given JSON representation of a notify EV charging needs response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingNeedsResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingNeedsResponseSerializer">A delegate to serialize custom notify EV charging needs responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingNeedsResponse>?  CustomNotifyEVChargingNeedsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyEVChargingNeedsResponseSerializer is not null
                       ? CustomNotifyEVChargingNeedsResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify EV charging needs response failed.
        /// </summary>
        /// <param name="Request">The notify EV charging needs request leading to this response.</param>
        public static NotifyEVChargingNeedsResponse Failed(CP.NotifyEVChargingNeedsRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingNeedsResponse1, NotifyEVChargingNeedsResponse2)

        /// <summary>
        /// Compares two notify EV charging needs responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsResponse1">A notify EV charging needs response.</param>
        /// <param name="NotifyEVChargingNeedsResponse2">Another notify EV charging needs response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse1,
                                           NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingNeedsResponse1, NotifyEVChargingNeedsResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingNeedsResponse1 is null || NotifyEVChargingNeedsResponse2 is null)
                return false;

            return NotifyEVChargingNeedsResponse1.Equals(NotifyEVChargingNeedsResponse2);

        }

        #endregion

        #region Operator != (NotifyEVChargingNeedsResponse1, NotifyEVChargingNeedsResponse2)

        /// <summary>
        /// Compares two notify EV charging needs responses for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsResponse1">A notify EV charging needs response.</param>
        /// <param name="NotifyEVChargingNeedsResponse2">Another notify EV charging needs response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse1,
                                           NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse2)

            => !(NotifyEVChargingNeedsResponse1 == NotifyEVChargingNeedsResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingNeedsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify EV charging needs responses for equality.
        /// </summary>
        /// <param name="Object">A notify EV charging needs response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingNeedsResponse notifyEVChargingNeedsResponse &&
                   Equals(notifyEVChargingNeedsResponse);

        #endregion

        #region Equals(NotifyEVChargingNeedsResponse)

        /// <summary>
        /// Compares two notify EV charging needs responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsResponse">A notify EV charging needs response to compare with.</param>
        public override Boolean Equals(NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse)

            => NotifyEVChargingNeedsResponse is not null &&

               Status.     Equals(NotifyEVChargingNeedsResponse.Status) &&

             ((StatusInfo is     null && NotifyEVChargingNeedsResponse.StatusInfo is     null) ||
               StatusInfo is not null && NotifyEVChargingNeedsResponse.StatusInfo is not null && StatusInfo.Equals(NotifyEVChargingNeedsResponse.StatusInfo)) &&

               base.GenericEquals(NotifyEVChargingNeedsResponse);

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

            => Status.ToString();

        #endregion

    }

}
