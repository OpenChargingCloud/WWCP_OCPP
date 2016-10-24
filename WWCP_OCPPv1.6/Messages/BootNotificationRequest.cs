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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    //After start-up, every charge point SHALL send a request to the
    // central system with information about its configuration
    // (e.g.version, vendor, etc.).

    /// <summary>
    /// An OCPP boot notification request.
    /// </summary>
    public class BootNotificationRequest
    {

        #region Properties

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        public String ChargePointVendor         { get; }

        /// <summary>
        /// The charge point model identification.
        /// </summary>
        public String ChargePointModel          { get; }

        /// <summary>
        /// The serial number of the charge point.
        /// </summary>
        public String ChargePointSerialNumber   { get; }

        /// <summary>
        /// The firmware version of the charge point.
        /// </summary>
        public String FirmwareVersion           { get; }

        /// <summary>
        /// The ICCID of the charge point's SIM card.
        /// </summary>
        public String Iccid                     { get; }

        /// <summary>
        /// The IMSI of the charge point’s SIM card.
        /// </summary>
        public String IMSI                      { get; }

        /// <summary>
        /// The meter type of the main power meter of the charge point.
        /// </summary>
        public String MeterType                 { get; }

        /// <summary>
        /// The serial number of the main power meter of the charge point.
        /// </summary>
        public String MeterSerialNumber         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP BootNotification XML/SOAP request.
        /// </summary>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// <param name="ChargePointSerialNumber">The serial number of the charge point.</param>
        /// <param name="FirmwareVersion">The firmware version of the charge point.</param>
        /// <param name="Iccid">The ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">The IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">The meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">The serial number of the main power meter of the charge point.</param>
        public BootNotificationRequest(String ChargePointVendor,                  // case-insensitive 20
                                       String ChargePointModel,                   // case-insensitive 20
                                       String ChargePointSerialNumber  = null,    // case-insensitive 25
                                       String FirmwareVersion          = null,    // case-insensitive 50
                                       String Iccid                    = null,    // case-insensitive 20
                                       String IMSI                     = null,    // case-insensitive 20
                                       String MeterType                = null,    // case-insensitive 25
                                       String MeterSerialNumber        = null)    // case-insensitive 25
        {

            #region Initial checks

            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),  "The given charge point vendor identification must not be null or empty!");

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),   "The given charge point model identification must not be null or empty!");

            #endregion

            this.ChargePointVendor        = ChargePointVendor;
            this.ChargePointModel         = ChargePointModel;
            this.ChargePointSerialNumber  = ChargePointSerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Iccid                    = Iccid;
            this.IMSI                     = IMSI;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;

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
        //       <ns:bootNotificationRequest>
        //
        //          <ns:chargePointVendor>?</ns:chargePointVendor>
        //          <ns:chargePointModel>?</ns:chargePointModel>
        //
        //          <!--Optional:-->
        //          <ns:chargePointSerialNumber>?</ns:chargePointSerialNumber>
        //
        //          <!--Optional:-->
        //          <ns:chargeBoxSerialNumber>?</ns:chargeBoxSerialNumber>
        //
        //          <!--Optional:-->
        //          <ns:firmwareVersion>?</ns:firmwareVersion>
        //
        //          <!--Optional:-->
        //          <ns:iccid>?</ns:iccid>
        //
        //          <!--Optional:-->
        //          <ns:imsi>?</ns:imsi>
        //
        //          <!--Optional:-->
        //          <ns:meterType>?</ns:meterType>
        //
        //          <!--Optional:-->
        //          <ns:meterSerialNumber>?</ns:meterSerialNumber>
        //
        //       </ns:bootNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(BootNotificationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP boot notification request.
        /// </summary>
        /// <param name="BootNotificationRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationRequest Parse(XElement             BootNotificationRequestXML,
                                                    OnExceptionDelegate  OnException = null)
        {

            BootNotificationRequest _BootNotificationRequest;

            if (TryParse(BootNotificationRequestXML, out _BootNotificationRequest, OnException))
                return _BootNotificationRequest;

            return null;

        }

        #endregion

        #region (static) Parse(BootNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP boot notification request.
        /// </summary>
        /// <param name="BootNotificationRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationRequest Parse(String               BootNotificationRequestText,
                                                    OnExceptionDelegate  OnException = null)
        {

            BootNotificationRequest _BootNotificationRequest;

            if (TryParse(BootNotificationRequestText, out _BootNotificationRequest, OnException))
                return _BootNotificationRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(BootNotificationRequestXML,  out BootNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP boot notification request.
        /// </summary>
        /// <param name="BootNotificationRequestXML">The XML to parse.</param>
        /// <param name="BootNotificationRequest">The parsed boot notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              BootNotificationRequestXML,
                                       out BootNotificationRequest  BootNotificationRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                BootNotificationRequest = new BootNotificationRequest(

                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "chargePointVendor"),
                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "chargePointModel"),
                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "chargePointSerialNumber"),
                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "firmwareVersion"),
                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "iccid"),
                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "imsi"),
                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "meterType"),
                                              BootNotificationRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "meterSerialNumber")

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, BootNotificationRequestXML, e);

                BootNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(BootNotificationRequestText, out BootNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP boot notification request.
        /// </summary>
        /// <param name="BootNotificationRequestText">The text to parse.</param>
        /// <param name="BootNotificationRequest">The parsed boot notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       BootNotificationRequestText,
                                       out BootNotificationRequest  BootNotificationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(BootNotificationRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out BootNotificationRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, BootNotificationRequestText, e);
            }

            BootNotificationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "bootNotificationRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "chargePointVendor",        ChargePointVendor),
                   new XElement(OCPPNS.OCPPv1_6_CS + "chargePointModel",         ChargePointModel),
                   new XElement(OCPPNS.OCPPv1_6_CS + "chargePointSerialNumber",  ChargePointSerialNumber),
                   new XElement(OCPPNS.OCPPv1_6_CS + "firmwareVersion",          FirmwareVersion),
                   new XElement(OCPPNS.OCPPv1_6_CS + "iccid",                    Iccid),
                   new XElement(OCPPNS.OCPPv1_6_CS + "imsi",                     IMSI),
                   new XElement(OCPPNS.OCPPv1_6_CS + "meterType",                MeterType),
                   new XElement(OCPPNS.OCPPv1_6_CS + "meterSerialNumber",        MeterSerialNumber)

               );

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A boot notification request.</param>
        /// <param name="BootNotificationRequest2">Another boot notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationRequest BootNotificationRequest1, BootNotificationRequest BootNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(BootNotificationRequest1, BootNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) BootNotificationRequest1 == null) || ((Object) BootNotificationRequest2 == null))
                return false;

            return BootNotificationRequest1.Equals(BootNotificationRequest2);

        }

        #endregion

        #region Operator != (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two boot notification requests for inequality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A boot notification request.</param>
        /// <param name="BootNotificationRequest2">Another boot notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BootNotificationRequest BootNotificationRequest1, BootNotificationRequest BootNotificationRequest2)

            => !(BootNotificationRequest1 == BootNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationRequest> Members

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

            // Check if the given object is a boot notification request.
            var BootNotificationRequest = Object as BootNotificationRequest;
            if ((Object) BootNotificationRequest == null)
                return false;

            return this.Equals(BootNotificationRequest);

        }

        #endregion

        #region Equals(BootNotificationRequest)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest">A boot notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(BootNotificationRequest BootNotificationRequest)
        {

            if ((Object) BootNotificationRequest == null)
                return false;

            return ChargePointVendor      .Equals(BootNotificationRequest.ChargePointVendor)       &&
                   ChargePointModel       .Equals(BootNotificationRequest.ChargePointModel)        &&
                   ChargePointSerialNumber.Equals(BootNotificationRequest.ChargePointSerialNumber) &&
                   FirmwareVersion        .Equals(BootNotificationRequest.FirmwareVersion)         &&
                   Iccid                  .Equals(BootNotificationRequest.Iccid)                   &&
                   IMSI                   .Equals(BootNotificationRequest.IMSI)                    &&
                   MeterType              .Equals(BootNotificationRequest.MeterType)               &&
                   MeterSerialNumber      .Equals(BootNotificationRequest.MeterSerialNumber);

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

                return ChargePointVendor.      GetHashCode() * 31 ^
                       ChargePointModel.       GetHashCode() * 29 ^
                       ChargePointSerialNumber.GetHashCode() * 23 ^
                       FirmwareVersion.        GetHashCode() * 17 ^
                       Iccid.                  GetHashCode() * 11 ^
                       IMSI.                   GetHashCode() *  7 ^
                       MeterType.              GetHashCode() *  5 ^
                       MeterSerialNumber.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargePointVendor, " / ", ChargePointModel);

        #endregion


    }

}
