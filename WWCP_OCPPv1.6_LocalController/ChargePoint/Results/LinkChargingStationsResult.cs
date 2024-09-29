/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    public class LinkChargePointsResult : AResult<ChargePoint, ChargePoint>
    {

        public ChargePoint                        ChargePointOut
            => Object1;

        public ChargePoint2ChargePointEdgeLabel  EdgeLabel       { get; }

        public ChargePoint                        ChargePointIn
            => Object2;


        public LinkChargePointsResult(ChargePoint                        ChargePointOut,
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


        public static LinkChargePointsResult Success(ChargePoint                        ChargePointOut,
                                                      ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                      ChargePoint                        ChargePointIn,
                                                      EventTracking_Id                    EventTrackingId)

            => new LinkChargePointsResult(ChargePointOut,
                                           EdgeLabel,
                                           ChargePointIn,
                                           EventTrackingId,
                                           true);


        public static LinkChargePointsResult ArgumentError(ChargePoint                        ChargePointOut,
                                                            ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                            ChargePoint                        ChargePointIn,
                                                            EventTracking_Id                    EventTrackingId,
                                                            String                              Argument,
                                                            String                              Description)

            => new LinkChargePointsResult(ChargePointOut,
                                           EdgeLabel,
                                           ChargePointIn,
                                           EventTrackingId,
                                           false,
                                           Argument,
                                           I18NString.Create(Languages.en,
                                                             Description));

        public static LinkChargePointsResult ArgumentError(ChargePoint                        ChargePointOut,
                                                            ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                            ChargePoint                        ChargePointIn,
                                                            EventTracking_Id                    EventTrackingId,
                                                            String                              Argument,
                                                            I18NString                          Description)

            => new LinkChargePointsResult(ChargePointOut,
                                           EdgeLabel,
                                           ChargePointIn,
                                           EventTrackingId,
                                           false,
                                           Argument,
                                           Description);


        public static LinkChargePointsResult Failed(ChargePoint                        ChargePointOut,
                                                     ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                     ChargePoint                        ChargePointIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     String                              Description)

            => new LinkChargePointsResult(ChargePointOut,
                                           EdgeLabel,
                                           ChargePointIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           I18NString.Create(Languages.en,
                                                             Description));

        public static LinkChargePointsResult Failed(ChargePoint                        ChargePointOut,
                                                     ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                     ChargePoint                        ChargePointIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     I18NString                          Description)

            => new LinkChargePointsResult(ChargePointOut,
                                           EdgeLabel,
                                           ChargePointIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           Description);

        public static LinkChargePointsResult Failed(ChargePoint                        ChargePointOut,
                                                     ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                                     ChargePoint                        ChargePointIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     Exception                           Exception)

            => new LinkChargePointsResult(ChargePointOut,
                                           EdgeLabel,
                                           ChargePointIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           I18NString.Create(Languages.en,
                                                             Exception.Message));

    }

}
