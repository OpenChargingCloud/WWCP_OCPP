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

namespace cloud.charging.open.protocols.OCPPv2_0.CSMS
{

    /// <summary>
    /// The reset request.
    /// </summary>
    public class ResetRequest : ARequest<ResetRequest>
    {

        #region Properties

        /// <summary>
        /// The type of reset that the charging station should perform.
        /// </summary>
        [Mandatory]
        public ResetTypes  ResetType    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charging station should perform.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ResetRequest(ChargeBox_Id        ChargeBoxId,
                            ResetTypes          ResetType,

                            CustomData?         CustomData          = null,
                            Request_Id?         RequestId           = null,
                            DateTime?           RequestTimestamp    = null,
                            TimeSpan?           RequestTimeout      = null,
                            EventTracking_Id?   EventTrackingId     = null,
                            CancellationToken?  CancellationToken   = null)


            : base(ChargeBoxId,
                   "Reset",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ResetType = ResetType;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ResetRequest",
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
        //     "ResetEnumType": {
        //       "description": "This contains the type of reset that the Charging Station or EVSE should perform.\r\n",
        //       "javaType": "ResetEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Immediate",
        //         "OnIdle"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "type": {
        //       "$ref": "#/definitions/ResetEnumType"
        //     },
        //     "evseId": {
        //       "description": "This contains the ID of a specific EVSE that needs to be reset, instead of the entire Charging Station.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "type"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomResetRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a reset request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomResetRequestParser">A delegate to parse custom reset requests.</param>
        public static ResetRequest Parse(JObject                                     JSON,
                                         Request_Id                                  RequestId,
                                         ChargeBox_Id                                ChargeBoxId,
                                         CustomJObjectParserDelegate<ResetRequest>?  CustomResetRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var resetRequest,
                         out var errorResponse,
                         CustomResetRequestParser))
            {
                return resetRequest!;
            }

            throw new ArgumentException("The given JSON representation of a reset request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out ResetRequest, out ErrorResponse, CustomResetRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a reset request.
        /// </summary>
        /// <param name="ResetRequestJSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetRequest">The parsed reset request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject            ResetRequestJSON,
                                       Request_Id         RequestId,
                                       ChargeBox_Id       ChargeBoxId,
                                       out ResetRequest?  ResetRequest,
                                       out String?        ErrorResponse)

            => TryParse(ResetRequestJSON,
                        RequestId,
                        ChargeBoxId,
                        out ResetRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a reset request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetRequest">The parsed reset request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomResetRequestParser">A delegate to parse custom Reset requests.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       Request_Id                                  RequestId,
                                       ChargeBox_Id                                ChargeBoxId,
                                       out ResetRequest?                           ResetRequest,
                                       out String?                                 ErrorResponse,
                                       CustomJObjectParserDelegate<ResetRequest>?  CustomResetRequestParser)
        {

            try
            {

                ResetRequest = null;

                #region ResetType      [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "reset type",
                                         ResetTypesExtensions.TryParse,
                                         out ResetTypes ResetType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId    [optional, OCPP_CSE]

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


                ResetRequest = new ResetRequest(ChargeBoxId,
                                                ResetType,
                                                CustomData,
                                                RequestId);

                if (CustomResetRequestParser is not null)
                    ResetRequest = CustomResetRequestParser(JSON,
                                                            ResetRequest);

                return true;

            }
            catch (Exception e)
            {
                ResetRequest   = null;
                ErrorResponse  = "The given JSON representation of a reset request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomResetRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomResetRequestSerializer">A delegate to serialize custom reset requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ResetRequest>?  CustomResetRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?    CustomCustomDataSerializer     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("type",        ResetType. AsText()),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomResetRequestSerializer is not null
                       ? CustomResetRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ResetRequest1, ResetRequest2)

        /// <summary>
        /// Compares two reset requests for equality.
        /// </summary>
        /// <param name="ResetRequest1">A reset request.</param>
        /// <param name="ResetRequest2">Another reset request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ResetRequest? ResetRequest1,
                                           ResetRequest? ResetRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ResetRequest1, ResetRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ResetRequest1 is null || ResetRequest2 is null)
                return false;

            return ResetRequest1.Equals(ResetRequest2);

        }

        #endregion

        #region Operator != (ResetRequest1, ResetRequest2)

        /// <summary>
        /// Compares two reset requests for inequality.
        /// </summary>
        /// <param name="ResetRequest1">A reset request.</param>
        /// <param name="ResetRequest2">Another reset request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ResetRequest? ResetRequest1,
                                           ResetRequest? ResetRequest2)

            => !(ResetRequest1 == ResetRequest2);

        #endregion

        #endregion

        #region IEquatable<ResetRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reset requests for equality.
        /// </summary>
        /// <param name="Object">A reset request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResetRequest resetRequest &&
                   Equals(resetRequest);

        #endregion

        #region Equals(ResetRequest)

        /// <summary>
        /// Compares two reset requests for equality.
        /// </summary>
        /// <param name="ResetRequest">A reset request to compare with.</param>
        public override Boolean Equals(ResetRequest? ResetRequest)

            => ResetRequest is not null &&

               ResetType.  Equals(ResetRequest.ResetType) &&

               base.GenericEquals(ResetRequest);

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

                return ResetType.GetHashCode() * 3 ^
                       base.     GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ResetType.ToString();

        #endregion

    }

}
