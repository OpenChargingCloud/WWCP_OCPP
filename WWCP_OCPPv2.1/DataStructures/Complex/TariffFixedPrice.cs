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

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The prices within a TariffFixed tariff element.
    /// </summary>
    public class TariffFixedPrice : ACustomData,
                                 ICBORSerializable<TariffFixedPrice>
    {

        #region Properties

        /// <summary>
        /// The fixed price (excl. tax) for this tariff element.
        /// </summary>
        [Mandatory]
        public Decimal            PriceFixed    { get; }

        /// <summary>
        /// Optional tariff conditions.
        /// </summary>
        [Optional]
        public TariffConditions?  Conditions    { get;  }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new prices for an fixed tariff element.
        /// </summary>
        /// <param name="PriceFixed">A fixed price (excl. tax) for this TariffFixedPrice.</param>
        /// <param name="Conditions">Optional tariff conditions.</param>
        public TariffFixedPrice(Decimal            PriceFixed,
                                TariffConditions?  Conditions   = null,
                                CustomData?        CustomData   = null)

            : base(CustomData)

        {

            this.PriceFixed  = PriceFixed;
            this.Conditions  = Conditions;

            unchecked
            {

                hashCode = this.PriceFixed. GetHashCode() * 3 ^
                          (this.Conditions?.GetHashCode() ?? 0) ^
                          base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Tariff with optional conditions for a fixed price.",
        //     "javaType": "TariffFixedPrice",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "conditions": {
        //             "$ref": "#/definitions/TariffConditionsFixedType"
        //         },
        //         "priceFixed": {
        //             "description": "Fixed price  for this element e.g. a start fee.",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "priceFixed"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffFixedPricesParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TariffFixedPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffFixedPriceParser">An optional delegate to parse custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static TariffFixedPrice Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<TariffFixedPrice>?  CustomTariffFixedPriceParser   = null,
                                             CustomJObjectParserDelegate<TariffConditions>?  CustomTariffConditionsParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffFixedPrice,
                         out var errorResponse,
                         CustomTariffFixedPriceParser,
                         CustomTariffConditionsParser))
            {
                return tariffFixedPrice;
            }

            throw new ArgumentException("The given JSON representation of a TariffFixedPrice is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffFixedPrices, out ErrorResponse, CustomTariffFixedPricesParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a TariffFixedPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffFixedPrices">The parsed TariffFixedPrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out TariffFixedPrice?  TariffFixedPrices,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out TariffFixedPrices,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a TariffFixedPrice.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffFixedPrice">The parsed TariffFixedPrice.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffFixedPriceParser">An optional delegate to parse custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsParser">An optional delegate to parse custom TariffConditions JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out TariffFixedPrice?      TariffFixedPrice,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<TariffFixedPrice>?  CustomTariffFixedPriceParser   = null,
                                       CustomJObjectParserDelegate<TariffConditions>?  CustomTariffConditionsParser    = null)
        {

            try
            {

                TariffFixedPrice = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PriceFixed    [mandatory]

                if (!JSON.ParseMandatory("priceFixed",
                                         "fixed price",
                                         out Decimal PriceFixed,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Conditions    [optional]

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


                #region Parse CustomData    [optional]

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


                TariffFixedPrice = new TariffFixedPrice(
                                       PriceFixed,
                                       Conditions,
                                       CustomData
                                   );


                if (CustomTariffFixedPriceParser is not null)
                    TariffFixedPrice = CustomTariffFixedPriceParser(JSON,
                                                                    TariffFixedPrice);

                return true;

            }
            catch (Exception e)
            {
                TariffFixedPrice  = default;
                ErrorResponse     = "The given JSON representation of a tariff element is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffFixedPriceSerializer = null, CustomTariffConditionsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffFixedPrice>?  CustomTariffFixedPriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?  CustomTariffConditionsSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priceFixed",   PriceFixed),

                           Conditions is not null
                               ? new JProperty("conditions",   Conditions.ToJSON(CustomTariffConditionsSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON())
                               : null

                       );

            return CustomTariffFixedPriceSerializer is not null
                       ? CustomTariffFixedPriceSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out TariffFixedPrice, out ErrorResponse, CustomTariffFixedPriceParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a tariff fixed price.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TariffFixedPrice">The tariff fixed price.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out TariffFixedPrice?  TariffFixedPrice,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out TariffFixedPrice,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a tariff fixed price.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TariffFixedPrice">The tariff fixed price.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffFixedPriceParser">An optional delegate to read custom tariff fixed prices.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out TariffFixedPrice?           TariffFixedPrice,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<TariffFixedPrice>?  CustomTariffFixedPriceParser)
        {

            try
            {

                TariffFixedPrice = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a tariff fixed price is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryDecimal("priceFixed",
                                                "fixed price",
                                                out var PriceFixed,
                                                out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptional("conditions",
                                   "conditions",
                                   OCPPv2_1.TariffConditions.TryParseCBOR,
                                   out TariffConditions? Conditions,
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

                TariffFixedPrice = new TariffFixedPrice(
                                       PriceFixed,
                                       Conditions,
                                       CustomData
                                   );

                if (CustomTariffFixedPriceParser is not null)
                    TariffFixedPrice = CustomTariffFixedPriceParser(CBOR,
                                                  TariffFixedPrice);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TariffFixedPrice  = default;
                ErrorResponse  = "The given CBOR representation of a tariff fixed price is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<TariffFixedPrice>.TryParse(CBOR, out TariffFixedPrice, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a tariff fixed price - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<TariffFixedPrice>.TryParse(CBORValue                         CBOR,
                                                                  out TariffFixedPrice                  Value,
                                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomTariffFixedPriceSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this tariff fixed price: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom tariff fixed prices.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<TariffFixedPrice>? CustomTariffFixedPriceSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("priceFixed",              CBORValue.FromDecimal(PriceFixed)),
                           ("conditions",              Conditions?.ToCBOR()),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomTariffFixedPriceSerializer is not null
                       ? CustomTariffFixedPriceSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffFixedPrice.
        /// </summary>
        public TariffFixedPrice Clone()

            => new (
                   PriceFixed,
                   Conditions?.Clone(),
                   CustomData
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffFixedPrices1, TariffFixedPrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffFixedPrices1">A TariffFixedPrice.</param>
        /// <param name="TariffFixedPrices2">Another TariffFixedPrice.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffFixedPrice? TariffFixedPrices1,
                                           TariffFixedPrice? TariffFixedPrices2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffFixedPrices1, TariffFixedPrices2))
                return true;

            // If one is null, but not both, return false.
            if (TariffFixedPrices1 is null || TariffFixedPrices2 is null)
                return false;

            return TariffFixedPrices1.Equals(TariffFixedPrices2);

        }

        #endregion

        #region Operator != (TariffFixedPrices1, TariffFixedPrices2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffFixedPrices1">A TariffFixedPrice.</param>
        /// <param name="TariffFixedPrices2">Another TariffFixedPrice.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffFixedPrice? TariffFixedPrices1,
                                           TariffFixedPrice? TariffFixedPrices2)

            => !(TariffFixedPrices1 == TariffFixedPrices2);

        #endregion

        #endregion

        #region IEquatable<TariffFixedPrices> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffFixedPrices for equality.
        /// </summary>
        /// <param name="Object">A TariffFixedPrice to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffFixedPrice tariffFixedPrice &&
                   Equals(tariffFixedPrice);

        #endregion

        #region Equals(TariffFixedPrices)

        /// <summary>
        /// Compares two TariffFixedPrices for equality.
        /// </summary>
        /// <param name="TariffFixedPrice">A TariffFixedPrice to compare with.</param>
        public Boolean Equals(TariffFixedPrice? TariffFixedPrice)

            => TariffFixedPrice is not null &&

               PriceFixed.Equals(TariffFixedPrice.PriceFixed) &&

             ((Conditions is     null && TariffFixedPrice.Conditions is     null) ||
              (Conditions is not null && TariffFixedPrice.Conditions is not null && Conditions.Equals(TariffFixedPrice.Conditions))) &&

               base.Equals(TariffFixedPrice);

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

            => $"{PriceFixed} €{(Conditions is not null ? $", with '{Conditions}'" : "")}!";

        #endregion

    }

}
