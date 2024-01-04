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

using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{


    public delegate Task OnJSONMessageRequestSentDelegate   (DateTime                    Timestamp,
                                                             IOCPPWebSocketAdapterOUT    Server,
                                                             OCPP_JSONRequestMessage     JSONRequestMessage);

    public delegate Task OnJSONMessageResponseSentDelegate  (DateTime                    Timestamp,
                                                             IOCPPWebSocketAdapterOUT    Server,
                                                             OCPP_JSONResponseMessage    JSONResponseMessage);

    public delegate Task OnJSONErrorMessageSentDelegate     (DateTime                    Timestamp,
                                                             IOCPPWebSocketAdapterOUT    Server,
                                                             OCPP_JSONErrorMessage       JSONErrorMessage);



    public delegate Task OnBinaryMessageRequestSentDelegate (DateTime                    Timestamp,
                                                             IOCPPWebSocketAdapterOUT    Server,
                                                             OCPP_BinaryRequestMessage   BinaryRequestMessage);

    public delegate Task OnBinaryMessageResponseSentDelegate(DateTime                    Timestamp,
                                                             IOCPPWebSocketAdapterOUT    Server,
                                                             OCPP_BinaryResponseMessage  BinaryResponseMessage);

    //public delegate Task OnBinaryErrorMessageSentDelegate   (DateTime                    Timestamp,
    //                                                         IOCPPWebSocketAdapterOUT    Server,
    //                                                         OCPP_BinaryErrorMessage     BinaryErrorMessage);


    /// <summary>
    /// The common interface of all central systems channels.
    /// CSMS might have multiple channels, e.g. a SOAP and a WebSockets channel.
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

        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON request was sent.
        /// </summary>
        event OnJSONMessageRequestSentDelegate?          OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever a JSON response was sent.
        /// </summary>
        event OnJSONMessageResponseSentDelegate?         OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever a JSON error response was sent.
        /// </summary>
        event OnJSONErrorMessageSentDelegate?            OnJSONErrorResponseSent;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary request was sent.
        /// </summary>
        event OnBinaryMessageRequestSentDelegate?        OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever a binary response was sent.
        /// </summary>
        event OnBinaryMessageResponseSentDelegate?       OnBinaryMessageResponseSent;

        ///// <summary>
        ///// An event sent whenever a binary error response was sent.
        ///// </summary>
        //event OnWebSocketBinaryErrorResponseDelegate?    OnBinaryErrorResponseSent;

        #endregion


        Task<DataTransferResponse>           DataTransfer         (          DataTransferRequest           Request);

        Task NotifyJSONMessageResponseSent  (OCPP_JSONResponseMessage   JSONResponseMessage);
        Task NotifyJSONErrorResponseSent    (OCPP_JSONErrorMessage      JSONErrorMessage);
        Task NotifyBinaryMessageResponseSent(OCPP_BinaryResponseMessage BinaryResponseMessage);


    }

}
