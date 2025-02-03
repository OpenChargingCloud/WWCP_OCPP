﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// An open periodic event stream response.
    /// </summary>
    public class OpenPeriodicEventStreamResponse : AResponse<OpenPeriodicEventStreamRequest,
                                                             OpenPeriodicEventStreamResponse>,
                                                   IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/openPeriodicEventStreamResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The response status.
        /// </summary>
        [Mandatory]
        public GenericStatus   Status        { get; }

        /// <summary>
        /// An optional element providing more information about the response status.
        /// </summary>
        [Optional]
        public StatusInfo?     StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region OpenPeriodicEventStreamResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="Status">The response status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the response status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public OpenPeriodicEventStreamResponse(OpenPeriodicEventStreamRequest  Request,
                                               GenericStatus                   Status,
                                               StatusInfo?                     StatusInfo          = null,
                                               DateTime?                       ResponseTimestamp   = null,

                                               IEnumerable<KeyPair>?           SignKeys            = null,
                                               IEnumerable<SignInfo>?          SignInfos           = null,
                                               IEnumerable<Signature>?         Signatures          = null,

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

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion

        #region OpenPeriodicEventStreamResponse(Request, Result)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public OpenPeriodicEventStreamResponse(OpenPeriodicEventStreamRequest  Request,
                                               Result                          Result)

            : base(Request,
                   Result)

        {

            this.Status = GenericStatus.Rejected;

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:OpenPeriodicEventStreamResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "GenericStatusEnumType": {
        //             "description": "Result of request.",
        //             "javaType": "GenericStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "status": {
        //             "$ref": "#/definitions/GenericStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomOpenPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an open periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomOpenPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static OpenPeriodicEventStreamResponse Parse(CS.OpenPeriodicEventStreamRequest                              Request,
                                                            JObject                                                        JSON,
                                                            CustomJObjectParserDelegate<OpenPeriodicEventStreamResponse>?  CustomOpenPeriodicEventStreamResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var openPeriodicEventStreamResponse,
                         out var errorResponse,
                         CustomOpenPeriodicEventStreamResponseParser) &&
                openPeriodicEventStreamResponse is not null)
            {
                return openPeriodicEventStreamResponse;
            }

            throw new ArgumentException("The given JSON representation of an open periodic event stream response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out OpenPeriodicEventStreamResponse, out ErrorResponse, CustomOpenPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an open periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OpenPeriodicEventStreamResponse">The parsed open periodic event stream response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOpenPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static Boolean TryParse(OpenPeriodicEventStreamRequest                                 Request,
                                       JObject                                                        JSON,
                                       out OpenPeriodicEventStreamResponse?                           OpenPeriodicEventStreamResponse,
                                       out String?                                                    ErrorResponse,
                                       CustomJObjectParserDelegate<OpenPeriodicEventStreamResponse>?  CustomOpenPeriodicEventStreamResponseParser   = null)
        {

            try
            {

                OpenPeriodicEventStreamResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "response status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (Status == GenericStatus.Unknown)
                {
                    ErrorResponse = "Unknown response status '" + (JSON.GetString("status") ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                OpenPeriodicEventStreamResponse = new OpenPeriodicEventStreamResponse(
                                                      Request,
                                                      Status,
                                                      StatusInfo,
                                                      null,
                                                      null,
                                                      null,
                                                      Signatures,
                                                      CustomData
                                                  );

                if (CustomOpenPeriodicEventStreamResponseParser is not null)
                    OpenPeriodicEventStreamResponse = CustomOpenPeriodicEventStreamResponseParser(JSON,
                                                                                                  OpenPeriodicEventStreamResponse);

                return true;

            }
            catch (Exception e)
            {
                OpenPeriodicEventStreamResponse  = null;
                ErrorResponse                    = "The given JSON representation of an open periodic event stream response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomOpenPeriodicEventStreamResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOpenPeriodicEventStreamResponseSerializer">A delegate to serialize custom open periodic event stream responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                            IncludeJSONLDContext                              = false,
                              CustomJObjectSerializerDelegate<OpenPeriodicEventStreamResponse>?  CustomOpenPeriodicEventStreamResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                       CustomStatusInfoSerializer                        = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomOpenPeriodicEventStreamResponseSerializer is not null
                       ? CustomOpenPeriodicEventStreamResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The open periodic event stream failed.
        /// </summary>
        public static OpenPeriodicEventStreamResponse Failed(OpenPeriodicEventStreamRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (OpenPeriodicEventStreamResponse1, OpenPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="OpenPeriodicEventStreamResponse1">An open periodic event stream response.</param>
        /// <param name="OpenPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (OpenPeriodicEventStreamResponse? OpenPeriodicEventStreamResponse1,
                                           OpenPeriodicEventStreamResponse? OpenPeriodicEventStreamResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OpenPeriodicEventStreamResponse1, OpenPeriodicEventStreamResponse2))
                return true;

            // If one is null, but not both, return false.
            if (OpenPeriodicEventStreamResponse1 is null || OpenPeriodicEventStreamResponse2 is null)
                return false;

            return OpenPeriodicEventStreamResponse1.Equals(OpenPeriodicEventStreamResponse2);

        }

        #endregion

        #region Operator != (OpenPeriodicEventStreamResponse1, OpenPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for inequality.
        /// </summary>
        /// <param name="OpenPeriodicEventStreamResponse1">An open periodic event stream response.</param>
        /// <param name="OpenPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (OpenPeriodicEventStreamResponse? OpenPeriodicEventStreamResponse1,
                                           OpenPeriodicEventStreamResponse? OpenPeriodicEventStreamResponse2)

            => !(OpenPeriodicEventStreamResponse1 == OpenPeriodicEventStreamResponse2);

        #endregion

        #endregion

        #region IEquatable<OpenPeriodicEventStreamResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="Object">An open periodic event stream response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OpenPeriodicEventStreamResponse openPeriodicEventStreamResponse &&
                   Equals(openPeriodicEventStreamResponse);

        #endregion

        #region Equals(OpenPeriodicEventStreamResponse)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="OpenPeriodicEventStreamResponse">An open periodic event stream response to compare with.</param>
        public override Boolean Equals(OpenPeriodicEventStreamResponse? OpenPeriodicEventStreamResponse)

            => OpenPeriodicEventStreamResponse is not null &&

               Status.Equals(OpenPeriodicEventStreamResponse.Status) &&

             ((StatusInfo is     null && OpenPeriodicEventStreamResponse.StatusInfo is     null) ||
              (StatusInfo is not null && OpenPeriodicEventStreamResponse.StatusInfo is not null && StatusInfo.Equals(OpenPeriodicEventStreamResponse.StatusInfo))) &&

               base.GenericEquals(OpenPeriodicEventStreamResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

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
