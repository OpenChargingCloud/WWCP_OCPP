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

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// The set monitoring level request.
    /// </summary>
    public class SetMonitoringLevelRequest : ARequest<SetMonitoringLevelRequest>
    {

        #region Properties

        /// <summary>
        /// The charging station SHALL only report events with a severity number lower than or equal to this severity.
        /// The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.
        /// </summary>
        [Mandatory]
        public Severities  Severity    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set monitoring level request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Severity">The charging station SHALL only report events with a severity number lower than or equal to this severity.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetMonitoringLevelRequest(ChargeBox_Id        ChargeBoxId,
                                         Severities          Severity,
                                         CustomData?         CustomData          = null,

                                         Request_Id?         RequestId           = null,
                                         DateTime?           RequestTimestamp    = null,
                                         TimeSpan?           RequestTimeout      = null,
                                         EventTracking_Id?   EventTrackingId     = null,
                                         CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "SetMonitoringLevel",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.Severity = Severity;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetMonitoringLevelRequest",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "severity": {
        //       "description": "The Charging Station SHALL only report events with a severity number lower than or equal to this severity.\r\nThe severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.\r\n\r\n\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "severity"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSetMonitoringLevelRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set monitoring level request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSetMonitoringLevelRequestParser">A delegate to parse custom set monitoring level requests.</param>
        public static SetMonitoringLevelRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      ChargeBox_Id                                             ChargeBoxId,
                                                      CustomJObjectParserDelegate<SetMonitoringLevelRequest>?  CustomSetMonitoringLevelRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var setMonitoringLevelRequest,
                         out var errorResponse,
                         CustomSetMonitoringLevelRequestParser))
            {
                return setMonitoringLevelRequest!;
            }

            throw new ArgumentException("The given JSON representation of a set monitoring level request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SetMonitoringLevelRequest, out ErrorResponse, CustomBootNotificationResponseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a set monitoring level request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetMonitoringLevelRequest">The parsed set monitoring level request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         JSON,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out SetMonitoringLevelRequest?  SetMonitoringLevelRequest,
                                       out String?                     ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SetMonitoringLevelRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a set monitoring level request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetMonitoringLevelRequest">The parsed set monitoring level request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetMonitoringLevelRequestParser">A delegate to parse custom set monitoring level requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       ChargeBox_Id                                             ChargeBoxId,
                                       out SetMonitoringLevelRequest?                           SetMonitoringLevelRequest,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<SetMonitoringLevelRequest>?  CustomSetMonitoringLevelRequestParser)
        {

            try
            {

                SetMonitoringLevelRequest = null;

                #region Severity       [mandatory]

                if (!JSON.ParseMandatory("severity",
                                         "severity",
                                         out Byte severity,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Severity = SeveritiesExtensions.TryParse(severity);

                if (!Severity.HasValue)
                    return false;

                #endregion

                #region CustomData     [optional]

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

                #region ChargeBoxId    [optional, OCPP_CSE]

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


                SetMonitoringLevelRequest = new SetMonitoringLevelRequest(ChargeBoxId,
                                                                          Severity.Value,
                                                                          CustomData,
                                                                          RequestId);

                if (CustomSetMonitoringLevelRequestParser is not null)
                    SetMonitoringLevelRequest = CustomSetMonitoringLevelRequestParser(JSON,
                                                                                      SetMonitoringLevelRequest);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringLevelRequest  = null;
                ErrorResponse              = "The given JSON representation of a set monitoring level request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringLevelRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringLevelRequestSerializer">A delegate to serialize custom set monitoring level requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetMonitoringLevelRequest>?  CustomSetMonitoringLevelRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("severity",    Severity.  AsNumber()),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetMonitoringLevelRequestSerializer is not null
                       ? CustomSetMonitoringLevelRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringLevelRequest1, SetMonitoringLevelRequest2)

        /// <summary>
        /// Compares two set monitoring level requests for equality.
        /// </summary>
        /// <param name="SetMonitoringLevelRequest1">A set monitoring level request.</param>
        /// <param name="SetMonitoringLevelRequest2">Another set monitoring level request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetMonitoringLevelRequest? SetMonitoringLevelRequest1,
                                           SetMonitoringLevelRequest? SetMonitoringLevelRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringLevelRequest1, SetMonitoringLevelRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringLevelRequest1 is null || SetMonitoringLevelRequest2 is null)
                return false;

            return SetMonitoringLevelRequest1.Equals(SetMonitoringLevelRequest2);

        }

        #endregion

        #region Operator != (SetMonitoringLevelRequest1, SetMonitoringLevelRequest2)

        /// <summary>
        /// Compares two set monitoring level requests for inequality.
        /// </summary>
        /// <param name="SetMonitoringLevelRequest1">A set monitoring level request.</param>
        /// <param name="SetMonitoringLevelRequest2">Another set monitoring level request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetMonitoringLevelRequest? SetMonitoringLevelRequest1,
                                           SetMonitoringLevelRequest? SetMonitoringLevelRequest2)

            => !(SetMonitoringLevelRequest1 == SetMonitoringLevelRequest2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringLevelRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set monitoring level requests for equality.
        /// </summary>
        /// <param name="Object">A set monitoring level request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringLevelRequest setMonitoringLevelRequest &&
                   Equals(setMonitoringLevelRequest);

        #endregion

        #region Equals(SetMonitoringLevelRequest)

        /// <summary>
        /// Compares two set monitoring level requests for equality.
        /// </summary>
        /// <param name="SetMonitoringLevelRequest">A set monitoring level request to compare with.</param>
        public override Boolean Equals(SetMonitoringLevelRequest? SetMonitoringLevelRequest)

            => SetMonitoringLevelRequest is not null &&

               Severity.   Equals(SetMonitoringLevelRequest.Severity) &&

               base.GenericEquals(SetMonitoringLevelRequest);

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

                return Severity.GetHashCode() * 3 ^
                       base.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Severity.AsText();

        #endregion

    }

}
