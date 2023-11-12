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
    /// A get charging profiles response.
    /// </summary>
    public class GetChargingProfilesResponse : AResponse<CSMS.GetChargingProfilesRequest,
                                                         GetChargingProfilesResponse>,
                                               IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getChargingProfilesResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the charging station is able to accept this request.
        /// </summary>
        [Mandatory]
        public GetChargingProfileStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region GetChargingProfilesResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get charging profiles response.
        /// </summary>
        /// <param name="Request">The get charging profiles request leading to this response.</param>
        /// <param name="Status">Whether the charging station is able to accept this request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetChargingProfilesResponse(CSMS.GetChargingProfilesRequest   Request,
                                           GetChargingProfileStatus          Status,
                                           StatusInfo?                       StatusInfo          = null,
                                           DateTime?                         ResponseTimestamp   = null,

                                           IEnumerable<KeyPair>?             SignKeys            = null,
                                           IEnumerable<SignInfo>?            SignInfos           = null,
                                           IEnumerable<Signature>?           Signatures          = null,

                                           CustomData?                       CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region GetChargingProfilesResponse(Request, Result)

        /// <summary>
        /// Create a new get charging profiles response.
        /// </summary>
        /// <param name="Request">The get charging profiles request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetChargingProfilesResponse(CSMS.GetChargingProfilesRequest  Request,
                                           Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetChargingProfilesResponse",
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
        //     "GetChargingProfileStatusEnumType": {
        //       "description": "This indicates whether the Charging Station is able to process this request and will send &lt;&lt;reportchargingprofilesrequest, ReportChargingProfilesRequest&gt;&gt; messages.\r\n",
        //       "javaType": "GetChargingProfileStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "NoProfiles"
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
        //       "$ref": "#/definitions/GetChargingProfileStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomGetChargingProfilesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get charging profiles response.
        /// </summary>
        /// <param name="Request">The get charging profiles request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetChargingProfilesResponse Parse(CSMS.GetChargingProfilesRequest                            Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<GetChargingProfilesResponse>?  CustomGetChargingProfilesResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getChargingProfilesResponse,
                         out var errorResponse,
                         CustomGetChargingProfilesResponseParser) &&
                getChargingProfilesResponse is not null)
            {
                return getChargingProfilesResponse;
            }

            throw new ArgumentException("The given JSON representation of a get charging profiles response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetChargingProfilesResponse, out ErrorResponse, CustomGetChargingProfilesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get charging profiles response.
        /// </summary>
        /// <param name="Request">The get charging profiles request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetChargingProfilesResponse">The parsed get charging profiles response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetChargingProfilesResponseParser">A delegate to parse custom get charging profiles responses.</param>
        public static Boolean TryParse(CSMS.GetChargingProfilesRequest                            Request,
                                       JObject                                                    JSON,
                                       out GetChargingProfilesResponse?                           GetChargingProfilesResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetChargingProfilesResponse>?  CustomGetChargingProfilesResponseParser   = null)
        {

            try
            {

                GetChargingProfilesResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic device model status",
                                         GetChargingProfileStatusExtensions.TryParse,
                                         out GetChargingProfileStatus GetChargingProfilesStatus,
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


                GetChargingProfilesResponse = new GetChargingProfilesResponse(
                                                  Request,
                                                  GetChargingProfilesStatus,
                                                  StatusInfo,
                                                  null,
                                                  null,
                                                  null,
                                                  Signatures,
                                                  CustomData
                                              );

                if (CustomGetChargingProfilesResponseParser is not null)
                    GetChargingProfilesResponse = CustomGetChargingProfilesResponseParser(JSON,
                                                                                          GetChargingProfilesResponse);

                return true;

            }
            catch (Exception e)
            {
                GetChargingProfilesResponse  = null;
                ErrorResponse                = "The given JSON representation of a get charging profiles response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetChargingProfilesResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetChargingProfilesResponseSerializer">A delegate to serialize custom get charging profiles responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetChargingProfilesResponse>?  CustomGetChargingProfilesResponseSerializer   = null,
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

            return CustomGetChargingProfilesResponseSerializer is not null
                       ? CustomGetChargingProfilesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get charging profiles command failed.
        /// </summary>
        /// <param name="Request">The get charging profiles request leading to this response.</param>
        public static GetChargingProfilesResponse Failed(CSMS.GetChargingProfilesRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetChargingProfilesResponse1, GetChargingProfilesResponse2)

        /// <summary>
        /// Compares two get charging profiles responses for equality.
        /// </summary>
        /// <param name="GetChargingProfilesResponse1">A get charging profiles response.</param>
        /// <param name="GetChargingProfilesResponse2">Another get charging profiles response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetChargingProfilesResponse? GetChargingProfilesResponse1,
                                           GetChargingProfilesResponse? GetChargingProfilesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetChargingProfilesResponse1, GetChargingProfilesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetChargingProfilesResponse1 is null || GetChargingProfilesResponse2 is null)
                return false;

            return GetChargingProfilesResponse1.Equals(GetChargingProfilesResponse2);

        }

        #endregion

        #region Operator != (GetChargingProfilesResponse1, GetChargingProfilesResponse2)

        /// <summary>
        /// Compares two get charging profiles responses for inequality.
        /// </summary>
        /// <param name="GetChargingProfilesResponse1">A get charging profiles response.</param>
        /// <param name="GetChargingProfilesResponse2">Another get charging profiles response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetChargingProfilesResponse? GetChargingProfilesResponse1,
                                           GetChargingProfilesResponse? GetChargingProfilesResponse2)

            => !(GetChargingProfilesResponse1 == GetChargingProfilesResponse2);

        #endregion

        #endregion

        #region IEquatable<GetChargingProfilesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get charging profiles responses for equality.
        /// </summary>
        /// <param name="Object">A get charging profiles response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetChargingProfilesResponse getChargingProfilesResponse &&
                   Equals(getChargingProfilesResponse);

        #endregion

        #region Equals(GetChargingProfilesResponse)

        /// <summary>
        /// Compares two get charging profiles responses for equality.
        /// </summary>
        /// <param name="GetChargingProfilesResponse">A get charging profiles response to compare with.</param>
        public override Boolean Equals(GetChargingProfilesResponse? GetChargingProfilesResponse)

            => GetChargingProfilesResponse is not null &&

               Status.     Equals(GetChargingProfilesResponse.Status) &&

             ((StatusInfo is     null && GetChargingProfilesResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetChargingProfilesResponse.StatusInfo is not null && StatusInfo.Equals(GetChargingProfilesResponse.StatusInfo)) &&

               base.GenericEquals(GetChargingProfilesResponse);

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
