/*
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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extension methods for secure data transfer statuss.
    /// </summary>
    public static class SecureDataTransferStatusExtensions
    {

        /// <summary>
        /// Indicates whether this secure data transfer status is null or empty.
        /// </summary>
        /// <param name="SecureDataTransferStatus">A secure data transfer status.</param>
        public static Boolean IsNullOrEmpty(this SecureDataTransferStatus? SecureDataTransferStatus)
            => !SecureDataTransferStatus.HasValue || SecureDataTransferStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this secure data transfer status is null or empty.
        /// </summary>
        /// <param name="SecureDataTransferStatus">A secure data transfer status.</param>
        public static Boolean IsNotNullOrEmpty(this SecureDataTransferStatus? SecureDataTransferStatus)
            => SecureDataTransferStatus.HasValue && SecureDataTransferStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A secure data transfer status.
    /// </summary>
    public readonly struct SecureDataTransferStatus : IId,
                                                      IEquatable<SecureDataTransferStatus>,
                                                      IComparable<SecureDataTransferStatus>
    {

        #region Data

        private readonly static Dictionary<String, SecureDataTransferStatus>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                                    InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this secure data transfer status is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this secure data transfer status is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the secure data transfer status.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new secure data transfer status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a secure data transfer status.</param>
        private SecureDataTransferStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static SecureDataTransferStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new SecureDataTransferStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a secure data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a secure data transfer status.</param>
        public static SecureDataTransferStatus Parse(String Text)
        {

            if (TryParse(Text, out var secureDataTransferStatus))
                return secureDataTransferStatus;

            throw new ArgumentException($"Invalid text representation of a secure data transfer status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as secure data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a secure data transfer status.</param>
        public static SecureDataTransferStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var secureDataTransferStatus))
                return secureDataTransferStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SecureDataTransferStatus)

        /// <summary>
        /// Try to parse the given text as secure data transfer status.
        /// </summary>
        /// <param name="Text">A text representation of a secure data transfer status.</param>
        /// <param name="SecureDataTransferStatus">The parsed secure data transfer status.</param>
        public static Boolean TryParse(String Text, out SecureDataTransferStatus SecureDataTransferStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out SecureDataTransferStatus))
                    SecureDataTransferStatus = Register(Text);

                return true;

            }

            SecureDataTransferStatus = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this secure data transfer status.
        /// </summary>
        public SecureDataTransferStatus Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Message has been accepted, and the contained request is accepted.
        /// </summary>
        public static SecureDataTransferStatus Accepted            { get; }
            = Register("Accepted");

        /// <summary>
        /// Message has been accepted, but the contained request is rejected.
        /// </summary>
        public static SecureDataTransferStatus Rejected            { get; }
            = Register("Rejected");

        /// <summary>
        /// Message could not be interpreted due to unknown MessageId string.
        /// </summary>
        public static SecureDataTransferStatus UnknownMessageId    { get; }
            = Register("UnknownMessageId");

        /// <summary>
        /// Message could not be interpreted due to unknown VendorId string.
        /// </summary>
        public static SecureDataTransferStatus UnknownVendorId     { get; }
            = Register("UnknownVendorId");

        /// <summary>
        /// The digital signature(s) of the message is/are invalid.
        /// </summary>
        public static SecureDataTransferStatus InvalidSignature    { get; }
            = Register("InvalidSignature");

        #endregion


        #region Operator overloading

        #region Operator == (SecureDataTransferStatus1, SecureDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecureDataTransferStatus1">A secure data transfer status.</param>
        /// <param name="SecureDataTransferStatus2">Another secure data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SecureDataTransferStatus SecureDataTransferStatus1,
                                           SecureDataTransferStatus SecureDataTransferStatus2)

            => SecureDataTransferStatus1.Equals(SecureDataTransferStatus2);

        #endregion

        #region Operator != (SecureDataTransferStatus1, SecureDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecureDataTransferStatus1">A secure data transfer status.</param>
        /// <param name="SecureDataTransferStatus2">Another secure data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SecureDataTransferStatus SecureDataTransferStatus1,
                                           SecureDataTransferStatus SecureDataTransferStatus2)

            => !SecureDataTransferStatus1.Equals(SecureDataTransferStatus2);

        #endregion

        #region Operator <  (SecureDataTransferStatus1, SecureDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecureDataTransferStatus1">A secure data transfer status.</param>
        /// <param name="SecureDataTransferStatus2">Another secure data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SecureDataTransferStatus SecureDataTransferStatus1,
                                          SecureDataTransferStatus SecureDataTransferStatus2)

            => SecureDataTransferStatus1.CompareTo(SecureDataTransferStatus2) < 0;

        #endregion

        #region Operator <= (SecureDataTransferStatus1, SecureDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecureDataTransferStatus1">A secure data transfer status.</param>
        /// <param name="SecureDataTransferStatus2">Another secure data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SecureDataTransferStatus SecureDataTransferStatus1,
                                           SecureDataTransferStatus SecureDataTransferStatus2)

            => SecureDataTransferStatus1.CompareTo(SecureDataTransferStatus2) <= 0;

        #endregion

        #region Operator >  (SecureDataTransferStatus1, SecureDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecureDataTransferStatus1">A secure data transfer status.</param>
        /// <param name="SecureDataTransferStatus2">Another secure data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SecureDataTransferStatus SecureDataTransferStatus1,
                                          SecureDataTransferStatus SecureDataTransferStatus2)

            => SecureDataTransferStatus1.CompareTo(SecureDataTransferStatus2) > 0;

        #endregion

        #region Operator >= (SecureDataTransferStatus1, SecureDataTransferStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecureDataTransferStatus1">A secure data transfer status.</param>
        /// <param name="SecureDataTransferStatus2">Another secure data transfer status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SecureDataTransferStatus SecureDataTransferStatus1,
                                           SecureDataTransferStatus SecureDataTransferStatus2)

            => SecureDataTransferStatus1.CompareTo(SecureDataTransferStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<SecureDataTransferStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two secure data transfer statuss.
        /// </summary>
        /// <param name="Object">A secure data transfer status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SecureDataTransferStatus secureDataTransferStatus
                   ? CompareTo(secureDataTransferStatus)
                   : throw new ArgumentException("The given object is not secure data transfer status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SecureDataTransferStatus)

        /// <summary>
        /// Compares two secure data transfer statuss.
        /// </summary>
        /// <param name="SecureDataTransferStatus">A secure data transfer status to compare with.</param>
        public Int32 CompareTo(SecureDataTransferStatus SecureDataTransferStatus)

            => String.Compare(InternalId,
                              SecureDataTransferStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SecureDataTransferStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two secure data transfer statuss for equality.
        /// </summary>
        /// <param name="Object">A secure data transfer status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecureDataTransferStatus secureDataTransferStatus &&
                   Equals(secureDataTransferStatus);

        #endregion

        #region Equals(SecureDataTransferStatus)

        /// <summary>
        /// Compares two secure data transfer statuss for equality.
        /// </summary>
        /// <param name="SecureDataTransferStatus">A secure data transfer status to compare with.</param>
        public Boolean Equals(SecureDataTransferStatus SecureDataTransferStatus)

            => String.Equals(InternalId,
                             SecureDataTransferStatus.InternalId,
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
