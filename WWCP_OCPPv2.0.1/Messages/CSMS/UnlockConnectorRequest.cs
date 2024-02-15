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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// The unlock connector request.
    /// </summary>
    public class UnlockConnectorRequest : ARequest<UnlockConnectorRequest>
    {

        #region Properties

        /// <summary>
        /// The identifier of the EVSE to be unlocked.
        /// </summary>
        [Mandatory]
        public EVSE_Id       EVSEId         { get; }

        /// <summary>
        /// The identifier of the connector to be unlocked.
        /// </summary>
        [Mandatory]
        public Connector_Id  ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unlock connector request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="EVSEId">The identifier of the EVSE to be unlocked.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public UnlockConnectorRequest(ChargeBox_Id       ChargeBoxId,
                                      EVSE_Id            EVSEId,
                                      Connector_Id       ConnectorId,
                                      CustomData?        CustomData          = null,

                                      Request_Id?        RequestId           = null,
                                      DateTime?          RequestTimestamp    = null,
                                      TimeSpan?          RequestTimeout      = null,
                                      EventTracking_Id?  EventTrackingId     = null,
                                      CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "UnlockConnector",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.EVSEId       = EVSEId;
            this.ConnectorId  = ConnectorId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UnlockConnectorRequest",
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
        //     },
        //     "evseId": {
        //       "description": "This contains the identifier of the EVSE for which a connector needs to be unlocked.\r\n",
        //       "type": "integer"
        //     },
        //     "connectorId": {
        //       "description": "This contains the identifier of the connector that needs to be unlocked.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "evseId",
        //     "connectorId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomUnlockConnectorRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomUnlockConnectorRequestParser">An optional delegate to parse custom unlock connector requests.</param>
        public static UnlockConnectorRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   ChargeBox_Id                                          ChargeBoxId,
                                                   CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var unlockConnectorRequest,
                         out var errorResponse,
                         CustomUnlockConnectorRequestParser))
            {
                return unlockConnectorRequest!;
            }

            throw new ArgumentException("The given JSON representation of an unlock connector request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON,  RequestId, ChargeBoxId, out UnlockConnectorRequest, out ErrorResponse, CustomUnlockConnectorRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out UnlockConnectorRequest?  UnlockConnectorRequest,
                                       out String?                  ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out UnlockConnectorRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorRequestParser">An optional delegate to parse custom unlock connector requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       ChargeBox_Id                                          ChargeBoxId,
                                       out UnlockConnectorRequest?                           UnlockConnectorRequest,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser)
        {

            try
            {

                UnlockConnectorRequest = null;

                #region EVSEId         [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId    [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData     [optional]

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


                UnlockConnectorRequest = new UnlockConnectorRequest(ChargeBoxId,
                                                                    EVSEId,
                                                                    ConnectorId,
                                                                    CustomData,
                                                                    RequestId);

                if (CustomUnlockConnectorRequestParser is not null)
                    UnlockConnectorRequest = CustomUnlockConnectorRequestParser(JSON,
                                                                                UnlockConnectorRequest);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorRequest  = null;
                ErrorResponse           = "The given JSON representation of an unlock connector request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnlockConnectorRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorRequestSerializer">A delegate to serialize custom unlock connector requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",        EVSEId.     Value),

                                 new JProperty("connectorId",   ConnectorId.Value),

                           CustomData is not null
                               ? new JProperty("customData",    CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnlockConnectorRequestSerializer is not null
                       ? CustomUnlockConnectorRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorRequest1, UnlockConnectorRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorRequest1 is null || UnlockConnectorRequest2 is null)
                return false;

            return UnlockConnectorRequest1.Equals(UnlockConnectorRequest2);

        }

        #endregion

        #region Operator != (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for inequality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)

            => !(UnlockConnectorRequest1 == UnlockConnectorRequest2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="Object">An unlock connector request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorRequest unlockConnectorRequest &&
                   Equals(unlockConnectorRequest);

        #endregion

        #region Equals(UnlockConnectorRequest)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest">An unlock connector request to compare with.</param>
        public override Boolean Equals(UnlockConnectorRequest? UnlockConnectorRequest)

            => UnlockConnectorRequest is not null &&

               EVSEId.     Equals(UnlockConnectorRequest.EVSEId)      &&
               ConnectorId.Equals(UnlockConnectorRequest.ConnectorId) &&

               base.GenericEquals(UnlockConnectorRequest);

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

                return EVSEId.     GetHashCode() * 5 ^
                       ConnectorId.GetHashCode() * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   EVSEId,
                   " / ",
                   ConnectorId

               );

        #endregion

    }

}
