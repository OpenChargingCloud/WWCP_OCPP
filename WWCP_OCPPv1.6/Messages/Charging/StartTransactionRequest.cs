/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// The StartTransaction request.
    /// </summary>
    public class StartTransactionRequest : ARequest<StartTransactionRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/startTransactionRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The connector identification at the charge point.
        /// </summary>
        public Connector_Id     ConnectorId       { get; }

        /// <summary>
        /// The identifier for which a transaction has to be started.
        /// </summary>
        public IdToken          IdTag             { get; }

        /// <summary>
        /// The timestamp of the transaction start.
        /// </summary>
        public DateTimeOffset   StartTimestamp    { get; }

        /// <summary>
        /// The energy meter value in Wh for the connector at start
        /// of the transaction.
        /// </summary>
        public WattHour         MeterStart        { get; }

        /// <summary>
        /// An optional identification of the reservation that will
        /// terminate as a result of this transaction.
        /// </summary>
        public Reservation_Id?  ReservationId     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new StartTransaction request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The energy meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
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
        public StartTransactionRequest(SourceRouting            Destination,
                                       Connector_Id             ConnectorId,
                                       IdToken                  IdTag,
                                       DateTimeOffset           StartTimestamp,
                                       WattHour                 MeterStart,
                                       Reservation_Id?          ReservationId         = null,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       Request_Id?              RequestId             = null,
                                       DateTimeOffset?          RequestTimestamp      = null,
                                       TimeSpan?                RequestTimeout        = null,
                                       EventTracking_Id?        EventTrackingId       = null,
                                       NetworkPath?             NetworkPath           = null,
                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(StartTransactionRequest)[..^7],

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

            this.ConnectorId     = ConnectorId;
            this.IdTag           = IdTag;
            this.StartTimestamp  = StartTimestamp;
            this.MeterStart      = MeterStart;
            this.ReservationId   = ReservationId;


            unchecked
            {

                hashCode = this.ConnectorId.   GetHashCode()       * 13 ^
                           this.IdTag.         GetHashCode()       * 11 ^
                           this.StartTimestamp.GetHashCode()       *  7 ^
                           this.MeterStart.    GetHashCode()       *  5 ^
                          (this.ReservationId?.GetHashCode() ?? 0) *  3 ^
                           base.               GetHashCode();

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
        //       <ns:startTransactionRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:idTag>?</ns:idTag>
        //          <ns:timestamp>?</ns:timestamp>
        //          <ns:meterStart>?</ns:meterStart>
        //
        //          <!--Optional:-->
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:startTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:StartTransactionRequest",
        //     "title":   "StartTransactionRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "idTag": {
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "meterStart": {
        //             "type": "integer"
        //         },
        //         "reservationId": {
        //             "type": "integer"
        //         },
        //         "timestamp": {
        //             "type": "string",
        //             "format": "date-time"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "idTag",
        //         "meterStart",
        //         "timestamp"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a start transaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static StartTransactionRequest Parse(XElement       XML,
                                                    Request_Id     RequestId,
                                                    SourceRouting  Destination,
                                                    NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var startTransactionRequest,
                         out var errorResponse))
            {
                return startTransactionRequest;
            }

            throw new ArgumentException("The given JSON representation of a start transaction request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a start transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomStartTransactionRequestParser">A delegate to parse custom StartTransaction requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static StartTransactionRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTimeOffset?                                        RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<StartTransactionRequest>?  CustomStartTransactionRequestParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var startTransactionRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomStartTransactionRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return startTransactionRequest;
            }

            throw new ArgumentException("The given JSON representation of a start transaction request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out StartTransactionRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a start transaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="StartTransactionRequest">The parsed StartTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                           XML,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out StartTransactionRequest?  StartTransactionRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse)
        {

            try
            {

                StartTransactionRequest = new StartTransactionRequest(

                                              Destination,

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                     Connector_Id.Parse),

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                     IdToken.Parse),

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                     DateTime.Parse),

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "meterStart",
                                                                     text => WattHour.FromWh(UInt64.Parse(text))),

                                              XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "reservationId",
                                                                     Reservation_Id.Parse),

                                              RequestId: RequestId

                                          );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                StartTransactionRequest  = null;
                ErrorResponse            = "The given XML representation of a start transaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out StartTransactionRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given text representation of a start transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="StartTransactionRequest">The parsed StartTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomStartTransactionRequestParser">A delegate to parse custom StartTransaction requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out StartTransactionRequest?      StartTransactionRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTimeOffset?                                        RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<StartTransactionRequest>?  CustomStartTransactionRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                StartTransactionRequest = null;

                #region ConnectorId      [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTag            [mandatory]

                if (!JSON.ParseMandatory("idTag",
                                         "identification tag",
                                         IdToken.TryParse,
                                         out IdToken IdTag,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp        [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MeterStart       [mandatory]

                if (!JSON.ParseMandatory("meterStart",
                                         "meter start",
                                         out WattHour MeterStart,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReservationId    [optional]

                if (JSON.ParseOptional("reservationId",
                                       "reservation identification",
                                       Reservation_Id.TryParse,
                                       out Reservation_Id? ReservationId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                StartTransactionRequest = new StartTransactionRequest(

                                              Destination,
                                              ConnectorId,
                                              IdTag,
                                              Timestamp,
                                              MeterStart,
                                              ReservationId,

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

                if (CustomStartTransactionRequestParser is not null)
                    StartTransactionRequest = CustomStartTransactionRequestParser(JSON,
                                                                                  StartTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                StartTransactionRequest  = null;
                ErrorResponse            = "The given JSON representation of a start transaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "startTransactionRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",          ConnectorId),
                   new XElement(OCPPNS.OCPPv1_6_CS + "idTag",                IdTag.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "timestamp",            StartTimestamp.ToISO8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "meterStart",           (UInt64) Math.Round(MeterStart.Value)),

                   ReservationId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "reservationId",  ReservationId.Value)
                       : null

               );

        #endregion

        #region ToJSON(CustomStartTransactionRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStartTransactionRequestSerializer">A delegate to serialize custom StartTransaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartTransactionRequest>?  CustomStartTransactionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("connectorId",     ConnectorId.        Value),
                                 new JProperty("idTag",           IdTag.              ToString()),
                                 new JProperty("timestamp",       StartTimestamp.     ToISO8601()),
                                 new JProperty("meterStart",      (UInt64) Math.Round(MeterStart.Value)),

                           ReservationId.HasValue
                               ? new JProperty("reservationId",   ReservationId.Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomStartTransactionRequestSerializer is not null
                       ? CustomStartTransactionRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out StartTransactionRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a StartTransaction request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="StartTransactionRequest">The StartTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomStartTransactionRequestParser">A delegate to read custom StartTransaction requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                           CBOR,
                                           Request_Id                                          RequestId,
                                           SourceRouting                                       Destination,
                                           NetworkPath                                         NetworkPath,
                                           [NotNullWhen(true)]  out StartTransactionRequest?   StartTransactionRequest,
                                           [NotNullWhen(false)] out String?                    ErrorResponse,
                                           DateTimeOffset?                                     RequestTimestamp                      = null,
                                           TimeSpan?                                           RequestTimeout                        = null,
                                           EventTracking_Id?                                   EventTrackingId                       = null,
                                           CustomCBORParserDelegate<StartTransactionRequest>?  CustomStartTransactionRequestParser   = null)
        {

            try
            {

                StartTransactionRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a StartTransaction request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("connectorId",
                                               "connector identification",
                                               out var ConnectorIdNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (ConnectorIdNumber > UInt64.MaxValue || !global::cloud.charging.open.protocols.OCPPv1_6.Connector_Id.TryParse((UInt64) ConnectorIdNumber, out var ConnectorId))
                {
                    ErrorResponse = $"Invalid connector identification '{ConnectorIdNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("idTag",
                                             "identification tag",
                                             out var IdTagText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!global::cloud.charging.open.protocols.OCPPv1_6.IdToken.TryParse(IdTagText, out var IdTag))
                {
                    ErrorResponse = $"Invalid identification tag '{IdTagText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("timestamp",
                                              "timestamp",
                                              OCPPCBORExtensions.TryParseTimestamp,
                                              out DateTimeOffset Timestamp,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("meterStart",
                                              "meter start",
                                              OCPPCBORExtensions.TryParseWattHour,
                                              out WattHour MeterStart,
                                              out ErrorResponse))
                {
                    return false;
                }

                Reservation_Id? ReservationId = null;

                if (CBOR.ParseOptionalUInt64("reservationId",
                                             "reservation identification",
                                             out var ReservationIdNumber,
                                             out ErrorResponse))
                {

                    if (ReservationIdNumber is not UInt64 ReservationIdValue || ReservationIdValue > UInt64.MaxValue || !global::cloud.charging.open.protocols.OCPPv1_6.Reservation_Id.TryParse((UInt64) ReservationIdValue, out var ReservationIdId))
                    {
                        ErrorResponse = $"Invalid reservation identification '{ReservationIdNumber}'!";
                        return false;
                    }

                    ReservationId = ReservationIdId;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                StartTransactionRequest = new StartTransactionRequest(

                                              Destination,
                                              ConnectorId,
                                              IdTag,
                                              Timestamp,
                                              MeterStart,
                                              ReservationId,

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

                if (CustomStartTransactionRequestParser is not null)
                    StartTransactionRequest = CustomStartTransactionRequestParser(CBOR,
                                                                                 StartTransactionRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                StartTransactionRequest = null;
                ErrorResponse = "The given CBOR representation of a StartTransaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomStartTransactionRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this StartTransaction request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomStartTransactionRequestSerializer">A delegate to serialize custom StartTransaction requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<StartTransactionRequest>? CustomStartTransactionRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("connectorId",               CBORValue.FromUInt64(ConnectorId.Value)),
                           ("idTag",                     CBORValue.FromText(IdTag.ToString())),
                           ("timestamp",                 StartTimestamp.ToCBOR()),
                           ("meterStart",                MeterStart.ToCBOR()),
                           ("reservationId",             OCPPCBORExtensions.UInt(ReservationId?.Value)),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomStartTransactionRequestSerializer is not null
                       ? CustomStartTransactionRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StartTransactionRequest1, StartTransactionRequest2)

        /// <summary>
        /// Compares two StartTransaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A StartTransaction request.</param>
        /// <param name="StartTransactionRequest2">Another StartTransaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StartTransactionRequest? StartTransactionRequest1,
                                           StartTransactionRequest? StartTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StartTransactionRequest1, StartTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (StartTransactionRequest1 is null || StartTransactionRequest2 is null)
                return false;

            return StartTransactionRequest1.Equals(StartTransactionRequest2);

        }

        #endregion

        #region Operator != (StartTransactionRequest1, StartTransactionRequest2)

        /// <summary>
        /// Compares two StartTransaction requests for inequality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A StartTransaction request.</param>
        /// <param name="StartTransactionRequest2">Another StartTransaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StartTransactionRequest? StartTransactionRequest1,
                                           StartTransactionRequest? StartTransactionRequest2)

            => !(StartTransactionRequest1 == StartTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<StartTransactionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two start transaction requests for equality.
        /// </summary>
        /// <param name="Object">A start transaction request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StartTransactionRequest startTransactionRequest &&
                   Equals(startTransactionRequest);

        #endregion

        #region Equals(StartTransactionRequest)

        /// <summary>
        /// Compares two start transaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest">A start transaction request to compare with.</param>
        public override Boolean Equals(StartTransactionRequest? StartTransactionRequest)

            => StartTransactionRequest is not null &&

               ConnectorId.   Equals(StartTransactionRequest.ConnectorId)    &&
               IdTag.         Equals(StartTransactionRequest.IdTag)          &&
               StartTimestamp.Equals(StartTransactionRequest.StartTimestamp) &&
               MeterStart.    Equals(StartTransactionRequest.MeterStart)     &&

            ((!ReservationId.HasValue && !StartTransactionRequest.ReservationId.HasValue) ||
              (ReservationId.HasValue &&  StartTransactionRequest.ReservationId.HasValue && ReservationId.Equals(StartTransactionRequest.ReservationId))) &&

               base.   GenericEquals(StartTransactionRequest);

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

            => String.Concat(

                   $"'{IdTag}' at connector '{ConnectorId}'",

                   ReservationId.HasValue
                       ? $", using reservation '{ReservationId.Value}'"
                       : ""

               );

        #endregion

    }

}
