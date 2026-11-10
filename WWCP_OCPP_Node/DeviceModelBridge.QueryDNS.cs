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

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// QueryDNS of the white paper "Advanced Diagnostics for OCPP": a CSMS has
    /// the node look a name up, the way it looks up names, or ask one of its
    /// name servers alone.
    /// </summary>
    /// <remarks>
    /// Answered by <see cref="WWCPNode.ResolveAsync"/>, as the DNS test of the
    /// web interface and queryDNS of the command line are, so that none of
    /// the three can say something about a name server the others do not.
    /// What this adds is OCPP's: a name server named by the id of its
    /// DNSServerController - in OCPP 1.6, which has none, by its place in the
    /// configuration key dns.servers - and Rejected with a reason code where
    /// nothing was asked - every other outcome is an answer, and Accepted.
    /// </remarks>
    public sealed partial class DeviceModelBridge
    {

        #region QueryDNS(Request, CancellationToken = default)

        /// <summary>
        /// Answer a QueryDNS request of OCPP 2.1.
        /// </summary>
        /// <param name="Request">The QueryDNS request.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task<OCPPv2_1.CS.QueryDNSResponse> QueryDNS(OCPPv2_1.CSMS.QueryDNSRequest  Request,
                                                                 CancellationToken              CancellationToken   = default)
        {

            if (!WWCPNode.TryParseRecordTypes(Request.RecordTypes, out var recordTypes, out var error, out var refused))
                return OCPPv2_1.CS.QueryDNSResponse.Rejected(Request, refused, error);

            #region The one name server, by the id of its DNSServerController

            Int32? server = null;

            if (Request.ServerId is String serverId)
            {

                var ids   = node.DNSServerIds;
                var index = ids.ToList().FindIndex(id => String.Equals(id, serverId, StringComparison.OrdinalIgnoreCase));

                if (index < 0)
                    return OCPPv2_1.CS.QueryDNSResponse.Rejected(
                               Request,
                               DNSQueryRefusals.UnknownServer,
                               ids.Count == 0
                                   ? $"This {node.Kind.Name} has no name server to ask on its own."
                                   : $"'{serverId}' is none of this {node.Kind.Name}'s name servers, which are {String.Join(", ", ids)}."
                           );

                server = index;

            }

            #endregion

            // The line the web interface and the command line write, naming
            // whoever sent the request.
            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP to resolve '{Request.Name}'.",
                          "dns", "test", "ocpp");

            var result = await node.ResolveAsync(Request.Name, recordTypes, server, CancellationToken);

            if (result.Value<String>("refused") is String reasonCode)
                return OCPPv2_1.CS.QueryDNSResponse.Rejected(Request, reasonCode, result.Value<String>("error") ?? reasonCode);

            var answer = Answered(result);

            return new OCPPv2_1.CS.QueryDNSResponse(
                       Request,
                       OCPPv2_1.GenericStatus.Accepted,
                       QueriedName:          answer.Name,
                       ReverseLookupOf:      answer.ReverseLookupOf,
                       RecordTypes:          answer.RecordTypes,
                       ServerId:             answer.ServerId,
                       AnsweredBy:           answer.AnsweredBy,
                       ResponseCode:         answer.ResponseCode,
                       Authoritative:        answer.Authoritative,
                       Truncated:            answer.Truncated,
                       TimedOut:             answer.TimedOut,
                       DNSSEC:               answer.DNSSEC,
                       QueryRuntime:         answer.Runtime,
                       Answers:              answer.Records.Select(record => new OCPPv2_1.DNSAnswer(record.Name, record.Type, record.TimeToLive, record.Value)),
                       OmittedAnswers:       answer.Omitted,
                       Certificates:         answer.Certificates.Select(certificate => new OCPPv2_1.NameServerCertificate(certificate.Server,
                                                                                                                         certificate.Accepted,
                                                                                                                         certificate.Status,
                                                                                                                         certificate.CertificateFingerprint,
                                                                                                                         certificate.RootFingerprint)),
                       Error:                answer.Error,
                       SerializationFormat:  Request.SerializationFormat
                   );

        }

        #endregion

        #region QueryDNS(Request, CancellationToken = default)

        /// <summary>
        /// Answer a QueryDNS request of OCPP 1.6.
        /// </summary>
        /// <remarks>
        /// As for OCPP 2.1, but with a name server named by its place in the
        /// configuration key dns.servers, counted from 1 - the place the page
        /// and the command line count it by, too - and with the reason code
        /// as a property of its own, as OCPP 1.6 has no StatusInfo.
        /// </remarks>
        /// <param name="Request">The QueryDNS request.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task<OCPPv1_6.CP.QueryDNSResponse> QueryDNS(OCPPv1_6.CS.QueryDNSRequest  Request,
                                                                 CancellationToken            CancellationToken   = default)
        {

            if (!WWCPNode.TryParseRecordTypes(Request.RecordTypes, out var recordTypes, out var error, out var refused))
                return OCPPv1_6.CP.QueryDNSResponse.Rejected(Request, refused, error);

            #region The one name server, by its place in dns.servers

            Int32? server = null;

            if (Request.Server is UInt32 place)
            {

                var ids = node.DNSServerIds;

                if (place < 1 || place > ids.Count)
                    return OCPPv1_6.CP.QueryDNSResponse.Rejected(
                               Request,
                               DNSQueryRefusals.UnknownServer,
                               ids.Count == 0
                                   ? $"This {node.Kind.Name} has no name server to ask on its own."
                                   : $"This {node.Kind.Name} has no name server number {place}, only {ids.Count}."
                           );

                server = (Int32) place - 1;

            }

            #endregion

            // The line the web interface and the command line write, naming
            // whoever sent the request.
            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP to resolve '{Request.Name}'.",
                          "dns", "test", "ocpp");

            var result = await node.ResolveAsync(Request.Name, recordTypes, server, CancellationToken);

            if (result.Value<String>("refused") is String reasonCode)
                return OCPPv1_6.CP.QueryDNSResponse.Rejected(Request, reasonCode, result.Value<String>("error") ?? reasonCode);

            var answer = Answered(result);

            return new OCPPv1_6.CP.QueryDNSResponse(
                       Request,
                       OCPPv1_6.GenericStatus.Accepted,
                       QueriedName:          answer.Name,
                       ReverseLookupOf:      answer.ReverseLookupOf,
                       RecordTypes:          answer.RecordTypes,
                       Server:               Request.Server,
                       AnsweredBy:           answer.AnsweredBy,
                       ResponseCode:         answer.ResponseCode,
                       Authoritative:        answer.Authoritative,
                       Truncated:            answer.Truncated,
                       TimedOut:             answer.TimedOut,
                       DNSSEC:               answer.DNSSEC,
                       QueryRuntime:         answer.Runtime,
                       Answers:              answer.Records.Select(record => new OCPPv1_6.DNSAnswer(record.Name, record.Type, record.TimeToLive, record.Value)),
                       OmittedAnswers:       answer.Omitted,
                       Certificates:         answer.Certificates.Select(certificate => new OCPPv1_6.NameServerCertificate(certificate.Server,
                                                                                                                         certificate.Accepted,
                                                                                                                         certificate.Status,
                                                                                                                         certificate.CertificateFingerprint,
                                                                                                                         certificate.RootFingerprint)),
                       Error:                answer.Error,
                       SerializationFormat:  Request.SerializationFormat
                   );

        }

        #endregion


        #region (internal) QueryDNSAnswer

        /// <summary>
        /// What a node's test query brought back, read out of its JSON once
        /// for every version of OCPP that reports it.
        /// </summary>
        internal sealed record QueryDNSAnswer(String?                                    Name,
                                              String?                                    ReverseLookupOf,
                                              IReadOnlyList<String>                      RecordTypes,
                                              String?                                    ServerId,
                                              String?                                    AnsweredBy,
                                              String?                                    ResponseCode,
                                              Boolean?                                   Authoritative,
                                              Boolean?                                   Truncated,
                                              Boolean?                                   TimedOut,
                                              String?                                    DNSSEC,
                                              TimeSpan?                                  Runtime,
                                              IReadOnlyList<QueryDNSRecord>              Records,
                                              UInt32?                                    Omitted,
                                              IReadOnlyList<QueryDNSCertificate>         Certificates,
                                              String?                                    Error);

        /// <summary>
        /// One record of the answer section.
        /// </summary>
        internal sealed record QueryDNSRecord(String Name, String Type, TimeSpan TimeToLive, String Value);

        /// <summary>
        /// What was made of the certificate of one name server.
        /// </summary>
        internal sealed record QueryDNSCertificate(String Server, Boolean Accepted, String Status, String? CertificateFingerprint, String? RootFingerprint);

        #endregion

        #region (internal static) Answered(Result)

        /// <summary>
        /// The answer of <see cref="WWCPNode.ResolveAsync"/>, in the words of
        /// QueryDNS.
        /// </summary>
        /// <remarks>
        /// A judgement's outcome is the value of DNSServerController.CertificateStatus
        /// it stands for, as the Device Model reports it after every handshake.
        /// </remarks>
        /// <param name="Result">What the node's test query brought back.</param>
        internal static QueryDNSAnswer Answered(JObject Result)

            => new (
                   Result.Value<String>("name"),
                   Result.Value<String>("reverseLookupOf"),
                   (Result["recordTypes"] as JArray ?? []).Select(type => type.ToString()).ToArray(),
                   Result.Value<String>("serverId"),
                   Result.Value<String>("answeredBy"),
                   Result.Value<String>("responseCode"),
                   Result.Value<Boolean?>("authoritative"),
                   Result.Value<Boolean?>("truncated"),
                   Result.Value<Boolean?>("timedOut"),
                   Result.Value<String>("dnssec"),
                   Result.Value<Int64?>("runtime_ms") is Int64 milliseconds
                       ? TimeSpan.FromMilliseconds(milliseconds)
                       : null,
                   (Result["answers"] as JArray ?? []).OfType<JObject>().
                       Select(record => new QueryDNSRecord(record.Value<String>("name")  ?? "",
                                                           record.Value<String>("type")  ?? "",
                                                           TimeSpan.FromSeconds(Math.Max(0, record.Value<Int64>("timeToLive"))),
                                                           record.Value<String>("value") ?? "")).
                       ToArray(),
                   Result.Value<Int64?>("more") is Int64 more && more > 0
                       ? (UInt32) Math.Min(more, UInt32.MaxValue)
                       : null,
                   (Result["certificates"] as JArray ?? []).OfType<JObject>().
                       Select(judgement => new QueryDNSCertificate(judgement.Value<String>("server") ?? "",
                                                                   judgement.Value<Boolean>("accepted"),
                                                                   WWCPNode.CertificateStatusOf(judgement.Value<String>("outcome")),
                                                                   judgement.Value<String>("certificate"),
                                                                   judgement.Value<String>("root"))).
                       ToArray(),
                   Result.Value<String>("error")
               );

        #endregion

    }

}
