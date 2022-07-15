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

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    // After start-up, every charge point SHALL send a request to the
    // central system with information about its configuration
    // (e.g.version, vendor, etc.).

    /// <summary>
    /// A boot notification request.
    /// </summary>
    public class BootNotificationRequest : ARequest<BootNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// A physical system where an electrical vehicle (EV) can be charged.
        /// </summary>
        [Mandatory]
        public ChargingStation  ChargingStation    { get; }

        /// <summary>
        /// The the reason for sending this boot notification to the CSMS.
        /// </summary>
        [Mandatory]
        public BootReasons      Reason             { get; }

        /// <summary>
        /// The custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData       CustomData         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a boot notification request.
        /// </summary>
        /// <param name="ChargingStation">A physical system where an electrical vehicle (EV) can be charged.</param>
        /// <param name="Reason">The the reason for sending this boot notification to the CSMS.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public BootNotificationRequest(ChargingStation  ChargingStation,
                                       BootReasons      Reason,
                                       CustomData       CustomData   = null)
        {

            this.ChargingStation  = ChargingStation;
            this.Reason           = Reason;
            this.CustomData       = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:BootNotificationRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "BootReasonEnumType": {
        //       "description": "This contains the reason for sending this message to the CSMS.\r\n",
        //       "javaType": "BootReasonEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ApplicationReset",
        //         "FirmwareUpdate",
        //         "LocalReset",
        //         "PowerUp",
        //         "RemoteReset",
        //         "ScheduledReset",
        //         "Triggered",
        //         "Unknown",
        //         "Watchdog"
        //       ]
        //     },
        //     "ChargingStationType": {
        //       "description": "Charge_ Point\r\nurn:x-oca:ocpp:uid:2:233122\r\nThe physical system where an Electrical Vehicle (EV) can be charged.\r\n",
        //       "javaType": "ChargingStation",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "serialNumber": {
        //           "description": "Device. Serial_ Number. Serial_ Number\r\nurn:x-oca:ocpp:uid:1:569324\r\nVendor-specific device identifier.\r\n",
        //           "type": "string",
        //           "maxLength": 25
        //         },
        //         "model": {
        //           "description": "Device. Model. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569325\r\nDefines the model of the device.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "modem": {
        //           "$ref": "#/definitions/ModemType"
        //         },
        //         "vendorName": {
        //           "description": "Identifies the vendor (not necessarily in a unique manner).\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "firmwareVersion": {
        //           "description": "This contains the firmware version of the Charging Station.\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "model",
        //         "vendorName"
        //       ]
        //     },
        //     "ModemType": {
        //       "description": "Wireless_ Communication_ Module\r\nurn:x-oca:ocpp:uid:2:233306\r\nDefines parameters required for initiating and maintaining wireless communication with other devices.\r\n",
        //       "javaType": "Modem",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "iccid": {
        //           "description": "Wireless_ Communication_ Module. ICCID. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569327\r\nThis contains the ICCID of the modem’s SIM card.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "imsi": {
        //           "description": "Wireless_ Communication_ Module. IMSI. CI20_ Text\r\nurn:x-oca:ocpp:uid:1:569328\r\nThis contains the IMSI of the modem’s SIM card.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         }
        //       }
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "chargingStation": {
        //       "$ref": "#/definitions/ChargingStationType"
        //     },
        //     "reason": {
        //       "$ref": "#/definitions/BootReasonEnumType"
        //     }
        //   },
        //   "required": [
        //     "reason",
        //     "chargingStation"
        //   ]
        // }

        #endregion

        #region (static) Parse   (BootNotificationRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a boot notification request.
        /// </summary>
        /// <param name="BootNotificationRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationRequest Parse(JObject              BootNotificationRequestJSON,
                                                    OnExceptionDelegate  OnException = null)
        {


            if (TryParse(BootNotificationRequestJSON,
                         out BootNotificationRequest bootNotificationRequest,
                         OnException))
            {
                return bootNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (BootNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a boot notification request.
        /// </summary>
        /// <param name="BootNotificationRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationRequest Parse(String               BootNotificationRequestText,
                                                    OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(BootNotificationRequestText,
                         out BootNotificationRequest bootNotificationRequest,
                         OnException))
            {
                return bootNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(BootNotificationRequestJSON, out BootNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a boot notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="BootNotificationRequest">The parsed boot notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       out BootNotificationRequest  BootNotificationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                BootNotificationRequest = null;

                #region ChargingStation

                if (!JSON.ParseMandatory3("chargingStation",
                                                                 "charging station",
                                                                 ChargingStation.TryParse,
                                                                 out ChargingStation  chargingStation,
                                                                 out String           ErrorResponse,
                                                                 OnException))
                {
                    return false;
                }

                #endregion

                #region Reason

                if (!JSON.MapMandatory("reason",
                                                              "boot reason",
                                                              BootReasonsExtentions.Parse,
                                                              out BootReasons  Reason,
                                                              out              ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData  CustomData,
                                           out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                BootNotificationRequest = new BootNotificationRequest(chargingStation,
                                                                      Reason,
                                                                      CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, JSON, e);

                BootNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(BootNotificationRequestText, out BootNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a boot notification request.
        /// </summary>
        /// <param name="BootNotificationRequestText">The text to be parsed.</param>
        /// <param name="BootNotificationRequest">The parsed boot notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       BootNotificationRequestText,
                                       out BootNotificationRequest  BootNotificationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                BootNotificationRequestText = BootNotificationRequestText?.Trim();

                if (BootNotificationRequestText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(BootNotificationRequestText),
                                           out BootNotificationRequest,
                                           OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, BootNotificationRequestText, e);
            }

            BootNotificationRequest = null;
            return false;

        }

        #endregion

        #region ToJSON(CustomBootNotificationRequestSerializer = null, CustomChargingStationResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationRequestSerializer">A delegate to serialize custom boot notification requests.</param>
        /// <param name="CustomChargingStationResponseSerializer">A delegate to serialize custom ChargingStations.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BootNotificationRequest> CustomBootNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingStation>         CustomChargingStationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>              CustomCustomDataResponseSerializer        = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("chargingStation",   ChargingStation.ToJSON(CustomChargingStationResponseSerializer)),
                           new JProperty("reason",            Reason.         AsText()),

                           CustomData != null
                               ? new JProperty("customData", CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomBootNotificationRequestSerializer != null
                       ? CustomBootNotificationRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A boot notification request.</param>
        /// <param name="BootNotificationRequest2">Another boot notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationRequest BootNotificationRequest1, BootNotificationRequest BootNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationRequest1, BootNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((BootNotificationRequest1 is null) || (BootNotificationRequest2 is null))
                return false;

            return BootNotificationRequest1.Equals(BootNotificationRequest2);

        }

        #endregion

        #region Operator != (BootNotificationRequest1, BootNotificationRequest2)

        /// <summary>
        /// Compares two boot notification requests for inequality.
        /// </summary>
        /// <param name="BootNotificationRequest1">A boot notification request.</param>
        /// <param name="BootNotificationRequest2">Another boot notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BootNotificationRequest BootNotificationRequest1, BootNotificationRequest BootNotificationRequest2)

            => !(BootNotificationRequest1 == BootNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationRequest> Members

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

            if (!(Object is BootNotificationRequest BootNotificationRequest))
                return false;

            return Equals(BootNotificationRequest);

        }

        #endregion

        #region Equals(BootNotificationRequest)

        /// <summary>
        /// Compares two boot notification requests for equality.
        /// </summary>
        /// <param name="BootNotificationRequest">A boot notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(BootNotificationRequest BootNotificationRequest)
        {

            if (BootNotificationRequest is null)
                return false;

            return ChargingStation.Equals(BootNotificationRequest.ChargingStation) &&
                   Reason.         Equals(BootNotificationRequest.Reason)          &&

                   ((CustomData == null && BootNotificationRequest.CustomData == null) ||
                    (CustomData != null && BootNotificationRequest.CustomData != null && CustomData.Equals(BootNotificationRequest.CustomData)));

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

                return ChargingStation.GetHashCode() * 5 ^
                       Reason.         GetHashCode() * 3 ^

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

            => String.Concat("Boot reason: " + Reason.AsText());

        #endregion

    }

}
