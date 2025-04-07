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
    /// Extension methods for OCPP versions.
    /// </summary>
    public static class OCPPVersionExtensions
    {

        /// <summary>
        /// Indicates whether this OCPP version is null or empty.
        /// </summary>
        /// <param name="OCPPVersion">An OCPP version.</param>
        public static Boolean IsNullOrEmpty(this OCPPVersion? OCPPVersion)
            => !OCPPVersion.HasValue || OCPPVersion.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this OCPP version is null or empty.
        /// </summary>
        /// <param name="OCPPVersion">An OCPP version.</param>
        public static Boolean IsNotNullOrEmpty(this OCPPVersion? OCPPVersion)
            => OCPPVersion.HasValue && OCPPVersion.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An OCPP version.
    /// </summary>
    public readonly struct OCPPVersion : IId,
                                         IEquatable<OCPPVersion>,
                                         IComparable<OCPPVersion>
    {

        #region Data

        private readonly static Dictionary<String, OCPPVersion>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                           InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this OCPP version is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this OCPP version is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the OCPP version.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP version based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an OCPP version.</param>
        private OCPPVersion(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static OCPPVersion Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new OCPPVersion(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an OCPP version.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP version.</param>
        public static OCPPVersion Parse(String Text)
        {

            if (TryParse(Text, out var ocppVersion))
                return ocppVersion;

            throw new ArgumentException($"Invalid text representation of an OCPP version: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as OCPP version.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP version.</param>
        public static OCPPVersion? TryParse(String Text)
        {

            if (TryParse(Text, out var ocppVersion))
                return ocppVersion;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out OCPPVersion)

        /// <summary>
        /// Try to parse the given text as OCPP version.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP version.</param>
        /// <param name="OCPPVersion">The parsed OCPP version.</param>
        public static Boolean TryParse(String Text, out OCPPVersion OCPPVersion)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out OCPPVersion))
                    OCPPVersion = Register(Text);

                return true;

            }

            OCPPVersion = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this OCPP version.
        /// </summary>
        public OCPPVersion Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// OCPP Version v1.2
        /// </summary>
        public static OCPPVersion OCPP12     { get; }
            = Register("OCPP12");

        /// <summary>
        /// OCPP Version v1.5
        /// </summary>
        public static OCPPVersion OCPP15     { get; }
            = Register("OCPP15");

        /// <summary>
        /// OCPP Version v1.6, websocket subprotocol: ocpp1.6
        /// </summary>
        public static OCPPVersion OCPP16     { get; }
            = Register("OCPP16");

        /// <summary>
        /// OCPP Version v2.0 has been withdrawn,.therefore the value OCPP20 is treated as OCPP2.0.1.
        /// </summary>
        [Obsolete]
        public static OCPPVersion OCPP20     { get; }
            = Register("OCPP20");

        /// <summary>
        /// OCPP Version v2.0.1, websocket subprotocol: ocpp2.0.1
        /// </summary>
        public static OCPPVersion OCPP201    { get; }
            = Register("OCPP201");

        /// <summary>
        /// OCPP Version v2.1, websocket subprotocol: ocpp2.1
        /// </summary>
        public static OCPPVersion OCPP21     { get; }
            = Register("OCPP21");

        #endregion


        #region Operator overloading

        #region Operator == (OCPPVersion1, OCPPVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPPVersion1">An OCPP version.</param>
        /// <param name="OCPPVersion2">Another OCPP version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCPPVersion OCPPVersion1,
                                           OCPPVersion OCPPVersion2)

            => OCPPVersion1.Equals(OCPPVersion2);

        #endregion

        #region Operator != (OCPPVersion1, OCPPVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPPVersion1">An OCPP version.</param>
        /// <param name="OCPPVersion2">Another OCPP version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCPPVersion OCPPVersion1,
                                           OCPPVersion OCPPVersion2)

            => !OCPPVersion1.Equals(OCPPVersion2);

        #endregion

        #region Operator <  (OCPPVersion1, OCPPVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPPVersion1">An OCPP version.</param>
        /// <param name="OCPPVersion2">Another OCPP version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OCPPVersion OCPPVersion1,
                                          OCPPVersion OCPPVersion2)

            => OCPPVersion1.CompareTo(OCPPVersion2) < 0;

        #endregion

        #region Operator <= (OCPPVersion1, OCPPVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPPVersion1">An OCPP version.</param>
        /// <param name="OCPPVersion2">Another OCPP version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OCPPVersion OCPPVersion1,
                                           OCPPVersion OCPPVersion2)

            => OCPPVersion1.CompareTo(OCPPVersion2) <= 0;

        #endregion

        #region Operator >  (OCPPVersion1, OCPPVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPPVersion1">An OCPP version.</param>
        /// <param name="OCPPVersion2">Another OCPP version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OCPPVersion OCPPVersion1,
                                          OCPPVersion OCPPVersion2)

            => OCPPVersion1.CompareTo(OCPPVersion2) > 0;

        #endregion

        #region Operator >= (OCPPVersion1, OCPPVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPPVersion1">An OCPP version.</param>
        /// <param name="OCPPVersion2">Another OCPP version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OCPPVersion OCPPVersion1,
                                           OCPPVersion OCPPVersion2)

            => OCPPVersion1.CompareTo(OCPPVersion2) >= 0;

        #endregion

        #endregion

        #region IComparable<OCPPVersion> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two OCPP versions.
        /// </summary>
        /// <param name="Object">OCPP version to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OCPPVersion ocppVersion
                   ? CompareTo(ocppVersion)
                   : throw new ArgumentException("The given object is not OCPP version!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OCPPVersion)

        /// <summary>
        /// Compares two OCPP versions.
        /// </summary>
        /// <param name="OCPPVersion">OCPP version to compare with.</param>
        public Int32 CompareTo(OCPPVersion OCPPVersion)

            => String.Compare(InternalId,
                              OCPPVersion.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<OCPPVersion> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCPP versions for equality.
        /// </summary>
        /// <param name="Object">OCPP version to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OCPPVersion ocppVersion &&
                   Equals(ocppVersion);

        #endregion

        #region Equals(OCPPVersion)

        /// <summary>
        /// Compares two OCPP versions for equality.
        /// </summary>
        /// <param name="OCPPVersion">OCPP version to compare with.</param>
        public Boolean Equals(OCPPVersion OCPPVersion)

            => String.Equals(InternalId,
                             OCPPVersion.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
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
