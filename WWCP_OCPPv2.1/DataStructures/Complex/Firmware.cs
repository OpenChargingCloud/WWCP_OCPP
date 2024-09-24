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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A firmware.
    /// </summary>
    public class Firmware : ACustomData,
                            IEquatable<Firmware>
    {

        #region Properties

        /// <summary>
        /// The URL where to download the firmware.
        /// </summary>
        [Mandatory]
        public URL        FirmwareURL           { get; }

        /// <summary>
        /// The timestamp at which the charging station shall retrieve the firmware image.
        /// </summary>
        [Mandatory]
        public DateTime   RetrieveTimestamp     { get; }

        /// <summary>
        /// The timestamp at which the charging station shall installed the firmware image.
        /// </summary>
        [Optional]
        public DateTime?  InstallTimestamp      { get; }

        /// <summary>
        /// The optional PEM encoded X.509 firmware signing certificate.
        /// [max 5500]
        /// </summary>
        public String?    SigningCertificate    { get; }

        /// <summary>
        /// The optional base64 encoded firmware signature.
        /// [max 800]
        /// </summary>
        public String?    Signature             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new firmware.
        /// </summary>
        /// <param name="FirmwareURL">The URL where to download the firmware image.</param>
        /// <param name="RetrieveTimestamp">The timestamp at which the charging station shall retrieve the firmware image.</param>
        /// <param name="InstallTimestamp">The timestamp at which the charging station shall installed the firmware image.</param>
        /// <param name="SigningCertificate">The optional PEM encoded X.509 firmware signing certificate.</param>
        /// <param name="Signature">The optional base64 encoded firmware signature.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Firmware(URL          FirmwareURL,
                        DateTime     RetrieveTimestamp,
                        DateTime?    InstallTimestamp     = null,
                        String?      SigningCertificate   = null,
                        String?      Signature            = null,
                        CustomData?  CustomData           = null)

            : base(CustomData)

        {

            this.FirmwareURL         = FirmwareURL;
            this.RetrieveTimestamp   = RetrieveTimestamp;
            this.InstallTimestamp    = InstallTimestamp;
            this.SigningCertificate  = SigningCertificate;
            this.Signature           = Signature;

        }

        #endregion


        #region Documentation

        // "FirmwareType": {
        //   "description": "Firmware\r\nurn:x-enexis:ecdm:uid:2:233291\r\nRepresents a copy of the firmware that can be loaded/updated on the Charging Station.",
        //   "javaType": "Firmware",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "location": {
        //       "description": "Firmware. Location. URI\r\nurn:x-enexis:ecdm:uid:1:569460\r\nURI defining the origin of the firmware.",
        //       "type": "string",
        //       "maxLength": 512
        //     },
        //     "retrieveDateTime": {
        //       "description": "Firmware. Retrieve. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569461\r\nDate and time at which the firmware shall be retrieved.",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "installDateTime": {
        //       "description": "Firmware. Install. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569462\r\nDate and time at which the firmware shall be installed.",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "signingCertificate": {
        //       "description": "Certificate with which the firmware was signed.\r\nPEM encoded X.509 certificate.",
        //       "type": "string",
        //       "maxLength": 5500
        //     },
        //     "signature": {
        //       "description": "Firmware. Signature. Signature\r\nurn:x-enexis:ecdm:uid:1:569464\r\nBase64 encoded firmware signature.",
        //       "type": "string",
        //       "maxLength": 800
        //     }
        //   },
        //   "required": [
        //     "location",
        //     "retrieveDateTime"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomFirmwareParser = null)

        /// <summary>
        /// Parse the given JSON representation of a firmware.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFirmwareParser">A delegate to parse custom firmware JSON objects.</param>
        public static Firmware Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<Firmware>?  CustomFirmwareParser   = null)
        {

            if (TryParse(JSON,
                         out var firmware,
                         out var errorResponse,
                         CustomFirmwareParser) &&
                firmware is not null)
            {
                return firmware;
            }

            throw new ArgumentException("The given JSON representation of a firmware is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Firmware, CustomFirmwareParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a firmware.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Firmware">The parsed firmware.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject        JSON,
                                       out Firmware?  Firmware,
                                       out String?    ErrorResponse)

            => TryParse(JSON,
                        out Firmware,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a firmware.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Firmware">The parsed firmware.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirmwareParser">A delegate to parse custom firmware JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       out Firmware?                           Firmware,
                                       out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<Firmware>?  CustomFirmwareParser)
        {

            try
            {

                Firmware = default;

                #region FirmwareURL           [mandatory]

                if (!JSON.ParseMandatory("location",
                                         "firmware download URL",
                                         URL.TryParse,
                                         out URL FirmwareURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region RetrieveTimestamp     [mandatory]

                if (!JSON.ParseMandatory("retrieveDateTime",
                                         "retrieve timestamp",
                                         out DateTime RetrieveTimestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region InstallTimestamp      [optional]

                if (!JSON.ParseOptional("installDateTime",
                                        "install timestamp",
                                        out DateTime? InstallTimestamp,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SigningCertificate    [optional]

                var SigningCertificate  = JSON.GetString("signingCertificate");

                #endregion

                #region Signature             [optional]

                var Signature           = JSON.GetString("signature");

                #endregion

                #region CustomData            [optional]

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


                Firmware = new Firmware(FirmwareURL,
                                        RetrieveTimestamp,
                                        InstallTimestamp,
                                        SigningCertificate,
                                        Signature,
                                        CustomData);

                if (CustomFirmwareParser is not null)
                    Firmware = CustomFirmwareParser(JSON,
                                                    Firmware);

                return true;

            }
            catch (Exception e)
            {
                Firmware       = default;
                ErrorResponse  = "The given JSON representation of a firmware is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFirmwareSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareSerializer">A delegate to serialize custom firmwares.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Firmware>?    CustomFirmwareSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("location",             FirmwareURL.           ToString()),
                                 new JProperty("retrieveDateTime",     RetrieveTimestamp.     ToIso8601()),

                           InstallTimestamp.HasValue
                               ? new JProperty("installDateTime",      InstallTimestamp.Value.ToIso8601())
                               : null,

                           SigningCertificate is not null
                               ? new JProperty("signingCertificate",   SigningCertificate)
                               : null,

                           Signature is not null
                               ? new JProperty("signature",            Signature)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFirmwareSerializer is not null
                       ? CustomFirmwareSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Firmware1, Firmware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Firmware1">A firmware.</param>
        /// <param name="Firmware2">Another firmware.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Firmware? Firmware1,
                                           Firmware? Firmware2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Firmware1, Firmware2))
                return true;

            // If one is null, but not both, return false.
            if (Firmware1 is null || Firmware2 is null)
                return false;

            return Firmware1.Equals(Firmware2);

        }

        #endregion

        #region Operator != (Firmware1, Firmware2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Firmware1">A firmware.</param>
        /// <param name="Firmware2">Another firmware.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Firmware? Firmware1,
                                           Firmware? Firmware2)

            => !(Firmware1 == Firmware2);

        #endregion

        #endregion

        #region IEquatable<Firmware> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two firmwares for equality.
        /// </summary>
        /// <param name="Object">A firmware to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Firmware firmware &&
                   Equals(firmware);

        #endregion

        #region Equals(Firmware)

        /// <summary>
        /// Compares two firmwares for equality.
        /// </summary>
        /// <param name="Firmware">A firmware to compare with.</param>
        public Boolean Equals(Firmware? Firmware)

            => Firmware is not null &&

               FirmwareURL.      Equals(Firmware.FirmwareURL)       &&
               RetrieveTimestamp.Equals(Firmware.RetrieveTimestamp) &&

            ((!InstallTimestamp.  HasValue    && !Firmware.InstallTimestamp.  HasValue)    ||
               InstallTimestamp.  HasValue    &&  Firmware.InstallTimestamp.  HasValue    && InstallTimestamp.Value.Equals(Firmware.InstallTimestamp.Value)) &&

             ((SigningCertificate is     null &&  Firmware.SigningCertificate is     null) ||
              (SigningCertificate is not null &&  Firmware.SigningCertificate is not null && SigningCertificate.    Equals(Firmware.SigningCertificate)))    &&

             ((Signature          is     null &&  Firmware.Signature          is     null) ||
              (Signature          is not null &&  Firmware.Signature          is not null && Signature.             Equals(Firmware.Signature)))             &&

               base.  Equals(Firmware);

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

                return FirmwareURL.        GetHashCode()       * 13 ^
                       RetrieveTimestamp.  GetHashCode()       * 11 ^
                      (InstallTimestamp?.  GetHashCode() ?? 0) *  7 ^
                      (SigningCertificate?.GetHashCode() ?? 0) *  5 ^
                      (Signature?.         GetHashCode() ?? 0) *  3 ^

                       base.               GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   FirmwareURL,
                   " retrieve till ",
                   RetrieveTimestamp.ToIso8601(),

                   InstallTimestamp.HasValue
                       ? " install till "          + InstallTimestamp.Value.ToIso8601()
                       : "",

                   SigningCertificate is not null
                       ? ", signing certificate: " + SigningCertificate.SubstringMax(30)
                       : "",

                   Signature is not null
                       ? ", signature: "           + Signature.         SubstringMax(30)
                       : ""

               );

        #endregion

    }

}
