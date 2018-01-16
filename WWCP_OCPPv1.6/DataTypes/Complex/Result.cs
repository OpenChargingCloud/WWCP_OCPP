/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// A generic OCPP result.
    /// </summary>
    public class Result : IEquatable<Result>
    {

        #region Properties

        /// <summary>
        /// The machine-readable result code.
        /// </summary>
        public ResultCodes  ResultCode     { get; }

        /// <summary>
        /// A human-readable error description.
        /// </summary>
        public String       Description    { get; }

        #endregion

        #region Statics

        /// <summary>
        /// Unknown result code.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result Unknown(String Description = null)

            => new Result(ResultCodes.Unknown,
                          Description);


        /// <summary>
        /// Data accepted and processed.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result OK(String Description = null)

            => new Result(ResultCodes.Unknown,
                          Description);


        /// <summary>
        /// Only part of the data was accepted.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result Partly(String Description = null)

            => new Result(ResultCodes.Unknown,
                          Description);


        /// <summary>
        /// Wrong username and/or password.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result NotAuthorized(String Description = null)

            => new Result(ResultCodes.Unknown,
                          Description);


        /// <summary>
        /// One or more ID (EVSE/Contract) were not valid for this user.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result InvalidId(String Description = null)

            => new Result(ResultCodes.Unknown,
                          Description);


        /// <summary>
        /// Internal server error.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result Server(String Description = null)

            => new Result(ResultCodes.Unknown,
                          Description);


        /// <summary>
        /// Data has technical errors.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result Format(String Description = null)

            => new Result(ResultCodes.Unknown,
                          Description);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic OCPP result.
        /// </summary>
        /// <param name="ResultCode">The machine-readable result code.</param>
        /// <param name="Description">A human-readable error description.</param>
        public Result(ResultCodes  ResultCode,
                      String       Description = null)
        {

            this.ResultCode   = ResultCode;

            this.Description  = Description.IsNotNullOrEmpty()
                                    ? Description.Trim()
                                    : "";

        }

        #endregion


        #region Operator overloading

        #region Operator == (Result1, Result2)

        /// <summary>
        /// Compares two results for equality.
        /// </summary>
        /// <param name="Result1">A result.</param>
        /// <param name="Result2">Another result.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Result Result1, Result Result2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Result1, Result2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Result1 == null) || ((Object) Result2 == null))
                return false;

            return Result1.Equals(Result2);

        }

        #endregion

        #region Operator != (Result1, Result2)

        /// <summary>
        /// Compares two results for inequality.
        /// </summary>
        /// <param name="Result1">A result.</param>
        /// <param name="Result2">Another result.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Result Result1, Result Result2)

            => !(Result1 == Result2);

        #endregion

        #endregion

        #region IEquatable<Result> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a result.
            var Result = Object as Result;
            if ((Object) Result == null)
                return false;

            return this.Equals(Result);

        }

        #endregion

        #region Equals(Result)

        /// <summary>
        /// Compares two results for equality.
        /// </summary>
        /// <param name="Result">An result to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Result Result)
        {

            if ((Object) Result == null)
                return false;

            return this.ResultCode. Equals(Result.ResultCode) &&
                   this.Description.Equals(Result.Description);

        }

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

                return ResultCode. GetHashCode() * 11 ^

                       (Description != null
                            ? Description.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ResultCode + (Description.IsNotNullOrEmpty() ? " - " + Description : "");

        #endregion

    }

}
