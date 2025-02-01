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
    /// Extension methods for delete certificate status.
    /// </summary>
    public static class DeleteCertificateStatusExtensions
    {

        /// <summary>
        /// Indicates whether this delete certificate status is null or empty.
        /// </summary>
        /// <param name="DeleteCertificateStatus">A delete certificate status.</param>
        public static Boolean IsNullOrEmpty(this DeleteCertificateStatus? DeleteCertificateStatus)
            => !DeleteCertificateStatus.HasValue || DeleteCertificateStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this delete certificate status is null or empty.
        /// </summary>
        /// <param name="DeleteCertificateStatus">A delete certificate status.</param>
        public static Boolean IsNotNullOrEmpty(this DeleteCertificateStatus? DeleteCertificateStatus)
            => DeleteCertificateStatus.HasValue && DeleteCertificateStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A delete certificate status.
    /// </summary>
    public readonly struct DeleteCertificateStatus : IId,
                                                       IEquatable<DeleteCertificateStatus>,
                                                       IComparable<DeleteCertificateStatus>
    {

        #region Data

        private readonly static Dictionary<String, DeleteCertificateStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this delete certificate status is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this delete certificate status is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the delete certificate status.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered reset types.
        /// </summary>
        public static    IEnumerable<DeleteCertificateStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new delete certificate status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a delete certificate status.</param>
        private DeleteCertificateStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static DeleteCertificateStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new DeleteCertificateStatus(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a delete certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a delete certificate status.</param>
        public static DeleteCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var deleteCertificateStatus))
                return deleteCertificateStatus;

            throw new ArgumentException($"Invalid text representation of a delete certificate status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a delete certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a delete certificate status.</param>
        public static DeleteCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var deleteCertificateStatus))
                return deleteCertificateStatus;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out DeleteCertificateStatus)

        /// <summary>
        /// Try to parse the given text as a delete certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a delete certificate status.</param>
        /// <param name="DeleteCertificateStatus">The parsed delete certificate status.</param>
        public static Boolean TryParse (String                                      Text,
                                        [NotNullWhen(true)] out DeleteCertificateStatus  DeleteCertificateStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out DeleteCertificateStatus))
                    DeleteCertificateStatus = Register(Text);

                return true;

            }

            DeleteCertificateStatus = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out DeleteCertificateStatus)

        /// <summary>
        /// Check whether the given text is a defined delete certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a delete certificate status.</param>
        /// <param name="DeleteCertificateStatus">The validated delete certificate status.</param>
        public static Boolean IsDefined(String                                     Text,
                                       [NotNullWhen(true)] out DeleteCertificateStatus  DeleteCertificateStatus)

            => lookup.TryGetValue(Text.Trim(), out DeleteCertificateStatus);

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this delete certificate status.
        /// </summary>
        public DeleteCertificateStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static DeleteCertificateStatus  Accepted          { get; }
            = Register("Accepted");

        /// <summary>
        /// Failed
        /// </summary>
        public static DeleteCertificateStatus  Failed            { get; }
            = Register("Failed");

        /// <summary>
        /// NotFound
        /// </summary>
        public static DeleteCertificateStatus  NotFound          { get; }
            = Register("NotFound");


        public static DeleteCertificateStatus  Error             { get; }
            = Register("Error");

        public static DeleteCertificateStatus  SignatureError    { get; }
            = Register("SignatureError");

        #endregion


        #region Operator overloading

        #region Operator == (DeleteCertificateStatus1, DeleteCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteCertificateStatus1">A delete certificate status.</param>
        /// <param name="DeleteCertificateStatus2">Another delete certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DeleteCertificateStatus DeleteCertificateStatus1,
                                           DeleteCertificateStatus DeleteCertificateStatus2)

            => DeleteCertificateStatus1.Equals(DeleteCertificateStatus2);

        #endregion

        #region Operator != (DeleteCertificateStatus1, DeleteCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteCertificateStatus1">A delete certificate status.</param>
        /// <param name="DeleteCertificateStatus2">Another delete certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DeleteCertificateStatus DeleteCertificateStatus1,
                                           DeleteCertificateStatus DeleteCertificateStatus2)

            => !DeleteCertificateStatus1.Equals(DeleteCertificateStatus2);

        #endregion

        #region Operator <  (DeleteCertificateStatus1, DeleteCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteCertificateStatus1">A delete certificate status.</param>
        /// <param name="DeleteCertificateStatus2">Another delete certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DeleteCertificateStatus DeleteCertificateStatus1,
                                          DeleteCertificateStatus DeleteCertificateStatus2)

            => DeleteCertificateStatus1.CompareTo(DeleteCertificateStatus2) < 0;

        #endregion

        #region Operator <= (DeleteCertificateStatus1, DeleteCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteCertificateStatus1">A delete certificate status.</param>
        /// <param name="DeleteCertificateStatus2">Another delete certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DeleteCertificateStatus DeleteCertificateStatus1,
                                           DeleteCertificateStatus DeleteCertificateStatus2)

            => DeleteCertificateStatus1.CompareTo(DeleteCertificateStatus2) <= 0;

        #endregion

        #region Operator >  (DeleteCertificateStatus1, DeleteCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteCertificateStatus1">A delete certificate status.</param>
        /// <param name="DeleteCertificateStatus2">Another delete certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DeleteCertificateStatus DeleteCertificateStatus1,
                                          DeleteCertificateStatus DeleteCertificateStatus2)

            => DeleteCertificateStatus1.CompareTo(DeleteCertificateStatus2) > 0;

        #endregion

        #region Operator >= (DeleteCertificateStatus1, DeleteCertificateStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DeleteCertificateStatus1">A delete certificate status.</param>
        /// <param name="DeleteCertificateStatus2">Another delete certificate status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DeleteCertificateStatus DeleteCertificateStatus1,
                                           DeleteCertificateStatus DeleteCertificateStatus2)

            => DeleteCertificateStatus1.CompareTo(DeleteCertificateStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<DeleteCertificateStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two delete certificate status.
        /// </summary>
        /// <param name="Object">A delete certificate status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DeleteCertificateStatus deleteCertificateStatus
                   ? CompareTo(deleteCertificateStatus)
                   : throw new ArgumentException("The given object is not a delete certificate status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DeleteCertificateStatus)

        /// <summary>
        /// Compares two delete certificate status.
        /// </summary>
        /// <param name="DeleteCertificateStatus">A delete certificate status to compare with.</param>
        public Int32 CompareTo(DeleteCertificateStatus DeleteCertificateStatus)

            => String.Compare(InternalId,
                              DeleteCertificateStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<DeleteCertificateStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two delete certificate status for equality.
        /// </summary>
        /// <param name="Object">A delete certificate status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteCertificateStatus deleteCertificateStatus &&
                   Equals(deleteCertificateStatus);

        #endregion

        #region Equals(DeleteCertificateStatus)

        /// <summary>
        /// Compares two delete certificate status for equality.
        /// </summary>
        /// <param name="DeleteCertificateStatus">A delete certificate status to compare with.</param>
        public Boolean Equals(DeleteCertificateStatus DeleteCertificateStatus)

            => String.Equals(InternalId,
                             DeleteCertificateStatus.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
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
