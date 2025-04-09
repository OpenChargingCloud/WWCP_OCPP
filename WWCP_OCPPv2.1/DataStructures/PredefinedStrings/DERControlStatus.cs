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
    public static class DERControlStatusExtensions
    {

        /// <summary>
        /// Indicates whether this DER control status is null or empty.
        /// </summary>
        /// <param name="DERControlStatus">A DER control status.</param>
        public static Boolean IsNullOrEmpty(this DERControlStatus? DERControlStatus)
            => !DERControlStatus.HasValue || DERControlStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this DER control status is null or empty.
        /// </summary>
        /// <param name="DERControlStatus">A DER control status.</param>
        public static Boolean IsNotNullOrEmpty(this DERControlStatus? DERControlStatus)
            => DERControlStatus.HasValue && DERControlStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A Distributed Energy Resource (DER) control status.
    /// </summary>
    public readonly struct DERControlStatus : IId,
                                              IEquatable<DERControlStatus>,
                                              IComparable<DERControlStatus>
    {

        #region Data

        private readonly static Dictionary<String, DERControlStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this DER control status is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this DER control status is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the DER control status.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered reset types.
        /// </summary>
        public static    IEnumerable<DERControlStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DER control status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a DER control status.</param>
        private DERControlStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static DERControlStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new DERControlStatus(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a DER control status.
        /// </summary>
        /// <param name="Text">A text representation of a DER control status.</param>
        public static DERControlStatus Parse(String Text)
        {

            if (TryParse(Text, out var derControlStatus))
                return derControlStatus;

            throw new ArgumentException($"Invalid text representation of a DER control status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a DER control status.
        /// </summary>
        /// <param name="Text">A text representation of a DER control status.</param>
        public static DERControlStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var derControlStatus))
                return derControlStatus;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out DERControlStatus)

        /// <summary>
        /// Try to parse the given text as a DER control status.
        /// </summary>
        /// <param name="Text">A text representation of a DER control status.</param>
        /// <param name="DERControlStatus">The parsed DER control status.</param>
        public static Boolean TryParse (String                                      Text,
                                        [NotNullWhen(true)] out DERControlStatus  DERControlStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out DERControlStatus))
                    DERControlStatus = Register(Text);

                return true;

            }

            DERControlStatus = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out DERControlStatus)

        /// <summary>
        /// Check whether the given text is a defined DER control status.
        /// </summary>
        /// <param name="Text">A text representation of a DER control status.</param>
        /// <param name="DERControlStatus">The validated DER control status.</param>
        public static Boolean IsDefined(String                                     Text,
                                       [NotNullWhen(true)] out DERControlStatus  DERControlStatus)

            => lookup.TryGetValue(Text.Trim(), out DERControlStatus);

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this DER control status.
        /// </summary>
        public DERControlStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static DERControlStatus  Accepted          { get; }
            = Register("Accepted");

        /// <summary>
        /// Rejected
        /// </summary>
        public static DERControlStatus  Rejected          { get; }
            = Register("Rejected");

        /// <summary>
        /// NotSupported
        /// </summary>
        public static DERControlStatus  NotSupported      { get; }
            = Register("NotSupported");

        /// <summary>
        /// NotFound
        /// </summary>
        public static DERControlStatus  NotFound          { get; }
            = Register("NotFound");


        /// <summary>
        /// Error
        /// </summary>
        public static DERControlStatus  Error             { get; }
            = Register("Error");

        /// <summary>
        /// Signature Error
        /// </summary>
        public static DERControlStatus  SignatureError    { get; }
            = Register("SignatureError");

        #endregion


        #region Operator overloading

        #region Operator == (DERControlStatus1, DERControlStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlStatus1">A DER control status.</param>
        /// <param name="DERControlStatus2">Another DER control status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERControlStatus DERControlStatus1,
                                           DERControlStatus DERControlStatus2)

            => DERControlStatus1.Equals(DERControlStatus2);

        #endregion

        #region Operator != (DERControlStatus1, DERControlStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlStatus1">A DER control status.</param>
        /// <param name="DERControlStatus2">Another DER control status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERControlStatus DERControlStatus1,
                                           DERControlStatus DERControlStatus2)

            => !DERControlStatus1.Equals(DERControlStatus2);

        #endregion

        #region Operator <  (DERControlStatus1, DERControlStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlStatus1">A DER control status.</param>
        /// <param name="DERControlStatus2">Another DER control status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DERControlStatus DERControlStatus1,
                                          DERControlStatus DERControlStatus2)

            => DERControlStatus1.CompareTo(DERControlStatus2) < 0;

        #endregion

        #region Operator <= (DERControlStatus1, DERControlStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlStatus1">A DER control status.</param>
        /// <param name="DERControlStatus2">Another DER control status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DERControlStatus DERControlStatus1,
                                           DERControlStatus DERControlStatus2)

            => DERControlStatus1.CompareTo(DERControlStatus2) <= 0;

        #endregion

        #region Operator >  (DERControlStatus1, DERControlStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlStatus1">A DER control status.</param>
        /// <param name="DERControlStatus2">Another DER control status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DERControlStatus DERControlStatus1,
                                          DERControlStatus DERControlStatus2)

            => DERControlStatus1.CompareTo(DERControlStatus2) > 0;

        #endregion

        #region Operator >= (DERControlStatus1, DERControlStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlStatus1">A DER control status.</param>
        /// <param name="DERControlStatus2">Another DER control status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DERControlStatus DERControlStatus1,
                                           DERControlStatus DERControlStatus2)

            => DERControlStatus1.CompareTo(DERControlStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<DERControlStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two DER control status.
        /// </summary>
        /// <param name="Object">A DER control status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DERControlStatus derControlStatus
                   ? CompareTo(derControlStatus)
                   : throw new ArgumentException("The given object is not a DER control status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DERControlStatus)

        /// <summary>
        /// Compares two DER control status.
        /// </summary>
        /// <param name="DERControlStatus">A DER control status to compare with.</param>
        public Int32 CompareTo(DERControlStatus DERControlStatus)

            => String.Compare(InternalId,
                              DERControlStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<DERControlStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER control status for equality.
        /// </summary>
        /// <param name="Object">A DER control status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERControlStatus derControlStatus &&
                   Equals(derControlStatus);

        #endregion

        #region Equals(DERControlStatus)

        /// <summary>
        /// Compares two DER control status for equality.
        /// </summary>
        /// <param name="DERControlStatus">A DER control status to compare with.</param>
        public Boolean Equals(DERControlStatus DERControlStatus)

            => String.Equals(InternalId,
                             DERControlStatus.InternalId,
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
