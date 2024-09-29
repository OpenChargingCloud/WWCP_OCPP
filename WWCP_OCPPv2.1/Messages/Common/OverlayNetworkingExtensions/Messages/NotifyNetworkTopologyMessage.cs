/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The NotifyNetworkTopology message.
    /// </summary>
    public class NotifyNetworkTopologyMessage : AMessage<NotifyNetworkTopologyMessage>,
                                                IMessage
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/notifyNetworkTopologyRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The network topology information.
        /// </summary>
        [Mandatory]
        public NetworkTopologyInformation  NetworkTopologyInformation    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyNetworkTopology request.
        /// </summary>
        /// <param name="Destination">The networking node identification of the message destination.</param>
        /// <param name="NetworkTopologyInformation">Network topology information.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyNetworkTopologyMessage(SourceRouting               Destination,
                                            NetworkTopologyInformation  NetworkTopologyInformation,

                                            IEnumerable<KeyPair>?       SignKeys              = null,
                                            IEnumerable<SignInfo>?      SignInfos             = null,
                                            IEnumerable<Signature>?     Signatures            = null,

                                            CustomData?                 CustomData            = null,

                                            Request_Id?                 RequestId             = null,
                                            DateTime?                   RequestTimestamp      = null,
                                            EventTracking_Id?           EventTrackingId       = null,
                                            NetworkPath?                NetworkPath           = null,
                                            SerializationFormats?       SerializationFormat   = null,
                                            CancellationToken           CancellationToken     = default)

            : base(Destination,
                   nameof(NotifyNetworkTopologyMessage)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.NetworkTopologyInformation = NetworkTopologyInformation;

            unchecked
            {
                hashCode = this.NetworkTopologyInformation.GetHashCode() * 3 ^
                           base.                           GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomNotifyNetworkTopologyMessageParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyNetworkTopology request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyNetworkTopologyMessageParser">An optional delegate to parse custom NotifyNetworkTopology requests.</param>
        public static NotifyNetworkTopologyMessage Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                               Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   RequestTimestamp                           = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<NotifyNetworkTopologyMessage>?  CustomNotifyNetworkTopologyMessageParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyNetworkTopologyRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         EventTrackingId,
                         CustomNotifyNetworkTopologyMessageParser))
            {
                return notifyNetworkTopologyRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyNetworkTopology request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out NotifyNetworkTopologyMessage, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyNetworkTopology request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyNetworkTopologyMessage">The parsed NotifyNetworkTopology request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyNetworkTopologyMessageParser">An optional delegate to parse custom NotifyNetworkTopology requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out NotifyNetworkTopologyMessage?      NotifyNetworkTopologyMessage,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   RequestTimestamp                           = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<NotifyNetworkTopologyMessage>?  CustomNotifyNetworkTopologyMessageParser   = null)
        {

            try
            {

                NotifyNetworkTopologyMessage = null;

                #region NetworkTopologyInformation    [mandatory]

                if (!JSON.ParseMandatoryJSON("networkTopologyInformation",
                                             "network topology information",
                                             WWCP.NetworkingNode.NetworkTopologyInformation.TryParse,
                                             out NetworkTopologyInformation? NetworkTopologyInformation,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures                    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyNetworkTopologyMessage = new NotifyNetworkTopologyMessage(

                                                   Destination,
                                                   NetworkTopologyInformation,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData,

                                                   RequestId,
                                                   RequestTimestamp,
                                                   EventTrackingId,
                                                   NetworkPath

                                               );

                if (CustomNotifyNetworkTopologyMessageParser is not null)
                    NotifyNetworkTopologyMessage = CustomNotifyNetworkTopologyMessageParser(JSON,
                                                                                            NotifyNetworkTopologyMessage);

                return true;

            }
            catch (Exception e)
            {
                NotifyNetworkTopologyMessage  = null;
                ErrorResponse                 = "The given JSON representation of a NotifyNetworkTopology request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyNetworkTopologyMessageSerializer = null, CustomNetworkTopologyInformationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyNetworkTopologyMessageSerializer">A delegate to serialize custom NotifyNetworkTopology requests.</param>
        /// <param name="CustomNetworkTopologyInformationSerializer">A delegate to serialize custom signature policies.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyNetworkTopologyMessage>?  CustomNotifyNetworkTopologyMessageSerializer   = null,
                              CustomJObjectSerializerDelegate<NetworkTopologyInformation>?    CustomNetworkTopologyInformationSerializer     = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("networkTopologyInformation",   NetworkTopologyInformation.ToJSON(CustomNetworkTopologyInformationSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",                   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                   CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyNetworkTopologyMessageSerializer is not null
                       ? CustomNotifyNetworkTopologyMessageSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyNetworkTopologyMessage1, NotifyNetworkTopologyMessage2)

        /// <summary>
        /// Compares two NotifyNetworkTopology requests for equality.
        /// </summary>
        /// <param name="NotifyNetworkTopologyMessage1">A NotifyNetworkTopology request.</param>
        /// <param name="NotifyNetworkTopologyMessage2">Another NotifyNetworkTopology request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyNetworkTopologyMessage? NotifyNetworkTopologyMessage1,
                                           NotifyNetworkTopologyMessage? NotifyNetworkTopologyMessage2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyNetworkTopologyMessage1, NotifyNetworkTopologyMessage2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyNetworkTopologyMessage1 is null || NotifyNetworkTopologyMessage2 is null)
                return false;

            return NotifyNetworkTopologyMessage1.Equals(NotifyNetworkTopologyMessage2);

        }

        #endregion

        #region Operator != (NotifyNetworkTopologyMessage1, NotifyNetworkTopologyMessage2)

        /// <summary>
        /// Compares two NotifyNetworkTopology requests for inequality.
        /// </summary>
        /// <param name="NotifyNetworkTopologyMessage1">A NotifyNetworkTopology request.</param>
        /// <param name="NotifyNetworkTopologyMessage2">Another NotifyNetworkTopology request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyNetworkTopologyMessage? NotifyNetworkTopologyMessage1,
                                           NotifyNetworkTopologyMessage? NotifyNetworkTopologyMessage2)

            => !(NotifyNetworkTopologyMessage1 == NotifyNetworkTopologyMessage2);

        #endregion

        #endregion

        #region IEquatable<NotifyNetworkTopologyMessage> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyNetworkTopology requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyNetworkTopology request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyNetworkTopologyMessage notifyNetworkTopologyRequest &&
                   Equals(notifyNetworkTopologyRequest);

        #endregion

        #region Equals(NotifyNetworkTopologyMessage)

        /// <summary>
        /// Compares two NotifyNetworkTopology requests for equality.
        /// </summary>
        /// <param name="NotifyNetworkTopologyMessage">A NotifyNetworkTopology request to compare with.</param>
        public override Boolean Equals(NotifyNetworkTopologyMessage? NotifyNetworkTopologyMessage)

            => NotifyNetworkTopologyMessage is not null &&

               NetworkTopologyInformation.Equals(NotifyNetworkTopologyMessage.NetworkTopologyInformation) &&

               base.    GenericEquals(NotifyNetworkTopologyMessage);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => NetworkTopologyInformation.ToString();

        #endregion

    }

}
