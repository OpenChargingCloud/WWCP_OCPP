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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;
using cloud.charging.open.protocols.OCPPv2_0;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CP
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
        public DateTime               Timestamp          { get; }

        /// <summary>
        /// The current status of the connector.
        /// </summary>
        public ConnectorStatus        ConnectorStatus    { get; }

        /// <summary>
        /// The identification of the EVSE to which the connector belongs for which the the status is reported.
        /// </summary>
        public EVSE_Id                EVSEId             { get; }

        /// <summary>
        /// The identification of the connector within the EVSE for which the status is reported.
        /// </summary>
        public Connector_Id           ConnectorId        { get; }

        /// <summary>
        /// The custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData             CustomData         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a status notification request.
        /// </summary>
        /// <param name="Timestamp">The time for which the status is reported.</param>
        /// <param name="ConnectorStatus">The current status of the connector.</param>
        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public StatusNotificationRequest(DateTime         Timestamp,
                                         ConnectorStatus  ConnectorStatus,
                                         EVSE_Id          EVSEId,
                                         Connector_Id     ConnectorId,
                                         CustomData       CustomData   = null)
        {

            this.Timestamp        = Timestamp;
            this.ConnectorStatus  = ConnectorStatus;
            this.EVSEId           = EVSEId;
            this.ConnectorId      = ConnectorId;
            this.CustomData       = CustomData;

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
        // }
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

        #region (static) Parse   (StatusNotificationRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a status notification request.
        /// </summary>
        /// <param name="StatusNotificationRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationRequest Parse(JObject              StatusNotificationRequestJSON,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(StatusNotificationRequestJSON,
                         out StatusNotificationRequest statusNotificationRequest,
                         OnException))
            {
                return statusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (StatusNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a status notification request.
        /// </summary>
        /// <param name="StatusNotificationRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationRequest Parse(String               StatusNotificationRequestText,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(StatusNotificationRequestText,
                         out StatusNotificationRequest statusNotificationRequest,
                         OnException))
            {
                return statusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(StatusNotificationRequestJSON, out StatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusNotificationRequest">The parsed status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       out StatusNotificationRequest  StatusNotificationRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                StatusNotificationRequest = null;

                #region Timestamp

                if (!JSON.ParseMandatory("timestamp",
                                                                  "timestamp",
                                                                  out DateTime  Timestamp,
                                                                  out String    ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorStatus

                if (!JSON.MapMandatory("connectorStatus",
                                                                "connector status",
                                                                ConnectorStatusExtentions.Parse,
                                                                out ConnectorStatus  ConnectorStatus,
                                                                out                  ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId

                if (!JSON.ParseMandatory("EVSEId",
                                                                  "EVSE identification",
                                                                  EVSE_Id.TryParse,
                                                                  out EVSE_Id  EVSEId,
                                                                  out          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId

                if (!JSON.ParseMandatory("connectorId",
                                                                  "connector identification",
                                                                  Connector_Id.TryParse,
                                                                  out Connector_Id  ConnectorId,
                                                                  out               ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData  CustomData,
                                           out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                StatusNotificationRequest = new StatusNotificationRequest(Timestamp,
                                                                          ConnectorStatus,
                                                                          EVSEId,
                                                                          ConnectorId,
                                                                          CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, JSON, e);

                StatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StatusNotificationRequestText, out StatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a status notification request.
        /// </summary>
        /// <param name="StatusNotificationRequestText">The text to be parsed.</param>
        /// <param name="StatusNotificationRequest">The parsed status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                         StatusNotificationRequestText,
                                       out StatusNotificationRequest  StatusNotificationRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                StatusNotificationRequestText = StatusNotificationRequestText?.Trim();

                if (StatusNotificationRequestText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(StatusNotificationRequestText),
                                           out StatusNotificationRequest,
                                           OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, StatusNotificationRequestText, e);
            }

            StatusNotificationRequest = null;
            return false;

        }

        #endregion

        #region ToJSON(CustomStatusNotificationRequestSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationRequestSerializer">A delegate to serialize custom StatusNotification requests.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationRequest> CustomStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>                CustomCustomDataResponseSerializer          = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("timestamp",         Timestamp.      ToIso8601()),
                           new JProperty("connectorStatus",   ConnectorStatus.AsText()),
                           new JProperty("evseId",            EVSEId.         ToString()),
                           new JProperty("connectorId",       ConnectorId.    ToString()),

                           CustomData != null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null);

            return CustomStatusNotificationRequestSerializer != null
                       ? CustomStatusNotificationRequestSerializer(this, JSON)
                       : JSON;

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
        public static Boolean operator == (StatusNotificationRequest StatusNotificationRequest1, StatusNotificationRequest StatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusNotificationRequest1, StatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((StatusNotificationRequest1 is null) || (StatusNotificationRequest2 is null))
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
        public static Boolean operator != (StatusNotificationRequest StatusNotificationRequest1, StatusNotificationRequest StatusNotificationRequest2)

            => !(StatusNotificationRequest1 == StatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is StatusNotificationRequest StatusNotificationRequest))
                return false;

            return Equals(StatusNotificationRequest);

        }

        #endregion

        #region Equals(StatusNotificationRequest)

        /// <summary>
        /// Compares two status notification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest">A status notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(StatusNotificationRequest StatusNotificationRequest)
        {

            if (StatusNotificationRequest is null)
                return false;

            return Timestamp.      Equals(StatusNotificationRequest.Timestamp)       &&
                   ConnectorStatus.Equals(StatusNotificationRequest.ConnectorStatus) &&
                   EVSEId.         Equals(StatusNotificationRequest.EVSEId)          &&
                   ConnectorId.    Equals(StatusNotificationRequest.ConnectorId)     &&

                   ((CustomData == null && StatusNotificationRequest.CustomData == null) ||
                    (CustomData != null && StatusNotificationRequest.CustomData != null && CustomData.Equals(StatusNotificationRequest.CustomData)));

        }

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

                       (CustomData != null
                            ? CustomData.GetHashCode()
                            : 0);

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
