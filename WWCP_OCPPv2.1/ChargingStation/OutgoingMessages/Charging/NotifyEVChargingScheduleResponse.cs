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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A notify EV charging schedule response.
    /// </summary>
    public class NotifyEVChargingScheduleResponse : AResponse<CS.NotifyEVChargingScheduleRequest,
                                                                 NotifyEVChargingScheduleResponse>,
                                                    IResponse
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

        #region NotifyEVChargingScheduleResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new notify EV charging schedule response.
        /// </summary>
        /// <param name="Request">The notify EV charging schedule request leading to this response.</param>
        /// <param name="Status">Whether the CSMS has been able to process the message successfully. It does not imply any approval of the charging schedule.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyEVChargingScheduleResponse(CS.NotifyEVChargingScheduleRequest  Request,
                                                GenericStatus                       Status,
                                                StatusInfo?                         StatusInfo          = null,
                                                DateTime?                           ResponseTimestamp   = null,

                                                IEnumerable<KeyPair>?               SignKeys            = null,
                                                IEnumerable<SignInfo>?              SignInfos           = null,
                                                IEnumerable<OCPP.Signature>?        Signatures          = null,

                                                CustomData?                         CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region NotifyEVChargingScheduleResponse(Request, Result)

        /// <summary>
        /// Create a new notify EV charging schedule response.
        /// </summary>
        /// <param name="Request">The notify EV charging schedule request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyEVChargingScheduleResponse(CS.NotifyEVChargingScheduleRequest  Request,
                                                Result                              Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyEVChargingScheduleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify EV charging schedule response.
        /// </summary>
        /// <param name="Request">The notify EV charging schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyEVChargingScheduleResponseParser">A delegate to parse custom notify EV charging schedule responses.</param>
        public static NotifyEVChargingScheduleResponse Parse(CS.NotifyEVChargingScheduleRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyEVChargingScheduleResponse,
                         out var errorResponse,
                         CustomNotifyEVChargingScheduleResponseParser))
            {
                return notifyEVChargingScheduleResponse;
            }

            throw new ArgumentException("The given JSON representation of a notify EV charging schedule response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyEVChargingScheduleResponse, out ErrorResponse, CustomNotifyEVChargingScheduleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging schedule response.
        /// </summary>
        /// <param name="Request">The notify EV charging schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyEVChargingScheduleResponse">The parsed notify EV charging schedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingScheduleResponseParser">A delegate to parse custom notify EV charging schedule responses.</param>
        public static Boolean TryParse(CS.NotifyEVChargingScheduleRequest                              Request,
                                       JObject                                                         JSON,
                                       [NotNullWhen(true)]  out NotifyEVChargingScheduleResponse?      NotifyEVChargingScheduleResponse,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseParser   = null)
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
                                           OCPP.StatusInfo.TryParse,
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
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


                NotifyEVChargingScheduleResponse = new NotifyEVChargingScheduleResponse(
                                                       Request,
                                                       Status,
                                                       StatusInfo,
                                                       null,
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
                ErrorResponse                     = "The given JSON representation of a notify EV charging schedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingScheduleResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingScheduleResponseSerializer">A delegate to serialize custom notify EV charging schedule responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingScheduleResponse>?  CustomNotifyEVChargingScheduleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                    CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
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

            return CustomNotifyEVChargingScheduleResponseSerializer is not null
                       ? CustomNotifyEVChargingScheduleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify EV charging schedule response failed.
        /// </summary>
        /// <param name="Request">The notify EV charging schedule request leading to this response.</param>
        public static NotifyEVChargingScheduleResponse Failed(CS.NotifyEVChargingScheduleRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingScheduleResponse1, NotifyEVChargingScheduleResponse2)

        /// <summary>
        /// Compares two notify EV charging schedule responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleResponse1">A notify EV charging schedule response.</param>
        /// <param name="NotifyEVChargingScheduleResponse2">Another notify EV charging schedule response.</param>
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
        /// Compares two notify EV charging schedule responses for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleResponse1">A notify EV charging schedule response.</param>
        /// <param name="NotifyEVChargingScheduleResponse2">Another notify EV charging schedule response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse1,
                                           NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse2)

            => !(NotifyEVChargingScheduleResponse1 == NotifyEVChargingScheduleResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingScheduleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify EV charging schedule responses for equality.
        /// </summary>
        /// <param name="Object">A notify EV charging schedule response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingScheduleResponse notifyEVChargingScheduleResponse &&
                   Equals(notifyEVChargingScheduleResponse);

        #endregion

        #region Equals(NotifyEVChargingScheduleResponse)

        /// <summary>
        /// Compares two notify EV charging schedule responses for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleResponse">A notify EV charging schedule response to compare with.</param>
        public override Boolean Equals(NotifyEVChargingScheduleResponse? NotifyEVChargingScheduleResponse)

            => NotifyEVChargingScheduleResponse is not null &&

               Status.     Equals(NotifyEVChargingScheduleResponse.Status) &&

             ((StatusInfo is     null && NotifyEVChargingScheduleResponse.StatusInfo is     null) ||
               StatusInfo is not null && NotifyEVChargingScheduleResponse.StatusInfo is not null && StatusInfo.Equals(NotifyEVChargingScheduleResponse.StatusInfo)) &&

               base.GenericEquals(NotifyEVChargingScheduleResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

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
