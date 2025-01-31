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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for binary data transfer statuss.
    /// </summary>
    public static class BinaryDataTransferStatusExtensions
    {

        /// <summary>
        /// Indicates whether this binary data transfer status is null or empty.
        /// </summary>
        /// <param name="BinaryDataTransferStatus">A binary data transfer status.</param>
        public static Boolean IsNullOrEmpty(this BinaryDataTransferStatus? BinaryDataTransferStatus)
            => !BinaryDataTransferStatus.HasValue || BinaryDataTransferStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this binary data transfer status is null or empty.
        /// </summary>
        /// <param name="BinaryDataTransferStatus">A binary data transfer status.</param>
        public static Boolean IsNotNullOrEmpty(this BinaryDataTransferStatus? BinaryDataTransferStatus)
            => BinaryDataTransferStatus.HasValue && BinaryDataTransferStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A binary data transfer status.
    /// </summary>
    public readonly struct BinaryDataTransferStatus : IId,
                                                      IEquatable<BinaryDataTransferStatus>,
                                                      IComparable<BinaryDataTransferStatus>
    {

        #region Data

        private readonly static Dictionary<String, BinaryDataTransferStatus>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                                    InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this binary data transfer status is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this binary data transfer status is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the binary data transfer status.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new binary data transfer status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a binary data transfer status.</param>
        private BinaryDataTransferStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static BinaryDataTransferStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new BinaryDataTransferStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a binary data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a binary data transfer status.</param>
        public static BinaryDataTransferStatus Parse(String Text)
        {

            if (TryParse(Text, out var binaryDataTransferStatus))
                return binaryDataTransferStatus;

            throw new ArgumentException($"Invalid text representation of a binary data transfer status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as binary data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a binary data transfer status.</param>
        public static BinaryDataTransferStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var binaryDataTransferStatus))
                return binaryDataTransferStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out BinaryDataTransferStatus)

        /// <summary>
        /// Try to parse the given text as binary data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus">The parsed binary data transfer status.</param>
        public static Boolean TryParse(String Text, out BinaryDataTransferStatus BinaryDataTransferStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out BinaryDataTransferStatus))
                    BinaryDataTransferStatus = Register(Text);

                return true;

            }

            BinaryDataTransferStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this binary data transfer status.
        /// </summary>
        public BinaryDataTransferStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Message has been accepted, and the contained request is accepted.
        /// </summary>
        public static BinaryDataTransferStatus Accepted            { get; }
            = Register("Accepted");

        /// <summary>
        /// Message has been accepted, but the contained request is rejected.
        /// </summary>
        public static BinaryDataTransferStatus Rejected            { get; }
            = Register("Rejected");

        /// <summary>
        /// Message could not be interpreted due to unknown MessageId string.
        /// </summary>
        public static BinaryDataTransferStatus UnknownMessageId    { get; }
            = Register("UnknownMessageId");

        /// <summary>
        /// Message could not be interpreted due to unknown VendorId string.
        /// </summary>
        public static BinaryDataTransferStatus UnknownVendorId     { get; }
            = Register("UnknownVendorId");

        /// <summary>
        /// The digital signature(s) of the message is/are invalid.
        /// </summary>
        public static BinaryDataTransferStatus InvalidSignature    { get; }
            = Register("InvalidSignature");

        #endregion


        #region Operator overloading

        #region Operator == (BinaryDataTransferStatus1, BinaryDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BinaryDataTransferStatus1">A binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus2">Another binary data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BinaryDataTransferStatus BinaryDataTransferStatus1,
                                           BinaryDataTransferStatus BinaryDataTransferStatus2)

            => BinaryDataTransferStatus1.Equals(BinaryDataTransferStatus2);

        #endregion

        #region Operator != (BinaryDataTransferStatus1, BinaryDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BinaryDataTransferStatus1">A binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus2">Another binary data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BinaryDataTransferStatus BinaryDataTransferStatus1,
                                           BinaryDataTransferStatus BinaryDataTransferStatus2)

            => !BinaryDataTransferStatus1.Equals(BinaryDataTransferStatus2);

        #endregion

        #region Operator <  (BinaryDataTransferStatus1, BinaryDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BinaryDataTransferStatus1">A binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus2">Another binary data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BinaryDataTransferStatus BinaryDataTransferStatus1,
                                          BinaryDataTransferStatus BinaryDataTransferStatus2)

            => BinaryDataTransferStatus1.CompareTo(BinaryDataTransferStatus2) < 0;

        #endregion

        #region Operator <= (BinaryDataTransferStatus1, BinaryDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BinaryDataTransferStatus1">A binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus2">Another binary data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BinaryDataTransferStatus BinaryDataTransferStatus1,
                                           BinaryDataTransferStatus BinaryDataTransferStatus2)

            => BinaryDataTransferStatus1.CompareTo(BinaryDataTransferStatus2) <= 0;

        #endregion

        #region Operator >  (BinaryDataTransferStatus1, BinaryDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BinaryDataTransferStatus1">A binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus2">Another binary data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BinaryDataTransferStatus BinaryDataTransferStatus1,
                                          BinaryDataTransferStatus BinaryDataTransferStatus2)

            => BinaryDataTransferStatus1.CompareTo(BinaryDataTransferStatus2) > 0;

        #endregion

        #region Operator >= (BinaryDataTransferStatus1, BinaryDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BinaryDataTransferStatus1">A binary data transfer status.</param>
        /// <param name="BinaryDataTransferStatus2">Another binary data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BinaryDataTransferStatus BinaryDataTransferStatus1,
                                           BinaryDataTransferStatus BinaryDataTransferStatus2)

            => BinaryDataTransferStatus1.CompareTo(BinaryDataTransferStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<BinaryDataTransferStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two binary data transfer statuss.
        /// </summary>
        /// <param name="Object">A binary data transfer status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BinaryDataTransferStatus binaryDataTransferStatus
                   ? CompareTo(binaryDataTransferStatus)
                   : throw new ArgumentException("The given object is not binary data transfer status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BinaryDataTransferStatus)

        /// <summary>
        /// Compares two binary data transfer statuss.
        /// </summary>
        /// <param name="BinaryDataTransferStatus">A binary data transfer status to compare with.</param>
        public Int32 CompareTo(BinaryDataTransferStatus BinaryDataTransferStatus)

            => String.Compare(InternalId,
                              BinaryDataTransferStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<BinaryDataTransferStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two binary data transfer statuss for equality.
        /// </summary>
        /// <param name="Object">A binary data transfer status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BinaryDataTransferStatus binaryDataTransferStatus &&
                   Equals(binaryDataTransferStatus);

        #endregion

        #region Equals(BinaryDataTransferStatus)

        /// <summary>
        /// Compares two binary data transfer statuss for equality.
        /// </summary>
        /// <param name="BinaryDataTransferStatus">A binary data transfer status to compare with.</param>
        public Boolean Equals(BinaryDataTransferStatus BinaryDataTransferStatus)

            => String.Equals(InternalId,
                             BinaryDataTransferStatus.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
