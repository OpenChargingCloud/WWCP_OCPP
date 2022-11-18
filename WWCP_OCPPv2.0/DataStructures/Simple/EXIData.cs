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
    /// Extention methods for EXI data.
    /// </summary>
    public static class EXIDataExtentions
    {

        /// <summary>
        /// Indicates whether this EXI data is null or empty.
        /// </summary>
        /// <param name="EXIData">A EXI data.</param>
        public static Boolean IsNullOrEmpty(this EXIData? EXIData)
            => !EXIData.HasValue || EXIData.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this EXI data is null or empty.
        /// </summary>
        /// <param name="EXIData">A EXI data.</param>
        public static Boolean IsNotNullOrEmpty(this EXIData? EXIData)
            => EXIData.HasValue && EXIData.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// EXI data.
    /// </summary>
    public readonly struct EXIData : IId,
                                     IEquatable<EXIData>,
                                     IComparable<EXIData>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this EXI data is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this EXI data is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the EXI data.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new EXI data based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of EXI data.</param>
        private EXIData(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as EXI data.
        /// </summary>
        /// <param name="Text">A text representation of EXI data.</param>
        public static EXIData Parse(String Text)
        {

            if (TryParse(Text, out var exiData))
                return exiData;

            throw new ArgumentException("The given text representation of EXI data is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as EXI data.
        /// </summary>
        /// <param name="Text">A text representation of EXI data.</param>
        public static EXIData? TryParse(String Text)
        {

            if (TryParse(Text, out var exiData))
                return exiData;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EXIData)

        /// <summary>
        /// Try to parse the given string as EXI data.
        /// </summary>
        /// <param name="Text">A text representation of EXI data.</param>
        /// <param name="EXIData">The parsed EXI data.</param>
        public static Boolean TryParse(String Text, out EXIData EXIData)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                EXIData = default;
                return false;
            }

            #endregion

            try
            {
                EXIData = new EXIData(Text);
                return true;
            }
            catch (Exception)
            { }

            EXIData = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge box identification.
        /// </summary>
        public EXIData Clone

            => new (
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (EXIData1, EXIData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EXIData1">EXI data.</param>
        /// <param name="EXIData2">Other EXI data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EXIData EXIData1,
                                           EXIData EXIData2)

            => EXIData1.Equals(EXIData2);

        #endregion

        #region Operator != (EXIData1, EXIData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EXIData1">EXI data.</param>
        /// <param name="EXIData2">Other EXI data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EXIData EXIData1,
                                           EXIData EXIData2)

            => !EXIData1.Equals(EXIData2);

        #endregion

        #region Operator <  (EXIData1, EXIData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EXIData1">EXI data.</param>
        /// <param name="EXIData2">Other EXI data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EXIData EXIData1,
                                          EXIData EXIData2)

            => EXIData1.CompareTo(EXIData2) < 0;

        #endregion

        #region Operator <= (EXIData1, EXIData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EXIData1">EXI data.</param>
        /// <param name="EXIData2">Other EXI data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EXIData EXIData1,
                                           EXIData EXIData2)

            => EXIData1.CompareTo(EXIData2) <= 0;

        #endregion

        #region Operator >  (EXIData1, EXIData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EXIData1">EXI data.</param>
        /// <param name="EXIData2">Other EXI data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EXIData EXIData1,
                                          EXIData EXIData2)

            => EXIData1.CompareTo(EXIData2) > 0;

        #endregion

        #region Operator >= (EXIData1, EXIData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EXIData1">EXI data.</param>
        /// <param name="EXIData2">Other EXI data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EXIData EXIData1,
                                           EXIData EXIData2)

            => EXIData1.CompareTo(EXIData2) >= 0;

        #endregion

        #endregion

        #region IComparable<EXIData> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EXI data sets.
        /// </summary>
        /// <param name="Object">EXI data to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EXIData exiData
                   ? CompareTo(exiData)
                   : throw new ArgumentException("The given object is not EXI data!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EXIData)

        /// <summary>
        /// Compares two EXI data sets.
        /// </summary>
        /// <param name="EXIData">EXI data to compare with.</param>
        public Int32 CompareTo(EXIData EXIData)

            => String.Compare(InternalId,
                              EXIData.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<EXIData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EXI data sets for equality.
        /// </summary>
        /// <param name="Object">EXI data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EXIData exiData &&
                   Equals(exiData);

        #endregion

        #region Equals(EXIData)

        /// <summary>
        /// Compares two EXI data sets for equality.
        /// </summary>
        /// <param name="EXIData">EXI data to compare with.</param>
        public Boolean Equals(EXIData EXIData)

            => String.Equals(InternalId,
                             EXIData.InternalId,
                             StringComparison.Ordinal);

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
