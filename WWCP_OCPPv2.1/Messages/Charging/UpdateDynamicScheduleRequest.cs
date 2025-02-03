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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The UpdateDynamicSchedule request.
    /// </summary>
    public class UpdateDynamicScheduleRequest : ARequest<UpdateDynamicScheduleRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/updateDynamicScheduleRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of the charging profile to update.
        /// </summary>
        [Mandatory]
        public ChargingProfile_Id      ChargingProfileId      { get; }

        /// <summary>
        /// The schedule update.
        /// </summary>
        [Mandatory]
        public ChargingScheduleUpdate  ScheduleUpdate    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an UpdateDynamicSchedule request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ChargingProfileId">The identification of the charging profile to update.</param>
        /// <param name="ScheduleUpdate">A schedule update.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public UpdateDynamicScheduleRequest(SourceRouting            Destination,
                                            ChargingProfile_Id       ChargingProfileId,
                                            ChargingScheduleUpdate   ScheduleUpdate,

                                            IEnumerable<KeyPair>?    SignKeys              = null,
                                            IEnumerable<SignInfo>?   SignInfos             = null,
                                            IEnumerable<Signature>?  Signatures            = null,

                                            CustomData?              CustomData            = null,

                                            Request_Id?              RequestId             = null,
                                            DateTime?                RequestTimestamp      = null,
                                            TimeSpan?                RequestTimeout        = null,
                                            EventTracking_Id?        EventTrackingId       = null,
                                            NetworkPath?             NetworkPath           = null,
                                            SerializationFormats?    SerializationFormat   = null,
                                            CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(UpdateDynamicScheduleRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.ChargingProfileId  = ChargingProfileId;
            this.ScheduleUpdate     = ScheduleUpdate;

            unchecked
            {

                hashCode = ChargingProfileId.GetHashCode() * 5 ^
                           ScheduleUpdate.   GetHashCode() * 3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:UpdateDynamicScheduleRequest",
        //     "description": "Id of dynamic charging profile to update.",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ChargingScheduleUpdateType": {
        //             "description": "Updates to a ChargingSchedulePeriodType for dynamic charging profiles.",
        //             "javaType": "ChargingScheduleUpdate",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "limit": {
        //                     "description": "Optional only when not required by the _operationMode_, as in CentralSetpoint, ExternalSetpoint, ExternalLimits, LocalFrequency,  LocalLoadBalancing. +\r\nCharging rate limit during the schedule period, in the applicable _chargingRateUnit_. \r\nThis SHOULD be a non-negative value; a negative value is only supported for backwards compatibility with older systems that use a negative value to specify a discharging limit.\r\nFor AC this field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "limit_L2": {
        //                     "description": "*(2.1)* Charging rate limit on phase L2  in the applicable _chargingRateUnit_. ",
        //                     "type": "number"
        //                 },
        //                 "limit_L3": {
        //                     "description": "*(2.1)* Charging rate limit on phase L3  in the applicable _chargingRateUnit_. ",
        //                     "type": "number"
        //                 },
        //                 "dischargeLimit": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ that the EV is allowed to discharge with. Note, these are negative values in order to be consistent with _setpoint_, which can be positive and negative.  +\r\nFor AC this field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "dischargeLimit_L2": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ on phase L2 that the EV is allowed to discharge with. ",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "dischargeLimit_L3": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ on phase L3 that the EV is allowed to discharge with. ",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "setpoint": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow as close as possible. Use negative values for discharging. +\r\nWhen a limit and/or _dischargeLimit_ are given the overshoot when following _setpoint_ must remain within these values.\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "setpoint_L2": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow on phase L2 as close as possible.",
        //                     "type": "number"
        //                 },
        //                 "setpoint_L3": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow on phase L3 as close as possible. ",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow as closely as possible. Positive values for inductive, negative for capacitive reactive power or current.  +\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive_L2": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow on phase L2 as closely as possible. ",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive_L3": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow on phase L3 as closely as possible. ",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "chargingProfileId": {
        //             "description": "Id of charging profile to update.",
        //             "type": "integer"
        //         },
        //         "scheduleUpdate": {
        //             "$ref": "#/definitions/ChargingScheduleUpdateType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "chargingProfileId",
        //         "scheduleUpdate"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an UpdateDynamicSchedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomUpdateDynamicScheduleRequestParser">A delegate to parse custom UpdateDynamicSchedule requests.</param>
        public static UpdateDynamicScheduleRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                               Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<UpdateDynamicScheduleRequest>?  CustomUpdateDynamicScheduleRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var updateDynamicScheduleRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomUpdateDynamicScheduleRequestParser))
            {
                return updateDynamicScheduleRequest;
            }

            throw new ArgumentException("The given JSON representation of an UpdateDynamicSchedule request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out UpdateDynamicScheduleRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateDynamicSchedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="UpdateDynamicScheduleRequest">The parsed UpdateDynamicSchedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomUpdateDynamicScheduleRequestParser">A delegate to parse custom UpdateDynamicSchedule requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out UpdateDynamicScheduleRequest?      UpdateDynamicScheduleRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<UpdateDynamicScheduleRequest>?  CustomUpdateDynamicScheduleRequestParser   = null)
        {

            try
            {

                UpdateDynamicScheduleRequest = null;

                #region ChargingProfileId    [mandatory]

                if (!JSON.ParseMandatory("chargingProfileId",
                                         "charging profile identification",
                                         ChargingProfile_Id.TryParse,
                                         out ChargingProfile_Id ChargingProfileId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ScheduleUpdate       [mandatory]

                if (!JSON.ParseMandatoryJSON("scheduleUpdate",
                                             "charging schedule update",
                                             ChargingScheduleUpdate.TryParse,
                                             out ChargingScheduleUpdate? ScheduleUpdate,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                UpdateDynamicScheduleRequest = new UpdateDynamicScheduleRequest(

                                                   Destination,
                                                   ChargingProfileId,
                                                   ScheduleUpdate,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData,

                                                   RequestId,
                                                   RequestTimestamp,
                                                   RequestTimeout,
                                                   EventTrackingId,
                                                   NetworkPath

                                               );

                if (CustomUpdateDynamicScheduleRequestParser is not null)
                    UpdateDynamicScheduleRequest = CustomUpdateDynamicScheduleRequestParser(JSON,
                                                                                            UpdateDynamicScheduleRequest);

                return true;

            }
            catch (Exception e)
            {
                UpdateDynamicScheduleRequest  = null;
                ErrorResponse                 = "The given JSON representation of an UpdateDynamicSchedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateDynamicScheduleRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateDynamicScheduleRequestSerializer">A delegate to serialize custom UpdateDynamicSchedule requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                         IncludeJSONLDContext                           = false,
                              CustomJObjectSerializerDelegate<UpdateDynamicScheduleRequest>?  CustomUpdateDynamicScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingScheduleUpdate>?        CustomChargingScheduleUpdateSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",            DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("chargingProfileId",   ChargingProfileId.   ToString()),
                                 new JProperty("scheduleUpdate",      ScheduleUpdate.      ToJSON(CustomChargingScheduleUpdateSerializer,
                                                                                                  CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomUpdateDynamicScheduleRequestSerializer is not null
                       ? CustomUpdateDynamicScheduleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UpdateDynamicScheduleRequest1, UpdateDynamicScheduleRequest2)

        /// <summary>
        /// Compares two UpdateDynamicSchedule requests for equality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleRequest1">An UpdateDynamicSchedule request.</param>
        /// <param name="UpdateDynamicScheduleRequest2">Another UpdateDynamicSchedule request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest1,
                                           UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateDynamicScheduleRequest1, UpdateDynamicScheduleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateDynamicScheduleRequest1 is null || UpdateDynamicScheduleRequest2 is null)
                return false;

            return UpdateDynamicScheduleRequest1.Equals(UpdateDynamicScheduleRequest2);

        }

        #endregion

        #region Operator != (UpdateDynamicScheduleRequest1, UpdateDynamicScheduleRequest2)

        /// <summary>
        /// Compares two UpdateDynamicSchedule requests for inequality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleRequest1">An UpdateDynamicSchedule request.</param>
        /// <param name="UpdateDynamicScheduleRequest2">Another UpdateDynamicSchedule request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest1,
                                           UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest2)

            => !(UpdateDynamicScheduleRequest1 == UpdateDynamicScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateDynamicScheduleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateDynamicSchedule requests for equality.
        /// </summary>
        /// <param name="Object">An UpdateDynamicSchedule request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateDynamicScheduleRequest updateDynamicScheduleRequest &&
                   Equals(updateDynamicScheduleRequest);

        #endregion

        #region Equals(UpdateDynamicScheduleRequest)

        /// <summary>
        /// Compares two UpdateDynamicSchedule requests for equality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleRequest">An UpdateDynamicSchedule request to compare with.</param>
        public override Boolean Equals(UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest)

            => UpdateDynamicScheduleRequest is not null &&

               ChargingProfileId.Equals(UpdateDynamicScheduleRequest.ChargingProfileId) &&
               ScheduleUpdate.   Equals(UpdateDynamicScheduleRequest.ScheduleUpdate)    &&

               base.GenericEquals(UpdateDynamicScheduleRequest);

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

            => $"{ChargingProfileId}: {ScheduleUpdate}";

        #endregion

    }

}
