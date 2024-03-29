﻿/*
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
    /// A get display messages response.
    /// </summary>
    public class GetDisplayMessagesResponse : AResponse<CSMS.GetDisplayMessagesRequest,
                                                           GetDisplayMessagesResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getDisplayMessagesResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charging station will indicate whether it has display messages
        /// that match the request criteria.
        /// </summary>
        [Mandatory]
        public GetDisplayMessagesStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region GetDisplayMessagesResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get display messages response.
        /// </summary>
        /// <param name="Request">The get display messages request leading to this response.</param>
        /// <param name="Status">The charging station will indicate whether it has display messages that match the request criteria.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetDisplayMessagesResponse(CSMS.GetDisplayMessagesRequest  Request,
                                          GetDisplayMessagesStatus        Status,
                                          StatusInfo?                     StatusInfo          = null,
                                          DateTime?                       ResponseTimestamp   = null,

                                          IEnumerable<KeyPair>?           SignKeys            = null,
                                          IEnumerable<SignInfo>?          SignInfos           = null,
                                          IEnumerable<OCPP.Signature>?    Signatures          = null,

                                          CustomData?                     CustomData          = null)

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

        #region GetDisplayMessagesResponse(Request, Result)

        /// <summary>
        /// Create a new get display messages response.
        /// </summary>
        /// <param name="Request">The get display messages request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetDisplayMessagesResponse(CSMS.GetDisplayMessagesRequest  Request,
                                          Result                          Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetDisplayMessagesResponse",
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
        //     "GetDisplayMessagesStatusEnumType": {
        //       "description": "Indicates if the Charging Station has Display Messages that match the request criteria in the &lt;&lt;getdisplaymessagesrequest,GetDisplayMessagesRequest&gt;&gt;\r\n",
        //       "javaType": "GetDisplayMessagesStatusEnum",
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
        //       "$ref": "#/definitions/GetDisplayMessagesStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomGetDisplayMessagesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get display messages response.
        /// </summary>
        /// <param name="Request">The get display messages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetDisplayMessagesResponseParser">A delegate to parse custom get display messages responses.</param>
        public static GetDisplayMessagesResponse Parse(CSMS.GetDisplayMessagesRequest                            Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<GetDisplayMessagesResponse>?  CustomGetDisplayMessagesResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getDisplayMessagesResponse,
                         out var errorResponse,
                         CustomGetDisplayMessagesResponseParser))
            {
                return getDisplayMessagesResponse;
            }

            throw new ArgumentException("The given JSON representation of a get display messages response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetDisplayMessagesResponse, out ErrorResponse, CustomGetDisplayMessagesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get display messages response.
        /// </summary>
        /// <param name="Request">The get display messages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetDisplayMessagesResponse">The parsed get display messages response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetDisplayMessagesResponseParser">A delegate to parse custom get display messages responses.</param>
        public static Boolean TryParse(CSMS.GetDisplayMessagesRequest                            Request,
                                       JObject                                                   JSON,
                                       [NotNullWhen(true)]  out GetDisplayMessagesResponse?      GetDisplayMessagesResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       CustomJObjectParserDelegate<GetDisplayMessagesResponse>?  CustomGetDisplayMessagesResponseParser   = null)
        {

            try
            {

                GetDisplayMessagesResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "get display messages status",
                                         GetDisplayMessagesStatusExtensions.TryParse,
                                         out GetDisplayMessagesStatus Status,
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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetDisplayMessagesResponse = new GetDisplayMessagesResponse(
                                                 Request,
                                                 Status,
                                                 StatusInfo,
                                                 null,
                                                 null,
                                                 null,
                                                 Signatures,
                                                 CustomData
                                             );

                if (CustomGetDisplayMessagesResponseParser is not null)
                    GetDisplayMessagesResponse = CustomGetDisplayMessagesResponseParser(JSON,
                                                                                        GetDisplayMessagesResponse);

                return true;

            }
            catch (Exception e)
            {
                GetDisplayMessagesResponse  = null;
                ErrorResponse               = "The given JSON representation of a get display messages response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDisplayMessagesResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDisplayMessagesResponseSerializer">A delegate to serialize custom get display messages responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDisplayMessagesResponse>?  CustomGetDisplayMessagesResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                  CustomStatusInfoSerializer                   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?              CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomGetDisplayMessagesResponseSerializer is not null
                       ? CustomGetDisplayMessagesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get display messages request failed.
        /// </summary>
        /// <param name="Request">The get display messages request leading to this response.</param>
        public static GetDisplayMessagesResponse Failed(CSMS.GetDisplayMessagesRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetDisplayMessagesResponse1, GetDisplayMessagesResponse2)

        /// <summary>
        /// Compares two get display messages responses for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesResponse1">A get display messages response.</param>
        /// <param name="GetDisplayMessagesResponse2">Another get display messages response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDisplayMessagesResponse? GetDisplayMessagesResponse1,
                                           GetDisplayMessagesResponse? GetDisplayMessagesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDisplayMessagesResponse1, GetDisplayMessagesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetDisplayMessagesResponse1 is null || GetDisplayMessagesResponse2 is null)
                return false;

            return GetDisplayMessagesResponse1.Equals(GetDisplayMessagesResponse2);

        }

        #endregion

        #region Operator != (GetDisplayMessagesResponse1, GetDisplayMessagesResponse2)

        /// <summary>
        /// Compares two get display messages responses for inequality.
        /// </summary>
        /// <param name="GetDisplayMessagesResponse1">A get display messages response.</param>
        /// <param name="GetDisplayMessagesResponse2">Another get display messages response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDisplayMessagesResponse? GetDisplayMessagesResponse1,
                                           GetDisplayMessagesResponse? GetDisplayMessagesResponse2)

            => !(GetDisplayMessagesResponse1 == GetDisplayMessagesResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDisplayMessagesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get display messages responses for equality.
        /// </summary>
        /// <param name="Object">A get display messages response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDisplayMessagesResponse getDisplayMessagesResponse &&
                   Equals(getDisplayMessagesResponse);

        #endregion

        #region Equals(GetDisplayMessagesResponse)

        /// <summary>
        /// Compares two get display messages responses for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesResponse">A get display messages response to compare with.</param>
        public override Boolean Equals(GetDisplayMessagesResponse? GetDisplayMessagesResponse)

            => GetDisplayMessagesResponse is not null &&

               Status.     Equals(GetDisplayMessagesResponse.Status) &&

             ((StatusInfo is     null && GetDisplayMessagesResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetDisplayMessagesResponse.StatusInfo is not null && StatusInfo.Equals(GetDisplayMessagesResponse.StatusInfo)) &&

               base.GenericEquals(GetDisplayMessagesResponse);

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
