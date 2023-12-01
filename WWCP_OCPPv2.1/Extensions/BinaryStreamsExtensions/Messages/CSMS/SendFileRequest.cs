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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A send file request.
    /// </summary>
    public class SendFileRequest : ARequest<SendFileRequest>,
                                   IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/sendFileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The name of the file including its absolute path.
        /// </summary>
        [Mandatory]
        public FilePath                FileName           { get; }

        /// <summary>
        /// The file content.
        /// </summary>
        [Mandatory]
        public Byte[]                  FileContent        { get; }

        /// <summary>
        /// The content/MIME type of the file.
        /// </summary>
        [Mandatory]
        public ContentType             FileContentType    { get; }

        /// <summary>
        /// The optional SHA256 hash value of the file content.
        /// </summary>
        [Optional]
        public Byte[]                  FileSHA256         { get; }

        /// <summary>
        /// The optional SHA512 hash value of the file content.
        /// </summary>
        [Optional]
        public Byte[]                  FileSHA512         { get; }

        /// <summary>
        /// The optional enumeration of cryptographic signatures for the file content.
        /// </summary>
        [Optional]
        public IEnumerable<Signature>  FileSignatures     { get; }

        /// <summary>
        /// The optional priority of the file request.
        /// </summary>
        [Optional]
        public Byte?                   Priority           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SendFile request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="FileContent">The file content.</param>
        /// <param name="FileContentType">An optional content/MIME type of the file.</param>
        /// <param name="FileSHA256">An optional SHA256 hash value of the file content.</param>
        /// <param name="FileSHA512">An optional SHA512 hash value of the file content.</param>
        /// <param name="FileSignatures">An optional enumeration of cryptographic signatures for the file content.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SendFileRequest(NetworkingNode_Id        NetworkingNodeId,
                               FilePath                 FileName,
                               Byte[]                   FileContent,
                               ContentType?             FileContentType     = null,
                               Byte[]?                  FileSHA256          = null,
                               Byte[]?                  FileSHA512          = null,
                               IEnumerable<Signature>?  FileSignatures      = null,
                               Byte?                    Priority            = null,

                               IEnumerable<KeyPair>?    SignKeys            = null,
                               IEnumerable<SignInfo>?   SignInfos           = null,
                               IEnumerable<Signature>?  Signatures          = null,

                               CustomData?              CustomData          = null,

                               Request_Id?              RequestId           = null,
                               DateTime?                RequestTimestamp    = null,
                               TimeSpan?                RequestTimeout      = null,
                               EventTracking_Id?        EventTrackingId     = null,
                               NetworkPath?             NetworkPath         = null,
                               CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(SendFileRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.FileName         = FileName;
            this.FileContent      = FileContent;
            this.FileContentType  = FileContentType            ?? ContentType.Application.OctetStream;
            this.FileSHA256       = FileSHA256                 ?? [];
            this.FileSHA512       = FileSHA512                 ?? [];
            this.FileSignatures   = FileSignatures?.Distinct() ?? Array.Empty<Signature>();
            this.Priority         = Priority;


            unchecked
            {

                hashCode = this.FileName.       GetHashCode()       * 19 ^
                           this.FileContent.    GetHashCode()       * 17 ^
                           this.FileContentType.GetHashCode()       * 13 ^
                           this.FileSHA256.     GetHashCode()       * 11 ^
                           this.FileSHA512.     GetHashCode()       *  7 ^
                           this.FileSignatures. CalcHashCode()      *  5 ^
                          (this.Priority?.      GetHashCode() ?? 0) *  3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Binary, RequestId, NetworkingNodeId, NetworkPath, CustomSendFileRequestParser = null)

        /// <summary>
        /// Parse the given binary representation of a SendFileRequest request.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSendFileRequestParser">A delegate to parse custom SendFileRequest requests.</param>
        public static SendFileRequest Parse(Byte[]                                        Binary,
                                            Request_Id                                    RequestId,
                                            NetworkingNode_Id                             NetworkingNodeId,
                                            NetworkPath                                   NetworkPath,
                                            CustomBinaryParserDelegate<SendFileRequest>?  CustomSendFileRequestParser   = null)
        {


            if (TryParse(Binary,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var sendFileRequest,
                         out var errorResponse,
                         CustomSendFileRequestParser) &&
                sendFileRequest is not null)
            {
                return sendFileRequest;
            }

            throw new ArgumentException("The given binary representation of a SendFile request is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(Binary, RequestId, NetworkingNodeId, NetworkPath, out SendFileRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SendFile request.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SendFileRequest">The parsed SendFileRequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(Byte[]                Binary,
                                       Request_Id            RequestId,
                                       NetworkingNode_Id     NetworkingNodeId,
                                       NetworkPath           NetworkPath,
                                       out SendFileRequest?  SendFileRequest,
                                       out String?           ErrorResponse)

            => TryParse(Binary,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out SendFileRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given binary representation of a SendFile request.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SendFileRequest">The parsed SendFileRequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendFileRequestParser">A delegate to parse custom SendFileRequest requests.</param>
        public static Boolean TryParse(Byte[]                                        Binary,
                                       Request_Id                                    RequestId,
                                       NetworkingNode_Id                             NetworkingNodeId,
                                       NetworkPath                                   NetworkPath,
                                       out SendFileRequest?                          SendFileRequest,
                                       out String?                                   ErrorResponse,
                                       CustomBinaryParserDelegate<SendFileRequest>?  CustomSendFileRequestParser)
        {

            try
            {

                SendFileRequest = null;
                ErrorResponse   = null;

                var stream  = new MemoryStream(Binary);

                #region File name

                var fileNameLength        = stream.ReadUInt64();
                var fileNameText          = stream.ReadBytes(fileNameLength)?.ToUTF8String()?.Trim();
                if (fileNameText is null || fileNameText.IsNullOrEmpty())
                {
                    ErrorResponse = $"The received get file name '{fileNameText}' is invalid!";
                    return false;
                }
                var fileName              = FilePath.TryParse(fileNameText);
                if (!fileName.HasValue)
                {
                    ErrorResponse = $"The received get file name '{fileNameText}' is invalid!";
                    return false;
                }

                #endregion

                #region File content

                var fileContentLength      = stream.ReadUInt64();
                var fileContent            = stream.ReadBytes(fileContentLength);

                #endregion

                #region File content type

                var fileContentTypeLength  = stream.ReadByte();
                if (fileContentTypeLength < 0)
                {
                    ErrorResponse = "The received file content type length is invalid!";
                    return false;
                }
                var fileContentType        = ContentType.TryParse(stream.ReadBytes((UInt64) fileContentTypeLength)?.ToUTF8String() ?? "");

                #endregion

                #region File SHA256

                var fileSHA256Length      = stream.ReadByte();
                if (fileSHA256Length < 0)
                {
                    ErrorResponse = "The received file SHA256 length is invalid!";
                    return false;
                }
                var fileSHA256            = stream.ReadBytes((UInt64) fileSHA256Length);

                #endregion

                #region File SHA512

                var fileSHA512Length      = stream.ReadByte();
                if (fileSHA512Length < 0)
                {
                    ErrorResponse = "The received file SHA512 length is invalid!";
                    return false;
                }
                var fileSHA512            = stream.ReadBytes((UInt64) fileSHA512Length);

                #endregion

                #region File signatures

                var fileSignatures        = new HashSet<Signature>();
                var fileSignaturesCount   = stream.ReadByte();
                for (var i = 0; i < fileSignaturesCount; i++)
                {

                    var signatureLength   = stream.ReadUInt16();
                    var signatureBytes    = stream.ReadBytes((UInt64) signatureLength);

                    if (!Signature.TryParse(signatureBytes, out var signature, out ErrorResponse) ||
                        signature is null)
                    {
                        return false;
                    }

                    fileSignatures.Add(signature);

                }

                #endregion

                #region Signatures

                var signatures            = new HashSet<Signature>();
                var signaturesCount       = stream.ReadByte();
                for (var i = 0; i < signaturesCount; i++)
                {

                    var signatureLength   = stream.ReadUInt16();
                    var signatureBytes    = stream.ReadBytes((UInt64) signatureLength);

                    if (!Signature.TryParse(signatureBytes, out var signature, out ErrorResponse) ||
                        signature is null)
                    {
                        return false;
                    }

                    signatures.Add(signature);

                }

                #endregion

                //ToDo: Check if there are some bytes left!


                SendFileRequest = new SendFileRequest(

                                      NetworkingNodeId,
                                      fileName.Value,
                                      fileContent,
                                      fileContentType,
                                      fileSHA256,
                                      fileSHA512,
                                      fileSignatures,
                                      null, // priority (Not serialized as this is only for the sender!)

                                      null,
                                      null,
                                      signatures,

                                      null, // CustomData

                                      RequestId,
                                      null,
                                      null,
                                      null,
                                      NetworkPath

                                  );


                if (CustomSendFileRequestParser is not null)
                    SendFileRequest = CustomSendFileRequestParser(Binary,
                                                                  SendFileRequest);

                return true;

            }
            catch (Exception e)
            {
                SendFileRequest  = null;
                ErrorResponse    = "The given JSON representation of a SendFile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToBinary(CustomSendFileRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomSendFileRequestSerializer">A delegate to serialize custom SendFileRequest requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="IncludeSignatures">Whether to include the digital signatures (default), or not.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<SendFileRequest>?  CustomSendFileRequestSerializer   = null,
                               CustomBinarySerializerDelegate<Signature>?        CustomSignatureSerializer         = null,
                               Boolean                                           IncludeSignatures                 = true)
        {

            try
            {

                var binaryStream = new MemoryStream();

                var fileNameBytes         = FileName.       ToString().ToUTF8Bytes();
                binaryStream.WriteUInt64((UInt64) fileNameBytes.       Length);
                binaryStream.Write(fileNameBytes);

                binaryStream.WriteUInt64((UInt64) FileContent.         Length);
                binaryStream.Write(FileContent);

                var fileContentTypeBytes  = FileContentType.ToString().ToUTF8Bytes();
                binaryStream.WriteByte  ((Byte)   fileContentTypeBytes.Length);
                binaryStream.Write(fileContentTypeBytes);

                binaryStream.WriteByte  ((Byte)   FileSHA256.          Length);
                if (FileSHA256.Length > 0)
                    binaryStream.Write(FileSHA256);

                binaryStream.WriteByte  ((Byte)   FileSHA512.          Length);
                if (FileSHA512.Length > 0)
                    binaryStream.Write(FileSHA512);

                var fileSignaturesCount   = (UInt16) FileSignatures.Count();
                binaryStream.WriteByte  ((Byte)   fileSignaturesCount);

                foreach (var fileSignature in FileSignatures)
                {
                    var binaryFileSignature = fileSignature.ToBinary();
                    binaryStream.WriteUInt16((UInt16) binaryFileSignature.Length);
                    binaryStream.Write(binaryFileSignature);
                }


                var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                binaryStream.WriteByte((Byte) signaturesCount);

                if (IncludeSignatures) {
                    foreach (var signature in Signatures)
                    {
                        var binarySignature = signature.ToBinary();
                        binaryStream.WriteUInt16((UInt16) binarySignature.Length);
                        binaryStream.Write(binarySignature);
                    }
                }


                var binary = binaryStream.ToArray();

                return CustomSendFileRequestSerializer is not null
                           ? CustomSendFileRequestSerializer(this, binary)
                           : binary;

            }
            catch (Exception e)
            {
                throw new Exception("The given data structure of a get file request is invalid: " + e.Message, e);
            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (SendFileRequest1, SendFileRequest2)

        /// <summary>
        /// Compares two SendFile requests for equality.
        /// </summary>
        /// <param name="SendFileRequest1">A SendFile request.</param>
        /// <param name="SendFileRequest2">Another SendFile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendFileRequest? SendFileRequest1,
                                           SendFileRequest? SendFileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendFileRequest1, SendFileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SendFileRequest1 is null || SendFileRequest2 is null)
                return false;

            return SendFileRequest1.Equals(SendFileRequest2);

        }

        #endregion

        #region Operator != (SendFileRequest1, SendFileRequest2)

        /// <summary>
        /// Compares two SendFile requests for inequality.
        /// </summary>
        /// <param name="SendFileRequest1">A SendFile request.</param>
        /// <param name="SendFileRequest2">Another SendFile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendFileRequest? SendFileRequest1,
                                           SendFileRequest? SendFileRequest2)

            => !(SendFileRequest1 == SendFileRequest2);

        #endregion

        #endregion

        #region IEquatable<SendFileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two send file requests for equality.
        /// </summary>
        /// <param name="Object">A send file request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendFileRequest sendFileRequest &&
                   Equals(sendFileRequest);

        #endregion

        #region Equals(SendFileRequest)

        /// <summary>
        /// Compares two send file requests for equality.
        /// </summary>
        /// <param name="SendFileRequest">A send file request to compare with.</param>
        public override Boolean Equals(SendFileRequest? SendFileRequest)

            => SendFileRequest is not null               &&

               FileName.Equals(SendFileRequest.FileName) &&

            ((!Priority.HasValue && !SendFileRequest.Priority.HasValue) ||
              (Priority.HasValue &&  SendFileRequest.Priority.HasValue && Priority.Equals(SendFileRequest.Priority))) &&

               base.GenericEquals(SendFileRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   FileName,

                   Priority.HasValue
                        ? $" ({Priority})"
                        : ""

                );

        #endregion


    }

}
