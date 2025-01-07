/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The GetFile response.
    /// </summary>
    public class GetFileResponse : AResponse<GetFileRequest,
                                             GetFileResponse>,
                                   IResponse<Result>

    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getFileResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The absolute path and name of the file to fetch.
        /// </summary>
        [Mandatory]
        public FilePath                FileName           { get; }

        /// <summary>
        /// The success or failure status of the get file.
        /// </summary>
        [Mandatory]
        public GetFileStatus           Status             { get; }

        /// <summary>
        /// The optional file content.
        /// </summary>
        [Optional]
        public Byte[]                  FileContent        { get; }

        /// <summary>
        /// The optional content/MIME type of the file.
        /// </summary>
        [Optional]
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

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get file response.
        /// </summary>
        /// <param name="Request">The get file request leading to this response.</param>
        /// 
        /// <param name="FileName">The absolute path and name of the file to fetch.</param>
        /// <param name="Status">The success or failure status of the get file.</param>
        /// <param name="FileContent">The optional file content.</param>
        /// <param name="FileContentType">The optional content/MIME type of the file.</param>
        /// <param name="FileSHA256">The optional SHA256 hash value of the file content.</param>
        /// <param name="FileSHA512">The optional SHA512 hash value of the file content.</param>
        /// <param name="FileSignatures">The optional enumeration of cryptographic signatures for the file content.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public GetFileResponse(GetFileRequest           Request,

                               FilePath                 FileName,
                               GetFileStatus            Status,
                               Byte[]?                  FileContent           = null,
                               ContentType?             FileContentType       = null,
                               Byte[]?                  FileSHA256            = null,
                               Byte[]?                  FileSHA512            = null,
                               IEnumerable<Signature>?  FileSignatures        = null,

                               Result?                  Result                = null,
                               DateTime?                ResponseTimestamp     = null,

                               SourceRouting?           Destination           = null,
                               NetworkPath?             NetworkPath           = null,

                               IEnumerable<KeyPair>?    SignKeys              = null,
                               IEnumerable<SignInfo>?   SignInfos             = null,
                               IEnumerable<Signature>?  Signatures            = null,

                               CustomData?              CustomData            = null,

                               SerializationFormats?    SerializationFormat   = null,
                               CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.BinaryTextIds,
                   CancellationToken)

        {

            this.FileName         = FileName;
            this.Status           = Status;
            this.FileContent      = FileContent                ?? [];
            this.FileContentType  = FileContentType            ?? ContentType.Application.OctetStream;
            this.FileSHA256       = FileSHA256                 ?? [];
            this.FileSHA512       = FileSHA512                 ?? [];
            this.FileSignatures   = FileSignatures?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.FileName.       GetHashCode() * 19 ^
                           this.Status.         GetHashCode() * 17 ^
                           this.FileContent.    GetHashCode() * 13 ^
                           this.FileContentType.GetHashCode() * 11 ^
                           this.FileSHA256.     GetHashCode() *  7 ^
                           this.FileSHA512.     GetHashCode() *  5 ^
                           this.FileSignatures. GetHashCode() *  3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, Binary, CustomGetFileResponseSerializer = null)

        /// <summary>
        /// Parse the given JSON representation of a get file response.
        /// </summary>
        /// <param name="Request">The get file request leading to this response.</param>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="CustomGetFileResponseParser">An optional delegate to parse custom get file responses.</param>
        public static GetFileResponse Parse(GetFileRequest                                Request,
                                            Byte[]                                        Binary,
                                            SourceRouting                             Destination,
                                            NetworkPath                                   NetworkPath,
                                            DateTime?                                     ResponseTimestamp             = null,
                                            CustomBinaryParserDelegate<GetFileResponse>?  CustomGetFileResponseParser   = null,
                                            CustomJObjectParserDelegate<StatusInfo>?      CustomStatusInfoParser        = null,
                                            CustomJObjectParserDelegate<Signature>?       CustomSignatureParser         = null,
                                            CustomJObjectParserDelegate<CustomData>?      CustomCustomDataParser        = null)
        {

            if (TryParse(Request,
                         Binary,
                         Destination,
                         NetworkPath,
                         out var getFileResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetFileResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getFileResponse;
            }

            throw new ArgumentException("The given binary representation of a get file response is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(Request, Binary, out GetFileResponse, out ErrorResponse, CustomGetFileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get file response.
        /// </summary>
        /// <param name="Request">The get file request leading to this response.</param>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="GetFileResponse">The parsed get file response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetFileResponseParser">An optional delegate to parse custom get file responses.</param>
        public static Boolean TryParse(GetFileRequest                                Request,
                                       Byte[]                                        Binary,
                                       SourceRouting                             Destination,
                                       NetworkPath                                   NetworkPath,
                                       [NotNullWhen(true)]  out GetFileResponse?     GetFileResponse,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       DateTime?                                     ResponseTimestamp             = null,
                                       CustomBinaryParserDelegate<GetFileResponse>?  CustomGetFileResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?      CustomStatusInfoParser        = null,
                                       CustomJObjectParserDelegate<Signature>?       CustomSignatureParser         = null,
                                       CustomJObjectParserDelegate<CustomData>?      CustomCustomDataParser        = null)
        {

            try
            {

                GetFileResponse = null;
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

                #region Get file status

                var getFileStatusLength   = stream.ReadUInt16();
                var getFileStatusText     = stream.ReadUTF8String(getFileStatusLength);
                if (!GetFileStatus.TryParse(getFileStatusText, out var getFileStatus))
                {
                    ErrorResponse = $"The received get file status '{getFileStatusText}' is invalid!";
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

                    if (!Signature.TryParse(signatureBytes, out var signature, out ErrorResponse))
                        return false;

                    signatures.Add(signature);

                }

                #endregion

                //ToDo: Check if there are some bytes left!

                #region CustomData    [optional]

                CustomData? CustomData = null;

                #endregion


                GetFileResponse = new GetFileResponse(

                                      Request,
                                      fileName.Value,
                                      getFileStatus,
                                      fileContent,
                                      fileContentType,
                                      fileSHA256,
                                      fileSHA512,
                                      fileSignatures,
                                      null,
                                      null,

                                      null,
                                      null,

                                      null,
                                      null,
                                      signatures,

                                      CustomData

                                  );


                if (CustomGetFileResponseParser is not null)
                    GetFileResponse = CustomGetFileResponseParser(Binary,
                                                                  GetFileResponse);

                return true;

            }
            catch (Exception e)
            {
                GetFileResponse  = null;
                ErrorResponse    = "The given binary representation of a get file response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToBinary(CustomGetFileResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomGetFileResponseSerializer">A delegate to serialize custom get file responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomBinarySignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        /// <param name="IncludeSignatures">Whether to include the digital signatures (default), or not.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<GetFileResponse>?  CustomGetFileResponseSerializer   = null,
                               CustomJObjectSerializerDelegate<StatusInfo>?      CustomStatusInfoSerializer        = null,
                               CustomBinarySerializerDelegate<Signature>?        CustomBinarySignatureSerializer   = null,
                               CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer        = null,
                               Boolean                                           IncludeSignatures                 = true)
        {

            try
            {

                var binaryStream = new MemoryStream();

                var fileNameBytes         = FileName.       ToString().ToUTF8Bytes();
                binaryStream.WriteUInt64((UInt64) fileNameBytes.       Length);
                binaryStream.Write(fileNameBytes);

                var statusBytes           = Status.         ToString().ToUTF8Bytes();
                binaryStream.WriteUInt16((UInt16) statusBytes.         Length);
                binaryStream.Write(statusBytes);

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
                        var binarySignature = signature.ToBinary(CustomBinarySignatureSerializer);
                        binaryStream.WriteUInt16((UInt16) binarySignature.Length);
                        binaryStream.Write(binarySignature);
                    }
                }


                var binary = binaryStream.ToArray();

                return CustomGetFileResponseSerializer is not null
                           ? CustomGetFileResponseSerializer(this, binary)
                           : binary;

            }
            catch (Exception e)
            {
                throw new Exception("The given data structure of a get file response is invalid: " + e.Message, e);
            }

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetFile failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetFile request.</param>
        public static GetFileResponse RequestError(GetFileRequest           Request,
                                                   EventTracking_Id         EventTrackingId,
                                                   ResultCode               ErrorCode,
                                                   String?                  ErrorDescription    = null,
                                                   JObject?                 ErrorDetails        = null,
                                                   DateTime?                ResponseTimestamp   = null,

                                                   SourceRouting?           Destination         = null,
                                                   NetworkPath?             NetworkPath         = null,

                                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                                   IEnumerable<Signature>?  Signatures          = null,

                                                   CustomData?              CustomData          = null)

            => new (

                   Request,
                   Request.FileName,
                   GetFileStatus.Rejected,
                   null,
                   null,
                   null,
                   null,
                   null,
                  OCPPv2_1.Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The GetFile failed.
        /// </summary>
        /// <param name="Request">The GetFile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetFileResponse FormationViolation(GetFileRequest  Request,
                                                         String          ErrorDescription)

            => new (Request,
                    Request.FileName,
                    GetFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetFile failed.
        /// </summary>
        /// <param name="Request">The GetFile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetFileResponse SignatureError(GetFileRequest  Request,
                                                     String          ErrorDescription)

            => new (Request,
                    Request.FileName,
                    GetFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetFile failed.
        /// </summary>
        /// <param name="Request">The GetFile request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetFileResponse Failed(GetFileRequest  Request,
                                             String?         Description   = null)

            => new (Request,
                    Request.FileName,
                    GetFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetFile failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetFile request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetFileResponse ExceptionOccured(GetFileRequest  Request,
                                                       Exception       Exception)

            => new (Request,
                    Request.FileName,
                    GetFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetFileResponse1, GetFileResponse2)

        /// <summary>
        /// Compares two get file responses for equality.
        /// </summary>
        /// <param name="GetFileResponse1">A get file response.</param>
        /// <param name="GetFileResponse2">Another get file response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetFileResponse? GetFileResponse1,
                                           GetFileResponse? GetFileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetFileResponse1, GetFileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetFileResponse1 is null || GetFileResponse2 is null)
                return false;

            return GetFileResponse1.Equals(GetFileResponse2);

        }

        #endregion

        #region Operator != (GetFileResponse1, GetFileResponse2)

        /// <summary>
        /// Compares two get file responses for inequality.
        /// </summary>
        /// <param name="GetFileResponse1">A get file response.</param>
        /// <param name="GetFileResponse2">Another get file response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetFileResponse? GetFileResponse1,
                                           GetFileResponse? GetFileResponse2)

            => !(GetFileResponse1 == GetFileResponse2);

        #endregion

        #endregion

        #region IEquatable<GetFileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get file responses for equality.
        /// </summary>
        /// <param name="Object">A get file response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetFileResponse getFileResponse &&
                   Equals(getFileResponse);

        #endregion

        #region Equals(GetFileResponse)

        /// <summary>
        /// Compares two get file responses for equality.
        /// </summary>
        /// <param name="GetFileResponse">A get file response to compare with.</param>
        public override Boolean Equals(GetFileResponse? GetFileResponse)

            => GetFileResponse is not null &&

               Status.     Equals(GetFileResponse.Status) &&

             //((AdditionalStatusInfo is     null && GetFileResponse.AdditionalStatusInfo is     null) ||
             //  AdditionalStatusInfo is not null && GetFileResponse.AdditionalStatusInfo is not null && AdditionalStatusInfo.Equals(GetFileResponse.AdditionalStatusInfo)) &&

             //((FileConent                 is     null && GetFileResponse.FileConent                 is     null) ||
             // (FileConent                 is not null && GetFileResponse.FileConent                 is not null && FileConent.                Equals(GetFileResponse.FileConent)))                &&

               base.GenericEquals(GetFileResponse);

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

                   $"{FileName} => {Status}",

                   FileContent?.Length > 0
                       ? $": '{FileContent.ToBase64().SubstringMax(100)}' [{FileContentType} {FileContent.Length} bytes]"
                       : "",

                   FileSHA256.Length > 0
                       ? ""
                       : $", SHA256: {FileSHA256.ToHexString()}",

                   FileSHA512.Length > 0
                       ? ""
                       : $", SHA512: {FileSHA512.ToHexString()}"

               );

        #endregion

    }

}
