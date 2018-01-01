/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// OCPP XML I/O.
    /// </summary>
    public static class XML_IO
    {

        #region AsAuthorizationStatus(Text)

        public static AuthorizationStatus AsAuthorizationStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return AuthorizationStatus.Accepted;

                case "Blocked":
                    return AuthorizationStatus.Blocked;

                case "Expired":
                    return AuthorizationStatus.Expired;

                case "Invalid":
                    return AuthorizationStatus.Invalid;

                case "ConcurrentTx":
                    return AuthorizationStatus.ConcurrentTx;


                default:
                    return AuthorizationStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this AuthorizationStatus)

        public static String AsText(this AuthorizationStatus AuthorizationStatus)
        {

            switch (AuthorizationStatus)
            {

                case AuthorizationStatus.Accepted:
                    return "Accepted";

                case AuthorizationStatus.Blocked:
                    return "Blocked";

                case AuthorizationStatus.Expired:
                    return "Expired";

                case AuthorizationStatus.Invalid:
                    return "Invalid";

                case AuthorizationStatus.ConcurrentTx:
                    return "ConcurrentTx";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsUnlockStatus(Text)

        public static UnlockStatus AsUnlockStatus(this String Text)
        {

            switch (Text)
            {

                case "Unlocked":
                    return UnlockStatus.Unlocked;

                case "UnlockFailed":
                    return UnlockStatus.UnlockFailed;

                case "NotSupported":
                    return UnlockStatus.NotSupported;


                default:
                    return UnlockStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this UnlockStatus)

        public static String AsText(this UnlockStatus UnlockStatus)
        {

            switch (UnlockStatus)
            {

                case UnlockStatus.Unlocked:
                    return "Unlocked";

                case UnlockStatus.UnlockFailed:
                    return "UnlockFailed";

                case UnlockStatus.NotSupported:
                    return "NotSupported";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsResetType(Text)

        public static ResetTypes AsResetType(this String Text)
        {

            switch (Text)
            {

                case "Hard":
                    return ResetTypes.Hard;

                case "Soft":
                    return ResetTypes.Soft;


                default:
                    return ResetTypes.Unknown;

            }

        }

        #endregion

        #region AsText(this ResetType)

        public static String AsText(this ResetTypes ResetType)
        {

            switch (ResetType)
            {

                case ResetTypes.Hard:
                    return "Hard";

                case ResetTypes.Soft:
                    return "Soft";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsResetStatus(Text)

        public static ResetStatus AsResetStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ResetStatus.Accepted;

                case "Rejected":
                    return ResetStatus.Rejected;


                default:
                    return ResetStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ResetStatus)

        public static String AsText(this ResetStatus ResetStatus)
        {

            switch (ResetStatus)
            {

                case ResetStatus.Accepted:
                    return "Accepted";

                case ResetStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsAvailabilityType(Text)

        public static AvailabilityTypes AsAvailabilityType(this String Text)
        {

            switch (Text)
            {

                case "Inoperative":
                    return AvailabilityTypes.Inoperative;

                case "Operative":
                    return AvailabilityTypes.Operative;


                default:
                    return AvailabilityTypes.Unknown;

            }

        }

        #endregion

        #region AsText(this AvailabilityType)

        public static String AsText(this AvailabilityTypes AvailabilityType)
        {

            switch (AvailabilityType)
            {

                case AvailabilityTypes.Inoperative:
                    return "Inoperative";

                case AvailabilityTypes.Operative:
                    return "Operative";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsAvailabilityStatus(Text)

        public static AvailabilityStatus AsAvailabilityStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return AvailabilityStatus.Accepted;

                case "Rejected":
                    return AvailabilityStatus.Rejected;

                case "Scheduled":
                    return AvailabilityStatus.Scheduled;


                default:
                    return AvailabilityStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this AvailabilityStatus)

        public static String AsText(this AvailabilityStatus AvailabilityStatus)
        {

            switch (AvailabilityStatus)
            {

                case AvailabilityStatus.Accepted:
                    return "Accepted";

                case AvailabilityStatus.Rejected:
                    return "Rejected";

                case AvailabilityStatus.Scheduled:
                    return "Scheduled";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsClearCacheStatus(Text)

        public static ClearCacheStatus AsClearCacheStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ClearCacheStatus.Accepted;

                case "Rejected":
                    return ClearCacheStatus.Rejected;


                default:
                    return ClearCacheStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ClearCacheStatus)

        public static String AsText(this ClearCacheStatus ClearCacheStatus)
        {

            switch (ClearCacheStatus)
            {

                case ClearCacheStatus.Accepted:
                    return "Accepted";

                case ClearCacheStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsClearChargingProfileStatus(Text)

        public static ClearChargingProfileStatus AsClearChargingProfileStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ClearChargingProfileStatus.Accepted;


                default:
                    return ClearChargingProfileStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ClearChargingProfileStatus)

        public static String AsText(this ClearChargingProfileStatus ClearChargingProfileStatus)
        {

            switch (ClearChargingProfileStatus)
            {

                case ClearChargingProfileStatus.Accepted:
                    return "Accepted";


                default:
                    return "Unknown";

            }

        }

        #endregion


        #region AsConfigurationStatus(Text)

        public static ConfigurationStatus AsConfigurationStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ConfigurationStatus.Accepted;

                case "Rejected":
                    return ConfigurationStatus.Rejected;

                case "RebootRequired":
                    return ConfigurationStatus.RebootRequired;

                case "NotSupported":
                    return ConfigurationStatus.NotSupported;


                default:
                    return ConfigurationStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ConfigurationStatus)

        public static String AsText(this ConfigurationStatus ConfigurationStatus)
        {

            switch (ConfigurationStatus)
            {

                case ConfigurationStatus.Accepted:
                    return "Accepted";

                case ConfigurationStatus.Rejected:
                    return "Rejected";

                case ConfigurationStatus.RebootRequired:
                    return "RebootRequired";

                case ConfigurationStatus.NotSupported:
                    return "NotSupported";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsRemoteStartStopStatus(Text)

        public static RemoteStartStopStatus AsRemoteStartStopStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return RemoteStartStopStatus.Accepted;

                case "Rejected":
                    return RemoteStartStopStatus.Rejected;


                default:
                    return RemoteStartStopStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this RemoteStartStopStatus)

        public static String AsText(this RemoteStartStopStatus RemoteStartStopStatus)
        {

            switch (RemoteStartStopStatus)
            {

                case RemoteStartStopStatus.Accepted:
                    return "Accepted";

                case RemoteStartStopStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsCancelReservationStatus(Text)

        public static CancelReservationStatus AsCancelReservationStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return CancelReservationStatus.Accepted;

                case "Rejected":
                    return CancelReservationStatus.Rejected;


                default:
                    return CancelReservationStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this CancelReservationStatus)

        public static String AsText(this CancelReservationStatus CancelReservationStatus)
        {

            switch (CancelReservationStatus)
            {

                case CancelReservationStatus.Accepted:
                    return "Accepted";

                case CancelReservationStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsDataTransferStatus(Text)

        public static DataTransferStatus AsDataTransferStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return DataTransferStatus.Accepted;

                case "Rejected":
                    return DataTransferStatus.Rejected;

                case "UnknownMessageId":
                    return DataTransferStatus.UnknownMessageId;

                case "UnknownVendorId":
                    return DataTransferStatus.UnknownVendorId;


                default:
                    return DataTransferStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this DataTransferStatus)

        public static String AsText(this DataTransferStatus DataTransferStatus)
        {

            switch (DataTransferStatus)
            {

                case DataTransferStatus.Accepted:
                    return "Accepted";

                case DataTransferStatus.Rejected:
                    return "Rejected";

                case DataTransferStatus.UnknownMessageId:
                    return "UnknownMessageId";

                case DataTransferStatus.UnknownVendorId:
                    return "UnknownVendorId";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsChargingRateUnit(Text)

        public static ChargingRateUnits AsChargingRateUnit(this String Text)
        {

            switch (Text)
            {

                case "A":
                    return ChargingRateUnits.Amperes;

                case "W":
                    return ChargingRateUnits.Watts;


                default:
                    return ChargingRateUnits.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargingRateUnitType)

        public static String AsText(this ChargingRateUnits ChargingRateUnitType)
        {

            switch (ChargingRateUnitType)
            {

                case ChargingRateUnits.Amperes:
                    return "A";

                case ChargingRateUnits.Watts:
                    return "W";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsGetCompositeScheduleStatus(Text)

        public static GetCompositeScheduleStatus AsGetCompositeScheduleStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return GetCompositeScheduleStatus.Accepted;

                case "Rejected":
                    return GetCompositeScheduleStatus.Rejected;


                default:
                    return GetCompositeScheduleStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this GetCompositeScheduleStatus)

        public static String AsText(this GetCompositeScheduleStatus GetCompositeScheduleStatus)
        {

            switch (GetCompositeScheduleStatus)
            {

                case GetCompositeScheduleStatus.Accepted:
                    return "Accepted";

                case GetCompositeScheduleStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsReservationStatus(Text)

        public static ReservationStatus AsReservationStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ReservationStatus.Accepted;

                case "Faulted":
                    return ReservationStatus.Faulted;

                case "Occupied":
                    return ReservationStatus.Occupied;

                case "Rejected":
                    return ReservationStatus.Rejected;

                case "Unavailable":
                    return ReservationStatus.Unavailable;


                default:
                    return ReservationStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ReservationStatus)

        public static String AsText(this ReservationStatus ReservationStatus)
        {

            switch (ReservationStatus)
            {

                case ReservationStatus.Accepted:
                    return "Accepted";

                case ReservationStatus.Faulted:
                    return "Faulted";

                case ReservationStatus.Occupied:
                    return "Occupied";

                case ReservationStatus.Rejected:
                    return "Rejected";

                case ReservationStatus.Unavailable:
                    return "Unavailable";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsUpdateType(Text)

        public static UpdateTypes AsUpdateType(this String Text)
        {

            switch (Text)
            {

                case "Differential":
                    return UpdateTypes.Differential;

                case "Full":
                    return UpdateTypes.Full;


                default:
                    return UpdateTypes.Unknown;

            }

        }

        #endregion

        #region AsText(this UpdateType)

        public static String AsText(this UpdateTypes UpdateType)
        {

            switch (UpdateType)
            {

                case UpdateTypes.Differential:
                    return "Differential";

                case UpdateTypes.Full:
                    return "Full";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsUpdateStatus(Text)

        public static UpdateStatus AsUpdateStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return UpdateStatus.Accepted;

                case "Failed":
                    return UpdateStatus.Failed;

                case "NotSupported":
                    return UpdateStatus.NotSupported;

                case "VersionMismatch":
                    return UpdateStatus.VersionMismatch;


                default:
                    return UpdateStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this UpdateStatus)

        public static String AsText(this UpdateStatus UpdateStatus)
        {

            switch (UpdateStatus)
            {

                case UpdateStatus.Accepted:
                    return "Accepted";

                case UpdateStatus.Failed:
                    return "Failed";

                case UpdateStatus.NotSupported:
                    return "NotSupported";

                case UpdateStatus.VersionMismatch:
                    return "VersionMismatch";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsChargingProfileKind(Text)

        public static ChargingProfileKinds AsChargingProfileKind(this String Text)
        {

            switch (Text)
            {

                case "Absolute":
                    return ChargingProfileKinds.Absolute;

                case "Recurring":
                    return ChargingProfileKinds.Recurring;

                case "Relative":
                    return ChargingProfileKinds.Relative;


                default:
                    return ChargingProfileKinds.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargingProfileKindType)

        public static String AsText(this ChargingProfileKinds ChargingProfileKindType)
        {

            switch (ChargingProfileKindType)
            {

                case ChargingProfileKinds.Absolute:
                    return "Absolute";

                case ChargingProfileKinds.Recurring:
                    return "Recurring";

                case ChargingProfileKinds.Relative:
                    return "Relative";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsRecurrencyKind(Text)

        public static RecurrencyKinds AsRecurrencyKind(this String Text)
        {

            switch (Text)
            {

                case "Daily":
                    return RecurrencyKinds.Daily;

                case "Weekly":
                    return RecurrencyKinds.Weekly;


                default:
                    return RecurrencyKinds.Unknown;

            }

        }

        #endregion

        #region AsText(this RecurrencyKindType)

        public static String AsText(this RecurrencyKinds RecurrencyKindType)
        {

            switch (RecurrencyKindType)
            {

                case RecurrencyKinds.Daily:
                    return "Daily";

                case RecurrencyKinds.Weekly:
                    return "Weekly";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsChargingProfileStatus(Text)

        public static ChargingProfileStatus AsChargingProfileStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ChargingProfileStatus.Accepted;

                case "Rejected":
                    return ChargingProfileStatus.Rejected;

                case "NotSupported":
                    return ChargingProfileStatus.NotSupported;


                default:
                    return ChargingProfileStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargingProfileStatus)

        public static String AsText(this ChargingProfileStatus ChargingProfileStatus)
        {

            switch (ChargingProfileStatus)
            {

                case ChargingProfileStatus.Accepted:
                    return "Accepted";

                case ChargingProfileStatus.Rejected:
                    return "Rejected";

                case ChargingProfileStatus.NotSupported:
                    return "NotSupported";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsChargingProfilePurpose(Text)

        public static ChargingProfilePurposes AsChargingProfilePurpose(this String Text)
        {

            switch (Text)
            {

                case "ChargePointMaxProfile":
                    return ChargingProfilePurposes.ChargePointMaxProfile;

                case "TxProfile":
                    return ChargingProfilePurposes.TxProfile;


                default:
                    return ChargingProfilePurposes.TxDefaultProfile;

            }

        }

        #endregion

        #region AsText(this ChargingProfilePurpose)

        public static String AsText(this ChargingProfilePurposes ChargingProfilePurpose)
        {

            switch (ChargingProfilePurpose)
            {

                case ChargingProfilePurposes.ChargePointMaxProfile:
                    return "ChargePointMaxProfile";

                case ChargingProfilePurposes.TxProfile:
                    return "TxProfile";


                default:
                    return "TxDefaultProfile";

            }

        }

        #endregion


        #region AsMessageTrigger(Text)

        public static MessageTriggers AsMessageTrigger(this String Text)
        {

            switch (Text)
            {

                case "BootNotification":
                    return MessageTriggers.BootNotification;

                case "DiagnosticsStatusNotification":
                    return MessageTriggers.DiagnosticsStatusNotification;

                case "FirmwareStatusNotification":
                    return MessageTriggers.FirmwareStatusNotification;

                case "Heartbeat":
                    return MessageTriggers.Heartbeat;

                case "MeterValues":
                    return MessageTriggers.MeterValues;

                case "StatusNotification":
                    return MessageTriggers.StatusNotification;


                default:
                    return MessageTriggers.Unknown;

            }

        }

        #endregion

        #region AsText(this MessageTrigger)

        public static String AsText(this MessageTriggers MessageTrigger)
        {

            switch (MessageTrigger)
            {

                case MessageTriggers.BootNotification:
                    return "BootNotification";

                case MessageTriggers.DiagnosticsStatusNotification:
                    return "DiagnosticsStatusNotification";

                case MessageTriggers.FirmwareStatusNotification:
                    return "FirmwareStatusNotification";

                case MessageTriggers.Heartbeat:
                    return "Heartbeat";

                case MessageTriggers.MeterValues:
                    return "MeterValues";

                case MessageTriggers.StatusNotification:
                    return "StatusNotification";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsTriggerMessageStatus(Text)

        public static TriggerMessageStatus AsTriggerMessageStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return TriggerMessageStatus.Accepted;

                case "Rejected":
                    return TriggerMessageStatus.Rejected;


                default:
                    return TriggerMessageStatus.NotImplemented;

            }

        }

        #endregion

        #region AsText(this TriggerMessageStatus)

        public static String AsText(this TriggerMessageStatus TriggerMessageStatus)
        {

            switch (TriggerMessageStatus)
            {

                case TriggerMessageStatus.Accepted:
                    return "Accepted";

                case TriggerMessageStatus.Rejected:
                    return "Rejected";


                default:
                    return "NotImplemented";

            }

        }

        #endregion


        #region AsReasons(Text)

        public static Reasons AsReasons(this String Text)
        {

            switch (Text)
            {

                case "EmergencyStop":
                    return Reasons.EmergencyStop;

                case "EVDisconnected":
                    return Reasons.EVDisconnected;

                case "HardReset":
                    return Reasons.HardReset;

                case "Local":
                    return Reasons.Local;

                case "Other":
                    return Reasons.Other;

                case "PowerLoss":
                    return Reasons.PowerLoss;

                case "Reboot":
                    return Reasons.Reboot;

                case "Remote":
                    return Reasons.Remote;

                case "SoftReset":
                    return Reasons.SoftReset;

                case "UnlockCommand":
                    return Reasons.UnlockCommand;

                case "DeAuthorized":
                    return Reasons.DeAuthorized;


                default:
                    return Reasons.Unknown;

            }

        }

        #endregion

        #region AsText(this Reasons)

        public static String AsText(this Reasons Reasons)
        {

            switch (Reasons)
            {

                case Reasons.EmergencyStop:
                    return "EmergencyStop";

                case Reasons.EVDisconnected:
                    return "EVDisconnected";

                case Reasons.HardReset:
                    return "HardReset";

                case Reasons.Local:
                    return "Local";

                case Reasons.Other:
                    return "Other";

                case Reasons.PowerLoss:
                    return "PowerLoss";

                case Reasons.Reboot:
                    return "Reboot";

                case Reasons.Remote:
                    return "Remote";

                case Reasons.SoftReset:
                    return "SoftReset";

                case Reasons.UnlockCommand:
                    return "UnlockCommand";

                case Reasons.DeAuthorized:
                    return "DeAuthorized";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsReadingContexts(Text)

        public static ReadingContexts AsReadingContexts(this String Text)
        {

            switch (Text)
            {

                case "Interruption.Begin":
                    return ReadingContexts.InterruptionBegin;

                case "Interruption.End":
                    return ReadingContexts.InterruptionEnd;

                case "Other":
                    return ReadingContexts.Other;

                case "Sample.Clock":
                    return ReadingContexts.SampleClock;

                case "Transaction.Begin":
                    return ReadingContexts.TransactionBegin;

                case "Transaction.End":
                    return ReadingContexts.TransactionEnd;

                case "Trigger":
                    return ReadingContexts.Trigger;


                default:
                    return ReadingContexts.SamplePeriodic;

            }

        }

        #endregion

        #region AsText(this ReadingContexts)

        public static String AsText(this ReadingContexts ReadingContexts)
        {

            switch (ReadingContexts)
            {

                case ReadingContexts.InterruptionBegin:
                    return "Interruption.Begin";

                case ReadingContexts.InterruptionEnd:
                    return "Interruption.End";

                case ReadingContexts.Other:
                    return "Other";

                case ReadingContexts.SampleClock:
                    return "Sample.Clock";

                case ReadingContexts.TransactionBegin:
                    return "Transaction.Begin";

                case ReadingContexts.TransactionEnd:
                    return "Transaction.End";

                case ReadingContexts.Trigger:
                    return "Trigger";


                default:
                    return "Sample.Periodic";

            }

        }

        #endregion


        #region AsMeasurand(Text)

        public static Measurands AsMeasurand(this String Text)
        {

            switch (Text)
            {

                case "Current.Export":
                    return Measurands.CurrentExport;

                case "Current.Import":
                    return Measurands.CurrentImport;

                case "Current.Offered":
                    return Measurands.CurrentOffered;

                case "Energy.Active.Export.Register":
                    return Measurands.EnergyReactiveExportRegister;

                case "Energy.Active.Import.Register":
                    return Measurands.EnergyActiveImportRegister;

                case "Energy.Reactive.Export.Register":
                    return Measurands.EnergyReactiveExportRegister;

                case "Energy.Reactive.Import.Register":
                    return Measurands.EnergyReactiveImportRegister;

                case "Energy.Active.Export.Interval":
                    return Measurands.EnergyActiveExportInterval;

                case "Energy.Active.Import.Interval":
                    return Measurands.EnergyActiveImportInterval;

                case "Energy.Reactive.Export.Interval":
                    return Measurands.EnergyReactiveExportInterval;

                case "Energy.Reactive.Import.Interval":
                    return Measurands.EnergyReactiveImportInterval;

                case "Frequency":
                    return Measurands.Frequency;

                case "Power.Active.Export":
                    return Measurands.PowerActiveExport;

                case "Power.Active.Import":
                    return Measurands.PowerActiveImport;

                case "Power.Factor":
                    return Measurands.PowerFactor;

                case "Power.Offered":
                    return Measurands.PowerOffered;

                case "Power.Reactive.Export":
                    return Measurands.PowerReactiveExport;

                case "Power.Reactive.Import":
                    return Measurands.PowerReactiveImport;

                case "RPM":
                    return Measurands.RPM;

                case "SoC":
                    return Measurands.SoC;

                case "Temperature":
                    return Measurands.Temperature;

                case "Voltage":
                    return Measurands.Voltage;


                default:
                    return Measurands.EnergyActiveImportRegister;

            }

        }

        #endregion

        #region AsText(this Measurand)

        public static String AsText(this Measurands Measurand)
        {

            switch (Measurand)
            {

                case Measurands.CurrentExport:
                    return "Current.Export";

                case Measurands.CurrentImport:
                    return "Current.Import";

                case Measurands.CurrentOffered:
                    return "Current.Offered";

                case Measurands.EnergyActiveExportRegister:
                    return "Energy.Active.Export.Register";

                case Measurands.EnergyActiveImportRegister:
                    return "Energy.Active.Import.Register";

                case Measurands.EnergyReactiveExportRegister:
                    return "Energy.Reactive.Export.Register";

                case Measurands.EnergyReactiveImportRegister:
                    return "Energy.Reactive.Import.Register";

                case Measurands.EnergyActiveExportInterval:
                    return "Energy.Active.Export.Interval";

                case Measurands.EnergyActiveImportInterval:
                    return "Energy.Active.Import.Interval";

                case Measurands.EnergyReactiveExportInterval:
                    return "Energy.Reactive.Export.Interval";

                case Measurands.EnergyReactiveImportInterval:
                    return "Energy.Reactive.Import.Interval";

                case Measurands.Frequency:
                    return "Frequency";

                case Measurands.PowerActiveExport:
                    return "Power.Active.Export";

                case Measurands.PowerActiveImport:
                    return "Power.Active.Import";

                case Measurands.PowerFactor:
                    return "Power.Factor";

                case Measurands.PowerOffered:
                    return "Power.Offered";

                case Measurands.PowerReactiveExport:
                    return "Power.Reactive.Export";

                case Measurands.PowerReactiveImport:
                    return "Power.Reactive.Import";

                case Measurands.RPM:
                    return "RPM";

                case Measurands.SoC:
                    return "SoC";

                case Measurands.Temperature:
                    return "Temperature";

                case Measurands.Voltage:
                    return "Voltage";


                default:
                    return "Energy.Active.Import.Register";

            }

        }

        #endregion


        #region AsValueFormats(Text)

        public static ValueFormats AsValueFormats(this String Text)
        {

            switch (Text)
            {

                case "SignedData":
                    return ValueFormats.SignedData;


                default:
                    return ValueFormats.Raw;

            }

        }

        #endregion

        #region AsText(this ValueFormat)

        public static String AsText(this ValueFormats ValueFormat)
        {

            switch (ValueFormat)
            {

                case ValueFormats.SignedData:
                    return "SignedData";


                default:
                    return "Raw";

            }

        }

        #endregion


        #region AsUnitsOfMeasure(Text)

        public static UnitsOfMeasure AsUnitsOfMeasure(this String Text)
        {

            switch (Text)
            {

                case "Celsius":
                    return UnitsOfMeasure.Celsius;

                case "Fahrenheit":
                    return UnitsOfMeasure.Fahrenheit;

                case "kWh":
                    return UnitsOfMeasure.kWh;

                case "varh":
                    return UnitsOfMeasure.varh;

                case "kvarh":
                    return UnitsOfMeasure.kvarh;

                case "W":
                    return UnitsOfMeasure.Watts;

                case "kW":
                    return UnitsOfMeasure.kW;

                case "VA":
                    return UnitsOfMeasure.VoltAmpere;

                case "kVA":
                    return UnitsOfMeasure.kVA;

                case "var":
                    return UnitsOfMeasure.var;

                case "kvar":
                    return UnitsOfMeasure.kvar;

                case "A":
                    return UnitsOfMeasure.Amperes;

                case "V":
                    return UnitsOfMeasure.Voltage;

                case "K":
                    return UnitsOfMeasure.Kelvin;

                case "Percent":
                    return UnitsOfMeasure.Percent;


                default:
                    return UnitsOfMeasure.Wh;

            }

        }

        #endregion

        #region AsText(this UnitOfMeasure)

        public static String AsText(this UnitsOfMeasure UnitOfMeasure)
        {

            switch (UnitOfMeasure)
            {

                case UnitsOfMeasure.Celsius:
                    return "Celsius";

                case UnitsOfMeasure.Fahrenheit:
                    return "Fahrenheit";

                case UnitsOfMeasure.kWh:
                    return "kWh";

                case UnitsOfMeasure.varh:
                    return "varh";

                case UnitsOfMeasure.kvarh:
                    return "kvarh";

                case UnitsOfMeasure.Watts:
                    return "W";

                case UnitsOfMeasure.kW:
                    return "kW";

                case UnitsOfMeasure.VoltAmpere:
                    return "VA";

                case UnitsOfMeasure.kVA:
                    return "kVA";

                case UnitsOfMeasure.var:
                    return "var";

                case UnitsOfMeasure.kvar:
                    return "kvar";

                case UnitsOfMeasure.Amperes:
                    return "A";

                case UnitsOfMeasure.Voltage:
                    return "V";

                case UnitsOfMeasure.Kelvin:
                    return "K";

                case UnitsOfMeasure.Percent:
                    return "Percent";


                default:
                    return "Wh";

            }

        }

        #endregion


        #region AsPhases(Text)

        public static Phases AsPhases(this String Text)
        {

            switch (Text)
            {

                case "L1":
                    return Phases.L1;

                case "L2":
                    return Phases.L2;

                case "L3":
                    return Phases.L3;

                case "N":
                    return Phases.N;

                case "L1-N":
                    return Phases.L1_N;

                case "L2-N":
                    return Phases.L2_N;

                case "L3-N":
                    return Phases.L3_N;

                case "L1-L2":
                    return Phases.L1_L2;

                case "L2-L3":
                    return Phases.L2_L3;

                case "L3-L1":
                    return Phases.L3_L1;


                default:
                    return Phases.Unknown;

            }

        }

        #endregion

        #region AsText(this Phase)

        public static String AsText(this Phases Phase)
        {

            switch (Phase)
            {

                case Phases.L1:
                    return "L1";

                case Phases.L2:
                    return "L2";

                case Phases.L3:
                    return "L3";

                case Phases.N:
                    return "N";

                case Phases.L1_N:
                    return "L1-N";

                case Phases.L2_N:
                    return "L2-N";

                case Phases.L3_N:
                    return "L3-N";

                case Phases.L1_L2:
                    return "L1-L2";

                case Phases.L2_L3:
                    return "L2-L3";

                case Phases.L3_L1:
                    return "L3-L1";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsLocations(Text)

        public static Locations AsLocations(this String Text)
        {

            switch (Text)
            {

                case "Body":
                    return Locations.Body;

                case "Cable":
                    return Locations.Cable;

                case "EV":
                    return Locations.EV;

                case "Inlet":
                    return Locations.Inlet;


                default:
                    return Locations.Outlet;

            }

        }

        #endregion

        #region AsText(this Location)

        public static String AsText(this Locations Location)
        {

            switch (Location)
            {

                case Locations.Body:
                    return "Body";

                case Locations.Cable:
                    return "Cable";

                case Locations.EV:
                    return "EV";

                case Locations.Inlet:
                    return "Inlet";


                default:
                    return "Outlet";

            }

        }

        #endregion


        #region AsRegistrationStatus(Text)

        public static RegistrationStatus AsRegistrationStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return RegistrationStatus.Accepted;

                case "Pending":
                    return RegistrationStatus.Pending;

                case "Rejected":
                    return RegistrationStatus.Rejected;


                default:
                    return RegistrationStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this RegistrationStatus)

        public static String AsText(this RegistrationStatus RegistrationStatus)
        {

            switch (RegistrationStatus)
            {

                case RegistrationStatus.Accepted:
                    return "Accepted";

                case RegistrationStatus.Pending:
                    return "Pending";

                case RegistrationStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsChargePointErrorCodes(Text)

        public static ChargePointErrorCodes AsChargePointErrorCodes(this String Text)
        {

            switch (Text)
            {

                case "ConnectorLockFailure":
                    return ChargePointErrorCodes.ConnectorLockFailure;

                case "EVCommunicationError":
                    return ChargePointErrorCodes.EVCommunicationError;

                case "GroundFailure":
                    return ChargePointErrorCodes.GroundFailure;

                case "HighTemperature":
                    return ChargePointErrorCodes.HighTemperature;

                case "InternalError":
                    return ChargePointErrorCodes.InternalError;

                case "LocalListConflict":
                    return ChargePointErrorCodes.LocalListConflict;

                case "NoError":
                    return ChargePointErrorCodes.NoError;

                case "OtherError":
                    return ChargePointErrorCodes.OtherError;

                case "OverCurrentFailure":
                    return ChargePointErrorCodes.OverCurrentFailure;

                case "OverVoltage":
                    return ChargePointErrorCodes.OverVoltage;

                case "PowerMeterFailure":
                    return ChargePointErrorCodes.PowerMeterFailure;

                case "PowerSwitchFailure":
                    return ChargePointErrorCodes.PowerSwitchFailure;

                case "ReaderFailure":
                    return ChargePointErrorCodes.ReaderFailure;

                case "ResetFailure":
                    return ChargePointErrorCodes.ResetFailure;

                case "UnderVoltage":
                    return ChargePointErrorCodes.UnderVoltage;

                case "WeakSignal":
                    return ChargePointErrorCodes.WeakSignal;


                default:
                    return ChargePointErrorCodes.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargePointErrorCode)

        public static String AsText(this ChargePointErrorCodes ChargePointErrorCode)
        {

            switch (ChargePointErrorCode)
            {

                case ChargePointErrorCodes.ConnectorLockFailure:
                    return "ConnectorLockFailure";

                case ChargePointErrorCodes.EVCommunicationError:
                    return "EVCommunicationError";

                case ChargePointErrorCodes.GroundFailure:
                    return "GroundFailure";

                case ChargePointErrorCodes.HighTemperature:
                    return "HighTemperature";

                case ChargePointErrorCodes.InternalError:
                    return "InternalError";

                case ChargePointErrorCodes.LocalListConflict:
                    return "LocalListConflict";

                case ChargePointErrorCodes.NoError:
                    return "NoError";

                case ChargePointErrorCodes.OtherError:
                    return "OtherError";

                case ChargePointErrorCodes.OverCurrentFailure:
                    return "OverCurrentFailure";

                case ChargePointErrorCodes.OverVoltage:
                    return "OverVoltage";

                case ChargePointErrorCodes.PowerMeterFailure:
                    return "PowerMeterFailure";

                case ChargePointErrorCodes.PowerSwitchFailure:
                    return "PowerSwitchFailure";

                case ChargePointErrorCodes.ReaderFailure:
                    return "ReaderFailure";

                case ChargePointErrorCodes.ResetFailure:
                    return "ResetFailure";

                case ChargePointErrorCodes.UnderVoltage:
                    return "UnderVoltage";

                case ChargePointErrorCodes.WeakSignal:
                    return "WeakSignal";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsChargePointStatus(Text)

        public static ChargePointStatus AsChargePointStatus(this String Text)
        {

            switch (Text)
            {

                case "Available":
                    return ChargePointStatus.Available;

                case "Preparing":
                    return ChargePointStatus.Preparing;

                case "Charging":
                    return ChargePointStatus.Charging;

                case "SuspendedEV":
                    return ChargePointStatus.SuspendedEV;

                case "SuspendedEVSE":
                    return ChargePointStatus.SuspendedEVSE;

                case "Finishing":
                    return ChargePointStatus.Finishing;

                case "Reserved":
                    return ChargePointStatus.Reserved;

                case "Faulted":
                    return ChargePointStatus.Faulted;

                case "Unavailable":
                    return ChargePointStatus.Unavailable;


                default:
                    return ChargePointStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargePointStatus)

        public static String AsText(this ChargePointStatus ChargePointStatus)
        {

            switch (ChargePointStatus)
            {

                case ChargePointStatus.Available:
                    return "Available";

                case ChargePointStatus.Preparing:
                    return "Preparing";

                case ChargePointStatus.Charging:
                    return "Charging";

                case ChargePointStatus.SuspendedEV:
                    return "SuspendedEV";

                case ChargePointStatus.SuspendedEVSE:
                    return "SuspendedEVSE";

                case ChargePointStatus.Finishing:
                    return "Finishing";

                case ChargePointStatus.Reserved:
                    return "Reserved";

                case ChargePointStatus.Faulted:
                    return "Faulted";

                case ChargePointStatus.Unavailable:
                    return "Unavailable";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsFirmwareStatus(Text)

        public static FirmwareStatus AsFirmwareStatus(this String Text)
        {

            switch (Text)
            {

                case "Downloaded":
                    return FirmwareStatus.Downloaded;

                case "DownloadFailed":
                    return FirmwareStatus.DownloadFailed;

                case "Downloading":
                    return FirmwareStatus.Downloading;

                case "Idle":
                    return FirmwareStatus.Idle;

                case "InstallationFailed":
                    return FirmwareStatus.InstallationFailed;

                case "Installed":
                    return FirmwareStatus.Installed;

                case "Installing":
                    return FirmwareStatus.Installing;


                default:
                    return FirmwareStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this FirmwareStatus)

        public static String AsText(this FirmwareStatus FirmwareStatus)
        {

            switch (FirmwareStatus)
            {

                case FirmwareStatus.Downloaded:
                    return "Downloaded";

                case FirmwareStatus.DownloadFailed:
                    return "DownloadFailed";

                case FirmwareStatus.Downloading:
                    return "Downloading";

                case FirmwareStatus.Idle:
                    return "Idle";

                case FirmwareStatus.InstallationFailed:
                    return "InstallationFailed";

                case FirmwareStatus.Installed:
                    return "Installed";

                case FirmwareStatus.Installing:
                    return "Installing";


                default:
                    return "unknown";

            }

        }

        #endregion


        #region AsDiagnosticsStatus(Text)

        public static DiagnosticsStatus AsDiagnosticsStatus(this String Text)
        {

            switch (Text)
            {

                case "Idle":
                    return DiagnosticsStatus.Idle;

                case "Uploaded":
                    return DiagnosticsStatus.Uploaded;

                case "UploadFailed":
                    return DiagnosticsStatus.UploadFailed;

                case "Uploading":
                    return DiagnosticsStatus.Uploading;


                default:
                    return DiagnosticsStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this DiagnosticsStatus)

        public static String AsText(this DiagnosticsStatus DiagnosticsStatus)
        {

            switch (DiagnosticsStatus)
            {

                case DiagnosticsStatus.Idle:
                    return "Idle";

                case DiagnosticsStatus.Uploaded:
                    return "Uploaded";

                case DiagnosticsStatus.UploadFailed:
                    return "UploadFailed";

                case DiagnosticsStatus.Uploading:
                    return "Uploading";


                default:
                    return "unknown";

            }

        }

        #endregion


    }

}