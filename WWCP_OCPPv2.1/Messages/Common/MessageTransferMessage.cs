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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The MessageTransfer message.
    /// </summary>
    public class MessageTransferMessage : AMessage<MessageTransferMessage>,
                                          IMessage
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/common/message");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The vendor identification or namespace of the given message.
        /// </summary>
        [Mandatory]
        public Vendor_Id      VendorId     { get; }

        /// <summary>
        /// The optional message identification.
        /// </summary>
        [Optional]
        public Message_Id?    MessageId    { get; }

        /// <summary>
        /// Optional vendor-specific message data (a JSON token).
        /// </summary>
        [Optional]
        public JToken?        Data         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message.
        /// </summary>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">Optional vendor-specific message data (a JSON token).</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public MessageTransferMessage(SourceRouting            Destination,
                                      Vendor_Id                VendorId,
                                      Message_Id?              MessageId             = null,
                                      JToken?                  Data                  = null,

                                      IEnumerable<KeyPair>?    SignKeys              = null,
                                      IEnumerable<SignInfo>?   SignInfos             = null,
                                      IEnumerable<Signature>?  Signatures            = null,

                                      Request_Id?              RequestId             = null,
                                      DateTime?                RequestTimestamp      = null,
                                      EventTracking_Id?        EventTrackingId       = null,
                                      NetworkPath?             NetworkPath           = null,
                                      SerializationFormats?    SerializationFormat   = null,
                                      CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(MessageTransferMessage)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   null,

                   RequestId,
                   RequestTimestamp,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.VendorId   = VendorId;
            this.MessageId  = MessageId;
            this.Data       = Data;

            unchecked
            {

                hashCode = this.VendorId.  GetHashCode()       * 7 ^
                          (this.MessageId?.GetHashCode() ?? 0) * 5 ^
                          (this.Data?.     GetHashCode() ?? 0) * 3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, MessageId, SourceRouting, NetworkPath, CustomDatagramRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a Datagram request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MessageId">The message identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="MessageTimestamp">An optional message timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomMessageTransferMessageParser">A delegate to parse custom MessageTransfer messages.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static MessageTransferMessage Parse(JObject                                               JSON,
                                                   Request_Id                                            MessageId,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             MessageTimestamp                     = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<MessageTransferMessage>?  CustomMessageTransferMessageParser   = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            if (TryParse(JSON,
                         MessageId,
                         Destination,
                         NetworkPath,
                         out var messageTransferMessage,
                         out var errorResponse,
                         MessageTimestamp,
                         EventTrackingId,
                         CustomMessageTransferMessageParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return messageTransferMessage;
            }

            throw new ArgumentException("The given JSON representation of a Message datagram is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, MessageId, SourceRouting, NetworkPath, out MessageTransferMessage, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a Datagram request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MessageId">The message identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="MessageTransferMessage">The parsed MessageTransfer message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="MessageTimestamp">An optional message timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomMessageTransferMessageParser">A delegate to parse custom MessageTransfer messages.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            MessageId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out MessageTransferMessage?      MessageTransferMessage,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             MessageTimestamp                     = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<MessageTransferMessage>?  CustomMessageTransferMessageParser   = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                MessageTransferMessage = null;

                #region VendorId             [mandatory]

                if (!JSON.ParseMandatory("vendorId",
                                         "vendor identification",
                                         Vendor_Id.TryParse,
                                         out Vendor_Id VendorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MessageId2           [optional]

                if (JSON.ParseOptional("messageId",
                                       "message identification",
                                       Message_Id.TryParse,
                                       out Message_Id? MessageId2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Data                 [optional]

                var Data = JSON["data"];

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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


                MessageTransferMessage = new MessageTransferMessage(

                                             Destination,
                                             VendorId,
                                             MessageId2,
                                             Data,

                                             null,
                                             null,
                                             Signatures,

                                             MessageId,
                                             MessageTimestamp,
                                             EventTrackingId,
                                             NetworkPath

                                         );

                if (CustomMessageTransferMessageParser is not null)
                    MessageTransferMessage = CustomMessageTransferMessageParser(JSON,
                                                                                MessageTransferMessage);

                return true;

            }
            catch (Exception e)
            {
                MessageTransferMessage  = null;
                ErrorResponse           = "The given JSON representation of a Datagram request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDatagramRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDatagramRequestSerializer">A delegate to serialize custom Datagram requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MessageTransferMessage>?  CustomDatagramRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("vendorId",     VendorId.       TextId),

                           MessageId.HasValue
                               ? new JProperty("messageId",    MessageId.Value.TextId)
                               : null,

                           Data is not null
                               ? new JProperty("data",         Data)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDatagramRequestSerializer is not null
                       ? CustomDatagramRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DatagramRequest1, DatagramRequest2)

        /// <summary>
        /// Compares two Datagram requests for equality.
        /// </summary>
        /// <param name="DatagramRequest1">A Datagram request.</param>
        /// <param name="DatagramRequest2">Another Datagram request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MessageTransferMessage? DatagramRequest1,
                                           MessageTransferMessage? DatagramRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DatagramRequest1, DatagramRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DatagramRequest1 is null || DatagramRequest2 is null)
                return false;

            return DatagramRequest1.Equals(DatagramRequest2);

        }

        #endregion

        #region Operator != (DatagramRequest1, DatagramRequest2)

        /// <summary>
        /// Compares two Datagram requests for inequality.
        /// </summary>
        /// <param name="DatagramRequest1">A Datagram request.</param>
        /// <param name="DatagramRequest2">Another Datagram request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MessageTransferMessage? DatagramRequest1,
                                           MessageTransferMessage? DatagramRequest2)

            => !(DatagramRequest1 == DatagramRequest2);

        #endregion

        #endregion

        #region IEquatable<DatagramRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Datagram requests for equality.
        /// </summary>
        /// <param name="Object">A Datagram request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessageTransferMessage dataTransferRequest &&
                   Equals(dataTransferRequest);

        #endregion

        #region Equals(DatagramRequest)

        /// <summary>
        /// Compares two Datagram requests for equality.
        /// </summary>
        /// <param name="DatagramRequest">A Datagram request to compare with.</param>
        public override Boolean Equals(MessageTransferMessage? DatagramRequest)

            => DatagramRequest is not null               &&

               VendorId.Equals(DatagramRequest.VendorId) &&

             ((MessageId is     null && DatagramRequest.MessageId is     null) ||
              (MessageId is not null && DatagramRequest.MessageId is not null && MessageId.Equals(DatagramRequest.MessageId))) &&

             ((Data      is     null && DatagramRequest.Data      is     null) ||
              (Data      is not null && DatagramRequest.Data      is not null && Data.     Equals(DatagramRequest.Data)))      &&

               base.GenericEquals(DatagramRequest);

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

            => $"{VendorId}: {MessageId?.ToString() ?? "-"} => {Data?.ToString() ?? "-"}";

        #endregion


    }

}
