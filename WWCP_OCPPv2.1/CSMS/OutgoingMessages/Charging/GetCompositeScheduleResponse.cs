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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A get composite schedule response.
    /// </summary>
    public class GetCompositeScheduleResponse : AResponse<CSMS.GetCompositeScheduleRequest,
                                                          GetCompositeScheduleResponse>,
                                                IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getCompositeScheduleResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charging station will indicate if it was able to process the request.
        /// </summary>
        [Mandatory]
        public GenericStatus       Status        { get; }

        /// <summary>
        /// The calculated composite schedule.
        /// It may only be omitted when this message contains status 'rejected'.
        /// </summary>
        [Optional]
        public CompositeSchedule?  Schedule      { get;  }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region GetCompositeScheduleResponse(Request, Status, Schedule = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="Status">The charging station will indicate if it was able to process the request.</param>
        /// <param name="Schedule">The calculated composite schedule. It may only be omitted when this message contains status 'rejected'.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetCompositeScheduleResponse(CSMS.GetCompositeScheduleRequest  Request,
                                            GenericStatus                     Status,
                                            CompositeSchedule?                Schedule            = null,
                                            StatusInfo?                       StatusInfo          = null,
                                            DateTime?                         ResponseTimestamp   = null,

                                            IEnumerable<KeyPair>?             SignKeys            = null,
                                            IEnumerable<SignInfo>?            SignInfos           = null,
                                            IEnumerable<OCPP.Signature>?      Signatures          = null,

                                            CustomData?                       CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.Schedule    = Schedule;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region GetCompositeScheduleResponse(Request, Result)

        /// <summary>
        /// Create a new get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetCompositeScheduleResponse(CSMS.GetCompositeScheduleRequest  Request,
                                            Result                            Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetCompositeScheduleResponse",
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
        //     "ChargingRateUnitEnumType": {
        //       "description": "The unit of measure Limit is\r\nexpressed in.\r\n",
        //       "javaType": "ChargingRateUnitEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "W",
        //         "A"
        //       ]
        //     },
        //     "GenericStatusEnumType": {
        //       "description": "The Charging Station will indicate if it was\r\nable to process the request\r\n",
        //       "javaType": "GenericStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected"
        //       ]
        //     },
        //     "ChargingSchedulePeriodType": {
        //       "description": "Charging_ Schedule_ Period\r\nurn:x-oca:ocpp:uid:2:233257\r\nCharging schedule period structure defines a time period in a charging schedule.\r\n",
        //       "javaType": "ChargingSchedulePeriod",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "startPeriod": {
        //           "description": "Charging_ Schedule_ Period. Start_ Period. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569240\r\nStart of the period, in seconds from the start of schedule. The value of StartPeriod also defines the stop time of the previous period.\r\n",
        //           "type": "integer"
        //         },
        //         "limit": {
        //           "description": "Charging_ Schedule_ Period. Limit. Measure\r\nurn:x-oca:ocpp:uid:1:569241\r\nCharging rate limit during the schedule period, in the applicable chargingRateUnit, for example in Amperes (A) or Watts (W). Accepts at most one digit fraction (e.g. 8.1).\r\n",
        //           "type": "number"
        //         },
        //         "numberPhases": {
        //           "description": "Charging_ Schedule_ Period. Number_ Phases. Counter\r\nurn:x-oca:ocpp:uid:1:569242\r\nThe number of phases that can be used for charging. If a number of phases is needed, numberPhases=3 will be assumed unless another number is given.\r\n",
        //           "type": "integer"
        //         },
        //         "phaseToUse": {
        //           "description": "Values: 1..3, Used if numberPhases=1 and if the EVSE is capable of switching the phase connected to the EV, i.e. ACPhaseSwitchingSupported is defined and true. It’s not allowed unless both conditions above are true. If both conditions are true, and phaseToUse is omitted, the Charging Station / EVSE will make the selection on its own.\r\n\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "startPeriod",
        //         "limit"
        //       ]
        //     },
        //     "CompositeScheduleType": {
        //       "description": "Composite_ Schedule\r\nurn:x-oca:ocpp:uid:2:233362\r\n",
        //       "javaType": "CompositeSchedule",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "chargingSchedulePeriod": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/ChargingSchedulePeriodType"
        //           },
        //           "minItems": 1
        //         },
        //         "evseId": {
        //           "description": "The ID of the EVSE for which the\r\nschedule is requested. When evseid=0, the\r\nCharging Station calculated the expected\r\nconsumption for the grid connection.\r\n",
        //           "type": "integer"
        //         },
        //         "duration": {
        //           "description": "Duration of the schedule in seconds.\r\n",
        //           "type": "integer"
        //         },
        //         "scheduleStart": {
        //           "description": "Composite_ Schedule. Start. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569456\r\nDate and time at which the schedule becomes active. All time measurements within the schedule are relative to this timestamp.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "chargingRateUnit": {
        //           "$ref": "#/definitions/ChargingRateUnitEnumType"
        //         }
        //       },
        //       "required": [
        //         "evseId",
        //         "duration",
        //         "scheduleStart",
        //         "chargingRateUnit",
        //         "chargingSchedulePeriod"
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
        //       "$ref": "#/definitions/GenericStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "schedule": {
        //       "$ref": "#/definitions/CompositeScheduleType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetCompositeScheduleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetCompositeScheduleResponseParser">A delegate to parse custom get composite schedule responses.</param>
        public static GetCompositeScheduleResponse Parse(CSMS.GetCompositeScheduleRequest                            Request,
                                                         JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getCompositeScheduleResponse,
                         out var errorResponse,
                         CustomGetCompositeScheduleResponseParser))
            {
                return getCompositeScheduleResponse;
            }

            throw new ArgumentException("The given JSON representation of a get composite schedule response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetCompositeScheduleResponse, out ErrorResponse, CustomGetCompositeScheduleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCompositeScheduleResponseParser">A delegate to parse custom get composite schedule responses.</param>
        public static Boolean TryParse(CSMS.GetCompositeScheduleRequest                            Request,
                                       JObject                                                     JSON,
                                       [NotNullWhen(true)]  out GetCompositeScheduleResponse?      GetCompositeScheduleResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseParser   = null)
        {

            try
            {

                GetCompositeScheduleResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "get composite schedule status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Schedule      [optional]

                if (JSON.ParseOptionalJSON("schedule",
                                           "charging schedule",
                                           CompositeSchedule.TryParse,
                                           out CompositeSchedule? Schedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPP.StatusInfo.TryParse,
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(
                                                   Request,
                                                   Status,
                                                   Schedule,
                                                   StatusInfo,
                                                   null,
                                                   null,
                                                   null,
                                                   Signatures,
                                                   CustomData
                                               );

                if (CustomGetCompositeScheduleResponseParser is not null)
                    GetCompositeScheduleResponse = CustomGetCompositeScheduleResponseParser(JSON,
                                                                                            GetCompositeScheduleResponse);

                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleResponse  = null;
                ErrorResponse                 = "The given JSON representation of a get composite schedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCompositeScheduleResponseSerializer = null, CustomCompositeScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCompositeScheduleResponseSerializer">A delegate to serialize custom get composite schedule responses.</param>
        /// <param name="CustomCompositeScheduleSerializer">A delegate to serialize custom composite schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CompositeSchedule>?             CustomCompositeScheduleSerializer              = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?        CustomChargingSchedulePeriodSerializer         = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                    CustomStatusInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Schedule is not null
                               ? new JProperty("schedule",     Schedule.  ToJSON(CustomCompositeScheduleSerializer,
                                                                                 CustomChargingSchedulePeriodSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

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

            return CustomGetCompositeScheduleResponseSerializer is not null
                       ? CustomGetCompositeScheduleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get composite schedule request failed.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        public static GetCompositeScheduleResponse Failed(CSMS.GetCompositeScheduleRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetCompositeScheduleResponse1, GetCompositeScheduleResponse2)

        /// <summary>
        /// Compares two get composite schedule responses for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse1">A get composite schedule response.</param>
        /// <param name="GetCompositeScheduleResponse2">Another get composite schedule response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCompositeScheduleResponse? GetCompositeScheduleResponse1,
                                           GetCompositeScheduleResponse? GetCompositeScheduleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCompositeScheduleResponse1, GetCompositeScheduleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetCompositeScheduleResponse1 is null || GetCompositeScheduleResponse2 is null)
                return false;

            return GetCompositeScheduleResponse1.Equals(GetCompositeScheduleResponse2);

        }

        #endregion

        #region Operator != (GetCompositeScheduleResponse1, GetCompositeScheduleResponse2)

        /// <summary>
        /// Compares two get composite schedule responses for inequality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse1">A get composite schedule response.</param>
        /// <param name="GetCompositeScheduleResponse2">Another get composite schedule response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCompositeScheduleResponse? GetCompositeScheduleResponse1,
                                           GetCompositeScheduleResponse? GetCompositeScheduleResponse2)

            => !(GetCompositeScheduleResponse1 == GetCompositeScheduleResponse2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get composite schedule responses for equality.
        /// </summary>
        /// <param name="Object">A get composite schedule response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCompositeScheduleResponse getCompositeScheduleResponse &&
                   Equals(getCompositeScheduleResponse);

        #endregion

        #region Equals(GetCompositeScheduleResponse)

        /// <summary>
        /// Compares two get composite schedule responses for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse">A get composite schedule response to compare with.</param>
        public override Boolean Equals(GetCompositeScheduleResponse? GetCompositeScheduleResponse)

            => GetCompositeScheduleResponse is not null &&

               Status.     Equals(GetCompositeScheduleResponse.Status) &&

             ((Schedule   is     null && GetCompositeScheduleResponse.Schedule   is     null) ||
              (Schedule   is not null && GetCompositeScheduleResponse.Schedule   is not null && Schedule.  Equals(GetCompositeScheduleResponse.Schedule))) &&

             ((StatusInfo is     null && GetCompositeScheduleResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetCompositeScheduleResponse.StatusInfo is not null && StatusInfo.Equals(GetCompositeScheduleResponse.StatusInfo)) &&

               base.GenericEquals(GetCompositeScheduleResponse);

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

                return Status.     GetHashCode()       * 7 ^
                      (Schedule?.  GetHashCode() ?? 0) * 5 ^
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

            => String.Concat(Status.AsText(),
                             Schedule is not null
                                 ? ": " + Schedule
                                 : "");

        #endregion

    }

}
