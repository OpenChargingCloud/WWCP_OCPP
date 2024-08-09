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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The ClearCache response.
    /// </summary>
    public class ClearCacheResponse : AResponse<CSMS.ClearCacheRequest,
                                                ClearCacheResponse>,
                                      IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/clearCacheResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ClearCache command.
        /// </summary>
        [Mandatory]
        public ClearCacheStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?       StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region ClearCacheResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="Status">The success or failure of the ClearCache command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ClearCacheResponse(CSMS.ClearCacheRequest        Request,
                                  ClearCacheStatus              Status,
                                  StatusInfo?                   StatusInfo          = null,
                                  DateTime?                     ResponseTimestamp   = null,

                                  NetworkingNode_Id?            DestinationId       = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<Signature>?       Signatures          = null,

                                  CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region ClearCacheResponse(Request, Result)

        /// <summary>
        /// Create a new ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearCacheResponse(CSMS.ClearCacheRequest   Request,
                                  Result                   Result,
                                  DateTime?                ResponseTimestamp   = null,

                                  NetworkingNode_Id?       DestinationId       = null,
                                  NetworkPath?             NetworkPath         = null,

                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                  IEnumerable<Signature>?  Signatures          = null,

                                  CustomData?              CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearCacheResponse",
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
        //     "ClearCacheStatusEnumType": {
        //       "description": "Accepted if the Charging Station has executed the request, otherwise rejected.",
        //       "javaType": "ClearCacheStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.",
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
        //       "$ref": "#/definitions/ClearCacheStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomClearCacheResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearCacheResponseParser">A delegate to parse custom ClearCache responses.</param>
        public static ClearCacheResponse Parse(CSMS.ClearCacheRequest                            Request,
                                               JObject                                           JSON,
                                               NetworkingNode_Id                                 DestinationId,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<ClearCacheResponse>?  CustomClearCacheResponseParser   = null,
                                               CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var clearCacheResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomClearCacheResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearCacheResponse;
            }

            throw new ArgumentException("The given JSON representation of a ClearCache response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearCacheResponse, out ErrorResponse, CustomClearCacheResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearCacheResponse">The parsed ClearCache response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearCacheResponseParser">A delegate to parse custom ClearCache responses.</param>
        public static Boolean TryParse(CSMS.ClearCacheRequest                            Request,
                                       JObject                                           JSON,
                                       NetworkingNode_Id                                 DestinationId,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out ClearCacheResponse?      ClearCacheResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<ClearCacheResponse>?  CustomClearCacheResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                ClearCacheResponse = null;

                #region ClearCacheStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "ClearCache status",
                                         ClearCacheStatusExtensions.TryParse,
                                         out ClearCacheStatus ClearCacheStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo          [optional]

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

                #region Signatures          [optional, OCPP_CSE]

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

                #region CustomData          [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ClearCacheResponse = new ClearCacheResponse(

                                         Request,
                                         ClearCacheStatus,
                                         StatusInfo,
                                         ResponseTimestamp,

                                         DestinationId,
                                         NetworkPath,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData

                                     );

                if (CustomClearCacheResponseParser is not null)
                    ClearCacheResponse = CustomClearCacheResponseParser(JSON,
                                                                        ClearCacheResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearCacheResponse  = null;
                ErrorResponse       = "The given JSON representation of a ClearCache response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearCacheResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearCacheResponseSerializer">A delegate to serialize custom ClearCache responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearCacheResponse>?  CustomClearCacheResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?          CustomStatusInfoSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
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

            return CustomClearCacheResponseSerializer is not null
                       ? CustomClearCacheResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ClearCache failed because of a request error.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        public static ClearCacheResponse RequestError(CSMS.ClearCacheRequest   Request,
                                                      EventTracking_Id         EventTrackingId,
                                                      ResultCode               ErrorCode,
                                                      String?                  ErrorDescription    = null,
                                                      JObject?                 ErrorDetails        = null,
                                                      DateTime?                ResponseTimestamp   = null,

                                                      NetworkingNode_Id?       DestinationId       = null,
                                                      NetworkPath?             NetworkPath         = null,

                                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                                      IEnumerable<Signature>?  Signatures          = null,

                                                      CustomData?              CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The ClearCache failed.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearCacheResponse FormationViolation(CSMS.ClearCacheRequest  Request,
                                                            String                  ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The ClearCache failed.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearCacheResponse SignatureError(CSMS.ClearCacheRequest  Request,
                                                        String                  ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The ClearCache failed.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ClearCacheResponse Failed(CSMS.ClearCacheRequest  Request,
                                                String?                 Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The ClearCache failed because of an exception.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="Exception">The exception.</param>
        public static ClearCacheResponse ExceptionOccured(CSMS.ClearCacheRequest  Request,
                                                          Exception               Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two ClearCache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A ClearCache response.</param>
        /// <param name="ClearCacheResponse2">Another ClearCache response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearCacheResponse? ClearCacheResponse1,
                                           ClearCacheResponse? ClearCacheResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearCacheResponse1, ClearCacheResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearCacheResponse1 is null || ClearCacheResponse2 is null)
                return false;

            return ClearCacheResponse1.Equals(ClearCacheResponse2);

        }

        #endregion

        #region Operator != (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two ClearCache responses for inequality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A ClearCache response.</param>
        /// <param name="ClearCacheResponse2">Another ClearCache response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearCacheResponse? ClearCacheResponse1,
                                           ClearCacheResponse? ClearCacheResponse2)

            => !(ClearCacheResponse1 == ClearCacheResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearCacheResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearCache responses for equality.
        /// </summary>
        /// <param name="Object">A ClearCache response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearCacheResponse clearCacheResponse &&
                   Equals(clearCacheResponse);

        #endregion

        #region Equals(ClearCacheResponse)

        /// <summary>
        /// Compares two ClearCache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse">A ClearCache response to compare with.</param>
        public override Boolean Equals(ClearCacheResponse? ClearCacheResponse)

            => ClearCacheResponse is not null &&

               Status.     Equals(ClearCacheResponse.Status) &&

             ((StatusInfo is     null && ClearCacheResponse.StatusInfo is     null) ||
               StatusInfo is not null && ClearCacheResponse.StatusInfo is not null && StatusInfo.Equals(ClearCacheResponse.StatusInfo)) &&

               base.GenericEquals(ClearCacheResponse);

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
