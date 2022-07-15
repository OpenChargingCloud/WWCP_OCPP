/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A electric vehicle supply equipment (EVSE).
    /// </summary>
    public class EVSE
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

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData     CustomData     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new electric vehicle supply equipment (EVSE).
        /// </summary>
        /// <param name="Id">The EVSE identification at a charging station.</param>
        /// <param name="ConnectorId">The connector identification at an EVSE.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public EVSE(EVSE_Id        Id,
                    Connector_Id?  ConnectorId   = null,
                    CustomData     CustomData    = null)
        {

            this.Id           = Id;
            this.ConnectorId  = ConnectorId;
            this.CustomData   = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:EVSEType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": EVSE\r\nurn:x-oca:ocpp:uid:2:233123\r\nElectric Vehicle Supply Equipment\r\n",
        //   "javaType": "EVSE",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "id": {
        //       "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nEVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.\r\n",
        //       "type": "integer"
        //     },
        //     "connectorId": {
        //       "description": "An id to designate a specific connector (on an EVSE) by connector index number.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "id"
        //   ]
        // }

        #endregion

        #region (static) Parse   (EVSEJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="EVSEJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static EVSE Parse(JObject              EVSEJSON,
                                 OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(EVSEJSON,
                         out EVSE evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (EVSEText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="EVSEText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static EVSE Parse(String               EVSEText,
                                 OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(EVSEText,
                         out EVSE evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(EVSEJSON, out EVSE, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="EVSEJSON">The JSON to be parsed.</param>
        /// <param name="EVSE">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              EVSEJSON,
                                       out EVSE             EVSE,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                EVSE = default;

                #region EVSEId

                if (!EVSEJSON.ParseMandatory("id",
                                             "evse identification",
                                             EVSE_Id.TryParse,
                                             out EVSE_Id  EVSEId,
                                             out String   ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId

                if (EVSEJSON.ParseOptionalStruct("connectorId",
                                                 "connector identification",
                                                 Connector_Id.TryParse,
                                                 out Connector_Id?  ConnectorId,
                                                 out                ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region CustomData

                if (EVSEJSON.ParseOptionalJSON("customData",
                                               "custom data",
                                               OCPPv2_0.CustomData.TryParse,
                                               out CustomData  CustomData,
                                               out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                EVSE = new EVSE(EVSEId,
                                ConnectorId,
                                CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, EVSEJSON, e);

                EVSE = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(EVSEText, out EVSE, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="EVSEText">The text to be parsed.</param>
        /// <param name="EVSE">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               EVSEText,
                                       out EVSE             EVSE,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                EVSEText = EVSEText?.Trim();

                if (EVSEText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(EVSEText),
                             out EVSE,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, EVSEText, e);
            }

            EVSE = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomEVSEResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVSEResponseSerializer">A delegate to serialize custom EVSE objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVSE>       CustomEVSEResponseSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData> CustomCustomDataResponseSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("id",                 Id.ToString()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",  ConnectorId.Value.ToString())
                               : null,

                           CustomData != null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomEVSEResponseSerializer != null
                       ? CustomEVSEResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An id tag info.</param>
        /// <param name="EVSE2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSE EVSE1, EVSE EVSE2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVSE1, EVSE2))
                return true;

            // If one is null, but not both, return false.
            if (EVSE1 is null || EVSE2 is null)
                return false;

            if (EVSE1 is null)
                throw new ArgumentNullException(nameof(EVSE1),  "The given id tag info must not be null!");

            return EVSE1.Equals(EVSE2);

        }

        #endregion

        #region Operator != (EVSE1, EVSE2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE1">An id tag info.</param>
        /// <param name="EVSE2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSE EVSE1, EVSE EVSE2)
            => !(EVSE1 == EVSE2);

        #endregion

        #endregion

        #region IEquatable<EVSE> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is EVSE EVSE))
                return false;

            return Equals(EVSE);

        }

        #endregion

        #region Equals(EVSE)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="EVSE">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EVSE EVSE)
        {

            if (EVSE is null)
                return false;

            return Id.Equals(EVSE.Id) &&

                   ((!ConnectorId.HasValue && !EVSE.ConnectorId.HasValue) ||
                     (ConnectorId.HasValue &&  EVSE.ConnectorId.HasValue && ConnectorId.Value.Equals(EVSE.ConnectorId.Value))) &&

                   ((CustomData == null    &&  EVSE.CustomData == null) ||
                    (CustomData != null    &&  EVSE.CustomData != null   && CustomData.Equals(EVSE.CustomData)));

        }

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

                return Id.GetHashCode() * 5 ^

                       (ConnectorId.HasValue
                            ? ConnectorId.GetHashCode() * 3
                            : 0) ^

                       (CustomData != null
                            ? CustomData.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Id, ConnectorId.HasValue ? " (" + ConnectorId.Value + ")" : "");

        #endregion

    }

}
