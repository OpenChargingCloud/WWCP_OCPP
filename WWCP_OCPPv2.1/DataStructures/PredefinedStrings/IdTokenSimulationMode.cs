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
    /// Extension methods for IdTokenSimulationModes.
    /// </summary>
    public static class IdTokenSimulationModeExtensions
    {

        /// <summary>
        /// Indicates whether this IdTokenSimulationMode is null or empty.
        /// </summary>
        /// <param name="IdTokenSimulationMode">An IdTokenSimulationMode.</param>
        public static Boolean IsNullOrEmpty(this IdTokenSimulationMode? IdTokenSimulationMode)
            => !IdTokenSimulationMode.HasValue || IdTokenSimulationMode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this IdTokenSimulationMode is null or empty.
        /// </summary>
        /// <param name="IdTokenSimulationMode">An IdTokenSimulationMode.</param>
        public static Boolean IsNotNullOrEmpty(this IdTokenSimulationMode? IdTokenSimulationMode)
            => IdTokenSimulationMode.HasValue && IdTokenSimulationMode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An IdTokenSimulationMode.
    /// </summary>
    public readonly struct IdTokenSimulationMode : IId,
                                                   IEquatable<IdTokenSimulationMode>,
                                                   IComparable<IdTokenSimulationMode>
    {

        #region Data

        private readonly static Dictionary<String, IdTokenSimulationMode>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this IdTokenSimulationMode is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this IdTokenSimulationMode is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the IdTokenSimulationMode.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered IdTokenSimulationModes.
        /// </summary>
        public static IEnumerable<IdTokenSimulationMode>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new IdTokenSimulationMode based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an IdTokenSimulationMode.</param>
        private IdTokenSimulationMode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static IdTokenSimulationMode Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new IdTokenSimulationMode(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an IdTokenSimulationMode.
        /// </summary>
        /// <param name="Text">A text representation of an IdTokenSimulationMode.</param>
        public static IdTokenSimulationMode Parse(String Text)
        {

            if (TryParse(Text, out var idTokenSimulationMode))
                return idTokenSimulationMode;

            throw new ArgumentException($"Invalid text representation of an IdTokenSimulationMode: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an IdTokenSimulationMode.
        /// </summary>
        /// <param name="Text">A text representation of an IdTokenSimulationMode.</param>
        public static IdTokenSimulationMode? TryParse(String Text)
        {

            if (TryParse(Text, out var idTokenSimulationMode))
                return idTokenSimulationMode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out IdTokenSimulationMode)

        /// <summary>
        /// Try to parse the given text as an IdTokenSimulationMode.
        /// </summary>
        /// <param name="Text">A text representation of an IdTokenSimulationMode.</param>
        /// <param name="IdTokenSimulationMode">The parsed IdTokenSimulationMode.</param>
        public static Boolean TryParse(String Text, out IdTokenSimulationMode IdTokenSimulationMode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out IdTokenSimulationMode))
                    IdTokenSimulationMode = Register(Text);

                return true;

            }

            IdTokenSimulationMode = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this IdTokenSimulationMode.
        /// </summary>
        public IdTokenSimulationMode Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Software simulation.
        /// </summary>
        public static IdTokenSimulationMode  Software    { get; }
            = Register("Software");

        /// <summary>
        /// Hardware simulation.
        /// </summary>
        public static IdTokenSimulationMode  Hardware    { get; }
            = Register("Hardware");

        #endregion


        #region Operator overloading

        #region Operator == (IdTokenSimulationMode1, IdTokenSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenSimulationMode1">An IdTokenSimulationMode.</param>
        /// <param name="IdTokenSimulationMode2">Another IdTokenSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTokenSimulationMode IdTokenSimulationMode1,
                                           IdTokenSimulationMode IdTokenSimulationMode2)

            => IdTokenSimulationMode1.Equals(IdTokenSimulationMode2);

        #endregion

        #region Operator != (IdTokenSimulationMode1, IdTokenSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenSimulationMode1">An IdTokenSimulationMode.</param>
        /// <param name="IdTokenSimulationMode2">Another IdTokenSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTokenSimulationMode IdTokenSimulationMode1,
                                           IdTokenSimulationMode IdTokenSimulationMode2)

            => !IdTokenSimulationMode1.Equals(IdTokenSimulationMode2);

        #endregion

        #region Operator <  (IdTokenSimulationMode1, IdTokenSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenSimulationMode1">An IdTokenSimulationMode.</param>
        /// <param name="IdTokenSimulationMode2">Another IdTokenSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (IdTokenSimulationMode IdTokenSimulationMode1,
                                          IdTokenSimulationMode IdTokenSimulationMode2)

            => IdTokenSimulationMode1.CompareTo(IdTokenSimulationMode2) < 0;

        #endregion

        #region Operator <= (IdTokenSimulationMode1, IdTokenSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenSimulationMode1">An IdTokenSimulationMode.</param>
        /// <param name="IdTokenSimulationMode2">Another IdTokenSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (IdTokenSimulationMode IdTokenSimulationMode1,
                                           IdTokenSimulationMode IdTokenSimulationMode2)

            => IdTokenSimulationMode1.CompareTo(IdTokenSimulationMode2) <= 0;

        #endregion

        #region Operator >  (IdTokenSimulationMode1, IdTokenSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenSimulationMode1">An IdTokenSimulationMode.</param>
        /// <param name="IdTokenSimulationMode2">Another IdTokenSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (IdTokenSimulationMode IdTokenSimulationMode1,
                                          IdTokenSimulationMode IdTokenSimulationMode2)

            => IdTokenSimulationMode1.CompareTo(IdTokenSimulationMode2) > 0;

        #endregion

        #region Operator >= (IdTokenSimulationMode1, IdTokenSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenSimulationMode1">An IdTokenSimulationMode.</param>
        /// <param name="IdTokenSimulationMode2">Another IdTokenSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (IdTokenSimulationMode IdTokenSimulationMode1,
                                           IdTokenSimulationMode IdTokenSimulationMode2)

            => IdTokenSimulationMode1.CompareTo(IdTokenSimulationMode2) >= 0;

        #endregion

        #endregion

        #region IComparable<IdTokenSimulationMode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two IdTokenSimulationModes.
        /// </summary>
        /// <param name="Object">An IdTokenSimulationMode to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is IdTokenSimulationMode idTokenSimulationMode
                   ? CompareTo(idTokenSimulationMode)
                   : throw new ArgumentException("The given object is not an IdTokenSimulationMode!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(IdTokenSimulationMode)

        /// <summary>
        /// Compares two IdTokenSimulationModes.
        /// </summary>
        /// <param name="IdTokenSimulationMode">An IdTokenSimulationMode to compare with.</param>
        public Int32 CompareTo(IdTokenSimulationMode IdTokenSimulationMode)

            => String.Compare(InternalId,
                              IdTokenSimulationMode.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<IdTokenSimulationMode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two IdTokenSimulationModes for equality.
        /// </summary>
        /// <param name="Object">An IdTokenSimulationMode to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdTokenSimulationMode idTokenSimulationMode &&
                   Equals(idTokenSimulationMode);

        #endregion

        #region Equals(IdTokenSimulationMode)

        /// <summary>
        /// Compares two IdTokenSimulationModes for equality.
        /// </summary>
        /// <param name="IdTokenSimulationMode">An IdTokenSimulationMode to compare with.</param>
        public Boolean Equals(IdTokenSimulationMode IdTokenSimulationMode)

            => String.Equals(InternalId,
                             IdTokenSimulationMode.InternalId,
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
