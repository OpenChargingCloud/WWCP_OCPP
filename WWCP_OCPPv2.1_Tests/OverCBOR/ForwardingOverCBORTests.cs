/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.tests.LocalController;
using cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.OverlayNetwork.Default;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverCBOR
{

    // A local controller between a charging station and a CSMS translates: what
    // arrives over a connection that agreed on "ocpp2.1cbor" leaves over one that
    // agreed on "ocpp2.1" as JSON, and the other way round. The tests of the overlay
    // network run again in the three ways its two connections can differ from JSON
    // on both sides - nothing of what a test asks of a message changes with how it
    // crossed either wire.

    #region UnknownActionsThroughTheLocalController

    /// <summary>
    /// A request nobody has a class for - a message of a vendor, or of a version the
    /// local controller does not know - is forwarded as it is. Here over JSON on both
    /// sides; below, where one side or both speak CBOR, it is carried blind: as what
    /// its JSON says, value by value.
    /// </summary>
    /// <remarks>
    /// The CSMS refuses it, as it refuses every action it does not know. That refusal
    /// does not come back to the charging station through the overlay network - over
    /// JSON on both sides as little as over CBOR - so the test does not ask for it.
    /// </remarks>
    [TestFixture]
    public class UnknownActionsThroughTheLocalController : ADefaultOverlayNetwork
    {

        [Test]
        public async Task ARequestNobodyKnows_ReachesTheCSMSAsItWas()
        {

            var seen     = new ConcurrentQueue<JObject>();

            OCPPv2_1.NetworkingNode.OnJSONRequestMessageReceivedDelegate told = (timestamp, sender, connection, request, cancellationToken) => {
                if (request.Action == "NobodyKnowsThis")
                    seen.Enqueue(request.Payload);
                return Task.CompletedTask;
            };

            CSMS!.OCPP.IN.OnJSONRequestMessageReceived += told;

            try
            {

                var payload  = new JObject(
                                   new JProperty("text",    "hello"),
                                   new JProperty("number",  23),
                                   new JProperty("when",    "2026-11-12T10:00:00.000Z"),
                                   new JProperty("list",    new JArray(1, "two", new JObject(new JProperty("three", true))))
                               );

                var state    = await chargingStation!.OCPP.OUT.SendJSONRequestAndWait(
                                         new OCPP_JSONRequestMessage(
                                             RequestTimestamp:  Timestamp.Now,
                                             EventTrackingId:   EventTracking_Id.New,
                                             NetworkingMode:    NetworkingMode.Standard,
                                             Destination:       SourceRouting.CSMS,
                                             NetworkPath:       NetworkPath.From(chargingStation.Id),
                                             RequestId:         Request_Id.NewRandom(),
                                             Action:            "NobodyKnowsThis",
                                             Payload:           payload,
                                             RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(2)
                                         )
                                     );

                Assert.Multiple(() => {

                    Assert.That(state,                                    Is.Not.Null);

                    Assert.That(seen,                                     Has.Count.EqualTo(1), "The request did not reach the CSMS.");

                    if (seen.TryPeek(out var arrived))
                        Assert.That(JToken.DeepEquals(arrived, payload),  Is.True, arrived.ToString(Newtonsoft.Json.Formatting.None));

                });

            }
            finally
            {
                CSMS!.OCPP.IN.OnJSONRequestMessageReceived -= told;
            }

        }

    }

    #endregion


    #region CBOR below the local controller, JSON above it

    /// <summary>
    /// What a charging station asks the CSMS, through the local controller: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class CS_2_CSMS_CBORDown_Tests : CS_2_CSMS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// A request for the CSMS is delivered there, not forwarded on: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class CS_2_CSMS_Delivery_CBORDown_Tests : CS_2_CSMS_Delivery
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// What the CSMS asks a charging station, through the local controller: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class CSMS_2_CS_CBORDown_Tests : CSMS_2_CS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// What a charging station asks the local controller: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class CS_2_LC_CBORDown_Tests : CS_2_LC_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// What the local controller asks a charging station: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class LC_2_CS_CBORDown_Tests : LC_2_CS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// What the local controller asks the CSMS: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class LC_2_CSMS_CBORDown_Tests : LC_2_CSMS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// What the CSMS asks the local controller: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class CSMS_2_LC_CBORDown_Tests : CSMS_2_LC_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// A request of the CSMS to the local controller, and its way back: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class CSMS_2_LC_RoundTrip_CBORDown_Tests : CSMS_2_LC_RoundTrip
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// A request nobody has a class for, through the local controller: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class UnknownActionsThroughTheLocalController_CBORDown_Tests : UnknownActionsThroughTheLocalController
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    /// <summary>
    /// A local controller in standard networking mode forwards its stations' requests up: CBOR below the local controller, JSON above it.
    /// </summary>
    [TestFixture]
    public class ForwardingUpInStandardModeTests_CBORDown_Tests : ForwardingUpInStandardModeTests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.JSON;
    }

    #endregion

    #region JSON below the local controller, CBOR above it

    /// <summary>
    /// What a charging station asks the CSMS, through the local controller: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class CS_2_CSMS_CBORUp_Tests : CS_2_CSMS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A request for the CSMS is delivered there, not forwarded on: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class CS_2_CSMS_Delivery_CBORUp_Tests : CS_2_CSMS_Delivery
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the CSMS asks a charging station, through the local controller: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class CSMS_2_CS_CBORUp_Tests : CSMS_2_CS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What a charging station asks the local controller: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class CS_2_LC_CBORUp_Tests : CS_2_LC_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the local controller asks a charging station: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class LC_2_CS_CBORUp_Tests : LC_2_CS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the local controller asks the CSMS: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class LC_2_CSMS_CBORUp_Tests : LC_2_CSMS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the CSMS asks the local controller: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class CSMS_2_LC_CBORUp_Tests : CSMS_2_LC_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A request of the CSMS to the local controller, and its way back: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class CSMS_2_LC_RoundTrip_CBORUp_Tests : CSMS_2_LC_RoundTrip
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A request nobody has a class for, through the local controller: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class UnknownActionsThroughTheLocalController_CBORUp_Tests : UnknownActionsThroughTheLocalController
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A local controller in standard networking mode forwards its stations' requests up: JSON below the local controller, CBOR above it.
    /// </summary>
    [TestFixture]
    public class ForwardingUpInStandardModeTests_CBORUp_Tests : ForwardingUpInStandardModeTests
    {
        protected override MessageEncodings Downlink => MessageEncodings.JSON;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    #endregion

    #region CBOR on both sides of the local controller

    /// <summary>
    /// What a charging station asks the CSMS, through the local controller: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class CS_2_CSMS_CBORBoth_Tests : CS_2_CSMS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A request for the CSMS is delivered there, not forwarded on: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class CS_2_CSMS_Delivery_CBORBoth_Tests : CS_2_CSMS_Delivery
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the CSMS asks a charging station, through the local controller: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class CSMS_2_CS_CBORBoth_Tests : CSMS_2_CS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What a charging station asks the local controller: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class CS_2_LC_CBORBoth_Tests : CS_2_LC_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the local controller asks a charging station: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class LC_2_CS_CBORBoth_Tests : LC_2_CS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the local controller asks the CSMS: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class LC_2_CSMS_CBORBoth_Tests : LC_2_CSMS_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// What the CSMS asks the local controller: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class CSMS_2_LC_CBORBoth_Tests : CSMS_2_LC_Tests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A request of the CSMS to the local controller, and its way back: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class CSMS_2_LC_RoundTrip_CBORBoth_Tests : CSMS_2_LC_RoundTrip
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A request nobody has a class for, through the local controller: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class UnknownActionsThroughTheLocalController_CBORBoth_Tests : UnknownActionsThroughTheLocalController
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    /// <summary>
    /// A local controller in standard networking mode forwards its stations' requests up: CBOR on both sides of the local controller.
    /// </summary>
    [TestFixture]
    public class ForwardingUpInStandardModeTests_CBORBoth_Tests : ForwardingUpInStandardModeTests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    #endregion

}
