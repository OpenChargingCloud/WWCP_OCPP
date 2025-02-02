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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// AC charging parameters.
    /// </summary>
    public class ACChargingParameters : ACustomData,
                                        IEquatable<ACChargingParameters>
    {

        #region Properties

        /// <summary>
        /// The amount of energy requested (in Wh).
        /// This includes energy required for preconditioning.
        /// </summary>
        [Mandatory]
        public WattHour  EnergyAmount    { get; }

        /// <summary>
        /// The minimum current (in A) supported by the electric vehicle (per phase).
        /// </summary>
        [Mandatory]
        public Ampere    EVMinCurrent    { get; }

        /// <summary>
        /// The maximum current (in A) supported by the electric vehicle (per phase)
        /// including the maximum cable capacity.
        /// </summary>
        [Mandatory]
        public Ampere    EVMaxCurrent    { get; }

        /// <summary>
        /// The maximum voltage (in V) supported by the electric vehicle.
        /// </summary>
        [Mandatory]
        public Volt      EVMaxVoltage    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AC charging parameters.
        /// </summary>
        /// <param name="EnergyAmount">The amount of energy requested (in Wh). This includes energy required for preconditioning.</param>
        /// <param name="EVMinCurrent">The minimum current (in A) supported by the electric vehicle (per phase).</param>
        /// <param name="EVMaxCurrent">The maximum current (in A) supported by the electric vehicle (per phase) including the maximum cable capacity.</param>
        /// <param name="EVMaxVoltage">The maximum voltage (in V) supported by the electric vehicle.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ACChargingParameters(WattHour     EnergyAmount,
                                    Ampere       EVMinCurrent,
                                    Ampere       EVMaxCurrent,
                                    Volt         EVMaxVoltage,
                                    CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.EnergyAmount  = EnergyAmount;
            this.EVMinCurrent  = EVMinCurrent;
            this.EVMaxCurrent  = EVMaxCurrent;
            this.EVMaxVoltage  = EVMaxVoltage;

            unchecked
            {

                hashCode = EnergyAmount.GetHashCode() * 11 ^
                           EVMinCurrent.GetHashCode() *  7 ^
                           EVMaxCurrent.GetHashCode() *  5 ^
                           EVMaxVoltage.GetHashCode() *  3 ^
                           base.        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "EV AC charging parameters for ISO 15118-2",
        //     "javaType": "ACChargingParameters",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "energyAmount": {
        //             "description": "Amount of energy requested (in Wh). This includes energy required for preconditioning.\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EAmount +\r\n*ISO 15118-20*: Dynamic/Scheduled_SEReqControlModeType: EVTargetEnergyRequest",
        //             "type": "number"
        //         },
        //         "evMinCurrent": {
        //             "description": "Minimum current (amps) supported by the electric vehicle (per phase).\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EVMinCurrent",
        //             "type": "number"
        //         },
        //         "evMaxCurrent": {
        //             "description": "Maximum current (amps) supported by the electric vehicle (per phase). Includes cable capacity.\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EVMaxCurrent",
        //             "type": "number"
        //         },
        //         "evMaxVoltage": {
        //             "description": "Maximum voltage supported by the electric vehicle.\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EVMaxVoltage",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "energyAmount",
        //         "evMinCurrent",
        //         "evMaxCurrent",
        //         "evMaxVoltage"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomACChargingParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of AC charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomACChargingParametersParser">A delegate to parse custom AC charging parameters JSON objects.</param>
        public static ACChargingParameters Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<ACChargingParameters>?  CustomACChargingParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var acChargingParameters,
                         out var errorResponse,
                         CustomACChargingParametersParser))
            {
                return acChargingParameters;
            }

            throw new ArgumentException("The given JSON representation of AC charging parameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ACChargingParameters, out ErrorResponse, CustomACChargingParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of AC charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ACChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out ACChargingParameters?  ACChargingParameters,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out ACChargingParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of AC charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ACChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomACChargingParametersParser">A delegate to parse custom AC charging parameters JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out ACChargingParameters?      ACChargingParameters,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       CustomJObjectParserDelegate<ACChargingParameters>?  CustomACChargingParametersParser)
        {

            try
            {

                ACChargingParameters = default;

                #region EnergyAmount    [mandatory]

                if (!JSON.ParseMandatory("energyAmount",
                                         "energy amount",
                                         WattHour.TryParse,
                                         out WattHour EnergyAmount,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVMinCurrent    [mandatory]

                if (!JSON.ParseMandatory("evMinCurrent",
                                         "ev min current",
                                         Ampere.TryParse,
                                         out Ampere EVMinCurrent,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVMaxCurrent    [mandatory]

                if (!JSON.ParseMandatory("evMaxCurrent",
                                         "ev max current",
                                         Ampere.TryParse,
                                         out Ampere EVMaxCurrent,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVMaxVoltage    [mandatory]

                if (!JSON.ParseMandatory("evMaxVoltage",
                                         "ev max voltage",
                                         Volt.TryParse,
                                         out Volt EVMaxVoltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData      [optional]

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


                ACChargingParameters = new ACChargingParameters(
                                           EnergyAmount,
                                           EVMinCurrent,
                                           EVMaxCurrent,
                                           EVMaxVoltage,
                                           CustomData
                                       );

                if (CustomACChargingParametersParser is not null)
                    ACChargingParameters = CustomACChargingParametersParser(JSON,
                                                                            ACChargingParameters);

                return true;

            }
            catch (Exception e)
            {
                ACChargingParameters  = default;
                ErrorResponse         = "The given JSON representation of AC charging parameters is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomACChargingParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomACChargingParametersSerializer">A delegate to serialize custom AC charging parameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ACChargingParameters>?  CustomACChargingParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("energyAmount",   EnergyAmount.IntegerValue),
                                 new JProperty("evMinCurrent",   EVMinCurrent.IntegerValue),
                                 new JProperty("evMaxCurrent",   EVMaxCurrent.IntegerValue),
                                 new JProperty("evMaxVoltage",   EVMaxVoltage.IntegerValue),

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomACChargingParametersSerializer is not null
                       ? CustomACChargingParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ACChargingParameters1, ACChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ACChargingParameters1">AC charging parameters.</param>
        /// <param name="ACChargingParameters2">Another AC charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ACChargingParameters? ACChargingParameters1,
                                           ACChargingParameters? ACChargingParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ACChargingParameters1, ACChargingParameters2))
                return true;

            // If one is null, but not both, return false.
            if (ACChargingParameters1 is null || ACChargingParameters2 is null)
                return false;

            return ACChargingParameters1.Equals(ACChargingParameters2);

        }

        #endregion

        #region Operator != (ACChargingParameters1, ACChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ACChargingParameters1">AC charging parameters.</param>
        /// <param name="ACChargingParameters2">Another AC charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ACChargingParameters? ACChargingParameters1,
                                           ACChargingParameters? ACChargingParameters2)

            => !(ACChargingParameters1 == ACChargingParameters2);

        #endregion

        #endregion

        #region IEquatable<ACChargingParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AC charging parameters for equality..
        /// </summary>
        /// <param name="Object">AC charging parameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ACChargingParameters acChargingParameters &&
                   Equals(acChargingParameters);

        #endregion

        #region Equals(ACChargingParameters)

        /// <summary>
        /// Compares two AC charging parameters for equality.
        /// </summary>
        /// <param name="ACChargingParameters">AC charging parameters to compare with.</param>
        public Boolean Equals(ACChargingParameters? ACChargingParameters)

            => ACChargingParameters is not null &&

               EnergyAmount.Equals(ACChargingParameters.EnergyAmount) &&
               EVMinCurrent.Equals(ACChargingParameters.EVMinCurrent) &&
               EVMaxCurrent.Equals(ACChargingParameters.EVMaxCurrent) &&
               EVMaxVoltage.Equals(ACChargingParameters.EVMaxVoltage) &&

               base.Equals(ACChargingParameters);

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
                   EnergyAmount, " Wh, ",
                   EVMinCurrent, " A, ",
                   EVMaxCurrent, " A, ",
                   EVMaxVoltage, " V"
               );

        #endregion

    }

}
