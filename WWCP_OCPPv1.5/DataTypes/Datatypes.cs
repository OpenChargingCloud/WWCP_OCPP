/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/GraphDefined/WWCP_OCPP>
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

using System;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_5
{

    // Central System Requests

    /// <summary>
    /// Defines single value of the meter-value-value
    /// </summary>
    public class MeterValue
    {

        public DateTime        Timestamp  { get; set; }
        public UInt64          Value      { get; set; }

        // XML attributes of value
        public ReadingContext  Context    { get; set; }
        public ValueFormat     Format     { get; set; }
        public Measurand       Measurand  { get; set; }
        public Location        Location   { get; set; }
        public UnitOfMeasure   Unit       { get; set; }

    }

    // Central System Responses

    /// <summary>
    /// Defines the charge-point-error-value
    /// </summary>
    public enum ChargePointErrorCode
    {
        ConnectorLockFailure,
        HighTemperature,
        Mode3Error,
        NoError,
        PowerMeterFailure,
        PowerSwitchFailure,
        ReaderFailure,
        ResetFailure,
        GroundFailure,
        OverCurrentFailure,
        UnderVoltage,
        WeakSignal,
        OtherError
    }

    /// <summary>
    /// Defines the charge-point-status-value
    /// </summary>
    public enum ChargePointStatus
    {
        Available,
        Occupied,
        Faulted,
        Unavailable,
        Reserved
    }

    /// <summary>
    /// Defines the firmware-status-value
    /// </summary>
    public enum FirmwareStatus
    {
        Downloaded,
        DownloadFailed,
        InstallationFailed,
        Installed
    }

    /// <summary>
    /// Defines the diagnostics-status-value
    /// </summary>
    public enum DiagnosticsStatus
    {
        Uploaded,
        UploadFailed
    }

    /// <summary>
    /// Defines the status returned in DataTransfer.conf
    /// </summary>
    public enum DataTransferStatus
    {
        Accepted,
        Rejected,
        UnknownMessageId,
        UnknownVendorId
    }


    public class IdTagInfo
    {

        public AuthorizationStatus  Status       { get; set; }
        public DateTime?            ExpiryDate   { get; set; }
        public String               ParentIdTag  { get; set; }

        public IdTagInfo(AuthorizationStatus  Status,
                         DateTime?            ExpiryDate   = null,
                         String               ParentIdTag  = null)
        {
            this.Status       = Status;
            this.ExpiryDate   = ExpiryDate;
            this.ParentIdTag  = ParentIdTag;
        }

    }

    public class TransactionData
    {
        public IEnumerable<MeterValue> Values { get; set; }
    }

    public enum ReadingContext
    {
        Interruption_Begin,
        Interruption_End,
        Sample_Clock,
        Sample_Periodic,
        Transaction_Begin,
        Transaction_End
    }

    public enum Measurand
    {
        Energy_Active_Export_Register,
        Energy_Active_Import_Register,
        Energy_Reactive_Export_Register,
        Energy_Reactive_Import_Register,
        Energy_Active_Export_Interval,
        Energy_Active_Import_Interval,
        Energy_Reactive_Export_Interval,
        Energy_Reactive_Import_Interval,
        Power_Active_Export,
        Power_Active_Import,
        Power_Reactive_Export,
        Power_Reactive_Import,
        Current_Export,
        Current_Import,
        Voltage,
        Temperature
    }

    public enum ValueFormat
    {
        Raw,
        SignedData
    }

    public enum UnitOfMeasure
    {
        Wh,
        kWh,
        varh,
        kvarh,
        W,
        kW,
        var,
        kvar,
        Amp,
        Volt,
        Celsius
    }

    public enum Location
    {
        Inlet,
        Outlet,
        Body
    }


}
