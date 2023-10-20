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
    /// The environmental impact specifies the amount of waste produced/emitted per kWh.
    /// </summary>
    public readonly struct EnvironmentalImpact : IEquatable<EnvironmentalImpact>,
                                                 IComparable<EnvironmentalImpact>,
                                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The environmental impact.
        /// </summary>
        [Mandatory]
        public EnvironmentalImpactCategory  Category    { get; }

        /// <summary>
        /// The amount of this environmental impact.
        /// </summary>
        [Mandatory]
        public Double                       Amount      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new environmental impact.
        /// </summary>
        /// <param name="Category">The environmental impact category.</param>
        /// <param name="Amount">The amount of this environmental impact.</param>
        public EnvironmentalImpact(EnvironmentalImpactCategory  Category,
                                   Double                       Amount)
        {

            this.Category  = Category;
            this.Amount    = Amount;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnvironmentalImpactParser = null)

        /// <summary>
        /// Parse the given JSON representation of a hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnvironmentalImpactParser">A delegate to parse custom environmental impact JSON objects.</param>
        public static EnvironmentalImpact Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactParser   = null)
        {

            if (TryParse(JSON,
                         out var energySource,
                         out var errorResponse,
                         CustomEnvironmentalImpactParser))
            {
                return energySource;
            }

            throw new ArgumentException("The given JSON representation of a hour is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EnvironmentalImpact, out ErrorResponse, CustomEnvironmentalImpactParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnvironmentalImpact">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                  JSON,
                                       out EnvironmentalImpact  EnvironmentalImpact,
                                       out String?              ErrorResponse)

            => TryParse(JSON,
                        out EnvironmentalImpact,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a hour.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnvironmentalImpact">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnvironmentalImpactParser">A delegate to parse custom environmental impact JSON objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       out EnvironmentalImpact                            EnvironmentalImpact,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactParser)
        {

            try
            {

                EnvironmentalImpact = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse EnvironmentalImpactCategory    [mandatory]

                if (!JSON.ParseMandatory("category",
                                         "environmental impact category",
                                         OCPPv2_1.EnvironmentalImpactCategory.TryParse,
                                         out EnvironmentalImpactCategory EnvironmentalImpactCategory,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Amount                         [mandatory]

                if (!JSON.ParseMandatory("amount",
                                         "amount",
                                         out Double Amount,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                EnvironmentalImpact = new EnvironmentalImpact(EnvironmentalImpactCategory,
                                                              Amount);


                if (CustomEnvironmentalImpactParser is not null)
                    EnvironmentalImpact = CustomEnvironmentalImpactParser(JSON,
                                                                          EnvironmentalImpact);

                return true;

            }
            catch (Exception e)
            {
                EnvironmentalImpact  = default;
                ErrorResponse        = "The given JSON representation of a hour is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEnvironmentalImpactSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EnvironmentalImpact>? CustomEnvironmentalImpactSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("category",  Category.ToString()),
                           new JProperty("amount",    Amount)
                       );

            return CustomEnvironmentalImpactSerializer is not null
                       ? CustomEnvironmentalImpactSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public EnvironmentalImpact Clone()

            => new (Category.Clone,
                    Amount);

        #endregion


        #region Operator overloading

        #region Operator == (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.Equals(EnvironmentalImpact2);

        #endregion

        #region Operator != (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => !EnvironmentalImpact1.Equals(EnvironmentalImpact2);

        #endregion

        #region Operator <  (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnvironmentalImpact EnvironmentalImpact1,
                                          EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.CompareTo(EnvironmentalImpact2) < 0;

        #endregion

        #region Operator <= (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.CompareTo(EnvironmentalImpact2) <= 0;

        #endregion

        #region Operator >  (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnvironmentalImpact EnvironmentalImpact1,
                                          EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.CompareTo(EnvironmentalImpact2) > 0;

        #endregion

        #region Operator >= (EnvironmentalImpact1, EnvironmentalImpact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpact1">An environmental impact.</param>
        /// <param name="EnvironmentalImpact2">Another environmental impact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnvironmentalImpact EnvironmentalImpact1,
                                           EnvironmentalImpact EnvironmentalImpact2)

            => EnvironmentalImpact1.CompareTo(EnvironmentalImpact2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnvironmentalImpact> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two environmental impacts.
        /// </summary>
        /// <param name="Object">An environmental impact to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnvironmentalImpact environmentalImpact
                   ? CompareTo(environmentalImpact)
                   : throw new ArgumentException("The given object is not an environmental impact!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnvironmentalImpact)

        /// <summary>
        /// Compares two environmental impacts.
        /// </summary>
        /// <param name="EnvironmentalImpact">An environmental impact to compare with.</param>
        public Int32 CompareTo(EnvironmentalImpact EnvironmentalImpact)
        {

            var c = Category.CompareTo(EnvironmentalImpact.Category);

            if (c == 0)
                c = Amount.  CompareTo(EnvironmentalImpact.Amount);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnvironmentalImpact> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two environmental impacts for equality.
        /// </summary>
        /// <param name="Object">An environmental impact to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnvironmentalImpact environmentalImpact &&
                   Equals(environmentalImpact);

        #endregion

        #region Equals(EnvironmentalImpact)

        /// <summary>
        /// Compares two environmental impacts for equality.
        /// </summary>
        /// <param name="EnvironmentalImpact">An environmental impact to compare with.</param>
        public Boolean Equals(EnvironmentalImpact EnvironmentalImpact)

            => Category.Equals(EnvironmentalImpact.Category) &&
               Amount.  Equals(EnvironmentalImpact.Amount);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Category.GetHashCode() * 3 ^
                       Amount.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Category,
                   ": ",
                   Amount

               );

        #endregion

    }

}
