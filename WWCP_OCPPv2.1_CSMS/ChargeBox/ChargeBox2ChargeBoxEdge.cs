/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    [Flags]
    public enum ChargeBox2ChargeBoxEdgeLabel
    {
        IsSubsidary,
        IsParent,
        IsChildOf,
    }


    public class ChargeBox2ChargeBoxEdge : MiniEdge<ChargeBox, ChargeBox2ChargeBoxEdgeLabel, ChargeBox>
    {

        /// <summary>
        /// Create a new miniedge.
        /// </summary>
        /// <param name="ChargeBoxA">The source of the edge.</param>
        /// <param name="EdgeLabel">The label of the edge.</param>
        /// <param name="ChargeBoxB">The target of the edge</param>
        /// <param name="PrivacyLevel">The level of privacy of this edge.</param>
        /// <param name="Created">The creation timestamp of the miniedge.</param>
        public ChargeBox2ChargeBoxEdge(ChargeBox                     ChargeBoxA,
                                       ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                       ChargeBox                     ChargeBoxB,
                                       PrivacyLevel                  PrivacyLevel  = PrivacyLevel.Private,
                                       DateTime?                     Created       = null)

            : base(ChargeBoxA ?? throw new ArgumentNullException(nameof(ChargeBoxA), "The given charging station must not be null!"),
                   EdgeLabel,
                   ChargeBoxB ?? throw new ArgumentNullException(nameof(ChargeBoxB), "The given charging station must not be null!"),
                   PrivacyLevel,
                   Created)

        { }

    }

}
