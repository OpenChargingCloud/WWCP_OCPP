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
    /// The GetInstalledSSHKeys response (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class GetInstalledSSHKeysResponse : AResponse<GetInstalledSSHKeysRequest,
                                                   GetInstalledSSHKeysResponse>,
                                              IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getInstalledSSHKeysResponse");

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

        /// <summary>
        /// The keys held, each with its station, purpose and fingerprint.
        /// </summary>
        [Optional]
        public IEnumerable<InstalledSSHKey>  Keys  { get; }

        /// <summary>
        /// The SHA-256 fingerprints of the Local Controller's host keys.
        /// </summary>
        [Optional]
        public IEnumerable<String>  HostKeys  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a GetInstalledSSHKeys response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">Whether the request was taken.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="Keys">The keys held, each with its station, purpose and fingerprint.</param>
        /// <param name="HostKeys">The SHA-256 fingerprints of the Local Controller's host keys.</param>
        public GetInstalledSSHKeysResponse(GetInstalledSSHKeysRequest              Request,
                                GenericStatus                Status,
                                StatusInfo?                  StatusInfo            = null,
                                IEnumerable<InstalledSSHKey> Keys = null,
                                IEnumerable<String>          HostKeys = null,

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
            this.Keys = Keys?.Distinct() ?? [];
            this.HostKeys = HostKeys?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 23 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 29 ^
                           this.Keys.Aggregate(0, (sum, item) => sum ^ item.GetHashCode()) * 3 ^
                           this.HostKeys.Aggregate(0, (sum, item) => sum ^ item.GetHashCode()) * 5 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetInstalledSSHKeys response.
        /// </summary>
        public static GetInstalledSSHKeysResponse Parse(GetInstalledSSHKeysRequest                                 Request,
                                                  JObject                                     JSON,
                                                  SourceRouting                               Destination,
                                                  NetworkPath                                 NetworkPath,
                                                  DateTimeOffset?                             ResponseTimestamp            = null,
                                                  CustomJObjectParserDelegate<GetInstalledSSHKeysResponse>?  CustomGetInstalledSSHKeysResponseParser    = null,
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
                         CustomGetInstalledSSHKeysResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return response;
            }

            throw new ArgumentException("The given JSON representation of a GetInstalledSSHKeys response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out GetInstalledSSHKeysResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetInstalledSSHKeys response.
        /// </summary>
        public static Boolean TryParse(GetInstalledSSHKeysRequest                                 Request,
                                       JObject                                     JSON,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out GetInstalledSSHKeysResponse?      GetInstalledSSHKeysResponse,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       DateTimeOffset?                             ResponseTimestamp           = null,
                                       CustomJObjectParserDelegate<GetInstalledSSHKeysResponse>?  CustomGetInstalledSSHKeysResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?    CustomStatusInfoParser      = null,
                                       CustomJObjectParserDelegate<Signature>?     CustomSignatureParser       = null,
                                       CustomJObjectParserDelegate<CustomData>?    CustomCustomDataParser      = null)
        {

            try
            {

                GetInstalledSSHKeysResponse = null;

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

                #region Keys                 [optional]

                var Keys = new List<InstalledSSHKey>();

                if (JSON["keys"] is JToken keysToken && keysToken.Type != JTokenType.Null)
                {

                    if (keysToken is not JArray keysArray)
                    {
                        ErrorResponse = "The given 'keys' must be an array!";
                        return false;
                    }

                    foreach (var item in keysArray)
                    {
                        if (item is not JObject itemJSON || !InstalledSSHKey.TryParse(itemJSON, out var installed, out ErrorResponse))
                        {
                            ErrorResponse ??= "An element of 'keys' is no JSON object!";
                            return false;
                        }
                        Keys.Add(installed);
                    }

                }

                #endregion

                #region HostKeys             [optional]

                var HostKeys = new List<String>();

                if (JSON["hostKeys"] is JToken hostKeysToken && hostKeysToken.Type != JTokenType.Null)
                {

                    if (hostKeysToken is not JArray hostKeysArray || hostKeysArray.Any(item => item.Type != JTokenType.String))
                    {
                        ErrorResponse = "The given 'hostKeys' must be an array of strings!";
                        return false;
                    }

                    HostKeys.AddRange(hostKeysArray.Values<String>().OfType<String>());

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


                GetInstalledSSHKeysResponse = new GetInstalledSSHKeysResponse(

                                   Request,
                                   Status,
                                   StatusInfo,
                                   Keys,
                                   HostKeys,

                                   null,
                                   ResponseTimestamp,

                                   Destination,
                                   NetworkPath,

                                   null,
                                   null,
                                   Signatures,

                                   CustomData

                               );

                if (CustomGetInstalledSSHKeysResponseParser is not null)
                    GetInstalledSSHKeysResponse = CustomGetInstalledSSHKeysResponseParser(JSON,
                                                                          GetInstalledSSHKeysResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetInstalledSSHKeysResponse   = null;
                ErrorResponse  = "The given JSON representation of a GetInstalledSSHKeys response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomGetInstalledSSHKeysResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomGetInstalledSSHKeysResponseSerializer">A delegate to serialize custom GetInstalledSSHKeys responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                         IncludeJSONLDContext               = false,
                              CustomJObjectSerializerDelegate<GetInstalledSSHKeysResponse>?  CustomGetInstalledSSHKeysResponseSerializer   = null,
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

                           Keys.Any()
                               ? new JProperty("keys",  new JArray(Keys.Select(key => key.ToJSON())))
                               : null,

                           HostKeys.Any()
                               ? new JProperty("hostKeys",  new JArray(HostKeys))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetInstalledSSHKeysResponseSerializer is not null
                       ? CustomGetInstalledSSHKeysResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetInstalledSSHKeys failed because of a request error.
        /// </summary>
        public static GetInstalledSSHKeysResponse RequestError(GetInstalledSSHKeysRequest          Request,
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
                   null,
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
        /// The GetInstalledSSHKeys failed: its data did not take the form it must.
        /// </summary>
        public static GetInstalledSSHKeysResponse FormationViolation(GetInstalledSSHKeysRequest  Request,
                                                               String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetInstalledSSHKeys failed: a signature was not valid.
        /// </summary>
        public static GetInstalledSSHKeysResponse SignatureError(GetInstalledSSHKeysRequest  Request,
                                                           String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetInstalledSSHKeys failed.
        /// </summary>
        public static GetInstalledSSHKeysResponse Failed(GetInstalledSSHKeysRequest  Request,
                                                   String?          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The GetInstalledSSHKeys failed because of an exception.
        /// </summary>
        public static GetInstalledSSHKeysResponse ExceptionOccurred(GetInstalledSSHKeysRequest  Request,
                                                              Exception        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two GetInstalledSSHKeys responses for equality.
        /// </summary>
        public static Boolean operator == (GetInstalledSSHKeysResponse? GetInstalledSSHKeysResponse1,
                                           GetInstalledSSHKeysResponse? GetInstalledSSHKeysResponse2)

            => ReferenceEquals(GetInstalledSSHKeysResponse1, GetInstalledSSHKeysResponse2) ||
               (GetInstalledSSHKeysResponse1 is not null && GetInstalledSSHKeysResponse2 is not null && GetInstalledSSHKeysResponse1.Equals(GetInstalledSSHKeysResponse2));

        /// <summary>
        /// Compares two GetInstalledSSHKeys responses for inequality.
        /// </summary>
        public static Boolean operator != (GetInstalledSSHKeysResponse? GetInstalledSSHKeysResponse1,
                                           GetInstalledSSHKeysResponse? GetInstalledSSHKeysResponse2)

            => !(GetInstalledSSHKeysResponse1 == GetInstalledSSHKeysResponse2);

        #endregion

        #region IEquatable<GetInstalledSSHKeysResponse> Members

        /// <summary>
        /// Compares two GetInstalledSSHKeys responses for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is GetInstalledSSHKeysResponse response &&
                   Equals(response);

        /// <summary>
        /// Compares two GetInstalledSSHKeys responses for equality.
        /// </summary>
        public override Boolean Equals(GetInstalledSSHKeysResponse? GetInstalledSSHKeysResponse)

            => GetInstalledSSHKeysResponse is not null &&

               Status.Equals(GetInstalledSSHKeysResponse.Status) &&

             ((StatusInfo is     null && GetInstalledSSHKeysResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetInstalledSSHKeysResponse.StatusInfo is not null && StatusInfo.Equals(GetInstalledSSHKeysResponse.StatusInfo)) &&

               Keys.SequenceEqual(GetInstalledSSHKeysResponse.Keys) &&
               HostKeys.SequenceEqual(GetInstalledSSHKeysResponse.HostKeys) &&

               base.GenericEquals(GetInstalledSSHKeysResponse);

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
