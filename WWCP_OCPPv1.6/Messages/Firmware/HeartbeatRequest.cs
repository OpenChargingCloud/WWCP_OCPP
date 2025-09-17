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
    /// The Heartbeat request.
    /// </summary>
    public class HeartbeatRequest : ARequest<HeartbeatRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/heartbeatRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Heartbeat request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
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
        public HeartbeatRequest(SourceRouting            Destination,

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
                   nameof(HeartbeatRequest)[..^7],

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

            unchecked
            {
                hashCode = base.GetHashCode();
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
        //       <ns:heartbeatRequest/>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema":    "http://json-schema.org/draft-04/schema#",
        //     "id":         "urn:OCPP:1.6:2019:12:HeartbeatRequest",
        //     "title":      "HeartbeatRequest",
        //     "type":       "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a Heartbeat request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static HeartbeatRequest Parse(XElement       XML,
                                             Request_Id     RequestId,
                                             SourceRouting  Destination,
                                             NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var heartbeatRequest,
                         out var errorResponse))
            {
                return heartbeatRequest;
            }

            throw new ArgumentException("The given XML representation of a Heartbeat request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a Heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomHeartbeatRequestParser">A delegate to parse custom Heartbeat requests.</param>
        public static HeartbeatRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             SourceRouting                                   Destination,
                                             NetworkPath                                     NetworkPath,
                                             DateTimeOffset?                                 RequestTimestamp               = null,
                                             TimeSpan?                                       RequestTimeout                 = null,
                                             EventTracking_Id?                               EventTrackingId                = null,
                                             CustomJObjectParserDelegate<HeartbeatRequest>?  CustomHeartbeatRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var heartbeatRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomHeartbeatRequestParser))
            {
                return heartbeatRequest;
            }

            throw new ArgumentException("The given JSON representation of a Heartbeat request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out HeartbeatRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a Heartbeat request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                    XML,
                                       Request_Id                                  RequestId,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out HeartbeatRequest?  HeartbeatRequest,
                                       [NotNullWhen(false)] out String?            ErrorResponse)
        {

            try
            {

                HeartbeatRequest = new HeartbeatRequest(
                                       Destination,
                                       RequestId: RequestId
                                   );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                HeartbeatRequest  = null;
                ErrorResponse     = "The given XML representation of a Heartbeat request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out HeartbeatRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a Heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="HeartbeatRequest">The parsed Heartbeat request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomHeartbeatRequestParser">A delegate to parse custom Heartbeat requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       SourceRouting                                   Destination,
                                       NetworkPath                                     NetworkPath,
                                       [NotNullWhen(true)]  out HeartbeatRequest?      HeartbeatRequest,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTimeOffset?                                 RequestTimestamp               = null,
                                       TimeSpan?                                       RequestTimeout                 = null,
                                       EventTracking_Id?                               EventTrackingId                = null,
                                       CustomJObjectParserDelegate<HeartbeatRequest>?  CustomHeartbeatRequestParser   = null)
        {

            try
            {

                HeartbeatRequest  = null;

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


                HeartbeatRequest  = new HeartbeatRequest(

                                        Destination,

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

                if (CustomHeartbeatRequestParser is not null)
                    HeartbeatRequest = CustomHeartbeatRequestParser(JSON,
                                                                    HeartbeatRequest);

                return true;

            }
            catch (Exception e)
            {
                HeartbeatRequest  = null;
                ErrorResponse     = "The given JSON representation of a Heartbeat request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "heartbeatRequest");

        #endregion

        #region ToJSON(CustomHeartbeatRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHeartbeatRequestSerializer">A delegate to serialize custom heartbeat requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<HeartbeatRequest>?  CustomHeartbeatRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?         CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
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

            return CustomHeartbeatRequestSerializer is not null
                       ? CustomHeartbeatRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest1">a Heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (HeartbeatRequest? HeartbeatRequest1,
                                           HeartbeatRequest? HeartbeatRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(HeartbeatRequest1, HeartbeatRequest2))
                return true;

            // If one is null, but not both, return false.
            if (HeartbeatRequest1 is null || HeartbeatRequest2 is null)
                return false;

            return HeartbeatRequest1.Equals(HeartbeatRequest2);

        }

        #endregion

        #region Operator != (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for inequality.
        /// </summary>
        /// <param name="HeartbeatRequest1">a Heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (HeartbeatRequest? HeartbeatRequest1,
                                           HeartbeatRequest? HeartbeatRequest2)

            => !(HeartbeatRequest1 == HeartbeatRequest2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="Object">a Heartbeat request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HeartbeatRequest heartbeatRequest &&
                   Equals(heartbeatRequest);

        #endregion

        #region Equals(HeartbeatRequest)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest">a Heartbeat request to compare with.</param>
        public override Boolean Equals(HeartbeatRequest? HeartbeatRequest)

            => HeartbeatRequest is not null &&

               base.GenericEquals(HeartbeatRequest);

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

            => "HeartbeatRequest";

        #endregion

    }

}
