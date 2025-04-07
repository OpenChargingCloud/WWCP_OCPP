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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extension methods for legal status.
    /// </summary>
    public static class LegalStatusExtensions
    {

        /// <summary>
        /// Indicates whether this legal status is null or empty.
        /// </summary>
        /// <param name="LegalStatus">A legal status.</param>
        public static Boolean IsNullOrEmpty(this LegalStatus? LegalStatus)
            => !LegalStatus.HasValue || LegalStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this legal status is NOT null or empty.
        /// </summary>
        /// <param name="LegalStatus">A legal status.</param>
        public static Boolean IsNotNullOrEmpty(this LegalStatus? LegalStatus)
            => LegalStatus.HasValue && LegalStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The legal status of a charging transparency software.
    /// </summary>
    public readonly struct LegalStatus : IId<LegalStatus>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this legal status is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this legal status is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the legal status.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new legal status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a legal status.</param>
        private LegalStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a legal status.
        /// </summary>
        /// <param name="Text">A text representation of a legal status.</param>
        public static LegalStatus Parse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            throw new ArgumentException($"Invalid text representation of a legal status: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a legal status.
        /// </summary>
        /// <param name="Text">A text representation of a legal status.</param>
        public static LegalStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LegalStatus)

        /// <summary>
        /// Try to parse the given text as a legal status.
        /// </summary>
        /// <param name="Text">A text representation of a legal status.</param>
        /// <param name="LegalStatus">The parsed legal status.</param>
        public static Boolean TryParse(String Text, out LegalStatus LegalStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    LegalStatus = new LegalStatus(Text);
                    return true;
                }
                catch
                { }
            }

            LegalStatus = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this legal status.
        /// </summary>
        public LegalStatus Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static Definitions

        /// <summary>
        /// The transparency software is not legally binding, but it can be used for information purposes.
        /// </summary>
        public static LegalStatus ForInformationOnly
            => new ("ForInformationOnly");

        /// <summary>
        /// The transparency software is not legally binding, but verified by the given entity.
        /// </summary>
        public static LegalStatus Verified
            => new ("verified");

        /// <summary>
        /// The status is legally binding under the German Calibration Law.
        /// </summary>
        public static LegalStatus GermanCalibrationLaw
            => new ("GermanCalibrationLaw");

        #endregion


        #region Operator overloading

        #region Operator == (LegalStatus1, LegalStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LegalStatus1">A legal status.</param>
        /// <param name="LegalStatus2">Another legal status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (LegalStatus LegalStatus1,
                                           LegalStatus LegalStatus2)

            => LegalStatus1.Equals(LegalStatus2);

        #endregion

        #region Operator != (LegalStatus1, LegalStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LegalStatus1">A legal status.</param>
        /// <param name="LegalStatus2">Another legal status.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (LegalStatus LegalStatus1,
                                           LegalStatus LegalStatus2)

            => !LegalStatus1.Equals(LegalStatus2);

        #endregion

        #region Operator <  (LegalStatus1, LegalStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LegalStatus1">A legal status.</param>
        /// <param name="LegalStatus2">Another legal status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator < (LegalStatus LegalStatus1,
                                          LegalStatus LegalStatus2)

            => LegalStatus1.CompareTo(LegalStatus2) < 0;

        #endregion

        #region Operator <= (LegalStatus1, LegalStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LegalStatus1">A legal status.</param>
        /// <param name="LegalStatus2">Another legal status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator <= (LegalStatus LegalStatus1,
                                           LegalStatus LegalStatus2)

            => LegalStatus1.CompareTo(LegalStatus2) <= 0;

        #endregion

        #region Operator >  (LegalStatus1, LegalStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LegalStatus1">A legal status.</param>
        /// <param name="LegalStatus2">Another legal status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator > (LegalStatus LegalStatus1,
                                          LegalStatus LegalStatus2)

            => LegalStatus1.CompareTo(LegalStatus2) > 0;

        #endregion

        #region Operator >= (LegalStatus1, LegalStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LegalStatus1">A legal status.</param>
        /// <param name="LegalStatus2">Another legal status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator >= (LegalStatus LegalStatus1,
                                           LegalStatus LegalStatus2)

            => LegalStatus1.CompareTo(LegalStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<LegalStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two legal status.
        /// </summary>
        /// <param name="Object">A legal status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LegalStatus meterId
                   ? CompareTo(meterId)
                   : throw new ArgumentException("The given object is not a legal status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LegalStatus)

        /// <summary>
        /// Compares two legal status.
        /// </summary>
        /// <param name="LegalStatus">A legal status to compare with.</param>
        public Int32 CompareTo(LegalStatus LegalStatus)

            => String.Compare(InternalId,
                              LegalStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LegalStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two legal status for equality.
        /// </summary>
        /// <param name="Object">A legal status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LegalStatus meterId &&
                   Equals(meterId);

        #endregion

        #region Equals(LegalStatus)

        /// <summary>
        /// Compares two legal status for equality.
        /// </summary>
        /// <param name="LegalStatus">A legal status to compare with.</param>
        public Boolean Equals(LegalStatus LegalStatus)

            => String.Equals(InternalId,
                             LegalStatus.InternalId,
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
