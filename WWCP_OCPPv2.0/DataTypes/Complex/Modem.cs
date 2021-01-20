/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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
    /// A wireless communication module.
    /// </summary>
    public class Modem
    {

        #region Properties

        /// <summary>
        /// The ICCID of the modem’s SIM card. 20
        /// </summary>
        public String      ICCID         { get; }

        /// <summary>
        /// The IMSI of the modem’s SIM card. 20
        /// </summary>
        public String      IMSI          { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData  CustomData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new wireless communication module.
        /// </summary>
        /// <param name="ICCID">The integrated circuit card identifier of the modem’s SIM card.</param>
        /// <param name="IMSI">The IMSI of the modem’s SIM card.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Modem(String      ICCID,
                     String      IMSI,
                     CustomData  CustomData  = null)
        {

            this.ICCID       = ICCID;
            this.IMSI        = IMSI;
            this.CustomData  = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ModemType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "Wireless_ Communication_ Module\r\nurn:x-oca:ocpp:uid:2:233306\r\nDefines parameters required for initiating and maintaining wireless communication with other devices.\r\n",
        //   "javaType": "Modem",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "iccid": {
        //       "description": "Wireless_ Communication_ Module. ICCID. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569327\r\nThis contains the ICCID of the modem’s SIM card.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "imsi": {
        //       "description": "Wireless_ Communication_ Module. IMSI. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569328\r\nThis contains the IMSI of the modem’s SIM card.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (ModemJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="ModemJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Modem Parse(JObject              ModemJSON,
                                  OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(ModemJSON,
                         out Modem modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (ModemText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="ModemText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Modem Parse(String               ModemText,
                                  OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(ModemText,
                         out Modem modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(ModemJSON, out Modem, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="ModemJSON">The JSON to be parsed.</param>
        /// <param name="Modem">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              ModemJSON,
                                       out Modem            Modem,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                Modem = default;

                #region ICCID

                if (ModemJSON.ParseOptional("iccid",
                                            "integrated circuit card identifier",
                                            out String  ICCID,
                                            out String  ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region IMSI

                if (ModemJSON.ParseOptional("imsi",
                                            "international mobile subscriber identity",
                                            out String  IMSI,
                                            out         ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region CustomData

                if (ModemJSON.ParseOptionalJSON("customData",
                                                "custom data",
                                                OCPPv2_0.CustomData.TryParse,
                                                out CustomData  CustomData,
                                                out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                Modem = new Modem(ICCID?.Trim(),
                                  IMSI?. Trim(),
                                  CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ModemJSON, e);

                Modem = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ModemText, out Modem, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="ModemText">The text to be parsed.</param>
        /// <param name="Modem">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               ModemText,
                                       out Modem            Modem,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ModemText = ModemText?.Trim();

                if (ModemText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(ModemText),
                             out Modem,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ModemText, e);
            }

            Modem = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomModemResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomModemResponseSerializer">A delegate to serialize custom Modems.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Modem>      CustomModemResponseSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData> CustomCustomDataResponseSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           IMSI != null
                               ? new JProperty("iccid",       ICCID)
                               : null,

                           IMSI != null
                               ? new JProperty("imsi",        IMSI)
                               : null,

                           CustomData != null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomModemResponseSerializer != null
                       ? CustomModemResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Modem1, Modem2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Modem1">An id tag info.</param>
        /// <param name="Modem2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Modem Modem1, Modem Modem2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Modem1, Modem2))
                return true;

            // If one is null, but not both, return false.
            if (Modem1 is null || Modem2 is null)
                return false;

            if (Modem1 is null)
                throw new ArgumentNullException(nameof(Modem1),  "The given id tag info must not be null!");

            return Modem1.Equals(Modem2);

        }

        #endregion

        #region Operator != (Modem1, Modem2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Modem1">An id tag info.</param>
        /// <param name="Modem2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Modem Modem1, Modem Modem2)
            => !(Modem1 == Modem2);

        #endregion

        #endregion

        #region IEquatable<Modem> Members

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

            if (!(Object is Modem Modem))
                return false;

            return Equals(Modem);

        }

        #endregion

        #region Equals(Modem)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="Modem">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Modem Modem)
        {

            if (Modem is null)
                return false;

            return ((ICCID      == null && Modem.ICCID      == null) ||
                    (ICCID      != null && Modem.ICCID      != null && ICCID.     Equals(Modem.ICCID))) &&

                   ((IMSI       == null && Modem.IMSI       == null) ||
                    (IMSI       != null && Modem.IMSI       != null && IMSI.      Equals(Modem.IMSI)))  &&

                   ((CustomData == null && Modem.CustomData == null) ||
                    (CustomData != null && Modem.CustomData != null && CustomData.Equals(Modem.CustomData)));

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

                return (ICCID != null
                            ? ICCID.GetHashCode() * 5
                            : 0) ^

                       (IMSI != null
                            ? IMSI.GetHashCode()  * 3
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

            => new String[] {
                   ICCID != null ? "ICCID: " + ICCID : null,
                   IMSI  != null ? "IMSI: "  + IMSI  : null
               }.
               SafeWhere(_ => _ != null).
               AggregateWith(", ");

        #endregion

    }

}
