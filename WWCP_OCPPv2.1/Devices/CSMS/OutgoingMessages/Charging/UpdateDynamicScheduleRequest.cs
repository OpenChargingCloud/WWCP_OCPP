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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// An update dynamic schedule request.
    /// </summary>
    public class UpdateDynamicScheduleRequest : ARequest<UpdateDynamicScheduleRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/updateDynamicScheduleRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of the charging profile to update.
        /// </summary>
        [Mandatory]
        public ChargingProfile_Id  ChargingProfileId      { get; }


        /// <summary>
        /// Optional charging rate limit in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Limit                  { get; }

        /// <summary>
        /// Optional charging rate limit in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Limit_L2               { get; }

        /// <summary>
        /// Optional charging rate limit in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Limit_L3               { get; }


        /// <summary>
        /// Optional discharging limit in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  DischargeLimit         { get; }

        /// <summary>
        /// Optional discharging limit in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  DischargeLimit_L2      { get; }

        /// <summary>
        /// Optional discharging limit in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  DischargeLimit_L3      { get; }


        /// <summary>
        /// Optional setpoint in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Setpoint               { get; }

        /// <summary>
        /// Optional setpoint in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Setpoint_L2            { get; }

        /// <summary>
        /// Optional setpoint in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Setpoint_L3            { get; }


        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  SetpointReactive       { get; }

        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  SetpointReactive_L2    { get; }

        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  SetpointReactive_L3    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an update dynamic schedule request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="ChargingProfileId">The identification of the charging profile to update.</param>
        /// 
        /// <param name="Limit">Optional charging rate limit in chargingRateUnit.</param>
        /// <param name="Limit_L2">Optional charging rate limit in chargingRateUnit on phase L2.</param>
        /// <param name="Limit_L3">Optional charging rate limit in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="DischargeLimit">Optional discharging limit in chargingRateUnit.</param>
        /// <param name="DischargeLimit_L2">Optional discharging limit in chargingRateUnit on phase L2.</param>
        /// <param name="DischargeLimit_L3">Optional discharging limit in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="Setpoint">Optional setpoint in chargingRateUnit.</param>
        /// <param name="Setpoint_L2">Optional setpoint in chargingRateUnit on phase L2.</param>
        /// <param name="Setpoint_L3">Optional setpoint in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="SetpointReactive">Optional setpoint for reactive power (or current) in chargingRateUnit.</param>
        /// <param name="SetpointReactive_L2">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.</param>
        /// <param name="SetpointReactive_L3">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public UpdateDynamicScheduleRequest(NetworkingNode_Id        DestinationId,
                                            ChargingProfile_Id       ChargingProfileId,

                                            ChargingRateValue?       Limit                 = null,
                                            ChargingRateValue?       Limit_L2              = null,
                                            ChargingRateValue?       Limit_L3              = null,

                                            ChargingRateValue?       DischargeLimit        = null,
                                            ChargingRateValue?       DischargeLimit_L2     = null,
                                            ChargingRateValue?       DischargeLimit_L3     = null,

                                            ChargingRateValue?       Setpoint              = null,
                                            ChargingRateValue?       Setpoint_L2           = null,
                                            ChargingRateValue?       Setpoint_L3           = null,

                                            ChargingRateValue?       SetpointReactive      = null,
                                            ChargingRateValue?       SetpointReactive_L2   = null,
                                            ChargingRateValue?       SetpointReactive_L3   = null,

                                            IEnumerable<KeyPair>?    SignKeys              = null,
                                            IEnumerable<SignInfo>?   SignInfos             = null,
                                            IEnumerable<Signature>?       Signatures            = null,

                                            CustomData?              CustomData            = null,

                                            Request_Id?              RequestId             = null,
                                            DateTime?                RequestTimestamp      = null,
                                            TimeSpan?                RequestTimeout        = null,
                                            EventTracking_Id?        EventTrackingId       = null,
                                            NetworkPath?             NetworkPath           = null,
                                            CancellationToken        CancellationToken     = default)

            : base(DestinationId,
                   nameof(UpdateDynamicScheduleRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

           #region (Discharge)Limit checks

            if (Limit.HasValue && Limit.Value.Value < 0)
                throw new ArgumentException($"The given charging rate limit (for phase L1) {Limit.Value.Value} must not be negative!",
                                            nameof(Limit));

            if (Limit_L2.HasValue && Limit_L2.Value.Value < 0)
                throw new ArgumentException($"The given charging rate limit for phase L2 {Limit_L2.Value.Value} must not be negative!",
                                            nameof(Limit_L2));

            if (Limit_L3.HasValue && Limit_L3.Value.Value < 0)
                throw new ArgumentException($"The given charging rate limit for phase L3 {Limit_L3.Value.Value} must not be negative!",
                                            nameof(Limit_L3));


            if (DischargeLimit.HasValue && DischargeLimit.Value.Value > 0)
                throw new ArgumentException($"The given discharging rate limit (for phase L1) {DischargeLimit.Value.Value} must not be positive!",
                                            nameof(DischargeLimit));

            if (DischargeLimit_L2.HasValue && DischargeLimit_L2.Value.Value > 0)
                throw new ArgumentException($"The given discharging rate limit for phase L2 {DischargeLimit_L2.Value.Value} must not be positive!",
                                            nameof(DischargeLimit_L2));

            if (DischargeLimit_L3.HasValue && DischargeLimit_L3.Value.Value > 0)
                throw new ArgumentException($"The given discharging rate limit for phase L3 {DischargeLimit_L3.Value.Value} must not be positive!",
                                            nameof(DischargeLimit_L3));

            #endregion

            this.ChargingProfileId    = ChargingProfileId;

            this.Limit                = Limit;
            this.Limit_L2             = Limit_L2;
            this.Limit_L3             = Limit_L3;

            this.DischargeLimit       = DischargeLimit;
            this.DischargeLimit_L2    = DischargeLimit_L2;
            this.DischargeLimit_L3    = DischargeLimit_L3;

            this.Setpoint             = Setpoint;
            this.Setpoint_L2          = Setpoint_L2;
            this.Setpoint_L3          = Setpoint_L3;

            this.SetpointReactive     = SetpointReactive;
            this.SetpointReactive_L2  = SetpointReactive_L2;
            this.SetpointReactive_L3  = SetpointReactive_L3;


            unchecked
            {

                hashCode = ChargingProfileId.   GetHashCode()       * 47 ^

                          (Limit?.              GetHashCode() ?? 0) * 43 ^
                          (Limit_L2?.           GetHashCode() ?? 0) * 41 ^
                          (Limit_L3?.           GetHashCode() ?? 0) * 37 ^

                          (DischargeLimit?.     GetHashCode() ?? 0) * 31 ^
                          (DischargeLimit_L2?.  GetHashCode() ?? 0) * 29 ^
                          (DischargeLimit_L3?.  GetHashCode() ?? 0) * 23 ^

                          (Setpoint?.           GetHashCode() ?? 0) * 17 ^
                          (Setpoint_L2?.        GetHashCode() ?? 0) * 13 ^
                          (Setpoint_L3?.        GetHashCode() ?? 0) * 11 ^

                          (SetpointReactive?.   GetHashCode() ?? 0) *  7 ^
                          (SetpointReactive_L2?.GetHashCode() ?? 0) *  5 ^
                          (SetpointReactive_L3?.GetHashCode() ?? 0) *  3 ^

                           base.                GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId,  DestinationId, NetworkPath, CustomUpdateDynamicScheduleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an update dynamic schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomUpdateDynamicScheduleRequestParser">A delegate to parse custom update dynamic schedule requests.</param>
        public static UpdateDynamicScheduleRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         NetworkingNode_Id                                           DestinationId,
                                                         NetworkPath                                                 NetworkPath,
                                                         CustomJObjectParserDelegate<UpdateDynamicScheduleRequest>?  CustomUpdateDynamicScheduleRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var updateDynamicScheduleRequest,
                         out var errorResponse,
                         CustomUpdateDynamicScheduleRequestParser))
            {
                return updateDynamicScheduleRequest;
            }

            throw new ArgumentException("The given JSON representation of an update dynamic schedule request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId,  DestinationId, NetworkPath, out UpdateDynamicScheduleRequest, out ErrorResponse, CustomUpdateDynamicScheduleRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an update dynamic schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="UpdateDynamicScheduleRequest">The parsed update dynamic schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateDynamicScheduleRequestParser">A delegate to parse custom update dynamic schedule requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       NetworkingNode_Id                                           DestinationId,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out UpdateDynamicScheduleRequest?      UpdateDynamicScheduleRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateDynamicScheduleRequest>?  CustomUpdateDynamicScheduleRequestParser)
        {

            try
            {

                UpdateDynamicScheduleRequest = null;

                #region ChargingProfileId      [mandatory]

                if (!JSON.ParseMandatory("chargingProfileId",
                                         "charging profile identification",
                                         ChargingProfile_Id.TryParse,
                                         out ChargingProfile_Id ChargingProfileId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Limit                  [optional]

                if (JSON.ParseOptional("limit",
                                       "charging rate limit",
                                       out ChargingRateValue? Limit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Limit_L2               [optional]

                if (JSON.ParseOptional("limit_L2",
                                       "charging rate limit on phase L2",
                                       out ChargingRateValue? Limit_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Limit_L3               [optional]

                if (JSON.ParseOptional("limit_L3",
                                       "charging rate limit on phase L3",
                                       out ChargingRateValue? Limit_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region DischargeLimit         [optional]

                if (JSON.ParseOptional("dischargeLimit",
                                       "discharging rate limit",
                                       out ChargingRateValue? DischargeLimit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DischargeLimit_L2      [optional]

                if (JSON.ParseOptional("dischargeLimit_L2",
                                       "discharging rate limit on phase L2",
                                       out ChargingRateValue? DischargeLimit_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DischargeLimit_L3      [optional]

                if (JSON.ParseOptional("dischargeLimit_L3",
                                       "discharging rate limit on phase L3",
                                       out ChargingRateValue? DischargeLimit_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Setpoint               [optional]

                if (JSON.ParseOptional("setpoint",
                                       "charging rate setpoint",
                                       out ChargingRateValue? Setpoint,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Setpoint_L2            [optional]

                if (JSON.ParseOptional("setpoint_L2",
                                       "charging rate setpoint on phase L2",
                                       out ChargingRateValue? Setpoint_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Setpoint_L3            [optional]

                if (JSON.ParseOptional("setpoint_L3",
                                       "charging rate setpoint on phase L3",
                                       out ChargingRateValue? Setpoint_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region SetpointReactive       [optional]

                if (JSON.ParseOptional("setpointReactive",
                                       "charging rate setpoint reactive",
                                       out ChargingRateValue? SetpointReactive,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SetpointReactive_L2    [optional]

                if (JSON.ParseOptional("setpointReactive_L2",
                                       "charging rate setpoint reactive on phase L2",
                                       out ChargingRateValue? SetpointReactive_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SetpointReactive_L3    [optional]

                if (JSON.ParseOptional("setpointReactive_L3",
                                       "charging rate setpoint reactive on phase L3",
                                       out ChargingRateValue? SetpointReactive_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UpdateDynamicScheduleRequest = new UpdateDynamicScheduleRequest(

                                                   DestinationId,
                                                   ChargingProfileId,

                                                   Limit,
                                                   Limit_L2,
                                                   Limit_L3,

                                                   DischargeLimit,
                                                   DischargeLimit_L2,
                                                   DischargeLimit_L3,

                                                   Setpoint,
                                                   Setpoint_L2,
                                                   Setpoint_L3,

                                                   SetpointReactive,
                                                   SetpointReactive_L2,
                                                   SetpointReactive_L3,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData,

                                                   RequestId,
                                                   null,
                                                   null,
                                                   null,
                                                   NetworkPath

                                               );

                if (CustomUpdateDynamicScheduleRequestParser is not null)
                    UpdateDynamicScheduleRequest = CustomUpdateDynamicScheduleRequestParser(JSON,
                                                                                            UpdateDynamicScheduleRequest);

                return true;

            }
            catch (Exception e)
            {
                UpdateDynamicScheduleRequest  = null;
                ErrorResponse                 = "The given JSON representation of an update dynamic schedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateDynamicScheduleRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateDynamicScheduleRequestSerializer">A delegate to serialize custom UpdateDynamicSchedule requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateDynamicScheduleRequest>?  CustomUpdateDynamicScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingProfileId",     ChargingProfileId.ToString()),

                           Limit.              HasValue
                               ? new JProperty("limit",                 Limit.              Value.Value)
                               : null,

                           Limit_L2.           HasValue
                               ? new JProperty("limit_L2",              Limit_L2.           Value.Value)
                               : null,

                           Limit_L3.           HasValue
                               ? new JProperty("limit_L3",              Limit_L3.           Value.Value)
                               : null,


                           DischargeLimit.     HasValue
                               ? new JProperty("dischargeLimit",        DischargeLimit.     Value.Value)
                               : null,

                           DischargeLimit_L2.  HasValue
                               ? new JProperty("dischargeLimit_L2",     DischargeLimit_L2.  Value.Value)
                               : null,

                           DischargeLimit_L3.  HasValue
                               ? new JProperty("dischargeLimit_L3",     DischargeLimit_L3.  Value.Value)
                               : null,


                           Setpoint.           HasValue
                               ? new JProperty("setpoint",              Setpoint.           Value.Value)
                               : null,

                           Setpoint_L2.        HasValue
                               ? new JProperty("setpoint_L2",           Setpoint_L2.        Value.Value)
                               : null,

                           Setpoint_L3.        HasValue
                               ? new JProperty("setpoint_L3",           Setpoint_L3.        Value.Value)
                               : null,


                           SetpointReactive.   HasValue
                               ? new JProperty("setpointReactive",      SetpointReactive.   Value.Value)
                               : null,

                           SetpointReactive_L2.HasValue
                               ? new JProperty("setpointReactive_L2",   SetpointReactive_L2.Value.Value)
                               : null,

                           SetpointReactive_L3.HasValue
                               ? new JProperty("setpointReactive_L3",   SetpointReactive_L3.Value.Value)
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",            new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomUpdateDynamicScheduleRequestSerializer is not null
                       ? CustomUpdateDynamicScheduleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UpdateDynamicScheduleRequest1, UpdateDynamicScheduleRequest2)

        /// <summary>
        /// Compares two update dynamic schedule requests for equality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleRequest1">An update dynamic schedule request.</param>
        /// <param name="UpdateDynamicScheduleRequest2">Another update dynamic schedule request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest1,
                                           UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateDynamicScheduleRequest1, UpdateDynamicScheduleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateDynamicScheduleRequest1 is null || UpdateDynamicScheduleRequest2 is null)
                return false;

            return UpdateDynamicScheduleRequest1.Equals(UpdateDynamicScheduleRequest2);

        }

        #endregion

        #region Operator != (UpdateDynamicScheduleRequest1, UpdateDynamicScheduleRequest2)

        /// <summary>
        /// Compares two update dynamic schedule requests for inequality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleRequest1">An update dynamic schedule request.</param>
        /// <param name="UpdateDynamicScheduleRequest2">Another update dynamic schedule request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest1,
                                           UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest2)

            => !(UpdateDynamicScheduleRequest1 == UpdateDynamicScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateDynamicScheduleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two update dynamic schedule requests for equality.
        /// </summary>
        /// <param name="Object">An update dynamic schedule request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateDynamicScheduleRequest updateDynamicScheduleRequest &&
                   Equals(updateDynamicScheduleRequest);

        #endregion

        #region Equals(UpdateDynamicScheduleRequest)

        /// <summary>
        /// Compares two update dynamic schedule requests for equality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleRequest">An update dynamic schedule request to compare with.</param>
        public override Boolean Equals(UpdateDynamicScheduleRequest? UpdateDynamicScheduleRequest)

            => UpdateDynamicScheduleRequest is not null &&

               ChargingProfileId.Equals(UpdateDynamicScheduleRequest.ChargingProfileId) &&


             ((!Limit.              HasValue && !UpdateDynamicScheduleRequest.Limit.              HasValue) ||
                Limit.              HasValue &&  UpdateDynamicScheduleRequest.Limit.              HasValue &&
                Limit.              Value.Equals(UpdateDynamicScheduleRequest.Limit.              Value))  &&

             ((!Limit_L2.           HasValue && !UpdateDynamicScheduleRequest.Limit_L2.           HasValue) ||
                Limit_L2.           HasValue &&  UpdateDynamicScheduleRequest.Limit_L2.           HasValue &&
                Limit_L2.           Value.Equals(UpdateDynamicScheduleRequest.Limit_L2.           Value))  &&

             ((!Limit_L3.           HasValue && !UpdateDynamicScheduleRequest.Limit_L3.           HasValue) ||
                Limit_L3.           HasValue &&  UpdateDynamicScheduleRequest.Limit_L3.           HasValue &&
                Limit_L3.           Value.Equals(UpdateDynamicScheduleRequest.Limit_L3.           Value))  &&


             ((!DischargeLimit.     HasValue && !UpdateDynamicScheduleRequest.DischargeLimit.     HasValue) ||
                DischargeLimit.     HasValue &&  UpdateDynamicScheduleRequest.DischargeLimit.     HasValue &&
                DischargeLimit.     Value.Equals(UpdateDynamicScheduleRequest.DischargeLimit.     Value))  &&

             ((!DischargeLimit_L2.  HasValue && !UpdateDynamicScheduleRequest.DischargeLimit_L2.  HasValue) ||
                DischargeLimit_L2.  HasValue &&  UpdateDynamicScheduleRequest.DischargeLimit_L2.  HasValue &&
                DischargeLimit_L2.  Value.Equals(UpdateDynamicScheduleRequest.DischargeLimit_L2.  Value))  &&

             ((!DischargeLimit_L3.  HasValue && !UpdateDynamicScheduleRequest.DischargeLimit_L3.  HasValue) ||
                DischargeLimit_L3.  HasValue &&  UpdateDynamicScheduleRequest.DischargeLimit_L3.  HasValue &&
                DischargeLimit_L3.  Value.Equals(UpdateDynamicScheduleRequest.DischargeLimit_L3.  Value))  &&


             ((!Setpoint.           HasValue && !UpdateDynamicScheduleRequest.Setpoint.           HasValue) ||
                Setpoint.           HasValue &&  UpdateDynamicScheduleRequest.Setpoint.           HasValue &&
                Setpoint.           Value.Equals(UpdateDynamicScheduleRequest.Setpoint.           Value))  &&

             ((!Setpoint_L2.        HasValue && !UpdateDynamicScheduleRequest.Setpoint_L2.        HasValue) ||
                Setpoint_L2.        HasValue &&  UpdateDynamicScheduleRequest.Setpoint_L2.        HasValue &&
                Setpoint_L2.        Value.Equals(UpdateDynamicScheduleRequest.Setpoint_L2.        Value))  &&

             ((!Setpoint_L3.        HasValue && !UpdateDynamicScheduleRequest.Setpoint_L3.        HasValue) ||
                Setpoint_L3.        HasValue &&  UpdateDynamicScheduleRequest.Setpoint_L3.        HasValue &&
                Setpoint_L3.        Value.Equals(UpdateDynamicScheduleRequest.Setpoint_L3.        Value))  &&


             ((!SetpointReactive.   HasValue && !UpdateDynamicScheduleRequest.SetpointReactive.   HasValue) ||
                SetpointReactive.   HasValue &&  UpdateDynamicScheduleRequest.SetpointReactive.   HasValue &&
                SetpointReactive.   Value.Equals(UpdateDynamicScheduleRequest.SetpointReactive.   Value))  &&

             ((!SetpointReactive_L2.HasValue && !UpdateDynamicScheduleRequest.SetpointReactive_L2.HasValue) ||
                SetpointReactive_L2.HasValue &&  UpdateDynamicScheduleRequest.SetpointReactive_L2.HasValue &&
                SetpointReactive_L2.Value.Equals(UpdateDynamicScheduleRequest.SetpointReactive_L2.Value))  &&

             ((!SetpointReactive_L3.HasValue && !UpdateDynamicScheduleRequest.SetpointReactive_L3.HasValue) ||
                SetpointReactive_L3.HasValue &&  UpdateDynamicScheduleRequest.SetpointReactive_L3.HasValue &&
                SetpointReactive_L3.Value.Equals(UpdateDynamicScheduleRequest.SetpointReactive_L3.Value))  &&


               base.GenericEquals(UpdateDynamicScheduleRequest);

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

                   ChargingProfileId.ToString(),

                   Limit.HasValue
                       ? "limit: "                + Limit.              Value.ToString()
                       : null,

                   Limit_L2.HasValue
                       ? "limit L2: "             + Limit_L2.           Value.ToString()
                       : null,

                   Limit_L3.HasValue
                       ? "limit L3: "             + Limit_L3.           Value.ToString()
                       : null,


                   DischargeLimit.HasValue
                       ? "discharge limit: "      + DischargeLimit.     Value.ToString()
                       : null,

                   DischargeLimit_L2.HasValue
                       ? "discharge limit L2: "   + DischargeLimit_L2.  Value.ToString()
                       : null,

                   DischargeLimit_L3.HasValue
                       ? "discharge limit L3: "   + DischargeLimit_L3.  Value.ToString()
                       : null,


                   Setpoint.HasValue
                       ? "setpoint: "             + Setpoint.           Value.ToString()
                       : null,

                   Setpoint_L2.HasValue
                       ? "setpoint L2: "          + Setpoint_L2.        Value.ToString()
                       : null,

                   Setpoint_L3.HasValue
                       ? "setpoint L3: "          + Setpoint_L3.        Value.ToString()
                       : null,


                   SetpointReactive.HasValue
                       ? "setpoint reactive: "    + SetpointReactive.   Value.ToString()
                       : null,

                   SetpointReactive_L2.HasValue
                       ? "setpoint reactive L2: " + SetpointReactive_L2.Value.ToString()
                       : null,

                   SetpointReactive_L3.HasValue
                       ? "setpoint reactive L3: " + SetpointReactive_L3.Value.ToString()
                       : null

               ).AggregateWith(", ");

        #endregion

    }

}
