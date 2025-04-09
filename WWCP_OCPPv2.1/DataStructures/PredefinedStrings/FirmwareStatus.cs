/*
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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for Distributed Energy Resource (DER) control status.
    /// </summary>
    public static class FirmwareStatusExtensions
    {

        /// <summary>
        /// Indicates whether this firmware status is null or empty.
        /// </summary>
        /// <param name="FirmwareStatus">A firmware status.</param>
        public static Boolean IsNullOrEmpty(this FirmwareStatus? FirmwareStatus)
            => !FirmwareStatus.HasValue || FirmwareStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this firmware status is null or empty.
        /// </summary>
        /// <param name="FirmwareStatus">A firmware status.</param>
        public static Boolean IsNotNullOrEmpty(this FirmwareStatus? FirmwareStatus)
            => FirmwareStatus.HasValue && FirmwareStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A Distributed Energy Resource (DER) control status.
    /// </summary>
    public readonly struct FirmwareStatus : IId,
                                              IEquatable<FirmwareStatus>,
                                              IComparable<FirmwareStatus>
    {

        #region Data

        private readonly static Dictionary<String, FirmwareStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this firmware status is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this firmware status is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the firmware status.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered reset types.
        /// </summary>
        public static    IEnumerable<FirmwareStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new firmware status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a firmware status.</param>
        private FirmwareStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static FirmwareStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new FirmwareStatus(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a firmware status.</param>
        public static FirmwareStatus Parse(String Text)
        {

            if (TryParse(Text, out var derControlStatus))
                return derControlStatus;

            throw new ArgumentException($"Invalid text representation of a firmware status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a firmware status.</param>
        public static FirmwareStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var derControlStatus))
                return derControlStatus;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out FirmwareStatus)

        /// <summary>
        /// Try to parse the given text as a firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a firmware status.</param>
        /// <param name="FirmwareStatus">The parsed firmware status.</param>
        public static Boolean TryParse (String                                      Text,
                                        [NotNullWhen(true)] out FirmwareStatus  FirmwareStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out FirmwareStatus))
                    FirmwareStatus = Register(Text);

                return true;

            }

            FirmwareStatus = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out FirmwareStatus)

        /// <summary>
        /// Check whether the given text is a defined firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a firmware status.</param>
        /// <param name="FirmwareStatus">The validated firmware status.</param>
        public static Boolean IsDefined(String                                     Text,
                                       [NotNullWhen(true)] out FirmwareStatus  FirmwareStatus)

            => lookup.TryGetValue(Text.Trim(), out FirmwareStatus);

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this firmware status.
        /// </summary>
        public FirmwareStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// New firmware has been downloaded by the charging station (Intermediate state).
        /// </summary>
        public static FirmwareStatus  Downloaded                   { get; }
            = Register("Downloaded");

        /// <summary>
        /// The charging station failed to download the firmware (Failure end state).
        /// </summary>
        public static FirmwareStatus  DownloadFailed               { get; }
            = Register("DownloadFailed");

        /// <summary>
        /// The firmware is being downloaded (Intermediate state).
        /// </summary>
        public static FirmwareStatus  Downloading                  { get; }
            = Register("Downloading");

        /// <summary>
        /// Downloading of new firmware has been scheduled (Intermediate state).
        /// </summary>
        public static FirmwareStatus  DownloadScheduled            { get; }
            = Register("DownloadScheduled");

        /// <summary>
        /// Downloading of the firmware has been paused (Intermediate state).
        /// </summary>
        public static FirmwareStatus  DownloadPaused               { get; }
            = Register("DownloadPaused");


        /// <summary>
        /// The charging station is not performing firmware update related tasks.
        /// Status Idle SHALL only be used as in a FirmwareStatusNotification
        /// request that was triggered by a TriggerMessage request.
        /// </summary>
        public static FirmwareStatus  Idle                         { get; }
            = Register("Idle");


        /// <summary>
        /// Installation of new firmware has failed (Failure end state).
        /// </summary>
        public static FirmwareStatus  InstallationFailed           { get; }
            = Register("InstallationFailed");

        /// <summary>
        /// Firmware is being installed (Intermediate state).
        /// </summary>
        public static FirmwareStatus  Installing                   { get; }
            = Register("Installing");

        /// <summary>
        /// New firmware has successfully been installed in charging station (Successful end state).
        /// </summary>
        public static FirmwareStatus  Installed                    { get; }
            = Register("Installed");

        /// <summary>
        /// The charging station is about to reboot to activate new firmware. This status MAY be omitted
        /// if a reboot is an integral part of the installation and cannot be reported separately
        /// (Intermediate state).
        /// </summary>
        public static FirmwareStatus  InstallRebooting             { get; }
            = Register("InstallRebooting");

        /// <summary>
        /// Installation of the downloaded firmware is scheduled to take place on installDateTime given in SignedUpdateFirmware.req (Intermediate state).
        /// </summary>
        public static FirmwareStatus  InstallScheduled             { get; }
            = Register("InstallScheduled");

        /// <summary>
        /// Verification of the new firmware (e.g. using a checksum or some other means) has failed and installation will not proceed (Failure end state).
        /// </summary>
        public static FirmwareStatus  InstallVerificationFailed    { get; }
            = Register("InstallVerificationFailed");

        /// <summary>
        /// The firmware signature is not valid (Failure end state).
        /// </summary>
        public static FirmwareStatus  InvalidSignature             { get; }
            = Register("InvalidSignature");

        /// <summary>
        /// Provide firmware signature successfully verified (Intermediate state).
        /// </summary>
        public static FirmwareStatus  SignatureVerified            { get; }
            = Register("SignatureVerified");


        /// <summary>
        /// Error
        /// </summary>
        public static FirmwareStatus  Error                        { get; }
            = Register("Error");

        /// <summary>
        /// Signature Error (OCCP command)
        /// </summary>
        public static FirmwareStatus  SignatureError               { get; }
            = Register("SignatureError");

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatus1, FirmwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareStatus1">A firmware status.</param>
        /// <param name="FirmwareStatus2">Another firmware status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FirmwareStatus FirmwareStatus1,
                                           FirmwareStatus FirmwareStatus2)

            => FirmwareStatus1.Equals(FirmwareStatus2);

        #endregion

        #region Operator != (FirmwareStatus1, FirmwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareStatus1">A firmware status.</param>
        /// <param name="FirmwareStatus2">Another firmware status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FirmwareStatus FirmwareStatus1,
                                           FirmwareStatus FirmwareStatus2)

            => !FirmwareStatus1.Equals(FirmwareStatus2);

        #endregion

        #region Operator <  (FirmwareStatus1, FirmwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareStatus1">A firmware status.</param>
        /// <param name="FirmwareStatus2">Another firmware status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FirmwareStatus FirmwareStatus1,
                                          FirmwareStatus FirmwareStatus2)

            => FirmwareStatus1.CompareTo(FirmwareStatus2) < 0;

        #endregion

        #region Operator <= (FirmwareStatus1, FirmwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareStatus1">A firmware status.</param>
        /// <param name="FirmwareStatus2">Another firmware status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FirmwareStatus FirmwareStatus1,
                                           FirmwareStatus FirmwareStatus2)

            => FirmwareStatus1.CompareTo(FirmwareStatus2) <= 0;

        #endregion

        #region Operator >  (FirmwareStatus1, FirmwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareStatus1">A firmware status.</param>
        /// <param name="FirmwareStatus2">Another firmware status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FirmwareStatus FirmwareStatus1,
                                          FirmwareStatus FirmwareStatus2)

            => FirmwareStatus1.CompareTo(FirmwareStatus2) > 0;

        #endregion

        #region Operator >= (FirmwareStatus1, FirmwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareStatus1">A firmware status.</param>
        /// <param name="FirmwareStatus2">Another firmware status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FirmwareStatus FirmwareStatus1,
                                           FirmwareStatus FirmwareStatus2)

            => FirmwareStatus1.CompareTo(FirmwareStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<FirmwareStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two firmware status.
        /// </summary>
        /// <param name="Object">A firmware status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is FirmwareStatus derControlStatus
                   ? CompareTo(derControlStatus)
                   : throw new ArgumentException("The given object is not a firmware status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FirmwareStatus)

        /// <summary>
        /// Compares two firmware status.
        /// </summary>
        /// <param name="FirmwareStatus">A firmware status to compare with.</param>
        public Int32 CompareTo(FirmwareStatus FirmwareStatus)

            => String.Compare(InternalId,
                              FirmwareStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two firmware status for equality.
        /// </summary>
        /// <param name="Object">A firmware status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FirmwareStatus derControlStatus &&
                   Equals(derControlStatus);

        #endregion

        #region Equals(FirmwareStatus)

        /// <summary>
        /// Compares two firmware status for equality.
        /// </summary>
        /// <param name="FirmwareStatus">A firmware status to compare with.</param>
        public Boolean Equals(FirmwareStatus FirmwareStatus)

            => String.Equals(InternalId,
                             FirmwareStatus.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
