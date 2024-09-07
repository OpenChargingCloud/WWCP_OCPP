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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The StatusNotification request.
    /// </summary>
    public class StatusNotificationRequest : ARequest<StatusNotificationRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/statusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

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
        /// Create a StatusNotification request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Timestamp">The time for which the status is reported.</param>
        /// <param name="ConnectorStatus">The current status of the connector.</param>
        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public StatusNotificationRequest(SourceRouting            Destination,
                                         DateTime                 Timestamp,
                                         ConnectorStatus          ConnectorStatus,
                                         EVSE_Id                  EVSEId,
                                         Connector_Id             ConnectorId,

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
                   nameof(StatusNotificationRequest)[..^7],

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

            this.Timestamp        = Timestamp;
            this.ConnectorStatus  = ConnectorStatus;
            this.EVSEId           = EVSEId;
            this.ConnectorId      = ConnectorId;


            unchecked
            {

                hashCode = this.Timestamp.      GetHashCode() * 11 ^
                           this.ConnectorStatus.GetHashCode() *  7 ^
                           this.EVSEId.         GetHashCode() *  5 ^
                           this.ConnectorId.    GetHashCode() *  3 ^
                           base.                GetHashCode();

            }

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
        //       "description": "This contains the current status of the Connector.",
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
        //       "description": "The time for which the status is reported. If absent time of receipt of the message will be assumed.",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "connectorStatus": {
        //       "$ref": "#/definitions/ConnectorStatusEnumType"
        //     },
        //     "evseId": {
        //       "description": "The id of the EVSE to which the connector belongs for which the the status is reported.",
        //       "type": "integer"
        //     },
        //     "connectorId": {
        //       "description": "The id of the connector within the EVSE for which the status is reported.",
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

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a StatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomStatusNotificationRequestParser">A delegate to parse custom StatusNotification requests.</param>
        public static StatusNotificationRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                        Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var statusNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomStatusNotificationRequestParser))
            {
                return statusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a StatusNotification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out StatusNotificationRequest, out ErrorResponse, CustomStatusNotificationRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a StatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="StatusNotificationRequest">The parsed StatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomStatusNotificationRequestParser">A delegate to parse custom StatusNotification requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out StatusNotificationRequest?      StatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestParser   = null)
        {

            try
            {

                StatusNotificationRequest = null;

                #region Timestamp            [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorStatus      [mandatory]

                if (!JSON.ParseMandatory("connectorStatus",
                                         "connector status",
                                         OCPPv2_1.ConnectorStatus.TryParse,
                                         out ConnectorStatus ConnectorStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

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

                #region ConnectorId          [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
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


                StatusNotificationRequest = new StatusNotificationRequest(

                                                Destination,
                                                Timestamp,
                                                ConnectorStatus,
                                                EVSEId,
                                                ConnectorId,

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

                if (CustomStatusNotificationRequestParser is not null)
                    StatusNotificationRequest = CustomStatusNotificationRequestParser(JSON,
                                                                                      StatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                StatusNotificationRequest  = null;
                ErrorResponse              = "The given JSON representation of a StatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationRequestSerializer">A delegate to serialize custom StatusNotification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timestamp",         Timestamp.      ToIso8601()),
                                 new JProperty("connectorStatus",   ConnectorStatus.ToString()),
                                 new JProperty("evseId",            EVSEId.         Value),
                                 new JProperty("connectorId",       ConnectorId.    Value),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

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
        /// Compares two StatusNotification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A StatusNotification request.</param>
        /// <param name="StatusNotificationRequest2">Another StatusNotification request.</param>
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
        /// Compares two StatusNotification requests for inequality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A StatusNotification request.</param>
        /// <param name="StatusNotificationRequest2">Another StatusNotification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationRequest? StatusNotificationRequest1,
                                           StatusNotificationRequest? StatusNotificationRequest2)

            => !(StatusNotificationRequest1 == StatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two StatusNotification requests for equality.
        /// </summary>
        /// <param name="Object">A StatusNotification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusNotificationRequest statusNotificationRequest &&
                   Equals(statusNotificationRequest);

        #endregion

        #region Equals(StatusNotificationRequest)

        /// <summary>
        /// Compares two StatusNotification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest">A StatusNotification request to compare with.</param>
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

            => $"{Timestamp}: {EVSEId} / {ConnectorId} => {ConnectorStatus}";

        #endregion

    }

}
