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
using cloud.charging.open.protocols.WWCP.Node;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// SyncNetworkTime and TestTimeServer of the white paper "Secure Time
    /// Synchronization", section 3.2.7 for OCPP 2.1 and section 3.3.5 for
    /// OCPP 1.6: a CSMS has the node ask a group of its time servers now, or
    /// test one of them step by step.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Answered by <see cref="WWCPNode.SyncTimeAsync(NTSGroup, CancellationToken)"/>
    /// and <see cref="WWCPNode.TestTimeServerAsync(NTSGroup, String, CancellationToken)"/>,
    /// as syncNTS on the command line and 'Sync now' and 'Test' on the NTS page
    /// are, so that none of them can say something about a time server the
    /// others do not. A round asked for here is a round like every other: it
    /// sets the time of the group, and one that fails counts towards MaxErrors.
    /// </para>
    /// <para>
    /// What this adds is OCPP's: a server named by the id of its
    /// NTPClientController in 2.1 and by its group and name in 1.6, Rejected
    /// with a reason code where nothing is asked, Accepted at once otherwise -
    /// and the outcome told later, in a NotifyNetworkTimeResult request,
    /// because a round can take several timeouts and OCPP-J lets the CSMS have
    /// one request outstanding at a time.
    /// </para>
    /// <para>
    /// One requested round or test at a time, whichever version asked: a
    /// request that comes while one runs is Busy. The rounds the node checks
    /// its clock with on its own are not counted; they go on as they always do.
    /// </para>
    /// </remarks>
    public sealed partial class DeviceModelBridge
    {

        #region Data

        /// <summary>
        /// 1 while a round or test a CSMS asked for runs, 0 otherwise.
        /// </summary>
        private Int32 networkTimeRequested;

        #endregion

        #region Events

        /// <summary>
        /// The outcome of a round or a test a CSMS of OCPP 2.1 asked for, just
        /// before it is sent - for whoever wants to see it whether or not the
        /// CSMS can be reached.
        /// </summary>
        public event Action<OCPPv2_1.CS.NotifyNetworkTimeResultRequest>? OnNetworkTimeResult;

        /// <summary>
        /// The same for a central system of OCPP 1.6.
        /// </summary>
        public event Action<OCPPv1_6.CP.NotifyNetworkTimeResultRequest>? OnNetworkTimeResult16;

        #endregion


        #region (private) ServeNetworkTimeOnDemand(Node / ChargePoint)

        /// <summary>
        /// Answer SyncNetworkTime and TestTimeServer of the given node of OCPP
        /// 2.1, where it leaves them to whoever keeps its time.
        /// </summary>
        /// <param name="Node">A node of OCPP 2.1.</param>
        private void ServeNetworkTimeOnDemand(OCPPv2_1.IDeviceModelNode Node)
        {

            if (Node is not OCPPv2_1.INetworkTimeOnDemandNode onDemand)
                return;

            onDemand.SyncNetworkTimeProcessor = (timestamp, sender, connection, request, cancellationToken) => SyncNetworkTime(request);
            onDemand.TestTimeServerProcessor  = (timestamp, sender, connection, request, cancellationToken) => TestTimeServer (request);

        }

        /// <summary>
        /// The same for a node of OCPP 1.6.
        /// </summary>
        /// <param name="ChargePoint">A node of OCPP 1.6.</param>
        private void ServeNetworkTimeOnDemand(OCPPv1_6.CP.IConfigurationKeysNode ChargePoint)
        {

            if (ChargePoint is not OCPPv1_6.INetworkTimeOnDemandNode onDemand)
                return;

            onDemand.SyncNetworkTimeProcessor = (timestamp, sender, connection, request, cancellationToken) => SyncNetworkTime(request);
            onDemand.TestTimeServerProcessor  = (timestamp, sender, connection, request, cancellationToken) => TestTimeServer (request);

        }

        #endregion


        #region SyncNetworkTime(Request)   of OCPP 2.1

        /// <summary>
        /// Answer a SyncNetworkTime request of OCPP 2.1: ask the group now, and
        /// tell the CSMS what came of it once the round is over.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        public Task<OCPPv2_1.CS.SyncNetworkTimeResponse> SyncNetworkTime(OCPPv2_1.CSMS.SyncNetworkTimeRequest Request)

            => Task.FromResult(

                   BeginRound(Request.GroupOrLegal,
                              Request.NetworkTimeRequestId,
                              Asker(Request.NetworkPath, Request.Signatures),
                              (group, result) => Tell2_1(Request.NetworkTimeRequestId, Round: RoundOf(group, result)),
                              out var refusal,
                              out var why)

                       ? OCPPv2_1.CS.SyncNetworkTimeResponse.Accepted(Request)
                       : OCPPv2_1.CS.SyncNetworkTimeResponse.Rejected(Request, refusal, why)

               );

        #endregion

        #region SyncNetworkTime(Request)   of OCPP 1.6

        /// <summary>
        /// Answer a SyncNetworkTime request of OCPP 1.6: the same, with the
        /// clients of the round named by their host names alone, since OCPP 1.6
        /// has no client ids.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        public Task<OCPPv1_6.CP.SyncNetworkTimeResponse> SyncNetworkTime(OCPPv1_6.CS.SyncNetworkTimeRequest Request)

            => Task.FromResult(

                   BeginRound(Request.GroupOrLegal,
                              Request.NetworkTimeRequestId,
                              Asker(Request.NetworkPath, Request.Signatures),
                              (group, result) => Tell1_6(Request.NetworkTimeRequestId, Round: WithoutClientIds(RoundOf(group, result))),
                              out var refusal,
                              out var why)

                       ? OCPPv1_6.CP.SyncNetworkTimeResponse.Accepted(Request)
                       : OCPPv1_6.CP.SyncNetworkTimeResponse.Rejected(Request, refusal, why)

               );

        #endregion

        #region TestTimeServer (Request)   of OCPP 2.1

        /// <summary>
        /// Answer a TestTimeServer request of OCPP 2.1: ask the client step by
        /// step, and tell the CSMS what came of it once the test is over.
        /// </summary>
        /// <param name="Request">The TestTimeServer request.</param>
        public Task<OCPPv2_1.CS.TestTimeServerResponse> TestTimeServer(OCPPv2_1.CSMS.TestTimeServerRequest Request)
        {

            // Only a server of the node: a name that was mistyped would
            // otherwise be a key exchange with whoever answers to it.
            if (!TryFindClient(Request.Client, out var group, out var host))
            {

                var clients = node.NTSGroups.SelectMany(other => other.ServerIds).ToArray();

                return Task.FromResult(
                           OCPPv2_1.CS.TestTimeServerResponse.Rejected(
                               Request,
                               NetworkTimeRefusals.UnknownClient,
                               clients.Length == 0
                                   ? $"This {node.Kind.Name} has no time server to test."
                                   : $"'{Request.Client}' is none of this {node.Kind.Name}'s time servers, which are {String.Join(", ", clients)}."
                           )
                       );

            }

            return Task.FromResult(

                       BeginTest(group,
                                 host,
                                 Request.NetworkTimeRequestId,
                                 Asker(Request.NetworkPath, Request.Signatures),
                                 result => Tell2_1(Request.NetworkTimeRequestId, Test: TestOf(Request.Client, result)),
                                 out var refusal,
                                 out var why)

                           ? OCPPv2_1.CS.TestTimeServerResponse.Accepted(Request)
                           : OCPPv2_1.CS.TestTimeServerResponse.Rejected(Request, refusal, why)

                   );

        }

        #endregion

        #region TestTimeServer (Request)   of OCPP 1.6

        /// <summary>
        /// Answer a TestTimeServer request of OCPP 1.6, which names the server
        /// by its group and by its name as ntp.&lt;group&gt;.servers lists it.
        /// </summary>
        /// <param name="Request">The TestTimeServer request.</param>
        public Task<OCPPv1_6.CP.TestTimeServerResponse> TestTimeServer(OCPPv1_6.CS.TestTimeServerRequest Request)
        {

            var group = node.NTSGroupOf(Request.GroupOrLegal);

            if (group is null)
                return Task.FromResult(
                           OCPPv1_6.CP.TestTimeServerResponse.Rejected(
                               Request,
                               NetworkTimeRefusals.UnknownGroup,
                               UnknownGroupText(Request.GroupOrLegal)
                           )
                       );

            var wanted = Request.Server.Trim().TrimEnd('.');
            var host   = group.Sources.Sources.Select(source => source.Hostname.Trimmed).
                                              FirstOrDefault(name => String.Equals(name, wanted, StringComparison.OrdinalIgnoreCase));

            if (host is null)
            {

                var servers = group.Sources.Sources.Select(source => source.Hostname.Trimmed).ToArray();

                return Task.FromResult(
                           OCPPv1_6.CP.TestTimeServerResponse.Rejected(
                               Request,
                               NetworkTimeRefusals.UnknownServer,
                               servers.Length == 0
                                   ? $"ntp.{group.Id}.servers names no time server."
                                   : $"'{Request.Server}' is none of the servers of ntp.{group.Id}.servers, which are {String.Join(", ", servers)}."
                           )
                       );

            }

            return Task.FromResult(

                       BeginTest(group,
                                 host,
                                 Request.NetworkTimeRequestId,
                                 Asker(Request.NetworkPath, Request.Signatures),
                                 result => Tell1_6(Request.NetworkTimeRequestId, Test: TestOf(null, result)),
                                 out var refusal,
                                 out var why)

                           ? OCPPv1_6.CP.TestTimeServerResponse.Accepted(Request)
                           : OCPPv1_6.CP.TestTimeServerResponse.Rejected(Request, refusal, why)

                   );

        }

        #endregion


        #region (private) BeginRound(GroupId, NetworkTimeRequestId, Asker, Tell, out Refusal, out Why)

        /// <summary>
        /// Begin a round of the given group in the background and tell its
        /// outcome when it is over - or say why nothing is asked.
        /// </summary>
        /// <returns>Whether the round was begun.</returns>
        private Boolean BeginRound(String                               GroupId,
                                   Int32                                NetworkTimeRequestId,
                                   String                               Asker,
                                   Func<NTSGroup, JObject, Task>        Tell,
                                   [NotNullWhen(false)] out String?     Refusal,
                                   [NotNullWhen(false)] out String?     Why)
        {

            var group = node.NTSGroupOf(GroupId);

            if (group is null)
            {
                Refusal  = NetworkTimeRefusals.UnknownGroup;
                Why      = UnknownGroupText(GroupId);
                return false;
            }

            if (!group.Enabled)
            {
                Refusal  = NetworkTimeRefusals.Disabled;
                Why      = $"NTS group '{group.Id}' is switched off, so it asks nobody.";
                return false;
            }

            if (Interlocked.CompareExchange(ref networkTimeRequested, 1, 0) != 0)
            {
                Refusal  = NetworkTimeRefusals.Busy;
                Why      = "A round or a test asked for before is still running.";
                return false;
            }

            // The line the command line and the page write when they are asked,
            // naming whoever sent the request. Before anything is asked: what
            // follows records which servers were asked and what came of it, and
            // nothing in it says who wanted it.
            node.Log.Notice(
                (group.IsLegal
                     ? $"{Asker} asked this {node.Kind.Name} over OCPP to synchronise its time"
                     : $"{Asker} asked this {node.Kind.Name} over OCPP to check its time against the NTS group '{group.Id}'") +
                $" (request {NetworkTimeRequestId}).",
                "nts", "test", "ocpp"
            );

            _ = Task.Run(async () => {

                try
                {

                    // Not with the request's token: the request is answered at
                    // once, and the round is a round all the same.
                    await Tell(group, await node.SyncTimeAsync(group));

                }
                catch (Exception e)
                {
                    node.Log.Warning($"The round of NTS group '{group.Id}' that {Asker} asked for over OCPP could not be told: {e.Message}", "nts", "ocpp");
                }
                finally
                {
                    Interlocked.Exchange(ref networkTimeRequested, 0);
                }

            });

            Refusal  = null;
            Why      = null;
            return true;

        }

        #endregion

        #region (private) BeginTest (Group, Host, NetworkTimeRequestId, Asker, Tell, out Refusal, out Why)

        /// <summary>
        /// Begin a test of the given server of the given group in the
        /// background and tell its outcome when it is over - or say why nothing
        /// is asked.
        /// </summary>
        /// <returns>Whether the test was begun.</returns>
        private Boolean BeginTest(NTSGroup                             Group,
                                  String                               Host,
                                  Int32                                NetworkTimeRequestId,
                                  String                               Asker,
                                  Func<JObject, Task>                  Tell,
                                  [NotNullWhen(false)] out String?     Refusal,
                                  [NotNullWhen(false)] out String?     Why)
        {

            // A group that is switched off asks nobody, not even for a test.
            if (!Group.Enabled)
            {
                Refusal  = NetworkTimeRefusals.Disabled;
                Why      = $"NTS group '{Group.Id}' of '{Host}' is switched off, so it asks nobody.";
                return false;
            }

            if (Interlocked.CompareExchange(ref networkTimeRequested, 1, 0) != 0)
            {
                Refusal  = NetworkTimeRefusals.Busy;
                Why      = "A round or a test asked for before is still running.";
                return false;
            }

            node.Log.Info(
                $"{Asker} asked this {node.Kind.Name} over OCPP to test the time server '{Host}'" +
                (Group.IsLegal ? "" : $" of the NTS group '{Group.Id}'") +
                $" (request {NetworkTimeRequestId}).",
                "nts", "test", "ocpp"
            );

            _ = Task.Run(async () => {

                try
                {
                    await Tell(await node.TestTimeServerAsync(Group, Host));
                }
                catch (Exception e)
                {
                    node.Log.Warning($"The test of '{Host}' that {Asker} asked for over OCPP could not be told: {e.Message}", "nts", "ocpp");
                }
                finally
                {
                    Interlocked.Exchange(ref networkTimeRequested, 0);
                }

            });

            Refusal  = null;
            Why      = null;
            return true;

        }

        #endregion


        #region (private) TryFindClient(Client, out Group, out Host)

        /// <summary>
        /// The group and the host name of the client of the given id: the
        /// instance of its NTPClientController, in any case.
        /// </summary>
        private Boolean TryFindClient(String                          Client,
                                      [NotNullWhen(true)] out NTSGroup?  Group,
                                      [NotNullWhen(true)] out String?    Host)
        {

            foreach (var group in node.NTSGroups)
            {

                var index = 0;

                foreach (var source in group.Sources.Sources)
                {

                    if (index < group.ServerIds.Count &&
                        String.Equals(group.ServerIds[index], Client, StringComparison.OrdinalIgnoreCase))
                    {
                        Group = group;
                        Host  = source.Hostname.Trimmed;
                        return true;
                    }

                    index++;

                }

            }

            Group  = null;
            Host   = null;
            return false;

        }

        #endregion

        #region (private) UnknownGroupText(GroupId)

        private String UnknownGroupText(String GroupId)

            => node.NTSGroups.Count == 0
                   ? $"This {node.Kind.Name} checks its clock against no group of time servers."
                   : $"'{GroupId}' is none of this {node.Kind.Name}'s NTS groups, which are {String.Join(", ", node.NTSGroups.Select(other => other.Id))}.";

        #endregion

        #region (private) Tell2_1(NetworkTimeRequestId, Round = null, Test = null)

        /// <summary>
        /// Tell the CSMS the outcome of a round or a test it asked for, through
        /// the OCPP 2.1 node handed last.
        /// </summary>
        /// <remarks>
        /// Not queued: what a round found is kept in the group and in the log,
        /// and a test is cheap to repeat. Where the CSMS cannot be reached it
        /// is written into the log that it was not told.
        /// </remarks>
        private async Task Tell2_1(Int32              NetworkTimeRequestId,
                                   NetworkTimeRound?  Round   = null,
                                   TimeServerTest?    Test    = null)
        {

            OCPPv2_1.IDeviceModelNode? node2_1;

            lock (componentsLock)
            {
                node2_1 = deviceModelNode;
            }

            if (node2_1 is not OCPPv2_1.NetworkingNode.INetworkingNode ocppNode)
                return;

            var notification = new OCPPv2_1.CS.NotifyNetworkTimeResultRequest(
                                   Destination:           SourceRouting.CSMS,
                                   NetworkTimeRequestId:  NetworkTimeRequestId,
                                   GeneratedAt:           node.TimeProvider.GetUtcNow(),
                                   Round:                 Round,
                                   Test:                  Test,

                                   RequestId:             ocppNode.OCPP.NextRequestId,
                                   RequestTimestamp:      node.TimeProvider.GetUtcNow(),
                                   RequestTimeout:        ocppNode.OCPP.DefaultRequestTimeout,
                                   EventTrackingId:       EventTracking_Id.New,
                                   NetworkPath:           NetworkPath.Empty
                               );

            OnNetworkTimeResult?.Invoke(notification);

            var response = await ocppNode.OCPP.OUT.NotifyNetworkTimeResult(notification);

            if (response.Result.ResultCode != ResultCode.OK)
                node.Log.Warning($"The CSMS was not told the outcome of request {NetworkTimeRequestId}: {response.Result.Description ?? response.Result.ResultCode.ToString()}", "nts", "ocpp");

        }

        #endregion

        #region (private) Tell1_6(NetworkTimeRequestId, Round = null, Test = null)

        /// <summary>
        /// The same, through the OCPP 1.6 node handed last.
        /// </summary>
        private async Task Tell1_6(Int32              NetworkTimeRequestId,
                                   NetworkTimeRound?  Round   = null,
                                   TimeServerTest?    Test    = null)
        {

            OCPPv1_6.CP.IConfigurationKeysNode? node1_6;

            lock (componentsLock)
            {
                node1_6 = configurationKeysNode;
            }

            if (node1_6 is not OCPPv1_6.NetworkingNode.INetworkingNode ocppNode)
                return;

            // A central system of OCPP 1.6 is addressed as such, not as a CSMS.
            var notification = new OCPPv1_6.CP.NotifyNetworkTimeResultRequest(
                                   Destination:           SourceRouting.To(NetworkingNode_Id.CentralSystem),
                                   NetworkTimeRequestId:  NetworkTimeRequestId,
                                   GeneratedAt:           node.TimeProvider.GetUtcNow(),
                                   Round:                 Round,
                                   Test:                  Test,

                                   RequestId:             ocppNode.OCPP.NextRequestId,
                                   RequestTimestamp:      node.TimeProvider.GetUtcNow(),
                                   RequestTimeout:        ocppNode.OCPP.DefaultRequestTimeout,
                                   EventTrackingId:       EventTracking_Id.New,
                                   NetworkPath:           NetworkPath.Empty
                               );

            OnNetworkTimeResult16?.Invoke(notification);

            var response = await ocppNode.OCPP.OUT.NotifyNetworkTimeResult(notification);

            if (response.Result.ResultCode != ResultCode.OK)
                node.Log.Warning($"The central system was not told the outcome of request {NetworkTimeRequestId}: {response.Result.Description ?? response.Result.ResultCode.ToString()}", "nts", "ocpp");

        }

        #endregion


        #region (internal) RoundOf(Group, Result)

        /// <summary>
        /// What <see cref="WWCPNode.SyncTimeAsync(NTSGroup, CancellationToken)"/>
        /// came back with, in the words of NetworkTimeRoundType: seconds where it
        /// says milliseconds, client ids where it says host names.
        /// </summary>
        /// <param name="Group">The group that was asked.</param>
        /// <param name="Result">What the round came back with.</param>
        internal NetworkTimeRound RoundOf(NTSGroup  Group,
                                          JObject   Result)
        {

            var groupJSON  = Result["group"] as JObject;

            // The time is legal time or not after the round, which is what the
            // legal group's round was for.
            var clock      = Group.IsLegal ? node.ClockJSON() : null;
            var legal      = clock?.Value<Boolean?>("legal");
            var reason     = clock is not null ? NTSDeviceModel.ReasonOf(clock["why"]) : null;

            return new NetworkTimeRound(

                       Group:              Group.Id,
                       OK:                 Result.Value<Boolean>("ok"),
                       Duration:           Seconds(Result.Value<Double?>("runtime_ms")) ?? 0,
                       Answered:           groupJSON?.Value<Int32?>("answered") ?? 0,
                       Required:           groupJSON?.Value<Int32?>("required") ?? Group.Sources.MinServers,
                       Clients:            (Result["servers"] as JArray ?? []).OfType<JObject>().Select(server => new NetworkTimeClient(

                                               Host:            (server.Value<String>("hostname") ?? "").TrimEnd('.'),
                                               OK:              server.Value<Boolean>("ok"),
                                               Client:          server.Value<String>("id"),
                                               Offset:          Seconds(server.Value<Double?>("offset_ms")),
                                               RoundTrip:       Seconds(server.Value<Double?>("roundTrip_ms")),
                                               KeyExchange:     server.Value<String>("keyExchange") switch {
                                                                    "new"     => NetworkTimeClient.NewKeyExchange,
                                                                    "reused"  => NetworkTimeClient.ReusedKeyExchange,
                                                                    _         => null
                                                                },
                                               Preflight:       server.Value<Boolean?>("preflight"),
                                               SignedResponse:  (server["signed"] as JObject)?.Value<Boolean?>("verified"),
                                               Error:           server.Value<String>("error")

                                           )),

                       Offset:             Result.Value<Boolean>("ok") ? Seconds(groupJSON?.Value<Double?>("offset_ms")) : null,
                       Spread:             Seconds(groupJSON?.Value<Double?>("spread_ms")),
                       DeviationExceeded:  groupJSON?.Value<Boolean?>("deviationExceeded"),
                       LegalTime:          legal,
                       LegalTimeReason:    legal == false && reason?.Length > 0 ? reason : null,
                       Error:              Result.Value<Boolean>("ok") ? null : Result.Value<String>("error"),

                       // Said only where it says something: "Absent when it equals answered".
                       Agreeing:           groupJSON?.Value<Int32?>("agreeing") is Int32 agreeing &&
                                           agreeing != (groupJSON?.Value<Int32?>("answered") ?? 0)
                                               ? agreeing
                                               : null

                   );

        }

        #endregion

        #region (internal static) WithoutClientIds(Round)

        /// <summary>
        /// The outcome of a round as OCPP 1.6 tells it: its clients named by
        /// their host names alone.
        /// </summary>
        internal static NetworkTimeRound WithoutClientIds(NetworkTimeRound Round)

            => new (Round.Group,
                    Round.OK,
                    Round.Duration,
                    Round.Answered,
                    Round.Required,
                    Round.Clients.Select(client => new NetworkTimeClient(client.Host,
                                                                         client.OK,
                                                                         null,
                                                                         client.Offset,
                                                                         client.RoundTrip,
                                                                         client.KeyExchange,
                                                                         client.Preflight,
                                                                         client.SignedResponse,
                                                                         client.Error)),
                    Round.Offset,
                    Round.Spread,
                    Round.DeviationExceeded,
                    Round.LegalTime,
                    Round.LegalTimeReason,
                    Round.Error,
                    Round.Agreeing);

        #endregion

        #region (internal static) TestOf(Client, Result)

        /// <summary>
        /// What <see cref="WWCPNode.TestTimeServerAsync(NTSGroup, String, CancellationToken)"/>
        /// came back with, in the words of TimeServerTestType: seconds where it
        /// says milliseconds, and the levels of its steps capitalised.
        /// </summary>
        /// <param name="Client">The client that was tested, as the CSMS named it; none in OCPP 1.6.</param>
        /// <param name="Result">What the test came back with.</param>
        internal static TimeServerTest TestOf(String?  Client,
                                              JObject  Result)

            => new (

                   Host:       (Result.Value<String>("host") ?? "").TrimEnd('.'),
                   OK:         Result.Value<Boolean>("ok"),
                   Duration:   Seconds(Result.Value<Double?>("runtime_ms")) ?? 0,
                   Steps:      (Result["steps"] as JArray ?? []).OfType<JObject>().Select(step => new TimeServerTestStep(
                                   Elapsed:  Seconds(step.Value<Double?>("at_ms")) ?? 0,
                                   Stage:    step.Value<String>("stage") ?? TimeServerTestStages.Configuration,
                                   Level:    Capitalised(step.Value<String>("level") ?? "info"),
                                   Text:     step.Value<String>("text") ?? ""
                               )),
                   Client:     Client,
                   FailedAt:   Result.Value<String>("failedAt"),
                   Offset:     Seconds(Result.Value<Double?>("offset_ms")),
                   RoundTrip:  Seconds(Result.Value<Double?>("roundTrip_ms"))

               );

        #endregion


        #region (private static) Asker(Path, Signatures)

        /// <summary>
        /// Who asked, as the log names it: the node the request came from, and
        /// whoever signed it where it was signed - by the name of the signature,
        /// or its key id where it has none.
        /// </summary>
        /// <param name="Path">The network path of the request.</param>
        /// <param name="Signatures">The signatures of the request.</param>
        private static String Asker(NetworkPath             Path,
                                    IEnumerable<Signature>  Signatures)
        {

            var asker   = $"'{Path.Source}'";

            var signers = Signatures.Select(signature => signature.Name ?? (signature.KeyId.Length > 0 ? Convert.ToHexString(signature.KeyId) : null)).
                                     Where (signer    => !String.IsNullOrEmpty(signer)).
                                     ToArray();

            return signers.Length > 0
                       ? $"{asker} (signed by {String.Join(", ", signers)})"
                       : asker;

        }

        #endregion

        #region (private static) Seconds(Milliseconds)

        /// <summary>
        /// Milliseconds as decimal seconds, to the microsecond.
        /// </summary>
        private static Decimal? Seconds(Double? Milliseconds)

            => Milliseconds.HasValue
                   ? Math.Round(Milliseconds.Value.AsDecimal() / 1000m, 6)
                   : null;

        #endregion

        #region (private static) Capitalised(Word)

        private static String Capitalised(String Word)

            => Word.Length > 0
                   ? Char.ToUpperInvariant(Word[0]) + Word[1..]
                   : Word;

        #endregion

    }


    /// <summary>
    /// The reason codes of a SyncNetworkTime or TestTimeServer that asked
    /// nobody - section 3.2.7 of the white paper "Secure Time Synchronization".
    /// </summary>
    public static class NetworkTimeRefusals
    {

        /// <summary>
        /// The group is no instance of NTPClientGroupController.
        /// </summary>
        public const String UnknownGroup   = "UnknownGroup";

        /// <summary>
        /// The client is no instance of NTPClientController.
        /// </summary>
        public const String UnknownClient  = "UnknownClient";

        /// <summary>
        /// OCPP 1.6: the server is not listed in ntp.&lt;group&gt;.servers.
        /// </summary>
        public const String UnknownServer  = "UnknownServer";

        /// <summary>
        /// The group, or the group of the client, is switched off.
        /// </summary>
        public const String Disabled       = "Disabled";

        /// <summary>
        /// A round or a test asked for before is still running.
        /// </summary>
        public const String Busy           = "Busy";

    }

}
