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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The OCPP log of a node: an answer is told by the action of the request
    /// it answers - the one that went the other way, to or from the same node
    /// - the ring keeps the newest, and a reader catches up with "after".
    /// </summary>
    [TestFixture]
    public class OCPPMessageLogTests
    {

        private static JArray Call  (String Id, String Action) => new (2, Id, Action, new JObject());
        private static JArray Result(String Id)                => new (3, Id, new JObject());


        [Test]
        public void AnAnswerIsToldByTheActionOfItsRequest()
        {

            var log = new OCPPMessageLog();

            // Both ends number their requests from "1": the station's
            // BootNotification and the CSMS's GetVariables have the same id.
            log.Add(OCPPMessageDirection.Out, OCPPMessageKind.Request,  "2.1", "CSMS", "1", "BootNotification", Call("1", "BootNotification"));
            log.Add(OCPPMessageDirection.In,  OCPPMessageKind.Request,  "2.1", "CSMS", "1", "GetVariables",     Call("1", "GetVariables"));

            var bootAnswered  = log.Add(OCPPMessageDirection.In,  OCPPMessageKind.Response, "2.1", "CSMS", "1", null, Result("1"));
            var getAnswered   = log.Add(OCPPMessageDirection.Out, OCPPMessageKind.Response, "2.1", "CSMS", "1", null, Result("1"));

            Assert.That(bootAnswered.Action, Is.EqualTo("BootNotification"));
            Assert.That(getAnswered. Action, Is.EqualTo("GetVariables"));

        }

        [Test]
        public void AnAnswerToAnotherNodesRequestIsNotToldByIt()
        {

            var log = new OCPPMessageLog();

            log.Add(OCPPMessageDirection.Out, OCPPMessageKind.Request, "2.1", "LC001", "7", "Heartbeat", Call("7", "Heartbeat"));

            var answer = log.Add(OCPPMessageDirection.In, OCPPMessageKind.Response, "2.1", "test02", "7", null, Result("7"));

            Assert.That(answer.Action, Is.Null);

        }

        [Test]
        public void TheRingKeepsTheNewestAndAReaderCatchesUp()
        {

            var log = new OCPPMessageLog(Capacity: 3);

            for (var i = 1; i <= 5; i++)
                log.Add(OCPPMessageDirection.Out, OCPPMessageKind.Send, "2.1", "CSMS", i.ToString(), "NotifyPeriodicEventStream", Call(i.ToString(), "NotifyPeriodicEventStream"));

            Assert.That(log.Count,  Is.EqualTo(3));
            Assert.That(log.LastId, Is.EqualTo(5UL));
            Assert.That(log.Recent(10).Select(entry => entry.Id),           Is.EqualTo(new UInt64[] { 3, 4, 5 }));
            Assert.That(log.Recent(10, After: 4).Select(entry => entry.Id), Is.EqualTo(new UInt64[] { 5 }));

            var json = log.Recent(1).Single().ToJSON();

            Assert.That(json["direction"]?.Value<String>(), Is.EqualTo("out"));
            Assert.That(json["kind"]?.     Value<String>(), Is.EqualTo("send"));
            Assert.That(json["message"],                   Is.InstanceOf<JArray>());

        }

    }

}
