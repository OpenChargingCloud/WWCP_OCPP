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
    /// A charging station is a physical system where an electrical vehicle can be charged.
    /// </summary>
    public class ChargingStation : ACustomData,
                                   IEquatable<ChargingStation>
    {

        #region Properties

        /// <summary>
        /// The model of the charging station. 20
        /// </summary>
        [Mandatory]
        public String   Model              { get; }

        /// <summary>
        /// The vendor name/identification (not necessarily unique). 50
        /// </summary>
        [Mandatory]
        public String   VendorName         { get; }

        /// <summary>
        /// A vendor-specific charging station identifier.
        /// It must match the "Common Name" within the TLS client certificate.   25
        /// </summary>
        public String?  SerialNumber       { get; }

        /// <summary>
        /// The wireless communication module.
        /// </summary>
        public Modem?   Modem              { get; }

        /// <summary>
        /// The firmware version of the charging station. 50
        /// </summary>
        public String?  FirmwareVersion    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station.
        /// </summary>
        /// <param name="Model">The model of the charging station.</param>
        /// <param name="VendorName">The vendor name/identification (not necessarily unique).</param>
        /// <param name="SerialNumber">An optional vendor-specific charging station identifier.</param>
        /// <param name="Modem">An optional wireless communication module.</param>
        /// <param name="FirmwareVersion">The optional firmware version of the charging station.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public ChargingStation(String       Model,
                               String       VendorName,
                               String?      SerialNumber      = null,
                               Modem?       Modem             = null,
                               String?      FirmwareVersion   = null,
                               CustomData?  CustomData        = null)

            : base(CustomData)

        {

            this.Model            = Model;
            this.VendorName       = VendorName;
            this.SerialNumber     = SerialNumber;
            this.Modem            = Modem;
            this.FirmwareVersion  = FirmwareVersion;

        }

        #endregion


        #region Documentation

        // "ChargingStationType": {
        //   "description": "Charge_ Point\r\nurn:x-oca:ocpp:uid:2:233122\r\nThe physical system where an Electrical Vehicle (EV) can be charged.\r\n",
        //   "javaType": "ChargingStation",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "serialNumber": {
        //       "description": "Device. Serial_ Number. Serial_ Number\r\nurn:x-oca:ocpp:uid:1:569324\r\nVendor-specific device identifier.\r\n",
        //       "type": "string",
        //       "maxLength": 25
        //     },
        //     "model": {
        //       "description": "Device. Model. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569325\r\nDefines the model of the device.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "modem": {
        //       "$ref": "#/definitions/ModemType"
        //     },
        //     "vendorName": {
        //       "description": "Identifies the vendor (not necessarily in a unique manner).\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "firmwareVersion": {
        //       "description": "This contains the firmware version of the Charging Station.\r\n\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     }
        //   },
        //   "required": [
        //     "model",
        //     "vendorName"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomChargingStationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging station.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingStationParser">A delegate to parse custom charging stations.</param>
        public static ChargingStation Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<ChargingStation>?  CustomChargingStationParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingStation,
                         out var errorResponse,
                         CustomChargingStationParser))
            {
                return chargingStation!;
            }

            throw new ArgumentException("The given JSON representation of a charging station is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingStation, out ErrorResponse, CustomChargingStationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a custom data object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingStation">The parsed charging station.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject               JSON,
                                       out ChargingStation?  ChargingStation,
                                       out String?           ErrorResponse)

            => TryParse(JSON,
                        out ChargingStation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a custom data object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingStation">The parsed charging station.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingStationParser">A delegate to parse custom charging stations.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       out ChargingStation?                           ChargingStation,
                                       out String?                                    ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingStation>?  CustomChargingStationParser)
        {

            try
            {

                ChargingStation = default;

                #region Model              [mandatory]

                if (!JSON.ParseMandatoryText("model",
                                             "charging station model",
                                             out String Model,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VendorName         [mandatory]

                if (!JSON.ParseMandatoryText("vendorName",
                                             "vendor name/identification",
                                             out String VendorName,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SerialNumber       [optional]

                var SerialNumber = JSON.GetOptional("serialNumber");

                #endregion

                #region Modem              [optional]

                if (JSON.ParseOptionalJSON("Modem",
                                           "wireless communication module",
                                           OCPPv2_0.Modem.TryParse,
                                           out Modem? Modem,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region FirmwareVersion    [optional]

                var FirmwareVersion = JSON.GetOptional("firmwareVersion");

                #endregion

                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingStation = new ChargingStation(Model,
                                                      VendorName,
                                                      SerialNumber,
                                                      Modem,
                                                      FirmwareVersion,
                                                      CustomData);

                if (CustomChargingStationParser is not null)
                    ChargingStation = CustomChargingStationParser(JSON,
                                                                  ChargingStation);

                return true;

            }
            catch (Exception e)
            {
                ChargingStation  = default;
                ErrorResponse    = "The given JSON representation of a charging station is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingStationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingStationResponseSerializer">A delegate to serialize custom charging stations.</param>
        /// <param name="CustomModemResponseSerializer">A delegate to serialize custom modems.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingStation>?  CustomChargingStationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Modem>?            CustomModemResponseSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataResponseSerializer        = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("model",                  Model),
                           new JProperty("vendorName",             VendorName),

                           SerialNumber    is not null
                               ? new JProperty("serialNumber",     SerialNumber)
                               : null,

                           Modem           is not null
                               ? new JProperty("serialNumber",     Modem.     ToJSON(CustomModemResponseSerializer,
                                                                                     CustomCustomDataResponseSerializer))
                               : null,

                           FirmwareVersion is not null
                               ? new JProperty("firmwareVersion",  FirmwareVersion)
                               : null,

                           CustomData      is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomChargingStationResponseSerializer is not null
                       ? CustomChargingStationResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An id tag info.</param>
        /// <param name="ChargingStation2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingStation? ChargingStation1,
                                           ChargingStation? ChargingStation2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingStation1, ChargingStation2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingStation1 is null || ChargingStation2 is null)
                return false;

            return ChargingStation1.Equals(ChargingStation2);

        }

        #endregion

        #region Operator != (ChargingStation1, ChargingStation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation1">An id tag info.</param>
        /// <param name="ChargingStation2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingStation? ChargingStation1,
                                           ChargingStation? ChargingStation2)

            => !(ChargingStation1 == ChargingStation2);

        #endregion

        #endregion

        #region IEquatable<ChargingStation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging stations for equality.
        /// </summary>
        /// <param name="Object">A charging station to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingStation chargingStation &&
                   Equals(chargingStation);

        #endregion

        #region Equals(ChargingStation)

        /// <summary>
        /// Compares two charging stations for equality.
        /// </summary>
        /// <param name="ChargingStation">A charging station to compare with.</param>
        public Boolean Equals(ChargingStation? ChargingStation)

            => ChargingStation is not null &&

               Model.     Equals(ChargingStation.Model)      &&
               VendorName.Equals(ChargingStation.VendorName) &&

             ((CustomData      is     null && ChargingStation.CustomData      is     null) ||
              (CustomData      is not null && ChargingStation.CustomData      is not null && CustomData.     Equals(ChargingStation.CustomData)))      &&

             ((SerialNumber    is     null && ChargingStation.SerialNumber    is     null) ||
              (SerialNumber    is not null && ChargingStation.SerialNumber    is not null && SerialNumber.   Equals(ChargingStation.SerialNumber)))    &&

             ((Modem           is     null && ChargingStation.Modem           is     null) ||
              (Modem           is not null && ChargingStation.Modem           is not null && Modem.          Equals(ChargingStation.Modem)))           &&

             ((FirmwareVersion is     null && ChargingStation.FirmwareVersion is     null) ||
              (FirmwareVersion is not null && ChargingStation.FirmwareVersion is not null && FirmwareVersion.Equals(ChargingStation.FirmwareVersion))) &&

               base.      Equals(ChargingStation);

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

                return Model.           GetHashCode()       * 13 ^
                       VendorName.      GetHashCode()       * 11 ^

                      (SerialNumber?.   GetHashCode() ?? 0) *  7 ^
                      (Modem?.          GetHashCode() ?? 0) *  5 ^
                      (FirmwareVersion?.GetHashCode() ?? 0) *  3 ^

                      base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("model: ",         Model,
                             ", vendor name: ", VendorName,
                             SerialNumber    is not null ? ", serial number: "    + SerialNumber    : "",
                             FirmwareVersion is not null ? ", firmware version: " + FirmwareVersion : "");

        #endregion

    }

}
