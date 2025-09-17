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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The TriggerMessage request.
    /// </summary>
    public class TriggerMessageRequest : ARequest<TriggerMessageRequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/triggerMessageRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The message to trigger.
        /// </summary>
        public MessageTrigger  RequestedMessage    { get; }

        /// <summary>
        /// Optional connector identification whenever the message
        /// applies to a specific connector.
        /// </summary>
        public Connector_Id?   ConnectorId         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TriggerMessage request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
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
        public TriggerMessageRequest(SourceRouting            Destination,
                                     MessageTrigger           RequestedMessage,
                                     Connector_Id?            ConnectorId           = null,

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
                   nameof(TriggerMessageRequest)[..^7],

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

            this.RequestedMessage  = RequestedMessage;
            this.ConnectorId       = ConnectorId;

            unchecked
            {

                hashCode = this.RequestedMessage.GetHashCode()       * 5 ^
                          (this.ConnectorId?.    GetHashCode() ?? 0) * 3 ^
                           base.                 GetHashCode();

            }

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
        //       <ns:triggerMessageRequest>
        //
        //          <ns:requestedMessage>?</ns:requestedMessage>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //       </ns:triggerMessageRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:TriggerMessageRequest",
        //     "title":   "TriggerMessageRequest",
        //     "type":    "object",
        //     "properties": {
        //         "requestedMessage": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "BootNotification",
        //                 "DiagnosticsStatusNotification",
        //                 "FirmwareStatusNotification",
        //                 "Heartbeat",
        //                 "MeterValues",
        //                 "StatusNotification"
        //             ]
        //         },
        //         "connectorId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "requestedMessage"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a TriggerMessage request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static TriggerMessageRequest Parse(XElement       XML,
                                                  Request_Id     RequestId,
                                                  SourceRouting  Destination,
                                                  NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var triggerMessageRequest,
                         out var errorResponse))
            {
                return triggerMessageRequest;
            }

            throw new ArgumentException("The given XML representation of a TriggerMessage request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a TriggerMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomTriggerMessageRequestParser">A delegate to parse custom TriggerMessage requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static TriggerMessageRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  SourceRouting                                        Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTimeOffset?                                      RequestTimestamp                    = null,
                                                  TimeSpan?                                            RequestTimeout                      = null,
                                                  EventTracking_Id?                                    EventTrackingId                     = null,
                                                  CustomJObjectParserDelegate<TriggerMessageRequest>?  CustomTriggerMessageRequestParser   = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var triggerMessageRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomTriggerMessageRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return triggerMessageRequest;
            }

            throw new ArgumentException("The given JSON representation of a TriggerMessage request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out TriggerMessageRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a TriggerMessage request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="TriggerMessageRequest">The parsed TriggerMessage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                         XML,
                                       Request_Id                                       RequestId,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out TriggerMessageRequest?  TriggerMessageRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)
        {

            try
            {

                TriggerMessageRequest = new TriggerMessageRequest(

                                            Destination,

                                            XML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "requestedMessage",
                                                                    MessageTrigger.Parse),

                                            XML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                    Connector_Id.Parse),

                                            RequestId:    RequestId,
                                            NetworkPath:  NetworkPath

                                        );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TriggerMessageRequest  = null;
                ErrorResponse          = "The given XML representation of a TriggerMessage request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out TriggerMessageRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a TriggerMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="TriggerMessageRequest">The parsed TriggerMessage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomTriggerMessageRequestParser">A delegate to parse custom TriggerMessage requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out TriggerMessageRequest?      TriggerMessageRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTimeOffset?                                      RequestTimestamp                    = null,
                                       TimeSpan?                                            RequestTimeout                      = null,
                                       EventTracking_Id?                                    EventTrackingId                     = null,
                                       CustomJObjectParserDelegate<TriggerMessageRequest>?  CustomTriggerMessageRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                TriggerMessageRequest = null;

                #region MessageTriggers    [mandatory]

                if (!JSON.ParseMandatory("requestedMessage",
                                         "requested message",
                                         MessageTrigger.TryParse,
                                         out MessageTrigger MessageTriggers,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId        [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id ConnectorId,
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


                TriggerMessageRequest = new TriggerMessageRequest(

                                            Destination,
                                            MessageTriggers,
                                            ConnectorId,

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

                if (CustomTriggerMessageRequestParser is not null)
                    TriggerMessageRequest = CustomTriggerMessageRequestParser(JSON,
                                                                              TriggerMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                TriggerMessageRequest  = null;
                ErrorResponse          = "The given JSON representation of a TriggerMessage request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "triggerMessageRequest",

                         new XElement(OCPPNS.OCPPv1_6_CP + "requestedMessage",   RequestedMessage.ToString()),

                   ConnectorId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",        ConnectorId.     ToString())
                       : null

               );

        #endregion

        #region ToJSON(CustomTriggerMessageRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTriggerMessageRequestSerializer">A delegate to serialize custom TriggerMessage requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageRequest>? CustomTriggerMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestedMessage",   RequestedMessage.ToString()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",        ConnectorId.Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTriggerMessageRequestSerializer is not null
                       ? CustomTriggerMessageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TriggerMessageRequest1, TriggerMessageRequest2)

        /// <summary>
        /// Compares two TriggerMessage requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest1">A TriggerMessage request.</param>
        /// <param name="TriggerMessageRequest2">Another TriggerMessage request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TriggerMessageRequest? TriggerMessageRequest1,
                                           TriggerMessageRequest? TriggerMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageRequest1, TriggerMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (TriggerMessageRequest1 is null || TriggerMessageRequest2 is null)
                return false;

            return TriggerMessageRequest1.Equals(TriggerMessageRequest2);

        }

        #endregion

        #region Operator != (TriggerMessageRequest1, TriggerMessageRequest2)

        /// <summary>
        /// Compares two TriggerMessage requests for inequality.
        /// </summary>
        /// <param name="TriggerMessageRequest1">A TriggerMessage request.</param>
        /// <param name="TriggerMessageRequest2">Another TriggerMessage request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TriggerMessageRequest? TriggerMessageRequest1,
                                           TriggerMessageRequest? TriggerMessageRequest2)

            => !(TriggerMessageRequest1 == TriggerMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TriggerMessage requests for equality.
        /// </summary>
        /// <param name="Object">A TriggerMessage request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TriggerMessageRequest triggerMessageRequest &&
                   Equals(triggerMessageRequest);

        #endregion

        #region Equals(TriggerMessageRequest)

        /// <summary>
        /// Compares two TriggerMessage requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest">A TriggerMessage request to compare with.</param>
        public override Boolean Equals(TriggerMessageRequest? TriggerMessageRequest)

            => TriggerMessageRequest is not null &&

               RequestedMessage.Equals(TriggerMessageRequest.RequestedMessage) &&

            ((!ConnectorId.HasValue && !TriggerMessageRequest.ConnectorId.HasValue) ||
              (ConnectorId.HasValue &&  TriggerMessageRequest.ConnectorId.HasValue && ConnectorId.Equals(TriggerMessageRequest.ConnectorId))) &&

               base.     GenericEquals(TriggerMessageRequest);

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

                   RequestedMessage,

                   ConnectorId.HasValue
                       ? $" at connector '{ConnectorId}'"
                       : ""

               );

        #endregion

    }

}
