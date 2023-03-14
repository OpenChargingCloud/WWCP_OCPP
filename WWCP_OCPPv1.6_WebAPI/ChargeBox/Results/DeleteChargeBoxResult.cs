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

using System;

using org.GraphDefined.Vanaheimr.Illias;
using social.OpenData.UsersAPI;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    public class DeleteChargeBoxResult : AResult<ChargeBox>
    {

        private DeleteChargeBoxResult(ChargeBox         ChargeBox,
                                      EventTracking_Id  EventTrackingId,
                                      Boolean           IsSuccess,
                                      String            Argument           = null,
                                      I18NString        ErrorDescription   = null)

            : base(ChargeBox,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static DeleteChargeBoxResult Success(ChargeBox      ChargeBox,
                                                       EventTracking_Id  EventTrackingId)

            => new DeleteChargeBoxResult(ChargeBox,
                                            EventTrackingId,
                                            true);


        public static DeleteChargeBoxResult ArgumentError(ChargeBox      ChargeBox,
                                                             EventTracking_Id  EventTrackingId,
                                                             String            Argument,
                                                             String            Description)

            => new DeleteChargeBoxResult(ChargeBox,
                                            EventTrackingId,
                                            false,
                                            Argument,
                                            I18NString.Create(Languages.en,
                                                              Description));

        public static DeleteChargeBoxResult ArgumentError(ChargeBox      ChargeBox,
                                                             EventTracking_Id  EventTrackingId,
                                                             String            Argument,
                                                             I18NString        Description)

            => new DeleteChargeBoxResult(ChargeBox,
                                            EventTrackingId,
                                            false,
                                            Argument,
                                            Description);


        public static DeleteChargeBoxResult Failed(ChargeBox      ChargeBox,
                                                      EventTracking_Id  EventTrackingId,
                                                      String            Description)

            => new DeleteChargeBoxResult(ChargeBox,
                                            EventTrackingId,
                                            false,
                                            null,
                                            I18NString.Create(Languages.en,
                                                              Description));

        public static DeleteChargeBoxResult Failed(ChargeBox      ChargeBox,
                                                      EventTracking_Id  EventTrackingId,
                                                      I18NString        Description)

            => new DeleteChargeBoxResult(ChargeBox,
                                            EventTrackingId,
                                            false,
                                            null,
                                            Description);

        public static DeleteChargeBoxResult Failed(ChargeBox      ChargeBox,
                                                      EventTracking_Id  EventTrackingId,
                                                      Exception         Exception)

            => new DeleteChargeBoxResult(ChargeBox,
                                            EventTrackingId,
                                            false,
                                            null,
                                            I18NString.Create(Languages.en,
                                                              Exception.Message));

    }

}
