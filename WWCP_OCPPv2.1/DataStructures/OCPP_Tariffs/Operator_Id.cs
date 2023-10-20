/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for charging station operator identifications.
    /// </summary>
    public static class OperatorIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging station operator identification is null or empty.
        /// </summary>
        /// <param name="OperatorId">A charging station operator identification.</param>
        public static Boolean IsNullOrEmpty(this Operator_Id? OperatorId)
            => !OperatorId.HasValue || OperatorId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging station operator identification is null or empty.
        /// </summary>
        /// <param name="OperatorId">A charging station operator identification.</param>
        public static Boolean IsNotNullOrEmpty(this Operator_Id? OperatorId)
            => OperatorId.HasValue && OperatorId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a charging station operator.
    /// </summary>
    public readonly struct Operator_Id : IId<Operator_Id>
    {

        #region Data

        /// <summary>
        /// The regular expression for parsing a charging station operator identification:
        /// ^([A-Za-z]{2}\*?[A-Za-z0-9]{3})$
        /// </summary>
        public static readonly Regex OperatorId_RegEx = new (@"^([A-Za-z]{2})(\*?)([A-Za-z0-9]{3})$",
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
        public Boolean IsNullOrEmpty
            => Suffix.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging station operator identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => Suffix.IsNotNullOrEmpty();

        /// <summary>
        /// Returns the length of the charging station operator identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (CountryCode.Alpha2Code.Length + (Separator.HasValue ? 1 : 0) + (Suffix?.Length ?? 0));

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station operator identification.
        /// </summary>
        /// <param name="CountryCode">The country code.</param>
        /// <param name="Suffix">The suffix of the charging station operator identification.</param>
        /// <param name="Separator">The optional separator "*".</param>
        private Operator_Id(Country  CountryCode,
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
        public static Operator_Id Parse(String Text)
        {

            if (TryParse(Text, out var operatorId))
                return operatorId;

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
        public static Operator_Id Parse(Country  CountryCode,
                                        String   Suffix,
                                        Boolean  Separator   = true)

            => Parse(CountryCode.Alpha2Code + (Separator ? "*" : "") + Suffix);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text representation of a charging station operator identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging station operator identification.</param>
        public static Operator_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var operatorId))
                return operatorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out OperatorId)

        /// <summary>
        /// Try to parse the given text representation of a charging station operator identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging station operator identification.</param>
        /// <param name="OperatorId">The parsed charging station operator identification.</param>
        public static Boolean TryParse(String           Text,
                                       out Operator_Id  OperatorId)
        {

            try
            {

                var matchCollection = OperatorId_RegEx.Matches(Text.Trim());

                if (matchCollection.Count == 1)
                {

                    // DE...
                    if (Country.TryParseAlpha2Code(matchCollection[0].Groups[1].Value, out var country))
                    {

                        OperatorId = new Operator_Id(
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

                        OperatorId = new Operator_Id(
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

            OperatorId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(CountryCode, Suffix, out OperatorId, Separator = true)

        /// <summary>
        /// Try to parse the given text representation of a charging station operator identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="Suffix">The suffix of a charging station operator identification.</param>
        /// <param name="OperatorId">The parsed e-mobility charging station operator identification.</param>
        /// <param name="Separator">Whether to use the optional separator "*".</param>
        public static Boolean TryParse(Country          CountryCode,
                                       String           Suffix,
                                       out Operator_Id  OperatorId,
                                       Boolean          Separator   = true)
        {

            if (CountryCode is not null && Suffix.IsNeitherNullNorEmpty() &&
                TryParse(CountryCode.Alpha2Code + (Separator ? "*" : "") + Suffix, out OperatorId))
            {
                return true;
            }

            OperatorId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging station operator identification.
        /// </summary>
        public Operator_Id Clone

            => new (CountryCode,
                    new String(Suffix.ToCharArray()),
                    Separator);

        #endregion


        #region Operator overloading

        #region Operator == (OperatorId1, OperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperatorId1">An charging station operator identification.</param>
        /// <param name="OperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Operator_Id OperatorId1,
                                           Operator_Id OperatorId2)

            => OperatorId1.Equals(OperatorId2);

        #endregion

        #region Operator != (OperatorId1, OperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperatorId1">An charging station operator identification.</param>
        /// <param name="OperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Operator_Id OperatorId1,
                                           Operator_Id OperatorId2)

            => !OperatorId1.Equals(OperatorId2);

        #endregion

        #region Operator <  (OperatorId1, OperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperatorId1">An charging station operator identification.</param>
        /// <param name="OperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Operator_Id OperatorId1,
                                          Operator_Id OperatorId2)

            => OperatorId1.CompareTo(OperatorId2) < 0;

        #endregion

        #region Operator <= (OperatorId1, OperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperatorId1">An charging station operator identification.</param>
        /// <param name="OperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Operator_Id OperatorId1,
                                           Operator_Id OperatorId2)

            => OperatorId1.CompareTo(OperatorId2) <= 0;

        #endregion

        #region Operator >  (OperatorId1, OperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperatorId1">An charging station operator identification.</param>
        /// <param name="OperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Operator_Id OperatorId1,
                                          Operator_Id OperatorId2)

            => OperatorId1.CompareTo(OperatorId2) > 0;

        #endregion

        #region Operator >= (OperatorId1, OperatorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperatorId1">An charging station operator identification.</param>
        /// <param name="OperatorId2">Another charging station operator identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Operator_Id OperatorId1,
                                           Operator_Id OperatorId2)

            => OperatorId1.CompareTo(OperatorId2) >= 0;

        #endregion

        #endregion

        #region IComparable<OperatorId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="Object">A charging station operator identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Operator_Id operatorId
                   ? CompareTo(operatorId)
                   : throw new ArgumentException("The given object is not a charging station operator identification!", nameof(Object));

        #endregion

        #region CompareTo(OperatorId)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="OperatorId">A charging station operator identification to compare with.</param>
        public Int32 CompareTo(Operator_Id OperatorId)
        {

            var c = CountryCode.CompareTo(OperatorId.CountryCode);

            if (c == 0)
                c = String.Compare(Suffix,
                                   OperatorId.Suffix,
                                   StringComparison.OrdinalIgnoreCase);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<OperatorId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="Object">A charging station operator identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Operator_Id operatorId &&
                   Equals(operatorId);

        #endregion

        #region Equals(OperatorId)

        /// <summary>
        /// Compares two charging station operator identifications for equality.
        /// </summary>
        /// <param name="OperatorId">A charging station operator identification to compare with.</param>
        public Boolean Equals(Operator_Id OperatorId)

            => CountryCode.Equals(OperatorId.CountryCode) &&

                    String.Equals(Suffix,
                                  OperatorId.Suffix,
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
