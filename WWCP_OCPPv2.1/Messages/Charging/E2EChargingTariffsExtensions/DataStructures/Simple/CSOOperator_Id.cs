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
    /// Extension methods for charging station operator identifications.
    /// </summary>
    public static class CSOCSOOperatorIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging station operator identification is null or empty.
        /// </summary>
        /// <param name="CSOOperatorId">A charging station operator identification.</param>
        public static Boolean IsNullOrEmpty(this CSOOperator_Id? CSOOperatorId)
            => !CSOOperatorId.HasValue || CSOOperatorId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging station operator identification is null or empty.
        /// </summary>
        /// <param name="CSOOperatorId">A charging station operator identification.</param>
        public static Boolean IsNotNullOrEmpty(this CSOOperator_Id? CSOOperatorId)
            => CSOOperatorId.HasValue && CSOOperatorId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a charging station operator.
    /// </summary>
    public readonly struct CSOOperator_Id : IId<CSOOperator_Id>
    {

        #region Data

        /// <summary>
        /// The regular expression for parsing a charging station operator identification:
        /// ^([A-Za-z]{2}\*?[A-Za-z0-9]{3})$
        /// </summary>
        public static readonly Regex CSOOperatorId_RegEx = new (@"^([A-Za-z]{2})(\*?)([A-Za-z0-9]{3})$",
                                                             RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Properties

        /// <summary>
        /// The country code.
        /// </summary>
        public Country  CountryCode    { get; }

        /// <summary>
        /// The identificator suffix.
        /// </summary>
        public String   Suffix         { get; }

        /// <summary>
        /// Whether to use the optional separator "*".
        /// </summary>
        public Char?    Separator      { get; }


        /// <summary>
        /// Indicates whether this charging station operator identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => Suffix.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging station operator identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => Suffix.IsNotNullOrEmpty();

        /// <summary>
        /// Returns the length of the charging station operator identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (CountryCode.Alpha2Code.Length + (Separator.HasValue ? 1 : 0) + (Suffix?.Length ?? 0));

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station operator identification.
        /// </summary>
        /// <param name="CountryCode">The country code.</param>
        /// <param name="Suffix">The suffix of the charging station operator identification.</param>
        /// <param name="Separator">The optional separator "*".</param>
        private CSOOperator_Id(Country  CountryCode,
                            String   Suffix,
                            Char?    Separator   = '*')
        {

            if (Suffix.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Suffix), "The charging station operator identification suffix must not be null or empty!");

            this.CountryCode  = CountryCode;
            this.Suffix       = Suffix;
            this.Separator    = Separator;

        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text representation of a charging station operator identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging station operator identification.</param>
        public static CSOOperator_Id Parse(String Text)
        {

            if (TryParse(Text, out var csoOperatorId))
                return csoOperatorId;

            throw new ArgumentException($"Invalid text representation of a charging station operator identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (CountryCode, Suffix, Separator = true)

        /// <summary>
        /// Parse the given string as an charging station operator identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="Suffix">The suffix of an charging station operator identification.</param>
        /// <param name="Separator">Whether to use the optional separator "*".</param>
        public static CSOOperator_Id Parse(Country  CountryCode,
                                        String   Suffix,
                                        Boolean  Separator   = true)

            => Parse(CountryCode.Alpha2Code + (Separator ? "*" : "") + Suffix);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text representation of a charging station operator identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging station operator identification.</param>
        public static CSOOperator_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var csoOperatorId))
                return csoOperatorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CSOOperatorId)

        /// <summary>
        /// Try to parse the given text representation of a charging station operator identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging station operator identification.</param>
        /// <param name="CSOOperatorId">The parsed charging station operator identification.</param>
        public static Boolean TryParse(String           Text,
                                       out CSOOperator_Id  CSOOperatorId)
        {

            try
            {

                var matchCollection = CSOOperatorId_RegEx.Matches(Text.Trim());

                if (matchCollection.Count == 1)
                {

                    // DE...
                    if (Country.TryParseAlpha2Code(matchCollection[0].Groups[1].Value, out var country))
                    {

                        CSOOperatorId = new CSOOperator_Id(
                                         country,
                                         matchCollection[0].Groups[3].Value,
                                         matchCollection[0].Groups[2].Value.Length == 1
                                             ? matchCollection[0].Groups[2].Value[0]
                                             : null
                                     );

                        return true;

                    }

                    // An unknown/unassigned alpha-2 country code, like e.g. "DT"...
                    if (Country.Alpha2Codes_RegEx.IsMatch(matchCollection[0].Groups[1].Value))
                    {

                        CSOOperatorId = new CSOOperator_Id(
                                         new Country(
                                             I18NString.Create(Languages.en, matchCollection[0].Groups[1].Value),
                                             matchCollection[0].Groups[1].Value,
                                             matchCollection[0].Groups[1].Value + "X",
                                             0,
                                             0
                                         ),
                                         matchCollection[0].Groups[3].Value,
                                         matchCollection[0].Groups[2].Value.Length == 1
                                             ? matchCollection[0].Groups[2].Value[0]
                                             : null
                                     );

                        return true;

                    }

                }

            }

            catch
            { }

            CSOOperatorId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(CountryCode, Suffix, out CSOOperatorId, Separator = true)

        /// <summary>
        /// Try to parse the given text representation of a charging station operator identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="Suffix">The suffix of a charging station operator identification.</param>
        /// <param name="CSOOperatorId">The parsed e-mobility charging station operator identification.</param>
        /// <param name="Separator">Whether to use the optional separator "*".</param>
        public static Boolean TryParse(Country          CountryCode,
                                       String           Suffix,
                                       out CSOOperator_Id  CSOOperatorId,
                                       Boolean          Separator   = true)
        {

            if (CountryCode is not null && Suffix.IsNeitherNullNorEmpty() &&
                TryParse(CountryCode.Alpha2Code + (Separator ? "*" : "") + Suffix, out CSOOperatorId))
            {
                return true;
            }

            CSOOperatorId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charging station operator identification.
        /// </summary>
        public CSOOperator_Id Clone()

            => new (
                   CountryCode,
                   Suffix.CloneString(),
                   Separator
               );

        #endregion


        #region Operator overloading

        #region Operator == (CSOOperatorId1, CSOOperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSOOperatorId1">An charging station operator identification.</param>
        /// <param name="CSOOperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CSOOperator_Id CSOOperatorId1,
                                           CSOOperator_Id CSOOperatorId2)

            => CSOOperatorId1.Equals(CSOOperatorId2);

        #endregion

        #region Operator != (CSOOperatorId1, CSOOperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSOOperatorId1">An charging station operator identification.</param>
        /// <param name="CSOOperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CSOOperator_Id CSOOperatorId1,
                                           CSOOperator_Id CSOOperatorId2)

            => !CSOOperatorId1.Equals(CSOOperatorId2);

        #endregion

        #region Operator <  (CSOOperatorId1, CSOOperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSOOperatorId1">An charging station operator identification.</param>
        /// <param name="CSOOperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CSOOperator_Id CSOOperatorId1,
                                          CSOOperator_Id CSOOperatorId2)

            => CSOOperatorId1.CompareTo(CSOOperatorId2) < 0;

        #endregion

        #region Operator <= (CSOOperatorId1, CSOOperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSOOperatorId1">An charging station operator identification.</param>
        /// <param name="CSOOperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CSOOperator_Id CSOOperatorId1,
                                           CSOOperator_Id CSOOperatorId2)

            => CSOOperatorId1.CompareTo(CSOOperatorId2) <= 0;

        #endregion

        #region Operator >  (CSOOperatorId1, CSOOperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSOOperatorId1">An charging station operator identification.</param>
        /// <param name="CSOOperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CSOOperator_Id CSOOperatorId1,
                                          CSOOperator_Id CSOOperatorId2)

            => CSOOperatorId1.CompareTo(CSOOperatorId2) > 0;

        #endregion

        #region Operator >= (CSOOperatorId1, CSOOperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSOOperatorId1">An charging station operator identification.</param>
        /// <param name="CSOOperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CSOOperator_Id CSOOperatorId1,
                                           CSOOperator_Id CSOOperatorId2)

            => CSOOperatorId1.CompareTo(CSOOperatorId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CSOOperatorId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="Object">A charging station operator identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CSOOperator_Id csoOperatorId
                   ? CompareTo(csoOperatorId)
                   : throw new ArgumentException("The given object is not a charging station operator identification!", nameof(Object));

        #endregion

        #region CompareTo(CSOOperatorId)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="CSOOperatorId">A charging station operator identification to compare with.</param>
        public Int32 CompareTo(CSOOperator_Id CSOOperatorId)
        {

            var c = CountryCode.CompareTo(CSOOperatorId.CountryCode);

            if (c == 0)
                c = String.Compare(Suffix,
                                   CSOOperatorId.Suffix,
                                   StringComparison.OrdinalIgnoreCase);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CSOOperatorId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="Object">A charging station operator identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CSOOperator_Id csoOperatorId &&
                   Equals(csoOperatorId);

        #endregion

        #region Equals(CSOOperatorId)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="CSOOperatorId">A charging station operator identification to compare with.</param>
        public Boolean Equals(CSOOperator_Id CSOOperatorId)

            => CountryCode.Equals(CSOOperatorId.CountryCode) &&

                    String.Equals(Suffix,
                                  CSOOperatorId.Suffix,
                                  StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => CountryCode.GetHashCode() ^
               Suffix.     GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CountryCode.Alpha2Code + Separator + Suffix;

        #endregion


    }

}
