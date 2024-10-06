/*UpdateChargingStationAccessResult
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The result of an update charging station access request.
    /// </summary>
    public class UpdateChargingStationAccessResult : AEnitityResult<ChargingStation, ChargingStation_Id>
    {

        #region Properties

        public ChargingStation?            ChargingStation
            => Entity;

        public ChargingStationAccessTypes  ChargingStationAccessType    { get; }

        #endregion

        #region Constructor(s)

        public UpdateChargingStationAccessResult(ChargingStation             ChargingStation,
                                                 ChargingStationAccessTypes  ChargingStationAccessType,
                                                 CommandResult               Result,
                                                 EventTracking_Id?           EventTrackingId   = null,
                                                 IId?                        SenderId          = null,
                                                 Object?                     Sender            = null,
                                                 I18NString?                 Description       = null,
                                                 IEnumerable<Warning>?       Warnings          = null,
                                                 TimeSpan?                   Runtime           = null)

            : base(ChargingStation,
                   Result,
                   EventTrackingId,
                   SenderId,
                   Sender,
                   Description,
                   Warnings,
                   Runtime)

        {

            this.ChargingStationAccessType = ChargingStationAccessType;

        }


        public UpdateChargingStationAccessResult(ChargingStation_Id          ChargingStationId,
                                                 ChargingStationAccessTypes  ChargingStationAccessType,
                                                 CommandResult               Result,
                                                 EventTracking_Id?           EventTrackingId   = null,
                                                 IId?                        SenderId          = null,
                                                 Object?                     Sender            = null,
                                                 I18NString?                 Description       = null,
                                                 IEnumerable<Warning>?       Warnings          = null,
                                                 TimeSpan?                   Runtime           = null)

            : base(ChargingStationId,
                   Result,
                   EventTrackingId,
                   SenderId,
                   Sender,
                   Description,
                   Warnings,
                   Runtime)

        {

            this.ChargingStationAccessType = ChargingStationAccessType;

        }

        #endregion


        #region (static) AdminDown    (ChargingStation,   ...)

        public static UpdateChargingStationAccessResult

            AdminDown(ChargingStation             ChargingStation,
                      ChargingStationAccessTypes  ChargingStationAccessType,
                      EventTracking_Id?           EventTrackingId   = null,
                      IId?                        SenderId          = null,
                      Object?                     Sender            = null,
                      I18NString?                 Description       = null,
                      IEnumerable<Warning>?       Warnings          = null,
                      TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.AdminDown,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) NoOperation  (ChargingStation,   ...)

        public static UpdateChargingStationAccessResult

            NoOperation(ChargingStation             ChargingStation,
                        ChargingStationAccessTypes  ChargingStationAccessType,
                        EventTracking_Id?           EventTrackingId   = null,
                        IId?                        SenderId          = null,
                        Object?                     Sender            = null,
                        I18NString?                 Description       = null,
                        IEnumerable<Warning>?       Warnings          = null,
                        TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.NoOperation,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) Enqueued     (ChargingStation,   ...)

        public static UpdateChargingStationAccessResult

            Enqueued(ChargingStation             ChargingStation,
                     ChargingStationAccessTypes  ChargingStationAccessType,
                     EventTracking_Id?           EventTrackingId   = null,
                     IId?                        SenderId          = null,
                     Object?                     Sender            = null,
                     I18NString?                 Description       = null,
                     IEnumerable<Warning>?       Warnings          = null,
                     TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.Enqueued,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Success      (ChargingStation,   ...)

        public static UpdateChargingStationAccessResult

            Success(ChargingStation             ChargingStation,
                    ChargingStationAccessTypes  ChargingStationAccessType,
                    EventTracking_Id?           EventTrackingId   = null,
                    IId?                        SenderId          = null,
                    Object?                     Sender            = null,
                    I18NString?                 Description       = null,
                    IEnumerable<Warning>?       Warnings          = null,
                    TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.Success,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) ArgumentError(ChargingStation,   Description, ...)

        public static UpdateChargingStationAccessResult

            ArgumentError(ChargingStation             ChargingStation,
                          ChargingStationAccessTypes  ChargingStationAccessType,
                          I18NString                  Description,
                          EventTracking_Id?           EventTrackingId   = null,
                          IId?                        SenderId          = null,
                          Object?                     Sender            = null,
                          IEnumerable<Warning>?       Warnings          = null,
                          TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.ArgumentError,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) ArgumentError(ChargingStationId, Description, ...)

        public static UpdateChargingStationAccessResult

            ArgumentError(ChargingStation_Id          ChargingStationId,
                          ChargingStationAccessTypes  ChargingStationAccessType,
                          I18NString                  Description,
                          EventTracking_Id?           EventTrackingId   = null,
                          IId?                        SenderId          = null,
                          Object?                     Sender            = null,
                          IEnumerable<Warning>?       Warnings          = null,
                          TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.ArgumentError,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargingStation,   Description, ...)

        public static UpdateChargingStationAccessResult

            Error(ChargingStation             ChargingStation,
                  ChargingStationAccessTypes  ChargingStationAccessType,
                  I18NString                  Description,
                  EventTracking_Id?           EventTrackingId   = null,
                  IId?                        SenderId          = null,
                  Object?                     Sender            = null,
                  IEnumerable<Warning>?       Warnings          = null,
                  TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargingStation,   Exception,   ...)

        public static UpdateChargingStationAccessResult

            Error(ChargingStation             ChargingStation,
                  ChargingStationAccessTypes  ChargingStationAccessType,
                  Exception                   Exception,
                  EventTracking_Id?           EventTrackingId   = null,
                  IId?                        SenderId          = null,
                  Object?                     Sender            = null,
                  IEnumerable<Warning>?       Warnings          = null,
                  TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Exception.Message.ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Timeout      (ChargingStation,   Timeout,     ...)

        public static UpdateChargingStationAccessResult

            Timeout(ChargingStation             ChargingStation,
                    ChargingStationAccessTypes  ChargingStationAccessType,
                    TimeSpan                    Timeout,
                    EventTracking_Id?           EventTrackingId   = null,
                    IId?                        SenderId          = null,
                    Object?                     Sender            = null,
                    IEnumerable<Warning>?       Warnings          = null,
                    TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.Timeout,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        $"Timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) LockTimeout  (ChargingStation,   Timeout,     ...)

        public static UpdateChargingStationAccessResult

            LockTimeout(ChargingStation             ChargingStation,
                        ChargingStationAccessTypes  ChargingStationAccessType,
                        TimeSpan                    Timeout,
                        EventTracking_Id?           EventTrackingId   = null,
                        IId?                        SenderId          = null,
                        Object?                     Sender            = null,
                        IEnumerable<Warning>?       Warnings          = null,
                        TimeSpan?                   Runtime           = null)

                => new (ChargingStation,
                        ChargingStationAccessType,
                        CommandResult.LockTimeout,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        $"Lock timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion


    }

}
