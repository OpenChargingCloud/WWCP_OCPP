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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The DeleteFile request.
    /// </summary>
    public class DeleteFileRequest : ARequest<DeleteFileRequest>,
                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/deleteFileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The name of the file including its absolute path.
        /// </summary>
        [Mandatory]
        public FilePath       FileName      { get; }

        /// <summary>
        /// The optional SHA256 hash value of the file content.
        /// </summary>
        [Optional]
        public Byte[]         FileSHA256    { get; }

        /// <summary>
        /// The optional SHA512 hash value of the file content.
        /// </summary>
        [Optional]
        public Byte[]         FileSHA512    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DeleteFile request.
        /// </summary>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="FileSHA256">An optional SHA256 hash value of the file content.</param>
        /// <param name="FileSHA512">An optional SHA512 hash value of the file content.</param>
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
        public DeleteFileRequest(SourceRouting            SourceRouting,
                                 FilePath                 FileName,
                                 Byte[]?                  FileSHA256            = null,
                                 Byte[]?                  FileSHA512            = null,

                                 IEnumerable<KeyPair>?    SignKeys              = null,
                                 IEnumerable<SignInfo>?   SignInfos             = null,
                                 IEnumerable<Signature>?  Signatures            = null,

                                 CustomData?              CustomData            = null,

                                 Request_Id?              RequestId             = null,
                                 DateTime?                RequestTimestamp      = null,
                                 TimeSpan?                RequestTimeout        = null,
                                 EventTracking_Id?        EventTrackingId       = null,
                                 NetworkPath?             NetworkPath           = null,
                                 SerializationFormats?    SerializationFormat   = null,
                                 CancellationToken        CancellationToken     = default)

            : base(SourceRouting,
                   nameof(DeleteFileRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.FileName    = FileName;
            this.FileSHA256  = FileSHA256 ?? [];
            this.FileSHA512  = FileSHA512 ?? [];

            unchecked
            {

                hashCode = this.FileName.  GetHashCode() * 7 ^
                           this.FileSHA256.GetHashCode() * 5 ^
                           this.FileSHA512.GetHashCode() * 3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomDeleteFileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a DeleteFileRequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteFileRequestParser">An optional delegate to parse custom DeleteFileRequest requests.</param>
        public static DeleteFileRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              SourceRouting                                    SourceRouting,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        RequestTimestamp                = null,
                                              TimeSpan?                                        RequestTimeout                  = null,
                                              EventTracking_Id?                                EventTrackingId                 = null,
                                              CustomJObjectParserDelegate<DeleteFileRequest>?  CustomDeleteFileRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                             SourceRouting,
                         NetworkPath,
                         out var deleteFileRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDeleteFileRequestParser))
            {
                return deleteFileRequest;
            }

            throw new ArgumentException("The given JSON representation of a DeleteFile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out DeleteFileRequest, out ErrorResponse, CustomDeleteFileRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteFile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DeleteFileRequest">The parsed DeleteFileRequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteFileRequestParser">An optional delegate to parse custom DeleteFileRequest requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       SourceRouting                                    SourceRouting,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out DeleteFileRequest?      DeleteFileRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        RequestTimestamp                = null,
                                       TimeSpan?                                        RequestTimeout                  = null,
                                       EventTracking_Id?                                EventTrackingId                 = null,
                                       CustomJObjectParserDelegate<DeleteFileRequest>?  CustomDeleteFileRequestParser   = null)
        {

            try
            {

                DeleteFileRequest = null;

                #region FileName             [mandatory]

                if (!JSON.ParseMandatory("fileName",
                                         "filename with absolute path",
                                         FilePath.TryParse,
                                         out FilePath FileName,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region FileSHA256           [optional]

                if (JSON.ParseOptional("fileSHA256",
                                       "file content SHA256 value",
                                       out String? fileSHA256Text,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var FileSHA256 = fileSHA256Text?.FromBase64();

                #endregion

                #region FileSHA512           [optional]

                if (JSON.ParseOptional("fileSHA512",
                                       "file content SHA512 value",
                                       out String? fileSHA512Text,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var FileSHA512 = fileSHA512Text?.FromBase64();

                #endregion

                #region Signatures           [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DeleteFileRequest = new DeleteFileRequest(

                                            SourceRouting,
                                        FileName,
                                        FileSHA256,
                                        FileSHA512,

                                        null,
                                        null,
                                        Signatures,

                                        CustomData,

                                        RequestId,
                                        RequestTimestamp,
                                        RequestTimeout,
                                        EventTrackingId,
                                        NetworkPath

                                    );

                if (CustomDeleteFileRequestParser is not null)
                    DeleteFileRequest = CustomDeleteFileRequestParser(JSON,
                                                                      DeleteFileRequest);

                return true;

            }
            catch (Exception e)
            {
                DeleteFileRequest  = null;
                ErrorResponse      = "The given JSON representation of a DeleteFile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteFileRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteFileRequestSerializer">A delegate to serialize custom DeleteFileRequest requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteFileRequest>?  CustomDeleteFileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("fileName",     FileName.ToString()),

                           FileSHA256.Length > 0
                               ? new JProperty("fileSHA256",   FileSHA256.ToBase64())
                               : null,

                           FileSHA512.Length > 0
                               ? new JProperty("fileSHA512",   FileSHA512.ToBase64())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDeleteFileRequestSerializer is not null
                       ? CustomDeleteFileRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DeleteFileRequest1, DeleteFileRequest2)

        /// <summary>
        /// Compares two DeleteFile requests for equality.
        /// </summary>
        /// <param name="DeleteFileRequest1">A DeleteFile request.</param>
        /// <param name="DeleteFileRequest2">Another DeleteFile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteFileRequest? DeleteFileRequest1,
                                           DeleteFileRequest? DeleteFileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteFileRequest1, DeleteFileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteFileRequest1 is null || DeleteFileRequest2 is null)
                return false;

            return DeleteFileRequest1.Equals(DeleteFileRequest2);

        }

        #endregion

        #region Operator != (DeleteFileRequest1, DeleteFileRequest2)

        /// <summary>
        /// Compares two DeleteFile requests for inequality.
        /// </summary>
        /// <param name="DeleteFileRequest1">A DeleteFile request.</param>
        /// <param name="DeleteFileRequest2">Another DeleteFile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteFileRequest? DeleteFileRequest1,
                                           DeleteFileRequest? DeleteFileRequest2)

            => !(DeleteFileRequest1 == DeleteFileRequest2);

        #endregion

        #endregion

        #region IEquatable<DeleteFileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteFile requests for equality.
        /// </summary>
        /// <param name="Object">A DeleteFile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteFileRequest deleteFileRequest &&
                   Equals(deleteFileRequest);

        #endregion

        #region Equals(DeleteFileRequest)

        /// <summary>
        /// Compares two DeleteFile requests for equality.
        /// </summary>
        /// <param name="DeleteFileRequest">A DeleteFile request to compare with.</param>
        public override Boolean Equals(DeleteFileRequest? DeleteFileRequest)

            => DeleteFileRequest is not null               &&

               FileName.Equals(DeleteFileRequest.FileName) &&

               base.GenericEquals(DeleteFileRequest);

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

            => FileName.ToString();

        #endregion


    }

}
