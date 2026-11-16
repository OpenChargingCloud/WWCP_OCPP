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

using org.GraphDefined.Vanaheimr.Hermod.Messaging;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// A connection a node accepted that is its line up all the same: the
    /// CSMS, or the controller above it, dialled the node - where the node
    /// cannot dial them.
    /// </summary>
    /// <remarks>
    /// Who opened a connection says nothing of who is above whom. A frame of
    /// plain OCPP names neither where it goes nor where it comes from, and a
    /// node reads that off the connection: what comes over a connection it
    /// dialled is for the node itself, and what comes over one it accepted is
    /// on its way to the CSMS - a charging station's, below a local
    /// controller. A connection marked here is read as one the node dialled.
    /// </remarks>
    public static class LineUpExtensions
    {

        /// <summary>
        /// What a connection carries that a node accepted as its line up.
        /// </summary>
        public const String LineUpKey = "ocpp.lineUp";

        /// <summary>
        /// Mark the given connection, which this node accepted, as its line up.
        /// </summary>
        /// <param name="Connection">A connection.</param>
        public static void IsTheLineUp(this IMessageConnection Connection)
        {
            Connection.TryAddCustomData(LineUpKey, true);
        }

        /// <summary>
        /// Whether the given connection was accepted as the line up of its node.
        /// </summary>
        /// <param name="Connection">A connection.</param>
        public static Boolean IsALineUp(this IMessageConnection Connection)

            => Connection.TryGetCustomData(LineUpKey) is true;

    }

}
