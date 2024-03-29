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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CS
{

    /// <summary>
    /// A set monitoring base response.
    /// </summary>
    public class SetMonitoringBaseResponse : AResponse<CSMS.SetMonitoringBaseRequest,
                                                       SetMonitoringBaseResponse>
    {

        #region Properties

        /// <summary>
        /// Whether the charging station was able to accept the request.
        /// </summary>
        [Mandatory]
        public GenericDeviceModelStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region SetMonitoringBaseResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new set monitoring base response.
        /// </summary>
        /// <param name="Request">The set monitoring base request leading to this response.</param>
        /// <param name="Status">Whether the charging station was able to accept the request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public SetMonitoringBaseResponse(CSMS.SetMonitoringBaseRequest  Request,
                                         GenericDeviceModelStatus       Status,
                                         StatusInfo?                    StatusInfo   = null,
                                         CustomData?                    CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region SetMonitoringBaseResponse(Request, Result)

        /// <summary>
        /// Create a new set monitoring base response.
        /// </summary>
        /// <param name="Request">The set monitoring base request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SetMonitoringBaseResponse(CSMS.SetMonitoringBaseRequest  Request,
                                         Result                         Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetMonitoringBaseResponse",
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
        //     "GenericDeviceModelStatusEnumType": {
        //       "description": "Indicates whether the Charging Station was able to accept the request.\r\n",
        //       "javaType": "GenericDeviceModelStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "NotSupported",
        //         "EmptyResultSet"
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
        //       "$ref": "#/definitions/GenericDeviceModelStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSetMonitoringBaseResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set monitoring base response.
        /// </summary>
        /// <param name="Request">The set monitoring base request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetMonitoringBaseResponseParser">An optional delegate to parse custom set monitoring base responses.</param>
        public static SetMonitoringBaseResponse Parse(CSMS.SetMonitoringBaseRequest                            Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<SetMonitoringBaseResponse>?  CustomSetMonitoringBaseResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var setDisplayMessageResponse,
                         out var errorResponse,
                         CustomSetMonitoringBaseResponseParser))
            {
                return setDisplayMessageResponse!;
            }

            throw new ArgumentException("The given JSON representation of a set monitoring base response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetMonitoringBaseResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a set monitoring base response.
        /// </summary>
        /// <param name="Request">The set monitoring base request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetMonitoringBaseResponse">The parsed set monitoring base response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetMonitoringBaseResponseParser">An optional delegate to parse custom set monitoring base responses.</param>
        public static Boolean TryParse(CSMS.SetMonitoringBaseRequest                            Request,
                                       JObject                                                  JSON,
                                       out SetMonitoringBaseResponse?                           SetMonitoringBaseResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<SetMonitoringBaseResponse>?  CustomSetMonitoringBaseResponseParser   = null)
        {

            try
            {

                SetMonitoringBaseResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "display message status",
                                         GenericDeviceModelStatusExtensions.TryParse,
                                         out GenericDeviceModelStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_0_1.StatusInfo.TryParse,
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
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetMonitoringBaseResponse = new SetMonitoringBaseResponse(Request,
                                                                          Status,
                                                                          StatusInfo,
                                                                          CustomData);

                if (CustomSetMonitoringBaseResponseParser is not null)
                    SetMonitoringBaseResponse = CustomSetMonitoringBaseResponseParser(JSON,
                                                                                      SetMonitoringBaseResponse);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringBaseResponse  = null;
                ErrorResponse              = "The given JSON representation of a set monitoring base response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringBaseResponseSerializer = null, CustomStatusInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringBaseResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetMonitoringBaseResponse>?  CustomSetMonitoringBaseResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
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

            return CustomSetMonitoringBaseResponseSerializer is not null
                       ? CustomSetMonitoringBaseResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The set monitoring base command failed.
        /// </summary>
        /// <param name="Request">The set monitoring base request leading to this response.</param>
        public static SetMonitoringBaseResponse Failed(CSMS.SetMonitoringBaseRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringBaseResponse1, SetMonitoringBaseResponse2)

        /// <summary>
        /// Compares two set monitoring base responses for equality.
        /// </summary>
        /// <param name="SetMonitoringBaseResponse1">A set monitoring base response.</param>
        /// <param name="SetMonitoringBaseResponse2">Another set monitoring base response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetMonitoringBaseResponse? SetMonitoringBaseResponse1,
                                           SetMonitoringBaseResponse? SetMonitoringBaseResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringBaseResponse1, SetMonitoringBaseResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringBaseResponse1 is null || SetMonitoringBaseResponse2 is null)
                return false;

            return SetMonitoringBaseResponse1.Equals(SetMonitoringBaseResponse2);

        }

        #endregion

        #region Operator != (SetMonitoringBaseResponse1, SetMonitoringBaseResponse2)

        /// <summary>
        /// Compares two set monitoring base responses for inequality.
        /// </summary>
        /// <param name="SetMonitoringBaseResponse1">A set monitoring base response.</param>
        /// <param name="SetMonitoringBaseResponse2">Another set monitoring base response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetMonitoringBaseResponse? SetMonitoringBaseResponse1,
                                           SetMonitoringBaseResponse? SetMonitoringBaseResponse2)

            => !(SetMonitoringBaseResponse1 == SetMonitoringBaseResponse2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringBaseResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set monitoring base responses for equality.
        /// </summary>
        /// <param name="Object">A set monitoring base response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringBaseResponse setDisplayMessageResponse &&
                   Equals(setDisplayMessageResponse);

        #endregion

        #region Equals(SetMonitoringBaseResponse)

        /// <summary>
        /// Compares two set monitoring base responses for equality.
        /// </summary>
        /// <param name="SetMonitoringBaseResponse">A set monitoring base response to compare with.</param>
        public override Boolean Equals(SetMonitoringBaseResponse? SetMonitoringBaseResponse)

            => SetMonitoringBaseResponse is not null &&

               Status.     Equals(SetMonitoringBaseResponse.Status) &&

             ((StatusInfo is     null && SetMonitoringBaseResponse.StatusInfo is     null) ||
               StatusInfo is not null && SetMonitoringBaseResponse.StatusInfo is not null && StatusInfo.Equals(SetMonitoringBaseResponse.StatusInfo)) &&

               base.GenericEquals(SetMonitoringBaseResponse);

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
