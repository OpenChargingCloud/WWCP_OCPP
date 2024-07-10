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

using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NN;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public abstract class ANetworkingNode : IBaseNetworkingNode
    {

        #region Data

        protected        readonly  HashSet<OCPPWebSocketServer>  ocppWebSocketServers            = [];
        protected        readonly  List<OCPPWebSocketClient>     ocppWebSocketClients            = [];

        //private                  readonly  ConcurrentDictionary<NetworkingNode_Id, Tuple<CSMS.ICSMSChannel, DateTime>>   connectedNetworkingNodes     = [];
        protected        readonly  ConcurrentDictionary<NetworkingNode_Id, NetworkingNode_Id>                    reachableViaNetworkingHubs   = [];

//        

        /// <summary>
        /// The default time span between maintenance tasks.
        /// </summary>
        protected static readonly  TimeSpan                      DefaultMaintenanceEvery      = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => Id.ToString();

        /// <summary>
        /// The unique identification of this networking node.
        /// </summary>
        public NetworkingNode_Id           Id                         { get; }

        /// <summary>
        /// An optional multi-language networking node description.
        /// </summary>
        [Optional]
        public I18NString?                 Description                { get; }



        public CustomData                  CustomData                 { get; }


        /// <summary>
        /// Disable all maintenance tasks.
        /// </summary>
        public Boolean                     DisableMaintenanceTasks    { get; set; }

        /// <summary>
        /// The maintenance interval.
        /// </summary>
        public TimeSpan                    MaintenanceEvery           { get; }      = DefaultMaintenanceEvery;


        public DNSClient                   DNSClient                  { get; }
        public IOCPPAdapter                OCPP                       { get; }






        public String? ClientCloseMessage
            => "Bye!";


        public IEnumerable<OCPPWebSocketClient> OCPPWebSocketClients
            => ocppWebSocketClients;

        public IEnumerable<OCPPWebSocketServer> OCPPWebSocketServers
            => ocppWebSocketServers;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public ANetworkingNode(NetworkingNode_Id  Id,
                               I18NString?        Description                 = null,

                               SignaturePolicy?   SignaturePolicy             = null,
                               SignaturePolicy?   ForwardingSignaturePolicy   = null,

                               Boolean            DisableSendHeartbeats       = false,
                               TimeSpan?          SendHeartbeatsEvery         = null,
                               TimeSpan?          DefaultRequestTimeout       = null,

                               Boolean            DisableMaintenanceTasks     = false,
                               TimeSpan?          MaintenanceEvery            = null,
                               DNSClient?         DNSClient                   = null)

        {

            if (Id.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id),          "The given networking node identification must not be null or empty!");

            this.Id                       = Id;
            this.Description              = Description;
            this.CustomData               = CustomData       ?? new CustomData(Vendor_Id.GraphDefined);

            this.DisableMaintenanceTasks  = DisableMaintenanceTasks;
            this.MaintenanceEvery         = MaintenanceEvery ?? DefaultMaintenanceEvery;
            this.DNSClient                = DNSClient        ?? new DNSClient(SearchForIPv6DNSServers: false);

            this.OCPP                     = new OCPPAdapter(
                                                this,
                                                DisableSendHeartbeats,
                                                SendHeartbeatsEvery,
                                                DefaultRequestTimeout,
                                                SignaturePolicy,
                                                ForwardingSignaturePolicy
                                            );

        }

        #endregion



        public Byte[] GetEncryptionKey(NetworkingNode_Id  DestinationNodeId,
                                       UInt16?            KeyId   = null)
        {
            return "5a733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12".HexStringToByteArray();
        }

        public Byte[] GetDecryptionKey(NetworkingNode_Id  SourceNodeId,
                                       UInt16?            KeyId   = null)
        {
            return "5a733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12".HexStringToByteArray();
        }


        public UInt64 GetEncryptionNonce(NetworkingNode_Id  DestinationNodeId,
                                         UInt16?            KeyId   = null)
        {
            return 1;
        }

        public UInt64 GetEncryptionCounter(NetworkingNode_Id  DestinationNodeId,
                                           UInt16?            KeyId   = null)
        {
            return 1;
        }



        #region (virtual) HandleErrors(Module, Caller, ErrorResponse)

        public virtual Task HandleErrors(String  Module,
                                         String  Caller,
                                         String  ErrorResponse)
        {

            return Task.CompletedTask;

        }

        #endregion

        #region (virtual) HandleErrors(Module, Caller, ExceptionOccured)

        public virtual Task HandleErrors(String     Module,
                                         String     Caller,
                                         Exception  ExceptionOccured)
        {

            return Task.CompletedTask;

        }

        #endregion


    }

}
