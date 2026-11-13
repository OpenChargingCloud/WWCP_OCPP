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

using cloud.charging.open.protocols.OCPP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// The stores of connections say what they refuse and warn of in the
    /// words of the node they are the stores of. These tests came from the
    /// charging station and read them as a station says them - which also
    /// shows that the word is the node's, and not the stores'.
    /// </summary>
    [SetUpFixture]
    public class ReadAsAStationSaysThem
    {

        private String  noun      = "";
        private String  longNoun  = "";

        [OneTimeSetUp]
        public void ThisIsAStation()
        {
            noun      = NodeWording.Noun;
            longNoun  = NodeWording.LongNoun;
            NodeWording.Is("station", "charging station");
        }

        [OneTimeTearDown]
        public void AsItWas()
        {
            NodeWording.Is(noun, longNoun);
        }

    }

}
