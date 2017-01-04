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
    /// An OCPP firmware status notification request.
    /// </summary>
    public class FirmwareStatusNotificationRequest : ARequest<FirmwareStatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The current status of a firmware installation.
        /// </summary>
        public FirmwareStatus  Status   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP FirmwareStatusNotificationRequest XML/SOAP request.
        /// </summary>
        /// <param name="Status">The current status of a firmware installation.</param>
        public FirmwareStatusNotificationRequest(FirmwareStatus Status)
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
        //       <ns:firmwareStatusNotificationRequest>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:firmwareStatusNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(FirmwareStatusNotificationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationRequest Parse(XElement             FirmwareStatusNotificationRequestXML,
                                                              OnExceptionDelegate  OnException = null)
        {

            FirmwareStatusNotificationRequest _FirmwareStatusNotificationRequest;

            if (TryParse(FirmwareStatusNotificationRequestXML, out _FirmwareStatusNotificationRequest, OnException))
                return _FirmwareStatusNotificationRequest;

            return null;

        }

        #endregion

        #region (static) Parse(FirmwareStatusNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationRequest Parse(String               FirmwareStatusNotificationRequestText,
                                                              OnExceptionDelegate  OnException = null)
        {

            FirmwareStatusNotificationRequest _FirmwareStatusNotificationRequest;

            if (TryParse(FirmwareStatusNotificationRequestText, out _FirmwareStatusNotificationRequest, OnException))
                return _FirmwareStatusNotificationRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(FirmwareStatusNotificationRequestXML,  out FirmwareStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestXML">The XML to parse.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                               FirmwareStatusNotificationRequestXML,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       OnExceptionDelegate                    OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(

                                                        FirmwareStatusNotificationRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                                                            XML_IO.AsFirmwareStatus)

                                                    );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, FirmwareStatusNotificationRequestXML, e);

                FirmwareStatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(FirmwareStatusNotificationRequestText, out FirmwareStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestText">The text to parse.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                                 FirmwareStatusNotificationRequestText,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       OnExceptionDelegate                    OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(FirmwareStatusNotificationRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out FirmwareStatusNotificationRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, FirmwareStatusNotificationRequestText, e);
            }

            FirmwareStatusNotificationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "firmwareStatusNotificationRequest",
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) FirmwareStatusNotificationRequest1 == null) || ((Object) FirmwareStatusNotificationRequest2 == null))
                return false;

            return FirmwareStatusNotificationRequest1.Equals(FirmwareStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest2)

            => !(FirmwareStatusNotificationRequest1 == FirmwareStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationRequest> Members

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

            // Check if the given object is a firmware status notification request.
            var FirmwareStatusNotificationRequest = Object as FirmwareStatusNotificationRequest;
            if ((Object) FirmwareStatusNotificationRequest == null)
                return false;

            return this.Equals(FirmwareStatusNotificationRequest);

        }

        #endregion

        #region Equals(FirmwareStatusNotificationRequest)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest)
        {

            if ((Object) FirmwareStatusNotificationRequest == null)
                return false;

            return Status.Equals(FirmwareStatusNotificationRequest.Status);

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
