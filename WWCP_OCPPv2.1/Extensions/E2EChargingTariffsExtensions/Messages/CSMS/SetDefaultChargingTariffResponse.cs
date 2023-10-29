/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using System.Collections.ObjectModel;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A set default charging tariff response.
    /// </summary>
    public class SetDefaultChargingTariffResponse : AResponse<CSMS.SetDefaultChargingTariffRequest,
                                                                   SetDefaultChargingTariffResponse>,
                                                    IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/bootNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                                            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The registration status.
        /// </summary>
        [Mandatory]
        public GenericStatus                                            Status         { get; }

        /// <summary>
        /// An optional element providing more information about the registration status.
        /// </summary>
        [Optional]
        public StatusInfo?                                              StatusInfo     { get; }

        public IReadOnlyDictionary<EVSE_Id, StatusInfo<GenericStatus>>  StatusInfos    { get; }

        #endregion

        #region Constructor(s)

        #region SetChargingTariffResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new set default charging tariff response.
        /// </summary>
        /// <param name="Request">The set default charging tariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetDefaultChargingTariffResponse(CSMS.SetDefaultChargingTariffRequest                     Request,
                                                GenericStatus                                            Status,
                                                IReadOnlyDictionary<EVSE_Id, StatusInfo<GenericStatus>>  StatusInfos,
                                                StatusInfo?                                              StatusInfo          = null,
                                                DateTime?                                                ResponseTimestamp   = null,

                                                IEnumerable<KeyPair>?                                    SignKeys            = null,
                                                IEnumerable<SignInfo>?                                   SignInfos           = null,
                                                IEnumerable<Signature>?                                  Signatures          = null,

                                                CustomData?                                              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status       = Status;
            this.StatusInfos  = StatusInfos;
            this.StatusInfo   = StatusInfo;
 
        }

        #endregion

        #region SetChargingTariffResponse(Request, Result)

        /// <summary>
        /// Create a new set default charging tariff response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public SetDefaultChargingTariffResponse(CSMS.SetDefaultChargingTariffRequest  Request,
                                                Result                                Result)

            : base(Request,
                   Result)

        {

            this.Status       = GenericStatus.Rejected;
            this.StatusInfos  = Request.EVSEIds.ToDictionary(
                                                    evseId => evseId,
                                                    evseId => new StatusInfo<GenericStatus>(GenericStatus.Rejected));

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomSetChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set default charging tariff response.
        /// </summary>
        /// <param name="Request">The set default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetChargingTariffResponseParser">A delegate to parse custom set default charging tariff responses.</param>
        public static SetDefaultChargingTariffResponse Parse(CSMS.SetDefaultChargingTariffRequest                            Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<SetDefaultChargingTariffResponse>?  CustomSetChargingTariffResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         CustomSetChargingTariffResponseParser))
            {
                return bootNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a set default charging tariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetChargingTariffResponse, out ErrorResponse, CustomSetChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a set default charging tariff response.
        /// </summary>
        /// <param name="Request">The set default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetChargingTariffResponse">The parsed set default charging tariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingTariffResponseParser">A delegate to parse custom set default charging tariff responses.</param>
        public static Boolean TryParse(CSMS.SetDefaultChargingTariffRequest                            Request,
                                       JObject                                                         JSON,
                                       out SetDefaultChargingTariffResponse?                           SetChargingTariffResponse,
                                       out String?                                                     ErrorResponse,
                                       CustomJObjectParserDelegate<SetDefaultChargingTariffResponse>?  CustomSetChargingTariffResponseParser   = null)
        {

            try
            {

                SetChargingTariffResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == GenericStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + (JSON["status"]?.Value<String>() ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region StatusInfos    [mandatory]

                if (!JSON.ParseMandatory("statusInfos",
                                         "status infos",
                                         out JObject StatusInfosJSON,
                                         out ErrorResponse))
                {
                    return false;
                }

                var StatusInfos = new Dictionary<EVSE_Id, StatusInfo<GenericStatus>>();

                foreach (var statusInfoProperty in StatusInfosJSON.Properties())
                {

                    var evseId = EVSE_Id.TryParse(statusInfoProperty.Name);

                    if (!evseId.HasValue)
                        continue;

                    if (statusInfoProperty.Value["status"] is not JObject statusInfoJObject)
                        continue;

                    if (!StatusInfo<GenericStatus>.TryParse(statusInfoJObject,
                                                            out var statusInfo,
                                                            out var errorResponse,
                                                            GenericStatusExtensions.TryParse,
                                                            null) ||
                        statusInfo is null)
                    {
                        continue;
                    }

                    StatusInfos.Add(evseId.Value, statusInfo);

                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetChargingTariffResponse = new SetDefaultChargingTariffResponse(

                                                Request,
                                                RegistrationStatus,
                                                new ReadOnlyDictionary<EVSE_Id, StatusInfo<GenericStatus>>(StatusInfos),
                                                StatusInfo,
                                                null,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData

                                            );

                if (CustomSetChargingTariffResponseParser is not null)
                    SetChargingTariffResponse = CustomSetChargingTariffResponseParser(JSON,
                                                                                    SetChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                SetChargingTariffResponse  = null;
                ErrorResponse             = "The given JSON representation of a set default charging tariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingTariffResponseSerializer">A delegate to serialize custom set default charging tariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDefaultChargingTariffResponse>?  CustomSetChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                  CustomCustomDataSerializer))
                               : null,

                                 new JProperty("statusInfos",   new JObject(StatusInfos.Select(statusInfo => new JProperty(
                                                                                                                 statusInfo.Key.  ToString(),
                                                                                                                 statusInfo.Value.ToJSON(status => status.AsText(),
                                                                                                                                         CustomStatusInfoSerializer,
                                                                                                                                         CustomCustomDataSerializer)
                                                                                                             )))),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray (Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                            CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetChargingTariffResponseSerializer is not null
                       ? CustomSetChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The set default charging tariff failed.
        /// </summary>
        public static SetDefaultChargingTariffResponse Failed(CSMS.SetDefaultChargingTariffRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingTariffResponse1, SetChargingTariffResponse2)

        /// <summary>
        /// Compares two set default charging tariff responses for equality.
        /// </summary>
        /// <param name="SetChargingTariffResponse1">A set default charging tariff response.</param>
        /// <param name="SetChargingTariffResponse2">Another set default charging tariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDefaultChargingTariffResponse? SetChargingTariffResponse1,
                                           SetDefaultChargingTariffResponse? SetChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetChargingTariffResponse1, SetChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetChargingTariffResponse1 is null || SetChargingTariffResponse2 is null)
                return false;

            return SetChargingTariffResponse1.Equals(SetChargingTariffResponse2);

        }

        #endregion

        #region Operator != (SetChargingTariffResponse1, SetChargingTariffResponse2)

        /// <summary>
        /// Compares two set default charging tariff responses for inequality.
        /// </summary>
        /// <param name="SetChargingTariffResponse1">A set default charging tariff response.</param>
        /// <param name="SetChargingTariffResponse2">Another set default charging tariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDefaultChargingTariffResponse? SetChargingTariffResponse1,
                                           SetDefaultChargingTariffResponse? SetChargingTariffResponse2)

            => !(SetChargingTariffResponse1 == SetChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<SetChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set default charging tariff responses for equality.
        /// </summary>
        /// <param name="Object">A set default charging tariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDefaultChargingTariffResponse bootNotificationResponse &&
                   Equals(bootNotificationResponse);

        #endregion

        #region Equals(SetChargingTariffResponse)

        /// <summary>
        /// Compares two set default charging tariff responses for equality.
        /// </summary>
        /// <param name="SetChargingTariffResponse">A set default charging tariff response to compare with.</param>
        public override Boolean Equals(SetDefaultChargingTariffResponse? SetChargingTariffResponse)

            => SetChargingTariffResponse is not null &&

               Status.Equals(SetChargingTariffResponse.Status) &&

             ((StatusInfo is     null && SetChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && SetChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(SetChargingTariffResponse.StatusInfo))) &&

               base.GenericEquals(SetChargingTariffResponse);

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

            => Status.AsText();

        #endregion

    }

}
