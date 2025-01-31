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
    /// Extension methods for RequestStartStop status.
    /// </summary>
    public static class RequestStartStopStatusExtensions
    {

        /// <summary>
        /// Indicates whether this RequestStartStop status is null or empty.
        /// </summary>
        /// <param name="RequestStartStopStatus">A RequestStartStop status.</param>
        public static Boolean IsNullOrEmpty(this RequestStartStopStatus? RequestStartStopStatus)
            => !RequestStartStopStatus.HasValue || RequestStartStopStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this RequestStartStop status is null or empty.
        /// </summary>
        /// <param name="RequestStartStopStatus">A RequestStartStop status.</param>
        public static Boolean IsNotNullOrEmpty(this RequestStartStopStatus? RequestStartStopStatus)
            => RequestStartStopStatus.HasValue && RequestStartStopStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A RequestStartStop status.
    /// </summary>
    public readonly struct RequestStartStopStatus : IId,
                                                    IEquatable<RequestStartStopStatus>,
                                                    IComparable<RequestStartStopStatus>
    {

        #region Data

        private readonly static Dictionary<String, RequestStartStopStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this RequestStartStop status is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this RequestStartStop status is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the RequestStartStop status.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RequestStartStop status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a RequestStartStop status.</param>
        private RequestStartStopStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static RequestStartStopStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new RequestStartStopStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a RequestStartStop status.
        /// </summary>
        /// <param name="Text">A text representation of a RequestStartStop status.</param>
        public static RequestStartStopStatus Parse(String Text)
        {

            if (TryParse(Text, out var requestStartStopStatus))
                return requestStartStopStatus;

            throw new ArgumentException($"Invalid text representation of a RequestStartStop status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a RequestStartStop status.
        /// </summary>
        /// <param name="Text">A text representation of a RequestStartStop status.</param>
        public static RequestStartStopStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var requestStartStopStatus))
                return requestStartStopStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out RequestStartStopStatus)

        /// <summary>
        /// Try to parse the given text as a RequestStartStop status.
        /// </summary>
        /// <param name="Text">A text representation of a RequestStartStop status.</param>
        /// <param name="RequestStartStopStatus">The parsed RequestStartStop status.</param>
        public static Boolean TryParse(String Text, out RequestStartStopStatus RequestStartStopStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out RequestStartStopStatus))
                    RequestStartStopStatus = Register(Text);

                return true;

            }

            RequestStartStopStatus = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this RequestStartStop status.
        /// </summary>
        public RequestStartStopStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        public static class Const {
            public const String Accepted  = "Accepted";
            public const String Rejected  = "Rejected";
        }

        /// <summary>
        /// Command will be executed.
        /// </summary>
        public static RequestStartStopStatus  Accepted    { get; }
            = Register(Const.Accepted);

        /// <summary>
        /// Command will not be executed.
        /// </summary>
        public static RequestStartStopStatus  Rejected    { get; }
            = Register(Const.Rejected);

        #endregion


        #region Operator overloading

        #region Operator == (RequestStartStopStatus1, RequestStartStopStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStartStopStatus1">A RequestStartStop status.</param>
        /// <param name="RequestStartStopStatus2">Another RequestStartStop status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RequestStartStopStatus RequestStartStopStatus1,
                                           RequestStartStopStatus RequestStartStopStatus2)

            => RequestStartStopStatus1.Equals(RequestStartStopStatus2);

        #endregion

        #region Operator != (RequestStartStopStatus1, RequestStartStopStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStartStopStatus1">A RequestStartStop status.</param>
        /// <param name="RequestStartStopStatus2">Another RequestStartStop status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RequestStartStopStatus RequestStartStopStatus1,
                                           RequestStartStopStatus RequestStartStopStatus2)

            => !RequestStartStopStatus1.Equals(RequestStartStopStatus2);

        #endregion

        #region Operator <  (RequestStartStopStatus1, RequestStartStopStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStartStopStatus1">A RequestStartStop status.</param>
        /// <param name="RequestStartStopStatus2">Another RequestStartStop status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RequestStartStopStatus RequestStartStopStatus1,
                                          RequestStartStopStatus RequestStartStopStatus2)

            => RequestStartStopStatus1.CompareTo(RequestStartStopStatus2) < 0;

        #endregion

        #region Operator <= (RequestStartStopStatus1, RequestStartStopStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStartStopStatus1">A RequestStartStop status.</param>
        /// <param name="RequestStartStopStatus2">Another RequestStartStop status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RequestStartStopStatus RequestStartStopStatus1,
                                           RequestStartStopStatus RequestStartStopStatus2)

            => RequestStartStopStatus1.CompareTo(RequestStartStopStatus2) <= 0;

        #endregion

        #region Operator >  (RequestStartStopStatus1, RequestStartStopStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStartStopStatus1">A RequestStartStop status.</param>
        /// <param name="RequestStartStopStatus2">Another RequestStartStop status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RequestStartStopStatus RequestStartStopStatus1,
                                          RequestStartStopStatus RequestStartStopStatus2)

            => RequestStartStopStatus1.CompareTo(RequestStartStopStatus2) > 0;

        #endregion

        #region Operator >= (RequestStartStopStatus1, RequestStartStopStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestStartStopStatus1">A RequestStartStop status.</param>
        /// <param name="RequestStartStopStatus2">Another RequestStartStop status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RequestStartStopStatus RequestStartStopStatus1,
                                           RequestStartStopStatus RequestStartStopStatus2)

            => RequestStartStopStatus1.CompareTo(RequestStartStopStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<RequestStartStopStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two RequestStartStop status.
        /// </summary>
        /// <param name="Object">A RequestStartStop status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RequestStartStopStatus RequestStartStopStatus
                   ? CompareTo(RequestStartStopStatus)
                   : throw new ArgumentException("The given object is not a RequestStartStop status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RequestStartStopStatus)

        /// <summary>
        /// Compares two RequestStartStop status.
        /// </summary>
        /// <param name="RequestStartStopStatus">A RequestStartStop status to compare with.</param>
        public Int32 CompareTo(RequestStartStopStatus RequestStartStopStatus)

            => String.Compare(InternalId,
                              RequestStartStopStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<RequestStartStopStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RequestStartStop status for equality.
        /// </summary>
        /// <param name="Object">A RequestStartStop status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestStartStopStatus RequestStartStopStatus &&
                   Equals(RequestStartStopStatus);

        #endregion

        #region Equals(RequestStartStopStatus)

        /// <summary>
        /// Compares two RequestStartStop status for equality.
        /// </summary>
        /// <param name="RequestStartStopStatus">A RequestStartStop status to compare with.</param>
        public Boolean Equals(RequestStartStopStatus RequestStartStopStatus)

            => String.Equals(InternalId,
                             RequestStartStopStatus.InternalId,
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
