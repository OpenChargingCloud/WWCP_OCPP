/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The extended trigger message request.
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
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The message to trigger.
        /// </summary>
        public MessageTriggers  RequestedMessage    { get; }

        /// <summary>
        /// Optional connector identification whenever the message
        /// applies to a specific connector.
        /// </summary>
        public Connector_Id?    ConnectorId         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new extended trigger message request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
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
        public ExtendedTriggerMessageRequest(NetworkingNode_Id             NetworkingNodeId,
                                             MessageTriggers               RequestedMessage,
                                             Connector_Id?                 ConnectorId         = null,

                                             IEnumerable<KeyPair>?         SignKeys            = null,
                                             IEnumerable<SignInfo>?        SignInfos           = null,
                                             IEnumerable<OCPP.Signature>?  Signatures          = null,

                                             CustomData?                   CustomData          = null,

                                             Request_Id?                   RequestId           = null,
                                             DateTime?                     RequestTimestamp    = null,
                                             TimeSpan?                     RequestTimeout      = null,
                                             EventTracking_Id?             EventTrackingId     = null,
                                             NetworkPath?                  NetworkPath         = null,
                                             CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
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
                   CancellationToken)

        {

            this.RequestedMessage  = RequestedMessage;
            this.ConnectorId       = ConnectorId;

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

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomExtendedTriggerMessageRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an extended trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomExtendedTriggerMessageRequestParser">A delegate to parse custom extended trigger message requests.</param>
        public static ExtendedTriggerMessageRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          NetworkingNode_Id                                            NetworkingNodeId,
                                                          NetworkPath                                                  NetworkPath,
                                                          CustomJObjectParserDelegate<ExtendedTriggerMessageRequest>?  CustomExtendedTriggerMessageRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var extendedTriggerMessageRequest,
                         out var errorResponse,
                         CustomExtendedTriggerMessageRequestParser) &&
                extendedTriggerMessageRequest is not null)
            {
                return extendedTriggerMessageRequest!;
            }

            throw new ArgumentException("The given JSON representation of an extended trigger message request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out ExtendedTriggerMessageRequest, out ErrorResponse, CustomExtendedTriggerMessageRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an extended trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ExtendedTriggerMessageRequest">The parsed extended trigger message request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       Request_Id                          RequestId,
                                       NetworkingNode_Id                   NetworkingNodeId,
                                       NetworkPath                         NetworkPath,
                                       out ExtendedTriggerMessageRequest?  ExtendedTriggerMessageRequest,
                                       out String?                         ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out ExtendedTriggerMessageRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an extended trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ExtendedTriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomExtendedTriggerMessageRequestParser">A delegate to parse custom extended trigger message requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       NetworkingNode_Id                                            NetworkingNodeId,
                                       NetworkPath                                                  NetworkPath,
                                       out ExtendedTriggerMessageRequest?                           ExtendedTriggerMessageRequest,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<ExtendedTriggerMessageRequest>?  CustomExtendedTriggerMessageRequestParser)
        {

            try
            {

                ExtendedTriggerMessageRequest = null;

                #region MessageExtendedTriggers    [mandatory]

                if (!JSON.MapMandatory("requestedMessage",
                                       "requested message",
                                       MessageTriggersExtensions.Parse,
                                       out MessageTriggers  MessageExtendedTriggers,
                                       out                  ErrorResponse))
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ExtendedTriggerMessageRequest = new ExtendedTriggerMessageRequest(

                                                    NetworkingNodeId,
                                                    MessageExtendedTriggers,
                                                    ConnectorId,

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

                if (CustomExtendedTriggerMessageRequestParser is not null)
                    ExtendedTriggerMessageRequest = CustomExtendedTriggerMessageRequestParser(JSON,
                                                                                              ExtendedTriggerMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                ExtendedTriggerMessageRequest  = null;
                ErrorResponse                  = "The given JSON representation of an extended trigger message request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomExtendedTriggerMessageRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomExtendedTriggerMessageRequestSerializer">A delegate to serialize custom extended trigger message requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ExtendedTriggerMessageRequest>?  CustomExtendedTriggerMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                 CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestedMessage",    RequestedMessage.AsText()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",         ConnectorId.Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.      ToJSON(CustomCustomDataSerializer))
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
        /// Compares two extended trigger message requests for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageRequest1">A extended trigger message request.</param>
        /// <param name="ExtendedTriggerMessageRequest2">Another extended trigger message request.</param>
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
        /// Compares two extended trigger message requests for inequality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageRequest1">A extended trigger message request.</param>
        /// <param name="ExtendedTriggerMessageRequest2">Another extended trigger message request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest1,
                                           ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest2)

            => !(ExtendedTriggerMessageRequest1 == ExtendedTriggerMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<ExtendedTriggerMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two extended trigger message requests for equality.
        /// </summary>
        /// <param name="Object">A extended trigger message request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ExtendedTriggerMessageRequest extendedTriggerMessageRequest &&
                   Equals(extendedTriggerMessageRequest);

        #endregion

        #region Equals(ExtendedTriggerMessageRequest)

        /// <summary>
        /// Compares two extended trigger message requests for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageRequest">A extended trigger message request to compare with.</param>
        public override Boolean Equals(ExtendedTriggerMessageRequest? ExtendedTriggerMessageRequest)

            => ExtendedTriggerMessageRequest is not null &&

               RequestedMessage.Equals(ExtendedTriggerMessageRequest.RequestedMessage) &&

            ((!ConnectorId.HasValue && !ExtendedTriggerMessageRequest.ConnectorId.HasValue) ||
              (ConnectorId.HasValue &&  ExtendedTriggerMessageRequest.ConnectorId.HasValue && ConnectorId.Equals(ExtendedTriggerMessageRequest.ConnectorId))) &&

               base.     GenericEquals(ExtendedTriggerMessageRequest);

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

                return RequestedMessage.GetHashCode()       * 5 ^
                      (ConnectorId?.    GetHashCode() ?? 0) * 3 ^
                       base.            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(RequestedMessage,

                             ConnectorId.HasValue
                                 ? " for " + ConnectorId
                                 : "");

        #endregion

    }

}
