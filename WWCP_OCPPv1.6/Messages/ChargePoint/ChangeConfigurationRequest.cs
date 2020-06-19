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
    /// An OCPP change configuration request.
    /// </summary>
    public class ChangeConfigurationRequest : ARequest<ChangeConfigurationRequest>
    {

        #region Properties

        /// <summary>
        /// The name of the configuration setting to change.
        /// </summary>
        public String  Key     { get; }

        /// <summary>
        /// The new value as string for the setting.
        /// </summary>
        public String  Value   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP ChangeConfigurationRequest XML/SOAP request.
        /// </summary>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
        public ChangeConfigurationRequest(String  Key,
                                          String  Value)
        {

            #region Initial checks

            if (Key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Key),  "The given configuration key must not be null or empty!");

            #endregion

            this.Key    = Key;
            this.Value  = Value;

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
        //       <ns:changeConfigurationRequest>
        //
        //          <ns:key>?</ns:key>
        //          <ns:value>?</ns:value>
        //
        //       </ns:changeConfigurationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (ChangeConfigurationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP change configuration request.
        /// </summary>
        /// <param name="ChangeConfigurationRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeConfigurationRequest Parse(XElement             ChangeConfigurationRequestXML,
                                                       OnExceptionDelegate  OnException = null)
        {

            ChangeConfigurationRequest _ChangeConfigurationRequest;

            if (TryParse(ChangeConfigurationRequestXML, out _ChangeConfigurationRequest, OnException))
                return _ChangeConfigurationRequest;

            return null;

        }

        #endregion

        #region (static) Parse   (ChangeConfigurationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP change configuration request.
        /// </summary>
        /// <param name="ChangeConfigurationRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeConfigurationRequest Parse(String               ChangeConfigurationRequestText,
                                                       OnExceptionDelegate  OnException = null)
        {

            ChangeConfigurationRequest _ChangeConfigurationRequest;

            if (TryParse(ChangeConfigurationRequestText, out _ChangeConfigurationRequest, OnException))
                return _ChangeConfigurationRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(ChangeConfigurationRequestXML,  out ChangeConfigurationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP change configuration request.
        /// </summary>
        /// <param name="ChangeConfigurationRequestXML">The XML to be parsed.</param>
        /// <param name="ChangeConfigurationRequest">The parsed change configuration request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                        ChangeConfigurationRequestXML,
                                       out ChangeConfigurationRequest  ChangeConfigurationRequest,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                ChangeConfigurationRequest = new ChangeConfigurationRequest(

                                                 ChangeConfigurationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "key"),
                                                 ChangeConfigurationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "value")

                                             );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChangeConfigurationRequestXML, e);

                ChangeConfigurationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChangeConfigurationRequestText, out ChangeConfigurationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP change configuration request.
        /// </summary>
        /// <param name="ChangeConfigurationRequestText">The text to be parsed.</param>
        /// <param name="ChangeConfigurationRequest">The parsed change configuration request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                          ChangeConfigurationRequestText,
                                       out ChangeConfigurationRequest  ChangeConfigurationRequest,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ChangeConfigurationRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out ChangeConfigurationRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ChangeConfigurationRequestText, e);
            }

            ChangeConfigurationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "changeConfigurationRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "key",   Key),
                   new XElement(OCPPNS.OCPPv1_6_CP + "value", Value)

               );

        #endregion


        #region Operator overloading

        #region Operator == (ChangeConfigurationRequest1, ChangeConfigurationRequest2)

        /// <summary>
        /// Compares two change configuration requests for equality.
        /// </summary>
        /// <param name="ChangeConfigurationRequest1">A change configuration request.</param>
        /// <param name="ChangeConfigurationRequest2">Another change configuration request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeConfigurationRequest ChangeConfigurationRequest1, ChangeConfigurationRequest ChangeConfigurationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeConfigurationRequest1, ChangeConfigurationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChangeConfigurationRequest1 == null) || ((Object) ChangeConfigurationRequest2 == null))
                return false;

            return ChangeConfigurationRequest1.Equals(ChangeConfigurationRequest2);

        }

        #endregion

        #region Operator != (ChangeConfigurationRequest1, ChangeConfigurationRequest2)

        /// <summary>
        /// Compares two change configuration requests for inequality.
        /// </summary>
        /// <param name="ChangeConfigurationRequest1">A change configuration request.</param>
        /// <param name="ChangeConfigurationRequest2">Another change configuration request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeConfigurationRequest ChangeConfigurationRequest1, ChangeConfigurationRequest ChangeConfigurationRequest2)

            => !(ChangeConfigurationRequest1 == ChangeConfigurationRequest2);

        #endregion

        #endregion

        #region IEquatable<ChangeConfigurationRequest> Members

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

            // Check if the given object is a change configuration request.
            var ChangeConfigurationRequest = Object as ChangeConfigurationRequest;
            if ((Object) ChangeConfigurationRequest == null)
                return false;

            return this.Equals(ChangeConfigurationRequest);

        }

        #endregion

        #region Equals(ChangeConfigurationRequest)

        /// <summary>
        /// Compares two change configuration requests for equality.
        /// </summary>
        /// <param name="ChangeConfigurationRequest">A change configuration request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ChangeConfigurationRequest ChangeConfigurationRequest)
        {

            if ((Object) ChangeConfigurationRequest == null)
                return false;

            return Key.  Equals(ChangeConfigurationRequest.Key) &&

                   ((Value == null && ChangeConfigurationRequest.Value == null) ||
                    (Value != null && ChangeConfigurationRequest.Value != null && Value.Equals(ChangeConfigurationRequest.Value)));

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

                return Key.GetHashCode() * 5 ^

                       (Value != null
                            ? Value.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Key,
                             " = ",
                             Value != null ? Value : "");

        #endregion


    }

}
