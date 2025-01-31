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
    /// Extension methods for data consistency models.
    /// </summary>
    public static class DataConsistencyModelExtensions
    {

        /// <summary>
        /// Indicates whether this data consistency model is null or empty.
        /// </summary>
        /// <param name="DataConsistencyModel">A data consistency model.</param>
        public static Boolean IsNullOrEmpty(this DataConsistencyModel? DataConsistencyModel)
            => !DataConsistencyModel.HasValue || DataConsistencyModel.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this data consistency model is null or empty.
        /// </summary>
        /// <param name="DataConsistencyModel">A data consistency model.</param>
        public static Boolean IsNotNullOrEmpty(this DataConsistencyModel? DataConsistencyModel)
            => DataConsistencyModel.HasValue && DataConsistencyModel.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A data consistency model.
    /// </summary>
    public readonly struct DataConsistencyModel : IId,
                                                  IEquatable<DataConsistencyModel>,
                                                  IComparable<DataConsistencyModel>
    {

        #region Data

        private readonly static Dictionary<String, DataConsistencyModel>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                              InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this data consistency model is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this data consistency model is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the data consistency model.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data consistency model based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a data consistency model.</param>
        private DataConsistencyModel(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static DataConsistencyModel Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new DataConsistencyModel(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a data consistency model.
        /// </summary>
        /// <param name="Text">A text representation of a data consistency model.</param>
        public static DataConsistencyModel Parse(String Text)
        {

            if (TryParse(Text, out var dataConsistencyModel))
                return dataConsistencyModel;

            throw new ArgumentException($"Invalid text representation of a data consistency model: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as data consistency model.
        /// </summary>
        /// <param name="Text">A text representation of a data consistency model.</param>
        public static DataConsistencyModel? TryParse(String Text)
        {

            if (TryParse(Text, out var dataConsistencyModel))
                return dataConsistencyModel;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out DataConsistencyModel)

        /// <summary>
        /// Try to parse the given text as data consistency model.
        /// </summary>
        /// <param name="Text">A text representation of a data consistency model.</param>
        /// <param name="DataConsistencyModel">The parsed data consistency model.</param>
        public static Boolean TryParse(String Text, out DataConsistencyModel DataConsistencyModel)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out DataConsistencyModel))
                    DataConsistencyModel = Register(Text);

                return true;

            }

            DataConsistencyModel = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this data consistency model.
        /// </summary>
        public DataConsistencyModel Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Independent requests.
        /// </summary>
        public static DataConsistencyModel  IndependentRequests    { get; }
            = Register("IndependentRequests");

        /// <summary>
        /// Basically Available, Soft-state, Eventually consistent changes.
        /// </summary>
        public static DataConsistencyModel  BASE                   { get; }
            = Register("BASE");

        /// <summary>
        /// Atomicity, Consistency, Isolation, and Durability guarantees
        /// the reliability and integrity of the changes.
        /// </summary>
        public static DataConsistencyModel ACID                    { get; }
            = Register("ACID");

        #endregion


        #region Operator overloading

        #region Operator == (DataConsistencyModel1, DataConsistencyModel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataConsistencyModel1">A data consistency model.</param>
        /// <param name="DataConsistencyModel2">Another data consistency model.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DataConsistencyModel DataConsistencyModel1,
                                           DataConsistencyModel DataConsistencyModel2)

            => DataConsistencyModel1.Equals(DataConsistencyModel2);

        #endregion

        #region Operator != (DataConsistencyModel1, DataConsistencyModel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataConsistencyModel1">A data consistency model.</param>
        /// <param name="DataConsistencyModel2">Another data consistency model.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DataConsistencyModel DataConsistencyModel1,
                                           DataConsistencyModel DataConsistencyModel2)

            => !DataConsistencyModel1.Equals(DataConsistencyModel2);

        #endregion

        #region Operator <  (DataConsistencyModel1, DataConsistencyModel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataConsistencyModel1">A data consistency model.</param>
        /// <param name="DataConsistencyModel2">Another data consistency model.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DataConsistencyModel DataConsistencyModel1,
                                          DataConsistencyModel DataConsistencyModel2)

            => DataConsistencyModel1.CompareTo(DataConsistencyModel2) < 0;

        #endregion

        #region Operator <= (DataConsistencyModel1, DataConsistencyModel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataConsistencyModel1">A data consistency model.</param>
        /// <param name="DataConsistencyModel2">Another data consistency model.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DataConsistencyModel DataConsistencyModel1,
                                           DataConsistencyModel DataConsistencyModel2)

            => DataConsistencyModel1.CompareTo(DataConsistencyModel2) <= 0;

        #endregion

        #region Operator >  (DataConsistencyModel1, DataConsistencyModel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataConsistencyModel1">A data consistency model.</param>
        /// <param name="DataConsistencyModel2">Another data consistency model.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DataConsistencyModel DataConsistencyModel1,
                                          DataConsistencyModel DataConsistencyModel2)

            => DataConsistencyModel1.CompareTo(DataConsistencyModel2) > 0;

        #endregion

        #region Operator >= (DataConsistencyModel1, DataConsistencyModel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DataConsistencyModel1">A data consistency model.</param>
        /// <param name="DataConsistencyModel2">Another data consistency model.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DataConsistencyModel DataConsistencyModel1,
                                           DataConsistencyModel DataConsistencyModel2)

            => DataConsistencyModel1.CompareTo(DataConsistencyModel2) >= 0;

        #endregion

        #endregion

        #region IComparable<DataConsistencyModel> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two data consistency models.
        /// </summary>
        /// <param name="Object">A data consistency model to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DataConsistencyModel dataConsistencyModel
                   ? CompareTo(dataConsistencyModel)
                   : throw new ArgumentException("The given object is not data consistency model!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DataConsistencyModel)

        /// <summary>
        /// Compares two data consistency models.
        /// </summary>
        /// <param name="DataConsistencyModel">A data consistency model to compare with.</param>
        public Int32 CompareTo(DataConsistencyModel DataConsistencyModel)

            => String.Compare(InternalId,
                              DataConsistencyModel.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<DataConsistencyModel> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two data consistency models for equality.
        /// </summary>
        /// <param name="Object">A data consistency model to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DataConsistencyModel dataConsistencyModel &&
                   Equals(dataConsistencyModel);

        #endregion

        #region Equals(DataConsistencyModel)

        /// <summary>
        /// Compares two data consistency models for equality.
        /// </summary>
        /// <param name="DataConsistencyModel">A data consistency model to compare with.</param>
        public Boolean Equals(DataConsistencyModel DataConsistencyModel)

            => String.Equals(InternalId,
                             DataConsistencyModel.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
