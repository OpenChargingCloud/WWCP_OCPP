/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
    /// A trigger message response.
    /// </summary>
    public class TriggerMessageResponse : AResponse<CS.TriggerMessageRequest,
                                                       TriggerMessageResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the trigger message command.
        /// </summary>
        [Mandatory]
        public TriggerMessageStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?           StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region TriggerMessageResponse(Request, Status)

        /// <summary>
        /// Create a new trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="Status">The success or failure of the trigger message command.</param>
        public TriggerMessageResponse(CS.TriggerMessageRequest  Request,
                                      TriggerMessageStatus      Status,
                                      StatusInfo?               StatusInfo = null,
                                      CustomData?               CustomData = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status = Status;
            this.StatusInfo = StatusInfo;

        }

        #endregion

        #region TriggerMessageResponse(Request, Result)

        /// <summary>
        /// Create a new trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public TriggerMessageResponse(CS.TriggerMessageRequest  Request,
                                      Result                    Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:TriggerMessageResponse",
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
        //     "TriggerMessageStatusEnumType": {
        //       "description": "Indicates whether the Charging Station will send the requested notification or not.\r\n",
        //       "javaType": "TriggerMessageStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "NotImplemented"
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
        //       "$ref": "#/definitions/TriggerMessageStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomTriggerMessageResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomTriggerMessageResponseParser">A delegate to parse custom trigger message responses.</param>
        public static TriggerMessageResponse Parse(CS.TriggerMessageRequest                              Request,
                                                   JObject                                               JSON,
                                                   CustomJObjectParserDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var triggerMessageResponse,
                         out var errorResponse,
                         CustomTriggerMessageResponseParser))
            {
                return triggerMessageResponse!;
            }

            throw new ArgumentException("The given JSON representation of a trigger message response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out TriggerMessageResponse, out ErrorResponse, CustomTriggerMessageResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTriggerMessageResponseParser">A delegate to parse custom trigger message responses.</param>
        public static Boolean TryParse(CS.TriggerMessageRequest                              Request,
                                       JObject                                               JSON,
                                       out TriggerMessageResponse?                           TriggerMessageResponse,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseParser   = null)
        {

            try
            {

                TriggerMessageResponse = null;

                #region TriggerMessageStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "trigger message status",
                                       TriggerMessageStatusExtentions.Parse,
                                       out TriggerMessageStatus TriggerMessageStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo              [optional]

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

                #region CustomData              [optional]

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


                TriggerMessageResponse = new TriggerMessageResponse(Request,
                                                                    TriggerMessageStatus,
                                                                    StatusInfo,
                                                                    CustomData);

                if (CustomTriggerMessageResponseParser is not null)
                    TriggerMessageResponse = CustomTriggerMessageResponseParser(JSON,
                                                                                TriggerMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                TriggerMessageResponse  = null;
                ErrorResponse           = "The given JSON representation of a trigger message response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTriggerMessageResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTriggerMessageResponseSerializer">A delegate to serialize custom trigger message responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?              CustomStatusInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
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

            return CustomTriggerMessageResponseSerializer is not null
                       ? CustomTriggerMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The trigger message command failed.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        public static TriggerMessageResponse Failed(CS.TriggerMessageRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A trigger message response.</param>
        /// <param name="TriggerMessageResponse2">Another trigger message response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TriggerMessageResponse? TriggerMessageResponse1,
                                           TriggerMessageResponse? TriggerMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageResponse1, TriggerMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (TriggerMessageResponse1 is null || TriggerMessageResponse2 is null)
                return false;

            return TriggerMessageResponse1.Equals(TriggerMessageResponse2);

        }

        #endregion

        #region Operator != (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two trigger message responses for inequality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A trigger message response.</param>
        /// <param name="TriggerMessageResponse2">Another trigger message response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TriggerMessageResponse? TriggerMessageResponse1,
                                           TriggerMessageResponse? TriggerMessageResponse2)

            => !(TriggerMessageResponse1 == TriggerMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="Object">A trigger message response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TriggerMessageResponse triggerMessageResponse &&
                   Equals(triggerMessageResponse);

        #endregion

        #region Equals(TriggerMessageResponse)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse">A trigger message response to compare with.</param>
        public override Boolean Equals(TriggerMessageResponse? TriggerMessageResponse)

            => TriggerMessageResponse is not null &&

               Status.     Equals(TriggerMessageResponse.Status) &&

             ((StatusInfo is     null && TriggerMessageResponse.StatusInfo is     null) ||
               StatusInfo is not null && TriggerMessageResponse.StatusInfo is not null && StatusInfo.Equals(TriggerMessageResponse.StatusInfo)) &&

               base.GenericEquals(TriggerMessageResponse);

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
