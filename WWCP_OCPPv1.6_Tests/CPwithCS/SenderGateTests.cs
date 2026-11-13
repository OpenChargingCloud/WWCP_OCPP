/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Collections.Concurrent;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CPwithCS
{

    /// <summary>
    /// Who a request or a message that comes in may be from, in OCPP 1.6 as in
    /// OCPP 2.1: in the overlay network a frame names its network path itself,
    /// and the sender gate of a node says whether the connection it came over
    /// may send it in that name.
    /// </summary>
    [TestFixture]
    public class SenderGateTests : ACPwithCSTests
    {

        #region (private) Heartbeat(From, Mode, Destination, Path)

        private static Task<SendRequestState> Heartbeat(TestChargePointNode  From,
                                                        NetworkingMode       Mode,
                                                        SourceRouting        Destination,
                                                        NetworkPath          Path)

            => From.OCPP.OUT.SendJSONRequestAndWait(
                   new OCPP_JSONRequestMessage(
                       RequestTimestamp:  Timestamp.Now,
                       EventTrackingId:   EventTracking_Id.New,
                       NetworkingMode:    Mode,
                       Destination:       Destination,
                       NetworkPath:       Path,
                       RequestId:         Request_Id.NewRandom(),
                       Action:            "Heartbeat",
                       Payload:           new JObject(),
                       RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(2)
                   )
               );

        #endregion


        #region ASenderGateDropsEveryKindOfMessage_BeforeAnythingIsDoneWithIt()

        /// <summary>
        /// The sender gate of a node is asked of every request and every send message that comes in - JSON and
        /// binary - with the node the connection is known by and the network path the message names. What it
        /// refuses is not answered, not processed, and raises no event; a gate that fails drops as well. What
        /// it lets through goes on as before.
        /// </summary>
        [Test]
        public async Task ASenderGateDropsEveryKindOfMessage_BeforeAnythingIsDoneWithIt()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            var asked     = new ConcurrentQueue<String>();
            var received  = new ConcurrentQueue<String>();
            var fails     = false;
            var refuses   = true;

            // What the central system took in: nothing it was not to.
            centralSystem!.OCPP.IN.OnJSONRequestMessageReceived   += (timestamp, sender, connection, message, cancellationToken) => { received.Enqueue(message.Action); return Task.CompletedTask; };
            centralSystem. OCPP.IN.OnJSONSendMessageReceived      += (timestamp, sender, connection, message, cancellationToken) => { received.Enqueue(message.Action); return Task.CompletedTask; };
            centralSystem. OCPP.IN.OnBinaryRequestMessageReceived += (timestamp, sender, connection, message, cancellationToken) => { received.Enqueue(message.Action); return Task.CompletedTask; };
            centralSystem. OCPP.IN.OnBinarySendMessageReceived    += (timestamp, sender, connection, message, cancellationToken) => { received.Enqueue(message.Action); return Task.CompletedTask; };

            centralSystem.OCPP.IN.SenderGate = (connection, node, path, action) => {

                asked.Enqueue($"{node} {String.Join(",", path)} {action}");

                if (fails)
                    throw new InvalidOperationException("the gate failed");

                return refuses ? "Not in this name." : null;

            };

            try
            {

                var destination    = SourceRouting.To(NetworkingNode_Id.CentralSystem);
                var path           = NetworkPath.From(chargePoint!.Id);

                var jsonRequest    = await Heartbeat(chargePoint, NetworkingMode.Standard, destination, path);

                await chargePoint.OCPP.OUT.SendJSONSendMessage(
                          new OCPP_JSONSendMessage(
                              Timestamp.Now,
                              EventTracking_Id.New,
                              NetworkingMode.Standard,
                              destination,
                              path,
                              Request_Id.NewRandom(),
                              "DataTransfer",
                              new JObject(new JProperty("vendorId", "GraphDefined"))
                          )
                      );

                var binaryRequest  = await chargePoint.OCPP.OUT.SendBinaryRequestAndWait(
                                         new OCPP_BinaryRequestMessage(
                                             Timestamp.Now,
                                             EventTracking_Id.New,
                                             NetworkingMode.Standard,
                                             destination,
                                             path,
                                             Request_Id.NewRandom(),
                                             "BinaryDataTransfer",
                                             [ 1, 2, 3 ],
                                             Timestamp.Now + TimeSpan.FromSeconds(2)
                                         )
                                     );

                await chargePoint.OCPP.OUT.SendBinarySendMessage(
                          new OCPP_BinarySendMessage(
                              Timestamp.Now,
                              EventTracking_Id.New,
                              NetworkingMode.Standard,
                              destination,
                              path,
                              Request_Id.NewRandom(),
                              "BinaryDataTransfer",
                              [ 4, 5, 6 ]
                          )
                      );

                var until = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10);
                while (asked.Count < 4 && DateTimeOffset.UtcNow < until)
                    await Task.Delay(10);

                fails              = true;
                var whenItFails    = await Heartbeat(chargePoint, NetworkingMode.Standard, destination, path);

                fails              = false;
                refuses            = false;
                var letThrough     = await Heartbeat(chargePoint, NetworkingMode.Standard, destination, path);

                var name           = chargePoint.Id.ToString();

                Assert.Multiple(() => {
                    Assert.That(asked.Take(4),                    Is.EqualTo(new[] { $"{name} {name} Heartbeat",
                                                                                     $"{name} {name} DataTransfer",
                                                                                     $"{name} {name} BinaryDataTransfer",
                                                                                     $"{name} {name} BinaryDataTransfer" }));
                    Assert.That(jsonRequest.  JSONResponse,       Is.Null, "a JSON request the gate refused was answered");
                    Assert.That(jsonRequest.  JSONRequestErrorMessage?.ErrorCode, Is.EqualTo(ResultCode.Timeout), "a JSON request the gate refused was answered with an error, not left unanswered");
                    Assert.That(binaryRequest.BinaryResponse,     Is.Null, "a binary request the gate refused was answered");
                    Assert.That(whenItFails.  JSONResponse,       Is.Null, "a request was taken where the gate failed");
                    Assert.That(letThrough.   JSONResponse,       Is.Not.Null, "a request the gate let through was not answered");
                    Assert.That(received,                         Is.EqualTo(new[] { "Heartbeat" }), "the central system took in what the gate refused");
                });

            }
            finally
            {
                centralSystem.OCPP.IN.SenderGate = null;
            }

        }

        #endregion

        #region AChargePointThatNamesAnotherAsTheSender_IsSeenByTheGateAsItIs()

        /// <summary>
        /// A charge point that dials in the overlay network names its sender and its path itself in every
        /// frame. The gate is given the node the connection is known by next to the path the frame names,
        /// so a node can tell the two apart - here by the rule a CSMS has: the last hop is the connection's
        /// node, and a connection speaks for itself alone.
        /// </summary>
        [Test]
        public async Task AChargePointThatNamesAnotherAsTheSender_IsSeenByTheGateAsItIs()
        {

            Assert.That(centralSystem, Is.Not.Null);

            var other     = NetworkingNode_Id.Parse("CP-SOMEBODY-ELSE");
            var asked     = new ConcurrentQueue<String>();
            var received  = new ConcurrentQueue<String>();

            var dialler   = new TestChargePointNode(
                                ChargeBoxId:        NetworkingNode_Id.Parse("GD-OVERLAY"),
                                ChargePointVendor:  "GraphDefined OEM #1",
                                ChargePointModel:   "VCP.1",
                                Connectors:         [],
                                DNSClient:          centralSystem!.DNSClient
                            );

            var upgrade   = await dialler.ConnectOCPPWebSocketClient(
                                      URL.Parse($"http://127.0.0.1:{centralSystemWSS!.IPPort}/{dialler.Id}"),
                                      SecWebSocketProtocols:  Offered,
                                      NetworkingMode:         NetworkingMode.OverlayNetwork
                                  );

            Assert.That(upgrade?.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            centralSystem.OCPP.IN.OnJSONRequestMessageReceived += (timestamp, sender, connection, message, cancellationToken) => {
                received.Enqueue($"{message.NetworkPath.Source} {message.Action}");
                return Task.CompletedTask;
            };

            centralSystem.OCPP.IN.SenderGate = (connection, node, path, action) => {

                asked.Enqueue($"{node} {String.Join(",", path)} {action}");

                return path.Length > 0 && path.Last == node && path.Source == node
                           ? null
                           : "A connection speaks for itself alone.";

            };

            try
            {

                var toTheCentralSystem  = SourceRouting.To(NetworkingNode_Id.CentralSystem);
                var name                = dialler.Id.ToString();

                // In the name of another charge point, as if this one had forwarded it...
                var forwarded  = await Heartbeat(dialler, NetworkingMode.OverlayNetwork, toTheCentralSystem, NetworkPath.From(other).Append(dialler.Id));

                // ...in that name alone...
                var foreign    = await Heartbeat(dialler, NetworkingMode.OverlayNetwork, toTheCentralSystem, NetworkPath.From(other));

                // ...and in its own.
                var own        = await Heartbeat(dialler, NetworkingMode.OverlayNetwork, toTheCentralSystem, NetworkPath.From(dialler.Id));

                Assert.Multiple(() => {
                    Assert.That(asked,                    Is.EqualTo(new[] { $"{name} {other},{name} Heartbeat",
                                                                             $"{name} {other} Heartbeat",
                                                                             $"{name} {name} Heartbeat" }));
                    Assert.That(forwarded.JSONResponse,   Is.Null,     "a request in another charge point's name was answered");
                    Assert.That(foreign.  JSONResponse,   Is.Null,     "a request whose path does not end in the connection's node was answered");
                    Assert.That(own.      JSONResponse,   Is.Not.Null, "a request in the charge point's own name was not answered");
                    Assert.That(received,                 Is.EqualTo(new[] { $"{name} Heartbeat" }), "the central system took in what was sent in another name");
                });

            }
            finally
            {
                centralSystem.OCPP.IN.SenderGate = null;
            }

        }

        #endregion

    }

}
