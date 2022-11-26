﻿/*
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
    /// A clear charging profile response.
    /// </summary>
    public class ClearChargingProfileResponse : AResponse<CS.ClearChargingProfileRequest,
                                                             ClearChargingProfileResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the clear charging profile command.
        /// </summary>
        [Mandatory]
        public ClearChargingProfileStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                 StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region ClearChargingProfileResponse(Request, Status)

        /// <summary>
        /// Create a new clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public ClearChargingProfileResponse(CS.ClearChargingProfileRequest  Request,
                                            ClearChargingProfileStatus      Status,
                                            StatusInfo?                     StatusInfo   = null,
                                            CustomData?                     CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region ClearChargingProfileResponse(Request, Result)

        /// <summary>
        /// Create a new clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearChargingProfileResponse(CS.ClearChargingProfileRequest  Request,
                                            Result                          Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearChargingProfileResponse",
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
        //     "ClearChargingProfileStatusEnumType": {
        //       "description": "Indicates if the Charging Station was able to execute the request.\r\n",
        //       "javaType": "ClearChargingProfileStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Unknown"
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
        //       "$ref": "#/definitions/ClearChargingProfileStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomClearChargingProfileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearChargingProfileResponseParser">A delegate to parse custom clear charging profile responses.</param>
        public static ClearChargingProfileResponse Parse(CS.ClearChargingProfileRequest                              Request,
                                                         JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var clearChargingProfileResponse,
                         out var errorResponse,
                         CustomClearChargingProfileResponseParser))
            {
                return clearChargingProfileResponse!;
            }

            throw new ArgumentException("The given JSON representation of a clear charging profile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearChargingProfileResponse, out ErrorResponse, CustomClearChargingProfileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileResponseParser">A delegate to parse custom clear charging profile responses.</param>
        public static Boolean TryParse(CS.ClearChargingProfileRequest                              Request,
                                       JObject                                                     JSON,
                                       out ClearChargingProfileResponse?                           ClearChargingProfileResponse,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseParser   = null)
        {

            try
            {

                ClearChargingProfileResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "clear charging profile status",
                                       ClearChargingProfileStatusExtentions.Parse,
                                       out ClearChargingProfileStatus Status,
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


                ClearChargingProfileResponse = new ClearChargingProfileResponse(Request,
                                                                                Status,
                                                                                StatusInfo,
                                                                                CustomData);

                if (CustomClearChargingProfileResponseParser is not null)
                    ClearChargingProfileResponse = CustomClearChargingProfileResponseParser(JSON,
                                                                                            ClearChargingProfileResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileResponse  = null;
                ErrorResponse                 = "The given JSON representation of a clear charging profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearChargingProfileSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileSerializer">A delegate to serialize custom clear charging profile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status info objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                    CustomStatusInfoSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearChargingProfileSerializer is not null
                       ? CustomClearChargingProfileSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The clear charging profile command failed.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        public static ClearChargingProfileResponse Failed(CS.ClearChargingProfileRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileResponse? ClearChargingProfileResponse1,
                                           ClearChargingProfileResponse? ClearChargingProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfileResponse1, ClearChargingProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearChargingProfileResponse1 is null || ClearChargingProfileResponse2 is null)
                return false;

            return ClearChargingProfileResponse1.Equals(ClearChargingProfileResponse2);

        }

        #endregion

        #region Operator != (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileResponse? ClearChargingProfileResponse1,
                                           ClearChargingProfileResponse? ClearChargingProfileResponse2)

            => !(ClearChargingProfileResponse1 == ClearChargingProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="Object">A clear charging profile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileResponse clearChargingProfileResponse &&
                   Equals(clearChargingProfileResponse);

        #endregion

        #region Equals(ClearChargingProfileResponse)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse">A clear charging profile response to compare with.</param>
        public override Boolean Equals(ClearChargingProfileResponse? ClearChargingProfileResponse)

            => ClearChargingProfileResponse is not null &&

               Status.     Equals(ClearChargingProfileResponse.Status) &&

             ((StatusInfo is     null && ClearChargingProfileResponse.StatusInfo is     null) ||
               StatusInfo is not null && ClearChargingProfileResponse.StatusInfo is not null && StatusInfo.Equals(ClearChargingProfileResponse.StatusInfo)) &&

               base.GenericEquals(ClearChargingProfileResponse);

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

            => Status.AsText();

        #endregion

    }

}
