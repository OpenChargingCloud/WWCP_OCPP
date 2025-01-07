/*UpdateChargeBoxAccessResult
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

namespace cloud.charging.open.protocols.OCPPv1_6.CSMS
{

    /// <summary>
    /// The result of an update charge box access request.
    /// </summary>
    public class UpdateChargeBoxAccessResult : AEnitityResult<ChargeBox, ChargeBox_Id>
    {

        #region Properties

        public ChargeBox?            ChargeBox
            => Entity;

        public ChargeBoxAccessTypes  ChargeBoxAccessType    { get; }

        #endregion

        #region Constructor(s)

        public UpdateChargeBoxAccessResult(ChargeBox              ChargeBox,
                                           ChargeBoxAccessTypes   ChargeBoxAccessType,
                                           CommandResult          Result,
                                           EventTracking_Id?      EventTrackingId   = null,
                                           IId?                   SenderId          = null,
                                           Object?                Sender            = null,
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

            this.ChargeBoxAccessType = ChargeBoxAccessType;

        }


        public UpdateChargeBoxAccessResult(ChargeBox_Id           ChargeBoxId,
                                           ChargeBoxAccessTypes   ChargeBoxAccessType,
                                           CommandResult          Result,
                                           EventTracking_Id?      EventTrackingId   = null,
                                           IId?                   SenderId          = null,
                                           Object?                Sender            = null,
                                           I18NString?            Description       = null,
                                           IEnumerable<Warning>?  Warnings          = null,
                                           TimeSpan?              Runtime           = null)

            : base(ChargeBoxId,
                   Result,
                   EventTrackingId,
                   SenderId,
                   Sender,
                   Description,
                   Warnings,
                   Runtime)

        {

            this.ChargeBoxAccessType = ChargeBoxAccessType;

        }

        #endregion


        #region (static) AdminDown    (ChargeBox,   ...)

        public static UpdateChargeBoxAccessResult

            AdminDown(ChargeBox              ChargeBox,
                      ChargeBoxAccessTypes   ChargeBoxAccessType,
                      EventTracking_Id?      EventTrackingId   = null,
                      IId?                   SenderId          = null,
                      Object?                Sender            = null,
                      I18NString?            Description       = null,
                      IEnumerable<Warning>?  Warnings          = null,
                      TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.AdminDown,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) NoOperation  (ChargeBox,   ...)

        public static UpdateChargeBoxAccessResult

            NoOperation(ChargeBox              ChargeBox,
                        ChargeBoxAccessTypes   ChargeBoxAccessType,
                        EventTracking_Id?      EventTrackingId   = null,
                        IId?                   SenderId          = null,
                        Object?                Sender            = null,
                        I18NString?            Description       = null,
                        IEnumerable<Warning>?  Warnings          = null,
                        TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.NoOperation,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) Enqueued     (ChargeBox,   ...)

        public static UpdateChargeBoxAccessResult

            Enqueued(ChargeBox              ChargeBox,
                     ChargeBoxAccessTypes   ChargeBoxAccessType,
                     EventTracking_Id?      EventTrackingId   = null,
                     IId?                   SenderId          = null,
                     Object?                Sender            = null,
                     I18NString?            Description       = null,
                     IEnumerable<Warning>?  Warnings          = null,
                     TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.Enqueued,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Success      (ChargeBox,   ...)

        public static UpdateChargeBoxAccessResult

            Success(ChargeBox              ChargeBox,
                    ChargeBoxAccessTypes   ChargeBoxAccessType,
                    EventTracking_Id?      EventTrackingId   = null,
                    IId?                   SenderId          = null,
                    Object?                Sender            = null,
                    I18NString?            Description       = null,
                    IEnumerable<Warning>?  Warnings          = null,
                    TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.Success,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) ArgumentError(ChargeBox,   Description, ...)

        public static UpdateChargeBoxAccessResult

            ArgumentError(ChargeBox              ChargeBox,
                          ChargeBoxAccessTypes   ChargeBoxAccessType,
                          I18NString             Description,
                          EventTracking_Id?      EventTrackingId   = null,
                          IId?                   SenderId          = null,
                          Object?                Sender            = null,
                          IEnumerable<Warning>?  Warnings          = null,
                          TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.ArgumentError,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) ArgumentError(ChargeBoxId, Description, ...)

        public static UpdateChargeBoxAccessResult

            ArgumentError(ChargeBox_Id           ChargeBoxId,
                          ChargeBoxAccessTypes   ChargeBoxAccessType,
                          I18NString             Description,
                          EventTracking_Id?      EventTrackingId   = null,
                          IId?                   SenderId          = null,
                          Object?                Sender            = null,
                          IEnumerable<Warning>?  Warnings          = null,
                          TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.ArgumentError,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargeBox,   Description, ...)

        public static UpdateChargeBoxAccessResult

            Error(ChargeBox              ChargeBox,
                  ChargeBoxAccessTypes   ChargeBoxAccessType,
                  I18NString             Description,
                  EventTracking_Id?      EventTrackingId   = null,
                  IId?                   SenderId          = null,
                  Object?                Sender            = null,
                  IEnumerable<Warning>?  Warnings          = null,
                  TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargeBox,   Exception,   ...)

        public static UpdateChargeBoxAccessResult

            Error(ChargeBox              ChargeBox,
                  ChargeBoxAccessTypes   ChargeBoxAccessType,
                  Exception              Exception,
                  EventTracking_Id?      EventTrackingId   = null,
                  IId?                   SenderId          = null,
                  Object?                Sender            = null,
                  IEnumerable<Warning>?  Warnings          = null,
                  TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Exception.Message.ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Timeout      (ChargeBox,   Timeout,     ...)

        public static UpdateChargeBoxAccessResult

            Timeout(ChargeBox              ChargeBox,
                    ChargeBoxAccessTypes   ChargeBoxAccessType,
                    TimeSpan               Timeout,
                    EventTracking_Id?      EventTrackingId   = null,
                    IId?                   SenderId          = null,
                    Object?                Sender            = null,
                    IEnumerable<Warning>?  Warnings          = null,
                    TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
                        CommandResult.Timeout,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        $"Timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) LockTimeout  (ChargeBox,   Timeout,     ...)

        public static UpdateChargeBoxAccessResult

            LockTimeout(ChargeBox              ChargeBox,
                        ChargeBoxAccessTypes   ChargeBoxAccessType,
                        TimeSpan               Timeout,
                        EventTracking_Id?      EventTrackingId   = null,
                        IId?                   SenderId          = null,
                        Object?                Sender            = null,
                        IEnumerable<Warning>?  Warnings          = null,
                        TimeSpan?              Runtime           = null)

                => new (ChargeBox,
                        ChargeBoxAccessType,
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
