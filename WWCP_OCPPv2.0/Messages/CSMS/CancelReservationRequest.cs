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
    /// The cancel reservation request.
    /// </summary>
    public class CancelReservationRequest : ARequest<CancelReservationRequest>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the reservation to cancel.
        /// </summary>
        [Mandatory]
        public Reservation_Id  ReservationId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a cancel reservation request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public CancelReservationRequest(ChargeBox_Id        ChargeBoxId,

                                        Reservation_Id      ReservationId,

                                        CustomData?         CustomData          = null,
                                        Request_Id?         RequestId           = null,
                                        DateTime?           RequestTimestamp    = null,
                                        TimeSpan?           RequestTimeout      = null,
                                        EventTracking_Id?   EventTrackingId     = null,
                                        CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "CancelReservation",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ReservationId  = ReservationId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:CancelReservationRequest",
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
        //     "reservationId": {
        //       "description": "Id of the reservation to cancel.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "reservationId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomCancelReservationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cancel reservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomCancelReservationRequestParser">A delegate to parse custom CancelReservation requests.</param>
        public static CancelReservationRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     ChargeBox_Id                                            ChargeBoxId,
                                                     CustomJObjectParserDelegate<CancelReservationRequest>?  CustomCancelReservationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var cancelReservationRequest,
                         out var errorResponse,
                         CustomCancelReservationRequestParser))
            {
                return cancelReservationRequest!;
            }

            throw new ArgumentException("The given JSON representation of a cancel reservation request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out CancelReservationRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a cancel reservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CancelReservationRequest">The parsed cancel reservation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       ChargeBox_Id                   ChargeBoxId,
                                       out CancelReservationRequest?  CancelReservationRequest,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out CancelReservationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a cancel reservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CancelReservationRequest">The parsed CancelReservation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancelReservationRequestParser">A delegate to parse custom cancel reservation requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       ChargeBox_Id                                            ChargeBoxId,
                                       out CancelReservationRequest?                           CancelReservationRequest,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<CancelReservationRequest>?  CustomCancelReservationRequestParser)
        {

            try
            {

                CancelReservationRequest = null;

                #region ReservationId    [mandatory]

                if (!JSON.ParseMandatory("reservationId",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id ReservationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData       [optional]

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

                #region ChargeBoxId      [optional, OCPP_CSE]

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


                CancelReservationRequest = new CancelReservationRequest(ChargeBoxId,
                                                                        ReservationId,
                                                                        CustomData,
                                                                        RequestId);

                if (CustomCancelReservationRequestParser is not null)
                    CancelReservationRequest = CustomCancelReservationRequestParser(JSON,
                                                                                    CancelReservationRequest);

                return true;

            }
            catch (Exception e)
            {
                CancelReservationRequest  = null;
                ErrorResponse             = "The given JSON representation of a cancel reservation request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCancelReservationRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationRequestSerializer">A delegate to serialize custom cancel reservation requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationRequest>?  CustomCancelReservationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           new JProperty("reservationId",     ReservationId.Value),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCancelReservationRequestSerializer is not null
                       ? CustomCancelReservationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two CancelReservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A CancelReservation request.</param>
        /// <param name="CancelReservationRequest2">Another CancelReservation request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationRequest? CancelReservationRequest1,
                                           CancelReservationRequest? CancelReservationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CancelReservationRequest1, CancelReservationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (CancelReservationRequest1 is null || CancelReservationRequest2 is null)
                return false;

            return CancelReservationRequest1.Equals(CancelReservationRequest2);

        }

        #endregion

        #region Operator != (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two CancelReservation requests for inequality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A CancelReservation request.</param>
        /// <param name="CancelReservationRequest2">Another CancelReservation request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationRequest? CancelReservationRequest1,
                                           CancelReservationRequest? CancelReservationRequest2)

            => !(CancelReservationRequest1 == CancelReservationRequest2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cancel reservation requests for equality.
        /// </summary>
        /// <param name="Object">A cancel reservation request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CancelReservationRequest cancelReservationRequest &&
                   Equals(cancelReservationRequest);

        #endregion

        #region Equals(CancelReservationRequest)

        /// <summary>
        /// Compares two cancel reservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest">A cancel reservation request to compare with.</param>
        public override Boolean Equals(CancelReservationRequest? CancelReservationRequest)

            => CancelReservationRequest is not null &&

               ReservationId.Equals(CancelReservationRequest.ReservationId) &&

               base.  GenericEquals(CancelReservationRequest);

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

                return ReservationId.GetHashCode() * 3 ^
                       base.         GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ReservationId.ToString();

        #endregion

    }

}
