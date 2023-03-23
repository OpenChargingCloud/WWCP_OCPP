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
    /// A notify EV charging needs request.
    /// </summary>
    public class NotifyEVChargingNeedsRequest : ARequest<NotifyEVChargingNeedsRequest>
    {

        #region Properties

        /// <summary>
        /// The EVSE and connector to which the EV is connected to.
        /// </summary>
        [Mandatory]
        public EVSE_Id        EVSEId               { get; }

        /// <summary>
        /// The characteristics of the energy delivery required.
        /// </summary>
        [Mandatory]
        public ChargingNeeds  ChargingNeeds        { get; }

        /// <summary>
        /// The optional maximum schedule tuples the car supports per schedule.
        /// </summary>
        [Optional]
        public UInt16?        MaxScheduleTuples    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify EV charging needs request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="MaxScheduleTuples">The optional maximum schedule tuples the car supports per schedule.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyEVChargingNeedsRequest(ChargeBox_Id        ChargeBoxId,
                                            EVSE_Id             EVSEId,
                                            ChargingNeeds       ChargingNeeds,
                                            UInt16?             MaxScheduleTuples   = null,
                                            CustomData?         CustomData          = null,

                                            Request_Id?         RequestId           = null,
                                            DateTime?           RequestTimestamp    = null,
                                            TimeSpan?           RequestTimeout      = null,
                                            EventTracking_Id?   EventTrackingId     = null,
                                            CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "NotifyEVChargingNeeds",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.EVSEId             = EVSEId;
            this.ChargingNeeds      = ChargingNeeds;
            this.MaxScheduleTuples  = MaxScheduleTuples;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyEVChargingNeedsRequest",
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
        //     "EnergyTransferModeEnumType": {
        //       "description": "Charging_ Needs. Requested. Energy_ Transfer_ Mode_ Code\r\nurn:x-oca:ocpp:uid:1:569209\r\nMode of energy transfer requested by the EV.\r\n",
        //       "javaType": "EnergyTransferModeEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "DC",
        //         "AC_single_phase",
        //         "AC_two_phase",
        //         "AC_three_phase"
        //       ]
        //     },
        //     "ACChargingParametersType": {
        //       "description": "AC_ Charging_ Parameters\r\nurn:x-oca:ocpp:uid:2:233250\r\nEV AC charging parameters.\r\n\r\n",
        //       "javaType": "ACChargingParameters",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "energyAmount": {
        //           "description": "AC_ Charging_ Parameters. Energy_ Amount. Energy_ Amount\r\nurn:x-oca:ocpp:uid:1:569211\r\nAmount of energy requested (in Wh). This includes energy required for preconditioning.\r\n",
        //           "type": "integer"
        //         },
        //         "evMinCurrent": {
        //           "description": "AC_ Charging_ Parameters. EV_ Min. Current\r\nurn:x-oca:ocpp:uid:1:569212\r\nMinimum current (amps) supported by the electric vehicle (per phase).\r\n",
        //           "type": "integer"
        //         },
        //         "evMaxCurrent": {
        //           "description": "AC_ Charging_ Parameters. EV_ Max. Current\r\nurn:x-oca:ocpp:uid:1:569213\r\nMaximum current (amps) supported by the electric vehicle (per phase). Includes cable capacity.\r\n",
        //           "type": "integer"
        //         },
        //         "evMaxVoltage": {
        //           "description": "AC_ Charging_ Parameters. EV_ Max. Voltage\r\nurn:x-oca:ocpp:uid:1:569214\r\nMaximum voltage supported by the electric vehicle\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "energyAmount",
        //         "evMinCurrent",
        //         "evMaxCurrent",
        //         "evMaxVoltage"
        //       ]
        //     },
        //     "ChargingNeedsType": {
        //       "description": "Charging_ Needs\r\nurn:x-oca:ocpp:uid:2:233249\r\n",
        //       "javaType": "ChargingNeeds",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "acChargingParameters": {
        //           "$ref": "#/definitions/ACChargingParametersType"
        //         },
        //         "dcChargingParameters": {
        //           "$ref": "#/definitions/DCChargingParametersType"
        //         },
        //         "requestedEnergyTransfer": {
        //           "$ref": "#/definitions/EnergyTransferModeEnumType"
        //         },
        //         "departureTime": {
        //           "description": "Charging_ Needs. Departure_ Time. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569223\r\nEstimated departure time of the EV.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         }
        //       },
        //       "required": [
        //         "requestedEnergyTransfer"
        //       ]
        //     },
        //     "DCChargingParametersType": {
        //       "description": "DC_ Charging_ Parameters\r\nurn:x-oca:ocpp:uid:2:233251\r\nEV DC charging parameters\r\n\r\n\r\n",
        //       "javaType": "DCChargingParameters",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "evMaxCurrent": {
        //           "description": "DC_ Charging_ Parameters. EV_ Max. Current\r\nurn:x-oca:ocpp:uid:1:569215\r\nMaximum current (amps) supported by the electric vehicle. Includes cable capacity.\r\n",
        //           "type": "integer"
        //         },
        //         "evMaxVoltage": {
        //           "description": "DC_ Charging_ Parameters. EV_ Max. Voltage\r\nurn:x-oca:ocpp:uid:1:569216\r\nMaximum voltage supported by the electric vehicle\r\n",
        //           "type": "integer"
        //         },
        //         "energyAmount": {
        //           "description": "DC_ Charging_ Parameters. Energy_ Amount. Energy_ Amount\r\nurn:x-oca:ocpp:uid:1:569217\r\nAmount of energy requested (in Wh). This inludes energy required for preconditioning.\r\n",
        //           "type": "integer"
        //         },
        //         "evMaxPower": {
        //           "description": "DC_ Charging_ Parameters. EV_ Max. Power\r\nurn:x-oca:ocpp:uid:1:569218\r\nMaximum power (in W) supported by the electric vehicle. Required for DC charging.\r\n",
        //           "type": "integer"
        //         },
        //         "stateOfCharge": {
        //           "description": "DC_ Charging_ Parameters. State_ Of_ Charge. Numeric\r\nurn:x-oca:ocpp:uid:1:569219\r\nEnergy available in the battery (in percent of the battery capacity)\r\n",
        //           "type": "integer",
        //           "minimum": 0.0,
        //           "maximum": 100.0
        //         },
        //         "evEnergyCapacity": {
        //           "description": "DC_ Charging_ Parameters. EV_ Energy_ Capacity. Numeric\r\nurn:x-oca:ocpp:uid:1:569220\r\nCapacity of the electric vehicle battery (in Wh)\r\n",
        //           "type": "integer"
        //         },
        //         "fullSoC": {
        //           "description": "DC_ Charging_ Parameters. Full_ SOC. Percentage\r\nurn:x-oca:ocpp:uid:1:569221\r\nPercentage of SoC at which the EV considers the battery fully charged. (possible values: 0 - 100)\r\n",
        //           "type": "integer",
        //           "minimum": 0.0,
        //           "maximum": 100.0
        //         },
        //         "bulkSoC": {
        //           "description": "DC_ Charging_ Parameters. Bulk_ SOC. Percentage\r\nurn:x-oca:ocpp:uid:1:569222\r\nPercentage of SoC at which the EV considers a fast charging process to end. (possible values: 0 - 100)\r\n",
        //           "type": "integer",
        //           "minimum": 0.0,
        //           "maximum": 100.0
        //         }
        //       },
        //       "required": [
        //         "evMaxCurrent",
        //         "evMaxVoltage"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "maxScheduleTuples": {
        //       "description": "Contains the maximum schedule tuples the car supports per schedule.\r\n",
        //       "type": "integer"
        //     },
        //     "chargingNeeds": {
        //       "$ref": "#/definitions/ChargingNeedsType"
        //     },
        //     "evseId": {
        //       "description": "Defines the EVSE and connector to which the EV is connected. EvseId may not be 0.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "evseId",
        //     "chargingNeeds"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyEVChargingNeedsRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify EV charging needs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyEVChargingNeedsRequestParser">A delegate to parse custom notify EV charging needs requests.</param>
        public static NotifyEVChargingNeedsRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         ChargeBox_Id                                                ChargeBoxId,
                                                         CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var notifyDisplayMessagesRequest,
                         out var errorResponse,
                         CustomNotifyEVChargingNeedsRequestParser))
            {
                return notifyDisplayMessagesRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify EV charging needs request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyEVChargingNeedsRequest, out ErrorResponse, CustomNotifyEVChargingNeedsRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging needs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyEVChargingNeedsRequest">The parsed notify EV charging needs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingNeedsRequestParser">A delegate to parse custom notify EV charging needs requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       ChargeBox_Id                                                ChargeBoxId,
                                       out NotifyEVChargingNeedsRequest?                           NotifyEVChargingNeedsRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestParser)
        {

            try
            {

                NotifyEVChargingNeedsRequest = null;

                #region EVSEId               [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingNeeds        [mandatory]

                if (!JSON.ParseMandatoryJSON("messageInfo",
                                             "message infos",
                                             OCPPv2_0.ChargingNeeds.TryParse,
                                             out ChargingNeeds? ChargingNeeds,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (ChargingNeeds is null)
                    return false;

                #endregion

                #region MaxScheduleTuples    [optional]

                if (JSON.ParseOptional("maxScheduleTuples",
                                       "max schedule tuples",
                                       out UInt16? MaxScheduleTuples,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

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

                #region ChargeBoxId          [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                NotifyEVChargingNeedsRequest = new NotifyEVChargingNeedsRequest(ChargeBoxId,
                                                                                EVSEId,
                                                                                ChargingNeeds,
                                                                                MaxScheduleTuples,
                                                                                CustomData,
                                                                                RequestId);

                if (CustomNotifyEVChargingNeedsRequestParser is not null)
                    NotifyEVChargingNeedsRequest = CustomNotifyEVChargingNeedsRequestParser(JSON,
                                                                                            NotifyEVChargingNeedsRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingNeedsRequest  = null;
                ErrorResponse                 = "The given JSON representation of a notify EV charging needs request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingNeedsRequestSerializer = null, CustomChargingNeedsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingNeedsRequestSerializer">A delegate to serialize custom NotifyEVChargingNeeds requests.</param>
        /// <param name="CustomChargingNeedsSerializer">A delegate to serialize custom charging needss.</param>
        /// <param name="CustomACChargingParametersSerializer">A delegate to serialize custom AC charging parameters.</param>
        /// <param name="CustomDCChargingParametersSerializer">A delegate to serialize custom DC charging parameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingNeeds>?                 CustomChargingNeedsSerializer                  = null,
                              CustomJObjectSerializerDelegate<ACChargingParameters>?          CustomACChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<DCChargingParameters>?          CustomDCChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("evseId",              EVSEId.       ToString()),

                                 new JProperty("chargingNeeds",       ChargingNeeds.ToJSON(CustomChargingNeedsSerializer,
                                                                                           CustomACChargingParametersSerializer,
                                                                                           CustomDCChargingParametersSerializer,
                                                                                           CustomCustomDataSerializer)),

                           MaxScheduleTuples.HasValue
                               ? new JProperty("maxScheduleTuples",   MaxScheduleTuples.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyEVChargingNeedsRequestSerializer is not null
                       ? CustomNotifyEVChargingNeedsRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest1">A notify EV charging needs request.</param>
        /// <param name="NotifyEVChargingNeedsRequest2">Another notify EV charging needs request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest1,
                                           NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingNeedsRequest1 is null || NotifyEVChargingNeedsRequest2 is null)
                return false;

            return NotifyEVChargingNeedsRequest1.Equals(NotifyEVChargingNeedsRequest2);

        }

        #endregion

        #region Operator != (NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2)

        /// <summary>
        /// Compares two notify EV charging needs requests for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest1">A notify EV charging needs request.</param>
        /// <param name="NotifyEVChargingNeedsRequest2">Another notify EV charging needs request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest1,
                                           NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest2)

            => !(NotifyEVChargingNeedsRequest1 == NotifyEVChargingNeedsRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingNeedsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="Object">A notify EV charging needs request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingNeedsRequest notifyDisplayMessagesRequest &&
                   Equals(notifyDisplayMessagesRequest);

        #endregion

        #region Equals(NotifyEVChargingNeedsRequest)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest">A notify EV charging needs request to compare with.</param>
        public override Boolean Equals(NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest)

            => NotifyEVChargingNeedsRequest is not null &&

               EVSEId.       Equals(NotifyEVChargingNeedsRequest.EVSEId)        &&
               ChargingNeeds.Equals(NotifyEVChargingNeedsRequest.ChargingNeeds) &&

            ((!MaxScheduleTuples.HasValue && !NotifyEVChargingNeedsRequest.MaxScheduleTuples.HasValue) ||
               MaxScheduleTuples.HasValue &&  NotifyEVChargingNeedsRequest.MaxScheduleTuples.HasValue && MaxScheduleTuples.Value.Equals(NotifyEVChargingNeedsRequest.MaxScheduleTuples.Value)) &&

               base.GenericEquals(NotifyEVChargingNeedsRequest);

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

                return EVSEId.            GetHashCode()       * 7 ^
                       ChargingNeeds.     GetHashCode()       * 5 ^
                      (MaxScheduleTuples?.GetHashCode() ?? 0) * 3 ^

                       base.              GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   EVSEId,
                   ": ",
                   ChargingNeeds.ToString(),

                   MaxScheduleTuples.HasValue
                       ? ", max schedule tuples: " + MaxScheduleTuples
                       : ""
               );

        #endregion

    }

}
