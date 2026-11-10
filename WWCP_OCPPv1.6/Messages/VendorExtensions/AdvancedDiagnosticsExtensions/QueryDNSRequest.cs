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

using System.Globalization;
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
    /// The QueryDNS request: have the charge point look a name up, the way
    /// it looks up names, or ask one of its name servers alone.
    /// </summary>
    /// <remarks>
    /// A Diagnostic Tool of the white paper "Advanced Diagnostics for OCPP".
    /// The record types are text, as the request carries them, and not read
    /// here: a type a charge point does not know is the charge point's to
    /// refuse, with the reason code InvalidRecordType, and not a request that
    /// cannot be read at all.
    ///
    /// OCPP 1.6 has no DNSServerController to name a name server by, so a
    /// name server is named by its place in the configuration key dns.servers,
    /// counted from 1.
    /// </remarks>
    public class QueryDNSRequest : ARequest<QueryDNSRequest>,
                                   IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/queryDNSRequest");

        /// <summary>
        /// The longest name a request may carry: a domain name is at most 253
        /// characters written out, 255 with its root's dot and some room.
        /// </summary>
        public const Int32 MaxNameLength = 255;

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The domain name or IP address to look up.
        /// </summary>
        [Mandatory]
        public String               Name           { get; }

        /// <summary>
        /// The record types to ask for, each a mnemonic, a number or the generic
        /// form "TYPEnnn"; none for A and AAAA, or PTR for an IP address.
        /// </summary>
        [Optional]
        public IEnumerable<String>  RecordTypes    { get; }

        /// <summary>
        /// The one name server to ask, as the n-th server of the configuration
        /// key dns.servers, counted from 1; null for all of them, the way the
        /// charge point resolves names.
        /// </summary>
        [Optional]
        public UInt32?              Server         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new QueryDNS request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Name">The domain name or IP address to look up.</param>
        /// <param name="RecordTypes">The record types to ask for; none for A and AAAA, or PTR for an IP address.</param>
        /// <param name="Server">The one name server to ask, as the n-th server of the configuration key dns.servers, counted from 1.</param>
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
        public QueryDNSRequest(SourceRouting            Destination,
                               String                   Name,
                               IEnumerable<String>?     RecordTypes           = null,
                               UInt32?                  Server                = null,

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
                   nameof(QueryDNSRequest)[..^7],

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

            this.Name         = Name.Trim();
            this.RecordTypes  = RecordTypes?.Select(recordType => recordType.Trim()).ToArray() ?? [];
            this.Server       = Server;

            unchecked
            {

                hashCode = this.Name.        GetHashCode()       * 7 ^
                           this.RecordTypes. Aggregate(0, (hash, recordType) => hash * 31 ^ recordType.GetHashCode()) * 5 ^
                          (this.Server?.     GetHashCode() ?? 0) * 3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "name":        "ntske1.cpo.example",       // mandatory, ≤ 255
        //   "recordTypes": [ "A", "AAAA" ],             // optional, 1…8
        //   "server":      1,                           // optional, ≥ 1
        //   "signatures":  [ ... ],                     // optional
        //   "customData":  { ... }                      // optional
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a QueryDNS request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomQueryDNSRequestParser">An optional delegate to parse custom QueryDNS requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static QueryDNSRequest Parse(JObject                                        JSON,
                                            Request_Id                                     RequestId,
                                            SourceRouting                                  Destination,
                                            NetworkPath                                    NetworkPath,
                                            DateTimeOffset?                                RequestTimestamp              = null,
                                            TimeSpan?                                      RequestTimeout                = null,
                                            EventTracking_Id?                              EventTrackingId               = null,
                                            CustomJObjectParserDelegate<QueryDNSRequest>?  CustomQueryDNSRequestParser   = null,
                                            CustomJObjectParserDelegate<Signature>?        CustomSignatureParser         = null,
                                            CustomJObjectParserDelegate<CustomData>?       CustomCustomDataParser        = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var queryDNSRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomQueryDNSRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return queryDNSRequest;
            }

            throw new ArgumentException("The given JSON representation of a QueryDNS request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out QueryDNSRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a QueryDNS request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="QueryDNSRequest">The parsed QueryDNS request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomQueryDNSRequestParser">An optional delegate to parse custom QueryDNS requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       Request_Id                                     RequestId,
                                       SourceRouting                                  Destination,
                                       NetworkPath                                    NetworkPath,
                                       [NotNullWhen(true)]  out QueryDNSRequest?      QueryDNSRequest,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       DateTimeOffset?                                RequestTimestamp              = null,
                                       TimeSpan?                                      RequestTimeout                = null,
                                       EventTracking_Id?                              EventTrackingId               = null,
                                       CustomJObjectParserDelegate<QueryDNSRequest>?  CustomQueryDNSRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?        CustomSignatureParser         = null,
                                       CustomJObjectParserDelegate<CustomData>?       CustomCustomDataParser        = null)
        {

            try
            {

                QueryDNSRequest = null;

                #region Name           [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "name to look up",
                                             out String name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region RecordTypes    [optional]

                if (!TryParseTexts(JSON["recordTypes"], "recordTypes", out var recordTypes, out ErrorResponse))
                    return false;

                #endregion

                #region Server         [optional]

                if (!TryParseServer(JSON["server"], out var server, out ErrorResponse))
                    return false;

                #endregion

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

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


                if (!TryCheck(name, out ErrorResponse))
                    return false;

                QueryDNSRequest = new QueryDNSRequest(

                                      Destination,
                                      name,
                                      recordTypes,
                                      server,

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

                if (CustomQueryDNSRequestParser is not null)
                    QueryDNSRequest = CustomQueryDNSRequestParser(JSON,
                                                                  QueryDNSRequest);

                return true;

            }
            catch (Exception e)
            {
                QueryDNSRequest  = null;
                ErrorResponse    = "The given JSON representation of a QueryDNS request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomQueryDNSRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomQueryDNSRequestSerializer">A delegate to serialize custom QueryDNS requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<QueryDNSRequest>?  CustomQueryDNSRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?        CustomSignatureSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("name",          Name),

                           RecordTypes.Any()
                               ? new JProperty("recordTypes",   new JArray(RecordTypes))
                               : null,

                           Server.HasValue
                               ? new JProperty("server",        Server.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomQueryDNSRequestSerializer is not null
                       ? CustomQueryDNSRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out QueryDNSRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a QueryDNS request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="QueryDNSRequest">The QueryDNS request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomQueryDNSRequestParser">A delegate to read custom QueryDNS requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                           Request_Id                                  RequestId,
                                           SourceRouting                               Destination,
                                           NetworkPath                                 NetworkPath,
                                           [NotNullWhen(true)]  out QueryDNSRequest?   QueryDNSRequest,
                                           [NotNullWhen(false)] out String?            ErrorResponse,
                                           DateTimeOffset?                             RequestTimestamp              = null,
                                           TimeSpan?                                   RequestTimeout                = null,
                                           EventTracking_Id?                           EventTrackingId               = null,
                                           CustomCBORParserDelegate<QueryDNSRequest>?  CustomQueryDNSRequestParser   = null)
        {

            try
            {

                QueryDNSRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a QueryDNS request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("name",
                                             "name to look up",
                                             out var name,
                                             out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalList<String>("recordTypes",
                                               "record types",
                                               TryParseCBORText,
                                               out var recordTypes,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (!TryParseCBORServer(CBOR, "server", out var server, out ErrorResponse))
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


                if (!TryCheck(name, out ErrorResponse))
                    return false;

                QueryDNSRequest = new QueryDNSRequest(

                                      Destination,
                                      name,
                                      recordTypes,
                                      server,

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

                if (CustomQueryDNSRequestParser is not null)
                    QueryDNSRequest = CustomQueryDNSRequestParser(CBOR,
                                                                  QueryDNSRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                QueryDNSRequest  = null;
                ErrorResponse    = "The given CBOR representation of a QueryDNS request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomQueryDNSRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this QueryDNS request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomQueryDNSRequestSerializer">A delegate to serialize custom QueryDNS requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<QueryDNSRequest>? CustomQueryDNSRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("name",         CBORValue.FromText(Name)),
                           ("recordTypes",  OCPPCBORExtensions.Array(RecordTypes, CBORValue.FromText)),
                           ("server",       OCPPCBORExtensions.UInt(Server)),
                           ("signatures",   OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",   CustomData?.ToCBOR())
                       );

            return CustomQueryDNSRequestSerializer is not null
                       ? CustomQueryDNSRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region (static) TryParseTexts(JSON, Key, out Texts, out ErrorResponse)

        /// <summary>
        /// An optional array of text, in its order: a set would lose the order
        /// in which somebody asked for the record types, which is the order the
        /// answer reports them in.
        /// </summary>
        internal static Boolean TryParseTexts(JToken?                           JSON,
                                              String                            Key,
                                              out List<String>                  Texts,
                                              [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Texts          = [];
            ErrorResponse  = null;

            if (JSON is null || JSON.Type == JTokenType.Null)
                return true;

            if (JSON is not JArray array)
            {
                ErrorResponse = $"'{Key}' must be an array!";
                return false;
            }

            foreach (var item in array)
            {

                if (item.Type != JTokenType.String)
                {
                    ErrorResponse = $"Every entry of '{Key}' must be text!";
                    return false;
                }

                Texts.Add(item.Value<String>() ?? "");

            }

            return true;

        }

        #endregion

        #region (static) TryParseCBORText(CBOR, out Text, out ErrorResponse)

        /// <summary>
        /// One entry of an array of text, as CBOR.
        /// </summary>
        internal static Boolean TryParseCBORText(CBORValue                         CBOR,
                                                 [NotNullWhen(true)]  out String?  Text,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (CBOR.Kind == CBORValueKind.TextString)
            {
                Text           = CBOR.AsText();
                ErrorResponse  = null;
                return true;
            }

            Text           = null;
            ErrorResponse  = "An entry that must be text is not!";
            return false;

        }

        #endregion

        #region (static) TryParseServer(JSON, out Server, out ErrorResponse)

        /// <summary>
        /// An optional name server, as its place in the configuration key
        /// dns.servers: a whole number of at least 1.
        /// </summary>
        /// <remarks>
        /// Any such number, and not only one that names a configured name
        /// server: a place behind the last of them is one the charge point
        /// answers with Rejected and the reason code UnknownServer, and not a
        /// request that cannot be read at all.
        /// </remarks>
        internal static Boolean TryParseServer(JToken?                           JSON,
                                               out UInt32?                       Server,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Server         = null;
            ErrorResponse  = null;

            if (JSON is null || JSON.Type == JTokenType.Null)
                return true;

            if (JSON is not JValue { Type: JTokenType.Integer } value ||
                !UInt32.TryParse(Convert.ToString(value.Value, CultureInfo.InvariantCulture),
                                 NumberStyles.None,
                                 CultureInfo.InvariantCulture,
                                 out var server) ||
                server < 1)
            {
                ErrorResponse = "The name server must be a whole number of at least 1!";
                return false;
            }

            Server = server;
            return true;

        }

        #endregion

        #region (static) TryParseCBORServer(CBOR, Key, out Server, out ErrorResponse)

        /// <summary>
        /// An optional name server, as its place in the configuration key
        /// dns.servers, as CBOR.
        /// </summary>
        internal static Boolean TryParseCBORServer(CBORValue                         CBOR,
                                                   String                            Key,
                                                   out UInt32?                       Server,
                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Server = null;

            CBOR.ParseOptionalUInt64(Key,
                                     "name server",
                                     out var server,
                                     out ErrorResponse);

            if (ErrorResponse is not null)
                return false;

            if (server is < 1 or > UInt32.MaxValue)
            {
                ErrorResponse = "The name server must be a whole number of at least 1!";
                return false;
            }

            Server = (UInt32?) server;
            return true;

        }

        #endregion

        #region (private static) TryCheck(Name, out ErrorResponse)

        /// <summary>
        /// What the white paper limits beyond the types of the properties: a
        /// name of at most 255 characters. Whether it means anything is the
        /// charge point's to say.
        /// </summary>
        /// <remarks>
        /// Not the number of record types: a request with more than eight is
        /// one the charge point answers with Rejected and the reason code
        /// TooManyRecordTypes, and not one that cannot be read at all, which
        /// would be a CALLERROR instead of an answer.
        /// </remarks>
        private static Boolean TryCheck(String                            Name,
                                        [NotNullWhen(false)] out String?  ErrorResponse)
        {

            ErrorResponse = Name.Trim().Length == 0      ? "The name to look up must not be empty!"
                          : Name.Length > MaxNameLength  ? $"The name to look up must not be longer than {MaxNameLength} characters!"
                          : null;

            return ErrorResponse is null;

        }

        #endregion


        #region Operator overloading

        #region Operator == (QueryDNSRequest1, QueryDNSRequest2)

        /// <summary>
        /// Compares two QueryDNS requests for equality.
        /// </summary>
        /// <param name="QueryDNSRequest1">A QueryDNS request.</param>
        /// <param name="QueryDNSRequest2">Another QueryDNS request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (QueryDNSRequest? QueryDNSRequest1,
                                           QueryDNSRequest? QueryDNSRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(QueryDNSRequest1, QueryDNSRequest2))
                return true;

            // If one is null, but not both, return false.
            if (QueryDNSRequest1 is null || QueryDNSRequest2 is null)
                return false;

            return QueryDNSRequest1.Equals(QueryDNSRequest2);

        }

        #endregion

        #region Operator != (QueryDNSRequest1, QueryDNSRequest2)

        /// <summary>
        /// Compares two QueryDNS requests for inequality.
        /// </summary>
        /// <param name="QueryDNSRequest1">A QueryDNS request.</param>
        /// <param name="QueryDNSRequest2">Another QueryDNS request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (QueryDNSRequest? QueryDNSRequest1,
                                           QueryDNSRequest? QueryDNSRequest2)

            => !(QueryDNSRequest1 == QueryDNSRequest2);

        #endregion

        #endregion

        #region IEquatable<QueryDNSRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two QueryDNS requests for equality.
        /// </summary>
        /// <param name="Object">A QueryDNS request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is QueryDNSRequest queryDNSRequest &&
                   Equals(queryDNSRequest);

        #endregion

        #region Equals(QueryDNSRequest)

        /// <summary>
        /// Compares two QueryDNS requests for equality.
        /// </summary>
        /// <param name="QueryDNSRequest">A QueryDNS request to compare with.</param>
        public override Boolean Equals(QueryDNSRequest? QueryDNSRequest)

            => QueryDNSRequest is not null &&

               String.Equals(Name, QueryDNSRequest.Name, StringComparison.Ordinal) &&
               RecordTypes.SequenceEqual(QueryDNSRequest.RecordTypes) &&
               Server == QueryDNSRequest.Server &&

               base.GenericEquals(QueryDNSRequest);

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

                   Name,

                   RecordTypes.Any()
                       ? $" {String.Join(", ", RecordTypes)}"
                       : "",

                   Server.HasValue
                       ? $" @{Server.Value}"
                       : ""

               );

        #endregion

    }

}
