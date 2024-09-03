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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyEVChargingNeeds response.
    /// </summary>
    public class NotifyEVChargingNeedsResponse : AResponse<NotifyEVChargingNeedsRequest,
                                                           NotifyEVChargingNeedsResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyEVChargingNeedsResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the CSMS has been able to process the message successfully.
        /// It does not imply that the EV charging needs can be met with the current charging profile.
        /// </summary>
        [Mandatory]
        public NotifyEVChargingNeedsStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                  StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyEVChargingNeeds response.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request leading to this response.</param>
        /// <param name="Status">Whether the CSMS has been able to process the message successfully. It does not imply that the EV charging needs can be met with the current charging profile.</param>
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
        public NotifyEVChargingNeedsResponse(NotifyEVChargingNeedsRequest  Request,
                                             NotifyEVChargingNeedsStatus   Status,
                                             StatusInfo?                   StatusInfo            = null,

                                             Result?                       Result                = null,
                                             DateTime?                     ResponseTimestamp     = null,

                                             SourceRouting?                Destination           = null,
                                             NetworkPath?                  NetworkPath           = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

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

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion



        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyEVChargingNeedsResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyEVChargingNeeds response.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyEVChargingNeedsResponseParser">A delegate to parse custom NotifyEVChargingNeeds responses.</param>
        public static NotifyEVChargingNeedsResponse Parse(NotifyEVChargingNeedsRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<NotifyEVChargingNeedsResponse>?  CustomNotifyEVChargingNeedsResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyEVChargingNeedsResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyEVChargingNeedsResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyEVChargingNeedsResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyEVChargingNeeds response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyEVChargingNeedsResponse, out ErrorResponse, CustomNotifyEVChargingNeedsResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyEVChargingNeeds response.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyEVChargingNeedsResponse">The parsed NotifyEVChargingNeeds response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingNeedsResponseParser">A delegate to parse custom NotifyEVChargingNeeds responses.</param>
        public static Boolean TryParse(NotifyEVChargingNeedsRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out NotifyEVChargingNeedsResponse?      NotifyEVChargingNeedsResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<NotifyEVChargingNeedsResponse>?  CustomNotifyEVChargingNeedsResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                NotifyEVChargingNeedsResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "NotifyEVChargingNeeds status",
                                         NotifyEVChargingNeedsStatusExtensions.TryParse,
                                         out NotifyEVChargingNeedsStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyEVChargingNeedsResponse = new NotifyEVChargingNeedsResponse(

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

                if (CustomNotifyEVChargingNeedsResponseParser is not null)
                    NotifyEVChargingNeedsResponse = CustomNotifyEVChargingNeedsResponseParser(JSON,
                                                                                              NotifyEVChargingNeedsResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingNeedsResponse  = null;
                ErrorResponse                  = "The given JSON representation of a NotifyEVChargingNeeds response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingNeedsResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingNeedsResponseSerializer">A delegate to serialize custom NotifyEVChargingNeeds responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingNeedsResponse>?  CustomNotifyEVChargingNeedsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

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

            return CustomNotifyEVChargingNeedsResponseSerializer is not null
                       ? CustomNotifyEVChargingNeedsResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyEVChargingNeeds failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request.</param>
        public static NotifyEVChargingNeedsResponse RequestError(NotifyEVChargingNeedsRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 SourceRouting?                Destination         = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

            => new (

                   Request,
                   NotifyEVChargingNeedsStatus.Rejected,
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
        /// The NotifyEVChargingNeeds failed.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyEVChargingNeedsResponse FormationViolation(NotifyEVChargingNeedsRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    NotifyEVChargingNeedsStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyEVChargingNeeds failed.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyEVChargingNeedsResponse SignatureError(NotifyEVChargingNeedsRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    NotifyEVChargingNeedsStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyEVChargingNeeds failed.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyEVChargingNeedsResponse Failed(NotifyEVChargingNeedsRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    NotifyEVChargingNeedsStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyEVChargingNeeds failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingNeeds request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyEVChargingNeedsResponse ExceptionOccured(NotifyEVChargingNeedsRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    NotifyEVChargingNeedsStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingNeedsResponse1, NotifyEVChargingNeedsResponse2)

        /// <summary>
        /// Compares two NotifyEVChargingNeeds responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsResponse1">A NotifyEVChargingNeeds response.</param>
        /// <param name="NotifyEVChargingNeedsResponse2">Another NotifyEVChargingNeeds response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse1,
                                           NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingNeedsResponse1, NotifyEVChargingNeedsResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingNeedsResponse1 is null || NotifyEVChargingNeedsResponse2 is null)
                return false;

            return NotifyEVChargingNeedsResponse1.Equals(NotifyEVChargingNeedsResponse2);

        }

        #endregion

        #region Operator != (NotifyEVChargingNeedsResponse1, NotifyEVChargingNeedsResponse2)

        /// <summary>
        /// Compares two NotifyEVChargingNeeds responses for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsResponse1">A NotifyEVChargingNeeds response.</param>
        /// <param name="NotifyEVChargingNeedsResponse2">Another NotifyEVChargingNeeds response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse1,
                                           NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse2)

            => !(NotifyEVChargingNeedsResponse1 == NotifyEVChargingNeedsResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingNeedsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyEVChargingNeeds responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyEVChargingNeeds response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingNeedsResponse notifyEVChargingNeedsResponse &&
                   Equals(notifyEVChargingNeedsResponse);

        #endregion

        #region Equals(NotifyEVChargingNeedsResponse)

        /// <summary>
        /// Compares two NotifyEVChargingNeeds responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsResponse">A NotifyEVChargingNeeds response to compare with.</param>
        public override Boolean Equals(NotifyEVChargingNeedsResponse? NotifyEVChargingNeedsResponse)

            => NotifyEVChargingNeedsResponse is not null &&

               Status.     Equals(NotifyEVChargingNeedsResponse.Status) &&

             ((StatusInfo is     null && NotifyEVChargingNeedsResponse.StatusInfo is     null) ||
               StatusInfo is not null && NotifyEVChargingNeedsResponse.StatusInfo is not null && StatusInfo.Equals(NotifyEVChargingNeedsResponse.StatusInfo)) &&

               base.GenericEquals(NotifyEVChargingNeedsResponse);

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

            => Status.ToString();

        #endregion

    }

}
