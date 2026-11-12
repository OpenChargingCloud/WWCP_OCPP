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

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverCBOR
{

    // The tests of the messages a charging station and a CSMS send each other, once
    // more - with charging stations that offer "ocpp2.1cbor" alone. Nothing of what
    // a test asks of a message changes with how it crossed the wire.

    /// <summary>
    /// The messages of a charging station, over CBOR.
    /// </summary>
    [TestFixture]
    public class CS_Messages_OverCBOR_Tests : CS_Messages_Tests
    {

        protected override MessageEncodings Encoding
            => MessageEncodings.CBOR;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }

    /// <summary>
    /// The messages of a CSMS, over CBOR.
    /// </summary>
    [TestFixture]
    public class CSMS_Messages_OverCBOR_Tests : CSMS_Messages_Tests
    {

        protected override MessageEncodings Encoding
            => MessageEncodings.CBOR;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }

    /// <summary>
    /// The signed messages of a charging station, over CBOR: their signatures are
    /// those over the JSON of the message, which its CBOR carries along.
    /// </summary>
    [TestFixture]
    public class CS_SignedMessages_OverCBOR_Tests : CS_SignedMessages_Tests
    {

        protected override MessageEncodings Encoding
            => MessageEncodings.CBOR;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }

    /// <summary>
    /// The signed messages of a CSMS, over CBOR.
    /// </summary>
    [TestFixture]
    public class CSMS_SignedMessages_OverCBOR_Tests : CSMS_SignedMessages_Tests
    {

        protected override MessageEncodings Encoding
            => MessageEncodings.CBOR;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }


    internal static class OverCBOR
    {

        /// <summary>
        /// The connection of the given charging station agreed on "ocpp2.1cbor" - or
        /// the fixture ran its tests over JSON, and proved nothing about CBOR.
        /// </summary>
        internal static void AgreedOnCBOR(TestChargingStationNode? ChargingStation)
        {

            Assert.That(ChargingStation, Is.Not.Null);

            Assert.That(ChargingStation!.WWCPWebSocketClients.OfType<WWCPWebSocketClient>().Single().WebSocketClientConnection?.Subprotocol,
                        Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));

        }

    }

}
