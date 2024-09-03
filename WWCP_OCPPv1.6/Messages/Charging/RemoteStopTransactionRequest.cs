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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The remote stop transaction request.
    /// </summary>
    public class RemoteStopTransactionRequest : ARequest<RemoteStopTransactionRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/remoteStopTransactionRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of the transaction which the charge
        /// point is requested to stop.
        /// </summary>
        public Transaction_Id  TransactionId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remote stop transaction request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
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
        public RemoteStopTransactionRequest(NetworkingNode_Id             NetworkingNodeId,
                                            Transaction_Id                TransactionId,

                                            IEnumerable<WWCP.KeyPair>?    SignKeys            = null,
                                            IEnumerable<WWCP.SignInfo>?   SignInfos           = null,
                                            IEnumerable<Signature>?  Signatures          = null,

                                            CustomData?                   CustomData          = null,

                                            Request_Id?                   RequestId           = null,
                                            DateTime?                     RequestTimestamp    = null,
                                            TimeSpan?                     RequestTimeout      = null,
                                            EventTracking_Id?             EventTrackingId     = null,
                                            NetworkPath?                  NetworkPath         = null,
                                            CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(RemoteStopTransactionRequest)[..^7],

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

            this.TransactionId = TransactionId;

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
        //       <ns:remoteStopTransactionRequest>
        //
        //          <ns:transactionId>?</ns:transactionId>
        //
        //       </ns:remoteStopTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStopTransactionRequest",
        //     "title":   "RemoteStopTransactionRequest",
        //     "type":    "object",
        //     "properties": {
        //         "transactionId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "transactionId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, NetworkingNodeId, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a remote stop transaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static RemoteStopTransactionRequest Parse(XElement           XML,
                                                         Request_Id         RequestId,
                                                         NetworkingNode_Id  NetworkingNodeId,
                                                         NetworkPath        NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var remoteStopTransactionRequest,
                         out var errorResponse) &&
                remoteStopTransactionRequest is not null)
            {
                return remoteStopTransactionRequest;
            }

            throw new ArgumentException("The given XML representation of a remote stop transaction request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomRemoteStopTransactionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a remote stop transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomRemoteStopTransactionRequestParser">An optional delegate to parse custom remote stop transaction requests.</param>
        public static RemoteStopTransactionRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         NetworkingNode_Id                                           NetworkingNodeId,
                                                         NetworkPath                                                 NetworkPath,
                                                         CustomJObjectParserDelegate<RemoteStopTransactionRequest>?  CustomRemoteStopTransactionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var remoteStopTransactionRequest,
                         out var errorResponse,
                         CustomRemoteStopTransactionRequestParser) &&
                remoteStopTransactionRequest is not null)
            {
                return remoteStopTransactionRequest;
            }

            throw new ArgumentException("The given JSON representation of a remote stop transaction request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, NetworkingNodeId, NetworkPath, out RemoteStopTransactionRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a remote stop transaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed remote stop transaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                           XML,
                                       Request_Id                         RequestId,
                                       NetworkingNode_Id                  NetworkingNodeId,
                                       NetworkPath                        NetworkPath,
                                       out RemoteStopTransactionRequest?  RemoteStopTransactionRequest,
                                       out String?                        ErrorResponse)
        {

            try
            {

                RemoteStopTransactionRequest = new RemoteStopTransactionRequest(

                                                   NetworkingNodeId,

                                                   XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "transactionId",
                                                                      Transaction_Id.Parse),

                                                   RequestId:    RequestId,
                                                   NetworkPath:  NetworkPath

                                               );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                RemoteStopTransactionRequest  = null;
                ErrorResponse                 = "The given XML representation of a remote stop transaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out RemoteStopTransactionRequest, out ErrorResponse, CustomRemoteStopTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a remote stop transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed remote stop transaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       Request_Id                         RequestId,
                                       NetworkingNode_Id                  NetworkingNodeId,
                                       NetworkPath                        NetworkPath,
                                       out RemoteStopTransactionRequest?  RemoteStopTransactionRequest,
                                       out String?                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out RemoteStopTransactionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a remote stop transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed remote stop transaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoteStopTransactionRequestParser">An optional delegate to parse custom remote stop transaction requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       NetworkingNode_Id                                           NetworkingNodeId,
                                       NetworkPath                                                 NetworkPath,
                                       out RemoteStopTransactionRequest?                           RemoteStopTransactionRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<RemoteStopTransactionRequest>?  CustomRemoteStopTransactionRequestParser)
        {

            try
            {

                RemoteStopTransactionRequest = null;

                #region TransactionId    [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
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


                RemoteStopTransactionRequest = new RemoteStopTransactionRequest(

                                                   NetworkingNodeId,
                                                   TransactionId,

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

                if (CustomRemoteStopTransactionRequestParser is not null)
                    RemoteStopTransactionRequest = CustomRemoteStopTransactionRequestParser(JSON,
                                                                                            RemoteStopTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                RemoteStopTransactionRequest  = null;
                ErrorResponse                 = "The given JSON representation of a remote stop transaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "remoteStopTransactionRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "transactionId",  TransactionId.ToString())

               );

        #endregion

        #region ToJSON(CustomRemoteStopTransactionRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStopTransactionRequestSerializer">A delegate to serialize custom remote stop transaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStopTransactionRequest>?  CustomRemoteStopTransactionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("transactionId",   TransactionId.Value),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRemoteStopTransactionRequestSerializer is not null
                       ? CustomRemoteStopTransactionRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStopTransactionRequest1, RemoteStopTransactionRequest2)

        /// <summary>
        /// Compares two remote stop transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest1">A remote stop transaction request.</param>
        /// <param name="RemoteStopTransactionRequest2">Another remote stop transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStopTransactionRequest? RemoteStopTransactionRequest1,
                                           RemoteStopTransactionRequest? RemoteStopTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStopTransactionRequest1, RemoteStopTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (RemoteStopTransactionRequest1 is null || RemoteStopTransactionRequest2 is null)
                return false;

            return RemoteStopTransactionRequest1.Equals(RemoteStopTransactionRequest2);

        }

        #endregion

        #region Operator != (RemoteStopTransactionRequest1, RemoteStopTransactionRequest2)

        /// <summary>
        /// Compares two remote stop transaction requests for inequality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest1">A remote stop transaction request.</param>
        /// <param name="RemoteStopTransactionRequest2">Another remote stop transaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStopTransactionRequest? RemoteStopTransactionRequest1,
                                           RemoteStopTransactionRequest? RemoteStopTransactionRequest2)

            => !(RemoteStopTransactionRequest1 == RemoteStopTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<RemoteStopTransactionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote stop transaction requests for equality.
        /// </summary>
        /// <param name="Object">A remote stop transaction request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteStopTransactionRequest remoteStopTransactionRequest &&
                   Equals(remoteStopTransactionRequest);

        #endregion

        #region Equals(RemoteStopTransactionRequest)

        /// <summary>
        /// Compares two remote stop transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest">A remote stop transaction request to compare with.</param>
        public override Boolean Equals(RemoteStopTransactionRequest? RemoteStopTransactionRequest)

            => RemoteStopTransactionRequest is not null &&

               TransactionId.Equals(RemoteStopTransactionRequest.TransactionId) &&

               base.  GenericEquals(RemoteStopTransactionRequest);

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

                return TransactionId.GetHashCode() * 3 ^
                       base.         GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => TransactionId.ToString();

        #endregion

    }

}
