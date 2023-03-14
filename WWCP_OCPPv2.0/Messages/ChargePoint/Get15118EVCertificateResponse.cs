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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// The get 15118 EV certificate response.
    /// </summary>
    public class Get15118EVCertificateResponse : AResponse<CP.Get15118EVCertificateRequest,
                                                              Get15118EVCertificateResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the EXI message processing.
        /// </summary>
        [Mandatory]
        public ISO15118EVCertificateStatus  Status         { get; }

        /// <summary>
        /// Base64 encoded certificate installation response to the electric vehicle.
        /// [max 5600]
        /// </summary>
        [Mandatory]
        public EXIData                      EXIResponse    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                  StatusInfo     { get; }

        #endregion

        #region Constructor(s)

        #region Get15118EVCertificateResponse(Request, Status, EXIResponse, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the EXI message processing.</param>
        /// <param name="EXIResponse">Base64 encoded certificate installation response to the electric vehicle.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Get15118EVCertificateResponse(CP.Get15118EVCertificateRequest  Request,
                                             ISO15118EVCertificateStatus      Status,
                                             EXIData                          EXIResponse,
                                             StatusInfo?                      StatusInfo   = null,
                                             CustomData?                      CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status       = Status;
            this.EXIResponse  = EXIResponse;
            this.StatusInfo   = StatusInfo;

        }

        #endregion

        #region Get15118EVCertificateResponse(Request, Result)

        /// <summary>
        /// Create a new get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public Get15118EVCertificateResponse(CP.Get15118EVCertificateRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:Get15118EVCertificateResponse",
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
        //     "Iso15118EVCertificateStatusEnumType": {
        //       "description": "Indicates whether the message was processed properly.\r\n",
        //       "javaType": "Iso15118EVCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed"
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
        //       "$ref": "#/definitions/Iso15118EVCertificateStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "exiResponse": {
        //       "description": "Raw CertificateInstallationRes response for the EV, Base64 encoded.\r\n",
        //       "type": "string",
        //       "maxLength": 5600
        //     }
        //   },
        //   "required": [
        //     "status",
        //     "exiResponse"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGet15118EVCertificateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGet15118EVCertificateResponseParser">A delegate to parse custom get 15118 EV certificate responses.</param>
        public static Get15118EVCertificateResponse Parse(CP.Get15118EVCertificateRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var get15118EVCertificateResponse,
                         out var errorResponse,
                         CustomGet15118EVCertificateResponseParser))
            {
                return get15118EVCertificateResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get 15118 EV certificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out Get15118EVCertificateResponse, out ErrorResponse, CustomGet15118EVCertificateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Get15118EVCertificateResponse">The parsed get 15118 EV certificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGet15118EVCertificateResponseParser">A delegate to parse custom get 15118 EV certificate responses.</param>
        public static Boolean TryParse(CP.Get15118EVCertificateRequest                              Request,
                                       JObject                                                      JSON,
                                       out Get15118EVCertificateResponse?                           Get15118EVCertificateResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseParser   = null)
        {

            try
            {

                Get15118EVCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "ISO 15118 EV certificate status",
                                         ISO15118EVCertificateStatusExtensions.TryParse,
                                         out ISO15118EVCertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EXIRequest    [mandatory]

                if (!JSON.ParseMandatory("vendorId",
                                         "vendor identification",
                                         EXIData.TryParse,
                                         out EXIData EXIRequest,
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


                Get15118EVCertificateResponse = new Get15118EVCertificateResponse(Request,
                                                                                  Status,
                                                                                  EXIRequest,
                                                                                  StatusInfo,
                                                                                  CustomData);

                if (CustomGet15118EVCertificateResponseParser is not null)
                    Get15118EVCertificateResponse = CustomGet15118EVCertificateResponseParser(JSON,
                                                                                              Get15118EVCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                Get15118EVCertificateResponse  = null;
                ErrorResponse                  = "The given JSON representation of a get 15118 EV certificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGet15118EVCertificateResponseSerializer = null, CustomStatusInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGet15118EVCertificateResponseSerializer">A delegate to serialize custom get 15118 EV certificate responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("status",       Status.     AsText()),
                                 new JProperty("exiResponse",  EXIResponse.ToString()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo. ToJSON(CustomStatusInfoSerializer,
                                                                                  CustomCustomDataSerializer))
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGet15118EVCertificateResponseSerializer is not null
                       ? CustomGet15118EVCertificateResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get 15118 EV certificate failed.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        public static Get15118EVCertificateResponse Failed(CP.Get15118EVCertificateRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (Get15118EVCertificateResponse1, Get15118EVCertificateResponse2)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse1">A get 15118 EV certificate response.</param>
        /// <param name="Get15118EVCertificateResponse2">Another get 15118 EV certificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Get15118EVCertificateResponse? Get15118EVCertificateResponse1,
                                           Get15118EVCertificateResponse? Get15118EVCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Get15118EVCertificateResponse1, Get15118EVCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (Get15118EVCertificateResponse1 is null || Get15118EVCertificateResponse2 is null)
                return false;

            return Get15118EVCertificateResponse1.Equals(Get15118EVCertificateResponse2);

        }

        #endregion

        #region Operator != (Get15118EVCertificateResponse1, Get15118EVCertificateResponse2)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for inequality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse1">A get 15118 EV certificate response.</param>
        /// <param name="Get15118EVCertificateResponse2">Another get 15118 EV certificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Get15118EVCertificateResponse? Get15118EVCertificateResponse1,
                                           Get15118EVCertificateResponse? Get15118EVCertificateResponse2)

            => !(Get15118EVCertificateResponse1 == Get15118EVCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<Get15118EVCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for equality.
        /// </summary>
        /// <param name="Object">A get 15118 EV certificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Get15118EVCertificateResponse get15118EVCertificateResponse &&
                   Equals(get15118EVCertificateResponse);

        #endregion

        #region Equals(Get15118EVCertificateResponse)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse">A get 15118 EV certificate response to compare with.</param>
        public override Boolean Equals(Get15118EVCertificateResponse? Get15118EVCertificateResponse)

            => Get15118EVCertificateResponse is not null &&

               Status.     Equals(Get15118EVCertificateResponse.Status)      &&
               EXIResponse.Equals(Get15118EVCertificateResponse.EXIResponse) &&

             ((StatusInfo is     null && Get15118EVCertificateResponse.StatusInfo is     null) ||
               StatusInfo is not null && Get15118EVCertificateResponse.StatusInfo is not null && StatusInfo.Equals(Get15118EVCertificateResponse.StatusInfo)) &&

               base.GenericEquals(Get15118EVCertificateResponse);

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

                return Status.     GetHashCode()       * 7 ^
                       EXIResponse.GetHashCode()       * 5 ^
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
