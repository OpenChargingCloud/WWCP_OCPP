/*
 * Copyright (c) 2014-2017 GraphDefined GmbH
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
    /// An OCPP authorize request.
    /// </summary>
    public class AuthorizeRequest : ARequest<AuthorizeRequest>
    {

        #region Properties

        /// <summary>
        /// The identifier that needs to be authorized.
        /// </summary>
        public IdToken  IdTag   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP Authorize XML/SOAP request.
        /// </summary>
        /// <param name="IdTag">The identifier that needs to be authorized.</param>
        public AuthorizeRequest(IdToken  IdTag)
        {

            this.IdTag = IdTag;

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
        //       <ns:authorizeRequest>
        //
        //          <ns:idTag>?</ns:idTag>
        //
        //       </ns:authorizeRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(AuthorizeRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(XElement             AuthorizeRequestXML,
                                             OnExceptionDelegate  OnException = null)
        {

            AuthorizeRequest _AuthorizeRequest;

            if (TryParse(AuthorizeRequestXML, out _AuthorizeRequest, OnException))
                return _AuthorizeRequest;

            return null;

        }

        #endregion

        #region (static) Parse(AuthorizeRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(String               AuthorizeRequestText,
                                             OnExceptionDelegate  OnException = null)
        {

            AuthorizeRequest _AuthorizeRequest;

            if (TryParse(AuthorizeRequestText, out _AuthorizeRequest, OnException))
                return _AuthorizeRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(AuthorizeRequestXML,  out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestXML">The XML to parse.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              AuthorizeRequestXML,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                AuthorizeRequest = new AuthorizeRequest(

                                       AuthorizeRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "idTag",
                                                                          IdToken.Parse)

                                   );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, AuthorizeRequestXML, e);

                AuthorizeRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(AuthorizeRequestText, out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestText">The text to parse.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                AuthorizeRequestText,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(AuthorizeRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out AuthorizeRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, AuthorizeRequestText, e);
            }

            AuthorizeRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "authorizeRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "idTag",  IdTag.ToString())

               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest1">A authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeRequest AuthorizeRequest1, AuthorizeRequest AuthorizeRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(AuthorizeRequest1, AuthorizeRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) AuthorizeRequest1 == null) || ((Object) AuthorizeRequest2 == null))
                return false;

            return AuthorizeRequest1.Equals(AuthorizeRequest2);

        }

        #endregion

        #region Operator != (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two authorize requests for inequality.
        /// </summary>
        /// <param name="AuthorizeRequest1">A authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeRequest AuthorizeRequest1, AuthorizeRequest AuthorizeRequest2)

            => !(AuthorizeRequest1 == AuthorizeRequest2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeRequest> Members

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

            // Check if the given object is a authorize request.
            var AuthorizeRequest = Object as AuthorizeRequest;
            if ((Object) AuthorizeRequest == null)
                return false;

            return this.Equals(AuthorizeRequest);

        }

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">A authorize request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(AuthorizeRequest AuthorizeRequest)
        {

            if ((Object) AuthorizeRequest == null)
                return false;

            return IdTag.Equals(AuthorizeRequest.IdTag);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => IdTag.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => IdTag.ToString();

        #endregion


    }

}
