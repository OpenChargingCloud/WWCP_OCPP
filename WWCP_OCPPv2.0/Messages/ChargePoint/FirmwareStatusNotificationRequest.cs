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

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    /// <summary>
    /// The firmware status notification request.
    /// </summary>
    public class FirmwareStatusNotificationRequest : ARequest<FirmwareStatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The status of the firmware installation.
        /// </summary>
        [Mandatory]
        public FirmwareStatus  Status                     { get; }

        /// <summary>
        /// The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.
        /// This field is _mandatory_, unless the message was triggered by a TriggerMessageRequest AND there is no firmware update ongoing.
        /// </summary>
        [Optional]
        public Int64?          UpdateFirmwareRequestId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new firmware status notification request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="Status">The status of the firmware installation.</param>
        /// <param name="UpdateFirmwareRequestId">The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public FirmwareStatusNotificationRequest(ChargeBox_Id        ChargeBoxId,

                                                 FirmwareStatus      Status,
                                                 Int64?              UpdateFirmwareRequestId   = null,

                                                 CustomData?         CustomData                = null,
                                                 Request_Id?         RequestId                 = null,
                                                 DateTime?           RequestTimestamp          = null,
                                                 TimeSpan?           RequestTimeout            = null,
                                                 EventTracking_Id?   EventTrackingId           = null,
                                                 CancellationToken?  CancellationToken         = null)

            : base(ChargeBoxId,
                   "FirmwareStatusNotification",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.Status                   = Status;
            this.UpdateFirmwareRequestId  = UpdateFirmwareRequestId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:FirmwareStatusNotificationRequest",
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
        //     "FirmwareStatusEnumType": {
        //       "description": "This contains the progress status of the firmware installation.\r\n",
        //       "javaType": "FirmwareStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Downloaded",
        //         "DownloadFailed",
        //         "Downloading",
        //         "DownloadScheduled",
        //         "DownloadPaused",
        //         "Idle",
        //         "InstallationFailed",
        //         "Installing",
        //         "Installed",
        //         "InstallRebooting",
        //         "InstallScheduled",
        //         "InstallVerificationFailed",
        //         "InvalidSignature",
        //         "SignatureVerified"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/FirmwareStatusEnumType"
        //     },
        //     "requestId": {
        //       "description": "The request id that was provided in the\r\nUpdateFirmwareRequest that started this firmware update.\r\nThis field is mandatory, unless the message was triggered by a TriggerMessageRequest AND there is no firmware update ongoing.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">A delegate to parse custom firmware status notification requests.</param>
        public static FirmwareStatusNotificationRequest Parse(JObject                                                          JSON,
                                                              Request_Id                                                       RequestId,
                                                              ChargeBox_Id                                                     ChargeBoxId,
                                                              CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var firmwareStatusNotificationRequest,
                         out var errorResponse,
                         CustomFirmwareStatusNotificationRequestParser))
            {
                return firmwareStatusNotificationRequest!;
            }

            throw new ArgumentException("The given JSON representation of a firmware status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out FirmwareStatusNotificationRequest, out ErrorResponse)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       Request_Id                              RequestId,
                                       ChargeBox_Id                            ChargeBoxId,
                                       out FirmwareStatusNotificationRequest?  FirmwareStatusNotificationRequest,
                                       out String?                             ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out FirmwareStatusNotificationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">A delegate to parse custom FirmwareStatusNotification requests.</param>
        public static Boolean TryParse(JObject                                                          JSON,
                                       Request_Id                                                       RequestId,
                                       ChargeBox_Id                                                     ChargeBoxId,
                                       out FirmwareStatusNotificationRequest?                           FirmwareStatusNotificationRequest,
                                       out String?                                                      ErrorResponse,
                                       CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestParser)
        {

            try
            {

                FirmwareStatusNotificationRequest = null;

                #region Status                     [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "firmware status",
                                         FirmwareStatusExtentions.TryParse,
                                         out FirmwareStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region UpdateFirmwareRequestId    [optional]

                if (JSON.ParseOptional("customData",
                                       "custom data",
                                       out Int64? UpdateFirmwareRequestId,
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


                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(ChargeBoxId,
                                                                                          Status,
                                                                                          UpdateFirmwareRequestId,
                                                                                          CustomData,
                                                                                          RequestId);

                if (CustomFirmwareStatusNotificationRequestParser is not null)
                    FirmwareStatusNotificationRequest = CustomFirmwareStatusNotificationRequestParser(JSON,
                                                                                                      FirmwareStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                FirmwareStatusNotificationRequest  = null;
                ErrorResponse                      = "The given JSON representation of a firmware status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFirmwareStatusNotificationRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareStatusNotificationRequestSerializer">A delegate to serialize custom firmware status notification requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           UpdateFirmwareRequestId.HasValue
                               ? new JProperty("requestId",   UpdateFirmwareRequestId.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFirmwareStatusNotificationRequestSerializer is not null
                       ? CustomFirmwareStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest1,
                                           FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (FirmwareStatusNotificationRequest1 is null || FirmwareStatusNotificationRequest2 is null)
                return false;

            return FirmwareStatusNotificationRequest1.Equals(FirmwareStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest1,
                                           FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest2)

            => !(FirmwareStatusNotificationRequest1 == FirmwareStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="Object">A firmware status notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FirmwareStatusNotificationRequest firmwareStatusNotificationRequest &&
                   Equals(firmwareStatusNotificationRequest);


        #endregion

        #region Equals(FirmwareStatusNotificationRequest)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request to compare with.</param>
        public override Boolean Equals(FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest)

            => FirmwareStatusNotificationRequest is not null &&

               Status.     Equals(FirmwareStatusNotificationRequest.Status) &&

            ((!UpdateFirmwareRequestId.HasValue && !FirmwareStatusNotificationRequest.UpdateFirmwareRequestId.HasValue) ||
               UpdateFirmwareRequestId.HasValue &&  FirmwareStatusNotificationRequest.UpdateFirmwareRequestId.HasValue && UpdateFirmwareRequestId.Value.Equals(FirmwareStatusNotificationRequest.UpdateFirmwareRequestId.Value)) &&

               base.GenericEquals(FirmwareStatusNotificationRequest);

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

                return Status.                  GetHashCode()       * 5 ^

                      (UpdateFirmwareRequestId?.GetHashCode() ?? 0) * 3 ^

                       base.                    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
