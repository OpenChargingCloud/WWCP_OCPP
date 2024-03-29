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

namespace cloud.charging.open.protocols.OCPPv2_0_1.CS
{

    /// <summary>
    /// A cleared charging limit request.
    /// </summary>
    public class ClearedChargingLimitRequest : ARequest<ClearedChargingLimitRequest>
    {

        #region Properties

        /// <summary>
        /// The source of the charging limit.
        /// </summary>
        [Mandatory]
        public ChargingLimitSources  ChargingLimitSource    { get; }

        /// <summary>
        /// The optional EVSE identification.
        /// </summary>
        [Optional]
        public EVSE_Id?              EVSEId                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a cleared charging limit request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingLimitSource">A source of the charging limit.</param>
        /// <param name="EVSEId">An optional EVSE identification.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ClearedChargingLimitRequest(ChargeBox_Id          ChargeBoxId,
                                           ChargingLimitSources  ChargingLimitSource,
                                           EVSE_Id?              EVSEId,
                                           CustomData?           CustomData          = null,

                                           Request_Id?           RequestId           = null,
                                           DateTime?             RequestTimestamp    = null,
                                           TimeSpan?             RequestTimeout      = null,
                                           EventTracking_Id?     EventTrackingId     = null,
                                           CancellationToken     CancellationToken   = default)

            : base(ChargeBoxId,
                   "ClearedChargingLimit",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ChargingLimitSource  = ChargingLimitSource;
            this.EVSEId               = EVSEId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearedChargingLimitRequest",
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
        //     "ChargingLimitSourceEnumType": {
        //       "description": "Source of the charging limit.\r\n",
        //       "javaType": "ChargingLimitSourceEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "EMS",
        //         "Other",
        //         "SO",
        //         "CSO"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "chargingLimitSource": {
        //       "$ref": "#/definitions/ChargingLimitSourceEnumType"
        //     },
        //     "evseId": {
        //       "description": "EVSE Identifier.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "chargingLimitSource"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomClearedChargingLimitRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cleared charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomClearedChargingLimitRequestParser">An optional delegate to parse custom cleared charging limit requests.</param>
        public static ClearedChargingLimitRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        ChargeBox_Id                                               ChargeBoxId,
                                                        CustomJObjectParserDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var clearedChargingLimitRequest,
                         out var errorResponse,
                         CustomClearedChargingLimitRequestParser))
            {
                return clearedChargingLimitRequest!;
            }

            throw new ArgumentException("The given JSON representation of a cleared charging limit request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out ClearedChargingLimitRequest, out ErrorResponse, CustomClearedChargingLimitRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cleared charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearedChargingLimitRequest">The parsed cleared charging limit request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearedChargingLimitRequestParser">An optional delegate to parse custom cleared charging limit requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       ChargeBox_Id                                               ChargeBoxId,
                                       out ClearedChargingLimitRequest?                           ClearedChargingLimitRequest,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestParser)
        {

            try
            {

                ClearedChargingLimitRequest = null;

                #region ChargingLimitSource    [mandatory]

                if (!JSON.ParseMandatory("chargingLimitSource",
                                         "charging limit source",
                                         ChargingLimitSourcesExtensions.TryParse,
                                         out ChargingLimitSources ChargingLimitSource,
                                         out ErrorResponse))
                {              
                    return false;
                }

                #endregion

                #region EVSEId                 [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData             [optional]

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

                #region ChargeBoxId            [optional, OCPP_CSE]

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


                ClearedChargingLimitRequest = new ClearedChargingLimitRequest(
                                                  ChargeBoxId,
                                                  ChargingLimitSource,
                                                  EVSEId,
                                                  CustomData,
                                                  RequestId
                                              );

                if (CustomClearedChargingLimitRequestParser is not null)
                    ClearedChargingLimitRequest = CustomClearedChargingLimitRequestParser(JSON,
                                                                                          ClearedChargingLimitRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearedChargingLimitRequest  = null;
                ErrorResponse                = "The given JSON representation of a cleared charging limit request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearedChargingLimitRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearedChargingLimitRequestSerializer">A delegate to serialize custom ClearedChargingLimit requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingLimitSource",   ChargingLimitSource.AsText()),

                           EVSEId.HasValue
                               ? new JProperty("evseId",                EVSEId.             Value.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomClearedChargingLimitRequestSerializer is not null
                       ? CustomClearedChargingLimitRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearedChargingLimitRequest1, ClearedChargingLimitRequest2)

        /// <summary>
        /// Compares two cleared charging limit requests for equality.
        /// </summary>
        /// <param name="ClearedChargingLimitRequest1">A cleared charging limit request.</param>
        /// <param name="ClearedChargingLimitRequest2">Another cleared charging limit request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearedChargingLimitRequest? ClearedChargingLimitRequest1,
                                           ClearedChargingLimitRequest? ClearedChargingLimitRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearedChargingLimitRequest1, ClearedChargingLimitRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearedChargingLimitRequest1 is null || ClearedChargingLimitRequest2 is null)
                return false;

            return ClearedChargingLimitRequest1.Equals(ClearedChargingLimitRequest2);

        }

        #endregion

        #region Operator != (ClearedChargingLimitRequest1, ClearedChargingLimitRequest2)

        /// <summary>
        /// Compares two cleared charging limit requests for inequality.
        /// </summary>
        /// <param name="ClearedChargingLimitRequest1">A cleared charging limit request.</param>
        /// <param name="ClearedChargingLimitRequest2">Another cleared charging limit request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearedChargingLimitRequest? ClearedChargingLimitRequest1,
                                           ClearedChargingLimitRequest? ClearedChargingLimitRequest2)

            => !(ClearedChargingLimitRequest1 == ClearedChargingLimitRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearedChargingLimitRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cleared charging limit requests for equality.
        /// </summary>
        /// <param name="Object">A cleared charging limit request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearedChargingLimitRequest clearedChargingLimitRequest &&
                   Equals(clearedChargingLimitRequest);

        #endregion

        #region Equals(ClearedChargingLimitRequest)

        /// <summary>
        /// Compares two cleared charging limit requests for equality.
        /// </summary>
        /// <param name="ClearedChargingLimitRequest">A cleared charging limit request to compare with.</param>
        public override Boolean Equals(ClearedChargingLimitRequest? ClearedChargingLimitRequest)

            => ClearedChargingLimitRequest is not null &&

               ChargingLimitSource.Equals(ClearedChargingLimitRequest.ChargingLimitSource) &&

            ((!EVSEId.HasValue && !ClearedChargingLimitRequest.EVSEId.HasValue) ||
               EVSEId.HasValue &&  ClearedChargingLimitRequest.EVSEId.HasValue && EVSEId.Value.Equals(ClearedChargingLimitRequest.EVSEId.Value)) &&

               base.        GenericEquals(ClearedChargingLimitRequest);

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

                return ChargingLimitSource.GetHashCode()       * 5 ^

                      (EVSEId?.            GetHashCode() ?? 0) * 3 ^

                       base.               GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   ChargingLimitSource.AsText(),

                   EVSEId.HasValue
                       ? " at " + EVSEId.Value
                       : ""

               );

        #endregion

    }

}
