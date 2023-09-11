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

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// A reservation status update response.
    /// </summary>
    public class ReservationStatusUpdateResponse : AResponse<CS.ReservationStatusUpdateRequest,
                                                             ReservationStatusUpdateResponse>
    {

        #region Constructor(s)

        #region ReservationStatusUpdateResponse(Request, ...)

        /// <summary>
        /// Create a new reservation status update response.
        /// </summary>
        /// <param name="Request">The reservation status update request leading to this response.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public ReservationStatusUpdateResponse(CS.ReservationStatusUpdateRequest  Request,
                                               CustomData?                        CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        { }

        #endregion

        #region ReservationStatusUpdateResponse(Request, Result)

        /// <summary>
        /// Create a new reservation status update response.
        /// </summary>
        /// <param name="Request">The reservation status update request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ReservationStatusUpdateResponse(CS.ReservationStatusUpdateRequest  Request,
                                               Result                             Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ReservationStatusUpdateResponse",
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

        #region (static) Parse   (Request, JSON, CustomReservationStatusUpdateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a reservation status update response.
        /// </summary>
        /// <param name="Request">The reservation status update request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomReservationStatusUpdateResponseParser">A delegate to parse custom reservation status update responses.</param>
        public static ReservationStatusUpdateResponse Parse(CS.ReservationStatusUpdateRequest                              Request,
                                                            JObject                                                        JSON,
                                                            CustomJObjectParserDelegate<ReservationStatusUpdateResponse>?  CustomReservationStatusUpdateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var reservationStatusUpdateResponse,
                         out var errorResponse,
                         CustomReservationStatusUpdateResponseParser))
            {
                return reservationStatusUpdateResponse!;
            }

            throw new ArgumentException("The given JSON representation of a reservation status update response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ReservationStatusUpdateResponse, out ErrorResponse, CustomReservationStatusUpdateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a reservation status update response.
        /// </summary>
        /// <param name="Request">The reservation status update request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ReservationStatusUpdateResponse">The parsed reservation status update response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReservationStatusUpdateResponseParser">A delegate to parse custom reservation status update responses.</param>
        public static Boolean TryParse(CS.ReservationStatusUpdateRequest                              Request,
                                       JObject                                                        JSON,
                                       out ReservationStatusUpdateResponse?                           ReservationStatusUpdateResponse,
                                       out String?                                                    ErrorResponse,
                                       CustomJObjectParserDelegate<ReservationStatusUpdateResponse>?  CustomReservationStatusUpdateResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                ReservationStatusUpdateResponse = null;

                #region CustomData    [optional]

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


                ReservationStatusUpdateResponse = new ReservationStatusUpdateResponse(
                                                      Request,
                                                      CustomData
                                                  );

                if (CustomReservationStatusUpdateResponseParser is not null)
                    ReservationStatusUpdateResponse = CustomReservationStatusUpdateResponseParser(JSON,
                                                                                                  ReservationStatusUpdateResponse);

                return true;

            }
            catch (Exception e)
            {
                ReservationStatusUpdateResponse  = null;
                ErrorResponse                    = "The given JSON representation of a reservation status update response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReservationStatusUpdateResponseSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReservationStatusUpdateResponseSerializer">A delegate to serialize custom reservation status update responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReservationStatusUpdateResponse>?  CustomReservationStatusUpdateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomReservationStatusUpdateResponseSerializer is not null
                       ? CustomReservationStatusUpdateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The reservation status update request failed.
        /// </summary>
        public static ReservationStatusUpdateResponse Failed(CS.ReservationStatusUpdateRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ReservationStatusUpdateResponse1, ReservationStatusUpdateResponse2)

        /// <summary>
        /// Compares two reservation status update responses for equality.
        /// </summary>
        /// <param name="ReservationStatusUpdateResponse1">A reservation status update response.</param>
        /// <param name="ReservationStatusUpdateResponse2">Another reservation status update response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReservationStatusUpdateResponse? ReservationStatusUpdateResponse1,
                                           ReservationStatusUpdateResponse? ReservationStatusUpdateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReservationStatusUpdateResponse1, ReservationStatusUpdateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ReservationStatusUpdateResponse1 is null || ReservationStatusUpdateResponse2 is null)
                return false;

            return ReservationStatusUpdateResponse1.Equals(ReservationStatusUpdateResponse2);

        }

        #endregion

        #region Operator != (ReservationStatusUpdateResponse1, ReservationStatusUpdateResponse2)

        /// <summary>
        /// Compares two reservation status update responses for inequality.
        /// </summary>
        /// <param name="ReservationStatusUpdateResponse1">A reservation status update response.</param>
        /// <param name="ReservationStatusUpdateResponse2">Another reservation status update response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReservationStatusUpdateResponse? ReservationStatusUpdateResponse1,
                                           ReservationStatusUpdateResponse? ReservationStatusUpdateResponse2)

            => !(ReservationStatusUpdateResponse1 == ReservationStatusUpdateResponse2);

        #endregion

        #endregion

        #region IEquatable<ReservationStatusUpdateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reservation status update responses for equality.
        /// </summary>
        /// <param name="Object">A reservation status update response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReservationStatusUpdateResponse reservationStatusUpdateResponse &&
                   Equals(reservationStatusUpdateResponse);

        #endregion

        #region Equals(ReservationStatusUpdateResponse)

        /// <summary>
        /// Compares two reservation status update responses for equality.
        /// </summary>
        /// <param name="ReservationStatusUpdateResponse">A reservation status update response to compare with.</param>
        public override Boolean Equals(ReservationStatusUpdateResponse? ReservationStatusUpdateResponse)

            => ReservationStatusUpdateResponse is not null &&
                   base.GenericEquals(ReservationStatusUpdateResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "ReservationStatusUpdateResponse";

        #endregion

    }

}
