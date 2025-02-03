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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Hysteresis
    /// </summary>
    public class Hysteresis : ACustomData,
                              IEquatable<Hysteresis>
    {

        #region Properties

        /// <summary>
        /// High value for return to normal operation after a grid event, in absolute value.
        /// This value adopts the same unit as defined by yUnit
        /// </summary>
        [Mandatory]
        public Decimal   High        { get; }

        /// <summary>
        /// Low value for return to normal operation after a grid event, in absolute value.
        /// This value adopts the same unit as defined by yUnit.
        /// </summary>
        [Mandatory]
        public Decimal   Low         { get; }

        /// <summary>
        /// Delay in seconds, once grid parameter within Low and High,
        /// for the EV to return to normal operation after a grid event.
        /// </summary>
        [Mandatory]
        public TimeSpan  Delay       { get; }

        /// <summary>
        /// The data value of the Y-axis (dependent) variable, depending on the DER unit of the curve.
        /// If _y_ is power factor, then a positive value means DER is absorbing reactive power (under-excited),
        /// a negative value when DER is injecting reactive power (over-excited).
        /// </summary>
        [Mandatory]
        public Decimal   Gradient    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new hysteresis.
        /// </summary>
        /// <param name="High">High value for return to normal operation after a grid event, in absolute value.</param>
        /// <param name="Low">Low value for return to normal operation after a grid event, in absolute value.</param>
        /// <param name="Delay">Delay in seconds, once grid parameter within Low and High, for the EV to return to normal operation after a grid event.</param>
        /// <param name="Gradient">The data value of the Y-axis (dependent) variable, depending on the DER unit of the curve.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param></param>
        public Hysteresis(Decimal      High,
                          Decimal      Low,
                          TimeSpan     Delay,
                          Decimal      Gradient,
                          CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.High      = High;
            this.Low       = Low;
            this.Delay     = Delay;
            this.Gradient  = Gradient;

            unchecked
            {

                hashCode = this.High.    GetHashCode() * 11 ^
                           this.Low.     GetHashCode() *  7 ^
                           this.Delay.   GetHashCode() *  5 ^
                           this.Gradient.GetHashCode() *  3 ^
                           base.         GetHashCode();

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

                #region High          [mandatory]

                if (!JSON.ParseMandatory("high",
                                         "high",
                                         out Decimal High,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Low           [mandatory]

                if (!JSON.ParseMandatory("low",
                                         "low",
                                         out Decimal Low,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Delay         [mandatory]

                if (!JSON.ParseMandatory("delay",
                                         "delay",
                                         out TimeSpan Delay,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Gradient      [mandatory]

                if (!JSON.ParseMandatory("gradient",
                                         "gradient",
                                         out Decimal Gradient,
                                         out ErrorResponse))
                {
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

                                 new JProperty("high",         High),
                                 new JProperty("low",          Low),
                                 new JProperty("delay",        Delay.TotalSeconds),
                                 new JProperty("gradient",     Gradient),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomHysteresisSerializer is not null
                       ? CustomHysteresisSerializer(this, json)
                       : json;

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

               High.    Equals(Hysteresis.High)     &&
               Low.     Equals(Hysteresis.Low)      &&
               Delay.   Equals(Hysteresis.Delay)    &&
               Gradient.Equals(Hysteresis.Gradient) &&

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

            => $"{High} <--( {Delay.TotalSeconds} sec. / { Gradient } )--> {Low}";

        #endregion

    }

}
