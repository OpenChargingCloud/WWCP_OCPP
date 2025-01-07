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
    /// The ExtendedTriggerMessage request.
    /// </summary>
    [SecurityExtensions]
    public class ExtendedTriggerMessageRequest : ARequest<ExtendedTriggerMessageRequest>,
                                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/extendedTriggerMessageRequest");

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
        /// Create a new ExtendedTriggerMessage request.
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
        public ExtendedTriggerMessageRequest(SourceRouting            Destination,
                                             MessageTrigger           RequestedMessage,
                                             Connector_Id?            ConnectorId           = null,

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
                   nameof(ExtendedTriggerMessageRequest)[..^7],

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
        //       <ns:extendedTriggerMessageRequest>
        //
        //          <ns:requestedMessage>?</ns:requestedMessage>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //       </ns:extendedTriggerMessageRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ExtendedTriggerMessageRequest",
        //     "title":   "ExtendedTriggerMessageRequest",
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

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an ExtendedTriggerMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomExtendedTriggerMessageRequestParser">A delegate to parse custom ExtendedTriggerMessage requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static ExtendedTriggerMessageRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          SourceRouting                                                Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    RequestTimestamp                            = null,
                                                          TimeSpan?                                                    RequestTimeout                              = null,
                                                          EventTracking_Id?                                            EventTrackingId                             = null,
                                                          CustomJObjectParserDelegate<ExtendedTriggerMessageRequest>?  CustomExtendedTriggerMessageRequestParser   = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var extendedTriggerMessageRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomExtendedTriggerMessageRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return extendedTriggerMessageRequest;
            }

            throw new ArgumentException("The given JSON representation of an ExtendedTriggerMessage request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ExtendedTriggerMessageRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an ExtendedTriggerMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ExtendedTriggerMessageRequest">The parsed ExtendedTriggerMessage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomExtendedTriggerMessageRequestParser">A delegate to parse custom ExtendedTriggerMessage requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out ExtendedTriggerMessageRequest?      ExtendedTriggerMessageRequest,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    RequestTimestamp                            = null,
                                       TimeSpan?                                                    RequestTimeout                              = null,
                                       EventTracking_Id?                                            EventTrackingId                             = null,
                                       CustomJObjectParserDelegate<ExtendedTriggerMessageRequest>?  CustomExtendedTriggerMessageRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                ExtendedTriggerMessageRequest = null;

                #region MessageExtendedTriggers    [mandatory]

                if (!JSON.ParseMandatory("requestedMessage",
                                         "requested message",
                                         MessageTrigger.TryParse,
                                         out MessageTrigger  MessageExtendedTriggers,
                                         out                 ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId                [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id  ConnectorId,
                                       out               ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                 [optional, OCPP_CSE]

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

                #region CustomData                 [optional]

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


                ExtendedTriggerMessageRequest = new ExtendedTriggerMessageRequest(

                                                    Destination,
                                                    MessageExtendedTriggers,
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

                if (CustomExtendedTriggerMessageRequestParser is not null)
                    ExtendedTriggerMessageRequest = CustomExtendedTriggerMessageRequestParser(JSON,
                                                                                              ExtendedTriggerMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                ExtendedTriggerMessageRequest  = null;
                ErrorResponse                  = "The given JSON representation of an ExtendedTriggerMessage request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomExtendedTriggerMessageRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomExtendedTriggerMessageRequestSerializer">A delegate to serialize custom ExtendedTriggerMessage requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ExtendedTriggerMessageRequest>?  CustomExtendedTriggerMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
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

            return CustomExtendedTriggerMessageRequestSerializer is not null
                       ? CustomExtendedTriggerMessageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ExtendedTriggerMessageRequest1, ExtendedTriggerMessageRequest2)

        /// <summary>
        /// Compares two ExtendedTriggerMessage requests for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageRequest1">A ExtendedTriggerMessage request.</param>
        /// <param name="ExtendedTriggerMessageRequest2">Another ExtendedTriggerMessage request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest1,
                                           ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ExtendedTriggerMessageRequest1, ExtendedTriggerMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ExtendedTriggerMessageRequest1 is null || ExtendedTriggerMessageRequest2 is null)
                return false;

            return ExtendedTriggerMessageRequest1.Equals(ExtendedTriggerMessageRequest2);

        }

        #endregion

        #region Operator != (ExtendedTriggerMessageRequest1, ExtendedTriggerMessageRequest2)

        /// <summary>
        /// Compares two ExtendedTriggerMessage requests for inequality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageRequest1">A ExtendedTriggerMessage request.</param>
        /// <param name="ExtendedTriggerMessageRequest2">Another ExtendedTriggerMessage request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest1,
                                           ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest2)

            => !(ExtendedTriggerMessageRequest1 == ExtendedTriggerMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<ExtendedTriggerMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ExtendedTriggerMessage requests for equality.
        /// </summary>
        /// <param name="Object">A ExtendedTriggerMessage request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ExtendedTriggerMessageRequest extendedTriggerMessageRequest &&
                   Equals(extendedTriggerMessageRequest);

        #endregion

        #region Equals(ExtendedTriggerMessageRequest)

        /// <summary>
        /// Compares two ExtendedTriggerMessage requests for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageRequest">A ExtendedTriggerMessage request to compare with.</param>
        public override Boolean Equals(ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest)

            => ExtendedTriggerMessageRequest is not null &&

               RequestedMessage.Equals(ExtendedTriggerMessageRequest.RequestedMessage) &&

            ((!ConnectorId.HasValue && !ExtendedTriggerMessageRequest.ConnectorId.HasValue) ||
              (ConnectorId.HasValue &&  ExtendedTriggerMessageRequest.ConnectorId.HasValue && ConnectorId.Equals(ExtendedTriggerMessageRequest.ConnectorId))) &&

               base.     GenericEquals(ExtendedTriggerMessageRequest);

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
                       ? $" for {ConnectorId}"
                       : ""

               );

        #endregion

    }

}
