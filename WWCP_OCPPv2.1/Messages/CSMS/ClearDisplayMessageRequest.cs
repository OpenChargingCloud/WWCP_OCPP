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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The clear display message request.
    /// </summary>
    public class ClearDisplayMessageRequest : ARequest<ClearDisplayMessageRequest>
    {

        #region Properties

        /// <summary>
        /// The identification of the display message to be removed.
        /// </summary>
        [Mandatory]
        public DisplayMessage_Id  DisplayMessageId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new clear display message request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DisplayMessageId">The identification of the display message to be removed.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ClearDisplayMessageRequest(ChargeBox_Id       ChargeBoxId,
                                          DisplayMessage_Id  DisplayMessageId,
                                          CustomData?        CustomData          = null,

                                          Request_Id?        RequestId           = null,
                                          DateTime?          RequestTimestamp    = null,
                                          TimeSpan?          RequestTimeout      = null,
                                          EventTracking_Id?  EventTrackingId     = null,
                                          CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "ClearDisplayMessage",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.DisplayMessageId = DisplayMessageId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearDisplayMessageRequest",
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
        //     "id": {
        //       "description": "Id of the message that SHALL be removed from the Charging Station.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "id"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomClearDisplayMessageRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear display message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomClearDisplayMessageRequestParser">A delegate to parse custom clear display message requests.</param>
        public static ClearDisplayMessageRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       ChargeBox_Id                                              ChargeBoxId,
                                                       CustomJObjectParserDelegate<ClearDisplayMessageRequest>?  CustomClearDisplayMessageRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var clearDisplayMessageRequest,
                         out var errorResponse,
                         CustomClearDisplayMessageRequestParser))
            {
                return clearDisplayMessageRequest!;
            }

            throw new ArgumentException("The given JSON representation of a clear display message request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out ClearDisplayMessageRequest, out ErrorResponse, CustomClearDisplayMessageRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a clear display message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearDisplayMessageRequest">The parsed ClearDisplayMessage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       Request_Id                       RequestId,
                                       ChargeBox_Id                     ChargeBoxId,
                                       out ClearDisplayMessageRequest?  ClearDisplayMessageRequest,
                                       out String?                      ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out ClearDisplayMessageRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a clear display message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearDisplayMessageRequest">The parsed ClearDisplayMessage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearDisplayMessageRequestParser">A delegate to parse custom clear display message requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       ChargeBox_Id                                              ChargeBoxId,
                                       out ClearDisplayMessageRequest?                           ClearDisplayMessageRequest,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<ClearDisplayMessageRequest>?  CustomClearDisplayMessageRequestParser)
        {

            try
            {

                ClearDisplayMessageRequest = null;

                #region Id             [mandatory]

                if (JSON.ParseMandatory("id",
                                        "display message identification",
                                        DisplayMessage_Id.TryParse,
                                        out DisplayMessage_Id DisplayMessageId,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

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


                ClearDisplayMessageRequest = new ClearDisplayMessageRequest(ChargeBoxId,
                                                                            DisplayMessageId,
                                                                            CustomData,
                                                                            RequestId);

                if (CustomClearDisplayMessageRequestParser is not null)
                    ClearDisplayMessageRequest = CustomClearDisplayMessageRequestParser(JSON,
                                                                                        ClearDisplayMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearDisplayMessageRequest  = null;
                ErrorResponse               = "The given JSON representation of a clear display message request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearDisplayMessageRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearDisplayMessageRequestSerializer">A delegate to serialize custom clear display message requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearDisplayMessageRequest>?  CustomClearDisplayMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",           DisplayMessageId.Value),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearDisplayMessageRequestSerializer is not null
                       ? CustomClearDisplayMessageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearDisplayMessageRequest1, ClearDisplayMessageRequest2)

        /// <summary>
        /// Compares two clear display message requests for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageRequest1">A clear display message request.</param>
        /// <param name="ClearDisplayMessageRequest2">Another clear display message request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearDisplayMessageRequest? ClearDisplayMessageRequest1,
                                           ClearDisplayMessageRequest? ClearDisplayMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearDisplayMessageRequest1, ClearDisplayMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearDisplayMessageRequest1 is null || ClearDisplayMessageRequest2 is null)
                return false;

            return ClearDisplayMessageRequest1.Equals(ClearDisplayMessageRequest2);

        }

        #endregion

        #region Operator != (ClearDisplayMessageRequest1, ClearDisplayMessageRequest2)

        /// <summary>
        /// Compares two clear display message requests for inequality.
        /// </summary>
        /// <param name="ClearDisplayMessageRequest1">A clear display message request.</param>
        /// <param name="ClearDisplayMessageRequest2">Another clear display message request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearDisplayMessageRequest? ClearDisplayMessageRequest1,
                                           ClearDisplayMessageRequest? ClearDisplayMessageRequest2)

            => !(ClearDisplayMessageRequest1 == ClearDisplayMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearDisplayMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear display message requests for equality.
        /// </summary>
        /// <param name="Object">A clear display message request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearDisplayMessageRequest clearDisplayMessageRequest &&
                   Equals(clearDisplayMessageRequest);

        #endregion

        #region Equals(ClearDisplayMessageRequest)

        /// <summary>
        /// Compares two clear display message requests for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageRequest">A clear display message request to compare with.</param>
        public override Boolean Equals(ClearDisplayMessageRequest? ClearDisplayMessageRequest)

            => ClearDisplayMessageRequest is not null &&

               DisplayMessageId.Equals(ClearDisplayMessageRequest.DisplayMessageId) &&

               base.     GenericEquals(ClearDisplayMessageRequest);

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

                return DisplayMessageId.GetHashCode() * 3 ^
                       base.            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => DisplayMessageId.ToString();

        #endregion

    }

}
