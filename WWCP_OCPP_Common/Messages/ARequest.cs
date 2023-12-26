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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// An abstract generic OCPP request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the OCPP request.</typeparam>
    public abstract class ARequest<TRequest> : ACustomSignableData,
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
        /// The networking node identification of the message destination.
        /// </summary>
        [Mandatory]
        public NetworkingNode_Id  DestinationNodeId    { get; }

        /// <summary>
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        [Mandatory]
        public NetworkPath        NetworkPath          { get; }

        /// <summary>
        /// The request identification.
        /// </summary>
        [Mandatory]
        public Request_Id         RequestId            { get; set; }

        /// <summary>
        /// The timestamp of the request message creation.
        /// </summary>
        [Mandatory]
        public DateTime           RequestTimestamp     { get; }

        /// <summary>
        /// The timeout of this request.
        /// </summary>
        [Mandatory]
        public TimeSpan           RequestTimeout       { get; }

        /// <summary>
        /// An event tracking identification for correlating this request with other events.
        /// </summary>
        [Mandatory]
        public EventTracking_Id   EventTrackingId      { get; }

        /// <summary>
        /// The OCPP HTTP Web Socket action.
        /// </summary>
        [Mandatory]
        public String             Action               { get; }

        /// <summary>
        /// An optional token to cancel this request.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic OCPP request message.
        /// </summary>
        /// <param name="DestinationNodeId">The networking node identification of the message destination.</param>
        /// <param name="Action">The OCPP HTTP Web Socket action.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this request.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">An optional (recorded) path of the request through the overlay network.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ARequest(NetworkingNode_Id        DestinationNodeId,
                        String                   Action,

                        IEnumerable<KeyPair>?    SignKeys            = null,
                        IEnumerable<SignInfo>?   SignInfos           = null,
                        IEnumerable<Signature>?  Signatures          = null,

                        CustomData?              CustomData          = null,

                        Request_Id?              RequestId           = null,
                        DateTime?                RequestTimestamp    = null,
                        TimeSpan?                RequestTimeout      = null,
                        EventTracking_Id?        EventTrackingId     = null,
                        NetworkPath?             NetworkPath         = null,
                        CancellationToken        CancellationToken   = default)

            : base(SignKeys,
                   SignInfos,
                   Signatures,
                   CustomData)

        {

            this.DestinationNodeId  = DestinationNodeId;
            this.Action             = Action;

            this.RequestId          = RequestId        ?? Request_Id.NewRandom();
            this.RequestTimestamp   = RequestTimestamp ?? Timestamp.Now;
            this.RequestTimeout     = RequestTimeout   ?? DefaultRequestTimeout;
            this.EventTrackingId    = EventTrackingId  ?? EventTracking_Id.New;
            this.NetworkPath        = NetworkPath      ?? NetworkPath.Empty;
            this.CancellationToken  = CancellationToken;

            unchecked
            {

                hashCode = this.DestinationNodeId.GetHashCode() * 17 ^
                           this.NetworkPath.      GetHashCode() * 13 ^
                           this.Action.           GetHashCode() * 11 ^
                           this.RequestId.        GetHashCode() *  7 ^
                           this.RequestTimestamp. GetHashCode() *  5 ^
                           this.RequestTimeout.   GetHashCode() *  3 ^
                           this.EventTrackingId.  GetHashCode();

            }

        }

        #endregion


        public JObject ToAbstractJSON(Object RequestData)

            => ToAbstractJSON(null, RequestData);

        public JObject ToAbstractJSON(IWebSocketConnection  Connection,
                                      Object                RequestData)
        {

            var json = JSONObject.Create(
                           new JProperty("id",                  RequestId.       ToString()),
                           new JProperty("timestamp",           RequestTimestamp.ToIso8601()),
                           new JProperty("eventTrackingId",     EventTrackingId. ToString()),
                         //  new JProperty("connection",          Connection?.     ToJSON()),
                           new JProperty("destinationNodeId",   DestinationNodeId.ToString()),
                           new JProperty("networkPath",         NetworkPath.     ToJSON()),
                           new JProperty("timeout",             RequestTimeout.  TotalSeconds),
                           new JProperty("action",              Action),
                           new JProperty("data",                RequestData)
                       );

            return json;

        }


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

               NetworkPath.     Equals(ARequest.NetworkPath)      &&
               RequestId.       Equals(ARequest.RequestId)        &&
               RequestTimestamp.Equals(ARequest.RequestTimestamp) &&
               RequestTimeout.  Equals(ARequest.RequestTimeout)   &&
               EventTrackingId. Equals(ARequest.EventTrackingId)  &&
               Action.          Equals(ARequest.Action)           &&

             ((CustomData is     null && ARequest.CustomData is     null) ||
              (CustomData is not null && ARequest.CustomData is not null && CustomData.Equals(ARequest.CustomData)));

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => hashCode ^
               base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Action}/{RequestId} '{DestinationNodeId}' via [{NetworkPath}]";

        #endregion


    }

}
