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

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    /// <summary>
    /// The unique identification of an OBIS identification.
    /// </summary>
    public readonly struct OBIS : IId,
                                  IEquatable <OBIS>,
                                  IComparable<OBIS>
    {

        #region Data

        /// <summary>
        /// The regular expression for parsing an OBIS identification.
        /// https://wiki.volkszaehler.org/software/obis
        /// https://github.com/volkszaehler/vzlogger/blob/master/src/Obis.cpp
        /// https://www.promotic.eu/en/pmdoc/Subsystems/Comm/PmDrivers/IEC62056_OBIS.htm
        /// https://www.bundesnetzagentur.de/DE/Service-Funktionen/Beschlusskammern/BK06/BK6_81_GPKE_GeLi/Mitteilung_nr_62/Anlagen/Codeliste_OBIS_Kennzahlen_2.2g.pdf?__blob=publicationFile&v=2
        /// http://www.nzr.de/download.php?id=612: 1.17.0 => Signierter Zählerstand (nur im EDL40-Modus)
        ///
        /// format: "A-B:C.D.E[*&]F"
        /// A, B, E, F are optional
        /// C & D are mandatory
        /// </summary>
        public static readonly Regex OBIS_RegEx = new Regex(@"^([0-9A-Fa-f]{3}-)*([0-9A-Fa-f]{3}:)*[0-9A-Fa-f]{3}\.[0-9A-Fa-f]{3}(\.[0-9A-Fa-f]{3})*(\*[0-9A-Fa-f]{3})*$",
                                                            RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => false;

        /// <summary>
        /// The length of the meter identificator.
        /// </summary>
        public readonly UInt64 Length
            => 6;

        public Byte Media       { get; } // A
        public Byte Channel     { get; } // B
        public Byte Indicator   { get; } // C =>  1: Wirkenergie Bezug P+, kWh
        public Byte Mode        { get; } // D => 17: Signierter Momentanwert (vgl. 7)
        public Byte Quantities  { get; } // E =>  0: Total
        public Byte Storage     { get; } // F

        public readonly Byte[] Bytes

            => new Byte[] {
                   Media,
                   Channel,
                   Indicator,
                   Mode,
                   Quantities,
                   Storage
               };

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OBIS identification.
        /// </summary>
        private OBIS(Byte Media,
                     Byte Channel,
                     Byte Indicator,
                     Byte Mode,
                     Byte Quantities,
                     Byte Storage)
        {

            this.Media       = Media;
            this.Channel     = Channel;
            this.Indicator   = Indicator;
            this.Mode        = Mode;
            this.Quantities  = Quantities;
            this.Storage     = Storage;

        }

        #endregion


        #region (static) ParseHex   (Text)

        /// <summary>
        /// Parse the given string as a hexadecimal OBIS identification.
        /// </summary>
        /// <param name="Text">A hexadecimal text representation of an OBIS identification.</param>
        public static OBIS ParseHex(String Text)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.Length != 12)
                throw new ArgumentNullException(nameof(Text), "The length of the given hexadecimal text representation of an OBIS identification is illegal!");

            #endregion

            if (TryParseHex(Text, out var obis))
                return obis;

            throw new ArgumentNullException(nameof(Text),
                                            "The given hexadecimal text representation of an OBIS identification is illegal!");

        }

        #endregion

        #region (static) TryParseHex(Text)

        /// <summary>
        /// Try to parse the given string as a hexadecimal OBIS identification.
        /// </summary>
        /// <param name="Text">A hexadecimal text representation of an OBIS identification.</param>
        public static OBIS? TryParseHex(String Text)
        {

            if (TryParseHex(Text, out var obis))
                return obis;

            return null;

        }

        #endregion

        #region (static) TryParseHex(Text, out OBIS)

        /// <summary>
        /// Try to parse the given string as a hexadecimal OBIS identification.
        /// </summary>
        /// <param name="Text">A hexadecimal text representation of an OBIS identification.</param>
        /// <param name="OBIS">The parsed OBIS identification.</param>
        public static Boolean TryParseHex(String Text, out OBIS OBIS)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty() || Text.Length != 12)
            {
                OBIS = default;
                return false;
            }

            #endregion

            try
            {

                // https://wiki.volkszaehler.org/software/obis
                // https://github.com/volkszaehler/vzlogger/blob/master/src/Obis.cpp
                // https://www.promotic.eu/en/pmdoc/Subsystems/Comm/PmDrivers/IEC62056_OBIS.htm
                // https://www.bundesnetzagentur.de/DE/Service-Funktionen/Beschlusskammern/BK06/BK6_81_GPKE_GeLi/Mitteilung_nr_62/Anlagen/Codeliste_OBIS_Kennzahlen_2.2g.pdf?__blob=publicationFile&v=2
                // http://www.nzr.de/download.php?id=612: 1.17.0 => Signierter Zählerstand (nur im EDL40-Modus)

                // format: "A-B:C.D.E[*&]F"
                // A, B, E, F are optional
                // C & D are mandatory
                OBIS = new OBIS(
                    Media:      Convert.ToByte(Text.Substring( 0,  2), 16), // A
                    Channel:    Convert.ToByte(Text.Substring( 2,  4), 16), // B
                    Indicator:  Convert.ToByte(Text.Substring( 4,  6), 16), // C =>  1: Wirkenergie Bezug P+, kWh
                    Mode:       Convert.ToByte(Text.Substring( 6,  8), 16), // D => 17: Signierter Momentanwert (vgl. 7)
                    Quantities: Convert.ToByte(Text.Substring( 8, 10), 16), // E =>  0: Total
                    Storage:    Convert.ToByte(Text.Substring(10, 12), 16)  // F
                );

                return true;

            }
            catch (Exception)
            { }

            OBIS = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this OBIS identification.
        /// </summary>
        public readonly OBIS Clone

            => new (Media,
                    Channel,
                    Indicator,
                    Mode,
                    Quantities,
                    Storage);

        #endregion


        #region Operator overloading

        #region Operator == (OBIS1, OBIS2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OBIS1">A OBIS identification.</param>
        /// <param name="OBIS2">Another OBIS identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OBIS OBIS1, OBIS OBIS2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(OBIS1, OBIS2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) OBIS1 == null) || ((Object) OBIS2 == null))
                return false;

            return OBIS1.Equals(OBIS2);

        }

        #endregion

        #region Operator != (OBIS1, OBIS2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OBIS1">A OBIS identification.</param>
        /// <param name="OBIS2">Another OBIS identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OBIS OBIS1, OBIS OBIS2)
            => !(OBIS1 == OBIS2);

        #endregion

        #region Operator <  (OBIS1, OBIS2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OBIS1">A OBIS identification.</param>
        /// <param name="OBIS2">Another OBIS identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OBIS OBIS1, OBIS OBIS2)
        {

            if ((Object) OBIS1 == null)
                throw new ArgumentNullException(nameof(OBIS1), "The given OBIS1 must not be null!");

            return OBIS1.CompareTo(OBIS2) < 0;

        }

        #endregion

        #region Operator <= (OBIS1, OBIS2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OBIS1">A OBIS identification.</param>
        /// <param name="OBIS2">Another OBIS identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OBIS OBIS1, OBIS OBIS2)
            => !(OBIS1 > OBIS2);

        #endregion

        #region Operator >  (OBIS1, OBIS2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OBIS1">A OBIS identification.</param>
        /// <param name="OBIS2">Another OBIS identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OBIS OBIS1, OBIS OBIS2)
        {

            if ((Object) OBIS1 == null)
                throw new ArgumentNullException(nameof(OBIS1), "The given OBIS1 must not be null!");

            return OBIS1.CompareTo(OBIS2) > 0;

        }

        #endregion

        #region Operator >= (OBIS1, OBIS2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OBIS1">A OBIS identification.</param>
        /// <param name="OBIS2">Another OBIS identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OBIS OBIS1, OBIS OBIS2)
            => !(OBIS1 < OBIS2);

        #endregion

        #endregion

        #region IComparable<OBIS> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is OBIS obis))
                throw new ArgumentException("The given object is not an OBIS identification!",
                                            nameof(Object));

            return CompareTo(obis);

        }

        #endregion

        #region CompareTo(OBIS)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OBIS">An object to compare with.</param>
        public Int32 CompareTo(OBIS OBIS)
        {

            if ((Object) OBIS == null)
                throw new ArgumentNullException(nameof(OBIS),  "The given OBIS identification must not be null!");

            var c = Media.CompareTo(OBIS.Media);
            if (c != 0)
                return c;

            c = Channel.CompareTo(OBIS.Channel);
            if (c != 0)
                return c;

            c = Indicator.CompareTo(OBIS.Indicator);
            if (c != 0)
                return c;

            c = Mode.CompareTo(OBIS.Mode);
            if (c != 0)
                return c;

            c = Quantities.CompareTo(OBIS.Quantities);
            if (c != 0)
                return c;

            return Storage.CompareTo(OBIS.Storage);

        }

        #endregion

        #endregion

        #region IEquatable<OBIS> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is OBIS obis))
                return false;

            return Equals(obis);

        }

        #endregion

        #region Equals(OBIS)

        /// <summary>
        /// Compares two OBISs for equality.
        /// </summary>
        /// <param name="OBIS">A OBIS identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(OBIS OBIS)
        {

            if ((Object) OBIS == null)
                return false;

            return Media.     Equals(OBIS.Media)      &&
                   Channel.   Equals(OBIS.Channel)    &&
                   Indicator. Equals(OBIS.Indicator)  &&
                   Mode.      Equals(OBIS.Mode)       &&
                   Quantities.Equals(OBIS.Quantities) &&
                   Storage.   Equals(OBIS.Storage);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Media.     GetHashCode() * 13 ^
               Channel.   GetHashCode() * 11 ^
               Indicator. GetHashCode() *  7 ^
               Mode.      GetHashCode() *  5 ^
               Quantities.GetHashCode() *  3 ^
               Storage.   GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Media,      "-",
                             Channel,    ":",
                             Indicator,  ".",
                             Mode,       ".",
                             Quantities, "*",
                             Storage);

        #endregion

    }

}
