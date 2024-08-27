///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System.Diagnostics.CodeAnalysis;

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
//{

//    /// <summary>
//    /// The NotifyNetworkTopology response.
//    /// </summary>
//    public class NotifyNetworkTopologyResponse : AResponse<NotifyNetworkTopologyRequest,
//                                                           NotifyNetworkTopologyResponse>,
//                                                 IResponse
//    {

//        #region Data

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/nn/notifyNetworkTopologyResponse");

//        #endregion

//        #region Properties

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public JSONLDContext Context
//            => DefaultJSONLDContext;

//        /// <summary>
//        /// The status of the NotifyNetworkTopology request.
//        /// </summary>
//        public NetworkTopologyStatus  Status        { get; }

//        /// <summary>
//        /// An optional element providing more information about the response status.
//        /// </summary>
//        [Optional]
//        public StatusInfo?            StatusInfo    { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new NotifyNetworkTopology response.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request leading to this response.</param>
//        /// <param name="Status">The status of the NotifyNetworkTopology request.</param>
//        /// 
//        /// <param name="Result">The machine-readable result code.</param>
//        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
//        /// 
//        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
//        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
//        /// 
//        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
//        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
//        /// 
//        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
//        public NotifyNetworkTopologyResponse(NotifyNetworkTopologyRequest  Request,
//                                             NetworkTopologyStatus         Status,
//                                             StatusInfo?                   StatusInfo            = null,

//                                             Result?                       Result                = null,
//                                             DateTime?                     ResponseTimestamp     = null,

//                                             SourceRouting?                Destination           = null,
//                                             NetworkPath?                  NetworkPath           = null,

//                                             IEnumerable<KeyPair>?         SignKeys              = null,
//                                             IEnumerable<SignInfo>?        SignInfos             = null,
//                                             IEnumerable<Signature>?       Signatures            = null,

//                                             CustomData?                   CustomData            = null,

//                                             SerializationFormats?         SerializationFormat   = null,
//                                             CancellationToken             CancellationToken     = default)

//            : base(Request,
//                   Result ?? Result.OK(),
//                   ResponseTimestamp,

//                   Destination,
//                   NetworkPath,

//                   SignKeys,
//                   SignInfos,
//                   Signatures,

//                   CustomData,

//                   SerializationFormat ?? SerializationFormats.JSON,
//                   CancellationToken)

//        {

//            this.Status      = Status;
//            this.StatusInfo  = StatusInfo;

//            unchecked
//            {

//                hashCode = this.Status.     GetHashCode()       * 5 ^
//                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
//                           base.GetHashCode();

//            }

//        }

//        #endregion


//        #region Documentation

//        // tba.

//        #endregion

//        #region (static) Parse   (Request, JSON, CustomNotifyNetworkTopologyResponseParser = null)

//        /// <summary>
//        /// Parse the given JSON representation of a NotifyNetworkTopology response.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request leading to this response.</param>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="CustomNotifyNetworkTopologyResponseParser">An optional delegate to parse custom NotifyNetworkTopology responses.</param>
//        public static NotifyNetworkTopologyResponse Parse(NotifyNetworkTopologyRequest                                 Request,
//                                                          JObject                                                      JSON,
//                                                          SourceRouting                                            Destination,
//                                                          NetworkPath                                                  NetworkPath,
//                                                          DateTime?                                                    ResponseTimestamp                           = null,
//                                                          CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseParser   = null,
//                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
//                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
//                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
//        {

//            if (TryParse(Request,
//                         JSON,
//                         Destination,
//                         NetworkPath,
//                         out var notifyNetworkTopologyResponse,
//                         out var errorResponse,
//                         ResponseTimestamp,
//                         CustomNotifyNetworkTopologyResponseParser,
//                         CustomStatusInfoParser,
//                         CustomSignatureParser,
//                         CustomCustomDataParser))
//            {
//                return notifyNetworkTopologyResponse;
//            }

//            throw new ArgumentException("The given JSON representation of a NotifyNetworkTopology response is invalid: " + errorResponse,
//                                        nameof(JSON));

//        }

//        #endregion

//        #region (static) TryParse(Request, JSON, out NotifyNetworkTopologyResponse, out ErrorResponse, CustomNotifyNetworkTopologyResponseParser = null)

//        /// <summary>
//        /// Try to parse the given JSON representation of a NotifyNetworkTopology response.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request leading to this response.</param>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="NotifyNetworkTopologyResponse">The parsed NotifyNetworkTopology response.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        /// <param name="CustomNotifyNetworkTopologyResponseParser">An optional delegate to parse custom NotifyNetworkTopology responses.</param>
//        public static Boolean TryParse(NotifyNetworkTopologyRequest                                 Request,
//                                       JObject                                                      JSON,
//                                       SourceRouting                                            Destination,
//                                       NetworkPath                                                  NetworkPath,
//                                       [NotNullWhen(true)]  out NotifyNetworkTopologyResponse?      NotifyNetworkTopologyResponse,
//                                       [NotNullWhen(false)] out String?                             ErrorResponse,
//                                       DateTime?                                                    ResponseTimestamp                           = null,
//                                       CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseParser   = null,
//                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
//                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
//                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
//        {

//            ErrorResponse = null;

//            try
//            {

//                NotifyNetworkTopologyResponse = null;

//                #region Status        [optional]

//                if (!JSON.ParseMandatory("status",
//                                         "status",
//                                         NetworkTopologyStatus.TryParse,
//                                         out NetworkTopologyStatus Status,
//                                         out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region StatusInfo    [optional]

//                if (JSON.ParseOptionalJSON("statusInfo",
//                                           "detailed status info",
//                                           OCPPv2_1.StatusInfo.TryParse,
//                                           out StatusInfo? StatusInfo,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region Signatures    [optional, OCPP_CSE]

//                if (JSON.ParseOptionalHashSet("signatures",
//                                              "cryptographic signatures",
//                                              Signature.TryParse,
//                                              out HashSet<Signature> Signatures,
//                                              out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region CustomData    [optional]

//                if (JSON.ParseOptionalJSON("customData",
//                                           "custom data",
//                                           WWCP.CustomData.TryParse,
//                                           out CustomData CustomData,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion


//                NotifyNetworkTopologyResponse = new NotifyNetworkTopologyResponse(

//                                                    Request,
//                                                    Status,
//                                                    StatusInfo,

//                                                    null,
//                                                    ResponseTimestamp,

//                                                    Destination,
//                                                    NetworkPath,

//                                                    null,
//                                                    null,
//                                                    Signatures,

//                                                    CustomData

//                                                );

//                if (CustomNotifyNetworkTopologyResponseParser is not null)
//                    NotifyNetworkTopologyResponse = CustomNotifyNetworkTopologyResponseParser(JSON,
//                                                                                              NotifyNetworkTopologyResponse);

//                return true;

//            }
//            catch (Exception e)
//            {
//                NotifyNetworkTopologyResponse  = null;
//                ErrorResponse                  = "The given JSON representation of a NotifyNetworkTopology response is invalid: " + e.Message;
//                return false;
//            }

//        }

//        #endregion

//        #region ToJSON(CustomNotifyNetworkTopologyResponseSerializer = null, CustomSignatureSerializer = null, ...)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomNotifyNetworkTopologyResponseSerializer">A delegate to serialize custom NotifyNetworkTopology responses.</param>
//        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
//        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
//        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
//        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseSerializer   = null,
//                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
//                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
//                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
//        {

//            var json = JSONObject.Create(

//                                 new JProperty("status",       Status.ToString()),

//                           Signatures.Any()
//                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
//                                                                                                                          CustomCustomDataSerializer))))
//                               : null,

//                           CustomData is not null
//                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
//                               : null

//                       );

//            return CustomNotifyNetworkTopologyResponseSerializer is not null
//                       ? CustomNotifyNetworkTopologyResponseSerializer(this, json)
//                       : json;

//        }

//        #endregion


//        #region Static methods

//        /// <summary>
//        /// The NotifyNetworkTopology failed because of a request error.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request.</param>
//        public static NotifyNetworkTopologyResponse RequestError(NotifyNetworkTopologyRequest  Request,
//                                                                 EventTracking_Id              EventTrackingId,
//                                                                 ResultCode                    ErrorCode,
//                                                                 String?                       ErrorDescription    = null,
//                                                                 JObject?                      ErrorDetails        = null,
//                                                                 DateTime?                     ResponseTimestamp   = null,

//                                                                 SourceRouting?                Destination         = null,
//                                                                 NetworkPath?                  NetworkPath         = null,

//                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
//                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
//                                                                 IEnumerable<Signature>?       Signatures          = null,

//                                                                 CustomData?                   CustomData          = null)

//            => new (

//                   Request,
//                   NetworkTopologyStatus.Error,
//                   null,
//                  OCPPv2_1.Result.FromErrorResponse(
//                       ErrorCode,
//                       ErrorDescription,
//                       ErrorDetails
//                   ),
//                   ResponseTimestamp,

//                   Destination,
//                   NetworkPath,

//                   SignKeys,
//                   SignInfos,
//                   Signatures,

//                   CustomData

//               );


//        /// <summary>
//        /// The NotifyNetworkTopology failed.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request.</param>
//        /// <param name="ErrorDescription">An optional error description.</param>
//        public static NotifyNetworkTopologyResponse FormationViolation(NotifyNetworkTopologyRequest  Request,
//                                                                       String                        ErrorDescription)

//            => new (Request,
//                    NetworkTopologyStatus.Error,
//                    Result:  OCPPv2_1.Result.FormationViolation(
//                                 $"Invalid data format: {ErrorDescription}"
//                             ));


//        /// <summary>
//        /// The NotifyNetworkTopology failed.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request.</param>
//        /// <param name="ErrorDescription">An optional error description.</param>
//        public static NotifyNetworkTopologyResponse SignatureError(NotifyNetworkTopologyRequest  Request,
//                                                                   String                        ErrorDescription)

//            => new (Request,
//                    NetworkTopologyStatus.Error,
//                    Result:  OCPPv2_1.Result.SignatureError(
//                                 $"Invalid signature(s): {ErrorDescription}"
//                             ));


//        /// <summary>
//        /// The NotifyNetworkTopology failed.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request.</param>
//        /// <param name="Description">An optional error description.</param>
//        public static NotifyNetworkTopologyResponse Failed(NotifyNetworkTopologyRequest  Request,
//                                                           String?                       Description   = null)

//            => new (Request,
//                    NetworkTopologyStatus.Error,
//                    Result:  OCPPv2_1.Result.Server(Description));


//        /// <summary>
//        /// The NotifyNetworkTopology failed because of an exception.
//        /// </summary>
//        /// <param name="Request">The NotifyNetworkTopology request.</param>
//        /// <param name="Exception">The exception.</param>
//        public static NotifyNetworkTopologyResponse ExceptionOccured(NotifyNetworkTopologyRequest  Request,
//                                                                     Exception                     Exception)

//            => new (Request,
//                    NetworkTopologyStatus.Error,
//                    Result:  OCPPv2_1.Result.FromException(Exception));

//        #endregion


//        #region Operator overloading

//        #region Operator == (NotifyNetworkTopologyResponse1, NotifyNetworkTopologyResponse2)

//        /// <summary>
//        /// Compares two NotifyNetworkTopology responses for equality.
//        /// </summary>
//        /// <param name="NotifyNetworkTopologyResponse1">A NotifyNetworkTopology response.</param>
//        /// <param name="NotifyNetworkTopologyResponse2">Another NotifyNetworkTopology response.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public static Boolean operator == (NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse1,
//                                           NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(NotifyNetworkTopologyResponse1, NotifyNetworkTopologyResponse2))
//                return true;

//            // If one is null, but not both, return false.
//            if (NotifyNetworkTopologyResponse1 is null || NotifyNetworkTopologyResponse2 is null)
//                return false;

//            return NotifyNetworkTopologyResponse1.Equals(NotifyNetworkTopologyResponse2);

//        }

//        #endregion

//        #region Operator != (NotifyNetworkTopologyResponse1, NotifyNetworkTopologyResponse2)

//        /// <summary>
//        /// Compares two NotifyNetworkTopology responses for inequality.
//        /// </summary>
//        /// <param name="NotifyNetworkTopologyResponse1">A NotifyNetworkTopology response.</param>
//        /// <param name="NotifyNetworkTopologyResponse2">Another NotifyNetworkTopology response.</param>
//        /// <returns>False if both match; True otherwise.</returns>
//        public static Boolean operator != (NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse1,
//                                           NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse2)

//            => !(NotifyNetworkTopologyResponse1 == NotifyNetworkTopologyResponse2);

//        #endregion

//        #endregion

//        #region IEquatable<NotifyNetworkTopologyResponse> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two NotifyNetworkTopology responses for equality.
//        /// </summary>
//        /// <param name="Object">A NotifyNetworkTopology response to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is NotifyNetworkTopologyResponse notifyNetworkTopologyResponse &&
//                   Equals(notifyNetworkTopologyResponse);

//        #endregion

//        #region Equals(NotifyNetworkTopologyResponse)

//        /// <summary>
//        /// Compares two NotifyNetworkTopology responses for equality.
//        /// </summary>
//        /// <param name="NotifyNetworkTopologyResponse">A NotifyNetworkTopology response to compare with.</param>
//        public override Boolean Equals(NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse)

//            => NotifyNetworkTopologyResponse is not null &&
//                   base.GenericEquals(NotifyNetworkTopologyResponse);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        private readonly Int32 hashCode;

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => hashCode;

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => "NotifyNetworkTopologyResponse";

//        #endregion

//    }

//}
