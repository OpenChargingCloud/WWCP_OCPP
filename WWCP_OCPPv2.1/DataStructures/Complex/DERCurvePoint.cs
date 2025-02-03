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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DER Curve Point
    /// </summary>
    public class DERCurvePoint : ACustomData,
                                 IEquatable<DERCurvePoint>
    {

        #region Properties

        /// <summary>
        /// The data value of the X-axis (independent) variable, depending on the curve type.
        /// </summary>
        [Optional]
        public Decimal  X    { get; }

        /// <summary>
        /// The data value of the Y-axis (dependent) variable, depending on the  &lt;&lt;cmn_derunitenumtype&gt;&gt; of the curve.
        /// If _y_ is power factor, then a positive value means DER is absorbing reactive power (under-excited), a negative value
        /// when DER is injecting reactive power (over-excited).
        /// </summary>
        [Optional]
        public Decimal  Y    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new DER curve point
        /// </summary>
        /// <param name="X">The data value of the X-axis (independent) variable, depending on the curve type.</param>
        /// <param name="Y">The data value of the Y-axis (dependent) variable, depending on the  &lt;&lt;cmn_derunitenumtype&gt;&gt; of the curve. If _y_ is power factor, then a positive value means DER is absorbing reactive power (under-excited), a negative value when DER is injecting reactive power (over-excited).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERCurvePoint(Decimal      X,
                             Decimal      Y,
                             CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.X  = X;
            this.Y  = Y;

            unchecked
            {

                hashCode = this.X.GetHashCode() * 5 ^
                           this.Y.GetHashCode() * 3 ^
                           base.  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "DERCurvePoints",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "x": {
        //             "description": "The data value of the X-axis (independent) variable, depending on the curve type.",
        //             "type": "number"
        //         },
        //         "y": {
        //             "description": "The data value of the Y-axis (dependent) variable, depending on the  &lt;&lt;cmn_derunitenumtype&gt;&gt; of the curve.
        //                             If _y_ is power factor, then a positive value means DER is absorbing reactive power (under-excited), a negative value
        //                             when DER is injecting reactive power (over-excited).",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "x",
        //         "y"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomDERCurvePointParser = null)

        /// <summary>
        /// Parse the given JSON representation of DER curve point.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDERCurvePointParser">A delegate to parse custom DER curve point JSON objects.</param>
        public static DERCurvePoint Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<DERCurvePoint>?  CustomDERCurvePointParser   = null)
        {

            if (TryParse(JSON,
                         out var derCurvePoint,
                         out var errorResponse,
                         CustomDERCurvePointParser))
            {
                return derCurvePoint;
            }

            throw new ArgumentException("The given JSON representation of DER curve point is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DERCurvePoint, out ErrorResponse, CustomDERCurvePointParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of DER curve point.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERCurvePoint">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out DERCurvePoint?  DERCurvePoint,
                                       [NotNullWhen(false)] out String?         ErrorResponse)

            => TryParse(JSON,
                        out DERCurvePoint,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of DER curve point.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERCurvePoint">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDERCurvePointParser">A delegate to parse custom DER curve point JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out DERCurvePoint?      DERCurvePoint,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       CustomJObjectParserDelegate<DERCurvePoint>?  CustomDERCurvePointParser)
        {

            try
            {

                DERCurvePoint = default;

                #region X             [mandatory]

                if (!JSON.ParseMandatory("x",
                                         "X",
                                         out Decimal X,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Y             [mandatory]

                if (!JSON.ParseMandatory("y",
                                         "Y",
                                         out Decimal Y,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData    [optional]

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


                DERCurvePoint = new DERCurvePoint(
                                    X,
                                    Y,
                                    CustomData
                                );

                if (CustomDERCurvePointParser is not null)
                    DERCurvePoint = CustomDERCurvePointParser(JSON,
                                                              DERCurvePoint);

                return true;

            }
            catch (Exception e)
            {
                DERCurvePoint  = default;
                ErrorResponse  = "The given JSON representation of DER curve point is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomDERCurvePointSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDERCurvePointSerializer">A delegate to serialize custom DER curve point.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERCurvePoint>?  CustomDERCurvePointSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?     CustomCustomDataSerializer      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("x",            X),
                                 new JProperty("y",            Y),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDERCurvePointSerializer is not null
                       ? CustomDERCurvePointSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DERCurvePoint1, DERCurvePoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERCurvePoint1">DER curve point.</param>
        /// <param name="DERCurvePoint2">Another DER curve point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERCurvePoint? DERCurvePoint1,
                                           DERCurvePoint? DERCurvePoint2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DERCurvePoint1, DERCurvePoint2))
                return true;

            // If one is null, but not both, return false.
            if (DERCurvePoint1 is null || DERCurvePoint2 is null)
                return false;

            return DERCurvePoint1.Equals(DERCurvePoint2);

        }

        #endregion

        #region Operator != (DERCurvePoint1, DERCurvePoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERCurvePoint1">DER curve point.</param>
        /// <param name="DERCurvePoint2">Another DER curve point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERCurvePoint? DERCurvePoint1,
                                           DERCurvePoint? DERCurvePoint2)

            => !(DERCurvePoint1 == DERCurvePoint2);

        #endregion

        #endregion

        #region IEquatable<DERCurvePoint> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER curve point for equality..
        /// </summary>
        /// <param name="Object">DER curve point to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERCurvePoint derCurvePoint &&
                   Equals(derCurvePoint);

        #endregion

        #region Equals(DERCurvePoint)

        /// <summary>
        /// Compares two DER curve point for equality.
        /// </summary>
        /// <param name="DERCurvePoint">DER curve point to compare with.</param>
        public Boolean Equals(DERCurvePoint? DERCurvePoint)

            => DERCurvePoint is not null &&

               X.   Equals(DERCurvePoint.X) &&
               Y.   Equals(DERCurvePoint.Y) &&

               base.Equals(DERCurvePoint);

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

            => $"{X} / {Y}";

        #endregion

    }

}
