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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A wireless communication module.
    /// </summary>
    public class Modem : ACustomData,
                         IEquatable<Modem>
    {

        #region Properties

        /// <summary>
        /// The ICCID of the modem’s SIM card. 20
        /// </summary>
        [Optional]
        public String?  ICCID    { get; }

        /// <summary>
        /// The IMSI of the modem’s SIM card. 20
        /// </summary>
        [Optional]
        public String?  IMSI     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new wireless communication module.
        /// </summary>
        /// <param name="ICCID">An optional integrated circuit card identifier of the modem’s SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the modem’s SIM card.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Modem(String?      ICCID        = null,
                     String?      IMSI         = null,
                     CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.ICCID  = ICCID;
            this.IMSI   = IMSI;

        }

        #endregion


        #region Documentation

        // "ModemType": {
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

        #region (static) Parse   (JSON, CustomModemParser = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomModemParser">A delegate to parse custom modem values.</param>
        public static Modem Parse(JObject                              JSON,
                                  CustomJObjectParserDelegate<Modem>?  CustomModemParser   = null)
        {

            if (TryParse(JSON,
                         out var modem,
                         out var errorResponse,
                         CustomModemParser))
            {
                return modem!;
            }

            throw new ArgumentException("The given JSON representation of a modem is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(ModemJSON, out Modem, out ErrorResponse, CustomModemParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Modem">The parsed communication module.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out Modem?   Modem,
                                       out String?  ErrorResponse)

            => TryParse(JSON,
                        out Modem,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Modem">The parsed communication module.</param>
        /// <param name="CustomModemParser">A delegate to parse custom modem values.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       out Modem?                           Modem,
                                       out String?                          ErrorResponse,
                                       CustomJObjectParserDelegate<Modem>?  CustomModemParser   = null)
        {

            try
            {

                Modem = default;

                #region ICCID         [optional]

                if (JSON.ParseOptional("iccid",
                                       "integrated circuit card identifier",
                                       out String? ICCID,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region IMSI          [optional]

                if (JSON.ParseOptional("imsi",
                                       "international mobile subscriber identity",
                                       out String? IMSI,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Modem = new Modem(ICCID?.Trim(),
                                  IMSI?. Trim(),
                                  CustomData);

                if (CustomModemParser is not null)
                    Modem = CustomModemParser(JSON,
                                              Modem);

                if (ICCID is null &&
                    IMSI  is null)
                {
                    Modem = null;
                }

                return true;

            }
            catch (Exception e)
            {
                Modem          = default;
                ErrorResponse  = "The given JSON representation of a modem is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomModemSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomModemSerializer">A delegate to serialize custom modems.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Modem>?       CustomModemSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           IMSI is not null
                               ? new JProperty("iccid",       ICCID)
                               : null,

                           IMSI is not null
                               ? new JProperty("imsi",        IMSI)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomModemSerializer is not null
                       ? CustomModemSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Modem1, Modem2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Modem1">A modem.</param>
        /// <param name="Modem2">Another modem.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Modem? Modem1,
                                           Modem? Modem2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Modem1, Modem2))
                return true;

            // If one is null, but not both, return false.
            if (Modem1 is null || Modem2 is null)
                return false;

            return Modem1.Equals(Modem2);

        }

        #endregion

        #region Operator != (Modem1, Modem2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Modem1">A modem.</param>
        /// <param name="Modem2">Another modem.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Modem? Modem1,
                                           Modem? Modem2)

            => !(Modem1 == Modem2);

        #endregion

        #endregion

        #region IEquatable<Modem> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two modems for equality.
        /// </summary>
        /// <param name="Object">A modem to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Modem modem &&
                   Equals(modem);

        #endregion

        #region Equals(Modem)

        /// <summary>
        /// Compares two modems for equality.
        /// </summary>
        /// <param name="Modem">A modem to compare with.</param>
        public Boolean Equals(Modem? Modem)

            => Modem is not null &&

             ((ICCID is     null && Modem.ICCID is     null) ||
              (ICCID is not null && Modem.ICCID is not null && ICCID.Equals(Modem.ICCID))) &&

             ((IMSI  is     null && Modem.IMSI  is     null) ||
              (IMSI  is not null && Modem.IMSI  is not null && IMSI. Equals(Modem.IMSI)))  &&

               base.Equals(Modem);

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

                return (ICCID?.GetHashCode() ?? 0) * 5 ^
                       (IMSI?. GetHashCode() ?? 0) * 3 ^

                       base.   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new String?[] {

                    ICCID is not null
                        ? "ICCID: " + ICCID
                        : null,

                    IMSI  is not null
                        ? "IMSI: "  + IMSI
                        : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
