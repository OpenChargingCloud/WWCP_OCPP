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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests
{

    /// <summary>
    /// The binary frames of a connection that agreed on a CBOR subprotocol, for the
    /// frame logs of the tests: an OCPP message as CBOR, shown as the JSON array it
    /// stands for - so that a test counts and reads its frames the same way, however
    /// they crossed the wire.
    /// </summary>
    internal static class CBORFrames
    {

        /// <summary>
        /// The given CBOR message - as OnCBORMessageReceived and OnCBORMessageSent tell
        /// of it, over a connection that speaks CBOR - as a JSON array, where it is an
        /// OCPP message.
        /// </summary>
        /// <param name="CBORMessage">The CBOR of the frame.</param>
        /// <param name="JSON">The JSON array the frame stands for.</param>
        internal static Boolean TryAsJSON(CBORValue   CBORMessage,
                                          out JArray  JSON)
        {

            JSON = [];

            if (!OCPP_CBORFrame.TryAsJSON(CBORMessage, out var json, out _))
                return false;

            JSON = json;
            return true;

        }

    }

}
