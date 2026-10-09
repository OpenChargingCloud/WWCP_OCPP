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
    /// Hysteresis
    /// </summary>
    public class Hysteresis : ACustomData,
                              ICBORSerializable<Hysteresis>,
                              IEquatable<Hysteresis>
    {

        #region Properties

        /// <summary>
        /// The optional high value for return to normal operation after a grid
        /// event, in absolute value. This value adopts the same unit as defined
        /// by yUnit.
        /// </summary>
        [Optional]
        public Decimal?   High        { get; }

        /// <summary>
        /// The optional low value for return to normal operation after a grid
        /// event, in absolute value. This value adopts the same unit as defined
        /// by yUnit.
        /// </summary>
        [Optional]
        public Decimal?   Low         { get; }

        /// <summary>
        /// The optional delay, once grid parameter within Low and High, for the
        /// EV to return to normal operation after a grid event.
        /// </summary>
        [Optional]
        public TimeSpan?  Delay       { get; }

        /// <summary>
        /// The optional default rate of change (ramp rate %/s) for the EV to
        /// return to normal operation after a grid event.
        /// </summary>
        [Optional]
        public Decimal?   Gradient    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new hysteresis.
        /// </summary>
        /// <param name="High">The optional high value for return to normal operation after a grid event, in absolute value.</param>
        /// <param name="Low">The optional low value for return to normal operation after a grid event, in absolute value.</param>
        /// <param name="Delay">The optional delay, once grid parameter within Low and High, for the EV to return to normal operation after a grid event.</param>
        /// <param name="Gradient">The optional default rate of change (ramp rate %/s) for the EV to return to normal operation after a grid event.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Hysteresis(Decimal?     High         = null,
                          Decimal?     Low          = null,
                          TimeSpan?    Delay        = null,
                          Decimal?     Gradient     = null,
                          CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.High      = High;
            this.Low       = Low;
            this.Delay     = Delay;
            this.Gradient  = Gradient;

            unchecked
            {

                hashCode = (this.High?.    GetHashCode() ?? 0) * 11 ^
                           (this.Low?.     GetHashCode() ?? 0) *  7 ^
                           (this.Delay?.   GetHashCode() ?? 0) *  5 ^
                           (this.Gradient?.GetHashCode() ?? 0) *  3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        //Note: This does not look correct!

        // {
        //     "javaType": "Hysteresis",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "hysteresisHigh": {
        //             "description": "High value for return to normal operation after a grid event, in absolute value. This value adopts the same unit as defined by yUnit",
        //             "type": "number"
        //         },
        //         "hysteresisLow": {
        //             "description": "Low value for return to normal operation after a grid event, in absolute value. This value adopts the same unit as defined by yUnit",
        //             "type": "number"
        //         },
        //         "hysteresisDelay": {
        //             "description": "Delay in seconds, once grid parameter within HysteresisLow and HysteresisHigh, for the EV to return to normal operation after a grid event.",
        //             "type": "number"
        //         },
        //         "hysteresisGradient": {
        //             "description": "Set default rate of change (ramp rate %/s) for the EV to return to normal operation after a grid event",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomHysteresisParser = null)

        /// <summary>
        /// Parse the given JSON representation of hysteresis.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomHysteresisParser">A delegate to parse custom hysteresis JSON objects.</param>
        public static Hysteresis Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<Hysteresis>?  CustomHysteresisParser   = null)
        {

            if (TryParse(JSON,
                         out var hysteresis,
                         out var errorResponse,
                         CustomHysteresisParser))
            {
                return hysteresis;
            }

            throw new ArgumentException("The given JSON representation of hysteresis is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Hysteresis, out ErrorResponse, CustomHysteresisParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of hysteresis.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Hysteresis">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out Hysteresis?  Hysteresis,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

            => TryParse(JSON,
                        out Hysteresis,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of hysteresis.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Hysteresis">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHysteresisParser">A delegate to parse custom hysteresis JSON objects.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out Hysteresis?      Hysteresis,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<Hysteresis>?  CustomHysteresisParser)
        {

            try
            {

                Hysteresis = default;

                // The schema's names, and every one of them optional: these
                // were read as "high", "low", "delay" and "gradient", and
                // required.

                #region High          [optional]

                if (JSON.ParseOptional("hysteresisHigh",
                                       "hysteresis high",
                                       out Decimal? High,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Low           [optional]

                if (JSON.ParseOptional("hysteresisLow",
                                       "hysteresis low",
                                       out Decimal? Low,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Delay         [optional]

                // A number of seconds, and not only a whole one.
                if (JSON.ParseOptional("hysteresisDelay",
                                       "hysteresis delay",
                                       out Decimal? DelaySeconds,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var Delay = DelaySeconds.HasValue
                                ? TimeSpan.FromSeconds((Double) DelaySeconds.Value)
                                : (TimeSpan?) null;

                #endregion

                #region Gradient      [optional]

                if (JSON.ParseOptional("hysteresisGradient",
                                       "hysteresis gradient",
                                       out Decimal? Gradient,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

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


                Hysteresis = new Hysteresis(
                                 High,
                                 Low,
                                 Delay,
                                 Gradient,
                                 CustomData
                             );

                if (CustomHysteresisParser is not null)
                    Hysteresis = CustomHysteresisParser(JSON,
                                                        Hysteresis);

                return true;

            }
            catch (Exception e)
            {
                Hysteresis     = default;
                ErrorResponse  = "The given JSON representation of hysteresis is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomHysteresisSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHysteresisSerializer">A delegate to serialize custom hysteresis.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Hysteresis>?  CustomHysteresisSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                           High.HasValue
                               ? new JProperty("hysteresisHigh",       High.    Value)
                               : null,

                           Low.HasValue
                               ? new JProperty("hysteresisLow",        Low.     Value)
                               : null,

                           Delay.HasValue
                               ? new JProperty("hysteresisDelay",      (Decimal) Delay.Value.TotalSeconds)
                               : null,

                           Gradient.HasValue
                               ? new JProperty("hysteresisGradient",   Gradient.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomHysteresisSerializer is not null
                       ? CustomHysteresisSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out Hysteresis, out ErrorResponse, CustomHysteresisParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a hysteresis.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Hysteresis">The hysteresis.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out Hysteresis?  Hysteresis,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out Hysteresis,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a hysteresis.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Hysteresis">The hysteresis.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHysteresisParser">An optional delegate to read custom hysteresis.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out Hysteresis?           Hysteresis,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<Hysteresis>?  CustomHysteresisParser)
        {

            try
            {

                Hysteresis = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a hysteresis is not a map!";
                    return false;
                }

                CBOR.ParseOptionalDecimal("hysteresisHigh",
                                          "hysteresis high",
                                          out var High,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalDecimal("hysteresisLow",
                                          "hysteresis low",
                                          out var Low,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("hysteresisDelay",
                                        "hysteresis delay",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? Delay,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                // A ramp rate in % per second.
                CBOR.ParseOptionalValue("hysteresisGradient",
                                        "hysteresis gradient",
                                        (CBORValue value, out Decimal gradient, out String? errorResponse) =>
                                            OCPPCBORExtensions.TryParseQuantity(value, OCPPCBORExtensions.PercentPerSecond, out gradient, out errorResponse),
                                        out Decimal? Gradient,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                Hysteresis = new Hysteresis(
                                 High,
                                 Low,
                                 Delay,
                                 Gradient,
                                 CustomData
                             );

                if (CustomHysteresisParser is not null)
                    Hysteresis = CustomHysteresisParser(CBOR,
                                            Hysteresis);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Hysteresis  = default;
                ErrorResponse  = "The given CBOR representation of a hysteresis is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<Hysteresis>.TryParse(CBOR, out Hysteresis, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a hysteresis - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<Hysteresis>.TryParse(CBORValue                         CBOR,
                                                            out Hysteresis                  Value,
                                                            [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomHysteresisSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this hysteresis: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomHysteresisSerializer">A delegate to serialize custom hysteresis.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<Hysteresis>? CustomHysteresisSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("hysteresisHigh",                         OCPPCBORExtensions.Number(High)),
                           ("hysteresisLow",                          OCPPCBORExtensions.Number(Low)),
                           ("hysteresisDelay",                        Delay?.ToCBOR()),
                           ("hysteresisGradient",                     Gradient.HasValue ? (CBORValue?) OCPPCBORExtensions.QuantityToCBOR(Gradient.Value, OCPPCBORExtensions.PercentPerSecond) : null),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomHysteresisSerializer is not null
                       ? CustomHysteresisSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (Hysteresis1, Hysteresis2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hysteresis1">hysteresis.</param>
        /// <param name="Hysteresis2">Another hysteresis.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Hysteresis? Hysteresis1,
                                           Hysteresis? Hysteresis2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Hysteresis1, Hysteresis2))
                return true;

            // If one is null, but not both, return false.
            if (Hysteresis1 is null || Hysteresis2 is null)
                return false;

            return Hysteresis1.Equals(Hysteresis2);

        }

        #endregion

        #region Operator != (Hysteresis1, Hysteresis2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Hysteresis1">hysteresis.</param>
        /// <param name="Hysteresis2">Another hysteresis.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Hysteresis? Hysteresis1,
                                           Hysteresis? Hysteresis2)

            => !(Hysteresis1 == Hysteresis2);

        #endregion

        #endregion

        #region IEquatable<Hysteresis> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two hysteresis for equality..
        /// </summary>
        /// <param name="Object">hysteresis to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Hysteresis hysteresis &&
                   Equals(hysteresis);

        #endregion

        #region Equals(Hysteresis)

        /// <summary>
        /// Compares two hysteresis for equality.
        /// </summary>
        /// <param name="Hysteresis">hysteresis to compare with.</param>
        public Boolean Equals(Hysteresis? Hysteresis)

            => Hysteresis is not null &&

               Nullable.Equals(High,      Hysteresis.High)     &&
               Nullable.Equals(Low,       Hysteresis.Low)      &&
               Nullable.Equals(Delay,     Hysteresis.Delay)    &&
               Nullable.Equals(Gradient,  Hysteresis.Gradient) &&

               base.Equals(Hysteresis);

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

            => $"{High?.ToString() ?? "-"} <--( {Delay?.TotalSeconds.ToString() ?? "-"} sec. / {Gradient?.ToString() ?? "-"} )--> {Low?.ToString() ?? "-"}";

        #endregion

    }

}
