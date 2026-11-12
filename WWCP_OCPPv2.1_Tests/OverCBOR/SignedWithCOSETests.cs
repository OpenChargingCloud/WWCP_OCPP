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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverCBOR
{

    // The tests of signed messages, once more - with nodes that write a COSE_Sign1
    // over the CBOR of a message instead of the JWS over its JSON. Such a signature
    // signs the same bytes wherever the message is read, so the tests run over JSON
    // connections, where it travels as its bytes in base64url, and over CBOR
    // connections, where it is the COSE_Sign1 it is.

    #region COSE signatures over JSON connections

    /// <summary>
    /// The signed messages of a charging station, signed with COSE, over JSON.
    /// </summary>
    [TestFixture]
    public class CS_SignedMessages_COSE_Tests : CS_SignedMessages_Tests
    {
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;
    }

    /// <summary>
    /// The signed messages of a CSMS, signed with COSE, over JSON.
    /// </summary>
    [TestFixture]
    public class CSMS_SignedMessages_COSE_Tests : CSMS_SignedMessages_Tests
    {
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;
    }

    /// <summary>
    /// A charging station with a signature policy, signing with COSE, over JSON.
    /// </summary>
    [TestFixture]
    public class CS_WithSignaturePolicy_COSE_Tests : CS_WithSignaturePolicy_Tests
    {
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;
    }

    /// <summary>
    /// A CSMS with a signature policy, signing with COSE, over JSON.
    /// </summary>
    [TestFixture]
    public class CSMS_WithSignaturePolicy_COSE_Tests : CSMS_WithSignaturePolicy_Tests
    {
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;
    }

    #endregion

    #region COSE signatures over CBOR connections

    /// <summary>
    /// The signed messages of a charging station, signed with COSE, over CBOR.
    /// </summary>
    [TestFixture]
    public class CS_SignedMessages_COSE_OverCBOR_Tests : CS_SignedMessages_Tests
    {

        protected override MessageEncodings Encoding         => MessageEncodings.CBOR;
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }

    /// <summary>
    /// The signed messages of a CSMS, signed with COSE, over CBOR.
    /// </summary>
    [TestFixture]
    public class CSMS_SignedMessages_COSE_OverCBOR_Tests : CSMS_SignedMessages_Tests
    {

        protected override MessageEncodings Encoding         => MessageEncodings.CBOR;
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }

    /// <summary>
    /// A charging station with a signature policy, signing with COSE, over CBOR.
    /// </summary>
    [TestFixture]
    public class CS_WithSignaturePolicy_COSE_OverCBOR_Tests : CS_WithSignaturePolicy_Tests
    {

        protected override MessageEncodings Encoding         => MessageEncodings.CBOR;
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }

    /// <summary>
    /// A CSMS with a signature policy, signing with COSE, over CBOR.
    /// </summary>
    [TestFixture]
    public class CSMS_WithSignaturePolicy_COSE_OverCBOR_Tests : CSMS_WithSignaturePolicy_Tests
    {

        protected override MessageEncodings Encoding         => MessageEncodings.CBOR;
        protected override SignatureFormats SignatureFormats => SignatureFormats.COSE;

        [Test]
        public void TheChargingStationSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargingStation1);

    }

    #endregion

}
