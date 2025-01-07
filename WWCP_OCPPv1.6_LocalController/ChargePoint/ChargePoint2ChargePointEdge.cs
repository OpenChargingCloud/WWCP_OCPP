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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    [Flags]
    public enum ChargePoint2ChargePointEdgeLabel
    {
        IsSubsidary,
        IsParent,
        IsChildOf,
    }


    public class ChargePoint2ChargePointEdge : MiniEdge<ChargePoint, ChargePoint2ChargePointEdgeLabel, ChargePoint>
    {

        /// <summary>
        /// Create a new miniedge.
        /// </summary>
        /// <param name="ChargePointA">The source of the edge.</param>
        /// <param name="EdgeLabel">The label of the edge.</param>
        /// <param name="ChargePointB">The target of the edge</param>
        /// <param name="PrivacyLevel">The level of privacy of this edge.</param>
        /// <param name="Created">The creation timestamp of the miniedge.</param>
        public ChargePoint2ChargePointEdge(ChargePoint                     ChargePointA,
                                       ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                       ChargePoint                     ChargePointB,
                                       PrivacyLevel                  PrivacyLevel  = PrivacyLevel.Private,
                                       DateTime?                     Created       = null)

            : base(ChargePointA ?? throw new ArgumentNullException(nameof(ChargePointA), "The given charge point must not be null!"),
                   EdgeLabel,
                   ChargePointB ?? throw new ArgumentNullException(nameof(ChargePointB), "The given charge point must not be null!"),
                   PrivacyLevel,
                   Created)

        { }

    }

}
