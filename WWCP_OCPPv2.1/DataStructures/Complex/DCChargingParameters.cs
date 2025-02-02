/*
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DC charging parameters.
    /// </summary>
    public class DCChargingParameters : ACustomData,
                                        IEquatable<DCChargingParameters>
    {

        #region Properties

        /// <summary>
        /// The maximum current (amperes) supported by the electric vehicle (per phase)
        /// including the maximum cable capacity.
        /// </summary>
        [Mandatory]
        public Ampere          EVMaxCurrent        { get; }

        /// <summary>
        /// The maximum voltage (volts) supported by the electric vehicle.
        /// </summary>
        [Mandatory]
        public Volt            EVMaxVoltage        { get; }

        /// <summary>
        /// The optional amount of energy requested (in Wh).
        /// This includes energy required for preconditioning.
        /// </summary>
        [Optional]
        public WattHour?       EnergyAmount        { get; }

        /// <summary>
        /// The optional maximum power (in W) supported by the electric vehicle.
        /// Required for DC charging.
        /// </summary>
        [Optional]
        public Watt?           EVMaxPower          { get; }

        /// <summary>
        /// The optional energy available in the battery (in percent of the battery capacity).
        /// </summary>
        [Optional]
        public PercentageByte?  StateOfCharge       { get; }

        /// <summary>
        /// The optional capacity of the electric vehicle battery (in Wh).
        /// </summary>
        [Optional]
        public WattHour?       EVEnergyCapacity    { get; }

        /// <summary>
        /// The optional percentage of SoC at which the EV considers the battery fully charged.
        /// </summary>
        [Optional]
        public PercentageByte?  FullSoC             { get; }

        /// <summary>
        /// The optional percentage of SoC at which the EV considers a fast charging process to end.
        /// </summary>
        [Optional]
        public PercentageByte?  BulkSoC             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DC charging parameters.
        /// </summary>
        /// <param name="EVMaxCurrent">The maximum current (amperes) supported by the electric vehicle (per phase) including the maximum cable capacity.</param>
        /// <param name="EVMaxVoltage">The maximum voltage (volts) supported by the electric vehicle.</param>
        /// <param name="EnergyAmount">The optional amount of energy requested (in Wh). This includes energy required for preconditioning.</param>
        /// <param name="EVMaxPower">The optional maximum power (in W) supported by the electric vehicle. Required for DC charging.</param>
        /// <param name="StateOfCharge">The optional energy available in the battery (in percent of the battery capacity).</param>
        /// <param name="EVEnergyCapacity">The optional capacity of the electric vehicle battery (in Wh).</param>
        /// <param name="FullSoC">The optional percentage of SoC at which the EV considers the battery fully charged.</param>
        /// <param name="BulkSoC">The optional percentage of SoC at which the EV considers a fast charging process to end.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DCChargingParameters(Ampere           EVMaxCurrent,
                                    Volt             EVMaxVoltage,
                                    WattHour?        EnergyAmount,
                                    Watt?            EVMaxPower,
                                    PercentageByte?  StateOfCharge,
                                    WattHour?        EVEnergyCapacity,
                                    PercentageByte?  FullSoC,
                                    PercentageByte?  BulkSoC,
                                    CustomData?      CustomData   = null)

            : base(CustomData)

        {

            this.EVMaxCurrent      = EVMaxCurrent;
            this.EVMaxVoltage      = EVMaxVoltage;
            this.EnergyAmount      = EnergyAmount;
            this.EVMaxPower        = EVMaxPower;
            this.StateOfCharge     = StateOfCharge;
            this.EVEnergyCapacity  = EVEnergyCapacity;
            this.FullSoC           = FullSoC;
            this.BulkSoC           = BulkSoC;

            unchecked
            {

                hashCode = EVMaxCurrent.     GetHashCode()       * 23 ^
                           EVMaxVoltage.     GetHashCode()       * 19 ^
                          (EnergyAmount?.    GetHashCode() ?? 0) * 17 ^
                          (EVMaxPower?.      GetHashCode() ?? 0) * 13 ^
                          (StateOfCharge?.   GetHashCode() ?? 0) * 11 ^
                          (EVEnergyCapacity?.GetHashCode() ?? 0) *  7 ^
                          (FullSoC?.         GetHashCode() ?? 0) *  5 ^
                          (BulkSoC?.         GetHashCode() ?? 0) *  3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "EV DC charging parameters for ISO 15118-2",
        //     "javaType": "DCChargingParameters",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "evMaxCurrent": {
        //             "description": "Maximum current (in A) supported by the electric vehicle. Includes cable capacity.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType:EVMaximumCurrentLimit",
        //             "type": "number"
        //         },
        //         "evMaxVoltage": {
        //             "description": "Maximum voltage supported by the electric vehicle.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVMaximumVoltageLimit",
        //             "type": "number"
        //         },
        //         "evMaxPower": {
        //             "description": "Maximum power (in W) supported by the electric vehicle. Required for DC charging.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVMaximumPowerLimit",
        //             "type": "number"
        //         },
        //         "evEnergyCapacity": {
        //             "description": "Capacity of the electric vehicle battery (in Wh).\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVEnergyCapacity",
        //             "type": "number"
        //         },
        //         "energyAmount": {
        //             "description": "Amount of energy requested (in Wh). This inludes energy required for preconditioning.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVEnergyRequest",
        //             "type": "number"
        //         },
        //         "stateOfCharge": {
        //             "description": "Energy available in the battery (in percent of the battery capacity)\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: DC_EVStatus: EVRESSSOC",
        //             "type": "integer",
        //             "minimum": 0.0,
        //             "maximum": 100.0
        //         },
        //         "fullSoC": {
        //             "description": "Percentage of SoC at which the EV considers the battery fully charged. (possible values: 0 - 100)\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: FullSOC",
        //             "type": "integer",
        //             "minimum": 0.0,
        //             "maximum": 100.0
        //         },
        //         "bulkSoC": {
        //             "description": "Percentage of SoC at which the EV considers a fast charging process to end. (possible values: 0 - 100)\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: BulkSOC",
        //             "type": "integer",
        //             "minimum": 0.0,
        //             "maximum": 100.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "evMaxCurrent",
        //         "evMaxVoltage"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomDCChargingParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of DC charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDCChargingParametersParser">A delegate to parse custom DC charging parameters JSON objects.</param>
        public static DCChargingParameters Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<DCChargingParameters>?  CustomDCChargingParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var dcChargingParameters,
                         out var errorResponse,
                         CustomDCChargingParametersParser))
            {
                return dcChargingParameters;
            }

            throw new ArgumentException("The given JSON representation of DC charging parameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DCChargingParameters, out ErrorResponse, CustomDCChargingParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of DC charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DCChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out DCChargingParameters?  DCChargingParameters,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out DCChargingParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of DC charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DCChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDCChargingParametersParser">A delegate to parse custom DC charging parameters JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out DCChargingParameters?      DCChargingParameters,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<DCChargingParameters>?  CustomDCChargingParametersParser)
        {

            try
            {

                DCChargingParameters = default;

                #region EVMaxCurrent        [mandatory]

                if (!JSON.ParseMandatory("evMaxCurrent",
                                         "ev max current",
                                         Ampere.TryParse,
                                         out Ampere EVMaxCurrent,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVMaxVoltage        [mandatory]

                if (!JSON.ParseMandatory("evMaxVoltage",
                                         "ev max voltage",
                                         Volt.TryParse,
                                         out Volt EVMaxVoltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EnergyAmount        [optional]

                if (JSON.ParseOptional("energyAmount",
                                       "energy amount",
                                       WattHour.TryParse,
                                       out WattHour? EnergyAmount,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVMaxPower          [optional]

                if (JSON.ParseOptional("evMaxPower",
                                       "EV max power",
                                       Watt.TryParse,
                                       out Watt? EVMaxPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StateOfCharge       [optional]

                if (JSON.ParseOptional("stateOfCharge",
                                       "state of charge",
                                       PercentageByte.TryParse,
                                       out PercentageByte? StateOfCharge,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVEnergyCapacity    [optional]

                if (JSON.ParseOptional("evEnergyCapacity",
                                       "ev energy capacity",
                                       WattHour.TryParse,
                                       out WattHour? EVEnergyCapacity,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region FullSoC             [optional]

                if (JSON.ParseOptional("fullSoC",
                                       "full state of charge",
                                       PercentageByte.TryParse,
                                       out PercentageByte? FullSoC,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region BulkSoC             [optional]

                if (JSON.ParseOptional("bulkSoC",
                                       "bulk state of charge",
                                       PercentageByte.TryParse,
                                       out PercentageByte? BulkSoC,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData          [optional]

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


                DCChargingParameters = new DCChargingParameters(
                                           EVMaxCurrent,
                                           EVMaxVoltage,
                                           EnergyAmount,
                                           EVMaxPower,
                                           StateOfCharge,
                                           EVEnergyCapacity,
                                           FullSoC,
                                           BulkSoC,
                                           CustomData
                                       );

                if (CustomDCChargingParametersParser is not null)
                    DCChargingParameters = CustomDCChargingParametersParser(JSON,
                                                                            DCChargingParameters);

                return true;

            }
            catch (Exception e)
            {
                DCChargingParameters  = default;
                ErrorResponse         = "The given JSON representation of DC charging parameters is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomDCChargingParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDCChargingParametersSerializer">A delegate to serialize custom DC charging parameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DCChargingParameters>?  CustomDCChargingParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evMaxCurrent",       EVMaxCurrent.          IntegerValue),
                                 new JProperty("evMaxVoltage",       EVMaxVoltage.          IntegerValue),

                           EnergyAmount.HasValue
                               ? new JProperty("energyAmount",       EnergyAmount.    Value.IntegerValue)
                               : null,

                           EVMaxPower.HasValue
                               ? new JProperty("evMaxPower",         EVMaxPower.      Value.IntegerValue)
                               : null,

                           StateOfCharge.HasValue
                               ? new JProperty("stateOfCharge",      StateOfCharge.   Value.Value)
                               : null,

                           EVEnergyCapacity.HasValue
                               ? new JProperty("evEnergyCapacity",   EVEnergyCapacity.Value.IntegerValue)
                               : null,

                           FullSoC.HasValue
                               ? new JProperty("fullSoC",            FullSoC.         Value.Value)
                               : null,

                           BulkSoC.HasValue
                               ? new JProperty("bulkSoC",            BulkSoC.         Value.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDCChargingParametersSerializer is not null
                       ? CustomDCChargingParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DCChargingParameters1, DCChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DCChargingParameters1">DC charging parameters.</param>
        /// <param name="DCChargingParameters2">Another DC charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DCChargingParameters? DCChargingParameters1,
                                           DCChargingParameters? DCChargingParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DCChargingParameters1, DCChargingParameters2))
                return true;

            // If one is null, but not both, return false.
            if (DCChargingParameters1 is null || DCChargingParameters2 is null)
                return false;

            return DCChargingParameters1.Equals(DCChargingParameters2);

        }

        #endregion

        #region Operator != (DCChargingParameters1, DCChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DCChargingParameters1">DC charging parameters.</param>
        /// <param name="DCChargingParameters2">Another DC charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DCChargingParameters? DCChargingParameters1,
                                           DCChargingParameters? DCChargingParameters2)

            => !(DCChargingParameters1 == DCChargingParameters2);

        #endregion

        #endregion

        #region IEquatable<DCChargingParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DC charging parameters for equality.
        /// </summary>
        /// <param name="Object">DC charging parameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DCChargingParameters dcChargingParameters &&
                   Equals(dcChargingParameters);

        #endregion

        #region Equals(DCChargingParameters)

        /// <summary>
        /// Compares two DC charging parameters for equality.
        /// </summary>
        /// <param name="DCChargingParameters">DC charging parameters to compare with.</param>
        public Boolean Equals(DCChargingParameters? DCChargingParameters)

            => DCChargingParameters is not null &&

               EVMaxCurrent.Equals(DCChargingParameters.EVMaxCurrent) &&
               EVMaxVoltage.Equals(DCChargingParameters.EVMaxVoltage) &&

            ((!EnergyAmount.    HasValue && !DCChargingParameters.EnergyAmount.    HasValue) ||
               EnergyAmount.    HasValue &&  DCChargingParameters.EnergyAmount.    HasValue && EnergyAmount.    Value.Equals(DCChargingParameters.EnergyAmount.    Value)) &&

            ((!EVMaxPower.      HasValue && !DCChargingParameters.EVMaxPower.      HasValue) ||
               EVMaxPower.      HasValue &&  DCChargingParameters.EVMaxPower.      HasValue && EVMaxPower.      Value.Equals(DCChargingParameters.EVMaxPower.      Value)) &&

            ((!StateOfCharge.   HasValue && !DCChargingParameters.StateOfCharge.   HasValue) ||
               StateOfCharge.   HasValue &&  DCChargingParameters.StateOfCharge.   HasValue && StateOfCharge.   Value.Equals(DCChargingParameters.StateOfCharge.   Value)) &&

            ((!EVEnergyCapacity.HasValue && !DCChargingParameters.EVEnergyCapacity.HasValue) ||
               EVEnergyCapacity.HasValue &&  DCChargingParameters.EVEnergyCapacity.HasValue && EVEnergyCapacity.Value.Equals(DCChargingParameters.EVEnergyCapacity.Value)) &&

            ((!FullSoC.         HasValue && !DCChargingParameters.FullSoC.         HasValue) ||
               FullSoC.         HasValue &&  DCChargingParameters.FullSoC.         HasValue && FullSoC.         Value.Equals(DCChargingParameters.FullSoC.         Value)) &&

            ((!BulkSoC.         HasValue && !DCChargingParameters.BulkSoC.         HasValue) ||
               BulkSoC.         HasValue &&  DCChargingParameters.BulkSoC.         HasValue && BulkSoC.         Value.Equals(DCChargingParameters.BulkSoC.         Value)) &&

               base.        Equals(DCChargingParameters);

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

                   EVMaxCurrent, " A, ",
                   EVMaxVoltage, " V",

                   EnergyAmount.    HasValue ? ", " + EnergyAmount     + " Wh" : "",
                   EVMaxPower.      HasValue ? ", " + EVMaxPower       + " W"  : "",
                   StateOfCharge.   HasValue ? ", " + StateOfCharge    + "%"   : "",
                   EVEnergyCapacity.HasValue ? ", " + EVEnergyCapacity + " Wh" : "",
                   FullSoC.         HasValue ? ", " + FullSoC          + "%"   : "",
                   BulkSoC.         HasValue ? ", " + BulkSoC          + "%"   : ""

               );

        #endregion


    }

}
