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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A QueryDNS response: what came of the question, or why nothing was
    /// asked.
    /// </summary>
    /// <remarks>
    /// Accepted means that the question was put. What came of it - an
    /// answer, NXDomain, a timeout, a certificate that was refused - is part
    /// of the response and no reason for Rejected, which means that nothing
    /// was asked, with the reason code in ReasonCode and the reason in words
    /// in Error. OCPP 1.6 has no StatusInfo, so the reason code is a property
    /// of its own, which OCPP 2.1 puts into statusInfo.reasonCode.
    /// </remarks>
    public class QueryDNSResponse : AResponse<QueryDNSRequest,
                                              QueryDNSResponse>,
                                    IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/queryDNSResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Accepted when the question was put, Rejected when nothing was asked.
        /// </summary>
        public GenericStatus                       Status             { get; }

        /// <summary>
        /// With Rejected: the reason code - Disabled, UnknownServer, ServerByName,
        /// InvalidName, InvalidRecordType or TooManyRecordTypes.
        /// </summary>
        [Optional]
        public String?                             ReasonCode         { get; }

        /// <summary>
        /// The name that was asked: that of the request, or the reverse name
        /// made of its IP address.
        /// </summary>
        [Optional]
        public String?                             QueriedName        { get; }

        /// <summary>
        /// The IP address of the request, where its reverse name was asked instead.
        /// </summary>
        [Optional]
        public String?                             ReverseLookupOf    { get; }

        /// <summary>
        /// The record types that were asked for.
        /// </summary>
        [Optional]
        public IEnumerable<String>                 RecordTypes        { get; }

        /// <summary>
        /// The one name server that was asked, where the request named one: its
        /// place in the configuration key dns.servers, counted from 1.
        /// </summary>
        [Optional]
        public UInt32?                             Server             { get; }

        /// <summary>
        /// The name server the answer came from, as "transport://address:port".
        /// </summary>
        [Optional]
        public String?                             AnsweredBy         { get; }

        /// <summary>
        /// The RCODE of the answer, as the name of the IANA registry or as its
        /// number; null when no answer came.
        /// </summary>
        [Optional]
        public String?                             ResponseCode       { get; }

        /// <summary>
        /// The Authoritative Answer bit of the answer.
        /// </summary>
        [Optional]
        public Boolean?                            Authoritative      { get; }

        /// <summary>
        /// The Truncation bit of the answer.
        /// </summary>
        [Optional]
        public Boolean?                            Truncated          { get; }

        /// <summary>
        /// Whether no name server answered within its query timeout.
        /// </summary>
        [Optional]
        public Boolean?                            TimedOut           { get; }

        /// <summary>
        /// The security status the charge point validated the answer to:
        /// Secure, Insecure, Bogus or Indeterminate (RFC 4033 section 5).
        /// </summary>
        [Optional]
        public String?                             DNSSEC             { get; }

        /// <summary>
        /// How long the query took, in whole milliseconds - "runtime" in its
        /// JSON. Not Runtime, which is how long this response took to arrive.
        /// </summary>
        [Optional]
        public TimeSpan?                           QueryRuntime       { get; }

        /// <summary>
        /// The records of the answer section, at most 200.
        /// </summary>
        [Optional]
        public IEnumerable<DNSAnswer>              Answers            { get; }

        /// <summary>
        /// How many further records of the answer section were left out.
        /// </summary>
        [Optional]
        public UInt32?                             OmittedAnswers     { get; }

        /// <summary>
        /// What the charge point made of the certificates of the name servers
        /// it asked over TLS or HTTPS during this query.
        /// </summary>
        [Optional]
        public IEnumerable<NameServerCertificate>  Certificates       { get; }

        /// <summary>
        /// Why nothing was asked (Rejected), or why no answer was received
        /// (Accepted), in words.
        /// </summary>
        [Optional]
        public String?                             Error              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new QueryDNS response.
        /// </summary>
        /// <param name="Request">The QueryDNS request leading to this response.</param>
        /// <param name="Status">Accepted when the question was put, Rejected when nothing was asked.</param>
        /// <param name="ReasonCode">With Rejected: the reason code.</param>
        /// <param name="QueriedName">The name that was asked.</param>
        /// <param name="ReverseLookupOf">The IP address of the request, where its reverse name was asked instead.</param>
        /// <param name="RecordTypes">The record types that were asked for.</param>
        /// <param name="Server">The one name server that was asked, where the request named one.</param>
        /// <param name="AnsweredBy">The name server the answer came from, as "transport://address:port".</param>
        /// <param name="ResponseCode">The RCODE of the answer, as the name of the IANA registry or as its number.</param>
        /// <param name="Authoritative">The Authoritative Answer bit of the answer.</param>
        /// <param name="Truncated">The Truncation bit of the answer.</param>
        /// <param name="TimedOut">Whether no name server answered within its query timeout.</param>
        /// <param name="DNSSEC">The security status the charge point validated the answer to.</param>
        /// <param name="QueryRuntime">How long the query took.</param>
        /// <param name="Answers">The records of the answer section.</param>
        /// <param name="OmittedAnswers">How many further records of the answer section were left out.</param>
        /// <param name="Certificates">What the charge point made of the certificates of the name servers it asked.</param>
        /// <param name="Error">Why nothing was asked, or why no answer was received, in words.</param>
        ///
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        ///
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        ///
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        ///
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public QueryDNSResponse(QueryDNSRequest                      Request,
                                GenericStatus                        Status,
                                String?                              ReasonCode            = null,
                                String?                              QueriedName           = null,
                                String?                              ReverseLookupOf       = null,
                                IEnumerable<String>?                 RecordTypes           = null,
                                UInt32?                              Server                = null,
                                String?                              AnsweredBy            = null,
                                String?                              ResponseCode          = null,
                                Boolean?                             Authoritative         = null,
                                Boolean?                             Truncated             = null,
                                Boolean?                             TimedOut              = null,
                                String?                              DNSSEC                = null,
                                TimeSpan?                            QueryRuntime          = null,
                                IEnumerable<DNSAnswer>?              Answers               = null,
                                UInt32?                              OmittedAnswers        = null,
                                IEnumerable<NameServerCertificate>?  Certificates          = null,
                                String?                              Error                 = null,

                                Result?                              Result                = null,
                                DateTimeOffset?                      ResponseTimestamp     = null,

                                SourceRouting?                       Destination           = null,
                                NetworkPath?                         NetworkPath           = null,

                                IEnumerable<KeyPair>?                SignKeys              = null,
                                IEnumerable<SignInfo>?               SignInfos             = null,
                                IEnumerable<Signature>?              Signatures            = null,

                                CustomData?                          CustomData            = null,

                                SerializationFormats?                SerializationFormat   = null,
                                CancellationToken                    CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status           = Status;
            this.ReasonCode       = ReasonCode;
            this.QueriedName      = QueriedName;
            this.ReverseLookupOf  = ReverseLookupOf;
            this.RecordTypes      = RecordTypes?.ToArray()  ?? [];
            this.Server           = Server;
            this.AnsweredBy       = AnsweredBy;
            this.ResponseCode     = ResponseCode;
            this.Authoritative    = Authoritative;
            this.Truncated        = Truncated;
            this.TimedOut         = TimedOut;
            this.DNSSEC           = DNSSEC;
            this.QueryRuntime     = QueryRuntime.HasValue
                                        ? TimeSpan.FromMilliseconds(Math.Floor(Math.Max(0, QueryRuntime.Value.TotalMilliseconds)))
                                        : null;
            this.Answers          = Answers?.ToArray()      ?? [];
            this.OmittedAnswers   = OmittedAnswers > 0 ? OmittedAnswers : null;
            this.Certificates     = Certificates?.ToArray() ?? [];
            this.Error            = Error;

            unchecked
            {

                hashCode = this.Status.          GetHashCode()       * 23 ^
                          (this.ReasonCode?.     GetHashCode() ?? 0) * 19 ^
                          (this.QueriedName?.    GetHashCode() ?? 0) * 17 ^
                          (this.Server?.         GetHashCode() ?? 0) * 13 ^
                          (this.ResponseCode?.   GetHashCode() ?? 0) * 11 ^
                          (this.QueryRuntime?.   GetHashCode() ?? 0) *  7 ^
                           this.Answers.         Aggregate(0, (hash, answer) => hash * 31 ^ answer.GetHashCode()) * 5 ^
                          (this.Error?.          GetHashCode() ?? 0) *  3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // See the white paper "Advanced Diagnostics for OCPP", section QueryDNS.

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a QueryDNS response.
        /// </summary>
        /// <param name="Request">The QueryDNS request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomQueryDNSResponseParser">An optional delegate to parse custom QueryDNS responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static QueryDNSResponse Parse(QueryDNSRequest                                 Request,
                                             JObject                                         JSON,
                                             SourceRouting                                   Destination,
                                             NetworkPath                                     NetworkPath,
                                             DateTimeOffset?                                 ResponseTimestamp              = null,
                                             CustomJObjectParserDelegate<QueryDNSResponse>?  CustomQueryDNSResponseParser   = null,
                                             CustomJObjectParserDelegate<Signature>?         CustomSignatureParser          = null,
                                             CustomJObjectParserDelegate<CustomData>?        CustomCustomDataParser         = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var queryDNSResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomQueryDNSResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return queryDNSResponse;
            }

            throw new ArgumentException("The given JSON representation of a QueryDNS response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out QueryDNSResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a QueryDNS response.
        /// </summary>
        /// <param name="Request">The QueryDNS request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="QueryDNSResponse">The parsed QueryDNS response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomQueryDNSResponseParser">An optional delegate to parse custom QueryDNS responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(QueryDNSRequest                                 Request,
                                       JObject                                         JSON,
                                       SourceRouting                                   Destination,
                                       NetworkPath                                     NetworkPath,
                                       [NotNullWhen(true)]  out QueryDNSResponse?      QueryDNSResponse,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTimeOffset?                                 ResponseTimestamp              = null,
                                       CustomJObjectParserDelegate<QueryDNSResponse>?  CustomQueryDNSResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?         CustomSignatureParser          = null,
                                       CustomJObjectParserDelegate<CustomData>?        CustomCustomDataParser         = null)
        {

            try
            {

                QueryDNSResponse = null;

                #region Status            [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatus.TryParse,
                                         out GenericStatus status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region The texts         [optional]

                if (!TryText(JSON, "reasonCode",       out var reasonCode,       out ErrorResponse) ||
                    !TryText(JSON, "name",             out var queriedName,      out ErrorResponse) ||
                    !TryText(JSON, "reverseLookupOf",  out var reverseLookupOf,  out ErrorResponse) ||
                    !TryText(JSON, "answeredBy",       out var answeredBy,       out ErrorResponse) ||
                    !TryText(JSON, "responseCode",     out var responseCode,     out ErrorResponse) ||
                    !TryText(JSON, "dnssec",           out var dnssec,           out ErrorResponse) ||
                    !TryText(JSON, "error",            out var error,            out ErrorResponse))
                {
                    return false;
                }

                if (!QueryDNSRequest.TryParseTexts(JSON["recordTypes"], "recordTypes", out var recordTypes, out ErrorResponse))
                    return false;

                #endregion

                #region Server            [optional]

                if (!QueryDNSRequest.TryParseServer(JSON["server"], out var server, out ErrorResponse))
                    return false;

                #endregion

                #region The flags         [optional]

                if (!TryFlag(JSON, "authoritative",  out var authoritative,  out ErrorResponse) ||
                    !TryFlag(JSON, "truncated",      out var truncated,      out ErrorResponse) ||
                    !TryFlag(JSON, "timedOut",       out var timedOut,       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region The numbers       [optional]

                if (!TryNumber(JSON, "runtime",         out var runtime,         out ErrorResponse) ||
                    !TryNumber(JSON, "omittedAnswers",  out var omittedAnswers,  out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Answers           [optional]

                if (!TryObjects<DNSAnswer>(JSON, "answers", DNSAnswer.TryParse, out var answers, out ErrorResponse))
                    return false;

                #endregion

                #region Certificates      [optional]

                if (!TryObjects<NameServerCertificate>(JSON, "certificates", NameServerCertificate.TryParse, out var certificates, out ErrorResponse))
                    return false;

                #endregion

                #region Signatures        [optional, OCPP_CSE]

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

                #region CustomData        [optional]

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


                QueryDNSResponse = new QueryDNSResponse(

                                       Request,
                                       status,
                                       reasonCode,
                                       queriedName,
                                       reverseLookupOf,
                                       recordTypes,
                                       server,
                                       answeredBy,
                                       responseCode,
                                       authoritative,
                                       truncated,
                                       timedOut,
                                       dnssec,
                                       runtime.HasValue ? TimeSpan.FromMilliseconds(runtime.Value) : null,
                                       answers,
                                       omittedAnswers.HasValue ? (UInt32) Math.Min(omittedAnswers.Value, UInt32.MaxValue) : null,
                                       certificates,
                                       error,

                                       null,
                                       ResponseTimestamp,

                                       Destination,
                                       NetworkPath,

                                       null,
                                       null,
                                       signatures,

                                       customData

                                   );

                if (CustomQueryDNSResponseParser is not null)
                    QueryDNSResponse = CustomQueryDNSResponseParser(JSON,
                                                                    QueryDNSResponse);

                return true;

            }
            catch (Exception e)
            {
                QueryDNSResponse  = null;
                ErrorResponse     = "The given JSON representation of a QueryDNS response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomQueryDNSResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomQueryDNSResponseSerializer">A delegate to serialize custom QueryDNS responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<QueryDNSResponse>?  CustomQueryDNSResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?         CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",           Status.ToString()),

                           ReasonCode      is not null ? new JProperty("reasonCode",       ReasonCode)                                         : null,
                           QueriedName     is not null ? new JProperty("name",             QueriedName)                                        : null,
                           ReverseLookupOf is not null ? new JProperty("reverseLookupOf",  ReverseLookupOf)                                    : null,
                           RecordTypes.Any()           ? new JProperty("recordTypes",      new JArray(RecordTypes))                            : null,
                           Server.       HasValue      ? new JProperty("server",           Server.       Value)                                : null,
                           AnsweredBy      is not null ? new JProperty("answeredBy",       AnsweredBy)                                         : null,
                           ResponseCode    is not null ? new JProperty("responseCode",     ResponseCode)                                       : null,
                           Authoritative.HasValue      ? new JProperty("authoritative",    Authoritative.Value)                                : null,
                           Truncated.    HasValue      ? new JProperty("truncated",        Truncated.    Value)                                : null,
                           TimedOut.     HasValue      ? new JProperty("timedOut",         TimedOut.     Value)                                : null,
                           DNSSEC          is not null ? new JProperty("dnssec",           DNSSEC)                                             : null,
                           QueryRuntime. HasValue      ? new JProperty("runtime",          (UInt64) QueryRuntime.Value.TotalMilliseconds)      : null,
                           Answers.Any()               ? new JProperty("answers",          new JArray(Answers.     Select(answer      => answer.     ToJSON()))) : null,
                           OmittedAnswers.HasValue     ? new JProperty("omittedAnswers",   OmittedAnswers.Value)                               : null,
                           Certificates.Any()          ? new JProperty("certificates",     new JArray(Certificates.Select(certificate => certificate.ToJSON()))) : null,
                           Error           is not null ? new JProperty("error",            Error)                                              : null,

                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomQueryDNSResponseSerializer is not null
                       ? CustomQueryDNSResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out QueryDNSResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a QueryDNS response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="QueryDNSResponse">The QueryDNS response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomQueryDNSResponseParser">A delegate to read custom QueryDNS responses.</param>
        public static Boolean TryParseCBOR(QueryDNSRequest                              Request,
                                           CBORValue                                    CBOR,
                                           SourceRouting                                Destination,
                                           NetworkPath                                  NetworkPath,
                                           [NotNullWhen(true)]  out QueryDNSResponse?   QueryDNSResponse,
                                           [NotNullWhen(false)] out String?             ErrorResponse,
                                           DateTimeOffset?                              ResponseTimestamp              = null,
                                           CustomCBORParserDelegate<QueryDNSResponse>?  CustomQueryDNSResponseParser   = null)
        {

            try
            {

                QueryDNSResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a QueryDNS response is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("status",
                                             "generic status",
                                             out var statusText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!Enum.TryParse(statusText, out GenericStatus status))
                {
                    ErrorResponse = $"Invalid generic status '{statusText}'!";
                    return false;
                }

                String? reasonCode = null, queriedName = null, reverseLookupOf = null, answeredBy = null, responseCode = null, dnssec = null, error = null;

                foreach (var (key, assign) in new (String, Action<String?>)[] {
                                                  ("reasonCode",       text => reasonCode       = text),
                                                  ("name",             text => queriedName      = text),
                                                  ("reverseLookupOf",  text => reverseLookupOf  = text),
                                                  ("answeredBy",       text => answeredBy       = text),
                                                  ("responseCode",     text => responseCode     = text),
                                                  ("dnssec",           text => dnssec           = text),
                                                  ("error",            text => error            = text)
                                              })
                {

                    CBOR.ParseOptionalText(key, key, out var text, out ErrorResponse);

                    if (ErrorResponse is not null)
                        return false;

                    assign(text);

                }

                CBOR.ParseOptionalList<String>("recordTypes",
                                               "record types",
                                               QueryDNSRequest.TryParseCBORText,
                                               out var recordTypes,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (!QueryDNSRequest.TryParseCBORServer(CBOR, "server", out var server, out ErrorResponse))
                    return false;

                CBOR.ParseOptionalBoolean("authoritative", "authoritative answer", out var authoritative, out ErrorResponse);
                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("truncated",     "truncation",           out var truncated,     out ErrorResponse);
                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("timedOut",      "timed out",            out var timedOut,      out ErrorResponse);
                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalUInt64("runtime",        "runtime",              out var runtime,       out ErrorResponse);
                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalUInt64("omittedAnswers", "omitted answers",      out var omittedAnswers, out ErrorResponse);
                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DNSAnswer>("answers",
                                                  "answers",
                                                  DNSAnswer.TryParseCBOR,
                                                  out var answers,
                                                  out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<NameServerCertificate>("certificates",
                                                              "name server certificates",
                                                              NameServerCertificate.TryParseCBOR,
                                                              out var certificates,
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


                QueryDNSResponse = new QueryDNSResponse(

                                       Request,
                                       status,
                                       reasonCode,
                                       queriedName,
                                       reverseLookupOf,
                                       recordTypes,
                                       server,
                                       answeredBy,
                                       responseCode,
                                       authoritative,
                                       truncated,
                                       timedOut,
                                       dnssec,
                                       runtime.HasValue ? TimeSpan.FromMilliseconds(runtime.Value) : null,
                                       answers,
                                       omittedAnswers.HasValue ? (UInt32) Math.Min(omittedAnswers.Value, UInt32.MaxValue) : null,
                                       certificates,
                                       error,

                                       null,
                                       ResponseTimestamp,

                                       Destination,
                                       NetworkPath,

                                       null,
                                       null,
                                       signatures,

                                       customData

                                   );

                if (CustomQueryDNSResponseParser is not null)
                    QueryDNSResponse = CustomQueryDNSResponseParser(CBOR,
                                                                    QueryDNSResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                QueryDNSResponse  = null;
                ErrorResponse     = "The given CBOR representation of a QueryDNS response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomQueryDNSResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this QueryDNS response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomQueryDNSResponseSerializer">A delegate to serialize custom QueryDNS responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<QueryDNSResponse>? CustomQueryDNSResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",           CBORValue.FromText(Status.ToString())),
                           ("reasonCode",       OCPPCBORExtensions.Text(ReasonCode)),
                           ("name",             OCPPCBORExtensions.Text(QueriedName)),
                           ("reverseLookupOf",  OCPPCBORExtensions.Text(ReverseLookupOf)),
                           ("recordTypes",      OCPPCBORExtensions.Array(RecordTypes, CBORValue.FromText)),
                           ("server",           OCPPCBORExtensions.UInt(Server)),
                           ("answeredBy",       OCPPCBORExtensions.Text(AnsweredBy)),
                           ("responseCode",     OCPPCBORExtensions.Text(ResponseCode)),
                           ("authoritative",    OCPPCBORExtensions.Flag(Authoritative)),
                           ("truncated",        OCPPCBORExtensions.Flag(Truncated)),
                           ("timedOut",         OCPPCBORExtensions.Flag(TimedOut)),
                           ("dnssec",           OCPPCBORExtensions.Text(DNSSEC)),
                           ("runtime",          OCPPCBORExtensions.UInt(QueryRuntime.HasValue ? (UInt64) QueryRuntime.Value.TotalMilliseconds : null)),
                           ("answers",          OCPPCBORExtensions.Array(Answers,      answer      => answer.     ToCBOR())),
                           ("omittedAnswers",   OCPPCBORExtensions.UInt(OmittedAnswers)),
                           ("certificates",     OCPPCBORExtensions.Array(Certificates, certificate => certificate.ToCBOR())),
                           ("error",            OCPPCBORExtensions.Text(Error)),
                           ("signatures",       OCPPCBORExtensions.Array(Signatures,   s           => s.          ToCBOR())),
                           ("customData",       CustomData?.ToCBOR())
                       );

            return CustomQueryDNSResponseSerializer is not null
                       ? CustomQueryDNSResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region (private static) TryText   (JSON, Key, out Text,   out ErrorResponse)

        private static Boolean TryText(JObject JSON, String Key, out String? Text, [NotNullWhen(false)] out String? ErrorResponse)
        {

            Text           = null;
            ErrorResponse  = null;

            if (JSON[Key] is null or { Type: JTokenType.Null })
                return true;

            if (JSON[Key] is not JValue { Type: JTokenType.String } value)
            {
                ErrorResponse = $"'{Key}' must be text!";
                return false;
            }

            Text = value.Value<String>();
            return true;

        }

        #endregion

        #region (private static) TryFlag   (JSON, Key, out Flag,   out ErrorResponse)

        private static Boolean TryFlag(JObject JSON, String Key, out Boolean? Flag, [NotNullWhen(false)] out String? ErrorResponse)
        {

            Flag           = null;
            ErrorResponse  = null;

            if (JSON[Key] is null or { Type: JTokenType.Null })
                return true;

            if (JSON[Key] is not JValue { Type: JTokenType.Boolean } value)
            {
                ErrorResponse = $"'{Key}' must be a boolean!";
                return false;
            }

            Flag = value.Value<Boolean>();
            return true;

        }

        #endregion

        #region (private static) TryNumber (JSON, Key, out Number, out ErrorResponse)

        private static Boolean TryNumber(JObject JSON, String Key, out UInt64? Number, [NotNullWhen(false)] out String? ErrorResponse)
        {

            Number         = null;
            ErrorResponse  = null;

            if (JSON[Key] is null or { Type: JTokenType.Null })
                return true;

            if (JSON[Key] is not JValue { Type: JTokenType.Integer } value || value.Value<Int64>() < 0)
            {
                ErrorResponse = $"'{Key}' must be a number that is not negative!";
                return false;
            }

            Number = value.Value<UInt64>();
            return true;

        }

        #endregion

        #region (private static) TryObjects(JSON, Key, TryParse, out Objects, out ErrorResponse)

        private delegate Boolean TryParseObject<T>(JObject JSON, [NotNullWhen(true)] out T? Value, [NotNullWhen(false)] out String? ErrorResponse);

        private static Boolean TryObjects<T>(JObject                           JSON,
                                             String                            Key,
                                             TryParseObject<T>                 TryParse,
                                             out List<T>                       Objects,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Objects        = [];
            ErrorResponse  = null;

            if (JSON[Key] is null or { Type: JTokenType.Null })
                return true;

            if (JSON[Key] is not JArray array)
            {
                ErrorResponse = $"'{Key}' must be an array!";
                return false;
            }

            foreach (var item in array)
            {

                if (item is not JObject itemJSON ||
                    !TryParse(itemJSON, out var parsed, out ErrorResponse))
                {
                    ErrorResponse ??= $"Every entry of '{Key}' must be an object!";
                    return false;
                }

                Objects.Add(parsed);

            }

            return true;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// Nothing was asked, for the given reason.
        /// </summary>
        /// <param name="Request">The QueryDNS request.</param>
        /// <param name="ReasonCode">The reason code of the white paper: Disabled, UnknownServer, ServerByName, InvalidName, InvalidRecordType, TooManyRecordTypes.</param>
        /// <param name="Error">The reason, in words.</param>
        public static QueryDNSResponse Rejected(QueryDNSRequest  Request,
                                                String           ReasonCode,
                                                String           Error)

            => new (Request,
                    GenericStatus.Rejected,
                    ReasonCode:           ReasonCode,
                    Error:                Error,
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The QueryDNS failed because of a request error.
        /// </summary>
        /// <param name="Request">The QueryDNS request leading to this response.</param>
        public static QueryDNSResponse RequestError(QueryDNSRequest          Request,
                                                    EventTracking_Id         EventTrackingId,
                                                    ResultCode               ErrorCode,
                                                    String?                  ErrorDescription    = null,
                                                    JObject?                 ErrorDetails        = null,
                                                    DateTimeOffset?          ResponseTimestamp   = null,

                                                    SourceRouting?           Destination         = null,
                                                    NetworkPath?             NetworkPath         = null,

                                                    IEnumerable<KeyPair>?    SignKeys            = null,
                                                    IEnumerable<SignInfo>?   SignInfos           = null,
                                                    IEnumerable<Signature>?  Signatures          = null,

                                                    CustomData?              CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   Result:             Result.FromErrorResponse(
                                           ErrorCode,
                                           ErrorDescription,
                                           ErrorDetails
                                       ),
                   ResponseTimestamp:  ResponseTimestamp,

                   Destination:        Destination,
                   NetworkPath:        NetworkPath,

                   SignKeys:           SignKeys,
                   SignInfos:          SignInfos,
                   Signatures:         Signatures,

                   CustomData:         CustomData

               );


        /// <summary>
        /// The QueryDNS failed.
        /// </summary>
        /// <param name="Request">The QueryDNS request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static QueryDNSResponse FormationViolation(QueryDNSRequest  Request,
                                                          String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The QueryDNS failed.
        /// </summary>
        /// <param name="Request">The QueryDNS request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static QueryDNSResponse SignatureError(QueryDNSRequest  Request,
                                                      String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The QueryDNS failed.
        /// </summary>
        /// <param name="Request">The QueryDNS request.</param>
        /// <param name="Description">An optional error description.</param>
        public static QueryDNSResponse Failed(QueryDNSRequest  Request,
                                              String?          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The QueryDNS failed because of an exception.
        /// </summary>
        /// <param name="Request">The QueryDNS request.</param>
        /// <param name="Exception">The exception.</param>
        public static QueryDNSResponse ExceptionOccurred(QueryDNSRequest  Request,
                                                         Exception        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (QueryDNSResponse1, QueryDNSResponse2)

        /// <summary>
        /// Compares two QueryDNS responses for equality.
        /// </summary>
        /// <param name="QueryDNSResponse1">A QueryDNS response.</param>
        /// <param name="QueryDNSResponse2">Another QueryDNS response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (QueryDNSResponse? QueryDNSResponse1,
                                           QueryDNSResponse? QueryDNSResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(QueryDNSResponse1, QueryDNSResponse2))
                return true;

            // If one is null, but not both, return false.
            if (QueryDNSResponse1 is null || QueryDNSResponse2 is null)
                return false;

            return QueryDNSResponse1.Equals(QueryDNSResponse2);

        }

        #endregion

        #region Operator != (QueryDNSResponse1, QueryDNSResponse2)

        /// <summary>
        /// Compares two QueryDNS responses for inequality.
        /// </summary>
        /// <param name="QueryDNSResponse1">A QueryDNS response.</param>
        /// <param name="QueryDNSResponse2">Another QueryDNS response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (QueryDNSResponse? QueryDNSResponse1,
                                           QueryDNSResponse? QueryDNSResponse2)

            => !(QueryDNSResponse1 == QueryDNSResponse2);

        #endregion

        #endregion

        #region IEquatable<QueryDNSResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two QueryDNS responses for equality.
        /// </summary>
        /// <param name="Object">A QueryDNS response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is QueryDNSResponse queryDNSResponse &&
                   Equals(queryDNSResponse);

        #endregion

        #region Equals(QueryDNSResponse)

        /// <summary>
        /// Compares two QueryDNS responses for equality.
        /// </summary>
        /// <param name="QueryDNSResponse">A QueryDNS response to compare with.</param>
        public override Boolean Equals(QueryDNSResponse? QueryDNSResponse)

            => QueryDNSResponse is not null &&

               Status.Equals(QueryDNSResponse.Status) &&

               String.Equals(ReasonCode,       QueryDNSResponse.ReasonCode,       StringComparison.Ordinal) &&
               String.Equals(QueriedName,      QueryDNSResponse.QueriedName,      StringComparison.Ordinal) &&
               String.Equals(ReverseLookupOf,  QueryDNSResponse.ReverseLookupOf,  StringComparison.Ordinal) &&
               RecordTypes.SequenceEqual(QueryDNSResponse.RecordTypes) &&
               Server         == QueryDNSResponse.Server        &&
               String.Equals(AnsweredBy,       QueryDNSResponse.AnsweredBy,       StringComparison.Ordinal) &&
               String.Equals(ResponseCode,     QueryDNSResponse.ResponseCode,     StringComparison.Ordinal) &&
               Authoritative  == QueryDNSResponse.Authoritative &&
               Truncated      == QueryDNSResponse.Truncated     &&
               TimedOut       == QueryDNSResponse.TimedOut      &&
               String.Equals(DNSSEC,           QueryDNSResponse.DNSSEC,           StringComparison.Ordinal) &&
               QueryRuntime   == QueryDNSResponse.QueryRuntime  &&
               Answers.SequenceEqual(QueryDNSResponse.Answers) &&
               OmittedAnswers == QueryDNSResponse.OmittedAnswers &&
               Certificates.SequenceEqual(QueryDNSResponse.Certificates) &&
               String.Equals(Error,            QueryDNSResponse.Error,            StringComparison.Ordinal);

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

                   Status,

                   ReasonCode is not null
                       ? $" ({ReasonCode})"
                       : "",

                   ResponseCode is not null
                       ? $", {QueriedName} -> {ResponseCode} from {AnsweredBy}, {Answers.Count()} answer(s)"
                       : "",

                   Error is not null
                       ? $": {Error}"
                       : ""

               );

        #endregion

    }

}
