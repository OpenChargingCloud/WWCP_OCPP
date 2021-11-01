/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

    public class AddChargeBoxIfNotExistsResult : AResult<ChargeBox>
    {

        public ChargeBox  ChargeBox
            => Object;

        public ChargeBox     ParentChargeBox   { get; internal set; }

        public AddedOrIgnored?  AddedOrIgnored       { get; internal set; }


        public AddChargeBoxIfNotExistsResult(ChargeBox      ChargeBox,
                                                EventTracking_Id  EventTrackingId,
                                                Boolean           IsSuccess,
                                                String            Argument             = null,
                                                I18NString        ErrorDescription     = null,
                                                ChargeBox      ParentChargeBox   = null,
                                                AddedOrIgnored?   AddedOrIgnored       = null)

            : base(ChargeBox,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.ParentChargeBox  = ParentChargeBox;
            this.AddedOrIgnored      = AddedOrIgnored;

        }


        public static AddChargeBoxIfNotExistsResult Success(ChargeBox      ChargeBox,
                                                               AddedOrIgnored    AddedOrIgnored,
                                                               EventTracking_Id  EventTrackingId,
                                                               ChargeBox      ParentChargeBox  = null)

            => new AddChargeBoxIfNotExistsResult(ChargeBox,
                                                    EventTrackingId,
                                                    true,
                                                    null,
                                                    null,
                                                    ParentChargeBox,
                                                    AddedOrIgnored);


        public static AddChargeBoxIfNotExistsResult ArgumentError(ChargeBox      ChargeBox,
                                                                     EventTracking_Id  EventTrackingId,
                                                                     String            Argument,
                                                                     String            Description)

            => new AddChargeBoxIfNotExistsResult(ChargeBox,
                                                    EventTrackingId,
                                                    false,
                                                    Argument,
                                                    I18NString.Create(Languages.en,
                                                                      Description));

        public static AddChargeBoxIfNotExistsResult ArgumentError(ChargeBox      ChargeBox,
                                                                     EventTracking_Id  EventTrackingId,
                                                                     String            Argument,
                                                                     I18NString        Description)

            => new AddChargeBoxIfNotExistsResult(ChargeBox,
                                                    EventTrackingId,
                                                    false,
                                                    Argument,
                                                    Description);


        public static AddChargeBoxIfNotExistsResult Failed(ChargeBox      ChargeBox,
                                                              EventTracking_Id  EventTrackingId,
                                                              String            Description,
                                                              ChargeBox      ParentChargeBox  = null)

            => new AddChargeBoxIfNotExistsResult(ChargeBox,
                                                    EventTrackingId,
                                                    false,
                                                    null,
                                                    I18NString.Create(Languages.en,
                                                                      Description),
                                                    ParentChargeBox);

        public static AddChargeBoxIfNotExistsResult Failed(ChargeBox      ChargeBox,
                                                              EventTracking_Id  EventTrackingId,
                                                              I18NString        Description,
                                                              ChargeBox      ParentChargeBox  = null)

            => new AddChargeBoxIfNotExistsResult(ChargeBox,
                                                    EventTrackingId,
                                                    false,
                                                    null,
                                                    Description,
                                                    ParentChargeBox);

        public static AddChargeBoxIfNotExistsResult Failed(ChargeBox      ChargeBox,
                                                              EventTracking_Id  EventTrackingId,
                                                              Exception         Exception,
                                                              ChargeBox      ParentChargeBox  = null)

            => new AddChargeBoxIfNotExistsResult(ChargeBox,
                                                    EventTrackingId,
                                                    false,
                                                    null,
                                                    I18NString.Create(Languages.en,
                                                                      Exception.Message),
                                                    ParentChargeBox);

    }

}
