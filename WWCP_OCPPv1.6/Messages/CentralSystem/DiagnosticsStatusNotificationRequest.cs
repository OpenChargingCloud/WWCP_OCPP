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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP diagnostics status notification request.
    /// </summary>
    public class DiagnosticsStatusNotificationRequest : ARequest<DiagnosticsStatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The status of the diagnostics upload.
        /// </summary>
        public DiagnosticsStatus  Status   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP DiagnosticsStatusNotification XML/SOAP request.
        /// </summary>
        /// <param name="Status">The status of the diagnostics upload.</param>
        public DiagnosticsStatusNotificationRequest(DiagnosticsStatus Status)
        {

            this.Status = Status;

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
        //       <ns:diagnosticsStatusNotificationRequest>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:diagnosticsStatusNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(DiagnosticsStatusNotificationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationRequest Parse(XElement             DiagnosticsStatusNotificationRequestXML,
                                                                 OnExceptionDelegate  OnException = null)
        {

            DiagnosticsStatusNotificationRequest _DiagnosticsStatusNotificationRequest;

            if (TryParse(DiagnosticsStatusNotificationRequestXML, out _DiagnosticsStatusNotificationRequest, OnException))
                return _DiagnosticsStatusNotificationRequest;

            return null;

        }

        #endregion

        #region (static) Parse(DiagnosticsStatusNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationRequest Parse(String               DiagnosticsStatusNotificationRequestText,
                                                                 OnExceptionDelegate  OnException = null)
        {

            DiagnosticsStatusNotificationRequest _DiagnosticsStatusNotificationRequest;

            if (TryParse(DiagnosticsStatusNotificationRequestText, out _DiagnosticsStatusNotificationRequest, OnException))
                return _DiagnosticsStatusNotificationRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(DiagnosticsStatusNotificationRequestXML,  out DiagnosticsStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestXML">The XML to parse.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed diagnostics status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                                  DiagnosticsStatusNotificationRequestXML,
                                       out DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest,
                                       OnExceptionDelegate                       OnException  = null)
        {

            try
            {

                DiagnosticsStatusNotificationRequest = new DiagnosticsStatusNotificationRequest(

                                                           DiagnosticsStatusNotificationRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                                                                  XML_IO.AsDiagnosticsStatus)

                                                       );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, DiagnosticsStatusNotificationRequestXML, e);

                DiagnosticsStatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(DiagnosticsStatusNotificationRequestText, out DiagnosticsStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestText">The text to parse.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed diagnostics status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                                    DiagnosticsStatusNotificationRequestText,
                                       out DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest,
                                       OnExceptionDelegate                       OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(DiagnosticsStatusNotificationRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out DiagnosticsStatusNotificationRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, DiagnosticsStatusNotificationRequestText, e);
            }

            DiagnosticsStatusNotificationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationRequest",
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two diagnostics status notification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A diagnostics status notification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another diagnostics status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) DiagnosticsStatusNotificationRequest1 == null) || ((Object) DiagnosticsStatusNotificationRequest2 == null))
                return false;

            return DiagnosticsStatusNotificationRequest1.Equals(DiagnosticsStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two diagnostics status notification requests for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A diagnostics status notification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another diagnostics status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest2)

            => !(DiagnosticsStatusNotificationRequest1 == DiagnosticsStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationRequest> Members

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

            // Check if the given object is a diagnostics status notification request.
            var DiagnosticsStatusNotificationRequest = Object as DiagnosticsStatusNotificationRequest;
            if ((Object) DiagnosticsStatusNotificationRequest == null)
                return false;

            return this.Equals(DiagnosticsStatusNotificationRequest);

        }

        #endregion

        #region Equals(DiagnosticsStatusNotificationRequest)

        /// <summary>
        /// Compares two diagnostics status notification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest)
        {

            if ((Object) DiagnosticsStatusNotificationRequest == null)
                return false;

            return Status.Equals(DiagnosticsStatusNotificationRequest.Status);

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
