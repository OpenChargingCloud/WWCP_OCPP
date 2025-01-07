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

namespace cloud.charging.open.protocols.OCPP.NetworkingNode
{

    /// <summary>
    /// Message forwarding decisions
    /// </summary>
    public enum ForwardingDecisions
    {

        /// <summary>
        /// DROP the message without any notification.
        /// </summary>
        DROP,

        /// <summary>
        /// REJECT the message with a notification.
        /// </summary>
        REJECT,

        /// <summary>
        /// FORWARD the message.
        /// </summary>
        FORWARD,

        /// <summary>
        /// REPLACE the received message and sent a new message instead.
        /// </summary>
        REPLACE,

        /// <summary>
        /// Let the next processing step handle the message.
        /// </summary>
        NEXT

    }

}
