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
    /// A price component defines the pricing of a tariff.
    /// </summary>
    public readonly struct PriceComponent : IEquatable<PriceComponent>,
                                            IComparable<PriceComponent>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// The tariff dimension.
        /// </summary>
        [Mandatory]
        public TariffDimension  Type        { get; }

        /// <summary>
        /// The price per unit (excl. VAT) for this tariff dimension.
        /// </summary>
        [Mandatory]
        public Decimal          Price       { get; }

        /// <summary>
        /// The enumeration of applicable tax percentages for this tariff dimension.
        /// If omitted, no tax is applicable. Not providing a tax is different from 0% tax, which would be a value of 0.0 here.
        /// </summary>
        [Optional]
        public TaxRates         TaxRates    { get; }

        /// <summary>
        /// The minimum amount to be billed. This unit will be billed in this step_size blocks.
        /// </summary>
        /// <example>
        /// If type is time and step_size is 300, then time will be billed in blocks of 5 minutes,
        /// so if 6 minutes is used, 10 minutes (2 blocks of step_size) will be billed.
        /// </example>
        [Mandatory]
        public UInt32?          StepSize    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price component defining the pricing of a tariff.
        /// </summary>
        /// <param name="Type">A tariff dimension.</param>
        /// <param name="Price">A price per unit (excl. VAT) for this tariff dimension.</param>
        /// <param name="TaxRates">The enumeration of applicable tax percentages for this tariff dimension.</param>
        /// <param name="StepSize">The minimum amount to be billed. This unit will be billed in this step_size blocks.</param>
        public PriceComponent(TariffDimension  Type,
                              Decimal          Price,
                              TaxRates?        TaxRates   = null,
                              UInt32?          StepSize   = null)
        {

            this.Type      = Type;
            this.Price     = Price;
            this.TaxRates  = TaxRates ?? TaxRates.Empty;
            this.StepSize  = StepSize;

        }

        #endregion


        #region (static) Parse   (JSON, CustomPriceComponentParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static PriceComponent Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<PriceComponent>?  CustomPriceComponentParser   = null)
        {

            if (TryParse(JSON,
                         out var priceComponent,
                         out var errorResponse,
                         CustomPriceComponentParser))
            {
                return priceComponent;
            }

            throw new ArgumentException("The given JSON representation of a price component is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomPriceComponentParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static PriceComponent? TryParse(JObject                                          JSON,
                                                  CustomJObjectParserDelegate<PriceComponent>?  CustomPriceComponentParser   = null)
        {

            if (TryParse(JSON,
                         out var priceComponent,
                         out var errorResponse,
                         CustomPriceComponentParser))
            {
                return priceComponent;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out PriceComponent, out ErrorResponse, CustomPriceComponentParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PriceComponent">The parsed price component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out PriceComponent  PriceComponent,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out PriceComponent,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price component.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PriceComponent">The parsed price component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceComponentParser">A delegate to parse custom price component JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out PriceComponent                            PriceComponent,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<PriceComponent>?  CustomPriceComponentParser   = null)
        {

            try
            {

                PriceComponent = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Type        [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "tariff dimension type",
                                         TariffDimension.TryParse,
                                         out TariffDimension Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Price       [mandatory]

                if (!JSON.ParseMandatory("price",
                                         "price",
                                         out Decimal Price,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TaxRates    [optional]

                if (JSON.ParseOptionalJSONArray("taxRates",
                                                "tax rates",
                                                OCPPv2_1.TaxRates.TryParse,
                                                out TaxRates TaxRates,
                                                out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse StepSize    [mandatory]

                if (!JSON.ParseMandatory("step_size",
                                         "step size",
                                         out UInt32 StepSize,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PriceComponent = new PriceComponent(
                                     Type,
                                     Price,
                                     TaxRates,
                                     StepSize
                                 );


                if (CustomPriceComponentParser is not null)
                    PriceComponent = CustomPriceComponentParser(JSON,
                                                                PriceComponent);

                return true;

            }
            catch (Exception e)
            {
                PriceComponent  = default;
                ErrorResponse   = "The given JSON representation of a price component is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceComponentSerializer = null, CustomTaxRateSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rate JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PriceComponent>?  CustomPriceComponentSerializer   = null,
                              CustomJObjectSerializerDelegate<TaxRate>?         CustomTaxRateSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("type",       Type.ToString()),
                                 new JProperty("price",      Price),

                           TaxRates.Any()
                               ? new JProperty("taxRates",   new JArray(TaxRates.Select(taxRate => taxRate.ToJSON(CustomTaxRateSerializer))))
                               : null,

                           StepSize.HasValue
                               ? new JProperty("stepSize",   StepSize.Value)
                               : null

                       );

            return CustomPriceComponentSerializer is not null
                       ? CustomPriceComponentSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public PriceComponent Clone()

            => new (
                   Type.Clone,
                   Price,
                   TaxRates,
                   StepSize
               );

        #endregion


        #region Static definitions

        #region (static) Flat        (Price, VAT = null)

        /// <summary>
        /// Create a new flat rate price component.
        /// </summary>
        /// <param name="Price">A flat rate price (excl. VAT).</param>
        public static PriceComponent FlatRate(Decimal   Price,
                                              Decimal?  VAT   = null)

            => new (TariffDimension.FLAT,
                    Price,
                    VAT.HasValue
                        ? TaxRates.VAT(VAT.Value)
                        : null,
                    1);

        #endregion

        #region (static) Energy      (Price, VAT = null, StepSize = 1)

        /// <summary>
        /// Create a new energy price component.
        /// </summary>
        /// <param name="Price">An energy price (excl. VAT).</param>
        /// <param name="StepSize">An optional minimum granularity of Wh that will be billed.</param>
        public static PriceComponent Energy(Decimal   Price,
                                            Decimal?  VAT        = null,
                                            UInt32    StepSize   = 1)

            => new (TariffDimension.ENERGY,
                    Price,
                    VAT.HasValue
                        ? TaxRates.VAT(VAT.Value)
                        : null,
                    StepSize);

        #endregion

        #region (static) Time        (Price, VAT = null, Duration = null)

        /// <summary>
        /// Create a new time-based charging price component.
        /// </summary>
        /// <param name="Price">A price per time span (excl. VAT).</param>
        /// <param name="Duration">An optional minimum granularity of time that will be billed.</param>
        public static PriceComponent Time(Decimal    Price,
                                          Decimal?   VAT        = null,
                                          TimeSpan?  Duration   = null)

            => new (TariffDimension.TIME,
                    Price,
                    VAT.     HasValue
                        ? TaxRates.VAT(VAT.Value)
                        : null,
                    Duration.HasValue
                        ? (UInt32) Math.Round(Duration.Value.TotalSeconds, 0)
                        : 1);

        #endregion

        #region (static) ParkingTime (Price, VAT = null, Duration = null)

        /// <summary>
        /// Create a new time-based parking price component.
        /// </summary>
        /// <param name="Price">A price per time span (excl. VAT).</param>
        /// <param name="Duration">An optional minimum granularity of time that will be billed.</param>
        public static PriceComponent ParkingTime(Decimal    Price,
                                                 Decimal?   VAT        = null,
                                                 TimeSpan?  Duration   = null)

            => new (TariffDimension.PARKING_TIME,
                    Price,
                    VAT.HasValue
                        ? TaxRates.VAT(VAT.Value)
                        : null,
                    Duration.HasValue
                        ? (UInt32) Math.Round(Duration.Value.TotalSeconds, 0)
                        : 1);

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => PriceComponent1.Equals(PriceComponent2);

        #endregion

        #region Operator != (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => !PriceComponent1.Equals(PriceComponent2);

        #endregion

        #region Operator <  (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PriceComponent PriceComponent1,
                                          PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) < 0;

        #endregion

        #region Operator <= (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) <= 0;

        #endregion

        #region Operator >  (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PriceComponent PriceComponent1,
                                          PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) > 0;

        #endregion

        #region Operator >= (PriceComponent1, PriceComponent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceComponent1">A price component.</param>
        /// <param name="PriceComponent2">Another price component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PriceComponent PriceComponent1,
                                           PriceComponent PriceComponent2)

            => PriceComponent1.CompareTo(PriceComponent2) >= 0;

        #endregion

        #endregion

        #region IComparable<PriceComponent> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two price components.
        /// </summary>
        /// <param name="Object">A price component to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PriceComponent priceComponent
                   ? CompareTo(priceComponent)
                   : throw new ArgumentException("The given object is not a price component!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PriceComponent)

        /// <summary>
        /// Compares two price components.
        /// </summary>
        /// <param name="PriceComponent">A price component to compare with.</param>
        public Int32 CompareTo(PriceComponent PriceComponent)
        {

            var c = Type.           CompareTo(PriceComponent.Type);

            if (c == 0)
                c = Price.          CompareTo(PriceComponent.Price);

            if (c == 0)
                c = (StepSize ?? 1).CompareTo(PriceComponent.StepSize ?? 1);

            if (c == 0)
                c = Price.          CompareTo(PriceComponent.Price);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PriceComponent> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price components for equality.
        /// </summary>
        /// <param name="Object">A price component to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceComponent priceComponent &&
                   Equals(priceComponent);

        #endregion

        #region Equals(PriceComponent)

        /// <summary>
        /// Compares two price components for equality.
        /// </summary>
        /// <param name="PriceComponent">A price component to compare with.</param>
        public Boolean Equals(PriceComponent PriceComponent)

            => Type.    Equals(PriceComponent.Type)     &&
               Price.   Equals(PriceComponent.Price)    &&
               TaxRates.Equals(PriceComponent.TaxRates) &&
               StepSize.Equals(PriceComponent.StepSize);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Type.    GetHashCode() * 7 ^
                       Price.   GetHashCode() * 5 ^
                       StepSize.GetHashCode() * 3 ^
                       TaxRates.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{Type}, {StepSize}, {Price}",

                   TaxRates.Any()
                       ? ", " + TaxRates.ToString()
                       : ""

               );

        #endregion

    }

}
