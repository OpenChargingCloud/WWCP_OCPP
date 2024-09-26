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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// Extension methods for all Central System nodes.
    /// </summary>
    public static class ICentralSystemNodeExtensions
    {

        #region Reset                       (Destination, ResetType, ...)

        /// <summary>
        /// Reset the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The charge point/networking node identification.</param>
        /// <param name="ResetType">The type of reset that the charge point should perform.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<ResetResponse>

            Reset(this ICentralSystemNode  CentralSystem,
                  SourceRouting            Destination,
                  ResetType                ResetType,

                  IEnumerable<KeyPair>?    SignKeys              = null,
                  IEnumerable<SignInfo>?   SignInfos             = null,
                  IEnumerable<Signature>?  Signatures            = null,

                  CustomData?              CustomData            = null,

                  Request_Id?              RequestId             = null,
                  DateTime?                RequestTimestamp      = null,
                  TimeSpan?                RequestTimeout        = null,
                  EventTracking_Id?        EventTrackingId       = null,
                  SerializationFormats?    SerializationFormat   = null,
                  CancellationToken        CancellationToken     = default)

                => CentralSystem.OCPP.OUT.Reset(
                       new ResetRequest(
                           Destination,
                           ResetType,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region ChangeAvailability          (Destination, ConnectorId, Availability, ...)

        /// <summary>
        /// Change the availability of the given charge point.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<ChangeAvailabilityResponse>

            ChangeAvailability(this ICentralSystemNode  CentralSystem,
                               SourceRouting            Destination,
                               Connector_Id             ConnectorId,
                               Availabilities           Availability,

                               IEnumerable<KeyPair>?    SignKeys              = null,
                               IEnumerable<SignInfo>?   SignInfos             = null,
                               IEnumerable<Signature>?  Signatures            = null,

                               CustomData?              CustomData            = null,

                               Request_Id?              RequestId             = null,
                               DateTime?                RequestTimestamp      = null,
                               TimeSpan?                RequestTimeout        = null,
                               EventTracking_Id?        EventTrackingId       = null,
                               SerializationFormats?    SerializationFormat   = null,
                               CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.ChangeAvailability(
                       new ChangeAvailabilityRequest(
                           Destination,
                           ConnectorId,
                           Availability,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion


        #region GetConfiguration            (Destination, Keys = null, ...)

        /// <summary>
        /// Get the configuration of the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<GetConfigurationResponse>

            GetConfiguration(this ICentralSystemNode  CentralSystem,
                             SourceRouting            Destination,
                             IEnumerable<String>?     Keys                  = null,

                             IEnumerable<KeyPair>?    SignKeys              = null,
                             IEnumerable<SignInfo>?   SignInfos             = null,
                             IEnumerable<Signature>?  Signatures            = null,

                             CustomData?              CustomData            = null,

                             Request_Id?              RequestId             = null,
                             DateTime?                RequestTimestamp      = null,
                             TimeSpan?                RequestTimeout        = null,
                             EventTracking_Id?        EventTrackingId       = null,
                             SerializationFormats?    SerializationFormat   = null,
                             CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.GetConfiguration(
                       new GetConfigurationRequest(
                           Destination,
                           Keys,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion

        #region GetConfiguration            (Destination, Key, Value, ...)

        /// <summary>
        /// Change the configuration of the given charge point/networking node.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<ChangeConfigurationResponse>

            ChangeConfiguration(this ICentralSystemNode  CentralSystem,
                                SourceRouting            Destination,
                                String                   Key,
                                String                   Value,

                                IEnumerable<KeyPair>?    SignKeys              = null,
                                IEnumerable<SignInfo>?   SignInfos             = null,
                                IEnumerable<Signature>?  Signatures            = null,

                                CustomData?              CustomData            = null,

                                Request_Id?              RequestId             = null,
                                DateTime?                RequestTimestamp      = null,
                                TimeSpan?                RequestTimeout        = null,
                                EventTracking_Id?        EventTrackingId       = null,
                                SerializationFormats?    SerializationFormat   = null,
                                CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.ChangeConfiguration(
                       new ChangeConfigurationRequest(
                           Destination,
                           Key,
                           Value,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion



        #region TransferData                (Destination, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given data to the given charge point.
        /// </summary>
        /// <param name="CentralSystem">The central system.</param>
        /// <param name="Destination">The networking node identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DataTransferResponse>

            TransferData(this ICentralSystemNode  CentralSystem,
                         SourceRouting            Destination,
                         Vendor_Id                VendorId,
                         Message_Id?              MessageId             = null,
                         JToken?                  Data                  = null,

                         IEnumerable<KeyPair>?    SignKeys              = null,
                         IEnumerable<SignInfo>?   SignInfos             = null,
                         IEnumerable<Signature>?  Signatures            = null,

                         Request_Id?              RequestId             = null,
                         DateTime?                RequestTimestamp      = null,
                         TimeSpan?                RequestTimeout        = null,
                         EventTracking_Id?        EventTrackingId       = null,
                         SerializationFormats?    SerializationFormat   = null,
                         CancellationToken        CancellationToken     = default)


                => CentralSystem.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(
                           Destination,
                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? CentralSystem.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? CentralSystem.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(CentralSystem.Id),
                           SerializationFormat,
                           CancellationToken
                       )
                   );

        #endregion


    }

}
