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

#endregion

namespace cloud.charging.open.protocols.OCPP.CS
{

    /// <summary>
    /// A delete file response.
    /// </summary>
    public class DeleteFileResponse : AResponse<CSMS.DeleteFileRequest,
                                                     DeleteFileResponse>,
                                      IResponse
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
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The name of the stored file including its absolute path.
        /// </summary>
        [Mandatory]
        public FilePath        FileName      { get; }

        /// <summary>
        /// The response status.
        /// </summary>
        [Mandatory]
        public DeleteFileStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the response status.
        /// </summary>
        [Optional]
        public StatusInfo?     StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region DeleteFileResponse(Request, Status = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new delete file response.
        /// </summary>
        /// <param name="Request">The delete file request leading to this response.</param>
        /// <param name="FileName">The name of the stored file including its absolute path.</param>
        /// <param name="Status">An optional response status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the response status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DeleteFileResponse(CSMS.DeleteFileRequest   Request,
                                  FilePath                 FileName,
                                  DeleteFileStatus         Status,
                                  StatusInfo?              StatusInfo          = null,
                                  DateTime?                ResponseTimestamp   = null,

                                  NetworkingNode_Id?       DestinationNodeId   = null,
                                  NetworkPath?             NetworkPath         = null,

                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                  IEnumerable<Signature>?  Signatures          = null,

                                  CustomData?              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationNodeId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

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

        #region DeleteFileResponse(Request, Result)

        /// <summary>
        /// Create a new delete file response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public DeleteFileResponse(CSMS.DeleteFileRequest  Request,
                                  Result                  Result)

            : base(Request,
                   Result)

        {

            this.Status = DeleteFileStatus.Rejected;

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomDeleteFileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a delete file response.
        /// </summary>
        /// <param name="Request">The delete file request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteFileResponseParser">A delegate to parse custom delete file responses.</param>
        public static DeleteFileResponse Parse(CSMS.DeleteFileRequest                            Request,
                                               JObject                                           JSON,
                                               CustomJObjectParserDelegate<DeleteFileResponse>?  CustomDeleteFileResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var deleteFileResponse,
                         out var errorResponse,
                         CustomDeleteFileResponseParser))
            {
                return deleteFileResponse;
            }

            throw new ArgumentException("The given JSON representation of a DeleteFile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteFileResponse, out ErrorResponse, CustomDeleteFileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a delete file response.
        /// </summary>
        /// <param name="Request">The delete file request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteFileResponse">The parsed delete file response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteFileResponseParser">A delegate to parse custom delete file responses.</param>
        public static Boolean TryParse(CSMS.DeleteFileRequest                            Request,
                                       JObject                                           JSON,
                                       [NotNullWhen(true)]  out DeleteFileResponse?      DeleteFileResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<DeleteFileResponse>?  CustomDeleteFileResponseParser   = null)
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
                                           OCPP.StatusInfo.TryParse,
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
                                           OCPP.CustomData.TryParse,
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

                                         null,
                                         null,

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
        /// <param name="CustomDeleteFileResponseSerializer">A delegate to serialize custom delete file responses.</param>
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
        /// The delete file failed.
        /// </summary>
        public static DeleteFileResponse Failed(CSMS.DeleteFileRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (DeleteFileResponse1, DeleteFileResponse2)

        /// <summary>
        /// Compares two delete file responses for equality.
        /// </summary>
        /// <param name="DeleteFileResponse1">A delete file response.</param>
        /// <param name="DeleteFileResponse2">Another delete file response.</param>
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
        /// Compares two delete file responses for inequality.
        /// </summary>
        /// <param name="DeleteFileResponse1">A delete file response.</param>
        /// <param name="DeleteFileResponse2">Another delete file response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteFileResponse? DeleteFileResponse1,
                                           DeleteFileResponse? DeleteFileResponse2)

            => !(DeleteFileResponse1 == DeleteFileResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteFileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two delete file responses for equality.
        /// </summary>
        /// <param name="Object">A delete file response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteFileResponse deleteFileResponse &&
                   Equals(deleteFileResponse);

        #endregion

        #region Equals(DeleteFileResponse)

        /// <summary>
        /// Compares two delete file responses for equality.
        /// </summary>
        /// <param name="DeleteFileResponse">A delete file response to compare with.</param>
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
