﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/CloseChargingCloud/WWCP_OCPP>
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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A close periodic event stream request.
    /// </summary>
    public class ClosePeriodicEventStreamRequest : ARequest<ClosePeriodicEventStreamRequest>,
                                                   IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/closePeriodicEventStreamRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unqiue identification of the periodic event stream to close.
        /// </summary>
        [Mandatory]
        public PeriodicEventStream_Id  Id    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new close periodic event stream request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Id">The unqiue identification of the periodic event stream to close.</param>
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
        public ClosePeriodicEventStreamRequest(SourceRouting            Destination,
                                               PeriodicEventStream_Id   Id,

                                               IEnumerable<KeyPair>?    SignKeys              = null,
                                               IEnumerable<SignInfo>?   SignInfos             = null,
                                               IEnumerable<Signature>?  Signatures            = null,

                                               CustomData?              CustomData            = null,

                                               Request_Id?              RequestId             = null,
                                               DateTime?                RequestTimestamp      = null,
                                               TimeSpan?                RequestTimeout        = null,
                                               EventTracking_Id?        EventTrackingId       = null,
                                               NetworkPath?             NetworkPath           = null,
                                               SerializationFormats?    SerializationFormat   = null,
                                               CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(ClosePeriodicEventStreamRequest)[..^7],

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

            this.Id = Id;

            unchecked
            {
                hashCode = this.Id.GetHashCode() * 3 ^
                           base.   GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ClosePeriodicEventStreamRequest",
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
        //         "id": {
        //             "description": "Id of stream to close.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an ClosePeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomClosePeriodicEventStreamRequestParser">A delegate to parse custom ClosePeriodicEventStream requests.</param>
        public static ClosePeriodicEventStreamRequest Parse(JObject                                                        JSON,
                                                            Request_Id                                                     RequestId,
                                                            SourceRouting                                                  Destination,
                                                            NetworkPath                                                    NetworkPath,
                                                            CustomJObjectParserDelegate<ClosePeriodicEventStreamRequest>?  CustomClosePeriodicEventStreamRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var closePeriodicEventStreamRequest,
                         out var errorResponse,
                         CustomClosePeriodicEventStreamRequestParser))
            {
                return closePeriodicEventStreamRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClosePeriodicEventStream request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ClosePeriodicEventStreamRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ClosePeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClosePeriodicEventStreamRequest">The parsed ClosePeriodicEventStream request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       SourceRouting                                              Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out ClosePeriodicEventStreamRequest?  ClosePeriodicEventStreamRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        Destination,
                        NetworkPath,
                        out ClosePeriodicEventStreamRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ClosePeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClosePeriodicEventStreamRequest">The parsed ClosePeriodicEventStream request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClosePeriodicEventStreamRequestParser">A delegate to parse custom ClosePeriodicEventStream requests.</param>
        public static Boolean TryParse(JObject                                                        JSON,
                                       Request_Id                                                     RequestId,
                                       SourceRouting                                                  Destination,
                                       NetworkPath                                                    NetworkPath,
                                       [NotNullWhen(true)]  out ClosePeriodicEventStreamRequest?      ClosePeriodicEventStreamRequest,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       CustomJObjectParserDelegate<ClosePeriodicEventStreamRequest>?  CustomClosePeriodicEventStreamRequestParser)
        {

            try
            {

                ClosePeriodicEventStreamRequest = null;

                #region Id                   [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "periodic event stream identification",
                                         PeriodicEventStream_Id.TryParse,
                                         out PeriodicEventStream_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                ClosePeriodicEventStreamRequest = new ClosePeriodicEventStreamRequest(

                                                      Destination,
                                                      Id,

                                                      null,
                                                      null,
                                                      Signatures,

                                                      CustomData,

                                                      RequestId,
                                                      null,
                                                      null,
                                                      null,
                                                      NetworkPath

                                                  );

                if (CustomClosePeriodicEventStreamRequestParser is not null)
                    ClosePeriodicEventStreamRequest = CustomClosePeriodicEventStreamRequestParser(JSON,
                                                                                                  ClosePeriodicEventStreamRequest);

                return true;

            }
            catch (Exception e)
            {
                ClosePeriodicEventStreamRequest  = null;
                ErrorResponse                    = "The given JSON representation of a ClosePeriodicEventStream request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClosePeriodicEventStreamRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClosePeriodicEventStreamRequestSerializer">A delegate to serialize custom ClosePeriodicEventStream requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                            IncludeJSONLDContext                              = false,
                              CustomJObjectSerializerDelegate<ClosePeriodicEventStreamRequest>?  CustomClosePeriodicEventStreamRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("id",           Id.Value),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClosePeriodicEventStreamRequestSerializer is not null
                       ? CustomClosePeriodicEventStreamRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClosePeriodicEventStreamRequest1, ClosePeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two ClosePeriodicEventStream requests for equality.
        /// </summary>
        /// <param name="ClosePeriodicEventStreamRequest1">A ClosePeriodicEventStream request.</param>
        /// <param name="ClosePeriodicEventStreamRequest2">Another ClosePeriodicEventStream request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClosePeriodicEventStreamRequest? ClosePeriodicEventStreamRequest1,
                                           ClosePeriodicEventStreamRequest? ClosePeriodicEventStreamRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClosePeriodicEventStreamRequest1, ClosePeriodicEventStreamRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClosePeriodicEventStreamRequest1 is null || ClosePeriodicEventStreamRequest2 is null)
                return false;

            return ClosePeriodicEventStreamRequest1.Equals(ClosePeriodicEventStreamRequest2);

        }

        #endregion

        #region Operator != (ClosePeriodicEventStreamRequest1, ClosePeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two ClosePeriodicEventStream requests for inequality.
        /// </summary>
        /// <param name="ClosePeriodicEventStreamRequest1">A ClosePeriodicEventStream request.</param>
        /// <param name="ClosePeriodicEventStreamRequest2">Another ClosePeriodicEventStream request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClosePeriodicEventStreamRequest? ClosePeriodicEventStreamRequest1,
                                           ClosePeriodicEventStreamRequest? ClosePeriodicEventStreamRequest2)

            => !(ClosePeriodicEventStreamRequest1 == ClosePeriodicEventStreamRequest2);

        #endregion

        #endregion

        #region IEquatable<ClosePeriodicEventStreamRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClosePeriodicEventStream requests for equality.
        /// </summary>
        /// <param name="Object">A ClosePeriodicEventStream request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClosePeriodicEventStreamRequest closePeriodicEventStreamRequest &&
                   Equals(closePeriodicEventStreamRequest);

        #endregion

        #region Equals(ClosePeriodicEventStreamRequest)

        /// <summary>
        /// Compares two ClosePeriodicEventStream requests for equality.
        /// </summary>
        /// <param name="ClosePeriodicEventStreamRequest">A ClosePeriodicEventStream request to compare with.</param>
        public override Boolean Equals(ClosePeriodicEventStreamRequest? ClosePeriodicEventStreamRequest)

            => ClosePeriodicEventStreamRequest is not null &&

               Id.         Equals(ClosePeriodicEventStreamRequest.Id) &&

               base.GenericEquals(ClosePeriodicEventStreamRequest);

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

            => Id.ToString();

        #endregion


    }

}
