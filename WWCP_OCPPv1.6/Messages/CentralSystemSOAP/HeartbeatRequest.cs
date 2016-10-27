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
    /// An OCPP heartbeat request.
    /// </summary>
    public class HeartbeatRequest
    {

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
        //       <ns:heartbeatRequest/>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(HeartbeatRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP heartbeat request.
        /// </summary>
        /// <param name="HeartbeatRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static HeartbeatRequest Parse(XElement             HeartbeatRequestXML,
                                             OnExceptionDelegate  OnException = null)
        {

            HeartbeatRequest _HeartbeatRequest;

            if (TryParse(HeartbeatRequestXML, out _HeartbeatRequest, OnException))
                return _HeartbeatRequest;

            return null;

        }

        #endregion

        #region (static) Parse(HeartbeatRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP heartbeat request.
        /// </summary>
        /// <param name="HeartbeatRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static HeartbeatRequest Parse(String               HeartbeatRequestText,
                                             OnExceptionDelegate  OnException = null)
        {

            HeartbeatRequest _HeartbeatRequest;

            if (TryParse(HeartbeatRequestText, out _HeartbeatRequest, OnException))
                return _HeartbeatRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(HeartbeatRequestXML,  out HeartbeatRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP heartbeat request.
        /// </summary>
        /// <param name="HeartbeatRequestXML">The XML to parse.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              HeartbeatRequestXML,
                                       out HeartbeatRequest  HeartbeatRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                HeartbeatRequest = new HeartbeatRequest();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, HeartbeatRequestXML, e);

                HeartbeatRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(HeartbeatRequestText, out HeartbeatRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP heartbeat request.
        /// </summary>
        /// <param name="HeartbeatRequestText">The text to parse.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                HeartbeatRequestText,
                                       out HeartbeatRequest  HeartbeatRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(HeartbeatRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out HeartbeatRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, HeartbeatRequestText, e);
            }

            HeartbeatRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "heartbeatRequest");

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest1">A heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (HeartbeatRequest HeartbeatRequest1, HeartbeatRequest HeartbeatRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(HeartbeatRequest1, HeartbeatRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) HeartbeatRequest1 == null) || ((Object) HeartbeatRequest2 == null))
                return false;

            return HeartbeatRequest1.Equals(HeartbeatRequest2);

        }

        #endregion

        #region Operator != (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for inequality.
        /// </summary>
        /// <param name="HeartbeatRequest1">A heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (HeartbeatRequest HeartbeatRequest1, HeartbeatRequest HeartbeatRequest2)

            => !(HeartbeatRequest1 == HeartbeatRequest2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatRequest> Members

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

            // Check if the given object is a heartbeat request.
            var HeartbeatRequest = Object as HeartbeatRequest;
            if ((Object) HeartbeatRequest == null)
                return false;

            return this.Equals(HeartbeatRequest);

        }

        #endregion

        #region Equals(HeartbeatRequest)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest">A heartbeat request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(HeartbeatRequest HeartbeatRequest)
        {

            if ((Object) HeartbeatRequest == null)
                return false;

            return Object.ReferenceEquals(this, HeartbeatRequest);

        }

        #endregion

        #endregion

        #region GetHashCode()

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

            => "HeartbeatRequest";

        #endregion


    }

}
