/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    /// <summary>
    /// A cancel reservation response.
    /// </summary>
    public class CancelReservationResponse : AResponse<CS.CancelReservationRequest,
                                                          CancelReservationResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reservation cancellation.
        /// </summary>
        public CancelReservationStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        public StatusInfo?              StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region CancelReservationResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public CancelReservationResponse(CS.CancelReservationRequest  Request,
                                         CancelReservationStatus      Status,
                                         StatusInfo?                  StatusInfo   = null,
                                         CustomData?                  CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region CancelReservationResponse(Request, Result)

        /// <summary>
        /// Create a new cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public CancelReservationResponse(CS.CancelReservationRequest  Request,
                                         Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:CancelReservationResponse",
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
        //     "CancelReservationStatusEnumType": {
        //       "description": "This indicates the success or failure of the canceling of a reservation by CSMS.\r\n",
        //       "javaType": "CancelReservationStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.\r\n",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "reasonCode"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/CancelReservationStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomCancelReservationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCancelReservationResponseParser">A delegate to parse custom cancel reservation responses.</param>
        public static CancelReservationResponse Parse(CS.CancelReservationRequest                              Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var cancelReservationResponse,
                         out var errorResponse,
                         CustomCancelReservationResponseParser))
            {
                return cancelReservationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a cancel reservation response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CancelReservationResponse, out ErrorResponse, CustomCancelReservationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancelReservationResponseParser">A delegate to parse custom cancel reservation responses.</param>
        public static Boolean TryParse(CS.CancelReservationRequest                              Request,
                                       JObject                                                  JSON,
                                       out CancelReservationResponse?                           CancelReservationResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null)
        {

            try
            {

                CancelReservationResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "cancel reservation status",
                                       CancelReservationStatusExtentions.Parse,
                                       out CancelReservationStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_0.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

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


                CancelReservationResponse = new CancelReservationResponse(Request,
                                                                          Status,
                                                                          StatusInfo,
                                                                          CustomData);

                if (CustomCancelReservationResponseParser is not null)
                    CancelReservationResponse = CustomCancelReservationResponseParser(JSON,
                                                                                      CancelReservationResponse);

                return true;

            }
            catch (Exception e)
            {
                CancelReservationResponse  = null;
                ErrorResponse              = "The given JSON representation of a cancel reservation response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCancelReservationResponseSerializer = null, CustomStatusInfoResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationResponseSerializer">A delegate to serialize custom cancel reservation responses.</param>
        /// <param name="CustomStatusInfoResponseSerializer">A delegate to serialize a custom status info objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationResponse>?  CustomCancelReservationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoResponseSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataResponseSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoResponseSerializer,
                                                                                CustomCustomDataResponseSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomCancelReservationResponseSerializer is not null
                       ? CustomCancelReservationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The cancel reservation failed.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        public static CancelReservationResponse Failed(CS.CancelReservationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationResponse? CancelReservationResponse1,
                                           CancelReservationResponse? CancelReservationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CancelReservationResponse1, CancelReservationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (CancelReservationResponse1 is null || CancelReservationResponse2 is null)
                return false;

            return CancelReservationResponse1.Equals(CancelReservationResponse2);

        }

        #endregion

        #region Operator != (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for inequality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationResponse? CancelReservationResponse1,
                                           CancelReservationResponse? CancelReservationResponse2)

            => !(CancelReservationResponse1 == CancelReservationResponse2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="Object">A cancel reservation response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CancelReservationResponse cancelReservationResponse &&
                   Equals(cancelReservationResponse);

        #endregion

        #region Equals(CancelReservationResponse)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse">A cancel reservation response to compare with.</param>
        public override Boolean Equals(CancelReservationResponse? CancelReservationResponse)

            => CancelReservationResponse is not null &&

               Status.     Equals(CancelReservationResponse.Status) &&

             ((StatusInfo is     null && CancelReservationResponse.StatusInfo is     null) ||
               StatusInfo is not null && CancelReservationResponse.StatusInfo is not null && StatusInfo.Equals(CancelReservationResponse.StatusInfo)) &&

               base.GenericEquals(CancelReservationResponse);

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

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
