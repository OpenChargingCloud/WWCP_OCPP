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
    /// Extension methods for get file statuss.
    /// </summary>
    public static class GetFileStatusExtensions
    {

        /// <summary>
        /// Indicates whether this get file status is null or empty.
        /// </summary>
        /// <param name="GetFileStatus">A get file status.</param>
        public static Boolean IsNullOrEmpty(this GetFileStatus? GetFileStatus)
            => !GetFileStatus.HasValue || GetFileStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this get file status is null or empty.
        /// </summary>
        /// <param name="GetFileStatus">A get file status.</param>
        public static Boolean IsNotNullOrEmpty(this GetFileStatus? GetFileStatus)
            => GetFileStatus.HasValue && GetFileStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A get file status.
    /// </summary>
    public readonly struct GetFileStatus : IId,
                                           IEquatable<GetFileStatus>,
                                           IComparable<GetFileStatus>
    {

        #region Data

        private readonly static Dictionary<String, GetFileStatus>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this get file status is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this get file status is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the get file status.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get file status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a get file status.</param>
        private GetFileStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static GetFileStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new GetFileStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a get file status.
        /// </summary>
        /// <param name="Text">A text representation of a get file status.</param>
        public static GetFileStatus Parse(String Text)
        {

            if (TryParse(Text, out var getFileStatus))
                return getFileStatus;

            throw new ArgumentException($"Invalid text representation of a get file status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as get file status.
        /// </summary>
        /// <param name="Text">A text representation of a get file status.</param>
        public static GetFileStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var getFileStatus))
                return getFileStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out GetFileStatus)

        /// <summary>
        /// Try to parse the given text as get file status.
        /// </summary>
        /// <param name="Text">A text representation of a get file status.</param>
        /// <param name="GetFileStatus">The parsed get file status.</param>
        public static Boolean TryParse(String Text, out GetFileStatus GetFileStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out GetFileStatus))
                    GetFileStatus = Register(Text);

                return true;

            }

            GetFileStatus = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this get file status.
        /// </summary>
        public GetFileStatus Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The GetFileRequest has been accepted and the file is included in the response.
        /// </summary>
        public static GetFileStatus Success             { get; }
            = Register("Success");

        /// <summary>
        /// The GetFileRequest has been accepted, but the file was not found.
        /// </summary>
        public static GetFileStatus NotFound            { get; }
            = Register("NotFound");

        /// <summary>
        /// The GetFileRequest has been rejected.
        /// </summary>
        public static GetFileStatus Rejected            { get; }
            = Register("Rejected");

        /// <summary>
        /// The digital signature(s) of the message is/are invalid.
        /// </summary>
        public static GetFileStatus InvalidSignature    { get; }
            = Register("InvalidSignature");

        #endregion


        #region Operator overloading

        #region Operator == (GetFileStatus1, GetFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetFileStatus1">A get file status.</param>
        /// <param name="GetFileStatus2">Another get file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GetFileStatus GetFileStatus1,
                                           GetFileStatus GetFileStatus2)

            => GetFileStatus1.Equals(GetFileStatus2);

        #endregion

        #region Operator != (GetFileStatus1, GetFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetFileStatus1">A get file status.</param>
        /// <param name="GetFileStatus2">Another get file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GetFileStatus GetFileStatus1,
                                           GetFileStatus GetFileStatus2)

            => !GetFileStatus1.Equals(GetFileStatus2);

        #endregion

        #region Operator <  (GetFileStatus1, GetFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetFileStatus1">A get file status.</param>
        /// <param name="GetFileStatus2">Another get file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (GetFileStatus GetFileStatus1,
                                          GetFileStatus GetFileStatus2)

            => GetFileStatus1.CompareTo(GetFileStatus2) < 0;

        #endregion

        #region Operator <= (GetFileStatus1, GetFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetFileStatus1">A get file status.</param>
        /// <param name="GetFileStatus2">Another get file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (GetFileStatus GetFileStatus1,
                                           GetFileStatus GetFileStatus2)

            => GetFileStatus1.CompareTo(GetFileStatus2) <= 0;

        #endregion

        #region Operator >  (GetFileStatus1, GetFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetFileStatus1">A get file status.</param>
        /// <param name="GetFileStatus2">Another get file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (GetFileStatus GetFileStatus1,
                                          GetFileStatus GetFileStatus2)

            => GetFileStatus1.CompareTo(GetFileStatus2) > 0;

        #endregion

        #region Operator >= (GetFileStatus1, GetFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetFileStatus1">A get file status.</param>
        /// <param name="GetFileStatus2">Another get file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (GetFileStatus GetFileStatus1,
                                           GetFileStatus GetFileStatus2)

            => GetFileStatus1.CompareTo(GetFileStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<GetFileStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two get file statuss.
        /// </summary>
        /// <param name="Object">A get file status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is GetFileStatus getFileStatus
                   ? CompareTo(getFileStatus)
                   : throw new ArgumentException("The given object is not get file status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(GetFileStatus)

        /// <summary>
        /// Compares two get file statuss.
        /// </summary>
        /// <param name="GetFileStatus">A get file status to compare with.</param>
        public Int32 CompareTo(GetFileStatus GetFileStatus)

            => String.Compare(InternalId,
                              GetFileStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<GetFileStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get file statuss for equality.
        /// </summary>
        /// <param name="Object">A get file status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetFileStatus getFileStatus &&
                   Equals(getFileStatus);

        #endregion

        #region Equals(GetFileStatus)

        /// <summary>
        /// Compares two get file statuss for equality.
        /// </summary>
        /// <param name="GetFileStatus">A get file status to compare with.</param>
        public Boolean Equals(GetFileStatus GetFileStatus)

            => String.Equals(InternalId,
                             GetFileStatus.InternalId,
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
