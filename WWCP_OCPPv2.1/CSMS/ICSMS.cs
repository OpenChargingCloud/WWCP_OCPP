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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all central systems channels.
    /// CSMS might have multiple channels, e.g. a SOAP and a WebSockets channel.
    /// </summary>
    public interface ICSMSChannel : ICSMSClient,
                                    ICSMSServer
    {


    }

    /// <summary>
    /// The common interface of all central systems.
    /// </summary>
    public interface ICSMS : ICSMSClient,
                             ICSMSServerLogger
    {

        CSMS_Id                    Id                       { get; }

        TimeSpan                   DefaultRequestTimeout    { get; }

        Request_Id                 NextRequestId            { get; }


        IEnumerable<ICSMSChannel>  CSMSChannels             { get; }


    }

}
