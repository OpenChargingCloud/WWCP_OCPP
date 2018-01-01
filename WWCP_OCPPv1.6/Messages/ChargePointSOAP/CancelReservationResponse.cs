/*/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
    /// An OCPP cancel reservation response.
    /// </summary>
    public class CancelReservationResponse : AResponse<CancelReservationResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reservation cancellation.
        /// </summary>
        public CancelReservationStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The cancel reservation failed.
        /// </summary>
        public static CancelReservationResponse Failed
            => new CancelReservationResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region CancelReservationResponse(Status)

        /// <summary>
        /// Create a new OCPP cancel reservation response.
        /// </summary>
        /// <param name="Status">The success or failure of the reservation.</param>
        public CancelReservationResponse(CancelReservationStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region CancelReservationResponse(Result)

        /// <summary>
        /// Create a new OCPP cancel reservation response.
        /// </summary>
        public CancelReservationResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:cancelReservationStatus>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:cancelReservationStatus>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(CancelReservationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP cancel reservation response.
        /// </summary>
        /// <param name="CancelReservationResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationResponse Parse(XElement             CancelReservationResponseXML,
                                                      OnExceptionDelegate  OnException = null)
        {

            CancelReservationResponse _CancelReservationResponse;

            if (TryParse(CancelReservationResponseXML, out _CancelReservationResponse, OnException))
                return _CancelReservationResponse;

            return null;

        }

        #endregion

        #region (static) Parse(CancelReservationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP cancel reservation response.
        /// </summary>
        /// <param name="CancelReservationResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationResponse Parse(String               CancelReservationResponseText,
                                                      OnExceptionDelegate  OnException = null)
        {

            CancelReservationResponse _CancelReservationResponse;

            if (TryParse(CancelReservationResponseText, out _CancelReservationResponse, OnException))
                return _CancelReservationResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(CancelReservationResponseXML,  out CancelReservationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP cancel reservation response.
        /// </summary>
        /// <param name="CancelReservationResponseXML">The XML to parse.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                       CancelReservationResponseXML,
                                       out CancelReservationResponse  CancelReservationResponse,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                CancelReservationResponse = new CancelReservationResponse(

                                                CancelReservationResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                            XML_IO.AsCancelReservationStatus)

                                            );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, CancelReservationResponseXML, e);

                CancelReservationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(CancelReservationResponseText, out CancelReservationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP cancel reservation response.
        /// </summary>
        /// <param name="CancelReservationResponseText">The text to parse.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                         CancelReservationResponseText,
                                       out CancelReservationResponse  CancelReservationResponse,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(CancelReservationResponseText).Root,
                             out CancelReservationResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, CancelReservationResponseText, e);
            }

            CancelReservationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "cancelReservationResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationResponse CancelReservationResponse1, CancelReservationResponse CancelReservationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(CancelReservationResponse1, CancelReservationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) CancelReservationResponse1 == null) || ((Object) CancelReservationResponse2 == null))
                return false;

            return CancelReservationResponse1.Equals(CancelReservationResponse2);

        }

        #endregion

        #region Operator != (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for inequality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationResponse CancelReservationResponse1, CancelReservationResponse CancelReservationResponse2)

            => !(CancelReservationResponse1 == CancelReservationResponse2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationResponse> Members

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

            // Check if the given object is a cancel reservation response.
            var CancelReservationResponse = Object as CancelReservationResponse;
            if ((Object) CancelReservationResponse == null)
                return false;

            return this.Equals(CancelReservationResponse);

        }

        #endregion

        #region Equals(CancelReservationResponse)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse">A cancel reservation response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(CancelReservationResponse CancelReservationResponse)
        {

            if ((Object) CancelReservationResponse == null)
                return false;

            return Status.Equals(CancelReservationResponse.Status);

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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => Status.ToString();

        #endregion


    }

}
