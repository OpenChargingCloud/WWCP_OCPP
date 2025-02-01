///*
// * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/FlushChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Affero GPL license, Version 3.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.gnu.org/licenses/agpl.html
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//using cloud.charging.open.protocols.WWCP;
//using cloud.charging.open.protocols.WWCP.NetworkingNode;
//using cloud.charging.open.protocols.OCPP;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
//{

//    /// <summary>
//    /// A flush periodic event stream request.
//    /// </summary>
//    public class FlushPeriodicEventStreamRequest : ARequest<FlushPeriodicEventStreamRequest>,
//                                                   IRequest
//    {

//        #region Data

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/addSignaturePolicyRequest");

//        #endregion

//        #region Properties

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public JSONLDContext           Context
//            => DefaultJSONLDContext;

//        /// <summary>
//        /// The unqiue identification of the periodic event stream to be flushed.
//        /// </summary>
//        [Mandatory]
//        public PeriodicEventStream_Id  Id    { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new flush periodic event stream request.
//        /// </summary>
//        /// <param name="Destination">The destination networking node identification or source routing path.</param>
//        /// <param name="Id">The unqiue identification of the periodic event stream to be flushed.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public FlushPeriodicEventStreamRequest(SourceRouting            Destination,
//                                               PeriodicEventStream_Id   Id,

//                                               IEnumerable<KeyPair>?    SignKeys              = null,
//                                               IEnumerable<SignInfo>?   SignInfos             = null,
//                                               IEnumerable<Signature>?  Signatures            = null,

//                                               CustomData?              CustomData            = null,

//                                               Request_Id?              RequestId             = null,
//                                               DateTime?                RequestTimestamp      = null,
//                                               TimeSpan?                RequestTimeout        = null,
//                                               EventTracking_Id?        EventTrackingId       = null,
//                                               NetworkPath?             NetworkPath           = null,
//                                               SerializationFormats?    SerializationFormat   = null,
//                                               CancellationToken        CancellationToken     = default)

//            : base(Destination,
//                   nameof(FlushPeriodicEventStreamRequest)[..^7],

//                   SignKeys,
//                   SignInfos,
//                   Signatures,

//                   CustomData,

//                   RequestId,
//                   RequestTimestamp,
//                   RequestTimeout,
//                   EventTrackingId,
//                   NetworkPath,
//                   SerializationFormat ?? SerializationFormats.JSON,
//                   CancellationToken)

//        {

//            this.Id = Id;

//            unchecked
//            {
//                hashCode = this.Id.GetHashCode() * 3 ^
//                           base.   GetHashCode();
//            }

//        }

//        #endregion


//        //ToDo: Update schema documentation after the official release of OCPP v2.1!

//        #region Documentation

//        // tba.

//        #endregion

//        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomFlushPeriodicEventStreamRequestParser = null)

//        /// <summary>
//        /// Parse the given JSON representation of an FlushPeriodicEventStream request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="Destination">The destination networking node identification or source routing path.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="CustomFlushPeriodicEventStreamRequestParser">A delegate to parse custom FlushPeriodicEventStream requests.</param>
//        public static FlushPeriodicEventStreamRequest Parse(JObject                                                        JSON,
//                                                            Request_Id                                                     RequestId,
//                                                            SourceRouting                                              Destination,
//                                                            NetworkPath                                                    NetworkPath,
//                                                            CustomJObjectParserDelegate<FlushPeriodicEventStreamRequest>?  CustomFlushPeriodicEventStreamRequestParser   = null)
//        {


//            if (TryParse(JSON,
//                         RequestId,
//                         Destination,
//                         NetworkPath,
//                         out var addSignaturePolicyRequest,
//                         out var errorResponse,
//                         CustomFlushPeriodicEventStreamRequestParser) &&
//                addSignaturePolicyRequest is not null)
//            {
//                return addSignaturePolicyRequest;
//            }

//            throw new ArgumentException("The given JSON representation of a FlushPeriodicEventStream request is invalid: " + errorResponse,
//                                        nameof(JSON));

//        }

//        #endregion

//        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out FlushPeriodicEventStreamRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

//        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

//        /// <summary>
//        /// Try to parse the given JSON representation of a FlushPeriodicEventStream request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="Destination">The destination networking node identification or source routing path.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="FlushPeriodicEventStreamRequest">The parsed FlushPeriodicEventStream request.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        public static Boolean TryParse(JObject                               JSON,
//                                       Request_Id                            RequestId,
//                                       SourceRouting                     Destination,
//                                       NetworkPath                           NetworkPath,
//                                       out FlushPeriodicEventStreamRequest?  FlushPeriodicEventStreamRequest,
//                                       out String?                           ErrorResponse)

//            => TryParse(JSON,
//                        RequestId,
//                        Destination,
//                        NetworkPath,
//                        out FlushPeriodicEventStreamRequest,
//                        out ErrorResponse,
//                        null);


//        /// <summary>
//        /// Try to parse the given JSON representation of a FlushPeriodicEventStream request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="Destination">The destination networking node identification or source routing path.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="FlushPeriodicEventStreamRequest">The parsed FlushPeriodicEventStream request.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        /// <param name="CustomFlushPeriodicEventStreamRequestParser">A delegate to parse custom FlushPeriodicEventStream requests.</param>
//        public static Boolean TryParse(JObject                                                        JSON,
//                                       Request_Id                                                     RequestId,
//                                       SourceRouting                                              Destination,
//                                       NetworkPath                                                    NetworkPath,
//                                       out FlushPeriodicEventStreamRequest?                           FlushPeriodicEventStreamRequest,
//                                       out String?                                                    ErrorResponse,
//                                       CustomJObjectParserDelegate<FlushPeriodicEventStreamRequest>?  CustomFlushPeriodicEventStreamRequestParser)
//        {

//            try
//            {

//                FlushPeriodicEventStreamRequest = null;

//                #region Id                   [mandatory]

//                if (!JSON.ParseMandatory("id",
//                                         "periodic event stream identification",
//                                         PeriodicEventStream_Id.TryParse,
//                                         out PeriodicEventStream_Id Id,
//                                         out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region Signatures           [optional, OCPP_CSE]

//                if (JSON.ParseOptionalHashSet("signatures",
//                                              "cryptographic signatures",
//                                              Signature.TryParse,
//                                              out HashSet<Signature> Signatures,
//                                              out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region CustomData           [optional]

//                if (JSON.ParseOptionalJSON("customData",
//                                           "custom data",
//                                           WWCP.CustomData.TryParse,
//                                           out CustomData? CustomData,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion


//                FlushPeriodicEventStreamRequest = new FlushPeriodicEventStreamRequest(

//                                                      Destination,
//                                                      Id,

//                                                      null,
//                                                      null,
//                                                      Signatures,

//                                                      CustomData,

//                                                      RequestId,
//                                                      null,
//                                                      null,
//                                                      null,
//                                                      NetworkPath

//                                                  );

//                if (CustomFlushPeriodicEventStreamRequestParser is not null)
//                    FlushPeriodicEventStreamRequest = CustomFlushPeriodicEventStreamRequestParser(JSON,
//                                                                                                  FlushPeriodicEventStreamRequest);

//                return true;

//            }
//            catch (Exception e)
//            {
//                FlushPeriodicEventStreamRequest  = null;
//                ErrorResponse                    = "The given JSON representation of a FlushPeriodicEventStream request is invalid: " + e.Message;
//                return false;
//            }

//        }

//        #endregion

//        #region ToJSON(CustomFlushPeriodicEventStreamRequestSerializer = null, CustomSignatureSerializer = null, ...)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomFlushPeriodicEventStreamRequestSerializer">A delegate to serialize custom FlushPeriodicEventStream requests.</param>
//        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
//        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
//        public JObject ToJSON(Boolean                                                            IncludeJSONLDContext                              = false,
//                              CustomJObjectSerializerDelegate<FlushPeriodicEventStreamRequest>?  CustomFlushPeriodicEventStreamRequestSerializer   = null,
//                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
//                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
//        {

//            var json = JSONObject.Create(

//                           IncludeJSONLDContext
//                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
//                               : null,

//                                 new JProperty("id",           Id.Value),

//                           Signatures.Any()
//                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
//                                                                                                                          CustomCustomDataSerializer))))
//                               : null,

//                           CustomData is not null
//                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
//                               : null

//                       );

//            return CustomFlushPeriodicEventStreamRequestSerializer is not null
//                       ? CustomFlushPeriodicEventStreamRequestSerializer(this, json)
//                       : json;

//        }

//        #endregion


//        #region Operator overloading

//        #region Operator == (FlushPeriodicEventStreamRequest1, FlushPeriodicEventStreamRequest2)

//        /// <summary>
//        /// Compares two FlushPeriodicEventStream requests for equality.
//        /// </summary>
//        /// <param name="FlushPeriodicEventStreamRequest1">A FlushPeriodicEventStream request.</param>
//        /// <param name="FlushPeriodicEventStreamRequest2">Another FlushPeriodicEventStream request.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public static Boolean operator == (FlushPeriodicEventStreamRequest? FlushPeriodicEventStreamRequest1,
//                                           FlushPeriodicEventStreamRequest? FlushPeriodicEventStreamRequest2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(FlushPeriodicEventStreamRequest1, FlushPeriodicEventStreamRequest2))
//                return true;

//            // If one is null, but not both, return false.
//            if (FlushPeriodicEventStreamRequest1 is null || FlushPeriodicEventStreamRequest2 is null)
//                return false;

//            return FlushPeriodicEventStreamRequest1.Equals(FlushPeriodicEventStreamRequest2);

//        }

//        #endregion

//        #region Operator != (FlushPeriodicEventStreamRequest1, FlushPeriodicEventStreamRequest2)

//        /// <summary>
//        /// Compares two FlushPeriodicEventStream requests for inequality.
//        /// </summary>
//        /// <param name="FlushPeriodicEventStreamRequest1">A FlushPeriodicEventStream request.</param>
//        /// <param name="FlushPeriodicEventStreamRequest2">Another FlushPeriodicEventStream request.</param>
//        /// <returns>False if both match; True otherwise.</returns>
//        public static Boolean operator != (FlushPeriodicEventStreamRequest? FlushPeriodicEventStreamRequest1,
//                                           FlushPeriodicEventStreamRequest? FlushPeriodicEventStreamRequest2)

//            => !(FlushPeriodicEventStreamRequest1 == FlushPeriodicEventStreamRequest2);

//        #endregion

//        #endregion

//        #region IEquatable<FlushPeriodicEventStreamRequest> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two FlushPeriodicEventStream requests for equality.
//        /// </summary>
//        /// <param name="Object">A FlushPeriodicEventStream request to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is FlushPeriodicEventStreamRequest addSignaturePolicyRequest &&
//                   Equals(addSignaturePolicyRequest);

//        #endregion

//        #region Equals(FlushPeriodicEventStreamRequest)

//        /// <summary>
//        /// Compares two FlushPeriodicEventStream requests for equality.
//        /// </summary>
//        /// <param name="FlushPeriodicEventStreamRequest">A FlushPeriodicEventStream request to compare with.</param>
//        public override Boolean Equals(FlushPeriodicEventStreamRequest? FlushPeriodicEventStreamRequest)

//            => FlushPeriodicEventStreamRequest is not null &&

//               Id.         Equals(FlushPeriodicEventStreamRequest.Id) &&

//               base.GenericEquals(FlushPeriodicEventStreamRequest);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        private readonly Int32 hashCode;

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => hashCode;

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => Id.ToString();

//        #endregion


//    }

//}
