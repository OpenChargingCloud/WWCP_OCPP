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

    /// <summary>
    /// The StatusNotification request.
    /// </summary>
    public class StatusNotificationRequest : ARequest<StatusNotificationRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/statusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The connector identification at the charge point.
        /// Id '0' (zero) is used if the status is for the charge point main controller.
        /// </summary>
        public Connector_Id           ConnectorId        { get; }

        /// <summary>
        /// The current status of the charge point.
        /// </summary>
        public ChargePointStatus      Status             { get; }

        /// <summary>
        /// The error code reported by the charge point.
        /// </summary>
        public ChargePointErrorCodes  ErrorCode          { get; }

        /// <summary>
        /// Additional free format information related to the error.
        /// </summary>
        public String?                Info               { get; }

        /// <summary>
        /// The time for which the status is reported.
        /// </summary>
        public DateTime?              StatusTimestamp    { get; }

        /// <summary>
        /// An optional identifier of a vendor-specific extension.
        /// </summary>
        public String?                VendorId           { get; }

        /// <summary>
        /// An optional vendor-specific error code.
        /// </summary>
        public String?                VendorErrorCode    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new StatusNotification request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="Status">The current status of the charge point.</param>
        /// <param name="ErrorCode">The error code reported by the charge point.</param>
        /// 
        /// <param name="Info">Additional free format information related to the error.</param>
        /// <param name="StatusTimestamp">The time for which the status is reported.</param>
        /// <param name="VendorId">An optional identifier of a vendor-specific extension.</param>
        /// <param name="VendorErrorCode">An optional vendor-specific error code.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public StatusNotificationRequest(SourceRouting            Destination,
                                         Connector_Id             ConnectorId,
                                         ChargePointStatus        Status,
                                         ChargePointErrorCodes    ErrorCode,

                                         String?                  Info                  = null,
                                         DateTime?                StatusTimestamp       = null,
                                         String?                  VendorId              = null,
                                         String?                  VendorErrorCode       = null,

                                         IEnumerable<KeyPair>?    SignKeys              = null,
                                         IEnumerable<SignInfo>?   SignInfos             = null,
                                         IEnumerable<Signature>?  Signatures            = null,

                                         CustomData?              CustomData            = null,

                                         Request_Id?              RequestId             = null,
                                         DateTime?                RequestTimestamp      = null,
                                         TimeSpan?                RequestTimeout        = null,
                                         EventTracking_Id?        EventTrackingId       = null,
                                         NetworkPath?             NetworkPath           = null,
                                         SerializationFormats?    SerializationFormat   = null,
                                         CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(StatusNotificationRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.ConnectorId      = ConnectorId;
            this.Status           = Status;
            this.ErrorCode        = ErrorCode;

            this.Info             = Info?.           Trim();
            this.StatusTimestamp  = StatusTimestamp;
            this.VendorId         = VendorId?.       Trim();
            this.VendorErrorCode  = VendorErrorCode?.Trim();

            unchecked
            {

                hashCode = this.ConnectorId.     GetHashCode()       * 19 ^
                           this.Status.          GetHashCode()       * 17 ^
                           this.ErrorCode.       GetHashCode()       * 13 ^
                          (this.Info?.           GetHashCode() ?? 0) * 11 ^
                          (this.StatusTimestamp?.GetHashCode() ?? 0) *  7 ^
                          (this.VendorId?.       GetHashCode() ?? 0) *  5 ^
                          (this.VendorErrorCode?.GetHashCode() ?? 0) *  3 ^
                           base.                 GetHashCode();

            }

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

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:StatusNotificationRequest",
        //     "title":    "StatusNotificationRequest",
        //     "type":     "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "errorCode": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ConnectorLockFailure",
        //                 "EVCommunicationError",
        //                 "GroundFailure",
        //                 "HighTemperature",
        //                 "InternalError",
        //                 "LocalListConflict",
        //                 "NoError",
        //                 "OtherError",
        //                 "OverCurrentFailure",
        //                 "PowerMeterFailure",
        //                 "PowerSwitchFailure",
        //                 "ReaderFailure",
        //                 "StatusNotificationFailure",
        //                 "UnderVoltage",
        //                 "OverVoltage",
        //                 "WeakSignal"
        //             ]
        //         },
        //         "info": {
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Available",
        //                 "Preparing",
        //                 "Charging",
        //                 "SuspendedEVSE",
        //                 "SuspendedEV",
        //                 "Finishing",
        //                 "Reserved",
        //                 "Unavailable",
        //                 "Faulted"
        //             ]
        //         },
        //         "timestamp": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "vendorId": {
        //             "type": "string",
        //             "maxLength": 255
        //         },
        //         "vendorErrorCode": {
        //             "type": "string",
        //             "maxLength": 50
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "errorCode",
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a StatusNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static StatusNotificationRequest Parse(XElement       XML,
                                                      Request_Id     RequestId,
                                                      SourceRouting  Destination,
                                                      NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var statusNotificationRequest,
                         out var errorResponse))
            {
                return statusNotificationRequest;
            }

            throw new ArgumentException("The given XML representation of a StatusNotification request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a StatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomStatusNotificationRequestParser">A delegate to parse custom StatusNotification requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static StatusNotificationRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestParser   = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var statusNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomStatusNotificationRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return statusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a StatusNotification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out StatusNotificationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a StatusNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="StatusNotificationRequest">The parsed StatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                             XML,
                                       Request_Id                                           RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out StatusNotificationRequest?  StatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse)
        {

            try
            {

                StatusNotificationRequest = new StatusNotificationRequest(

                                                Destination,

                                                XML.MapValueOrFail       (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                          Connector_Id.Parse),

                                                XML.MapEnumValuesOrFail  (OCPPNS.OCPPv1_6_CS + "status",
                                                                          ChargePointStatusExtensions.Parse),

                                                XML.MapEnumValuesOrFail  (OCPPNS.OCPPv1_6_CS + "errorCode",
                                                                          ChargePointErrorCodeExtensions.Parse),

                                                XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "info"),

                                                XML.MapValueOrNullable   (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                          DateTime.Parse),

                                                XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "vendorId"),

                                                XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "vendorErrorCode"),

                                                RequestId: RequestId

                                            );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                StatusNotificationRequest  = null;
                ErrorResponse              = "The given XML representation of a StatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out StatusNotificationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a StatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="StatusNotificationRequest">The parsed StatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomStatusNotificationRequestParser">A delegate to parse custom StatusNotification requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out StatusNotificationRequest?      StatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                StatusNotificationRequest = null;

                #region ConnectorId        [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Status             [mandatory]

                if (!JSON.MapMandatory("status",
                                       "status",
                                       ChargePointStatusExtensions.Parse,
                                       out ChargePointStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ErrorCode          [mandatory]

                if (!JSON.MapMandatory("errorCode",
                                       "error code",
                                       ChargePointErrorCodeExtensions.Parse,
                                       out ChargePointErrorCodes ErrorCode,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp          [optional]

                if (!JSON.ParseOptional("timestamp",
                                        "timestamp",
                                        out DateTime? Timestamp,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Info               [optional]

                if (!JSON.ParseOptional("info",
                                        "info",
                                        out String? Info,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region VendorId           [optional]

                if (!JSON.ParseOptional("vendorId",
                                        "vendor identification",
                                        out String? VendorId,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region VendorErrorCode    [optional]

                if (!JSON.ParseOptional("vendorErrorCode",
                                        "vendor error code",
                                        out String? VendorErrorCode,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures         [optional, OCPP_CSE]

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

                #region CustomData         [optional]

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


                StatusNotificationRequest = new StatusNotificationRequest(

                                                Destination,
                                                ConnectorId,
                                                Status,
                                                ErrorCode,

                                                Info,
                                                Timestamp,
                                                VendorId,
                                                VendorErrorCode,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData,

                                                RequestId,
                                                RequestTimestamp,
                                                RequestTimeout,
                                                EventTrackingId,
                                                NetworkPath

                                            );

                if (CustomStatusNotificationRequestParser is not null)
                    StatusNotificationRequest = CustomStatusNotificationRequestParser(JSON,
                                                                                      StatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                StatusNotificationRequest  = null;
                ErrorResponse              = "The given JSON representation of a StatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "statusNotificationRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",            ConnectorId.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",                 Status.     AsText()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "errorCode",              ErrorCode.  AsText()),

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

        #region ToJSON(CustomStatusNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationRequestSerializer">A delegate to serialize custom StatusNotification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationRequest>?  CustomStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("connectorId",       ConnectorId.Value),
                                 new JProperty("status",            Status.     AsText()),
                                 new JProperty("errorCode",         ErrorCode.  AsText()),

                           Info.IsNotNullOrEmpty()
                               ? new JProperty("info",              Info)
                               : null,

                           StatusTimestamp.HasValue
                               ? new JProperty("timestamp",         StatusTimestamp.Value.ToIso8601())
                               : null,

                           VendorId.IsNotNullOrEmpty()
                               ? new JProperty("vendorId",          VendorId)
                               : null,

                           VendorErrorCode.IsNotNullOrEmpty()
                               ? new JProperty("vendorErrorCode",   VendorErrorCode)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.           ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomStatusNotificationRequestSerializer is not null
                       ? CustomStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StatusNotificationRequest1, StatusNotificationRequest2)

        /// <summary>
        /// Compares two StatusNotification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A StatusNotification request.</param>
        /// <param name="StatusNotificationRequest2">Another StatusNotification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StatusNotificationRequest? StatusNotificationRequest1,
                                           StatusNotificationRequest? StatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusNotificationRequest1, StatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (StatusNotificationRequest1 is null || StatusNotificationRequest2 is null)
                return false;

            return StatusNotificationRequest1.Equals(StatusNotificationRequest2);

        }

        #endregion

        #region Operator != (StatusNotificationRequest1, StatusNotificationRequest2)

        /// <summary>
        /// Compares two StatusNotification requests for inequality.
        /// </summary>
        /// <param name="StatusNotificationRequest1">A StatusNotification request.</param>
        /// <param name="StatusNotificationRequest2">Another StatusNotification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationRequest? StatusNotificationRequest1,
                                           StatusNotificationRequest? StatusNotificationRequest2)

            => !(StatusNotificationRequest1 == StatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two StatusNotification requests for equality.
        /// </summary>
        /// <param name="Object">A StatusNotification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusNotificationRequest statusNotificationRequest &&
                   Equals(statusNotificationRequest);

        #endregion

        #region Equals(StatusNotificationRequest)

        /// <summary>
        /// Compares two StatusNotification requests for equality.
        /// </summary>
        /// <param name="StatusNotificationRequest">A StatusNotification request to compare with.</param>
        public override Boolean Equals(StatusNotificationRequest? StatusNotificationRequest)

            => StatusNotificationRequest is not null &&

               ConnectorId.    Equals(StatusNotificationRequest.ConnectorId) &&
               Status.         Equals(StatusNotificationRequest.Status)      &&
               ErrorCode.      Equals(StatusNotificationRequest.ErrorCode)   &&

             ((Info is     null            &&  StatusNotificationRequest.Info            is     null) ||
              (Info is not null            &&  StatusNotificationRequest.Info            is not null && Info.                 Equals(StatusNotificationRequest.Info)))                  &&

            ((!StatusTimestamp.HasValue    && !StatusNotificationRequest.StatusTimestamp.HasValue)    ||
              (StatusTimestamp.HasValue    &&  StatusNotificationRequest.StatusTimestamp.HasValue    && StatusTimestamp.Value.Equals(StatusNotificationRequest.StatusTimestamp.Value))) &&

             ((VendorId is     null        &&  StatusNotificationRequest.VendorId        is     null) ||
              (VendorId is not null        &&  StatusNotificationRequest.VendorId        is not null && VendorId.             Equals(StatusNotificationRequest.VendorId)))              &&

             ((VendorErrorCode is     null &&  StatusNotificationRequest.VendorErrorCode is     null) ||
              (VendorErrorCode is not null &&  StatusNotificationRequest.VendorErrorCode is not null && VendorErrorCode.      Equals(StatusNotificationRequest.VendorErrorCode)))       &&

               base.GenericEquals(StatusNotificationRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{ConnectorId} => {Status} / {ErrorCode}";

        #endregion

    }

}
