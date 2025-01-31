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

using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for charging pool identifications.
    /// </summary>
    public static class ChargingPoolIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging pool identification is null or empty.
        /// </summary>
        /// <param name="ChargingPoolId">A charging pool identification.</param>
        public static Boolean IsNullOrEmpty(this ChargingPool_Id? ChargingPoolId)
            => !ChargingPoolId.HasValue || ChargingPoolId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging pool identification is null or empty.
        /// </summary>
        /// <param name="ChargingPoolId">A charging pool identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingPool_Id? ChargingPoolId)
            => ChargingPoolId.HasValue && ChargingPoolId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a charging pool.
    /// </summary>
    public readonly struct ChargingPool_Id : IId<ChargingPool_Id>
    {

        #region Data

        /// <summary>
        /// The regular expression for parsing an charging pool identification:
        /// ^([A-Za-z]{2}\*?[A-Za-z0-9]{3}(\*?)P[A-Za-z0-9\*]{1,30})$
        /// </summary>
        public static readonly Regex ChargingPoolId_RegEx = new (@"^([A-Za-z]{2}\*?[A-Za-z0-9]{3})(\*?)P([A-Za-z0-9\*]{1,30})$",
                                                                 RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Properties

        /// <summary>
        /// The raw/unparsed Electric Vehicle Supply Equipment (charging pool) identification.
        /// </summary>
        public String       RAW           { get; }

        /// <summary>
        /// The charging pool operator identification.
        /// </summary>
        public CSOOperator_Id  OperatorId    { get; }

        /// <summary>
        /// Whether to use the optional separator "*".
        /// </summary>
        public Char?        Separator      { get; }

        /// <summary>
        /// The suffix of the charging pool identification.
        /// </summary>
        public String       Suffix        { get; }


        /// <summary>
        /// Indicates whether this charging pool identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => Suffix.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging pool identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => Suffix.IsNotNullOrEmpty();

        /// <summary>
        /// Returns the length of the identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (RAW?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Generate a new Electric Vehicle Supply Equipment (charging pool) identification
        /// based on the given charging pool operator and identification suffix.
        /// </summary>
        /// <param name="RAW">An raw/unparsed Electric Vehicle Supply Equipment (charging pool) identification.</param>
        /// <param name="OperatorId">The unique identification of a charging pool operator.</param>
        /// <param name="Suffix">The suffix of the charging pool identification.</param>
        private ChargingPool_Id(String       RAW,
                                CSOOperator_Id  OperatorId,
                                String       Suffix,
                                Char?        Separator   = '*')
        {

            if (Suffix.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Suffix),  "The charging pool identification suffix must not be null or empty!");

            this.RAW         = RAW;
            this.OperatorId  = OperatorId;
            this.Suffix      = Suffix;
            this.Separator   = Separator;

            unchecked
            {

                hashCode = this.OperatorId.             GetHashCode() ^
                           this.Suffix.Replace("*", "").GetHashCode() ^
                           base.                        GetHashCode();

            }

        }

        #endregion


        #region (static) NewRandom(OperatorId, Length = 12, Mapper = null)

        /// <summary>
        /// Generate a new random identification of an charging pool.
        /// </summary>
        /// <param name="OperatorId">The unique identification of a charging pool operator.</param>
        /// <param name="Length">The expected length of the charging pool identification suffix</param>
        /// <param name="Mapper">A delegate to modify the newly generated charging pool identification.</param>
        public static ChargingPool_Id NewRandom(CSOOperator_Id            OperatorId,
                                                Byte                   Length   = 12,
                                                Func<String, String>?  Mapper   = null)
        {

            var ext = Mapper is not null
                          ? Mapper(RandomExtensions.RandomString(Length))
                          :        RandomExtensions.RandomString(Length);

            return new ("",
                        OperatorId,
                        ext);

        }

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given string as an charging pool identification.
        /// </summary>
        /// <param name="Text">A text representation of an charging pool identification.</param>
        public static ChargingPool_Id Parse(String Text)
        {

            if (TryParse(Text, out var chargingPoolId))
                return chargingPoolId;

            throw new ArgumentException($"Invalid text representation of a charging pool identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse    (OperatorId, Suffix)

        /// <summary>
        /// Parse the given string as an charging pool identification.
        /// </summary>
        /// <param name="OperatorId">The unique identification of a charging pool operator.</param>
        /// <param name="Suffix">The suffix of the charging pool identification.</param>
        public static ChargingPool_Id Parse(CSOOperator_Id  OperatorId,
                                            String       Suffix)
        {

            #region Initial checks

            if (Suffix is not null)
                Suffix = Suffix.Trim();

            if (Suffix.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Suffix), "The given text representation of an charging pool identification suffix must not be null or empty!");

            #endregion

            return Parse(OperatorId +  "P" + Suffix);

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text representation of an charging pool identification.
        /// </summary>
        /// <param name="Text">A text representation of an charging pool identification.</param>
        public static ChargingPool_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingPoolId))
                return chargingPoolId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out chargingPoolId)

        /// <summary>
        /// Try to parse the given text representation of an charging pool identification.
        /// </summary>
        /// <param name="Text">A text representation of an charging pool identification.</param>
        /// <param name="chargingPoolId">The parsed charging pool identification.</param>
        public static Boolean TryParse(String Text, out ChargingPool_Id chargingPoolId)
        {

            try
            {

                var matchCollection = ChargingPoolId_RegEx.Matches(Text.Trim());

                if (matchCollection.Count == 1 &&
                    CSOOperator_Id.TryParse(matchCollection[0].Groups[1].Value, out var operatorId))
                {

                    chargingPoolId = new ChargingPool_Id(
                                         Text,
                                         operatorId,
                                         matchCollection[0].Groups[3].Value,
                                         matchCollection[0].Groups[2].Value.Length == 1
                                                     ? matchCollection[0].Groups[2].Value[0]
                                                     : null
                                     );

                    return true;

                }

            }
            catch
            { }

            chargingPoolId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charging pool identification.
        /// </summary>
        public ChargingPool_Id Clone()

            => new (
                   RAW.       CloneString(),
                   OperatorId.Clone(),
                   Suffix.    CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (chargingPoolId1, chargingPoolId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="chargingPoolId1">A charging pool identification.</param>
        /// <param name="chargingPoolId2">Another charging pool identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingPool_Id chargingPoolId1,
                                           ChargingPool_Id chargingPoolId2)

            => chargingPoolId1.Equals(chargingPoolId2);

        #endregion

        #region Operator != (chargingPoolId1, chargingPoolId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="chargingPoolId1">A charging pool identification.</param>
        /// <param name="chargingPoolId2">Another charging pool identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingPool_Id chargingPoolId1,
                                           ChargingPool_Id chargingPoolId2)

            => !chargingPoolId1.Equals(chargingPoolId2);

        #endregion

        #region Operator <  (chargingPoolId1, chargingPoolId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="chargingPoolId1">A charging pool identification.</param>
        /// <param name="chargingPoolId2">Another charging pool identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingPool_Id chargingPoolId1,
                                          ChargingPool_Id chargingPoolId2)

            => chargingPoolId1.CompareTo(chargingPoolId2) < 0;

        #endregion

        #region Operator <= (chargingPoolId1, chargingPoolId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="chargingPoolId1">A charging pool identification.</param>
        /// <param name="chargingPoolId2">Another charging pool identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingPool_Id chargingPoolId1,
                                           ChargingPool_Id chargingPoolId2)

            => chargingPoolId1.CompareTo(chargingPoolId2) <= 0;

        #endregion

        #region Operator >  (chargingPoolId1, chargingPoolId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="chargingPoolId1">A charging pool identification.</param>
        /// <param name="chargingPoolId2">Another charging pool identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingPool_Id chargingPoolId1,
                                          ChargingPool_Id chargingPoolId2)

            => chargingPoolId1.CompareTo(chargingPoolId2) > 0;

        #endregion

        #region Operator >= (chargingPoolId1, chargingPoolId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="chargingPoolId1">A charging pool identification.</param>
        /// <param name="chargingPoolId2">Another charging pool identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingPool_Id chargingPoolId1,
                                           ChargingPool_Id chargingPoolId2)

            => chargingPoolId1.CompareTo(chargingPoolId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingPoolId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingPool_Id chargingPoolId
                   ? CompareTo(chargingPoolId)
                   : throw new ArgumentException("The given object is not an charging pool identification!", nameof(Object));

        #endregion

        #region CompareTo(ChargingPoolId)

        /// <summary>
        /// Compares two charging pool identifications.
        /// </summary>
        /// <param name="ChargingPoolId">A charging pool identification to compare with.</param>
        public Int32 CompareTo(ChargingPool_Id ChargingPoolId)
        {

            var c = OperatorId.CompareTo(ChargingPoolId.OperatorId);

            if (c == 0)
                c = String.Compare(Suffix.               Replace("*", ""),
                                   ChargingPoolId.Suffix.Replace("*", ""),
                                   StringComparison.OrdinalIgnoreCase);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingPoolId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging pool identifications for equality.
        /// </summary>
        /// <param name="Object">A charging pool identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingPool_Id chargingPoolId &&
                   Equals(chargingPoolId);

        #endregion

        #region Equals(ChargingPoolId)

        /// <summary>
        /// Compares two charging pool identifications for equality.
        /// </summary>
        /// <param name="ChargingPoolId">A charging pool identification to compare with.</param>
        public Boolean Equals(ChargingPool_Id ChargingPoolId)

            => OperatorId.Equals(ChargingPoolId.OperatorId) &&

                   String.Equals(Suffix.               Replace("*", ""),
                                 ChargingPoolId.Suffix.Replace("*", ""),
                                 StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => RAW;

        #endregion

    }

}
