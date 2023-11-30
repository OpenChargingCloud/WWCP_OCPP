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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    public class LinkChargeBoxsResult : AResult<ChargeBox, ChargeBox>
    {

        public ChargeBox                        ChargeBoxOut
            => Object1;

        public ChargeBox2ChargeBoxEdgeLabel  EdgeLabel       { get; }

        public ChargeBox                        ChargeBoxIn
            => Object2;


        public LinkChargeBoxsResult(ChargeBox                        ChargeBoxOut,
                                       ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                       ChargeBox                        ChargeBoxIn,
                                       EventTracking_Id                    EventTrackingId,
                                       Boolean                             IsSuccess,
                                       String                              Argument           = null,
                                       I18NString                          ErrorDescription   = null)

            : base(ChargeBoxOut,
                   ChargeBoxIn,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.EdgeLabel = EdgeLabel;

        }


        public static LinkChargeBoxsResult Success(ChargeBox                        ChargeBoxOut,
                                                      ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                                      ChargeBox                        ChargeBoxIn,
                                                      EventTracking_Id                    EventTrackingId)

            => new LinkChargeBoxsResult(ChargeBoxOut,
                                           EdgeLabel,
                                           ChargeBoxIn,
                                           EventTrackingId,
                                           true);


        public static LinkChargeBoxsResult ArgumentError(ChargeBox                        ChargeBoxOut,
                                                            ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                                            ChargeBox                        ChargeBoxIn,
                                                            EventTracking_Id                    EventTrackingId,
                                                            String                              Argument,
                                                            String                              Description)

            => new LinkChargeBoxsResult(ChargeBoxOut,
                                           EdgeLabel,
                                           ChargeBoxIn,
                                           EventTrackingId,
                                           false,
                                           Argument,
                                           I18NString.Create(Languages.en,
                                                             Description));

        public static LinkChargeBoxsResult ArgumentError(ChargeBox                        ChargeBoxOut,
                                                            ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                                            ChargeBox                        ChargeBoxIn,
                                                            EventTracking_Id                    EventTrackingId,
                                                            String                              Argument,
                                                            I18NString                          Description)

            => new LinkChargeBoxsResult(ChargeBoxOut,
                                           EdgeLabel,
                                           ChargeBoxIn,
                                           EventTrackingId,
                                           false,
                                           Argument,
                                           Description);


        public static LinkChargeBoxsResult Failed(ChargeBox                        ChargeBoxOut,
                                                     ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                                     ChargeBox                        ChargeBoxIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     String                              Description)

            => new LinkChargeBoxsResult(ChargeBoxOut,
                                           EdgeLabel,
                                           ChargeBoxIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           I18NString.Create(Languages.en,
                                                             Description));

        public static LinkChargeBoxsResult Failed(ChargeBox                        ChargeBoxOut,
                                                     ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                                     ChargeBox                        ChargeBoxIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     I18NString                          Description)

            => new LinkChargeBoxsResult(ChargeBoxOut,
                                           EdgeLabel,
                                           ChargeBoxIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           Description);

        public static LinkChargeBoxsResult Failed(ChargeBox                        ChargeBoxOut,
                                                     ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                                     ChargeBox                        ChargeBoxIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     Exception                           Exception)

            => new LinkChargeBoxsResult(ChargeBoxOut,
                                           EdgeLabel,
                                           ChargeBoxIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           I18NString.Create(Languages.en,
                                                             Exception.Message));

    }

}
