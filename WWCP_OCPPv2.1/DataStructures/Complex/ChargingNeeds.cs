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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Charging needs.
    /// </summary>
    public class ChargingNeeds : ACustomData,
                                 IEquatable<ChargingNeeds>
    {

        #region Properties

        /// <summary>
        /// The source of the charging needs.
        /// </summary>
        [Mandatory]
        public EnergyTransferModes               RequestedEnergyTransfer    { get; }

        /// <summary>
        /// The enumeration of energy transfer modes marked as available by the EV.
        /// </summary>
        [Optional]
        public IEnumerable<EnergyTransferModes>  AvailableEnergyTransfer    { get; }

        /// <summary>
        /// Optional indication whether the EV wants to operate in dynamic or scheduled mode.
        /// When absent, scheduled mode is assumed.
        /// </summary>
        [Optional]
        public ControlModes?                     ControlMode                { get; }

        /// <summary>
        /// Optional indication whether only the EV or also the EVSE or CSMS
        /// determines min/target state-of-charge and departure time.
        /// </summary>
        [Optional]
        public MobilityNeedsModes?               MobilityNeedsMode          { get; }

        /// <summary>
        /// The optional pricing structure type that will be offered.
        /// </summary>
        [Optional]
        public PricingTypes?                     Pricing                    { get; }

        /// <summary>
        /// The optional indication whether the charging needs is critical for the grid.
        /// </summary>
        [Optional]
        public DateTime?                         DepartureTime              { get; }

        /// <summary>
        /// Optional EV AC charging parameters.
        /// </summary>
        [Optional]
        public ACChargingParameters?             ACChargingParameters       { get; }

        /// <summary>
        /// Optional EV DC charging parameters.
        /// </summary>
        [Optional]
        public DCChargingParameters?             DCChargingParameters       { get; }

        /// <summary>
        /// Optional EV ISO 15118-20 charging parameters.
        /// </summary>
        [Optional]
        public V2XChargingParameters?            V2XChargingParameters      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging needs.
        /// </summary>
        /// <param name="RequestedEnergyTransfer">The source of the charging needs.</param>
        /// <param name="DepartureTime">An optional indication whether the charging needs is critical for the grid.</param>
        /// <param name="ACChargingParameters">Optional EV AC charging parameters.</param>
        /// <param name="DCChargingParameters">Optional EV DC charging parameters.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public ChargingNeeds(EnergyTransferModes    RequestedEnergyTransfer,
                             DateTime?              DepartureTime,
                             ACChargingParameters?  ACChargingParameters,
                             DCChargingParameters?  DCChargingParameters,
                             CustomData?            CustomData   = null)

            : base(CustomData)

        {

            this.RequestedEnergyTransfer  = RequestedEnergyTransfer;
            this.DepartureTime            = DepartureTime;
            this.ACChargingParameters     = ACChargingParameters;
            this.DCChargingParameters     = DCChargingParameters;

        }

        #endregion


        #region Documentation

        // "ChargingNeedsType": {
        //   "description": "Charging_ Needs\r\nurn:x-oca:ocpp:uid:2:233249\r\n",
        //   "javaType": "ChargingNeeds",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "acChargingParameters": {
        //       "$ref": "#/definitions/ACChargingParametersType"
        //     },
        //     "dcChargingParameters": {
        //       "$ref": "#/definitions/DCChargingParametersType"
        //     },
        //     "requestedEnergyTransfer": {
        //       "$ref": "#/definitions/EnergyTransferModeEnumType"
        //     },
        //     "departureTime": {
        //       "description": "Charging_ Needs. Departure_ Time. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569223\r\nEstimated departure time of the EV.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     }
        //   },
        //   "required": [
        //     "requestedEnergyTransfer"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomChargingNeedsParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging needs.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingNeedsParser">A delegate to parse custom CustomChargingNeeds JSON objects.</param>
        public static ChargingNeeds Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<ChargingNeeds>?  CustomChargingNeedsParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingNeeds,
                         out var errorResponse,
                         CustomChargingNeedsParser))
            {
                return chargingNeeds!;
            }

            throw new ArgumentException("The given JSON representation of a charging needs is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingNeeds, out ErrorResponse, CustomChargingNeedsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging needs.
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
        /// Try to parse the given JSON representation of a charging needs.
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

                #region RequestedEnergyTransfer    [mandatory]

                if (!JSON.ParseMandatory("requestedEnergyTransfer",
                                         "requested energy transfer",
                                         EnergyTransferModesExtensions.TryParse,
                                         out EnergyTransferModes RequestedEnergyTransfer,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region DepartureTime              [optional]

                if (JSON.ParseOptional("departureTime",
                                       "departure time",
                                       out DateTime? DepartureTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion

                #region ACChargingParameters       [optional]

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

                #region DCChargingParameters       [optional]

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

                #region CustomData                 [optional]

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


                ChargingNeeds = new ChargingNeeds(RequestedEnergyTransfer,
                                                  DepartureTime,
                                                  ACChargingParameters,
                                                  DCChargingParameters,
                                                  CustomData);

                if (CustomChargingNeedsParser is not null)
                    ChargingNeeds = CustomChargingNeedsParser(JSON,
                                                              ChargingNeeds);

                return true;

            }
            catch (Exception e)
            {
                ChargingNeeds  = default;
                ErrorResponse  = "The given JSON representation of a charging needs is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomChargingNeedsSerializer = null, CustomACChargingParametersSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingNeedsSerializer">A delegate to serialize custom charging needss.</param>
        /// <param name="CustomACChargingParametersSerializer">A delegate to serialize custom AC charging parameters.</param>
        /// <param name="CustomDCChargingParametersSerializer">A delegate to serialize custom DC charging parameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingNeeds>?         CustomChargingNeedsSerializer          = null,
                              CustomJObjectSerializerDelegate<ACChargingParameters>?  CustomACChargingParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<DCChargingParameters>?  CustomDCChargingParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestedEnergyTransfer",   RequestedEnergyTransfer.AsText()),

                           DepartureTime.HasValue
                               ? new JProperty("departureTime",             DepartureTime.Value.    ToIso8601())
                               : null,

                           ACChargingParameters is not null
                               ? new JProperty("acChargingParameters",      ACChargingParameters.   ToJSON(CustomACChargingParametersSerializer,
                                                                                                           CustomCustomDataSerializer))
                               : null,

                           DCChargingParameters is not null
                               ? new JProperty("dcChargingParameters",      DCChargingParameters.   ToJSON(CustomDCChargingParametersSerializer,
                                                                                                           CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.             ToJSON(CustomCustomDataSerializer))
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
        /// Compares two charging needss for equality..
        /// </summary>
        /// <param name="Object">Charging needs to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingNeeds chargingNeeds &&
                   Equals(chargingNeeds);

        #endregion

        #region Equals(ChargingNeeds)

        /// <summary>
        /// Compares two charging needss for equality.
        /// </summary>
        /// <param name="ChargingNeeds">Charging needs to compare with.</param>
        public Boolean Equals(ChargingNeeds? ChargingNeeds)

            => ChargingNeeds is not null &&

               RequestedEnergyTransfer.Equals(ChargingNeeds.RequestedEnergyTransfer) &&

            ((!DepartureTime.       HasValue    && !ChargingNeeds.DepartureTime.       HasValue)    ||
              (DepartureTime.       HasValue    &&  ChargingNeeds.DepartureTime.       HasValue    && DepartureTime.Value. Equals(ChargingNeeds.DepartureTime.Value)))  &&

             ((ACChargingParameters is     null && ChargingNeeds. ACChargingParameters is     null) ||
              (ACChargingParameters is not null && ChargingNeeds. ACChargingParameters is not null && ACChargingParameters.Equals(ChargingNeeds.ACChargingParameters))) &&

             ((DCChargingParameters is     null && ChargingNeeds. DCChargingParameters is     null) ||
              (DCChargingParameters is not null && ChargingNeeds. DCChargingParameters is not null && DCChargingParameters.Equals(ChargingNeeds.DCChargingParameters))) &&

               base.                   Equals(ChargingNeeds);

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

                return RequestedEnergyTransfer.GetHashCode()       * 11 ^
                      (DepartureTime?.         GetHashCode() ?? 0) *  7 ^
                      (ACChargingParameters?.  GetHashCode() ?? 0) *  5 ^
                      (DCChargingParameters?.  GetHashCode() ?? 0) *  3 ^

                       base.                   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   RequestedEnergyTransfer,

                   DepartureTime.HasValue
                      ? ", Departure time: "       + DepartureTime.Value. ToIso8601()
                      : "",

                   ACChargingParameters is not null
                      ? ", ACChargingParameters: " + ACChargingParameters.ToString()
                      : "",

                   DCChargingParameters is not null
                      ? ", DCChargingParameters: " + DCChargingParameters.ToString()
                      : ""

               );

        #endregion

    }

}
