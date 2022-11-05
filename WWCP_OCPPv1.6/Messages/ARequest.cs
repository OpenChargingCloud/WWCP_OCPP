/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// An abstract generic OCPP request message.
    /// </summary>
    public abstract class ARequest<T> : IRequest,
                                        IEquatable<T>

        where T : class

    {

        #region Properties

        /// <summary>
        /// The charge box identification.
        /// </summary>
        [Mandatory]
        public ChargeBox_Id        ChargeBoxId          { get; }

        /// <summary>
        /// The request identification.
        /// </summary>
        [Mandatory]
        public Request_Id          RequestId            { get; }

        /// <summary>
        /// The timestamp of the request message creation.
        /// </summary>
        [Mandatory]
        public DateTime            RequestTimestamp     { get; }

        /// <summary>
        /// The timeout of this request.
        /// </summary>
        [Optional]
        public TimeSpan?           RequestTimeout       { get; }

        /// <summary>
        /// An event tracking identification for correlating this request with other events.
        /// </summary>
        public EventTracking_Id    EventTrackingId      { get; }

        /// <summary>
        /// An optional token to cancel this request.
        /// </summary>
        public CancellationToken?  CancellationToken    { get; }


        /// <summary>
        /// The OCPP SOAP and HTTP Web Socket action.
        /// </summary>
        [Mandatory]
        public String              Action               { get; }

        #endregion

        #region Constructor(s)

        ///// <summary>
        ///// Create a new generic request message.
        ///// </summary>
        //public ARequest()
        //{

        //    this.RequestId         = Request_Id.Parse("0");
        //    this.RequestTimestamp  = Timestamp.Now;
        //    this.EventTrackingId   = EventTracking_Id.New;
        //    this.ChargeBoxId       = ChargeBox_Id.Parse("0");
        //    this.WebSocketAction   = "";

        //}

        /// <summary>
        /// Create a new generic OCPP request message.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Action">The OCPP SOAP and HTTP Web Socket action.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ARequest(ChargeBox_Id        ChargeBoxId,
                        String              Action,

                        Request_Id?         RequestId           = null,
                        DateTime?           RequestTimestamp    = null,
                        TimeSpan?           RequestTimeout      = null,
                        EventTracking_Id?   EventTrackingId     = null,
                        CancellationToken?  CancellationToken   = null)
        {

            this.ChargeBoxId        = ChargeBoxId;
            this.Action             = Action;

            this.RequestId          = RequestId        ?? Request_Id.NewRandom();
            this.RequestTimestamp   = RequestTimestamp ?? Timestamp.Now;
            this.RequestTimeout     = RequestTimeout;

            this.EventTrackingId    = EventTrackingId  ?? EventTracking_Id.New;
            this.CancellationToken  = CancellationToken;

        }

        #endregion


        #region IEquatable<ARequest> Members

        /// <summary>
        /// Compare two requests for equality.
        /// </summary>
        /// <param name="ARequest">Another abstract generic OCPP request.</param>
        public abstract Boolean Equals(T? ARequest);

        #endregion

        public abstract JObject ToJSON();

    }

}
