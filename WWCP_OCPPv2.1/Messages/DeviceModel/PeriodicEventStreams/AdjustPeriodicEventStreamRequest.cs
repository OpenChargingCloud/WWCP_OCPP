﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/GetChargingCloud/WWCP_OCPP>
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
    /// The AdjustPeriodicEventStream request.
    /// </summary>
    public class AdjustPeriodicEventStreamRequest : ARequest<AdjustPeriodicEventStreamRequest>,
                                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/AdjustPeriodicEventStreamsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the data stream to adjust.
        /// </summary>
        [Mandatory]
        public PeriodicEventStream_Id         StreamId      { get; }

        /// <summary>
        /// The updated rate of sending data.
        /// </summary>
        [Mandatory]
        public PeriodicEventStreamParameters  Parameters    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AdjustPeriodicEventStream request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="StreamId">The unique identification of the data stream to adjust.</param>
        /// <param name="Parameters">The updated rate of sending data.</param>
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
        public AdjustPeriodicEventStreamRequest(SourceRouting                  Destination,
                                                PeriodicEventStream_Id         StreamId,
                                                PeriodicEventStreamParameters  Parameters,

                                                IEnumerable<KeyPair>?          SignKeys              = null,
                                                IEnumerable<SignInfo>?         SignInfos             = null,
                                                IEnumerable<Signature>?        Signatures            = null,

                                                CustomData?                    CustomData            = null,

                                                Request_Id?                    RequestId             = null,
                                                DateTime?                      RequestTimestamp      = null,
                                                TimeSpan?                      RequestTimeout        = null,
                                                EventTracking_Id?              EventTrackingId       = null,
                                                NetworkPath?                   NetworkPath           = null,
                                                SerializationFormats?          SerializationFormat   = null,
                                                CancellationToken              CancellationToken     = default)

            : base(Destination,
                   nameof(AdjustPeriodicEventStreamRequest)[..^7],

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

            this.StreamId    = StreamId;
            this.Parameters  = Parameters;

            unchecked
            {
                hashCode = this.StreamId.  GetHashCode() * 5 ^
                           this.Parameters.GetHashCode() * 3 ^
                           base.           GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:AdjustPeriodicEventStreamRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "PeriodicEventStreamParamsType": {
        //             "javaType": "PeriodicEventStreamParams",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "interval": {
        //                     "description": "Time in seconds after which stream data is sent.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "values": {
        //                     "description": "Number of items to be sent together in stream.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
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
        //         "id": {
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "params": {
        //             "$ref": "#/definitions/PeriodicEventStreamParamsType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id",
        //         "params"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an AdjustPeriodicEventStreams request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomAdjustPeriodicEventStreamRequestParser">A delegate to parse custom AdjustPeriodicEventStreams requests.</param>
        public static AdjustPeriodicEventStreamRequest Parse(JObject                                                         JSON,
                                                             Request_Id                                                      RequestId,
                                                             SourceRouting                                                   Destination,
                                                             NetworkPath                                                     NetworkPath,
                                                             DateTime?                                                       RequestTimestamp                               = null,
                                                             TimeSpan?                                                       RequestTimeout                                 = null,
                                                             EventTracking_Id?                                               EventTrackingId                                = null,
                                                             CustomJObjectParserDelegate<AdjustPeriodicEventStreamRequest>?  CustomAdjustPeriodicEventStreamRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var adjustPeriodicEventStreamsRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomAdjustPeriodicEventStreamRequestParser))
            {
                return adjustPeriodicEventStreamsRequest;
            }

            throw new ArgumentException("The given JSON representation of an AdjustPeriodicEventStream request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out AdjustPeriodicEventStreamRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an AdjustPeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AdjustPeriodicEventStreamRequest">The parsed AdjustPeriodicEventStreams request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAdjustPeriodicEventStreamRequestParser">A delegate to parse custom AdjustPeriodicEventStreams requests.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       Request_Id                                                      RequestId,
                                       SourceRouting                                                   Destination,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out AdjustPeriodicEventStreamRequest?      AdjustPeriodicEventStreamRequest,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       DateTime?                                                       RequestTimestamp                               = null,
                                       TimeSpan?                                                       RequestTimeout                                 = null,
                                       EventTracking_Id?                                               EventTrackingId                                = null,
                                       CustomJObjectParserDelegate<AdjustPeriodicEventStreamRequest>?  CustomAdjustPeriodicEventStreamRequestParser   = null)
        {

            try
            {

                AdjustPeriodicEventStreamRequest = null;

                #region StreamId      [mandatory]

                if (JSON.ParseMandatory("id",
                                        "unique event stream id",
                                        PeriodicEventStream_Id.TryParse,
                                        out PeriodicEventStream_Id StreamId,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parameters    [mandatory]

                if (!JSON.ParseMandatoryJSON("params",
                                             "event stream parameters",
                                             PeriodicEventStreamParameters.TryParse,
                                             out PeriodicEventStreamParameters? Parameters,
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


                AdjustPeriodicEventStreamRequest = new AdjustPeriodicEventStreamRequest(

                                                    Destination,
                                                    StreamId,
                                                    Parameters,

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

                if (CustomAdjustPeriodicEventStreamRequestParser is not null)
                    AdjustPeriodicEventStreamRequest = CustomAdjustPeriodicEventStreamRequestParser(JSON,
                                                                                                    AdjustPeriodicEventStreamRequest);

                return true;

            }
            catch (Exception e)
            {
                AdjustPeriodicEventStreamRequest  = null;
                ErrorResponse                     = "The given JSON representation of an AdjustPeriodicEventStream request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAdjustPeriodicEventStreamRequestSerializer = null, CustomPeriodicEventStreamParametersSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAdjustPeriodicEventStreamRequestSerializer">A delegate to serialize custom AdjustPeriodicEventStreams requests.</param>
        /// <param name="CustomPeriodicEventStreamParametersSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                             IncludeJSONLDContext                               = false,
                              CustomJObjectSerializerDelegate<AdjustPeriodicEventStreamRequest>?  CustomAdjustPeriodicEventStreamRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?     CustomPeriodicEventStreamParametersSerializer      = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("id",           StreamId.Value),
                                 new JProperty("params",       Parameters.          ToJSON(CustomPeriodicEventStreamParametersSerializer,
                                                                                           CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAdjustPeriodicEventStreamRequestSerializer is not null
                       ? CustomAdjustPeriodicEventStreamRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AdjustPeriodicEventStreamRequest1, AdjustPeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two AdjustPeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="AdjustPeriodicEventStreamRequest1">An AdjustPeriodicEventStreams request.</param>
        /// <param name="AdjustPeriodicEventStreamRequest2">Another AdjustPeriodicEventStreams request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AdjustPeriodicEventStreamRequest? AdjustPeriodicEventStreamRequest1,
                                           AdjustPeriodicEventStreamRequest? AdjustPeriodicEventStreamRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AdjustPeriodicEventStreamRequest1, AdjustPeriodicEventStreamRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AdjustPeriodicEventStreamRequest1 is null || AdjustPeriodicEventStreamRequest2 is null)
                return false;

            return AdjustPeriodicEventStreamRequest1.Equals(AdjustPeriodicEventStreamRequest2);

        }

        #endregion

        #region Operator != (AdjustPeriodicEventStreamRequest1, AdjustPeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two AdjustPeriodicEventStreams requests for inequality.
        /// </summary>
        /// <param name="AdjustPeriodicEventStreamRequest1">An AdjustPeriodicEventStreams request.</param>
        /// <param name="AdjustPeriodicEventStreamRequest2">Another AdjustPeriodicEventStreams request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AdjustPeriodicEventStreamRequest? AdjustPeriodicEventStreamRequest1,
                                           AdjustPeriodicEventStreamRequest? AdjustPeriodicEventStreamRequest2)

            => !(AdjustPeriodicEventStreamRequest1 == AdjustPeriodicEventStreamRequest2);

        #endregion

        #endregion

        #region IEquatable<AdjustPeriodicEventStreamRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AdjustPeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="Object">An AdjustPeriodicEventStreams request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AdjustPeriodicEventStreamRequest AdjustPeriodicEventStreamsRequest &&
                   Equals(AdjustPeriodicEventStreamsRequest);

        #endregion

        #region Equals(AdjustPeriodicEventStreamRequest)

        /// <summary>
        /// Compares two AdjustPeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="AdjustPeriodicEventStreamRequest">An AdjustPeriodicEventStreams request to compare with.</param>
        public override Boolean Equals(AdjustPeriodicEventStreamRequest? AdjustPeriodicEventStreamRequest)

            => AdjustPeriodicEventStreamRequest is not null &&

               StreamId.  Equals(AdjustPeriodicEventStreamRequest.StreamId)   &&
               Parameters.Equals(AdjustPeriodicEventStreamRequest.Parameters) &&

               base.GenericEquals(AdjustPeriodicEventStreamRequest);

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

            => $"{StreamId}: {Parameters}";

        #endregion


    }

}
