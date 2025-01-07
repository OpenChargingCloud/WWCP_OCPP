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
    /// Extension methods for connector types.
    /// </summary>
    public static class ConnectorTypeExtensions
    {

        /// <summary>
        /// Indicates whether this connector type is null or empty.
        /// </summary>
        /// <param name="ConnectorType">A connector type.</param>
        public static Boolean IsNullOrEmpty(this ConnectorType? ConnectorType)
            => !ConnectorType.HasValue || ConnectorType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this connector type is null or empty.
        /// </summary>
        /// <param name="ConnectorType">A connector type.</param>
        public static Boolean IsNotNullOrEmpty(this ConnectorType? ConnectorType)
            => ConnectorType.HasValue && ConnectorType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A connector type.
    /// </summary>
    public readonly struct ConnectorType : IId,
                                           IEquatable<ConnectorType>,
                                           IComparable<ConnectorType>
    {

        #region Data

        private readonly static Dictionary<String, ConnectorType>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this connector type is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this connector type is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the connector type.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connector type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a connector type.</param>
        private ConnectorType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ConnectorType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ConnectorType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        public static ConnectorType Parse(String Text)
        {

            if (TryParse(Text, out var connectorType))
                return connectorType;

            throw new ArgumentException($"Invalid text representation of a connector type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        public static ConnectorType? TryParse(String Text)
        {

            if (TryParse(Text, out var connectorType))
                return connectorType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ConnectorType)

        /// <summary>
        /// Try to parse the given text as connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        /// <param name="ConnectorType">The parsed connector type.</param>
        public static Boolean TryParse(String Text, out ConnectorType ConnectorType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ConnectorType))
                    ConnectorType = Register(Text);

                return true;

            }

            ConnectorType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this connector type.
        /// </summary>
        public ConnectorType Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// Combined Charging System 1 (captive cabled) a.k.a. Combo 1
        /// </summary>
        public static ConnectorType cCCS1              { get; }
            = Register("cCCS1");

        /// <summary>
        /// Combined Charging System 2 (captive cabled) a.k.a. Combo 2
        /// </summary>
        public static ConnectorType cCCS2              { get; }
            = Register("cCCS2");

        /// <summary>
        /// ChaoJi (captive cabled) a.k.a. CHAdeMO 3.0
        /// </summary>
        public static ConnectorType cChaoJi            { get; }
            = Register("cChaoJi");

        /// <summary>
        /// JARI G105-1993 (captive cabled) a.k.a. CHAdeMO
        /// </summary>
        public static ConnectorType cG105              { get; }
            = Register("cG105");

        /// <summary>
        /// GB/T 20234.3 DC connector (captive cabled)
        /// </summary>
        public static ConnectorType cGBT_DC            { get; }
            = Register("cGBT-DC");

        /// <summary>
        /// Megawatt Charging System (captive cabled)
        /// </summary>
        public static ConnectorType cMCS               { get; }
            = Register("cMCS");

        /// <summary>
        /// North American Charging Standard (captive cabled)
        /// </summary>
        public static ConnectorType cNACS              { get; }
            = Register("cNACS");

        /// <summary>
        /// Tesla Connector (captive cabled)
        /// </summary>
        public static ConnectorType cTesla             { get; }
            = Register("cTesla");

        /// <summary>
        /// IEC62196-2 Type 1 connector (captive cabled) a.k.a. J1772
        /// </summary>
        public static ConnectorType cType1             { get; }
            = Register("cType1");

        /// <summary>
        /// IEC62196-2 Type 2 connector (captive cabled) a.k.a. Mennekes connector
        /// </summary>
        public static ConnectorType cType2             { get; }
            = Register("cType2");

        /// <summary>
        /// Ultra-ChaoJi for megawatt charging
        /// </summary>
        public static ConnectorType cUltraChaoJi       { get; }
            = Register("cUltraChaoJi");

        /// <summary>
        /// 16A 1 phase IEC60309 socket
        /// </summary>
        public static ConnectorType s309_1P_16A        { get; }
            = Register("s309-1P-16A");

        /// <summary>
        /// 32A 1 phase IEC60309 socket
        /// </summary>
        public static ConnectorType s309_1P_32A        { get; }
            = Register("s309-1P-32A");

        /// <summary>
        /// 16A 3 phase IEC60309 socket
        /// </summary>
        public static ConnectorType s309_3P_16A        { get; }
            = Register("s309-3P-16A");

        /// <summary>
        /// 32A 3 phase IEC60309 socket
        /// </summary>
        public static ConnectorType s309_3P_32A        { get; }
            = Register("s309-3P-32A");

        /// <summary>
        /// UK domestic socket a.k.a. 13Amp
        /// </summary>
        public static ConnectorType sBS1361            { get; }
            = Register("sBS1361");

        /// <summary>
        /// CEE 7/7 16A socket. May represent 7/4 & 7/5 a.k.a Schuko
        /// </summary>
        public static ConnectorType sCEE_7_7           { get; }
            = Register("sCEE-7-7");

        /// <summary>
        /// IEC62196-2 Type 2 socket a.k.a. Mennekes connector
        /// </summary>
        public static ConnectorType sType2             { get; }
            = Register("sType2");

        /// <summary>
        /// IEC62196-2 Type 2 socket a.k.a. Scame
        /// </summary>
        public static ConnectorType sType3             { get; }
            = Register("sType3");

        /// <summary>
        /// Wireless inductively coupled connection (generic)
        /// </summary>
        public static ConnectorType wInductive         { get; }
            = Register("wInductive");

        /// <summary>
        /// Wireless resonant coupled connection (generic)
        /// </summary>
        public static ConnectorType wResonant          { get; }
            = Register("wResonant");

        /// <summary>
        /// Other single phase (domestic) sockets not mentioned above, rated at no more than 16A.
        /// CEE7/17, AS3112, NEMA 5-15, NEMA 5-20, JISC8303, TIS166, SI 32, CPCS-CCC, SEV1011, etc.
        /// </summary>
        public static ConnectorType Other1PhMax16A     { get; }
            = Register("Other1PhMax16A");

        /// <summary>
        /// Other single phase sockets not mentioned above (over 16A)
        /// </summary>
        public static ConnectorType Other1PhOver16A    { get; }
            = Register("Other1PhOver16A");

        /// <summary>
        /// Other 3 phase sockets not mentioned above. NEMA14-30, NEMA14-50
        /// </summary>
        public static ConnectorType Other3Ph           { get; }
            = Register("Other3Ph");

        /// <summary>
        /// Pantograph connector
        /// </summary>
        public static ConnectorType Pan                { get; }
            = Register("Pan");

        /// <summary>
        /// Yet to be determined (e.g. before plugged in)
        /// </summary>
        public static ConnectorType Undetermined       { get; }
            = Register("Undetermined");

        /// <summary>
        /// Unknown/not determinable
        /// </summary>
        public static ConnectorType Unknown            { get; }
            = Register("Unknown");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => ConnectorType1.Equals(ConnectorType2);

        #endregion

        #region Operator != (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => !ConnectorType1.Equals(ConnectorType2);

        #endregion

        #region Operator <  (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConnectorType ConnectorType1,
                                          ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) < 0;

        #endregion

        #region Operator <= (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) <= 0;

        #endregion

        #region Operator >  (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConnectorType ConnectorType1,
                                          ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) > 0;

        #endregion

        #region Operator >= (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectorType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connector types.
        /// </summary>
        /// <param name="Object">A connector type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConnectorType connectorType
                   ? CompareTo(connectorType)
                   : throw new ArgumentException("The given object is not connector type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectorType)

        /// <summary>
        /// Compares two connector types.
        /// </summary>
        /// <param name="ConnectorType">A connector type to compare with.</param>
        public Int32 CompareTo(ConnectorType ConnectorType)

            => String.Compare(InternalId,
                              ConnectorType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ConnectorType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connector types for equality.
        /// </summary>
        /// <param name="Object">A connector type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConnectorType connectorType &&
                   Equals(connectorType);

        #endregion

        #region Equals(ConnectorType)

        /// <summary>
        /// Compares two connector types for equality.
        /// </summary>
        /// <param name="ConnectorType">A connector type to compare with.</param>
        public Boolean Equals(ConnectorType ConnectorType)

            => String.Equals(InternalId,
                             ConnectorType.InternalId,
                             StringComparison.OrdinalIgnoreCase);

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
