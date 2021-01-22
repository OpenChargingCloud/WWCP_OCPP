/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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
using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// An abstract generic request message.
    /// </summary>
    public abstract class ARequest<T> : IRequest,
                                        IEquatable<T>

        where T : class

    {

        #region Properties

        /// <summary>
        /// The request identification.
        /// </summary>
        public Request_Id    RequestId           { get; }

        /// <summary>
        /// The timestamp of the request message creation.
        /// </summary>
        [Mandatory]
        public DateTime      RequestTimestamp    { get; }


        /// <summary>
        /// Charge box identification.
        /// </summary>
        [Mandatory]
        public ChargeBox_Id  ChargeBoxId         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic request message.
        /// </summary>
        public ARequest()
        {

            this.RequestId         = Request_Id.Parse("0");
            this.RequestTimestamp  = DateTime.UtcNow;
            this.ChargeBoxId       = ChargeBox_Id.Parse("0");

        }

        /// <summary>
        /// Create a new generic request message.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public ARequest(ChargeBox_Id  ChargeBoxId,
                        Request_Id?   RequestId          = null,
                        DateTime?     RequestTimestamp   = null)
        {

            this.ChargeBoxId       = ChargeBoxId;
            this.RequestId         = RequestId        ?? Request_Id.Random();
            this.RequestTimestamp  = RequestTimestamp ?? DateTime.UtcNow;

        }

        #endregion


        #region IEquatable<ARequest> Members

        /// <summary>
        /// Compare two requests for equality.
        /// </summary>
        /// <param name="ARequest">Another abstract generic OCPP request.</param>
        public abstract Boolean Equals(T ARequest);

        #endregion

    }

}
