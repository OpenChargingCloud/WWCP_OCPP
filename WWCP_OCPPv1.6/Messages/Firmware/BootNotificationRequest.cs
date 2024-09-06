/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    //After start-up, every charge point SHALL send a request to the
    // central system with information about its configuration
    // (e.g.version, vendor, etc.).

    /// <summary>
    /// The BootNotification request.
    /// </summary>
    public class BootNotificationRequest : ARequest<BootNotificationRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/bootNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        [Mandatory]
        public String         ChargePointVendor         { get; }

        /// <summary>
        /// The charge point model identification.
        /// </summary>
        [Mandatory]
        public String         ChargePointModel          { get; }

        /// <summary>
        /// The serial number of the charge point.
        /// </summary>
        [Optional]
        public String?        ChargePointSerialNumber   { get; }

        /// <summary>
        /// The serial number of the charge box.
        /// </summary>
        [Optional]
        public String?        ChargeBoxSerialNumber     { get; }

        /// <summary>
        /// The firmware version of the charge point.
        /// </summary>
        [Optional]
        public String?        FirmwareVersion           { get; }

        /// <summary>
        /// The ICCID of the charge point's SIM card.
        /// </summary>
        [Optional]
        public String?        Iccid                     { get; }

        /// <summary>
        /// The IMSI of the charge point’s SIM card.
        /// </summary>
        [Optional]
        public String?        IMSI                      { get; }

        /// <summary>
        /// The meter type of the main power meter of the charge point.
        /// </summary>
        [Optional]
        public String?        MeterType                 { get; }

        /// <summary>
        /// The serial number of the main power meter of the charge point.
        /// </summary>
        [Optional]
        public String?        MeterSerialNumber         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new BootNotification request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
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
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public BootNotificationRequest(SourceRouting            Destination,
                                       String                   ChargePointVendor,
                                       String                   ChargePointModel,

                                       String?                  ChargePointSerialNumber   = null,
                                       String?                  ChargeBoxSerialNumber     = null,
                                       String?                  FirmwareVersion           = null,
                                       String?                  Iccid                     = null,
                                       String?                  IMSI                      = null,
                                       String?                  MeterType                 = null,
                                       String?                  MeterSerialNumber         = null,

                                       IEnumerable<KeyPair>?    SignKeys                  = null,
                                       IEnumerable<SignInfo>?   SignInfos                 = null,
                                       IEnumerable<Signature>?  Signatures                = null,

                                       CustomData?              CustomData                = null,

                                       Request_Id?              RequestId                 = null,
                                       DateTime?                RequestTimestamp          = null,
                                       TimeSpan?                RequestTimeout            = null,
                                       EventTracking_Id?        EventTrackingId           = null,
                                       NetworkPath?             NetworkPath               = null,
                                       SerializationFormats?    SerializationFormat       = null,
                                       CancellationToken        CancellationToken         = default)

            : base(Destination,
                   nameof(BootNotificationRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat,
                   CancellationToken)

        {

            this.ChargePointVendor        = ChargePointVendor.       Trim();
            this.ChargePointModel         = ChargePointModel.        Trim();
            this.ChargePointSerialNumber  = ChargePointSerialNumber?.Trim();
            this.ChargeBoxSerialNumber    = ChargeBoxSerialNumber?.  Trim();
            this.FirmwareVersion          = FirmwareVersion?.        Trim();
            this.Iccid                    = Iccid?.                  Trim();
            this.IMSI                     = IMSI?.                   Trim();
            this.MeterType                = MeterType?.              Trim();
            this.MeterSerialNumber        = MeterSerialNumber?.      Trim();


            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),
                                                "The given charge point vendor identification must not be null or empty!");

            if (ChargePointVendor.Length > 20)
                throw new ArgumentException    ("The given charge point vendor identification is too long!",
                                                nameof(ChargePointVendor));

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),
                                                "The given charge point model identification must not be null or empty!");

            if (ChargePointModel.Length > 20)
                throw new ArgumentException    ("The given charge point model identification is too long!",
                                                nameof(ChargePointModel));


            if (ChargePointSerialNumber?.Length > 25)
                throw new ArgumentException    ("The given charge point serial number is too long!",
                                                nameof(ChargePointSerialNumber));

            if (ChargeBoxSerialNumber?.  Length > 25)
                throw new ArgumentException    ("The given charge box serial number is too long!",
                                                nameof(ChargeBoxSerialNumber));

            if (FirmwareVersion?.        Length > 50)
                throw new ArgumentException    ("The given firmware version is too long!",
                                                nameof(FirmwareVersion));

            if (Iccid?.                  Length > 20)
                throw new ArgumentException    ("The given Iccid is too long!",
                                                nameof(Iccid));

            if (IMSI?.                   Length > 20)
                throw new ArgumentException    ("The given IMSI is too long!",
                                                nameof(IMSI));

            if (MeterType?.              Length > 25)
                throw new ArgumentException    ("The given meter type is too long!",
                                                nameof(MeterType));

            if (MeterSerialNumber?.      Length > 25)
                throw new ArgumentException    ("The given meter serial number is too long!",
                                                nameof(MeterSerialNumber));

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

        #region (static) Parse   (XML,  RequestId, NetworkingNodeId)

        /// <summary>
        /// Parse the given XML representation of a BootNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        public static BootNotificationRequest Parse(XElement       XML,
                                                    Request_Id     RequestId,
                                                    SourceRouting  Destination)
        {


            if (TryParse(XML,
                         RequestId,
                         Destination,
                         out var bootNotificationRequest,
                         out var errorResponse))
            {
                return bootNotificationRequest;
            }

            throw new ArgumentException("The given XML representation of a BootNotification request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomBootNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a BootNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomBootNotificationRequestParser">An optional delegate to parse custom BootNotification requests.</param>
        public static BootNotificationRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<BootNotificationRequest>?  CustomBootNotificationRequestParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var bootNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomBootNotificationRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return bootNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a BootNotification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, NetworkingNodeId, out BootNotificationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a BootNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                           XML,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       [NotNullWhen(true)]  out BootNotificationRequest?  BootNotificationRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse)
        {

            try
            {

                BootNotificationRequest = new BootNotificationRequest(
                                              Destination,
                                              XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargePointVendor"),
                                              XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "chargePointModel"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "chargePointSerialNumber"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "chargeBoxSerialNumber"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "firmwareVersion"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "iccid"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "imsi"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "meterType"),
                                              XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "meterSerialNumber"),
                                              RequestId: RequestId
                                          );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                BootNotificationRequest  = null;
                ErrorResponse            = "The given XML representation of a BootNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out BootNotificationRequest, out ErrorResponse, CustomBootNotificationRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a BootNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="BootNotificationRequest">The parsed BootNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBootNotificationRequestParser">An optional delegate to parse custom BootNotification requests.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationRequest?      BootNotificationRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<BootNotificationRequest>?  CustomBootNotificationRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
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

                #region Signatures           [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BootNotificationRequest = new BootNotificationRequest(

                                              Destination,
                                              ChargePointVendor,
                                              ChargePointModel,
                                              JSON["chargePointSerialNumber"]?.Value<String>(),
                                              JSON["chargeBoxSerialNumber"]?.  Value<String>(),
                                              JSON["firmwareVersion"]?.        Value<String>(),
                                              JSON["iccid"]?.                  Value<String>(),
                                              JSON["imsi"]?.                   Value<String>(),
                                              JSON["meterType"]?.              Value<String>(),
                                              JSON["meterSerialNumber"]?.      Value<String>(),

                                              null,
                                              null,
                                              Signatures,

                                              CustomData,

                                              RequestId,
                                              null,
                                              null,
                                              null,
                                              NetworkPath

                                          );

                if (CustomBootNotificationRequestParser is not null)
                    BootNotificationRequest = CustomBootNotificationRequestParser(JSON,
                                                                                  BootNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationRequest  = null;
                ErrorResponse            = "The given JSON representation of a BootNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "bootNotificationRequest",

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

        #region ToJSON(CustomBootNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationRequestSerializer">A delegate to serialize custom BootNotification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargePointVendor",         ChargePointVendor),
                                 new JProperty("chargePointModel",          ChargePointModel),

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
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",                new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomBootNotificationRequestSerializer is not null
                       ? CustomBootNotificationRequestSerializer(this, json)
                       : json;

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
        public static Boolean operator == (BootNotificationRequest? BootNotificationRequest1,
                                           BootNotificationRequest? BootNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationRequest1, BootNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (BootNotificationRequest1 is null || BootNotificationRequest2 is null)
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
        public static Boolean operator != (BootNotificationRequest? BootNotificationRequest1,
                                           BootNotificationRequest? BootNotificationRequest2)

            => !(BootNotificationRequest1 == BootNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two BootNotification requests for equality.
        /// </summary>
        /// <param name="Object">A BootNotification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BootNotificationRequest bootNotificationRequest &&
                   Equals(bootNotificationRequest);

        #endregion

        #region Equals(BootNotificationRequest)

        /// <summary>
        /// Compares two BootNotification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest">A BootNotification request to compare with.</param>
        public override Boolean Equals(BootNotificationRequest? BootNotificationRequest)

            => BootNotificationRequest is not null &&

               ChargePointVendor.Equals(BootNotificationRequest.ChargePointVendor) &&
               ChargePointModel. Equals(BootNotificationRequest.ChargePointModel)  &&

               ((ChargePointSerialNumber is     null && BootNotificationRequest.ChargePointSerialNumber is     null) ||
                (ChargePointSerialNumber is not null && BootNotificationRequest.ChargePointSerialNumber is not null &&
                 String.Equals(ChargePointSerialNumber,
                               BootNotificationRequest.ChargePointSerialNumber,
                               StringComparison.OrdinalIgnoreCase))) &&

               ((ChargeBoxSerialNumber   is     null && BootNotificationRequest.ChargeBoxSerialNumber   is     null) ||
                (ChargeBoxSerialNumber   is not null && BootNotificationRequest.ChargeBoxSerialNumber   is not null &&
                 String.Equals(ChargeBoxSerialNumber,
                               BootNotificationRequest.ChargeBoxSerialNumber,
                               StringComparison.OrdinalIgnoreCase))) &&

               ((ChargePointSerialNumber is     null && BootNotificationRequest.ChargePointSerialNumber is     null) ||
                (ChargePointSerialNumber is not null && BootNotificationRequest.ChargePointSerialNumber is not null &&
                 String.Equals(ChargePointSerialNumber,
                               BootNotificationRequest.ChargePointSerialNumber,
                               StringComparison.OrdinalIgnoreCase))) &&

               ((FirmwareVersion         is     null && BootNotificationRequest.FirmwareVersion         is     null) ||
                (FirmwareVersion         is not null && BootNotificationRequest.FirmwareVersion         is not null &&
                 String.Equals(FirmwareVersion,
                               BootNotificationRequest.FirmwareVersion,
                               StringComparison.OrdinalIgnoreCase))) &&

               ((Iccid                   is     null && BootNotificationRequest.Iccid                   is     null) ||
                (Iccid                   is not null && BootNotificationRequest.Iccid                   is not null &&
                 String.Equals(Iccid,
                               BootNotificationRequest.Iccid,
                               StringComparison.OrdinalIgnoreCase))) &&

               ((IMSI                    is     null && BootNotificationRequest.IMSI                    is     null) ||
                (IMSI                    is not null && BootNotificationRequest.IMSI                    is not null &&
                 String.Equals(IMSI,
                               BootNotificationRequest.IMSI,
                               StringComparison.OrdinalIgnoreCase))) &&

               ((MeterType               is     null && BootNotificationRequest.MeterType               is     null) ||
                (MeterType               is not null && BootNotificationRequest.MeterType               is not null &&
                 String.Equals(MeterType,
                               BootNotificationRequest.MeterType,
                               StringComparison.OrdinalIgnoreCase))) &&

               ((MeterSerialNumber       is     null && BootNotificationRequest.MeterSerialNumber       is     null) ||
                (MeterSerialNumber       is not null && BootNotificationRequest.MeterSerialNumber       is not null &&
                 String.Equals(MeterSerialNumber,
                               BootNotificationRequest.MeterSerialNumber,
                               StringComparison.OrdinalIgnoreCase))) &&

               base.GenericEquals(BootNotificationRequest);

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

                return ChargePointVendor.       GetHashCode()       * 29 ^
                       ChargePointModel.        GetHashCode()       * 23 ^

                      (ChargePointSerialNumber?.GetHashCode() ?? 0) * 19 ^
                      (ChargeBoxSerialNumber?.  GetHashCode() ?? 0) * 17 ^
                      (FirmwareVersion?.        GetHashCode() ?? 0) * 13 ^
                      (Iccid?.                  GetHashCode() ?? 0) * 11 ^
                      (IMSI?.                   GetHashCode() ?? 0) *  7 ^
                      (MeterType?.              GetHashCode() ?? 0) *  5 ^
                      (MeterSerialNumber?.      GetHashCode() ?? 0) *  3 ^

                       base.                    GetHashCode();

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
