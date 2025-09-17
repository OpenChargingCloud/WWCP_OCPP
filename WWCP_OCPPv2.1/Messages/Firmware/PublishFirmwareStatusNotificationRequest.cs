/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A publish firmware status notification request.
    /// </summary>
    public class PublishFirmwareStatusNotificationRequest : ARequest<PublishFirmwareStatusNotificationRequest>,
                                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/publishFirmwareStatusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

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
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Status">The progress status of the publish firmware request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequestId">The optional unique identification of the publish firmware status notification request.</param>
        /// <param name="DownloadLocations">The optional enumeration of downstream firmware download locations for all attached charging stations.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public PublishFirmwareStatusNotificationRequest(SourceRouting            Destination,
                                                        PublishFirmwareStatus    Status,
                                                        Int32?                   PublishFirmwareStatusNotificationRequestId,
                                                        IEnumerable<URL>?        DownloadLocations,

                                                        IEnumerable<KeyPair>?    SignKeys              = null,
                                                        IEnumerable<SignInfo>?   SignInfos             = null,
                                                        IEnumerable<Signature>?  Signatures            = null,

                                                        CustomData?              CustomData            = null,

                                                        Request_Id?              RequestId             = null,
                                                        DateTimeOffset?          RequestTimestamp      = null,
                                                        TimeSpan?                RequestTimeout        = null,
                                                        EventTracking_Id?        EventTrackingId       = null,
                                                        NetworkPath?             NetworkPath           = null,
                                                        SerializationFormats?    SerializationFormat   = null,
                                                        CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(PublishFirmwareStatusNotificationRequest)[..^7],

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

            this.Status                                      = Status;
            this.PublishFirmwareStatusNotificationRequestId  = PublishFirmwareStatusNotificationRequestId;
            this.DownloadLocations                           = DownloadLocations?.Distinct() ?? Array.Empty<URL>();

            unchecked
            {

                hashCode = this.Status.                                     GetHashCode()       * 7 ^
                          (this.PublishFirmwareStatusNotificationRequestId?.GetHashCode() ?? 0) * 5 ^
                           this.DownloadLocations.                          CalcHashCode()      * 3 ^
                           base.                                            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:PublishFirmwareStatusNotificationRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "PublishFirmwareStatusEnumType": {
        //             "description": "This contains the progress status of the publishfirmware\r\ninstallation.",
        //             "javaType": "PublishFirmwareStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Idle",
        //                 "DownloadScheduled",
        //                 "Downloading",
        //                 "Downloaded",
        //                 "Published",
        //                 "DownloadFailed",
        //                 "DownloadPaused",
        //                 "InvalidChecksum",
        //                 "ChecksumVerified",
        //                 "PublishFailed"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "status": {
        //             "$ref": "#/definitions/PublishFirmwareStatusEnumType"
        //         },
        //         "location": {
        //             "description": "Required if status is Published. Can be multiple URI\u2019s, if the Local Controller supports e.g. HTTP, HTTPS, and FTP.",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "type": "string",
        //                 "maxLength": 2000
        //             },
        //             "minItems": 1
        //         },
        //         "requestId": {
        //             "description": "The request id that was\r\nprovided in the\r\nPublishFirmwareRequest which\r\ntriggered this action.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a publish firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomPublishFirmwareStatusNotificationRequestParser">A delegate to parse custom publish firmware status notification requests.</param>
        public static PublishFirmwareStatusNotificationRequest Parse(JObject                                                                 JSON,
                                                                     Request_Id                                                              RequestId,
                                                                     SourceRouting                                                           Destination,
                                                                     NetworkPath                                                             NetworkPath,
                                                                     DateTimeOffset?                                                         RequestTimestamp                                       = null,
                                                                     TimeSpan?                                                               RequestTimeout                                         = null,
                                                                     EventTracking_Id?                                                       EventTrackingId                                        = null,
                                                                     CustomJObjectParserDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var publishFirmwareStatusNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomPublishFirmwareStatusNotificationRequestParser))
            {
                return publishFirmwareStatusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a publish firmware status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out PublishFirmwareStatusNotificationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a publish firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequest">The parsed publish firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomPublishFirmwareStatusNotificationRequestParser">A delegate to parse custom publish firmware status notification requests.</param>
        public static Boolean TryParse(JObject                                                                 JSON,
                                       Request_Id                                                              RequestId,
                                       SourceRouting                                                           Destination,
                                       NetworkPath                                                             NetworkPath,
                                       [NotNullWhen(true)]  out PublishFirmwareStatusNotificationRequest?      PublishFirmwareStatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                                        ErrorResponse,
                                       DateTimeOffset?                                                         RequestTimestamp                                       = null,
                                       TimeSpan?                                                               RequestTimeout                                         = null,
                                       EventTracking_Id?                                                       EventTrackingId                                        = null,
                                       CustomJObjectParserDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestParser   = null)
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PublishFirmwareStatusNotificationRequest = new PublishFirmwareStatusNotificationRequest(

                                                               Destination,
                                                               Status,
                                                               PublishFirmwareStatusNotificationRequestId,
                                                               DownloadLocations,

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
        public JObject ToJSON(Boolean                                                                     IncludeJSONLDContext                                       = false,
                              CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                                 CustomSignatureSerializer                                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                CustomCustomDataSerializer                                 = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),

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
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
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
