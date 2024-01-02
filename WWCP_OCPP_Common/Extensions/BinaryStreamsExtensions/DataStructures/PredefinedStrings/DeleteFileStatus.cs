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
    /// Extension methods for delete file statuss.
    /// </summary>
    public static class DeleteFileStatusExtensions
    {

        /// <summary>
        /// Indicates whether this delete file status is null or empty.
        /// </summary>
        /// <param name="DeleteFileStatus">A delete file status.</param>
        public static Boolean IsNullOrEmpty(this DeleteFileStatus? DeleteFileStatus)
            => !DeleteFileStatus.HasValue || DeleteFileStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this delete file status is null or empty.
        /// </summary>
        /// <param name="DeleteFileStatus">A delete file status.</param>
        public static Boolean IsNotNullOrEmpty(this DeleteFileStatus? DeleteFileStatus)
            => DeleteFileStatus.HasValue && DeleteFileStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A delete file status.
    /// </summary>
    public readonly struct DeleteFileStatus : IId,
                                              IEquatable<DeleteFileStatus>,
                                              IComparable<DeleteFileStatus>
    {

        #region Data

        private readonly static Dictionary<String, DeleteFileStatus>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                                InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this delete file status is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this delete file status is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the delete file status.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new delete file status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a delete file status.</param>
        private DeleteFileStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static DeleteFileStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new DeleteFileStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a delete file status.
        /// </summary>
        /// <param name="Text">A text representation of a delete file status.</param>
        public static DeleteFileStatus Parse(String Text)
        {

            if (TryParse(Text, out var deleteFileStatus))
                return deleteFileStatus;

            throw new ArgumentException($"Invalid text representation of a delete file status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as delete file status.
        /// </summary>
        /// <param name="Text">A text representation of a delete file status.</param>
        public static DeleteFileStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var deleteFileStatus))
                return deleteFileStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out DeleteFileStatus)

        /// <summary>
        /// Try to parse the given text as delete file status.
        /// </summary>
        /// <param name="Text">A text representation of a delete file status.</param>
        /// <param name="DeleteFileStatus">The parsed delete file status.</param>
        public static Boolean TryParse(String Text, out DeleteFileStatus DeleteFileStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out DeleteFileStatus))
                    DeleteFileStatus = Register(Text);

                return true;

            }

            DeleteFileStatus = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this delete file status.
        /// </summary>
        public DeleteFileStatus Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The DeleteFileRequest has been accepted and the file is included in the response.
        /// </summary>
        public static DeleteFileStatus Success             { get; }
            = Register("Success");

        /// <summary>
        /// The DeleteFileRequest has been accepted, but the file was not found.
        /// </summary>
        public static DeleteFileStatus NotFound            { get; }
            = Register("NotFound");

        /// <summary>
        /// The DeleteFileRequest was understood, but the file is locked.
        /// </summary>
        public static DeleteFileStatus Locked              { get; }
            = Register("Locked");

        /// <summary>
        /// The DeleteFileRequest has been rejected.
        /// </summary>
        public static DeleteFileStatus Rejected            { get; }
            = Register("Rejected");

        /// <summary>
        /// The digital signature(s) of the message is/are invalid.
        /// </summary>
        public static DeleteFileStatus InvalidSignature    { get; }
            = Register("InvalidSignature");

        #endregion


        #region Operator overloading

        #region Operator == (DeleteFileStatus1, DeleteFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteFileStatus1">A delete file status.</param>
        /// <param name="DeleteFileStatus2">Another delete file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DeleteFileStatus DeleteFileStatus1,
                                           DeleteFileStatus DeleteFileStatus2)

            => DeleteFileStatus1.Equals(DeleteFileStatus2);

        #endregion

        #region Operator != (DeleteFileStatus1, DeleteFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteFileStatus1">A delete file status.</param>
        /// <param name="DeleteFileStatus2">Another delete file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DeleteFileStatus DeleteFileStatus1,
                                           DeleteFileStatus DeleteFileStatus2)

            => !DeleteFileStatus1.Equals(DeleteFileStatus2);

        #endregion

        #region Operator <  (DeleteFileStatus1, DeleteFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteFileStatus1">A delete file status.</param>
        /// <param name="DeleteFileStatus2">Another delete file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DeleteFileStatus DeleteFileStatus1,
                                          DeleteFileStatus DeleteFileStatus2)

            => DeleteFileStatus1.CompareTo(DeleteFileStatus2) < 0;

        #endregion

        #region Operator <= (DeleteFileStatus1, DeleteFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteFileStatus1">A delete file status.</param>
        /// <param name="DeleteFileStatus2">Another delete file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DeleteFileStatus DeleteFileStatus1,
                                           DeleteFileStatus DeleteFileStatus2)

            => DeleteFileStatus1.CompareTo(DeleteFileStatus2) <= 0;

        #endregion

        #region Operator >  (DeleteFileStatus1, DeleteFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteFileStatus1">A delete file status.</param>
        /// <param name="DeleteFileStatus2">Another delete file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DeleteFileStatus DeleteFileStatus1,
                                          DeleteFileStatus DeleteFileStatus2)

            => DeleteFileStatus1.CompareTo(DeleteFileStatus2) > 0;

        #endregion

        #region Operator >= (DeleteFileStatus1, DeleteFileStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteFileStatus1">A delete file status.</param>
        /// <param name="DeleteFileStatus2">Another delete file status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DeleteFileStatus DeleteFileStatus1,
                                           DeleteFileStatus DeleteFileStatus2)

            => DeleteFileStatus1.CompareTo(DeleteFileStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<DeleteFileStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two delete file statuss.
        /// </summary>
        /// <param name="Object">A delete file status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DeleteFileStatus deleteFileStatus
                   ? CompareTo(deleteFileStatus)
                   : throw new ArgumentException("The given object is not delete file status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DeleteFileStatus)

        /// <summary>
        /// Compares two delete file statuss.
        /// </summary>
        /// <param name="DeleteFileStatus">A delete file status to compare with.</param>
        public Int32 CompareTo(DeleteFileStatus DeleteFileStatus)

            => String.Compare(InternalId,
                              DeleteFileStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<DeleteFileStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two delete file statuss for equality.
        /// </summary>
        /// <param name="Object">A delete file status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteFileStatus deleteFileStatus &&
                   Equals(deleteFileStatus);

        #endregion

        #region Equals(DeleteFileStatus)

        /// <summary>
        /// Compares two delete file statuss for equality.
        /// </summary>
        /// <param name="DeleteFileStatus">A delete file status to compare with.</param>
        public Boolean Equals(DeleteFileStatus DeleteFileStatus)

            => String.Equals(InternalId,
                             DeleteFileStatus.InternalId,
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
