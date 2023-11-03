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
    /// The cost dimension volume.
    /// </summary>
    public readonly struct CostDimensionVolume : IEquatable<CostDimensionVolume>,
                                                 IComparable<CostDimensionVolume>,
                                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The cost dimension.
        /// </summary>
        [Mandatory]
        public CostDimension  Type      { get; }

        /// <summary>
        /// The volume of the dimension consumed, measured according to the dimension type.
        /// </summary>
        [Mandatory]
        public Decimal        Volume    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cost dimension volume.
        /// </summary>
        /// <param name="Type">A cost dimension.</param>
        /// <param name="Volume">A volume of the dimension consumed, measured according to the dimension type.</param>
        public CostDimensionVolume(CostDimension  Type,
                                   Decimal        Volume)
        {

            this.Type    = Type;
            this.Volume  = Volume;

        }

        #endregion


        #region (static) Parse   (JSON, CustomCostDimensionValueParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cost dimension volume.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCostDimensionValueParser">A delegate to parse custom cost dimension volume JSON objects.</param>
        public static CostDimensionVolume Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<CostDimensionVolume>?  CustomCostDimensionValueParser   = null)
        {

            if (TryParse(JSON,
                         out var costDimensionVolume,
                         out var errorResponse,
                         CustomCostDimensionValueParser))
            {
                return costDimensionVolume;
            }

            throw new ArgumentException("The given JSON representation of a cost dimension volume is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CostDimensionValue, out ErrorResponse, CustomCostDimensionValueParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a cost dimension volume.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CostDimensionValue">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                  JSON,
                                       out CostDimensionVolume  CostDimensionValue,
                                       out String?              ErrorResponse)

            => TryParse(JSON,
                        out CostDimensionValue,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a cost dimension volume.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CostDimensionValue">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostDimensionValueParser">A delegate to parse custom cost dimension volume JSON objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       out CostDimensionVolume                            CostDimensionValue,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<CostDimensionVolume>?  CustomCostDimensionValueParser)
        {

            try
            {

                CostDimensionValue = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Type      [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "cost dimension type",
                                         CostDimension.TryParse,
                                         out CostDimension Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Volume    [mandatory]

                if (!JSON.ParseMandatory("volume",
                                         "cost dimension volume",
                                         out Decimal Volume,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CostDimensionValue = new CostDimensionVolume(
                                         Type,
                                         Volume
                                     );


                if (CustomCostDimensionValueParser is not null)
                    CostDimensionValue = CustomCostDimensionValueParser(JSON,
                                                                        CostDimensionValue);

                return true;

            }
            catch (Exception e)
            {
                CostDimensionValue  = default;
                ErrorResponse       = "The given JSON representation of a cost dimension volume is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCostDimensionValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCostDimensionValueSerializer">A delegate to serialize custom cost dimension volume JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CostDimensionVolume>? CustomCostDimensionValueSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("type",    Type.ToString()),
                           new JProperty("volume",  Volume)
                       );

            return CustomCostDimensionValueSerializer is not null
                       ? CustomCostDimensionValueSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public CostDimensionVolume Clone()

            => new (
                   Type.Clone,
                   Volume
               );

        #endregion


        #region Operator overloading

        #region Operator == (CostDimensionValue1, CostDimensionValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimensionValue1">A cost dimension volume.</param>
        /// <param name="CostDimensionValue2">Another cost dimension volume.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CostDimensionVolume CostDimensionValue1,
                                           CostDimensionVolume CostDimensionValue2)

            => CostDimensionValue1.Equals(CostDimensionValue2);

        #endregion

        #region Operator != (CostDimensionValue1, CostDimensionValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimensionValue1">A cost dimension volume.</param>
        /// <param name="CostDimensionValue2">Another cost dimension volume.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CostDimensionVolume CostDimensionValue1,
                                           CostDimensionVolume CostDimensionValue2)

            => !(CostDimensionValue1 == CostDimensionValue2);

        #endregion

        #region Operator <  (CostDimensionValue1, CostDimensionValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimensionValue1">A cost dimension volume.</param>
        /// <param name="CostDimensionValue2">Another cost dimension volume.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CostDimensionVolume CostDimensionValue1,
                                          CostDimensionVolume CostDimensionValue2)

            => CostDimensionValue1.CompareTo(CostDimensionValue2) < 0;

        #endregion

        #region Operator <= (CostDimensionValue1, CostDimensionValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimensionValue1">A cost dimension volume.</param>
        /// <param name="CostDimensionValue2">Another cost dimension volume.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CostDimensionVolume CostDimensionValue1,
                                           CostDimensionVolume CostDimensionValue2)

            => !(CostDimensionValue1 > CostDimensionValue2);

        #endregion

        #region Operator >  (CostDimensionValue1, CostDimensionValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimensionValue1">A cost dimension volume.</param>
        /// <param name="CostDimensionValue2">Another cost dimension volume.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CostDimensionVolume CostDimensionValue1,
                                          CostDimensionVolume CostDimensionValue2)

            => CostDimensionValue1.CompareTo(CostDimensionValue2) > 0;

        #endregion

        #region Operator >= (CostDimensionValue1, CostDimensionValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimensionValue1">A cost dimension volume.</param>
        /// <param name="CostDimensionValue2">Another cost dimension volume.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CostDimensionVolume CostDimensionValue1,
                                           CostDimensionVolume CostDimensionValue2)

            => !(CostDimensionValue1 < CostDimensionValue2);

        #endregion

        #endregion

        #region IComparable<CostDimensionValue> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cost dimension volumes.
        /// </summary>
        /// <param name="Object">A cost dimension volume to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CostDimensionVolume costDimensionVolume
                   ? CompareTo(costDimensionVolume)
                   : throw new ArgumentException("The given object is not a cost dimension volume!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CostDimensionValue)

        /// <summary>
        /// Compares two cost dimension volumes.
        /// </summary>
        /// <param name="CostDimensionValue">A cost dimension volume to compare with.</param>
        public Int32 CompareTo(CostDimensionVolume CostDimensionValue)
        {

            var c = Type.  CompareTo(CostDimensionValue.Type);

            if (c == 0)
                c = Volume.CompareTo(CostDimensionValue.Volume);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CostDimensionValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cost dimension volumes for equality.
        /// </summary>
        /// <param name="Object">A cost dimension volume to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CostDimensionVolume costDimensionVolume &&
                   Equals(costDimensionVolume);

        #endregion

        #region Equals(CostDimensionValue)

        /// <summary>
        /// Compares two cost dimension volumes for equality.
        /// </summary>
        /// <param name="CostDimensionValue">A cost dimension volume to compare with.</param>
        public Boolean Equals(CostDimensionVolume CostDimensionValue)

            => Type.  Equals(CostDimensionValue.Type) &&
               Volume.Equals(CostDimensionValue.Volume);

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

                return Type.  GetHashCode() * 3 ^
                       Volume.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Type}: {Volume}";

        #endregion

    }

}
