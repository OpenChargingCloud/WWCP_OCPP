/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// An EVSE identification.
    /// </summary>
    public readonly struct EVSE_Id : IId,
                                     IEquatable<EVSE_Id>,
                                     IComparable<EVSE_Id>
    {

        #region Data

        private readonly UInt64 _Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) _Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EVSE identification.
        /// </summary>
        /// <param name="Token">An integer.</param>
        private EVSE_Id(UInt64 Token)
        {
            this._Value = Token;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of a EVSE identification.</param>
        public static EVSE_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a EVSE identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out EVSE_Id EVSEId))
                return EVSEId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of a EVSE identification is invalid!");

        }

        #endregion

        #region (static) Parse   (Integer)

        /// <summary>
        /// Parse the given number as a EVSE identification.
        /// </summary>
        public static EVSE_Id Parse(UInt64 Integer)
            => new EVSE_Id(Integer);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of a EVSE identification.</param>
        public static EVSE_Id? TryParse(String Text)
        {

            if (TryParse(Text, out EVSE_Id EVSEId))
                return EVSEId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a EVSE identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a EVSE identification.</param>
        public static EVSE_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out EVSE_Id EVSEId))
                return EVSEId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out ConnectorId)

        /// <summary>
        /// Try to parse the given string as a EVSE identification.
        /// </summary>
        /// <param name="Text">A text representation of a EVSE identification.</param>
        /// <param name="ConnectorId">The parsed EVSE identification.</param>
        public static Boolean TryParse(String Text, out EVSE_Id ConnectorId)
        {

            #region Initial checks

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
            {
                ConnectorId = default;
                return false;
            }

            #endregion

            if (UInt64.TryParse(Text, out UInt64 number))
            {
                ConnectorId = new EVSE_Id(number);
                return true;
            }

            ConnectorId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out ConnectorId)

        /// <summary>
        /// Try to parse the given number as a EVSE identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a EVSE identification.</param>
        /// <param name="ConnectorId">The parsed EVSE identification.</param>
        public static Boolean TryParse(UInt64 Number, out EVSE_Id ConnectorId)
        {

            ConnectorId = new EVSE_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this EVSE identification.
        /// </summary>
        public EVSE_Id Clone
            => new EVSE_Id(_Value);

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">An EVSE identification.</param>
        /// <param name="ConnectorId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE_Id ConnectorId1, EVSE_Id ConnectorId2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ConnectorId1, ConnectorId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ConnectorId1 == null) || ((Object) ConnectorId2 == null))
                return false;

            if ((Object) ConnectorId1 == null)
                throw new ArgumentNullException(nameof(ConnectorId1),  "The given EVSE identification must not be null!");

            return ConnectorId1.Equals(ConnectorId2);

        }

        #endregion

        #region Operator != (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">An EVSE identification.</param>
        /// <param name="ConnectorId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSE_Id ConnectorId1, EVSE_Id ConnectorId2)
            => !(ConnectorId1 == ConnectorId2);

        #endregion

        #region Operator <  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">An EVSE identification.</param>
        /// <param name="ConnectorId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSE_Id ConnectorId1, EVSE_Id ConnectorId2)
        {

            if ((Object) ConnectorId1 == null)
                throw new ArgumentNullException(nameof(ConnectorId1),  "The given EVSE identification must not be null!");

            return ConnectorId1.CompareTo(ConnectorId2) < 0;

        }

        #endregion

        #region Operator <= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">An EVSE identification.</param>
        /// <param name="ConnectorId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSE_Id ConnectorId1, EVSE_Id ConnectorId2)
            => !(ConnectorId1 > ConnectorId2);

        #endregion

        #region Operator >  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">An EVSE identification.</param>
        /// <param name="ConnectorId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSE_Id ConnectorId1, EVSE_Id ConnectorId2)
        {

            if ((Object) ConnectorId1 == null)
                throw new ArgumentNullException(nameof(ConnectorId1),  "The given EVSE identification must not be null!");

            return ConnectorId1.CompareTo(ConnectorId2) > 0;

        }

        #endregion

        #region Operator >= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">An EVSE identification.</param>
        /// <param name="ConnectorId2">Another EVSE identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSE_Id ConnectorId1, EVSE_Id ConnectorId2)
            => !(ConnectorId1 < ConnectorId2);

        #endregion

        #endregion

        #region IComparable<ConnectorId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            if (!(Object is EVSE_Id ConnectorId))
                throw new ArgumentException("The given object is not a EVSE identification!", nameof(Object));

            return CompareTo(ConnectorId);

        }

        #endregion

        #region CompareTo(ConnectorId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId">An object to compare with.</param>
        public Int32 CompareTo(EVSE_Id ConnectorId)
        {

            if ((Object) ConnectorId == null)
                throw new ArgumentNullException(nameof(ConnectorId),  "The given EVSE identification must not be null!");

            return _Value.CompareTo(ConnectorId._Value);

        }

        #endregion

        #endregion

        #region IEquatable<ConnectorId> Members

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

            if (!(Object is EVSE_Id ConnectorId))
                return false;

            return Equals(ConnectorId);

        }

        #endregion

        #region Equals(ConnectorId)

        /// <summary>
        /// Compares two EVSE identifications for equality.
        /// </summary>
        /// <param name="ConnectorId">An EVSE identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EVSE_Id ConnectorId)
        {

            if ((Object) ConnectorId == null)
                return false;

            return _Value.Equals(ConnectorId._Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => _Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => _Value.ToString();

        #endregion


    }

}
