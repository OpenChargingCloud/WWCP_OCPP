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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A heartbeat request.
    /// </summary>
    public class HeartbeatRequest : ARequest<HeartbeatRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/heartbeatRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a heartbeat request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public HeartbeatRequest(ChargingStation_Id       ChargingStationId,

                                IEnumerable<KeyPair>?    SignKeys            = null,
                                IEnumerable<SignInfo>?   SignInfos           = null,
                                SignaturePolicy?         SignaturePolicy     = null,
                                IEnumerable<Signature>?  Signatures          = null,

                                CustomData?              CustomData          = null,

                                Request_Id?              RequestId           = null,
                                DateTime?                RequestTimestamp    = null,
                                TimeSpan?                RequestTimeout      = null,
                                EventTracking_Id?        EventTrackingId     = null,
                                CancellationToken        CancellationToken   = default)

            : base(ChargingStationId,
                   "Heartbeat",

                   SignKeys,
                   SignInfos,
                   SignaturePolicy,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            unchecked
            {

                hashCode = base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:HeartbeatRequest",
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
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomHeartbeatRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomHeartbeatRequestParser">A delegate to parse custom heartbeat requests.</param>
        public static HeartbeatRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             ChargingStation_Id                              ChargingStationId,
                                             CustomJObjectParserDelegate<HeartbeatRequest>?  CustomHeartbeatRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var heartbeatRequest,
                         out var errorResponse,
                         CustomHeartbeatRequestParser))
            {
                return heartbeatRequest!;
            }

            throw new ArgumentException("The given JSON representation of a heartbeat request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out HeartbeatRequest, out ErrorResponse, CustomHeartbeatRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       Request_Id             RequestId,
                                       ChargingStation_Id     ChargingStationId,
                                       out HeartbeatRequest?  HeartbeatRequest,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out HeartbeatRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHeartbeatRequestParser">A delegate to parse custom heartbeat requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       ChargingStation_Id                              ChargingStationId,
                                       out HeartbeatRequest?                           HeartbeatRequest,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<HeartbeatRequest>?  CustomHeartbeatRequestParser)
        {

            try
            {

                HeartbeatRequest = null;

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingStationId    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargingStationId",
                                       "charging station identification",
                                       ChargingStation_Id.TryParse,
                                       out ChargingStation_Id? chargingStationId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargingStationId_PayLoad.HasValue)
                        ChargingStationId = chargingStationId_PayLoad.Value;

                }

                #endregion


                HeartbeatRequest = new HeartbeatRequest(
                                       ChargingStationId,
                                       null,
                                       null,
                                       null,
                                       Signatures,
                                       CustomData,
                                       RequestId
                                   );

                if (CustomHeartbeatRequestParser is not null)
                    HeartbeatRequest = CustomHeartbeatRequestParser(JSON,
                                                                    HeartbeatRequest);

                return true;

            }
            catch (Exception e)
            {
                HeartbeatRequest  = null;
                ErrorResponse     = "The given JSON representation of a heartbeat request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomHeartbeatRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHeartbeatRequestSerializer">A delegate to serialize custom heartbeat requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<HeartbeatRequest>?  CustomHeartbeatRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?         CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomHeartbeatRequestSerializer is not null
                       ? CustomHeartbeatRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest1">A heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (HeartbeatRequest? HeartbeatRequest1,
                                           HeartbeatRequest? HeartbeatRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(HeartbeatRequest1, HeartbeatRequest2))
                return true;

            // If one is null, but not both, return false.
            if (HeartbeatRequest1 is null || HeartbeatRequest2 is null)
                return false;

            return HeartbeatRequest1.Equals(HeartbeatRequest2);

        }

        #endregion

        #region Operator != (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for inequality.
        /// </summary>
        /// <param name="HeartbeatRequest1">A heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (HeartbeatRequest? HeartbeatRequest1,
                                           HeartbeatRequest? HeartbeatRequest2)

            => !(HeartbeatRequest1 == HeartbeatRequest2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="Object">A heartbeat request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HeartbeatRequest heartbeatRequest &&
                   Equals(heartbeatRequest);

        #endregion

        #region Equals(HeartbeatRequest)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest">A heartbeat request to compare with.</param>
        public override Boolean Equals(HeartbeatRequest? HeartbeatRequest)

            => HeartbeatRequest is not null &&

               base.GenericEquals(HeartbeatRequest);

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

            => "HeartbeatRequest";

        #endregion

    }

}
