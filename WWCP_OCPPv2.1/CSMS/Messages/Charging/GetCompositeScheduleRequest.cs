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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The get composite schedule request.
    /// </summary>
    public class GetCompositeScheduleRequest : ARequest<GetCompositeScheduleRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/getCompositeScheduleRequest");

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
        /// Create a new get composite schedule request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
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
        public GetCompositeScheduleRequest(NetworkingNode_Id        NetworkingNodeId,
                                           TimeSpan                 Duration,
                                           EVSE_Id                  EVSEId,
                                           ChargingRateUnits?       ChargingRateUnit    = null,

                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                           IEnumerable<OCPP.Signature>?  Signatures          = null,

                                           CustomData?              CustomData          = null,

                                           Request_Id?              RequestId           = null,
                                           DateTime?                RequestTimestamp    = null,
                                           TimeSpan?                RequestTimeout      = null,
                                           EventTracking_Id?        EventTrackingId     = null,
                                           NetworkPath?             NetworkPath         = null,
                                           CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
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
        //       "description": "The ID of the EVSE for which the schedule is requested. When evseid=0, the Charging Station will calculate the expected consumption for the grid connection.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "duration",
        //     "evseId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomGetCompositeScheduleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get composite schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomGetCompositeScheduleRequestParser">A delegate to parse custom get composite schedule requests.</param>
        public static GetCompositeScheduleRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        NetworkingNode_Id                                          NetworkingNodeId,
                                                        NetworkPath                                                NetworkPath,
                                                        CustomJObjectParserDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var getCompositeScheduleRequest,
                         out var errorResponse,
                         CustomGetCompositeScheduleRequestParser) &&
                getCompositeScheduleRequest is not null)
            {
                return getCompositeScheduleRequest;
            }

            throw new ArgumentException("The given JSON representation of a get composite schedule request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out GetCompositeScheduleRequest, out ErrorResponse, CustomGetCompositeScheduleRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get composite schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed get composite schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       Request_Id                        RequestId,
                                       NetworkingNode_Id                 NetworkingNodeId,
                                       NetworkPath                       NetworkPath,
                                       out GetCompositeScheduleRequest?  GetCompositeScheduleRequest,
                                       out String?                       ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out GetCompositeScheduleRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get composite schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed get composite schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCompositeScheduleRequestParser">A delegate to parse custom get composite schedule requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       NetworkingNode_Id                                          NetworkingNodeId,
                                       NetworkPath                                                NetworkPath,
                                       out GetCompositeScheduleRequest?                           GetCompositeScheduleRequest,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestParser)
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetCompositeScheduleRequest = new GetCompositeScheduleRequest(

                                                  NetworkingNodeId,
                                                  Duration,
                                                  EVSEId,
                                                  ChargingRateUnit,

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

                if (CustomGetCompositeScheduleRequestParser is not null)
                    GetCompositeScheduleRequest = CustomGetCompositeScheduleRequestParser(JSON,
                                                                                          GetCompositeScheduleRequest);

                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleRequest  = null;
                ErrorResponse                = "The given JSON representation of a get composite schedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCompositeScheduleRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCompositeScheduleRequestSerializer">A delegate to serialize custom get composite schedule requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?               CustomSignatureSerializer                     = null,
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
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest1">A get composite schedule request.</param>
        /// <param name="GetCompositeScheduleRequest2">Another get composite schedule request.</param>
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
        /// Compares two get composite schedule requests for inequality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest1">A get composite schedule request.</param>
        /// <param name="GetCompositeScheduleRequest2">Another get composite schedule request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCompositeScheduleRequest? GetCompositeScheduleRequest1,
                                           GetCompositeScheduleRequest? GetCompositeScheduleRequest2)

            => !(GetCompositeScheduleRequest1 == GetCompositeScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="Object">A get composite schedule request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCompositeScheduleRequest getCompositeScheduleRequest &&
                   Equals(getCompositeScheduleRequest);

        #endregion

        #region Equals(GetCompositeScheduleRequest)

        /// <summary>
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest">A get composite schedule request to compare with.</param>
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
