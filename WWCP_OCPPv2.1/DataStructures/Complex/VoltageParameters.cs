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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Voltage Parameters
    /// </summary>
    public class VoltageParameters : ACustomData,
                                     ICBORSerializable<VoltageParameters>,
                                     IEquatable<VoltageParameters>
    {

        #region Properties

        /// <summary>
        /// 10-Minute Mean High Voltage Value (EN 50549-1 chapter 4.9.3.4)
        /// 
        /// Voltage threshold for the 10 min time window mean value monitoring.
        /// The 10 min mean is recalculated up to every 3 s.
        /// If the present voltage is above this threshold for more than the time defined by _hv10MinMeanValue_, the EV must trip.
        /// This value is mandatory if _hv10MinMeanTripDelay_ is set.
        /// </summary>
        [Optional]
        public Decimal?               HighVoltage_10Min_MeanValue        { get; }

        /// <summary>
        /// 10-Minute Mean High Voltage Trip Delay
        /// 
        /// Time for which the voltage is allowed to stay above the 10 min mean value.
        /// After this time, the EV must trip (disconnect).
        /// This value is mandatory if OverVoltageMeanValue10min is set.
        /// </summary>
        [Optional]
        public TimeSpan?              HighVoltage_10Min_MeanTripDelay    { get; }

        /// <summary>
        /// The optional power during cessation.
        /// </summary>
        [Optional]
        public PowerDuringCessation?  PowerDuringCessation               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new VoltageParameters.
        /// </summary>
        /// <param name="HighVoltage_10Min_MeanValue">10-Minute Mean High Voltage Value</param>
        /// <param name="HighVoltage_10Min_MeanTripDelay">10-Minute Mean High Voltage Trip Delay</param>
        /// <param name="PowerDuringCessation">The optional power during cessation.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param></param>
        public VoltageParameters(Decimal?               HighVoltage_10Min_MeanValue       = null,
                                 TimeSpan?              HighVoltage_10Min_MeanTripDelay   = null,
                                 PowerDuringCessation?  PowerDuringCessation              = null,
                                 CustomData?            CustomData                        = null)

            : base(CustomData)

        {

            this.HighVoltage_10Min_MeanValue      = HighVoltage_10Min_MeanValue;
            this.HighVoltage_10Min_MeanTripDelay  = HighVoltage_10Min_MeanTripDelay;
            this.PowerDuringCessation             = PowerDuringCessation;

            unchecked
            {

                hashCode = (this.HighVoltage_10Min_MeanValue?.    GetHashCode() ?? 0) * 7 ^
                           (this.HighVoltage_10Min_MeanTripDelay?.GetHashCode() ?? 0) * 5 ^
                          (this.PowerDuringCessation?.          GetHashCode() ?? 0) * 3 ^
                           base.                                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "VoltageParams",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "hv10MinMeanValue": {
        //             "description": "EN 50549-1 chapter 4.9.3.4\r\n    Voltage threshold for the 10 min time window mean value monitoring.\r\n    The 10 min mean is recalculated up to every 3 s. \r\n    If the present voltage is above this threshold for more than the time defined by _hv10MinMeanValue_, the EV must trip.\r\n    This value is mandatory if _hv10MinMeanTripDelay_ is set.",
        //             "type": "number"
        //         },
        //         "hv10MinMeanTripDelay": {
        //             "description": "Time for which the voltage is allowed to stay above the 10 min mean value. \r\n    After this time, the EV must trip.\r\n    This value is mandatory if OverVoltageMeanValue10min is set.",
        //             "type": "number"
        //         },
        //         "powerDuringCessation": {
        //             "$ref": "#/definitions/PowerDuringCessationEnumType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomVoltageParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of reactivePowerParameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVoltageParametersParser">A delegate to parse custom reactivePowerParameters JSON objects.</param>
        public static VoltageParameters Parse(JObject                                                JSON,
                                                    CustomJObjectParserDelegate<VoltageParameters>?  CustomVoltageParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var reactivePowerParameters,
                         out var errorResponse,
                         CustomVoltageParametersParser))
            {
                return reactivePowerParameters;
            }

            throw new ArgumentException("The given JSON representation of VoltageParameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VoltageParameters, out ErrorResponse, CustomVoltageParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of reactivePowerParameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VoltageParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out VoltageParameters?  VoltageParameters,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out VoltageParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of reactivePowerParameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VoltageParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVoltageParametersParser">A delegate to parse custom reactivePowerParameters JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out VoltageParameters?      VoltageParameters,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<VoltageParameters>?  CustomVoltageParametersParser)
        {

            try
            {

                VoltageParameters = default;

                // Both optional in OCPP 2.1 - and the trip delay was read from
                // "hv10MinMeanValue", the mean value itself, as whole seconds.

                #region HighVoltage_10Min_MeanValue        [optional]

                if (JSON.ParseOptional("hv10MinMeanValue",
                                       "10-minute mean high voltage value",
                                       out Decimal? HighVoltage_10Min_MeanValue,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region HighVoltage_10Min_MeanTripDelay    [optional]

                if (JSON.ParseOptional("hv10MinMeanTripDelay",
                                       "10-minute mean high voltage trip delay",
                                       out Decimal? HighVoltage_10Min_MeanTripDelaySeconds,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var HighVoltage_10Min_MeanTripDelay = HighVoltage_10Min_MeanTripDelaySeconds.HasValue
                                                          ? TimeSpan.FromSeconds((Double) HighVoltage_10Min_MeanTripDelaySeconds.Value)
                                                          : (TimeSpan?) null;

                #endregion

                #region PowerDuringCessation               [optional]

                if (JSON.ParseOptional("powerDuringCessation",
                                       "power during cessation",
                                       OCPPv2_1.PowerDuringCessation.TryParse,
                                       out PowerDuringCessation? PowerDuringCessation,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                         [optional]

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


                VoltageParameters = new VoltageParameters(
                                        HighVoltage_10Min_MeanValue,
                                        HighVoltage_10Min_MeanTripDelay,
                                        PowerDuringCessation,
                                        CustomData
                                    );

                if (CustomVoltageParametersParser is not null)
                    VoltageParameters = CustomVoltageParametersParser(JSON,
                                                                      VoltageParameters);

                return true;

            }
            catch (Exception e)
            {
                VoltageParameters  = default;
                ErrorResponse      = "The given JSON representation of VoltageParameters is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomVoltageParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVoltageParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VoltageParameters>?  CustomVoltageParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           HighVoltage_10Min_MeanValue.HasValue
                               ? new JProperty("hv10MinMeanValue",       HighVoltage_10Min_MeanValue.Value)
                               : null,

                           // In seconds - the TimeSpan itself was written, as "00:00:30".
                           HighVoltage_10Min_MeanTripDelay.HasValue
                               ? new JProperty("hv10MinMeanTripDelay",   (Decimal) HighVoltage_10Min_MeanTripDelay.Value.TotalSeconds)
                               : null,

                           PowerDuringCessation.HasValue
                               ? new JProperty("powerDuringCessation",   PowerDuringCessation.Value.ToString())
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",             CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVoltageParametersSerializer is not null
                       ? CustomVoltageParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out VoltageParameters, out ErrorResponse, CustomVoltageParametersParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a voltage parameters.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="VoltageParameters">The voltage parameters.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out VoltageParameters?  VoltageParameters,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out VoltageParameters,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a voltage parameters.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="VoltageParameters">The voltage parameters.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVoltageParametersParser">An optional delegate to read custom voltage parameters.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out VoltageParameters?           VoltageParameters,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<VoltageParameters>?  CustomVoltageParametersParser)
        {

            try
            {

                VoltageParameters = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a voltage parameters is not a map!";
                    return false;
                }

                CBOR.ParseOptionalDecimal("hv10MinMeanValue",
                                          "high voltage 10 min mean value",
                                          out var HighVoltage_10Min_MeanValue,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("hv10MinMeanTripDelay",
                                        "high voltage 10 min mean trip delay",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? HighVoltage_10Min_MeanTripDelay,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                PowerDuringCessation? PowerDuringCessation = null;

                if (CBOR.ParseOptionalText("powerDuringCessation",
                                           "power during cessation",
                                           out var PowerDuringCessationText,
                                           out ErrorResponse))
                {

                    if (!OCPPv2_1.PowerDuringCessation.TryParse(PowerDuringCessationText!, out var PowerDuringCessationValue))
                    {
                        ErrorResponse = $"Invalid power during cessation '{PowerDuringCessationText}'!";
                        return false;
                    }

                    PowerDuringCessation = PowerDuringCessationValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                VoltageParameters = new VoltageParameters(
                                        HighVoltage_10Min_MeanValue,
                                        HighVoltage_10Min_MeanTripDelay,
                                        PowerDuringCessation,
                                        CustomData
                                    );

                if (CustomVoltageParametersParser is not null)
                    VoltageParameters = CustomVoltageParametersParser(CBOR,
                                                   VoltageParameters);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                VoltageParameters  = default;
                ErrorResponse  = "The given CBOR representation of a voltage parameters is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<VoltageParameters>.TryParse(CBOR, out VoltageParameters, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a voltage parameters - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<VoltageParameters>.TryParse(CBORValue                         CBOR,
                                                                   out VoltageParameters                  Value,
                                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomVoltageParametersSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this voltage parameters: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomVoltageParametersSerializer">A delegate to serialize custom voltage parameters.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<VoltageParameters>? CustomVoltageParametersSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("hv10MinMeanValue",                       OCPPCBORExtensions.Number(HighVoltage_10Min_MeanValue)),
                           ("hv10MinMeanTripDelay",                   HighVoltage_10Min_MeanTripDelay?.ToCBOR()),
                           ("powerDuringCessation",                   OCPPCBORExtensions.Text(PowerDuringCessation?.ToString())),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomVoltageParametersSerializer is not null
                       ? CustomVoltageParametersSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (VoltageParameters1, VoltageParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltageParameters1">reactivePowerParameters.</param>
        /// <param name="VoltageParameters2">Another reactivePowerParameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VoltageParameters? VoltageParameters1,
                                           VoltageParameters? VoltageParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VoltageParameters1, VoltageParameters2))
                return true;

            // If one is null, but not both, return false.
            if (VoltageParameters1 is null || VoltageParameters2 is null)
                return false;

            return VoltageParameters1.Equals(VoltageParameters2);

        }

        #endregion

        #region Operator != (VoltageParameters1, VoltageParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VoltageParameters1">reactivePowerParameters.</param>
        /// <param name="VoltageParameters2">Another reactivePowerParameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VoltageParameters? VoltageParameters1,
                                           VoltageParameters? VoltageParameters2)

            => !(VoltageParameters1 == VoltageParameters2);

        #endregion

        #endregion

        #region IEquatable<VoltageParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reactivePowerParameters for equality..
        /// </summary>
        /// <param name="Object">reactivePowerParameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VoltageParameters reactivePowerParameters &&
                   Equals(reactivePowerParameters);

        #endregion

        #region Equals(VoltageParameters)

        /// <summary>
        /// Compares two reactivePowerParameters for equality.
        /// </summary>
        /// <param name="VoltageParameters">reactivePowerParameters to compare with.</param>
        public Boolean Equals(VoltageParameters? VoltageParameters)

            => VoltageParameters is not null &&

               Nullable.Equals(HighVoltage_10Min_MeanValue,     VoltageParameters.HighVoltage_10Min_MeanValue)     &&
               Nullable.Equals(HighVoltage_10Min_MeanTripDelay, VoltageParameters.HighVoltage_10Min_MeanTripDelay) &&

             ((!PowerDuringCessation.HasValue && !VoltageParameters.PowerDuringCessation.HasValue) ||
                PowerDuringCessation.HasValue &&  VoltageParameters.PowerDuringCessation.HasValue && PowerDuringCessation.Value.Equals(VoltageParameters.PowerDuringCessation.Value)) &&

               base.Equals(VoltageParameters);

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

                   $"HighVoltage_10Min_MeanValue: {HighVoltage_10Min_MeanValue?.ToString() ?? "-"}, {(HighVoltage_10Min_MeanTripDelay.HasValue ? Math.Round(HighVoltage_10Min_MeanTripDelay.Value.TotalSeconds, 2).ToString() : "-")} seconds",

                   PowerDuringCessation.HasValue
                       ? $", power during cessation: {PowerDuringCessation}"
                       : ""

               );

        #endregion

    }

}
