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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for IdTagSimulationModes.
    /// </summary>
    public static class IdTagSimulationModeExtensions
    {

        /// <summary>
        /// Indicates whether this IdTagSimulationMode is null or empty.
        /// </summary>
        /// <param name="IdTagSimulationMode">An IdTagSimulationMode.</param>
        public static Boolean IsNullOrEmpty(this IdTagSimulationMode? IdTagSimulationMode)
            => !IdTagSimulationMode.HasValue || IdTagSimulationMode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this IdTagSimulationMode is null or empty.
        /// </summary>
        /// <param name="IdTagSimulationMode">An IdTagSimulationMode.</param>
        public static Boolean IsNotNullOrEmpty(this IdTagSimulationMode? IdTagSimulationMode)
            => IdTagSimulationMode.HasValue && IdTagSimulationMode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An IdTagSimulationMode.
    /// </summary>
    public readonly struct IdTagSimulationMode : IId,
                                                 IEquatable<IdTagSimulationMode>,
                                                 IComparable<IdTagSimulationMode>
    {

        #region Data

        private readonly static Dictionary<String, IdTagSimulationMode>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this IdTagSimulationMode is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this IdTagSimulationMode is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the IdTagSimulationMode.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered IdTagSimulationModes.
        /// </summary>
        public static IEnumerable<IdTagSimulationMode>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new IdTagSimulationMode based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an IdTagSimulationMode.</param>
        private IdTagSimulationMode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static IdTagSimulationMode Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new IdTagSimulationMode(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an IdTagSimulationMode.
        /// </summary>
        /// <param name="Text">A text representation of an IdTagSimulationMode.</param>
        public static IdTagSimulationMode Parse(String Text)
        {

            if (TryParse(Text, out var idTagSimulationMode))
                return idTagSimulationMode;

            throw new ArgumentException($"Invalid text representation of an IdTagSimulationMode: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an IdTagSimulationMode.
        /// </summary>
        /// <param name="Text">A text representation of an IdTagSimulationMode.</param>
        public static IdTagSimulationMode? TryParse(String Text)
        {

            if (TryParse(Text, out var idTagSimulationMode))
                return idTagSimulationMode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out IdTagSimulationMode)

        /// <summary>
        /// Try to parse the given text as an IdTagSimulationMode.
        /// </summary>
        /// <param name="Text">A text representation of an IdTagSimulationMode.</param>
        /// <param name="IdTagSimulationMode">The parsed IdTagSimulationMode.</param>
        public static Boolean TryParse(String Text, out IdTagSimulationMode IdTagSimulationMode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out IdTagSimulationMode))
                    IdTagSimulationMode = Register(Text);

                return true;

            }

            IdTagSimulationMode = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this IdTagSimulationMode.
        /// </summary>
        public IdTagSimulationMode Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Software simulation.
        /// </summary>
        public static IdTagSimulationMode  Software    { get; }
            = Register("Software");

        /// <summary>
        /// Hardware simulation.
        /// </summary>
        public static IdTagSimulationMode  Hardware    { get; }
            = Register("Hardware");

        #endregion


        #region Operator overloading

        #region Operator == (IdTagSimulationMode1, IdTagSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagSimulationMode1">An IdTagSimulationMode.</param>
        /// <param name="IdTagSimulationMode2">Another IdTagSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTagSimulationMode IdTagSimulationMode1,
                                           IdTagSimulationMode IdTagSimulationMode2)

            => IdTagSimulationMode1.Equals(IdTagSimulationMode2);

        #endregion

        #region Operator != (IdTagSimulationMode1, IdTagSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagSimulationMode1">An IdTagSimulationMode.</param>
        /// <param name="IdTagSimulationMode2">Another IdTagSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTagSimulationMode IdTagSimulationMode1,
                                           IdTagSimulationMode IdTagSimulationMode2)

            => !IdTagSimulationMode1.Equals(IdTagSimulationMode2);

        #endregion

        #region Operator <  (IdTagSimulationMode1, IdTagSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagSimulationMode1">An IdTagSimulationMode.</param>
        /// <param name="IdTagSimulationMode2">Another IdTagSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (IdTagSimulationMode IdTagSimulationMode1,
                                          IdTagSimulationMode IdTagSimulationMode2)

            => IdTagSimulationMode1.CompareTo(IdTagSimulationMode2) < 0;

        #endregion

        #region Operator <= (IdTagSimulationMode1, IdTagSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagSimulationMode1">An IdTagSimulationMode.</param>
        /// <param name="IdTagSimulationMode2">Another IdTagSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (IdTagSimulationMode IdTagSimulationMode1,
                                           IdTagSimulationMode IdTagSimulationMode2)

            => IdTagSimulationMode1.CompareTo(IdTagSimulationMode2) <= 0;

        #endregion

        #region Operator >  (IdTagSimulationMode1, IdTagSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagSimulationMode1">An IdTagSimulationMode.</param>
        /// <param name="IdTagSimulationMode2">Another IdTagSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (IdTagSimulationMode IdTagSimulationMode1,
                                          IdTagSimulationMode IdTagSimulationMode2)

            => IdTagSimulationMode1.CompareTo(IdTagSimulationMode2) > 0;

        #endregion

        #region Operator >= (IdTagSimulationMode1, IdTagSimulationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagSimulationMode1">An IdTagSimulationMode.</param>
        /// <param name="IdTagSimulationMode2">Another IdTagSimulationMode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (IdTagSimulationMode IdTagSimulationMode1,
                                           IdTagSimulationMode IdTagSimulationMode2)

            => IdTagSimulationMode1.CompareTo(IdTagSimulationMode2) >= 0;

        #endregion

        #endregion

        #region IComparable<IdTagSimulationMode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two IdTagSimulationModes.
        /// </summary>
        /// <param name="Object">An IdTagSimulationMode to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is IdTagSimulationMode idTagSimulationMode
                   ? CompareTo(idTagSimulationMode)
                   : throw new ArgumentException("The given object is not an IdTagSimulationMode!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(IdTagSimulationMode)

        /// <summary>
        /// Compares two IdTagSimulationModes.
        /// </summary>
        /// <param name="IdTagSimulationMode">An IdTagSimulationMode to compare with.</param>
        public Int32 CompareTo(IdTagSimulationMode IdTagSimulationMode)

            => String.Compare(InternalId,
                              IdTagSimulationMode.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<IdTagSimulationMode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two IdTagSimulationModes for equality.
        /// </summary>
        /// <param name="Object">An IdTagSimulationMode to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdTagSimulationMode idTagSimulationMode &&
                   Equals(idTagSimulationMode);

        #endregion

        #region Equals(IdTagSimulationMode)

        /// <summary>
        /// Compares two IdTagSimulationModes for equality.
        /// </summary>
        /// <param name="IdTagSimulationMode">An IdTagSimulationMode to compare with.</param>
        public Boolean Equals(IdTagSimulationMode IdTagSimulationMode)

            => String.Equals(InternalId,
                             IdTagSimulationMode.InternalId,
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
