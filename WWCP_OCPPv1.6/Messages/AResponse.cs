/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// A generic OCHP response.
    /// </summary>
    public abstract class AResponse<T> : IEquatable<T>

        where T : class

    {

        #region Properties

        /// <summary>
        /// The machine-readable result code.
        /// </summary>
        public Result Result  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic OCHP response.
        /// </summary>
        /// <param name="Result">A generic OCHP result.</param>
        public AResponse(Result Result)
        {
            this.Result  = Result;
        }

        #endregion


        //#region Operator overloading

        //#region Operator == (AResponse1, AResponse2)

        ///// <summary>
        ///// Compares two responses for equality.
        ///// </summary>
        ///// <param name="AResponse1">A response.</param>
        ///// <param name="AResponse2">Another response.</param>
        ///// <returns>True if both match; False otherwise.</returns>
        //public static Boolean operator == (AResponse AResponse1, AResponse AResponse2)
        //{

        //    // If both are null, or both are same instance, return true.
        //    if (Object.ReferenceEquals(AResponse1, AResponse2))
        //        return true;

        //    // If one is null, but not both, return false.
        //    if (((Object) AResponse1 == null) || ((Object) AResponse2 == null))
        //        return false;

        //    return AResponse1.Equals(AResponse2);

        //}

        //#endregion

        //#region Operator != (AResponse1, AResponse2)

        ///// <summary>
        ///// Compares two responses for inequality.
        ///// </summary>
        ///// <param name="AResponse1">A response.</param>
        ///// <param name="AResponse2">Another response.</param>
        ///// <returns>False if both match; True otherwise.</returns>
        //public static Boolean operator != (AResponse AResponse1, AResponse AResponse2)

        //    => !(AResponse1 == AResponse2);

        //#endregion

        //#endregion

        #region IEquatable<AResponse> Members

        public abstract Boolean Equals(T AResponse);

        #endregion

        //#region GetHashCode()

        ///// <summary>
        ///// Return the HashCode of this object.
        ///// </summary>
        ///// <returns>The HashCode of this object.</returns>
        //public override Int32 GetHashCode()
        //{
        //    unchecked
        //    {
        //        return Result.GetHashCode();
        //    }
        //}

        //#endregion

        //#region (override) ToString()

        ///// <summary>
        ///// Return a string representation of this object.
        ///// </summary>
        //public override String ToString()

        //    => Result.ToString();

        //#endregion

    }

}
