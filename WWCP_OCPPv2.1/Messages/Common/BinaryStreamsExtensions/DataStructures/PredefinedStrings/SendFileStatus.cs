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
    /// Extension methods for send file statuss.
    /// </summary>
    public static class SendFileStatusExtensions
    {

        /// <summary>
        /// Indicates whether this send file status is null or empty.
        /// </summary>
        /// <param name="SendFileStatus">A send file status.</param>
        public static Boolean IsNullOrEmpty(this SendFileStatus? SendFileStatus)
            => !SendFileStatus.HasValue || SendFileStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this send file status is null or empty.
        /// </summary>
        /// <param name="SendFileStatus">A send file status.</param>
        public static Boolean IsNotNullOrEmpty(this SendFileStatus? SendFileStatus)
            => SendFileStatus.HasValue && SendFileStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A send file status.
    /// </summary>
    public readonly struct SendFileStatus : IId,
                                            IEquatable<SendFileStatus>,
                                            IComparable<SendFileStatus>
    {

        #region Data

        private readonly static Dictionary<String, SendFileStatus>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                              InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this send file status is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this send file status is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the send file status.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new send file status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a send file status.</param>
        private SendFileStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static SendFileStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new SendFileStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a send file status.
        /// </summary>
        /// <param name="Text">A text representation of a send file status.</param>
        public static SendFileStatus Parse(String Text)
        {

            if (TryParse(Text, out var sendFileStatus))
                return sendFileStatus;

            throw new ArgumentException($"Invalid text representation of a send file status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as send file status.
        /// </summary>
        /// <param name="Text">A text representation of a send file status.</param>
        public static SendFileStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var sendFileStatus))
                return sendFileStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SendFileStatus)

        /// <summary>
        /// Try to parse the given text as send file status.
        /// </summary>
        /// <param name="Text">A text representation of a send file status.</param>
        /// <param name="SendFileStatus">The parsed send file status.</param>
        public static Boolean TryParse(String Text, out SendFileStatus SendFileStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out SendFileStatus))
                    SendFileStatus = Register(Text);

                return true;

            }

            SendFileStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this send file status.
        /// </summary>
        public SendFileStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The SendFileRequest has been accepted and the file is included in the response.
        /// </summary>
        public static SendFileStatus Success             { get; }
            = Register("Success");

        /// <summary>
        /// The SendFileRequest has been accepted, but the file location was not found.
        /// </summary>
        public static SendFileStatus NotFound            { get; }
            = Register("NotFound");

        /// <summary>
        /// The SendFileStatus was understood, but the file is locked.
        /// </summary>
        public static SendFileStatus Locked              { get; }
            = Register("Locked");

        /// <summary>
        /// The SendFileRequest has been rejected.
        /// </summary>
        public static SendFileStatus Rejected            { get; }
            = Register("Rejected");

        /// <summary>
        /// The digital signature(s) of the message is/are invalid.
        /// </summary>
        public static SendFileStatus InvalidSignature    { get; }
            = Register("InvalidSignature");

        #endregion


        #region Operator overloading

        #region Operator == (SendFileStatus1, SendFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SendFileStatus1">A send file status.</param>
        /// <param name="SendFileStatus2">Another send file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SendFileStatus SendFileStatus1,
                                           SendFileStatus SendFileStatus2)

            => SendFileStatus1.Equals(SendFileStatus2);

        #endregion

        #region Operator != (SendFileStatus1, SendFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SendFileStatus1">A send file status.</param>
        /// <param name="SendFileStatus2">Another send file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SendFileStatus SendFileStatus1,
                                           SendFileStatus SendFileStatus2)

            => !SendFileStatus1.Equals(SendFileStatus2);

        #endregion

        #region Operator <  (SendFileStatus1, SendFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SendFileStatus1">A send file status.</param>
        /// <param name="SendFileStatus2">Another send file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SendFileStatus SendFileStatus1,
                                          SendFileStatus SendFileStatus2)

            => SendFileStatus1.CompareTo(SendFileStatus2) < 0;

        #endregion

        #region Operator <= (SendFileStatus1, SendFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SendFileStatus1">A send file status.</param>
        /// <param name="SendFileStatus2">Another send file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SendFileStatus SendFileStatus1,
                                           SendFileStatus SendFileStatus2)

            => SendFileStatus1.CompareTo(SendFileStatus2) <= 0;

        #endregion

        #region Operator >  (SendFileStatus1, SendFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SendFileStatus1">A send file status.</param>
        /// <param name="SendFileStatus2">Another send file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SendFileStatus SendFileStatus1,
                                          SendFileStatus SendFileStatus2)

            => SendFileStatus1.CompareTo(SendFileStatus2) > 0;

        #endregion

        #region Operator >= (SendFileStatus1, SendFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SendFileStatus1">A send file status.</param>
        /// <param name="SendFileStatus2">Another send file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SendFileStatus SendFileStatus1,
                                           SendFileStatus SendFileStatus2)

            => SendFileStatus1.CompareTo(SendFileStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<SendFileStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two send file statuss.
        /// </summary>
        /// <param name="Object">A send file status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SendFileStatus sendFileStatus
                   ? CompareTo(sendFileStatus)
                   : throw new ArgumentException("The given object is not send file status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SendFileStatus)

        /// <summary>
        /// Compares two send file statuss.
        /// </summary>
        /// <param name="SendFileStatus">A send file status to compare with.</param>
        public Int32 CompareTo(SendFileStatus SendFileStatus)

            => String.Compare(InternalId,
                              SendFileStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SendFileStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two send file statuss for equality.
        /// </summary>
        /// <param name="Object">A send file status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendFileStatus sendFileStatus &&
                   Equals(sendFileStatus);

        #endregion

        #region Equals(SendFileStatus)

        /// <summary>
        /// Compares two send file statuss for equality.
        /// </summary>
        /// <param name="SendFileStatus">A send file status to compare with.</param>
        public Boolean Equals(SendFileStatus SendFileStatus)

            => String.Equals(InternalId,
                             SendFileStatus.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
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
