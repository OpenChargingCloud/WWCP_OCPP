/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The price rule stack.
    /// </summary>
    public class PriceRuleStack : IEquatable<PriceRuleStack>
    {

        #region Properties

        /// <summary>
        /// The duration.
        /// </summary>
        public TimeSpan                Duration      { get; }

        /// <summary>
        /// The enumeration of price rules.
        /// [max 8]
        /// </summary>
        public IEnumerable<PriceRule>  PriceRules    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price rule stack.
        /// </summary>
        /// <param name="Duration">A duration.</param>
        /// <param name="PriceRules">An enumeration of price rules [max 8].</param>
        public PriceRuleStack(TimeSpan                Duration,
                              IEnumerable<PriceRule>  PriceRules)
        {

            if (!PriceRules.Any())
                throw new ArgumentException("The given enumeration of price rules must not be empty!",
                                            nameof(PriceRules));

            this.Duration    = Duration;
            this.PriceRules  = PriceRules.Distinct();

            unchecked
            {

                hashCode = this.Duration.  GetHashCode()  * 5 ^
                           this.PriceRules.CalcHashCode() * 3 ^

                           base.           GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomPriceRuleStackParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price rule stack.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPriceRuleStackParser">An optional delegate to parse custom price rule stacks.</param>
        public static PriceRuleStack Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<PriceRuleStack>?  CustomPriceRuleStackParser   = null)
        {

            if (TryParse(JSON,
                         out var priceRuleStack,
                         out var errorResponse,
                         CustomPriceRuleStackParser) &&
                priceRuleStack is not null)
            {
                return priceRuleStack;
            }

            throw new ArgumentException("The given JSON representation of a price rule stack is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PriceRuleStack, out ErrorResponse, CustomPriceRuleStackParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price rule stack.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceRuleStack">The parsed price rule stack.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out PriceRuleStack?  PriceRuleStack,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out PriceRuleStack,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price rule stack.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceRuleStack">The parsed price rule stack.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceRuleStackParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out PriceRuleStack?                           PriceRuleStack,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<PriceRuleStack>?  CustomPriceRuleStackParser)
        {

            try
            {

                PriceRuleStack = null;

                #region Duration      [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out UInt64 duration,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Duration = TimeSpan.FromSeconds(duration);

                #endregion

                #region PriceRules    [mandatory]

                if (!JSON.ParseMandatoryHashSet("priceRules",
                                                "price rules",
                                                PriceRule.TryParse,
                                                out HashSet<PriceRule> PriceRules,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PriceRuleStack = new PriceRuleStack(Duration,
                                                    PriceRules);

                if (CustomPriceRuleStackParser is not null)
                    PriceRuleStack = CustomPriceRuleStackParser(JSON,
                                                                PriceRuleStack);

                return true;

            }
            catch (Exception e)
            {
                PriceRuleStack  = null;
                ErrorResponse   = "The given JSON representation of a price rule stack is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceRuleStackSerializer = null, CustomPriceRuleSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceRuleStackSerializer">A delegate to serialize custom price rule stacks.</param>
        /// <param name="CustomPriceRuleSerializer">A delegate to serialize custom price rules.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PriceRuleStack>?  CustomPriceRuleStackSerializer   = null,
                              CustomJObjectSerializerDelegate<PriceRule>?       CustomPriceRuleSerializer        = null)
        {

            var json = JSONObject.Create(

                           new JProperty("duration",    (UInt64) Math.Round(Duration.TotalSeconds, 0)),
                           new JProperty("priceRules",  new JArray(PriceRules.Select(priceRule => priceRule.ToJSON(CustomPriceRuleSerializer))))

                       );

            return CustomPriceRuleStackSerializer is not null
                       ? CustomPriceRuleStackSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PriceRuleStack1, PriceRuleStack2)

        /// <summary>
        /// Compares two price rule stacks for equality.
        /// </summary>
        /// <param name="PriceRuleStack1">A price rule stack.</param>
        /// <param name="PriceRuleStack2">Another price rule stack.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PriceRuleStack? PriceRuleStack1,
                                           PriceRuleStack? PriceRuleStack2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PriceRuleStack1, PriceRuleStack2))
                return true;

            // If one is null, but not both, return false.
            if (PriceRuleStack1 is null || PriceRuleStack2 is null)
                return false;

            return PriceRuleStack1.Equals(PriceRuleStack2);

        }

        #endregion

        #region Operator != (PriceRuleStack1, PriceRuleStack2)

        /// <summary>
        /// Compares two price rule stacks for inequality.
        /// </summary>
        /// <param name="PriceRuleStack1">A price rule stack.</param>
        /// <param name="PriceRuleStack2">Another price rule stack.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PriceRuleStack? PriceRuleStack1,
                                           PriceRuleStack? PriceRuleStack2)

            => !(PriceRuleStack1 == PriceRuleStack2);

        #endregion

        #endregion

        #region IEquatable<PriceRuleStack> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price rule stacks for equality.
        /// </summary>
        /// <param name="Object">A price rule stack to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceRuleStack priceRuleStack &&
                   Equals(priceRuleStack);

        #endregion

        #region Equals(PriceRuleStack)

        /// <summary>
        /// Compares two price rule stacks for equality.
        /// </summary>
        /// <param name="PriceRuleStack">A price rule stack to compare with.</param>
        public Boolean Equals(PriceRuleStack? PriceRuleStack)

            => PriceRuleStack is not null &&

               Duration.Equals(PriceRuleStack.Duration) &&

               PriceRules.Count().Equals(PriceRuleStack.PriceRules.Count()) &&
               PriceRules.All(priceRule => PriceRuleStack.PriceRules.Contains(priceRule));

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

            => $"{Duration.TotalSeconds} second(s), {PriceRules.Count()} price rule(s)";

        #endregion

    }

}
