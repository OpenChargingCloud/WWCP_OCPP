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

namespace cloud.charging.open.protocols.OCPPv2_1.CMS
{

    public class LinkChargingStationsResult : AResult<ChargingStation, ChargingStation>
    {

        public ChargingStation                        ChargingStationOut
            => Object1;

        public ChargingStation2ChargingStationEdgeLabel  EdgeLabel       { get; }

        public ChargingStation                        ChargingStationIn
            => Object2;


        public LinkChargingStationsResult(ChargingStation                        ChargingStationOut,
                                       ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                       ChargingStation                        ChargingStationIn,
                                       EventTracking_Id                    EventTrackingId,
                                       Boolean                             IsSuccess,
                                       String                              Argument           = null,
                                       I18NString                          ErrorDescription   = null)

            : base(ChargingStationOut,
                   ChargingStationIn,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.EdgeLabel = EdgeLabel;

        }


        public static LinkChargingStationsResult Success(ChargingStation                        ChargingStationOut,
                                                      ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                                      ChargingStation                        ChargingStationIn,
                                                      EventTracking_Id                    EventTrackingId)

            => new LinkChargingStationsResult(ChargingStationOut,
                                           EdgeLabel,
                                           ChargingStationIn,
                                           EventTrackingId,
                                           true);


        public static LinkChargingStationsResult ArgumentError(ChargingStation                        ChargingStationOut,
                                                            ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                                            ChargingStation                        ChargingStationIn,
                                                            EventTracking_Id                    EventTrackingId,
                                                            String                              Argument,
                                                            String                              Description)

            => new LinkChargingStationsResult(ChargingStationOut,
                                           EdgeLabel,
                                           ChargingStationIn,
                                           EventTrackingId,
                                           false,
                                           Argument,
                                           I18NString.Create(Languages.en,
                                                             Description));

        public static LinkChargingStationsResult ArgumentError(ChargingStation                        ChargingStationOut,
                                                            ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                                            ChargingStation                        ChargingStationIn,
                                                            EventTracking_Id                    EventTrackingId,
                                                            String                              Argument,
                                                            I18NString                          Description)

            => new LinkChargingStationsResult(ChargingStationOut,
                                           EdgeLabel,
                                           ChargingStationIn,
                                           EventTrackingId,
                                           false,
                                           Argument,
                                           Description);


        public static LinkChargingStationsResult Failed(ChargingStation                        ChargingStationOut,
                                                     ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                                     ChargingStation                        ChargingStationIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     String                              Description)

            => new LinkChargingStationsResult(ChargingStationOut,
                                           EdgeLabel,
                                           ChargingStationIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           I18NString.Create(Languages.en,
                                                             Description));

        public static LinkChargingStationsResult Failed(ChargingStation                        ChargingStationOut,
                                                     ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                                     ChargingStation                        ChargingStationIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     I18NString                          Description)

            => new LinkChargingStationsResult(ChargingStationOut,
                                           EdgeLabel,
                                           ChargingStationIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           Description);

        public static LinkChargingStationsResult Failed(ChargingStation                        ChargingStationOut,
                                                     ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                                     ChargingStation                        ChargingStationIn,
                                                     EventTracking_Id                    EventTrackingId,
                                                     Exception                           Exception)

            => new LinkChargingStationsResult(ChargingStationOut,
                                           EdgeLabel,
                                           ChargingStationIn,
                                           EventTrackingId,
                                           false,
                                           null,
                                           I18NString.Create(Languages.en,
                                                             Exception.Message));

    }

}
