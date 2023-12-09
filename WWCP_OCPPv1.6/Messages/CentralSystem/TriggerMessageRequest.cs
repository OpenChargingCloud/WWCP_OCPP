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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The trigger message request.
    /// </summary>
    public class TriggerMessageRequest : ARequest<TriggerMessageRequest>
    {

        #region Properties

        /// <summary>
        /// The message to trigger.
        /// </summary>
        public MessageTriggers  RequestedMessage    { get; }

        /// <summary>
        /// Optional connector identification whenever the message
        /// applies to a specific connector.
        /// </summary>
        public Connector_Id?    ConnectorId         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new trigger message request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public TriggerMessageRequest(NetworkingNode_Id  NetworkingNodeId,
                                     MessageTriggers    RequestedMessage,
                                     Connector_Id?      ConnectorId         = null,

                                     Request_Id?        RequestId           = null,
                                     DateTime?          RequestTimestamp    = null,
                                     TimeSpan?          RequestTimeout      = null,
                                     EventTracking_Id?  EventTrackingId     = null,
                                     CancellationToken  CancellationToken   = default)

            : base(NetworkingNodeId,
                   "TriggerMessage",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.RequestedMessage  = RequestedMessage;
            this.ConnectorId       = ConnectorId;

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:triggerMessageRequest>
        //
        //          <ns:requestedMessage>?</ns:requestedMessage>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //       </ns:triggerMessageRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:TriggerMessageRequest",
        //     "title":   "TriggerMessageRequest",
        //     "type":    "object",
        //     "properties": {
        //         "requestedMessage": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "BootNotification",
        //                 "DiagnosticsStatusNotification",
        //                 "FirmwareStatusNotification",
        //                 "Heartbeat",
        //                 "MeterValues",
        //                 "StatusNotification"
        //             ]
        //         },
        //         "connectorId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "requestedMessage"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of a trigger message request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static TriggerMessageRequest Parse(XElement      XML,
                                                  Request_Id    RequestId,
                                                  ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var triggerMessageRequest,
                         out var errorResponse))
            {
                return triggerMessageRequest!;
            }

            throw new ArgumentException("The given XML representation of a trigger message request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomTriggerMessageRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomTriggerMessageRequestParser">A delegate to parse custom trigger message requests.</param>
        public static TriggerMessageRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  ChargeBox_Id                                         ChargeBoxId,
                                                  CustomJObjectParserDelegate<TriggerMessageRequest>?  CustomTriggerMessageRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var triggerMessageRequest,
                         out var errorResponse,
                         CustomTriggerMessageRequestParser))
            {
                return triggerMessageRequest!;
            }

            throw new ArgumentException("The given JSON representation of a trigger message request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out TriggerMessageRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a trigger message request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                    XML,
                                       Request_Id                  RequestId,
                                       ChargeBox_Id                ChargeBoxId,
                                       out TriggerMessageRequest?  TriggerMessageRequest,
                                       out String?                 ErrorResponse)
        {

            try
            {

                TriggerMessageRequest = new TriggerMessageRequest(

                                            ChargeBoxId,

                                            XML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "requestedMessage",
                                                                    MessageTriggersExtensions.Parse),

                                            XML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                    Connector_Id.Parse),

                                            RequestId

                                        );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TriggerMessageRequest  = null;
                ErrorResponse          = "The given XML representation of a trigger message request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out TriggerMessageRequest, out ErrorResponse, CustomTriggerMessageRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                     JSON,
                                       Request_Id                  RequestId,
                                       ChargeBox_Id                ChargeBoxId,
                                       out TriggerMessageRequest?  TriggerMessageRequest,
                                       out String?                 ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out TriggerMessageRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTriggerMessageRequestParser">A delegate to parse custom trigger message requests.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       ChargeBox_Id                                         ChargeBoxId,
                                       out TriggerMessageRequest?                           TriggerMessageRequest,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<TriggerMessageRequest>?  CustomTriggerMessageRequestParser)
        {

            try
            {

                TriggerMessageRequest = null;

                #region MessageTriggers    [mandatory]

                if (!JSON.MapMandatory("requestedMessage",
                                       "requested message",
                                       MessageTriggersExtensions.Parse,
                                       out MessageTriggers MessageTriggers,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId        [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id ConnectorId,
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


                TriggerMessageRequest = new TriggerMessageRequest(ChargeBoxId,
                                                                  MessageTriggers,
                                                                  ConnectorId,
                                                                  RequestId);

                if (CustomTriggerMessageRequestParser is not null)
                    TriggerMessageRequest = CustomTriggerMessageRequestParser(JSON,
                                                                              TriggerMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                TriggerMessageRequest  = null;
                ErrorResponse          = "The given JSON representation of a trigger message request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "triggerMessageRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "requestedMessage",   RequestedMessage.AsText()),

                   ConnectorId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString())
                       : null

               );

        #endregion

        #region ToJSON(CustomTriggerMessageRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTriggerMessageRequestSerializer">A delegate to serialize custom trigger message requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageRequest>? CustomTriggerMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?        CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                           new JProperty("requestedMessage",   RequestedMessage.AsText()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",  ConnectorId.Value.Value)
                               : null

                       );

            return CustomTriggerMessageRequestSerializer is not null
                       ? CustomTriggerMessageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TriggerMessageRequest1, TriggerMessageRequest2)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest1">A trigger message request.</param>
        /// <param name="TriggerMessageRequest2">Another trigger message request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TriggerMessageRequest? TriggerMessageRequest1,
                                           TriggerMessageRequest? TriggerMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageRequest1, TriggerMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (TriggerMessageRequest1 is null || TriggerMessageRequest2 is null)
                return false;

            return TriggerMessageRequest1.Equals(TriggerMessageRequest2);

        }

        #endregion

        #region Operator != (TriggerMessageRequest1, TriggerMessageRequest2)

        /// <summary>
        /// Compares two trigger message requests for inequality.
        /// </summary>
        /// <param name="TriggerMessageRequest1">A trigger message request.</param>
        /// <param name="TriggerMessageRequest2">Another trigger message request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TriggerMessageRequest? TriggerMessageRequest1,
                                           TriggerMessageRequest? TriggerMessageRequest2)

            => !(TriggerMessageRequest1 == TriggerMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="Object">A trigger message request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TriggerMessageRequest triggerMessageRequest &&
                   Equals(triggerMessageRequest);

        #endregion

        #region Equals(TriggerMessageRequest)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest">A trigger message request to compare with.</param>
        public override Boolean Equals(TriggerMessageRequest? TriggerMessageRequest)

            => TriggerMessageRequest is not null &&

               RequestedMessage.Equals(TriggerMessageRequest.RequestedMessage) &&

            ((!ConnectorId.HasValue && !TriggerMessageRequest.ConnectorId.HasValue) ||
              (ConnectorId.HasValue &&  TriggerMessageRequest.ConnectorId.HasValue && ConnectorId.Equals(TriggerMessageRequest.ConnectorId))) &&

               base.     GenericEquals(TriggerMessageRequest);

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

                return RequestedMessage.GetHashCode()       * 5 ^
                      (ConnectorId?.    GetHashCode() ?? 0) * 3 ^

                       base.            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(RequestedMessage,

                             ConnectorId.HasValue
                                 ? " for " + ConnectorId
                                 : "");

        #endregion

    }

}
