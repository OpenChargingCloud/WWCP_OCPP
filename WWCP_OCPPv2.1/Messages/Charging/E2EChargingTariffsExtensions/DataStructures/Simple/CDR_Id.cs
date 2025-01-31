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
    /// Extension methods for charge detail record identifications.
    /// </summary>
    public static class CDRIdExtensions
    {

        /// <summary>
        /// Indicates whether this charge detail record identification is null or empty.
        /// </summary>
        /// <param name="CDRId">A charge detail record identification.</param>
        public static Boolean IsNullOrEmpty(this CDR_Id? CDRId)
            => !CDRId.HasValue || CDRId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charge detail record identification is NOT null or empty.
        /// </summary>
        /// <param name="CDRId">A charge detail record identification.</param>
        public static Boolean IsNotNullOrEmpty(this CDR_Id? CDRId)
            => CDRId.HasValue && CDRId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a charge detail record.
    /// </summary>
    public readonly struct CDR_Id : IId<CDR_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this charge detail record identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charge detail record identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the charge detail record identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a charge detail record identification.</param>
        private CDR_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(ProviderId, Length = 30)

        /// <summary>
        /// Create a new random charge detail record identification based
        /// on the given e-mobility provider identification,
        /// the current timestamp and a random number of the given length.
        /// </summary>
        /// <param name="ProviderId">An e-mobility provider identification.</param>
        /// <param name="Length">The expected length of the charge detail record identification.</param>
        public static CDR_Id NewRandom(Provider_Id  ProviderId,
                                                  Byte         Length = 30)

            => new (String.Concat(
                        ProviderId,                      "T",
                        Timestamp.Now.ToUnixTimestamp(), "*",
                        RandomExtensions.RandomString(Length)
                    ));

        #endregion

        #region (static) Generate (ProviderId, Suffix, CreationTimestamp = null)

        /// <summary>
        /// Create a new charge detail record identification based
        /// on the given e-mobility provider identification,
        /// the current timestamp and a given suffix.
        /// </summary>
        /// <param name="ProviderId">An e-mobility provider identification.</param>
        /// <param name="Suffix">The suffix of the charge detail record identification.</param>
        /// <param name="CreationTimestamp">An optional creation timestamp.</param>
        public static CDR_Id Generate(Provider_Id  ProviderId,
                                                 String       Suffix,
                                                 DateTime?    CreationTimestamp   = null)

            => new ($"{ProviderId}T{(CreationTimestamp ?? Timestamp.Now).ToUnixTimestamp()}*{Suffix}");

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge detail record identification.</param>
        public static CDR_Id Parse(String Text)
        {

            if (TryParse(Text, out var tariffId))
                return tariffId;

            throw new ArgumentException($"Invalid text representation of a charge detail record identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge detail record identification.</param>
        public static CDR_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffId))
                return tariffId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out CDRId)

        /// <summary>
        /// Try to parse the given text as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge detail record identification.</param>
        /// <param name="CDRId">The parsed charge detail record identification.</param>
        public static Boolean TryParse(String Text, out CDR_Id CDRId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CDRId = new CDR_Id(Text);
                    return true;
                }
                catch
                { }
            }

            CDRId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charge detail record identification.
        /// </summary>
        public CDR_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => CDRId1.Equals(CDRId2);

        #endregion

        #region Operator != (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => !CDRId1.Equals(CDRId2);

        #endregion

        #region Operator <  (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR_Id CDRId1,
                                          CDR_Id CDRId2)

            => CDRId1.CompareTo(CDRId2) < 0;

        #endregion

        #region Operator <= (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => CDRId1.CompareTo(CDRId2) <= 0;

        #endregion

        #region Operator >  (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR_Id CDRId1,
                                          CDR_Id CDRId2)

            => CDRId1.CompareTo(CDRId2) > 0;

        #endregion

        #region Operator >= (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A charge detail record identification.</param>
        /// <param name="CDRId2">Another charge detail record identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDR_Id CDRId1,
                                           CDR_Id CDRId2)

            => CDRId1.CompareTo(CDRId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CDRId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charge detail record identifications.
        /// </summary>
        /// <param name="Object">A charge detail record identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDR_Id tariffId
                   ? CompareTo(tariffId)
                   : throw new ArgumentException("The given object is not a charge detail record identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRId)

        /// <summary>
        /// Compares two charge detail record identifications.
        /// </summary>
        /// <param name="CDRId">A charge detail record identification to compare with.</param>
        public Int32 CompareTo(CDR_Id CDRId)

            => String.Compare(InternalId,
                              CDRId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CDRId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charge detail record identifications for equality.
        /// </summary>
        /// <param name="Object">A charge detail record identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDR_Id tariffId &&
                   Equals(tariffId);

        #endregion

        #region Equals(CDRId)

        /// <summary>
        /// Compares two charge detail record identifications for equality.
        /// </summary>
        /// <param name="CDRId">A charge detail record identification to compare with.</param>
        public Boolean Equals(CDR_Id CDRId)

            => String.Equals(InternalId,
                             CDRId.InternalId,
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
