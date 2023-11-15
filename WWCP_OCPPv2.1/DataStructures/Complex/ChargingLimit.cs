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
    /// A charging limit.
    /// </summary>
    public class ChargingLimit : ACustomData,
                                 IEquatable<ChargingLimit>
    {

        #region Properties

        /// <summary>
        /// The source of the charging limit.
        /// </summary>
        [Mandatory]
        public ChargingLimitSource  ChargingLimitSource    { get; }

        /// <summary>
        /// Optional indication whether the charging limit is critical for the grid.
        /// </summary>
        [Optional]
        public Boolean?             IsGridCritical         { get; }

        /// <summary>
        /// Optional indication whether the reported limit concerns local generation
        /// that is provides extra capacity, instead of a limitation.
        /// </summary>
        [Optional]
        public Boolean?             IsLocalGeneration      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging limit.
        /// </summary>
        /// <param name="ChargingLimitSource">The source of the charging limit.</param>
        /// <param name="IsGridCritical">Optional indication whether the charging limit is critical for the grid.</param>
        /// <param name="IsLocalGeneration">Optional indication whether the reported limit concerns local generation that is provides extra capacity, instead of a limitation.</param>
        public ChargingLimit(ChargingLimitSource  ChargingLimitSource,
                             Boolean?             IsGridCritical      = null,
                             Boolean?             IsLocalGeneration   = null,
                             CustomData?          CustomData          = null)

            : base(CustomData)

        {

            this.ChargingLimitSource  = ChargingLimitSource;
            this.IsGridCritical       = IsGridCritical;
            this.IsLocalGeneration    = IsLocalGeneration;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // "ChargingLimitType": {
        //   "description": "Charging_ Limit\r\nurn:x-enexis:ecdm:uid:2:234489\r\n",
        //   "javaType": "ChargingLimit",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "chargingLimitSource": {
        //       "$ref": "#/definitions/ChargingLimitSourceEnumType"
        //     },
        //     "isGridCritical": {
        //       "description": "Charging_ Limit. Is_ Grid_ Critical. Indicator\r\nurn:x-enexis:ecdm:uid:1:570847\r\nIndicates whether the charging limit is critical for the grid.\r\n",
        //       "type": "boolean"
        //     }
        //   },
        //   "required": [
        //     "chargingLimitSource"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomChargingLimitParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingLimitParser">A delegate to parse custom charging limit JSON objects.</param>
        public static ChargingLimit Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<ChargingLimit>?  CustomChargingLimitParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingLimit,
                         out var errorResponse,
                         CustomChargingLimitParser) &&
                chargingLimit is not null)
            {
                return chargingLimit;
            }

            throw new ArgumentException("The given JSON representation of a charging limit is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingLimit, out ErrorResponse, CustomChargingLimitParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingLimit">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out ChargingLimit?  ChargingLimit,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out ChargingLimit,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging limit.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingLimit">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingLimitParser">A delegate to parse custom charging limit JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out ChargingLimit?                           ChargingLimit,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingLimit>?  CustomChargingLimitParser)
        {

            try
            {

                ChargingLimit = default;

                #region ChargingLimitSource    [mandatory]

                if (!JSON.ParseMandatory("chargingLimitSource",
                                         "charging limit source",
                                         OCPPv2_1.ChargingLimitSource.TryParse,
                                         out ChargingLimitSource ChargingLimitSource,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IsGridCritical         [optional]

                if (JSON.ParseOptional("isGridCritical",
                                       "is grid critical",
                                       out Boolean? IsGridCritical,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion

                #region IsLocalGeneration      [optional]

                if (JSON.ParseOptional("isLocalGeneration",
                                       "is local generation",
                                       out Boolean? IsLocalGeneration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData             [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingLimit = new ChargingLimit(
                                    ChargingLimitSource,
                                    IsGridCritical,
                                    IsLocalGeneration,
                                    CustomData
                                );

                if (CustomChargingLimitParser is not null)
                    ChargingLimit = CustomChargingLimitParser(JSON,
                                                              ChargingLimit);

                return true;

            }
            catch (Exception e)
            {
                ChargingLimit  = default;
                ErrorResponse  = "The given JSON representation of a charging limit is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomChargingLimitSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingLimitSerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingLimit>?  CustomChargingLimitSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?     CustomCustomDataSerializer      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingLimitSource",   ChargingLimitSource.ToString()),

                           IsGridCritical.HasValue
                               ? new JProperty("isGridCritical",        IsGridCritical)
                               : null,

                           IsLocalGeneration.HasValue
                               ? new JProperty("isLocalGeneration",     IsLocalGeneration)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChargingLimitSerializer is not null
                       ? CustomChargingLimitSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingLimit1, ChargingLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimit1">A charging limit.</param>
        /// <param name="ChargingLimit2">Another charging limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingLimit ChargingLimit1,
                                           ChargingLimit ChargingLimit2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingLimit1, ChargingLimit2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingLimit1 is null || ChargingLimit2 is null)
                return false;

            return ChargingLimit1.Equals(ChargingLimit2);

        }

        #endregion

        #region Operator != (ChargingLimit1, ChargingLimit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimit1">A charging limit.</param>
        /// <param name="ChargingLimit2">Another charging limit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingLimit ChargingLimit1,
                                           ChargingLimit ChargingLimit2)

            => !(ChargingLimit1 == ChargingLimit2);

        #endregion

        #endregion

        #region IEquatable<ChargingLimit> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging limits for equality..
        /// </summary>
        /// <param name="Object">A charging limit to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingLimit chargingLimit &&
                   Equals(chargingLimit);

        #endregion

        #region Equals(ChargingLimit)

        /// <summary>
        /// Compares two charging limits for equality.
        /// </summary>
        /// <param name="ChargingLimit">A charging limit to compare with.</param>
        public Boolean Equals(ChargingLimit? ChargingLimit)

            => ChargingLimit is not null &&

               ChargingLimitSource.Equals(ChargingLimit.ChargingLimitSource) &&

            ((!IsGridCritical.   HasValue && !ChargingLimit.IsGridCritical.   HasValue) ||
              (IsGridCritical.   HasValue &&  ChargingLimit.IsGridCritical.   HasValue && IsGridCritical.   Value.Equals(ChargingLimit.IsGridCritical.   Value))) &&

            ((!IsLocalGeneration.HasValue && !ChargingLimit.IsLocalGeneration.HasValue) ||
              (IsLocalGeneration.HasValue &&  ChargingLimit.IsLocalGeneration.HasValue && IsLocalGeneration.Value.Equals(ChargingLimit.IsLocalGeneration.Value))) &&

               base.Equals(ChargingLimit);

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

                return ChargingLimitSource.GetHashCode()       * 7 ^
                      (IsGridCritical?.    GetHashCode() ?? 0) * 5 ^
                      (IsLocalGeneration?. GetHashCode() ?? 0) * 3 ^

                       base.               GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   ChargingLimitSource,

                   IsGridCritical.HasValue
                       ? IsGridCritical.Value
                             ? ", grid critical"
                             : ""
                       : "",

                   IsLocalGeneration.HasValue
                       ? IsLocalGeneration.Value
                             ? ", local generation"
                             : ""
                       : ""

               );

        #endregion

    }

}
