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
    /// Extension methods for e-mobility provider identifications.
    /// </summary>
    public static class ProviderIdExtensions
    {

        /// <summary>
        /// Indicates whether this e-mobility provider identification is null or empty.
        /// </summary>
        /// <param name="ProviderId">A e-mobility provider identification.</param>
        public static Boolean IsNullOrEmpty(this Provider_Id? ProviderId)
            => !ProviderId.HasValue || ProviderId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this e-mobility provider identification is null or empty.
        /// </summary>
        /// <param name="ProviderId">A e-mobility provider identification.</param>
        public static Boolean IsNotNullOrEmpty(this Provider_Id? ProviderId)
            => ProviderId.HasValue && ProviderId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a e-mobility provider.
    /// </summary>
    public readonly struct Provider_Id : IId<Provider_Id>
    {

        #region Data

        /// <summary>
        /// The regular expression for parsing a e-mobility provider identification:
        /// ^([A-Za-z]{2}([\*|\-]?)[A-Za-z0-9]{3})$
        /// </summary>
        public static readonly Regex ProviderId_RegEx = new (@"^([A-Za-z]{2})([\*|\-]?)([A-Za-z0-9]{3})$",
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
        /// The optional separator "-" (or "*").
        /// </summary>
        public Char?    Separator      { get; }


        /// <summary>
        /// Indicates whether this e-mobility provider identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => Suffix.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this e-mobility provider identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => Suffix.IsNotNullOrEmpty();

        /// <summary>
        /// Returns the length of the e-mobility provider identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (CountryCode.Alpha2Code.Length + (Separator.HasValue ? 1 : 0)  + (Suffix?.Length ?? 0));

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new e-mobility provider identification.
        /// </summary>
        /// <param name="CountryCode">The country code.</param>
        /// <param name="Suffix">The suffix of the e-mobility provider identification.</param>
        /// <param name="Separator">An optional separator "-" (or "*").</param>
        private Provider_Id(Country  CountryCode,
                            String   Suffix,
                            Char?    Separator   = '-')
        {

            if (Suffix.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Suffix), "The e-mobility provider identification suffix must not be null or empty!");

            this.CountryCode  = CountryCode;
            this.Suffix       = Suffix;
            this.Separator    = Separator;

        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text representation of a e-mobility provider identification.
        /// </summary>
        /// <param name="Text">A text representation of a e-mobility provider identification.</param>
        public static Provider_Id Parse(String Text)
        {

            if (TryParse(Text, out var providerId))
                return providerId;

            throw new ArgumentException($"Invalid text representation of an e-mobility provider identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (CountryCode, Suffix, Separator = true)

        /// <summary>
        /// Parse the given string as an e-mobility provider identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="Suffix">The suffix of an e-mobility provider identification.</param>
        /// <param name="Separator">Whether to use the optional separator "-".</param>
        public static Provider_Id Parse(Country  CountryCode,
                                        String   Suffix,
                                        Boolean  Separator   = true)

            => Parse(CountryCode.Alpha2Code + (Separator ? "-" : "") + Suffix);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text representation of a e-mobility provider identification.
        /// </summary>
        /// <param name="Text">A text representation of a e-mobility provider identification.</param>
        public static Provider_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var providerId))
                return providerId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ProviderId)

        /// <summary>
        /// Try to parse the given text representation of a e-mobility provider identification.
        /// </summary>
        /// <param name="Text">A text representation of a e-mobility provider identification.</param>
        /// <param name="ProviderId">The parsed e-mobility provider identification.</param>
        public static Boolean TryParse(String           Text,
                                       out Provider_Id  ProviderId)
        {

            try
            {

                var matchCollection = ProviderId_RegEx.Matches(Text.Trim());

                if (matchCollection.Count == 1)
                {

                    // DE...
                    if (Country.TryParseAlpha2Code(matchCollection[0].Groups[1].Value, out var country))
                    {

                        ProviderId = new Provider_Id(
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

                        ProviderId = new Provider_Id(
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

            ProviderId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(CountryCode, Suffix, out ProviderId, Separator = '-')

        /// <summary>
        /// Try to parse the given text representation of a e-mobility provider identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="Suffix">The suffix of a e-mobility provider identification.</param>
        /// <param name="ProviderId">The parsed e-mobility e-mobility provider identification.</param>
        /// <param name="Separator">An optional separator '-' or '*'.</param>
        public static Boolean TryParse(Country          CountryCode,
                                       String           Suffix,
                                       out Provider_Id  ProviderId,
                                       Char?            Separator   = '-')
        {

            if (Separator.HasValue && (Separator.Value == '-' || Separator.Value == '-') &&
                CountryCode is not null &&
                Suffix.IsNeitherNullNorEmpty() &&
                TryParse(CountryCode.Alpha2Code + Separator + Suffix, out ProviderId))
            {
                return true;
            }

            ProviderId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this e-mobility provider identification.
        /// </summary>
        public Provider_Id Clone()

            => new (
                   CountryCode,
                   Suffix.CloneString(),
                   Separator
               );

        #endregion


        #region Provider overloading

        #region Provider == (ProviderId1, ProviderId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProviderId1">An e-mobility provider identification.</param>
        /// <param name="ProviderId2">Another e-mobility provider identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Provider_Id ProviderId1,
                                           Provider_Id ProviderId2)

            => ProviderId1.Equals(ProviderId2);

        #endregion

        #region Provider != (ProviderId1, ProviderId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProviderId1">An e-mobility provider identification.</param>
        /// <param name="ProviderId2">Another e-mobility provider identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Provider_Id ProviderId1,
                                           Provider_Id ProviderId2)

            => !ProviderId1.Equals(ProviderId2);

        #endregion

        #region Provider <  (ProviderId1, ProviderId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProviderId1">An e-mobility provider identification.</param>
        /// <param name="ProviderId2">Another e-mobility provider identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Provider_Id ProviderId1,
                                          Provider_Id ProviderId2)

            => ProviderId1.CompareTo(ProviderId2) < 0;

        #endregion

        #region Provider <= (ProviderId1, ProviderId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProviderId1">An e-mobility provider identification.</param>
        /// <param name="ProviderId2">Another e-mobility provider identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Provider_Id ProviderId1,
                                           Provider_Id ProviderId2)

            => ProviderId1.CompareTo(ProviderId2) <= 0;

        #endregion

        #region Provider >  (ProviderId1, ProviderId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProviderId1">An e-mobility provider identification.</param>
        /// <param name="ProviderId2">Another e-mobility provider identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Provider_Id ProviderId1,
                                          Provider_Id ProviderId2)

            => ProviderId1.CompareTo(ProviderId2) > 0;

        #endregion

        #region Provider >= (ProviderId1, ProviderId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ProviderId1">An e-mobility provider identification.</param>
        /// <param name="ProviderId2">Another e-mobility provider identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Provider_Id ProviderId1,
                                           Provider_Id ProviderId2)

            => ProviderId1.CompareTo(ProviderId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ProviderId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two e-mobility provider identifications for equality.
        /// </summary>
        /// <param name="Object">A e-mobility provider identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Provider_Id providerId
                   ? CompareTo(providerId)
                   : throw new ArgumentException("The given object is not a e-mobility provider identification!", nameof(Object));

        #endregion

        #region CompareTo(ProviderId)

        /// <summary>
        /// Compares two e-mobility provider identifications for equality.
        /// </summary>
        /// <param name="ProviderId">A e-mobility provider identification to compare with.</param>
        public Int32 CompareTo(Provider_Id ProviderId)
        {

            var c = CountryCode.CompareTo(ProviderId.CountryCode);

            if (c == 0)
                c = String.Compare(Suffix,
                                   ProviderId.Suffix,
                                   StringComparison.OrdinalIgnoreCase);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ProviderId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two e-mobility provider identifications for equality.
        /// </summary>
        /// <param name="Object">A e-mobility provider identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Provider_Id providerId &&
                   Equals(providerId);

        #endregion

        #region Equals(ProviderId)

        /// <summary>
        /// Compares two e-mobility provider identifications for equality.
        /// </summary>
        /// <param name="ProviderId">A e-mobility provider identification to compare with.</param>
        public Boolean Equals(Provider_Id ProviderId)

            => CountryCode.Equals(ProviderId.CountryCode) &&

                    String.Equals(Suffix,
                                  ProviderId.Suffix,
                                  StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
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
