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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The unlock connector request.
    /// </summary>
    public class UnlockConnectorRequest : ARequest<UnlockConnectorRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/unlockConnectorRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identifier of the connector to be unlocked.
        /// </summary>
        public Connector_Id   ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unlock connector request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
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
        public UnlockConnectorRequest(NetworkingNode_Id             NetworkingNodeId,
                                      Connector_Id                  ConnectorId,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<Signature>?       Signatures          = null,

                                      CustomData?                   CustomData          = null,

                                      Request_Id?                   RequestId           = null,
                                      DateTime?                     RequestTimestamp    = null,
                                      TimeSpan?                     RequestTimeout      = null,
                                      EventTracking_Id?             EventTrackingId     = null,
                                      NetworkPath?                  NetworkPath         = null,
                                      CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(UnlockConnectorRequest)[..^7],

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

            this.ConnectorId = ConnectorId;

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
        //       <ns:unlockConnectorRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //       </ns:unlockConnectorRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UnlockConnectorRequest",
        //     "title":   "UnlockConnectorRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, NetworkingNodeId, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of an unlock connector request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static UnlockConnectorRequest Parse(XElement           XML,
                                                   Request_Id         RequestId,
                                                   NetworkingNode_Id  NetworkingNodeId,
                                                   NetworkPath        NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var unlockConnectorRequest,
                         out var errorResponse) &&
                unlockConnectorRequest is not null)
            {
                return unlockConnectorRequest;
            }

            throw new ArgumentException("The given XML representation of an unlock connector request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomUnlockConnectorRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomUnlockConnectorRequestParser">An optional delegate to parse custom unlock connector requests.</param>
        public static UnlockConnectorRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   NetworkingNode_Id                                     NetworkingNodeId,
                                                   NetworkPath                                           NetworkPath,
                                                   CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var unlockConnectorRequest,
                         out var errorResponse,
                         CustomUnlockConnectorRequestParser) &&
                unlockConnectorRequest is not null)
            {
                return unlockConnectorRequest;
            }

            throw new ArgumentException("The given JSON representation of an unlock connector request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, NetworkingNodeId, NetworkPath, out UnlockConnectorRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an unlock connector request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                     XML,
                                       Request_Id                   RequestId,
                                       NetworkingNode_Id            NetworkingNodeId,
                                       NetworkPath                  NetworkPath,
                                       out UnlockConnectorRequest?  UnlockConnectorRequest,
                                       out String?                  ErrorResponse)
        {

            try
            {

                UnlockConnectorRequest = new UnlockConnectorRequest(

                                             NetworkingNodeId,

                                             XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                Connector_Id.Parse),

                                             RequestId:    RequestId,
                                             NetworkPath:  NetworkPath

                                         );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorRequest  = null;
                ErrorResponse           = "The given XML representation of an unlock connector request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out UnlockConnectorRequest, out ErrorResponse, CustomUnlockConnectorRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       NetworkingNode_Id            NetworkingNodeId,
                                       NetworkPath                  NetworkPath,
                                       out UnlockConnectorRequest?  UnlockConnectorRequest,
                                       out String?                  ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out UnlockConnectorRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorRequestParser">An optional delegate to parse custom unlock connector requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       NetworkingNode_Id                                     NetworkingNodeId,
                                       NetworkPath                                           NetworkPath,
                                       out UnlockConnectorRequest?                           UnlockConnectorRequest,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser)
        {

            try
            {

                UnlockConnectorRequest = null;

                #region ConnectorId    [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

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


                UnlockConnectorRequest = new UnlockConnectorRequest(

                                             NetworkingNodeId,
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

                if (CustomUnlockConnectorRequestParser is not null)
                    UnlockConnectorRequest = CustomUnlockConnectorRequestParser(JSON,
                                                                                UnlockConnectorRequest);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorRequest  = null;
                ErrorResponse           = "The given JSON representation of an unlock connector request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "unlockConnectorRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString())

               );

        #endregion

        #region ToJSON(CustomUnlockConnectorRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorRequestSerializer">A delegate to serialize custom unlock connector requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?          CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("connectorId",   ConnectorId.Value),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnlockConnectorRequestSerializer is not null
                       ? CustomUnlockConnectorRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorRequest1, UnlockConnectorRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorRequest1 is null || UnlockConnectorRequest2 is null)
                return false;

            return UnlockConnectorRequest1.Equals(UnlockConnectorRequest2);

        }

        #endregion

        #region Operator != (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for inequality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)

            => !(UnlockConnectorRequest1 == UnlockConnectorRequest2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="Object">An unlock connector request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorRequest unlockConnectorRequest &&
                   Equals(unlockConnectorRequest);

        #endregion

        #region Equals(UnlockConnectorRequest)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest">An unlock connector request to compare with.</param>
        public override Boolean Equals(UnlockConnectorRequest? UnlockConnectorRequest)

            => UnlockConnectorRequest is not null &&

               ConnectorId.Equals(UnlockConnectorRequest.ConnectorId) &&

               base.GenericEquals(UnlockConnectorRequest);

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

                return ConnectorId.GetHashCode() * 3 ^
                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ConnectorId.ToString();

        #endregion

    }

}
