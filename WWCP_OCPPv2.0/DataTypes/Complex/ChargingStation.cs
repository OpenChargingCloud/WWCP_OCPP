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
using Newtonsoft.Json;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A physical system where an electrical vehicle (EV) can be charged.
    /// </summary>
    public class ChargingStation
    {

        #region Properties

        /// <summary>
        /// The model of the charging station. 20
        /// </summary>
        [Mandatory]
        public String      Model              { get; }

        /// <summary>
        /// The vendor name/identification (not necessarily unique). 50
        /// </summary>
        [Mandatory]
        public String      VendorName         { get; }


        /// <summary>
        /// The custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData  CustomData         { get; }

        /// <summary>
        /// A vendor-specific charging station identifier.
        /// It must match the "Common Name" within the TLS client certificate.   25
        /// </summary>
        public String      SerialNumber       { get; }

        /// <summary>
        /// The wireless communication module.
        /// </summary>
        public Modem       Modem              { get; }

        /// <summary>
        /// The firmware version of the charging station. 50
        /// </summary>
        public String      FirmwareVersion    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station.
        /// </summary>
        /// <param name="Model">The model of the charging station.</param>
        /// <param name="VendorName">The vendor name/identification (not necessarily unique).</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        /// <param name="SerialNumber">An optional vendor-specific charging station identifier.</param>
        /// <param name="Modem">An optional wireless communication module.</param>
        /// <param name="FirmwareVersion">The optional firmware version of the charging station.</param>
        public ChargingStation(String      Model,
                               String      VendorName,
                               CustomData  CustomData        = null,
                               String      SerialNumber      = null,
                               Modem       Modem             = null,
                               String      FirmwareVersion   = null)
        {

            this.Model            = Model;
            this.VendorName       = VendorName;
            this.CustomData       = CustomData;
            this.SerialNumber     = SerialNumber;
            this.Modem            = Modem;
            this.FirmwareVersion  = FirmwareVersion;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ChargingStationType",
        //   "comment": "OCPP 2.0.1 FINAL",
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

        #region (static) Parse   (ChargingStationJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a custom data object.
        /// </summary>
        /// <param name="ChargingStationJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingStation Parse(JObject              ChargingStationJSON,
                                       OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(ChargingStationJSON,
                         out ChargingStation customData,
                         OnException))
            {
                return customData;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (ChargingStationText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a custom data object.
        /// </summary>
        /// <param name="ChargingStationText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingStation Parse(String               ChargingStationText,
                                       OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(ChargingStationText,
                         out ChargingStation customData,
                         OnException))
            {
                return customData;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(ChargingStationJSON, out ChargingStation, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a custom data object.
        /// </summary>
        /// <param name="ChargingStationJSON">The JSON to be parsed.</param>
        /// <param name="ChargingStation">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              ChargingStationJSON,
                                       out ChargingStation  ChargingStation,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ChargingStation = default;

                #region Model

                if (!ChargingStationJSON.ParseMandatoryText("model",
                                                            "charging station model",
                                                            out String  Model,
                                                            out String  ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VendorName

                if (!ChargingStationJSON.ParseMandatoryText("vendorName",
                                                            "vendor name/identification",
                                                            out String  VendorName,
                                                            out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

                if (ChargingStationJSON.ParseOptionalJSON("customData",
                                                          "custom data",
                                                          OCPPv2_0.CustomData.TryParse,
                                                          out CustomData  CustomData,
                                                          out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region SerialNumber

                if (!ChargingStationJSON.ParseMandatoryText("serialNumber",
                                                            "vendor-specific charging station identifier",
                                                            out String  SerialNumber,
                                                            out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Modem

                if (ChargingStationJSON.ParseOptionalJSON("Modem",
                                                          "wireless communication module",
                                                          OCPPv2_0.Modem.TryParse,
                                                          out Modem  Modem,
                                                          out        ErrorResponse,
                                                          OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region FirmwareVersion

                if (!ChargingStationJSON.ParseMandatoryText("firmwareVersion",
                                                            "firmware version",
                                                            out String  FirmwareVersion,
                                                            out         ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChargingStation = new ChargingStation(Model,
                                                      VendorName,
                                                      CustomData,
                                                      SerialNumber,
                                                      Modem,
                                                      FirmwareVersion);

                return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ChargingStationJSON, e);
                ChargingStation = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(ChargingStationText, out ChargingStation, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a custom data object.
        /// </summary>
        /// <param name="ChargingStationText">The text to be parsed.</param>
        /// <param name="ChargingStation">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               ChargingStationText,
                                       out ChargingStation       ChargingStation,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ChargingStationText = ChargingStationText?.Trim();

                if (ChargingStationText.IsNotNullOrEmpty() && TryParse(JObject.Parse(ChargingStationText),
                                                                       out ChargingStation,
                                                                       OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ChargingStationText, e);
            }

            ChargingStation = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomChargingStationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingStationResponseSerializer">A delegate to serialize custom ChargingStations.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingStation> CustomChargingStationResponseSerializer = null)
        {

            var JSON = JSONObject.Create();

            return CustomChargingStationResponseSerializer != null
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
        public static Boolean operator == (ChargingStation ChargingStation1, ChargingStation ChargingStation2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingStation1, ChargingStation2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargingStation1 == null) || ((Object) ChargingStation2 == null))
                return false;

            if ((Object) ChargingStation1 == null)
                throw new ArgumentNullException(nameof(ChargingStation1),  "The given id tag info must not be null!");

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
        public static Boolean operator != (ChargingStation ChargingStation1, ChargingStation ChargingStation2)
            => !(ChargingStation1 == ChargingStation2);

        #endregion

        #endregion

        #region IEquatable<ChargingStation> Members

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

            if (!(Object is ChargingStation ChargingStation))
                return false;

            return Equals(ChargingStation);

        }

        #endregion

        #region Equals(ChargingStation)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="ChargingStation">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingStation ChargingStation)
        {

            if (ChargingStation is null)
                return false;

            return Model.     Equals(ChargingStation.Model)      &&
                   VendorName.Equals(ChargingStation.VendorName) &&

                   ((CustomData      == null && ChargingStation.CustomData      == null) ||
                    (CustomData      != null && ChargingStation.CustomData      != null && CustomData.     Equals(ChargingStation.CustomData)))   &&

                   ((SerialNumber    == null && ChargingStation.SerialNumber    == null) ||
                    (SerialNumber    != null && ChargingStation.SerialNumber    != null && SerialNumber.   Equals(ChargingStation.SerialNumber))) &&

                   ((Modem           == null && ChargingStation.Modem           == null) ||
                    (Modem           != null && ChargingStation.Modem           != null && Modem.          Equals(ChargingStation.Modem)))        &&

                   ((FirmwareVersion == null && ChargingStation.FirmwareVersion == null) ||
                    (FirmwareVersion != null && ChargingStation.FirmwareVersion != null && FirmwareVersion.Equals(ChargingStation.FirmwareVersion)));

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

                return Model.     GetHashCode() * 17 ^
                       VendorName.GetHashCode() * 13 ^

                       (CustomData != null
                            ? CustomData.     GetHashCode() * 11
                            : 0) ^

                        (SerialNumber != null
                            ? SerialNumber.   GetHashCode() *  7
                            : 0) ^

                        (Modem != null
                            ? Modem.          GetHashCode() *  5
                            : 0) ^

                        (FirmwareVersion != null
                            ? FirmwareVersion.GetHashCode() *  3
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("Model: ",      Model,
                             "VendorName: ", VendorName,
                             SerialNumber    != null ? "SerialNumber: "    + SerialNumber    : "",
                             FirmwareVersion != null ? "FirmwareVersion: " + FirmwareVersion : "");

        #endregion

    }

}
