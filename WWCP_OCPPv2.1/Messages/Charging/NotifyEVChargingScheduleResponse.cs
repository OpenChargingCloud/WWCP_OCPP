﻿/*
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyEVChargingSchedule response.
    /// </summary>
    public class NotifyEVChargingScheduleResponse : AResponse<NotifyEVChargingScheduleRequest,
                                                              NotifyEVChargingScheduleResponse>,
                                                    IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyEVChargingScheduleResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the CSMS has been able to process the message successfully.
        /// It does not imply any approval of the charging schedule.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyEVChargingSchedule response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">The success or failure status of the NotifyEVChargingSchedule request.</param>
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NotifyEVChargingScheduleResponse(NotifyEVChargingScheduleRequest  Request,
                                                GenericStatus                    Status,
                                                StatusInfo?                      StatusInfo            = null,

                                                Result?                          Result                = null,
                                                DateTime?                        ResponseTimestamp     = null,

                                                SourceRouting?                   Destination           = null,
                                                NetworkPath?                     NetworkPath           = null,

                                                IEnumerable<KeyPair>?            SignKeys              = null,
                                                IEnumerable<SignInfo>?           SignInfos             = null,
                                                IEnumerable<Signature>?          Signatures            = null,

                                                CustomData?                      CustomData            = null,

                                                SerializationFormats?            SerializationFormat   = null,
                                                CancellationToken                CancellationToken     = default)

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

        #region (static) Parse   (Request, JSON, CustomNotifyEVChargingScheduleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyEVChargingSchedule response.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingSchedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyEVChargingScheduleResponseParser">A delegate to parse custom NotifyEVChargingSchedule responses.</param>
        public static NotifyEVChargingScheduleResponse Parse(NotifyEVChargingScheduleRequest                                 Request,
                                                             JObject                                                         JSON,
                                                             SourceRouting                                               Destination,
                                                             NetworkPath                                                     NetworkPath,
                                                             DateTime?                                                       ResponseTimestamp                              = null,
                                                             CustomJObjectParserDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseParser   = null,
                                                             CustomJObjectParserDelegate<StatusInfo>?                        CustomStatusInfoParser                         = null,
                                                             CustomJObjectParserDelegate<Signature>?                         CustomSignatureParser                          = null,
                                                             CustomJObjectParserDelegate<CustomData>?                        CustomCustomDataParser                         = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyEVChargingScheduleResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyEVChargingScheduleResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyEVChargingScheduleResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyEVChargingSchedule response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyEVChargingScheduleResponse, out ErrorResponse, CustomNotifyEVChargingScheduleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyEVChargingSchedule response.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingSchedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyEVChargingScheduleResponse">The parsed NotifyEVChargingSchedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingScheduleResponseParser">A delegate to parse custom NotifyEVChargingSchedule responses.</param>
        public static Boolean TryParse(NotifyEVChargingScheduleRequest                                 Request,
                                       JObject                                                         JSON,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out NotifyEVChargingScheduleResponse?      NotifyEVChargingScheduleResponse,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       DateTime?                                                       ResponseTimestamp                              = null,
                                       CustomJObjectParserDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                        CustomStatusInfoParser                         = null,
                                       CustomJObjectParserDelegate<Signature>?                         CustomSignatureParser                          = null,
                                       CustomJObjectParserDelegate<CustomData>?                        CustomCustomDataParser                         = null)
        {

            try
            {

                NotifyEVChargingScheduleResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
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


                NotifyEVChargingScheduleResponse = new NotifyEVChargingScheduleResponse(

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

                if (CustomNotifyEVChargingScheduleResponseParser is not null)
                    NotifyEVChargingScheduleResponse = CustomNotifyEVChargingScheduleResponseParser(JSON,
                                                                                                    NotifyEVChargingScheduleResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingScheduleResponse  = null;
                ErrorResponse                     = "The given JSON representation of a NotifyEVChargingSchedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingScheduleResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingScheduleResponseSerializer">A delegate to serialize custom NotifyEVChargingSchedule responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                             IncludeJSONLDContext                               = false,
                              CustomJObjectSerializerDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyEVChargingScheduleResponseSerializer is not null
                       ? CustomNotifyEVChargingScheduleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyEVChargingSchedule failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingSchedule request.</param>
        public static NotifyEVChargingScheduleResponse RequestError(NotifyEVChargingScheduleRequest  Request,
                                                                    EventTracking_Id                 EventTrackingId,
                                                                    ResultCode                       ErrorCode,
                                                                    String?                          ErrorDescription    = null,
                                                                    JObject?                         ErrorDetails        = null,
                                                                    DateTime?                        ResponseTimestamp   = null,

                                                                    SourceRouting?                   Destination         = null,
                                                                    NetworkPath?                     NetworkPath         = null,

                                                                    IEnumerable<KeyPair>?            SignKeys            = null,
                                                                    IEnumerable<SignInfo>?           SignInfos           = null,
                                                                    IEnumerable<Signature>?          Signatures          = null,

                                                                    CustomData?                      CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
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
        /// The NotifyEVChargingSchedule failed.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingSchedule request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyEVChargingScheduleResponse FormationViolation(NotifyEVChargingScheduleRequest  Request,
                                                                          String                           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyEVChargingSchedule failed.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingSchedule request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyEVChargingScheduleResponse SignatureError(NotifyEVChargingScheduleRequest  Request,
                                                                      String                           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyEVChargingSchedule failed.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingSchedule request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyEVChargingScheduleResponse Failed(NotifyEVChargingScheduleRequest  Request,
                                                              String?                          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyEVChargingSchedule failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyEVChargingSchedule request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyEVChargingScheduleResponse ExceptionOccured(NotifyEVChargingScheduleRequest  Request,
                                                                        Exception                        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingScheduleResponse1, NotifyEVChargingScheduleResponse2)

        /// <summary>
        /// Compares two NotifyEVChargingSchedule responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleResponse1">A NotifyEVChargingSchedule response.</param>
        /// <param name="NotifyEVChargingScheduleResponse2">Another NotifyEVChargingSchedule response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse1,
                                           NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingScheduleResponse1, NotifyEVChargingScheduleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingScheduleResponse1 is null || NotifyEVChargingScheduleResponse2 is null)
                return false;

            return NotifyEVChargingScheduleResponse1.Equals(NotifyEVChargingScheduleResponse2);

        }

        #endregion

        #region Operator != (NotifyEVChargingScheduleResponse1, NotifyEVChargingScheduleResponse2)

        /// <summary>
        /// Compares two NotifyEVChargingSchedule responses for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleResponse1">A NotifyEVChargingSchedule response.</param>
        /// <param name="NotifyEVChargingScheduleResponse2">Another NotifyEVChargingSchedule response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse1,
                                           NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse2)

            => !(NotifyEVChargingScheduleResponse1 == NotifyEVChargingScheduleResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingScheduleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyEVChargingSchedule responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyEVChargingSchedule response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingScheduleResponse notifyEVChargingScheduleResponse &&
                   Equals(notifyEVChargingScheduleResponse);

        #endregion

        #region Equals(NotifyEVChargingScheduleResponse)

        /// <summary>
        /// Compares two NotifyEVChargingSchedule responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleResponse">A NotifyEVChargingSchedule response to compare with.</param>
        public override Boolean Equals(NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse)

            => NotifyEVChargingScheduleResponse is not null &&

               Status.     Equals(NotifyEVChargingScheduleResponse.Status) &&

             ((StatusInfo is     null && NotifyEVChargingScheduleResponse.StatusInfo is     null) ||
               StatusInfo is not null && NotifyEVChargingScheduleResponse.StatusInfo is not null && StatusInfo.Equals(NotifyEVChargingScheduleResponse.StatusInfo)) &&

               base.GenericEquals(NotifyEVChargingScheduleResponse);

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
