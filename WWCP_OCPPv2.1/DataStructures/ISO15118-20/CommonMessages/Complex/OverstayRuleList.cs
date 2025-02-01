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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The overstay rule list.
    /// </summary>
    public class OverstayRuleList : IEquatable<OverstayRuleList>
    {

        #region Properties

        /// <summary>
        /// The enumeration of overstay rules.
        /// [max 5]
        /// </summary>
        [Mandatory]
        public IEnumerable<OverstayRule>  OverstayRules             { get; }

        /// <summary>
        /// The overstay time threshold.
        /// </summary>
        [Optional]
        public TimeSpan?                  OverstayTimeThreshold     { get; }

        /// <summary>
        /// The overstay power threshold.
        /// </summary>
        [Optional]
        public RationalNumber?            OverstayPowerThreshold    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new overstay rule list.
        /// </summary>
        /// <param name="OverstayRules">An enumeration of overstay rules.</param>
        /// <param name="OverstayTimeThreshold">An overstay time threshold.</param>
        /// <param name="OverstayPowerThreshold">An overstay power threshold.</param>
        public OverstayRuleList(IEnumerable<OverstayRule>  OverstayRules,
                                TimeSpan?                  OverstayTimeThreshold    = null,
                                RationalNumber?            OverstayPowerThreshold   = null)
        {

            this.OverstayRules           = OverstayRules.Distinct();
            this.OverstayTimeThreshold   = OverstayTimeThreshold;
            this.OverstayPowerThreshold  = OverstayPowerThreshold;

            unchecked
            {

                hashCode = this.OverstayRules.          CalcHashCode()      * 7 ^
                          (this.OverstayTimeThreshold?. GetHashCode() ?? 0) * 5 ^
                          (this.OverstayPowerThreshold?.GetHashCode() ?? 0) * 3 ^
                           base.                        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Part of ISO 15118-20 price schedule.\r\n\r\n",
        //     "javaType": "OverstayRuleList",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "overstayPowerThreshold": {
        //             "$ref": "#/definitions/RationalNumberType"
        //         },
        //         "overstayRule": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/OverstayRuleType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 5
        //         },
        //         "overstayTimeThreshold": {
        //             "description": "Time till overstay is applied in seconds.\r\n",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "overstayRule"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomOverstayRuleListParser = null)

        /// <summary>
        /// Parse the given JSON representation of an overstay rule list.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomOverstayRuleListParser">An optional delegate to parse custom overstay rule lists.</param>
        public static OverstayRuleList Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<OverstayRuleList>?  CustomOverstayRuleListParser   = null)
        {

            if (TryParse(JSON,
                         out var overstayRuleList,
                         out var errorResponse,
                         CustomOverstayRuleListParser) &&
                overstayRuleList is not null)
            {
                return overstayRuleList;
            }

            throw new ArgumentException("The given JSON representation of an overstay rule list is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out OverstayRuleList, out ErrorResponse, CustomOverstayRuleListParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an overstay rule list.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OverstayRuleList">The parsed overstay rule list.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out OverstayRuleList?  OverstayRuleList,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out OverstayRuleList,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an overstay rule list.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OverstayRuleList">The parsed overstay rule list.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOverstayRuleListParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out OverstayRuleList?      OverstayRuleList,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<OverstayRuleList>?  CustomOverstayRuleListParser)
        {

            try
            {

                OverstayRuleList = null;

                #region OverstayRules             [mandatory]

                if (!JSON.ParseMandatoryHashSet("overstayRule",
                                                "sub certificates",
                                                OverstayRule.TryParse,
                                                out HashSet<OverstayRule> OverstayRules,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region OverstayTimeThreshold     [optional]

                if (JSON.ParseOptional("overstayTimeThreshold",
                                       "overstay time threshold",
                                       out TimeSpan? OverstayTimeThreshold,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region OverstayPowerThreshold    [optional]

                if (JSON.ParseOptionalJSON("overstayPowerThreshold",
                                           "overstay power threshold",
                                           RationalNumber.TryParse,
                                           out RationalNumber? OverstayPowerThreshold,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                OverstayRuleList = new OverstayRuleList(
                                       OverstayRules,
                                       OverstayTimeThreshold,
                                       OverstayPowerThreshold
                                   );

                if (CustomOverstayRuleListParser is not null)
                    OverstayRuleList = CustomOverstayRuleListParser(JSON,
                                                                    OverstayRuleList);

                return true;

            }
            catch (Exception e)
            {
                OverstayRuleList  = null;
                ErrorResponse     = "The given JSON representation of an overstay rule list is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomOverstayRuleListSerializer = null, CustomOverstayRuleSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOverstayRuleListSerializer">A delegate to serialize custom overstay rule lists.</param>
        /// <param name="CustomOverstayRuleSerializer">A delegate to serialize custom overstay rules.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OverstayRuleList>? CustomOverstayRuleListSerializer   = null,
                              CustomJObjectSerializerDelegate<OverstayRule>?     CustomOverstayRuleSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("overstayRule",             new JArray(OverstayRules.Select(overstayRule => overstayRule.ToJSON(CustomOverstayRuleSerializer)))),

                           OverstayTimeThreshold.HasValue
                               ? new JProperty("overstayTimeThreshold",    (UInt64) Math.Round(OverstayTimeThreshold.Value.TotalSeconds, 0))
                               : null,

                           OverstayPowerThreshold is not null
                               ? new JProperty("overstayPowerThreshold",   OverstayPowerThreshold.ToJSON())
                               : null

                       );

            return CustomOverstayRuleListSerializer is not null
                       ? CustomOverstayRuleListSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (OverstayRuleList1, OverstayRuleList2)

        /// <summary>
        /// Compares two overstay rule lists for equality.
        /// </summary>
        /// <param name="OverstayRuleList1">An overstay rule list.</param>
        /// <param name="OverstayRuleList2">Another overstay rule list.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (OverstayRuleList? OverstayRuleList1,
                                           OverstayRuleList? OverstayRuleList2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OverstayRuleList1, OverstayRuleList2))
                return true;

            // If one is null, but not both, return false.
            if (OverstayRuleList1 is null || OverstayRuleList2 is null)
                return false;

            return OverstayRuleList1.Equals(OverstayRuleList2);

        }

        #endregion

        #region Operator != (OverstayRuleList1, OverstayRuleList2)

        /// <summary>
        /// Compares two overstay rule lists for inequality.
        /// </summary>
        /// <param name="OverstayRuleList1">An overstay rule list.</param>
        /// <param name="OverstayRuleList2">Another overstay rule list.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (OverstayRuleList? OverstayRuleList1,
                                           OverstayRuleList? OverstayRuleList2)

            => !(OverstayRuleList1 == OverstayRuleList2);

        #endregion

        #endregion

        #region IEquatable<OverstayRuleList> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two overstay rule lists for equality.
        /// </summary>
        /// <param name="Object">An overstay rule list to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OverstayRuleList overstayRuleList &&
                   Equals(overstayRuleList);

        #endregion

        #region Equals(OverstayRuleList)

        /// <summary>
        /// Compares two overstay rule lists for equality.
        /// </summary>
        /// <param name="OverstayRuleList">An overstay rule list to compare with.</param>
        public Boolean Equals(OverstayRuleList? OverstayRuleList)

            => OverstayRuleList is not null &&

               OverstayRules.ToHashSet().SetEquals(OverstayRuleList.OverstayRules) &&

            ((!OverstayTimeThreshold. HasValue    && !OverstayRuleList.OverstayTimeThreshold. HasValue)    ||
              (OverstayTimeThreshold. HasValue    &&  OverstayRuleList.OverstayTimeThreshold. HasValue    && OverstayTimeThreshold.Value.Equals(OverstayRuleList.OverstayTimeThreshold.Value))) &&

             ((OverstayPowerThreshold is     null &&  OverstayRuleList.OverstayPowerThreshold is     null) ||
              (OverstayPowerThreshold is not null &&  OverstayRuleList.OverstayPowerThreshold is not null && OverstayPowerThreshold.     Equals(OverstayRuleList.OverstayPowerThreshold)));

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

                   $"{OverstayRules.Count()} overstay rule(s)",

                   OverstayTimeThreshold.HasValue
                       ? $", time threshold: {OverstayTimeThreshold.Value.TotalSeconds} second(s)"
                       : null,

                   OverstayPowerThreshold is not null
                       ? $", power threshold: {OverstayPowerThreshold} kW"
                       : null

               );

        #endregion

    }

}
