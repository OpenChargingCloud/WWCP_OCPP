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
    /// An OCPP status notification request.
    /// </summary>
    public class StatusNotificationRequest
    {

        #region Properties

        /// <summary>
        /// The connector identification at the charge point.
        /// Id '0' (zero) is used if the status is for the charge point main controller.
        /// </summary>
        public Connector_Id           ConnectorId       { get; }

        /// <summary>
        /// The current status of the charge point.
        /// </summary>
        public ChargePointStatus      Status            { get; }

        /// <summary>
        /// The error code reported by the charge point.
        /// </summary>
        public ChargePointErrorCodes  ErrorCode         { get; }

        /// <summary>
        /// Additional free format information related to the error.
        /// </summary>
        public String                 Info              { get; }

        /// <summary>
        /// The time for which the status is reported.
        /// </summary>
        public DateTime?              StatusTimestamp   { get; }

        /// <summary>
        /// This identifies the vendor-specific implementation.
        /// </summary>
        public String                 VendorId          { get; }

        /// <summary>
        /// A vendor-specific error code.
        /// </summary>
        public String                 VendorErrorCode   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP StartTransaction XML/SOAP request.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="Status">The current status of the charge point.</param>
        /// <param name="ErrorCode">The error code reported by the charge point.</param>
        /// <param name="Info">Additional free format information related to the error.</param>
        /// <param name="StatusTimestamp">The time for which the status is reported.</param>
        /// <param name="VendorId">This identifies the vendor-specific implementation.</param>
        /// <param name="VendorErrorCode">A vendor-specific error code.</param>
        public StatusNotificationRequest(Connector_Id           ConnectorId,
                                         ChargePointStatus      Status,
                                         ChargePointErrorCodes  ErrorCode,
                                         String                 Info             = null,
                                         DateTime?              StatusTimestamp  = null,
                                         String                 VendorId         = null,
                                         String                 VendorErrorCode  = null)
        {

            #region Initial checks

            if (ConnectorId == null)
                throw new ArgumentNullException(nameof(ConnectorId),  "The given connector identification must not be null!");

            #endregion

            this.ConnectorId      = ConnectorId;
            this.Status           = Status;
            this.ErrorCode        = ErrorCode;

            this.Info             = Info.           Trim().IsNotNullOrEmpty() ? Info.           Trim() : "";
            this.StatusTimestamp  = StatusTimestamp ?? new DateTime?();
            this.VendorId         = VendorId.       Trim().IsNotNullOrEmpty() ? VendorId.       Trim() : "";
            this.VendorErrorCode  = VendorErrorCode.Trim().IsNotNullOrEmpty() ? VendorErrorCode.Trim() : "";

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
        //       <ns:statusNotificationRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:status>?</ns:status>
        //          <ns:errorCode>?</ns:errorCode>
        //
        //          <!--Optional:-->
        //          <ns:info>?</ns:info>
        //
        //          <!--Optional:-->
        //          <ns:timestamp>?</ns:timestamp>
        //
        //          <!--Optional:-->
        //          <ns:vendorId>?</ns:vendorId>
        //
        //          <!--Optional:-->
        //          <ns:vendorErrorCode>?</ns:vendorErrorCode>
        //
        //       </ns:statusNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(StatusNotificationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP status notification request.
        /// </summary>
        /// <param name="StatusNotificationRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationRequest Parse(XElement             StatusNotificationRequestXML,
                                                      OnExceptionDelegate  OnException = null)
        {

            StatusNotificationRequest _StatusNotificationRequest;

            if (TryParse(StatusNotificationRequestXML, out _StatusNotificationRequest, OnException))
                return _StatusNotificationRequest;

            return null;

        }

        #endregion

        #region (static) Parse(StatusNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP status notification request.
        /// </summary>
        /// <param name="StatusNotificationRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationRequest Parse(String               StatusNotificationRequestText,
                                                      OnExceptionDelegate  OnException = null)
        {

            StatusNotificationRequest _StatusNotificationRequest;

            if (TryParse(StatusNotificationRequestText, out _StatusNotificationRequest, OnException))
                return _StatusNotificationRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(StatusNotificationRequestXML,  out StatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP status notification request.
        /// </summary>
        /// <param name="StatusNotificationRequestXML">The XML to parse.</param>
        /// <param name="StatusNotificationRequest">The parsed status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                       StatusNotificationRequestXML,
                                       out StatusNotificationRequest  StatusNotificationRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                StatusNotificationRequest = new StatusNotificationRequest(

                                                StatusNotificationRequestXML.MapValueOrFail       (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                                                   Connector_Id.Parse),

                                                StatusNotificationRequestXML.MapEnumValuesOrFail  (OCPPNS.OCPPv1_6_CS + "status",
                                                                                                   XML_IO.AsChargePointStatus),

                                                StatusNotificationRequestXML.MapEnumValuesOrFail  (OCPPNS.OCPPv1_6_CS + "errorCode",
                                                                                                   XML_IO.AsChargePointErrorCodes),

                                                StatusNotificationRequestXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "info"),

                                                StatusNotificationRequestXML.MapValueOrNullable   (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                                                   DateTime.Parse),

                                                StatusNotificationRequestXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "vendorId"),

                                                StatusNotificationRequestXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "vendorErrorCode")

                                            );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, StatusNotificationRequestXML, e);

                StatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StatusNotificationRequestText, out StatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP status notification request.
        /// </summary>
        /// <param name="StatusNotificationRequestText">The text to parse.</param>
        /// <param name="StatusNotificationRequest">The parsed status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                         StatusNotificationRequestText,
                                       out StatusNotificationRequest  StatusNotificationRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(StatusNotificationRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out StatusNotificationRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, StatusNotificationRequestText, e);
            }

            StatusNotificationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "statusNotificationRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",            ConnectorId.Value),
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",                 XML_IO.AsText(Status)),
                   new XElement(OCPPNS.OCPPv1_6_CS + "errorCode",              XML_IO.AsText(ErrorCode)),

                   Info.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "info",             Info)
                       : null,

                   StatusTimestamp.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "timestamp",        StatusTimestamp.Value.ToIso8601())
                       : null,

                   VendorId.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "vendorId",         VendorId)
                       : null,

                   VendorErrorCode.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "vendorErrorCode",  VendorErrorCode)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (StatusNotificationRequest1, StatusNotificationRequest2)

        /// <summary>
        /// Compares two status notification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A status notification request.</param>
        /// <param name="StatusNotificationRequest2">Another status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StatusNotificationRequest StatusNotificationRequest1, StatusNotificationRequest StatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(StatusNotificationRequest1, StatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) StatusNotificationRequest1 == null) || ((Object) StatusNotificationRequest2 == null))
                return false;

            return StatusNotificationRequest1.Equals(StatusNotificationRequest2);

        }

        #endregion

        #region Operator != (StatusNotificationRequest1, StatusNotificationRequest2)

        /// <summary>
        /// Compares two status notification requests for inequality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A status notification request.</param>
        /// <param name="StatusNotificationRequest2">Another status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationRequest StatusNotificationRequest1, StatusNotificationRequest StatusNotificationRequest2)

            => !(StatusNotificationRequest1 == StatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationRequest> Members

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

            // Check if the given object is a status notification request.
            var StatusNotificationRequest = Object as StatusNotificationRequest;
            if ((Object) StatusNotificationRequest == null)
                return false;

            return this.Equals(StatusNotificationRequest);

        }

        #endregion

        #region Equals(StatusNotificationRequest)

        /// <summary>
        /// Compares two status notification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest">A status notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StatusNotificationRequest StatusNotificationRequest)
        {

            if ((Object) StatusNotificationRequest == null)
                return false;

            return ConnectorId.    Equals(StatusNotificationRequest.ConnectorId) &&
                   Status.         Equals(StatusNotificationRequest.Status)      &&
                   ErrorCode.      Equals(StatusNotificationRequest.ErrorCode)   &&

                   Info.Equals(StatusNotificationRequest.Info) &&

                   ((!StatusTimestamp.HasValue && !StatusNotificationRequest.StatusTimestamp.HasValue) ||
                     (StatusTimestamp.HasValue && StatusNotificationRequest.StatusTimestamp.HasValue && StatusTimestamp.Value.Equals(StatusNotificationRequest.StatusTimestamp.Value))) &&

                   VendorId.       Equals(StatusNotificationRequest.VendorId)    &&
                   VendorErrorCode.Equals(StatusNotificationRequest.VendorErrorCode);

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

                return ConnectorId.GetHashCode() * 37 ^
                       Status.     GetHashCode() * 31 ^
                       ErrorCode.  GetHashCode() * 29 ^
                       Info.       GetHashCode() * 23 ^

                       (StatusTimestamp.HasValue
                            ? StatusTimestamp.GetHashCode()
                            : 0) * 19 ^

                       ErrorCode.  GetHashCode() * 11 ^
                       Info.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId,
                             " / ", Status,
                             " / ", ErrorCode);

        #endregion


    }

}
