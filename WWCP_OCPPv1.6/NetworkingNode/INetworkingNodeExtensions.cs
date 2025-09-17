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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    /// <summary>
    /// Extension methods for all OCPP v1.6 Networking Nodes.
    /// </summary>
    public static class INetworkingNodeExtensions
    {

        //#region TransferData (DestinationId, VendorId, MessageId = null, Data = null, ...)

        ///// <summary>
        ///// Send the given vendor-specific data.
        ///// </summary>
        ///// <param name="Destination">The networking node identification.</param>
        ///// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        ///// <param name="MessageId">An optional message identification field.</param>
        ///// <param name="Data">Optional message data as text without specified length or format.</param>
        ///// 
        ///// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        ///// 
        ///// <param name="RequestId">An optional request identification.</param>
        ///// <param name="RequestTimestamp">An optional request timestamp.</param>
        ///// <param name="RequestTimeout">An optional timeout for this request.</param>
        ///// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        ///// 
        ///// <param name="CancellationToken">An optional token to cancel this request.</param>
        //public static Task<DataTransferResponse>

        //    TransferData(this INetworkingNode     NetworkingNode,
        //                 SourceRouting            Destination,
        //                 Vendor_Id                VendorId,
        //                 Message_Id?              MessageId             = null,
        //                 JToken?                  Data                  = null,

        //                 IEnumerable<KeyPair>?    SignKeys              = null,
        //                 IEnumerable<SignInfo>?   SignInfos             = null,
        //                 IEnumerable<Signature>?  Signatures            = null,

        //                 Request_Id?              RequestId             = null,
        //                 DateTimeOffset?          RequestTimestamp      = null,
        //                 TimeSpan?                RequestTimeout        = null,
        //                 EventTracking_Id?        EventTrackingId       = null,
        //                 SerializationFormats?    SerializationFormat   = null,
        //                 CancellationToken        CancellationToken     = default)


        //        => NetworkingNode.OCPP.OUT.DataTransfer(
        //               new DataTransferRequest(
        //                   Destination,
        //                   VendorId,
        //                   MessageId,
        //                   Data,

        //                   SignKeys,
        //                   SignInfos,
        //                   Signatures,

        //                   RequestId        ?? NetworkingNode.NextRequestId,
        //                   RequestTimestamp ?? Timestamp.Now,
        //                   RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
        //                   EventTrackingId  ?? EventTracking_Id.New,
        //                   NetworkPath.From(NetworkingNode.Id),
        //                   SerializationFormat,
        //                   CancellationToken
        //               )
        //           );

        //#endregion

        //#region SendMessage  (DestinationId, VendorId, MessageId = null, Data = null, ...)

        ///// <summary>
        ///// Send the given vendor-specific message (without the expectation of any response).
        ///// </summary>
        ///// <param name="Destination">The networking node identification.</param>
        ///// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        ///// <param name="MessageId">An optional message identification field.</param>
        ///// <param name="Data">Optional message data as text without specified length or format.</param>
        ///// 
        ///// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        ///// 
        ///// <param name="RequestId">An optional request identification.</param>
        ///// <param name="RequestTimestamp">An optional request timestamp.</param>
        ///// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        ///// 
        ///// <param name="CancellationToken">An optional token to cancel this request.</param>
        //public static Task<SentMessageResult>

        //    SendMessage(this INetworkingNode     NetworkingNode,
        //                SourceRouting            Destination,
        //                Vendor_Id                VendorId,
        //                Message_Id?              MessageId             = null,
        //                JToken?                  Data                  = null,

        //                IEnumerable<KeyPair>?    SignKeys              = null,
        //                IEnumerable<SignInfo>?   SignInfos             = null,
        //                IEnumerable<Signature>?  Signatures            = null,

        //                Request_Id?              RequestId             = null,
        //                DateTimeOffset?          RequestTimestamp      = null,
        //                EventTracking_Id?        EventTrackingId       = null,
        //                SerializationFormats?    SerializationFormat   = null,
        //                CancellationToken        CancellationToken     = default)


        //        => NetworkingNode.OCPP.OUT.MessageTransfer(
        //               new MessageTransferMessage(
        //                   Destination,
        //                   VendorId,
        //                   MessageId,
        //                   Data,

        //                   SignKeys,
        //                   SignInfos,
        //                   Signatures,

        //                   RequestId        ?? NetworkingNode.NextRequestId,
        //                   RequestTimestamp ?? Timestamp.Now,
        //                   EventTrackingId  ?? EventTracking_Id.New,
        //                   NetworkPath.From(NetworkingNode.Id),
        //                   SerializationFormat,
        //                   CancellationToken
        //               )
        //           );

        //#endregion


    }

}
