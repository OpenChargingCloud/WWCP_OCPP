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

using System.Security.Authentication;

using Newtonsoft.Json.Linq;

using NUnit.Framework;


using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The "webSocketServer" section: what may be written in it, what may not, and
    /// which of it can be changed without a restart.
    /// </summary>
    public class WebSocketServerConfigurationTests
    {

        #region (private static) Parse(JSON) / Refuse(JSON)

        private static WebSocketServerConfiguration Parse(JObject JSON)
        {
            Assert.That(WebSocketServerConfiguration.TryParse(JSON, out var configuration, out var error), Is.True, error);
            return configuration!;
        }

        private static String Refuse(JObject JSON)
        {
            Assert.That(WebSocketServerConfiguration.TryParse(JSON, out _, out var error), Is.False,
                        "This was accepted when it should have been refused.");
            return error!;
        }

        #endregion


        #region AnEmptySectionSaysNothingAtAll()

        [Test]
        public void AnEmptySectionSaysNothingAtAll()
        {

            var configuration = Parse([]);

            Assert.Multiple(() => {
                Assert.That(configuration.IsEmpty,  Is.True);
                Assert.That(configuration.Enabled,  Is.Null);
                Assert.That(configuration.TCPPort,  Is.Null);
            });

        }

        #endregion

        #region SwitchedOffIsWhatNobodySayingAnythingMeans()

        /// <summary>
        /// This is the one setting in the file that opens a port to a network
        /// rather than reaching out to one. A controller that started listening
        /// because it was updated to a version that can would be a port nobody
        /// chose to open.
        /// </summary>
        [Test]
        public void SwitchedOffIsWhatNobodySayingAnythingMeans()
        {

            var effective = new WebSocketServerConfiguration().Effective();

            Assert.Multiple(() => {
                Assert.That(effective.Enabled,           Is.False);
                Assert.That(effective.TCPPort,           Is.EqualTo(WebSocketServerConfiguration.DefaultTCPPort));
                Assert.That(effective.Address?.ToString(), Is.EqualTo("0.0.0.0"));
                Assert.That(effective.Subprotocols,      Is.EqualTo(WebSocketServerConfiguration.DefaultSubprotocols));
            });

        }

        #endregion

        #region TheWholeSectionComesBackAsItWasWritten()

        [Test]
        public void TheWholeSectionComesBackAsItWasWritten()
        {

            var written = new JObject(
                              new JProperty("enabled",                     true),
                              new JProperty("address",                     "10.0.0.1"),
                              new JProperty("port",                        9000),
                              new JProperty("clientAuthentication",        new JArray("httpAuthOverTLS", "tlsClientCertificate")),
                              new JProperty("subprotocols",                new JArray("ocpp2.1")),
                              new JProperty("reachableAs",                 new JArray("lc001.example.org", "10.0.0.1")),
                              new JProperty("minTLSVersion",               "1.3"),
                              new JProperty("checkCertificateRevocation",  true),
                              new JProperty("maxConnections",              42),
                              new JProperty("pingEverySeconds",            15)
                          );

            var configuration = Parse(written);

            Assert.Multiple(() => {
                Assert.That(configuration.Enabled,                     Is.True);
                Assert.That(configuration.Address?.ToString(),         Is.EqualTo("10.0.0.1"));
                Assert.That(configuration.TCPPort?.ToUInt16(),         Is.EqualTo(9000));
                Assert.That(configuration.ClientAuthentication,        Is.EqualTo(new[] { WebSocketClientAuthentication.HTTPAuthOverTLS, WebSocketClientAuthentication.TLSClientCertificate }));
                Assert.That(configuration.Subprotocols,                Is.EqualTo(new[] { "ocpp2.1" }));
                Assert.That(configuration.ReachableAs,                 Is.EqualTo(new[] { "lc001.example.org", "10.0.0.1" }));
                Assert.That(configuration.MinimumTLSVersion,           Is.EqualTo(SslProtocols.Tls13));
                Assert.That(configuration.CheckCertificateRevocation,  Is.True);
                Assert.That(configuration.MaxConnections,              Is.EqualTo(42));
                Assert.That(configuration.PingEvery,                   Is.EqualTo(TimeSpan.FromSeconds(15)));
            });

            // And out again the same way. Field by field and not as a whole:
            // a record holding lists compares those by reference, so comparing
            // the two records would pass for a reason that has nothing to do
            // with what was written.
            var again = Parse(configuration.ToJSON());

            Assert.Multiple(() => {
                Assert.That(again.Enabled,                     Is.EqualTo(configuration.Enabled));
                Assert.That(again.Address?.ToString(),         Is.EqualTo(configuration.Address?.ToString()));
                Assert.That(again.TCPPort,                     Is.EqualTo(configuration.TCPPort));
                Assert.That(again.ClientAuthentication,        Is.EqualTo(configuration.ClientAuthentication));
                Assert.That(again.Subprotocols,                Is.EqualTo(configuration.Subprotocols));
                Assert.That(again.ReachableAs,                 Is.EqualTo(configuration.ReachableAs));
                Assert.That(again.MinimumTLSVersion,           Is.EqualTo(configuration.MinimumTLSVersion));
                Assert.That(again.CheckCertificateRevocation,  Is.EqualTo(configuration.CheckCertificateRevocation));
                Assert.That(again.MaxConnections,              Is.EqualTo(configuration.MaxConnections));
                Assert.That(again.PingEvery,                   Is.EqualTo(configuration.PingEvery));
            });

        }

        #endregion

        #region OnlyTheThreeWaysToAuthenticateMayBeNamed()

        [Test]
        public void OnlyTheThreeWaysToAuthenticateMayBeNamed()
        {

            Assert.Multiple(() => {

                Assert.That(Refuse(new JObject(new JProperty("clientAuthentication", new JArray("byLookingTrustworthy")))),
                            Does.Contain("is no way a client may authenticate").And.Contain("\"tlsClientCertificate\""));

                Assert.That(Refuse(new JObject(new JProperty("clientAuthentication", new JArray(2)))),
                            Does.Contain("is no way a client may authenticate"));

                Assert.That(Refuse(new JObject(new JProperty("clientAuthentication", new JArray()))),
                            Does.Contain("at least one"));

                Assert.That(Refuse(new JObject(new JProperty("clientAuthentication", "all of them"))),
                            Does.Contain("array"));

            });

        }

        #endregion

        #region TheSecurityProfilesOfBeforeAreRefusedWithWhatToSayInstead()

        /// <summary>
        /// OCPP's security profiles are OCPP's, and no field of a WebSocket
        /// server: a file that names them is told what to say instead, and not
        /// read as if it had been obeyed.
        /// </summary>
        [Test]
        public void TheSecurityProfilesOfBeforeAreRefusedWithWhatToSayInstead()

            => Assert.That(Refuse(new JObject(new JProperty("securityProfiles", new JArray(2, 3)))),
                           Does.Contain("'webSocketServer.securityProfiles' is no more").
                                And.Contain("'webSocketServer.clientAuthentication'").
                                And.Contain("\"httpAuthOverTLS\""));

        #endregion

        #region EachWayIsTheSecurityProfileOCPPCallsIt()

        [Test]
        public void EachWayIsTheSecurityProfileOCPPCallsIt()
        {

            Assert.Multiple(() => {

                Assert.That(WebSocketClientAuthentication.HTTPAuthWithoutTLS.  SecurityProfile(), Is.EqualTo(1));
                Assert.That(WebSocketClientAuthentication.HTTPAuthOverTLS.     SecurityProfile(), Is.EqualTo(2));
                Assert.That(WebSocketClientAuthentication.TLSClientCertificate.SecurityProfile(), Is.EqualTo(3));

                Assert.That(ClientAuthenticationProfiles.OfSecurityProfile(3, out var three), Is.True);
                Assert.That(three,                                                             Is.EqualTo(WebSocketClientAuthentication.TLSClientCertificate));
                Assert.That(ClientAuthenticationProfiles.OfSecurityProfile(4, out _),         Is.False);

                Assert.That(ClientAuthenticationProfiles.SecurityProfiles(WebSocketClientAuthentications.All),
                            Is.EqualTo(new Byte[] { 1, 2, 3 }));

            });

        }

        #endregion

        #region TheSameWayTwiceIsTheSameWay()

        [Test]
        public void TheSameWayTwiceIsTheSameWay()

            => Assert.That(
                   Parse(new JObject(new JProperty("clientAuthentication", new JArray("tlsClientCertificate", "httpAuthOverTLS", "tlsClientCertificate")))).ClientAuthentication,
                   Is.EqualTo(new[] { WebSocketClientAuthentication.HTTPAuthOverTLS, WebSocketClientAuthentication.TLSClientCertificate })
               );

        #endregion

        #region OnlyOCPPVersionsThisControllerSpeaksMayBeOffered()

        [Test]
        public void OnlyOCPPVersionsThisControllerSpeaksMayBeOffered()
        {

            Assert.Multiple(() => {

                Assert.That(Refuse(new JObject(new JProperty("subprotocols", new JArray("ocpp3.0")))),
                            Does.Contain("ocpp3.0"));

                Assert.That(Refuse(new JObject(new JProperty("subprotocols", new JArray()))),
                            Does.Contain("at least one"));

                Assert.That(Parse(new JObject(new JProperty("subprotocols", new JArray("ocpp1.6")))).Subprotocols,
                            Is.EqualTo(new[] { "ocpp1.6" }));

            });

        }

        #endregion

        #region AReachableNameHasToBeANameOrAnAddress()

        /// <summary>
        /// These go into a signing request, and a certificate authority is not
        /// going to make sense of a sentence.
        /// </summary>
        [Test]
        public void AReachableNameHasToBeANameOrAnAddress()
        {

            Assert.Multiple(() => {

                Assert.That(Refuse(new JObject(new JProperty("reachableAs", new JArray("the controller in the car park")))),
                            Does.Contain("neither an IP address nor a domain name"));

                Assert.That(Parse(new JObject(new JProperty("reachableAs", new JArray("lc001.example.org", "LC001.example.org")))).ReachableAs,
                            Is.EqualTo(new[] { "lc001.example.org" }),
                            "The same name written two ways was kept twice.");

            });

        }

        #endregion

        #region NothingOlderThanTLS12IsOnOffer()

        [Test]
        public void NothingOlderThanTLS12IsOnOffer()
        {

            Assert.Multiple(() => {

                Assert.That(Refuse(new JObject(new JProperty("minTLSVersion", "1.0"))), Does.Contain("1.2"));

                Assert.That(Parse(new JObject(new JProperty("minTLSVersion", "1.2"))).MinimumTLSVersion,
                            Is.EqualTo(SslProtocols.Tls12 | SslProtocols.Tls13));

            });

        }

        #endregion

        #region AnAddressHasToBeAnAddress()

        [Test]
        public void AnAddressHasToBeAnAddress()
            => Assert.That(Refuse(new JObject(new JProperty("address", "the LAN side"))), Does.Contain("IP address"));

        #endregion

        #region AnAddressWithSomethingAroundItIsRefusedWithASentence(Text)

        /// <summary>
        /// An address with a port or a scheme around it is not an address, and
        /// is refused as the file's mistake - with the key named, and not with
        /// an exception: Hermod's IPAddress.TryParse used to find the address
        /// inside the text and then throw over the rest of it, which stopped a
        /// controller at its start without naming the file or the key.
        /// </summary>
        [TestCase("0.0.0.0:9000")]
        [TestCase("wss://192.168.1.10")]
        [TestCase("[2001:db8::1]:9000")]
        public void AnAddressWithSomethingAroundItIsRefusedWithASentence(String Text)
        {

            var      parsed  = true;
            String?  error   = null;

            Assert.That(() => parsed = WebSocketServerConfiguration.TryParse(new JObject(new JProperty("address", Text)), out _, out error),
                        Throws.Nothing);

            Assert.Multiple(() => {
                Assert.That(parsed,  Is.False);
                Assert.That(error,   Does.Contain("'webSocketServer.address'"));
            });

        }

        #endregion

        #region AReachableAddressWithAPortIsRefusedWithASentence(Text)

        /// <summary>
        /// The same for the names the stations reach this controller under: an
        /// address with a port is neither an address nor a name.
        /// </summary>
        [TestCase("192.168.1.10:9000")]
        [TestCase("wss://192.168.1.10")]
        public void AReachableAddressWithAPortIsRefusedWithASentence(String Text)
        {

            var      parsed  = true;
            String?  error   = null;

            Assert.That(() => parsed = WebSocketServerConfiguration.TryParse(new JObject(new JProperty("reachableAs", new JArray(Text))), out _, out error),
                        Throws.Nothing);

            Assert.Multiple(() => {
                Assert.That(parsed,  Is.False);
                Assert.That(error,   Does.Contain("'webSocketServer.reachableAs'").And.Contain(Text));
            });

        }

        #endregion

        #region ANameThatBeginsWithAnAddressIsAName()

        /// <summary>
        /// "10.0.0.1.nip.io" is a host name - the kind a certificate for a box
        /// that only has an address is often issued for - and not an address
        /// with something after it. Read as the latter, it used to stop the
        /// controller at its start.
        /// </summary>
        [Test]
        public void ANameThatBeginsWithAnAddressIsAName()
        {

            var                       parsed  = false;
            WebSocketServerConfiguration?  read    = null;
            String?                   error   = null;

            Assert.That(() => parsed = WebSocketServerConfiguration.TryParse(new JObject(new JProperty("reachableAs", new JArray("10.0.0.1.nip.io"))), out read, out error),
                        Throws.Nothing);

            Assert.Multiple(() => {
                Assert.That(parsed,             Is.True,  error);
                Assert.That(read?.ReachableAs,  Is.EqualTo(new[] { "10.0.0.1.nip.io" }));
            });

        }

        #endregion

        #region NumbersHaveToBeWithinReason()

        [Test]
        public void NumbersHaveToBeWithinReason()
        {

            Assert.Multiple(() => {
                Assert.That(Refuse(new JObject(new JProperty("port",              0))),      Does.Contain("port number"));
                Assert.That(Refuse(new JObject(new JProperty("maxConnections",    0))),      Does.Contain("between 1"));
                Assert.That(Refuse(new JObject(new JProperty("maxConnections",    999999))), Does.Contain("between 1"));
                Assert.That(Refuse(new JObject(new JProperty("pingEverySeconds",  1))),      Does.Contain("between 5"));
            });

        }

        #endregion


        #region MessageContentsCannotBeSwitchedOnWithoutSayingWhenTheyGoOff()

        /// <summary>
        /// Whoever turns this on is turning on the logging of who charged and
        /// where. "Until further notice" is not an answer anybody gives on
        /// purpose.
        /// </summary>
        [Test]
        public void MessageContentsCannotBeSwitchedOnWithoutSayingWhenTheyGoOff()
        {

            var error = Refuse(new JObject(
                            new JProperty("logging", new JObject(
                                new JProperty("messages",  true),
                                new JProperty("payloads",  true)
                            ))
                        ));

            Assert.That(error, Does.Contain("payloadsUntil"));

        }

        #endregion

        #region TheWindowIsWhatSwitchesTheContentsOffAgain()

        [Test]
        public void TheWindowIsWhatSwitchesTheContentsOffAgain()
        {

            var now     = new DateTimeOffset(2026, 6, 1, 12, 0, 0, TimeSpan.Zero);

            var logging = Parse(new JObject(
                              new JProperty("logging", new JObject(
                                  new JProperty("payloads",       true),
                                  new JProperty("payloadsUntil",  now.AddHours(1).ToString("o"))
                              ))
                          )).Logging!;

            Assert.Multiple(() => {
                Assert.That(logging.PayloadsAt(now),                 Is.True);
                Assert.That(logging.PayloadsAt(now.AddMinutes(59)),  Is.True);
                Assert.That(logging.PayloadsAt(now.AddHours(2)),     Is.False,
                            "The contents went on being logged after the window had passed.");
            });

        }

        #endregion

        #region AWindowIsReadWhicheverWayItArrived()

        /// <summary>
        /// A document parsed from text is not the same document as one built in
        /// memory: Newtonsoft turns what looks like a date into a date of its
        /// own accord, and what it turns it into is a DateTime.
        /// </summary>
        /// <remarks>
        /// Worth a test of its own because it is invisible from either side.
        /// Every test that hands a configuration over directly takes the second
        /// path and passes; every request from a browser takes the first.
        /// </remarks>
        [Test]
        public void AWindowIsReadWhicheverWayItArrived()
        {

            var until   = new DateTimeOffset(2026, 6, 1, 13, 0, 0, TimeSpan.Zero);

            var built   = new JObject(
                              new JProperty("logging", new JObject(
                                  new JProperty("payloads",       true),
                                  new JProperty("payloadsUntil",  until.ToString("o"))
                              ))
                          );

            var arrived = JObject.Parse(built.ToString());

            Assert.Multiple(() => {
                Assert.That(Parse(built)  .Logging?.PayloadsUntil, Is.EqualTo(until));
                Assert.That(Parse(arrived).Logging?.PayloadsUntil, Is.EqualTo(until),
                            "A window that arrived in a request body was not read as the moment it says.");
            });

        }

        #endregion

        #region SwitchingTheContentsOffTakesTheWindowWithIt()

        /// <summary>
        /// Or yesterday's window would be left behind for the next switching-on
        /// to inherit.
        /// </summary>
        [Test]
        public void SwitchingTheContentsOffTakesTheWindowWithIt()
        {

            var now      = new DateTimeOffset(2026, 6, 1, 12, 0, 0, TimeSpan.Zero);

            var before   = new WebSocketServerLogging(Payloads: true, PayloadsUntil: now.AddHours(1));
            var after    = before.Merge(new WebSocketServerLogging(Payloads: false));

            Assert.Multiple(() => {
                Assert.That(after.Payloads,           Is.False);
                Assert.That(after.PayloadsUntil,      Is.Null);
                Assert.That(after.PayloadsAt(now),    Is.False);
            });

        }

        #endregion

        #region TheLoudSwitchesAreOnAndTheQuietOnesAreOff()

        [Test]
        public void TheLoudSwitchesAreOnAndTheQuietOnesAreOff()
        {

            var logging = new WebSocketServerLogging().Effective();

            Assert.Multiple(() => {
                Assert.That(logging.Connections,     Is.True,  "A log that cannot say when a station was last seen answers nothing.");
                Assert.That(logging.Authentication,  Is.True,  "The only record of somebody trying was off by default.");
                Assert.That(logging.Messages,        Is.False);
                Assert.That(logging.Payloads,        Is.False, "Message contents were on by default.");
                Assert.That(logging.Pings,           Is.False);
            });

        }

        #endregion


        #region AMergeChangesWhatItMentionsAndNothingElse()

        /// <summary>
        /// A page that offers three checkboxes sends three checkboxes, and must
        /// not be able to take the port with it.
        /// </summary>
        [Test]
        public void AMergeChangesWhatItMentionsAndNothingElse()
        {

            var before = Parse(new JObject(
                             new JProperty("enabled",           true),
                             new JProperty("port",              9000),
                             new JProperty("reachableAs",       new JArray("lc001.example.org")),
                             new JProperty("clientAuthentication",  new JArray("httpAuthOverTLS", "tlsClientCertificate"))
                         ));

            var after  = before.Merge(Parse(new JObject(
                             new JProperty("logging", new JObject(new JProperty("pings", true)))
                         )));

            Assert.Multiple(() => {
                Assert.That(after.TCPPort?.ToUInt16(),  Is.EqualTo(9000));
                Assert.That(after.ReachableAs,          Is.EqualTo(new[] { "lc001.example.org" }));
                Assert.That(after.ClientAuthentication, Is.EqualTo(new[] { WebSocketClientAuthentication.HTTPAuthOverTLS, WebSocketClientAuthentication.TLSClientCertificate }));
                Assert.That(after.Logging?.Pings,       Is.True);
            });

        }

        #endregion

        #region TheSocketIsWhatWaitsForARestart()

        /// <summary>
        /// A page that quietly does nothing is worse than one that says "at the
        /// next start".
        /// </summary>
        [Test]
        public void TheSocketIsWhatWaitsForARestart()
        {

            var running = Parse(new JObject(
                              new JProperty("port",              9000),
                              new JProperty("clientAuthentication",  new JArray("httpAuthWithoutTLS", "httpAuthOverTLS")),
                              new JProperty("subprotocols",          new JArray("ocpp2.1"))
                          )).Effective();

            Assert.Multiple(() => {

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(new JProperty("port", 9001))))),
                            Does.Contain("port"));

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(new JProperty("address", "10.0.0.1"))))),
                            Does.Contain("address"));

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(new JProperty("subprotocols", new JArray("ocpp1.6")))))),
                            Does.Contain("subprotocols"));

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(new JProperty("minTLSVersion", "1.3"))))),
                            Does.Contain("minTLSVersion"));

            });

        }

        #endregion

        #region WhatIsReadAfreshDoesNotWaitForARestart()

        [Test]
        public void WhatIsReadAfreshDoesNotWaitForARestart()
        {

            var running = Parse(new JObject(
                              new JProperty("port",              9000),
                              new JProperty("clientAuthentication",  new JArray("httpAuthWithoutTLS", "httpAuthOverTLS", "tlsClientCertificate"))
                          )).Effective();

            Assert.Multiple(() => {

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(
                                new JProperty("logging", new JObject(new JProperty("messages", true))))))),
                            Is.Empty);

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(
                                new JProperty("reachableAs", new JArray("somewhere.else.example")))))),
                            Is.Empty);

                // How a client may authenticate is decided per connection: a
                // client is asked for a certificate either way, and what
                // happens to the answer is read afresh every time.
                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(
                                new JProperty("clientAuthentication", new JArray("httpAuthOverTLS", "tlsClientCertificate")))))),
                            Is.Empty);

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(
                                new JProperty("clientAuthentication", new JArray("httpAuthWithoutTLS", "httpAuthOverTLS", "tlsClientCertificate")))))),
                            Is.Empty);

                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(
                                new JProperty("enabled", true))))),
                            Is.Empty);

                // HTTP/3 opens and closes its UDP port while the server runs.
                Assert.That(running.NeedsARestartFor(running.Merge(Parse(new JObject(
                                new JProperty("http3", true))))),
                            Is.Empty);

            });

        }

        #endregion


        #region HTTP3IsOffUnlessSaid_AndWrittenWhereItWasSaid()

        /// <summary>
        /// HTTP/3 opens a UDP port beside the TCP one, which nobody is given
        /// who did not ask: off where the file says nothing, and "false" is
        /// written as it was said.
        /// </summary>
        [Test]
        public void HTTP3IsOffUnlessSaid_AndWrittenWhereItWasSaid()
        {

            var silent = Parse(new JObject(new JProperty("port", 9443)));
            var on     = Parse(new JObject(new JProperty("port", 9443), new JProperty("http3", true)));
            var off    = Parse(new JObject(new JProperty("port", 9443), new JProperty("http3", false)));

            Assert.Multiple(() => {

                Assert.That(silent.TakesHTTP3,                          Is.False, "HTTP/3 is on although the file says nothing of it");
                Assert.That(silent.ToJSON().ContainsKey("http3"),       Is.False);
                Assert.That(silent.Effective().TakesHTTP3,              Is.False);

                Assert.That(on.TakesHTTP3,                              Is.True);
                Assert.That(on.ToJSON().Value<Boolean>("http3"),        Is.True);
                Assert.That(on.IsEmpty,                                 Is.False);

                Assert.That(off.TakesHTTP3,                             Is.False);
                Assert.That(off.ToJSON().Value<Boolean?>("http3"),      Is.False);

                Assert.That(on.Merge(Parse([])).TakesHTTP3,             Is.True,  "a merge that says nothing of HTTP/3 took it away");
                Assert.That(on.Merge(off).TakesHTTP3,                   Is.False);

                Assert.That(Refuse(new JObject(new JProperty("http3", "yes"))), Does.Contain("http3"));

            });

        }

        #endregion

        #region APortOfItsOwnForOCPP16_IsNoMore()

        /// <summary>
        /// Every subprotocol is served on the one port - OCPP 1.6 where the
        /// subprotocols name it - and a file that still gives OCPP 1.6 a port
        /// of its own is told so, rather than read as if it had been obeyed.
        /// </summary>
        [Test]
        public void APortOfItsOwnForOCPP16_IsNoMore()
        {

            Assert.Multiple(() => {

                Assert.That(Refuse(new JObject(new JProperty("port", 9443), new JProperty("port16", 9444))),
                            Does.Contain("'webSocketServer.port16' is no more").And.Contain("\"ocpp1.6\""));

                Assert.That(WebSocketServerConfiguration.TryParse(new JObject(new JProperty("port", 9443), new JProperty("http3", true)), out var one, out var error),
                            Is.True, error);
                Assert.That(one!.ToJSON().ContainsKey("port16"),                Is.False);

                Assert.That(Parse(new JObject(new JProperty("subprotocols", new JArray("ocpp2.1", "ocpp1.6")))).Subprotocols,
                            Is.EqualTo(new[] { "ocpp2.1", "ocpp1.6" }));

            });

        }

        #endregion

        #region WhatIsNotSwitchedOnSaysSo()

        /// <summary>
        /// A section that says nothing of "enabled" is a server that is off,
        /// and is not said to be on.
        /// </summary>
        [Test]
        public void WhatIsNotSwitchedOnSaysSo()
        {

            Assert.Multiple(() => {
                Assert.That(Parse(new JObject(new JProperty("port", 9443))).ToString(),                                     Is.EqualTo("WebSocket server off"));
                Assert.That(Parse(new JObject(new JProperty("enabled", true), new JProperty("port", 9443))).ToString(),     Is.EqualTo("WebSocket server on every interface:9443"));
                Assert.That(new WebSocketServerConfiguration().Effective().ToString(),                                      Is.EqualTo("WebSocket server off"));
            });

        }

        #endregion

    }

}
