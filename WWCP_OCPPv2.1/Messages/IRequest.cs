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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The common interface of all request messages.
    /// </summary>
    public interface IRequest : ISignableMessage
    {

        /// <summary>
        /// The charging station identification.
        /// </summary>
        [Mandatory]
        ChargingStation_Id  ChargingStationId    { get; }

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        JSONLDContext       Context              { get; }

        /// <summary>
        /// The request identification.
        /// </summary>
        [Mandatory]
        Request_Id          RequestId            { get; }

        /// <summary>
        /// The timestamp of the request message creation.
        /// </summary>
        [Mandatory]
        DateTime            RequestTimestamp     { get; }

        /// <summary>
        /// The timeout of this request.
        /// </summary>
        [Mandatory]
        TimeSpan            RequestTimeout       { get; }

        /// <summary>
        /// An event tracking identification for correlating this request with other events.
        /// </summary>
        [Mandatory]
        EventTracking_Id    EventTrackingId      { get; }

        /// <summary>
        /// The OCPP HTTP Web Socket action.
        /// </summary>
        [Mandatory]
        String              Action               { get; }

        /// <summary>
        /// The custom data object to allow to store any kind of customer specific data.
        /// </summary>
        [Optional]
        CustomData?         CustomData           { get; }

        /// <summary>
        /// An optional token to cancel this request.
        /// </summary>
        CancellationToken   CancellationToken    { get; }

    }

}
