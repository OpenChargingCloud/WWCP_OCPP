/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A human language identifier, as defined in rfc5646.
    /// </summary>
    public readonly struct Language_Id : IId,
                                      IEquatable<Language_Id>,
                                      IComparable<Language_Id>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new human language identifier.
        /// </summary>
        /// <param name="Text">A string.</param>
        private Language_Id(String  Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a human language.
        /// </summary>
        /// <param name="Text">A text representation of a human language.</param>
        public static Language_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a human language must not be null or empty!");

            #endregion

            if (TryParse(Text, out Language_Id vendorId))
                return vendorId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of a human language is invalid!");

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a human language.
        /// </summary>
        /// <param name="Text">A text representation of a human language.</param>
        public static Language_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Language_Id vendorId))
                return vendorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out VendorId)

        /// <summary>
        /// Try to parse the given string as a human language.
        /// </summary>
        /// <param name="Text">A text representation of a human language.</param>
        /// <param name="VendorId">The parsed human language.</param>
        public static Boolean TryParse(String Text, out Language_Id VendorId)
        {

            #region Initial checks

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
            {
                VendorId = default;
                return false;
            }

            #endregion

            try
            {
                VendorId = new Language_Id(Text);
                return true;
            }
            catch (Exception)
            { }

            VendorId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge box identification.
        /// </summary>
        public Language_Id Clone
            => new Language_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">An charge box identification.</param>
        /// <param name="VendorId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Language_Id VendorId1, Language_Id VendorId2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VendorId1, VendorId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) VendorId1 == null) || ((Object) VendorId2 == null))
                return false;

            if ((Object) VendorId1 == null)
                throw new ArgumentNullException(nameof(VendorId1),  "The given charge box identification must not be null!");

            return VendorId1.Equals(VendorId2);

        }

        #endregion

        #region Operator != (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">An charge box identification.</param>
        /// <param name="VendorId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Language_Id VendorId1, Language_Id VendorId2)
            => !(VendorId1 == VendorId2);

        #endregion

        #region Operator <  (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">An charge box identification.</param>
        /// <param name="VendorId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Language_Id VendorId1, Language_Id VendorId2)
        {

            if ((Object) VendorId1 == null)
                throw new ArgumentNullException(nameof(VendorId1),  "The given charge box identification must not be null!");

            return VendorId1.CompareTo(VendorId2) < 0;

        }

        #endregion

        #region Operator <= (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">An charge box identification.</param>
        /// <param name="VendorId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Language_Id VendorId1, Language_Id VendorId2)
            => !(VendorId1 > VendorId2);

        #endregion

        #region Operator >  (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">An charge box identification.</param>
        /// <param name="VendorId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Language_Id VendorId1, Language_Id VendorId2)
        {

            if ((Object) VendorId1 == null)
                throw new ArgumentNullException(nameof(VendorId1),  "The given charge box identification must not be null!");

            return VendorId1.CompareTo(VendorId2) > 0;

        }

        #endregion

        #region Operator >= (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">An charge box identification.</param>
        /// <param name="VendorId2">Another charge box identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Language_Id VendorId1, Language_Id VendorId2)
            => !(VendorId1 < VendorId2);

        #endregion

        #endregion

        #region IComparable<VendorId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            // Check if the given object is a charge box identification.
            if (!(Object is Language_Id))
                throw new ArgumentException("The given object is not a charge box identification!", nameof(Object));

            return CompareTo((Language_Id) Object);

        }

        #endregion

        #region CompareTo(VendorId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId">An object to compare with.</param>
        public Int32 CompareTo(Language_Id VendorId)
        {

            if ((Object) VendorId == null)
                throw new ArgumentNullException(nameof(VendorId),  "The given charge box identification must not be null!");

            return String.Compare(InternalId, VendorId.InternalId, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<VendorId> Members

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

            // Check if the given object is a charge box identification.
            if (!(Object is Language_Id))
                return false;

            return this.Equals((Language_Id) Object);

        }

        #endregion

        #region Equals(VendorId)

        /// <summary>
        /// Compares two charge box identifications for equality.
        /// </summary>
        /// <param name="VendorId">An charge box identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Language_Id VendorId)
        {

            if ((Object) VendorId == null)
                return false;

            return InternalId.Equals(VendorId.InternalId);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion


    }

}
