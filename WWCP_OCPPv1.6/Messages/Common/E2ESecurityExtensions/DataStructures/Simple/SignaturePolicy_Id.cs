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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for signature policy identifications.
    /// </summary>
    public static class SignaturePolicyIdExtensions
    {

        /// <summary>
        /// Indicates whether this signature policy identification is null or empty.
        /// </summary>
        /// <param name="SignaturePolicyId">A signature policy identification.</param>
        public static Boolean IsNullOrEmpty(this SignaturePolicy_Id? SignaturePolicyId)
            => !SignaturePolicyId.HasValue || SignaturePolicyId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this signature policy identification is null or empty.
        /// </summary>
        /// <param name="SignaturePolicyId">A signature policy identification.</param>
        public static Boolean IsNotNullOrEmpty(this SignaturePolicy_Id? SignaturePolicyId)
            => SignaturePolicyId.HasValue && SignaturePolicyId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A signature policy identification.
    /// </summary>
    public readonly struct SignaturePolicy_Id : IId,
                                                IEquatable<SignaturePolicy_Id>,
                                                IComparable<SignaturePolicy_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the signature policy identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signature policy identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a signature policy identification.</param>
        private SignaturePolicy_Id(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        #region (static) Zero

        /// <summary>
        /// Zero
        /// </summary>
        public static SignaturePolicy_Id Zero
            => Parse("0");

        #endregion

        #region (static) NewRandom(Length = 30)

        /// <summary>
        /// Create a new random signature policy identification.
        /// </summary>
        /// <param name="Length">The expected length of the signature policy identification.</param>
        public static SignaturePolicy_Id NewRandom(Byte Length = 30)
            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a signature policy identification.
        /// </summary>
        /// <param name="Text">A text representation of a signature policy identification.</param>
        public static SignaturePolicy_Id Parse(String Text)
        {

            if (TryParse(Text, out var signaturePolicyId))
                return signaturePolicyId;

            throw new ArgumentException($"Invalid text representation of a signature policy identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a signature policy identification.
        /// </summary>
        /// <param name="Text">A text representation of a signature policy identification.</param>
        public static SignaturePolicy_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var signaturePolicyId))
                return signaturePolicyId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SignaturePolicyId)

        /// <summary>
        /// Try to parse the given text as a signature policy identification.
        /// </summary>
        /// <param name="Text">A text representation of a signature policy identification.</param>
        /// <param name="SignaturePolicyId">The parsed signature policy identification.</param>
        public static Boolean TryParse(String Text, out SignaturePolicy_Id SignaturePolicyId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                SignaturePolicyId = new SignaturePolicy_Id(Text);
                return true;
            }

            SignaturePolicyId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this signature policy identification.
        /// </summary>
        public SignaturePolicy_Id Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (SignaturePolicyId1, SignaturePolicyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyId1">A signature policy identification.</param>
        /// <param name="SignaturePolicyId2">Another signature policy identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignaturePolicy_Id SignaturePolicyId1,
                                           SignaturePolicy_Id SignaturePolicyId2)

            => SignaturePolicyId1.Equals(SignaturePolicyId2);

        #endregion

        #region Operator != (SignaturePolicyId1, SignaturePolicyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyId1">A signature policy identification.</param>
        /// <param name="SignaturePolicyId2">Another signature policy identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignaturePolicy_Id SignaturePolicyId1,
                                           SignaturePolicy_Id SignaturePolicyId2)

            => !SignaturePolicyId1.Equals(SignaturePolicyId2);

        #endregion

        #region Operator <  (SignaturePolicyId1, SignaturePolicyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyId1">A signature policy identification.</param>
        /// <param name="SignaturePolicyId2">Another signature policy identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SignaturePolicy_Id SignaturePolicyId1,
                                          SignaturePolicy_Id SignaturePolicyId2)

            => SignaturePolicyId1.CompareTo(SignaturePolicyId2) < 0;

        #endregion

        #region Operator <= (SignaturePolicyId1, SignaturePolicyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyId1">A signature policy identification.</param>
        /// <param name="SignaturePolicyId2">Another signature policy identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SignaturePolicy_Id SignaturePolicyId1,
                                           SignaturePolicy_Id SignaturePolicyId2)

            => SignaturePolicyId1.CompareTo(SignaturePolicyId2) <= 0;

        #endregion

        #region Operator >  (SignaturePolicyId1, SignaturePolicyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyId1">A signature policy identification.</param>
        /// <param name="SignaturePolicyId2">Another signature policy identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SignaturePolicy_Id SignaturePolicyId1,
                                          SignaturePolicy_Id SignaturePolicyId2)

            => SignaturePolicyId1.CompareTo(SignaturePolicyId2) > 0;

        #endregion

        #region Operator >= (SignaturePolicyId1, SignaturePolicyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyId1">A signature policy identification.</param>
        /// <param name="SignaturePolicyId2">Another signature policy identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SignaturePolicy_Id SignaturePolicyId1,
                                           SignaturePolicy_Id SignaturePolicyId2)

            => SignaturePolicyId1.CompareTo(SignaturePolicyId2) >= 0;

        #endregion

        #endregion

        #region IComparable<SignaturePolicyId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two signature policy identifications.
        /// </summary>
        /// <param name="Object">A signature policy identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SignaturePolicy_Id signaturePolicyId
                   ? CompareTo(signaturePolicyId)
                   : throw new ArgumentException("The given object is not a signature policy identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SignaturePolicyId)

        /// <summary>
        /// Compares two signature policy identifications.
        /// </summary>
        /// <param name="SignaturePolicyId">A signature policy identification to compare with.</param>
        public Int32 CompareTo(SignaturePolicy_Id SignaturePolicyId)

            => String.Compare(InternalId,
                              SignaturePolicyId.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<SignaturePolicyId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signature policy identifications for equality.
        /// </summary>
        /// <param name="Object">A signature policy identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignaturePolicy_Id signaturePolicyId &&
                   Equals(signaturePolicyId);

        #endregion

        #region Equals(SignaturePolicyId)

        /// <summary>
        /// Compares two signature policy identifications for equality.
        /// </summary>
        /// <param name="SignaturePolicyId">A signature policy identification to compare with.</param>
        public Boolean Equals(SignaturePolicy_Id SignaturePolicyId)

            => String.Equals(InternalId,
                             SignaturePolicyId.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
