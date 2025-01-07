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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    public class UnlinkChargePointsResult : AResult<ChargePoint, ChargePoint>
    {

        public ChargePoint                        ChargePointOut
            => Object1;

        public ChargePoint2ChargePointEdgeLabel  EdgeLabel       { get; }

        public ChargePoint                        ChargePointIn
            => Object2;


        public UnlinkChargePointsResult(ChargePoint                        ChargePointOut,
                                         ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                         ChargePoint                        ChargePointIn,
                                         EventTracking_Id                    EventTrackingId,
                                         Boolean                             IsSuccess,
                                         String                              Argument           = null,
                                         I18NString                          ErrorDescription   = null)

            : base(ChargePointOut,
                   ChargePointIn,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.EdgeLabel = EdgeLabel;

        }


        public static UnlinkChargePointsResult Success(ChargePoint                        ChargePointOut,
                                                        ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                        ChargePoint                        ChargePointIn,
                                                        EventTracking_Id                    EventTrackingId)

            => new UnlinkChargePointsResult(ChargePointOut,
                                             EdgeLabel,
                                             ChargePointIn,
                                             EventTrackingId,
                                             true);


        public static UnlinkChargePointsResult ArgumentError(ChargePoint                        ChargePointOut,
                                                              ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                              ChargePoint                        ChargePointIn,
                                                              EventTracking_Id                    EventTrackingId,
                                                              String                              Argument,
                                                              String                              Description)

            => new UnlinkChargePointsResult(ChargePointOut,
                                             EdgeLabel,
                                             ChargePointIn,
                                             EventTrackingId,
                                             false,
                                             Argument,
                                             I18NString.Create(Languages.en,
                                                               Description));

        public static UnlinkChargePointsResult ArgumentError(ChargePoint                        ChargePointOut,
                                                              ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                              ChargePoint                        ChargePointIn,
                                                              EventTracking_Id                    EventTrackingId,
                                                              String                              Argument,
                                                              I18NString                          Description)

            => new UnlinkChargePointsResult(ChargePointOut,
                                             EdgeLabel,
                                             ChargePointIn,
                                             EventTrackingId,
                                             false,
                                             Argument,
                                             Description);


        public static UnlinkChargePointsResult Failed(ChargePoint                        ChargePointOut,
                                                       ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                       ChargePoint                        ChargePointIn,
                                                       EventTracking_Id                    EventTrackingId,
                                                       String                              Description)

            => new UnlinkChargePointsResult(ChargePointOut,
                                             EdgeLabel,
                                             ChargePointIn,
                                             EventTrackingId,
                                             false,
                                             null,
                                             I18NString.Create(Languages.en,
                                                               Description));

        public static UnlinkChargePointsResult Failed(ChargePoint                        ChargePointOut,
                                                       ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                       ChargePoint                        ChargePointIn,
                                                       EventTracking_Id                    EventTrackingId,
                                                       I18NString                          Description)

            => new UnlinkChargePointsResult(ChargePointOut,
                                             EdgeLabel,
                                             ChargePointIn,
                                             EventTrackingId,
                                             false,
                                             null,
                                             Description);

        public static UnlinkChargePointsResult Failed(ChargePoint                        ChargePointOut,
                                                       ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                       ChargePoint                        ChargePointIn,
                                                       EventTracking_Id                    EventTrackingId,
                                                       Exception                           Exception)

            => new UnlinkChargePointsResult(ChargePointOut,
                                             EdgeLabel,
                                             ChargePointIn,
                                             EventTrackingId,
                                             false,
                                             null,
                                             I18NString.Create(Languages.en,
                                                               Exception.Message));

    }

}
