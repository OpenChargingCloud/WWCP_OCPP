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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A get report response.
    /// </summary>
    public class GetReportResponse : AResponse<CSMS.GetReportRequest,
                                               GetReportResponse>,
                                     IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getReportResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the get report command.
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

        #region GetReportResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Status">The success or failure of the get report command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetReportResponse(CSMS.GetReportRequest         Request,
                                 GenericDeviceModelStatus      Status,
                                 StatusInfo?                   StatusInfo          = null,
                                 DateTime?                     ResponseTimestamp   = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region GetReportResponse(Request, Result)

        /// <summary>
        /// Create a new get report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetReportResponse(CSMS.GetReportRequest  Request,
                                 Result                 Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetReportResponse",
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

        #region (static) Parse   (Request, JSON, CustomGetReportResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetReportResponseParser">A delegate to parse custom get report responses.</param>
        public static GetReportResponse Parse(CSMS.GetReportRequest                            Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<GetReportResponse>?  CustomGetReportResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getReportResponse,
                         out var errorResponse,
                         CustomGetReportResponseParser))
            {
                return getReportResponse;
            }

            throw new ArgumentException("The given JSON representation of a get report response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetReportResponse, out ErrorResponse, CustomGetReportResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get report response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetReportResponse">The parsed get report response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetReportResponseParser">A delegate to parse custom get report responses.</param>
        public static Boolean TryParse(CSMS.GetReportRequest                            Request,
                                       JObject                                          JSON,
                                       [NotNullWhen(true)]  out GetReportResponse?      GetReportResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<GetReportResponse>?  CustomGetReportResponseParser   = null)
        {

            try
            {

                GetReportResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "get report status",
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
                                           OCPP.StatusInfo.TryParse,
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetReportResponse = new GetReportResponse(
                                        Request,
                                        Status,
                                        StatusInfo,
                                        null,
                                        null,
                                        null,
                                        Signatures,
                                        CustomData
                                    );

                if (CustomGetReportResponseParser is not null)
                    GetReportResponse = CustomGetReportResponseParser(JSON,
                                                                      GetReportResponse);

                return true;

            }
            catch (Exception e)
            {
                GetReportResponse  = null;
                ErrorResponse      = "The given JSON representation of a get report response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetReportResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetReportResponseSerializer">A delegate to serialize custom get report responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetReportResponse>?  CustomGetReportResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?         CustomStatusInfoSerializer          = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?     CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
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

            return CustomGetReportResponseSerializer is not null
                       ? CustomGetReportResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get report command failed.
        /// </summary>
        /// <param name="Request">The get report request leading to this response.</param>
        public static GetReportResponse Failed(CSMS.GetReportRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetReportResponse1, GetReportResponse2)

        /// <summary>
        /// Compares two get report responses for equality.
        /// </summary>
        /// <param name="GetReportResponse1">A get report response.</param>
        /// <param name="GetReportResponse2">Another get report response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetReportResponse? GetReportResponse1,
                                           GetReportResponse? GetReportResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetReportResponse1, GetReportResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetReportResponse1 is null || GetReportResponse2 is null)
                return false;

            return GetReportResponse1.Equals(GetReportResponse2);

        }

        #endregion

        #region Operator != (GetReportResponse1, GetReportResponse2)

        /// <summary>
        /// Compares two get report responses for inequality.
        /// </summary>
        /// <param name="GetReportResponse1">A get report response.</param>
        /// <param name="GetReportResponse2">Another get report response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetReportResponse? GetReportResponse1,
                                           GetReportResponse? GetReportResponse2)

            => !(GetReportResponse1 == GetReportResponse2);

        #endregion

        #endregion

        #region IEquatable<GetReportResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get report responses for equality.
        /// </summary>
        /// <param name="Object">A get report response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetReportResponse getReportResponse &&
                   Equals(getReportResponse);

        #endregion

        #region Equals(GetReportResponse)

        /// <summary>
        /// Compares two get report responses for equality.
        /// </summary>
        /// <param name="GetReportResponse">A get report response to compare with.</param>
        public override Boolean Equals(GetReportResponse? GetReportResponse)

            => GetReportResponse is not null &&

               Status.     Equals(GetReportResponse.Status) &&

             ((StatusInfo is     null && GetReportResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetReportResponse.StatusInfo is not null && StatusInfo.Equals(GetReportResponse.StatusInfo)) &&

               base.GenericEquals(GetReportResponse);

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
