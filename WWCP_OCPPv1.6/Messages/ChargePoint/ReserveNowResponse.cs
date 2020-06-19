/*/*
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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP reserve now response.
    /// </summary>
    public class ReserveNowResponse : AResponse<ReserveNowResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reservation.
        /// </summary>
        public ReservationStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The reserve now failed.
        /// </summary>
        public static ReserveNowResponse Failed
            => new ReserveNowResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region ReserveNowResponse(Status)

        /// <summary>
        /// Create a new OCPP reserve now response.
        /// </summary>
        /// <param name="Status">The success or failure of the reservation.</param>
        public ReserveNowResponse(ReservationStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ReserveNowResponse(Result)

        /// <summary>
        /// Create a new OCPP reserve now response.
        /// </summary>
        public ReserveNowResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:reserveNowResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:reserveNowResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (ReserveNowResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP reserve now response.
        /// </summary>
        /// <param name="ReserveNowResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ReserveNowResponse Parse(XElement             ReserveNowResponseXML,
                                               OnExceptionDelegate  OnException = null)
        {

            ReserveNowResponse _ReserveNowResponse;

            if (TryParse(ReserveNowResponseXML, out _ReserveNowResponse, OnException))
                return _ReserveNowResponse;

            return null;

        }

        #endregion

        #region (static) Parse   (ReserveNowResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP reserve now response.
        /// </summary>
        /// <param name="ReserveNowResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ReserveNowResponse Parse(String               ReserveNowResponseText,
                                               OnExceptionDelegate  OnException = null)
        {

            ReserveNowResponse _ReserveNowResponse;

            if (TryParse(ReserveNowResponseText, out _ReserveNowResponse, OnException))
                return _ReserveNowResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(ReserveNowResponseXML,  out ReserveNowResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP reserve now response.
        /// </summary>
        /// <param name="ReserveNowResponseXML">The XML to be parsed.</param>
        /// <param name="ReserveNowResponse">The parsed reserve now response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                ReserveNowResponseXML,
                                       out ReserveNowResponse  ReserveNowResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ReserveNowResponse = new ReserveNowResponse(

                                         ReserveNowResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                              XML_IO.AsReservationStatus)

                                     );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ReserveNowResponseXML, e);

                ReserveNowResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ReserveNowResponseText, out ReserveNowResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP reserve now response.
        /// </summary>
        /// <param name="ReserveNowResponseText">The text to be parsed.</param>
        /// <param name="ReserveNowResponse">The parsed reserve now response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                  ReserveNowResponseText,
                                       out ReserveNowResponse  ReserveNowResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ReserveNowResponseText).Root,
                             out ReserveNowResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ReserveNowResponseText, e);
            }

            ReserveNowResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "reserveNowResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (ReserveNowResponse1, ReserveNowResponse2)

        /// <summary>
        /// Compares two reserve now responses for equality.
        /// </summary>
        /// <param name="ReserveNowResponse1">A reserve now response.</param>
        /// <param name="ReserveNowResponse2">Another reserve now response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReserveNowResponse ReserveNowResponse1, ReserveNowResponse ReserveNowResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReserveNowResponse1, ReserveNowResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ReserveNowResponse1 == null) || ((Object) ReserveNowResponse2 == null))
                return false;

            return ReserveNowResponse1.Equals(ReserveNowResponse2);

        }

        #endregion

        #region Operator != (ReserveNowResponse1, ReserveNowResponse2)

        /// <summary>
        /// Compares two reserve now responses for inequality.
        /// </summary>
        /// <param name="ReserveNowResponse1">A reserve now response.</param>
        /// <param name="ReserveNowResponse2">Another reserve now response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReserveNowResponse ReserveNowResponse1, ReserveNowResponse ReserveNowResponse2)

            => !(ReserveNowResponse1 == ReserveNowResponse2);

        #endregion

        #endregion

        #region IEquatable<ReserveNowResponse> Members

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

            // Check if the given object is a reserve now response.
            var ReserveNowResponse = Object as ReserveNowResponse;
            if ((Object) ReserveNowResponse == null)
                return false;

            return this.Equals(ReserveNowResponse);

        }

        #endregion

        #region Equals(ReserveNowResponse)

        /// <summary>
        /// Compares two reserve now responses for equality.
        /// </summary>
        /// <param name="ReserveNowResponse">A reserve now response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ReserveNowResponse ReserveNowResponse)
        {

            if ((Object) ReserveNowResponse == null)
                return false;

            return Status.Equals(ReserveNowResponse.Status);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => Status.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Status.ToString();

        #endregion


    }

}
