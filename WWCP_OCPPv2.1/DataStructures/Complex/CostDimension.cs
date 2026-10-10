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
    /// The volume of a dimension of the cost of a charging period: the energy,
    /// a current, a power or a time.
    /// </summary>
    public class CostDimension : IEquatable<CostDimension>,
                                 ICBORSerializable<CostDimension>
    {

        #region Properties

        /// <summary>
        /// The dimension.
        /// </summary>
        [Mandatory]
        public CostDimensions  Type          { get; }

        /// <summary>
        /// The volume of the dimension, in the unit of the dimension: Wh, A, W or seconds.
        /// </summary>
        [Mandatory]
        public Decimal         Volume        { get; }

        /// <summary>
        /// An optional custom data object allowing to store any kind of customer specific data.
        /// </summary>
        [Optional]
        public CustomData?     CustomData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new volume of a dimension of the cost of a charging period.
        /// </summary>
        /// <param name="Type">The dimension.</param>
        /// <param name="Volume">The volume of the dimension, in the unit of the dimension: Wh, A, W or seconds.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public CostDimension(CostDimensions  Type,
                             Decimal         Volume,
                             CustomData?     CustomData   = null)
        {

            this.Type        = Type;
            this.Volume      = Volume;
            this.CustomData  = CustomData;

            unchecked
            {
                hashCode = this.Type.       GetHashCode()       * 5 ^
                           this.Volume.     GetHashCode()       * 3 ^
                          (this.CustomData?.GetHashCode() ?? 0);
            }

        }

        #endregion


        #region Documentation

        // "CostDimensionType": {
        //   "description": "Volume consumed of cost dimension.",
        //   "javaType": "CostDimension",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "type":       { "$ref": "#/definitions/CostDimensionEnumType" },
        //     "volume":     { "description": "Volume of the dimension consumed, measured according to the dimension type.", "type": "number" },
        //     "customData": { "$ref": "#/definitions/CustomDataType" }
        //   },
        //   "required": [ "type", "volume" ]
        // }

        #endregion

        #region (static) TryParse    (JSON, out CostDimension, out ErrorResponse, CustomCostDimensionParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a volume of a cost dimension.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CostDimension">The parsed volume of a cost dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out CostDimension?       CostDimension,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

            => TryParse(JSON,
                        out CostDimension,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a volume of a cost dimension.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CostDimension">The parsed volume of a cost dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostDimensionParser">An optional delegate to parse custom volumes of cost dimensions.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out CostDimension?       CostDimension,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       CustomJObjectParserDelegate<CostDimension>?   CustomCostDimensionParser)
        {

            try
            {

                CostDimension = null;

                if (!JSON.ParseMandatory("type",
                                         "cost dimension",
                                         CostDimensionsExtensions.TryParse,
                                         out CostDimensions Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (!JSON.ParseMandatory("volume",
                                         "volume",
                                         out Decimal Volume,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }


                CostDimension = new CostDimension(
                                    Type,
                                    Volume,
                                    CustomData
                                );

                if (CustomCostDimensionParser is not null)
                    CostDimension = CustomCostDimensionParser(JSON,
                                                              CostDimension);

                return true;

            }
            catch (Exception e)
            {
                CostDimension  = null;
                ErrorResponse  = "The given JSON representation of a volume of a cost dimension is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCostDimensionSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this volume of a cost dimension.
        /// </summary>
        /// <param name="CustomCostDimensionSerializer">A delegate to serialize custom volumes of cost dimensions.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CostDimension>?  CustomCostDimensionSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?     CustomCustomDataSerializer      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("type",         Type.AsText()),
                                 new JProperty("volume",       Volume),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCostDimensionSerializer is not null
                       ? CustomCostDimensionSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out CostDimension, out ErrorResponse, CustomCostDimensionParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a volume of a cost dimension:
        /// its volume a metrological value in the unit of its dimension.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="CostDimension">The volume of a cost dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                    CBOR,
                                           [NotNullWhen(true)]  out CostDimension?      CostDimension,
                                           [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParseCBOR(CBOR,
                            out CostDimension,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a volume of a cost dimension:
        /// its volume a metrological value in the unit of its dimension.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="CostDimension">The volume of a cost dimension.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostDimensionParser">An optional delegate to read custom volumes of cost dimensions.</param>
        public static Boolean TryParseCBOR(CBORValue                                    CBOR,
                                           [NotNullWhen(true)]  out CostDimension?      CostDimension,
                                           [NotNullWhen(false)] out String?             ErrorResponse,
                                           CustomCBORParserDelegate<CostDimension>?     CustomCostDimensionParser)
        {

            try
            {

                CostDimension = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a volume of a cost dimension is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("type",
                                             "cost dimension",
                                             out var typeText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!CostDimensionsExtensions.TryParse(typeText, out var Type))
                {
                    ErrorResponse = $"Invalid cost dimension '{typeText}'!";
                    return false;
                }

                if (!CBOR.TryGetValue(CBORValue.FromText("volume"), out var volumeCBOR))
                {
                    ErrorResponse = "Missing CBOR property 'volume'!";
                    return false;
                }

                if (!OCPPCBORExtensions.TryParseQuantity(volumeCBOR,
                                                         Type.Unit(),
                                                         out var Volume,
                                                         out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                CostDimension = new CostDimension(
                                    Type,
                                    Volume,
                                    CustomData
                                );

                if (CustomCostDimensionParser is not null)
                    CostDimension = CustomCostDimensionParser(CBOR,
                                                              CostDimension);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                CostDimension  = null;
                ErrorResponse  = "The given CBOR representation of a volume of a cost dimension is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<CostDimension>.TryParse(CBOR, out CostDimension, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a volume of a cost dimension - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<CostDimension>.TryParse(CBORValue                         CBOR,
                                                                 out CostDimension                 Value,
                                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomCostDimensionSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this volume of a cost dimension: the keys
        /// of its JSON object, its volume a metrological value in the unit of its
        /// dimension - Wh, A, W or s.
        /// </summary>
        /// <param name="CustomCostDimensionSerializer">A delegate to serialize custom volumes of cost dimensions.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<CostDimension>? CustomCostDimensionSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("type",        CBORValue.FromText(Type.AsText())),
                           ("volume",      OCPPCBORExtensions.QuantityToCBOR(Volume, Type.Unit())),
                           ("customData",  CustomData?.ToCBOR())
                       );

            return CustomCostDimensionSerializer is not null
                       ? CustomCostDimensionSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region IEquatable<CostDimension> Members

        /// <summary>
        /// Compares two volumes of cost dimensions for equality.
        /// </summary>
        /// <param name="Object">A volume of a cost dimension to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CostDimension costDimension &&
                   Equals(costDimension);


        /// <summary>
        /// Compares two volumes of cost dimensions for equality.
        /// </summary>
        /// <param name="CostDimension">A volume of a cost dimension to compare with.</param>
        public Boolean Equals(CostDimension? CostDimension)

            => CostDimension is not null &&

               Type.  Equals(CostDimension.Type)   &&
               Volume.Equals(CostDimension.Volume) &&

             ((CustomData is null     && CostDimension.CustomData is null) ||
              (CustomData is not null && CustomData.Equals(CostDimension.CustomData)));

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

            => $"{Type.AsText()}: {Volume} {Type.Unit()}";

        #endregion

    }

}
