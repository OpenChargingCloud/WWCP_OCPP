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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The PublishFirmware request.
    /// In most cases this request will push a firmware image onto a local controller.
    /// </summary>
    public class PublishFirmwareRequest : ARequest<PublishFirmwareRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/publishFirmwareRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of this PublishFirmware request
        /// </summary>
        [Mandatory]
        public Int32          PublishFirmwareRequestId    { get; }

        /// <summary>
        /// The URL for downloading the firmware.onto the local controller.
        /// </summary>
        [Mandatory]
        public URL            DownloadLocation            { get; }

        /// <summary>
        /// The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.
        /// </summary>
        [Mandatory]
        public String         MD5Checksum                 { get; }

        /// <summary>
        /// The optional number of retries of a local controller for trying to
        /// download the firmware before giving up. If this field is not
        /// present, it is left to the local controller to decide how many times
        /// it wants to retry.
        /// </summary>
        [Optional]
        public Byte?          Retries                     { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to local controller to decide how long to
        /// wait between attempts.
        /// </summary>
        [Optional]
        public TimeSpan?      RetryInterval               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new PublishFirmware request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="PublishFirmwareRequestId">The unique identification of this PublishFirmware request</param>
        /// <param name="DownloadLocation">An URL for downloading the firmware.onto the local controller.</param>
        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware file as a hexadecimal string of length 32.</param>
        /// <param name="Retries">The optional number of retries of a charging station for trying to download the firmware before giving up. If this field is not present, it is left to the charging station to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charging station to decide how long to wait between attempts.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public PublishFirmwareRequest(NetworkingNode_Id        DestinationId,
                                      Int32                    PublishFirmwareRequestId,
                                      URL                      DownloadLocation,
                                      String                   MD5Checksum,
                                      Byte?                    Retries             = null,
                                      TimeSpan?                RetryInterval       = null,

                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                      IEnumerable<Signature>?       Signatures          = null,

                                      CustomData?              CustomData          = null,

                                      Request_Id?              RequestId           = null,
                                      DateTime?                RequestTimestamp    = null,
                                      TimeSpan?                RequestTimeout      = null,
                                      EventTracking_Id?        EventTrackingId     = null,
                                      NetworkPath?             NetworkPath         = null,
                                      CancellationToken        CancellationToken   = default)

            : base(DestinationId,
                   nameof(PublishFirmwareRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.PublishFirmwareRequestId  = PublishFirmwareRequestId;
            this.DownloadLocation          = DownloadLocation;
            this.MD5Checksum               = MD5Checksum;
            this.Retries                   = Retries;
            this.RetryInterval             = RetryInterval;


            unchecked
            {

                hashCode = this.PublishFirmwareRequestId.GetHashCode()       * 13 ^
                           this.DownloadLocation.        GetHashCode()       * 11 ^
                           this.MD5Checksum.             GetHashCode()       *  7 ^
                          (this.Retries?.                GetHashCode() ?? 0) *  5 ^
                          (this.RetryInterval?.          GetHashCode() ?? 0) *  3 ^
                           base.                         GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:PublishFirmwareRequest",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "location": {
        //       "description": "This contains a string containing a URI pointing to a\r\nlocation from which to retrieve the firmware.\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     },
        //     "retries": {
        //       "description": "This specifies how many times Charging Station must try\r\nto download the firmware before giving up. If this field is not\r\npresent, it is left to Charging Station to decide how many times it wants to retry.\r\n",
        //       "type": "integer"
        //     },
        //     "checksum": {
        //       "description": "The MD5 checksum over the entire firmware file as a hexadecimal string of length 32. \r\n",
        //       "type": "string",
        //       "maxLength": 32
        //     },
        //     "requestId": {
        //       "description": "The Id of the request.\r\n",
        //       "type": "integer"
        //     },
        //     "retryInterval": {
        //       "description": "The interval in seconds\r\nafter which a retry may be\r\nattempted. If this field is not\r\npresent, it is left to Charging\r\nStation to decide how long to wait\r\nbetween attempts.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "location",
        //     "checksum",
        //     "requestId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomPublishFirmwareRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a PublishFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomPublishFirmwareRequestParser">A delegate to parse custom PublishFirmware requests.</param>
        public static PublishFirmwareRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   NetworkingNode_Id                                     DestinationId,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             RequestTimestamp                     = null,
                                                   TimeSpan?                                             RequestTimeout                       = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<PublishFirmwareRequest>?  CustomPublishFirmwareRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var publishFirmwareRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomPublishFirmwareRequestParser))
            {
                return publishFirmwareRequest;
            }

            throw new ArgumentException("The given JSON representation of a PublishFirmware request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out PublishFirmwareRequest, out ErrorResponse, CustomPublishFirmwareRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a PublishFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="PublishFirmwareRequest">The parsed PublishFirmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomPublishFirmwareRequestParser">A delegate to parse custom PublishFirmware requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       NetworkingNode_Id                                     DestinationId,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out PublishFirmwareRequest?      PublishFirmwareRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             RequestTimestamp                     = null,
                                       TimeSpan?                                             RequestTimeout                       = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<PublishFirmwareRequest>?  CustomPublishFirmwareRequestParser   = null)
        {

            try
            {

                PublishFirmwareRequest = null;

                #region PublishFirmwareRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int32 PublishFirmwareRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region DownloadLocation            [mandatory]

                if (!JSON.ParseMandatory("location",
                                         "firmware location",
                                         URL.TryParse,
                                         out URL DownloadLocation,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MD5Checksum                 [mandatory]

                if (!JSON.ParseMandatoryText("checksum",
                                             "firmware checksum",
                                             out String MD5Checksum,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Retries                     [optional]

                if (JSON.ParseOptional("retries",
                                       "retries",
                                       out Byte? Retries,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RetryInterval               [optional]

                if (JSON.ParseOptional("retryInterval",
                                       "retry interval",
                                       out TimeSpan? RetryInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                  [optional, OCPP_CSE]

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

                #region CustomData                  [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PublishFirmwareRequest = new PublishFirmwareRequest(

                                             DestinationId,
                                             PublishFirmwareRequestId,
                                             DownloadLocation,
                                             MD5Checksum,
                                             Retries,
                                             RetryInterval,

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

                if (CustomPublishFirmwareRequestParser is not null)
                    PublishFirmwareRequest = CustomPublishFirmwareRequestParser(JSON,
                                                                                PublishFirmwareRequest);

                return true;

            }
            catch (Exception e)
            {
                PublishFirmwareRequest  = null;
                ErrorResponse           = "The given JSON representation of a PublishFirmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublishFirmwareRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublishFirmwareRequestSerializer">A delegate to serialize custom PublishFirmware requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PublishFirmwareRequest>?  CustomPublishFirmwareRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",       PublishFirmwareRequestId),
                                 new JProperty("location",        DownloadLocation.ToString()),
                                 new JProperty("checksum",        MD5Checksum),

                           Retries.HasValue
                               ? new JProperty("retries",         Retries.Value)
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",   (UInt64) RetryInterval.Value.TotalSeconds)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomPublishFirmwareRequestSerializer is not null
                       ? CustomPublishFirmwareRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PublishFirmwareRequest1, PublishFirmwareRequest2)

        /// <summary>
        /// Compares two PublishFirmware requests for equality.
        /// </summary>
        /// <param name="PublishFirmwareRequest1">A PublishFirmware request.</param>
        /// <param name="PublishFirmwareRequest2">Another PublishFirmware request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PublishFirmwareRequest? PublishFirmwareRequest1,
                                           PublishFirmwareRequest? PublishFirmwareRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PublishFirmwareRequest1, PublishFirmwareRequest2))
                return true;

            // If one is null, but not both, return false.
            if (PublishFirmwareRequest1 is null || PublishFirmwareRequest2 is null)
                return false;

            return PublishFirmwareRequest1.Equals(PublishFirmwareRequest2);

        }

        #endregion

        #region Operator != (PublishFirmwareRequest1, PublishFirmwareRequest2)

        /// <summary>
        /// Compares two PublishFirmware requests for inequality.
        /// </summary>
        /// <param name="PublishFirmwareRequest1">A PublishFirmware request.</param>
        /// <param name="PublishFirmwareRequest2">Another PublishFirmware request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PublishFirmwareRequest? PublishFirmwareRequest1,
                                           PublishFirmwareRequest? PublishFirmwareRequest2)

            => !(PublishFirmwareRequest1 == PublishFirmwareRequest2);

        #endregion

        #endregion

        #region IEquatable<PublishFirmwareRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PublishFirmware requests for equality.
        /// </summary>
        /// <param name="Object">A PublishFirmware request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublishFirmwareRequest publishFirmwareRequest &&
                   Equals(publishFirmwareRequest);

        #endregion

        #region Equals(PublishFirmwareRequest)

        /// <summary>
        /// Compares two PublishFirmware requests for equality.
        /// </summary>
        /// <param name="PublishFirmwareRequest">A PublishFirmware request to compare with.</param>
        public override Boolean Equals(PublishFirmwareRequest? PublishFirmwareRequest)

            => PublishFirmwareRequest is not null &&

               PublishFirmwareRequestId.Equals(PublishFirmwareRequest.PublishFirmwareRequestId) &&
               DownloadLocation.                Equals(PublishFirmwareRequest.DownloadLocation)                 &&
               MD5Checksum.                Equals(PublishFirmwareRequest.MD5Checksum)                 &&

               ((!Retries.      HasValue && !PublishFirmwareRequest.Retries.      HasValue) ||
                 (Retries.      HasValue &&  PublishFirmwareRequest.Retries.      HasValue && Retries.      Value.Equals(PublishFirmwareRequest.Retries.      Value))) &&

               ((!RetryInterval.HasValue && !PublishFirmwareRequest.RetryInterval.HasValue) ||
                 (RetryInterval.HasValue &&  PublishFirmwareRequest.RetryInterval.HasValue && RetryInterval.Value.Equals(PublishFirmwareRequest.RetryInterval.Value))) &&

               base.    GenericEquals(PublishFirmwareRequest);

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

                   PublishFirmwareRequestId,

                   ", ", DownloadLocation.ToString(),

                   ", md5: ", MD5Checksum,

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
