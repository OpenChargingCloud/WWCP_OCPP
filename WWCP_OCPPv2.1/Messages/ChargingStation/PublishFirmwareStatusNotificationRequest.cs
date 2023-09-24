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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A publish firmware status notification request.
    /// </summary>
    public class PublishFirmwareStatusNotificationRequest : ARequest<PublishFirmwareStatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The progress status of the publish firmware request.
        /// </summary>
        [Mandatory]
        public PublishFirmwareStatus  Status                                        { get; }

        /// <summary>
        /// The unique identification of the publish firmware status notification request.
        /// </summary>
        [Optional]
        public Int32?                 PublishFirmwareStatusNotificationRequestId    { get; }

        /// <summary>
        /// The optional enumeration of downstream firmware download locations for all
        /// attached charging stations.
        /// Required if status is "published".
        /// Can be multiple URI’s, if the local controller supports e.g.HTTP, HTTPS, and FTP.
        /// </summary>
        [Optional]
        public IEnumerable<URL>       DownloadLocations                             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a publish firmware status notification request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Status">The progress status of the publish firmware request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequestId">The optional unique identification of the publish firmware status notification request.</param>
        /// <param name="DownloadLocations">The optional enumeration of downstream firmware download locations for all attached charging stations.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public PublishFirmwareStatusNotificationRequest(ChargeBox_Id             ChargeBoxId,
                                                        PublishFirmwareStatus    Status,
                                                        Int32?                   PublishFirmwareStatusNotificationRequestId,
                                                        IEnumerable<URL>?        DownloadLocations,

                                                        IEnumerable<Signature>?  Signatures          = null,
                                                        CustomData?              CustomData          = null,

                                                        Request_Id?              RequestId           = null,
                                                        DateTime?                RequestTimestamp    = null,
                                                        TimeSpan?                RequestTimeout      = null,
                                                        EventTracking_Id?        EventTrackingId     = null,
                                                        CancellationToken        CancellationToken   = default)

            : base(ChargeBoxId,
                   "PublishFirmwareStatusNotification",
                   Signatures,
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.Status                                      = Status;
            this.PublishFirmwareStatusNotificationRequestId  = PublishFirmwareStatusNotificationRequestId;
            this.DownloadLocations                           = DownloadLocations?.Distinct() ?? Array.Empty<URL>();

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:PublishFirmwareStatusNotificationRequest",
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
        //     "PublishFirmwareStatusEnumType": {
        //       "description": "This contains the progress status of the publishfirmware\r\ninstallation.\r\n",
        //       "javaType": "PublishFirmwareStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Idle",
        //         "DownloadScheduled",
        //         "Downloading",
        //         "Downloaded",
        //         "Published",
        //         "DownloadFailed",
        //         "DownloadPaused",
        //         "InvalidChecksum",
        //         "ChecksumVerified",
        //         "PublishFailed"
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
        //       "$ref": "#/definitions/PublishFirmwareStatusEnumType"
        //     },
        //     "location": {
        //       "description": "Required if status is Published. Can be multiple URI’s, if the Local Controller supports e.g. HTTP, HTTPS, and FTP.\r\n",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "type": "string",
        //         "maxLength": 512
        //       },
        //       "minItems": 1
        //     },
        //     "requestId": {
        //       "description": "The request id that was\r\nprovided in the\r\nPublishFirmwareRequest which\r\ntriggered this action.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomPublishFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a publish firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomPublishFirmwareStatusNotificationRequestParser">A delegate to parse custom publish firmware status notification requests.</param>
        public static PublishFirmwareStatusNotificationRequest Parse(JObject                                                                 JSON,
                                                                     Request_Id                                                              RequestId,
                                                                     ChargeBox_Id                                                            ChargeBoxId,
                                                                     CustomJObjectParserDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var publishFirmwareStatusNotificationRequest,
                         out var errorResponse,
                         CustomPublishFirmwareStatusNotificationRequestParser))
            {
                return publishFirmwareStatusNotificationRequest!;
            }

            throw new ArgumentException("The given JSON representation of a publish firmware status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out PublishFirmwareStatusNotificationRequest, out ErrorResponse, CustomPublishFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a publish firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="PublishFirmwareStatusNotificationRequest">The parsed publish firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublishFirmwareStatusNotificationRequestParser">A delegate to parse custom publish firmware status notification requests.</param>
        public static Boolean TryParse(JObject                                                                 JSON,
                                       Request_Id                                                              RequestId,
                                       ChargeBox_Id                                                            ChargeBoxId,
                                       out PublishFirmwareStatusNotificationRequest?                           PublishFirmwareStatusNotificationRequest,
                                       out String?                                                             ErrorResponse,
                                       CustomJObjectParserDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestParser)
        {

            try
            {

                PublishFirmwareStatusNotificationRequest = null;

                #region Status                                        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "publish firmware status",
                                         PublishFirmwareStatusExtensions.TryParse,
                                         out PublishFirmwareStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PublishFirmwareStatusNotificationRequestId    [optional]

                if (JSON.ParseOptional("requestId",
                                       "publish firmware status notification request identification",
                                       out Int32? PublishFirmwareStatusNotificationRequestId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DownloadLocations                             [optional]

                if (JSON.ParseOptionalHashSet("reportData",
                                              "report data",
                                              URL.TryParse,
                                              out HashSet<URL> DownloadLocations,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                                    [optional, OCPP_CSE]

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

                #region CustomData                                    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId                                   [optional, OCPP_CSE]

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


                PublishFirmwareStatusNotificationRequest = new PublishFirmwareStatusNotificationRequest(
                                                               ChargeBoxId,
                                                               Status,
                                                               PublishFirmwareStatusNotificationRequestId,
                                                               DownloadLocations,
                                                               Signatures,
                                                               CustomData,
                                                               RequestId
                                                           );

                if (CustomPublishFirmwareStatusNotificationRequestParser is not null)
                    PublishFirmwareStatusNotificationRequest = CustomPublishFirmwareStatusNotificationRequestParser(JSON,
                                                                                                                    PublishFirmwareStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                PublishFirmwareStatusNotificationRequest  = null;
                ErrorResponse                             = "The given JSON representation of a publish firmware status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublishFirmwareStatusNotificationRequestSerializer">A delegate to serialize custom publish firmware status notification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                                 CustomSignatureSerializer                                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                CustomCustomDataSerializer                                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           PublishFirmwareStatusNotificationRequestId.HasValue
                               ? new JProperty("requestId",    PublishFirmwareStatusNotificationRequestId)
                               : null,

                           DownloadLocations.Any()
                               ? new JProperty("location",     new JArray(DownloadLocations.Select(downloadLocation => downloadLocation.ToString())))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.       Select(signature        => signature.       ToJSON(CustomSignatureSerializer,
                                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomPublishFirmwareStatusNotificationRequestSerializer is not null
                       ? CustomPublishFirmwareStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PublishFirmwareStatusNotificationRequest1, PublishFirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two publish firmware status notification requests for equality.
        /// </summary>
        /// <param name="PublishFirmwareStatusNotificationRequest1">A publish firmware status notification request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequest2">Another publish firmware status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PublishFirmwareStatusNotificationRequest? PublishFirmwareStatusNotificationRequest1,
                                           PublishFirmwareStatusNotificationRequest? PublishFirmwareStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PublishFirmwareStatusNotificationRequest1, PublishFirmwareStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (PublishFirmwareStatusNotificationRequest1 is null || PublishFirmwareStatusNotificationRequest2 is null)
                return false;

            return PublishFirmwareStatusNotificationRequest1.Equals(PublishFirmwareStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (PublishFirmwareStatusNotificationRequest1, PublishFirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two publish firmware status notification requests for inequality.
        /// </summary>
        /// <param name="PublishFirmwareStatusNotificationRequest1">A publish firmware status notification request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequest2">Another publish firmware status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PublishFirmwareStatusNotificationRequest? PublishFirmwareStatusNotificationRequest1,
                                           PublishFirmwareStatusNotificationRequest? PublishFirmwareStatusNotificationRequest2)

            => !(PublishFirmwareStatusNotificationRequest1 == PublishFirmwareStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<PublishFirmwareStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two publish firmware status notification requests for equality.
        /// </summary>
        /// <param name="Object">A publish firmware status notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublishFirmwareStatusNotificationRequest publishFirmwareStatusNotificationRequest &&
                   Equals(publishFirmwareStatusNotificationRequest);

        #endregion

        #region Equals(PublishFirmwareStatusNotificationRequest)

        /// <summary>
        /// Compares two publish firmware status notification requests for equality.
        /// </summary>
        /// <param name="PublishFirmwareStatusNotificationRequest">A publish firmware status notification request to compare with.</param>
        public override Boolean Equals(PublishFirmwareStatusNotificationRequest? PublishFirmwareStatusNotificationRequest)

            => PublishFirmwareStatusNotificationRequest is not null &&

               Status.     Equals(PublishFirmwareStatusNotificationRequest.Status) &&

             ((!PublishFirmwareStatusNotificationRequestId.HasValue && !PublishFirmwareStatusNotificationRequest.PublishFirmwareStatusNotificationRequestId.HasValue) ||
                PublishFirmwareStatusNotificationRequestId.HasValue &&  PublishFirmwareStatusNotificationRequest.PublishFirmwareStatusNotificationRequestId.HasValue && PublishFirmwareStatusNotificationRequestId.Value.Equals(PublishFirmwareStatusNotificationRequest.PublishFirmwareStatusNotificationRequestId.Value)) &&

               DownloadLocations.Count().Equals(PublishFirmwareStatusNotificationRequest.DownloadLocations.Count())     &&
               DownloadLocations.All(data => PublishFirmwareStatusNotificationRequest.DownloadLocations.Contains(data)) &&

               base.GenericEquals(PublishFirmwareStatusNotificationRequest);

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

                return Status.                                     GetHashCode()       * 7 ^
                      (PublishFirmwareStatusNotificationRequestId?.GetHashCode() ?? 0) * 5 ^
                       DownloadLocations.                          CalcHashCode()      * 3 ^

                       base.                                       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Status.AsText(),

                   PublishFirmwareStatusNotificationRequestId.HasValue
                       ? ", requestId: " + PublishFirmwareStatusNotificationRequestId.Value
                       : "",

                    DownloadLocations.Any()
                       ? ", download locations: " + DownloadLocations.Select(downloadLocation => downloadLocation.ToString()).AggregateWith(", ")
                       : ""

               );

        #endregion

    }

}
