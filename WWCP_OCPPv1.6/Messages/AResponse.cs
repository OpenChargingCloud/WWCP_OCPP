///*
// * Copyright (c) 2014-2023 GraphDefined GmbH
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using org.GraphDefined.Vanaheimr.Illias;

//using cloud.charging.open.protocols.OCPP;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6
//{

//    /// <summary>
//    /// An abstract generic response.
//    /// </summary>
//    public abstract class AResponse<TRequest, TResponse> : AResponse<TResponse>

//        where TRequest  : class, IRequest
//        where TResponse : class, IResponse

//    {

//        #region Properties

//        /// <summary>
//        /// The request leading to this response.
//        /// </summary>
//        public TRequest  Request    { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new abstract generic response.
//        /// </summary>
//        /// <param name="Request">The request leading to this result.</param>
//        /// <param name="Result">A generic result.</param>
//        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
//        public AResponse(TRequest   Request,
//                         Result     Result,
//                         DateTime?  ResponseTimestamp   = null)

//            : this(Request,
//                   Result,
//                   ResponseTimestamp ?? Timestamp.Now,
//                   null)

//        { }


//        /// <summary>
//        /// Create a new abstract generic response.
//        /// </summary>
//        /// <param name="Request">The request leading to this result.</param>
//        /// <param name="Result">A generic result.</param>
//        /// <param name="ResponseTimestamp">The response timestamp.</param>
//        public AResponse(TRequest   Request,
//                         Result     Result,
//                         DateTime   ResponseTimestamp,
//                         TimeSpan?  Runtime   = null)

//            : base(Result,
//                   ResponseTimestamp,
//                   Runtime ?? ResponseTimestamp - Request.RequestTimestamp)

//        {

//            this.Request = Request;

//            unchecked
//            {
//                hashCode = this.Request.GetHashCode() * 3 ^
//                           base.        GetHashCode();
//            }

//        }

//        #endregion


//        #region IEquatable<AResponse> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two abstract generic responses for equality.
//        /// </summary>
//        /// <param name="Object">An abstract generic response to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is AResponse<TRequest, TResponse> aResponse &&
//                   Equals(aResponse);

//        #endregion

//        #region Equals(AResponse)

//        /// <summary>
//        /// Compares two abstract generic responses for equality.
//        /// </summary>
//        /// <param name="AResponse">An abstract generic response to compare with.</param>
//        public Boolean Equals(AResponse<TRequest, TResponse>? AResponse)

//            => AResponse is not null &&

//               Request.Equals(AResponse.Request) &&
//               Runtime.Equals(AResponse.Runtime) &&

//               base.   Equals(AResponse);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        private readonly Int32 hashCode;

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => hashCode;

//        #endregion

//    }


//    /// <summary>
//    /// An abstract generic response.
//    /// </summary>
//    public abstract class AResponse<TResponse> : IResponse,
//                                                 IEquatable<TResponse>

//        where TResponse : class, IResponse

//    {

//        #region Properties

//        /// <summary>
//        /// The machine-readable result code.
//        /// </summary>
//        [Mandatory]
//        public Result    Result               { get; }

//        /// <summary>
//        /// The timestamp of the response.
//        /// </summary>
//        [Mandatory]
//        public DateTime  ResponseTimestamp    { get; }

//        /// <summary>
//        /// The runtime of the request.
//        /// </summary>
//        [Mandatory]
//        public TimeSpan  Runtime              { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new abstract generic response.
//        /// </summary>
//        /// <param name="Result">A generic result.</param>
//        /// <param name="ResponseTimestamp">The timestamp of the response.</param>
//        /// <param name="Runtime">The runtime of the request.</param>
//        public AResponse(Result    Result,
//                         DateTime  ResponseTimestamp,
//                         TimeSpan  Runtime)
//        {

//            this.Result             = Result;
//            this.ResponseTimestamp  = ResponseTimestamp;
//            this.Runtime            = Runtime;

//            unchecked
//            {

//                hashCode = this.Result.           GetHashCode() * 5 ^
//                           this.ResponseTimestamp.GetHashCode() * 3 ^
//                           this.Runtime.          GetHashCode();

//            }

//        }

//        #endregion


//        #region Operator overloading

//        #region Operator == (AResponse1, AResponse2)

//        /// <summary>
//        /// Compares two responses for equality.
//        /// </summary>
//        /// <param name="AResponse1">A response.</param>
//        /// <param name="AResponse2">Another response.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public static Boolean operator == (AResponse<TResponse>? AResponse1,
//                                           AResponse<TResponse>? AResponse2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(AResponse1, AResponse2))
//                return true;

//            // If one is null, but not both, return false.
//            if (AResponse1 is null || AResponse2 is null)
//                return false;

//            return AResponse1.Equals(AResponse2);

//        }

//        #endregion

//        #region Operator != (AResponse1, AResponse2)

//        /// <summary>
//        /// Compares two responses for inequality.
//        /// </summary>
//        /// <param name="AResponse1">A response.</param>
//        /// <param name="AResponse2">Another response.</param>
//        /// <returns>False if both match; True otherwise.</returns>
//        public static Boolean operator != (AResponse<TResponse>? AResponse1,
//                                           AResponse<TResponse>? AResponse2)

//            => !(AResponse1 == AResponse2);

//        #endregion

//        #endregion

//        #region IEquatable<AResponse> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two abstract generic responses for equality.
//        /// </summary>
//        /// <param name="Object">An abstract generic response to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is AResponse<TResponse> aResponse &&
//                   Equals(aResponse);

//        #endregion

//        #region Equals(AResponse)

//        /// <summary>
//        /// Compares two abstract generic responses for equality.
//        /// </summary>
//        /// <param name="AResponse">An abstract generic response to compare with.</param>
//        public virtual Boolean Equals(AResponse<TResponse>? AResponse)

//            => AResponse is not null &&

//               Result.           Equals(AResponse.Result) &&
//               ResponseTimestamp.Equals(AResponse.ResponseTimestamp);

//        #endregion

//        #region IEquatable<AResponse> Members

//        /// <summary>
//        /// Compares two abstract generic responses for equality.
//        /// </summary>
//        /// <param name="AResponse">An abstract generic response to compare with.</param>
//        public abstract Boolean Equals(TResponse? AResponse);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        private readonly Int32 hashCode;

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => hashCode;

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => Result.ToString();

//        #endregion

//    }

//}
