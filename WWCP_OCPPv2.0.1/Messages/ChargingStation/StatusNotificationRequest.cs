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
    /// A status notification request.
    /// </summary>
    public class StatusNotificationRequest : ARequest<StatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The time for which the status is reported.
        /// </summary>
        [Mandatory]
        public DateTime         Timestamp          { get; }

        /// <summary>
        /// The current status of the connector.
        /// </summary>
        [Mandatory]
        public ConnectorStatus  ConnectorStatus    { get; }

        /// <summary>
        /// The identification of the EVSE to which the connector belongs for which the the status is reported.
        /// </summary>
        [Mandatory]
        public EVSE_Id          EVSEId             { get; }

        /// <summary>
        /// The identification of the connector within the EVSE for which the status is reported.
        /// </summary>
        [Mandatory]
        public Connector_Id     ConnectorId        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a status notification request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Timestamp">The time for which the status is reported.</param>
        /// <param name="ConnectorStatus">The current status of the connector.</param>
        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public StatusNotificationRequest(ChargeBox_Id       ChargeBoxId,
                                         DateTime           Timestamp,
                                         ConnectorStatus    ConnectorStatus,
                                         EVSE_Id            EVSEId,
                                         Connector_Id       ConnectorId,
                                         CustomData?        CustomData          = null,

                                         Request_Id?        RequestId           = null,
                                         DateTime?          RequestTimestamp    = null,
                                         TimeSpan?          RequestTimeout      = null,
                                         EventTracking_Id?  EventTrackingId     = null,
                                         CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "StatusNotification",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.Timestamp        = Timestamp;
            this.ConnectorStatus  = ConnectorStatus;
            this.EVSEId           = EVSEId;
            this.ConnectorId      = ConnectorId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:StatusNotificationRequest",
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
        //     "ConnectorStatusEnumType": {
        //       "description": "This contains the current status of the Connector.\r\n",
        //       "javaType": "ConnectorStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Available",
        //         "Occupied",
        //         "Reserved",
        //         "Unavailable",
        //         "Faulted"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "timestamp": {
        //       "description": "The time for which the status is reported. If absent time of receipt of the message will be assumed.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "connectorStatus": {
        //       "$ref": "#/definitions/ConnectorStatusEnumType"
        //     },
        //     "evseId": {
        //       "description": "The id of the EVSE to which the connector belongs for which the the status is reported.\r\n",
        //       "type": "integer"
        //     },
        //     "connectorId": {
        //       "description": "The id of the connector within the EVSE for which the status is reported.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "timestamp",
        //     "connectorStatus",
        //     "evseId",
        //     "connectorId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomStatusNotificationRequestParser">An optional delegate to parse custom status notification requests.</param>
        public static StatusNotificationRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      ChargeBox_Id                                             ChargeBoxId,
                                                      CustomJObjectParserDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var statusNotificationRequest,
                         out var errorResponse,
                         CustomStatusNotificationRequestParser))
            {
                return statusNotificationRequest!;
            }

            throw new ArgumentException("The given JSON representation of a status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out StatusNotificationRequest, out ErrorResponse, CustomStatusNotificationRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StatusNotificationRequest">The parsed status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusNotificationRequestParser">An optional delegate to parse custom status notification requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       ChargeBox_Id                                             ChargeBoxId,
                                       out StatusNotificationRequest?                           StatusNotificationRequest,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestParser)
        {

            try
            {

                StatusNotificationRequest = null;

                #region Timestamp          [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorStatus    [mandatory]

                if (!JSON.ParseMandatory("connectorStatus",
                                         "connector status",
                                         ConnectorStatusExtensions.TryParse,
                                         out ConnectorStatus ConnectorStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId             [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId        [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData         [optional]

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

                #region ChargeBoxId        [optional, OCPP_CSE]

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


                StatusNotificationRequest = new StatusNotificationRequest(
                                                ChargeBoxId,
                                                Timestamp,
                                                ConnectorStatus,
                                                EVSEId,
                                                ConnectorId,
                                                CustomData,
                                                RequestId
                                            );

                if (CustomStatusNotificationRequestParser is not null)
                    StatusNotificationRequest = CustomStatusNotificationRequestParser(JSON,
                                                                                      StatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                StatusNotificationRequest  = null;
                ErrorResponse              = "The given JSON representation of a status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusNotificationRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationRequestSerializer">A delegate to serialize custom StatusNotification requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timestamp",         Timestamp.      ToIso8601()),
                                 new JProperty("connectorStatus",   ConnectorStatus.AsText()),
                                 new JProperty("evseId",            EVSEId.         Value),
                                 new JProperty("connectorId",       ConnectorId.    Value),

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomStatusNotificationRequestSerializer is not null
                       ? CustomStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StatusNotificationRequest1, StatusNotificationRequest2)

        /// <summary>
        /// Compares two status notification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A status notification request.</param>
        /// <param name="StatusNotificationRequest2">Another status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StatusNotificationRequest? StatusNotificationRequest1,
                                           StatusNotificationRequest? StatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusNotificationRequest1, StatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (StatusNotificationRequest1 is null || StatusNotificationRequest2 is null)
                return false;

            return StatusNotificationRequest1.Equals(StatusNotificationRequest2);

        }

        #endregion

        #region Operator != (StatusNotificationRequest1, StatusNotificationRequest2)

        /// <summary>
        /// Compares two status notification requests for inequality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A status notification request.</param>
        /// <param name="StatusNotificationRequest2">Another status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationRequest? StatusNotificationRequest1,
                                           StatusNotificationRequest? StatusNotificationRequest2)

            => !(StatusNotificationRequest1 == StatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two status notification requests for equality.
        /// </summary>
        /// <param name="Object">A status notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusNotificationRequest statusNotificationRequest &&
                   Equals(statusNotificationRequest);

        #endregion

        #region Equals(StatusNotificationRequest)

        /// <summary>
        /// Compares two status notification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest">A status notification request to compare with.</param>
        public override Boolean Equals(StatusNotificationRequest? StatusNotificationRequest)

            => StatusNotificationRequest is not null &&

               Timestamp.      Equals(StatusNotificationRequest.Timestamp)       &&
               ConnectorStatus.Equals(StatusNotificationRequest.ConnectorStatus) &&
               EVSEId.         Equals(StatusNotificationRequest.EVSEId)          &&
               ConnectorId.    Equals(StatusNotificationRequest.ConnectorId)     &&

               base.    GenericEquals(StatusNotificationRequest);

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

                return Timestamp.      GetHashCode() * 11 ^
                       ConnectorStatus.GetHashCode() *  7 ^
                       EVSEId.         GetHashCode() *  5 ^
                       ConnectorId.    GetHashCode() *  3 ^

                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Timestamp,
                             EVSEId, "-", ConnectorId,
                             " => ", ConnectorStatus);

        #endregion

    }

}
