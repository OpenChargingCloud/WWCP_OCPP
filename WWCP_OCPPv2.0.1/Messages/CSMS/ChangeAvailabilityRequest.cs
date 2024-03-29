﻿/*
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// The change availability request.
    /// </summary>
    public class ChangeAvailabilityRequest : ARequest<ChangeAvailabilityRequest>
    {

        #region Properties

        /// <summary>
        /// The new operational status of the charging station or EVSE.
        /// </summary>
        [Mandatory]
        public OperationalStatus  OperationalStatus    { get; }

        /// <summary>
        /// The optional identification of an EVSE/connector for which
        /// the operational status should be changed.
        /// </summary>
        [Optional]
        public EVSE?              EVSE                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new change availability request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OperationalStatus">A new operational status of the charging station or EVSE.</param>
        /// <param name="EVSE">Optional identification of an EVSE/connector for which the operational status should be changed.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ChangeAvailabilityRequest(ChargeBox_Id       ChargeBoxId,
                                         OperationalStatus  OperationalStatus,
                                         EVSE?              EVSE                = null,
                                         CustomData?        CustomData          = null,

                                         Request_Id?        RequestId           = null,
                                         DateTime?          RequestTimestamp    = null,
                                         TimeSpan?          RequestTimeout      = null,
                                         EventTracking_Id?  EventTrackingId     = null,
                                         CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "ChangeAvailability",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.OperationalStatus  = OperationalStatus;
            this.EVSE               = EVSE;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ChangeAvailabilityRequest",
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
        //     "OperationalStatusEnumType": {
        //       "description": "This contains the type of availability change that the Charging Station should perform.\r\n\r\n",
        //       "javaType": "OperationalStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Inoperative",
        //         "Operative"
        //       ]
        //     },
        //     "EVSEType": {
        //       "description": "EVSE\r\nurn:x-oca:ocpp:uid:2:233123\r\nElectric Vehicle Supply Equipment\r\n",
        //       "javaType": "EVSE",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nEVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.\r\n",
        //           "type": "integer"
        //         },
        //         "connectorId": {
        //           "description": "An id to designate a specific connector (on an EVSE) by connector index number.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "id"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "evse": {
        //       "$ref": "#/definitions/EVSEType"
        //     },
        //     "operationalStatus": {
        //       "$ref": "#/definitions/OperationalStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "operationalStatus"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomChangeAvailabilityRequestSerializer = null)

        /// <summary>
        /// Parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomChangeAvailabilityRequestSerializer">An optional delegate to parse custom ChangeAvailability requests.</param>
        public static ChangeAvailabilityRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      ChargeBox_Id                                             ChargeBoxId,
                                                      CustomJObjectParserDelegate<ChangeAvailabilityRequest>?  CustomChangeAvailabilityRequestSerializer   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var changeAvailabilityRequest,
                         out var errorResponse,
                         CustomChangeAvailabilityRequestSerializer))
            {
                return changeAvailabilityRequest!;
            }

            throw new ArgumentException("The given JSON representation of a change availability request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out ChangeAvailabilityRequest, out ErrorResponse, CustomChangeAvailabilityRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed ChangeAvailability request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         JSON,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out ChangeAvailabilityRequest?  ChangeAvailabilityRequest,
                                       out String?                     ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out ChangeAvailabilityRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed ChangeAvailability request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChangeAvailabilityRequestParser">An optional delegate to parse custom ChangeAvailability requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       ChargeBox_Id                                             ChargeBoxId,
                                       out ChangeAvailabilityRequest?                           ChangeAvailabilityRequest,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<ChangeAvailabilityRequest>?  CustomChangeAvailabilityRequestParser)
        {

            try
            {

                ChangeAvailabilityRequest = null;

                #region OperationalStatus    [mandatory]

                if (!JSON.ParseMandatory("operationalStatus",
                                         "operational status",
                                         OperationalStatusExtensions.TryParse,
                                         out OperationalStatus OperationalStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSE                 [optional]

                if (!JSON.ParseOptionalJSON("evse",
                                            "evse/connector identification",
                                            OCPPv2_0_1.EVSE.TryParse,
                                            out EVSE? EVSE,
                                            out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId          [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                ChangeAvailabilityRequest = new ChangeAvailabilityRequest(ChargeBoxId,
                                                                          OperationalStatus,
                                                                          EVSE,
                                                                          CustomData,
                                                                          RequestId);

                if (CustomChangeAvailabilityRequestParser is not null)
                    ChangeAvailabilityRequest = CustomChangeAvailabilityRequestParser(JSON,
                                                                                      ChangeAvailabilityRequest);

                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityRequest  = null;
                ErrorResponse              = "The given JSON representation of a change availability request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChangeAvailabilityRequestSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeAvailabilityRequestSerializer">A delegate to serialize custom ChangeAvailability requests.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?  CustomChangeAvailabilityRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<EVSE>?                       CustomEVSESerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                           new JProperty("operationalStatus",  OperationalStatus.AsText()),

                           EVSE is not null
                               ? new JProperty("evse",         EVSE.             ToJSON(CustomEVSESerializer,
                                                                                        CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChangeAvailabilityRequestSerializer is not null
                       ? CustomChangeAvailabilityRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChangeAvailabilityRequest1, ChangeAvailabilityRequest2)

        /// <summary>
        /// Compares two ChangeAvailability requests for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest1">A ChangeAvailability request.</param>
        /// <param name="ChangeAvailabilityRequest2">Another ChangeAvailability request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeAvailabilityRequest? ChangeAvailabilityRequest1,
                                           ChangeAvailabilityRequest? ChangeAvailabilityRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeAvailabilityRequest1, ChangeAvailabilityRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeAvailabilityRequest1 is null || ChangeAvailabilityRequest2 is null)
                return false;

            return ChangeAvailabilityRequest1.Equals(ChangeAvailabilityRequest2);

        }

        #endregion

        #region Operator != (ChangeAvailabilityRequest1, ChangeAvailabilityRequest2)

        /// <summary>
        /// Compares two ChangeAvailability requests for inequality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest1">A ChangeAvailability request.</param>
        /// <param name="ChangeAvailabilityRequest2">Another ChangeAvailability request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeAvailabilityRequest? ChangeAvailabilityRequest1,
                                           ChangeAvailabilityRequest? ChangeAvailabilityRequest2)

            => !(ChangeAvailabilityRequest1 == ChangeAvailabilityRequest2);

        #endregion

        #endregion

        #region IEquatable<ChangeAvailabilityRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two change availability requests for equality.
        /// </summary>
        /// <param name="Object">A change availability request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeAvailabilityRequest changeAvailabilityRequest &&
                   Equals(changeAvailabilityRequest);

        #endregion

        #region Equals(ChangeAvailabilityRequest)

        /// <summary>
        /// Compares two change availability requests for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest">A change availability request to compare with.</param>
        public override Boolean Equals(ChangeAvailabilityRequest? ChangeAvailabilityRequest)

            => ChangeAvailabilityRequest is not null &&

               OperationalStatus.Equals(ChangeAvailabilityRequest.OperationalStatus) &&

             ((EVSE is     null && ChangeAvailabilityRequest.EVSE is     null) ||
              (EVSE is not null && ChangeAvailabilityRequest.EVSE is not null && EVSE.Equals(ChangeAvailabilityRequest.EVSE))) &&

               base.      GenericEquals(ChangeAvailabilityRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return OperationalStatus.GetHashCode()       * 5 ^

                      (EVSE?.            GetHashCode() ?? 0) * 3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(OperationalStatus,

                             EVSE is not null
                                 ? " for " + EVSE.Id.ToString()
                                 : "",

                             EVSE is not null && EVSE.ConnectorId.HasValue
                                 ? "/" + EVSE.ConnectorId.Value.ToString()
                                 : "");

        #endregion

    }

}
