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
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A clear cache request.
    /// </summary>
    public class ClearCacheRequest : ARequest<ClearCacheRequest>,
                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/clearCacheRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearCache request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
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
        public ClearCacheRequest(NetworkingNode_Id             NetworkingNodeId,

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
                   nameof(ClearCacheRequest)[..^7],

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

        { }

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
        //
        //       <ns:clearCacheRequest>
        //
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ClearCacheRequest",
        //     "title":   "ClearCacheRequest",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a clear cache request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static ClearCacheRequest Parse(XElement           XML,
                                              Request_Id         RequestId,
                                              NetworkingNode_Id  NetworkingNodeId,
                                              NetworkPath        NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var clearCacheRequest,
                         out var errorResponse) &&
                clearCacheRequest is not null)
            {
                return clearCacheRequest;
            }

            throw new ArgumentException("The given XML representation of a clear cache request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomClearCacheRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomClearCacheRequestParser">An optional delegate to parse custom ClearCache requests.</param>
        public static ClearCacheRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              NetworkingNode_Id                                NetworkingNodeId,
                                              NetworkPath                                      NetworkPath,
                                              CustomJObjectParserDelegate<ClearCacheRequest>?  CustomClearCacheRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var clearCacheRequest,
                         out var errorResponse,
                         CustomClearCacheRequestParser) &&
                clearCacheRequest is not null)
            {
                return clearCacheRequest;
            }

            throw new ArgumentException("The given JSON representation of a clear cache request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out ClearCacheRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a clear cache request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearCacheRequest">The parsed ClearCache request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                XML,
                                       Request_Id              RequestId,
                                       NetworkingNode_Id       NetworkingNodeId,
                                       NetworkPath             NetworkPath,
                                       out ClearCacheRequest?  ClearCacheRequest,
                                       out String?             ErrorResponse)
        {

            try
            {

                ClearCacheRequest = new ClearCacheRequest(

                                        NetworkingNodeId,

                                        RequestId:    RequestId,
                                        NetworkPath:  NetworkPath

                                    );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ClearCacheRequest  = null;
                ErrorResponse      = "The given XML representation of a clear cache request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ClearCacheRequest, out ErrorResponse, CustomClearCacheRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearCacheRequest">The parsed ClearCache request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       Request_Id              RequestId,
                                       NetworkingNode_Id       NetworkingNodeId,
                                       NetworkPath             NetworkPath,
                                       out ClearCacheRequest?  ClearCacheRequest,
                                       out String?             ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out ClearCacheRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearCacheRequest">The parsed ClearCache request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearCacheRequestParser">An optional delegate to parse custom ClearCache requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       NetworkingNode_Id                                NetworkingNodeId,
                                       NetworkPath                                      NetworkPath,
                                       out ClearCacheRequest?                           ClearCacheRequest,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<ClearCacheRequest>?  CustomClearCacheRequestParser)
        {

            try
            {

                ClearCacheRequest = default;

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                ClearCacheRequest = new ClearCacheRequest(

                                        NetworkingNodeId,

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

                if (CustomClearCacheRequestParser is not null)
                    ClearCacheRequest = CustomClearCacheRequestParser(JSON,
                                                                      ClearCacheRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearCacheRequest  = null;
                ErrorResponse      = "The given JSON representation of a clear cache request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "clearCacheRequest");

        #endregion

        #region ToJSON(CustomClearCacheRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearCacheRequestSerializer">A delegate to serialize custom ClearCache requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearCacheRequest>?  CustomClearCacheRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?     CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearCacheRequestSerializer is not null
                       ? CustomClearCacheRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearCacheRequest1, ClearCacheRequest2)

        /// <summary>
        /// Compares two ClearCache requests for equality.
        /// </summary>
        /// <param name="ClearCacheRequest1">A ClearCache request.</param>
        /// <param name="ClearCacheRequest2">Another ClearCache request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearCacheRequest? ClearCacheRequest1,
                                           ClearCacheRequest? ClearCacheRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearCacheRequest1, ClearCacheRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearCacheRequest1 is null || ClearCacheRequest2 is null)
                return false;

            return ClearCacheRequest1.Equals(ClearCacheRequest2);

        }

        #endregion

        #region Operator != (ClearCacheRequest1, ClearCacheRequest2)

        /// <summary>
        /// Compares two ClearCache requests for inequality.
        /// </summary>
        /// <param name="ClearCacheRequest1">A ClearCache request.</param>
        /// <param name="ClearCacheRequest2">Another ClearCache request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearCacheRequest? ClearCacheRequest1,
                                           ClearCacheRequest? ClearCacheRequest2)

            => !(ClearCacheRequest1 == ClearCacheRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearCacheRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear cache requests for equality.
        /// </summary>
        /// <param name="Object">A clear cache request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearCacheRequest clearCacheRequest &&
                   Equals(clearCacheRequest);

        #endregion

        #region Equals(ClearCacheRequest)

        /// <summary>
        /// Compares two clear cache requests for equality.
        /// </summary>
        /// <param name="ClearCacheRequest">A clear cache request to compare with.</param>
        public override Boolean Equals(ClearCacheRequest? ClearCacheRequest)

            => ClearCacheRequest is not null &&

               base.GenericEquals(ClearCacheRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "ClearCacheRequest";

        #endregion

    }

}
