/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The NotifyNetworkTimeResult request: the outcome of a round of a group of
    /// time servers, or of a test of one time server, that a CSMS asked for
    /// with SyncNetworkTime or TestTimeServer - section 3.2.7 of the white paper
    /// "Secure Time Synchronization".
    /// </summary>
    /// <remarks>
    /// Sent once, when the round or the test is over, with the requestId of
    /// the request it answers. Exactly one of Round and Test is present.
    /// </remarks>
    public class NotifyNetworkTimeResultRequest : ARequest<NotifyNetworkTimeResultRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyNetworkTimeResultRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The requestId of the SyncNetworkTime or TestTimeServer request this
        /// is the outcome of.
        /// </summary>
        [Mandatory]
        public Int32              NetworkTimeRequestId    { get; }

        /// <summary>
        /// When the round or the test ended.
        /// </summary>
        [Mandatory]
        public DateTimeOffset     GeneratedAt             { get; }

        /// <summary>
        /// The outcome of a SyncNetworkTime.
        /// </summary>
        [Optional]
        public NetworkTimeRound?  Round                   { get; }

        /// <summary>
        /// The outcome of a TestTimeServer.
        /// </summary>
        [Optional]
        public TimeServerTest?    Test                    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyNetworkTimeResult request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkTimeRequestId">The requestId of the request this is the outcome of.</param>
        /// <param name="GeneratedAt">When the round or the test ended.</param>
        /// <param name="Round">The outcome of a SyncNetworkTime.</param>
        /// <param name="Test">The outcome of a TestTimeServer.</param>
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
        public NotifyNetworkTimeResultRequest(SourceRouting            Destination,
                                              Int32                    NetworkTimeRequestId,
                                              DateTimeOffset           GeneratedAt,
                                              NetworkTimeRound?        Round                 = null,
                                              TimeServerTest?          Test                  = null,

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
                   nameof(NotifyNetworkTimeResultRequest)[..^7],

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

            if (Round is null == Test is null)
                throw new ArgumentException("Exactly one of the outcome of a round and the outcome of a test must be given!");

            this.NetworkTimeRequestId  = NetworkTimeRequestId;
            this.GeneratedAt           = GeneratedAt;
            this.Round                 = Round;
            this.Test                  = Test;

            unchecked
            {

                hashCode = this.NetworkTimeRequestId.GetHashCode()       * 11 ^
                           this.GeneratedAt.         GetHashCode()       *  7 ^
                          (this.Round?.              GetHashCode() ?? 0) *  5 ^
                          (this.Test?.               GetHashCode() ?? 0) *  3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "requestId":    4711,
        //     "generatedAt":  "2026-11-10T14:03:12Z",
        //     "round":        { NetworkTimeRoundType },
        //     "test":         { TimeServerTestType },
        //     "customData":   { ... }
        // }
        //
        // Exactly one of "round" and "test".

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyNetworkTimeResult request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyNetworkTimeResultRequestParser">A delegate to parse custom NotifyNetworkTimeResult requests.</param>
        public static NotifyNetworkTimeResultRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTimeOffset?                                               RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<NotifyNetworkTimeResultRequest>?  CustomNotifyNetworkTimeResultRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyNetworkTimeResultRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyNetworkTimeResultRequestParser))
            {
                return notifyNetworkTimeResultRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyNetworkTimeResult request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyNetworkTimeResultRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyNetworkTimeResult request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyNetworkTimeResultRequest">The parsed NotifyNetworkTimeResult request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyNetworkTimeResultRequestParser">A delegate to parse custom NotifyNetworkTimeResult requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out NotifyNetworkTimeResultRequest?      NotifyNetworkTimeResultRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTimeOffset?                                               RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<NotifyNetworkTimeResultRequest>?  CustomNotifyNetworkTimeResultRequestParser   = null)
        {

            try
            {

                NotifyNetworkTimeResultRequest = null;

                #region NetworkTimeRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int32 networkTimeRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region GeneratedAt             [mandatory]

                if (!JSON.ParseMandatory("generatedAt",
                                         "generated at",
                                         out DateTimeOffset generatedAt,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Round                   [optional]

                if (JSON.ParseOptionalJSON("round",
                                           "outcome of a round",
                                           NetworkTimeRound.TryParse,
                                           out NetworkTimeRound? round,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Test                    [optional]

                if (JSON.ParseOptionalJSON("test",
                                           "outcome of a test",
                                           TimeServerTest.TryParse,
                                           out TimeServerTest? test,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                if (round is null == test is null)
                {
                    ErrorResponse = "Exactly one of 'round' and 'test' must be present!";
                    return false;
                }

                #region Signatures              [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData              [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? customData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyNetworkTimeResultRequest = new NotifyNetworkTimeResultRequest(

                                                     Destination,
                                                     networkTimeRequestId,
                                                     generatedAt,
                                                     round,
                                                     test,

                                                     null,
                                                     null,
                                                     signatures,

                                                     customData,

                                                     RequestId,
                                                     RequestTimestamp,
                                                     RequestTimeout,
                                                     EventTrackingId,
                                                     NetworkPath

                                                 );

                if (CustomNotifyNetworkTimeResultRequestParser is not null)
                    NotifyNetworkTimeResultRequest = CustomNotifyNetworkTimeResultRequestParser(JSON,
                                                                                                NotifyNetworkTimeResultRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                NotifyNetworkTimeResultRequest  = null;
                ErrorResponse                   = "The given JSON representation of a NotifyNetworkTimeResult request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomNotifyNetworkTimeResultRequestSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomNotifyNetworkTimeResultRequestSerializer">A delegate to serialize custom NotifyNetworkTimeResult requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                           IncludeJSONLDContext                             = false,
                              CustomJObjectSerializerDelegate<NotifyNetworkTimeResultRequest>?  CustomNotifyNetworkTimeResultRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",     NetworkTimeRequestId),
                                 new JProperty("generatedAt",   GeneratedAt.ToISO8601()),

                           Round is not null
                               ? new JProperty("round",         Round.ToJSON())
                               : null,

                           Test is not null
                               ? new JProperty("test",          Test.ToJSON())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyNetworkTimeResultRequestSerializer is not null
                       ? CustomNotifyNetworkTimeResultRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out NotifyNetworkTimeResultRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a NotifyNetworkTimeResult request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="NotifyNetworkTimeResultRequest">The NotifyNetworkTimeResult request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyNetworkTimeResultRequestParser">A delegate to read custom NotifyNetworkTimeResult requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                                  CBOR,
                                           Request_Id                                                 RequestId,
                                           SourceRouting                                              Destination,
                                           NetworkPath                                                NetworkPath,
                                           [NotNullWhen(true)]  out NotifyNetworkTimeResultRequest?   NotifyNetworkTimeResultRequest,
                                           [NotNullWhen(false)] out String?                           ErrorResponse,
                                           DateTimeOffset?                                            RequestTimestamp                             = null,
                                           TimeSpan?                                                  RequestTimeout                               = null,
                                           EventTracking_Id?                                          EventTrackingId                              = null,
                                           CustomCBORParserDelegate<NotifyNetworkTimeResultRequest>?  CustomNotifyNetworkTimeResultRequestParser   = null)
        {

            try
            {

                NotifyNetworkTimeResultRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a NotifyNetworkTimeResult request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryInt64("requestId",
                                              "request identification",
                                              out var requestIdNumber,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (requestIdNumber < Int32.MinValue || requestIdNumber > Int32.MaxValue)
                {
                    ErrorResponse = $"Invalid request identification '{requestIdNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("generatedAt",
                                              "generated at",
                                              OCPPCBORExtensions.TryParseTimestamp,
                                              out DateTimeOffset generatedAt,
                                              out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptional("round",
                                   "outcome of a round",
                                   NetworkTimeRound.TryParseCBOR,
                                   out NetworkTimeRound? round,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("test",
                                   "outcome of a test",
                                   TimeServerTest.TryParseCBOR,
                                   out TimeServerTest? test,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (round is null == test is null)
                {
                    ErrorResponse = "Exactly one of 'round' and 'test' must be present!";
                    return false;
                }

                CBOR.ParseOptionalList<Signature>("signatures",
                                                  "cryptographic signatures",
                                                  OCPPCBORExtensions.TryParseSignature,
                                                  out var signatures,
                                                  out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? customData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                NotifyNetworkTimeResultRequest = new NotifyNetworkTimeResultRequest(

                                                     Destination,
                                                     (Int32) requestIdNumber,
                                                     generatedAt,
                                                     round,
                                                     test,

                                                     null,
                                                     null,
                                                     signatures,

                                                     customData,

                                                     RequestId,
                                                     RequestTimestamp,
                                                     RequestTimeout,
                                                     EventTrackingId,
                                                     NetworkPath

                                                 );

                if (CustomNotifyNetworkTimeResultRequestParser is not null)
                    NotifyNetworkTimeResultRequest = CustomNotifyNetworkTimeResultRequestParser(CBOR,
                                                                                                NotifyNetworkTimeResultRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                NotifyNetworkTimeResultRequest  = null;
                ErrorResponse                   = "The given CBOR representation of a NotifyNetworkTimeResult request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomNotifyNetworkTimeResultRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this NotifyNetworkTimeResult request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomNotifyNetworkTimeResultRequestSerializer">A delegate to serialize custom NotifyNetworkTimeResult requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<NotifyNetworkTimeResultRequest>? CustomNotifyNetworkTimeResultRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("requestId",    CBORValue.FromInt64(NetworkTimeRequestId)),
                           ("generatedAt",  GeneratedAt.ToCBOR()),
                           ("round",        Round?.ToCBOR()),
                           ("test",         Test?. ToCBOR()),
                           ("signatures",   OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",   CustomData?.ToCBOR())
                       );

            return CustomNotifyNetworkTimeResultRequestSerializer is not null
                       ? CustomNotifyNetworkTimeResultRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyNetworkTimeResultRequest1, NotifyNetworkTimeResultRequest2)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult requests for equality.
        /// </summary>
        /// <param name="NotifyNetworkTimeResultRequest1">A NotifyNetworkTimeResult request.</param>
        /// <param name="NotifyNetworkTimeResultRequest2">Another NotifyNetworkTimeResult request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyNetworkTimeResultRequest? NotifyNetworkTimeResultRequest1,
                                           NotifyNetworkTimeResultRequest? NotifyNetworkTimeResultRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyNetworkTimeResultRequest1, NotifyNetworkTimeResultRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyNetworkTimeResultRequest1 is null || NotifyNetworkTimeResultRequest2 is null)
                return false;

            return NotifyNetworkTimeResultRequest1.Equals(NotifyNetworkTimeResultRequest2);

        }

        #endregion

        #region Operator != (NotifyNetworkTimeResultRequest1, NotifyNetworkTimeResultRequest2)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult requests for inequality.
        /// </summary>
        /// <param name="NotifyNetworkTimeResultRequest1">A NotifyNetworkTimeResult request.</param>
        /// <param name="NotifyNetworkTimeResultRequest2">Another NotifyNetworkTimeResult request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyNetworkTimeResultRequest? NotifyNetworkTimeResultRequest1,
                                           NotifyNetworkTimeResultRequest? NotifyNetworkTimeResultRequest2)

            => !(NotifyNetworkTimeResultRequest1 == NotifyNetworkTimeResultRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyNetworkTimeResultRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyNetworkTimeResult request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyNetworkTimeResultRequest notifyNetworkTimeResultRequest &&
                   Equals(notifyNetworkTimeResultRequest);

        #endregion

        #region Equals(NotifyNetworkTimeResultRequest)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult requests for equality.
        /// </summary>
        /// <param name="NotifyNetworkTimeResultRequest">A NotifyNetworkTimeResult request to compare with.</param>
        public override Boolean Equals(NotifyNetworkTimeResultRequest? NotifyNetworkTimeResultRequest)

            => NotifyNetworkTimeResultRequest is not null &&

               NetworkTimeRequestId.Equals(NotifyNetworkTimeResultRequest.NetworkTimeRequestId) &&
               GeneratedAt.         Equals(NotifyNetworkTimeResultRequest.GeneratedAt)          &&

             ((Round is     null && NotifyNetworkTimeResultRequest.Round is     null) ||
              (Round is not null && NotifyNetworkTimeResultRequest.Round is not null && Round.Equals(NotifyNetworkTimeResultRequest.Round))) &&

             ((Test  is     null && NotifyNetworkTimeResultRequest.Test  is     null) ||
              (Test  is not null && NotifyNetworkTimeResultRequest.Test  is not null && Test. Equals(NotifyNetworkTimeResultRequest.Test)))  &&

               base.GenericEquals(NotifyNetworkTimeResultRequest);

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

            => $"Id: {NetworkTimeRequestId}: {(Round is not null ? Round.ToString() : Test?.ToString())}";

        #endregion

    }

}
