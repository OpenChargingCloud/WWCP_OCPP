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

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// What the node that dials is called in what its connections say - in
    /// their warnings, in the log, in the refusals of their stores: "this
    /// station cannot ...", "this local controller cannot ...".
    /// </summary>
    /// <remarks>
    /// One word for the process, set once by the node when it is made: a
    /// process is one kind of node, and the texts are written where there
    /// is no node to ask - in a store, in a static check of what was typed.
    /// Nothing but texts reads it.
    /// </remarks>
    public static class NodeWording
    {

        /// <summary>
        /// The node in a word: "station", "local controller"; "node" where
        /// nobody said.
        /// </summary>
        public static String Noun        { get; set; } = "node";

        /// <summary>
        /// The node by its full name, where a text gives it: "charging
        /// station", "local controller"; "node" where nobody said.
        /// </summary>
        public static String LongNoun    { get; set; } = "node";

        /// <summary>
        /// Say what this process is.
        /// </summary>
        /// <param name="Noun">The node in a word.</param>
        /// <param name="LongNoun">The node by its full name; the word where there is none.</param>
        public static void Is(String Noun, String? LongNoun = null)
        {
            NodeWording.Noun      = Noun;
            NodeWording.LongNoun  = LongNoun ?? Noun;
        }

    }

}
