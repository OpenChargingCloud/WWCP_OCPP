/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// OCPP XML I/O.
    /// </summary>
    public static class XML_IO
    {

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