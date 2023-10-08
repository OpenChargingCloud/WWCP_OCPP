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
    /// A get monitoring report response.
    /// </summary>
    public class GetMonitoringReportResponse : AResponse<CSMS.GetMonitoringReportRequest,
                                                         GetMonitoringReportResponse>,
                                               IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getMonitoringReportResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the get monitoring report command.
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

        #region GetMonitoringReportResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new get monitoring report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Status">The success or failure of the get monitoring report command.</param>
        /// <param name="Filename">The name of the log file that will be uploaded. This field is not present when no logging information is available.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public GetMonitoringReportResponse(CSMS.GetMonitoringReportRequest  Request,
                                           GenericDeviceModelStatus         Status,
                                           StatusInfo?                      StatusInfo        = null,

                                           IEnumerable<KeyPair>?            SignKeys          = null,
                                           IEnumerable<SignInfo>?           SignInfos         = null,
                                           SignaturePolicy?                 SignaturePolicy   = null,
                                           IEnumerable<Signature>?          Signatures        = null,

                                           DateTime?                        Timestamp         = null,
                                           CustomData?                      CustomData        = null)

            : base(Request,
                   Result.OK(),
                   SignKeys,
                   SignInfos,
                   SignaturePolicy,
                   Signatures,
                   Timestamp,
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region GetMonitoringReportResponse(Request, Result)

        /// <summary>
        /// Create a new get monitoring report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetMonitoringReportResponse(CSMS.GetMonitoringReportRequest  Request,
                                           Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetMonitoringReportResponse",
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
        //       "description": "This field indicates whether the Charging Station was able to accept the request.\r\n",
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

        #region (static) Parse   (Request, JSON, CustomGetMonitoringReportResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get monitoring report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetMonitoringReportResponseParser">A delegate to parse custom get monitoring report responses.</param>
        public static GetMonitoringReportResponse Parse(CSMS.GetMonitoringReportRequest                            Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<GetMonitoringReportResponse>?  CustomGetMonitoringReportResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getMonitoringReportResponse,
                         out var errorResponse,
                         CustomGetMonitoringReportResponseParser))
            {
                return getMonitoringReportResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get monitoring report response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetMonitoringReportResponse, out ErrorResponse, CustomGetMonitoringReportResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get monitoring report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetMonitoringReportResponse">The parsed get monitoring report response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetMonitoringReportResponseParser">A delegate to parse custom get monitoring report responses.</param>
        public static Boolean TryParse(CSMS.GetMonitoringReportRequest                            Request,
                                       JObject                                                    JSON,
                                       out GetMonitoringReportResponse?                           GetMonitoringReportResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetMonitoringReportResponse>?  CustomGetMonitoringReportResponseParser   = null)
        {

            try
            {

                GetMonitoringReportResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "get monitoring report status",
                                         GenericDeviceModelStatusExtensions.TryParse,
                                         out GenericDeviceModelStatus Status,
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

                #region Signatures    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
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


                GetMonitoringReportResponse = new GetMonitoringReportResponse(
                                                  Request,
                                                  Status,
                                                  StatusInfo,
                                                  null,
                                                  null,
                                                  null,
                                                  Signatures,
                                                  null,
                                                  CustomData
                                              );

                if (CustomGetMonitoringReportResponseParser is not null)
                    GetMonitoringReportResponse = CustomGetMonitoringReportResponseParser(JSON,
                                                                                          GetMonitoringReportResponse);

                return true;

            }
            catch (Exception e)
            {
                GetMonitoringReportResponse  = null;
                ErrorResponse                = "The given JSON representation of a get monitoring report response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetMonitoringReportResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetMonitoringReportResponseSerializer">A delegate to serialize custom get monitoring report responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetMonitoringReportResponse>?  CustomGetMonitoringReportResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                   CustomStatusInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetMonitoringReportResponseSerializer is not null
                       ? CustomGetMonitoringReportResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get monitoring report command failed.
        /// </summary>
        /// <param name="Request">The get monitoring report request leading to this response.</param>
        public static GetMonitoringReportResponse Failed(CSMS.GetMonitoringReportRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetMonitoringReportResponse1, GetMonitoringReportResponse2)

        /// <summary>
        /// Compares two get monitoring report responses for equality.
        /// </summary>
        /// <param name="GetMonitoringReportResponse1">A get monitoring report response.</param>
        /// <param name="GetMonitoringReportResponse2">Another get monitoring report response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetMonitoringReportResponse? GetMonitoringReportResponse1,
                                           GetMonitoringReportResponse? GetMonitoringReportResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetMonitoringReportResponse1, GetMonitoringReportResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetMonitoringReportResponse1 is null || GetMonitoringReportResponse2 is null)
                return false;

            return GetMonitoringReportResponse1.Equals(GetMonitoringReportResponse2);

        }

        #endregion

        #region Operator != (GetMonitoringReportResponse1, GetMonitoringReportResponse2)

        /// <summary>
        /// Compares two get monitoring report responses for inequality.
        /// </summary>
        /// <param name="GetMonitoringReportResponse1">A get monitoring report response.</param>
        /// <param name="GetMonitoringReportResponse2">Another get monitoring report response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetMonitoringReportResponse? GetMonitoringReportResponse1,
                                           GetMonitoringReportResponse? GetMonitoringReportResponse2)

            => !(GetMonitoringReportResponse1 == GetMonitoringReportResponse2);

        #endregion

        #endregion

        #region IEquatable<GetMonitoringReportResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get monitoring report responses for equality.
        /// </summary>
        /// <param name="Object">A get monitoring report response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetMonitoringReportResponse getMonitoringReportResponse &&
                   Equals(getMonitoringReportResponse);

        #endregion

        #region Equals(GetMonitoringReportResponse)

        /// <summary>
        /// Compares two get monitoring report responses for equality.
        /// </summary>
        /// <param name="GetMonitoringReportResponse">A get monitoring report response to compare with.</param>
        public override Boolean Equals(GetMonitoringReportResponse? GetMonitoringReportResponse)

            => GetMonitoringReportResponse is not null &&

               Status.     Equals(GetMonitoringReportResponse.Status) &&

             ((StatusInfo is     null && GetMonitoringReportResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetMonitoringReportResponse.StatusInfo is not null && StatusInfo.Equals(GetMonitoringReportResponse.StatusInfo)) &&

               base.GenericEquals(GetMonitoringReportResponse);

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
