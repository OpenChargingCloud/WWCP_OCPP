/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/GetChargingCloud/WWCP_OCPP>
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The DeleteFile response.
    /// </summary>
    public class DeleteFileResponse : AResponse<DeleteFileRequest,
                                                DeleteFileResponse>,
                                      IResponse<Result>

    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/deleteFileResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The absolute path and name of the file to delete.
        /// </summary>
        [Mandatory]
        public FilePath          FileName      { get; }

        /// <summary>
        /// The response status.
        /// </summary>
        [Mandatory]
        public DeleteFileStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the response status.
        /// </summary>
        [Optional]
        public StatusInfo?       StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DeleteFile response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="FileName">The absolute path and name of the file to delete.</param>
        /// <param name="Status">The success or failure status of the DeleteFile request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DeleteFileResponse(DeleteFileRequest        Request,
                                  FilePath                 FileName,
                                  DeleteFileStatus         Status,
                                  StatusInfo?              StatusInfo            = null,

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

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.FileName    = FileName;
            this.Status      = Status;
            this.StatusInfo  = StatusInfo;


            unchecked
            {

                hashCode =  this.FileName.   GetHashCode()       * 7 ^
                            this.Status.     GetHashCode()       * 5 ^
                           (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                            base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomDeleteFileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a DeleteFile response.
        /// </summary>
        /// <param name="Request">The DeleteFile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteFileResponseParser">An optional delegate to parse custom DeleteFile responses.</param>
        public static DeleteFileResponse Parse(DeleteFileRequest                                 Request,
                                               JObject                                           JSON,
                                               SourceRouting                                 Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<DeleteFileResponse>?  CustomDeleteFileResponseParser   = null,
                                               CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var deleteFileResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomDeleteFileResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return deleteFileResponse;
            }

            throw new ArgumentException("The given JSON representation of a DeleteFile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteFileResponse, out ErrorResponse, CustomDeleteFileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteFile response.
        /// </summary>
        /// <param name="Request">The DeleteFile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteFileResponse">The parsed DeleteFile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteFileResponseParser">An optional delegate to parse custom DeleteFile responses.</param>
        public static Boolean TryParse(DeleteFileRequest                                 Request,
                                       JObject                                           JSON,
                                       SourceRouting                                 Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out DeleteFileResponse?      DeleteFileResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<DeleteFileResponse>?  CustomDeleteFileResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                DeleteFileResponse = null;

                #region FileName      [mandatory]

                if (!JSON.ParseMandatory("fileName",
                                         "file name with absolute path",
                                         FilePath.TryParse,
                                         out FilePath FileName,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "response status",
                                         DeleteFileStatus.TryParse,
                                         out DeleteFileStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DeleteFileResponse = new DeleteFileResponse(

                                         Request,
                                         FileName,
                                         Status,
                                         StatusInfo,
                                         null,
                                         ResponseTimestamp,

                                         Destination,
                                         NetworkPath,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData

                                     );

                if (CustomDeleteFileResponseParser is not null)
                    DeleteFileResponse = CustomDeleteFileResponseParser(JSON,
                                                                    DeleteFileResponse);

                return true;

            }
            catch (Exception e)
            {
                DeleteFileResponse  = null;
                ErrorResponse     = "The given JSON representation of a DeleteFileResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteFileResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteFileResponseSerializer">A delegate to serialize custom DeleteFile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteFileResponse>?  CustomDeleteFileResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?          CustomStatusInfoSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("fileName",     FileName.  ToString()),
                                 new JProperty("status",       Status.    ToString()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDeleteFileResponseSerializer is not null
                       ? CustomDeleteFileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The DeleteFile failed because of a request error.
        /// </summary>
        /// <param name="Request">The DeleteFile request.</param>
        public static DeleteFileResponse RequestError(DeleteFileRequest        Request,
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
                   DeleteFileStatus.Rejected,
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
        /// The DeleteFile failed.
        /// </summary>
        /// <param name="Request">The DeleteFile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteFileResponse FormationViolation(DeleteFileRequest  Request,
                                                            String             ErrorDescription)

            => new (Request,
                    Request.FileName,
                    DeleteFileStatus.FormationViolation,
                    Result:OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteFile failed.
        /// </summary>
        /// <param name="Request">The DeleteFile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteFileResponse SignatureError(DeleteFileRequest  Request,
                                                        String             ErrorDescription)

            => new (Request,
                    Request.FileName,
                    DeleteFileStatus.InvalidSignature,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteFile failed.
        /// </summary>
        /// <param name="Request">The DeleteFile request.</param>
        /// <param name="Description">An optional error description.</param>
        public static DeleteFileResponse Failed(DeleteFileRequest  Request,
                                                String?            Description   = null)

            => new (Request,
                    Request.FileName,
                    DeleteFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The DeleteFile failed because of an exception.
        /// </summary>
        /// <param name="Request">The DeleteFile request.</param>
        /// <param name="Exception">The exception.</param>
        public static DeleteFileResponse ExceptionOccured(DeleteFileRequest  Request,
                                                          Exception          Exception)

            => new (Request,
                    Request.FileName,
                    DeleteFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (DeleteFileResponse1, DeleteFileResponse2)

        /// <summary>
        /// Compares two DeleteFile responses for equality.
        /// </summary>
        /// <param name="DeleteFileResponse1">A DeleteFile response.</param>
        /// <param name="DeleteFileResponse2">Another DeleteFile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteFileResponse? DeleteFileResponse1,
                                           DeleteFileResponse? DeleteFileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteFileResponse1, DeleteFileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteFileResponse1 is null || DeleteFileResponse2 is null)
                return false;

            return DeleteFileResponse1.Equals(DeleteFileResponse2);

        }

        #endregion

        #region Operator != (DeleteFileResponse1, DeleteFileResponse2)

        /// <summary>
        /// Compares two DeleteFile responses for inequality.
        /// </summary>
        /// <param name="DeleteFileResponse1">A DeleteFile response.</param>
        /// <param name="DeleteFileResponse2">Another DeleteFile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteFileResponse? DeleteFileResponse1,
                                           DeleteFileResponse? DeleteFileResponse2)

            => !(DeleteFileResponse1 == DeleteFileResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteFileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteFile responses for equality.
        /// </summary>
        /// <param name="Object">A DeleteFile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteFileResponse deleteFileResponse &&
                   Equals(deleteFileResponse);

        #endregion

        #region Equals(DeleteFileResponse)

        /// <summary>
        /// Compares two DeleteFile responses for equality.
        /// </summary>
        /// <param name="DeleteFileResponse">A DeleteFile response to compare with.</param>
        public override Boolean Equals(DeleteFileResponse? DeleteFileResponse)

            => DeleteFileResponse is not null &&

               Status.Equals(DeleteFileResponse.Status) &&

             ((StatusInfo is     null && DeleteFileResponse.StatusInfo is     null) ||
              (StatusInfo is not null && DeleteFileResponse.StatusInfo is not null && StatusInfo.Equals(DeleteFileResponse.StatusInfo))) &&

               base.GenericEquals(DeleteFileResponse);

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

                   StatusInfo is not null
                       ? $", {StatusInfo}"
                       : ""

               );

        #endregion


    }

}
