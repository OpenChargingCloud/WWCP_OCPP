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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The FirmwareStatusNotification request.
    /// </summary>
    public class FirmwareStatusNotificationRequest : ARequest<FirmwareStatusNotificationRequest>,
                                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/firmwareStatusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

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
        /// Create a new FirmwareStatusNotification request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Status">The status of the firmware installation.</param>
        /// <param name="UpdateFirmwareRequestId">The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public FirmwareStatusNotificationRequest(SourceRouting            Destination,
                                                 FirmwareStatus           Status,
                                                 Int64?                   UpdateFirmwareRequestId   = null,

                                                 IEnumerable<KeyPair>?    SignKeys                  = null,
                                                 IEnumerable<SignInfo>?   SignInfos                 = null,
                                                 IEnumerable<Signature>?  Signatures                = null,

                                                 CustomData?              CustomData                = null,

                                                 Request_Id?              RequestId                 = null,
                                                 DateTime?                RequestTimestamp          = null,
                                                 TimeSpan?                RequestTimeout            = null,
                                                 EventTracking_Id?        EventTrackingId           = null,
                                                 NetworkPath?             NetworkPath               = null,
                                                 SerializationFormats?    SerializationFormat       = null,
                                                 CancellationToken        CancellationToken         = default)

            : base(Destination,
                   nameof(FirmwareStatusNotificationRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status                   = Status;
            this.UpdateFirmwareRequestId  = UpdateFirmwareRequestId;

            unchecked
            {
                hashCode = this.Status.                  GetHashCode()       * 5 ^
                          (this.UpdateFirmwareRequestId?.GetHashCode() ?? 0) * 3 ^
                           base.                         GetHashCode();
            }

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
        //       "description": "This contains the progress status of the firmware installation.",
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
        //       "description": "The request id that was provided in the\r\nUpdateFirmwareRequest that started this firmware update.\r\nThis field is mandatory, unless the message was triggered by a TriggerMessageRequest AND there is no firmware update ongoing.",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a FirmwareStatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">A delegate to parse custom FirmwareStatusNotification requests.</param>
        public static FirmwareStatusNotificationRequest Parse(JObject                                                          JSON,
                                                              Request_Id                                                       RequestId,
                                                              SourceRouting                                                    Destination,
                                                              NetworkPath                                                      NetworkPath,
                                                              DateTime?                                                        RequestTimestamp                                = null,
                                                              TimeSpan?                                                        RequestTimeout                                  = null,
                                                              EventTracking_Id?                                                EventTrackingId                                 = null,
                                                              CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var firmwareStatusNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomFirmwareStatusNotificationRequestParser))
            {
                return firmwareStatusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a FirmwareStatusNotification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out FirmwareStatusNotificationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a FirmwareStatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed FirmwareStatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">A delegate to parse custom FirmwareStatusNotification requests.</param>
        public static Boolean TryParse(JObject                                                          JSON,
                                       Request_Id                                                       RequestId,
                                       SourceRouting                                                    Destination,
                                       NetworkPath                                                      NetworkPath,
                                       [NotNullWhen(true)]  out FirmwareStatusNotificationRequest?      FirmwareStatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse,
                                       DateTime?                                                        RequestTimestamp                                = null,
                                       TimeSpan?                                                        RequestTimeout                                  = null,
                                       EventTracking_Id?                                                EventTrackingId                                 = null,
                                       CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestParser   = null)
        {

            try
            {

                FirmwareStatusNotificationRequest = null;

                #region Status                     [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "firmware status",
                                         FirmwareStatusExtensions.TryParse,
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

                #region Signatures                 [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

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


                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(

                                                        Destination,
                                                        Status,
                                                        UpdateFirmwareRequestId,

                                                        null,
                                                        null,
                                                        Signatures,

                                                        CustomData,

                                                        RequestId,
                                                        RequestTimestamp,
                                                        RequestTimeout,
                                                        EventTrackingId,
                                                        NetworkPath

                                                    );

                if (CustomFirmwareStatusNotificationRequestParser is not null)
                    FirmwareStatusNotificationRequest = CustomFirmwareStatusNotificationRequestParser(JSON,
                                                                                                      FirmwareStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                FirmwareStatusNotificationRequest  = null;
                ErrorResponse                      = "The given JSON representation of a FirmwareStatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFirmwareStatusNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareStatusNotificationRequestSerializer">A delegate to serialize custom FirmwareStatusNotification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                          CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           UpdateFirmwareRequestId.HasValue
                               ? new JProperty("requestId",    UpdateFirmwareRequestId.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
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
        /// Compares two FirmwareStatusNotification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A FirmwareStatusNotification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another FirmwareStatusNotification request.</param>
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
        /// Compares two FirmwareStatusNotification requests for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A FirmwareStatusNotification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another FirmwareStatusNotification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest1,
                                           FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest2)

            => !(FirmwareStatusNotificationRequest1 == FirmwareStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two FirmwareStatusNotification requests for equality.
        /// </summary>
        /// <param name="Object">A FirmwareStatusNotification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FirmwareStatusNotificationRequest firmwareStatusNotificationRequest &&
                   Equals(firmwareStatusNotificationRequest);


        #endregion

        #region Equals(FirmwareStatusNotificationRequest)

        /// <summary>
        /// Compares two FirmwareStatusNotification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest">A FirmwareStatusNotification request to compare with.</param>
        public override Boolean Equals(FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest)

            => FirmwareStatusNotificationRequest is not null &&

               Status.     Equals(FirmwareStatusNotificationRequest.Status) &&

            ((!UpdateFirmwareRequestId.HasValue && !FirmwareStatusNotificationRequest.UpdateFirmwareRequestId.HasValue) ||
               UpdateFirmwareRequestId.HasValue &&  FirmwareStatusNotificationRequest.UpdateFirmwareRequestId.HasValue && UpdateFirmwareRequestId.Value.Equals(FirmwareStatusNotificationRequest.UpdateFirmwareRequestId.Value)) &&

               base.GenericEquals(FirmwareStatusNotificationRequest);

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

            => $"{Status}{(UpdateFirmwareRequestId.HasValue ? $" ({UpdateFirmwareRequestId.Value})" : "")}";

        #endregion

    }

}
