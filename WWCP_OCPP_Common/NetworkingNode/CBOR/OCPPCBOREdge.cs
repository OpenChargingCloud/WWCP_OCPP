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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPP.NetworkingNode
{

    /// <summary>
    /// What the CBOR edge of an adapter needs of that adapter.
    /// </summary>
    public interface IOCPPCBOREdgeHost
    {

        /// <summary>The identification of the node of the adapter.</summary>
        NetworkingNode_Id  Id                       { get; }

        /// <summary>The time of the adapter.</summary>
        DateTimeOffset     Now                      { get; }

        /// <summary>How long a request waits for its answer, where it does not say.</summary>
        TimeSpan           DefaultRequestTimeout    { get; }

        /// <summary>Process a message that was received as JSON, or was made JSON at the edge.</summary>
        Task ProcessJSONMessage(DateTimeOffset      MessageTimestamp,
                                IMessageConnection  Connection,
                                NetworkingNode_Id?  SourceNodeId,
                                JArray              JSONMessage,
                                EventTracking_Id    EventTrackingId,
                                CancellationToken   CancellationToken);

        /// <summary>Process a message of the binary format.</summary>
        Task ProcessBinaryFormatMessage(DateTimeOffset      MessageTimestamp,
                                        IMessageConnection  Connection,
                                        NetworkingNode_Id?  SourceNodeId,
                                        Byte[]              BinaryMessage,
                                        EventTracking_Id    EventTrackingId,
                                        CancellationToken   CancellationToken);

        /// <summary>Send a request error as CBOR, over the given connection where it is known.</summary>
        Task<SentMessageResult> SendCBORRequestError(OCPP_CBORRequestErrorMessage  CBORRequestErrorMessage,
                                                     IMessageConnection?           Over);

        /// <summary>Tell whoever waits for the answer to a request that it is an error.</summary>
        Boolean ReceiveJSONRequestError(OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                        IMessageConnection            Connection);

        /// <summary>Tell whoever answered that its answer cannot be read.</summary>
        Task TellTheAnswererItCannotBeRead(IMessageConnection?  Connection,
                                           NetworkPath          ResponseNetworkPath,
                                           Request_Id           RequestId,
                                           EventTracking_Id     EventTrackingId,
                                           Boolean              Binary,
                                           String?              Why,
                                           CancellationToken    CancellationToken);

        /// <summary>Tell of an exception.</summary>
        Task HandleErrors(String     Caller,
                          Exception  ExceptionOccurred);

    }


    /// <summary>
    /// The CBOR edge of an OCPP adapter.
    ///
    /// An adapter works on JSON. What a connection that agreed on a CBOR subprotocol
    /// sends is translated here on its way out - the payload through the class of its
    /// message (<see cref="MessagesAsCBOR"/>), the envelope into the CBOR message of
    /// the same kind - and what it receives on its way in, so that the adapter is
    /// handed the JSON array the CBOR stands for. A response is read and written
    /// together with its request, so the requests that crossed a CBOR connection are
    /// kept here until they are answered or timed out. The same for every version of
    /// OCPP: which messages there are is what the translation knows.
    /// </summary>
    /// <param name="Translation">The translation of the messages of one version of OCPP.</param>
    /// <param name="Host">The adapter this is the edge of.</param>
    public sealed class OCPPCBOREdge(MessagesAsCBOR     Translation,
                                     IOCPPCBOREdgeHost  Host)
    {

        #region Data

        private readonly ConcurrentDictionary<Request_Id, CBORExchange>                        cborRequestsSent      = [];
        private readonly ConcurrentDictionary<(IMessageConnection, Request_Id), CBORExchange>  cborRequestsReceived  = [];

        #endregion

        #region Properties

        /// <summary>
        /// The translation of the messages.
        /// </summary>
        public MessagesAsCBOR  Translation    { get; } = Translation;

        private DateTimeOffset Now
            => Host.Now;

        private TimeSpan DefaultRequestTimeout
            => Host.DefaultRequestTimeout;

        #endregion


        #region (class) CBORExchange

        /// <summary>
        /// A request that crossed a CBOR connection, kept until it is answered: its
        /// response is read and written together with it.
        /// </summary>
        /// <param name="Action">The action of the request.</param>
        /// <param name="Request">The request, as its class read it - or null, for a request nobody here has a class for, which is carried blind as its response will be.</param>
        /// <param name="Until">When nobody waits for its answer any more.</param>
        /// <param name="From">The networking node the request came from, for a request that was received.</param>
        public sealed record CBORExchange(String              Action,
                                            Object?             Request,
                                            DateTimeOffset      Until,
                                            NetworkingNode_Id?  From   = null);

        #endregion

        #region (private) Forget(Exchanges)

        /// <summary>
        /// Let go of the requests nobody waits for any more.
        /// </summary>
        private void Forget<TKey>(ConcurrentDictionary<TKey, CBORExchange> Exchanges)
            where TKey : notnull
        {

            if (Exchanges.Count < 64)
                return;

            var now = Now;

            foreach (var exchange in Exchanges)
                if (exchange.Value.Until < now)
                    Exchanges.TryRemove(exchange.Key, out _);

        }

        #endregion

        #region RememberCBORRequestReceived(Connection, RequestId, Action, Request, Until)

        /// <summary>
        /// Remember a request that arrived as CBOR, for the response to it.
        /// </summary>
        public void RememberCBORRequestReceived(IMessageConnection  Connection,
                                                  Request_Id          RequestId,
                                                  String              Action,
                                                  Object?             Request,
                                                  DateTimeOffset      Until,
                                                  NetworkingNode_Id?  From   = null)
        {

            Forget(cborRequestsReceived);

            cborRequestsReceived[(Connection, RequestId)] = new CBORExchange(
                                                                Action,
                                                                Request,
                                                                Until > Now ? Until : Now + DefaultRequestTimeout,
                                                                From
                                                            );

        }

        #endregion

        #region TryTakeCBORRequestReceived (Over, RequestId, out Exchange)

        /// <summary>
        /// The request that arrived as CBOR over the given connection. Where the
        /// connection is not known - an answer that is forwarded comes without one - it
        /// is the request of that identification that came from the networking node the
        /// answer goes to: two charging stations behind a local controller number their
        /// requests the same way. Failing that, the first one of that identification.
        /// </summary>
        public Boolean TryTakeCBORRequestReceived(IMessageConnection?                        Over,
                                                   Request_Id                                 RequestId,
                                                   NetworkingNode_Id                          To,
                                                   [NotNullWhen(true)] out CBORExchange?      Exchange)
        {

            if (Over is not null && cborRequestsReceived.TryRemove((Over, RequestId), out Exchange))
                return true;

            foreach (var exchange in cborRequestsReceived)
                if (exchange.Key.Item2 == RequestId && exchange.Value.From == To && cborRequestsReceived.TryRemove(exchange.Key, out Exchange))
                    return true;

            foreach (var key in cborRequestsReceived.Keys)
                if (key.Item2 == RequestId && cborRequestsReceived.TryRemove(key, out Exchange))
                    return true;

            Exchange = null;
            return false;

        }

        #endregion

        #region TryTakeCBORRequestSent     (RequestId, out Exchange)

        /// <summary>
        /// The request this node sent as CBOR under the given identification.
        /// </summary>
        public Boolean TryTakeCBORRequestSent(Request_Id                             RequestId,
                                                [NotNullWhen(true)] out CBORExchange?  Exchange)

            => cborRequestsSent.TryRemove(RequestId, out Exchange);

        #endregion

        #region (private static) NotAsCBOR(Why)

        private static SentMessageResult NotAsCBOR(String? Why)

            => SentMessageResult.TransmissionFailed(
                   new InvalidOperationException(Why ?? "The message could not be written as CBOR!")
               );

        #endregion


        #region SendRequest      (Transport, JSONRequestMessage)

        /// <summary>
        /// Send the given request the way its connection speaks: as JSON, or as the CBOR
        /// its class writes.
        /// </summary>
        public async Task<SentMessageResult> SendRequest(IOCPPMessageTransport    Transport,
                                                          OCPP_JSONRequestMessage  JSONRequestMessage)
        {

            if (!Transport.SpeaksCBORTo(JSONRequestMessage.Destination.Next))
                return await Transport.SendJSONRequest(JSONRequestMessage);

            if (!Translation.TryRequestToCBOR(
                     JSONRequestMessage.Action,
                     JSONRequestMessage.Payload,
                     JSONRequestMessage.RequestId,
                     JSONRequestMessage.Destination,
                     JSONRequestMessage.NetworkPath,
                     out var cbor,
                     out var request,
                     out var errorResponse
                 ))
            {
                return NotAsCBOR(errorResponse);
            }

            // Before it is sent: its answer may be here before the sending returns.
            Forget(cborRequestsSent);

            cborRequestsSent[JSONRequestMessage.RequestId] = new CBORExchange(
                                                                 JSONRequestMessage.Action,
                                                                 request,
                                                                 JSONRequestMessage.RequestTimeout
                                                             );

            var cborRequestMessage  = new OCPP_CBORRequestMessage(
                                          JSONRequestMessage.RequestTimestamp,
                                          JSONRequestMessage.EventTrackingId,
                                          JSONRequestMessage.NetworkingMode,
                                          JSONRequestMessage.Destination,
                                          JSONRequestMessage.NetworkPath,
                                          JSONRequestMessage.RequestId,
                                          JSONRequestMessage.Action,
                                          cbor,
                                          JSONRequestMessage.RequestTimeout,
                                          JSONRequestMessage.ErrorMessage,
                                          JSONRequestMessage.CancellationToken
                                      );

            var sentMessageResult   = await Transport.SendCBORRequest(cborRequestMessage);

            // The transport says in which networking mode a message leaves, and whoever
            // listens to what this node sent sees the message it was handed.
            JSONRequestMessage.NetworkingMode = cborRequestMessage.NetworkingMode;

            if (sentMessageResult.Result != SentMessageResults.Success)
                cborRequestsSent.TryRemove(JSONRequestMessage.RequestId, out _);

            return sentMessageResult;

        }

        #endregion

        #region SendResponse     (Transport, JSONResponseMessage, Over)

        /// <summary>
        /// Send the given response the way its connection speaks. As CBOR it is written
        /// together with the request it answers - which arrived as CBOR, and was kept.
        /// </summary>
        public async Task<SentMessageResult> SendResponse(IOCPPMessageTransport     Transport,
                                                           OCPP_JSONResponseMessage  JSONResponseMessage,
                                                           IMessageConnection?       Over)
        {

            if (!Transport.SpeaksCBORTo(JSONResponseMessage.Destination.Next))
                return await Transport.SendJSONResponse(JSONResponseMessage);

            String? errorResponse;

            if (!TryTakeCBORRequestReceived(Over, JSONResponseMessage.RequestId, JSONResponseMessage.Destination.Next, out var asked))
                errorResponse = $"The request '{JSONResponseMessage.RequestId}' is not known any more, so its response can not be written as CBOR!";

            else if (Translation.TryResponseToCBOR(
                         asked.Action,
                         asked.Request,
                         JSONResponseMessage.Payload,
                         JSONResponseMessage.RequestId,
                         JSONResponseMessage.Destination,
                         JSONResponseMessage.NetworkPath,
                         out var cbor,
                         out errorResponse
                     ))
            {

                var cborResponseMessage  = new OCPP_CBORResponseMessage(
                                               JSONResponseMessage.ResponseTimestamp,
                                               JSONResponseMessage.EventTrackingId,
                                               JSONResponseMessage.NetworkingMode,
                                               JSONResponseMessage.Destination,
                                               JSONResponseMessage.NetworkPath,
                                               JSONResponseMessage.RequestId,
                                               cbor,
                                               JSONResponseMessage.CancellationToken
                                           );

                var sentMessageResult    = await Transport.SendCBORResponse(cborResponseMessage);

                JSONResponseMessage.NetworkingMode = cborResponseMessage.NetworkingMode;

                return sentMessageResult;

            }

            // Whoever asked is told, instead of waiting out its timeout.
            await Transport.SendCBORRequestError(
                      new OCPP_CBORRequestErrorMessage(
                          Now,
                          JSONResponseMessage.EventTrackingId,
                          JSONResponseMessage.NetworkingMode,
                          JSONResponseMessage.Destination,
                          JSONResponseMessage.NetworkPath,
                          JSONResponseMessage.RequestId,
                          ResultCode.InternalError,
                          errorResponse
                      )
                  );

            return NotAsCBOR(errorResponse);

        }

        #endregion

        #region SendRequestError (Transport, JSONRequestErrorMessage, Over)

        public async Task<SentMessageResult> SendRequestError(IOCPPMessageTransport         Transport,
                                                               OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,
                                                               IMessageConnection?           Over)
        {

            if (!Transport.SpeaksCBORTo(JSONRequestErrorMessage.Destination.Next))
                return await Transport.SendJSONRequestError(JSONRequestErrorMessage);

            // The request is answered with this.
            TryTakeCBORRequestReceived(Over, JSONRequestErrorMessage.RequestId, JSONRequestErrorMessage.Destination.Next, out _);

            var cborRequestErrorMessage  = OCPP_CBORRequestErrorMessage.From(JSONRequestErrorMessage);
            var sentMessageResult        = await Transport.SendCBORRequestError(cborRequestErrorMessage);

            JSONRequestErrorMessage.NetworkingMode = cborRequestErrorMessage.NetworkingMode;

            return sentMessageResult;

        }

        #endregion

        #region SendResponseError(Transport, JSONResponseErrorMessage)

        public static async Task<SentMessageResult> SendResponseError(IOCPPMessageTransport          Transport,
                                                                       OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage)
        {

            if (!Transport.SpeaksCBORTo(JSONResponseErrorMessage.Destination.Next))
                return await Transport.SendJSONResponseError(JSONResponseErrorMessage);

            var cborResponseErrorMessage  = OCPP_CBORResponseErrorMessage.From(JSONResponseErrorMessage);
            var sentMessageResult         = await Transport.SendCBORResponseError(cborResponseErrorMessage);

            JSONResponseErrorMessage.NetworkingMode = cborResponseErrorMessage.NetworkingMode;

            return sentMessageResult;

        }

        #endregion

        #region SendSendMessage  (Transport, JSONSendMessage)

        /// <summary>
        /// Send the given message without an answer the way its connections speak. A
        /// broadcast goes out both ways: a server may have connections of both kinds.
        /// </summary>
        public async Task<SentMessageResult> SendSendMessage(IOCPPMessageTransport  Transport,
                                                              OCPP_JSONSendMessage   JSONSendMessage)
        {

            var speaksCBOR   = Transport.SpeaksCBORTo(JSONSendMessage.Destination.Next);
            var isBroadcast  = JSONSendMessage.Destination.Next == NetworkingNode_Id.Broadcast;

            SentMessageResult? sentMessageResult = null;

            if (!speaksCBOR || isBroadcast)
                sentMessageResult = await Transport.SendJSONSendMessage(JSONSendMessage);

            if (speaksCBOR)
            {

                if (!Translation.TryMessageToCBOR(
                         JSONSendMessage.Action,
                         JSONSendMessage.Payload,
                         JSONSendMessage.MessageId,
                         JSONSendMessage.Destination,
                         JSONSendMessage.NetworkPath,
                         out var cbor,
                         out var errorResponse
                     ))
                {
                    return sentMessageResult ?? NotAsCBOR(errorResponse);
                }

                var cborSendMessage  = new OCPP_CBORSendMessage(
                                           JSONSendMessage.MessageTimestamp,
                                           JSONSendMessage.EventTrackingId,
                                           JSONSendMessage.NetworkingMode,
                                           JSONSendMessage.Destination,
                                           JSONSendMessage.NetworkPath,
                                           JSONSendMessage.MessageId,
                                           JSONSendMessage.Action,
                                           cbor,
                                           JSONSendMessage.ErrorMessage,
                                           JSONSendMessage.CancellationToken
                                       );

                var sentAsCBOR       = await Transport.SendCBORSendMessage(cborSendMessage);

                if (sentMessageResult is null || sentAsCBOR.Result == SentMessageResults.Success)
                {
                    sentMessageResult               = sentAsCBOR;
                    JSONSendMessage.NetworkingMode  = cborSendMessage.NetworkingMode;
                }

            }

            return sentMessageResult ?? SentMessageResult.UnknownClient();

        }

        #endregion

        #region SendRequest      (Transport, BinaryRequestMessage)

        /// <summary>
        /// Send the given request of the binary format the way its connection speaks.
        ///
        /// A connection that agreed on a CBOR subprotocol has CBOR in its binary frames
        /// and nothing else. There a message of the binary format is a CBOR message as
        /// every other, whose payload is a byte string: the bytes its class wrote.
        /// </summary>
        public async Task<SentMessageResult> SendRequest(IOCPPMessageTransport      Transport,
                                                          OCPP_BinaryRequestMessage  BinaryRequestMessage)
        {

            if (!Transport.SpeaksCBORTo(BinaryRequestMessage.Destination.Next))
                return await Transport.SendBinaryRequest(BinaryRequestMessage);

            // Before it is sent: its answer may be here before the sending returns. An
            // answer in JSON to it is carried as what that JSON says.
            Forget(cborRequestsSent);

            cborRequestsSent[BinaryRequestMessage.RequestId] = new CBORExchange(
                                                                   BinaryRequestMessage.Action,
                                                                   null,
                                                                   BinaryRequestMessage.RequestTimeout
                                                               );

            var cborRequestMessage  = new OCPP_CBORRequestMessage(
                                          BinaryRequestMessage.RequestTimestamp,
                                          BinaryRequestMessage.EventTrackingId,
                                          BinaryRequestMessage.NetworkingMode,
                                          BinaryRequestMessage.Destination,
                                          BinaryRequestMessage.NetworkPath,
                                          BinaryRequestMessage.RequestId,
                                          BinaryRequestMessage.Action,
                                          CBORValue.FromBytes(BinaryRequestMessage.Payload),
                                          BinaryRequestMessage.RequestTimeout,
                                          BinaryRequestMessage.ErrorMessage,
                                          BinaryRequestMessage.CancellationToken
                                      );

            var sentMessageResult   = await Transport.SendCBORRequest(cborRequestMessage);

            BinaryRequestMessage.NetworkingMode = cborRequestMessage.NetworkingMode;

            if (sentMessageResult.Result != SentMessageResults.Success)
                cborRequestsSent.TryRemove(BinaryRequestMessage.RequestId, out _);

            return sentMessageResult;

        }

        #endregion

        #region SendResponse     (Transport, BinaryResponseMessage, Over)

        public async Task<SentMessageResult> SendResponse(IOCPPMessageTransport       Transport,
                                                           OCPP_BinaryResponseMessage  BinaryResponseMessage,
                                                           IMessageConnection?         Over)
        {

            if (!Transport.SpeaksCBORTo(BinaryResponseMessage.Destination.Next))
                return await Transport.SendBinaryResponse(BinaryResponseMessage);

            // The request is answered with this.
            TryTakeCBORRequestReceived(Over, BinaryResponseMessage.RequestId, BinaryResponseMessage.Destination.Next, out _);

            var cborResponseMessage  = new OCPP_CBORResponseMessage(
                                           BinaryResponseMessage.ResponseTimestamp,
                                           BinaryResponseMessage.EventTrackingId,
                                           BinaryResponseMessage.NetworkingMode,
                                           BinaryResponseMessage.Destination,
                                           BinaryResponseMessage.NetworkPath,
                                           BinaryResponseMessage.RequestId,
                                           CBORValue.FromBytes(BinaryResponseMessage.Payload),
                                           BinaryResponseMessage.CancellationToken
                                       );

            var sentMessageResult    = await Transport.SendCBORResponse(cborResponseMessage);

            BinaryResponseMessage.NetworkingMode = cborResponseMessage.NetworkingMode;

            return sentMessageResult;

        }

        #endregion

        #region SendRequestError (Transport, BinaryRequestErrorMessage, Over)

        /// <summary>
        /// A request error says the same in every format: over CBOR it is the CBOR
        /// request error.
        /// </summary>
        public async Task<SentMessageResult> SendRequestError(IOCPPMessageTransport           Transport,
                                                               OCPP_BinaryRequestErrorMessage  BinaryRequestErrorMessage,
                                                               IMessageConnection?             Over)
        {

            if (!Transport.SpeaksCBORTo(BinaryRequestErrorMessage.Destination.Next))
                return await Transport.SendBinaryRequestError(BinaryRequestErrorMessage);

            // The request is answered with this.
            TryTakeCBORRequestReceived(Over, BinaryRequestErrorMessage.RequestId, BinaryRequestErrorMessage.Destination.Next, out _);

            var cborRequestErrorMessage  = new OCPP_CBORRequestErrorMessage(
                                               BinaryRequestErrorMessage.ResponseTimestamp,
                                               BinaryRequestErrorMessage.EventTrackingId,
                                               BinaryRequestErrorMessage.NetworkingMode,
                                               BinaryRequestErrorMessage.Destination,
                                               BinaryRequestErrorMessage.NetworkPath,
                                               BinaryRequestErrorMessage.RequestId,
                                               BinaryRequestErrorMessage.ErrorCode,
                                               BinaryRequestErrorMessage.ErrorDescription,
                                               BinaryRequestErrorMessage.ErrorDetails,
                                               BinaryRequestErrorMessage.CancellationToken
                                           );

            var sentMessageResult        = await Transport.SendCBORRequestError(cborRequestErrorMessage);

            BinaryRequestErrorMessage.NetworkingMode = cborRequestErrorMessage.NetworkingMode;

            return sentMessageResult;

        }

        #endregion

        #region SendResponseError(Transport, BinaryResponseErrorMessage)

        public static async Task<SentMessageResult> SendResponseError(IOCPPMessageTransport            Transport,
                                                                       OCPP_BinaryResponseErrorMessage  BinaryResponseErrorMessage)
        {

            if (!Transport.SpeaksCBORTo(BinaryResponseErrorMessage.Destination.Next))
                return await Transport.SendBinaryResponseError(BinaryResponseErrorMessage);

            var cborResponseErrorMessage  = new OCPP_CBORResponseErrorMessage(
                                                BinaryResponseErrorMessage.ResponseTimestamp,
                                                BinaryResponseErrorMessage.EventTrackingId,
                                                BinaryResponseErrorMessage.NetworkingMode,
                                                BinaryResponseErrorMessage.Destination,
                                                BinaryResponseErrorMessage.NetworkPath,
                                                BinaryResponseErrorMessage.RequestId,
                                                BinaryResponseErrorMessage.ErrorCode,
                                                BinaryResponseErrorMessage.ErrorDescription,
                                                BinaryResponseErrorMessage.ErrorDetails,
                                                BinaryResponseErrorMessage.CancellationToken
                                            );

            var sentMessageResult         = await Transport.SendCBORResponseError(cborResponseErrorMessage);

            BinaryResponseErrorMessage.NetworkingMode = cborResponseErrorMessage.NetworkingMode;

            return sentMessageResult;

        }

        #endregion

        #region SendSendMessage  (Transport, BinarySendMessage)

        /// <summary>
        /// Send the given message of the binary format without an answer the way its
        /// connections speak. A broadcast goes out both ways.
        /// </summary>
        public static async Task<SentMessageResult> SendSendMessage(IOCPPMessageTransport   Transport,
                                                                     OCPP_BinarySendMessage  BinarySendMessage)
        {

            var speaksCBOR   = Transport.SpeaksCBORTo(BinarySendMessage.Destination.Next);
            var isBroadcast  = BinarySendMessage.Destination.Next == NetworkingNode_Id.Broadcast;

            SentMessageResult? sentMessageResult = null;

            if (!speaksCBOR || isBroadcast)
                sentMessageResult = await Transport.SendBinarySendMessage(BinarySendMessage);

            if (speaksCBOR)
            {

                var cborSendMessage  = new OCPP_CBORSendMessage(
                                           BinarySendMessage.MessageTimestamp,
                                           BinarySendMessage.EventTrackingId,
                                           BinarySendMessage.NetworkingMode,
                                           BinarySendMessage.Destination,
                                           BinarySendMessage.NetworkPath,
                                           BinarySendMessage.MessageId,
                                           BinarySendMessage.Action,
                                           CBORValue.FromBytes(BinarySendMessage.Payload),
                                           BinarySendMessage.ErrorMessage,
                                           BinarySendMessage.CancellationToken
                                       );

                var sentAsCBOR       = await Transport.SendCBORSendMessage(cborSendMessage);

                if (sentMessageResult is null || sentAsCBOR.Result == SentMessageResults.Success)
                {
                    sentMessageResult                 = sentAsCBOR;
                    BinarySendMessage.NetworkingMode  = cborSendMessage.NetworkingMode;
                }

            }

            return sentMessageResult ?? SentMessageResult.UnknownClient();

        }

        #endregion



        #region ProcessCBORMessage   (MessageTimestamp, WebSocketConnection, CBORMessage,   EventTrackingId, CancellationToken)

        /// <summary>
        /// Process a binary frame of a connection that agreed on a CBOR subprotocol.
        ///
        /// The frame is the CBOR array of an OCPP message. Its payload is read by the class
        /// of the message and written as the JSON of that class, and what comes of it is
        /// processed as if it had arrived as JSON: by the same processors, with the same
        /// events, forwarded the same way. What is answered leaves as CBOR again, since the
        /// connection it leaves over speaks nothing else.
        /// </summary>
        /// <param name="MessageTimestamp">When the frame arrived.</param>
        /// <param name="WebSocketConnection">The connection it arrived over.</param>
        /// <param name="sourceNodeId">The networking node at the far end of that connection.</param>
        /// <param name="CBORMessage">The bytes of the frame.</param>
        /// <param name="EventTrackingId">An event tracking identification.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task ProcessCBORMessage(DateTimeOffset        MessageTimestamp,
                                             IMessageConnection    WebSocketConnection,
                                             NetworkingNode_Id?    sourceNodeId,
                                             Byte[]                CBORMessage,
                                             EventTracking_Id      EventTrackingId,
                                             CancellationToken     CancellationToken)
        {

            try
            {

                // Without a request identification there is nobody to tell.
                if (!OCPP_CBORFrame.TryParse(CBORMessage, out var cborArray, out var frameError))
                {
                    DebugX.Log($"{nameof(OCPPCBOREdge)}: A binary frame of a CBOR connection could not be read: {frameError}");
                    return;
                }

                if (!OCPP_CBORFrame.TrySplit(cborArray, out var jsonMessage, out var last, out var splitError))
                {
                    DebugX.Log($"{nameof(OCPPCBOREdge)}: A CBOR array that is no OCPP message was received: {splitError}");
                    return;
                }

                JObject? payload;

                #region A request

                if      (OCPP_CBORRequestMessage.      TryParse(cborArray, out var cborRequestMessage,       out var requestParsingError,  MessageTimestamp, null, EventTrackingId, sourceNodeId, CancellationToken))
                {

                    // A byte string is a message of the binary format: the bytes its
                    // class wrote, and reads again.
                    if (cborRequestMessage.Payload.TryGetBytes(out var binaryRequest))
                    {

                        // An answer in JSON to it is carried as what that JSON says.
                        RememberCBORRequestReceived(
                            WebSocketConnection,
                            cborRequestMessage.RequestId,
                            cborRequestMessage.Action,
                            null,
                            cborRequestMessage.RequestTimeout,
                            sourceNodeId
                        );

                        await Host.ProcessBinaryFormatMessage(
                                  MessageTimestamp,
                                  WebSocketConnection,
                                  sourceNodeId,
                                  new OCPP_BinaryRequestMessage(
                                      MessageTimestamp,
                                      EventTrackingId,
                                      cborRequestMessage.NetworkingMode,
                                      cborRequestMessage.Destination,
                                      cborRequestMessage.NetworkPath,
                                      cborRequestMessage.RequestId,
                                      cborRequestMessage.Action,
                                      binaryRequest,
                                      cborRequestMessage.RequestTimeout
                                  ).ToByteArray(),
                                  EventTrackingId,
                                  CancellationToken
                              );

                        return;

                    }

                    if (!Translation.TryRequestToJSON(
                             cborRequestMessage.Action,
                             cborRequestMessage.Payload,
                             cborRequestMessage.RequestId,
                             cborRequestMessage.Destination,
                             cborRequestMessage.NetworkPath,
                             out payload,
                             out var request,
                             out var requestError
                         ))
                    {

                        // Back to whoever asked, so that it does not wait out its timeout.
                        await Host.SendCBORRequestError(
                                  new OCPP_CBORRequestErrorMessage(
                                      Timestamp.Now,
                                      EventTracking_Id.New,
                                      NetworkingMode.Unknown,
                                      SourceRouting.To(cborRequestMessage.NetworkPath.Source),
                                      NetworkPath.From(Host.Id),
                                      cborRequestMessage.RequestId,
                                      Translation.KnowsRequest(cborRequestMessage.Action)
                                          ? ResultCode.FormationViolation
                                          : ResultCode.NotImplemented,
                                      requestError
                                  ),
                                  WebSocketConnection
                              );

                        return;

                    }

                    // Its response is written together with it.
                    RememberCBORRequestReceived(
                        WebSocketConnection,
                        cborRequestMessage.RequestId,
                        cborRequestMessage.Action,
                        request,
                        cborRequestMessage.RequestTimeout,
                        sourceNodeId
                    );

                }

                #endregion

                #region A response

                else if (OCPP_CBORResponseMessage.     TryParse(cborArray, out var cborResponseMessage,      out var responseParsingError, sourceNodeId))
                {

                    // A byte string is a response of the binary format.
                    if (cborResponseMessage.Payload.TryGetBytes(out var binaryResponse))
                    {

                        // The request is answered with this.
                        TryTakeCBORRequestSent(cborResponseMessage.RequestId, out _);

                        await Host.ProcessBinaryFormatMessage(
                                  MessageTimestamp,
                                  WebSocketConnection,
                                  sourceNodeId,
                                  new OCPP_BinaryResponseMessage(
                                      MessageTimestamp,
                                      EventTrackingId,
                                      cborResponseMessage.NetworkingMode,
                                      cborResponseMessage.Destination,
                                      cborResponseMessage.NetworkPath,
                                      cborResponseMessage.RequestId,
                                      binaryResponse
                                  ).ToByteArray(),
                                  EventTrackingId,
                                  CancellationToken
                              );

                        return;

                    }

                    // A response is read together with its request, and nobody but the
                    // node that sent that request as CBOR knows which one it was.
                    if (!TryTakeCBORRequestSent(cborResponseMessage.RequestId, out var asked))
                    {
                        DebugX.Log($"{nameof(OCPPCBOREdge)}: A CBOR response to the request '{cborResponseMessage.RequestId}' was received, which {Host.Id} never sent as CBOR.");
                        return;
                    }

                    if (!Translation.TryResponseToJSON(
                             asked.Action,
                             asked.Request,
                             cborResponseMessage.Payload,
                             cborResponseMessage.RequestId,
                             cborResponseMessage.Destination,
                             cborResponseMessage.NetworkPath,
                             out payload,
                             out var responseError
                         ))
                    {

                        // Whoever waits for it here learns it at once...
                        Host.ReceiveJSONRequestError(
                            new OCPP_JSONRequestErrorMessage(
                                Timestamp.Now,
                                EventTrackingId,
                                NetworkingMode.Unknown,
                                SourceRouting.To(Host.Id),
                                cborResponseMessage.NetworkPath,
                                cborResponseMessage.RequestId,
                                ResultCode.FormationViolation,
                                $"The answer cannot be read: {responseError}"
                            ),
                            WebSocketConnection
                        );

                        // ...and so does whoever answered.
                        await Host.TellTheAnswererItCannotBeRead(
                                  WebSocketConnection,
                                  cborResponseMessage.NetworkPath,
                                  cborResponseMessage.RequestId,
                                  EventTrackingId,
                                  Binary: false,
                                  responseError,
                                  CancellationToken
                              );

                        return;

                    }

                }

                #endregion

                #region A request error

                else if (OCPP_CBORRequestErrorMessage. TryParse(cborArray, out var cborRequestErrorMessage,  out var requestErrorError,    sourceNodeId))
                {

                    // The request is answered with this.
                    TryTakeCBORRequestSent(cborRequestErrorMessage.RequestId, out _);

                    payload = CBORJSON.ToJSON(last) as JObject ?? [];

                }

                #endregion

                #region A response error

                else if (OCPP_CBORResponseErrorMessage.TryParse(cborArray, out var cborResponseErrorMessage, out var responseErrorError,   sourceNodeId))
                    payload = CBORJSON.ToJSON(last) as JObject ?? [];

                #endregion

                #region A message without an answer

                else if (OCPP_CBORSendMessage.         TryParse(cborArray, out var cborSendMessage,          out var sendParsingError,     MessageTimestamp, EventTrackingId, sourceNodeId, CancellationToken))
                {

                    // A byte string is a message of the binary format.
                    if (cborSendMessage.Payload.TryGetBytes(out var binaryMessage))
                    {

                        await Host.ProcessBinaryFormatMessage(
                                  MessageTimestamp,
                                  WebSocketConnection,
                                  sourceNodeId,
                                  new OCPP_BinarySendMessage(
                                      MessageTimestamp,
                                      EventTrackingId,
                                      cborSendMessage.NetworkingMode,
                                      cborSendMessage.Destination,
                                      cborSendMessage.NetworkPath,
                                      cborSendMessage.MessageId,
                                      cborSendMessage.Action,
                                      binaryMessage
                                  ).ToByteArray(),
                                  EventTrackingId,
                                  CancellationToken
                              );

                        return;

                    }

                    if (!Translation.TryMessageToJSON(
                             cborSendMessage.Action,
                             cborSendMessage.Payload,
                             cborSendMessage.MessageId,
                             cborSendMessage.Destination,
                             cborSendMessage.NetworkPath,
                             out payload,
                             out var messageError
                         ))
                    {
                        DebugX.Log($"{nameof(OCPPCBOREdge)}: The CBOR message '{cborSendMessage.Action}' could not be read: {messageError}");
                        return;
                    }

                }

                #endregion

                else
                {
                    DebugX.Log($"{nameof(OCPPCBOREdge)}: A CBOR array that is no OCPP message was received: {requestParsingError ?? responseParsingError ?? requestErrorError ?? responseErrorError ?? sendParsingError ?? cborArray.ToDiagnosticString()}");
                    return;
                }

                // What stood in front of the payload is the JSON array already, with an empty
                // object where the payload belongs.
                jsonMessage[jsonMessage.Count - 1] = payload;

                await Host.ProcessJSONMessage(
                          MessageTimestamp,
                          WebSocketConnection,
                          sourceNodeId,
                          jsonMessage,
                          EventTrackingId,
                          CancellationToken
                      );

            }
            catch (Exception e)
            {
                await Host.HandleErrors(
                          nameof(ProcessCBORMessage),
                          e
                      );
            }

        }

        #endregion

    }

}
