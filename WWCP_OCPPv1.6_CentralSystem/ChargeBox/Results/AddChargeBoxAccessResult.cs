/*AddChargeBoxAccessResult
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
    /// The result of an add charge box access request.
    /// </summary>
    public class AddChargeBoxAccessResult// : AEnitityResult<ChargeBox, ChargeBox_Id>
    {

        #region Properties

        public ChargeBox_Id          ChargeBoxId            { get; }

        public ChargeBoxAccessTypes  ChargeBoxAccessType    { get; }



        /// <summary>
        /// The result of the command.
        /// </summary>
        public  CommandResult         Result             { get; }

        /// <summary>
        /// The optional unqiue identification of the sender.
        /// </summary>
        public  IId?                  SenderId           { get; }

        /// <summary>
        /// The optional sender of this result.
        /// </summary>
        public  Object?               Sender             { get; }

        /// <summary>
        /// The unique event tracking identification for correlating this request with other events.
        /// </summary>
        public  EventTracking_Id      EventTrackingId    { get; }

        /// <summary>
        /// An optional description of the result.
        /// </summary>
        public  I18NString            Description        { get; }

        /// <summary>
        /// Optional warnings.
        /// </summary>
        public  IEnumerable<Warning>  Warnings           { get; }

        /// <summary>
        /// The runtime of the command till this result.
        /// </summary>
        public  TimeSpan              Runtime            { get;  }

        #endregion

        #region Constructor(s)

        public AddChargeBoxAccessResult(ChargeBox_Id           ChargeBoxId,
                                        ChargeBoxAccessTypes   ChargeBoxAccessType,
                                        CommandResult          Result,
                                        EventTracking_Id?      EventTrackingId   = null,
                                        IId?                   SenderId          = null,
                                        Object?                Sender            = null,
                                        I18NString?            Description       = null,
                                        IEnumerable<Warning>?  Warnings          = null,
                                        TimeSpan?              Runtime           = null)
        {

            this.ChargeBoxId          = ChargeBoxId;
            this.ChargeBoxAccessType  = ChargeBoxAccessType;
            this.Result               = Result;
            this.EventTrackingId      = EventTrackingId ?? EventTracking_Id.New;
            this.SenderId             = SenderId;
            this.Sender               = Sender;
            this.Description          = Description     ?? I18NString.Empty;
            this.Warnings             = Warnings        ?? [];
            this.Runtime              = Runtime         ?? TimeSpan.Zero;

        }

        #endregion


        #region (static) AdminDown    (ChargeBoxId,   ...)

        public static AddChargeBoxAccessResult

            AdminDown(ChargeBox_Id           ChargeBoxId,
                      ChargeBoxAccessTypes   ChargeBoxAccessType,
                      EventTracking_Id?      EventTrackingId   = null,
                      IId?                   SenderId          = null,
                      Object?                Sender            = null,
                      I18NString?            Description       = null,
                      IEnumerable<Warning>?  Warnings          = null,
                      TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.AdminDown,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) NoOperation  (ChargeBoxId,   ...)

        public static AddChargeBoxAccessResult

            NoOperation(ChargeBox_Id           ChargeBoxId,
                        ChargeBoxAccessTypes   ChargeBoxAccessType,
                        EventTracking_Id?      EventTrackingId   = null,
                        IId?                   SenderId          = null,
                        Object?                Sender            = null,
                        I18NString?            Description       = null,
                        IEnumerable<Warning>?  Warnings          = null,
                        TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.NoOperation,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) Enqueued     (ChargeBoxId,   ...)

        public static AddChargeBoxAccessResult

            Enqueued(ChargeBox_Id           ChargeBoxId,
                     ChargeBoxAccessTypes   ChargeBoxAccessType,
                     EventTracking_Id?      EventTrackingId   = null,
                     IId?                   SenderId          = null,
                     Object?                Sender            = null,
                     I18NString?            Description       = null,
                     IEnumerable<Warning>?  Warnings          = null,
                     TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.Enqueued,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Success      (ChargeBoxId,   ...)

        public static AddChargeBoxAccessResult

            Success(ChargeBox_Id           ChargeBoxId,
                    ChargeBoxAccessTypes   ChargeBoxAccessType,
                    EventTracking_Id?      EventTrackingId   = null,
                    IId?                   SenderId          = null,
                    Object?                Sender            = null,
                    I18NString?            Description       = null,
                    IEnumerable<Warning>?  Warnings          = null,
                    TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.Success,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion


        #region (static) ArgumentError(ChargeBoxId, Description, ...)

        public static AddChargeBoxAccessResult

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

        #region (static) Error        (ChargeBoxId, Description, ...)

        public static AddChargeBoxAccessResult

            Error(ChargeBox_Id           ChargeBoxId,
                  ChargeBoxAccessTypes   ChargeBoxAccessType,
                  I18NString             Description,
                  EventTracking_Id?      EventTrackingId   = null,
                  IId?                   SenderId          = null,
                  Object?                Sender            = null,
                  IEnumerable<Warning>?  Warnings          = null,
                  TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Description,
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Error        (ChargeBoxId, Exception,   ...)

        public static AddChargeBoxAccessResult

            Error(ChargeBox_Id           ChargeBoxId,
                  ChargeBoxAccessTypes   ChargeBoxAccessType,
                  Exception              Exception,
                  EventTracking_Id?      EventTrackingId   = null,
                  IId?                   SenderId          = null,
                  Object?                Sender            = null,
                  IEnumerable<Warning>?  Warnings          = null,
                  TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.Error,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        Exception.Message.ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) Timeout      (ChargeBoxId, Timeout,     ...)

        public static AddChargeBoxAccessResult

            Timeout(ChargeBox_Id           ChargeBoxId,
                    ChargeBoxAccessTypes   ChargeBoxAccessType,
                    TimeSpan               Timeout,
                    EventTracking_Id?      EventTrackingId   = null,
                    IId?                   SenderId          = null,
                    Object?                Sender            = null,
                    IEnumerable<Warning>?  Warnings          = null,
                    TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
                        ChargeBoxAccessType,
                        CommandResult.Timeout,
                        EventTrackingId,
                        SenderId,
                        Sender,
                        $"Timeout after {Timeout.TotalSeconds} seconds!".ToI18NString(),
                        Warnings,
                        Runtime);

        #endregion

        #region (static) LockTimeout  (ChargeBoxId, Timeout,     ...)

        public static AddChargeBoxAccessResult

            LockTimeout(ChargeBox_Id           ChargeBoxId,
                        ChargeBoxAccessTypes   ChargeBoxAccessType,
                        TimeSpan               Timeout,
                        EventTracking_Id?      EventTrackingId   = null,
                        IId?                   SenderId          = null,
                        Object?                Sender            = null,
                        IEnumerable<Warning>?  Warnings          = null,
                        TimeSpan?              Runtime           = null)

                => new (ChargeBoxId,
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
