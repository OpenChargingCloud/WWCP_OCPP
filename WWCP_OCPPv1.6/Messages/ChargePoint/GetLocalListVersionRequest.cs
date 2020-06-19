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
    /// An OCPP get local list version request.
    /// </summary>
    public class GetLocalListVersionRequest : ARequest<GetLocalListVersionRequest>
    {

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
        //
        //       <ns:getLocalListVersionRequest>
        //
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (GetLocalListVersionRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP get local list version request.
        /// </summary>
        /// <param name="GetLocalListVersionRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionRequest Parse(XElement             GetLocalListVersionRequestXML,
                                                       OnExceptionDelegate  OnException = null)
        {

            GetLocalListVersionRequest _GetLocalListVersionRequest;

            if (TryParse(GetLocalListVersionRequestXML, out _GetLocalListVersionRequest, OnException))
                return _GetLocalListVersionRequest;

            return null;

        }

        #endregion

        #region (static) Parse   (GetLocalListVersionRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP get local list version request.
        /// </summary>
        /// <param name="GetLocalListVersionRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionRequest Parse(String               GetLocalListVersionRequestText,
                                                       OnExceptionDelegate  OnException = null)
        {

            GetLocalListVersionRequest _GetLocalListVersionRequest;

            if (TryParse(GetLocalListVersionRequestText, out _GetLocalListVersionRequest, OnException))
                return _GetLocalListVersionRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionRequestXML,  out GetLocalListVersionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP get local list version request.
        /// </summary>
        /// <param name="GetLocalListVersionRequestXML">The XML to be parsed.</param>
        /// <param name="GetLocalListVersionRequest">The parsed get local list version request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                        GetLocalListVersionRequestXML,
                                       out GetLocalListVersionRequest  GetLocalListVersionRequest,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                GetLocalListVersionRequest = new GetLocalListVersionRequest();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetLocalListVersionRequestXML, e);

                GetLocalListVersionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionRequestText, out GetLocalListVersionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP get local list version request.
        /// </summary>
        /// <param name="GetLocalListVersionRequestText">The text to be parsed.</param>
        /// <param name="GetLocalListVersionRequest">The parsed get local list version request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                          GetLocalListVersionRequestText,
                                       out GetLocalListVersionRequest  GetLocalListVersionRequest,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(GetLocalListVersionRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out GetLocalListVersionRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, GetLocalListVersionRequestText, e);
            }

            GetLocalListVersionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getLocalListVersionRequest");

        #endregion


        #region Operator overloading

        #region Operator == (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two get local list version requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A get local list version request.</param>
        /// <param name="GetLocalListVersionRequest2">Another get local list version request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLocalListVersionRequest GetLocalListVersionRequest1, GetLocalListVersionRequest GetLocalListVersionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionRequest1, GetLocalListVersionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GetLocalListVersionRequest1 == null) || ((Object) GetLocalListVersionRequest2 == null))
                return false;

            return GetLocalListVersionRequest1.Equals(GetLocalListVersionRequest2);

        }

        #endregion

        #region Operator != (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two get local list version requests for inequality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A get local list version request.</param>
        /// <param name="GetLocalListVersionRequest2">Another get local list version request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLocalListVersionRequest GetLocalListVersionRequest1, GetLocalListVersionRequest GetLocalListVersionRequest2)

            => !(GetLocalListVersionRequest1 == GetLocalListVersionRequest2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionRequest> Members

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

            // Check if the given object is a get local list version request.
            var GetLocalListVersionRequest = Object as GetLocalListVersionRequest;
            if ((Object) GetLocalListVersionRequest == null)
                return false;

            return this.Equals(GetLocalListVersionRequest);

        }

        #endregion

        #region Equals(GetLocalListVersionRequest)

        /// <summary>
        /// Compares two get local list version requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest">A get local list version request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetLocalListVersionRequest GetLocalListVersionRequest)
        {

            if ((Object) GetLocalListVersionRequest == null)
                return false;

            return Object.ReferenceEquals(this, GetLocalListVersionRequest);

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
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "GetLocalListVersionRequest";

        #endregion


    }

}
