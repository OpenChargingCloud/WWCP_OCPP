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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A rational number as used within ISO 15118.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <param name="Exponent">The exponent.</param>
    public class RationalNumber(Int32 Value,
                                Int32 Exponent) : IEquatable<RationalNumber>
    {

        #region Properties

        /// <summary>
        /// The value.
        /// </summary>
        [Mandatory]
        public Int32  Value       { get; } = Value;

        /// <summary>
        /// The exponent.
        /// </summary>
        [Optional]
        public Int32  Exponent    { get; } = Exponent;

        #endregion


        #region Documentation

        // {
        //     "description": "Part of ISO 15118-20 price schedule.\r\n\r\n",
        //     "javaType": "RationalNumber",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "exponent": {
        //             "description": "The exponent to base 10 (dec)\r\n",
        //             "type": "integer"
        //         },
        //         "value": {
        //             "description": "Value which shall be multiplied.\r\n",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "exponent",
        //         "value"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON)

        /// <summary>
        /// Parse the given JSON representation of a rational number.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRationalNumberParser">A delegate to parse custom rational number.</param>
        public static RationalNumber Parse(JObject JSON)
        {

            if (TryParse(JSON,
                         out var rationalNumber,
                         out var errorResponse))
            {
                return rationalNumber;
            }

            throw new ArgumentException("The given JSON representation of a rational number is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out RationalNumber, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a rational number.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RationalNumber">The parsed rational number.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out RationalNumber?      RationalNumber,
                                       [NotNullWhen(false)] out String?              ErrorResponse)
        {

            try
            {

                RationalNumber = default;

                #region Value       [mandatory]

                if (!JSON.ParseMandatory("value",
                                         "value",
                                         out Int32 Value,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Exponent    [mandatory]

                if (!JSON.ParseMandatory("exponent",
                                         "exponent",
                                         out Int32 Exponent,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                RationalNumber = new RationalNumber(
                                     Value,
                                     Exponent
                                 );

                return true;

            }
            catch (Exception e)
            {
                RationalNumber  = default;
                ErrorResponse   = "The given JSON representation of a rational number is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => new (
                   new JProperty("value",     Value),
                   new JProperty("exponent",  Exponent)
               );

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this rational number and a multiplier.
        /// </summary>
        public RationalNumber Clone()

            => new (
                   Value,
                   Exponent
               );

        #endregion


        #region ToDecimal()

        /// <summary>
        /// Better than .NET's naive approach, as it avoids internal Doubles!
        ///   return Value * (Decimal) Math.Pow(10, Exponent);
        /// </summary>
        public Decimal ToDecimal()
        {

            var factor = 1m;

            if (Exponent > 0)
                for (var i = 0; i < Exponent; i++)
                    factor *= 10m;

            else
                for (var i = 0; i > Exponent; i--)
                    factor /= 10m;

            return Value * factor;

        }

        #endregion

        #region FromDecimal(Value)

        /// <summary>
        /// A decimal in C# occupies 4x 32-bit Integers (16 bytes):
        ///   - The 1st-3rd Integers store the significant digit value (mantissa, 96 bits)
        ///   - The 4th Integer contains both the sign and the scaling factor (exponent)
        /// </summary>
        /// <param name="Value">Der umzuwandelnde decimal-Wert.</param>
        public static RationalNumber FromDecimal(Decimal Value)
        {

            var integers   = Decimal.GetBits(Value);

            var lo         =  integers[0];
            var mid        =  integers[1];
            var hi         =  integers[2];
            var scale      = (integers[3] >> 16) & 0xFF;

            //if (mid != 0 || hi != 0)
            //    throw new OverflowException("The internal value of the Decimal is too large for an Int32!");

            var isNegative = (integers[3] & 0x80000000) != 0;
            var intValue   = lo;

            if (isNegative)
                intValue   = -intValue;

            // The scale indicates how many decimal places the value has: decimal = intValue * 10^(-scale)
            // Our model expects:  Wert     = Value * 10^(Exponent)
            // So we set:          Exponent = -scale.
            var exponent   = -scale;

            return new RationalNumber(
                       intValue,
                       exponent
                   );

        }


        #endregion


        #region Operator overloading

        #region Operator == (RationalNumber1, RationalNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RationalNumber1">A rational number.</param>
        /// <param name="RationalNumber2">Another rational number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RationalNumber? RationalNumber1,
                                           RationalNumber? RationalNumber2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RationalNumber1, RationalNumber2))
                return true;

            // If one is null, but not both, return false.
            if (RationalNumber1 is null || RationalNumber2 is null)
                return false;

            return RationalNumber1.Equals(RationalNumber2);

        }

        #endregion

        #region Operator != (RationalNumber1, RationalNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RationalNumber1">A rational number.</param>
        /// <param name="RationalNumber2">Another rational number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RationalNumber? RationalNumber1,
                                           RationalNumber? RationalNumber2)

            => !(RationalNumber1 == RationalNumber2);

        #endregion

        #endregion

        #region IEquatable<RationalNumber> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two rational numbers for equality.
        /// </summary>
        /// <param name="Object">A rational number to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RationalNumber rationalNumber &&
                   Equals(rationalNumber);

        #endregion

        #region Equals(RationalNumber)

        /// <summary>
        /// Compares two rational numbers for equality.
        /// </summary>
        /// <param name="RationalNumber">A rational number to compare with.</param>
        public Boolean Equals(RationalNumber? RationalNumber)

            => RationalNumber is not null &&

               Value.   Equals(RationalNumber.Value) &&
               Exponent.Equals(RationalNumber.Exponent);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Value.   GetHashCode() * 3 ^
                       Exponent.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Value} * 10^{Exponent}";

        #endregion

    }

}
