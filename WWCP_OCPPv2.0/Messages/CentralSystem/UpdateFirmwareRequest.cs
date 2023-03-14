/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// The update firmware request.
    /// </summary>
    public class UpdateFirmwareRequest : ARequest<UpdateFirmwareRequest>
    {

        #region Properties

        /// <summary>
        /// The firmware image to be installed at the charging station.
        /// </summary>
        [Mandatory]
        public Firmware   Firmware                   { get; }

        /// <summary>
        /// The update firmware request identification.
        /// </summary>
        [Mandatory]
        public Int32      UpdateFirmwareRequestId    { get; }

        /// <summary>
        /// The optional number of retries of a charge point for trying to
        /// download the firmware before giving up. If this field is not
        /// present, it is left to the charge point to decide how many times
        /// it wants to retry.
        /// </summary>
        [Optional]
        public Byte?      Retries                    { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to charge point to decide how long to
        /// wait between attempts.
        /// </summary>
        [Optional]
        public TimeSpan?  RetryInterval              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new update firmware request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Firmware">The firmware image to be installed at the charging station.</param>
        /// <param name="UpdateFirmwareRequestId">The update firmware request identification.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public UpdateFirmwareRequest(ChargeBox_Id        ChargeBoxId,
                                     Firmware            Firmware,
                                     Int32               UpdateFirmwareRequestId,
                                     Byte?               Retries             = null,
                                     TimeSpan?           RetryInterval       = null,

                                     CustomData?         CustomData          = null,
                                     Request_Id?         RequestId           = null,
                                     DateTime?           RequestTimestamp    = null,
                                     TimeSpan?           RequestTimeout      = null,
                                     EventTracking_Id?   EventTrackingId     = null,
                                     CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "UpdateFirmware",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.Firmware                 = Firmware;
            this.UpdateFirmwareRequestId  = UpdateFirmwareRequestId;
            this.Retries                  = Retries;
            this.RetryInterval            = RetryInterval;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UpdateFirmwareRequest",
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
        //     "FirmwareType": {
        //       "description": "Firmware\r\nurn:x-enexis:ecdm:uid:2:233291\r\nRepresents a copy of the firmware that can be loaded/updated on the Charging Station.\r\n",
        //       "javaType": "Firmware",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "location": {
        //           "description": "Firmware. Location. URI\r\nurn:x-enexis:ecdm:uid:1:569460\r\nURI defining the origin of the firmware.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         },
        //         "retrieveDateTime": {
        //           "description": "Firmware. Retrieve. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569461\r\nDate and time at which the firmware shall be retrieved.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "installDateTime": {
        //           "description": "Firmware. Install. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569462\r\nDate and time at which the firmware shall be installed.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "signingCertificate": {
        //           "description": "Certificate with which the firmware was signed.\r\nPEM encoded X.509 certificate.\r\n",
        //           "type": "string",
        //           "maxLength": 5500
        //         },
        //         "signature": {
        //           "description": "Firmware. Signature. Signature\r\nurn:x-enexis:ecdm:uid:1:569464\r\nBase64 encoded firmware signature.\r\n",
        //           "type": "string",
        //           "maxLength": 800
        //         }
        //       },
        //       "required": [
        //         "location",
        //         "retrieveDateTime"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "retries": {
        //       "description": "This specifies how many times Charging Station must try to download the firmware before giving up. If this field is not present, it is left to Charging Station to decide how many times it wants to retry.\r\n",
        //       "type": "integer"
        //     },
        //     "retryInterval": {
        //       "description": "The interval in seconds after which a retry may be attempted. If this field is not present, it is left to Charging Station to decide how long to wait between attempts.\r\n",
        //       "type": "integer"
        //     },
        //     "requestId": {
        //       "description": "The Id of this request\r\n",
        //       "type": "integer"
        //     },
        //     "firmware": {
        //       "$ref": "#/definitions/FirmwareType"
        //     }
        //   },
        //   "required": [
        //     "requestId",
        //     "firmware"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomUpdateFirmwareRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an update firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomUpdateFirmwareRequestParser">A delegate to parse custom update firmware requests.</param>
        public static UpdateFirmwareRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  ChargeBox_Id                                         ChargeBoxId,
                                                  CustomJObjectParserDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var updateFirmwareRequest,
                         out var errorResponse,
                         CustomUpdateFirmwareRequestParser))
            {
                return updateFirmwareRequest!;
            }

            throw new ArgumentException("The given JSON representation of an update firmware request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out UpdateFirmwareRequest, out ErrorResponse, CustomUpdateFirmwareRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an update firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UpdateFirmwareRequest">The parsed update firmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                     JSON,
                                       Request_Id                  RequestId,
                                       ChargeBox_Id                ChargeBoxId,
                                       out UpdateFirmwareRequest?  UpdateFirmwareRequest,
                                       out String?                 ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out UpdateFirmwareRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an update firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UpdateFirmwareRequest">The parsed update firmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateFirmwareRequestParser">A delegate to parse custom update firmware requests.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       ChargeBox_Id                                         ChargeBoxId,
                                       out UpdateFirmwareRequest?                           UpdateFirmwareRequest,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestParser)
        {

            try
            {

                UpdateFirmwareRequest = null;

                #region Firmware                   [mandatory]

                if (!JSON.ParseMandatoryJSON("firmware",
                                             "firmware",
                                             OCPPv2_0.Firmware.TryParse,
                                             out Firmware? Firmware,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Firmware is null)
                    return false;

                #endregion

                #region UpdateFirmwareRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int32 UpdateFirmwareRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Retries                    [optional]

                if (JSON.ParseOptional("retries",
                                       "retries",
                                       out Byte? Retries,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RetryInterval              [optional]

                if (JSON.ParseOptional("retryInterval",
                                       "retry interval",
                                       out TimeSpan? RetryInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

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

                #region ChargeBoxId                [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                UpdateFirmwareRequest = new UpdateFirmwareRequest(ChargeBoxId,
                                                                  Firmware,
                                                                  UpdateFirmwareRequestId,
                                                                  Retries,
                                                                  RetryInterval,
                                                                  CustomData,
                                                                  RequestId);

                if (CustomUpdateFirmwareRequestParser is not null)
                    UpdateFirmwareRequest = CustomUpdateFirmwareRequestParser(JSON,
                                                                              UpdateFirmwareRequest);

                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareRequest  = null;
                ErrorResponse          = "The given JSON representation of an update firmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateFirmwareRequestSerializer = null, CustomFirmwareSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateFirmwareRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomFirmwareSerializer">A delegate to serialize custom firmwares.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Firmware>?               CustomFirmwareSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("firmware",       Firmware.ToJSON(CustomFirmwareSerializer,
                                                                                 CustomCustomDataSerializer)),

                                 new JProperty("requestId",      UpdateFirmwareRequestId),

                           Retries.HasValue
                               ? new JProperty("retries",        Retries.Value)
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",  (UInt64) RetryInterval.Value.TotalSeconds)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUpdateFirmwareRequestSerializer is not null
                       ? CustomUpdateFirmwareRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two update firmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">An update firmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another update firmware request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateFirmwareRequest? UpdateFirmwareRequest1,
                                           UpdateFirmwareRequest? UpdateFirmwareRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareRequest1, UpdateFirmwareRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateFirmwareRequest1 is null || UpdateFirmwareRequest2 is null)
                return false;

            return UpdateFirmwareRequest1.Equals(UpdateFirmwareRequest2);

        }

        #endregion

        #region Operator != (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two update firmware requests for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">An update firmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another update firmware request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateFirmwareRequest? UpdateFirmwareRequest1,
                                           UpdateFirmwareRequest? UpdateFirmwareRequest2)

            => !(UpdateFirmwareRequest1 == UpdateFirmwareRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateFirmwareRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two update firmware requests for equality.
        /// </summary>
        /// <param name="Object">An update firmware request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateFirmwareRequest updateFirmwareRequest &&
                   Equals(updateFirmwareRequest);

        #endregion

        #region Equals(UpdateFirmwareRequest)

        /// <summary>
        /// Compares two update firmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest">An update firmware request to compare with.</param>
        public override Boolean Equals(UpdateFirmwareRequest? UpdateFirmwareRequest)

            => UpdateFirmwareRequest is not null &&

               Firmware.               Equals(UpdateFirmwareRequest.Firmware)                &&
               UpdateFirmwareRequestId.Equals(UpdateFirmwareRequest.UpdateFirmwareRequestId) &&

            ((!Retries.      HasValue &&  !UpdateFirmwareRequest.Retries.      HasValue) ||
              (Retries.      HasValue &&   UpdateFirmwareRequest.Retries.      HasValue && Retries.      Value.Equals(UpdateFirmwareRequest.Retries.      Value))) &&

            ((!RetryInterval.HasValue &&  !UpdateFirmwareRequest.RetryInterval.HasValue) ||
              (RetryInterval.HasValue &&   UpdateFirmwareRequest.RetryInterval.HasValue && RetryInterval.Value.Equals(UpdateFirmwareRequest.RetryInterval.Value))) &&

               base.            GenericEquals(UpdateFirmwareRequest);

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

                return Firmware.               GetHashCode()       * 11 ^
                       UpdateFirmwareRequestId.GetHashCode()       *  7 ^
                      (Retries?.               GetHashCode() ?? 0) *  5 ^
                      (RetryInterval?.         GetHashCode() ?? 0) *  3 ^

                       base.                   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   UpdateFirmwareRequestId,

                   ": ", Firmware,

                   Retries.HasValue
                       ? ", " + Retries.Value + " retries"
                       : "",

                   RetryInterval.HasValue
                       ? ", retry interval " + RetryInterval.Value.TotalSeconds + " sec(s)"
                       : ""

                );

        #endregion

    }

}
