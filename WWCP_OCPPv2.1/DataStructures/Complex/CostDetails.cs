/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// The cost of a transaction as the charging station calculated it from
    /// its tariff: its total cost and usage, and its charging periods.
    /// </summary>
    /// <remarks>
    /// A reservation is no charging period: it took place outside of the transaction.
    /// </remarks>
    public class CostDetails : IEquatable<CostDetails>,
                               ICBORSerializable<CostDetails>
    {

        #region Properties

        /// <summary>
        /// The total cost.
        /// </summary>
        [Mandatory]
        public TotalCost                    TotalCost             { get; }

        /// <summary>
        /// The total usage.
        /// </summary>
        [Mandatory]
        public TotalUsage                   TotalUsage            { get; }

        /// <summary>
        /// The charging periods.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingPeriod>  ChargingPeriods       { get; }

        /// <summary>
        /// Whether the charging station failed to calculate the cost.
        /// </summary>
        [Optional]
        public Boolean?                     FailureToCalculate    { get; }

        /// <summary>
        /// An optional human-readable reason why the cost could not be calculated.
        /// </summary>
        [Optional]
        public String?                      FailureReason         { get; }

        /// <summary>
        /// An optional custom data object allowing to store any kind of customer specific data.
        /// </summary>
        [Optional]
        public CustomData?                  CustomData            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new cost details of a transaction.
        /// </summary>
        /// <param name="TotalCost">The total cost.</param>
        /// <param name="TotalUsage">The total usage.</param>
        /// <param name="ChargingPeriods">The charging periods.</param>
        /// <param name="FailureToCalculate">Whether the charging station failed to calculate the cost.</param>
        /// <param name="FailureReason">An optional human-readable reason why the cost could not be calculated.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public CostDetails(TotalCost                     TotalCost,
                           TotalUsage                    TotalUsage,
                           IEnumerable<ChargingPeriod>?  ChargingPeriods      = null,
                           Boolean?                      FailureToCalculate   = null,
                           String?                       FailureReason        = null,
                           CustomData?                   CustomData           = null)
        {

            this.TotalCost           = TotalCost;
            this.TotalUsage          = TotalUsage;
            this.ChargingPeriods     = ChargingPeriods?.ToArray() ?? [];
            this.FailureToCalculate  = FailureToCalculate;
            this.FailureReason       = FailureReason;
            this.CustomData          = CustomData;

            unchecked
            {
                hashCode = this.TotalCost.          GetHashCode()       * 13 ^
                           this.TotalUsage.         GetHashCode()       * 11 ^
                           this.ChargingPeriods.    CalcHashCode()      *  7 ^
                          (this.FailureToCalculate?.GetHashCode() ?? 0) *  5 ^
                          (this.FailureReason?.     GetHashCode() ?? 0) *  3 ^
                          (this.CustomData?.        GetHashCode() ?? 0);
            }

        }

        #endregion


        #region Documentation

        // "CostDetailsType": {
        //   "description": "CostDetailsType contains the cost as calculated by Charging Station based on provided TariffType.
        //                   NOTE: Reservation is not shown as a _chargingPeriod_, because it took place outside of the transaction.",
        //   "javaType": "CostDetails",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "chargingPeriods":    { "type": "array", "additionalItems": false, "items": { "$ref": "#/definitions/ChargingPeriodType" }, "minItems": 1 },
        //     "totalCost":          { "$ref": "#/definitions/TotalCostType" },
        //     "totalUsage":         { "$ref": "#/definitions/TotalUsageType" },
        //     "failureToCalculate": { "description": "If set to true, then Charging Station has failed to calculate the cost.", "type": "boolean" },
        //     "failureReason":      { "description": "Optional human-readable reason text in case of failure to calculate.", "type": "string", "maxLength": 500 },
        //     "customData":         { "$ref": "#/definitions/CustomDataType" }
        //   },
        //   "required": [ "totalCost", "totalUsage" ]
        // }

        #endregion

        #region (static) TryParse    (JSON, out CostDetails, out ErrorResponse, CustomCostDetailsParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of cost details.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CostDetails">The parsed cost details.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out CostDetails?       CostDetails,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out CostDetails,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of cost details.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CostDetails">The parsed cost details.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostDetailsParser">An optional delegate to parse custom cost details.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out CostDetails?       CostDetails,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<CostDetails>?   CustomCostDetailsParser)
        {

            try
            {

                CostDetails = null;

                if (!JSON.ParseMandatoryJSON("totalCost",
                                             "total cost",
                                             OCPPv2_1.TotalCost.TryParse,
                                             out TotalCost? TotalCost,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!JSON.ParseMandatoryJSON("totalUsage",
                                             "total usage",
                                             OCPPv2_1.TotalUsage.TryParse,
                                             out TotalUsage? TotalUsage,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (JSON.ParseOptionalJSON("chargingPeriods",
                                           "charging periods",
                                           ChargingPeriod.TryParse,
                                           out IEnumerable<ChargingPeriod> ChargingPeriods,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (JSON.ParseOptional("failureToCalculate",
                                       "failure to calculate",
                                       out Boolean? FailureToCalculate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var FailureReason = JSON["failureReason"]?.Value<String>();

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }


                CostDetails = new CostDetails(
                                  TotalCost,
                                  TotalUsage,
                                  ChargingPeriods,
                                  FailureToCalculate,
                                  FailureReason,
                                  CustomData
                              );

                if (CustomCostDetailsParser is not null)
                    CostDetails = CustomCostDetailsParser(JSON,
                                                          CostDetails);

                return true;

            }
            catch (Exception e)
            {
                CostDetails    = null;
                ErrorResponse  = "The given JSON representation of cost details is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCostDetailsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of these cost details.
        /// </summary>
        /// <param name="CustomCostDetailsSerializer">A delegate to serialize custom cost details.</param>
        /// <param name="CustomTotalCostSerializer">A delegate to serialize custom total costs.</param>
        /// <param name="CustomTotalUsageSerializer">A delegate to serialize custom total usages.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging periods.</param>
        /// <param name="CustomCostDimensionSerializer">A delegate to serialize custom volumes of cost dimensions.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom prices.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rates.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CostDetails>?     CustomCostDetailsSerializer      = null,
                              CustomJObjectSerializerDelegate<TotalCost>?       CustomTotalCostSerializer        = null,
                              CustomJObjectSerializerDelegate<TotalUsage>?      CustomTotalUsageSerializer       = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?  CustomChargingPeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CostDimension>?   CustomCostDimensionSerializer    = null,
                              CustomJObjectSerializerDelegate<Price>?           CustomPriceSerializer            = null,
                              CustomJObjectSerializerDelegate<TaxRate>?         CustomTaxRateSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                           ChargingPeriods.Any()
                               ? new JProperty("chargingPeriods",      new JArray(ChargingPeriods.Select(chargingPeriod => chargingPeriod.ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                 CustomCostDimensionSerializer,
                                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                                 new JProperty("totalCost",            TotalCost. ToJSON(CustomTotalCostSerializer,
                                                                                         CustomPriceSerializer,
                                                                                         CustomTaxRateSerializer)),

                                 new JProperty("totalUsage",           TotalUsage.ToJSON(CustomTotalUsageSerializer,
                                                                                         CustomCustomDataSerializer)),

                           FailureToCalculate.HasValue
                               ? new JProperty("failureToCalculate",   FailureToCalculate.Value)
                               : null,

                           FailureReason is not null
                               ? new JProperty("failureReason",        FailureReason)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCostDetailsSerializer is not null
                       ? CustomCostDetailsSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out CostDetails, out ErrorResponse, CustomCostDetailsParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of cost details.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="CostDetails">The cost details.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                           [NotNullWhen(true)]  out CostDetails?      CostDetails,
                                           [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out CostDetails,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of cost details.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="CostDetails">The cost details.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostDetailsParser">An optional delegate to read custom cost details.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                           [NotNullWhen(true)]  out CostDetails?      CostDetails,
                                           [NotNullWhen(false)] out String?           ErrorResponse,
                                           CustomCBORParserDelegate<CostDetails>?     CustomCostDetailsParser)
        {

            try
            {

                CostDetails = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of cost details is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatory("totalCost",
                                         "total cost",
                                         OCPPv2_1.TotalCost.TryParseCBOR,
                                         out TotalCost? TotalCost,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatory("totalUsage",
                                         "total usage",
                                         OCPPv2_1.TotalUsage.TryParseCBOR,
                                         out TotalUsage? TotalUsage,
                                         out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalList<ChargingPeriod>("chargingPeriods",
                                                       "charging periods",
                                                       ChargingPeriod.TryParseCBOR,
                                                       out var ChargingPeriods,
                                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("failureToCalculate",
                                          "failure to calculate",
                                          out var FailureToCalculate,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("failureReason",
                                       "failure reason",
                                       out var FailureReason,
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


                CostDetails = new CostDetails(
                                  TotalCost!,
                                  TotalUsage!,
                                  ChargingPeriods,
                                  FailureToCalculate,
                                  FailureReason,
                                  CustomData
                              );

                if (CustomCostDetailsParser is not null)
                    CostDetails = CustomCostDetailsParser(CBOR,
                                                          CostDetails);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                CostDetails    = null;
                ErrorResponse  = "The given CBOR representation of cost details is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<CostDetails>.TryParse(CBOR, out CostDetails, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of cost details - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<CostDetails>.TryParse(CBORValue                         CBOR,
                                                               out CostDetails                   Value,
                                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomCostDetailsSerializer = null)

        /// <summary>
        /// Return the CBOR representation of these cost details: the keys of their JSON object.
        /// </summary>
        /// <param name="CustomCostDetailsSerializer">A delegate to serialize custom cost details.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<CostDetails>? CustomCostDetailsSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("chargingPeriods",     OCPPCBORExtensions.Array(ChargingPeriods, chargingPeriod => chargingPeriod.ToCBOR())),
                           ("totalCost",           TotalCost. ToCBOR()),
                           ("totalUsage",          TotalUsage.ToCBOR()),
                           ("failureToCalculate",  OCPPCBORExtensions.Flag(FailureToCalculate)),
                           ("failureReason",       OCPPCBORExtensions.Text(FailureReason)),
                           ("customData",          CustomData?.ToCBOR())
                       );

            return CustomCostDetailsSerializer is not null
                       ? CustomCostDetailsSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region IEquatable<CostDetails> Members

        /// <summary>
        /// Compares two cost details for equality.
        /// </summary>
        /// <param name="Object">Cost details to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CostDetails costDetails &&
                   Equals(costDetails);


        /// <summary>
        /// Compares two cost details for equality.
        /// </summary>
        /// <param name="CostDetails">Cost details to compare with.</param>
        public Boolean Equals(CostDetails? CostDetails)

            => CostDetails is not null &&

               TotalCost. Equals(CostDetails.TotalCost)  &&
               TotalUsage.Equals(CostDetails.TotalUsage) &&
               ChargingPeriods.SequenceEqual(CostDetails.ChargingPeriods) &&
               Nullable.Equals(FailureToCalculate, CostDetails.FailureToCalculate) &&
               String.  Equals(FailureReason,      CostDetails.FailureReason) &&

             ((CustomData is null     && CostDetails.CustomData is null) ||
              (CustomData is not null && CustomData.Equals(CostDetails.CustomData)));

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

            => $"{TotalCost}, {TotalUsage}, {ChargingPeriods.Count()} charging period(s){(FailureToCalculate == true ? $", failed: {FailureReason}" : "")}";

        #endregion

    }

}
