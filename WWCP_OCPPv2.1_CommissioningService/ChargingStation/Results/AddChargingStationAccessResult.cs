/*AddChargingStationAccessResult
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CMS
{

    /// <summary>
    /// The result of an add charging station access request.
    /// </summary>
    public class AddChargingStationAccessResult : AEnitityResult<ChargingStation, ChargingStation_Id>
    {

        #region Properties

        public ChargingStation?            ChargingStation
            => Entity;

        public ChargingStationAccessTypes  ChargingStationAccessType    { get; }

        #endregion

        #region Constructor(s)

        public AddChargingStationAccessResult(ChargingStation_Id          ChargingStationId,
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


        #region (static) AdminDown    (ChargingStationId,   ...)

        public static AddChargingStationAccessResult

            AdminDown(ChargingStation_Id          ChargingStationId,
                      ChargingStationAccessTypes  ChargingStationAccessType,
                      EventTracking_Id?           EventTrackingId   = null,
                      IId?                        SenderId          = null,
                      Object?                     Sender            = null,
                      I18NString?                 Description       = null,
                      IEnumerable<Warning>?       Warnings          = null,
                      TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.AdminDown,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) NoOperation  (ChargingStationId,   ...)

        public static AddChargingStationAccessResult

            NoOperation(ChargingStation_Id          ChargingStationId,
                        ChargingStationAccessTypes  ChargingStationAccessType,
                        EventTracking_Id?           EventTrackingId   = null,
                        IId?                        SenderId          = null,
                        Object?                     Sender            = null,
                        I18NString?                 Description       = null,
                        IEnumerable<Warning>?       Warnings          = null,
                        TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.NoOperation,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) Enqueued     (ChargingStationId,   ...)

        public static AddChargingStationAccessResult

            Enqueued(ChargingStation_Id          ChargingStationId,
                     ChargingStationAccessTypes  ChargingStationAccessType,
                     EventTracking_Id?           EventTrackingId   = null,
                     IId?                        SenderId          = null,
                     Object?                     Sender            = null,
                     I18NString?                 Description       = null,
                     IEnumerable<Warning>?       Warnings          = null,
                     TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.Enqueued,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Success      (ChargingStationId,   ...)

        public static AddChargingStationAccessResult

            Success(ChargingStation_Id          ChargingStationId,
                    ChargingStationAccessTypes  ChargingStationAccessType,
                    EventTracking_Id?           EventTrackingId   = null,
                    IId?                        SenderId          = null,
                    Object?                     Sender            = null,
                    I18NString?                 Description       = null,
                    IEnumerable<Warning>?       Warnings          = null,
                    TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.Success,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) ArgumentError(ChargingStationId, Description, ...)

        public static AddChargingStationAccessResult

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

        #region (static) Error        (ChargingStationId, Description, ...)

        public static AddChargingStationAccessResult

            Error(ChargingStation_Id          ChargingStationId,
                  ChargingStationAccessTypes  ChargingStationAccessType,
                  I18NString                  Description,
                  EventTracking_Id?           EventTrackingId   = null,
                  IId?                        SenderId          = null,
                  Object?                     Sender            = null,
                  IEnumerable<Warning>?       Warnings          = null,
                  TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargingStationId, Exception,   ...)

        public static AddChargingStationAccessResult

            Error(ChargingStation_Id          ChargingStationId,
                  ChargingStationAccessTypes  ChargingStationAccessType,
                  Exception                   Exception,
                  EventTracking_Id?           EventTrackingId   = null,
                  IId?                        SenderId          = null,
                  Object?                     Sender            = null,
                  IEnumerable<Warning>?       Warnings          = null,
                  TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Exception.Message.ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Timeout      (ChargingStationId, Timeout,     ...)

        public static AddChargingStationAccessResult

            Timeout(ChargingStation_Id          ChargingStationId,
                    ChargingStationAccessTypes  ChargingStationAccessType,
                    TimeSpan                    Timeout,
                    EventTracking_Id?           EventTrackingId   = null,
                    IId?                        SenderId          = null,
                    Object?                     Sender            = null,
                    IEnumerable<Warning>?       Warnings          = null,
                    TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
                        ChargingStationAccessType,
                        CommandResult.Timeout,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        $"Timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) LockTimeout  (ChargingStationId, Timeout,     ...)

        public static AddChargingStationAccessResult

            LockTimeout(ChargingStation_Id          ChargingStationId,
                        ChargingStationAccessTypes  ChargingStationAccessType,
                        TimeSpan                    Timeout,
                        EventTracking_Id?           EventTrackingId   = null,
                        IId?                        SenderId          = null,
                        Object?                     Sender            = null,
                        IEnumerable<Warning>?       Warnings          = null,
                        TimeSpan?                   Runtime           = null)

                => new (ChargingStationId,
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
