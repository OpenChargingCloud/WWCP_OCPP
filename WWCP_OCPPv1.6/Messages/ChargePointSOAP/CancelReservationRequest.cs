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
    /// An OCPP cancel reservation request.
    /// </summary>
    public class CancelReservationRequest : IEquatable<CancelReservationRequest>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the reservation to cancel.
        /// </summary>
        public Reservation_Id  ReservationId   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP CancelReservationRequest XML/SOAP request.
        /// </summary>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        public CancelReservationRequest(Reservation_Id  ReservationId)
        {

            #region Initial checks

            if (ReservationId == null)
                throw new ArgumentNullException(nameof(ReservationId),  "The given unique reservation identification must not be null!");

            #endregion

            this.ReservationId  = ReservationId;

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
        //       <ns:cancelReservationRequest>
        //
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:cancelReservationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(CancelReservationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationRequest Parse(XElement             CancelReservationRequestXML,
                                                     OnExceptionDelegate  OnException = null)
        {

            CancelReservationRequest _CancelReservationRequest;

            if (TryParse(CancelReservationRequestXML, out _CancelReservationRequest, OnException))
                return _CancelReservationRequest;

            return null;

        }

        #endregion

        #region (static) Parse(CancelReservationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationRequest Parse(String               CancelReservationRequestText,
                                                     OnExceptionDelegate  OnException = null)
        {

            CancelReservationRequest _CancelReservationRequest;

            if (TryParse(CancelReservationRequestText, out _CancelReservationRequest, OnException))
                return _CancelReservationRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(CancelReservationRequestXML,  out CancelReservationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestXML">The XML to parse.</param>
        /// <param name="CancelReservationRequest">The parsed cancel reservation request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                      CancelReservationRequestXML,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                CancelReservationRequest = new CancelReservationRequest(

                                               CancelReservationRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "reservationId",
                                                                                          Reservation_Id.Parse)

                                           );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, CancelReservationRequestXML, e);

                CancelReservationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(CancelReservationRequestText, out CancelReservationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestText">The text to parse.</param>
        /// <param name="CancelReservationRequest">The parsed cancel reservation request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                        CancelReservationRequestText,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(CancelReservationRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out CancelReservationRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, CancelReservationRequestText, e);
            }

            CancelReservationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "reserveNowRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "reservationId",  ReservationId.ToString())

               );

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two cancel reservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A cancel reservation request.</param>
        /// <param name="CancelReservationRequest2">Another cancel reservation request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationRequest CancelReservationRequest1, CancelReservationRequest CancelReservationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(CancelReservationRequest1, CancelReservationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) CancelReservationRequest1 == null) || ((Object) CancelReservationRequest2 == null))
                return false;

            return CancelReservationRequest1.Equals(CancelReservationRequest2);

        }

        #endregion

        #region Operator != (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two cancel reservation requests for inequality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A cancel reservation request.</param>
        /// <param name="CancelReservationRequest2">Another cancel reservation request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationRequest CancelReservationRequest1, CancelReservationRequest CancelReservationRequest2)

            => !(CancelReservationRequest1 == CancelReservationRequest2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationRequest> Members

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

            // Check if the given object is a cancel reservation request.
            var CancelReservationRequest = Object as CancelReservationRequest;
            if ((Object) CancelReservationRequest == null)
                return false;

            return this.Equals(CancelReservationRequest);

        }

        #endregion

        #region Equals(CancelReservationRequest)

        /// <summary>
        /// Compares two cancel reservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest">A cancel reservation request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CancelReservationRequest CancelReservationRequest)
        {

            if ((Object) CancelReservationRequest == null)
                return false;

            return ReservationId.Equals(CancelReservationRequest.ReservationId);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => ReservationId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => ReservationId.ToString();

        #endregion


    }

}
