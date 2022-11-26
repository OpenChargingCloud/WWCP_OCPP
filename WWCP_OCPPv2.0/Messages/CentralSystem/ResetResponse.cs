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
    /// A reset response.
    /// </summary>
    public class ResetResponse : AResponse<CS.ResetRequest,
                                              ResetResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reset command.
        /// </summary>
        [Mandatory]
        public ResetStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?  StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region ResetResponse(Request, Status)

        /// <summary>
        /// Create a new reset response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public ResetResponse(CS.ResetRequest  Request,
                             ResetStatus      Status,
                             StatusInfo?      StatusInfo   = null,
                             CustomData?      CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region ResetResponse(Request, Result)

        /// <summary>
        /// Create a new reset response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ResetResponse(CS.ResetRequest  Request,
                             Result           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ResetResponse",
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
        //     "ResetStatusEnumType": {
        //       "description": "This indicates whether the Charging Station is able to perform the reset.\r\n",
        //       "javaType": "ResetStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "Scheduled"
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
        //       "$ref": "#/definitions/ResetStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomResetResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a reset response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomResetResponseParser">A delegate to parse custom reset responses.</param>
        public static ResetResponse Parse(CS.ResetRequest                              Request,
                                          JObject                                      JSON,
                                          CustomJObjectParserDelegate<ResetResponse>?  CustomResetResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var resetResponse,
                         out var errorResponse,
                         CustomResetResponseParser))
            {
                return resetResponse!;
            }

            throw new ArgumentException("The given JSON representation of a reset response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ResetResponse, out ErrorResponse, CustomResetResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a reset response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ResetResponse">The parsed reset response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomResetResponseParser">A delegate to parse custom reset responses.</param>
        public static Boolean TryParse(CS.ResetRequest                              Request,
                                       JObject                                      JSON,
                                       out ResetResponse?                           ResetResponse,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<ResetResponse>?  CustomResetResponseParser   = null)
        {

            try
            {

                ResetResponse = null;

                #region ResetStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "reset status",
                                       ResetStatusExtentions.Parse,
                                       out ResetStatus ResetStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

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


                ResetResponse = new ResetResponse(Request,
                                                  ResetStatus,
                                                  StatusInfo,
                                                  CustomData);

                if (CustomResetResponseParser is not null)
                    ResetResponse = CustomResetResponseParser(JSON,
                                                              ResetResponse);

                return true;

            }
            catch (Exception e)
            {
                ResetResponse  = null;
                ErrorResponse  = "The given JSON representation of a reset response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomResetResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomResetResponseSerializer">A delegate to serialize custom reset responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status info objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ResetResponse>? CustomResetResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?    CustomStatusInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<CustomData>?    CustomCustomDataSerializer      = null)
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

            return CustomResetResponseSerializer is not null
                       ? CustomResetResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The reset command failed.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        public static ResetResponse Failed(CS.ResetRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse1">A reset response.</param>
        /// <param name="ResetResponse2">Another reset response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ResetResponse? ResetResponse1,
                                           ResetResponse? ResetResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ResetResponse1, ResetResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ResetResponse1 is null || ResetResponse2 is null)
                return false;

            return ResetResponse1.Equals(ResetResponse2);

        }

        #endregion

        #region Operator != (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two reset responses for inequality.
        /// </summary>
        /// <param name="ResetResponse1">A reset response.</param>
        /// <param name="ResetResponse2">Another reset response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ResetResponse? ResetResponse1,
                                           ResetResponse? ResetResponse2)

            => !(ResetResponse1 == ResetResponse2);

        #endregion

        #endregion

        #region IEquatable<ResetResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reset responses for equality.
        /// </summary>
        /// <param name="Object">A reset response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResetResponse resetResponse &&
                   Equals(resetResponse);

        #endregion

        #region Equals(ResetResponse)

        /// <summary>
        /// Compares two reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse">A reset response to compare with.</param>
        public override Boolean Equals(ResetResponse? ResetResponse)

            => ResetResponse is not null &&

               Status.     Equals(ResetResponse.Status) &&

             ((StatusInfo is     null && ResetResponse.StatusInfo is     null) ||
               StatusInfo is not null && ResetResponse.StatusInfo is not null && StatusInfo.Equals(ResetResponse.StatusInfo)) &&

               base.GenericEquals(ResetResponse);

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
