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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Charging needs.
    /// This type has been extended with controlMode, mobilityNeedsMode and pricing parameters from the ISO 15118-20 service selection.
    /// </summary>
    public class ChargingNeeds : ACustomData,
                                 IEquatable<ChargingNeeds>
    {

        #region Properties

        /// <summary>
        /// The energy transfer mode requested by the EV.
        /// </summary>
        [Mandatory]
        public EnergyTransferMode               RequestedEnergyTransferMode    { get; }

        /// <summary>
        /// The optional enumeration of energy transfer modes marked as available by the EV.
        /// </summary>
        [Optional]
        public IEnumerable<EnergyTransferMode>  AvailableEnergyTransferModes    { get; }

        /// <summary>
        /// Optional indication whether the EV wants to operate in dynamic or scheduled mode (default).
        /// </summary>
        [Optional]
        public ControlModes?                    ControlMode                     { get; }

        /// <summary>
        /// Optional indication whether only the EV or also the EVSE or CSMS
        /// determines min/target state-of-charge and departure time.
        /// </summary>
        [Optional]
        public MobilityNeedsMode?               MobilityNeedsMode               { get; }

        /// <summary>
        /// The optional pricing structure type that will be offered.
        /// </summary>
        [Optional]
        public PricingTypes?                    Pricing                         { get; }

        /// <summary>
        /// The optional estimated departure time of the EV.
        /// </summary>
        [Optional]
        public DateTime?                        DepartureTime                   { get; }

        /// <summary>
        /// Optional ISO 15118-2 EV AC charging parameters.
        /// </summary>
        [Optional]
        public ACChargingParameters?            ACChargingParameters            { get; }

        /// <summary>
        /// Optional ISO 15118-2 EV DC charging parameters.
        /// </summary>
        [Optional]
        public DCChargingParameters?            DCChargingParameters            { get; }

        /// <summary>
        /// Optional ISO 15118-20 EV charging parameters.
        /// </summary>
        [Optional]
        public V2XChargingParameters?           V2XChargingParameters           { get; }

        /// <summary>
        /// Optional discharging and associated price offered by EV.
        /// Schedule periods during which EV is willing to discharge have a dischargingLimit
        /// set to the maximum amount it can discharge.
        /// ISO 15118-20: Scheduled_SEReqControlModeType: EVEnergyOffer
        /// </summary>
        [Optional]
        public EVEnergyOffer?                   EVEnergyOffer                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging needs.
        /// </summary>
        /// <param name="RequestedEnergyTransferMode">The energy transfer mode requested by the EV.</param>
        /// <param name="AvailableEnergyTransferModes">The optional enumeration of energy transfer modes marked as available by the EV.</param>
        /// <param name="ControlMode">Optional indication whether the EV wants to operate in dynamic or scheduled mode (default).</param>
        /// <param name="MobilityNeedsMode">Optional indication whether only the EV or also the EVSE or CSMS determines min/target state-of-charge and departure time.</param>
        /// <param name="Pricing">The optional pricing structure type that will be offered.</param>
        /// <param name="DepartureTime">An optional estimated departure time of the EV.</param>
        /// <param name="ACChargingParameters">Optional ISO 15118-2 EV AC charging parameters.</param>
        /// <param name="DCChargingParameters">Optional ISO 15118-2 EV DC charging parameters.</param>
        /// <param name="V2XChargingParameters">Optional ISO 15118-20 EV charging parameters.</param>
        /// <param name="EVEnergyOffer"></param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public ChargingNeeds(EnergyTransferMode                RequestedEnergyTransferMode,
                             IEnumerable<EnergyTransferMode>?  AvailableEnergyTransferModes   = null,
                             ControlModes?                     ControlMode                    = null,
                             MobilityNeedsMode?                MobilityNeedsMode              = null,
                             PricingTypes?                     Pricing                        = null,
                             DateTime?                         DepartureTime                  = null,
                             ACChargingParameters?             ACChargingParameters           = null,
                             DCChargingParameters?             DCChargingParameters           = null,
                             V2XChargingParameters?            V2XChargingParameters          = null,
                             EVEnergyOffer?                    EVEnergyOffer                  = null,
                             CustomData?                       CustomData                     = null)

            : base(CustomData)

        {

            this.RequestedEnergyTransferMode   = RequestedEnergyTransferMode;
            this.AvailableEnergyTransferModes  = AvailableEnergyTransferModes?.Distinct() ?? Array.Empty<EnergyTransferMode>();
            this.ControlMode                   = ControlMode;
            this.MobilityNeedsMode             = MobilityNeedsMode;
            this.Pricing                       = Pricing;
            this.DepartureTime                 = DepartureTime;
            this.ACChargingParameters          = ACChargingParameters;
            this.DCChargingParameters          = DCChargingParameters;
            this.V2XChargingParameters         = V2XChargingParameters;
            this.EVEnergyOffer                 = EVEnergyOffer;


            unchecked
            {

                hashCode = this.RequestedEnergyTransferMode. GetHashCode()       * 31 ^
                           this.AvailableEnergyTransferModes.CalcHashCode()      * 29 ^
                          (this.ControlMode?.                GetHashCode() ?? 0) * 23 ^
                          (this.MobilityNeedsMode?.          GetHashCode() ?? 0) * 19 ^
                          (this.Pricing?.                    GetHashCode() ?? 0) * 17 ^
                          (this.DepartureTime?.              GetHashCode() ?? 0) * 13 ^
                          (this.ACChargingParameters?.       GetHashCode() ?? 0) * 11 ^
                          (this.DCChargingParameters?.       GetHashCode() ?? 0) *  7 ^
                          (this.V2XChargingParameters?.      GetHashCode() ?? 0) *  5 ^
                          (this.EVEnergyOffer?.              GetHashCode() ?? 0) *  3 ^
                           base.                             GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomChargingNeedsParser = null)

        /// <summary>
        /// Parse the given JSON representation of charging needs.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingNeedsParser">A delegate to parse custom CustomChargingNeeds JSON objects.</param>
        public static ChargingNeeds Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<ChargingNeeds>?  CustomChargingNeedsParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingNeeds,
                         out var errorResponse,
                         CustomChargingNeedsParser) &&
                chargingNeeds is not null)
            {
                return chargingNeeds;
            }

            throw new ArgumentException("The given JSON representation of charging needs is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingNeeds, out ErrorResponse, CustomChargingNeedsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of charging needs.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingNeeds">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out ChargingNeeds?  ChargingNeeds,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out ChargingNeeds,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of charging needs.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingNeeds">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingNeedsParser">A delegate to parse custom CustomChargingNeeds JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out ChargingNeeds?                           ChargingNeeds,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingNeeds>?  CustomChargingNeedsParser)
        {

            try
            {

                ChargingNeeds = default;

                #region RequestedEnergyTransferMode     [mandatory]

                if (!JSON.ParseMandatory("requestedEnergyTransfer",
                                         "requested energy transfer mode",
                                         EnergyTransferMode.TryParse,
                                         out EnergyTransferMode RequestedEnergyTransferMode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AvailableEnergyTransferModes    [optional]

                if (JSON.ParseOptionalHashSet("availableEnergyTransfer",
                                              "available energy transfer modes",
                                              EnergyTransferMode.TryParse,
                                              out HashSet<EnergyTransferMode> AvailableEnergyTransferModes,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion

                #region ControlMode                     [optional]

                if (JSON.ParseOptional("controlMode",
                                       "control mode",
                                       ControlModesExtensions.TryParse,
                                       out ControlModes? ControlMode,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MobilityNeedsMode               [optional]

                if (JSON.ParseOptional("mobilityNeedsMode",
                                       "mobility needs mode",
                                       OCPPv2_1.MobilityNeedsMode.TryParse,
                                       out MobilityNeedsMode? MobilityNeedsMode,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Pricing                         [optional]

                if (JSON.ParseOptional("pricing",
                                       "pricing",
                                       PricingTypesExtensions.TryParse,
                                       out PricingTypes? Pricing,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DepartureTime                   [optional]

                if (JSON.ParseOptional("departureTime",
                                       "departure time",
                                       out DateTime? DepartureTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion

                #region ACChargingParameters            [optional]

                if (JSON.ParseOptionalJSON("acChargingParameters",
                                           "AC charging parameters",
                                           OCPPv2_1.ACChargingParameters.TryParse,
                                           out ACChargingParameters? ACChargingParameters,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DCChargingParameters            [optional]

                if (JSON.ParseOptionalJSON("dcChargingParameters",
                                           "DC charging parameters",
                                           OCPPv2_1.DCChargingParameters.TryParse,
                                           out DCChargingParameters? DCChargingParameters,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region V2XChargingParameters           [optional]

                if (JSON.ParseOptionalJSON("v2xChargingParameters",
                                           "V2X charging parameters",
                                           OCPPv2_1.V2XChargingParameters.TryParse,
                                           out V2XChargingParameters? V2XChargingParameters,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVEnergyOffer                   [optional]

                if (JSON.ParseOptionalJSON("evEnergyOffer",
                                           "ev energy offer",
                                           OCPPv2_1.EVEnergyOffer.TryParse,
                                           out EVEnergyOffer? EVEnergyOffer,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                      [optional]

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


                ChargingNeeds = new ChargingNeeds(
                                    RequestedEnergyTransferMode,
                                    AvailableEnergyTransferModes,
                                    ControlMode,
                                    MobilityNeedsMode,
                                    Pricing,
                                    DepartureTime,
                                    ACChargingParameters,
                                    DCChargingParameters,
                                    V2XChargingParameters,
                                    EVEnergyOffer,
                                    CustomData
                                );

                if (CustomChargingNeedsParser is not null)
                    ChargingNeeds = CustomChargingNeedsParser(JSON,
                                                              ChargingNeeds);

                return true;

            }
            catch (Exception e)
            {
                ChargingNeeds  = default;
                ErrorResponse  = "The given JSON representation of charging needs is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomChargingNeedsSerializer = null, CustomACChargingParametersSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingNeedsSerializer">A delegate to serialize custom charging needs.</param>
        /// <param name="CustomACChargingParametersSerializer">A delegate to serialize custom AC charging parameters.</param>
        /// <param name="CustomDCChargingParametersSerializer">A delegate to serialize custom DC charging parameters.</param>
        /// <param name="CustomV2XChargingParametersSerializer">A delegate to serialize custom V2X charging parameters.</param>
        /// <param name="CustomEVEnergyOfferSerializer">A delegate to serialize custom ev energy offers.</param>
        /// <param name="CustomEVPowerScheduleSerializer">A delegate to serialize custom ev power schedules.</param>
        /// <param name="CustomEVPowerScheduleEntrySerializer">A delegate to serialize custom ev power schedule entries.</param>
        /// <param name="CustomEVAbsolutePriceScheduleSerializer">A delegate to serialize custom ev absolute price schedules.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntrySerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom ev price rules.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingNeeds>?                 CustomChargingNeedsSerializer                  = null,
                              CustomJObjectSerializerDelegate<ACChargingParameters>?          CustomACChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<DCChargingParameters>?          CustomDCChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<V2XChargingParameters>?         CustomV2XChargingParametersSerializer          = null,
                              CustomJObjectSerializerDelegate<EVEnergyOffer>?                 CustomEVEnergyOfferSerializer                  = null,
                              CustomJObjectSerializerDelegate<EVPowerSchedule>?               CustomEVPowerScheduleSerializer                = null,
                              CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?          CustomEVPowerScheduleEntrySerializer           = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?       CustomEVAbsolutePriceScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<EVPriceRule>?                   CustomEVPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestedEnergyTransfer",   RequestedEnergyTransferMode.ToString()),

                           DepartureTime.HasValue
                               ? new JProperty("availableEnergyTransfer",   new JArray(AvailableEnergyTransferModes.Select(availableEnergyTransferMode => availableEnergyTransferMode.ToString())))
                               : null,

                           ControlMode.      HasValue
                               ? new JProperty("controlMode",               ControlMode.          Value.AsText())
                               : null,

                           MobilityNeedsMode.HasValue
                               ? new JProperty("mobilityNeedsMode",         MobilityNeedsMode.    Value.ToString())
                               : null,

                           Pricing.          HasValue
                               ? new JProperty("pricing",                   Pricing.              Value.AsText())
                               : null,

                           DepartureTime.    HasValue
                               ? new JProperty("departureTime",             DepartureTime.        Value.ToIso8601())
                               : null,

                           ACChargingParameters  is not null
                               ? new JProperty("acChargingParameters",      ACChargingParameters.       ToJSON(CustomACChargingParametersSerializer,
                                                                                                               CustomCustomDataSerializer))
                               : null,

                           DCChargingParameters  is not null
                               ? new JProperty("dcChargingParameters",      DCChargingParameters.       ToJSON(CustomDCChargingParametersSerializer,
                                                                                                               CustomCustomDataSerializer))
                               : null,

                           V2XChargingParameters is not null
                               ? new JProperty("v2xChargingParameters",     V2XChargingParameters.      ToJSON(CustomV2XChargingParametersSerializer,
                                                                                                               CustomCustomDataSerializer))
                               : null,

                           EVEnergyOffer         is not null
                               ? new JProperty("evEnergyOffer",             EVEnergyOffer.              ToJSON(CustomEVEnergyOfferSerializer,
                                                                                                               CustomEVPowerScheduleSerializer,
                                                                                                               CustomEVPowerScheduleEntrySerializer,
                                                                                                               CustomEVAbsolutePriceScheduleSerializer,
                                                                                                               CustomEVAbsolutePriceScheduleEntrySerializer,
                                                                                                               CustomEVPriceRuleSerializer,
                                                                                                               CustomCustomDataSerializer))
                               : null,

                           CustomData            is not null
                               ? new JProperty("customData",                CustomData.                 ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChargingNeedsSerializer is not null
                       ? CustomChargingNeedsSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingNeeds1, ChargingNeeds2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingNeeds1">Charging needs.</param>
        /// <param name="ChargingNeeds2">Another charging needs.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingNeeds? ChargingNeeds1,
                                           ChargingNeeds? ChargingNeeds2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingNeeds1, ChargingNeeds2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingNeeds1 is null || ChargingNeeds2 is null)
                return false;

            return ChargingNeeds1.Equals(ChargingNeeds2);

        }

        #endregion

        #region Operator != (ChargingNeeds1, ChargingNeeds2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingNeeds1">Charging needs.</param>
        /// <param name="ChargingNeeds2">Another charging needs.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingNeeds? ChargingNeeds1,
                                           ChargingNeeds? ChargingNeeds2)

            => !(ChargingNeeds1 == ChargingNeeds2);

        #endregion

        #endregion

        #region IEquatable<ChargingNeeds> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging needs for equality..
        /// </summary>
        /// <param name="Object">Charging needs to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingNeeds chargingNeeds &&
                   Equals(chargingNeeds);

        #endregion

        #region Equals(ChargingNeeds)

        /// <summary>
        /// Compares two charging needs for equality.
        /// </summary>
        /// <param name="ChargingNeeds">Charging needs to compare with.</param>
        public Boolean Equals(ChargingNeeds? ChargingNeeds)

            => ChargingNeeds is not null &&

               RequestedEnergyTransferMode.Equals(ChargingNeeds.RequestedEnergyTransferMode) &&

               AvailableEnergyTransferModes.Count().Equals(ChargingNeeds.AvailableEnergyTransferModes.Count()) &&
               AvailableEnergyTransferModes.All(energyTransferMode => ChargingNeeds.AvailableEnergyTransferModes.Contains(energyTransferMode)) &&

            ((!ControlMode.          HasValue    && !ChargingNeeds. ControlMode.          HasValue)    ||
              (ControlMode.          HasValue    &&  ChargingNeeds. ControlMode.          HasValue    && ControlMode.      Value.Equals(ChargingNeeds.ControlMode.      Value))) &&

            ((!MobilityNeedsMode.    HasValue    && !ChargingNeeds. MobilityNeedsMode.    HasValue)    ||
              (MobilityNeedsMode.    HasValue    &&  ChargingNeeds. MobilityNeedsMode.    HasValue    && MobilityNeedsMode.Value.Equals(ChargingNeeds.MobilityNeedsMode.Value))) &&

            ((!Pricing.              HasValue    && !ChargingNeeds. Pricing.              HasValue)    ||
              (Pricing.              HasValue    &&  ChargingNeeds. Pricing.              HasValue    && Pricing.          Value.Equals(ChargingNeeds.Pricing.          Value))) &&

            ((!DepartureTime.        HasValue    && !ChargingNeeds. DepartureTime.        HasValue)    ||
              (DepartureTime.        HasValue    &&  ChargingNeeds. DepartureTime.        HasValue    && DepartureTime.    Value.Equals(ChargingNeeds.DepartureTime.    Value))) &&

             ((ACChargingParameters  is     null &&  ChargingNeeds. ACChargingParameters  is     null) ||
              (ACChargingParameters  is not null &&  ChargingNeeds. ACChargingParameters  is not null && ACChargingParameters.   Equals(ChargingNeeds.ACChargingParameters)))    &&

             ((DCChargingParameters  is     null &&  ChargingNeeds. DCChargingParameters  is     null) ||
              (DCChargingParameters  is not null &&  ChargingNeeds. DCChargingParameters  is not null && DCChargingParameters.   Equals(ChargingNeeds.DCChargingParameters)))    &&

             ((V2XChargingParameters is     null &&  ChargingNeeds. V2XChargingParameters is     null) ||
              (V2XChargingParameters is not null &&  ChargingNeeds. V2XChargingParameters is not null && V2XChargingParameters.  Equals(ChargingNeeds.V2XChargingParameters)))   &&

             ((EVEnergyOffer         is     null &&  ChargingNeeds. EVEnergyOffer         is     null) ||
              (EVEnergyOffer         is not null &&  ChargingNeeds. EVEnergyOffer         is not null && EVEnergyOffer.          Equals(ChargingNeeds.EVEnergyOffer)))           &&

               base.Equals(ChargingNeeds);

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

                   RequestedEnergyTransferMode.ToString(),

                   AvailableEnergyTransferModes.Any()
                      ? $", available energy transfer modes: {AvailableEnergyTransferModes.AggregateWith(", ")}"
                      : "",

                   ControlMode.HasValue
                      ? $", control mode: {ControlMode.Value.AsText()}"
                      : "",

                   MobilityNeedsMode.HasValue
                      ? $", mobility needs mode: {MobilityNeedsMode.Value.ToString()}"
                      : "",

                   Pricing.HasValue
                      ? $", pricing: {Pricing.Value.AsText()}"
                      : "",

                   DepartureTime.HasValue
                      ? $", departure time: {DepartureTime.Value.ToIso8601()}"
                      : "",

                   ACChargingParameters is not null
                      ? $", AC charging parameters: {ACChargingParameters}"
                      : "",

                   DCChargingParameters is not null
                      ? $", DC charging parameters: {DCChargingParameters}"
                      : "",

                   V2XChargingParameters is not null
                      ? $", V2X charging parameters: {V2XChargingParameters}"
                      : "",

                   EVEnergyOffer is not null
                      ? $", EV energy offer: {EVEnergyOffer}"
                      : ""

               );

        #endregion

    }

}
