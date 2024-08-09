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
    /// The ClearVariableMonitoring response.
    /// </summary>
    public class ClearVariableMonitoringResponse : AResponse<CSMS.ClearVariableMonitoringRequest,
                                                             ClearVariableMonitoringResponse>,
                                                   IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/clearVariableMonitoringResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of ClearVariableMonitoring results.
        /// </summary>
        [Mandatory]
        public IEnumerable<ClearMonitoringResult>  ClearMonitoringResults    { get; }

        #endregion

        #region Constructor(s)

        #region ClearVariableMonitoringResponse(Request, ClearMonitoringResults, ...)

        /// <summary>
        /// Create a new ClearVariableMonitoring response.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request leading to this response.</param>
        /// <param name="ClearMonitoringResults">An enumeration of ClearVariableMonitoring results.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ClearVariableMonitoringResponse(CSMS.ClearVariableMonitoringRequest   Request,
                                               IEnumerable<ClearMonitoringResult>    ClearMonitoringResults,
                                               DateTime?                             ResponseTimestamp   = null,

                                               NetworkingNode_Id?                    DestinationId       = null,
                                               NetworkPath?                          NetworkPath         = null,

                                               IEnumerable<KeyPair>?                 SignKeys            = null,
                                               IEnumerable<SignInfo>?                SignInfos           = null,
                                               IEnumerable<Signature>?               Signatures          = null,

                                               CustomData?                           CustomData          = null)

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

            if (!ClearMonitoringResults.Any())
                throw new ArgumentException("The given enumeration of ClearVariableMonitoring results must not be empty!",
                                            nameof(ClearMonitoringResults));

            this.ClearMonitoringResults = ClearMonitoringResults.Distinct();

        }

        #endregion

        #region ClearVariableMonitoringResponse(Request, Result)

        /// <summary>
        /// Create a new ClearVariableMonitoring response.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearVariableMonitoringResponse(CSMS.ClearVariableMonitoringRequest  Request,
                                               Result                               Result,
                                               DateTime?                            ResponseTimestamp   = null,

                                               NetworkingNode_Id?                   DestinationId       = null,
                                               NetworkPath?                         NetworkPath         = null,

                                               IEnumerable<KeyPair>?                SignKeys            = null,
                                               IEnumerable<SignInfo>?               SignInfos           = null,
                                               IEnumerable<Signature>?              Signatures          = null,

                                               CustomData?                          CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.ClearMonitoringResults = [];

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearVariableMonitoringResponse",
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
        //     "ClearMonitoringStatusEnumType": {
        //       "description": "Result of the clear request for this monitor, identified by its Id.\r\n\r\n",
        //       "javaType": "ClearMonitoringStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "NotFound"
        //       ]
        //     },
        //     "ClearMonitoringResultType": {
        //       "javaType": "ClearMonitoringResult",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "status": {
        //           "$ref": "#/definitions/ClearMonitoringStatusEnumType"
        //         },
        //         "id": {
        //           "description": "Id of the monitor of which a clear was requested.\r\n\r\n",
        //           "type": "integer"
        //         },
        //         "statusInfo": {
        //           "$ref": "#/definitions/StatusInfoType"
        //         }
        //       },
        //       "required": [
        //         "status",
        //         "id"
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
        //     "clearMonitoringResult": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ClearMonitoringResultType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "clearMonitoringResult"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomClearVariableMonitoringResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearVariableMonitoring response.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearVariableMonitoringResponseParser">A delegate to parse custom ClearVariableMonitoring responses.</param>
        public static ClearVariableMonitoringResponse Parse(CSMS.ClearVariableMonitoringRequest                            Request,
                                                            JObject                                                        JSON,
                                                            NetworkingNode_Id                                              DestinationId,
                                                            NetworkPath                                                    NetworkPath,
                                                            DateTime?                                                      ResponseTimestamp                             = null,
                                                            CustomJObjectParserDelegate<ClearVariableMonitoringResponse>?  CustomClearVariableMonitoringResponseParser   = null,
                                                            CustomJObjectParserDelegate<ClearMonitoringResult>?            CustomClearMonitoringResultParser             = null,
                                                            CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                                            CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                                            CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var clearVariableMonitoringResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomClearVariableMonitoringResponseParser,
                         CustomClearMonitoringResultParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearVariableMonitoringResponse;
            }

            throw new ArgumentException("The given JSON representation of a ClearVariableMonitoring response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearVariableMonitoringResponse, out ErrorResponse, CustomClearVariableMonitoringResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearVariableMonitoring response.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearVariableMonitoringResponse">The parsed ClearVariableMonitoring response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearVariableMonitoringResponseParser">A delegate to parse custom ClearVariableMonitoring responses.</param>
        public static Boolean TryParse(CSMS.ClearVariableMonitoringRequest                            Request,
                                       JObject                                                        JSON,
                                       NetworkingNode_Id                                              DestinationId,
                                       NetworkPath                                                    NetworkPath,
                                       [NotNullWhen(true)]  out ClearVariableMonitoringResponse?      ClearVariableMonitoringResponse,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       DateTime?                                                      ResponseTimestamp                             = null,
                                       CustomJObjectParserDelegate<ClearVariableMonitoringResponse>?  CustomClearVariableMonitoringResponseParser   = null,
                                       CustomJObjectParserDelegate<ClearMonitoringResult>?            CustomClearMonitoringResultParser             = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                       CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                       CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            try
            {

                ClearVariableMonitoringResponse = null;

                #region ClearMonitoringResults    [mandatory]

                if (!JSON.ParseMandatoryHashSet("clearMonitoringResult",
                                                "ClearVariableMonitoring results",
                                                ClearMonitoringResult.TryParse,
                                                out HashSet<ClearMonitoringResult> ClearMonitoringResults,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

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


                ClearVariableMonitoringResponse = new ClearVariableMonitoringResponse(

                                                      Request,
                                                      ClearMonitoringResults,
                                                      ResponseTimestamp,

                                                      DestinationId,
                                                      NetworkPath,

                                                      null,
                                                      null,
                                                      Signatures,

                                                      CustomData

                                                  );

                if (CustomClearVariableMonitoringResponseParser is not null)
                    ClearVariableMonitoringResponse = CustomClearVariableMonitoringResponseParser(JSON,
                                                                                                  ClearVariableMonitoringResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearVariableMonitoringResponse  = null;
                ErrorResponse                    = "The given JSON representation of a ClearVariableMonitoring response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearVariableMonitoringResponseSerializer = null, CustomClearMonitoringResultSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearVariableMonitoringResponseSerializer">A delegate to serialize custom ClearVariableMonitoring responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearVariableMonitoringResponse>?  CustomClearVariableMonitoringResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ClearMonitoringResult>?            CustomClearMonitoringResultSerializer             = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                       CustomStatusInfoSerializer                        = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("clearMonitoringResult",   new JArray(ClearMonitoringResults.Select(result    => result.   ToJSON(CustomClearMonitoringResultSerializer,
                                                                                                                                                 CustomStatusInfoSerializer,
                                                                                                                                                 CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",              new JArray(Signatures.            Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",              CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearVariableMonitoringResponseSerializer is not null
                       ? CustomClearVariableMonitoringResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ClearVariableMonitoring failed because of a request error.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request.</param>
        public static ClearVariableMonitoringResponse RequestError(CSMS.ClearVariableMonitoringRequest  Request,
                                                                   EventTracking_Id                     EventTrackingId,
                                                                   ResultCode                           ErrorCode,
                                                                   String?                              ErrorDescription    = null,
                                                                   JObject?                             ErrorDetails        = null,
                                                                   DateTime?                            ResponseTimestamp   = null,

                                                                   NetworkingNode_Id?                   DestinationId       = null,
                                                                   NetworkPath?                         NetworkPath         = null,

                                                                   IEnumerable<KeyPair>?                SignKeys            = null,
                                                                   IEnumerable<SignInfo>?               SignInfos           = null,
                                                                   IEnumerable<Signature>?              Signatures          = null,

                                                                   CustomData?                          CustomData          = null)

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
        /// The ClearVariableMonitoring failed.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearVariableMonitoringResponse FormationViolation(CSMS.ClearVariableMonitoringRequest  Request,
                                                                         String                               ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The ClearVariableMonitoring failed.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearVariableMonitoringResponse SignatureError(CSMS.ClearVariableMonitoringRequest  Request,
                                                                     String                               ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The ClearVariableMonitoring failed.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ClearVariableMonitoringResponse Failed(CSMS.ClearVariableMonitoringRequest  Request,
                                                             String?                              Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The ClearVariableMonitoring failed because of an exception.
        /// </summary>
        /// <param name="Request">The ClearVariableMonitoring request.</param>
        /// <param name="Exception">The exception.</param>
        public static ClearVariableMonitoringResponse ExceptionOccured(CSMS.ClearVariableMonitoringRequest  Request,
                                                                       Exception                            Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ClearVariableMonitoringResponse1, ClearVariableMonitoringResponse2)

        /// <summary>
        /// Compares two ClearVariableMonitoring responses for equality.
        /// </summary>
        /// <param name="ClearVariableMonitoringResponse1">A ClearVariableMonitoring response.</param>
        /// <param name="ClearVariableMonitoringResponse2">Another ClearVariableMonitoring response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearVariableMonitoringResponse? ClearVariableMonitoringResponse1,
                                           ClearVariableMonitoringResponse? ClearVariableMonitoringResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearVariableMonitoringResponse1, ClearVariableMonitoringResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearVariableMonitoringResponse1 is null || ClearVariableMonitoringResponse2 is null)
                return false;

            return ClearVariableMonitoringResponse1.Equals(ClearVariableMonitoringResponse2);

        }

        #endregion

        #region Operator != (ClearVariableMonitoringResponse1, ClearVariableMonitoringResponse2)

        /// <summary>
        /// Compares two ClearVariableMonitoring responses for inequality.
        /// </summary>
        /// <param name="ClearVariableMonitoringResponse1">A ClearVariableMonitoring response.</param>
        /// <param name="ClearVariableMonitoringResponse2">Another ClearVariableMonitoring response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearVariableMonitoringResponse? ClearVariableMonitoringResponse1,
                                           ClearVariableMonitoringResponse? ClearVariableMonitoringResponse2)

            => !(ClearVariableMonitoringResponse1 == ClearVariableMonitoringResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearVariableMonitoringResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearVariableMonitoring responses for equality.
        /// </summary>
        /// <param name="Object">A ClearVariableMonitoring response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearVariableMonitoringResponse clearVariableMonitoringResponse &&
                   Equals(clearVariableMonitoringResponse);

        #endregion

        #region Equals(ClearVariableMonitoringResponse)

        /// <summary>
        /// Compares two ClearVariableMonitoring responses for equality.
        /// </summary>
        /// <param name="ClearVariableMonitoringResponse">A ClearVariableMonitoring response to compare with.</param>
        public override Boolean Equals(ClearVariableMonitoringResponse? ClearVariableMonitoringResponse)

            => ClearVariableMonitoringResponse is not null &&

               ClearMonitoringResults.Count().Equals(ClearVariableMonitoringResponse.ClearMonitoringResults.Count())         &&
               ClearMonitoringResults.All(result => ClearVariableMonitoringResponse.ClearMonitoringResults.Contains(result)) &&

               base.GenericEquals(ClearVariableMonitoringResponse);

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

                return ClearMonitoringResults.GetHashCode() * 3 ^
                       base.                  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ClearMonitoringResults.Select(result => result.Id + ": " + result.Status.ToString()).AggregateWith(", ");

        #endregion

    }

}
