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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A GetExecutingEnvironment response.
    /// </summary>
    public class GetExecutingEnvironmentResponse : AResponse<GetExecutingEnvironmentRequest,
                                                             GetExecutingEnvironmentResponse>,
                                                   IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/getExecutingEnvironmentResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The response status.
        /// </summary>
        public GenericStatus  Status           { get; }

        /// <summary>
        /// The optional process identification of the main application process.
        /// </summary>
        public UInt64?        ProcessId        { get; }

        /// <summary>
        /// The optional URL that can be called to kill and restart the entire process, e.g. tcp://192.168.178.23:8123 or http://192.168.178.23:8123.
        /// </summary>
        public URL?           RestartURL       { get; }

        /// <summary>
        /// The optional long string acting as shared secret to kill and restart the entire process.
        /// The restartURL determines the actual usage, e.g. sending this string as TCP data or POSTing it to the given HTTP URL.
        /// </summary>
        public String?        RestartSecret    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetExecutingEnvironment response.
        /// </summary>
        /// <param name="Status">The response status.</param>
        /// <param name="Request">The GetExecutingEnvironment request leading to this response.</param>
        /// <param name="ProcessId">An optional process identification of the main application process.</param>
        /// <param name="RestartURL">An optional URL that can be called to kill and restart the entire process, e.g. tcp://192.168.178.23:8123 or http://192.168.178.23:8123.</param>
        /// <param name="RestartSecret">An optional long string acting as shared secret to kill and restart the entire process.</param>
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
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetExecutingEnvironmentResponse(GetExecutingEnvironmentRequest  Request,
                                               GenericStatus                   Status,
                                               UInt64?                         ProcessId             = null,
                                               URL?                            RestartURL            = null,
                                               String?                         RestartSecret         = null,

                                               Result?                         Result                = null,
                                               DateTime?                       ResponseTimestamp     = null,

                                               SourceRouting?                  Destination           = null,
                                               NetworkPath?                    NetworkPath           = null,

                                               IEnumerable<KeyPair>?           SignKeys              = null,
                                               IEnumerable<SignInfo>?          SignInfos             = null,
                                               IEnumerable<Signature>?         Signatures            = null,

                                               CustomData?                     CustomData            = null,

                                               SerializationFormats?           SerializationFormat   = null,
                                               CancellationToken               CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status         = Status;
            this.ProcessId      = ProcessId;
            this.RestartURL     = RestartURL;
            this.RestartSecret  = RestartSecret;

            unchecked
            {

                hashCode =  this.Status.        GetHashCode()       * 11 ^
                           (this.ProcessId?.    GetHashCode() ?? 0) *  7 ^
                           (this.RestartURL?.   GetHashCode() ?? 0) *  5 ^
                           (this.RestartSecret?.GetHashCode() ?? 0) *  3 ^
                            base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetExecutingEnvironment response.
        /// </summary>
        /// <param name="Request">The GetExecutingEnvironment request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetExecutingEnvironmentResponseParser">An optional delegate to parse custom GetExecutingEnvironment responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static GetExecutingEnvironmentResponse Parse(GetExecutingEnvironmentRequest                                 Request,
                                                            JObject                                                        JSON,
                                                            SourceRouting                                                  Destination,
                                                            NetworkPath                                                    NetworkPath,
                                                            DateTime?                                                      ResponseTimestamp                             = null,
                                                            CustomJObjectParserDelegate<GetExecutingEnvironmentResponse>?  CustomGetExecutingEnvironmentResponseParser   = null,
                                                            CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                                            CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getExecutingEnvironmentResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetExecutingEnvironmentResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getExecutingEnvironmentResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetExecutingEnvironment response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out GetExecutingEnvironmentResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetExecutingEnvironment response.
        /// </summary>
        /// <param name="Request">The GetExecutingEnvironment request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetExecutingEnvironmentResponse">The parsed GetExecutingEnvironment response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetExecutingEnvironmentResponseParser">An optional delegate to parse custom GetExecutingEnvironment responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(GetExecutingEnvironmentRequest                                 Request,
                                       JObject                                                        JSON,
                                       SourceRouting                                                  Destination,
                                       NetworkPath                                                    NetworkPath,
                                       [NotNullWhen(true)]  out GetExecutingEnvironmentResponse?      GetExecutingEnvironmentResponse,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       DateTime?                                                      ResponseTimestamp                             = null,
                                       CustomJObjectParserDelegate<GetExecutingEnvironmentResponse>?  CustomGetExecutingEnvironmentResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                       CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            try
            {

                GetExecutingEnvironmentResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatus.TryParse,
                                         out GenericStatus status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ProcessId        [optional]

                if (JSON.ParseOptional("processId",
                                       "process identification",
                                       out UInt64? processId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RestartURL       [optional]

                if (JSON.ParseOptional("restartURL",
                                       "restart URL",
                                       URL.TryParse,
                                       out URL? restartURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RestartSecret    [optional]

                if (JSON.ParseOptional("restartSecret",
                                       "restart secret",
                                       out String? restartSecret,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                GetExecutingEnvironmentResponse = new GetExecutingEnvironmentResponse(

                                                      Request,
                                                      status,
                                                      processId,
                                                      restartURL,
                                                      restartSecret,

                                                      null,
                                                      ResponseTimestamp,

                                                      Destination,
                                                      NetworkPath,

                                                      null,
                                                      null,
                                                      Signatures,

                                                      CustomData

                                                  );

                if (CustomGetExecutingEnvironmentResponseParser is not null)
                    GetExecutingEnvironmentResponse = CustomGetExecutingEnvironmentResponseParser(JSON,
                                                                                                  GetExecutingEnvironmentResponse);

                return true;

            }
            catch (Exception e)
            {
                GetExecutingEnvironmentResponse  = null;
                ErrorResponse                    = "The given JSON representation of a GetExecutingEnvironment response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetExecutingEnvironmentResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetExecutingEnvironmentResponseSerializer">A delegate to serialize custom GetExecutingEnvironment responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetExecutingEnvironmentResponse>?  CustomGetExecutingEnvironmentResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",          Status.ToString()),

                           ProcessId. HasValue
                               ? new JProperty("processId",       ProcessId. Value)
                               : null,

                           RestartURL.HasValue
                               ? new JProperty("restartURL",      RestartURL.Value)
                               : null,

                           RestartSecret.IsNotNullOrEmpty()
                               ? new JProperty("restartSecret",   RestartSecret)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetExecutingEnvironmentResponseSerializer is not null
                       ? CustomGetExecutingEnvironmentResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetExecutingEnvironment failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetExecutingEnvironment request leading to this response.</param>
        public static GetExecutingEnvironmentResponse RequestError(GetExecutingEnvironmentRequest  Request,
                                                                   EventTracking_Id                EventTrackingId,
                                                                   ResultCode                      ErrorCode,
                                                                   String?                         ErrorDescription    = null,
                                                                   JObject?                        ErrorDetails        = null,
                                                                   DateTime?                       ResponseTimestamp   = null,

                                                                   SourceRouting?                  Destination         = null,
                                                                   NetworkPath?                    NetworkPath         = null,

                                                                   IEnumerable<KeyPair>?           SignKeys            = null,
                                                                   IEnumerable<SignInfo>?          SignInfos           = null,
                                                                   IEnumerable<Signature>?         Signatures          = null,

                                                                   CustomData?                     CustomData          = null)

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
        /// The GetExecutingEnvironment failed.
        /// </summary>
        /// <param name="Request">The GetExecutingEnvironment request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetExecutingEnvironmentResponse FormationViolation(GetExecutingEnvironmentRequest  Request,
                                                                         String                          ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The GetExecutingEnvironment failed.
        /// </summary>
        /// <param name="Request">The GetExecutingEnvironment request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetExecutingEnvironmentResponse SignatureError(GetExecutingEnvironmentRequest  Request,
                                                                     String                          ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The GetExecutingEnvironment failed.
        /// </summary>
        /// <param name="Request">The GetExecutingEnvironment request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetExecutingEnvironmentResponse Failed(GetExecutingEnvironmentRequest  Request,
                                                             String?                         Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The GetExecutingEnvironment failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetExecutingEnvironment request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetExecutingEnvironmentResponse ExceptionOccurred(GetExecutingEnvironmentRequest  Request,
                                                                        Exception                       Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (GetExecutingEnvironmentResponse1, GetExecutingEnvironmentResponse2)

        /// <summary>
        /// Compares two GetExecutingEnvironment responses for equality.
        /// </summary>
        /// <param name="GetExecutingEnvironmentResponse1">A GetExecutingEnvironment response.</param>
        /// <param name="GetExecutingEnvironmentResponse2">Another GetExecutingEnvironment response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetExecutingEnvironmentResponse? GetExecutingEnvironmentResponse1,
                                           GetExecutingEnvironmentResponse? GetExecutingEnvironmentResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetExecutingEnvironmentResponse1, GetExecutingEnvironmentResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetExecutingEnvironmentResponse1 is null || GetExecutingEnvironmentResponse2 is null)
                return false;

            return GetExecutingEnvironmentResponse1.Equals(GetExecutingEnvironmentResponse2);

        }

        #endregion

        #region Operator != (GetExecutingEnvironmentResponse1, GetExecutingEnvironmentResponse2)

        /// <summary>
        /// Compares two GetExecutingEnvironment responses for inequality.
        /// </summary>
        /// <param name="GetExecutingEnvironmentResponse1">A GetExecutingEnvironment response.</param>
        /// <param name="GetExecutingEnvironmentResponse2">Another GetExecutingEnvironment response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetExecutingEnvironmentResponse? GetExecutingEnvironmentResponse1,
                                           GetExecutingEnvironmentResponse? GetExecutingEnvironmentResponse2)

            => !(GetExecutingEnvironmentResponse1 == GetExecutingEnvironmentResponse2);

        #endregion

        #endregion

        #region IEquatable<GetExecutingEnvironmentResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetExecutingEnvironment responses for equality.
        /// </summary>
        /// <param name="Object">A GetExecutingEnvironment response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetExecutingEnvironmentResponse getExecutingEnvironmentResponse &&
                   Equals(getExecutingEnvironmentResponse);

        #endregion

        #region Equals(GetExecutingEnvironmentResponse)

        /// <summary>
        /// Compares two GetExecutingEnvironment responses for equality.
        /// </summary>
        /// <param name="GetExecutingEnvironmentResponse">A GetExecutingEnvironment response to compare with.</param>
        public override Boolean Equals(GetExecutingEnvironmentResponse? GetExecutingEnvironmentResponse)

            => GetExecutingEnvironmentResponse is not null &&

               Status.Equals(GetExecutingEnvironmentResponse.Status) &&

              ((!ProcessId.   HasValue    && !GetExecutingEnvironmentResponse.ProcessId.    HasValue) ||
                (ProcessId.   HasValue    &&  GetExecutingEnvironmentResponse.ProcessId.    HasValue    && ProcessId. Value == GetExecutingEnvironmentResponse.ProcessId. Value)) &&

              ((!RestartURL.  HasValue    && !GetExecutingEnvironmentResponse.RestartURL.   HasValue) ||
                (RestartURL.  HasValue    &&  GetExecutingEnvironmentResponse.RestartURL.   HasValue    && RestartURL.Value == GetExecutingEnvironmentResponse.RestartURL.Value)) &&

               (RestartSecret is null     &&  GetExecutingEnvironmentResponse.RestartSecret is null ||
                RestartSecret is not null &&  GetExecutingEnvironmentResponse.RestartSecret is not null && RestartSecret    == GetExecutingEnvironmentResponse.RestartSecret);

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

                   Status,

                   ProcessId.HasValue
                       ? $", processId: {ProcessId.Value}"
                       : "",

                   RestartURL.HasValue
                       ? $", restartURL: {RestartURL.Value}"
                       : "",

                   RestartSecret.IsNotNullOrEmpty()
                       ? $", restartSecret: {RestartSecret}"
                       : ""

               );

        #endregion

    }

}
