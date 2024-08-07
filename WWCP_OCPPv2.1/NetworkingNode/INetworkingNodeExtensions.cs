﻿/*
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// Extension methods for all networking nodes.
    /// </summary>
    public static class INetworkingNodeExtensions
    {

        #region TransferData                (DestinationId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="DestinationId">The networking node identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DataTransferResponse>

            TransferData(this INetworkingNode     NetworkingNode,
                         NetworkingNode_Id        DestinationId,
                         Vendor_Id                VendorId,
                         Message_Id?              MessageId           = null,
                         JToken?                  Data                = null,

                         IEnumerable<KeyPair>?    SignKeys            = null,
                         IEnumerable<SignInfo>?   SignInfos           = null,
                         IEnumerable<Signature>?  Signatures          = null,

                         CustomData?              CustomData          = null,

                         Request_Id?              RequestId           = null,
                         DateTime?                RequestTimestamp    = null,
                         TimeSpan?                RequestTimeout      = null,
                         EventTracking_Id?        EventTrackingId     = null,
                         CancellationToken        CancellationToken   = default)


                => NetworkingNode.OCPP.OUT.DataTransfer(
                       new DataTransferRequest(
                           DestinationId,
                           VendorId,
                           MessageId,
                           Data,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion


        // Binary Data Streams Extensions

        #region TransferBinaryData          (DestinationId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="DestinationId">The networking node identification.</param>
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
        public static Task<BinaryDataTransferResponse>

            TransferBinaryData(this INetworkingNode          NetworkingNode,

                               NetworkingNode_Id             DestinationId,
                               Vendor_Id                     VendorId,
                               Message_Id?                   MessageId           = null,
                               Byte[]?                       Data                = null,
                               BinaryFormats?                Format              = null,

                               NetworkPath?                  NetworkPath         = null,

                               IEnumerable<KeyPair>?         SignKeys            = null,
                               IEnumerable<SignInfo>?        SignInfos           = null,
                               IEnumerable<Signature>?       Signatures          = null,

                               Request_Id?                   RequestId           = null,
                               DateTime?                     RequestTimestamp    = null,
                               TimeSpan?                     RequestTimeout      = null,
                               EventTracking_Id?             EventTrackingId     = null,
                               CancellationToken             CancellationToken   = default)


                => NetworkingNode.OCPP.OUT.BinaryDataTransfer(
                       new BinaryDataTransferRequest(
                           DestinationId,
                           VendorId,
                           MessageId,
                           Data,
                           Format,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region GetFile                     (DestinationId, FileName, Priority = null, ...)

        /// <summary>
        /// Request to download the given file from the given networking node.
        /// </summary>
        /// <param name="DestinationId">The networking node identification.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<GetFileResponse>

            GetFile(this INetworkingNode          NetworkingNode,

                    NetworkingNode_Id             DestinationId,
                    FilePath                      FileName,
                    Byte?                         Priority            = null,

                    CustomData?                   CustomData          = null,

                    NetworkPath?                  NetworkPath         = null,

                    IEnumerable<KeyPair>?         SignKeys            = null,
                    IEnumerable<SignInfo>?        SignInfos           = null,
                    IEnumerable<Signature>?       Signatures          = null,

                    Request_Id?                   RequestId           = null,
                    DateTime?                     RequestTimestamp    = null,
                    TimeSpan?                     RequestTimeout      = null,
                    EventTracking_Id?             EventTrackingId     = null,
                    CancellationToken             CancellationToken   = default)


                => NetworkingNode.OCPP.OUT.GetFile(
                       new GetFileRequest(
                           DestinationId,
                           FileName,
                           Priority,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region SendFile                    (DestinationId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Send the given file to the given networking node.
        /// </summary>
        /// <param name="DestinationId">The networking node identification.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="FileContent">The file content.</param>
        /// <param name="FileContentType">An optional content/MIME type of the file.</param>
        /// <param name="FileSHA256">An optional SHA256 hash value of the file content.</param>
        /// <param name="FileSHA512">An optional SHA512 hash value of the file content.</param>
        /// <param name="FileSignatures">An optional enumeration of cryptographic signatures for the file content.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SendFileResponse>

            SendFile(this INetworkingNode          NetworkingNode,

                     NetworkingNode_Id             DestinationId,
                     FilePath                      FileName,
                     Byte[]                        FileContent,
                     ContentType?                  FileContentType     = null,
                     Byte[]?                       FileSHA256          = null,
                     Byte[]?                       FileSHA512          = null,
                     IEnumerable<Signature>?       FileSignatures      = null,
                     Byte?                         Priority            = null,

                     CustomData?                   CustomData          = null,

                     NetworkPath?                  NetworkPath         = null,

                     IEnumerable<KeyPair>?         SignKeys            = null,
                     IEnumerable<SignInfo>?        SignInfos           = null,
                     IEnumerable<Signature>?       Signatures          = null,

                     Request_Id?                   RequestId           = null,
                     DateTime?                     RequestTimestamp    = null,
                     TimeSpan?                     RequestTimeout      = null,
                     EventTracking_Id?             EventTrackingId     = null,
                     CancellationToken             CancellationToken   = default)


                => NetworkingNode.OCPP.OUT.SendFile(
                       new SendFileRequest(
                           DestinationId,
                           FileName,
                           FileContent,
                           FileContentType,
                           FileSHA256,
                           FileSHA512,
                           FileSignatures,
                           Priority,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region DeleteFile                  (DestinationId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// Delete the given file from the given networking node.
        /// </summary>
        /// <param name="DestinationId">The networking node identification.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="FileSHA256">An optional SHA256 hash value of the file content.</param>
        /// <param name="FileSHA512">An optional SHA512 hash value of the file content.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<DeleteFileResponse>

            DeleteFile(this INetworkingNode          NetworkingNode,

                       NetworkingNode_Id             DestinationId,
                       FilePath                      FileName,
                       Byte[]?                       FileSHA256          = null,
                       Byte[]?                       FileSHA512          = null,

                       CustomData?                   CustomData          = null,

                       NetworkPath?                  NetworkPath         = null,

                       IEnumerable<KeyPair>?         SignKeys            = null,
                       IEnumerable<SignInfo>?        SignInfos           = null,
                       IEnumerable<Signature>?       Signatures          = null,

                       Request_Id?                   RequestId           = null,
                       DateTime?                     RequestTimestamp    = null,
                       TimeSpan?                     RequestTimeout      = null,
                       EventTracking_Id?             EventTrackingId     = null,
                       CancellationToken             CancellationToken   = default)


                => NetworkingNode.OCPP.OUT.DeleteFile(
                       new DeleteFileRequest(
                           DestinationId,
                           FileName,
                           FileSHA256,
                           FileSHA512,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion

        #region ListDirectory               (DestinationId, Filename, FileContent, FileContentType = null, ...)

        /// <summary>
        /// List the given directory of the given networking node.
        /// </summary>
        /// <param name="DestinationId">The networking node identification.</param>
        /// <param name="DirectoryPath">The absolute path of the directory to list.</param>
        /// <param name="Format">The optional response format of the directory listing.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<ListDirectoryResponse>

            ListDirectory(this INetworkingNode          NetworkingNode,

                          NetworkingNode_Id             DestinationId,
                          FilePath                      DirectoryPath,
                          ListDirectoryFormat?          Format                 = null,
                          Boolean?                      WithFileSizes          = null,
                          Boolean?                      WithFileDates          = null,
                          Boolean?                      WithSHA256FileHashes   = null,
                          Boolean?                      WithSHA512FileHashes   = null,

                          CustomData?                   CustomData             = null,

                          NetworkPath?                  NetworkPath            = null,

                          IEnumerable<KeyPair>?         SignKeys               = null,
                          IEnumerable<SignInfo>?        SignInfos              = null,
                          IEnumerable<Signature>?       Signatures             = null,

                          Request_Id?                   RequestId              = null,
                          DateTime?                     RequestTimestamp       = null,
                          TimeSpan?                     RequestTimeout         = null,
                          EventTracking_Id?             EventTrackingId        = null,
                          CancellationToken             CancellationToken      = default)


                => NetworkingNode.OCPP.OUT.ListDirectory(
                       new ListDirectoryRequest(
                           DestinationId,
                           DirectoryPath,
                           Format,
                           WithFileSizes,
                           WithFileDates,
                           WithSHA256FileHashes,
                           WithSHA512FileHashes,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken
                       )
                   );

        #endregion


        // E2E Security Extensions

        #region TransferSecureData          (DestinationId, Parameter, Payload, KeyId = null, Key = null, Nonce = null, Counter = null, ...)

        /// <summary>
        /// Transfer the given binary data to the given charging station.
        /// </summary>
        /// <param name="DestinationId">The networking node identification.</param>
        /// <param name="Parameter">Encryption parameters.</param>
        /// <param name="KeyId">The unique identification of the encryption key.</param>
        /// <param name="Payload">The unencrypted encapsulated security payload.</param>
        /// <param name="Key"></param>
        /// <param name="Nonce"></param>
        /// <param name="Counter"></param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SecureDataTransferResponse>

            TransferSecureData(this INetworkingNode     NetworkingNode,
                               NetworkingNode_Id        DestinationId,
                               UInt16                   Parameter,
                               Byte[]                   Payload,
                               UInt16?                  KeyId               = null,
                               Byte[]?                  Key                 = null,
                               UInt64?                  Nonce               = null,
                               UInt64?                  Counter             = null,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<Signature>?  Signatures          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               CancellationToken        CancellationToken   = default)

        {

            try
            {

                return NetworkingNode.OCPP.OUT.SecureDataTransfer(
                           SecureDataTransferRequest.Encrypt(
                               DestinationId,
                               Parameter,
                               KeyId   ?? 0,
                               Key     ?? NetworkingNode.GetEncryptionKey    (DestinationId, KeyId),
                               Nonce   ?? NetworkingNode.GetEncryptionNonce  (DestinationId, KeyId),
                               Counter ?? NetworkingNode.GetEncryptionCounter(DestinationId, KeyId),
                               Payload,

                               SignKeys,
                               SignInfos,
                               Signatures,

                               RequestId        ?? NetworkingNode.NextRequestId,
                               RequestTimestamp ?? Timestamp.Now,
                               RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                               EventTrackingId  ?? EventTracking_Id.New,
                               NetworkPath.From(NetworkingNode.Id),
                               CancellationToken
                           )
                       );

            }
            catch (Exception e)
            {
                return Task.FromResult(
                           SecureDataTransferResponse.ExceptionOccured(null, e)
                       );
            }

        }

        #endregion



        // Overlay Networking Extensions

        #region NotifyNetworkTopology                 (NetworkingNode, ...)

        /// <summary>
        /// Transfer the given binary data to the CSMS.
        /// </summary>
        /// <param name="NetworkTopologyInformation">A network topology information.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<NotifyNetworkTopologyResponse>

            NotifyNetworkTopology(this INetworkingNode          NetworkingNode,

                                  NetworkTopologyInformation    NetworkTopologyInformation,

                                  CustomData?                   CustomData          = null,

                                  NetworkingNode_Id?            DestinationId   = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<Signature>?       Signatures          = null,

                                  Request_Id?                   RequestId           = null,
                                  DateTime?                     RequestTimestamp    = null,
                                  TimeSpan?                     RequestTimeout      = null,
                                  EventTracking_Id?             EventTrackingId     = null,
                                  CancellationToken             CancellationToken   = default)


                => NetworkingNode.OCPP.OUT.NotifyNetworkTopology(
                       new NotifyNetworkTopologyRequest(

                           DestinationId ?? NetworkingNode_Id.CSMS,

                           NetworkTopologyInformation,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           CancellationToken

                       )
                   );

        #endregion

        #region NotifyNetworkTopology                 (NetworkingNode, ...)

        ///// <summary>
        ///// Transfer the given binary data to the CSMS.
        ///// </summary>
        ///// <param name="NetworkTopologyInformation">A network topology information.</param>
        ///// 
        ///// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        ///// 
        ///// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        ///// 
        ///// <param name="RequestId">An optional request identification.</param>
        ///// <param name="RequestTimestamp">An optional request timestamp.</param>
        ///// <param name="RequestTimeout">An optional timeout for this request.</param>
        ///// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        ///// <param name="CancellationToken">An optional token to cancel this request.</param>
        //public static Task<OCPP.NN.NotifyNetworkTopologyResponse>

        //    NotifyNetworkTopology(this INetworkingNode2         NetworkingNode,

        //                          NetworkingNode_Id             DestinationId,
        //                          NetworkTopologyInformation    NetworkTopologyInformation,

        //                          CustomData?                   CustomData          = null,

        //                          IEnumerable<KeyPair>?         SignKeys            = null,
        //                          IEnumerable<SignInfo>?        SignInfos           = null,
        //                          IEnumerable<Signature>?       Signatures          = null,

        //                          Request_Id?                   RequestId           = null,
        //                          DateTime?                     RequestTimestamp    = null,
        //                          TimeSpan?                     RequestTimeout      = null,
        //                          EventTracking_Id?             EventTrackingId     = null,
        //                          CancellationToken             CancellationToken   = default)


        //        => NetworkingNode.ocppOUT.NotifyNetworkTopology(
        //               new OCPP.NN.NotifyNetworkTopologyRequest(

        //                   DestinationId,
        //                   NetworkTopologyInformation,

        //                   SignKeys,
        //                   SignInfos,
        //                   Signatures,

        //                   CustomData,

        //                   RequestId        ?? NetworkingNode.OCPPAdapter.NextRequestId,
        //                   RequestTimestamp ?? Timestamp.Now,
        //                   RequestTimeout   ?? NetworkingNode.OCPP.DefaultRequestTimeout,
        //                   EventTrackingId  ?? EventTracking_Id.New,
        //                   NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
        //                   CancellationToken

        //               )
        //           );

        #endregion



    }

}
