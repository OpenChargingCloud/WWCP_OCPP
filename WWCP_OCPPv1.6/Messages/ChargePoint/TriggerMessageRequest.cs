/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A trigger message request.
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
        /// Create a trigger message request.
        /// </summary>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        public TriggerMessageRequest(MessageTriggers  RequestedMessage,
                                     Connector_Id?    ConnectorId  = null)
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

        #region (static) Parse   (TriggerMessageRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageRequest Parse(XElement             TriggerMessageRequestXML,
                                                  OnExceptionDelegate  OnException = null)
        {

            if (TryParse(TriggerMessageRequestXML,
                         out TriggerMessageRequest triggerMessageRequest,
                         OnException))
            {
                return triggerMessageRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (TriggerMessageRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageRequest Parse(JObject              TriggerMessageRequestJSON,
                                                  OnExceptionDelegate  OnException = null)
        {

            if (TryParse(TriggerMessageRequestJSON,
                         out TriggerMessageRequest triggerMessageRequest,
                         OnException))
            {
                return triggerMessageRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (TriggerMessageRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageRequest Parse(String               TriggerMessageRequestText,
                                                  OnExceptionDelegate  OnException = null)
        {

            if (TryParse(TriggerMessageRequestText,
                         out TriggerMessageRequest triggerMessageRequest,
                         OnException))
            {
                return triggerMessageRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(TriggerMessageRequestXML,  out TriggerMessageRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestXML">The XML to be parsed.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                   TriggerMessageRequestXML,
                                       out TriggerMessageRequest  TriggerMessageRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                TriggerMessageRequest = new TriggerMessageRequest(

                                            TriggerMessageRequestXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "requestedMessage",
                                                                                         MessageTriggersExtentions.Parse),

                                            TriggerMessageRequestXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                         Connector_Id.Parse)

                                        );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, TriggerMessageRequestXML, e);

                TriggerMessageRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(TriggerMessageRequestJSON,  out TriggerMessageRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestJSON">The JSON to be parsed.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                    TriggerMessageRequestJSON,
                                       out TriggerMessageRequest  TriggerMessageRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                TriggerMessageRequest = null;

                #region MessageTriggers

                if (!TriggerMessageRequestJSON.MapMandatory("requestedMessage",
                                                            "requested message",
                                                            MessageTriggersExtentions.Parse,
                                                            out MessageTriggers  MessageTriggers,
                                                            out String           ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region ConnectorId

                if (TriggerMessageRequestJSON.ParseOptional("connectorId",
                                                            "connector identification",
                                                            Connector_Id.TryParse,
                                                            out Connector_Id  ConnectorId,
                                                            out               ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                TriggerMessageRequest = new TriggerMessageRequest(MessageTriggers,
                                                                  ConnectorId);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, TriggerMessageRequestJSON, e);

                TriggerMessageRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(TriggerMessageRequestText, out TriggerMessageRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestText">The text to be parsed.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                     TriggerMessageRequestText,
                                       out TriggerMessageRequest  TriggerMessageRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                TriggerMessageRequestText = TriggerMessageRequestText?.Trim();

                if (TriggerMessageRequestText.IsNotNullOrEmpty())
                {

                    if (TriggerMessageRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(TriggerMessageRequestText),
                                 out TriggerMessageRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(TriggerMessageRequestText).Root,
                                 out TriggerMessageRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, TriggerMessageRequestText, e);
            }

            TriggerMessageRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "triggerMessageRequest",

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
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageRequest> CustomTriggerMessageRequestSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("requestedMessage",   RequestedMessage.AsText()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",  ConnectorId.Value.ToString())
                               : null

                       );

            return CustomTriggerMessageRequestSerializer != null
                       ? CustomTriggerMessageRequestSerializer(this, JSON)
                       : JSON;

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
        public static Boolean operator == (TriggerMessageRequest TriggerMessageRequest1, TriggerMessageRequest TriggerMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageRequest1, TriggerMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((TriggerMessageRequest1 is null) || (TriggerMessageRequest2 is null))
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
        public static Boolean operator != (TriggerMessageRequest TriggerMessageRequest1, TriggerMessageRequest TriggerMessageRequest2)

            => !(TriggerMessageRequest1 == TriggerMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageRequest> Members

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

            if (!(Object is TriggerMessageRequest TriggerMessageRequest))
                return false;

            return Equals(TriggerMessageRequest);

        }

        #endregion

        #region Equals(TriggerMessageRequest)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest">A trigger message request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(TriggerMessageRequest TriggerMessageRequest)
        {

            if (TriggerMessageRequest is null)
                return false;

            return RequestedMessage.Equals(TriggerMessageRequest.RequestedMessage) &&

                   (!ConnectorId.HasValue && !TriggerMessageRequest.ConnectorId.HasValue) ||
                    (ConnectorId.HasValue &&  TriggerMessageRequest.ConnectorId.HasValue && ConnectorId.Equals(TriggerMessageRequest.ConnectorId));

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

                return RequestedMessage.GetHashCode() * 5 ^

                       (ConnectorId.HasValue
                            ? ConnectorId.GetHashCode()
                            : 0);

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
