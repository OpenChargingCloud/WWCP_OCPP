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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NN
{

    public interface INetworkingNodeIN : //INetworkingNodeIncomingMessages,
                                         INetworkingNodeIncomingMessagesEvents
    {

        #region Reset

        Task RaiseOnResetRequest (DateTime               Timestamp,
                                  IEventSender           Sender,
                                  IWebSocketConnection   Connection,
                                  ResetRequest           Request);

        Task RaiseOnResetResponse(DateTime               Timestamp,
                                  IEventSender           Sender,
                                  IWebSocketConnection   Connection,
                                  ResetRequest           Request,
                                  ResetResponse          Response,
                                  TimeSpan               Runtime);

        #endregion

    }


    public interface INetworkingNodeOUT : IEventSender,

                                          // as CS
                                          INetworkingNodeOutgoingMessages,
                                          INetworkingNodeOutgoingMessagesEvents,

                                          // as CSMS
                                          NetworkingNode.CSMS.INetworkingNodeOutgoingMessagesEvents

    {

        #region Reset

        Task RaiseOnResetRequest (DateTime               Timestamp,
                                  IEventSender           Sender,
                                  ResetRequest           Request);

        Task RaiseOnResetResponse(DateTime               Timestamp,
                                  IEventSender           Sender,
                                  ResetRequest           Request,
                                  ResetResponse          Response,
                                  TimeSpan               Runtime);

        #endregion

    }


    /// <summary>
    /// The common interface of all charging station.
    /// </summary>
    public interface INetworkingNode // as CS
                                     //  INetworkingNodeOutgoingMessages
                                     //  INetworkingNodeOutgoingMessagesEvents,
                                     //  //INetworkingNodeIncomingMessages,
                                     //  INetworkingNodeIncomingMessagesEvents,
                                     //
                                     //  // as CSMS
                                     //  cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS.INetworkingNodeOutgoingMessagesEvents
                                     //  //NetworkingNode.CSMS.INetworkingNodeIncomingMessagesEvents

    {


        #region Custom JSON serializer delegates

        #region Charging Station Messages

        CustomJObjectSerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer    { get; set; }


        #endregion

        #region Data Structures

        CustomJObjectSerializerDelegate<ChargingStation>?          CustomChargingStationSerializer            { get; set; }
        CustomJObjectSerializerDelegate<OCPP.Signature>?           CustomSignatureSerializer                  { get; set; }
        CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                 { get; set; }

        #endregion

        #endregion



        INetworkingNodeIN  IN                       { get; }
        INetworkingNodeOUT OUT                      { get; }


        NetworkingNode_Id  Id                       { get; }

        TimeSpan           DefaultRequestTimeout    { get; }

        Request_Id         NextRequestId            { get; }

        SignaturePolicy?   SignaturePolicy          { get; }


        String             Model                    { get; }
        String             VendorName               { get; }
        String?            SerialNumber             { get; }
        Modem?             Modem                    { get; }
        String?            FirmwareVersion          { get; }

        CustomData         CustomData               { get; }



        String? ClientCloseMessage { get; }


    }

}
