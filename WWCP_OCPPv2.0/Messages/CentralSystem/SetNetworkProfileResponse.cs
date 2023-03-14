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

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    /// <summary>
    /// A set network profile response.
    /// </summary>
    public class SetNetworkProfileResponse : AResponse<CS.SetNetworkProfileRequest,
                                                          SetNetworkProfileResponse>
    {

        #region Properties

        /// <summary>
        /// Whether the charging station was able to accept the request.
        /// </summary>
        [Mandatory]
        public SetNetworkProfileStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?              StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region SetNetworkProfileResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new set network profile response.
        /// </summary>
        /// <param name="Request">The set network profile request leading to this response.</param>
        /// <param name="Status">Whether the charging station was able to accept the request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public SetNetworkProfileResponse(CS.SetNetworkProfileRequest  Request,
                                         SetNetworkProfileStatus      Status,
                                         StatusInfo?                  StatusInfo   = null,
                                         CustomData?                  CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region SetNetworkProfileResponse(Request, Result)

        /// <summary>
        /// Create a new set network profile response.
        /// </summary>
        /// <param name="Request">The set network profile request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SetNetworkProfileResponse(CS.SetNetworkProfileRequest  Request,
                                         Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetNetworkProfileResponse",
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
        //     "SetNetworkProfileStatusEnumType": {
        //       "description": "Result of operation.\r\n",
        //       "javaType": "SetNetworkProfileStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "Failed"
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
        //       "$ref": "#/definitions/SetNetworkProfileStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSetNetworkProfileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set network profile response.
        /// </summary>
        /// <param name="Request">The set network profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetNetworkProfileResponseParser">A delegate to parse custom set network profile responses.</param>
        public static SetNetworkProfileResponse Parse(CS.SetNetworkProfileRequest                              Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<SetNetworkProfileResponse>?  CustomSetNetworkProfileResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var setNetworkProfileResponse,
                         out var errorResponse,
                         CustomSetNetworkProfileResponseParser))
            {
                return setNetworkProfileResponse!;
            }

            throw new ArgumentException("The given JSON representation of a set network profile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetNetworkProfileResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a set network profile response.
        /// </summary>
        /// <param name="Request">The set network profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetNetworkProfileResponse">The parsed set network profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetNetworkProfileResponseParser">A delegate to parse custom set network profile responses.</param>
        public static Boolean TryParse(CS.SetNetworkProfileRequest                              Request,
                                       JObject                                                  JSON,
                                       out SetNetworkProfileResponse?                           SetNetworkProfileResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<SetNetworkProfileResponse>?  CustomSetNetworkProfileResponseParser   = null)
        {

            try
            {

                SetNetworkProfileResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "set network profile status",
                                         SetNetworkProfileStatusExtensions.TryParse,
                                         out SetNetworkProfileStatus Status,
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


                SetNetworkProfileResponse = new SetNetworkProfileResponse(Request,
                                                                          Status,
                                                                          StatusInfo,
                                                                          CustomData);

                if (CustomSetNetworkProfileResponseParser is not null)
                    SetNetworkProfileResponse = CustomSetNetworkProfileResponseParser(JSON,
                                                                                      SetNetworkProfileResponse);

                return true;

            }
            catch (Exception e)
            {
                SetNetworkProfileResponse  = null;
                ErrorResponse              = "The given JSON representation of a set network profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetNetworkProfileResponseSerializer = null, CustomStatusInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetNetworkProfileResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetNetworkProfileResponse>?  CustomSetNetworkProfileResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
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

            return CustomSetNetworkProfileResponseSerializer is not null
                       ? CustomSetNetworkProfileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The set network profile command failed.
        /// </summary>
        /// <param name="Request">The set network profile request leading to this response.</param>
        public static SetNetworkProfileResponse Failed(CS.SetNetworkProfileRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SetNetworkProfileResponse1, SetNetworkProfileResponse2)

        /// <summary>
        /// Compares two set network profile responses for equality.
        /// </summary>
        /// <param name="SetNetworkProfileResponse1">A set network profile response.</param>
        /// <param name="SetNetworkProfileResponse2">Another set network profile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetNetworkProfileResponse? SetNetworkProfileResponse1,
                                           SetNetworkProfileResponse? SetNetworkProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetNetworkProfileResponse1, SetNetworkProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetNetworkProfileResponse1 is null || SetNetworkProfileResponse2 is null)
                return false;

            return SetNetworkProfileResponse1.Equals(SetNetworkProfileResponse2);

        }

        #endregion

        #region Operator != (SetNetworkProfileResponse1, SetNetworkProfileResponse2)

        /// <summary>
        /// Compares two set network profile responses for inequality.
        /// </summary>
        /// <param name="SetNetworkProfileResponse1">A set network profile response.</param>
        /// <param name="SetNetworkProfileResponse2">Another set network profile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetNetworkProfileResponse? SetNetworkProfileResponse1,
                                           SetNetworkProfileResponse? SetNetworkProfileResponse2)

            => !(SetNetworkProfileResponse1 == SetNetworkProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<SetNetworkProfileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set network profile responses for equality.
        /// </summary>
        /// <param name="Object">A set network profile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetNetworkProfileResponse setNetworkProfileResponse &&
                   Equals(setNetworkProfileResponse);

        #endregion

        #region Equals(SetNetworkProfileResponse)

        /// <summary>
        /// Compares two set network profile responses for equality.
        /// </summary>
        /// <param name="SetNetworkProfileResponse">A set network profile response to compare with.</param>
        public override Boolean Equals(SetNetworkProfileResponse? SetNetworkProfileResponse)

            => SetNetworkProfileResponse is not null &&

               Status.     Equals(SetNetworkProfileResponse.Status) &&

             ((StatusInfo is     null && SetNetworkProfileResponse.StatusInfo is     null) ||
               StatusInfo is not null && SetNetworkProfileResponse.StatusInfo is not null && StatusInfo.Equals(SetNetworkProfileResponse.StatusInfo)) &&

               base.GenericEquals(SetNetworkProfileResponse);

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
