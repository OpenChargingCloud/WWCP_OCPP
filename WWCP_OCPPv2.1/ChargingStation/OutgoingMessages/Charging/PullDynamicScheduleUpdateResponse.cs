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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A pull dynamic schedule update response.
    /// </summary>
    public class PullDynamicScheduleUpdateResponse : AResponse<CS.PullDynamicScheduleUpdateRequest,
                                                                  PullDynamicScheduleUpdateResponse>,
                                                     IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/pullDynamicScheduleUpdateResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

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

        #region PullDynamicScheduleUpdateResponse(Request, Limit, ...)

        /// <summary>
        /// Create a pull dynamic schedule update response.
        /// </summary>
        /// <param name="Request">The pull dynamic schedule update request leading to this response.</param>
        /// 
        /// <param name="Limit">Optional charging rate limit in chargingRateUnit (&gt;= 0).</param>
        /// <param name="Limit_L2">Optional charging rate limit in chargingRateUnit on phase L2 (&gt;= 0).</param>
        /// <param name="Limit_L3">Optional charging rate limit in chargingRateUnit on phase L3 (&gt;= 0).</param>
        /// 
        /// <param name="DischargeLimit">Optional discharging limit in chargingRateUnit (&lt;= 0).</param>
        /// <param name="DischargeLimit_L2">Optional discharging limit in chargingRateUnit on phase L2 (&lt;= 0).</param>
        /// <param name="DischargeLimit_L3">Optional discharging limit in chargingRateUnit on phase L3 (&lt;= 0).</param>
        /// 
        /// <param name="Setpoint">Optional setpoint in chargingRateUnit.</param>
        /// <param name="Setpoint_L2">Optional setpoint in chargingRateUnit on phase L2.</param>
        /// <param name="Setpoint_L3">Optional setpoint in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="SetpointReactive">Optional setpoint for reactive power (or current) in chargingRateUnit.</param>
        /// <param name="SetpointReactive_L2">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.</param>
        /// <param name="SetpointReactive_L3">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public PullDynamicScheduleUpdateResponse(CS.PullDynamicScheduleUpdateRequest  Request,

                                                 ChargingRateValue?                   Limit                 = null,
                                                 ChargingRateValue?                   Limit_L2              = null,
                                                 ChargingRateValue?                   Limit_L3              = null,

                                                 ChargingRateValue?                   DischargeLimit        = null,
                                                 ChargingRateValue?                   DischargeLimit_L2     = null,
                                                 ChargingRateValue?                   DischargeLimit_L3     = null,

                                                 ChargingRateValue?                   Setpoint              = null,
                                                 ChargingRateValue?                   Setpoint_L2           = null,
                                                 ChargingRateValue?                   Setpoint_L3           = null,

                                                 ChargingRateValue?                   SetpointReactive      = null,
                                                 ChargingRateValue?                   SetpointReactive_L2   = null,
                                                 ChargingRateValue?                   SetpointReactive_L3   = null,

                                                 DateTime?                            ResponseTimestamp     = null,

                                                 IEnumerable<KeyPair>?                SignKeys              = null,
                                                 IEnumerable<SignInfo>?               SignInfos             = null,
                                                 IEnumerable<OCPP.Signature>?         Signatures            = null,

                                                 CustomData?                          CustomData            = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

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

                hashCode = (Limit?.              GetHashCode() ?? 0) * 43 ^
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

        #region PullDynamicScheduleUpdateResponse(Request, Result)

        /// <summary>
        /// Create a pull dynamic schedule update response.
        /// </summary>
        /// <param name="Request">The pull dynamic schedule update request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public PullDynamicScheduleUpdateResponse(CS.PullDynamicScheduleUpdateRequest  Request,
                                                 Result                               Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomPullDynamicScheduleUpdateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a pull dynamic schedule update response.
        /// </summary>
        /// <param name="Request">The pull dynamic schedule update request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPullDynamicScheduleUpdateResponseParser">A delegate to parse custom pull dynamic schedule update responses.</param>
        public static PullDynamicScheduleUpdateResponse Parse(CS.PullDynamicScheduleUpdateRequest                              Request,
                                                              JObject                                                          JSON,
                                                              CustomJObjectParserDelegate<PullDynamicScheduleUpdateResponse>?  CustomPullDynamicScheduleUpdateResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var pullDynamicScheduleUpdateResponse,
                         out var errorResponse,
                         CustomPullDynamicScheduleUpdateResponseParser) &&
                pullDynamicScheduleUpdateResponse is not null)
            {
                return pullDynamicScheduleUpdateResponse;
            }

            throw new ArgumentException("The given JSON representation of a pull dynamic schedule update response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out PullDynamicScheduleUpdateResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a pull dynamic schedule update response.
        /// </summary>
        /// <param name="Request">The pull dynamic schedule update request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PullDynamicScheduleUpdateResponse">The parsed pull dynamic schedule update response.</param>
        /// <param name="CustomPullDynamicScheduleUpdateResponseParser">A delegate to parse custom pull dynamic schedule update responses.</param>
        public static Boolean TryParse(CS.PullDynamicScheduleUpdateRequest                              Request,
                                       JObject                                                          JSON,
                                       out PullDynamicScheduleUpdateResponse?                           PullDynamicScheduleUpdateResponse,
                                       out String?                                                      ErrorResponse,
                                       CustomJObjectParserDelegate<PullDynamicScheduleUpdateResponse>?  CustomPullDynamicScheduleUpdateResponseParser   = null)
        {

            try
            {

                PullDynamicScheduleUpdateResponse = null;

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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData             [optional]

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


                PullDynamicScheduleUpdateResponse = new PullDynamicScheduleUpdateResponse(

                                                        Request,

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
                                                        null,
                                                        Signatures,

                                                        CustomData

                                                    );

                if (CustomPullDynamicScheduleUpdateResponseParser is not null)
                    PullDynamicScheduleUpdateResponse = CustomPullDynamicScheduleUpdateResponseParser(JSON,
                                                                                                      PullDynamicScheduleUpdateResponse);

                return true;

            }
            catch (Exception e)
            {
                PullDynamicScheduleUpdateResponse  = null;
                ErrorResponse                      = "The given JSON representation of a pull dynamic schedule update response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPullDynamicScheduleUpdateResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPullDynamicScheduleUpdateResponseSerializer">A delegate to serialize custom pull dynamic schedule update responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateResponse>?  CustomPullDynamicScheduleUpdateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                     CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

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
                               ? new JProperty("customData",            CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomPullDynamicScheduleUpdateResponseSerializer is not null
                       ? CustomPullDynamicScheduleUpdateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The pull dynamic schedule update request failed.
        /// </summary>
        /// <param name="Request">The pull dynamic schedule update request leading to this response.</param>
        public static PullDynamicScheduleUpdateResponse Failed(CS.PullDynamicScheduleUpdateRequest Request)

            => new (Request,
                    Result.GenericError());

        #endregion


        #region Operator overloading

        #region Operator == (PullDynamicScheduleUpdateResponse1, PullDynamicScheduleUpdateResponse2)

        /// <summary>
        /// Compares two pull dynamic schedule update responses for equality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateResponse1">A pull dynamic schedule update response.</param>
        /// <param name="PullDynamicScheduleUpdateResponse2">Another pull dynamic schedule update response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse1,
                                           PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PullDynamicScheduleUpdateResponse1, PullDynamicScheduleUpdateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (PullDynamicScheduleUpdateResponse1 is null || PullDynamicScheduleUpdateResponse2 is null)
                return false;

            return PullDynamicScheduleUpdateResponse1.Equals(PullDynamicScheduleUpdateResponse2);

        }

        #endregion

        #region Operator != (PullDynamicScheduleUpdateResponse1, PullDynamicScheduleUpdateResponse2)

        /// <summary>
        /// Compares two pull dynamic schedule update responses for inequality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateResponse1">A pull dynamic schedule update response.</param>
        /// <param name="PullDynamicScheduleUpdateResponse2">Another pull dynamic schedule update response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse1,
                                           PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse2)

            => !(PullDynamicScheduleUpdateResponse1 == PullDynamicScheduleUpdateResponse2);

        #endregion

        #endregion

        #region IEquatable<PullDynamicScheduleUpdateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two pull dynamic schedule update responses for equality.
        /// </summary>
        /// <param name="Object">A pull dynamic schedule update response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PullDynamicScheduleUpdateResponse pullDynamicScheduleUpdateResponse &&
                   Equals(pullDynamicScheduleUpdateResponse);

        #endregion

        #region Equals(PullDynamicScheduleUpdateResponse)

        /// <summary>
        /// Compares two pull dynamic schedule update responses for equality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateResponse">A pull dynamic schedule update response to compare with.</param>
        public override Boolean Equals(PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse)

            => PullDynamicScheduleUpdateResponse is not null &&

             ((!Limit.              HasValue && !PullDynamicScheduleUpdateResponse.Limit.              HasValue) ||
                Limit.              HasValue &&  PullDynamicScheduleUpdateResponse.Limit.              HasValue &&
                Limit.              Value.Equals(PullDynamicScheduleUpdateResponse.Limit.              Value))  &&

             ((!Limit_L2.           HasValue && !PullDynamicScheduleUpdateResponse.Limit_L2.           HasValue) ||
                Limit_L2.           HasValue &&  PullDynamicScheduleUpdateResponse.Limit_L2.           HasValue &&
                Limit_L2.           Value.Equals(PullDynamicScheduleUpdateResponse.Limit_L2.           Value))  &&

             ((!Limit_L3.           HasValue && !PullDynamicScheduleUpdateResponse.Limit_L3.           HasValue) ||
                Limit_L3.           HasValue &&  PullDynamicScheduleUpdateResponse.Limit_L3.           HasValue &&
                Limit_L3.           Value.Equals(PullDynamicScheduleUpdateResponse.Limit_L3.           Value))  &&


             ((!DischargeLimit.     HasValue && !PullDynamicScheduleUpdateResponse.DischargeLimit.     HasValue) ||
                DischargeLimit.     HasValue &&  PullDynamicScheduleUpdateResponse.DischargeLimit.     HasValue &&
                DischargeLimit.     Value.Equals(PullDynamicScheduleUpdateResponse.DischargeLimit.     Value))  &&

             ((!DischargeLimit_L2.  HasValue && !PullDynamicScheduleUpdateResponse.DischargeLimit_L2.  HasValue) ||
                DischargeLimit_L2.  HasValue &&  PullDynamicScheduleUpdateResponse.DischargeLimit_L2.  HasValue &&
                DischargeLimit_L2.  Value.Equals(PullDynamicScheduleUpdateResponse.DischargeLimit_L2.  Value))  &&

             ((!DischargeLimit_L3.  HasValue && !PullDynamicScheduleUpdateResponse.DischargeLimit_L3.  HasValue) ||
                DischargeLimit_L3.  HasValue &&  PullDynamicScheduleUpdateResponse.DischargeLimit_L3.  HasValue &&
                DischargeLimit_L3.  Value.Equals(PullDynamicScheduleUpdateResponse.DischargeLimit_L3.  Value))  &&


             ((!Setpoint.           HasValue && !PullDynamicScheduleUpdateResponse.Setpoint.           HasValue) ||
                Setpoint.           HasValue &&  PullDynamicScheduleUpdateResponse.Setpoint.           HasValue &&
                Setpoint.           Value.Equals(PullDynamicScheduleUpdateResponse.Setpoint.           Value))  &&

             ((!Setpoint_L2.        HasValue && !PullDynamicScheduleUpdateResponse.Setpoint_L2.        HasValue) ||
                Setpoint_L2.        HasValue &&  PullDynamicScheduleUpdateResponse.Setpoint_L2.        HasValue &&
                Setpoint_L2.        Value.Equals(PullDynamicScheduleUpdateResponse.Setpoint_L2.        Value))  &&

             ((!Setpoint_L3.        HasValue && !PullDynamicScheduleUpdateResponse.Setpoint_L3.        HasValue) ||
                Setpoint_L3.        HasValue &&  PullDynamicScheduleUpdateResponse.Setpoint_L3.        HasValue &&
                Setpoint_L3.        Value.Equals(PullDynamicScheduleUpdateResponse.Setpoint_L3.        Value))  &&


             ((!SetpointReactive.   HasValue && !PullDynamicScheduleUpdateResponse.SetpointReactive.   HasValue) ||
                SetpointReactive.   HasValue &&  PullDynamicScheduleUpdateResponse.SetpointReactive.   HasValue &&
                SetpointReactive.   Value.Equals(PullDynamicScheduleUpdateResponse.SetpointReactive.   Value))  &&

             ((!SetpointReactive_L2.HasValue && !PullDynamicScheduleUpdateResponse.SetpointReactive_L2.HasValue) ||
                SetpointReactive_L2.HasValue &&  PullDynamicScheduleUpdateResponse.SetpointReactive_L2.HasValue &&
                SetpointReactive_L2.Value.Equals(PullDynamicScheduleUpdateResponse.SetpointReactive_L2.Value))  &&

             ((!SetpointReactive_L3.HasValue && !PullDynamicScheduleUpdateResponse.SetpointReactive_L3.HasValue) ||
                SetpointReactive_L3.HasValue &&  PullDynamicScheduleUpdateResponse.SetpointReactive_L3.HasValue &&
                SetpointReactive_L3.Value.Equals(PullDynamicScheduleUpdateResponse.SetpointReactive_L3.Value))  &&

               base.GenericEquals(PullDynamicScheduleUpdateResponse);

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

            => new[] {

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

               }.Where(value => value is not null).
                 AggregateWith(", ");

        #endregion

    }

}
