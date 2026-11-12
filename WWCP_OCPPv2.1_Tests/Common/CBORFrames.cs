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
        /// The given binary frame as a JSON array, where its connection speaks CBOR
        /// and the frame is an OCPP message.
        /// </summary>
        /// <param name="Connection">The connection of the frame.</param>
        /// <param name="Frame">The bytes of the frame.</param>
        /// <param name="JSON">The JSON array the frame stands for.</param>
        internal static Boolean TryAsJSON(IMessageConnection?  Connection,
                                          Byte[]               Frame,
                                          out JArray           JSON)
        {

            JSON = [];

            if (!Connection.SpeaksCBOR())
                return false;

            if (!OCPP_CBORFrame.TryParse(Frame, out var cborArray, out _) ||
                !OCPP_CBORFrame.TrySplit(cborArray, out var header, out var last, out _))
            {
                return false;
            }

            header[header.Count - 1] = CBORJSON.ToJSON(last);

            JSON = header;
            return true;

        }

    }

}
