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
        public String   Type                           { get; }

        /// <summary>
        /// The tax percentage.
        /// </summary>
        [Mandatory]
        public Decimal  Tax                            { get; }

        /// <summary>
        /// Whether the tax applies to the energy fee.
        /// </summary>
        [Mandatory]
        public Boolean  AppliesToEnergyFee             { get; }

        /// <summary>
        /// Whether the tax applies to the parking fee.
        /// </summary>
        [Mandatory]
        public Boolean  AppliesToParkingFee            { get; }

        /// <summary>
        /// Whether the tax applies to the overstay fee.
        /// </summary>
        [Mandatory]
        public Boolean  AppliesToOverstayFee           { get; }

        /// <summary>
        /// Whether the tax applies to minimum/maximum cost.
        /// </summary>
        [Mandatory]
        public Boolean  AppliesToMinimumMaximumCost    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tax rate.
        /// </summary>
        /// <param name="Type">The type of this tax, e.g. "VAT", "State", "Federal".</param>
        /// <param name="Tax">The tax percentage.</param>
        /// <param name="AppliesToEnergyFee">Whether the tax applies to the energy fee.</param>
        /// <param name="AppliesToParkingFee">Whether the tax applies to the parking fee.</param>
        /// <param name="AppliesToOverstayFee">Whether the tax applies to the overstay fee.</param>
        /// <param name="AppliesToMinimumMaximumCost">Whether the tax applies to minimum/maximum cost.</param>
        public TaxRate(String  Type,
                       Decimal Tax,
                       Boolean AppliesToEnergyFee            = false,
                       Boolean AppliesToParkingFee           = false,
                       Boolean AppliesToOverstayFee          = false,
                       Boolean AppliesToMinimumMaximumCost   = false)
        {

            this.Type                         = Type;
            this.Tax                          = Tax;
            this.AppliesToEnergyFee           = AppliesToEnergyFee;
            this.AppliesToParkingFee          = AppliesToParkingFee;
            this.AppliesToOverstayFee         = AppliesToOverstayFee;
            this.AppliesToMinimumMaximumCost  = AppliesToMinimumMaximumCost;

            unchecked
            {

                hashCode = this.Type.                       GetHashCode() * 17 ^
                           this.Tax.                        GetHashCode() * 13 ^
                           this.AppliesToEnergyFee.         GetHashCode() * 11 ^
                           this.AppliesToParkingFee.        GetHashCode() *  7 ^
                           this.AppliesToOverstayFee.       GetHashCode() *  5 ^
                           this.AppliesToMinimumMaximumCost.GetHashCode() *  3 ^
                           base.                            GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

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
        public static Boolean TryParse(JObject      JSON,
                                       out TaxRate  TaxRate,
                                       out String?  ErrorResponse)

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
                                       out TaxRate                            TaxRate,
                                       out String?                            ErrorResponse,
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

                #region Parse Type                     [mandatory]

                if (!JSON.ParseMandatoryText("type",
                                             "tax type",
                                             out String Type,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Tax                      [mandatory]

                if (!JSON.ParseMandatory("tax",
                                         "tax percentage",
                                         out Decimal Tax,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AppliesToEnergyFee             [mandatory]

                if (!JSON.ParseMandatory("appliesToEnergyFee",
                                         "applies to energy fee",
                                         out Boolean AppliesToEnergyFee,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AppliesToParkingFee            [mandatory]

                if (!JSON.ParseMandatory("appliesToParkingFee",
                                         "applies to parking fee",
                                         out Boolean AppliesToParkingFee,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AppliesToOverstayFee           [mandatory]

                if (!JSON.ParseMandatory("appliesToOverstayFee",
                                         "applies to overstay fee",
                                         out Boolean AppliesToOverstayFee,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AppliesToMinimumMaximumCost    [mandatory]

                if (!JSON.ParseMandatory("appliesToMinimumMaximumCost",
                                         "applies to minimum/maximum cost",
                                         out Boolean AppliesToMinimumMaximumCost,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                TaxRate = new TaxRate(
                              Type,
                              Tax,
                              AppliesToEnergyFee,
                              AppliesToParkingFee,
                              AppliesToOverstayFee,
                              AppliesToMinimumMaximumCost
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
                           new JProperty("type",                          Type),
                           new JProperty("tax",                           Tax),
                           new JProperty("appliesToEnergyFee",            AppliesToEnergyFee),
                           new JProperty("appliesToParkingFee",           AppliesToParkingFee),
                           new JProperty("appliesToOverstayFee",          AppliesToOverstayFee),
                           new JProperty("appliesToMinimumMaximumCost",   AppliesToMinimumMaximumCost)
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
                   new String(Type.ToCharArray()),
                   Tax,
                   AppliesToEnergyFee,
                   AppliesToParkingFee,
                   AppliesToOverstayFee,
                   AppliesToMinimumMaximumCost
               );

        #endregion


        #region Static definitions

        #region (static) VAT(Percentage)

        /// <summary>
        /// Valued Added Tax.
        /// </summary>
        /// <param name="Percentage">The tax percentage.</param>
        public static TaxRate VAT(Decimal Percentage)

            => new (
                   Type:                         "VAT",
                   Tax:                          Percentage,
                   AppliesToEnergyFee:           true,
                   AppliesToParkingFee:          true,
                   AppliesToOverstayFee:         true,
                   AppliesToMinimumMaximumCost:  true
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

            var c = String.                     Compare  (Type,
                                                          TaxRate.Type,
                                                          StringComparison.OrdinalIgnoreCase);

            if (c == 0)
                c = Tax.                        CompareTo(TaxRate.Tax);

            if (c == 0)
                c = AppliesToEnergyFee.         CompareTo(TaxRate.AppliesToEnergyFee);

            if (c == 0)
                c = AppliesToParkingFee.        CompareTo(TaxRate.AppliesToParkingFee);

            if (c == 0)
                c = AppliesToOverstayFee.       CompareTo(TaxRate.AppliesToOverstayFee);

            if (c == 0)
                c = AppliesToMinimumMaximumCost.CompareTo(TaxRate.AppliesToMinimumMaximumCost);

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

            => Type.                       Equals(TaxRate.Type, StringComparison.OrdinalIgnoreCase) &&
               Tax.                        Equals(TaxRate.Tax)                                      &&
               AppliesToEnergyFee.         Equals(TaxRate.AppliesToEnergyFee)                       &&
               AppliesToParkingFee.        Equals(TaxRate.AppliesToParkingFee)                      &&
               AppliesToOverstayFee.       Equals(TaxRate.AppliesToOverstayFee)                     &&
               AppliesToMinimumMaximumCost.Equals(TaxRate.AppliesToMinimumMaximumCost);

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

                   $"{Tax}% {Type}",

                   AppliesToEnergyFee
                       ? ", applies to energy fee"
                       : "",

                   AppliesToParkingFee
                       ? ", applies to parking fee"
                       : "",

                   AppliesToOverstayFee
                       ? ", applies to overstay fee"
                       : "",

                   AppliesToMinimumMaximumCost
                       ? ", applies to minimum/maximum cost"
                       : ""

               );

        #endregion

    }

}
