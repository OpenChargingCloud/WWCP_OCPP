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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using QueuedMessage = cloud.charging.open.protocols.OCPP.QueuedMessage;
using OfflineQueue  = cloud.charging.open.protocols.OCPP.OfflineQueue;
using ResultCode    = cloud.charging.open.protocols.OCPP.ResultCode;
using Outboxes      = cloud.charging.open.protocols.OCPP.Outboxes;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The security events a charge point tells its central system, kept in a
    /// queue until the central system took them (OCPP 1.6 Security
    /// Extensions) - as the OCPP 2.1 charging station keeps them.
    /// </summary>
    public abstract partial class AChargePointNode
    {

        #region Data

        /// <summary>
        /// The longest techInfo of a security event: a longer one is cut there.
        /// </summary>
        public const Int32 MaxTechInfoLength = 255;

        /// <summary>
        /// The longest type of a security event.
        /// </summary>
        public const Int32 MaxSecurityEventTypeLength = 50;

        private const String securityEventKind = "SecurityEventNotification";

        /// <summary>
        /// The requests a violation of the signature policy was told of: one
        /// security event per request, however many of its entries were refused.
        /// </summary>
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<ISignableMessage, Object> violationsTold = [];

        /// <summary>
        /// The security event that tells a back end how many messages waiting for it were let go.
        /// </summary>
        public  const String QueueOverflow     = "QueueOverflow";

        private          Outboxes?      queues;

        #endregion

        #region Properties

        /// <summary>
        /// The security events not yet taken by the central system: in memory,
        /// unless a queue kept in a file is given.
        /// </summary>
        public OfflineQueue  OfflineQueue          { get; set; } = new();

        /// <summary>
        /// How long to wait before the queue is tried again after the central
        /// system did not take a message.
        /// </summary>
        public TimeSpan      OfflineQueueRetry
        {
            get => Queues.Retry;
            set => Queues.Retry = value;
        }

        /// <summary>
        /// Whether what the queue holds waits rather than goes: queued all the
        /// same, and sent by <see cref="FlushOfflineQueueAsync"/> once this is
        /// false again.
        /// </summary>
        /// <remarks>
        /// For a charge point whose central system has not accepted its
        /// BootNotification yet, and which may tell it nothing else until then.
        /// </remarks>
        public Boolean       HoldOfflineQueue
        {
            get => Queues.IsHeld(Queues.CSMS);
            set => Queues.Hold  (Queues.CSMS, value);
        }

        #endregion


        #region (private) Queues

        /// <summary>
        /// A queue for each destination this node tells things to: the one of its
        /// central system is <see cref="OfflineQueue"/>.
        /// </summary>
        private Outboxes Queues

            => LazyInitializer.EnsureInitialized(
                   ref queues,
                   () => new Outboxes(
                             DefaultNextHopNetworkingNodeId,
                             () => OfflineQueue,
                             TrySendAsync,
                             TellOfOverflowAsync,
                             Clock
                         )
               );

        #endregion

        #region OfflineQueueFor(Destination) / HoldOfflineQueueFor(Destination, Held)

        /// <summary>
        /// The queue of what waits for the given destination: <see cref="OfflineQueue"/>
        /// for the central system, and one of its own for any other back end - a successor,
        /// during a migration - bounded by <see cref="Outboxes.OtherMaxMessages"/>
        /// and <see cref="Outboxes.OtherMaxAge"/>.
        /// </summary>
        /// <param name="Destination">Where its messages go.</param>
        public OfflineQueue OfflineQueueFor(NetworkingNode_Id Destination)
            => Queues.QueueFor(Destination);

        /// <summary>
        /// Let what is queued for the given destination wait, or go again: for a
        /// back end that has not accepted this station yet. For the central system, as
        /// <see cref="HoldOfflineQueue"/>.
        /// </summary>
        /// <param name="Destination">Where its messages go.</param>
        /// <param name="Held">Whether they wait.</param>
        public void HoldOfflineQueueFor(NetworkingNode_Id  Destination,
                                        Boolean            Held)

            => Queues.Hold(Destination, Held);

        #endregion

        #region (private) TellOfPolicyViolation(Violation)

        /// <summary>
        /// Tell the central system of a request that was not signed as the signature
        /// policy wants it, refused or executed all the same: a security event
        /// of the kind of the violation, whose techInfo names the action and
        /// the reason (white paper "Signed OCPP Messages and Signer Roles",
        /// section 5.4). A request is told of once.
        /// </summary>
        private void TellOfPolicyViolation(PolicyViolation Violation)
        {

            // What a connection may not carry is no fault of a signature, and section 5.4
            // names no security event for it: whoever asked is told with the SecurityError.
            if (Violation.Kind == PolicyViolationKinds.ConnectionNotAuthorized)
                return;

            if (!violationsTold.TryAdd(Violation.Message, Violation))
                return;

            var type = Violation.Kind switch {
                           PolicyViolationKinds.SignatureMissing     => SecurityEvent.MessageSignatureMissing,
                           PolicyViolationKinds.SignatureInvalid     => SecurityEvent.MessageSignatureInvalid,
                           PolicyViolationKinds.SignerUnknown        => SecurityEvent.MessageSignerUnknown,
                           PolicyViolationKinds.Replayed             => SecurityEvent.MessageReplayed,
                           PolicyViolationKinds.SignatureExpired     => SecurityEvent.MessageSignatureExpired,
                           _                                         => SecurityEvent.MessageSignerNotAuthorized
                       };

            try
            {

                _ = QueueSecurityEventAsync(
                        type,
                        $"{Violation.Action}{(Violation.Refused ? "" : ", executed all the same")}: {Violation.Reason}"
                    ).ContinueWith(
                        task => DebugX.LogException(task.Exception!, $"Charge point '{Id}': telling of a '{type}'"),
                        TaskContinuationOptions.OnlyOnFaulted
                    );

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Charge point '{Id}': telling of a '{type}'");
            }

        }

        #endregion

        #region (private) TellOfRoleTableChange(Change)

        /// <summary>
        /// Tell the central system that the signer roles or the signature
        /// policy of this charge point were changed, over the network or at
        /// the charge point itself: the security event "RoleTableChanged",
        /// whose techInfo says what was changed, of which role, and to which
        /// version (white paper "Signed OCPP Messages and Signer Roles",
        /// section 5.4).
        /// </summary>
        private void TellOfRoleTableChange(SignerRoleTableChange Change)
        {

            var techInfo = Change.Change switch {
                               SignerRoleTableChanges.RoleAdded            => $"The role '{Change.Role}' got its keys, version {Change.Version}",
                               SignerRoleTableChanges.RoleUpdated          => $"The role '{Change.Role}' got other keys, version {Change.Version}",
                               SignerRoleTableChanges.RoleDeleted          => $"The role '{Change.Role}' lost its keys, version {Change.Version}",
                               SignerRoleTableChanges.FirstOwnerInstalled  => $"The first owner: the role '{Change.Role}' got its keys, version {Change.Version}",
                               SignerRoleTableChanges.TableReset           => "The keys of every role and the signature policy were forgotten",
                               SignerRoleTableChanges.PolicySet            => $"The signature policy was set, version {Change.Version}",
                               _                                           => Change.Change.ToString()
                           } + (Change.Locally ? ", at the charge point itself" : "");

            try
            {

                _ = QueueSecurityEventAsync(
                        SecurityEvent.RoleTableChanged,
                        techInfo
                    ).ContinueWith(
                        task => DebugX.LogException(task.Exception!, $"Charge point '{Id}': telling of a '{SecurityEvent.RoleTableChanged}'"),
                        TaskContinuationOptions.OnlyOnFaulted
                    );

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Charge point '{Id}': telling of a '{SecurityEvent.RoleTableChanged}'");
            }

        }

        #endregion

        #region QueueSecurityEventAsync(Type, TechInfo = null, Timestamp = null, Destination = null)

        /// <summary>
        /// Send a security event, kept until the central system took it.
        /// </summary>
        /// <param name="Type">What happened.</param>
        /// <param name="TechInfo">What a technician needs to know of it: cut at <see cref="MaxTechInfoLength"/> characters.</param>
        /// <param name="Timestamp">When it happened; now where null.</param>
        /// <param name="Destination">Whom to tell; the central system where null.</param>
        public Task QueueSecurityEventAsync(SecurityEvent      Type,
                                            String?            TechInfo      = null,
                                            DateTimeOffset?    Timestamp     = null,
                                            NetworkingNode_Id? Destination   = null)
        {

            if (Type.Text.Length > MaxSecurityEventTypeLength)
                throw new ArgumentException($"The type of a security event has at most {MaxSecurityEventTypeLength} characters.", nameof(Type));

            var payload = new JObject(
                              new JProperty("type",       Type.Text),
                              new JProperty("timestamp",  (Timestamp ?? Now).ToString("o"))
                          );

            if (TechInfo is not null)
                payload["techInfo"] = TechInfo.Length > MaxTechInfoLength ? TechInfo[..MaxTechInfoLength] : TechInfo;

            Queues.QueueFor(Destination ?? Queues.CSMS).Enqueue(securityEventKind, payload, Evidence: true);

            return FlushOfflineQueueAsync(Destination);

        }

        #endregion

        #region FlushOfflineQueueAsync(Destination = null)

        /// <summary>
        /// Send what waits for the given destination - the central system where null -
        /// oldest first, until it does not take a message, which is tried again
        /// after <see cref="OfflineQueueRetry"/>. A back end that lost messages of
        /// its queue for room or for age is told how many first. Answers how many
        /// were taken.
        /// </summary>
        /// <param name="Destination">Whom to tell; the central system where null.</param>
        public Task<Int32> FlushOfflineQueueAsync(NetworkingNode_Id? Destination = null)

            => Queues.FlushAsync(Destination ?? Queues.CSMS);

        #endregion


        #region (private) RetryOfflineQueue()

        /// <summary>
        /// Try the queues again, at the node's maintenance, where something waits
        /// in one and its time to be tried again has come.
        /// </summary>
        private void RetryOfflineQueue()
        {

            foreach (var destination in Queues.DueForRetry())
                _ = Task.Run(async () => {
                        try
                        {
                            await FlushOfflineQueueAsync(destination);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogException(e, $"Charge point '{Id}': the offline queue of '{destination}'");
                        }
                    });

        }

        #endregion

        #region (private) TrySendAsync(Message)

        /// <summary>
        /// Send one message of the queue, answering whether the central system
        /// took it. A message of a kind nobody sends any more counts as taken.
        /// </summary>
        private async Task<Boolean> TrySendAsync(QueuedMessage      Message,
                                                 NetworkingNode_Id  Destination)
        {

            if (Message.Kind != securityEventKind)
                return true;

            try
            {

                var response = await this.SendSecurityEventNotification(
                                         SecurityEvent.Parse(Message.Payload.Value<String>("type") ?? ""),
                                         DateTimeOffset.Parse(Message.Payload.Value<String>("timestamp") ?? "", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                                         Message.Payload.Value<String>("techInfo"),
                                         Destination: SourceRouting.To(Destination)
                                     );

                return response.Result.ResultCode == ResultCode.OK;

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Charge point '{Id}': sending a queued {Message.Kind}");
                return false;
            }

        }

        #endregion

        #region (private) TellOfOverflowAsync(Dropped, Destination)

        /// <summary>
        /// Tell the given destination how many messages of its queue were let go,
        /// for room or for age, with a security event "QueueOverflow" - answering
        /// whether it took it.
        /// </summary>
        private async Task<Boolean> TellOfOverflowAsync(Int64              Dropped,
                                                        NetworkingNode_Id  Destination)
        {

            try
            {

                var response = await this.SendSecurityEventNotification(
                                         SecurityEvent.Parse(QueueOverflow),
                                         Now,
                                         $"{Dropped} message(s) waiting for this back end were let go, for room or for age, while it could not be reached.",
                                         Destination: SourceRouting.To(Destination)
                                     );

                return response.Result.ResultCode == ResultCode.OK;

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Charge point '{Id}': telling '{Destination}' of messages let go");
                return false;
            }

        }

        #endregion

    }

}
