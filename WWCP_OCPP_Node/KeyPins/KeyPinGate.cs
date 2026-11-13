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

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// What a wss:// connection on probation lets through: a pin set of key pins, and what a CSMS reads
    /// to find the instance of TLSClientController the pin set is for.
    /// </summary>
    public static class KeyPinGate
    {

        #region Rules(Probation, Action, Payload, NodeName = "node")

        /// <summary>
        /// Why a request that came over a wss:// connection on probation is not processed - or null
        /// where it is: SetVariables of the TLSClientController instance of this very connection, or
        /// ChangeConfiguration of evqi.serverKeyPins and evqi.serverKeyPinsVersion in OCPP 1.6 ("Key Pins
        /// and Raw Public Keys", sections 7.2 and 7.3); and GetVariables, and GetReport, of
        /// TLSClientController alone.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A write of the instance of this connection, and of no other: a server on probation does not
        /// confirm another one's key pins, nor its own through another instance's.
        /// </para>
        /// <para>
        /// A read of any instance, since the instance is named by the node's id of the connection, which
        /// the CSMS does not know before it read the URL, the key pin shown or the one learned of each:
        /// what that tells is the node's servers and the key pins of their public keys, nothing a server
        /// on probation could use against it. GetBaseReport is refused: it names no component, and
        /// reports the whole Device Model.
        /// </para>
        /// </remarks>
        /// <param name="Probation">The probation of the connection the request came over: one of key pins.</param>
        /// <param name="Action">The OCPP action.</param>
        /// <param name="Payload">Its payload.</param>
        /// <param name="NodeName">What the node is called in a sentence: "station", "local controller".</param>
        public static String? Rules(Probation  Probation,
                                    String     Action,
                                    JObject?   Payload,
                                    String     NodeName   = "node")
        {

            if (Action == "SetVariables" &&
                Payload?["setVariableData"] is JArray pinSet && pinSet.Count > 0 &&
                pinSet.All(entry => entry?["component"]?["name"]?.    Value<String>() == "TLSClientController" &&
                                    entry?["component"]?["instance"]?.Value<String>() == Probation.EndpointId))
                return null;

            if (Action == "GetVariables" &&
                Payload?["getVariableData"] is JArray reads && reads.Count > 0 &&
                reads.All(entry => entry?["component"]?["name"]?.Value<String>() == "TLSClientController"))
                return null;

            if (Action == "GetReport" &&
                Payload?["componentVariable"] is JArray asked && asked.Count > 0 &&
                asked.All(entry => entry?["component"]?["name"]?.Value<String>() == "TLSClientController"))
                return null;

            if (Action == "ChangeConfiguration" &&
                Payload?["key"]?.Value<String>() is "evqi.serverKeyPins" or "evqi.serverKeyPinsVersion")
                return null;

            return $"This {NodeName} believes its server by the key pin {(Probation.Trust as ServerKeyPinTrust)?.Learned}, which it learned on its first connection " +
                    "and which is on probation: until a signed pin set confirms it, nothing but such a pin set is executed.";

        }

        #endregion

    }

}
