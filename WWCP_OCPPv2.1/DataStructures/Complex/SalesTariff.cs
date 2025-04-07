﻿/*
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A sales tariff.
    /// </summary>
    public class SalesTariff : ACustomData,
                               IEquatable<SalesTariff>
    {

        #region Properties

        /// <summary>
        /// The unique identifier used to identify one sales tariff.
        /// A sales tariff identification remains a unique identifier for one schedule throughout a charging session.
        /// </summary>
        [Mandatory]
        public SalesTariff_Id                 Id                    { get; }

        /// <summary>
        /// An enumeration of all relevant details for one time interval of the sales tariff.
        /// The number of sales tariff entries is limited by the parameter maxScheduleTuples.
        /// [max 1024]
        /// </summary>
        [Mandatory]
        public IEnumerable<SalesTariffEntry>  SalesTariffEntries    { get; }

        /// <summary>
        /// The optional human readable description of the sales tariff e.g. for HMI displays.
        /// [max 32]
        /// </summary>
        [Optional]
        public String?                        Description           { get; }

        /// <summary>
        /// The optional number of overall distinct price levels used across all provided sales tariff elements.
        /// </summary>
        [Optional]
        public UInt16?                        NumEPriceLevels       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new sales tariff.
        /// </summary>
        /// <param name="Id">The unique identifier used to identify one sales tariff. A sales tariff identification remains a unique identifier for one schedule throughout a charging session.</param>
        /// <param name="SalesTariffEntries">An enumeration of all relevant details for one time interval of the sales tariff. The number of sales tariff entries is limited by the parameter maxScheduleTuples [max 1024].</param>
        /// <param name="Description">An optional human readable description of the sales tariff e.g. for HMI displays [max 32].</param>
        /// <param name="NumEPriceLevels">An optional number of overall distinct price levels used across all provided sales tariff elements.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SalesTariff(SalesTariff_Id                 Id,
                           IEnumerable<SalesTariffEntry>  SalesTariffEntries,
                           String?                        Description,
                           UInt16?                        NumEPriceLevels,
                           CustomData?                    CustomData   = null)

            : base(CustomData)

        {

            if (!SalesTariffEntries.Any())
                throw new ArgumentException("The given enumeration of sales tariff entries must not be empty!",
                                            nameof(SalesTariffEntries));

            this.Id                  = Id;
            this.SalesTariffEntries  = SalesTariffEntries.Distinct();
            this.Description         = Description;
            this.NumEPriceLevels     = NumEPriceLevels;

        }

        #endregion


        #region Documentation

        // {
        //     "description": "A SalesTariff provided by a Mobility Operator (EMSP).
        //                     NOTE: This dataType is based on dataTypes from ISO 15118-2.",
        //     "javaType": "SalesTariff",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "id": {
        //             "description": "SalesTariff identifier used to identify one sales tariff. An SAID remains a unique identifier for one schedule throughout a charging session.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "salesTariffDescription": {
        //             "description": "A human readable title/short description of the sales tariff e.g. for HMI display purposes.",
        //             "type": "string",
        //             "maxLength": 32
        //         },
        //         "numEPriceLevels": {
        //             "description": "Defines the overall number of distinct price levels used across all provided SalesTariff elements.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "salesTariffEntry": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/SalesTariffEntryType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 1024
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id",
        //         "salesTariffEntry"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomSalesTariffParser = null)

        /// <summary>
        /// Parse the given JSON representation of a sales tariff.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSalesTariffParser">An optional delegate to parse custom sales tariffs.</param>
        public static SalesTariff Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<SalesTariff>?  CustomSalesTariffParser   = null)
        {

            if (TryParse(JSON,
                         out var salesTariff,
                         out var errorResponse,
                         CustomSalesTariffParser))
            {
                return salesTariff;
            }

            throw new ArgumentException("The given JSON representation of a sales tariff is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(SalesTariffJSON, out SalesTariff, out ErrorResponse, CustomSalesTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a sales tariff.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SalesTariff">The parsed connector type.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out SalesTariff?  SalesTariff,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out SalesTariff,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a sales tariff.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SalesTariff">The parsed connector type.</param>
        /// <param name="CustomSalesTariffParser">An optional delegate to parse custom sales tariffs.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out SalesTariff?      SalesTariff,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<SalesTariff>?  CustomSalesTariffParser   = null)
        {

            try
            {

                SalesTariff = default;

                #region SalesTariffId         [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "sales tariff identification",
                                         SalesTariff_Id.TryParse,
                                         out SalesTariff_Id SalesTariffId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SalesTariffEntries    [mandatory]

                if (!JSON.ParseMandatoryHashSet("salesTariffEntry",
                                                "sales tariff entries",
                                                SalesTariffEntry.TryParse,
                                                out HashSet<SalesTariffEntry> SalesTariffEntries,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Description           [optional]

                var Description = JSON.GetString("salesTariffDescription");

                #endregion

                #region NumEPriceLevels       [optional]

                if (JSON.ParseOptional("numEPriceLevels",
                                       "number of e price levels",
                                       out UInt16? NumEPriceLevels,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData            [optional]

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


                SalesTariff = new SalesTariff(
                                  SalesTariffId,
                                  SalesTariffEntries,
                                  Description,
                                  NumEPriceLevels,
                                  CustomData
                              );

                if (CustomSalesTariffParser is not null)
                    SalesTariff = CustomSalesTariffParser(JSON,
                                                          SalesTariff);

                return true;

            }
            catch (Exception e)
            {
                SalesTariff    = default;
                ErrorResponse  = "The given JSON representation of a sales tariff is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSalesTariffSerializer = null, CustomSalesTariffEntrySerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom salesTariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SalesTariff>?           CustomSalesTariffSerializer            = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?      CustomSalesTariffEntrySerializer       = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?  CustomRelativeTimeIntervalSerializer   = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?       CustomConsumptionCostSerializer        = null,
                              CustomJObjectSerializerDelegate<Cost>?                  CustomCostSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                       Id.Value),

                                 new JProperty("salesTariffEntry",         new JArray(SalesTariffEntries.Select(salesTariffEntry => salesTariffEntry.ToJSON(CustomSalesTariffEntrySerializer,
                                                                                                                                                            CustomRelativeTimeIntervalSerializer,
                                                                                                                                                            CustomConsumptionCostSerializer,
                                                                                                                                                            CustomCostSerializer,
                                                                                                                                                            CustomCustomDataSerializer)))),

                           Description is not null
                               ? new JProperty("salesTariffDescription",   Description)
                               : null,

                           NumEPriceLevels.HasValue
                               ? new JProperty("numEPriceLevels",          NumEPriceLevels.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSalesTariffSerializer is not null
                       ? CustomSalesTariffSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SalesTariff1, SalesTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariff1">A sales tariff.</param>
        /// <param name="SalesTariff2">Another sales tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SalesTariff? SalesTariff1,
                                           SalesTariff? SalesTariff2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SalesTariff1, SalesTariff2))
                return true;

            // If one is null, but not both, return false.
            if (SalesTariff1 is null || SalesTariff2 is null)
                return false;

            return SalesTariff1.Equals(SalesTariff2);

        }

        #endregion

        #region Operator != (SalesTariff1, SalesTariff2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariff1">A sales tariff.</param>
        /// <param name="SalesTariff2">Another sales tariff.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SalesTariff? SalesTariff1,
                                           SalesTariff? SalesTariff2)

            => !(SalesTariff1 == SalesTariff2);

        #endregion

        #endregion

        #region IEquatable<SalesTariff> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two sales tariffs for equality.
        /// </summary>
        /// <param name="Object">A sales tariff to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SalesTariff salesTariff &&
                   Equals(salesTariff);

        #endregion

        #region Equals(SalesTariff)

        /// <summary>
        /// Compares two sales tariffs for equality.
        /// </summary>
        /// <param name="SalesTariff">A sales tariff to compare with.</param>
        public Boolean Equals(SalesTariff? SalesTariff)

            => SalesTariff is not null &&

               Id.Equals(SalesTariff.Id) &&

               SalesTariffEntries.Count().Equals(SalesTariff.SalesTariffEntries.Count())       &&
               SalesTariffEntries.All(entry => SalesTariff.SalesTariffEntries.Contains(entry)) &&

             ((Description      is     null &&  SalesTariff.Description     is     null) ||
              (Description      is not null &&  SalesTariff.Description     is not null && Description.          Equals(SalesTariff.Description)))          &&

             ((!NumEPriceLevels.HasValue    && !SalesTariff.NumEPriceLevels.HasValue)    ||
                NumEPriceLevels.HasValue    &&  SalesTariff.NumEPriceLevels.HasValue    && NumEPriceLevels.Value.Equals(SalesTariff.NumEPriceLevels.Value)) &&

               base.Equals(SalesTariff);

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

                return Id.                GetHashCode()       * 11 ^
                       SalesTariffEntries.CalcHashCode()      *  7 ^
                       (Description?.     GetHashCode() ?? 0) *  5 ^
                       (NumEPriceLevels?. GetHashCode() ?? 0) *  3 ^

                       base.              GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                    Id,
                    ", ",

                    SalesTariffEntries.Count(),
                    " sales tariff entries",

                    Description is not null
                        ? ", description: " + Description
                        : "",

                    NumEPriceLevels.HasValue
                        ? ", num e price levels: " + NumEPriceLevels.Value
                        : ""

               );

        #endregion

    }

}
