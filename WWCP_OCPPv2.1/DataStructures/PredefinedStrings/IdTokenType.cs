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
    /// Extension methods for identification token types.
    /// </summary>
    public static class IdTokenTypeExtensions
    {

        /// <summary>
        /// Indicates whether this identification token type is null or empty.
        /// </summary>
        /// <param name="IdTokenType">An identification token type.</param>
        public static Boolean IsNullOrEmpty(this IdTokenType? IdTokenType)
            => !IdTokenType.HasValue || IdTokenType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this identification token type is null or empty.
        /// </summary>
        /// <param name="IdTokenType">An identification token type.</param>
        public static Boolean IsNotNullOrEmpty(this IdTokenType? IdTokenType)
            => IdTokenType.HasValue && IdTokenType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An identification token type.
    /// </summary>
    public readonly struct IdTokenType : IId,
                                         IEquatable<IdTokenType>,
                                         IComparable<IdTokenType>
    {

        #region Data

        private readonly static Dictionary<String, IdTokenType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                           InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification token type is null or empty.
        /// </summary>
        public readonly  Boolean                   IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification token type is NOT null or empty.
        /// </summary>
        public readonly  Boolean                   IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the identification token type.
        /// </summary>
        public readonly  UInt64                    Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered identification token types.
        /// </summary>
        public static    IEnumerable<IdTokenType>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new identification token type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an identification token type.</param>
        private IdTokenType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static IdTokenType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new IdTokenType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an identification token type.
        /// </summary>
        /// <param name="Text">A text representation of an identification token type.</param>
        public static IdTokenType Parse(String Text)
        {

            if (TryParse(Text, out var idTokenType))
                return idTokenType;

            throw new ArgumentException($"Invalid text representation of an identification token type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as identification token type.
        /// </summary>
        /// <param name="Text">A text representation of an identification token type.</param>
        public static IdTokenType? TryParse(String Text)
        {

            if (TryParse(Text, out var idTokenType))
                return idTokenType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out IdTokenType)

        /// <summary>
        /// Try to parse the given text as identification token type.
        /// </summary>
        /// <param name="Text">A text representation of an identification token type.</param>
        /// <param name="IdTokenType">The parsed identification token type.</param>
        public static Boolean TryParse(String Text, out IdTokenType IdTokenType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out IdTokenType))
                    IdTokenType = Register(Text);

                return true;

            }

            IdTokenType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this identification token type.
        /// </summary>
        public IdTokenType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// A centrally, in the CSMS (or other server) generated id (for example used for a remotely started
        /// transaction that is activated by SMS). No format defined, might be an UUID.
        /// </summary>
        public static IdTokenType  Central            { get; }
            = Register("Central");

        /// <summary>
        /// An electro-mobility account identification, as defined in ISO 15118.
        /// </summary>
        public static IdTokenType  eMAID              { get; }
            = Register("eMAID");

        /// <summary>
        /// EVCCID of EV.
        /// For ISO 15118-2  this is the MAC address of the communication controller.
        /// For ISO 15118-20 this is an identifier up to 255 characters.
        /// </summary>
        public static IdTokenType  EVCCID             { get; }
            = Register("EVCCID");

        /// <summary>
        /// ISO14443 UID of an RFID card.
        /// It is represented as an array of 4 or 7 bytes in hexadecimal representation.
        /// </summary>
        public static IdTokenType  ISO14443           { get; }
            = Register("ISO14443");

        /// <summary>
        /// ISO15693 UID of RFID card.
        /// It is represented as an array of 8 bytes in hexadecimal representation.
        /// </summary>
        public static IdTokenType  ISO15693           { get; }
            = Register("ISO15693");

        /// <summary>
        /// A private key-code to authorize a charging transaction.
        /// For example: PIN-code.
        /// </summary>
        public static IdTokenType  KeyCode            { get; }
            = Register("KeyCode");

        /// <summary>
        /// A locally generated id (e.g. internal id created by the charging station).
        /// Needs no checking by CSMS. No format defined, might e.g. be an UUID.
        /// </summary>
        public static IdTokenType  Local              { get; }
            = Register("Local");

        /// <summary>
        /// The MAC Address of the EVCC (electric vehicle communication controller) that is connected to the EVSE.
        /// Used when MAC address is used for authorization (AutoCharge).
        /// </summary>
        public static IdTokenType  MACAddress         { get; }
            = Register("MacAddress");

        /// <summary>
        /// NEMA EVSE1 2018 token.
        /// </summary>
        public static IdTokenType  NEMA               { get; }
            = Register("NEMA");

        /// <summary>
        /// Transaction is started and no authorization possible.
        /// The charging station only has a start button or mechanical key etc.
        /// IdToken field SHALL be left empty.
        /// </summary>
        public static IdTokenType NoAuthorization    { get; }
            = Register("NoAuthorization");

        /// <summary>
        /// Vehicle Identification Number of EV.
        /// </summary>
        public static IdTokenType VIN                { get; }
            = Register("VIN");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (IdTokenType1, IdTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenType1">An identification token type.</param>
        /// <param name="IdTokenType2">Another identification token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTokenType IdTokenType1,
                                           IdTokenType IdTokenType2)

            => IdTokenType1.Equals(IdTokenType2);

        #endregion

        #region Operator != (IdTokenType1, IdTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenType1">An identification token type.</param>
        /// <param name="IdTokenType2">Another identification token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTokenType IdTokenType1,
                                           IdTokenType IdTokenType2)

            => !IdTokenType1.Equals(IdTokenType2);

        #endregion

        #region Operator <  (IdTokenType1, IdTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenType1">An identification token type.</param>
        /// <param name="IdTokenType2">Another identification token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (IdTokenType IdTokenType1,
                                          IdTokenType IdTokenType2)

            => IdTokenType1.CompareTo(IdTokenType2) < 0;

        #endregion

        #region Operator <= (IdTokenType1, IdTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenType1">An identification token type.</param>
        /// <param name="IdTokenType2">Another identification token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (IdTokenType IdTokenType1,
                                           IdTokenType IdTokenType2)

            => IdTokenType1.CompareTo(IdTokenType2) <= 0;

        #endregion

        #region Operator >  (IdTokenType1, IdTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenType1">An identification token type.</param>
        /// <param name="IdTokenType2">Another identification token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (IdTokenType IdTokenType1,
                                          IdTokenType IdTokenType2)

            => IdTokenType1.CompareTo(IdTokenType2) > 0;

        #endregion

        #region Operator >= (IdTokenType1, IdTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenType1">An identification token type.</param>
        /// <param name="IdTokenType2">Another identification token type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (IdTokenType IdTokenType1,
                                           IdTokenType IdTokenType2)

            => IdTokenType1.CompareTo(IdTokenType2) >= 0;

        #endregion

        #endregion

        #region IComparable<IdTokenType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two identification token types.
        /// </summary>
        /// <param name="Object">identification token type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is IdTokenType idTokenType
                   ? CompareTo(idTokenType)
                   : throw new ArgumentException("The given object is not identification token type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(IdTokenType)

        /// <summary>
        /// Compares two identification token types.
        /// </summary>
        /// <param name="IdTokenType">identification token type to compare with.</param>
        public Int32 CompareTo(IdTokenType IdTokenType)

            => String.Compare(InternalId,
                              IdTokenType.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<IdTokenType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two identification token types for equality.
        /// </summary>
        /// <param name="Object">identification token type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdTokenType idTokenType &&
                   Equals(idTokenType);

        #endregion

        #region Equals(IdTokenType)

        /// <summary>
        /// Compares two identification token types for equality.
        /// </summary>
        /// <param name="IdTokenType">identification token type to compare with.</param>
        public Boolean Equals(IdTokenType IdTokenType)

            => String.Equals(InternalId,
                             IdTokenType.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
