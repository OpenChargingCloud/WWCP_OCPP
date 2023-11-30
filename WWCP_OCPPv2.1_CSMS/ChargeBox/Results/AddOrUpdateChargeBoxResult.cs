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

    /// <summary>
    /// The result of an add or update charging station request.
    /// </summary>
    public class AddOrUpdateChargeBoxResult : AEnitityResult<ChargeBox, ChargingStation_Id>
    {

        #region Properties

        public ChargeBox?       ChargeBox
            => Entity;

        public AddedOrUpdated?  AddedOrUpdated    { get; internal set; }

        #endregion

        #region Constructor(s)

        public AddOrUpdateChargeBoxResult(ChargeBox              ChargeBox,
                                          CommandResult          Result,
                                          EventTracking_Id?      EventTrackingId   = null,
                                          IId?                   SenderId          = null,
                                          Object?                Sender            = null,
                                          AddedOrUpdated?        AddedOrUpdated    = null,
                                          I18NString?            Description       = null,
                                          IEnumerable<Warning>?  Warnings          = null,
                                          TimeSpan?              Runtime           = null)

            : base(ChargeBox,
                   Result,
                   EventTrackingId,
                   SenderId,
                   Sender,
                   Description,
                   Warnings,
                   Runtime)

        {

            this.AddedOrUpdated = AddedOrUpdated;

        }

        #endregion


        #region (static) AdminDown    (ChargeBox, ...)

        public static AddOrUpdateChargeBoxResult

            AdminDown(ChargeBox              ChargeBox,
                      EventTracking_Id?      EventTrackingId   = null,
                      IId?                   SenderId          = null,
                      Object?                Sender            = null,
                      I18NString?            Description       = null,
                      IEnumerable<Warning>?  Warnings          = null,
                      TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.AdminDown,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.NoOperation,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) NoOperation  (ChargeBox, ...)

        public static AddOrUpdateChargeBoxResult

            NoOperation(ChargeBox              ChargeBox,
                        EventTracking_Id?      EventTrackingId   = null,
                        IId?                   SenderId          = null,
                        Object?                Sender            = null,
                        I18NString?            Description       = null,
                        IEnumerable<Warning>?  Warnings          = null,
                        TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.NoOperation,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.NoOperation,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) Enqueued     (ChargeBox, ...)

        public static AddOrUpdateChargeBoxResult

            Enqueued(ChargeBox              ChargeBox,
                     EventTracking_Id?      EventTrackingId   = null,
                     IId?                   SenderId          = null,
                     Object?                Sender            = null,
                     I18NString?            Description       = null,
                     IEnumerable<Warning>?  Warnings          = null,
                     TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.Enqueued,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Enqueued,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Added        (ChargeBox, ...)

        public static AddOrUpdateChargeBoxResult

            Added(ChargeBox              ChargeBox,
                  EventTracking_Id?      EventTrackingId   = null,
                  IId?                   SenderId          = null,
                  Object?                Sender            = null,
                  I18NString?            Description       = null,
                  IEnumerable<Warning>?  Warnings          = null,
                  TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.Success,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Add,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Updated      (ChargeBox, ...)

        public static AddOrUpdateChargeBoxResult

            Updated(ChargeBox              ChargeBox,
                    EventTracking_Id?      EventTrackingId   = null,
                    IId?                   SenderId          = null,
                    Object?                Sender            = null,
                    I18NString?            Description       = null,
                    IEnumerable<Warning>?  Warnings          = null,
                    TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.Success,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Update,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) ArgumentError(ChargeBox, Description, ...)

        public static AddOrUpdateChargeBoxResult

            ArgumentError(ChargeBox              ChargeBox,
                          I18NString             Description,
                          EventTracking_Id?      EventTrackingId   = null,
                          IId?                   SenderId          = null,
                          Object?                Sender            = null,
                          IEnumerable<Warning>?  Warnings          = null,
                          TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.ArgumentError,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Failed,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargeBox, Description, ...)

        public static AddOrUpdateChargeBoxResult

            Error(ChargeBox              ChargeBox,
                  I18NString             Description,
                  EventTracking_Id?      EventTrackingId   = null,
                  IId?                   SenderId          = null,
                  Object?                Sender            = null,
                  IEnumerable<Warning>?  Warnings          = null,
                  TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Failed,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargeBox, Exception,   ...)

        public static AddOrUpdateChargeBoxResult

            Error(ChargeBox              ChargeBox,
                  Exception              Exception,
                  EventTracking_Id?      EventTrackingId   = null,
                  IId?                   SenderId          = null,
                  Object?                Sender            = null,
                  IEnumerable<Warning>?  Warnings          = null,
                  TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Failed,
                        Exception.Message.ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Timeout      (ChargeBox, Timeout,     ...)

        public static AddOrUpdateChargeBoxResult

            Timeout(ChargeBox              ChargeBox,
                    TimeSpan               Timeout,
                    EventTracking_Id?      EventTrackingId   = null,
                    IId?                   SenderId          = null,
                    Object?                Sender            = null,
                    IEnumerable<Warning>?  Warnings          = null,
                    TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.Timeout,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Failed,
                        $"Timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) LockTimeout  (ChargeBox, Timeout,     ...)

        public static AddOrUpdateChargeBoxResult

            LockTimeout(ChargeBox              ChargeBox,
                        TimeSpan               Timeout,
                        EventTracking_Id?      EventTrackingId   = null,
                        IId?                   SenderId          = null,
                        Object?                Sender            = null,
                        IEnumerable<Warning>?  Warnings          = null,
                        TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        CommandResult.LockTimeout,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        org.GraphDefined.Vanaheimr.Illias.AddedOrUpdated.Failed,
                        $"Lock timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion


    }

}
