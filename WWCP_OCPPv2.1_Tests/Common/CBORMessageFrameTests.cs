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

using Newtonsoft.Json.Linq;
using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.Common
{

    /// <summary>
    /// An OCPP message in CBOR is the array it is in JSON, item by item, with a CBOR
    /// map in its last place: written to bytes and read back it is the same message,
    /// in the standard and in the overlay networking mode, and what stands in front
    /// of the map is what the JSON message of the same kind has there.
    /// </summary>
    [TestFixture]
    public class CBORMessageFrameTests
    {

        #region Data

        private static readonly Request_Id         requestId    = Request_Id.Parse("4711");
        private static readonly NetworkingNode_Id  station      = NetworkingNode_Id.Parse("GD-CBOR-001");
        private static readonly NetworkingNode_Id  controller   = NetworkingNode_Id.Parse("GD-LC-001");

        private static readonly CBORValue          payload      = CBORValue.FromMap([
                                                                      new KeyValuePair<CBORValue, CBORValue>(CBORValue.FromText("reason"),   CBORValue.FromText("PowerUp")),
                                                                      new KeyValuePair<CBORValue, CBORValue>(CBORValue.FromText("interval"), CBORValue.FromUInt64(300))
                                                                  ]);

        private static readonly JObject            details      = new (
                                                                      new JProperty("why",   "Because!"),
                                                                      new JProperty("count", 3)
                                                                  );

        public static IEnumerable<NetworkingMode> Modes
            => [ NetworkingMode.Standard, NetworkingMode.OverlayNetwork ];

        #endregion

        #region (private static) HeaderOf(CBORArray) / HeaderOf(JSONArray)

        /// <summary>
        /// What stands in front of the last item, as JSON text.
        /// </summary>
        private static String HeaderOf(CBORValue CBORArray)
        {

            var items = CBORArray.AsArray();

            return new JArray(items.Take(items.Count - 1).Select(item => CBORJSON.ToJSON(item))).ToString(Newtonsoft.Json.Formatting.None);

        }

        private static String HeaderOf(JArray JSONArray)

            => new JArray(JSONArray.Take(JSONArray.Count - 1)).ToString(Newtonsoft.Json.Formatting.None);

        #endregion


        #region ARequest_IsReadBackAsItWasWritten(Mode)

        [TestCaseSource(nameof(Modes))]
        public void ARequest_IsReadBackAsItWasWritten(NetworkingMode Mode)
        {

            var written = new OCPP_CBORRequestMessage(
                              Timestamp.Now,
                              EventTracking_Id.New,
                              Mode,
                              SourceRouting.To(NetworkingNode_Id.CSMS),
                              NetworkPath.From(station),
                              requestId,
                              "BootNotification",
                              payload
                          );

            var bytes = written.ToByteArray();

            Assert.That(OCPP_CBORFrame.TryParse(bytes, out var cborArray, out var frameError), Is.True, frameError);
            Assert.That(OCPP_CBORRequestMessage.TryParse(cborArray, out var read, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {

                Assert.That(cborArray.AsArray(),          Has.Count.EqualTo(Mode == NetworkingMode.Standard ? 4 : 6));
                Assert.That(cborArray.AsArray()[0],       Is.EqualTo(CBORValue.FromUInt64(2)));

                Assert.That(read!.NetworkingMode,         Is.EqualTo(Mode));
                Assert.That(read.RequestId,               Is.EqualTo(requestId));
                Assert.That(read.Action,                  Is.EqualTo("BootNotification"));
                Assert.That(read.Payload,                 Is.EqualTo(payload));

                if (Mode == NetworkingMode.OverlayNetwork)
                {
                    Assert.That(read.Destination.Next,    Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(read.NetworkPath.Source,  Is.EqualTo(station));
                }

                // The frame is the frame of the JSON message.
                Assert.That(HeaderOf(cborArray),
                            Is.EqualTo(HeaderOf(new OCPP_JSONRequestMessage(
                                                    written.RequestTimestamp,
                                                    written.EventTrackingId,
                                                    Mode,
                                                    written.Destination,
                                                    written.NetworkPath,
                                                    requestId,
                                                    "BootNotification",
                                                    []
                                                ).ToJSON())));

            });

        }

        #endregion

        #region AResponse_IsReadBackAsItWasWritten(Mode)

        [TestCaseSource(nameof(Modes))]
        public void AResponse_IsReadBackAsItWasWritten(NetworkingMode Mode)
        {

            var written = new OCPP_CBORResponseMessage(
                              Timestamp.Now,
                              EventTracking_Id.New,
                              Mode,
                              SourceRouting.To(station),
                              NetworkPath.From(NetworkingNode_Id.CSMS),
                              requestId,
                              payload
                          );

            Assert.That(OCPP_CBORFrame.TryParse(written.ToByteArray(), out var cborArray, out var frameError), Is.True, frameError);
            Assert.That(OCPP_CBORResponseMessage.TryParse(cborArray, out var read, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {

                Assert.That(cborArray.AsArray(),          Has.Count.EqualTo(Mode == NetworkingMode.Standard ? 3 : 5));
                Assert.That(cborArray.AsArray()[0],       Is.EqualTo(CBORValue.FromUInt64(3)));

                Assert.That(read!.NetworkingMode,         Is.EqualTo(Mode));
                Assert.That(read.RequestId,               Is.EqualTo(requestId));
                Assert.That(read.Payload,                 Is.EqualTo(payload));

                if (Mode == NetworkingMode.OverlayNetwork)
                {
                    Assert.That(read.Destination.Next,    Is.EqualTo(station));
                    Assert.That(read.NetworkPath.Source,  Is.EqualTo(NetworkingNode_Id.CSMS));
                }

            });

        }

        #endregion

        #region ASendMessage_IsReadBackAsItWasWritten(Mode)

        [TestCaseSource(nameof(Modes))]
        public void ASendMessage_IsReadBackAsItWasWritten(NetworkingMode Mode)
        {

            var written = new OCPP_CBORSendMessage(
                              Timestamp.Now,
                              EventTracking_Id.New,
                              Mode,
                              SourceRouting.To(NetworkingNode_Id.CSMS),
                              NetworkPath.From(station),
                              requestId,
                              "NotifyPeriodicEventStream",
                              payload
                          );

            Assert.That(OCPP_CBORFrame.TryParse(written.ToByteArray(), out var cborArray, out var frameError), Is.True, frameError);
            Assert.That(OCPP_CBORSendMessage.TryParse(cborArray, out var read, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {

                Assert.That(cborArray.AsArray()[0],       Is.EqualTo(CBORValue.FromUInt64(6)));

                Assert.That(read!.NetworkingMode,         Is.EqualTo(Mode));
                Assert.That(read.MessageId,               Is.EqualTo(requestId));
                Assert.That(read.Action,                  Is.EqualTo("NotifyPeriodicEventStream"));
                Assert.That(read.Payload,                 Is.EqualTo(payload));

            });

        }

        #endregion

        #region ARequestError_IsReadBackAsItWasWritten(Mode)

        [TestCaseSource(nameof(Modes))]
        public void ARequestError_IsReadBackAsItWasWritten(NetworkingMode Mode)
        {

            var written = new OCPP_CBORRequestErrorMessage(
                              Timestamp.Now,
                              EventTracking_Id.New,
                              Mode,
                              SourceRouting.To(station),
                              NetworkPath.From(NetworkingNode_Id.CSMS),
                              requestId,
                              ResultCode.NotImplemented,
                              "Not served over CBOR yet!",
                              details
                          );

            Assert.That(OCPP_CBORFrame.TryParse(written.ToByteArray(), out var cborArray, out var frameError), Is.True, frameError);
            Assert.That(OCPP_CBORRequestErrorMessage.TryParse(cborArray, out var read, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {

                Assert.That(cborArray.AsArray()[0],       Is.EqualTo(CBORValue.FromUInt64(4)));
                Assert.That(cborArray.AsArray()[^1].Kind, Is.EqualTo(CBORValueKind.Map));

                Assert.That(read!.NetworkingMode,         Is.EqualTo(Mode));
                Assert.That(read.RequestId,               Is.EqualTo(requestId));
                Assert.That(read.ErrorCode,               Is.EqualTo(ResultCode.NotImplemented));
                Assert.That(read.ErrorDescription,        Is.EqualTo("Not served over CBOR yet!"));
                Assert.That(JToken.DeepEquals(read.ErrorDetails, details), Is.True, read.ErrorDetails.ToString());

                // Made as JSON, sent as CBOR - and the other way round.
                Assert.That(OCPP_CBORRequestErrorMessage.From(written.ToJSONMessage()).ToByteArray(),
                            Is.EqualTo(written.ToByteArray()));

            });

        }

        #endregion

        #region AResponseError_IsReadBackAsItWasWritten()

        /// <summary>
        /// In standard mode only: the JSON response error message, whose frame this is,
        /// writes the overlay form and reads the standard form alone.
        /// </summary>
        [Test]
        public void AResponseError_IsReadBackAsItWasWritten()
        {

            var Mode = NetworkingMode.Standard;

            var written = new OCPP_CBORResponseErrorMessage(
                              Timestamp.Now,
                              EventTracking_Id.New,
                              Mode,
                              SourceRouting.To(NetworkingNode_Id.CSMS),
                              NetworkPath.From(station),
                              requestId,
                              ResultCode.ProtocolError,
                              "The response could not be read!",
                              details
                          );

            Assert.That(OCPP_CBORFrame.TryParse(written.ToByteArray(), out var cborArray, out var frameError), Is.True, frameError);
            Assert.That(OCPP_CBORResponseErrorMessage.TryParse(cborArray, out var read, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {

                Assert.That(cborArray.AsArray()[0],       Is.EqualTo(CBORValue.FromUInt64(5)));

                Assert.That(read!.NetworkingMode,         Is.EqualTo(Mode));
                Assert.That(read.RequestId,               Is.EqualTo(requestId));
                Assert.That(read.ErrorCode,               Is.EqualTo(ResultCode.ProtocolError));
                Assert.That(read.ErrorDescription,        Is.EqualTo("The response could not be read!"));
                Assert.That(JToken.DeepEquals(read.ErrorDetails, details), Is.True, read.ErrorDetails.ToString());

            });

        }

        #endregion


        #region ARequestInStandardMode_CameFromWhoeverTheConnectionBelongsTo()

        /// <summary>
        /// A message in standard mode names no sender: it came from the far end of its connection.
        /// </summary>
        [Test]
        public void ARequestInStandardMode_CameFromWhoeverTheConnectionBelongsTo()
        {

            var cborArray = new OCPP_CBORRequestMessage(
                                Timestamp.Now,
                                EventTracking_Id.New,
                                NetworkingMode.Standard,
                                SourceRouting.To(NetworkingNode_Id.CSMS),
                                NetworkPath.Empty,
                                requestId,
                                "Heartbeat",
                                CBORValue.FromMap([])
                            ).ToCBOR();

            Assert.That(OCPP_CBORRequestMessage.TryParse(cborArray, out var read, out var errorResponse, ImplicitSourceNodeId: station), Is.True, errorResponse);
            Assert.That(read!.NetworkPath.Source, Is.EqualTo(station));

        }

        #endregion

        #region AMessageOfOneKind_IsNotReadAsAnother()

        [Test]
        public void AMessageOfOneKind_IsNotReadAsAnother()
        {

            var request   = new OCPP_CBORRequestMessage (Timestamp.Now, EventTracking_Id.New, NetworkingMode.Standard, SourceRouting.To(NetworkingNode_Id.CSMS), NetworkPath.From(station), requestId, "Heartbeat", payload).ToCBOR();
            var response  = new OCPP_CBORResponseMessage(Timestamp.Now, EventTracking_Id.New, NetworkingMode.Standard, SourceRouting.To(station), NetworkPath.From(NetworkingNode_Id.CSMS), requestId, payload).ToCBOR();

            Assert.Multiple(() => {
                Assert.That(OCPP_CBORResponseMessage.     TryParse(request,  out _, out _),  Is.False);
                Assert.That(OCPP_CBORSendMessage.         TryParse(request,  out _, out _),  Is.False);
                Assert.That(OCPP_CBORRequestErrorMessage. TryParse(request,  out _, out _),  Is.False);
                Assert.That(OCPP_CBORResponseErrorMessage.TryParse(request,  out _, out _),  Is.False);
                Assert.That(OCPP_CBORRequestMessage.      TryParse(response, out _, out _),  Is.False);
            });

        }

        #endregion

        #region ABinaryPayload_IsAByteString()

        /// <summary>
        /// A message of the binary format of the BinaryDataStreams extension is a CBOR
        /// message as every other, whose payload is a byte string: the bytes its class
        /// wrote.
        /// </summary>
        [Test]
        public void ABinaryPayload_IsAByteString()
        {

            var bytes  = new Byte[] { 0x00, 0x01, 0x02, 0xfe, 0xff };

            var array  = CBORValue.FromArray(
                             CBORValue.FromUInt64(2),
                             CBORValue.FromText  ("4711"),
                             CBORValue.FromText  ("BinaryDataTransfer"),
                             CBORValue.FromBytes (bytes)
                         );

            Assert.That(OCPP_CBORFrame.TryParse(array.ToByteArray(), out var parsed, out var e1),  Is.True, e1);
            Assert.That(OCPP_CBORFrame.TrySplit(parsed, out var header, out var last, out var e2), Is.True, e2);
            Assert.That(OCPP_CBORRequestMessage.TryParse(parsed, out var request, out var e3),     Is.True, e3);

            Assert.Multiple(() => {
                Assert.That(header,                                Has.Count.EqualTo(4));
                Assert.That(last.TryGetBytes(out var payload),     Is.True);
                Assert.That(payload,                               Is.EqualTo(bytes));
                Assert.That(request!.Action,                       Is.EqualTo("BinaryDataTransfer"));
                Assert.That(request.Payload.TryGetBytes(out var p), Is.True);
                Assert.That(p,                                     Is.EqualTo(bytes));
            });

        }

        #endregion

        #region WhatIsNoFrame_IsRefused()

        [Test]
        public void WhatIsNoFrame_IsRefused()
        {

            var aMap          = payload;
            var tooShort      = CBORValue.FromArray(CBORValue.FromUInt64(3), CBORValue.FromText("4711"));
            var noMapAtItsEnd = CBORValue.FromArray(CBORValue.FromUInt64(3), CBORValue.FromText("4711"), CBORValue.FromText("{}"));
            var aBooleanInIt  = CBORValue.FromArray(CBORValue.FromUInt64(2), CBORValue.FromBoolean(true), CBORValue.FromText("Heartbeat"), payload);
            var aNumberAsHop  = CBORValue.FromArray(CBORValue.FromUInt64(2), CBORValue.FromArray(CBORValue.FromUInt64(1)), CBORValue.FromText("CS"), CBORValue.FromText("4711"), CBORValue.FromText("Heartbeat"), payload);

            Assert.Multiple(() => {

                Assert.That(OCPP_CBORFrame.TryParse([],                                 out _, out var empty),     Is.False);
                Assert.That(empty,                                                      Is.Not.Null);

                Assert.That(OCPP_CBORFrame.TryParse([ 0xff, 0x00, 0x01 ],               out _, out var notCBOR),   Is.False);
                Assert.That(notCBOR,                                                    Is.Not.Null);

                // The binary format of the BinaryDataStreams extension begins with its message type.
                Assert.That(OCPP_CBORFrame.TryParse([ 0x02, 0x00, 0x00, 0x00, 0x00 ],   out _, out var binary),    Is.False);
                Assert.That(binary,                                                     Is.Not.Null);

                Assert.That(OCPP_CBORFrame.TryParse(aMap.ToByteArray(),                 out _, out var notAnArray), Is.False);
                Assert.That(notAnArray,                                                 Does.Contain("not an array"));

                Assert.That(OCPP_CBORRequestMessage.TryParse(aMap,          out _, out var e1),  Is.False);
                Assert.That(e1,                                                         Does.Contain("not an array"));

                Assert.That(OCPP_CBORResponseMessage.TryParse(tooShort,     out _, out var e2),  Is.False);
                Assert.That(e2,                                                         Does.Contain("at least 3"));

                Assert.That(OCPP_CBORResponseMessage.TryParse(noMapAtItsEnd, out _, out var e3), Is.False);
                Assert.That(e3,                                                         Does.Contain("neither a map nor a byte string"));

                Assert.That(OCPP_CBORRequestMessage.TryParse(aBooleanInIt,  out _, out var e4),  Is.False);
                Assert.That(e4,                                                         Does.Contain("position 1"));

                Assert.That(OCPP_CBORRequestMessage.TryParse(aNumberAsHop,  out _, out var e5),  Is.False);
                Assert.That(e5,                                                         Does.Contain("not a text"));

            });

        }

        #endregion


        #region Subprotocols_SayHowMessagesAreWritten()

        [Test]
        public void Subprotocols_SayHowMessagesAreWritten()
        {

            Assert.Multiple(() => {

                Assert.That(OCPPSubprotocols.IsCBOR("ocpp2.1cbor"),   Is.True);
                Assert.That(OCPPSubprotocols.IsCBOR("ocpp2.1"),       Is.False);
                Assert.That(OCPPSubprotocols.IsCBOR("cbor"),          Is.False);
                Assert.That(OCPPSubprotocols.IsCBOR("ocpp2.1CBOR"),   Is.False);
                Assert.That(OCPPSubprotocols.IsCBOR(null),            Is.False);

                Assert.That(OCPPSubprotocols.CBOROf("ocpp2.1"),       Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));
                Assert.That(OCPPSubprotocols.CBOROf("ocpp2.1cbor"),   Is.EqualTo("ocpp2.1cbor"));
                Assert.That(OCPPSubprotocols.CBOROf("ocpp2.0.1"),     Is.Null);
                Assert.That(OCPPSubprotocols.CBOROf("ocpp1.6"),       Is.EqualTo("ocpp1.6cbor"));
                Assert.That(OCPPSubprotocols.CBOROf("ocpp2.0.1"),     Is.Null);

                Assert.That(OCPPSubprotocols.JSONOf("ocpp2.1cbor"),   Is.EqualTo(Version.WebSocketSubProtocolId));
                Assert.That(OCPPSubprotocols.JSONOf("ocpp2.1"),       Is.EqualTo("ocpp2.1"));

            });

        }

        #endregion

        #region AnOffer_FollowsTheEncoding()

        [Test]
        public void AnOffer_FollowsTheEncoding()
        {

            String[] versions = [ "ocpp2.1", "ocpp2.0.1" ];

            Assert.Multiple(() => {

                Assert.That(OCPPSubprotocols.Offer(versions,        MessageEncodings.JSON),  Is.EqualTo(new[] { "ocpp2.1", "ocpp2.0.1" }));
                Assert.That(OCPPSubprotocols.Offer(versions,        MessageEncodings.CBOR),  Is.EqualTo(new[] { "ocpp2.1cbor" }));
                Assert.That(OCPPSubprotocols.Offer(versions,        MessageEncodings.Auto),  Is.EqualTo(new[] { "ocpp2.1cbor", "ocpp2.1", "ocpp2.0.1" }));

                // OCPP 1.6 has its CBOR form as OCPP 2.1 has.
                Assert.That(OCPPSubprotocols.Offer([ "ocpp1.6" ],   MessageEncodings.CBOR),  Is.EqualTo(new[] { "ocpp1.6cbor" }));
                Assert.That(OCPPSubprotocols.Offer([ "ocpp1.6" ],   MessageEncodings.Auto),  Is.EqualTo(new[] { "ocpp1.6cbor", "ocpp1.6" }));

                // OCPP 2.0.1 has no CBOR form: CBOR alone offers nothing, and nothing is connected.
                Assert.That(OCPPSubprotocols.Offer([ "ocpp2.0.1" ], MessageEncodings.CBOR),  Is.Empty);
                Assert.That(OCPPSubprotocols.Offer([ "ocpp2.0.1" ], MessageEncodings.Auto),  Is.EqualTo(new[] { "ocpp2.0.1" }));

                // Whoever hands in a CBOR form gets what its encoding says, not what it handed in.
                Assert.That(OCPPSubprotocols.Offer([ "ocpp2.1cbor", "ocpp2.1" ], MessageEncodings.JSON),  Is.EqualTo(new[] { "ocpp2.1" }));

            });

        }

        #endregion

    }

}
