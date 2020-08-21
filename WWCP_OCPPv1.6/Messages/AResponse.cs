/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;

#endregion

namespace cloud.charging.adapters.OCPPv1_6
{

    /// <summary>
    /// An abstract generic response.
    /// </summary>
    public abstract class AResponse<TRequest, TResponse> : AResponse<TResponse>

        where TRequest  : class, IRequest
        where TResponse : class, IResponse

    {

        #region Properties

        /// <summary>
        /// The request leading to this response.
        /// </summary>
        public TRequest  Request    { get; }

        /// <summary>
        /// The runtime of the request.
        /// </summary>
        public TimeSpan  Runtime    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic response.
        /// </summary>
        /// <param name="Request">The request leading to this result.</param>
        /// <param name="Result">A generic result.</param>
        public AResponse(TRequest  Request,
                         Result    Result)

            : base(Result)

        {

            this.Request  = Request;
            this.Runtime  = ResponseTimestamp - Request.RequestTimestamp;

        }

        #endregion

    }


    /// <summary>
    /// An abstract generic response.
    /// </summary>
    public abstract class AResponse<TResponse> : IResponse,
                                                 IEquatable<TResponse>

        where TResponse : class, IResponse

    {

        #region Properties

        /// <summary>
        /// The machine-readable result code.
        /// </summary>
        public Result    Result              { get; }

        /// <summary>
        /// The timestamp of the response message creation.
        /// </summary>
        public DateTime  ResponseTimestamp   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic response.
        /// </summary>
        /// <param name="Result">A generic result.</param>
        public AResponse(Result  Result)
        {

            this.Result             = Result;
            this.ResponseTimestamp  = DateTime.UtcNow;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AResponse1, AResponse2)

        /// <summary>
        /// Compares two responses for equality.
        /// </summary>
        /// <param name="AResponse1">A response.</param>
        /// <param name="AResponse2">Another response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AResponse<TResponse> AResponse1, AResponse<TResponse> AResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AResponse1, AResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((AResponse1 is null) || (AResponse2 is null))
                return false;

            return AResponse1.Equals(AResponse2);

        }

        #endregion

        #region Operator != (AResponse1, AResponse2)

        /// <summary>
        /// Compares two responses for inequality.
        /// </summary>
        /// <param name="AResponse1">A response.</param>
        /// <param name="AResponse2">Another response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AResponse<TResponse> AResponse1, AResponse<TResponse> AResponse2)

            => !(AResponse1 == AResponse2);

        #endregion

        #endregion

        #region IEquatable<AResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is AResponse<TResponse> AResponse))
                return false;

            return Equals(AResponse);

        }

        #endregion

        #region Equals(AResponse)

        /// <summary>
        /// Compares two responses for equality.
        /// </summary>
        /// <param name="AResponse">A response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AResponse<TResponse> AResponse)
        {

            if (AResponse is null)
                return false;

            return Result.Equals(AResponse.Result);

        }

        #endregion

        #region IEquatable<AResponse> Members

        /// <summary>
        /// Compare two responses for equality.
        /// </summary>
        /// <param name="AResponse">Another abstract generic response.</param>
        public abstract Boolean Equals(TResponse AResponse);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return Result.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Result.ToString();

        #endregion

    }

}
