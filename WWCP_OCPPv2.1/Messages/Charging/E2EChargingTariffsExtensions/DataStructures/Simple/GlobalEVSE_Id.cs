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
    /// Extension methods for EVSE identifications.
    /// </summary>
    public static class GlobalEVSEIdExtensions
    {

        /// <summary>
        /// Indicates whether this EVSE identification is null or empty.
        /// </summary>
        /// <param name="GlobalEVSEId">An EVSE identification.</param>
        public static Boolean IsNullOrEmpty(this GlobalEVSE_Id? GlobalEVSEId)
            => !GlobalEVSEId.HasValue || GlobalEVSEId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this EVSE identification is null or empty.
        /// </summary>
        /// <param name="GlobalEVSEId">An EVSE identification.</param>
        public static Boolean IsNotNullOrEmpty(this GlobalEVSE_Id? GlobalEVSEId)
            => GlobalEVSEId.HasValue && GlobalEVSEId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an electric vehicle supply equipment (EVSE).
    /// </summary>
    public readonly struct GlobalEVSE_Id : IId<GlobalEVSE_Id>
    {

        #region Data

        /// <summary>
        /// The regular expression for parsing an EVSE identification:
        /// ^([A-Za-z]{2}\*?[A-Za-z0-9]{3}(\*?)E[A-Za-z0-9\*]{1,30})$
        /// </summary>
        public static readonly Regex GlobalEVSEId_RegEx = new (@"^([A-Za-z]{2}\*?[A-Za-z0-9]{3})(\*?)E([A-Za-z0-9\*]{1,30})$",
                                                               RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Properties

        /// <summary>
        /// The raw/unparsed Electric Vehicle Supply Equipment (EVSE) identification.
        /// </summary>
        public String       RAW           { get; }

        /// <summary>
        /// The charging station operator identification.
        /// </summary>
        public CSOOperator_Id  OperatorId    { get; }

        /// <summary>
        /// Whether to use the optional separator "*".
        /// </summary>
        public Char?        Separator      { get; }

        /// <summary>
        /// The suffix of the EVSE identification.
        /// </summary>
        public String       Suffix        { get; }


        /// <summary>
        /// Indicates whether this EVSE identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => Suffix.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this EVSE identification is NOT null or empty.
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
        /// Generate a new Electric Vehicle Supply Equipment (EVSE) identification
        /// based on the given charging station operator and identification suffix.
        /// </summary>
        /// <param name="RAW">An raw/unparsed Electric Vehicle Supply Equipment (EVSE) identification.</param>
        /// <param name="OperatorId">The unique identification of a charging station operator.</param>
        /// <param name="Suffix">The suffix of the EVSE identification.</param>
        private GlobalEVSE_Id(String       RAW,
                              CSOOperator_Id  OperatorId,
                              String       Suffix,
                              Char?        Separator   = '*')
        {

            if (Suffix.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Suffix),  "The EVSE identification suffix must not be null or empty!");

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
        /// Generate a new random identification of an EVSE.
        /// </summary>
        /// <param name="OperatorId">The unique identification of a charging station operator.</param>
        /// <param name="Length">The expected length of the EVSE identification suffix</param>
        /// <param name="Mapper">A delegate to modify the newly generated EVSE identification.</param>
        public static GlobalEVSE_Id NewRandom(CSOOperator_Id            OperatorId,
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
        /// Parse the given string as an EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE identification.</param>
        public static GlobalEVSE_Id Parse(String Text)
        {

            if (TryParse(Text, out var evseId))
                return evseId;

            throw new ArgumentException($"Invalid text representation of an Electric Vehicle Supply Equipment (EVSE) identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse    (OperatorId, Suffix)

        /// <summary>
        /// Parse the given string as an EVSE identification.
        /// </summary>
        /// <param name="OperatorId">The unique identification of a charging station operator.</param>
        /// <param name="Suffix">The suffix of the EVSE identification.</param>
        public static GlobalEVSE_Id Parse(CSOOperator_Id  OperatorId,
                                          String       Suffix)
        {

            #region Initial checks

            if (Suffix is not null)
                Suffix = Suffix.Trim();

            if (Suffix.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Suffix), "The given text representation of an EVSE identification suffix must not be null or empty!");

            #endregion

            return Parse(OperatorId +  "E" + Suffix);

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text representation of an EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE identification.</param>
        public static GlobalEVSE_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var evseId))
                return evseId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out EVSEId)

        /// <summary>
        /// Try to parse the given text representation of an EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of an EVSE identification.</param>
        /// <param name="EVSEId">The parsed EVSE identification.</param>
        public static Boolean TryParse(String Text, out GlobalEVSE_Id EVSEId)
        {

            try
            {

                var matchCollection = GlobalEVSEId_RegEx.Matches(Text.Trim());

                if (matchCollection.Count == 1 &&
                    CSOOperator_Id.TryParse(matchCollection[0].Groups[1].Value, out var operatorId))
                {

                    EVSEId = new GlobalEVSE_Id(
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

            EVSEId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this EVSE identification.
        /// </summary>
        public GlobalEVSE_Id Clone

            => new (new String(RAW.   ToCharArray()),
                    OperatorId.Clone,
                    new String(Suffix.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An EVSE identification.</param>
        /// <param name="EVSEId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GlobalEVSE_Id EVSEId1,
                                           GlobalEVSE_Id EVSEId2)

            => EVSEId1.Equals(EVSEId2);

        #endregion

        #region Operator != (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An EVSE identification.</param>
        /// <param name="EVSEId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GlobalEVSE_Id EVSEId1,
                                           GlobalEVSE_Id EVSEId2)

            => !EVSEId1.Equals(EVSEId2);

        #endregion

        #region Operator <  (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An EVSE identification.</param>
        /// <param name="EVSEId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (GlobalEVSE_Id EVSEId1,
                                          GlobalEVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) < 0;

        #endregion

        #region Operator <= (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An EVSE identification.</param>
        /// <param name="EVSEId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (GlobalEVSE_Id EVSEId1,
                                           GlobalEVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) <= 0;

        #endregion

        #region Operator >  (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An EVSE identification.</param>
        /// <param name="EVSEId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (GlobalEVSE_Id EVSEId1,
                                          GlobalEVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) > 0;

        #endregion

        #region Operator >= (EVSEId1, EVSEId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEId1">An EVSE identification.</param>
        /// <param name="EVSEId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (GlobalEVSE_Id EVSEId1,
                                           GlobalEVSE_Id EVSEId2)

            => EVSEId1.CompareTo(EVSEId2) >= 0;

        #endregion

        #endregion

        #region IComparable<GlobalEVSEId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is GlobalEVSE_Id EVSEId
                   ? CompareTo(EVSEId)
                   : throw new ArgumentException("The given object is not an EVSE identification!", nameof(Object));

        #endregion

        #region CompareTo(GlobalEVSEId)

        /// <summary>
        /// Compares two EVSE identifications.
        /// </summary>
        /// <param name="GlobalEVSEId">An EVSE identification to compare with.</param>
        public Int32 CompareTo(GlobalEVSE_Id GlobalEVSEId)
        {

            var c = OperatorId.CompareTo(GlobalEVSEId.OperatorId);

            if (c == 0)
                c = String.Compare(Suffix.             Replace("*", ""),
                                   GlobalEVSEId.Suffix.Replace("*", ""),
                                   StringComparison.OrdinalIgnoreCase);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<GlobalEVSEId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EVSE identifications for equality.
        /// </summary>
        /// <param name="Object">An EVSE identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GlobalEVSE_Id EVSEId &&
                   Equals(EVSEId);

        #endregion

        #region Equals(GlobalEVSEId)

        /// <summary>
        /// Compares two EVSE identifications for equality.
        /// </summary>
        /// <param name="GlobalEVSEId">An EVSE identification to compare with.</param>
        public Boolean Equals(GlobalEVSE_Id GlobalEVSEId)

            => OperatorId.Equals(GlobalEVSEId.OperatorId) &&

                   String.Equals(Suffix.             Replace("*", ""),
                                 GlobalEVSEId.Suffix.Replace("*", ""),
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
