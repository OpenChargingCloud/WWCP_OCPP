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

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPP.CSMS
{

    /// <summary>
    /// The common interface of all CSMS servers.
    /// </summary>
    public interface ICSMSServerRequestResponseEvents : IEventSender
    {

        #region Properties

        /// <summary>
        /// The unique identifications of all connected networking nodes.
        /// </summary>
        IEnumerable<NetworkingNode_Id>  NetworkingNodeIds    { get; }

        #endregion


        #region OnJSONMessage   (-Received/-ResponseSent/-ErrorResponseSent)

        event OnWebSocketJSONMessageRequestDelegate?      OnJSONMessageRequestReceived;

        event OnWebSocketJSONMessageResponseDelegate?     OnJSONMessageResponseSent;

        event OnWebSocketTextErrorResponseDelegate?       OnJSONErrorResponseSent;


        event OnWebSocketJSONMessageRequestDelegate?      OnJSONMessageRequestSent;

        event OnWebSocketJSONMessageResponseDelegate?     OnJSONMessageResponseReceived;

        event OnWebSocketTextErrorResponseDelegate?       OnJSONErrorResponseReceived;

        #endregion

        #region OnBinaryMessage (-Received/-ResponseSent/-ErrorResponseSent)

        event OnWebSocketBinaryMessageRequestDelegate?    OnBinaryMessageRequestReceived;

        event OnWebSocketBinaryMessageResponseDelegate?   OnBinaryMessageResponseSent;

        //event OnWebSocketBinaryErrorResponseDelegate?     OnBinaryErrorResponseSent;


        event OnWebSocketBinaryMessageRequestDelegate?    OnBinaryMessageRequestSent;

        event OnWebSocketBinaryMessageResponseDelegate?   OnBinaryMessageResponseReceived;

        //event OnWebSocketBinaryErrorResponseDelegate?     OnBinaryErrorResponseReceived;

        #endregion


        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
        event OnIncomingBinaryDataTransferRequestDelegate    OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a firmware status notification request was sent.
        /// </summary>
        event OnIncomingBinaryDataTransferResponseDelegate   OnIncomingBinaryDataTransferResponse;

        #endregion


    }

}
