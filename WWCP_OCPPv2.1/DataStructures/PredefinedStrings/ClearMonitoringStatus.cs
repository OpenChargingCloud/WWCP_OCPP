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
    /// Extension methods for ClearMonitoring status.
    /// </summary>
    public static class ClearMonitoringStatusExtensions
    {

        /// <summary>
        /// Indicates whether this ClearMonitoring status is null or empty.
        /// </summary>
        /// <param name="ClearMonitoringStatus">A ClearMonitoring status.</param>
        public static Boolean IsNullOrEmpty(this ClearMonitoringStatus? ClearMonitoringStatus)
            => !ClearMonitoringStatus.HasValue || ClearMonitoringStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this ClearMonitoring status is null or empty.
        /// </summary>
        /// <param name="ClearMonitoringStatus">A ClearMonitoring status.</param>
        public static Boolean IsNotNullOrEmpty(this ClearMonitoringStatus? ClearMonitoringStatus)
            => ClearMonitoringStatus.HasValue && ClearMonitoringStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A ClearMonitoring status.
    /// </summary>
    public readonly struct ClearMonitoringStatus : IId,
                                                   IEquatable<ClearMonitoringStatus>,
                                                   IComparable<ClearMonitoringStatus>
    {

        #region Data

        private readonly static Dictionary<String, ClearMonitoringStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this ClearMonitoring status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this ClearMonitoring status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the ClearMonitoring status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearMonitoring status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a ClearMonitoring status.</param>
        private ClearMonitoringStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ClearMonitoringStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ClearMonitoringStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a ClearMonitoring status.
        /// </summary>
        /// <param name="Text">A text representation of a ClearMonitoring status.</param>
        public static ClearMonitoringStatus Parse(String Text)
        {

            if (TryParse(Text, out var clearMonitoringStatus))
                return clearMonitoringStatus;

            throw new ArgumentException($"Invalid text representation of a ClearMonitoring status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a ClearMonitoring status.
        /// </summary>
        /// <param name="Text">A text representation of a ClearMonitoring status.</param>
        public static ClearMonitoringStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var clearMonitoringStatus))
                return clearMonitoringStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ClearMonitoringStatus)

        /// <summary>
        /// Try to parse the given text as a ClearMonitoring status.
        /// </summary>
        /// <param name="Text">A text representation of a ClearMonitoring status.</param>
        /// <param name="ClearMonitoringStatus">The parsed ClearMonitoring status.</param>
        public static Boolean TryParse(String Text, out ClearMonitoringStatus ClearMonitoringStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ClearMonitoringStatus))
                    ClearMonitoringStatus = Register(Text);

                return true;

            }

            ClearMonitoringStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this ClearMonitoringStatus.
        /// </summary>
        public ClearMonitoringStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static ClearMonitoringStatus  Accepted    { get; }
            = Register("Accepted");

        /// <summary>
        /// Rejected
        /// </summary>
        public static ClearMonitoringStatus  Rejected    { get; }
            = Register("Rejected");

        /// <summary>
        /// Not Found
        /// </summary>
        public static ClearMonitoringStatus  NotFound    { get; }
            = Register("NotFound");

        #endregion


        #region Operator overloading

        #region Operator == (ClearMonitoringStatus1, ClearMonitoringStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringStatus1">A ClearMonitoring status.</param>
        /// <param name="ClearMonitoringStatus2">Another ClearMonitoring status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ClearMonitoringStatus ClearMonitoringStatus1,
                                           ClearMonitoringStatus ClearMonitoringStatus2)

            => ClearMonitoringStatus1.Equals(ClearMonitoringStatus2);

        #endregion

        #region Operator != (ClearMonitoringStatus1, ClearMonitoringStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringStatus1">A ClearMonitoring status.</param>
        /// <param name="ClearMonitoringStatus2">Another ClearMonitoring status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClearMonitoringStatus ClearMonitoringStatus1,
                                           ClearMonitoringStatus ClearMonitoringStatus2)

            => !ClearMonitoringStatus1.Equals(ClearMonitoringStatus2);

        #endregion

        #region Operator <  (ClearMonitoringStatus1, ClearMonitoringStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringStatus1">A ClearMonitoring status.</param>
        /// <param name="ClearMonitoringStatus2">Another ClearMonitoring status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ClearMonitoringStatus ClearMonitoringStatus1,
                                          ClearMonitoringStatus ClearMonitoringStatus2)

            => ClearMonitoringStatus1.CompareTo(ClearMonitoringStatus2) < 0;

        #endregion

        #region Operator <= (ClearMonitoringStatus1, ClearMonitoringStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringStatus1">A ClearMonitoring status.</param>
        /// <param name="ClearMonitoringStatus2">Another ClearMonitoring status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ClearMonitoringStatus ClearMonitoringStatus1,
                                           ClearMonitoringStatus ClearMonitoringStatus2)

            => ClearMonitoringStatus1.CompareTo(ClearMonitoringStatus2) <= 0;

        #endregion

        #region Operator >  (ClearMonitoringStatus1, ClearMonitoringStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringStatus1">A ClearMonitoring status.</param>
        /// <param name="ClearMonitoringStatus2">Another ClearMonitoring status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ClearMonitoringStatus ClearMonitoringStatus1,
                                          ClearMonitoringStatus ClearMonitoringStatus2)

            => ClearMonitoringStatus1.CompareTo(ClearMonitoringStatus2) > 0;

        #endregion

        #region Operator >= (ClearMonitoringStatus1, ClearMonitoringStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringStatus1">A ClearMonitoring status.</param>
        /// <param name="ClearMonitoringStatus2">Another ClearMonitoring status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ClearMonitoringStatus ClearMonitoringStatus1,
                                           ClearMonitoringStatus ClearMonitoringStatus2)

            => ClearMonitoringStatus1.CompareTo(ClearMonitoringStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<ClearMonitoringStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two ClearMonitoring status.
        /// </summary>
        /// <param name="Object">A ClearMonitoring status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ClearMonitoringStatus ClearMonitoringStatus
                   ? CompareTo(ClearMonitoringStatus)
                   : throw new ArgumentException("The given object is not a ClearMonitoring status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ClearMonitoringStatus)

        /// <summary>
        /// Compares two ClearMonitoring status.
        /// </summary>
        /// <param name="ClearMonitoringStatus">A ClearMonitoring status to compare with.</param>
        public Int32 CompareTo(ClearMonitoringStatus ClearMonitoringStatus)

            => String.Compare(InternalId,
                              ClearMonitoringStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ClearMonitoringStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearMonitoring status for equality.
        /// </summary>
        /// <param name="Object">A ClearMonitoring status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearMonitoringStatus ClearMonitoringStatus &&
                   Equals(ClearMonitoringStatus);

        #endregion

        #region Equals(ClearMonitoringStatus)

        /// <summary>
        /// Compares two ClearMonitoring status for equality.
        /// </summary>
        /// <param name="ClearMonitoringStatus">A ClearMonitoring status to compare with.</param>
        public Boolean Equals(ClearMonitoringStatus ClearMonitoringStatus)

            => String.Equals(InternalId,
                             ClearMonitoringStatus.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
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
