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

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The overstay rule.
    /// </summary>
    public class OverstayRule : IEquatable<OverstayRule>
    {

        #region Properties

        /// <summary>
        /// The time after trigger of the parent Overstay Rules for this particular fee to apply.
        /// </summary>
        [Mandatory]
        public TimeSpan        StartTime      { get; }

        /// <summary>
        /// The overstay fee period.
        /// </summary>
        [Mandatory]
        public TimeSpan        Period         { get; }

        /// <summary>
        /// The overstay fee.
        /// </summary>
        [Mandatory]
        public RationalNumber  Fee            { get; }

        /// <summary>
        /// The optional description.
        /// </summary>
        [Optional]
        public String?         Description    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new overstay rule.
        /// </summary>
        /// <param name="StartTime">A start time.</param>
        /// <param name="Period">An overstay fee period.</param>
        /// <param name="Fee">An overstay fee.</param>
        /// <param name="Description">An optional description.</param>
        public OverstayRule(TimeSpan        StartTime,
                            TimeSpan        Period,
                            RationalNumber  Fee,
                            String?         Description   = null)
        {

            this.StartTime    = StartTime;
            this.Period       = Period;
            this.Fee          = Fee;
            this.Description  = Description;

            unchecked
            {

                hashCode = this.StartTime.   GetHashCode()       * 11 ^
                           this.Period.      GetHashCode()       *  7 ^
                           this.Fee.         GetHashCode()       *  5 ^
                          (this.Description?.GetHashCode() ?? 0) *  3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Part of ISO 15118-20 price schedule.",
        //     "javaType": "OverstayRule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "overstayFee": {
        //             "$ref": "#/definitions/RationalNumberType"
        //         },
        //         "overstayRuleDescription": {
        //             "description": "Human readable string to identify the overstay rule.",
        //             "type": "string",
        //             "maxLength": 32
        //         },
        //         "startTime": {
        //             "description": "Time in seconds after trigger of the parent Overstay Rules for this particular fee to apply.",
        //             "type": "integer"
        //         },
        //         "overstayFeePeriod": {
        //             "description": "Time till overstay will be reapplied",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "startTime",
        //         "overstayFeePeriod",
        //         "overstayFee"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomOverstayRuleParser = null)

        /// <summary>
        /// Parse the given JSON representation of an overstay rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomOverstayRuleParser">An optional delegate to parse custom overstay rules.</param>
        public static OverstayRule Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<OverstayRule>?  CustomOverstayRuleParser   = null)
        {

            if (TryParse(JSON,
                         out var overstayRule,
                         out var errorResponse,
                         CustomOverstayRuleParser))
            {
                return overstayRule;
            }

            throw new ArgumentException("The given JSON representation of an overstay rule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out OverstayRule, out ErrorResponse, CustomOverstayRuleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an overstay rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OverstayRule">The parsed overstay rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out OverstayRule?  OverstayRule,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out OverstayRule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an overstay rule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OverstayRule">The parsed overstay rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOverstayRuleParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out OverstayRule?      OverstayRule,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<OverstayRule>?  CustomOverstayRuleParser)
        {

            try
            {

                OverstayRule = null;

                #region StartTime      [mandatory]

                if (!JSON.ParseMandatory("startTime",
                                         "start time",
                                         out TimeSpan StartTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Period         [mandatory]

                if (!JSON.ParseMandatory("overstayFeePeriod",
                                         "overstay fee period",
                                         out TimeSpan Period,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Fee            [mandatory]

                if (!JSON.ParseMandatoryJSON("overstayFee",
                                             "overstay fee",
                                             RationalNumber.TryParse,
                                             out RationalNumber? Fee,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Description    [optional]

                var Description = JSON["overstayRuleDescription"]?.Value<String>();

                #endregion


                OverstayRule = new OverstayRule(
                                   StartTime,
                                   Period,
                                   Fee,
                                   Description
                               );

                if (CustomOverstayRuleParser is not null)
                    OverstayRule = CustomOverstayRuleParser(JSON,
                                                            OverstayRule);

                return true;

            }
            catch (Exception e)
            {
                OverstayRule   = null;
                ErrorResponse  = "The given JSON representation of an overstay rule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomOverstayRuleSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOverstayRuleSerializer">A delegate to serialize custom overstay rules.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OverstayRule>? CustomOverstayRuleSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("startTime",    (UInt32) Math.Round(StartTime.TotalSeconds, 0)),
                                 new JProperty("period",       (UInt32) Math.Round(Period.   TotalSeconds, 0)),
                                 new JProperty("fee",          Fee.ToJSON()),

                           Description.IsNotNullOrEmpty()
                               ? new JProperty("description",  Description)
                               : null

                       );

            return CustomOverstayRuleSerializer is not null
                       ? CustomOverstayRuleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (OverstayRule1, OverstayRule2)

        /// <summary>
        /// Compares two overstay rules for equality.
        /// </summary>
        /// <param name="OverstayRule1">An overstay rule.</param>
        /// <param name="OverstayRule2">Another overstay rule.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (OverstayRule? OverstayRule1,
                                           OverstayRule? OverstayRule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OverstayRule1, OverstayRule2))
                return true;

            // If one is null, but not both, return false.
            if (OverstayRule1 is null || OverstayRule2 is null)
                return false;

            return OverstayRule1.Equals(OverstayRule2);

        }

        #endregion

        #region Operator != (OverstayRule1, OverstayRule2)

        /// <summary>
        /// Compares two overstay rules for inequality.
        /// </summary>
        /// <param name="OverstayRule1">An overstay rule.</param>
        /// <param name="OverstayRule2">Another overstay rule.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (OverstayRule? OverstayRule1,
                                           OverstayRule? OverstayRule2)

            => !(OverstayRule1 == OverstayRule2);

        #endregion

        #endregion

        #region IEquatable<OverstayRule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two overstay rules for equality.
        /// </summary>
        /// <param name="Object">An overstay rule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OverstayRule overstayRule &&
                   Equals(overstayRule);

        #endregion

        #region Equals(OverstayRule)

        /// <summary>
        /// Compares two overstay rules for equality.
        /// </summary>
        /// <param name="OverstayRule">An overstay rule to compare with.</param>
        public Boolean Equals(OverstayRule? OverstayRule)

            => OverstayRule is not null &&

               StartTime.Equals(OverstayRule.StartTime) &&
               Period.   Equals(OverstayRule.Period)    &&
               Fee.      Equals(OverstayRule.Fee)       &&

               String.   Equals(Description, OverstayRule.Description);

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

                   $"{StartTime}, {Period.TotalSeconds} second(s), {Fee} fee",

                   $"{(Description.IsNotNullOrEmpty()
                           ? $", description: '{Description}'"
                           : "")}"

               );

        #endregion

    }

}
