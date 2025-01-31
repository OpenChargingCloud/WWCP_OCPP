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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A sales tariff entry.
    /// </summary>
    public class SalesTariffEntry : ACustomData,
                                    IEquatable<SalesTariffEntry>
    {

        #region Properties

        /// <summary>
        /// The time interval the sales tariff entry is valid, based upon relative times.
        /// </summary>
        [Mandatory]
        public RelativeTimeInterval          RelativeTimeInterval    { get; }

        /// <summary>
        /// The optional price level of this sales tariff entry (referring to NumEPriceLevels).
        /// Small values for the EPriceLevel represent a cheaper TariffEntry.
        /// Large values for the EPriceLevel represent a more expensive sales tariff entry.
        /// </summary>
        [Optional]
        public UInt32?                       EPriceLevel             { get; }

        /// <summary>
        /// Optional additional means for further relative price information and/or alternative costs.
        /// </summary>
        [Optional]
        public IEnumerable<ConsumptionCost>  ConsumptionCosts        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new sales tariff entry.
        /// </summary>
        /// <param name="RelativeTimeInterval">The time interval the sales tariff entry is valid, based upon relative times.</param>
        /// <param name="EPriceLevel"></param>
        /// <param name="ConsumptionCosts">Optional additional means for further relative price information and/or alternative costs.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SalesTariffEntry(RelativeTimeInterval           RelativeTimeInterval,
                                UInt32?                        EPriceLevel        = null,
                                IEnumerable<ConsumptionCost>?  ConsumptionCosts   = null,
                                CustomData?                    CustomData         = null)

            : base(CustomData)

        {

            this.RelativeTimeInterval  = RelativeTimeInterval;
            this.EPriceLevel           = EPriceLevel;
            this.ConsumptionCosts      = ConsumptionCosts?.Distinct() ?? Array.Empty<ConsumptionCost>();

        }

        #endregion


        #region Documentation

        // "SalesTariffEntryType": {
        //   "description": "Sales_ Tariff_ Entry\r\nurn:x-oca:ocpp:uid:2:233271",
        //   "javaType": "SalesTariffEntry",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "relativeTimeInterval": {
        //       "$ref": "#/definitions/RelativeTimeIntervalType"
        //     },
        //     "ePriceLevel": {
        //       "description": "Sales_ Tariff_ Entry. E_ Price_ Level. Unsigned_ Integer\r\nurn:x-oca:ocpp:uid:1:569281\r\nDefines the price level of this SalesTariffEntry (referring to NumEPriceLevels). Small values for the EPriceLevel represent a cheaper TariffEntry. Large values for the EPriceLevel represent a more expensive TariffEntry.",
        //       "type": "integer",
        //       "minimum": 0.0
        //     },
        //     "consumptionCost": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ConsumptionCostType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 3
        //     }
        //   },
        //   "required": [
        //     "relativeTimeInterval"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomSalesTariffEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of a sales tariff entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSalesTariffEntryParser">An optional delegate to parse custom sales tariff entries.</param>
        public static SalesTariffEntry Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<SalesTariffEntry>?  CustomSalesTariffEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var salesTariffEntry,
                         out var errorResponse,
                         CustomSalesTariffEntryParser) &&
                salesTariffEntry is not null)
            {
                return salesTariffEntry;
            }

            throw new ArgumentException("The given JSON representation of a sales tariff entry is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(SalesTariffEntryJSON, out SalesTariffEntry, out ErrorResponse, CustomSalesTariffEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a sales tariff entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SalesTariffEntry">The parsed connector type.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out SalesTariffEntry?  SalesTariffEntry,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out SalesTariffEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a sales tariff entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SalesTariffEntry">The parsed connector type.</param>
        /// <param name="CustomSalesTariffEntryParser">An optional delegate to parse custom sales tariff entries.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out SalesTariffEntry?                           SalesTariffEntry,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<SalesTariffEntry>?  CustomSalesTariffEntryParser   = null)
        {

            try
            {

                SalesTariffEntry = default;

                #region RelativeTimeInterval    [mandatory]

                if (!JSON.ParseMandatoryJSON("relativeTimeInterval",
                                             "relative time interval",
                                             OCPPv2_1.RelativeTimeInterval.TryParse,
                                             out RelativeTimeInterval? RelativeTimeInterval,
                                             out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (RelativeTimeInterval is null)
                    return false;

                #endregion

                #region EPriceLevel             [optional]

                if (JSON.ParseOptional("ePriceLevel",
                                       "e price level",
                                       out UInt32? EPriceLevel,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ConsumptionCosts        [optional]

                if (JSON.ParseOptionalHashSet("consumptionCost",
                                              "consumption cost",
                                              ConsumptionCost.TryParse,
                                              out HashSet<ConsumptionCost> ConsumptionCosts,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData              [optional]

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


                SalesTariffEntry = new SalesTariffEntry(RelativeTimeInterval,
                                                        EPriceLevel,
                                                        ConsumptionCosts,
                                                        CustomData);

                if (CustomSalesTariffEntryParser is not null)
                    SalesTariffEntry = CustomSalesTariffEntryParser(JSON,
                                                                    SalesTariffEntry);

                return true;

            }
            catch (Exception e)
            {
                SalesTariffEntry  = default;
                ErrorResponse     = "The given JSON representation of a sales tariff entry is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSalesTariffEntrySerializer = null, CustomRelativeTimeIntervalSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SalesTariffEntry>?      CustomSalesTariffEntrySerializer       = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?  CustomRelativeTimeIntervalSerializer   = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?       CustomConsumptionCostSerializer        = null,
                              CustomJObjectSerializerDelegate<Cost>?                  CustomCostSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("relativeTimeInterval",   RelativeTimeInterval.ToJSON(CustomRelativeTimeIntervalSerializer,
                                                                                                    CustomCustomDataSerializer)),

                           EPriceLevel is not null
                               ? new JProperty("ePriceLevel",            EPriceLevel.Value)
                               : null,

                           ConsumptionCosts.Any()
                               ? new JProperty("consumptionCost",        new JArray(ConsumptionCosts.Select(consumptionCost => consumptionCost.ToJSON(CustomConsumptionCostSerializer,
                                                                                                                                                      CustomCostSerializer,
                                                                                                                                                      CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",             CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSalesTariffEntrySerializer is not null
                       ? CustomSalesTariffEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SalesTariffEntry1, SalesTariffEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffEntry1">A sales tariff entry.</param>
        /// <param name="SalesTariffEntry2">Another sales tariff entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SalesTariffEntry? SalesTariffEntry1,
                                           SalesTariffEntry? SalesTariffEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SalesTariffEntry1, SalesTariffEntry2))
                return true;

            // If one is null, but not both, return false.
            if (SalesTariffEntry1 is null || SalesTariffEntry2 is null)
                return false;

            return SalesTariffEntry1.Equals(SalesTariffEntry2);

        }

        #endregion

        #region Operator != (SalesTariffEntry1, SalesTariffEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SalesTariffEntry1">A sales tariff entry.</param>
        /// <param name="SalesTariffEntry2">Another sales tariff entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SalesTariffEntry? SalesTariffEntry1,
                                           SalesTariffEntry? SalesTariffEntry2)

            => !(SalesTariffEntry1 == SalesTariffEntry2);

        #endregion

        #endregion

        #region IEquatable<SalesTariffEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two sales tariff entries for equality.
        /// </summary>
        /// <param name="Object">A sales tariff entry to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SalesTariffEntry salesTariffEntry &&
                   Equals(salesTariffEntry);

        #endregion

        #region Equals(SalesTariffEntry)

        /// <summary>
        /// Compares two sales tariff entries for equality.
        /// </summary>
        /// <param name="SalesTariffEntry">A sales tariff entry to compare with.</param>
        public Boolean Equals(SalesTariffEntry? SalesTariffEntry)

            => SalesTariffEntry is not null &&

               RelativeTimeInterval.Equals(SalesTariffEntry.RelativeTimeInterval) &&

             ((!EPriceLevel.HasValue && !SalesTariffEntry.EPriceLevel.HasValue) ||
                EPriceLevel.HasValue &&  SalesTariffEntry.EPriceLevel.HasValue && EPriceLevel.Value.Equals(SalesTariffEntry.EPriceLevel.Value)) &&

               ConsumptionCosts.Count().Equals(SalesTariffEntry.ConsumptionCosts.Count())     &&
               ConsumptionCosts.All(data => SalesTariffEntry.ConsumptionCosts.Contains(data)) &&

               base.Equals(SalesTariffEntry);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return RelativeTimeInterval.GetHashCode()       * 7 ^
                      (EPriceLevel?.        GetHashCode() ?? 0) * 5 ^
                       ConsumptionCosts.    CalcHashCode()      * 3 ^

                       base.                GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   RelativeTimeInterval.ToString(),

                   EPriceLevel.HasValue
                       ? ", e price level: " + EPriceLevel.Value
                       : ""

               );

        #endregion

    }

}
