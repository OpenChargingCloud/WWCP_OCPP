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
using Org.BouncyCastle.Asn1.Crmf;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyDERStartStop request.
    /// </summary>
    public class NotifyDERStartStopRequest : ARequest<NotifyDERStartStopRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyDERStartStopRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext               Context
            => DefaultJSONLDContext;


        /// <summary>
        /// The DER control identification.
        /// </summary>
        [Mandatory]
        public DERControl_Id               ControlId        { get; }

        /// <summary>
        /// The timestamp of event start or end.
        /// </summary>
        [Mandatory]
        public DateTime                    Timestamp        { get; }

        /// <summary>
        /// True if DER control has started. False if it has ended.
        /// </summary>
        [Mandatory]
        public Boolean                     Started          { get; }

        /// <summary>
        /// The optional enumeration of controlIds that are superseded as a result of this control starting.
        /// </summary>
        [Optional]
        public IEnumerable<DERControl_Id>  SupersededIds    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyDERStartStop request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ControlId">The DER control identification.</param>
        /// <param name="Timestamp">The timestamp of the alarm start or end.</param>
        /// <param name="Started">True if DER control has started. False if it has ended.</param>
        /// <param name="SupersededIds">An optional enumeration of controlIds that are superseded as a result of this control starting.</param>
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
        public NotifyDERStartStopRequest(SourceRouting                Destination,
                                         DERControl_Id                ControlId,
                                         DateTime                     Timestamp,
                                         Boolean                      Started,
                                         IEnumerable<DERControl_Id>?  SupersededIds         = null,

                                         IEnumerable<KeyPair>?        SignKeys              = null,
                                         IEnumerable<SignInfo>?       SignInfos             = null,
                                         IEnumerable<Signature>?      Signatures            = null,

                                         CustomData?                  CustomData            = null,

                                         Request_Id?                  RequestId             = null,
                                         DateTime?                    RequestTimestamp      = null,
                                         TimeSpan?                    RequestTimeout        = null,
                                         EventTracking_Id?            EventTrackingId       = null,
                                         NetworkPath?                 NetworkPath           = null,
                                         SerializationFormats?        SerializationFormat   = null,
                                         CancellationToken            CancellationToken     = default)

            : base(Destination,
                   nameof(NotifyDERStartStopRequest)[..^7],

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

            this.ControlId      = ControlId;
            this.Timestamp      = Timestamp;
            this.Started        = Started;
            this.SupersededIds  = SupersededIds?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.ControlId.    GetHashCode()  * 11 ^
                           this.Timestamp.    GetHashCode()  *  7 ^
                           this.Started.      GetHashCode()  *  5 ^
                           this.SupersededIds.CalcHashCode() *  3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifyDERStartStopRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
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
        //         "controlId": {
        //             "description": "Id of the started or stopped DER control.\r\nCorresponds to the _controlId_ of the SetDERControlRequest.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "started": {
        //             "description": "True if DER control has started. False if it has ended.",
        //             "type": "boolean"
        //         },
        //         "timestamp": {
        //             "description": "Time of start or end of event.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "supersededIds": {
        //             "description": "List of controlIds that are superseded as a result of this control starting.",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "type": "string",
        //                 "maxLength": 36
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "controlId",
        //         "started",
        //         "timestamp"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyDERStartStop request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyDERStartStopRequestParser">A delegate to parse custom NotifyDERStartStop requests.</param>
        public static NotifyDERStartStopRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<NotifyDERStartStopRequest>?  CustomNotifyDERStartStopRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyDERStartStopRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyDERStartStopRequestParser))
            {
                return notifyDERStartStopRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyDERStartStop request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyDERStartStopRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyDERStartStop request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyDERStartStopRequest">The parsed NotifyDERStartStop request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyDERStartStopRequestParser">A delegate to parse custom NotifyDERStartStop requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out NotifyDERStartStopRequest?      NotifyDERStartStopRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<NotifyDERStartStopRequest>?  CustomNotifyDERStartStopRequestParser   = null)
        {

            try
            {

                NotifyDERStartStopRequest = null;

                #region ControlId        [mandatory]

                if (!JSON.ParseMandatory("controlId",
                                         "control identification",
                                         DERControl_Id.TryParse,
                                         out DERControl_Id ControlId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp        [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "alarm timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Started          [mandatory]

                if (!JSON.ParseMandatory("started",
                                         "event started",
                                         out Boolean Started,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SupersededIds    [optional]

                if (JSON.ParseOptionalHashSet("supersededIds",
                                              "superseded identifications",
                                              DERControl_Id.TryParse,
                                              out HashSet<DERControl_Id> SupersededIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                NotifyDERStartStopRequest = new NotifyDERStartStopRequest(

                                                Destination,
                                                ControlId,
                                                Timestamp,
                                                Started,
                                                SupersededIds,

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

                if (CustomNotifyDERStartStopRequestParser is not null)
                    NotifyDERStartStopRequest = CustomNotifyDERStartStopRequestParser(JSON,
                                                                                      NotifyDERStartStopRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyDERStartStopRequest  = null;
                ErrorResponse              = "The given JSON representation of a NotifyDERStartStop request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyDERStartStopRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyDERStartStopRequestSerializer">A delegate to serialize custom NotifyDERStartStop requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                      IncludeJSONLDContext                        = false,
                              CustomJObjectSerializerDelegate<NotifyDERStartStopRequest>?  CustomNotifyDERStartStopRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",        DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("controlId",       ControlId.           ToString()),
                                 new JProperty("timestamp",       Timestamp.           ToIso8601()),
                                 new JProperty("started",         Started),

                           SupersededIds.Any()
                               ? new JProperty("supersededIds",   new JArray(SupersededIds.Select(supersededId => supersededId.ToString())))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyDERStartStopRequestSerializer is not null
                       ? CustomNotifyDERStartStopRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyDERStartStopRequest1, NotifyDERStartStopRequest2)

        /// <summary>
        /// Compares two NotifyDERStartStop requests for equality.
        /// </summary>
        /// <param name="NotifyDERStartStopRequest1">A NotifyDERStartStop request.</param>
        /// <param name="NotifyDERStartStopRequest2">Another NotifyDERStartStop request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyDERStartStopRequest? NotifyDERStartStopRequest1,
                                           NotifyDERStartStopRequest? NotifyDERStartStopRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyDERStartStopRequest1, NotifyDERStartStopRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyDERStartStopRequest1 is null || NotifyDERStartStopRequest2 is null)
                return false;

            return NotifyDERStartStopRequest1.Equals(NotifyDERStartStopRequest2);

        }

        #endregion

        #region Operator != (NotifyDERStartStopRequest1, NotifyDERStartStopRequest2)

        /// <summary>
        /// Compares two NotifyDERStartStop requests for inequality.
        /// </summary>
        /// <param name="NotifyDERStartStopRequest1">A NotifyDERStartStop request.</param>
        /// <param name="NotifyDERStartStopRequest2">Another NotifyDERStartStop request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyDERStartStopRequest? NotifyDERStartStopRequest1,
                                           NotifyDERStartStopRequest? NotifyDERStartStopRequest2)

            => !(NotifyDERStartStopRequest1 == NotifyDERStartStopRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyDERStartStopRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyDERStartStop requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyDERStartStop request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyDERStartStopRequest notifyDERStartStopRequest &&
                   Equals(notifyDERStartStopRequest);

        #endregion

        #region Equals(NotifyDERStartStopRequest)

        /// <summary>
        /// Compares two NotifyDERStartStop requests for equality.
        /// </summary>
        /// <param name="NotifyDERStartStopRequest">A NotifyDERStartStop request to compare with.</param>
        public override Boolean Equals(NotifyDERStartStopRequest? NotifyDERStartStopRequest)

            => NotifyDERStartStopRequest is not null &&

               ControlId.Equals(NotifyDERStartStopRequest.ControlId) &&
               Timestamp.Equals(NotifyDERStartStopRequest.Timestamp) &&
               Started.  Equals(NotifyDERStartStopRequest.Started)   &&

               SupersededIds.ToHashSet().SetEquals(NotifyDERStartStopRequest.SupersededIds) &&

               base.GenericEquals(NotifyDERStartStopRequest);

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

                   $"'{ControlId}' @ '{Timestamp}' ",

                   Started
                       ? "ended"
                       : "started",

                   SupersededIds.Any()
                       ? $", superseeds: '{SupersededIds.AggregateWith(", ")}'"
                       : ""

                );

        #endregion

    }

}
