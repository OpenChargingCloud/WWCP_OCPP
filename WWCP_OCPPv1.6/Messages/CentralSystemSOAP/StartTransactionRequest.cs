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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP start transaction request.
    /// </summary>
    public class StartTransactionRequest
    {

        #region Properties

        /// <summary>
        /// The connector identification at the charge point.
        /// </summary>
        public Connector_Id     ConnectorId      { get; }

        /// <summary>
        /// The identifier for which a transaction has to be started.
        /// </summary>
        public IdToken          IdTag            { get; }

        /// <summary>
        /// The timestamp of the transaction start.
        /// </summary>
        public DateTime         Timestamp        { get; }

        /// <summary>
        /// The energy meter value in Wh for the connector at start
        /// of the transaction.
        /// </summary>
        public UInt64           MeterStart       { get; }

        /// <summary>
        /// An optional identification of the reservation that will
        /// terminate as a result of this transaction.
        /// </summary>
        public Reservation_Id?  ReservationId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP StartTransaction XML/SOAP request.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="Timestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The energy meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        public StartTransactionRequest(Connector_Id     ConnectorId,
                                       IdToken          IdTag,
                                       DateTime         Timestamp,
                                       UInt64           MeterStart,
                                       Reservation_Id?  ReservationId = null)
        {

            this.ConnectorId    = ConnectorId;
            this.IdTag          = IdTag;
            this.Timestamp      = Timestamp;
            this.MeterStart     = MeterStart;
            this.ReservationId  = ReservationId;

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:startTransactionRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:idTag>?</ns:idTag>
        //          <ns:timestamp>?</ns:timestamp>
        //          <ns:meterStart>?</ns:meterStart>
        //
        //          <!--Optional:-->
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:startTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(StartTransactionRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionRequest Parse(XElement             StartTransactionRequestXML,
                                                    OnExceptionDelegate  OnException = null)
        {

            StartTransactionRequest _StartTransactionRequest;

            if (TryParse(StartTransactionRequestXML, out _StartTransactionRequest, OnException))
                return _StartTransactionRequest;

            return null;

        }

        #endregion

        #region (static) Parse(StartTransactionRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionRequest Parse(String               StartTransactionRequestText,
                                                    OnExceptionDelegate  OnException = null)
        {

            StartTransactionRequest _StartTransactionRequest;

            if (TryParse(StartTransactionRequestText, out _StartTransactionRequest, OnException))
                return _StartTransactionRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(StartTransactionRequestXML,  out StartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestXML">The XML to parse.</param>
        /// <param name="StartTransactionRequest">The parsed start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                     StartTransactionRequestXML,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                StartTransactionRequest = new StartTransactionRequest(

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                                            Connector_Id.Parse),

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                                            IdToken.Parse),

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                                            DateTime.Parse),

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "meterStart",
                                                                                            UInt64.Parse),

                                              StartTransactionRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "reservationId",
                                                                                            Reservation_Id.Parse)

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, StartTransactionRequestXML, e);

                StartTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StartTransactionRequestText, out StartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestText">The text to parse.</param>
        /// <param name="StartTransactionRequest">The parsed start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       StartTransactionRequestText,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(StartTransactionRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out StartTransactionRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, StartTransactionRequestText, e);
            }

            StartTransactionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "startTransactionRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",          ConnectorId),
                   new XElement(OCPPNS.OCPPv1_6_CS + "idTag",                IdTag.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "timestamp",            Timestamp.ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "meterStart",           MeterStart),

                   ReservationId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "reservationId",  ReservationId.Value)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (StartTransactionRequest1, StartTransactionRequest2)

        /// <summary>
        /// Compares two start transaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A start transaction request.</param>
        /// <param name="StartTransactionRequest2">Another start transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StartTransactionRequest StartTransactionRequest1, StartTransactionRequest StartTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(StartTransactionRequest1, StartTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) StartTransactionRequest1 == null) || ((Object) StartTransactionRequest2 == null))
                return false;

            return StartTransactionRequest1.Equals(StartTransactionRequest2);

        }

        #endregion

        #region Operator != (StartTransactionRequest1, StartTransactionRequest2)

        /// <summary>
        /// Compares two start transaction requests for inequality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A start transaction request.</param>
        /// <param name="StartTransactionRequest2">Another start transaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StartTransactionRequest StartTransactionRequest1, StartTransactionRequest StartTransactionRequest2)

            => !(StartTransactionRequest1 == StartTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<StartTransactionRequest> Members

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

            // Check if the given object is a start transaction request.
            var StartTransactionRequest = Object as StartTransactionRequest;
            if ((Object) StartTransactionRequest == null)
                return false;

            return this.Equals(StartTransactionRequest);

        }

        #endregion

        #region Equals(StartTransactionRequest)

        /// <summary>
        /// Compares two start transaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest">A start transaction request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StartTransactionRequest StartTransactionRequest)
        {

            if ((Object) StartTransactionRequest == null)
                return false;

            return ConnectorId.Equals(StartTransactionRequest.ConnectorId) &&
                   IdTag.      Equals(StartTransactionRequest.IdTag)       &&
                   Timestamp.  Equals(StartTransactionRequest.Timestamp)   &&
                   MeterStart. Equals(StartTransactionRequest.MeterStart)  &&

                   ((!ReservationId.HasValue && !StartTransactionRequest.ReservationId.HasValue) ||
                     (ReservationId.HasValue &&  StartTransactionRequest.ReservationId.HasValue && ReservationId.Equals(StartTransactionRequest.ReservationId)));

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

                return ConnectorId.GetHashCode() * 19 ^
                       IdTag.      GetHashCode() * 11 ^
                       Timestamp.  GetHashCode() *  6 ^
                       MeterStart. GetHashCode() *  5 ^

                       (ReservationId.HasValue
                            ? ReservationId.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId,
                             " for ", IdTag,
                             ReservationId.HasValue ? " using reservation " + ReservationId : "");

        #endregion


    }

}
