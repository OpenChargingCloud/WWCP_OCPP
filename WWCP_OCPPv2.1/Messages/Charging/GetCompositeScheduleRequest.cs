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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetCompositeSchedule request.
    /// </summary>
    public class GetCompositeScheduleRequest : ARequest<GetCompositeScheduleRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getCompositeScheduleRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The length of requested schedule.
        /// </summary>
        [Mandatory]
        public TimeSpan            Duration            { get; }

        /// <summary>
        /// The EVSE identification for which the schedule is requested.
        /// EVSE identification is 0, the charging station will calculate the expected
        /// consumption for the grid connection.
        /// </summary>
        [Mandatory]
        public EVSE_Id             EVSEId              { get; }

        /// <summary>
        /// Can optionally be used to force a power or current profile.
        /// </summary>
        [Optional]
        public ChargingRateUnits?  ChargingRateUnit    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetCompositeSchedule request.
        /// </summary>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="EVSEId">The EVSE identification for which the schedule is requested. EVSE identification is 0, the charging station will calculate the expected consumption for the grid connection.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
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
        public GetCompositeScheduleRequest(SourceRouting            Destination,
                                           TimeSpan                 Duration,
                                           EVSE_Id                  EVSEId,
                                           ChargingRateUnits?       ChargingRateUnit    = null,

                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                           IEnumerable<Signature>?  Signatures          = null,

                                           CustomData?              CustomData          = null,

                                           Request_Id?              RequestId           = null,
                                           DateTime?                RequestTimestamp    = null,
                                           TimeSpan?                RequestTimeout      = null,
                                           EventTracking_Id?        EventTrackingId     = null,
                                           NetworkPath?             NetworkPath         = null,
                                           CancellationToken        CancellationToken   = default)

            : base(Destination,
                   nameof(GetCompositeScheduleRequest)[..^7],

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

            this.Duration          = Duration;
            this.EVSEId            = EVSEId;
            this.ChargingRateUnit  = ChargingRateUnit;


            unchecked
            {

                hashCode = this.Duration.         GetHashCode()       * 7 ^
                           this.EVSEId.           GetHashCode()       * 5 ^
                          (this.ChargingRateUnit?.GetHashCode() ?? 0) * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetCompositeScheduleRequest",
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
        //     "ChargingRateUnitEnumType": {
        //       "description": "Can be used to force a power or current profile.\r\n\r\n",
        //       "javaType": "ChargingRateUnitEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "W",
        //         "A"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "duration": {
        //       "description": "Length of the requested schedule in seconds.\r\n\r\n",
        //       "type": "integer"
        //     },
        //     "chargingRateUnit": {
        //       "$ref": "#/definitions/ChargingRateUnitEnumType"
        //     },
        //     "evseId": {
        //       "description": "The ID of the EVSE for which the schedule is requested. When evseid=0, the Charging Station will calculate the expected consumption for the grid connection.",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "duration",
        //     "evseId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetCompositeScheduleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetCompositeSchedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetCompositeScheduleRequestParser">A delegate to parse custom GetCompositeSchedule requests.</param>
        public static GetCompositeScheduleRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        SourceRouting                                              SourceRouting,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  RequestTimestamp                          = null,
                                                        TimeSpan?                                                  RequestTimeout                            = null,
                                                        EventTracking_Id?                                          EventTrackingId                           = null,
                                                        CustomJObjectParserDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                             SourceRouting,
                         NetworkPath,
                         out var getCompositeScheduleRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetCompositeScheduleRequestParser))
            {
                return getCompositeScheduleRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetCompositeSchedule request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetCompositeScheduleRequest, out ErrorResponse, CustomGetCompositeScheduleRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetCompositeSchedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed GetCompositeSchedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetCompositeScheduleRequestParser">A delegate to parse custom GetCompositeSchedule requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       SourceRouting                                              SourceRouting,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out GetCompositeScheduleRequest?      GetCompositeScheduleRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  RequestTimestamp                          = null,
                                       TimeSpan?                                                  RequestTimeout                            = null,
                                       EventTracking_Id?                                          EventTrackingId                           = null,
                                       CustomJObjectParserDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestParser   = null)
        {

            try
            {

                GetCompositeScheduleRequest = null;

                #region Duration             [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out TimeSpan Duration,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId               [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingRateUnit     [optional]

                if (JSON.ParseOptional("chargingRateUnit",
                                       "charging rate unit",
                                       ChargingRateUnitsExtensions.TryParse,
                                       out ChargingRateUnits? ChargingRateUnit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetCompositeScheduleRequest = new GetCompositeScheduleRequest(

                                                      SourceRouting,
                                                  Duration,
                                                  EVSEId,
                                                  ChargingRateUnit,

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

                if (CustomGetCompositeScheduleRequestParser is not null)
                    GetCompositeScheduleRequest = CustomGetCompositeScheduleRequestParser(JSON,
                                                                                          GetCompositeScheduleRequest);

                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleRequest  = null;
                ErrorResponse                = "The given JSON representation of a GetCompositeSchedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCompositeScheduleRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCompositeScheduleRequestSerializer">A delegate to serialize custom GetCompositeSchedule requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("duration",           (UInt64) Duration.TotalSeconds),
                                 new JProperty("evseId",             EVSEId.           Value),

                           ChargingRateUnit.HasValue
                               ? new JProperty("chargingRateUnit",   ChargingRateUnit. Value.AsText())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCompositeScheduleRequestSerializer is not null
                       ? CustomGetCompositeScheduleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetCompositeScheduleRequest1, GetCompositeScheduleRequest2)

        /// <summary>
        /// Compares two GetCompositeSchedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest1">A GetCompositeSchedule request.</param>
        /// <param name="GetCompositeScheduleRequest2">Another GetCompositeSchedule request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCompositeScheduleRequest? GetCompositeScheduleRequest1,
                                           GetCompositeScheduleRequest? GetCompositeScheduleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCompositeScheduleRequest1, GetCompositeScheduleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetCompositeScheduleRequest1 is null || GetCompositeScheduleRequest2 is null)
                return false;

            return GetCompositeScheduleRequest1.Equals(GetCompositeScheduleRequest2);

        }

        #endregion

        #region Operator != (GetCompositeScheduleRequest1, GetCompositeScheduleRequest2)

        /// <summary>
        /// Compares two GetCompositeSchedule requests for inequality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest1">A GetCompositeSchedule request.</param>
        /// <param name="GetCompositeScheduleRequest2">Another GetCompositeSchedule request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCompositeScheduleRequest? GetCompositeScheduleRequest1,
                                           GetCompositeScheduleRequest? GetCompositeScheduleRequest2)

            => !(GetCompositeScheduleRequest1 == GetCompositeScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetCompositeSchedule requests for equality.
        /// </summary>
        /// <param name="Object">A GetCompositeSchedule request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCompositeScheduleRequest getCompositeScheduleRequest &&
                   Equals(getCompositeScheduleRequest);

        #endregion

        #region Equals(GetCompositeScheduleRequest)

        /// <summary>
        /// Compares two GetCompositeSchedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest">A GetCompositeSchedule request to compare with.</param>
        public override Boolean Equals(GetCompositeScheduleRequest? GetCompositeScheduleRequest)

            => GetCompositeScheduleRequest is not null &&

               Duration.   Equals(GetCompositeScheduleRequest.Duration) &&
               EVSEId.     Equals(GetCompositeScheduleRequest.EVSEId)   &&

            ((!ChargingRateUnit.HasValue && !GetCompositeScheduleRequest.ChargingRateUnit.HasValue) ||
              (ChargingRateUnit.HasValue &&  GetCompositeScheduleRequest.ChargingRateUnit.HasValue && ChargingRateUnit.Value.Equals(GetCompositeScheduleRequest.ChargingRateUnit.Value))) &&

               base.GenericEquals(GetCompositeScheduleRequest);

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

            => $"{EVSEId} / {Math.Round(Duration.TotalSeconds, 2)} sec(s){(ChargingRateUnit.HasValue
                                                                               ? " / " + ChargingRateUnit.Value
                                                                               : "")}";

        #endregion

    }

}
