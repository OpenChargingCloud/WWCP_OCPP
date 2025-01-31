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
    /// Extension methods for Distributed Energy Resource (DER) control types.
    /// </summary>
    public static class DERControlTypeExtensions
    {

        /// <summary>
        /// Indicates whether this DER control type is null or empty.
        /// </summary>
        /// <param name="DERControlType">A DER control type.</param>
        public static Boolean IsNullOrEmpty(this DERControlType? DERControlType)
            => !DERControlType.HasValue || DERControlType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this DER control type is null or empty.
        /// </summary>
        /// <param name="DERControlType">A DER control type.</param>
        public static Boolean IsNotNullOrEmpty(this DERControlType? DERControlType)
            => DERControlType.HasValue && DERControlType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A Distributed Energy Resource (DER) control type.
    /// </summary>
    public readonly struct DERControlType : IId,
                                            IEquatable<DERControlType>,
                                            IComparable<DERControlType>
    {

        #region Data

        private readonly static Dictionary<String, DERControlType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this DER control type is null or empty.
        /// </summary>
        public readonly  Boolean                      IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this DER control type is NOT null or empty.
        /// </summary>
        public readonly  Boolean                      IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the DER control type.
        /// </summary>
        public readonly  UInt64                       Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered DER control types.
        /// </summary>
        public static    IEnumerable<DERControlType>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DER control type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a DER control type.</param>
        private DERControlType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static DERControlType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new DERControlType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a DER control type.
        /// </summary>
        /// <param name="Text">A text representation of a DER control type.</param>
        public static DERControlType Parse(String Text)
        {

            if (TryParse(Text, out var derControlType))
                return derControlType;

            throw new ArgumentException($"Invalid text representation of a DER control type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a DER control type.
        /// </summary>
        /// <param name="Text">A text representation of a DER control type.</param>
        public static DERControlType? TryParse(String Text)
        {

            if (TryParse(Text, out var derControlType))
                return derControlType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out DERControlType)

        /// <summary>
        /// Try to parse the given text as a DER control type.
        /// </summary>
        /// <param name="Text">A text representation of a DER control type.</param>
        /// <param name="DERControlType">The parsed DER control type.</param>
        public static Boolean TryParse(String Text, out DERControlType DERControlType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out DERControlType))
                    DERControlType = Register(Text);

                return true;

            }

            DERControlType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this DER control type.
        /// </summary>
        public DERControlType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// EnterService
        /// </summary>
        public static DERControlType  EnterService               { get; }
            = Register("EnterService");

        /// <summary>
        /// FreqDroop
        /// </summary>
        public static DERControlType  FreqDroop                  { get; }
            = Register("FreqDroop");

        /// <summary>
        /// FreqWatt
        /// </summary>
        public static DERControlType  FreqWatt                   { get; }
            = Register("FreqWatt");

        /// <summary>
        /// FixedPFAbsorb
        /// </summary>
        public static DERControlType  FixedPFAbsorb              { get; }
            = Register("FixedPFAbsorb");

        /// <summary>
        /// FixedPFInject
        /// </summary>
        public static DERControlType  FixedPFInject              { get; }
            = Register("FixedPFInject");

        /// <summary>
        /// FixedVar
        /// </summary>
        public static DERControlType  FixedVar                   { get; }
            = Register("FixedVar");

        /// <summary>
        /// Gradients
        /// </summary>
        public static DERControlType  Gradients                  { get; }
            = Register("Gradients");

        /// <summary>
        /// HFMustTrip
        /// </summary>
        public static DERControlType  HFMustTrip                 { get; }
            = Register("HFMustTrip");

        /// <summary>
        /// HFMayTrip
        /// </summary>
        public static DERControlType  HFMayTrip                  { get; }
            = Register("HFMayTrip");

        /// <summary>
        /// HVMustTrip
        /// </summary>
        public static DERControlType  HVMustTrip                 { get; }
            = Register("HVMustTrip");

        /// <summary>
        /// HVMomCess
        /// </summary>
        public static DERControlType  HVMomCess                  { get; }
            = Register("HVMomCess");

        /// <summary>
        /// HVMayTrip
        /// </summary>
        public static DERControlType  HVMayTrip                  { get; }
            = Register("HVMayTrip");

        /// <summary>
        /// LimitMaxDischarge
        /// </summary>
        public static DERControlType  LimitMaxDischarge          { get; }
            = Register("LimitMaxDischarge");

        /// <summary>
        /// LFMustTrip
        /// </summary>
        public static DERControlType  LFMustTrip                 { get; }
            = Register("LFMustTrip");

        /// <summary>
        /// LVMustTrip
        /// </summary>
        public static DERControlType  LVMustTrip                 { get; }
            = Register("LVMustTrip");

        /// <summary>
        /// LVMomCess
        /// </summary>
        public static DERControlType  LVMomCess                  { get; }
            = Register("LVMomCess");

        /// <summary>
        /// LVMayTrip
        /// </summary>
        public static DERControlType  LVMayTrip                  { get; }
            = Register("LVMayTrip");

        /// <summary>
        /// PowerMonitoringMustTrip
        /// </summary>
        public static DERControlType  PowerMonitoringMustTrip    { get; }
            = Register("PowerMonitoringMustTrip");

        /// <summary>
        /// VoltVar
        /// </summary>
        public static DERControlType  VoltVar                    { get; }
            = Register("VoltVar");

        /// <summary>
        /// VoltWatt
        /// </summary>
        public static DERControlType  VoltWatt                   { get; }
            = Register("VoltWatt");

        /// <summary>
        /// WattPF
        /// </summary>
        public static DERControlType  WattPF                     { get; }
            = Register("WattPF");

        /// <summary>
        /// WattVar
        /// </summary>
        public static DERControlType  WattVar                    { get; }
            = Register("WattVar");

        #endregion


        #region Operator overloading

        #region Operator == (DERControlType1, DERControlType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlType1">A DER control type.</param>
        /// <param name="DERControlType2">Another DER control type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERControlType DERControlType1,
                                           DERControlType DERControlType2)

            => DERControlType1.Equals(DERControlType2);

        #endregion

        #region Operator != (DERControlType1, DERControlType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlType1">A DER control type.</param>
        /// <param name="DERControlType2">Another DER control type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERControlType DERControlType1,
                                           DERControlType DERControlType2)

            => !DERControlType1.Equals(DERControlType2);

        #endregion

        #region Operator <  (DERControlType1, DERControlType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlType1">A DER control type.</param>
        /// <param name="DERControlType2">Another DER control type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DERControlType DERControlType1,
                                          DERControlType DERControlType2)

            => DERControlType1.CompareTo(DERControlType2) < 0;

        #endregion

        #region Operator <= (DERControlType1, DERControlType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlType1">A DER control type.</param>
        /// <param name="DERControlType2">Another DER control type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DERControlType DERControlType1,
                                           DERControlType DERControlType2)

            => DERControlType1.CompareTo(DERControlType2) <= 0;

        #endregion

        #region Operator >  (DERControlType1, DERControlType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlType1">A DER control type.</param>
        /// <param name="DERControlType2">Another DER control type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DERControlType DERControlType1,
                                          DERControlType DERControlType2)

            => DERControlType1.CompareTo(DERControlType2) > 0;

        #endregion

        #region Operator >= (DERControlType1, DERControlType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlType1">A DER control type.</param>
        /// <param name="DERControlType2">Another DER control type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DERControlType DERControlType1,
                                           DERControlType DERControlType2)

            => DERControlType1.CompareTo(DERControlType2) >= 0;

        #endregion

        #endregion

        #region IComparable<DERControlType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two DER control types.
        /// </summary>
        /// <param name="Object">A DER control type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DERControlType derControlType
                   ? CompareTo(derControlType)
                   : throw new ArgumentException("The given object is not a DER control type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DERControlType)

        /// <summary>
        /// Compares two DER control types.
        /// </summary>
        /// <param name="DERControlType">A DER control type to compare with.</param>
        public Int32 CompareTo(DERControlType DERControlType)

            => String.Compare(InternalId,
                              DERControlType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<DERControlType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER control types for equality.
        /// </summary>
        /// <param name="Object">A DER control type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERControlType derControlType &&
                   Equals(derControlType);

        #endregion

        #region Equals(DERControlType)

        /// <summary>
        /// Compares two DER control types for equality.
        /// </summary>
        /// <param name="DERControlType">A DER control type to compare with.</param>
        public Boolean Equals(DERControlType DERControlType)

            => String.Equals(InternalId,
                             DERControlType.InternalId,
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
