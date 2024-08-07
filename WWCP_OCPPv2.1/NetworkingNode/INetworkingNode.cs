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

using System.Runtime.CompilerServices;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all networking node.
    /// </summary>
    public interface INetworkingNode : IEventSender
    {

        CustomData?                CustomData               { get; }

        OCPPAdapter                OCPP                     { get; }

        new NetworkingNode_Id      Id                       { get; }

        /// <summary>
        /// An optional multi-language networking node description.
        /// </summary>
        [Optional]
        I18NString?                Description              { get; }

        Request_Id                 NextRequestId            { get; }

        //SignaturePolicy?           SignaturePolicy          { get; }


        Byte[]  GetEncryptionKey     (NetworkingNode_Id DestinationId, UInt16? KeyId = null);
        Byte[]  GetDecryptionKey     (NetworkingNode_Id SourceNodeId,  UInt16? KeyId = null);

        UInt64  GetEncryptionNonce   (NetworkingNode_Id DestinationId, UInt16? KeyId = null);
        UInt64  GetEncryptionCounter (NetworkingNode_Id DestinationId, UInt16? KeyId = null);



        String? ClientCloseMessage { get; }



        Task LogEvent<TDelegate>(String                                             OCPPIO,
                                 TDelegate?                                         Logger,
                                 Func<TDelegate, Task>                              LogHandler,
                                 [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                 [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate;


        Task HandleErrors(String     Module,
                          String     Caller,
                          Exception  ExceptionOccured);

        Task HandleErrors(String     Module,
                          String     Caller,
                          String     ErrorResponse);


    }

}
