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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// V2X charging parameters for ISO 15118-20 session instead of
    /// the AC/DCChargingParameters of ISO 15118-2.
    /// </summary>
    public class V2XChargingParameters : ACustomData,
                                         IEquatable<V2XChargingParameters>
    {

        #region Properties

        /// <summary>
        /// The optional minimum charge power in W, defined by max(EV, EVSE).
        /// This field represents the sum of all phases, unless values are
        /// provided for L2 and L3, in which case this field represents phase L1.
        /// ISO 15118-20: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumChargePower
        /// </summary>
        [Optional]
        public Watt?           MinChargePower           { get; }

        /// <summary>
        /// The optional minimum charge power in W on phase L2, defined by max(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargePower_L2
        /// </summary>
        [Optional]
        public Watt?           MinChargePower_L2        { get; }

        /// <summary>
        /// The optional minimum charge power in W on phase L3, defined by max(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargePower_L3
        /// </summary>
        [Optional]
        public Watt?           MinChargePower_L3        { get; }

        /// <summary>
        /// The optional maximum charge power in W, defined by min(EV, EVSE).
        /// This field represents the sum of all phases, unless values are
        /// provided for L2 and L3, in which case this field represents phase L1.
        /// ISO 15118-20: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumChargePower
        /// </summary>
        [Optional]
        public Watt?           MaxChargePower           { get; }

        /// <summary>
        /// The optional maximum charge power in W on phase L2, defined by min(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargePower_L2
        /// </summary>
        [Optional]
        public Watt?           MaxChargePower_L2        { get; }

        /// <summary>
        /// The optional maximum charge power in W on phase L3, defined by min(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargePower_L3
        /// </summary>
        [Optional]
        public Watt?           MaxChargePower_L3        { get; }


        /// <summary>
        /// The optional minimum discharge power in W, defined by max(EV, EVSE).
        /// This field represents the sum of all phases, unless values are
        /// provided for L2 and L3, in which case this field represents phase L1.
        /// ISO 15118-20: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumDischargePower
        /// </summary>
        [Optional]
        public Watt?           MinDischargePower        { get; }

        /// <summary>
        /// The optional minimum discharge power in W on phase L2, defined by max(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargePower_L2
        /// </summary>
        [Optional]
        public Watt?           MinDischargePower_L2     { get; }

        /// <summary>
        /// The optional minimum discharge power in W on phase L3, defined by max(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargePower_L3
        /// </summary>
        [Optional]
        public Watt?           MinDischargePower_L3     { get; }

        /// <summary>
        /// The optional maximum discharge power in W, defined by min(EV, EVSE).
        /// This field represents the sum of all phases, unless values are
        /// provided for L2 and L3, in which case this field represents phase L1.
        /// ISO 15118-20: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumDischargePower
        /// </summary>
        [Optional]
        public Watt?           MaxDischargePower        { get; }

        /// <summary>
        /// The optional maximum discharge power in W on phase L2, defined by min(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargePower_L2
        /// </summary>
        [Optional]
        public Watt?           MaxDischargePower_L2     { get; }

        /// <summary>
        /// The optional maximum discharge power in W on phase L3, defined by min(EV, EVSE).
        /// ISO 15118-20: BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargePower_L3
        /// </summary>
        [Optional]
        public Watt?           MaxDischargePower_L3     { get; }


        /// <summary>
        /// The optional minimum charge current in A, defined by max(EV, EVSE).
        /// ISO 15118-20: BPT_DC_CPDReqEnergyTransferModeType: EVMinimumChargeCurrent
        /// </summary>
        [Optional]
        public Ampere?         MinChargeCurrent         { get; }

        /// <summary>
        /// The optional maximum charge current in A, defined by min(EV, EVSE).
        /// ISO 15118-20: BPT_DC_CPDReqEnergyTransferModeType: EVMaximumChargeCurrent
        /// </summary>
        [Optional]
        public Ampere?         MaxChargeCurrent         { get; }

        /// <summary>
        /// The optional minimum discharge current in A, defined by max(EV, EVSE).
        /// ISO 15118-20: BPT_DC_CPDReqEnergyTransferModeType: EVMinimumDischargeCurrent
        /// </summary>
        [Optional]
        public Ampere?         MinDischargeCurrent      { get; }

        /// <summary>
        /// The optional maximum discharge current in A, defined by min(EV, EVSE).
        /// ISO 15118-20: BPT_DC_CPDReqEnergyTransferModeType: EVMaximumDischargeCurrent
        /// </summary>
        [Optional]
        public Ampere?         MaxDischargeCurrent      { get; }

        /// <summary>
        /// The optional minimum voltage in V, defined by max(EV, EVSE).
        /// ISO 15118-20: BPT_DC_CPDReqEnergyTransferModeType: EVMinimumVoltage
        /// </summary>
        [Optional]
        public Volt?           MinVoltage               { get; }

        /// <summary>
        /// The optional maximum voltage in V, defined by min(EV, EVSE).
        /// ISO 15118-20: BPT_DC_CPDReqEnergyTransferModeType: EVMaximumVoltage
        /// </summary>
        [Optional]
        public Volt?           MaxVoltage               { get; }


        /// <summary>
        /// The optional energy to requested state-of-charge in Wh.
        /// ISO 15118-20: Dynamic/Scheduled_SEReqControlModeType: EVTargetEnergyRequest
        /// </summary>
        [Optional]
        public WattHour?       EVTargetEnergyRequest    { get; }

        /// <summary>
        /// The optional energy to minimum allowed state-of-charge in Wh.
        /// ISO 15118-20: Dynamic/Scheduled_SEReqControlModeType: EVMinimumEnergyRequest
        /// </summary>
        [Optional]
        public WattHour?       EVMinEnergyRequest       { get; }

        /// <summary>
        /// The optional energy to maximum-state-of charge in Wh.
        /// ISO 15118-20: Dynamic/Scheduled_SEReqControlModeType: EVMaximumEnergyRequest
        /// </summary>
        [Optional]
        public WattHour?       EVMaxEnergyRequest       { get; }

        /// <summary>
        /// The optional energy to minimum state-of-charge for cycling (V2X) activity.
        /// Positive value means that current state of charge is below V2X range.
        /// ISO 15118-20: Dynamic_SEReqControlModeType: EVMinimumV2XEnergyRequest
        /// </summary>
        [Optional]
        public WattHour?       EVMinV2XEnergyRequest    { get; }

        /// <summary>
        /// The optional energy to maximum state-of-charge for cycling (V2X) activity.
        /// Negative value indicates that current state of charge is above V2X range.
        /// ISO 15118-20: Dynamic_SEReqControlModeType: EVMaximumV2XEnergyRequest
        /// </summary>
        [Optional]
        public WattHour?       EVMaxV2XEnergyRequest    { get; }

        /// <summary>
        /// The optional target state-of-charge at departure as a percentage.
        /// ISO 15118-20: BPT_DC_CPDReqEnergyTransferModeType: TargetSOC
        /// </summary>
        [Optional]
        public PercentageByte?  TargetSoC                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new V2X charging parameters.
        /// </summary>
        /// <param name="MinChargePower">The optional minimum charge power in W, defined by max(EV, EVSE).</param>
        /// <param name="MinChargePower_L2">The optional minimum charge power in W on phase L2, defined by max(EV, EVSE).</param>
        /// <param name="MinChargePower_L3">The optional minimum charge power in W on phase L3, defined by max(EV, EVSE).</param>
        /// <param name="MaxChargePower">The optional maximum charge power in W, defined by min(EV, EVSE).</param>
        /// <param name="MaxChargePower_L2">The optional maximum charge power in W on phase L2, defined by min(EV, EVSE).</param>
        /// <param name="MaxChargePower_L3">The optional maximum charge power in W on phase L3, defined by min(EV, EVSE).</param>
        /// 
        /// <param name="MinDischargePower">The optional minimum discharge power in W, defined by max(EV, EVSE).</param>
        /// <param name="MinDischargePower_L2">The optional minimum discharge power in W on phase L2, defined by max(EV, EVSE).</param>
        /// <param name="MinDischargePower_L3">The optional minimum discharge power in W on phase L3, defined by max(EV, EVSE).</param>
        /// <param name="MaxDischargePower">The optional maximum discharge power in W, defined by min(EV, EVSE).</param>
        /// <param name="MaxDischargePower_L2">The optional maximum discharge power in W on phase L2, defined by min(EV, EVSE).</param>
        /// <param name="MaxDischargePower_L3">The optional maximum discharge power in W on phase L3, defined by min(EV, EVSE).</param>
        /// 
        /// <param name="MinChargeCurrent">The optional minimum charge current in A, defined by max(EV, EVSE).</param>
        /// <param name="MaxChargeCurrent">The optional maximum charge current in A, defined by max(EV, EVSE).</param>
        /// <param name="MinDischargeCurrent">The optional minimum discharge current in A, defined by max(EV, EVSE).</param>
        /// <param name="MaxDischargeCurrent">The optional maximum discharge current in A, defined by max(EV, EVSE).</param>
        /// <param name="MinVoltage">The optional minimum voltage in V, defined by max(EV, EVSE).</param>
        /// <param name="MaxVoltage">The optional maximum voltage in V, defined by max(EV, EVSE).</param>
        /// 
        /// <param name="EVTargetEnergyRequest">The optional energy to requested state-of-charge in Wh.</param>
        /// <param name="EVMinEnergyRequest">The optional energy to minimum allowed state-of-charge in Wh.</param>
        /// <param name="EVMaxEnergyRequest">The optional energy to maximum-state-of charge in Wh.</param>
        /// <param name="EVMinV2XEnergyRequest">The optional energy to minimum state-of-charge for cycling (V2X) activity.</param>
        /// <param name="EVMaxV2XEnergyRequest">The optional energy to maximum state-of-charge for cycling (V2X) activity.</param>
        /// <param name="TargetSoC">The optional target state-of-charge at departure as a percentage.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public V2XChargingParameters(Watt?           MinChargePower          = null,
                                     Watt?           MinChargePower_L2       = null,
                                     Watt?           MinChargePower_L3       = null,
                                     Watt?           MaxChargePower          = null,
                                     Watt?           MaxChargePower_L2       = null,
                                     Watt?           MaxChargePower_L3       = null,

                                     Watt?           MinDischargePower       = null,
                                     Watt?           MinDischargePower_L2    = null,
                                     Watt?           MinDischargePower_L3    = null,
                                     Watt?           MaxDischargePower       = null,
                                     Watt?           MaxDischargePower_L2    = null,
                                     Watt?           MaxDischargePower_L3    = null,

                                     Ampere?         MinChargeCurrent        = null,
                                     Ampere?         MaxChargeCurrent        = null,
                                     Ampere?         MinDischargeCurrent     = null,
                                     Ampere?         MaxDischargeCurrent     = null,
                                     Volt?           MinVoltage              = null,
                                     Volt?           MaxVoltage              = null,

                                     WattHour?       EVTargetEnergyRequest   = null,
                                     WattHour?       EVMinEnergyRequest      = null,
                                     WattHour?       EVMaxEnergyRequest      = null,
                                     WattHour?       EVMinV2XEnergyRequest   = null,
                                     WattHour?       EVMaxV2XEnergyRequest   = null,
                                     PercentageByte?  TargetSoC               = null,

                                     CustomData?     CustomData              = null)

            : base(CustomData)

        {

            this.MinChargePower         = MinChargePower;
            this.MinChargePower_L2      = MinChargePower_L2;
            this.MinChargePower_L3      = MinChargePower_L3;
            this.MaxChargePower         = MaxChargePower;
            this.MaxChargePower_L2      = MaxChargePower_L2;
            this.MaxChargePower_L3      = MaxChargePower_L3;

            this.MinDischargePower      = MinDischargePower;
            this.MinDischargePower_L2   = MinDischargePower_L2;
            this.MinDischargePower_L3   = MinDischargePower_L3;
            this.MaxDischargePower      = MaxDischargePower;
            this.MaxDischargePower_L2   = MaxDischargePower_L2;
            this.MaxDischargePower_L3   = MaxDischargePower_L3;

            this.MinChargeCurrent       = MinChargeCurrent;
            this.MaxChargeCurrent       = MaxChargeCurrent;
            this.MinDischargeCurrent    = MinDischargeCurrent;
            this.MaxDischargeCurrent    = MaxDischargeCurrent;
            this.MinVoltage             = MinVoltage;
            this.MaxVoltage             = MaxVoltage;

            this.EVTargetEnergyRequest  = EVTargetEnergyRequest;
            this.EVMinEnergyRequest     = EVMinEnergyRequest;
            this.EVMaxEnergyRequest     = EVMaxEnergyRequest;
            this.EVMinV2XEnergyRequest  = EVMinV2XEnergyRequest;
            this.EVMaxV2XEnergyRequest  = EVMaxV2XEnergyRequest;
            this.TargetSoC              = TargetSoC;

            unchecked
            {

                hashCode = (this.MinChargePower?.       GetHashCode() ?? 0) * 97 ^
                           (this.MinChargePower_L2?.    GetHashCode() ?? 0) * 89 ^
                           (this.MinChargePower_L3?.    GetHashCode() ?? 0) * 83 ^
                           (this.MaxChargePower?.       GetHashCode() ?? 0) * 79 ^
                           (this.MaxChargePower_L2?.    GetHashCode() ?? 0) * 73 ^
                           (this.MaxChargePower_L3?.    GetHashCode() ?? 0) * 71 ^

                           (this.MinDischargePower?.    GetHashCode() ?? 0) * 67 ^
                           (this.MinDischargePower_L2?. GetHashCode() ?? 0) * 61 ^
                           (this.MinDischargePower_L3?. GetHashCode() ?? 0) * 59 ^
                           (this.MaxDischargePower?.    GetHashCode() ?? 0) * 53 ^
                           (this.MaxDischargePower_L2?. GetHashCode() ?? 0) * 47 ^
                           (this.MaxDischargePower_L3?. GetHashCode() ?? 0) * 43 ^

                           (this.MinChargeCurrent?.     GetHashCode() ?? 0) * 41 ^
                           (this.MaxChargeCurrent?.     GetHashCode() ?? 0) * 37 ^
                           (this.MinDischargeCurrent?.  GetHashCode() ?? 0) * 31 ^
                           (this.MaxDischargeCurrent?.  GetHashCode() ?? 0) * 29 ^
                           (this.MinVoltage?.           GetHashCode() ?? 0) * 23 ^
                           (this.MaxVoltage?.           GetHashCode() ?? 0) * 19 ^

                           (this.EVTargetEnergyRequest?.GetHashCode() ?? 0) * 17 ^
                           (this.EVMinEnergyRequest?.   GetHashCode() ?? 0) * 13 ^
                           (this.EVMaxEnergyRequest?.   GetHashCode() ?? 0) * 11 ^
                           (this.EVMinV2XEnergyRequest?.GetHashCode() ?? 0) *  7 ^
                           (this.EVMaxV2XEnergyRequest?.GetHashCode() ?? 0) *  5 ^
                           (this.TargetSoC?.            GetHashCode() ?? 0) *  3 ^

                            base.                       GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomV2XChargingParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of V2X charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomV2XChargingParametersParser">A delegate to parse custom V2X charging parameters JSON objects.</param>
        public static V2XChargingParameters Parse(JObject                                              JSON,
                                                  CustomJObjectParserDelegate<V2XChargingParameters>?  CustomV2XChargingParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var v2xChargingParameters,
                         out var errorResponse,
                         CustomV2XChargingParametersParser) &&
                v2xChargingParameters is not null)
            {
                return v2xChargingParameters;
            }

            throw new ArgumentException("The given JSON representation of V2X charging parameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out V2XChargingParameters, out ErrorResponse, CustomV2XChargingParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of V2X charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="V2XChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                     JSON,
                                       out V2XChargingParameters?  V2XChargingParameters,
                                       out String?                 ErrorResponse)

            => TryParse(JSON,
                        out V2XChargingParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of V2X charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="V2XChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomV2XChargingParametersParser">A delegate to parse custom V2X charging parameters JSON objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       out V2XChargingParameters?                           V2XChargingParameters,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<V2XChargingParameters>?  CustomV2XChargingParametersParser)
        {

            try
            {

                V2XChargingParameters = default;

                #region MinChargePower           [optional]

                if (JSON.ParseOptional("minChargePower",
                                       "minimum charge power",
                                       Watt.TryParse,
                                       out Watt? MinChargePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinChargePower_L2        [optional]

                if (JSON.ParseOptional("minChargePower_L2",
                                       "minimum charge power on phase L2",
                                       Watt.TryParse,
                                       out Watt? MinChargePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinChargePower_L3        [optional]

                if (JSON.ParseOptional("minChargePower_L3",
                                       "minimum charge power on phase L3",
                                       Watt.TryParse,
                                       out Watt? MinChargePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargePower           [optional]

                if (JSON.ParseOptional("maxChargePower",
                                       "maximum charge power",
                                       Watt.TryParse,
                                       out Watt? MaxChargePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargePower_L2        [optional]

                if (JSON.ParseOptional("maxChargePower_L2",
                                       "maximum charge power on phase L2",
                                       Watt.TryParse,
                                       out Watt? MaxChargePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargePower_L3        [optional]

                if (JSON.ParseOptional("maxChargePower_L3",
                                       "maximum charge power on phase L3",
                                       Watt.TryParse,
                                       out Watt? MaxChargePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region MinDischargePower        [optional]

                if (JSON.ParseOptional("minDischargePower",
                                       "minimum discharge power",
                                       Watt.TryParse,
                                       out Watt? MinDischargePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinDischargePower_L2     [optional]

                if (JSON.ParseOptional("minDischargePower_L2",
                                       "minimum discharge power on phase L2",
                                       Watt.TryParse,
                                       out Watt? MinDischargePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinDischargePower_L3     [optional]

                if (JSON.ParseOptional("minDischargePower_L3",
                                       "minimum discharge power on phase L3",
                                       Watt.TryParse,
                                       out Watt? MinDischargePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargePower        [optional]

                if (JSON.ParseOptional("maxDischargePower",
                                       "maximum discharge power",
                                       Watt.TryParse,
                                       out Watt? MaxDischargePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargePower_L2     [optional]

                if (JSON.ParseOptional("maxDischargePower_L2",
                                       "maximum discharge power on phase L2",
                                       Watt.TryParse,
                                       out Watt? MaxDischargePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargePower_L3     [optional]

                if (JSON.ParseOptional("maxDischargePower_L3",
                                       "maximum discharge power on phase L3",
                                       Watt.TryParse,
                                       out Watt? MaxDischargePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region MinChargeCurrent         [optional]

                if (JSON.ParseOptional("minChargeCurrent",
                                       "minimum charge current",
                                       Ampere.TryParse,
                                       out Ampere? MinChargeCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargeCurrent         [optional]

                if (JSON.ParseOptional("maxChargeCurrent",
                                       "maximum charge current",
                                       Ampere.TryParse,
                                       out Ampere? MaxChargeCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinDischargeCurrent      [optional]

                if (JSON.ParseOptional("minDischargeCurrent",
                                       "minimum discharge current",
                                       Ampere.TryParse,
                                       out Ampere? MinDischargeCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargeCurrent      [optional]

                if (JSON.ParseOptional("maxDischargeCurrent",
                                       "maximum discharge current",
                                       Ampere.TryParse,
                                       out Ampere? MaxDischargeCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinVoltage               [optional]

                if (JSON.ParseOptional("minVoltage",
                                       "minimum voltage",
                                       Volt.TryParse,
                                       out Volt? MinVoltage,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxVoltage               [optional]

                if (JSON.ParseOptional("maxVoltage",
                                       "maximum voltage",
                                       Volt.TryParse,
                                       out Volt? MaxVoltage,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region EVTargetEnergyRequest    [optional]

                if (JSON.ParseOptional("evTargetEnergyRequest",
                                       "ev target energy request",
                                       WattHour.TryParse,
                                       out WattHour? EVTargetEnergyRequest,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVMinEnergyRequest       [optional]

                if (JSON.ParseOptional("evMinEnergyRequest",
                                       "ev minimum energy request",
                                       WattHour.TryParse,
                                       out WattHour? EVMinEnergyRequest,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVMaxEnergyRequest       [optional]

                if (JSON.ParseOptional("evMaxEnergyRequest",
                                       "ev maximum energy request",
                                       WattHour.TryParse,
                                       out WattHour? EVMaxEnergyRequest,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVMinV2XEnergyRequest    [optional]

                if (JSON.ParseOptional("evMinV2XEnergyRequest",
                                       "ev min V2X energy request",
                                       out WattHour? EVMinV2XEnergyRequest,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVMaxV2XEnergyRequest    [optional]

                if (JSON.ParseOptional("evMaxV2XEnergyRequest",
                                       "ev max V2X energy request",
                                       out WattHour? EVMaxV2XEnergyRequest,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TargetSoC                [optional]

                if (JSON.ParseOptional("targetSoC",
                                       "target state-of-charge",
                                       PercentageByte.TryParse,
                                       out PercentageByte? TargetSoC,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region CustomData               [optional]

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


                V2XChargingParameters = new V2XChargingParameters(

                                            MinChargePower,
                                            MinChargePower_L2,
                                            MinChargePower_L3,
                                            MaxChargePower,
                                            MaxChargePower_L2,
                                            MaxChargePower_L3,

                                            MinDischargePower,
                                            MinDischargePower_L2,
                                            MinDischargePower_L3,
                                            MaxDischargePower,
                                            MaxDischargePower_L2,
                                            MaxDischargePower_L3,

                                            MinChargeCurrent,
                                            MaxChargeCurrent,
                                            MinDischargeCurrent,
                                            MaxDischargeCurrent,
                                            MinVoltage,
                                            MaxVoltage,

                                            EVTargetEnergyRequest,
                                            EVMinEnergyRequest,
                                            EVMaxEnergyRequest,
                                            EVMinV2XEnergyRequest,
                                            EVMaxV2XEnergyRequest,
                                            TargetSoC,

                                            CustomData

                                        );

                if (CustomV2XChargingParametersParser is not null)
                    V2XChargingParameters = CustomV2XChargingParametersParser(JSON,
                                                                              V2XChargingParameters);

                return true;

            }
            catch (Exception e)
            {
                V2XChargingParameters  = default;
                ErrorResponse          = "The given JSON representation of V2X charging parameters is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomV2XChargingParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomV2XChargingParametersSerializer">A delegate to serialize custom V2X charging parameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<V2XChargingParameters>?  CustomV2XChargingParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                           MinChargePower.       HasValue
                               ? new JProperty("minChargePower",          MinChargePower.       Value.Value)
                               : null,

                           MinChargePower_L2.    HasValue
                               ? new JProperty("minChargePower_L2",       MinChargePower_L2.    Value.Value)
                               : null,

                           MinChargePower_L3.    HasValue
                               ? new JProperty("minChargePower_L3",       MinChargePower_L3.    Value.Value)
                               : null,

                           MaxChargePower.       HasValue
                               ? new JProperty("maxChargePower",          MaxChargePower.       Value.Value)
                               : null,

                           MaxChargePower_L2.    HasValue
                               ? new JProperty("maxChargePower_L2",       MaxChargePower_L2.    Value.Value)
                               : null,

                           MaxChargePower_L3.    HasValue
                               ? new JProperty("maxChargePower_L3",       MaxChargePower_L3.    Value.Value)
                               : null,


                           MinDischargePower.    HasValue
                               ? new JProperty("minDischargePower",       MinDischargePower.    Value.Value)
                               : null,

                           MinDischargePower_L2. HasValue
                               ? new JProperty("minDischargePower_L2",    MinDischargePower_L2. Value.Value)
                               : null,

                           MinDischargePower_L3. HasValue
                               ? new JProperty("minDischargePower_L3",    MinDischargePower_L3. Value.Value)
                               : null,

                           MaxDischargePower.    HasValue
                               ? new JProperty("maxDischargePower",       MaxDischargePower.    Value.Value)
                               : null,

                           MaxDischargePower_L2. HasValue
                               ? new JProperty("maxDischargePower_L2",    MaxDischargePower_L2. Value.Value)
                               : null,

                           MaxDischargePower_L3. HasValue
                               ? new JProperty("maxDischargePower_L3",    MaxDischargePower_L3. Value.Value)
                               : null,


                           MinChargeCurrent.     HasValue
                               ? new JProperty("minChargeCurrent",        MinChargeCurrent.     Value.Value)
                               : null,

                           MaxChargeCurrent.     HasValue
                               ? new JProperty("maxChargeCurrent",        MaxChargeCurrent.     Value.Value)
                               : null,

                           MinDischargeCurrent.  HasValue
                               ? new JProperty("minDischargeCurrent",     MinDischargeCurrent.  Value.Value)
                               : null,

                           MaxDischargeCurrent.  HasValue
                               ? new JProperty("maxDischargeCurrent",     MaxDischargeCurrent.  Value.Value)
                               : null,

                           MinVoltage.           HasValue
                               ? new JProperty("minVoltage",              MinVoltage.           Value.Value)
                               : null,

                           MaxVoltage.           HasValue
                               ? new JProperty("maxVoltage",              MaxVoltage.           Value.Value)
                               : null,


                           EVTargetEnergyRequest.HasValue
                               ? new JProperty("evTargetEnergyRequest",   EVTargetEnergyRequest.Value.Value)
                               : null,

                           EVMinEnergyRequest.   HasValue
                               ? new JProperty("evMinEnergyRequest",      EVMinEnergyRequest.   Value.Value)
                               : null,

                           EVMaxEnergyRequest.   HasValue
                               ? new JProperty("evMaxEnergyRequest",      EVMaxEnergyRequest.   Value.Value)
                               : null,

                           EVMinV2XEnergyRequest.HasValue
                               ? new JProperty("evMinV2XEnergyRequest",   EVMinV2XEnergyRequest.Value.Value)
                               : null,

                           EVMaxV2XEnergyRequest.HasValue
                               ? new JProperty("evMaxV2XEnergyRequest",   EVMaxV2XEnergyRequest.Value.Value)
                               : null,

                           TargetSoC.            HasValue
                               ? new JProperty("targetSoC",               TargetSoC.            Value.Value)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",              CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomV2XChargingParametersSerializer is not null
                       ? CustomV2XChargingParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (V2XChargingParameters1, V2XChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="V2XChargingParameters1">V2X charging parameters.</param>
        /// <param name="V2XChargingParameters2">Another V2X charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (V2XChargingParameters? V2XChargingParameters1,
                                           V2XChargingParameters? V2XChargingParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(V2XChargingParameters1, V2XChargingParameters2))
                return true;

            // If one is null, but not both, return false.
            if (V2XChargingParameters1 is null || V2XChargingParameters2 is null)
                return false;

            return V2XChargingParameters1.Equals(V2XChargingParameters2);

        }

        #endregion

        #region Operator != (V2XChargingParameters1, V2XChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="V2XChargingParameters1">V2X charging parameters.</param>
        /// <param name="V2XChargingParameters2">Another V2X charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (V2XChargingParameters? V2XChargingParameters1,
                                           V2XChargingParameters? V2XChargingParameters2)

            => !(V2XChargingParameters1 == V2XChargingParameters2);

        #endregion

        #endregion

        #region IEquatable<V2XChargingParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two V2X charging parameters for equality.
        /// </summary>
        /// <param name="Object">V2X charging parameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is V2XChargingParameters v2xChargingParameters &&
                   Equals(v2xChargingParameters);

        #endregion

        #region Equals(V2XChargingParameters)

        /// <summary>
        /// Compares two V2X charging parameters for equality.
        /// </summary>
        /// <param name="V2XChargingParameters">V2X charging parameters to compare with.</param>
        public Boolean Equals(V2XChargingParameters? V2XChargingParameters)

            => V2XChargingParameters is not null &&

            ((!MinChargePower.       HasValue && !V2XChargingParameters.MinChargePower.       HasValue) ||
               MinChargePower.       HasValue &&  V2XChargingParameters.MinChargePower.       HasValue && MinChargePower.       Value.Equals(V2XChargingParameters.MinChargePower.       Value)) &&

            ((!MinChargePower_L2.    HasValue && !V2XChargingParameters.MinChargePower_L2.    HasValue) ||
               MinChargePower_L2.    HasValue &&  V2XChargingParameters.MinChargePower_L2.    HasValue && MinChargePower_L2.    Value.Equals(V2XChargingParameters.MinChargePower_L2.    Value)) &&

            ((!MinChargePower_L3.    HasValue && !V2XChargingParameters.MinChargePower_L3.    HasValue) ||
               MinChargePower_L3.    HasValue &&  V2XChargingParameters.MinChargePower_L3.    HasValue && MinChargePower_L3.    Value.Equals(V2XChargingParameters.MinChargePower_L3.    Value)) &&

            ((!MaxChargePower.       HasValue && !V2XChargingParameters.MaxChargePower.       HasValue) ||
               MaxChargePower.       HasValue &&  V2XChargingParameters.MaxChargePower.       HasValue && MaxChargePower.       Value.Equals(V2XChargingParameters.MaxChargePower.       Value)) &&

            ((!MaxChargePower_L2.    HasValue && !V2XChargingParameters.MaxChargePower_L2.    HasValue) ||
               MaxChargePower_L2.    HasValue &&  V2XChargingParameters.MaxChargePower_L2.    HasValue && MaxChargePower_L2.    Value.Equals(V2XChargingParameters.MaxChargePower_L2.    Value)) &&

            ((!MaxChargePower_L3.    HasValue && !V2XChargingParameters.MaxChargePower_L3.    HasValue) ||
               MaxChargePower_L3.    HasValue &&  V2XChargingParameters.MaxChargePower_L3.    HasValue && MaxChargePower_L3.    Value.Equals(V2XChargingParameters.MaxChargePower_L3.    Value)) &&


            ((!MinDischargePower.    HasValue && !V2XChargingParameters.MinDischargePower.    HasValue) ||
               MinDischargePower.    HasValue &&  V2XChargingParameters.MinDischargePower.    HasValue && MinDischargePower.    Value.Equals(V2XChargingParameters.MinDischargePower.    Value)) &&

            ((!MinDischargePower_L2. HasValue && !V2XChargingParameters.MinDischargePower_L2. HasValue) ||
               MinDischargePower_L2. HasValue &&  V2XChargingParameters.MinDischargePower_L2. HasValue && MinDischargePower_L2. Value.Equals(V2XChargingParameters.MinDischargePower_L2. Value)) &&

            ((!MinDischargePower_L3. HasValue && !V2XChargingParameters.MinDischargePower_L3. HasValue) ||
               MinDischargePower_L3. HasValue &&  V2XChargingParameters.MinDischargePower_L3. HasValue && MinDischargePower_L3. Value.Equals(V2XChargingParameters.MinDischargePower_L3. Value)) &&

            ((!MaxDischargePower.    HasValue && !V2XChargingParameters.MaxDischargePower.    HasValue) ||
               MaxDischargePower.    HasValue &&  V2XChargingParameters.MaxDischargePower.    HasValue && MaxDischargePower.    Value.Equals(V2XChargingParameters.MaxDischargePower.    Value)) &&

            ((!MaxDischargePower_L2. HasValue && !V2XChargingParameters.MaxDischargePower_L2. HasValue) ||
               MaxDischargePower_L2. HasValue &&  V2XChargingParameters.MaxDischargePower_L2. HasValue && MaxDischargePower_L2. Value.Equals(V2XChargingParameters.MaxDischargePower_L2. Value)) &&

            ((!MaxDischargePower_L3. HasValue && !V2XChargingParameters.MaxDischargePower_L3. HasValue) ||
               MaxDischargePower_L3. HasValue &&  V2XChargingParameters.MaxDischargePower_L3. HasValue && MaxDischargePower_L3. Value.Equals(V2XChargingParameters.MaxDischargePower_L3. Value)) &&


            ((!MinChargeCurrent.     HasValue && !V2XChargingParameters.MinChargeCurrent.     HasValue) ||
               MinChargeCurrent.     HasValue &&  V2XChargingParameters.MinChargeCurrent.     HasValue && MinChargeCurrent.     Value.Equals(V2XChargingParameters.MinChargeCurrent.     Value)) &&

            ((!MaxChargeCurrent.     HasValue && !V2XChargingParameters.MaxChargeCurrent.     HasValue) ||
               MaxChargeCurrent.     HasValue &&  V2XChargingParameters.MaxChargeCurrent.     HasValue && MaxChargeCurrent.     Value.Equals(V2XChargingParameters.MaxChargeCurrent.     Value)) &&

            ((!MinDischargeCurrent.  HasValue && !V2XChargingParameters.MinDischargeCurrent.  HasValue) ||
               MinDischargeCurrent.  HasValue &&  V2XChargingParameters.MinDischargeCurrent.  HasValue && MinDischargeCurrent.  Value.Equals(V2XChargingParameters.MinDischargeCurrent.  Value)) &&

            ((!MaxDischargeCurrent.  HasValue && !V2XChargingParameters.MaxDischargeCurrent.  HasValue) ||
               MaxDischargeCurrent.  HasValue &&  V2XChargingParameters.MaxDischargeCurrent.  HasValue && MaxDischargeCurrent.  Value.Equals(V2XChargingParameters.MaxDischargeCurrent.  Value)) &&

            ((!MinVoltage.           HasValue && !V2XChargingParameters.MinVoltage.           HasValue) ||
               MinVoltage.           HasValue &&  V2XChargingParameters.MinVoltage.           HasValue && MinVoltage.           Value.Equals(V2XChargingParameters.MinVoltage.           Value)) &&

            ((!MaxVoltage.           HasValue && !V2XChargingParameters.MaxVoltage.           HasValue) ||
               MaxVoltage.           HasValue &&  V2XChargingParameters.MaxVoltage.           HasValue && MaxVoltage.           Value.Equals(V2XChargingParameters.MaxVoltage.           Value)) &&


            ((!EVTargetEnergyRequest.HasValue && !V2XChargingParameters.EVTargetEnergyRequest.HasValue) ||
               EVTargetEnergyRequest.HasValue &&  V2XChargingParameters.EVTargetEnergyRequest.HasValue && EVTargetEnergyRequest.Value.Equals(V2XChargingParameters.EVTargetEnergyRequest.Value)) &&

            ((!EVMinEnergyRequest.   HasValue && !V2XChargingParameters.EVMinEnergyRequest.   HasValue) ||
               EVMinEnergyRequest.   HasValue &&  V2XChargingParameters.EVMinEnergyRequest.   HasValue && EVMinEnergyRequest.   Value.Equals(V2XChargingParameters.EVMinEnergyRequest.   Value)) &&

            ((!EVMaxEnergyRequest.   HasValue && !V2XChargingParameters.EVMaxEnergyRequest.   HasValue) ||
               EVMaxEnergyRequest.   HasValue &&  V2XChargingParameters.EVMaxEnergyRequest.   HasValue && EVMaxEnergyRequest.   Value.Equals(V2XChargingParameters.EVMaxEnergyRequest.   Value)) &&

            ((!EVMinV2XEnergyRequest.HasValue && !V2XChargingParameters.EVMinV2XEnergyRequest.HasValue) ||
               EVMinV2XEnergyRequest.HasValue &&  V2XChargingParameters.EVMinV2XEnergyRequest.HasValue && EVMinV2XEnergyRequest.Value.Equals(V2XChargingParameters.EVMinV2XEnergyRequest.Value)) &&

            ((!EVMaxV2XEnergyRequest.HasValue && !V2XChargingParameters.EVMaxV2XEnergyRequest.HasValue) ||
               EVMaxV2XEnergyRequest.HasValue &&  V2XChargingParameters.EVMaxV2XEnergyRequest.HasValue && EVMaxV2XEnergyRequest.Value.Equals(V2XChargingParameters.EVMaxV2XEnergyRequest.Value)) &&

            ((!TargetSoC.            HasValue && !V2XChargingParameters.TargetSoC.            HasValue) ||
               TargetSoC.            HasValue &&  V2XChargingParameters.TargetSoC.            HasValue && TargetSoC.            Value.Equals(V2XChargingParameters.TargetSoC.            Value)) &&

               base.Equals(V2XChargingParameters);

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

                   MinChargePower.       HasValue ? $", {MinChargePower.       Value} W"  : "",
                   MinChargePower_L2.    HasValue ? $", {MinChargePower_L2.    Value} W"  : "",
                   MinChargePower_L3.    HasValue ? $", {MinChargePower_L3.    Value} W"  : "",
                   MaxChargePower.       HasValue ? $", {MaxChargePower.       Value} W"  : "",
                   MaxChargePower_L2.    HasValue ? $", {MaxChargePower_L2.    Value} W"  : "",
                   MaxChargePower_L3.    HasValue ? $", {MaxChargePower_L3.    Value} W"  : "",

                   MinDischargePower.    HasValue ? $", {MinDischargePower.    Value} W"  : "",
                   MinDischargePower_L2. HasValue ? $", {MinDischargePower_L2. Value} W"  : "",
                   MinDischargePower_L3. HasValue ? $", {MinDischargePower_L3. Value} W"  : "",
                   MaxDischargePower.    HasValue ? $", {MaxDischargePower.    Value} W"  : "",
                   MaxDischargePower_L2. HasValue ? $", {MaxDischargePower_L2. Value} W"  : "",
                   MaxDischargePower_L3. HasValue ? $", {MaxDischargePower_L3. Value} W"  : "",

                   MinChargeCurrent.     HasValue ? $", {MinChargeCurrent.     Value} A"  : "",
                   MaxChargeCurrent.     HasValue ? $", {MaxChargeCurrent.     Value} A"  : "",
                   MinDischargeCurrent.  HasValue ? $", {MinDischargeCurrent.  Value} A"  : "",
                   MaxDischargeCurrent.  HasValue ? $", {MaxDischargeCurrent.  Value} A"  : "",
                   MinVoltage.           HasValue ? $", {MinVoltage.           Value} V"  : "",
                   MaxVoltage.           HasValue ? $", {MaxVoltage.           Value} V"  : "",

                   EVTargetEnergyRequest.HasValue ? $", {EVTargetEnergyRequest.Value} Wh" : "",
                   EVMinEnergyRequest.   HasValue ? $", {EVMinEnergyRequest.   Value} Wh" : "",
                   EVMaxEnergyRequest.   HasValue ? $", {EVMaxEnergyRequest.   Value} Wh" : "",
                   EVMinV2XEnergyRequest.HasValue ? $", {EVMinV2XEnergyRequest.Value} Wh" : "",
                   EVMaxV2XEnergyRequest.HasValue ? $", {EVMaxV2XEnergyRequest.Value} Wh" : "",
                   TargetSoC.            HasValue ? $", {TargetSoC.            Value} %"  : ""

               );

        #endregion

    }

}
