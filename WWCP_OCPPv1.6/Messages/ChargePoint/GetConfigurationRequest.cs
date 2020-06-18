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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP get configuration request.
    /// </summary>
    public class GetConfigurationRequest : ARequest<GetConfigurationRequest>
    {

        #region Properties

        /// <summary>
        /// An enumeration of keys for which the configuration is requested.
        /// Return all keys if empty.
        /// </summary>
        public IEnumerable<String>  Keys   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP GetConfigurationRequest XML/SOAP request.
        /// </summary>
        /// <param name="Keys">An enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        public GetConfigurationRequest(IEnumerable<String> Keys = null)
        {

            this.Keys = Keys ?? new String[0];

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
        //       <ns:getConfigurationRequest>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:key>?</ns:key>
        //
        //       </ns:getConfigurationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(GetConfigurationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP get configuration request.
        /// </summary>
        /// <param name="GetConfigurationRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationRequest Parse(XElement             GetConfigurationRequestXML,
                                                    OnExceptionDelegate  OnException = null)
        {

            GetConfigurationRequest _GetConfigurationRequest;

            if (TryParse(GetConfigurationRequestXML, out _GetConfigurationRequest, OnException))
                return _GetConfigurationRequest;

            return null;

        }

        #endregion

        #region (static) Parse(GetConfigurationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP get configuration request.
        /// </summary>
        /// <param name="GetConfigurationRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationRequest Parse(String               GetConfigurationRequestText,
                                                    OnExceptionDelegate  OnException = null)
        {

            GetConfigurationRequest _GetConfigurationRequest;

            if (TryParse(GetConfigurationRequestText, out _GetConfigurationRequest, OnException))
                return _GetConfigurationRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(GetConfigurationRequestXML,  out GetConfigurationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP get configuration request.
        /// </summary>
        /// <param name="GetConfigurationRequestXML">The XML to be parsed.</param>
        /// <param name="GetConfigurationRequest">The parsed get configuration request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                     GetConfigurationRequestXML,
                                       out GetConfigurationRequest  GetConfigurationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                GetConfigurationRequest = new GetConfigurationRequest(

                                              GetConfigurationRequestXML.ElementValues(OCPPNS.OCPPv1_6_CP + "key")

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetConfigurationRequestXML, e);

                GetConfigurationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetConfigurationRequestText, out GetConfigurationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP get configuration request.
        /// </summary>
        /// <param name="GetConfigurationRequestText">The text to be parsed.</param>
        /// <param name="GetConfigurationRequest">The parsed get configuration request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       GetConfigurationRequestText,
                                       out GetConfigurationRequest  GetConfigurationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(GetConfigurationRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out GetConfigurationRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, GetConfigurationRequestText, e);
            }

            GetConfigurationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getConfigurationRequest",

                   Keys.IsNeitherNullNorEmpty()
                       ? Keys.Select(key => new XElement(OCPPNS.OCPPv1_6_CP + "key",  key))
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (GetConfigurationRequest1, GetConfigurationRequest2)

        /// <summary>
        /// Compares two get configuration requests for equality.
        /// </summary>
        /// <param name="GetConfigurationRequest1">A get configuration request.</param>
        /// <param name="GetConfigurationRequest2">Another get configuration request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetConfigurationRequest GetConfigurationRequest1, GetConfigurationRequest GetConfigurationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetConfigurationRequest1, GetConfigurationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GetConfigurationRequest1 == null) || ((Object) GetConfigurationRequest2 == null))
                return false;

            return GetConfigurationRequest1.Equals(GetConfigurationRequest2);

        }

        #endregion

        #region Operator != (GetConfigurationRequest1, GetConfigurationRequest2)

        /// <summary>
        /// Compares two get configuration requests for inequality.
        /// </summary>
        /// <param name="GetConfigurationRequest1">A get configuration request.</param>
        /// <param name="GetConfigurationRequest2">Another get configuration request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetConfigurationRequest GetConfigurationRequest1, GetConfigurationRequest GetConfigurationRequest2)

            => !(GetConfigurationRequest1 == GetConfigurationRequest2);

        #endregion

        #endregion

        #region IEquatable<GetConfigurationRequest> Members

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

            // Check if the given object is a get configuration request.
            var GetConfigurationRequest = Object as GetConfigurationRequest;
            if ((Object) GetConfigurationRequest == null)
                return false;

            return this.Equals(GetConfigurationRequest);

        }

        #endregion

        #region Equals(GetConfigurationRequest)

        /// <summary>
        /// Compares two get configuration requests for equality.
        /// </summary>
        /// <param name="GetConfigurationRequest">A get configuration request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetConfigurationRequest GetConfigurationRequest)
        {

            if ((Object) GetConfigurationRequest == null)
                return false;

            return (Keys == null && GetConfigurationRequest.Keys == null) ||
                   (Keys != null && GetConfigurationRequest.Keys != null && Keys.Count().Equals(GetConfigurationRequest.Keys.Count()));

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Keys != null
                   ? Keys.GetHashCode()
                   : base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => (Keys != null
                    ? Keys.Count().ToString()
                    : "0") + " configuration key(s)";

        #endregion


    }

}
