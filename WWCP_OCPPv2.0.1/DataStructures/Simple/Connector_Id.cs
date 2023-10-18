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
    /// Extension methods for connector identifications.
    /// </summary>
    public static class ConnectorIdExtensions
    {

        /// <summary>
        /// Indicates whether this connector identification is null or empty.
        /// </summary>
        /// <param name="ConnectorId">A connector identification.</param>
        public static Boolean IsNullOrEmpty(this Connector_Id? ConnectorId)
            => !ConnectorId.HasValue || ConnectorId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this connector identification is null or empty.
        /// </summary>
        /// <param name="ConnectorId">A connector identification.</param>
        public static Boolean IsNotNullOrEmpty(this Connector_Id? ConnectorId)
            => ConnectorId.HasValue && ConnectorId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A connector identification.
    /// </summary>
    public readonly struct Connector_Id : IId,
                                          IEquatable<Connector_Id>,
                                          IComparable<Connector_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the connector identification.
        /// </summary>
        public readonly UInt64 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => true;

        /// <summary>
        /// The length of the connector identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connector identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a connector identification.</param>
        private Connector_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a connector identification.
        /// </summary>
        /// <param name="Text">A text representation of a connector identification.</param>
        public static Connector_Id Parse(String Text)
        {

            if (TryParse(Text, out var connectorId))
                return connectorId;

            throw new ArgumentException("Invalid text representation of a connector identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a connector identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a connector identification.</param>
        public static Connector_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector identification.
        /// </summary>
        /// <param name="Text">A text representation of a connector identification.</param>
        public static Connector_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var connectorId))
                return connectorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a connector identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a connector identification.</param>
        public static Connector_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var connectorId))
                return connectorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out ConnectorId)

        /// <summary>
        /// Try to parse the given text as a connector identification.
        /// </summary>
        /// <param name="Text">A text representation of a connector identification.</param>
        /// <param name="ConnectorId">The parsed connector identification.</param>
        public static Boolean TryParse(String Text, out Connector_Id ConnectorId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                ConnectorId = new Connector_Id(number);
                return true;
            }

            ConnectorId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out ConnectorId)

        /// <summary>
        /// Try to parse the given number as a connector identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a connector identification.</param>
        /// <param name="ConnectorId">The parsed connector identification.</param>
        public static Boolean TryParse(UInt64 Number, out Connector_Id ConnectorId)
        {

            ConnectorId = new Connector_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this connector identification.
        /// </summary>
        public Connector_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => ConnectorId1.Equals(ConnectorId2);

        #endregion

        #region Operator != (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => !ConnectorId1.Equals(ConnectorId2);

        #endregion

        #region Operator <  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Connector_Id ConnectorId1,
                                          Connector_Id ConnectorId2)

            => ConnectorId1.CompareTo(ConnectorId2) < 0;

        #endregion

        #region Operator <= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => ConnectorId1.CompareTo(ConnectorId2) <= 0;

        #endregion

        #region Operator >  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Connector_Id ConnectorId1,
                                          Connector_Id ConnectorId2)

            => ConnectorId1.CompareTo(ConnectorId2) > 0;

        #endregion

        #region Operator >= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => ConnectorId1.CompareTo(ConnectorId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectorId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connector identifications.
        /// </summary>
        /// <param name="Object">A connector identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Connector_Id connectorId
                   ? CompareTo(connectorId)
                   : throw new ArgumentException("The given object is not a connector identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectorId)

        /// <summary>
        /// Compares two connector identifications.
        /// </summary>
        /// <param name="ConnectorId">A connector identification to compare with.</param>
        public Int32 CompareTo(Connector_Id ConnectorId)

            => Value.CompareTo(ConnectorId.Value);

        #endregion

        #endregion

        #region IEquatable<ConnectorId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connector identifications for equality.
        /// </summary>
        /// <param name="Object">A connector identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Connector_Id connectorId &&
                   Equals(connectorId);

        #endregion

        #region Equals(ConnectorId)

        /// <summary>
        /// Compares two connector identifications for equality.
        /// </summary>
        /// <param name="ConnectorId">A connector identification to compare with.</param>
        public Boolean Equals(Connector_Id ConnectorId)

            => Value.Equals(ConnectorId.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value.ToString();

        #endregion

    }

}
