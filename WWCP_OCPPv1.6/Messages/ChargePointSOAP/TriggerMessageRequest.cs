/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP.NS;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP trigger message request.
    /// </summary>
    public class TriggerMessageRequest
    {

        #region Properties

        /// <summary>
        /// The message to trigger.
        /// </summary>
        public MessageTriggers  RequestedMessage   { get; }

        /// <summary>
        /// Optional connector identification whenever the message
        /// applies to a specific connector.
        /// </summary>
        public Connector_Id?    ConnectorId        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP TriggerMessageRequest XML/SOAP request.
        /// </summary>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        public TriggerMessageRequest(MessageTriggers  RequestedMessage,
                                     Connector_Id?    ConnectorId)
        {

            this.RequestedMessage  = RequestedMessage;
            this.ConnectorId       = ConnectorId ?? new Connector_Id?();

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
        //       </ns:triggerMessageRequest>        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(TriggerMessageRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageRequest Parse(XElement             TriggerMessageRequestXML,
                                                  OnExceptionDelegate  OnException = null)
        {

            TriggerMessageRequest _TriggerMessageRequest;

            if (TryParse(TriggerMessageRequestXML, out _TriggerMessageRequest, OnException))
                return _TriggerMessageRequest;

            return null;

        }

        #endregion

        #region (static) Parse(TriggerMessageRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageRequest Parse(String               TriggerMessageRequestText,
                                                  OnExceptionDelegate  OnException = null)
        {

            TriggerMessageRequest _TriggerMessageRequest;

            if (TryParse(TriggerMessageRequestText, out _TriggerMessageRequest, OnException))
                return _TriggerMessageRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(TriggerMessageRequestXML,  out TriggerMessageRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestXML">The XML to parse.</param>
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
                                                                                         XML_IO.AsMessageTrigger),

                                            TriggerMessageRequestXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                         Connector_Id.Parse)

                                        );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, TriggerMessageRequestXML, e);

                TriggerMessageRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(TriggerMessageRequestText, out TriggerMessageRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP trigger message request.
        /// </summary>
        /// <param name="TriggerMessageRequestText">The text to parse.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                     TriggerMessageRequestText,
                                       out TriggerMessageRequest  TriggerMessageRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(TriggerMessageRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out TriggerMessageRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, TriggerMessageRequestText, e);
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

                   new XElement(OCPPNS.OCPPv1_6_CP + "requestedMessage",   XML_IO.AsText(RequestedMessage)),

                   ConnectorId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString())
                       : null

               );

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
            if (Object.ReferenceEquals(TriggerMessageRequest1, TriggerMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) TriggerMessageRequest1 == null) || ((Object) TriggerMessageRequest2 == null))
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

            if (Object == null)
                return false;

            // Check if the given object is a trigger message request.
            var TriggerMessageRequest = Object as TriggerMessageRequest;
            if ((Object) TriggerMessageRequest == null)
                return false;

            return this.Equals(TriggerMessageRequest);

        }

        #endregion

        #region Equals(TriggerMessageRequest)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest">A trigger message request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(TriggerMessageRequest TriggerMessageRequest)
        {

            if ((Object) TriggerMessageRequest == null)
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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(RequestedMessage,

                             ConnectorId.HasValue
                                 ? " for " + ConnectorId
                                 : "");

        #endregion


    }

}
