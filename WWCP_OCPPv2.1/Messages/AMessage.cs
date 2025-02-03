/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An abstract generic OCPP message.
    /// </summary>
    /// <typeparam name="TMessage">The type of the OCPP message.</typeparam>
    public abstract class AMessage<TMessage> : ACustomSignableData,
                                               IEquatable<TMessage>

        where TMessage : class, IMessage

    {

        #region Properties

        /// <summary>
        /// The networking node identification of the final message destination.
        /// </summary>
        public NetworkingNode_Id     DestinationId
            => Destination.Last();

        /// <summary>
        /// The alternative source routing path through the overlay network
        /// towards the message destination.
        /// </summary>
        [Mandatory]
        public SourceRouting         Destination            { get; }

        /// <summary>
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        [Mandatory]
        public NetworkPath           NetworkPath            { get; }

        /// <summary>
        /// The message identification.
        /// </summary>
        [Mandatory]
        public Request_Id            MessageId              { get; set; }

        /// <summary>
        /// The timestamp of the request message creation.
        /// </summary>
        [Mandatory]
        public DateTime              MessageTimestamp       { get; }

        /// <summary>
        /// An event tracking identification for correlating this request with other events.
        /// </summary>
        [Mandatory]
        public EventTracking_Id      EventTrackingId        { get; }

        /// <summary>
        /// The OCPP HTTP WebSocket action.
        /// </summary>
        [Mandatory]
        public String                Action                 { get; }

        /// <summary>
        /// The serialization format of the request.
        /// </summary>
        public SerializationFormats  SerializationFormat    { get; }

        /// <summary>
        /// An optional token to cancel this request.
        /// </summary>
        public CancellationToken     CancellationToken      { get; }

        #endregion

        #region Constructor(s)

        public AMessage(SourceRouting            Destination,
                        String                   Action,

                        IEnumerable<KeyPair>?    SignKeys              = null,
                        IEnumerable<SignInfo>?   SignInfos             = null,
                        IEnumerable<Signature>?  Signatures            = null,

                        CustomData?              CustomData            = null,

                        Request_Id?              MessageId             = null,
                        DateTime?                MessageTimestamp      = null,
                        EventTracking_Id?        EventTrackingId       = null,
                        NetworkPath?             NetworkPath           = null,
                        SerializationFormats?    SerializationFormat   = null,
                        CancellationToken        CancellationToken     = default)

            : base(SignKeys,
                   SignInfos,
                   Signatures,
                   CustomData)

        {

            this.Destination          = Destination;
            this.Action               = Action;

            this.MessageId            = MessageId           ?? Request_Id.NewRandom();
            this.MessageTimestamp     = MessageTimestamp    ?? Timestamp.Now;
            this.EventTrackingId      = EventTrackingId     ?? EventTracking_Id.New;
            this.NetworkPath          = NetworkPath         ?? NetworkPath.Empty;
            this.SerializationFormat  = SerializationFormat ?? SerializationFormats.Default;
            this.CancellationToken    = CancellationToken;

            unchecked
            {

                hashCode = this.Destination.     GetHashCode() * 13 ^
                           this.NetworkPath.     GetHashCode() * 11 ^
                           this.Action.          GetHashCode() *  7 ^
                           this.MessageId.       GetHashCode() *  5 ^
                           this.MessageTimestamp.GetHashCode() *  3 ^
                           this.EventTrackingId. GetHashCode();

            }

        }

        #endregion


        public JObject ToAbstractJSON(Object DatagramData)

            => ToAbstractJSON(null, DatagramData);

        public JObject ToAbstractJSON(IWebSocketConnection  Connection,
                                      Object                DatagramData)
        {

            var json = JSONObject.Create(
                           new JProperty("id",               MessageId.       ToString()),
                           new JProperty("timestamp",        MessageTimestamp.ToIso8601()),
                           new JProperty("eventTrackingId",  EventTrackingId. ToString()),
                         //  new JProperty("connection",       Connection?.     ToJSON()),
                           new JProperty("destinationId",    DestinationId.   ToString()),
                           new JProperty("networkPath",      NetworkPath.     ToJSON()),
                           new JProperty("action",           Action),
                           new JProperty("data",             DatagramData)
                       );

            return json;

        }


        #region IEquatable<TDatagram> Members

        /// <summary>
        /// Compare two abstract generic OCPP requests for equality.
        /// </summary>
        /// <param name="TDatagram">Another abstract generic OCPP request.</param>
        public abstract Boolean Equals(TMessage? TDatagram);

        #endregion

        #region GenericEquals(ADatagram)

        /// <summary>
        /// Compare two abstract generic OCPP requests for equality.
        /// </summary>
        /// <param name="ADatagram">Another abstract generic OCPP request.</param>
        public Boolean GenericEquals(AMessage<TMessage>? ADatagram)

            => ADatagram is not null &&

               NetworkPath.     Equals(ADatagram.NetworkPath)      &&
               MessageId.       Equals(ADatagram.MessageId)        &&
               MessageTimestamp.Equals(ADatagram.MessageTimestamp) &&
               EventTrackingId. Equals(ADatagram.EventTrackingId)  &&
               Action.          Equals(ADatagram.Action)           &&

             ((CustomData is     null && ADatagram.CustomData is     null) ||
              (CustomData is not null && ADatagram.CustomData is not null && CustomData.Equals(ADatagram.CustomData)));

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

            => $"{Action}/{MessageId} '{DestinationId}' via [{NetworkPath}]";

        #endregion


    }

}
