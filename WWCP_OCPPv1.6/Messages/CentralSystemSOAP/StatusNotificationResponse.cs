/*/*
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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP status notification response.
    /// </summary>
    public class StatusNotificationResponse : AResponse<StatusNotificationResponse>
    {

        #region Statics

        /// <summary>
        /// The start transaction failed.
        /// </summary>
        public static StatusNotificationResponse Failed
            => new StatusNotificationResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region StatusNotificationResponse()

        /// <summary>
        /// Create a new OCPP status notification response.
        /// </summary>
        public StatusNotificationResponse()
            : base(Result.OK())
        { }

        #endregion

        #region StatusNotificationResponse(Result)

        /// <summary>
        /// Create a new OCPP status notification response.
        /// </summary>
        public StatusNotificationResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:statusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(StatusNotificationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP status notification response.
        /// </summary>
        /// <param name="StatusNotificationResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationResponse Parse(XElement             StatusNotificationResponseXML,
                                                       OnExceptionDelegate  OnException = null)
        {

            StatusNotificationResponse _StatusNotificationResponse;

            if (TryParse(StatusNotificationResponseXML, out _StatusNotificationResponse, OnException))
                return _StatusNotificationResponse;

            return null;

        }

        #endregion

        #region (static) Parse(StatusNotificationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP status notification response.
        /// </summary>
        /// <param name="StatusNotificationResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationResponse Parse(String               StatusNotificationResponseText,
                                                       OnExceptionDelegate  OnException = null)
        {

            StatusNotificationResponse _StatusNotificationResponse;

            if (TryParse(StatusNotificationResponseText, out _StatusNotificationResponse, OnException))
                return _StatusNotificationResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(StatusNotificationResponseXML,  out StatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP status notification response.
        /// </summary>
        /// <param name="StatusNotificationResponseXML">The XML to parse.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                        StatusNotificationResponseXML,
                                       out StatusNotificationResponse  StatusNotificationResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                StatusNotificationResponse = new StatusNotificationResponse();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, StatusNotificationResponseXML, e);

                StatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StatusNotificationResponseText, out StatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP status notification response.
        /// </summary>
        /// <param name="StatusNotificationResponseText">The text to parse.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                          StatusNotificationResponseText,
                                       out StatusNotificationResponse  StatusNotificationResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(StatusNotificationResponseText).Root,
                             out StatusNotificationResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, StatusNotificationResponseText, e);
            }

            StatusNotificationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "statusNotificationResponse");

        #endregion


        #region Operator overloading

        #region Operator == (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A status notification response.</param>
        /// <param name="StatusNotificationResponse2">Another status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StatusNotificationResponse StatusNotificationResponse1, StatusNotificationResponse StatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(StatusNotificationResponse1, StatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) StatusNotificationResponse1 == null) || ((Object) StatusNotificationResponse2 == null))
                return false;

            return StatusNotificationResponse1.Equals(StatusNotificationResponse2);

        }

        #endregion

        #region Operator != (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two status notification responses for inequality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A status notification response.</param>
        /// <param name="StatusNotificationResponse2">Another status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationResponse StatusNotificationResponse1, StatusNotificationResponse StatusNotificationResponse2)

            => !(StatusNotificationResponse1 == StatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationResponse> Members

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

            // Check if the given object is a status notification response.
            var StatusNotificationResponse = Object as StatusNotificationResponse;
            if ((Object) StatusNotificationResponse == null)
                return false;

            return this.Equals(StatusNotificationResponse);

        }

        #endregion

        #region Equals(StatusNotificationResponse)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse">A status notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(StatusNotificationResponse StatusNotificationResponse)
        {

            if ((Object) StatusNotificationResponse == null)
                return false;

            return Object.ReferenceEquals(this, StatusNotificationResponse);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => "StatusNotificationResponse";

        #endregion


    }

}
