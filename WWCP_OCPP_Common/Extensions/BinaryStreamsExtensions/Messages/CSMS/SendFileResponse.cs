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
    /// A send file response.
    /// </summary>
    public class SendFileResponse : AResponse<CSMS.SendFileRequest,
                                                   SendFileResponse>,
                                    IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/sendFileResponse");

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
        public SendFileStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the response status.
        /// </summary>
        [Optional]
        public StatusInfo?     StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region SendFileResponse(Request, Status = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new send file response.
        /// </summary>
        /// <param name="Request">The send file request leading to this response.</param>
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
        public SendFileResponse(CSMS.SendFileRequest     Request,
                                FilePath                 FileName,
                                SendFileStatus           Status,
                                StatusInfo?              StatusInfo          = null,
                                DateTime?                ResponseTimestamp   = null,

                                IEnumerable<KeyPair>?    SignKeys            = null,
                                IEnumerable<SignInfo>?   SignInfos           = null,
                                IEnumerable<Signature>?  Signatures          = null,

                                CustomData?              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

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

        #region SendFileResponse(Request, Result)

        /// <summary>
        /// Create a new send file response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public SendFileResponse(CSMS.SendFileRequest  Request,
                                Result                Result)

            : base(Request,
                   Result)

        {

            this.Status = SendFileStatus.Rejected;

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomSendFileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a send file response.
        /// </summary>
        /// <param name="Request">The send file request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSendFileResponseParser">A delegate to parse custom send file responses.</param>
        public static SendFileResponse Parse(CSMS.SendFileRequest                            Request,
                                             JObject                                         JSON,
                                             CustomJObjectParserDelegate<SendFileResponse>?  CustomSendFileResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var sendFileResponse,
                         out var errorResponse,
                         CustomSendFileResponseParser) &&
                sendFileResponse is not null)
            {
                return sendFileResponse;
            }

            throw new ArgumentException("The given JSON representation of a SendFile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SendFileResponse, out ErrorResponse, CustomSendFileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a send file response.
        /// </summary>
        /// <param name="Request">The send file request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SendFileResponse">The parsed send file response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendFileResponseParser">A delegate to parse custom send file responses.</param>
        public static Boolean TryParse(CSMS.SendFileRequest                            Request,
                                       JObject                                         JSON,
                                       [NotNullWhen(true)]  out SendFileResponse?      SendFileResponse,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<SendFileResponse>?  CustomSendFileResponseParser   = null)
        {

            try
            {

                SendFileResponse = null;

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
                                         SendFileStatus.TryParse,
                                         out SendFileStatus Status,
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


                SendFileResponse = new SendFileResponse(

                                       Request,
                                       FileName,
                                       Status,
                                       StatusInfo,
                                       null,

                                       null,
                                       null,
                                       Signatures,

                                       CustomData

                                   );

                if (CustomSendFileResponseParser is not null)
                    SendFileResponse = CustomSendFileResponseParser(JSON,
                                                                    SendFileResponse);

                return true;

            }
            catch (Exception e)
            {
                SendFileResponse  = null;
                ErrorResponse     = "The given JSON representation of a SendFileResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSendFileResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendFileResponseSerializer">A delegate to serialize custom send file responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SendFileResponse>?  CustomSendFileResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?        CustomStatusInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?         CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
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

            return CustomSendFileResponseSerializer is not null
                       ? CustomSendFileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The send file failed.
        /// </summary>
        public static SendFileResponse Failed(CSMS.SendFileRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SendFileResponse1, SendFileResponse2)

        /// <summary>
        /// Compares two send file responses for equality.
        /// </summary>
        /// <param name="SendFileResponse1">A send file response.</param>
        /// <param name="SendFileResponse2">Another send file response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendFileResponse? SendFileResponse1,
                                           SendFileResponse? SendFileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendFileResponse1, SendFileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SendFileResponse1 is null || SendFileResponse2 is null)
                return false;

            return SendFileResponse1.Equals(SendFileResponse2);

        }

        #endregion

        #region Operator != (SendFileResponse1, SendFileResponse2)

        /// <summary>
        /// Compares two send file responses for inequality.
        /// </summary>
        /// <param name="SendFileResponse1">A send file response.</param>
        /// <param name="SendFileResponse2">Another send file response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendFileResponse? SendFileResponse1,
                                           SendFileResponse? SendFileResponse2)

            => !(SendFileResponse1 == SendFileResponse2);

        #endregion

        #endregion

        #region IEquatable<SendFileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two send file responses for equality.
        /// </summary>
        /// <param name="Object">A send file response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendFileResponse sendFileResponse &&
                   Equals(sendFileResponse);

        #endregion

        #region Equals(SendFileResponse)

        /// <summary>
        /// Compares two send file responses for equality.
        /// </summary>
        /// <param name="SendFileResponse">A send file response to compare with.</param>
        public override Boolean Equals(SendFileResponse? SendFileResponse)

            => SendFileResponse is not null &&

               Status.Equals(SendFileResponse.Status) &&

             ((StatusInfo is     null && SendFileResponse.StatusInfo is     null) ||
              (StatusInfo is not null && SendFileResponse.StatusInfo is not null && StatusInfo.Equals(SendFileResponse.StatusInfo))) &&

               base.GenericEquals(SendFileResponse);

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
