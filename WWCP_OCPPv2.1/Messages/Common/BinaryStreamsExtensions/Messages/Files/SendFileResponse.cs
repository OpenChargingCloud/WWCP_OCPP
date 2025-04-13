/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/GetChargingCloud/WWCP_OCPP>
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The SendFile response.
    /// </summary>
    public class SendFileResponse : AResponse<SendFileRequest,
                                              SendFileResponse>,
                                    IResponse<Result>
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

        /// <summary>
        /// Create a new SendFile response.
        /// </summary>
        /// <param name="Request">The SendFile request leading to this response.</param>
        /// <param name="FileName">The name of the stored file including its absolute path.</param>
        /// <param name="Status">An optional response status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the response status.</param>
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
        public SendFileResponse(SendFileRequest          Request,
                                FilePath                 FileName,
                                SendFileStatus           Status,
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

        #region (static) Parse   (Request, JSON, CustomSendFileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SendFile response.
        /// </summary>
        /// <param name="Request">The SendFile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSendFileResponseParser">An optional delegate to parse custom SendFile responses.</param>
        public static SendFileResponse Parse(SendFileRequest                                 Request,
                                             JObject                                         JSON,
                                             SourceRouting                               Destination,
                                             NetworkPath                                     NetworkPath,
                                             DateTime?                                       ResponseTimestamp              = null,
                                             CustomJObjectParserDelegate<SendFileResponse>?  CustomSendFileResponseParser   = null,
                                             CustomJObjectParserDelegate<StatusInfo>?        CustomStatusInfoParser         = null,
                                             CustomJObjectParserDelegate<Signature>?         CustomSignatureParser          = null,
                                             CustomJObjectParserDelegate<CustomData>?        CustomCustomDataParser         = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var sendFileResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSendFileResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return sendFileResponse;
            }

            throw new ArgumentException("The given JSON representation of a SendFile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SendFileResponse, out ErrorResponse, CustomSendFileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SendFile response.
        /// </summary>
        /// <param name="Request">The SendFile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SendFileResponse">The parsed SendFile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendFileResponseParser">An optional delegate to parse custom SendFile responses.</param>
        public static Boolean TryParse(SendFileRequest                                 Request,
                                       JObject                                         JSON,
                                       SourceRouting                               Destination,
                                       NetworkPath                                     NetworkPath,
                                       [NotNullWhen(true)]  out SendFileResponse?      SendFileResponse,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTime?                                       ResponseTimestamp              = null,
                                       CustomJObjectParserDelegate<SendFileResponse>?  CustomSendFileResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?        CustomStatusInfoParser         = null,
                                       CustomJObjectParserDelegate<Signature>?         CustomSignatureParser          = null,
                                       CustomJObjectParserDelegate<CustomData>?        CustomCustomDataParser         = null)
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
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
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


                SendFileResponse = new SendFileResponse(

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
        /// <param name="CustomSendFileResponseSerializer">A delegate to serialize custom SendFile responses.</param>
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
        /// The SendFile failed because of a request error.
        /// </summary>
        /// <param name="Request">The SendFile request.</param>
        public static SendFileResponse RequestError(SendFileRequest          Request,
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
                   SendFileStatus.Rejected,
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
        /// The SendFile failed.
        /// </summary>
        /// <param name="Request">The SendFile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SendFileResponse FormationViolation(SendFileRequest  Request,
                                                          String           ErrorDescription)

            => new (Request,
                    Request.FileName,
                    SendFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SendFile failed.
        /// </summary>
        /// <param name="Request">The SendFile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SendFileResponse SignatureError(SendFileRequest  Request,
                                                      String           ErrorDescription)

            => new (Request,
                    Request.FileName,
                    SendFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SendFile failed.
        /// </summary>
        /// <param name="Request">The SendFile request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SendFileResponse Failed(SendFileRequest  Request,
                                              String?          Description   = null)

            => new (Request,
                    Request.FileName,
                    SendFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SendFile failed because of an exception.
        /// </summary>
        /// <param name="Request">The SendFile request.</param>
        /// <param name="Exception">The exception.</param>
        public static SendFileResponse ExceptionOccurred(SendFileRequest  Request,
                                                        Exception        Exception)

            => new (Request,
                    Request.FileName,
                    SendFileStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SendFileResponse1, SendFileResponse2)

        /// <summary>
        /// Compares two SendFile responses for equality.
        /// </summary>
        /// <param name="SendFileResponse1">A SendFile response.</param>
        /// <param name="SendFileResponse2">Another SendFile response.</param>
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
        /// Compares two SendFile responses for inequality.
        /// </summary>
        /// <param name="SendFileResponse1">A SendFile response.</param>
        /// <param name="SendFileResponse2">Another SendFile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendFileResponse? SendFileResponse1,
                                           SendFileResponse? SendFileResponse2)

            => !(SendFileResponse1 == SendFileResponse2);

        #endregion

        #endregion

        #region IEquatable<SendFileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SendFile responses for equality.
        /// </summary>
        /// <param name="Object">A SendFile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendFileResponse sendFileResponse &&
                   Equals(sendFileResponse);

        #endregion

        #region Equals(SendFileResponse)

        /// <summary>
        /// Compares two SendFile responses for equality.
        /// </summary>
        /// <param name="SendFileResponse">A SendFile response to compare with.</param>
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
