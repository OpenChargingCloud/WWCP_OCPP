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
    /// Extension methods for report bases.
    /// </summary>
    public static class ReportBaseExtensions
    {

        /// <summary>
        /// Indicates whether this report base is null or empty.
        /// </summary>
        /// <param name="ReportBase">A report base.</param>
        public static Boolean IsNullOrEmpty(this ReportBase? ReportBase)
            => !ReportBase.HasValue || ReportBase.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this report base is null or empty.
        /// </summary>
        /// <param name="ReportBase">A report base.</param>
        public static Boolean IsNotNullOrEmpty(this ReportBase? ReportBase)
            => ReportBase.HasValue && ReportBase.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A report base.
    /// </summary>
    public readonly struct ReportBase : IId,
                                        IEquatable<ReportBase>,
                                        IComparable<ReportBase>
    {

        #region Data

        private readonly static Dictionary<String, ReportBase>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                          InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this report base is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this report base is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the report base.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new report base based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a report base.</param>
        private ReportBase(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ReportBase Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ReportBase(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a report base.
        /// </summary>
        /// <param name="Text">A text representation of a report base.</param>
        public static ReportBase Parse(String Text)
        {

            if (TryParse(Text, out var reportBase))
                return reportBase;

            throw new ArgumentException($"Invalid text representation of a report base: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a report base.
        /// </summary>
        /// <param name="Text">A text representation of a report base.</param>
        public static ReportBase? TryParse(String Text)
        {

            if (TryParse(Text, out var reportBase))
                return reportBase;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ReportBase)

        /// <summary>
        /// Try to parse the given text as a report base.
        /// </summary>
        /// <param name="Text">A text representation of a report base.</param>
        /// <param name="ReportBase">The parsed report base.</param>
        public static Boolean TryParse(String Text, out ReportBase ReportBase)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ReportBase))
                    ReportBase = Register(Text);

                return true;

            }

            ReportBase = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this report base.
        /// </summary>
        public ReportBase Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// A (configuration) report that lists all components/variables that can be set
        /// by the operator.
        /// </summary>
        public static ReportBase ConfigurationInventory    { get; }
            = Register("ConfigurationInventory");

        /// <summary>
        /// A (full) report that lists everything except monitoring settings.
        /// </summary>
        public static ReportBase FullInventory             { get; }
            = Register("FullInventory");

        /// <summary>
        /// A (summary) report that lists components/variables relating to the charging station’s
        /// current charging availability, and to any existing problem conditions.
        /// </summary>
        public static ReportBase SummaryInventory          { get; }
            = Register("SummaryInventory");

        #endregion


        #region IComparable<ReportBase> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two report bases.
        /// </summary>
        /// <param name="Object">A report base to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ReportBase reportBase
                   ? CompareTo(reportBase)
                   : throw new ArgumentException("The given object is not a report base!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReportBase)

        /// <summary>
        /// Compares two report bases.
        /// </summary>
        /// <param name="ReportBase">A report base to compare with.</param>
        public Int32 CompareTo(ReportBase ReportBase)

            => String.Compare(InternalId,
                              ReportBase.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ReportBase> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two report bases for equality.
        /// </summary>
        /// <param name="Object">A report base to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReportBase reportBase &&
                   Equals(reportBase);

        #endregion

        #region Equals(ReportBase)

        /// <summary>
        /// Compares two report bases for equality.
        /// </summary>
        /// <param name="ReportBase">A report base to compare with.</param>
        public Boolean Equals(ReportBase ReportBase)

            => String.Equals(InternalId,
                             ReportBase.InternalId,
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
