/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The SSHKeySigned response (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class SSHKeySignedResponse : AResponse<SSHKeySignedRequest,
                                            SSHKeySignedResponse>,
                                       IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/sshKeySignedResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the request was taken: Accepted or Rejected, with the reason in the status info.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information, with a reason code.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a SSHKeySigned response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">Whether the request was taken.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>

        public SSHKeySignedResponse(SSHKeySignedRequest              Request,
                                GenericStatus                Status,
                                StatusInfo?                  StatusInfo            = null,


                                Result?                      Result                = null,
                                DateTimeOffset?              ResponseTimestamp     = null,

                                SourceRouting?               Destination           = null,
                                NetworkPath?                 NetworkPath           = null,

                                IEnumerable<KeyPair>?        SignKeys              = null,
                                IEnumerable<SignInfo>?       SignInfos             = null,
                                IEnumerable<Signature>?      Signatures            = null,

                                CustomData?                  CustomData            = null,

                                SerializationFormats?        SerializationFormat   = null,
                                CancellationToken            CancellationToken     = default)

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

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;


            unchecked
            {

                hashCode = this.Status.GetHashCode() * 23 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 29 ^

                           base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SSHKeySigned response.
        /// </summary>
        public static SSHKeySignedResponse Parse(SSHKeySignedRequest                                 Request,
                                           JObject                                     JSON,
                                           SourceRouting                               Destination,
                                           NetworkPath                                 NetworkPath,
                                           DateTimeOffset?                             ResponseTimestamp            = null,
                                           CustomJObjectParserDelegate<SSHKeySignedResponse>?  CustomSSHKeySignedResponseParser    = null,
                                           CustomJObjectParserDelegate<StatusInfo>?    CustomStatusInfoParser       = null,
                                           CustomJObjectParserDelegate<Signature>?     CustomSignatureParser        = null,
                                           CustomJObjectParserDelegate<CustomData>?    CustomCustomDataParser       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var response,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSSHKeySignedResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return response;
            }

            throw new ArgumentException("The given JSON representation of a SSHKeySigned response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SSHKeySignedResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SSHKeySigned response.
        /// </summary>
        public static Boolean TryParse(SSHKeySignedRequest                                 Request,
                                       JObject                                     JSON,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out SSHKeySignedResponse?      SSHKeySignedResponse,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       DateTimeOffset?                             ResponseTimestamp           = null,
                                       CustomJObjectParserDelegate<SSHKeySignedResponse>?  CustomSSHKeySignedResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?    CustomStatusInfoParser      = null,
                                       CustomJObjectParserDelegate<Signature>?     CustomSignatureParser       = null,
                                       CustomJObjectParserDelegate<CustomData>?    CustomCustomDataParser      = null)
        {

            try
            {

                SSHKeySignedResponse = null;

                #region Status               [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo           [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SSHKeySignedResponse = new SSHKeySignedResponse(

                                   Request,
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

                if (CustomSSHKeySignedResponseParser is not null)
                    SSHKeySignedResponse = CustomSSHKeySignedResponseParser(JSON,
                                                                   SSHKeySignedResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SSHKeySignedResponse   = null;
                ErrorResponse  = "The given JSON representation of a SSHKeySigned response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomSSHKeySignedResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomSSHKeySignedResponseSerializer">A delegate to serialize custom SSHKeySigned responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                         IncludeJSONLDContext               = false,
                              CustomJObjectSerializerDelegate<SSHKeySignedResponse>?  CustomSSHKeySignedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?    CustomStatusInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?     CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?    CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",    DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",      Status.AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                CustomCustomDataSerializer))
                               : null,



                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSSHKeySignedResponseSerializer is not null
                       ? CustomSSHKeySignedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SSHKeySigned failed because of a request error.
        /// </summary>
        public static SSHKeySignedResponse RequestError(SSHKeySignedRequest          Request,
                                                  EventTracking_Id         EventTrackingId,
                                                  ResultCode               ErrorCode,
                                                  String?                  ErrorDescription    = null,
                                                  JObject?                 ErrorDetails        = null,
                                                  DateTimeOffset?          ResponseTimestamp   = null,

                                                  SourceRouting?           Destination         = null,
                                                  NetworkPath?             NetworkPath         = null,

                                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                                  IEnumerable<Signature>?  Signatures          = null,

                                                  CustomData?              CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,

                   Result.FromErrorResponse(
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
        /// The SSHKeySigned failed: its data did not take the form it must.
        /// </summary>
        public static SSHKeySignedResponse FormationViolation(SSHKeySignedRequest  Request,
                                                        String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SSHKeySigned failed: a signature was not valid.
        /// </summary>
        public static SSHKeySignedResponse SignatureError(SSHKeySignedRequest  Request,
                                                    String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SSHKeySigned failed.
        /// </summary>
        public static SSHKeySignedResponse Failed(SSHKeySignedRequest  Request,
                                            String?          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The SSHKeySigned failed because of an exception.
        /// </summary>
        public static SSHKeySignedResponse ExceptionOccurred(SSHKeySignedRequest  Request,
                                                       Exception        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two SSHKeySigned responses for equality.
        /// </summary>
        public static Boolean operator == (SSHKeySignedResponse? SSHKeySignedResponse1,
                                           SSHKeySignedResponse? SSHKeySignedResponse2)

            => ReferenceEquals(SSHKeySignedResponse1, SSHKeySignedResponse2) ||
               (SSHKeySignedResponse1 is not null && SSHKeySignedResponse2 is not null && SSHKeySignedResponse1.Equals(SSHKeySignedResponse2));

        /// <summary>
        /// Compares two SSHKeySigned responses for inequality.
        /// </summary>
        public static Boolean operator != (SSHKeySignedResponse? SSHKeySignedResponse1,
                                           SSHKeySignedResponse? SSHKeySignedResponse2)

            => !(SSHKeySignedResponse1 == SSHKeySignedResponse2);

        #endregion

        #region IEquatable<SSHKeySignedResponse> Members

        /// <summary>
        /// Compares two SSHKeySigned responses for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is SSHKeySignedResponse response &&
                   Equals(response);

        /// <summary>
        /// Compares two SSHKeySigned responses for equality.
        /// </summary>
        public override Boolean Equals(SSHKeySignedResponse? SSHKeySignedResponse)

            => SSHKeySignedResponse is not null &&

               Status.Equals(SSHKeySignedResponse.Status) &&

             ((StatusInfo is     null && SSHKeySignedResponse.StatusInfo is     null) ||
               StatusInfo is not null && SSHKeySignedResponse.StatusInfo is not null && StatusInfo.Equals(SSHKeySignedResponse.StatusInfo)) &&



               base.GenericEquals(SSHKeySignedResponse);

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

            => Status.AsText() + (StatusInfo?.ReasonCode is String reasonCode ? $" ({reasonCode})" : "");

        #endregion

    }

}
