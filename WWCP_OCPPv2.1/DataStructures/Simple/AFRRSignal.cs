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
    /// Extension methods for automatic frequency restoration reserve (AFRR) signals.
    /// </summary>
    public static class AFRRSignalExtensions
    {

        /// <summary>
        /// Indicates whether this AFRR signal is null or empty.
        /// </summary>
        /// <param name="AFRRSignal">An AFRR signal.</param>
        public static Boolean IsNullOrEmpty(this AFRR_Signal? AFRRSignal)
            => !AFRRSignal.HasValue || AFRRSignal.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this AFRR signal is NOT null or empty.
        /// </summary>
        /// <param name="AFRRSignal">An AFRR signal.</param>
        public static Boolean IsNotNullOrEmpty(this AFRR_Signal? AFRRSignal)
            => AFRRSignal.HasValue && AFRRSignal.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An automatic frequency restoration reserve (AFRR) signal.
    /// </summary>
    public readonly struct AFRR_Signal : IId<AFRR_Signal>
    {

        #region Properties

        /// <summary>
        /// The internal identification.
        /// </summary>
        public SByte   Value    { get; }


        /// <summary>
        /// Indicates whether this AFRR signal is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this AFRR signal is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => true;

        /// <summary>
        /// The length of the AFRR signal.
        /// </summary>
        public UInt64 Length
            => 1;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new automatic frequency restoration reserve (AFRR) signal based on the given signed number.
        /// </summary>
        /// <param name="Value">The numeric representation of an AFRR signal.</param>
        private AFRR_Signal(SByte Value)
        {
            this.Value = Value;
        }

        #endregion


        #region (static) Parse    (Value)

        /// <summary>
        /// Parse the given signed number as an AFRR signal.
        /// </summary>
        /// <param name="Value">A numeric representation of an AFRR signal.</param>
        public static AFRR_Signal Parse(SByte Value)
        {

            if (TryParse(Value, out var afrrSignal))
                return afrrSignal;

            throw new ArgumentException($"Invalid numeric representation of an AFRR signal: '{Value}'!",
                                        nameof(Value));

        }

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as an AFRR signal.
        /// </summary>
        /// <param name="Text">A text representation of an AFRR signal.</param>
        public static AFRR_Signal Parse(String Text)
        {

            if (TryParse(Text, out var eMAId))
                return eMAId;

            throw new ArgumentException($"Invalid text representation of an AFRR signal: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Value)

        /// <summary>
        /// Try to parse the given signed number as an AFRR signal.
        /// </summary>
        /// <param name="Value">A numeric representation of an AFRR signal.</param>
        public static AFRR_Signal? TryParse(SByte Value)
        {

            if (TryParse(Value, out var afrrSignal))
                return afrrSignal;

            return null;

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as an AFRR signal.
        /// </summary>
        /// <param name="Value">A text representation of an AFRR signal.</param>
        public static AFRR_Signal? TryParse(String Text)
        {

            if (TryParse(Text, out var afrrSignal))
                return afrrSignal;

            return null;

        }

        #endregion

        #region (static) TryParse (Value, out AFRRSignal)

        /// <summary>
        /// Try to parse the given signed number as an AFRR signal.
        /// </summary>
        /// <param name="Value">A numeric representation of an AFRR signal.</param>
        /// <param name="AFRRSignal">The parsed AFRR signal.</param>
        public static Boolean TryParse(SByte Value, out AFRR_Signal AFRRSignal)
        {

            try
            {
                AFRRSignal = new AFRR_Signal(Value);
                return true;
            }
            catch
            { }

            AFRRSignal = default;
            return false;

        }

        #endregion

        #region (static) TryParse (Text,  out AFRRSignal)

        /// <summary>
        /// Try to parse the given text as an AFRR signal.
        /// </summary>
        /// <param name="Text">A text representation of an AFRR signal.</param>
        /// <param name="AFRRSignal">The parsed AFRR signal.</param>
        public static Boolean TryParse(String Text, out AFRR_Signal AFRRSignal)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                SByte.TryParse(Text, out var value))
            {
                try
                {
                    AFRRSignal = new AFRR_Signal(value);
                    return true;
                }
                catch
                { }
            }

            AFRRSignal = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this AFRR signal.
        /// </summary>
        public AFRR_Signal Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (AFRRSignal1, AFRRSignal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AFRRSignal1">An AFRR signal.</param>
        /// <param name="AFRRSignal2">Another AFRR signal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AFRR_Signal AFRRSignal1,
                                           AFRR_Signal AFRRSignal2)

            => AFRRSignal1.Equals(AFRRSignal2);

        #endregion

        #region Operator != (AFRRSignal1, AFRRSignal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AFRRSignal1">An AFRR signal.</param>
        /// <param name="AFRRSignal2">Another AFRR signal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AFRR_Signal AFRRSignal1,
                                           AFRR_Signal AFRRSignal2)

            => !AFRRSignal1.Equals(AFRRSignal2);

        #endregion

        #region Operator <  (AFRRSignal1, AFRRSignal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AFRRSignal1">An AFRR signal.</param>
        /// <param name="AFRRSignal2">Another AFRR signal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AFRR_Signal AFRRSignal1,
                                          AFRR_Signal AFRRSignal2)

            => AFRRSignal1.CompareTo(AFRRSignal2) < 0;

        #endregion

        #region Operator <= (AFRRSignal1, AFRRSignal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AFRRSignal1">An AFRR signal.</param>
        /// <param name="AFRRSignal2">Another AFRR signal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AFRR_Signal AFRRSignal1,
                                           AFRR_Signal AFRRSignal2)

            => AFRRSignal1.CompareTo(AFRRSignal2) <= 0;

        #endregion

        #region Operator >  (AFRRSignal1, AFRRSignal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AFRRSignal1">An AFRR signal.</param>
        /// <param name="AFRRSignal2">Another AFRR signal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AFRR_Signal AFRRSignal1,
                                          AFRR_Signal AFRRSignal2)

            => AFRRSignal1.CompareTo(AFRRSignal2) > 0;

        #endregion

        #region Operator >= (AFRRSignal1, AFRRSignal2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AFRRSignal1">An AFRR signal.</param>
        /// <param name="AFRRSignal2">Another AFRR signal.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AFRR_Signal AFRRSignal1,
                                           AFRR_Signal AFRRSignal2)

            => AFRRSignal1.CompareTo(AFRRSignal2) >= 0;

        #endregion

        #endregion

        #region IComparable<AFRRSignal> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two AFRR signals.
        /// </summary>
        /// <param name="Object">An AFRR signal to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AFRR_Signal afrrSignal
                   ? CompareTo(afrrSignal)
                   : throw new ArgumentException("The given object is not an AFRR signal!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AFRRSignal)

        /// <summary>
        /// Compares two AFRR signals.
        /// </summary>
        /// <param name="AFRRSignal">An AFRR signal to compare with.</param>
        public Int32 CompareTo(AFRR_Signal AFRRSignal)

            => Value.CompareTo(AFRRSignal.Value);

        #endregion

        #endregion

        #region IEquatable<AFRRSignal> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AFRR signals for equality.
        /// </summary>
        /// <param name="Object">An AFRR signal to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AFRR_Signal afrrSignal &&
                   Equals(afrrSignal);

        #endregion

        #region Equals(AFRRSignal)

        /// <summary>
        /// Compares two AFRR signals for equality.
        /// </summary>
        /// <param name="AFRRSignal">An AFRR signal to compare with.</param>
        public Boolean Equals(AFRR_Signal AFRRSignal)

            => Value.Equals(AFRRSignal.Value);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a numeric representation of this object.
        /// </summary>
        public override String ToString()

            => Value.ToString();

        #endregion

    }

}
