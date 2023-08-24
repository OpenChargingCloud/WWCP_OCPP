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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A get base report response.
    /// </summary>
    public class GetBaseReportResponse : AResponse<CSMS.GetBaseReportRequest,
                                                   GetBaseReportResponse>
    {

        #region Properties

        /// <summary>
        /// Whether the charging station is able to accept this request.
        /// </summary>
        [Mandatory]
        public GenericDeviceModelStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region GetBaseReportResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new get base report response.
        /// </summary>
        /// <param name="Request">The get base report request leading to this response.</param>
        /// <param name="Status">Whether the charging station is able to accept this request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public GetBaseReportResponse(CSMS.GetBaseReportRequest  Request,
                                     GenericDeviceModelStatus   Status,
                                     StatusInfo?                StatusInfo   = null,
                                     CustomData?                CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region GetBaseReportResponse(Request, Result)

        /// <summary>
        /// Create a new get base report response.
        /// </summary>
        /// <param name="Request">The get base report request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetBaseReportResponse(CSMS.GetBaseReportRequest  Request,
                                     Result                     Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetBaseReportResponse",
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
        //     "GenericDeviceModelStatusEnumType": {
        //       "description": "This indicates whether the Charging Station is able to accept this request.\r\n",
        //       "javaType": "GenericDeviceModelStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "NotSupported",
        //         "EmptyResultSet"
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
        //       "$ref": "#/definitions/GenericDeviceModelStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomGetBaseReportResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get base report response.
        /// </summary>
        /// <param name="Request">The get base report request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetBaseReportResponse Parse(CSMS.GetBaseReportRequest                            Request,
                                                  JObject                                              JSON,
                                                  CustomJObjectParserDelegate<GetBaseReportResponse>?  CustomGetBaseReportResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getBaseReportResponse,
                         out var errorResponse,
                         CustomGetBaseReportResponseParser))
            {
                return getBaseReportResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get base report response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetBaseReportResponse, out ErrorResponse, CustomGetBaseReportResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get base report response.
        /// </summary>
        /// <param name="Request">The get base report request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetBaseReportResponse">The parsed get base report response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetBaseReportResponseParser">A delegate to parse custom get base report responses.</param>
        public static Boolean TryParse(CSMS.GetBaseReportRequest                            Request,
                                       JObject                                              JSON,
                                       out GetBaseReportResponse?                           GetBaseReportResponse,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<GetBaseReportResponse>?  CustomGetBaseReportResponseParser   = null)
        {

            try
            {

                GetBaseReportResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic device model status",
                                         GenericDeviceModelStatusExtensions.TryParse,
                                         out GenericDeviceModelStatus GetBaseReportStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetBaseReportResponse = new GetBaseReportResponse(Request,
                                                                  GetBaseReportStatus,
                                                                  StatusInfo,
                                                                  CustomData);

                if (CustomGetBaseReportResponseParser is not null)
                    GetBaseReportResponse = CustomGetBaseReportResponseParser(JSON,
                                                                              GetBaseReportResponse);

                return true;

            }
            catch (Exception e)
            {
                GetBaseReportResponse  = null;
                ErrorResponse          = "The given JSON representation of a get base report response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetBaseReportResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetBaseReportResponseSerializer">A delegate to serialize custom get base report responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetBaseReportResponse>?  CustomGetBaseReportResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?             CustomStatusInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetBaseReportResponseSerializer is not null
                       ? CustomGetBaseReportResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get base report command failed.
        /// </summary>
        /// <param name="Request">The get base report request leading to this response.</param>
        public static GetBaseReportResponse Failed(CSMS.GetBaseReportRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetBaseReportResponse1, GetBaseReportResponse2)

        /// <summary>
        /// Compares two get base report responses for equality.
        /// </summary>
        /// <param name="GetBaseReportResponse1">A get base report response.</param>
        /// <param name="GetBaseReportResponse2">Another get base report response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetBaseReportResponse? GetBaseReportResponse1,
                                           GetBaseReportResponse? GetBaseReportResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetBaseReportResponse1, GetBaseReportResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetBaseReportResponse1 is null || GetBaseReportResponse2 is null)
                return false;

            return GetBaseReportResponse1.Equals(GetBaseReportResponse2);

        }

        #endregion

        #region Operator != (GetBaseReportResponse1, GetBaseReportResponse2)

        /// <summary>
        /// Compares two get base report responses for inequality.
        /// </summary>
        /// <param name="GetBaseReportResponse1">A get base report response.</param>
        /// <param name="GetBaseReportResponse2">Another get base report response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetBaseReportResponse? GetBaseReportResponse1,
                                           GetBaseReportResponse? GetBaseReportResponse2)

            => !(GetBaseReportResponse1 == GetBaseReportResponse2);

        #endregion

        #endregion

        #region IEquatable<GetBaseReportResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get base report responses for equality.
        /// </summary>
        /// <param name="Object">A get base report response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetBaseReportResponse getBaseReportResponse &&
                   Equals(getBaseReportResponse);

        #endregion

        #region Equals(GetBaseReportResponse)

        /// <summary>
        /// Compares two get base report responses for equality.
        /// </summary>
        /// <param name="GetBaseReportResponse">A get base report response to compare with.</param>
        public override Boolean Equals(GetBaseReportResponse? GetBaseReportResponse)

            => GetBaseReportResponse is not null &&

               Status.     Equals(GetBaseReportResponse.Status) &&

             ((StatusInfo is     null && GetBaseReportResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetBaseReportResponse.StatusInfo is not null && StatusInfo.Equals(GetBaseReportResponse.StatusInfo)) &&

               base.GenericEquals(GetBaseReportResponse);

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
