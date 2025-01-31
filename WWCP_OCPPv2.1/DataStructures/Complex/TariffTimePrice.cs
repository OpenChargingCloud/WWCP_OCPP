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
    /// The prices within an time tariff element.
    /// </summary>
    public class TariffTimePrice
    {

        #region Properties

        /// <summary>
        /// The price per minute (excl. tax) for this tariff element.
        /// </summary>
        [Mandatory]
        public Decimal            PriceMinute    { get; }

        /// <summary>
        /// The optional step size.
        /// When absent, the exact amount is billed.
        /// When present, this type is billed in blocks of stepSize of the base unit: Seconds.
        /// Amounts are rounded up to a multiple of stepSize.
        /// </summary>
        /// <example>
        /// When the step_size is 60, then time will be billed in blocks of 1 minute.
        /// So if it was charged for 70 seconds 2 minutes (2 blocks of step_size) will be billed.
        /// </example>
        [Optional]
        public TimeSpan?          StepSize       { get; }

        /// <summary>
        /// The optional enumeration of tariff conditions
        /// </summary>
        [Optional]
        public TariffConditions?  Conditions     { get;  }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new prices for an time tariff element.
        /// </summary>
        /// <param name="PriceMinute">A price per minute (excl. tax) for this time tariff.</param>
        /// <param name="StepSize">An optional seconds step size.</param>
        /// <param name="Conditions">Optional tariff conditions.</param>
        public TariffTimePrice(Decimal            PriceMinute,
                               TimeSpan?          StepSize     = null,
                               TariffConditions?  Conditions   = null)
        {

            this.PriceMinute  = PriceMinute;
            this.StepSize     = StepSize;
            this.Conditions   = Conditions;

            unchecked
            {

                hashCode = this.PriceMinute.GetHashCode()       * 5 ^
                          (this.StepSize?.  GetHashCode() ?? 0) * 3 ^
                          (this.Conditions?.GetHashCode() ?? 0);

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Tariff with optional conditions for a time duration price.",
        //     "javaType": "TariffTimePrice",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priceMinute": {
        //             "description": "Price per minute (excl. tax) for this element.",
        //             "type": "number"
        //         },
        //         "conditions": {
        //             "$ref": "#/definitions/TariffConditionsType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "priceMinute"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffTimePricesParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TariffTimePrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffTimePricesParser">An optional delegate to parse custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static TariffTimePrice Parse(JObject                                         JSON,
                                            CustomJObjectParserDelegate<TariffTimePrice>?   CustomTariffTimePricesParser   = null,
                                            CustomJObjectParserDelegate<TariffConditions>?  CustomTariffConditionsParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffTimePrice,
                         out var errorResponse,
                         CustomTariffTimePricesParser))
            {
                return tariffTimePrice;
            }

            throw new ArgumentException("The given JSON representation of a TariffTimePrice is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffTimePrices, out ErrorResponse, CustomTariffTimePricesParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a TariffTimePrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="tariffTimePrice">The parsed TariffTimePrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out TariffTimePrice?  tariffTimePrice,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out tariffTimePrice,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a TariffTimePrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="tariffTimePrice">The parsed TariffTimePrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffTimePricesParser">An optional delegate to parse custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out TariffTimePrice?       tariffTimePrice,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<TariffTimePrice>?   CustomTariffTimePricesParser   = null,
                                       CustomJObjectParserDelegate<TariffConditions>?  CustomTariffConditionsParser   = null)
        {

            try
            {

                tariffTimePrice = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PriceMinute    [mandatory]

                if (!JSON.ParseMandatory("priceMinute",
                                         "price per minute",
                                         out Decimal PriceMinute,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse StepSize       [optional]

                if (!JSON.ParseOptional("stepSize",
                                        "price components",
                                        out UInt32? StepSizeUInt32,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var StepSize = StepSizeUInt32.HasValue
                                  ? new TimeSpan?(TimeSpan.FromSeconds(StepSizeUInt32.Value))
                                  : null;

                #endregion

                #region Parse Conditions     [optional]

                if (JSON.ParseOptionalJSONMayBeNull("conditions",
                                                    "tariff conditions",
                                                    TariffConditions.TryParse,
                                                    out TariffConditions? Conditions,
                                                    out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                tariffTimePrice = new TariffTimePrice(
                                      PriceMinute,
                                      StepSize,
                                      Conditions
                                  );


                if (CustomTariffTimePricesParser is not null)
                    tariffTimePrice = CustomTariffTimePricesParser(JSON,
                                                                   tariffTimePrice);

                return true;

            }
            catch (Exception e)
            {
                tariffTimePrice  = default;
                ErrorResponse     = "The given JSON representation of a TariffTimePrice is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffTimePriceSerializer = null, CustomTariffConditionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffTimePriceSerializer">A delegate to serialize custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffTimePrice>?   CustomTariffTimePriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?  CustomTariffConditionsSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priceMinute",   PriceMinute),

                           StepSize.HasValue
                               ? new JProperty("stepSize",      Convert.ToInt32(StepSize.Value.TotalSeconds))
                               : null,

                           Conditions is not null
                               ? new JProperty("conditions",    Conditions.ToJSON(CustomTariffConditionsSerializer))
                               : null

                       );

            return CustomTariffTimePriceSerializer is not null
                       ? CustomTariffTimePriceSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffTimePrice.
        /// </summary>
        public TariffTimePrice Clone()

            => new (
                   PriceMinute,
                   StepSize,
                   Conditions?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffTimePrices1, TariffTimePrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffTimePrices1">A tariff time price.</param>
        /// <param name="TariffTimePrices2">Another tariff time price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffTimePrice? TariffTimePrices1,
                                           TariffTimePrice? TariffTimePrices2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffTimePrices1, TariffTimePrices2))
                return true;

            // If one is null, but not both, return false.
            if (TariffTimePrices1 is null || TariffTimePrices2 is null)
                return false;

            return TariffTimePrices1.Equals(TariffTimePrices2);

        }

        #endregion

        #region Operator != (TariffTimePrices1, TariffTimePrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffTimePrices1">A tariff time price.</param>
        /// <param name="TariffTimePrices2">Another tariff time price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffTimePrice? TariffTimePrices1,
                                           TariffTimePrice? TariffTimePrices2)

            => !(TariffTimePrices1 == TariffTimePrices2);

        #endregion

        #endregion

        #region IEquatable<TariffTimePrices> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff time prices for equality.
        /// </summary>
        /// <param name="Object">A tariff time price to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffTimePrice tariffTimePrice &&
                   Equals(tariffTimePrice);

        #endregion

        #region Equals(TariffTimePrices)

        /// <summary>
        /// Compares two tariff time prices for equality.
        /// </summary>
        /// <param name="TariffTimePrice">A tariff time price to compare with.</param>
        public Boolean Equals(TariffTimePrice? TariffTimePrice)

            => TariffTimePrice is not null &&

               PriceMinute.Equals(TariffTimePrice.PriceMinute) &&

            ((!StepSize.HasValue      && !TariffTimePrice.StepSize.HasValue)      ||
              (StepSize.HasValue      &&  TariffTimePrice.StepSize.HasValue      && StepSize.Value.Equals(TariffTimePrice.StepSize.Value))) &&

             ((Conditions is     null &&  TariffTimePrice.Conditions is     null) ||
              (Conditions is not null &&  TariffTimePrice.Conditions is not null && Conditions.    Equals(TariffTimePrice.Conditions)));

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

            => $"{PriceMinute} €/minute{(StepSize is not null ? $", per {StepSize.Value} second(s)" : "")}{(Conditions is not null ? $", with '{Conditions}'" : "")}!";

        #endregion

    }

}
