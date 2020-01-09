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

using org.GraphDefined.Vanaheimr.Illias;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP reserve now request.
    /// </summary>
    public class ReserveNowRequest : ARequest<ReserveNowRequest>
    {

        #region Properties

        /// <summary>
        /// The identification of the connector to be reserved.
        /// A value of 0 means that the reservation is not for
        /// a specific connector.
        /// </summary>
        public Connector_Id    ConnectorId      { get; }

        /// <summary>
        /// The unique identification of this reservation.
        /// </summary>
        public Reservation_Id  ReservationId    { get; }

        /// <summary>
        /// The timestamp when the reservation ends.
        /// </summary>
        public DateTime        ExpiryDate       { get; }

        /// <summary>
        /// The identifier for which the charge point has to
        /// reserve a connector.
        /// </summary>
        public IdToken         IdTag            { get; }

        /// <summary>
        /// An optional parent idTag.
        /// </summary>
        public IdToken?        ParentIdTag      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP ReserveNowRequest XML/SOAP request.
        /// </summary>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The identifier for which the charge point has to reserve a connector.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
        public ReserveNowRequest(Connector_Id    ConnectorId,
                                 Reservation_Id  ReservationId,
                                 DateTime        ExpiryDate,
                                 IdToken         IdTag,
                                 IdToken?        ParentIdTag = null)
        {

            #region Initial checks

            if (ConnectorId   == null)
                throw new ArgumentNullException(nameof(ConnectorId),    "The given unique connector identification must not be null!");

            if (ReservationId == null)
                throw new ArgumentNullException(nameof(ReservationId),  "The given unique reservation identification must not be null!");

            if (IdTag         == null)
                throw new ArgumentNullException(nameof(IdTag),          "The given identification tag must not be null!");

            #endregion

            this.ConnectorId    = ConnectorId;
            this.ReservationId  = ReservationId;
            this.ExpiryDate     = ExpiryDate;
            this.IdTag          = IdTag;
            this.ParentIdTag    = ParentIdTag ?? new IdToken?();

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
        //       <ns:reserveNowRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:expiryDate>?</ns:expiryDate>
        //          <ns:idTag>?</ns:idTag>
        //
        //          <!--Optional:-->
        //          <ns:parentIdTag>?</ns:parentIdTag>
        //
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:reserveNowRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(ReserveNowRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP reserve now request.
        /// </summary>
        /// <param name="ReserveNowRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ReserveNowRequest Parse(XElement             ReserveNowRequestXML,
                                              OnExceptionDelegate  OnException = null)
        {

            ReserveNowRequest _ReserveNowRequest;

            if (TryParse(ReserveNowRequestXML, out _ReserveNowRequest, OnException))
                return _ReserveNowRequest;

            return null;

        }

        #endregion

        #region (static) Parse(ReserveNowRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP reserve now request.
        /// </summary>
        /// <param name="ReserveNowRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ReserveNowRequest Parse(String               ReserveNowRequestText,
                                              OnExceptionDelegate  OnException = null)
        {

            ReserveNowRequest _ReserveNowRequest;

            if (TryParse(ReserveNowRequestText, out _ReserveNowRequest, OnException))
                return _ReserveNowRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(ReserveNowRequestXML,  out ReserveNowRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP reserve now request.
        /// </summary>
        /// <param name="ReserveNowRequestXML">The XML to parse.</param>
        /// <param name="ReserveNowRequest">The parsed reserve now request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement               ReserveNowRequestXML,
                                       out ReserveNowRequest  ReserveNowRequest,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                ReserveNowRequest = new ReserveNowRequest(

                                        ReserveNowRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                Connector_Id.Parse),

                                        ReserveNowRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "reservationId",
                                                                                Reservation_Id.Parse),

                                        ReserveNowRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "expiryDate",
                                                                                DateTime.Parse),

                                        ReserveNowRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "idTag",
                                                                                IdToken.Parse),

                                        ReserveNowRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "parentIdTag",
                                                                                IdToken.Parse)

                                    );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ReserveNowRequestXML, e);

                ReserveNowRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ReserveNowRequestText, out ReserveNowRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP reserve now request.
        /// </summary>
        /// <param name="ReserveNowRequestText">The text to parse.</param>
        /// <param name="ReserveNowRequest">The parsed reserve now request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                 ReserveNowRequestText,
                                       out ReserveNowRequest  ReserveNowRequest,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ReserveNowRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out ReserveNowRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ReserveNowRequestText, e);
            }

            ReserveNowRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "reserveNowRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",        ConnectorId.  ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "expiryDate",         ExpiryDate.   ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "idTag",              IdTag.        ToString()),

                   ParentIdTag.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "parentIdTag",  ParentIdTag.Value.ToString())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "reservationId",      ReservationId.ToString())

               );

        #endregion


        #region Operator overloading

        #region Operator == (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two reserve now requests for equality.
        /// </summary>
        /// <param name="ReserveNowRequest1">A reserve now request.</param>
        /// <param name="ReserveNowRequest2">Another reserve now request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReserveNowRequest ReserveNowRequest1, ReserveNowRequest ReserveNowRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ReserveNowRequest1, ReserveNowRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ReserveNowRequest1 == null) || ((Object) ReserveNowRequest2 == null))
                return false;

            return ReserveNowRequest1.Equals(ReserveNowRequest2);

        }

        #endregion

        #region Operator != (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two reserve now requests for inequality.
        /// </summary>
        /// <param name="ReserveNowRequest1">A reserve now request.</param>
        /// <param name="ReserveNowRequest2">Another reserve now request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReserveNowRequest ReserveNowRequest1, ReserveNowRequest ReserveNowRequest2)

            => !(ReserveNowRequest1 == ReserveNowRequest2);

        #endregion

        #endregion

        #region IEquatable<ReserveNowRequest> Members

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

            // Check if the given object is a reserve now request.
            var ReserveNowRequest = Object as ReserveNowRequest;
            if ((Object) ReserveNowRequest == null)
                return false;

            return this.Equals(ReserveNowRequest);

        }

        #endregion

        #region Equals(ReserveNowRequest)

        /// <summary>
        /// Compares two reserve now requests for equality.
        /// </summary>
        /// <param name="ReserveNowRequest">A reserve now request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ReserveNowRequest ReserveNowRequest)
        {

            if ((Object) ReserveNowRequest == null)
                return false;

            return ReservationId.Equals(ReserveNowRequest.ReservationId) &&
                   ConnectorId.  Equals(ReserveNowRequest.ConnectorId)   &&
                   ExpiryDate.   Equals(ReserveNowRequest.ExpiryDate)    &&
                   IdTag.        Equals(ReserveNowRequest.IdTag)         &&

                   ((!ParentIdTag.HasValue && !ReserveNowRequest.ParentIdTag.HasValue) ||
                     (ParentIdTag.HasValue &&  ReserveNowRequest.ParentIdTag.HasValue && ParentIdTag.Equals(ReserveNowRequest.ParentIdTag)));

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

                return ReservationId.GetHashCode() * 23 ^
                       ConnectorId  .GetHashCode() * 19 ^
                       ExpiryDate   .GetHashCode() * 17 ^
                       IdTag        .GetHashCode() * 11 ^

                       (ParentIdTag.HasValue
                            ? ParentIdTag.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId, " / ",
                             ExpiryDate.ToIso8601(), " / ",
                             IdTag,
                             ParentIdTag.HasValue ? "/" + ParentIdTag.Value : "",
                             " (", ReservationId, ")");

        #endregion


    }

}
