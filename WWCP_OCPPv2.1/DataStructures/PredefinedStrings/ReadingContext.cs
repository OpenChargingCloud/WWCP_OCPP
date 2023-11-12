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
    /// Extension methods for reading contexts.
    /// </summary>
    public static class ReadingContextExtensions
    {

        /// <summary>
        /// Indicates whether this reading context is null or empty.
        /// </summary>
        /// <param name="ReadingContext">A reading context.</param>
        public static Boolean IsNullOrEmpty(this ReadingContext? ReadingContext)
            => !ReadingContext.HasValue || ReadingContext.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this reading context is null or empty.
        /// </summary>
        /// <param name="ReadingContext">A reading context.</param>
        public static Boolean IsNotNullOrEmpty(this ReadingContext? ReadingContext)
            => ReadingContext.HasValue && ReadingContext.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A reading context.
    /// </summary>
    public readonly struct ReadingContext : IId,
                                            IEquatable<ReadingContext>,
                                            IComparable<ReadingContext>
    {

        #region Data

        private readonly static Dictionary<String, ReadingContext>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                              InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this reading context is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this reading context is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the reading context.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reading context based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a reading context.</param>
        private ReadingContext(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ReadingContext Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ReadingContext(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a reading context.
        /// </summary>
        /// <param name="Text">A text representation of a reading context.</param>
        public static ReadingContext Parse(String Text)
        {

            if (TryParse(Text, out var readingContext))
                return readingContext;

            throw new ArgumentException($"Invalid text representation of a reading context: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reading context.
        /// </summary>
        /// <param name="Text">A text representation of a reading context.</param>
        public static ReadingContext? TryParse(String Text)
        {

            if (TryParse(Text, out var readingContext))
                return readingContext;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ReadingContext)

        /// <summary>
        /// Try to parse the given text as a reading context.
        /// </summary>
        /// <param name="Text">A text representation of a reading context.</param>
        /// <param name="ReadingContext">The parsed reading context.</param>
        public static Boolean TryParse(String Text, out ReadingContext ReadingContext)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ReadingContext))
                    ReadingContext = Register(Text);

                return true;

            }

            ReadingContext = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this reading context.
        /// </summary>
        public ReadingContext Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Value taken at start of interruption.
        /// </summary>
        public static ReadingContext InterruptionBegin    { get; }
            = Register("InterruptionBegin");

        /// <summary>
        /// Value taken when resuming after interruption.
        /// </summary>
        public static ReadingContext InterruptionEnd      { get; }
            = Register("InterruptionEnd");

        /// <summary>
        /// Value for any other situations.
        /// </summary>
        public static ReadingContext Other                { get; }
            = Register("Other");

        /// <summary>
        /// Value taken at clock aligned interval.
        /// </summary>
        public static ReadingContext SampleClock          { get; }
            = Register("SampleClock");

        /// <summary>
        /// Value taken as periodic sample relative to start time of transaction.
        /// </summary>
        public static ReadingContext SamplePeriodic       { get; }
            = Register("SamplePeriodic");

        /// <summary>
        /// Value taken at the beginning of a transaction.
        /// </summary>
        public static ReadingContext TransactionBegin     { get; }
            = Register("TransactionBegin");

        /// <summary>
        /// Value taken at the end of a transaction.
        /// </summary>
        public static ReadingContext TransactionEnd       { get; }
            = Register("TransactionEnd");

        /// <summary>
        /// Value taken in response to a TriggerMessage request.
        /// </summary>
        public static ReadingContext Trigger              { get; }
            = Register("Trigger");

        #endregion


        #region Operator overloading

        #region Operator == (ReadingContext1, ReadingContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReadingContext1">A reading context.</param>
        /// <param name="ReadingContext2">Another reading context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReadingContext ReadingContext1,
                                           ReadingContext ReadingContext2)

            => ReadingContext1.Equals(ReadingContext2);

        #endregion

        #region Operator != (ReadingContext1, ReadingContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReadingContext1">A reading context.</param>
        /// <param name="ReadingContext2">Another reading context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReadingContext ReadingContext1,
                                           ReadingContext ReadingContext2)

            => !ReadingContext1.Equals(ReadingContext2);

        #endregion

        #region Operator <  (ReadingContext1, ReadingContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReadingContext1">A reading context.</param>
        /// <param name="ReadingContext2">Another reading context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReadingContext ReadingContext1,
                                          ReadingContext ReadingContext2)

            => ReadingContext1.CompareTo(ReadingContext2) < 0;

        #endregion

        #region Operator <= (ReadingContext1, ReadingContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReadingContext1">A reading context.</param>
        /// <param name="ReadingContext2">Another reading context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ReadingContext ReadingContext1,
                                           ReadingContext ReadingContext2)

            => ReadingContext1.CompareTo(ReadingContext2) <= 0;

        #endregion

        #region Operator >  (ReadingContext1, ReadingContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReadingContext1">A reading context.</param>
        /// <param name="ReadingContext2">Another reading context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReadingContext ReadingContext1,
                                          ReadingContext ReadingContext2)

            => ReadingContext1.CompareTo(ReadingContext2) > 0;

        #endregion

        #region Operator >= (ReadingContext1, ReadingContext2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReadingContext1">A reading context.</param>
        /// <param name="ReadingContext2">Another reading context.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ReadingContext ReadingContext1,
                                           ReadingContext ReadingContext2)

            => ReadingContext1.CompareTo(ReadingContext2) >= 0;

        #endregion

        #endregion

        #region IComparable<ReadingContext> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two reading contexts.
        /// </summary>
        /// <param name="Object">A reading context to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ReadingContext readingContext
                   ? CompareTo(readingContext)
                   : throw new ArgumentException("The given object is not a reading context!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReadingContext)

        /// <summary>
        /// Compares two reading contexts.
        /// </summary>
        /// <param name="ReadingContext">A reading context to compare with.</param>
        public Int32 CompareTo(ReadingContext ReadingContext)

            => String.Compare(InternalId,
                              ReadingContext.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ReadingContext> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reading contexts for equality.
        /// </summary>
        /// <param name="Object">A reading context to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReadingContext readingContext &&
                   Equals(readingContext);

        #endregion

        #region Equals(ReadingContext)

        /// <summary>
        /// Compares two reading contexts for equality.
        /// </summary>
        /// <param name="ReadingContext">A reading context to compare with.</param>
        public Boolean Equals(ReadingContext ReadingContext)

            => String.Equals(InternalId,
                             ReadingContext.InternalId,
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
