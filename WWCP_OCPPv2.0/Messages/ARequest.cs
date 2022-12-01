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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// An abstract generic OCPP request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the OCPP request.</typeparam>
    public abstract class ARequest<TRequest> : IRequest,
                                               IEquatable<TRequest>

        where TRequest : class, IRequest

    {

        #region Data

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(30);

        #endregion

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
        [Mandatory]
        public TimeSpan            RequestTimeout       { get; }

        /// <summary>
        /// An event tracking identification for correlating this request with other events.
        /// </summary>
        [Mandatory]
        public EventTracking_Id    EventTrackingId      { get; }

        /// <summary>
        /// The OCPP HTTP Web Socket action.
        /// </summary>
        [Mandatory]
        public String              Action               { get; }

        /// <summary>
        /// The custom data object to allow to store any kind of customer specific data.
        /// </summary>
        [Optional]
        public CustomData?         CustomData           { get; }

        /// <summary>
        /// An optional token to cancel this request.
        /// </summary>
        public CancellationToken?  CancellationToken    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic OCPP request message.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Action">The OCPP HTTP Web Socket action.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ARequest(ChargeBox_Id        ChargeBoxId,
                        String              Action,

                        CustomData?         CustomData          = null,

                        Request_Id?         RequestId           = null,
                        DateTime?           RequestTimestamp    = null,
                        TimeSpan?           RequestTimeout      = null,
                        EventTracking_Id?   EventTrackingId     = null,
                        CancellationToken?  CancellationToken   = null)
        {

            this.ChargeBoxId        = ChargeBoxId;
            this.Action             = Action;

            this.CustomData         = CustomData;

            this.RequestId          = RequestId        ?? Request_Id.NewRandom();
            this.RequestTimestamp   = RequestTimestamp ?? Timestamp.Now;
            this.RequestTimeout     = RequestTimeout   ?? DefaultRequestTimeout;
            this.EventTrackingId    = EventTrackingId  ?? EventTracking_Id.New;
            this.CancellationToken  = CancellationToken;

        }

        #endregion


        #region IEquatable<TRequest> Members

        /// <summary>
        /// Compare two abstract generic OCPP requests for equality.
        /// </summary>
        /// <param name="TRequest">Another abstract generic OCPP request.</param>
        public abstract Boolean Equals(TRequest? TRequest);

        #endregion

        #region GenericEquals(ARequest)

        /// <summary>
        /// Compare two abstract generic OCPP requests for equality.
        /// </summary>
        /// <param name="ARequest">Another abstract generic OCPP request.</param>
        public Boolean GenericEquals(ARequest<TRequest>? ARequest)

            => ARequest is not null &&

               ChargeBoxId.     Equals(ARequest.ChargeBoxId)      &&
               RequestId.       Equals(ARequest.RequestId)        &&
               RequestTimestamp.Equals(ARequest.RequestTimestamp) &&
               RequestTimeout.  Equals(ARequest.RequestTimeout)   &&
               EventTrackingId. Equals(ARequest.EventTrackingId)  &&
               Action.          Equals(ARequest.Action)           &&

             ((CustomData is     null && ARequest.CustomData is     null) ||
              (CustomData is not null && ARequest.CustomData is not null && CustomData.Equals(ARequest.CustomData)));

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ChargeBoxId.     GetHashCode() * 17 ^
                       RequestId.       GetHashCode() * 13 ^
                       RequestTimestamp.GetHashCode() * 11 ^
                       RequestTimeout.  GetHashCode() *  7 ^
                       EventTrackingId. GetHashCode() *  5 ^
                       Action.          GetHashCode() *  3 ^

                       CustomData?.     GetHashCode() ?? 0;

            }
        }

        #endregion

    }

}
