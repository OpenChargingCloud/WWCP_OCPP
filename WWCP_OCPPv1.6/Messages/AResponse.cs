﻿/*
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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An abstract generic OCPP response.
    /// </summary>
    public abstract class AResponse<T> : IEquatable<T>

        where T : class

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
        /// Create a new generic OCHP response.
        /// </summary>
        /// <param name="Result">A generic OCHP result.</param>
        public AResponse(Result Result)
        {

            this.Result             = Result;
            this.ResponseTimestamp  = DateTime.Now;

        }

        #endregion


        #region IEquatable<AResponse> Members

        /// <summary>
        /// Compare two responses for equality.
        /// </summary>
        /// <param name="AResponse">Another abstract generic OCPP response.</param>
        public abstract Boolean Equals(T AResponse);

        #endregion

    }

}
