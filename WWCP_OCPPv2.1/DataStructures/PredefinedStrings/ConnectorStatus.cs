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
    /// Extension methods for connector status.
    /// </summary>
    public static class ConnectorStatusExtensions
    {

        /// <summary>
        /// Indicates whether this connector status is null or empty.
        /// </summary>
        /// <param name="ConnectorStatus">A connector status.</param>
        public static Boolean IsNullOrEmpty(this ConnectorStatus? ConnectorStatus)
            => !ConnectorStatus.HasValue || ConnectorStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this connector status is null or empty.
        /// </summary>
        /// <param name="ConnectorStatus">A connector status.</param>
        public static Boolean IsNotNullOrEmpty(this ConnectorStatus? ConnectorStatus)
            => ConnectorStatus.HasValue && ConnectorStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A connector status.
    /// </summary>
    public readonly struct ConnectorStatus : IId,
                                             IEquatable<ConnectorStatus>,
                                             IComparable<ConnectorStatus>
    {

        #region Data

        private readonly static Dictionary<String, ConnectorStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                               InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this connector status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this connector status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the connector status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connector status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a connector status.</param>
        private ConnectorStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ConnectorStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ConnectorStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a connector status.
        /// </summary>
        /// <param name="Text">A text representation of a connector status.</param>
        public static ConnectorStatus Parse(String Text)
        {

            if (TryParse(Text, out var connectorStatus))
                return connectorStatus;

            throw new ArgumentException($"Invalid text representation of a connector status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector status.
        /// </summary>
        /// <param name="Text">A text representation of a connector status.</param>
        public static ConnectorStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var connectorStatus))
                return connectorStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ConnectorStatus)

        /// <summary>
        /// Try to parse the given text as a connector status.
        /// </summary>
        /// <param name="Text">A text representation of a connector status.</param>
        /// <param name="ConnectorStatus">The parsed connector status.</param>
        public static Boolean TryParse(String Text, out ConnectorStatus ConnectorStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ConnectorStatus))
                    ConnectorStatus = Register(Text);

                return true;

            }

            ConnectorStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this connector status.
        /// </summary>
        public ConnectorStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Unknown connector status.
        /// </summary>
        public static ConnectorStatus Unknown        { get; }
            = Register("Unknown");

        /// <summary>
        /// Available.
        /// </summary>
        public static ConnectorStatus Available      { get; }
            = Register("Available");

        /// <summary>
        /// Occupied.
        /// </summary>
        public static ConnectorStatus Occupied       { get; }
            = Register("Occupied");

        /// <summary>
        /// Reserved.
        /// </summary>
        public static ConnectorStatus Reserved       { get; }
            = Register("Reserved");

        /// <summary>
        /// Unavailable.
        /// </summary>
        public static ConnectorStatus Unavailable    { get; }
            = Register("Unavailable");

        /// <summary>
        /// Faulted.
        /// </summary>
        public static ConnectorStatus Faulted        { get; }
            = Register("Faulted");

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorStatus1, ConnectorStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorStatus1">A connector status.</param>
        /// <param name="ConnectorStatus2">Another connector status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConnectorStatus ConnectorStatus1,
                                           ConnectorStatus ConnectorStatus2)

            => ConnectorStatus1.Equals(ConnectorStatus2);

        #endregion

        #region Operator != (ConnectorStatus1, ConnectorStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorStatus1">A connector status.</param>
        /// <param name="ConnectorStatus2">Another connector status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConnectorStatus ConnectorStatus1,
                                           ConnectorStatus ConnectorStatus2)

            => !ConnectorStatus1.Equals(ConnectorStatus2);

        #endregion

        #region Operator <  (ConnectorStatus1, ConnectorStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorStatus1">A connector status.</param>
        /// <param name="ConnectorStatus2">Another connector status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConnectorStatus ConnectorStatus1,
                                          ConnectorStatus ConnectorStatus2)

            => ConnectorStatus1.CompareTo(ConnectorStatus2) < 0;

        #endregion

        #region Operator <= (ConnectorStatus1, ConnectorStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorStatus1">A connector status.</param>
        /// <param name="ConnectorStatus2">Another connector status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConnectorStatus ConnectorStatus1,
                                           ConnectorStatus ConnectorStatus2)

            => ConnectorStatus1.CompareTo(ConnectorStatus2) <= 0;

        #endregion

        #region Operator >  (ConnectorStatus1, ConnectorStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorStatus1">A connector status.</param>
        /// <param name="ConnectorStatus2">Another connector status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConnectorStatus ConnectorStatus1,
                                          ConnectorStatus ConnectorStatus2)

            => ConnectorStatus1.CompareTo(ConnectorStatus2) > 0;

        #endregion

        #region Operator >= (ConnectorStatus1, ConnectorStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorStatus1">A connector status.</param>
        /// <param name="ConnectorStatus2">Another connector status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConnectorStatus ConnectorStatus1,
                                           ConnectorStatus ConnectorStatus2)

            => ConnectorStatus1.CompareTo(ConnectorStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectorStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connector status.
        /// </summary>
        /// <param name="Object">A connector status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConnectorStatus connectorStatus
                   ? CompareTo(connectorStatus)
                   : throw new ArgumentException("The given object is not a connector status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectorStatus)

        /// <summary>
        /// Compares two connector status.
        /// </summary>
        /// <param name="ConnectorStatus">A connector status to compare with.</param>
        public Int32 CompareTo(ConnectorStatus ConnectorStatus)

            => String.Compare(InternalId,
                              ConnectorStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ConnectorStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connector status for equality.
        /// </summary>
        /// <param name="Object">A connector status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConnectorStatus connectorStatus &&
                   Equals(connectorStatus);

        #endregion

        #region Equals(ConnectorStatus)

        /// <summary>
        /// Compares two connector status for equality.
        /// </summary>
        /// <param name="ConnectorStatus">A connector status to compare with.</param>
        public Boolean Equals(ConnectorStatus ConnectorStatus)

            => String.Equals(InternalId,
                             ConnectorStatus.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
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
