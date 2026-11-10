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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SyncNetworkTime request: ask a group of time servers now, in a round
    /// like any other - section 3.2.7 of the white paper "Secure Time
    /// Synchronization".
    /// </summary>
    /// <remarks>
    /// Answered at once; the outcome of the round follows in a
    /// NotifyNetworkTimeResult request with the same requestId. It counts as a
    /// round in every respect: one that fails counts towards MaxErrors.
    /// </remarks>
    public class SyncNetworkTimeRequest : ARequest<SyncNetworkTimeRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/syncNetworkTimeRequest");

        /// <summary>
        /// The group asked when the request names none.
        /// </summary>
        public const String LegalGroup = "legal";

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Chosen by the CSMS; the NotifyNetworkTimeResult request carries it.
        /// </summary>
        [Mandatory]
        public Int32          NetworkTimeRequestId    { get; }

        /// <summary>
        /// The group to ask: an instance of NTPClientGroupController. None for
        /// the legal group.
        /// </summary>
        [Optional]
        public String?        Group                   { get; }

        /// <summary>
        /// The group to ask, the legal one where none was named.
        /// </summary>
        public String         GroupOrLegal
            => Group ?? LegalGroup;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SyncNetworkTime request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkTimeRequestId">Chosen by the CSMS; the NotifyNetworkTimeResult request carries it.</param>
        /// <param name="Group">The group to ask; none for the legal group.</param>
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
        public SyncNetworkTimeRequest(SourceRouting            Destination,
                                      Int32                    NetworkTimeRequestId,
                                      String?                  Group                 = null,

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
                   nameof(SyncNetworkTimeRequest)[..^7],

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

            this.NetworkTimeRequestId  = NetworkTimeRequestId;
            this.Group                 = String.IsNullOrWhiteSpace(Group)
                                             ? null
                                             : Group.Trim();

            unchecked
            {

                hashCode = this.NetworkTimeRequestId.GetHashCode()       * 5 ^
                          (this.Group?.              GetHashCode() ?? 0) * 3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "requestId":   4711,
        //     "group":       "legal",
        //     "customData":  { ... }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SyncNetworkTime request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSyncNetworkTimeRequestParser">An optional delegate to parse custom SyncNetworkTime requests.</param>
        public static SyncNetworkTimeRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTimeOffset?                                       RequestTimestamp                     = null,
                                                   TimeSpan?                                             RequestTimeout                       = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<SyncNetworkTimeRequest>?  CustomSyncNetworkTimeRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var syncNetworkTimeRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSyncNetworkTimeRequestParser))
            {
                return syncNetworkTimeRequest;
            }

            throw new ArgumentException("The given JSON representation of a SyncNetworkTime request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SyncNetworkTimeRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SyncNetworkTime request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SyncNetworkTimeRequest">The parsed SyncNetworkTime request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSyncNetworkTimeRequestParser">An optional delegate to parse custom SyncNetworkTime requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out SyncNetworkTimeRequest?      SyncNetworkTimeRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTimeOffset?                                       RequestTimestamp                     = null,
                                       TimeSpan?                                             RequestTimeout                       = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<SyncNetworkTimeRequest>?  CustomSyncNetworkTimeRequestParser   = null)
        {

            try
            {

                SyncNetworkTimeRequest = null;

                #region NetworkTimeRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int32 networkTimeRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Group                   [optional]

                if (JSON.ParseOptional("group",
                                       "group of time servers",
                                       out String? group,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

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


                SyncNetworkTimeRequest = new SyncNetworkTimeRequest(

                                             Destination,
                                             networkTimeRequestId,
                                             group,

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

                if (CustomSyncNetworkTimeRequestParser is not null)
                    SyncNetworkTimeRequest = CustomSyncNetworkTimeRequestParser(JSON,
                                                                                SyncNetworkTimeRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SyncNetworkTimeRequest  = null;
                ErrorResponse           = "The given JSON representation of a SyncNetworkTime request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSyncNetworkTimeRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSyncNetworkTimeRequestSerializer">A delegate to serialize custom SyncNetworkTime requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SyncNetworkTimeRequest>?  CustomSyncNetworkTimeRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",    NetworkTimeRequestId),

                           Group is not null
                               ? new JProperty("group",        Group)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSyncNetworkTimeRequestSerializer is not null
                       ? CustomSyncNetworkTimeRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out SyncNetworkTimeRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a SyncNetworkTime request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="SyncNetworkTimeRequest">The SyncNetworkTime request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSyncNetworkTimeRequestParser">A delegate to read custom SyncNetworkTime requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                          CBOR,
                                           Request_Id                                         RequestId,
                                           SourceRouting                                      Destination,
                                           NetworkPath                                        NetworkPath,
                                           [NotNullWhen(true)]  out SyncNetworkTimeRequest?   SyncNetworkTimeRequest,
                                           [NotNullWhen(false)] out String?                   ErrorResponse,
                                           DateTimeOffset?                                    RequestTimestamp                     = null,
                                           TimeSpan?                                          RequestTimeout                       = null,
                                           EventTracking_Id?                                  EventTrackingId                      = null,
                                           CustomCBORParserDelegate<SyncNetworkTimeRequest>?  CustomSyncNetworkTimeRequestParser   = null)
        {

            try
            {

                SyncNetworkTimeRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a SyncNetworkTime request is not a map!";
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

                CBOR.ParseOptionalText("group",
                                       "group of time servers",
                                       out var group,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

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


                SyncNetworkTimeRequest = new SyncNetworkTimeRequest(

                                             Destination,
                                             (Int32) requestIdNumber,
                                             group,

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

                if (CustomSyncNetworkTimeRequestParser is not null)
                    SyncNetworkTimeRequest = CustomSyncNetworkTimeRequestParser(CBOR,
                                                                                SyncNetworkTimeRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SyncNetworkTimeRequest  = null;
                ErrorResponse           = "The given CBOR representation of a SyncNetworkTime request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomSyncNetworkTimeRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this SyncNetworkTime request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomSyncNetworkTimeRequestSerializer">A delegate to serialize custom SyncNetworkTime requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<SyncNetworkTimeRequest>? CustomSyncNetworkTimeRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("requestId",   CBORValue.FromInt64(NetworkTimeRequestId)),
                           ("group",       OCPPCBORExtensions.Text(Group)),
                           ("signatures",  OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",  CustomData?.ToCBOR())
                       );

            return CustomSyncNetworkTimeRequestSerializer is not null
                       ? CustomSyncNetworkTimeRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SyncNetworkTimeRequest1, SyncNetworkTimeRequest2)

        /// <summary>
        /// Compares two SyncNetworkTime requests for equality.
        /// </summary>
        /// <param name="SyncNetworkTimeRequest1">A SyncNetworkTime request.</param>
        /// <param name="SyncNetworkTimeRequest2">Another SyncNetworkTime request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SyncNetworkTimeRequest? SyncNetworkTimeRequest1,
                                           SyncNetworkTimeRequest? SyncNetworkTimeRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SyncNetworkTimeRequest1, SyncNetworkTimeRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SyncNetworkTimeRequest1 is null || SyncNetworkTimeRequest2 is null)
                return false;

            return SyncNetworkTimeRequest1.Equals(SyncNetworkTimeRequest2);

        }

        #endregion

        #region Operator != (SyncNetworkTimeRequest1, SyncNetworkTimeRequest2)

        /// <summary>
        /// Compares two SyncNetworkTime requests for inequality.
        /// </summary>
        /// <param name="SyncNetworkTimeRequest1">A SyncNetworkTime request.</param>
        /// <param name="SyncNetworkTimeRequest2">Another SyncNetworkTime request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SyncNetworkTimeRequest? SyncNetworkTimeRequest1,
                                           SyncNetworkTimeRequest? SyncNetworkTimeRequest2)

            => !(SyncNetworkTimeRequest1 == SyncNetworkTimeRequest2);

        #endregion

        #endregion

        #region IEquatable<SyncNetworkTimeRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SyncNetworkTime requests for equality.
        /// </summary>
        /// <param name="Object">A SyncNetworkTime request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SyncNetworkTimeRequest syncNetworkTimeRequest &&
                   Equals(syncNetworkTimeRequest);

        #endregion

        #region Equals(SyncNetworkTimeRequest)

        /// <summary>
        /// Compares two SyncNetworkTime requests for equality.
        /// </summary>
        /// <param name="SyncNetworkTimeRequest">A SyncNetworkTime request to compare with.</param>
        public override Boolean Equals(SyncNetworkTimeRequest? SyncNetworkTimeRequest)

            => SyncNetworkTimeRequest is not null &&

               NetworkTimeRequestId.Equals(SyncNetworkTimeRequest.NetworkTimeRequestId) &&
               String.Equals(Group, SyncNetworkTimeRequest.Group)                        &&

               base.GenericEquals(SyncNetworkTimeRequest);

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

            => $"Id: {NetworkTimeRequestId}: group '{GroupOrLegal}'";

        #endregion

    }

}
