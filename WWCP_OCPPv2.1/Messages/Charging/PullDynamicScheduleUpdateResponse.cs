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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The PullDynamicScheduleUpdate response.
    /// </summary>
    public class PullDynamicScheduleUpdateResponse : AResponse<PullDynamicScheduleUpdateRequest,
                                                               PullDynamicScheduleUpdateResponse>,
                                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/pullDynamicScheduleUpdateResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status of the PullDynamicScheduleUpdate request.
        /// </summary>
        [Mandatory]
        public ChargingProfileStatus    Status                    { get; }

        /// <summary>
        /// The optional charging schedule update.
        /// </summary>
        [Optional]
        public ChargingScheduleUpdate?  ChargingScheduleUpdate    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a PullDynamicScheduleUpdate response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// 
        /// <param name="Status">The status of the PullDynamicScheduleUpdate request.</param>
        /// <param name="ChargingScheduleUpdate">An optional charging schedule update.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public PullDynamicScheduleUpdateResponse(PullDynamicScheduleUpdateRequest  Request,

                                                 ChargingProfileStatus             Status,
                                                 ChargingScheduleUpdate?           ChargingScheduleUpdate   = null,

                                                 Result?                           Result                   = null,
                                                 DateTime?                         ResponseTimestamp        = null,

                                                 SourceRouting?                    Destination              = null,
                                                 NetworkPath?                      NetworkPath              = null,

                                                 IEnumerable<KeyPair>?             SignKeys                 = null,
                                                 IEnumerable<SignInfo>?            SignInfos                = null,
                                                 IEnumerable<Signature>?           Signatures               = null,

                                                 CustomData?                       CustomData               = null,

                                                 SerializationFormats?             SerializationFormat      = null,
                                                 CancellationToken                 CancellationToken        = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status                  = Status;
            this.ChargingScheduleUpdate  = ChargingScheduleUpdate;

            unchecked
            {

                hashCode =  this.Status.                 GetHashCode()       * 5 ^
                           (this.ChargingScheduleUpdate?.GetHashCode() ?? 0) * 3 ^
                            base.                        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:PullDynamicScheduleUpdateResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ChargingProfileStatusEnumType": {
        //             "description": "Result of request.",
        //             "javaType": "ChargingProfileStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //         "ChargingScheduleUpdateType": {
        //             "description": "Updates to a ChargingSchedulePeriodType for dynamic charging profiles.",
        //             "javaType": "ChargingScheduleUpdate",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "limit": {
        //                     "description": "Optional only when not required by the _operationMode_, as in CentralSetpoint, ExternalSetpoint, ExternalLimits, LocalFrequency,  LocalLoadBalancing. +\r\nCharging rate limit during the schedule period, in the applicable _chargingRateUnit_. \r\nThis SHOULD be a non-negative value; a negative value is only supported for backwards compatibility with older systems that use a negative value to specify a discharging limit.\r\nFor AC this field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "limit_L2": {
        //                     "description": "*(2.1)* Charging rate limit on phase L2  in the applicable _chargingRateUnit_. ",
        //                     "type": "number"
        //                 },
        //                 "limit_L3": {
        //                     "description": "*(2.1)* Charging rate limit on phase L3  in the applicable _chargingRateUnit_. ",
        //                     "type": "number"
        //                 },
        //                 "dischargeLimit": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ that the EV is allowed to discharge with. Note, these are negative values in order to be consistent with _setpoint_, which can be positive and negative.  +\r\nFor AC this field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "dischargeLimit_L2": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ on phase L2 that the EV is allowed to discharge with. ",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "dischargeLimit_L3": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ on phase L3 that the EV is allowed to discharge with. ",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "setpoint": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow as close as possible. Use negative values for discharging. +\r\nWhen a limit and/or _dischargeLimit_ are given the overshoot when following _setpoint_ must remain within these values.\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "setpoint_L2": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow on phase L2 as close as possible.",
        //                     "type": "number"
        //                 },
        //                 "setpoint_L3": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow on phase L3 as close as possible. ",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow as closely as possible. Positive values for inductive, negative for capacitive reactive power or current.  +\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive_L2": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow on phase L2 as closely as possible. ",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive_L3": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow on phase L3 as closely as possible. ",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
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
        //         "scheduleUpdate": {
        //             "$ref": "#/definitions/ChargingScheduleUpdateType"
        //         },
        //         "status": {
        //             "$ref": "#/definitions/ChargingProfileStatusEnumType"
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

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a PullDynamicScheduleUpdate response.
        /// </summary>
        /// <param name="Request">The PullDynamicScheduleUpdate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPullDynamicScheduleUpdateResponseParser">A delegate to parse custom PullDynamicScheduleUpdate responses.</param>
        public static PullDynamicScheduleUpdateResponse Parse(PullDynamicScheduleUpdateRequest                                 Request,
                                                              JObject                                                          JSON,
                                                              SourceRouting                                                    Destination,
                                                              NetworkPath                                                      NetworkPath,
                                                              DateTime?                                                        ResponseTimestamp                               = null,
                                                              CustomJObjectParserDelegate<PullDynamicScheduleUpdateResponse>?  CustomPullDynamicScheduleUpdateResponseParser   = null,
                                                              CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                                              CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var pullDynamicScheduleUpdateResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomPullDynamicScheduleUpdateResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return pullDynamicScheduleUpdateResponse;
            }

            throw new ArgumentException("The given JSON representation of a PullDynamicScheduleUpdate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out PullDynamicScheduleUpdateResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a PullDynamicScheduleUpdate response.
        /// </summary>
        /// <param name="Request">The PullDynamicScheduleUpdate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PullDynamicScheduleUpdateResponse">The parsed PullDynamicScheduleUpdate response.</param>
        /// <param name="CustomPullDynamicScheduleUpdateResponseParser">A delegate to parse custom PullDynamicScheduleUpdate responses.</param>
        public static Boolean TryParse(PullDynamicScheduleUpdateRequest                                 Request,
                                       JObject                                                          JSON,
                                       SourceRouting                                                    Destination,
                                       NetworkPath                                                      NetworkPath,
                                       [NotNullWhen(true)]  out PullDynamicScheduleUpdateResponse?      PullDynamicScheduleUpdateResponse,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse,
                                       DateTime?                                                        ResponseTimestamp                               = null,
                                       CustomJObjectParserDelegate<PullDynamicScheduleUpdateResponse>?  CustomPullDynamicScheduleUpdateResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                       CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {

            try
            {

                PullDynamicScheduleUpdateResponse = null;

                #region Limit                     [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "charging profile status",
                                         ChargingProfileStatusExtensions.TryParse,
                                         out ChargingProfileStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingScheduleUpdate    [optional]

                if (JSON.ParseOptionalJSON("scheduleUpdate",
                                           "charging schedule update",
                                           OCPPv2_1.ChargingScheduleUpdate.TryParse,
                                           out ChargingScheduleUpdate? ChargingScheduleUpdate,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

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


                PullDynamicScheduleUpdateResponse = new PullDynamicScheduleUpdateResponse(

                                                        Request,

                                                        Status,
                                                        ChargingScheduleUpdate,

                                                        null,
                                                        ResponseTimestamp,

                                                        Destination,
                                                        NetworkPath,

                                                        null,
                                                        null,
                                                        Signatures,

                                                        CustomData

                                                    );

                if (CustomPullDynamicScheduleUpdateResponseParser is not null)
                    PullDynamicScheduleUpdateResponse = CustomPullDynamicScheduleUpdateResponseParser(JSON,
                                                                                                      PullDynamicScheduleUpdateResponse);

                return true;

            }
            catch (Exception e)
            {
                PullDynamicScheduleUpdateResponse  = null;
                ErrorResponse                      = "The given JSON representation of a PullDynamicScheduleUpdate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPullDynamicScheduleUpdateResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPullDynamicScheduleUpdateResponseSerializer">A delegate to serialize custom PullDynamicScheduleUpdate responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                              IncludeJSONLDContext                                = false,
                              CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateResponse>?  CustomPullDynamicScheduleUpdateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingScheduleUpdate>?             CustomChargingScheduleUpdateSerializer              = null,
                              CustomJObjectSerializerDelegate<Signature>?                          CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",         DefaultJSONLDContext.  ToString())
                               : null,

                                 new JProperty("status",           Status.                ToString()),

                           ChargingScheduleUpdate is not null
                               ? new JProperty("scheduleUpdate",   ChargingScheduleUpdate.ToJSON(CustomChargingScheduleUpdateSerializer,
                                                                                                 CustomCustomDataSerializer))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.            ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomPullDynamicScheduleUpdateResponseSerializer is not null
                       ? CustomPullDynamicScheduleUpdateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The PullDynamicScheduleUpdate failed because of a request error.
        /// </summary>
        /// <param name="Request">The PullDynamicScheduleUpdate request.</param>
        public static PullDynamicScheduleUpdateResponse RequestError(PullDynamicScheduleUpdateRequest  Request,
                                                                     EventTracking_Id                  EventTrackingId,
                                                                     ResultCode                        ErrorCode,
                                                                     String?                           ErrorDescription    = null,
                                                                     JObject?                          ErrorDetails        = null,
                                                                     DateTime?                         ResponseTimestamp   = null,

                                                                     SourceRouting?                    Destination         = null,
                                                                     NetworkPath?                      NetworkPath         = null,

                                                                     IEnumerable<KeyPair>?             SignKeys            = null,
                                                                     IEnumerable<SignInfo>?            SignInfos           = null,
                                                                     IEnumerable<Signature>?           Signatures          = null,

                                                                     CustomData?                       CustomData          = null)

            => new (

                   Request,

                   ChargingProfileStatus.Rejected,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The PullDynamicScheduleUpdate failed.
        /// </summary>
        /// <param name="Request">The PullDynamicScheduleUpdate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static PullDynamicScheduleUpdateResponse FormationViolation(PullDynamicScheduleUpdateRequest  Request,
                                                                           String                            ErrorDescription)

            => new (Request,
                    ChargingProfileStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The PullDynamicScheduleUpdate failed.
        /// </summary>
        /// <param name="Request">The PullDynamicScheduleUpdate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static PullDynamicScheduleUpdateResponse SignatureError(PullDynamicScheduleUpdateRequest  Request,
                                                                       String                            ErrorDescription)

            => new (Request,
                    ChargingProfileStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The PullDynamicScheduleUpdate failed.
        /// </summary>
        /// <param name="Request">The PullDynamicScheduleUpdate request.</param>
        /// <param name="Description">An optional error description.</param>
        public static PullDynamicScheduleUpdateResponse Failed(PullDynamicScheduleUpdateRequest  Request,
                                                               String?                           Description   = null)

            => new (Request,
                    ChargingProfileStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The PullDynamicScheduleUpdate failed because of an exception.
        /// </summary>
        /// <param name="Request">The PullDynamicScheduleUpdate request.</param>
        /// <param name="Exception">The exception.</param>
        public static PullDynamicScheduleUpdateResponse ExceptionOccured(PullDynamicScheduleUpdateRequest  Request,
                                                                         Exception                         Exception)

            => new (Request,
                    ChargingProfileStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (PullDynamicScheduleUpdateResponse1, PullDynamicScheduleUpdateResponse2)

        /// <summary>
        /// Compares two PullDynamicScheduleUpdate responses for equality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateResponse1">A PullDynamicScheduleUpdate response.</param>
        /// <param name="PullDynamicScheduleUpdateResponse2">Another PullDynamicScheduleUpdate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse1,
                                           PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PullDynamicScheduleUpdateResponse1, PullDynamicScheduleUpdateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (PullDynamicScheduleUpdateResponse1 is null || PullDynamicScheduleUpdateResponse2 is null)
                return false;

            return PullDynamicScheduleUpdateResponse1.Equals(PullDynamicScheduleUpdateResponse2);

        }

        #endregion

        #region Operator != (PullDynamicScheduleUpdateResponse1, PullDynamicScheduleUpdateResponse2)

        /// <summary>
        /// Compares two PullDynamicScheduleUpdate responses for inequality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateResponse1">A PullDynamicScheduleUpdate response.</param>
        /// <param name="PullDynamicScheduleUpdateResponse2">Another PullDynamicScheduleUpdate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse1,
                                           PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse2)

            => !(PullDynamicScheduleUpdateResponse1 == PullDynamicScheduleUpdateResponse2);

        #endregion

        #endregion

        #region IEquatable<PullDynamicScheduleUpdateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PullDynamicScheduleUpdate responses for equality.
        /// </summary>
        /// <param name="Object">A PullDynamicScheduleUpdate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PullDynamicScheduleUpdateResponse pullDynamicScheduleUpdateResponse &&
                   Equals(pullDynamicScheduleUpdateResponse);

        #endregion

        #region Equals(PullDynamicScheduleUpdateResponse)

        /// <summary>
        /// Compares two PullDynamicScheduleUpdate responses for equality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateResponse">A PullDynamicScheduleUpdate response to compare with.</param>
        public override Boolean Equals(PullDynamicScheduleUpdateResponse? PullDynamicScheduleUpdateResponse)

            => PullDynamicScheduleUpdateResponse is not null &&

               Status.Equals(PullDynamicScheduleUpdateResponse.Status) &&

             ((ChargingScheduleUpdate is     null && PullDynamicScheduleUpdateResponse.ChargingScheduleUpdate is     null) ||
              (ChargingScheduleUpdate is not null && PullDynamicScheduleUpdateResponse.ChargingScheduleUpdate is not null && ChargingScheduleUpdate.Equals(PullDynamicScheduleUpdateResponse.ChargingScheduleUpdate))) &&

               base.GenericEquals(PullDynamicScheduleUpdateResponse);

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

                   Status.ToString(),

                   ChargingScheduleUpdate is not null
                       ? $", schedule update: {ChargingScheduleUpdate}"
                       : null

               );

        #endregion

    }

}
