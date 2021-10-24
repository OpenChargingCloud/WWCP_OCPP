/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    //After start-up, every charge point SHALL send a request to the
    // central system with information about its configuration
    // (e.g.version, vendor, etc.).

    /// <summary>
    /// The BootNotification request.
    /// </summary>
    public class BootNotificationRequest : ARequest<BootNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        [Mandatory]
        public String ChargePointVendor         { get; }

        /// <summary>
        /// The charge point model identification.
        /// </summary>
        [Mandatory]
        public String ChargePointModel          { get; }

        /// <summary>
        /// The serial number of the charge point.
        /// </summary>
        [Optional]
        public String ChargePointSerialNumber   { get; }

        /// <summary>
        /// The serial number of the charge box.
        /// </summary>
        [Optional]
        public String ChargeBoxSerialNumber     { get; }

        /// <summary>
        /// The firmware version of the charge point.
        /// </summary>
        [Optional]
        public String FirmwareVersion           { get; }

        /// <summary>
        /// The ICCID of the charge point's SIM card.
        /// </summary>
        [Optional]
        public String Iccid                     { get; }

        /// <summary>
        /// The IMSI of the charge point’s SIM card.
        /// </summary>
        [Optional]
        public String IMSI                      { get; }

        /// <summary>
        /// The meter type of the main power meter of the charge point.
        /// </summary>
        [Optional]
        public String MeterType                 { get; }

        /// <summary>
        /// The serial number of the main power meter of the charge point.
        /// </summary>
        [Optional]
        public String MeterSerialNumber         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new BootNotification request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// 
        /// <param name="ChargePointSerialNumber">The optional serial number of the charge point.</param>
        /// <param name="ChargeBoxSerialNumber">The optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">The optional firmware version of the charge point.</param>
        /// <param name="Iccid">The optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">The optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">The optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">The optional serial number of the main power meter of the charge point.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        public BootNotificationRequest(ChargeBox_Id      ChargeBoxId,
                                       String            ChargePointVendor,
                                       String            ChargePointModel,

                                       String            ChargePointSerialNumber   = null,
                                       String            ChargeBoxSerialNumber     = null,
                                       String            FirmwareVersion           = null,
                                       String            Iccid                     = null,
                                       String            IMSI                      = null,
                                       String            MeterType                 = null,
                                       String            MeterSerialNumber         = null,

                                       Request_Id?       RequestId                 = null,
                                       DateTime?         RequestTimestamp          = null,
                                       EventTracking_Id  EventTrackingId           = null)

            : base(ChargeBoxId,
                   "BootNotification",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

        {

            this.ChargePointVendor        = ChargePointVendor?.      Trim();
            this.ChargePointModel         = ChargePointModel?.       Trim();
            this.ChargePointSerialNumber  = ChargePointSerialNumber?.Trim();
            this.ChargeBoxSerialNumber    = ChargeBoxSerialNumber?.  Trim();
            this.FirmwareVersion          = FirmwareVersion?.        Trim();
            this.Iccid                    = Iccid?.                  Trim();
            this.IMSI                     = IMSI?.                   Trim();
            this.MeterType                = MeterType?.              Trim();
            this.MeterSerialNumber        = MeterSerialNumber?.      Trim();


            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),    "The given charge point vendor identification must not be null or empty!");

            if (ChargePointVendor.Length > 20)
                throw new ArgumentException    (nameof(ChargePointVendor),    "The given charge point vendor identification is too long!");

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),     "The given charge point model identification must not be null or empty!");

            if (ChargePointModel.Length > 20)
                throw new ArgumentException    (nameof(ChargePointModel),     "The given charge point model identification is too long!");


            if (ChargePointSerialNumber?.Length > 25)
                throw new ArgumentException(nameof(ChargePointSerialNumber),  "The given charge point serial number is too long!");

            if (ChargeBoxSerialNumber?.  Length > 25)
                throw new ArgumentException(nameof(ChargeBoxSerialNumber),    "The given charge box serial number is too long!");

            if (FirmwareVersion?.        Length > 50)
                throw new ArgumentException(nameof(FirmwareVersion),          "The given firmware version is too long!");

            if (Iccid?.                  Length > 20)
                throw new ArgumentException(nameof(Iccid),                    "The given Iccid is too long!");

            if (IMSI?.                   Length > 20)
                throw new ArgumentException(nameof(IMSI),                     "The given IMSI is too long!");

            if (MeterType?.              Length > 25)
                throw new ArgumentException(nameof(MeterType),                "The given meter type is too long!");

            if (MeterSerialNumber?.      Length > 25)
                throw new ArgumentException(nameof(MeterSerialNumber),        "The given meter serial number is too long!");

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

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:BootNotificationRequest",
        //     "title":   "BootNotificationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "chargePointVendor": {
        //             "type":      "string",
        //             "maxLength": 20
        //         },
        //         "chargePointModel": {
        //             "type":      "string",
        //             "maxLength": 20
        //         },
        //         "chargePointSerialNumber": {
        //             "type":      "string",
        //             "maxLength": 25
        //         },
        //         "chargeBoxSerialNumber": {
        //             "type":      "string",
        //             "maxLength": 25
        //         },
        //         "firmwareVersion": {
        //             "type":      "string",
        //             "maxLength": 50
        //         },
        //         "iccid": {
        //             "type":      "string",
        //             "maxLength": 20
        //         },
        //         "imsi": {
        //             "type":      "string",
        //             "maxLength": 20
        //         },
        //         "meterType": {
        //             "type":      "string",
        //             "maxLength": 25
        //         },
        //         "meterSerialNumber": {
        //             "type":      "string",
        //             "maxLength": 25
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "chargePointVendor",
        //         "chargePointModel"
        //     ]
        // }

        // [
        //     2,
        //    "19223201",
        //    "BootNotification",
        //    {
        //        "chargePointVendor": "VendorX",
        //        "chargePointModel":  "SingleSocketCharger"
        //    }
        // ]

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a BootNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationRequest Parse(XElement             XML,
                                                    Request_Id           RequestId,
                                                    ChargeBox_Id         ChargeBoxId,
                                                    OnExceptionDelegate  OnException = null)
        {


            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out BootNotificationRequest bootNotificationRequest,
                         OnException))
            {
                return bootNotificationRequest;
            }

            throw new ArgumentException("The given XML representation of a BootNotification request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomBootNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a BootNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomBootNotificationRequestParser">A delegate to parse custom BootNotification requests.</param>
        public static BootNotificationRequest Parse(JObject                                               JSON,
                                                    Request_Id                                            RequestId,
                                                    ChargeBox_Id                                          ChargeBoxId,
                                                    CustomJObjectParserDelegate<BootNotificationRequest>  CustomBootNotificationRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out BootNotificationRequest  bootNotificationRequest,
                         out String                   ErrorResponse,
                         CustomBootNotificationRequestParser))
            {
                return bootNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a BootNotification request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a BootNotification request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationRequest Parse(String               Text,
                                                    Request_Id           RequestId,
                                                    ChargeBox_Id         ChargeBoxId,
                                                    OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out BootNotificationRequest bootNotificationRequest,
                         OnException))
            {
                return bootNotificationRequest;
            }

            throw new ArgumentException("The given text representation of a BootNotification request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out BootNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a BootNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                     XML,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out BootNotificationRequest  BootNotificationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                BootNotificationRequest = new BootNotificationRequest(
                                              ChargeBoxId,
                                              XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargePointVendor"),
                                              XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargePointModel"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "chargePointSerialNumber"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "chargeBoxSerialNumber"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "firmwareVersion"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "iccid"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "imsi"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "meterType"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "meterSerialNumber"),
                                              RequestId
                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, XML, e);

                BootNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out BootNotificationRequest, out ErrorResponse, CustomBootNotificationRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a BootNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out BootNotificationRequest  BootNotificationRequest,
                                       out String                   ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out BootNotificationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a BootNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBootNotificationRequestParser">A delegate to parse custom BootNotification requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       ChargeBox_Id                                          ChargeBoxId,
                                       out BootNotificationRequest                           BootNotificationRequest,
                                       out String                                            ErrorResponse,
                                       CustomJObjectParserDelegate<BootNotificationRequest>  CustomBootNotificationRequestParser)
        {

            try
            {

                BootNotificationRequest = null;

                #region ChargePointVendor    [mandatory]

                if (!JSON.ParseMandatoryText("chargePointVendor",
                                             "charge point vendor",
                                             out String ChargePointVendor,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargePointModel     [mandatory]

                if (!JSON.ParseMandatoryText("chargePointModel",
                                             "charge point model",
                                             out String ChargePointModel,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId          [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                BootNotificationRequest = new BootNotificationRequest(
                                              ChargeBoxId,
                                              ChargePointVendor,
                                              ChargePointModel,
                                              JSON["chargePointSerialNumber"]?.Value<String>(),
                                              JSON["chargeBoxSerialNumber"]?.  Value<String>(),
                                              JSON["firmwareVersion"]?.        Value<String>(),
                                              JSON["iccid"]?.                  Value<String>(),
                                              JSON["imsi"]?.                   Value<String>(),
                                              JSON["meterType"]?.              Value<String>(),
                                              JSON["meterSerialNumber"]?.      Value<String>(),
                                              RequestId
                                          );

                if (CustomBootNotificationRequestParser != null)
                    BootNotificationRequest = CustomBootNotificationRequestParser(JSON,
                                                                                  BootNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationRequest  = default;
                ErrorResponse            = "The given JSON representation of a BootNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out BootNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a BootNotification request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       Text,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out BootNotificationRequest  BootNotificationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                Text = Text?.Trim();

                if (Text.IsNotNullOrEmpty())
                {

                    if (Text.StartsWith("{") &&
                        TryParse(JObject.Parse(Text),
                                 RequestId,
                                 ChargeBoxId,
                                 out BootNotificationRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out BootNotificationRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, Text, e);
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

                   new XElement(OCPPNS.OCPPv1_6_CS + "chargePointVendor",              ChargePointVendor),
                   new XElement(OCPPNS.OCPPv1_6_CS + "chargePointModel",               ChargePointModel),

                   ChargePointSerialNumber.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "chargePointSerialNumber",  ChargePointSerialNumber)
                       : null,

                   ChargeBoxSerialNumber.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "chargeBoxSerialNumber",    ChargeBoxSerialNumber)
                       : null,

                   ChargePointSerialNumber.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "firmwareVersion",          FirmwareVersion)
                       : null,

                   ChargeBoxSerialNumber.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "iccid",                    Iccid)
                       : null,

                   ChargePointSerialNumber.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "imsi",                     IMSI)
                       : null,

                   ChargeBoxSerialNumber.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "meterType",                MeterType)
                       : null,

                   ChargePointSerialNumber.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "meterSerialNumber",        MeterSerialNumber)
                       : null

               );

        #endregion

        #region ToJSON(CustomBootNotificationRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationRequestSerializer">A delegate to serialize custom BootNotification requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BootNotificationRequest> CustomBootNotificationRequestSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("chargePointVendor",               ChargePointVendor),
                           new JProperty("chargePointModel",                ChargePointModel),

                           ChargePointSerialNumber.IsNotNullOrEmpty()
                               ? new JProperty("chargePointSerialNumber",   ChargePointSerialNumber)
                               : null,

                           ChargeBoxSerialNumber.  IsNotNullOrEmpty()
                               ? new JProperty("chargeBoxSerialNumber",     ChargeBoxSerialNumber)
                               : null,

                           FirmwareVersion.        IsNotNullOrEmpty()
                               ? new JProperty("firmwareVersion",           FirmwareVersion)
                               : null,

                           Iccid.                  IsNotNullOrEmpty()
                               ? new JProperty("iccid",                     Iccid)
                               : null,

                           IMSI.                   IsNotNullOrEmpty()
                               ? new JProperty("imsi",                      IMSI)
                               : null,

                           MeterType.              IsNotNullOrEmpty()
                               ? new JProperty("meterType",                 MeterType)
                               : null,

                           MeterSerialNumber.      IsNotNullOrEmpty()
                               ? new JProperty("meterSerialNumber",         MeterSerialNumber)
                               : null

                       );

            return CustomBootNotificationRequestSerializer != null
                       ? CustomBootNotificationRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two BootNotification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A BootNotification request.</param>
        /// <param name="BootNotificationRequest2">Another BootNotification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationRequest BootNotificationRequest1, BootNotificationRequest BootNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationRequest1, BootNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((BootNotificationRequest1 is null) || (BootNotificationRequest2 is null))
                return false;

            return BootNotificationRequest1.Equals(BootNotificationRequest2);

        }

        #endregion

        #region Operator != (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two BootNotification requests for inequality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A BootNotification request.</param>
        /// <param name="BootNotificationRequest2">Another BootNotification request.</param>
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

            if (Object is null)
                return false;

            if (!(Object is BootNotificationRequest BootNotificationRequest))
                return false;

            return Equals(BootNotificationRequest);

        }

        #endregion

        #region Equals(BootNotificationRequest)

        /// <summary>
        /// Compares two BootNotification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest">A BootNotification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(BootNotificationRequest BootNotificationRequest)
        {

            if (BootNotificationRequest is null)
                return false;

            return ChargePointVendor.Equals(BootNotificationRequest.ChargePointVendor) &&
                   ChargePointModel.Equals(BootNotificationRequest.ChargePointModel) &&

                   ((ChargePointSerialNumber == null && BootNotificationRequest.ChargePointSerialNumber == null) ||
                    (ChargePointSerialNumber != null && BootNotificationRequest.ChargePointSerialNumber != null &&
                     String.Equals(ChargePointSerialNumber,
                                   BootNotificationRequest.ChargePointSerialNumber,
                                   StringComparison.OrdinalIgnoreCase))) &&

                   ((ChargeBoxSerialNumber   == null && BootNotificationRequest.ChargeBoxSerialNumber   == null) ||
                    (ChargeBoxSerialNumber   != null && BootNotificationRequest.ChargeBoxSerialNumber   != null &&
                     String.Equals(ChargeBoxSerialNumber,
                                   BootNotificationRequest.ChargeBoxSerialNumber,
                                   StringComparison.OrdinalIgnoreCase))) &&

                   ((ChargePointSerialNumber == null && BootNotificationRequest.ChargePointSerialNumber == null) ||
                    (ChargePointSerialNumber != null && BootNotificationRequest.ChargePointSerialNumber != null &&
                     String.Equals(ChargePointSerialNumber,
                                   BootNotificationRequest.ChargePointSerialNumber,
                                   StringComparison.OrdinalIgnoreCase))) &&

                   ((FirmwareVersion         == null && BootNotificationRequest.FirmwareVersion         == null) ||
                    (FirmwareVersion         != null && BootNotificationRequest.FirmwareVersion         != null &&
                     String.Equals(FirmwareVersion,
                                   BootNotificationRequest.FirmwareVersion,
                                   StringComparison.OrdinalIgnoreCase))) &&

                   ((Iccid                   == null && BootNotificationRequest.Iccid                   == null) ||
                    (Iccid                   != null && BootNotificationRequest.Iccid                   != null &&
                     String.Equals(Iccid,
                                   BootNotificationRequest.Iccid,
                                   StringComparison.OrdinalIgnoreCase))) &&

                   ((IMSI                    == null && BootNotificationRequest.IMSI                    == null) ||
                    (IMSI                    != null && BootNotificationRequest.IMSI                    != null &&
                     String.Equals(IMSI,
                                   BootNotificationRequest.IMSI,
                                   StringComparison.OrdinalIgnoreCase))) &&

                   ((MeterType               == null && BootNotificationRequest.MeterType               == null) ||
                    (MeterType               != null && BootNotificationRequest.MeterType               != null &&
                     String.Equals(MeterType,
                                   BootNotificationRequest.MeterType,
                                   StringComparison.OrdinalIgnoreCase))) &&

                   ((MeterSerialNumber       == null && BootNotificationRequest.MeterSerialNumber       == null) ||
                    (MeterSerialNumber       != null && BootNotificationRequest.MeterSerialNumber       != null &&
                     String.Equals(MeterSerialNumber,
                                   BootNotificationRequest.MeterSerialNumber,
                                   StringComparison.OrdinalIgnoreCase)));

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

                       (ChargePointSerialNumber != null
                            ? ChargePointSerialNumber.GetHashCode() * 21
                            : 0) ^

                       (ChargeBoxSerialNumber   != null
                            ? ChargeBoxSerialNumber.  GetHashCode() * 17
                            : 0) ^

                       (FirmwareVersion         != null
                            ? FirmwareVersion.        GetHashCode() * 13
                            : 0) ^

                       (Iccid                   != null
                            ? Iccid.                  GetHashCode() * 11
                            : 0) ^

                       (IMSI                    != null
                            ? IMSI.                   GetHashCode() *  7
                            : 0) ^

                       (MeterType               != null
                            ? MeterType.              GetHashCode() *  5
                            : 0) ^

                       (MeterSerialNumber       != null
                            ? MeterSerialNumber.      GetHashCode() *  3
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargePointVendor, " / ", ChargePointModel);

        #endregion

    }

}
