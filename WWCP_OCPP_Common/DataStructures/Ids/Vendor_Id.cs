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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extension methods for a vendor identification.
    /// </summary>
    public static class VendorIdExtensions
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

        private readonly static Dictionary<String, Vendor_Id>  lookup = new (StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Properties

        public String  TextId       { get; }

        public UInt32  NumericId    { get; }


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => TextId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => TextId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the vendor identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (TextId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new vendor identification based on the given text and optional number.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        /// <param name="NumericId">An optional numeric representation of a vendor identification.</param>
        private Vendor_Id(String  Text,
                          UInt32  NumericId   = 0)
        {

            this.TextId  = Text;
            this.NumericId   = NumericId;

        }

        #endregion


        #region (private static) Register(Text, NumericId = 0)

        private static Vendor_Id Register(String  Text,
                                          UInt32  NumericId   = 0)

            => lookup.AddAndReturnValue(
                   Text,
                   new Vendor_Id(Text, NumericId)
               );

        #endregion


        #region (static) Parse   (Text, NumericId = 0)

        /// <summary>
        /// Parse the given string as a vendor identification.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        public static Vendor_Id Parse(String  Text,
                                      UInt32  NumericId   = 0)
        {

            if (TryParse(Text, out var vendorId, NumericId))
                return vendorId;

            throw new ArgumentException("The given text representation of a vendor identification is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text, NumericId = 0)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

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


        /// <summary>
        /// Try to parse the given text as a vendor identification.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        public static Vendor_Id? TryParse(String Text,
                                          UInt32 NumericId)
        {

            if (TryParse(Text, out var vendorId, NumericId))
                return vendorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out VendorId, NumericId = 0)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text as a vendor identification.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        /// <param name="VendorId">The parsed vendor identification.</param>
        public static Boolean TryParse(String         Text,
                                       out Vendor_Id  VendorId)

            => TryParse(Text,
                        out VendorId,
                        0);


        /// <summary>
        /// Try to parse the given text as a vendor identification.
        /// </summary>
        /// <param name="Text">A text representation of a vendor identification.</param>
        /// <param name="VendorId">The parsed vendor identification.</param>
        public static Boolean TryParse(String         Text,
                                       out Vendor_Id  VendorId,
                                       UInt32         NumericId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out VendorId))
                    VendorId = Register(Text, NumericId);

                return true;

            }

            VendorId = default;
            return false;

        }

        #endregion


        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a vendor identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a vendor identification.</param>
        public static Vendor_Id Parse(UInt32 Number)
        {

            if (TryParse(Number, out var vendorId))
                return vendorId;

            throw new ArgumentException("The given numeric representation of a vendor identification is invalid!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a vendor identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a vendor identification.</param>
        public static Vendor_Id? TryParse(UInt32 Number)
        {

            if (TryParse(Number, out var vendorId))
                return vendorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number, out VendorId)

        /// <summary>
        /// Try to parse the given number as a vendor identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a vendor identification.</param>
        /// <param name="VendorId">The parsed vendor identification.</param>
        public static Boolean TryParse(UInt32         Number,
                                       out Vendor_Id  VendorId)
        {

            var matches = lookup.Values.Where(vendorId => vendorId.NumericId == Number);

            if (matches.Any())
            {
                VendorId = matches.First();
                return true;
            }

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
                   new String(TextId?.ToCharArray()),
                   NumericId
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Open Charge Alliance
        /// </summary>
        public static Vendor_Id OpenChargeAlliance    { get; }
            = Register("Open Charge Alliance", 1);

        /// <summary>
        /// Open Charging Cloud
        /// </summary>
        public static Vendor_Id OpenChargingCloud     { get; }
            = Register("Open Charging Cloud",  2);

        /// <summary>
        /// GraphDefined GmbH
        /// </summary>
        public static Vendor_Id GraphDefined          { get; }
            = Register("GraphDefined",         3);

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

            => String.Compare(TextId,
                              VendorId.TextId,
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

            => String.Equals(TextId,
                             VendorId.TextId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => TextId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{TextId ?? ""} ({NumericId})";

        #endregion

    }

}
