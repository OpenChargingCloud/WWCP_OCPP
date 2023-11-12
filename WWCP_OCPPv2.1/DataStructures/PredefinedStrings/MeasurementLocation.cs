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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for measurement locations.
    /// </summary>
    public static class MeasurementLocationExtensions
    {

        /// <summary>
        /// Indicates whether this measurement location is null or empty.
        /// </summary>
        /// <param name="MeasurementLocation">A measurement location.</param>
        public static Boolean IsNullOrEmpty(this MeasurementLocation? MeasurementLocation)
            => !MeasurementLocation.HasValue || MeasurementLocation.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this measurement location is null or empty.
        /// </summary>
        /// <param name="MeasurementLocation">A measurement location.</param>
        public static Boolean IsNotNullOrEmpty(this MeasurementLocation? MeasurementLocation)
            => MeasurementLocation.HasValue && MeasurementLocation.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A measurement location.
    /// </summary>
    public readonly struct MeasurementLocation : IId,
                                                 IEquatable<MeasurementLocation>,
                                                 IComparable<MeasurementLocation>
    {

        #region Data

        private readonly static Dictionary<String, MeasurementLocation>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                   InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this measurement location is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this measurement location is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the measurement location.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new measurement location based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a measurement location.</param>
        private MeasurementLocation(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MeasurementLocation Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MeasurementLocation(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a measurement location.
        /// </summary>
        /// <param name="Text">A text representation of a measurement location.</param>
        public static MeasurementLocation Parse(String Text)
        {

            if (TryParse(Text, out var measurementLocation))
                return measurementLocation;

            throw new ArgumentException($"Invalid text representation of a measurement location: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a measurement location.
        /// </summary>
        /// <param name="Text">A text representation of a measurement location.</param>
        public static MeasurementLocation? TryParse(String Text)
        {

            if (TryParse(Text, out var measurementLocation))
                return measurementLocation;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MeasurementLocation)

        /// <summary>
        /// Try to parse the given text as a measurement location.
        /// </summary>
        /// <param name="Text">A text representation of a measurement location.</param>
        /// <param name="MeasurementLocation">The parsed measurement location.</param>
        public static Boolean TryParse(String Text, out MeasurementLocation MeasurementLocation)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MeasurementLocation))
                    MeasurementLocation = Register(Text);

                return true;

            }

            MeasurementLocation = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this measurement location.
        /// </summary>
        public MeasurementLocation Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Body
        /// </summary>
        public static MeasurementLocation Body        { get; }
            = Register("Body");

        /// <summary>
        /// Cable
        /// </summary>
        public static MeasurementLocation Cable       { get; }
            = Register("Cable");

        /// <summary>
        /// EV
        /// </summary>
        public static MeasurementLocation EV          { get; }
            = Register("EV");

        /// <summary>
        /// Inlet
        /// </summary>
        public static MeasurementLocation Inlet       { get; }
            = Register("Inlet");

        /// <summary>
        /// Outlet
        /// </summary>
        public static MeasurementLocation Outlet      { get; }
            = Register("Outlet");

        /// <summary>
        /// Measurement of an upstream meter (e.g. local grid meter).
        /// </summary>
        public static MeasurementLocation Upstream    { get; }
            = Register("Upstream");

        #endregion


        #region Operator overloading

        #region Operator == (MeasurementLocation1, MeasurementLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementLocation1">A measurement location.</param>
        /// <param name="MeasurementLocation2">Another measurement location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeasurementLocation MeasurementLocation1,
                                           MeasurementLocation MeasurementLocation2)

            => MeasurementLocation1.Equals(MeasurementLocation2);

        #endregion

        #region Operator != (MeasurementLocation1, MeasurementLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementLocation1">A measurement location.</param>
        /// <param name="MeasurementLocation2">Another measurement location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MeasurementLocation MeasurementLocation1,
                                           MeasurementLocation MeasurementLocation2)

            => !MeasurementLocation1.Equals(MeasurementLocation2);

        #endregion

        #region Operator <  (MeasurementLocation1, MeasurementLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementLocation1">A measurement location.</param>
        /// <param name="MeasurementLocation2">Another measurement location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MeasurementLocation MeasurementLocation1,
                                          MeasurementLocation MeasurementLocation2)

            => MeasurementLocation1.CompareTo(MeasurementLocation2) < 0;

        #endregion

        #region Operator <= (MeasurementLocation1, MeasurementLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementLocation1">A measurement location.</param>
        /// <param name="MeasurementLocation2">Another measurement location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MeasurementLocation MeasurementLocation1,
                                           MeasurementLocation MeasurementLocation2)

            => MeasurementLocation1.CompareTo(MeasurementLocation2) <= 0;

        #endregion

        #region Operator >  (MeasurementLocation1, MeasurementLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementLocation1">A measurement location.</param>
        /// <param name="MeasurementLocation2">Another measurement location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MeasurementLocation MeasurementLocation1,
                                          MeasurementLocation MeasurementLocation2)

            => MeasurementLocation1.CompareTo(MeasurementLocation2) > 0;

        #endregion

        #region Operator >= (MeasurementLocation1, MeasurementLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeasurementLocation1">A measurement location.</param>
        /// <param name="MeasurementLocation2">Another measurement location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MeasurementLocation MeasurementLocation1,
                                           MeasurementLocation MeasurementLocation2)

            => MeasurementLocation1.CompareTo(MeasurementLocation2) >= 0;

        #endregion

        #endregion

        #region IComparable<MeasurementLocation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two measurement locations.
        /// </summary>
        /// <param name="Object">A measurement location to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MeasurementLocation measurementLocation
                   ? CompareTo(measurementLocation)
                   : throw new ArgumentException("The given object is not a measurement location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MeasurementLocation)

        /// <summary>
        /// Compares two measurement locations.
        /// </summary>
        /// <param name="MeasurementLocation">A measurement location to compare with.</param>
        public Int32 CompareTo(MeasurementLocation MeasurementLocation)

            => String.Compare(InternalId,
                              MeasurementLocation.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MeasurementLocation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two measurement locations for equality.
        /// </summary>
        /// <param name="Object">A measurement location to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeasurementLocation measurementLocation &&
                   Equals(measurementLocation);

        #endregion

        #region Equals(MeasurementLocation)

        /// <summary>
        /// Compares two measurement locations for equality.
        /// </summary>
        /// <param name="MeasurementLocation">A measurement location to compare with.</param>
        public Boolean Equals(MeasurementLocation MeasurementLocation)

            => String.Equals(InternalId,
                             MeasurementLocation.InternalId,
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
