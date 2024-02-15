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
    /// An enumeration of tax rates.
    /// </summary>
    public class TaxRates : IEnumerable<TaxRate>,
                            IEquatable<TaxRates>,
                            IComparable<TaxRates>,
                            IComparable
    {

        #region Data

        private readonly HashSet<TaxRate> taxRates = new();

        #endregion

        #region Propertis

        #region Count

        /// <summary>
        /// The number of tax rates.
        /// </summary>
        public UInt32 Count

            => (UInt32) taxRates.Count;

        #endregion

        #endregion

        #region Constructor(s)

        #region TaxRates(Type, Tax, ...)

        /// <summary>
        /// Create a new tax rates.
        /// </summary>
        /// <param name="Type">The type of this tax, e.g. "VAT", "State", "Federal".</param>
        /// <param name="Tax">The tax percentage.</param>
        /// <param name="AppliesToEnergyFee">Whether the tax applies to the energy fee.</param>
        /// <param name="AppliesToParkingFee">Whether the tax applies to the parking fee.</param>
        /// <param name="AppliesToOverstayFee">Whether the tax applies to the overstay fee.</param>
        /// <param name="AppliesToMinimumMaximumCost">Whether the tax applies to minimum/maximum cost.</param>
        public TaxRates(String  Type,
                        Decimal Tax,
                        Boolean AppliesToEnergyFee            = false,
                        Boolean AppliesToParkingFee           = false,
                        Boolean AppliesToOverstayFee          = false,
                        Boolean AppliesToMinimumMaximumCost   = false)
        {

            taxRates.Add(
                new TaxRate(
                    Type,
                    Tax,
                    AppliesToEnergyFee,
                    AppliesToParkingFee,
                    AppliesToOverstayFee,
                    AppliesToMinimumMaximumCost
                )
            );

        }

        #endregion

        #region TaxRates(NamedURLs)

        /// <summary>
        /// Create a new tax rates based on the given enumeration of tax rates.
        /// </summary>
        public TaxRates(params TaxRate[] TaxRates)
        {

            if (TaxRates is not null)
                foreach (var taxRate in TaxRates)
                    taxRates.Add(taxRate);

        }

        #endregion

        #region TaxRates(TaxRates)

        /// <summary>
        /// Create a new tax rates based on the given enumeration of tax rates.
        /// </summary>
        public TaxRates(IEnumerable<TaxRate> TaxRates)
        {

            foreach (var taxRate in TaxRates)
                taxRates.Add(taxRate);

        }

        #endregion

        #endregion


        #region (static) Parse(JSONArray, CustomTaxRatesParser = null)

        /// <summary>
        /// Parse the given JSON representation of tax rates.
        /// </summary>
        /// <param name="JSONArray">The JSON array to parse.</param>
        /// <param name="CustomTaxRateParser">An optional delegate to parse custom tax rate JSON objects.</param>
        public static TaxRates Parse(JArray                                 JSONArray,
                                     CustomJObjectParserDelegate<TaxRate>?  CustomTaxRateParser   = null)
        {

            if (TryParse(JSONArray,
                         out var taxRates,
                         out var errorResponse,
                         CustomTaxRateParser) &&
                taxRates is not null)
            {
                return taxRates;
            }

            throw new ArgumentException("The given JSON representation of tax rates is invalid: " + errorResponse,
                                        nameof(JSONArray));

        }

        #endregion

        #region TryParse(JSONArray, out TaxRates, out ErrorResponse, CustomTaxRateParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of tax rates.
        /// </summary>
        /// <param name="JSONArray">The JSON array to parse.</param>
        /// <param name="TaxRates">The parsed tax rates.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JArray         JSONArray,
                                       out TaxRates?  TaxRates,
                                       out String?    ErrorResponse)

            => TryParse(JSONArray,
                        out TaxRates,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of tax rates.
        /// </summary>
        /// <param name="JSONArray">The JSON array to parse.</param>
        /// <param name="TaxRates">The parsed tax rates.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTaxRateParser">An optional delegate to parse custom tax rate JSON objects.</param>
        public static Boolean TryParse(JArray                                 JSONArray,
                                       out TaxRates?                          TaxRates,
                                       out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<TaxRate>?  CustomTaxRateParser   = null)
        {

            TaxRates       = null;
            ErrorResponse  = null;

            if (JSONArray is null)
            {
                return true;
            }

            var taxRates = new HashSet<TaxRate>();

            foreach (var jsonToken in JSONArray)
            {

                try
                {

                    if (jsonToken.Type == JTokenType.Object &&
                        jsonToken is JObject jsonObject)
                    {

                        if (!TaxRate.TryParse(jsonObject,
                                              out var taxRate,
                                              out ErrorResponse,
                                              CustomTaxRateParser))
                        {
                            return false;
                        }

                        taxRates.Add(taxRate);

                    }

                }
                catch (Exception e)
                {
                    ErrorResponse = e.Message;
                    return false;
                }

            }

            TaxRates = new TaxRates(taxRates);
            return true;

        }

        #endregion

        #region ToJSON(CustomTaxRateSerializer = null)

        /// <summary>
        /// Return a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rate JSON objects.</param>
        public JArray ToJSON(CustomJObjectSerializerDelegate<TaxRate>? CustomTaxRateSerializer = null)

            => taxRates.Any()
                   ? new JArray(taxRates.Select(taxRate => taxRate.ToJSON(CustomTaxRateSerializer)))
                   : new JArray();

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this multi-language text/string.
        /// </summary>
        public TaxRates Clone()

            => new (taxRates.Select(taxRate => taxRate.Clone()));

        #endregion


        #region Add   (Type, Tax, ...)

        /// <summary>
        /// Add a tax rate.
        /// </summary>
        /// <param name="Type">The type of this tax, e.g. "VAT", "State", "Federal".</param>
        /// <param name="Tax">The tax percentage.</param>
        /// <param name="AppliesToEnergyFee">Whether the tax applies to the energy fee.</param>
        /// <param name="AppliesToParkingFee">Whether the tax applies to the parking fee.</param>
        /// <param name="AppliesToOverstayFee">Whether the tax applies to the overstay fee.</param>
        /// <param name="AppliesToMinimumMaximumCost">Whether the tax applies to minimum/maximum cost.</param>
        public TaxRates Add(String  Type,
                            Decimal Tax,
                            Boolean AppliesToEnergyFee            = false,
                            Boolean AppliesToParkingFee           = false,
                            Boolean AppliesToOverstayFee          = false,
                            Boolean AppliesToMinimumMaximumCost   = false)

            => new (new List<TaxRate>(taxRates) {
                        new TaxRate(
                            Type,
                            Tax,
                            AppliesToEnergyFee,
                            AppliesToParkingFee,
                            AppliesToOverstayFee,
                            AppliesToMinimumMaximumCost
                        )
                    });

        #endregion

        #region Add   (TaxRate)

        /// <summary>
        /// Add a tax rate.
        /// </summary>
        /// <param name="TaxRate">A tax rate to add.</param>
        public TaxRates Add(TaxRate TaxRate)

            => new (new List<TaxRate>(taxRates) { TaxRate });

        #endregion


        #region Get   (TaxRateType)

        /// <summary>
        /// Get tax rates of the given type.
        /// </summary>
        /// <param name="TaxRateType">A tax rate type.</param>
        public IEnumerable<TaxRate> Get(String TaxRateType)

            => taxRates.Where(taxRate => taxRate.Type == TaxRateType);

        #endregion

        #region Get   (TaxRateType, ...)

        /// <summary>
        /// Get the first tax rate matching the given criteria.
        /// </summary>
        /// <param name="TaxRateType">A tax rate type.</param>
        /// <param name="AppliesToEnergyFee">Whether the tax applies to the energy fee.</param>
        /// <param name="AppliesToParkingFee">Whether the tax applies to the parking fee.</param>
        /// <param name="AppliesToOverstayFee">Whether the tax applies to the overstay fee.</param>
        /// <param name="AppliesToMinimumMaximumCost">Whether the tax applies to minimum/maximum cost.</param>
        public TaxRate? Get(String   TaxRateType,
                            Boolean? AppliesToEnergyFee            = null,
                            Boolean? AppliesToParkingFee           = null,
                            Boolean? AppliesToOverstayFee          = null,
                            Boolean? AppliesToMinimumMaximumCost   = null)
        {

            var matches = taxRates.Where(taxRate => (AppliesToEnergyFee.         HasValue ? taxRate.AppliesToEnergyFee          == AppliesToEnergyFee          : true) &&
                                                    (AppliesToParkingFee.        HasValue ? taxRate.AppliesToParkingFee         == AppliesToParkingFee         : true) &&
                                                    (AppliesToOverstayFee.       HasValue ? taxRate.AppliesToOverstayFee        == AppliesToOverstayFee        : true) &&
                                                    (AppliesToMinimumMaximumCost.HasValue ? taxRate.AppliesToMinimumMaximumCost == AppliesToMinimumMaximumCost : true) &&
                                                     taxRate.Type == TaxRateType);

            return matches.Any()
                       ? matches.First()
                       : null;

        }

        #endregion


        #region Remove(Type)

        /// <summary>
        /// Remove all tax rates having the given type.
        /// </summary>
        /// <param name="Type">A type of tax rates to remove.</param>
        public TaxRates Remove(String Type)

            => new (new List<TaxRate>(taxRates.Where(taxRate => taxRate.Type != Type)));

        #endregion

        #region Remove(TaxRate)

        /// <summary>
        /// Remove the given tax rate.
        /// </summary>
        /// <param name="TaxRate">A tax rate to remove.</param>
        public TaxRates Remove(TaxRate TaxRate)

            => new (new List<TaxRate>(taxRates.Where(taxRate => taxRate != TaxRate)));

        #endregion


        #region Static definitions

        #region (static) Empty

        /// <summary>
        /// Create an empty tax rates.
        /// </summary>
        public static TaxRates Empty

            => new();

        #endregion

        #region (static) VAT(Percentage)

        /// <summary>
        /// Valued Added Tax.
        /// </summary>
        /// <param name="Percentage">The tax percentage.</param>
        public static TaxRates VAT(Decimal Percentage)

            => new(
                   new TaxRate(
                       Type:                          "VAT",
                       Tax:                           Percentage,
                       AppliesToEnergyFee:            true,
                       AppliesToParkingFee:           true,
                       AppliesToOverstayFee:          true,
                       AppliesToMinimumMaximumCost:   true
                   )
               );

        #endregion

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Enumerate all tax rates.
        /// </summary>
        public IEnumerator<TaxRate> GetEnumerator()
            => taxRates.GetEnumerator();

        /// <summary>
        /// Enumerate all tax rates.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => taxRates.GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (TaxRates1, TaxRates2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRates1">Tax rates.</param>
        /// <param name="TaxRates2">Another tax rates.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TaxRates? TaxRates1,
                                           TaxRates? TaxRates2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(TaxRates1, TaxRates2))
                return true;

            // If one is null, but not both, return false.
            if (TaxRates1 is null || TaxRates2 is null)
                return false;

            return TaxRates1.Equals(TaxRates2);

        }

        #endregion

        #region Operator != (TaxRates1, TaxRates2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRates1">Tax rates.</param>
        /// <param name="TaxRates2">Another tax rates.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TaxRates? TaxRates1,
                                           TaxRates? TaxRates2)

            => !(TaxRates1 == TaxRates2);

        #endregion

        #region Operator <  (TaxRates1, TaxRates2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRates1">Tax rates.</param>
        /// <param name="TaxRates2">Another tax rates.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TaxRates TaxRates1,
                                          TaxRates TaxRates2)

            => TaxRates1.CompareTo(TaxRates2) < 0;

        #endregion

        #region Operator <= (TaxRates1, TaxRates2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRates1">Tax rates.</param>
        /// <param name="TaxRates2">Another tax rates.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TaxRates TaxRates1,
                                           TaxRates TaxRates2)

            => TaxRates1.CompareTo(TaxRates2) <= 0;

        #endregion

        #region Operator >  (TaxRates1, TaxRates2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRates1">Tax rates.</param>
        /// <param name="TaxRates2">Another tax rates.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TaxRates TaxRates1,
                                          TaxRates TaxRates2)

            => TaxRates1.CompareTo(TaxRates2) > 0;

        #endregion

        #region Operator >= (TaxRates1, TaxRates2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRates1">Tax rates.</param>
        /// <param name="TaxRates2">Another tax rates.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TaxRates TaxRates1,
                                           TaxRates TaxRates2)

            => TaxRates1.CompareTo(TaxRates2) >= 0;

        #endregion

        #endregion

        #region IComparable<TaxRates> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tax rates.
        /// </summary>
        /// <param name="Object">Tax rates to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TaxRates taxRates
                   ? CompareTo(taxRates)
                   : throw new ArgumentException("The given object is not a tax rates object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TaxRates)

        /// <summary>
        /// Compares two tax rates.
        /// </summary>
        /// <param name="TaxRates">Tax rates to compare with.</param>
        public Int32 CompareTo(TaxRates? TaxRates)
        {

            if (TaxRates is null)
                throw new ArgumentNullException(nameof(TaxRates), "The given tax rates must not be null!");

            var c = taxRates.Count.CompareTo(TaxRates.taxRates.Count);

            if (c == 0)
            {

                var a = taxRates.OrderBy(taxRate => taxRate).ToArray();
                var b = TaxRates.OrderBy(taxRate => taxRate).ToArray();

                for (var i = 0; i < a.Length; i++)
                {

                    c = a[i].CompareTo(b[i]);

                    if (c != 0)
                        return c;

                }

            }

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TaxRates> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tax rates for equality.
        /// </summary>
        /// <param name="Object">Tax rates to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TaxRates taxRates &&
                  Equals(taxRates);

        #endregion

        #region Equals(TaxRates)

        /// <summary>
        /// Compares two tax rates for equality.
        /// </summary>
        /// <param name="Object">Tax rates to compare with.</param>
        public Boolean Equals(TaxRates? OtherTaxRates)

            => OtherTaxRates is not null &&

               taxRates.Count.Equals(OtherTaxRates.taxRates.Count) &&
               taxRates.All(OtherTaxRates.taxRates.Contains);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => taxRates.CalcHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => taxRates.Count > 0

                   ? taxRates.
                         Select(taxRate => taxRate.ToString()).
                         AggregateWith("; ")

                   : "<empty>";

        #endregion


    }

}
