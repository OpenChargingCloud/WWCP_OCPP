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
    /// Extention methods for request identifications.
    /// </summary>
    public static class RequestIdExtensions
    {

        /// <summary>
        /// Indicates whether this request identification is null or empty.
        /// </summary>
        /// <param name="RequestId">A request identification.</param>
        public static Boolean IsNullOrEmpty(this Request_Id? RequestId)
            => !RequestId.HasValue || RequestId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this request identification is null or empty.
        /// </summary>
        /// <param name="RequestId">A request identification.</param>
        public static Boolean IsNotNullOrEmpty(this Request_Id? RequestId)
            => RequestId.HasValue && RequestId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A request identification.
    /// </summary>
    public readonly struct Request_Id : IId,
                                        IEquatable<Request_Id>,
                                        IComparable<Request_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the request identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new request identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a request identification.</param>
        private Request_Id(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        // Maximum of 36 characters, to allow for GUIDs
        // Sometimes called: messageId?!?!

        #region (static) NewRandom(Length = 30, IsLocal = false)

        /// <summary>
        /// Create a new random request identification.
        /// </summary>
        /// <param name="Length">The expected length of the request identification.</param>
        /// <param name="IsLocal">The request identification was generated locally and not received via network.</param>
        public static Request_Id NewRandom(Byte      Length    = 30,
                                           Boolean?  IsLocal   = false)

            => new ((IsLocal == true ? "Local:" : "") +
                    RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a request identification.
        /// </summary>
        /// <param name="Text">A text representation of a request identification.</param>
        public static Request_Id Parse(String Text)
        {

            if (TryParse(Text, out var requestId))
                return requestId;

            throw new ArgumentException("Invalid text representation of a request identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a request identification.
        /// </summary>
        /// <param name="Text">A text representation of a request identification.</param>
        public static Request_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var requestId))
                return requestId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out RequestId)

        /// <summary>
        /// Try to parse the given text as a request identification.
        /// </summary>
        /// <param name="Text">A text representation of a request identification.</param>
        /// <param name="RequestId">The parsed request identification.</param>
        public static Boolean TryParse(String Text, out Request_Id RequestId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                RequestId = new Request_Id(Text);
                return true;
            }

            RequestId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this request identification.
        /// </summary>
        public Request_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A request identification.</param>
        /// <param name="RequestId2">Another request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => RequestId1.Equals(RequestId2);

        #endregion

        #region Operator != (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A request identification.</param>
        /// <param name="RequestId2">Another request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => !RequestId1.Equals(RequestId2);

        #endregion

        #region Operator <  (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A request identification.</param>
        /// <param name="RequestId2">Another request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Request_Id RequestId1,
                                          Request_Id RequestId2)

            => RequestId1.CompareTo(RequestId2) < 0;

        #endregion

        #region Operator <= (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A request identification.</param>
        /// <param name="RequestId2">Another request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => RequestId1.CompareTo(RequestId2) <= 0;

        #endregion

        #region Operator >  (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A request identification.</param>
        /// <param name="RequestId2">Another request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Request_Id RequestId1,
                                          Request_Id RequestId2)

            => RequestId1.CompareTo(RequestId2) > 0;

        #endregion

        #region Operator >= (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A request identification.</param>
        /// <param name="RequestId2">Another request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => RequestId1.CompareTo(RequestId2) >= 0;

        #endregion

        #endregion

        #region IComparable<RequestId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two request identifications.
        /// </summary>
        /// <param name="Object">A request identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Request_Id requestId
                   ? CompareTo(requestId)
                   : throw new ArgumentException("The given object is not a request identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RequestId)

        /// <summary>
        /// Compares two request identifications.
        /// </summary>
        /// <param name="RequestId">A request identification to compare with.</param>
        public Int32 CompareTo(Request_Id RequestId)

            => String.Compare(InternalId,
                              RequestId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<RequestId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two request identifications for equality.
        /// </summary>
        /// <param name="Object">A request identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Request_Id requestId &&
                   Equals(requestId);

        #endregion

        #region Equals(RequestId)

        /// <summary>
        /// Compares two request identifications for equality.
        /// </summary>
        /// <param name="RequestId">A request identification to compare with.</param>
        public Boolean Equals(Request_Id RequestId)

            => String.Equals(InternalId,
                             RequestId.InternalId,
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
