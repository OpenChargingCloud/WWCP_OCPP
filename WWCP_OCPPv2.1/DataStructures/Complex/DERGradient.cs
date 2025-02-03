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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DER Gradient
    /// </summary>
    public class DERGradient : ACustomData,
                               IEquatable<DERGradient>
    {

        #region Properties

        /// <summary>
        /// The priority of the settings (0=highest)
        /// </summary>
        [Mandatory]
        public Byte      Priority        { get; }

        /// <summary>
        /// Default ramp rate (0 if not applicable).
        /// </summary>
        [Mandatory]
        public TimeSpan  Gradient        { get; }

        /// <summary>
        /// Soft-start ramp rate (0 if not applicable).
        /// </summary>
        [Mandatory]
        public TimeSpan  SoftGradient    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new DERGradient
        /// </summary>
        /// <param name="Priority">The priority of the settings (0=highest)</param>
        /// <param name="Gradient">Default ramp rate (0 if not applicable)</param>
        /// <param name="SoftGradient">Soft-start ramp rate (0 if not applicable)</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERGradient(Byte         Priority,
                           TimeSpan     Gradient,
                           TimeSpan     SoftGradient,
                           CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Priority      = Priority;
            this.Gradient      = Gradient;
            this.SoftGradient  = SoftGradient;

            unchecked
            {

                hashCode = this.Priority.    GetHashCode() * 7 ^
                           this.Gradient.    GetHashCode() * 5 ^
                           this.SoftGradient.GetHashCode() * 3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "Gradient",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priority": {
        //             "description": "Id of setting",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "gradient": {
        //             "description": "Default ramp rate in seconds (0 if not applicable)",
        //             "type": "number"
        //         },
        //         "softGradient": {
        //             "description": "Soft-start ramp rate in seconds (0 if not applicable)",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "priority",
        //         "gradient",
        //         "softGradient"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomDERGradientParser = null)

        /// <summary>
        /// Parse the given JSON representation of DERGradient.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDERGradientParser">A delegate to parse custom DERGradient JSON objects.</param>
        public static DERGradient Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<DERGradient>?  CustomDERGradientParser   = null)
        {

            if (TryParse(JSON,
                         out var derGradient,
                         out var errorResponse,
                         CustomDERGradientParser))
            {
                return derGradient;
            }

            throw new ArgumentException("The given JSON representation of a DERGradient is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DERGradient, out ErrorResponse, CustomDERGradientParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of DERGradient.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERGradient">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out DERGradient?  DERGradient,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out DERGradient,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of DERGradient.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERGradient">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDERGradientParser">A delegate to parse custom DERGradient JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out DERGradient?      DERGradient,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<DERGradient>?  CustomDERGradientParser)
        {

            try
            {

                DERGradient = default;

                #region Priority        [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "curve priority",
                                         out Byte Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Gradient        [mandatory]

                if (!JSON.ParseMandatory("gradient",
                                         "gradient",
                                         out TimeSpan Gradient,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SoftGradient    [mandatory]

                if (!JSON.ParseMandatory("softGradient",
                                         "soft gradient",
                                         out TimeSpan SoftGradient,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData      [optional]

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


                DERGradient = new DERGradient(
                                  Priority,
                                  Gradient,
                                  SoftGradient,
                                  CustomData
                              );

                if (CustomDERGradientParser is not null)
                    DERGradient = CustomDERGradientParser(JSON,
                                                          DERGradient);

                return true;

            }
            catch (Exception e)
            {
                DERGradient    = default;
                ErrorResponse  = "The given JSON representation of DERGradient is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomDERGradientSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDERGradientSerializer">A delegate to serialize custom DERGradient.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERGradient>?  CustomDERGradientSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priority",       Priority),
                                 new JProperty("gradient",       Gradient.    TotalSeconds),
                                 new JProperty("softGradient",   SoftGradient.TotalSeconds),

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDERGradientSerializer is not null
                       ? CustomDERGradientSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DERGradient1, DERGradient2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERGradient1">DERGradient.</param>
        /// <param name="DERGradient2">Another DERGradient.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERGradient? DERGradient1,
                                           DERGradient? DERGradient2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DERGradient1, DERGradient2))
                return true;

            // If one is null, but not both, return false.
            if (DERGradient1 is null || DERGradient2 is null)
                return false;

            return DERGradient1.Equals(DERGradient2);

        }

        #endregion

        #region Operator != (DERGradient1, DERGradient2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERGradient1">DERGradient.</param>
        /// <param name="DERGradient2">Another DERGradient.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERGradient? DERGradient1,
                                           DERGradient? DERGradient2)

            => !(DERGradient1 == DERGradient2);

        #endregion

        #endregion

        #region IEquatable<DERGradient> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DERGradient for equality..
        /// </summary>
        /// <param name="Object">DERGradient to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERGradient derGradient &&
                   Equals(derGradient);

        #endregion

        #region Equals(DERGradient)

        /// <summary>
        /// Compares two DERGradient for equality.
        /// </summary>
        /// <param name="DERGradient">DERGradient to compare with.</param>
        public Boolean Equals(DERGradient? DERGradient)

            => DERGradient is not null &&

               Priority.    Equals(DERGradient.Priority)     &&
               Gradient.    Equals(DERGradient.Gradient)     &&
               SoftGradient.Equals(DERGradient.SoftGradient) &&

               base.        Equals(DERGradient);

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

            => $"Priority '{Priority}': {Math.Round(Gradient.TotalSeconds, 2)} seconds / {Math.Round(SoftGradient.TotalSeconds, 2)} seconds";

        #endregion

    }

}
