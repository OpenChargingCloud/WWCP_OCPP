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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region Delegates


    public delegate Task OnJSONRequestMessageSentDelegate       (DateTime                       Timestamp,
                                                                 IOCPPWebSocketAdapterOUT       Server,
                                                                 OCPP_JSONRequestMessage        JSONRequestMessage,
                                                                 SendMessageResult              SendMessageResult);

    public delegate Task OnJSONResponseMessageSentDelegate      (DateTime                       Timestamp,
                                                                 IOCPPWebSocketAdapterOUT       Server,
                                                                 OCPP_JSONResponseMessage       JSONResponseMessage,
                                                                 SendMessageResult              SendMessageResult);

    public delegate Task OnJSONRequestErrorMessageSentDelegate  (DateTime                       Timestamp,
                                                                 IOCPPWebSocketAdapterOUT       Server,
                                                                 OCPP_JSONRequestErrorMessage   JSONRequestErrorMessage,
                                                                 SendMessageResult              SendMessageResult);

    public delegate Task OnJSONResponseErrorMessageSentDelegate (DateTime                       Timestamp,
                                                                 IOCPPWebSocketAdapterOUT       Server,
                                                                 OCPP_JSONResponseErrorMessage  JSONRequestErrorMessage,
                                                                 SendMessageResult              SendMessageResult);



    public delegate Task OnBinaryRequestMessageSentDelegate     (DateTime                       Timestamp,
                                                                 IOCPPWebSocketAdapterOUT       Server,
                                                                 OCPP_BinaryRequestMessage      BinaryRequestMessage,
                                                                 SendMessageResult              SendMessageResult);

    public delegate Task OnBinaryResponseMessageSentDelegate    (DateTime                       Timestamp,
                                                                 IOCPPWebSocketAdapterOUT       Server,
                                                                 OCPP_BinaryResponseMessage     BinaryResponseMessage,
                                                                 SendMessageResult              SendMessageResult);

    //public delegate Task OnBinaryErrorMessageSentDelegate       (DateTime                       Timestamp,
    //                                                             IOCPPWebSocketAdapterOUT       Server,
    //                                                             OCPP_BinaryErrorMessage        BinaryErrorMessage,
    //                                                             SendMessageResult              SendMessageResult);

    #endregion


    /// <summary>
    /// The common interface of all outgoing OCPP messages processors.
    /// </summary>
    public interface IOCPPWebSocketAdapterOUT : OCPP.NN.INetworkingNodeOutgoingMessages,
                                                OCPP.NN.INetworkingNodeOutgoingMessageEvents,

                                             //   OCPP.NN.CSMS.INetworkingNodeOutgoingMessages,
                                             //   OCPP.NN.CSMS.INetworkingNodeOutgoingMessageEvents,

                                                CS.  INetworkingNodeOutgoingMessages,
                                                CS.  INetworkingNodeOutgoingMessageEvents,

                                                CSMS.INetworkingNodeOutgoingMessages,
                                                CSMS.INetworkingNodeOutgoingMessageEvents

    {

        #region Events

        #region JSON   messages sent

        /// <summary>
        /// An event sent whenever a JSON request message was sent.
        /// </summary>
        event OnJSONRequestMessageSentDelegate?          OnJSONRequestMessageSent;

        /// <summary>
        /// An event sent whenever a JSON response message was sent.
        /// </summary>
        event OnJSONResponseMessageSentDelegate?         OnJSONResponseMessageSent;

        /// <summary>
        /// An event sent whenever a JSON request error was sent.
        /// </summary>
        event OnJSONRequestErrorMessageSentDelegate?     OnJSONRequestErrorMessageSent;

        /// <summary>
        /// An event sent whenever a JSON response error was sent.
        /// </summary>
        event OnJSONResponseErrorMessageSentDelegate?    OnJSONResponseErrorMessageSent;

        #endregion

        #region Binary messages sent

        /// <summary>
        /// An event sent whenever a binary request was sent.
        /// </summary>
        event OnBinaryRequestMessageSentDelegate?        OnBinaryRequestMessageSent;

        /// <summary>
        /// An event sent whenever a binary response was sent.
        /// </summary>
        event OnBinaryResponseMessageSentDelegate?       OnBinaryResponseMessageSent;

        ///// <summary>
        ///// An event sent whenever a binary error response was sent.
        ///// </summary>
        //event OnWebSocketBinaryErrorResponseDelegate?    OnBinaryErrorResponseSent;

        #endregion



        event OnDataTransferResponseSentDelegate?  OnDataTransferResponseSent;


        #endregion


        Task<SendMessageResult> SendJSONRequest       (OCPP_JSONRequestMessage        JSONRequestMessage);
        Task<SendMessageResult> SendJSONResponse      (OCPP_JSONResponseMessage       JSONResponseMessage);
        Task<SendMessageResult> SendJSONRequestError  (OCPP_JSONRequestErrorMessage   JSONRequestErrorMessage);
        Task<SendMessageResult> SendJSONResponseError (OCPP_JSONResponseErrorMessage  JSONResponseErrorMessage);

        Task<SendMessageResult> SendBinaryRequest     (OCPP_BinaryRequestMessage      BinaryRequestMessage);
        Task<SendMessageResult> SendBinaryResponse    (OCPP_BinaryResponseMessage     BinaryResponseMessage);


        Task NotifyJSONMessageResponseSent   (OCPP_JSONResponseMessage      JSONResponseMessage,      SendMessageResult SendMessageResult);
        Task NotifyJSONRequestErrorSent      (OCPP_JSONRequestErrorMessage  JSONRequestErrorMessage,  SendMessageResult SendMessageResult);
        Task NotifyJSONResponseErrorSent     (OCPP_JSONResponseErrorMessage JSONResponseErrorMessage, SendMessageResult SendMessageResult);

        Task NotifyBinaryMessageResponseSent (OCPP_BinaryResponseMessage    BinaryResponseMessage,    SendMessageResult SendMessageResult);




        Task<DataTransferResponse>           DataTransfer         (          DataTransferRequest           Request);



    }

}
