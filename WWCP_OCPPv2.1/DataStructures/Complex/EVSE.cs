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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A electric vehicle supply equipment (EVSE).
    /// </summary>
    public class EVSE : ACustomData,
                        IEquatable<EVSE>
    {

        #region Properties

        /// <summary>
        /// The EVSE identification at a charging station.
        /// </summary>
        public EVSE_Id        Id             { get; }

        /// <summary>
        /// The connector identification at an EVSE.
        /// </summary>
        public Connector_Id?  ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new electric vehicle supply equipment (EVSE).
        /// </summary>
        /// <param name="Id">The EVSE identification at a charging station.</param>
        /// <param name="ConnectorId">The connector identification at an EVSE.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public EVSE(EVSE_Id        Id,
                    Connector_Id?  ConnectorId   = null,
                    CustomData?    CustomData    = null)

            : base(CustomData)

        {

            this.Id           = Id;
            this.ConnectorId  = ConnectorId;

            unchecked
            {

                hashCode = this.Id.          GetHashCode()       * 5 ^
                          (this.ConnectorId?.GetHashCode() ?? 0) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Electric Vehicle Supply Equipment",
        //     "javaType": "EVSE",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "id": {
        //             "description": "EVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "connectorId": {
        //             "description": "An id to designate a specific connector (on an EVSE) by connector index number.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomEVSEParser = null)

        /// <summary>
        /// Parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSEs.</param>
        public static EVSE Parse(JObject                             JSON,
                                 CustomJObjectParserDelegate<EVSE>?  CustomEVSEParser   = null)
        {

            if (TryParse(JSON,
                         out var evse,
                         out var errorResponse,
                         CustomEVSEParser) &&
                evse is not null)
            {
                return evse;
            }

            throw new ArgumentException("The given JSON representation of an EVSE is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVSE, out ErrorResponse, CustomEVSEParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVSE">The parsed EVSE.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out EVSE?    EVSE,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out EVSE,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an EVSE.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVSE">The parsed EVSE.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSEs.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out EVSE?      EVSE,
                                       [NotNullWhen(false)] out String?    ErrorResponse,
                                       CustomJObjectParserDelegate<EVSE>?  CustomEVSEParser)
        {

            try
            {

                EVSE = default;

                #region EVSEId         [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "evse identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId    [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? ConnectorId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

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


                EVSE = new EVSE(
                           EVSEId,
                           ConnectorId,
                           CustomData
                       );

                if (CustomEVSEParser is not null)
                    EVSE = CustomEVSEParser(JSON,
                                            EVSE);

                return true;

            }
            catch (Exception e)
            {
                EVSE           = default;
                ErrorResponse  = "The given JSON representation of an EVSE is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEVSESerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVSE>?        CustomEVSESerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",            Id.               Value),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",   ConnectorId.Value.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVSESerializer is not null
                       ? CustomEVSESerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public EVSE Clone()

            => new (
                   Id.          Clone(),
                   ConnectorId?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE? EVSE1,
                                           EVSE? EVSE2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVSE1, EVSE2))
                return true;

            // If one is null, but not both, return false.
            if (EVSE1 is null || EVSE2 is null)
                return false;

            return EVSE1.Equals(EVSE2);

        }

        #endregion

        #region Operator != (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An EVSE.</param>
        /// <param name="EVSE2">Another EVSE.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSE? EVSE1,
                                           EVSE? EVSE2)

            => !(EVSE1 == EVSE2);

        #endregion

        #endregion

        #region IEquatable<EVSE> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EVSEs for equality.
        /// </summary>
        /// <param name="Object">An EVSE to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSE evse &&
                   Equals(evse);

        #endregion

        #region Equals(EVSE)

        /// <summary>
        /// Compares two EVSEs for equality.
        /// </summary>
        /// <param name="EVSE">An EVSE to compare with.</param>
        public Boolean Equals(EVSE? EVSE)

            => EVSE is not null &&

               Id.  Equals(EVSE.Id) &&

            ((!ConnectorId.HasValue && !EVSE.ConnectorId.HasValue) ||
              (ConnectorId.HasValue &&  EVSE.ConnectorId.HasValue && ConnectorId.Value.Equals(EVSE.ConnectorId.Value))) &&

               base.Equals(EVSE);

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

            => ConnectorId.HasValue
                   ? $"{Id} ({ConnectorId.Value})"
                   : Id.ToString();

        #endregion

    }

}
