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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A NotifyWebPaymentStarted request.
    /// </summary>
    public class NotifyWebPaymentStartedRequest : ARequest<NotifyWebPaymentStartedRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyWebPaymentStartedRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE identification for which the transaction is requested.
        /// </summary>
        [Mandatory]
        public EVSE_Id        EVSEId     { get; }

        /// <summary>
        /// The timeout after which no result of the QR code scanning is to be expected anymore.
        /// </summary>
        [Mandatory]
        public TimeSpan       Timeout    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a NotifyWebPaymentStarted request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="EVSEId">An EVSE identification for which the transaction is requested.</param>
        /// <param name="Timeout">A timeout after which no result of the QR code scanning is to be expected anymore.</param>
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
        public NotifyWebPaymentStartedRequest(SourceRouting            Destination,
                                              EVSE_Id                  EVSEId,
                                              TimeSpan                 Timeout,

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
                   nameof(NotifyWebPaymentStartedRequest)[..^7],

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

            this.EVSEId   = EVSEId;
            this.Timeout  = Timeout;

            unchecked
            {
                hashCode = EVSEId. GetHashCode() * 5 ^
                           Timeout.GetHashCode() * 3 ^
                           base.   GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifyWebPaymentStartedRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be
        //                             extended with arbitrary JSON properties to allow adding custom data.",
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
        //         "evseId": {
        //             "description": "EVSE id for which transaction is requested.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "timeout": {
        //             "description": "Timeout value in seconds after which no result of web payment process (e.g. QR code scanning) is to be expected anymore.",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "evseId",
        //         "timeout"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyWebPaymentStarted request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyWebPaymentStartedRequestParser">A delegate to parse custom NotifyWebPaymentStarted requests.</param>
        public static NotifyWebPaymentStartedRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTimeOffset?                                               RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<NotifyWebPaymentStartedRequest>?  CustomNotifyWebPaymentStartedRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyWebPaymentStartedRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyWebPaymentStartedRequestParser))
            {
                return notifyWebPaymentStartedRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyWebPaymentStarted request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyWebPaymentStartedRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyWebPaymentStarted request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyWebPaymentStartedRequest">The parsed NotifyWebPaymentStarted request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyWebPaymentStartedRequestParser">A delegate to parse custom NotifyWebPaymentStarted requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out NotifyWebPaymentStartedRequest?      NotifyWebPaymentStartedRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTimeOffset?                                               RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<NotifyWebPaymentStartedRequest>?  CustomNotifyWebPaymentStartedRequestParser   = null)
        {

            try
            {

                NotifyWebPaymentStartedRequest = null;

                #region EVSEId        [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "evse identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timeout       [mandatory]

                if (!JSON.ParseMandatory("timeout",
                                         "timeout",
                                         out TimeSpan Timeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                NotifyWebPaymentStartedRequest = new NotifyWebPaymentStartedRequest(

                                                     Destination,
                                                     EVSEId,
                                                     Timeout,

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

                if (CustomNotifyWebPaymentStartedRequestParser is not null)
                    NotifyWebPaymentStartedRequest = CustomNotifyWebPaymentStartedRequestParser(JSON,
                                                                                                NotifyWebPaymentStartedRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyWebPaymentStartedRequest  = null;
                ErrorResponse                   = "The given JSON representation of a NotifyWebPaymentStarted request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyWebPaymentStartedRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyWebPaymentStartedRequestSerializer">A delegate to serialize custom NotifyWebPaymentStarted requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                           IncludeJSONLDContext                             = false,
                              CustomJObjectSerializerDelegate<NotifyWebPaymentStartedRequest>?  CustomNotifyWebPaymentStartedRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("evseId",       EVSEId. Value),
                                 new JProperty("timeout",      Timeout.TotalSeconds),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyWebPaymentStartedRequestSerializer is not null
                       ? CustomNotifyWebPaymentStartedRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyWebPaymentStartedRequest1, NotifyWebPaymentStartedRequest2)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted requests for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedRequest1">A NotifyWebPaymentStarted request.</param>
        /// <param name="NotifyWebPaymentStartedRequest2">Another NotifyWebPaymentStarted request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest1,
                                           NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyWebPaymentStartedRequest1, NotifyWebPaymentStartedRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyWebPaymentStartedRequest1 is null || NotifyWebPaymentStartedRequest2 is null)
                return false;

            return NotifyWebPaymentStartedRequest1.Equals(NotifyWebPaymentStartedRequest2);

        }

        #endregion

        #region Operator != (NotifyWebPaymentStartedRequest1, NotifyWebPaymentStartedRequest2)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted requests for inequality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedRequest1">A NotifyWebPaymentStarted request.</param>
        /// <param name="NotifyWebPaymentStartedRequest2">Another NotifyWebPaymentStarted request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest1,
                                           NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest2)

            => !(NotifyWebPaymentStartedRequest1 == NotifyWebPaymentStartedRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyWebPaymentStartedRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyWebPaymentStarted request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyWebPaymentStartedRequest notifyWebPaymentStartedRequest &&
                   Equals(notifyWebPaymentStartedRequest);

        #endregion

        #region Equals(NotifyWebPaymentStartedRequest)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted requests for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedRequest">A NotifyWebPaymentStarted request to compare with.</param>
        public override Boolean Equals(NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest)

            => NotifyWebPaymentStartedRequest is not null &&

               EVSEId. Equals(NotifyWebPaymentStartedRequest.EVSEId)  &&
               Timeout.Equals(NotifyWebPaymentStartedRequest.Timeout) &&

               base.GenericEquals(NotifyWebPaymentStartedRequest);

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

            => $"NotifyWebPaymentStarted for '{EVSEId}', timeout: {Timeout.TotalSeconds} secs";

        #endregion

    }

}
