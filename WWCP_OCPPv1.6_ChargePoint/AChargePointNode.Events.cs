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

        private readonly SemaphoreSlim  flushing        = new (1, 1);
        private          DateTimeOffset nextQueueTry;

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
        public TimeSpan      OfflineQueueRetry     { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Whether what the queue holds waits rather than goes: queued all the
        /// same, and sent by <see cref="FlushOfflineQueueAsync"/> once this is
        /// false again.
        /// </summary>
        /// <remarks>
        /// For a charge point whose central system has not accepted its
        /// BootNotification yet, and which may tell it nothing else until then.
        /// </remarks>
        public Boolean       HoldOfflineQueue      { get; set; }

        #endregion


        #region QueueSecurityEventAsync(Type, TechInfo = null, Timestamp = null)

        /// <summary>
        /// Send a security event, kept until the central system took it.
        /// </summary>
        /// <param name="Type">What happened.</param>
        /// <param name="TechInfo">What a technician needs to know of it: cut at <see cref="MaxTechInfoLength"/> characters.</param>
        /// <param name="Timestamp">When it happened; now where null.</param>
        public Task QueueSecurityEventAsync(SecurityEvent    Type,
                                            String?          TechInfo    = null,
                                            DateTimeOffset?  Timestamp   = null)
        {

            if (Type.Text.Length > MaxSecurityEventTypeLength)
                throw new ArgumentException($"The type of a security event has at most {MaxSecurityEventTypeLength} characters.", nameof(Type));

            var payload = new JObject(
                              new JProperty("type",       Type.Text),
                              new JProperty("timestamp",  (Timestamp ?? Now).ToString("o"))
                          );

            if (TechInfo is not null)
                payload["techInfo"] = TechInfo.Length > MaxTechInfoLength ? TechInfo[..MaxTechInfoLength] : TechInfo;

            OfflineQueue.Enqueue(securityEventKind, payload, Evidence: true);

            return FlushOfflineQueueAsync();

        }

        #endregion

        #region FlushOfflineQueueAsync()

        /// <summary>
        /// Send what the queue holds, oldest first, until the central system
        /// does not take a message - which is tried again after
        /// <see cref="OfflineQueueRetry"/>. Answers how many were taken.
        /// </summary>
        public async Task<Int32> FlushOfflineQueueAsync()
        {

            // Held: nothing goes, and nothing was refused either, so the queue
            // is not put off until its next try.
            if (HoldOfflineQueue)
                return 0;

            await flushing.WaitAsync();

            try
            {

                var taken = 0;

                while (OfflineQueue.Pending.FirstOrDefault() is QueuedMessage message)
                {

                    if (!await TrySendAsync(message))
                    {
                        nextQueueTry = Now + OfflineQueueRetry;
                        break;
                    }

                    OfflineQueue.Remove(message);
                    taken++;

                }

                return taken;

            }
            finally
            {
                flushing.Release();
            }

        }

        #endregion


        #region (private) RetryOfflineQueue()

        /// <summary>
        /// Try the queue again, at the node's maintenance, where something waits
        /// in it and its time to be tried again has come.
        /// </summary>
        private void RetryOfflineQueue()
        {

            if (HoldOfflineQueue || Now < nextQueueTry || OfflineQueue.Pending.Count == 0 || flushing.CurrentCount == 0)
                return;

            nextQueueTry = Now + OfflineQueueRetry;

            _ = Task.Run(async () => {
                    try
                    {
                        await FlushOfflineQueueAsync();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, $"Charge point '{Id}': the offline queue");
                    }
                });

        }

        #endregion

        #region (private) TrySendAsync(Message)

        /// <summary>
        /// Send one message of the queue, answering whether the central system
        /// took it. A message of a kind nobody sends any more counts as taken.
        /// </summary>
        private async Task<Boolean> TrySendAsync(QueuedMessage Message)
        {

            if (Message.Kind != securityEventKind)
                return true;

            try
            {

                var response = await this.SendSecurityEventNotification(
                                         SecurityEvent.Parse(Message.Payload.Value<String>("type") ?? ""),
                                         DateTimeOffset.Parse(Message.Payload.Value<String>("timestamp") ?? "", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                                         Message.Payload.Value<String>("techInfo")
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

    }

}
