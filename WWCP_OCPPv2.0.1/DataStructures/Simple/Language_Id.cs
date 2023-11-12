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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extension methods for a human language identifier.
    /// </summary>
    public static class LanguageIdExtensions
    {

        /// <summary>
        /// Indicates whether this human language identifier is null or empty.
        /// </summary>
        /// <param name="LanguageId">A human language identifier.</param>
        public static Boolean IsNullOrEmpty(this Language_Id? LanguageId)
            => !LanguageId.HasValue || LanguageId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this human language identifier is null or empty.
        /// </summary>
        /// <param name="LanguageId">A human language identifier.</param>
        public static Boolean IsNotNullOrEmpty(this Language_Id? LanguageId)
            => LanguageId.HasValue && LanguageId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A human language identifier, as defined in rfc5646.
    /// </summary>
    public readonly struct Language_Id : IId,
                                         IEquatable<Language_Id>,
                                         IComparable<Language_Id>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the language identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new human language identifier based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a human language identifier.</param>
        private Language_Id(String  Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a human language.
        /// </summary>
        /// <param name="Text">A text representation of a human language.</param>
        public static Language_Id Parse(String Text)
        {

            if (TryParse(Text, out var languageId))
                return languageId;

            throw new ArgumentException("The given text representation of a human language is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a human language.
        /// </summary>
        /// <param name="Text">A text representation of a human language.</param>
        public static Language_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var languageId))
                return languageId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LanguageId)

        /// <summary>
        /// Try to parse the given text as a human language.
        /// </summary>
        /// <param name="Text">A text representation of a human language.</param>
        /// <param name="LanguageId">The parsed human language.</param>
        public static Boolean TryParse(String Text, out Language_Id LanguageId)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                LanguageId = default;
                return false;
            }

            #endregion

            try
            {
                LanguageId = new Language_Id(Text);
                return true;
            }
            catch (Exception)
            { }

            LanguageId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this human language identifier
        /// </summary>
        public Language_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => LanguageId1.Equals(LanguageId2);

        #endregion

        #region Operator != (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => !LanguageId1.Equals(LanguageId2);

        #endregion

        #region Operator <  (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Language_Id LanguageId1,
                                          Language_Id LanguageId2)

            => LanguageId1.CompareTo(LanguageId2) < 0;

        #endregion

        #region Operator <= (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => LanguageId1.CompareTo(LanguageId2) <= 0;

        #endregion

        #region Operator >  (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Language_Id LanguageId1,
                                          Language_Id LanguageId2)

            => LanguageId1.CompareTo(LanguageId2) > 0;

        #endregion

        #region Operator >= (LanguageId1, LanguageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LanguageId1">A language identification.</param>
        /// <param name="LanguageId2">Another language identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Language_Id LanguageId1,
                                           Language_Id LanguageId2)

            => LanguageId1.CompareTo(LanguageId2) >= 0;

        #endregion

        #endregion

        #region IComparable<LanguageId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two language identifications.
        /// </summary>
        /// <param name="Object">A language identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Language_Id languageId
                   ? CompareTo(languageId)
                   : throw new ArgumentException("The given object is not a language identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LanguageId)

        /// <summary>
        /// Compares two language identifications.
        /// </summary>
        /// <param name="LanguageId">A language identification to compare with.</param>
        public Int32 CompareTo(Language_Id LanguageId)

            => String.Compare(InternalId,
                              LanguageId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LanguageId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two language identifications for equality.
        /// </summary>
        /// <param name="Object">A language identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Language_Id languageId &&
                   Equals(languageId);

        #endregion

        #region Equals(LanguageId)

        /// <summary>
        /// Compares two language identifications for equality.
        /// </summary>
        /// <param name="LanguageId">A language identification to compare with.</param>
        public Boolean Equals(Language_Id LanguageId)

            => String.Equals(InternalId,
                             LanguageId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
