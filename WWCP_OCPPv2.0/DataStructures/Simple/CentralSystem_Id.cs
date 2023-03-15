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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for central system identifications.
    /// </summary>
    public static class CSMSIdExtensions
    {

        /// <summary>
        /// Indicates whether this central system identification is null or empty.
        /// </summary>
        /// <param name="CSMSId">A central system identification.</param>
        public static Boolean IsNullOrEmpty(this CSMS_Id? CSMSId)
            => !CSMSId.HasValue || CSMSId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this central system identification is null or empty.
        /// </summary>
        /// <param name="CSMSId">A central system identification.</param>
        public static Boolean IsNotNullOrEmpty(this CSMS_Id? CSMSId)
            => CSMSId.HasValue && CSMSId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A central system identification.
    /// </summary>
    public readonly struct CSMS_Id : IId,
                                              IEquatable<CSMS_Id>,
                                              IComparable<CSMS_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
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
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the central system identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the central system identification.</param>
        private CSMS_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) NewRandom(Length = 30)

        /// <summary>
        /// Create a new random central system identification.
        /// </summary>
        /// <param name="Length">The expected length of the central system identification.</param>
        public static CSMS_Id NewRandom(Byte Length = 30)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as central system identification.
        /// </summary>
        /// <param name="Text">A text representation of a central system identification.</param>
        public static CSMS_Id Parse(String Text)
        {

            if (TryParse(Text, out var centralSystemId))
                return centralSystemId;

            throw new ArgumentException("Invalid text representation of a central system identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as central system identification.
        /// </summary>
        /// <param name="Text">A text representation of a central system identification.</param>
        public static CSMS_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var centralSystemId))
                return centralSystemId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CSMSId)

        /// <summary>
        /// Try to parse the given text as central system identification.
        /// </summary>
        /// <param name="Text">A text representation of a central system identification.</param>
        /// <param name="CSMSId">The parsed central system identification.</param>
        public static Boolean TryParse(String Text, out CSMS_Id CSMSId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                CSMSId = new CSMS_Id(Text.Trim());
                return true;
            }

            CSMSId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this central system identification.
        /// </summary>
        public CSMS_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CSMSId1, CSMSId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSMSId1">A central system identification.</param>
        /// <param name="CSMSId2">Another central system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CSMS_Id CSMSId1,
                                           CSMS_Id CSMSId2)

            => CSMSId1.Equals(CSMSId2);

        #endregion

        #region Operator != (CSMSId1, CSMSId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSMSId1">A central system identification.</param>
        /// <param name="CSMSId2">Another central system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CSMS_Id CSMSId1,
                                           CSMS_Id CSMSId2)

            => !CSMSId1.Equals(CSMSId2);

        #endregion

        #region Operator <  (CSMSId1, CSMSId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSMSId1">A central system identification.</param>
        /// <param name="CSMSId2">Another central system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CSMS_Id CSMSId1,
                                          CSMS_Id CSMSId2)

            => CSMSId1.CompareTo(CSMSId2) < 0;

        #endregion

        #region Operator <= (CSMSId1, CSMSId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSMSId1">A central system identification.</param>
        /// <param name="CSMSId2">Another central system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CSMS_Id CSMSId1,
                                           CSMS_Id CSMSId2)

            => CSMSId1.CompareTo(CSMSId2) <= 0;

        #endregion

        #region Operator >  (CSMSId1, CSMSId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSMSId1">A central system identification.</param>
        /// <param name="CSMSId2">Another central system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CSMS_Id CSMSId1,
                                          CSMS_Id CSMSId2)

            => CSMSId1.CompareTo(CSMSId2) > 0;

        #endregion

        #region Operator >= (CSMSId1, CSMSId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CSMSId1">A central system identification.</param>
        /// <param name="CSMSId2">Another central system identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CSMS_Id CSMSId1,
                                           CSMS_Id CSMSId2)

            => CSMSId1.CompareTo(CSMSId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CSMSId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two central system identifications.
        /// </summary>
        /// <param name="Object">A central system identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CSMS_Id centralSystemId
                   ? CompareTo(centralSystemId)
                   : throw new ArgumentException("The given object is not a central system identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CSMSId)

        /// <summary>
        /// Compares two central system identifications.
        /// </summary>
        /// <param name="CSMSId">A central system identification to compare with.</param>
        public Int32 CompareTo(CSMS_Id CSMSId)

            => String.Compare(InternalId,
                              CSMSId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CSMSId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two central system identifications for equality.
        /// </summary>
        /// <param name="Object">A central system identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CSMS_Id centralSystemId &&
                   Equals(centralSystemId);

        #endregion

        #region Equals(CSMSId)

        /// <summary>
        /// Compares two central system identifications for equality.
        /// </summary>
        /// <param name="CSMSId">A central system identification to compare with.</param>
        public Boolean Equals(CSMS_Id CSMSId)

            => String.Equals(InternalId,
                             CSMSId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

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
