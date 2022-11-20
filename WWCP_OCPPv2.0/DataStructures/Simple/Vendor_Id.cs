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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for a vendor identification.
    /// </summary>
    public static class VendorIdExtentions
    {

        /// <summary>
        /// Indicates whether this vendor identification is null or empty.
        /// </summary>
        /// <param name="VendorId">A vendor identification.</param>
        public static Boolean IsNullOrEmpty(this Vendor_Id? VendorId)
            => !VendorId.HasValue || VendorId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this vendor identification is null or empty.
        /// </summary>
        /// <param name="VendorId">A vendor identification.</param>
        public static Boolean IsNotNullOrEmpty(this Vendor_Id? VendorId)
            => VendorId.HasValue && VendorId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A vendor identification.
    /// </summary>
    public readonly struct Vendor_Id : IId,
                                       IEquatable<Vendor_Id>,
                                       IComparable<Vendor_Id>
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
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the vendor identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new vendor identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        private Vendor_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a vendor identification.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        public static Vendor_Id Parse(String Text)
        {

            if (TryParse(Text, out var vendorId))
                return vendorId;

            throw new ArgumentException("The given text representation of a vendor identification is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a vendor identification.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        public static Vendor_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var vendorId))
                return vendorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out VendorId)

        /// <summary>
        /// Try to parse the given text as a vendor identification.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        /// <param name="VendorId">The parsed vendor identification.</param>
        public static Boolean TryParse(String Text, out Vendor_Id VendorId)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                VendorId = default;
                return false;
            }

            #endregion

            try
            {
                VendorId = new Vendor_Id(Text);
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
        /// Clone this vendor identification.
        /// </summary>
        public Vendor_Id Clone

            => new (
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">A vendor identification.</param>
        /// <param name="VendorId2">Another vendor identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Vendor_Id VendorId1,
                                           Vendor_Id VendorId2)

            => VendorId1.Equals(VendorId2);

        #endregion

        #region Operator != (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">A vendor identification.</param>
        /// <param name="VendorId2">Another vendor identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Vendor_Id VendorId1,
                                           Vendor_Id VendorId2)

            => !VendorId1.Equals(VendorId2);

        #endregion

        #region Operator <  (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">A vendor identification.</param>
        /// <param name="VendorId2">Another vendor identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Vendor_Id VendorId1,
                                          Vendor_Id VendorId2)

            => VendorId1.CompareTo(VendorId2) < 0;

        #endregion

        #region Operator <= (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">A vendor identification.</param>
        /// <param name="VendorId2">Another vendor identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Vendor_Id VendorId1,
                                           Vendor_Id VendorId2)

            => VendorId1.CompareTo(VendorId2) <= 0;

        #endregion

        #region Operator >  (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">A vendor identification.</param>
        /// <param name="VendorId2">Another vendor identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Vendor_Id VendorId1,
                                          Vendor_Id VendorId2)

            => VendorId1.CompareTo(VendorId2) > 0;

        #endregion

        #region Operator >= (VendorId1, VendorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VendorId1">A vendor identification.</param>
        /// <param name="VendorId2">Another vendor identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Vendor_Id VendorId1,
                                           Vendor_Id VendorId2)

            => VendorId1.CompareTo(VendorId2) >= 0;

        #endregion

        #endregion

        #region IComparable<VendorId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two vendor identifications.
        /// </summary>
        /// <param name="Object">A vendor identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Vendor_Id vendorId
                   ? CompareTo(vendorId)
                   : throw new ArgumentException("The given object is not a vendor identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VendorId)

        /// <summary>
        /// Compares two vendor identifications.
        /// </summary>
        /// <param name="VendorId">A vendor identification to compare with.</param>
        public Int32 CompareTo(Vendor_Id VendorId)

            => String.Compare(InternalId,
                              VendorId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<VendorId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two vendor identifications for equality.
        /// </summary>
        /// <param name="Object">A vendor identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Vendor_Id vendorId &&
                   Equals(vendorId);

        #endregion

        #region Equals(VendorId)

        /// <summary>
        /// Compares two vendor identifications for equality.
        /// </summary>
        /// <param name="VendorId">A vendor identification to compare with.</param>
        public Boolean Equals(Vendor_Id VendorId)

            => String.Equals(InternalId,
                             VendorId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
