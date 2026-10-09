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

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The tax rule.
    /// </summary>
    public class TaxRule : ACustomData,
                           ICBORSerializable<TaxRule>,
                           IEquatable<TaxRule>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the tax rule.
        /// </summary>
        [Mandatory]
        public TaxRule_Id      TaxRuleId                      { get; }

        /// <summary>
        /// The optional human readable string to identify the tax rule.
        /// </summary>
        [Optional]
        public String?         TaxRuleName                    { get; }

        /// <summary>
        /// The tax rate.
        /// </summary>
        [Mandatory]
        public RationalNumber  TaxRate                        { get; }

        /// <summary>
        /// Whether the tax is included within the price.
        /// </summary>
        [Optional]
        public Boolean?        TaxIncludedInPrice             { get; }

        /// <summary>
        /// Whether the tax applies to the energy fee.
        /// </summary>
        [Mandatory]
        public Boolean         AppliesToEnergyFee             { get; }

        /// <summary>
        /// Whether the tax applies to the parking fee.
        /// </summary>
        [Mandatory]
        public Boolean         AppliesToParkingFee            { get; }

        /// <summary>
        /// Whether the tax applies to the overstay fee.
        /// </summary>
        [Mandatory]
        public Boolean         AppliesToOverstayFee           { get; }

        /// <summary>
        /// Whether the tax applies to minimum/maximum cost.
        /// </summary>
        [Mandatory]
        public Boolean         AppliesToMinimumMaximumCost    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tax rule.
        /// </summary>
        /// <param name="TaxRuleId">An unique identification of the tax rule.</param>
        /// <param name="TaxRate">A tax rate.</param>
        /// <param name="AppliesToEnergyFee">Whether the tax applies to the energy fee.</param>
        /// <param name="AppliesToParkingFee">Whether the tax applies to the parking fee.</param>
        /// <param name="AppliesToOverstayFee">Whether the tax applies to the overstay fee.</param>
        /// <param name="AppliesToMinimumMaximumCost">Whether the tax applies to minimum/maximum cost.</param>
        /// <param name="TaxRuleName">An optional human readable string to identify the tax rule.</param>
        /// <param name="TaxIncludedInPrice">Whether the tax is included within the price.</param>
        public TaxRule(TaxRule_Id      TaxRuleId,
                       RationalNumber  TaxRate,
                       Boolean         AppliesToEnergyFee,
                       Boolean         AppliesToParkingFee,
                       Boolean         AppliesToOverstayFee,
                       Boolean         AppliesToMinimumMaximumCost,
                       String?         TaxRuleName          = null,
                       Boolean?        TaxIncludedInPrice   = null,
                       CustomData?     CustomData           = null)

            : base(CustomData)

        {

            this.TaxRuleId                    = TaxRuleId;
            this.TaxRate                      = TaxRate;
            this.AppliesToEnergyFee           = AppliesToEnergyFee;
            this.AppliesToParkingFee          = AppliesToParkingFee;
            this.AppliesToOverstayFee         = AppliesToOverstayFee;
            this.AppliesToMinimumMaximumCost  = AppliesToMinimumMaximumCost;
            this.TaxRuleName                  = TaxRuleName;
            this.TaxIncludedInPrice           = TaxIncludedInPrice;

            unchecked
            {

                hashCode = this.TaxRuleId.                  GetHashCode()       * 23 ^
                           this.TaxRate.                    GetHashCode()       * 19 ^
                           this.AppliesToEnergyFee.         GetHashCode()       * 17 ^
                           this.AppliesToParkingFee.        GetHashCode()       * 13 ^
                           this.AppliesToOverstayFee.       GetHashCode()       * 11 ^
                           this.AppliesToMinimumMaximumCost.GetHashCode()       *  7 ^
                          (this.TaxRuleName?.               GetHashCode() ?? 0) *  5 ^
                          (this.TaxIncludedInPrice?.        GetHashCode() ?? 0) *  3 ^
                           base.                            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Part of ISO 15118-20 price schedule.",
        //     "javaType": "TaxRule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "taxRuleID": {
        //             "description": "Id for the tax rule.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "taxRuleName": {
        //             "description": "Human readable string to identify the tax rule.",
        //             "type": "string",
        //             "maxLength": 100
        //         },
        //         "taxIncludedInPrice": {
        //             "description": "Indicates whether the tax is included in any price or not.",
        //             "type": "boolean"
        //         },
        //         "appliesToEnergyFee": {
        //             "description": "Indicates whether this tax applies to Energy Fees.",
        //             "type": "boolean"
        //         },
        //         "appliesToParkingFee": {
        //             "description": "Indicates whether this tax applies to Parking Fees.",
        //             "type": "boolean"
        //         },
        //         "appliesToOverstayFee": {
        //             "description": "Indicates whether this tax applies to Overstay Fees.",
        //             "type": "boolean"
        //         },
        //         "appliesToMinimumMaximumCost": {
        //             "description": "Indicates whether this tax applies to Minimum/Maximum Cost.",
        //             "type": "boolean"
        //         },
        //         "taxRate": {
        //             "$ref": "#/definitions/RationalNumberType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "taxRuleID",
        //         "appliesToEnergyFee",
        //         "appliesToParkingFee",
        //         "appliesToOverstayFee",
        //         "appliesToMinimumMaximumCost",
        //         "taxRate"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTaxRuleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tax rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomTaxRuleParser">An optional delegate to parse custom tax rules.</param>
        public static TaxRule Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<TaxRule>?  CustomTaxRuleParser   = null)
        {

            if (TryParse(JSON,
                         out var taxRule,
                         out var errorResponse,
                         CustomTaxRuleParser))
            {
                return taxRule;
            }

            throw new ArgumentException("The given JSON representation of a tax rule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TaxRule, out ErrorResponse, CustomTaxRuleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tax rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TaxRule">The parsed tax rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out TaxRule?  TaxRule,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

            => TryParse(JSON,
                        out TaxRule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tax rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TaxRule">The parsed tax rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTaxRuleParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out TaxRule?      TaxRule,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<TaxRule>?  CustomTaxRuleParser)
        {

            try
            {

                TaxRule = null;

                #region TaxRuleId                      [mandatory]

                if (!JSON.ParseMandatory("taxRuleID",
                                         "tax rule identification",
                                         TaxRule_Id.TryParse,
                                         out TaxRule_Id TaxRuleId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TaxRate                        [mandatory]

                if (!JSON.ParseMandatoryJSON("taxRate",
                                             "tax rate",
                                             RationalNumber.TryParse,
                                             out RationalNumber? TaxRate,
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

                #region TaxRuleName                    [optional]

                var TaxRuleName = JSON.GetString("taxRuleName");

                #endregion

                #region TaxIncludedInPrice             [optional]

                if (JSON.ParseOptional("taxIncludedInPrice",
                                       "tax included in price",
                                       out Boolean? TaxIncludedInPrice,
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


                TaxRule = new TaxRule(
                              TaxRuleId,
                              TaxRate,
                              AppliesToEnergyFee,
                              AppliesToParkingFee,
                              AppliesToOverstayFee,
                              AppliesToMinimumMaximumCost,
                              TaxRuleName,
                              TaxIncludedInPrice,
                              CustomData
                          );

                if (CustomTaxRuleParser is not null)
                    TaxRule = CustomTaxRuleParser(JSON,
                                                  TaxRule);

                return true;

            }
            catch (Exception e)
            {
                TaxRule        = null;
                ErrorResponse  = "The given JSON representation of a tax rule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTaxRuleSerializer = null, CustomRationalNumberSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTaxRuleSerializer">A delegate to serialize custom tax rules.</param>
        /// <param name="CustomRationalNumberSerializer">A delegate to serialize custom rational numbers.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TaxRule>? CustomTaxRuleSerializer   = null)
        {

            var json = JSONObject.Create(

                                  new JProperty("taxRuleID",                     TaxRuleId.        Value),
                                  new JProperty("taxRate",                       TaxRate.          ToJSON()),
                                  new JProperty("appliesToEnergyFee",            AppliesToEnergyFee),
                                  new JProperty("appliesToParkingFee",           AppliesToParkingFee),
                                  new JProperty("appliesToOverstayFee",          AppliesToOverstayFee),
                                  new JProperty("appliesToMinimumMaximumCost",   AppliesToMinimumMaximumCost),

                           TaxRuleName.IsNotNullOrEmpty()
                                ? new JProperty("taxRuleName",                   TaxRuleName)
                                : null,

                           TaxIncludedInPrice.HasValue
                                ? new JProperty("taxIncludedInPrice",            TaxIncludedInPrice.Value)
                                : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON())
                               : null

                       );

            return CustomTaxRuleSerializer is not null
                       ? CustomTaxRuleSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out TaxRule, out ErrorResponse, CustomTaxRuleParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a tax rule.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TaxRule">The tax rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out TaxRule?  TaxRule,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out TaxRule,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a tax rule.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TaxRule">The tax rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTaxRuleParser">An optional delegate to read custom tax rules.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out TaxRule?           TaxRule,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<TaxRule>?  CustomTaxRuleParser)
        {

            try
            {

                TaxRule = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a tax rule is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("taxRuleID",
                                               "tax rule identification",
                                               out var TaxRuleIdNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (TaxRuleIdNumber > UInt32.MaxValue || !ISO15118_20.CommonTypes.TaxRule_Id.TryParse((UInt32) TaxRuleIdNumber, out var TaxRuleId))
                {
                    ErrorResponse = $"Invalid tax rule identification '{TaxRuleIdNumber}'!";
                    return false;
                }

                CBOR.ParseOptionalText("taxRuleName",
                                       "tax rule name",
                                       out var TaxRuleName,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (!CBOR.ParseMandatory("taxRate",
                                         "tax rate",
                                         OCPPv2_1.RationalNumber.TryParseCBOR,
                                         out RationalNumber? TaxRate,
                                         out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalBoolean("taxIncludedInPrice",
                                          "tax included in price",
                                          out var TaxIncludedInPrice,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (!CBOR.ParseMandatoryBoolean("appliesToEnergyFee",
                                                "applies to the energy fee",
                                                out var AppliesToEnergyFee,
                                                out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryBoolean("appliesToParkingFee",
                                                "applies to the parking fee",
                                                out var AppliesToParkingFee,
                                                out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryBoolean("appliesToOverstayFee",
                                                "applies to the overstay fee",
                                                out var AppliesToOverstayFee,
                                                out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryBoolean("appliesToMinimumMaximumCost",
                                                "applies to the minimum and maximum cost",
                                                out var AppliesToMinimumMaximumCost,
                                                out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                TaxRule = new TaxRule(
                              TaxRuleId,
                              TaxRate,
                              AppliesToEnergyFee,
                              AppliesToParkingFee,
                              AppliesToOverstayFee,
                              AppliesToMinimumMaximumCost,
                              TaxRuleName,
                              TaxIncludedInPrice,
                              CustomData
                          );

                if (CustomTaxRuleParser is not null)
                    TaxRule = CustomTaxRuleParser(CBOR,
                                         TaxRule);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TaxRule  = default;
                ErrorResponse  = "The given CBOR representation of a tax rule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<TaxRule>.TryParse(CBOR, out TaxRule, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a tax rule - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<TaxRule>.TryParse(CBORValue                         CBOR,
                                                         out TaxRule                  Value,
                                                         [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomTaxRuleSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this tax rule: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomTaxRuleSerializer">A delegate to serialize custom tax rules.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<TaxRule>? CustomTaxRuleSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("taxRuleID",                    CBORValue.FromUInt64(TaxRuleId.Value)),
                           ("taxRuleName",                  OCPPCBORExtensions.Text(TaxRuleName)),
                           ("taxRate",                      TaxRate.ToCBOR()),
                           ("taxIncludedInPrice",           OCPPCBORExtensions.Flag(TaxIncludedInPrice)),
                           ("appliesToEnergyFee",           CBORValue.FromBoolean(AppliesToEnergyFee)),
                           ("appliesToParkingFee",          CBORValue.FromBoolean(AppliesToParkingFee)),
                           ("appliesToOverstayFee",         CBORValue.FromBoolean(AppliesToOverstayFee)),
                           ("appliesToMinimumMaximumCost",  CBORValue.FromBoolean(AppliesToMinimumMaximumCost)),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomTaxRuleSerializer is not null
                       ? CustomTaxRuleSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (TaxRule1, TaxRule2)

        /// <summary>
        /// Compares two tax rules for equality.
        /// </summary>
        /// <param name="TaxRule1">A tax rule.</param>
        /// <param name="TaxRule2">Another tax rule.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TaxRule? TaxRule1,
                                           TaxRule? TaxRule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TaxRule1, TaxRule2))
                return true;

            // If one is null, but not both, return false.
            if (TaxRule1 is null || TaxRule2 is null)
                return false;

            return TaxRule1.Equals(TaxRule2);

        }

        #endregion

        #region Operator != (TaxRule1, TaxRule2)

        /// <summary>
        /// Compares two tax rules for inequality.
        /// </summary>
        /// <param name="TaxRule1">A tax rule.</param>
        /// <param name="TaxRule2">Another tax rule.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TaxRule? TaxRule1,
                                           TaxRule? TaxRule2)

            => !(TaxRule1 == TaxRule2);

        #endregion

        #endregion

        #region IEquatable<TaxRule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tax rules for equality.
        /// </summary>
        /// <param name="Object">A tax rule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TaxRule taxRule &&
                   Equals(taxRule);

        #endregion

        #region Equals(TaxRule)

        /// <summary>
        /// Compares two tax rules for equality.
        /// </summary>
        /// <param name="TaxRule">A tax rule to compare with.</param>
        public Boolean Equals(TaxRule? TaxRule)

            => TaxRule is not null &&

               TaxRuleId.                  Equals(TaxRule.TaxRuleId)                   &&
               TaxRate.                    Equals(TaxRule.TaxRate)                     &&
               AppliesToEnergyFee.         Equals(TaxRule.AppliesToEnergyFee)          &&
               AppliesToParkingFee.        Equals(TaxRule.AppliesToParkingFee)         &&
               AppliesToOverstayFee.       Equals(TaxRule.AppliesToOverstayFee)        &&
               AppliesToMinimumMaximumCost.Equals(TaxRule.AppliesToMinimumMaximumCost) &&

             ((TaxRuleName        is     null &&  TaxRule.TaxRuleName        is     null) ||
              (TaxRuleName        is not null &&  TaxRule.TaxRuleName        is not null && TaxRuleName.             Equals(TaxRule.TaxRuleName))) &&

            ((!TaxIncludedInPrice.HasValue    && !TaxRule.TaxIncludedInPrice.HasValue) ||
              (TaxIncludedInPrice.HasValue    &&  TaxRule.TaxIncludedInPrice.HasValue    && TaxIncludedInPrice.Value.Equals(TaxRule.TaxIncludedInPrice.Value))) &&

               base.Equals(TaxRule);

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

                   TaxRuleName.IsNotNullOrEmpty()
                       ? TaxRuleName + " (" + TaxRuleId + ") "
                       : TaxRuleId,

                   TaxRate

               );

        #endregion

    }

}
