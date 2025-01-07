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
    /// A tax rate.
    /// </summary>
    public readonly struct TaxRate : IEquatable<TaxRate>,
                                     IComparable<TaxRate>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// Type of this tax, e.g. "VAT", "State", "Federal".
        /// </summary>
        [Mandatory]
        public TaxType     Type    { get; }

        /// <summary>
        /// The tax percentage.
        /// </summary>
        [Mandatory]
        public Percentage  Tax     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tax rate.
        /// </summary>
        /// <param name="Type">The type of this tax, e.g. "VAT", "State", "Federal".</param>
        /// <param name="Tax">The tax percentage.</param>
        public TaxRate(TaxType     Type,
                       Percentage  Tax)
        {

            this.Type  = Type;
            this.Tax   = Tax;

            unchecked
            {

                hashCode = this.Type.GetHashCode() * 5 ^
                           this.Tax. GetHashCode() * 3 ^
                           base.     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description":           "Tax percentage",
        //     "javaType":              "TaxRate",
        //     "type":                  "object",
        //     "additionalProperties":   false,
        //     "properties": {
        //         "type": {
        //             "description":  "Type of this tax, e.g. "Federal", "State", for information on receipt.",
        //             "type":         "string",
        //             "maxLength":     20
        //         },
        //         "tax": {
        //             "description":  "Tax percentage",
        //             "type":         "number"
        //         }
        //     },
        //     "required": [
        //         "type",
        //         "tax"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTaxRateParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tax rate.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTaxRateParser">An optional delegate to parse custom tax rate JSON objects.</param>
        public static TaxRate Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<TaxRate>?  CustomTaxRateParser   = null)
        {

            if (TryParse(JSON,
                         out var taxRate,
                         out var errorResponse,
                         CustomTaxRateParser))
            {
                return taxRate;
            }

            throw new ArgumentException("The given JSON representation of a tax rate is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TaxRate, out ErrorResponse, CustomTaxRateParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tax rate.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TaxRate">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out TaxRate  TaxRate,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out TaxRate,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tax rate.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TaxRate">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTaxRateParser">An optional delegate to parse custom tax rate JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out TaxRate       TaxRate,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<TaxRate>?  CustomTaxRateParser)
        {

            try
            {

                TaxRate = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Type    [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "tax type",
                                         TaxType.TryParse,
                                         out TaxType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Tax     [mandatory]

                if (!JSON.ParseMandatory("tax",
                                         "tax percentage",
                                         Percentage.TryParse,
                                         out Percentage Tax,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                TaxRate = new TaxRate(
                              Type,
                              Tax
                          );


                if (CustomTaxRateParser is not null)
                    TaxRate = CustomTaxRateParser(JSON,
                                                  TaxRate);

                return true;

            }
            catch (Exception e)
            {
                TaxRate        = default;
                ErrorResponse  = "The given JSON representation of a tax rate is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTaxRateSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rate JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TaxRate>? CustomTaxRateSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("type",  Type.ToString()),
                           new JProperty("tax",   Tax. Value)
                       );

            return CustomTaxRateSerializer is not null
                       ? CustomTaxRateSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TaxRate Clone()

            => new (
                   Type.Clone(),
                   Tax. Clone()
               );

        #endregion


        #region Static definitions

        #region (static) VAT     (Percentage)

        /// <summary>
        /// Valued Added Tax.
        /// </summary>
        /// <param name="Percentage">The tax percentage.</param>
        public static TaxRate VAT(Percentage Percentage)

            => new (
                   Type:  TaxType.VAT,
                   Tax:   Percentage
               );


        /// <summary>
        /// Valued Added Tax.
        /// </summary>
        /// <param name="Percentage">The tax percentage.</param>
        public static TaxRate VAT(Decimal Percentage)

            => new (
                   Type:  TaxType.VAT,
                   Tax:   org.GraphDefined.Vanaheimr.Illias.Percentage.Parse(Percentage)
               );

        #endregion

        #region (static) State   (Percentage)

        /// <summary>
        /// A state tax.
        /// </summary>
        /// <param name="Percentage">The tax percentage.</param>
        public static TaxRate State(Percentage Percentage)

            => new (
                   Type:  TaxType.State,
                   Tax:   Percentage
               );

        #endregion

        #region (static) Federal (Percentage)

        /// <summary>
        /// A federal tax.
        /// </summary>
        /// <param name="Percentage">The tax percentage.</param>
        public static TaxRate Federal(Percentage Percentage)

            => new (
                   Type:  TaxType.Federal,
                   Tax:   Percentage
               );

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (TaxRate1, TaxRate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRate1">A tax rate.</param>
        /// <param name="TaxRate2">Another tax rate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TaxRate TaxRate1,
                                           TaxRate TaxRate2)

            => TaxRate1.Equals(TaxRate2);

        #endregion

        #region Operator != (TaxRate1, TaxRate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRate1">A tax rate.</param>
        /// <param name="TaxRate2">Another tax rate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TaxRate TaxRate1,
                                           TaxRate TaxRate2)

            => !(TaxRate1 == TaxRate2);

        #endregion

        #region Operator <  (TaxRate1, TaxRate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRate1">A tax rate.</param>
        /// <param name="TaxRate2">Another tax rate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TaxRate TaxRate1,
                                          TaxRate TaxRate2)

            => TaxRate1.CompareTo(TaxRate2) < 0;

        #endregion

        #region Operator <= (TaxRate1, TaxRate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRate1">A tax rate.</param>
        /// <param name="TaxRate2">Another tax rate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TaxRate TaxRate1,
                                           TaxRate TaxRate2)

            => !(TaxRate1 > TaxRate2);

        #endregion

        #region Operator >  (TaxRate1, TaxRate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRate1">A tax rate.</param>
        /// <param name="TaxRate2">Another tax rate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TaxRate TaxRate1,
                                          TaxRate TaxRate2)

            => TaxRate1.CompareTo(TaxRate2) > 0;

        #endregion

        #region Operator >= (TaxRate1, TaxRate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRate1">A tax rate.</param>
        /// <param name="TaxRate2">Another tax rate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TaxRate TaxRate1,
                                           TaxRate TaxRate2)

            => !(TaxRate1 < TaxRate2);

        #endregion

        #endregion

        #region IComparable<TaxRate> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tax rates.
        /// </summary>
        /// <param name="Object">A tax rate to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TaxRate taxRate
                   ? CompareTo(taxRate)
                   : throw new ArgumentException("The given object is not a tax rate!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TaxRate)

        /// <summary>
        /// Compares two tax rates.
        /// </summary>
        /// <param name="TaxRate">A tax rate to compare with.</param>
        public Int32 CompareTo(TaxRate TaxRate)
        {

            var c = Type.CompareTo(TaxRate.Type);

            if (c == 0)
                c = Tax. CompareTo(TaxRate.Tax);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TaxRate> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tax rates for equality.
        /// </summary>
        /// <param name="Object">A tax rate to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TaxRate taxRate &&
                   Equals(taxRate);

        #endregion

        #region Equals(TaxRate)

        /// <summary>
        /// Compares two tax rates for equality.
        /// </summary>
        /// <param name="TaxRate">A tax rate to compare with.</param>
        public Boolean Equals(TaxRate TaxRate)

            => Type.Equals(TaxRate.Type) &&
               Tax. Equals(TaxRate.Tax);

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

            => $"{Tax}% {Type}";

        #endregion

    }

}
