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
    /// A clear variable monitoring response.
    /// </summary>
    public class ClearVariableMonitoringResponse : AResponse<CS.ClearVariableMonitoringRequest,
                                                             ClearVariableMonitoringResponse>
    {

        #region Properties

        /// <summary>
        /// The enumeration of clear variable monitoring results.
        /// </summary>
        [Mandatory]
        public IEnumerable<ClearMonitoringResult>  ClearMonitoringResults    { get; }

        #endregion

        #region Constructor(s)

        #region ClearVariableMonitoringResponse(Request, Status, ...)

        /// <summary>
        /// Create a new clear variable monitoring response.
        /// </summary>
        /// <param name="Request">The clear variable monitoring request leading to this response.</param>
        /// <param name="ClearMonitoringResults">An enumeration of clear variable monitoring results.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public ClearVariableMonitoringResponse(CS.ClearVariableMonitoringRequest   Request,
                                               IEnumerable<ClearMonitoringResult>  ClearMonitoringResults,
                                               CustomData?                         CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            if (!ClearMonitoringResults.Any())
                throw new ArgumentException("The given enumeration of clear variable monitoring results must not be empty!",
                                            nameof(ClearMonitoringResults));

            this.ClearMonitoringResults = ClearMonitoringResults.Distinct();

        }

        #endregion

        #region ClearVariableMonitoringResponse(Request, Result)

        /// <summary>
        /// Create a new clear variable monitoring response.
        /// </summary>
        /// <param name="Request">The clear variable monitoring request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearVariableMonitoringResponse(CS.ClearVariableMonitoringRequest  Request,
                                               Result                             Result)

            : base(Request,
                   Result)

        {

            this.ClearMonitoringResults = Array.Empty<ClearMonitoringResult>();

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
        /// Parse the given JSON representation of a clear variable monitoring response.
        /// </summary>
        /// <param name="Request">The clear variable monitoring request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearVariableMonitoringResponseParser">A delegate to parse custom clear variable monitoring responses.</param>
        public static ClearVariableMonitoringResponse Parse(CS.ClearVariableMonitoringRequest                              Request,
                                                            JObject                                                        JSON,
                                                            CustomJObjectParserDelegate<ClearVariableMonitoringResponse>?  CustomClearVariableMonitoringResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var clearVariableMonitoringResponse,
                         out var errorResponse,
                         CustomClearVariableMonitoringResponseParser))
            {
                return clearVariableMonitoringResponse!;
            }

            throw new ArgumentException("The given JSON representation of a clear variable monitoring response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearVariableMonitoringResponse, out ErrorResponse, CustomClearVariableMonitoringResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear variable monitoring response.
        /// </summary>
        /// <param name="Request">The clear variable monitoring request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearVariableMonitoringResponse">The parsed clear variable monitoring response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearVariableMonitoringResponseParser">A delegate to parse custom clear variable monitoring responses.</param>
        public static Boolean TryParse(CS.ClearVariableMonitoringRequest                              Request,
                                       JObject                                                        JSON,
                                       out ClearVariableMonitoringResponse?                           ClearVariableMonitoringResponse,
                                       out String?                                                    ErrorResponse,
                                       CustomJObjectParserDelegate<ClearVariableMonitoringResponse>?  CustomClearVariableMonitoringResponseParser   = null)
        {

            try
            {

                ClearVariableMonitoringResponse = null;

                #region ClearMonitoringResults    [mandatory]

                if (!JSON.ParseMandatoryHashSet("clearMonitoringResult",
                                                "clear variable monitoring results",
                                                ClearMonitoringResult.TryParse,
                                                out HashSet<ClearMonitoringResult> ClearMonitoringResults,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData                [optional]

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


                ClearVariableMonitoringResponse = new ClearVariableMonitoringResponse(Request,
                                                                                      ClearMonitoringResults,
                                                                                      CustomData);

                if (CustomClearVariableMonitoringResponseParser is not null)
                    ClearVariableMonitoringResponse = CustomClearVariableMonitoringResponseParser(JSON,
                                                                                                  ClearVariableMonitoringResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearVariableMonitoringResponse  = null;
                ErrorResponse                    = "The given JSON representation of a clear variable monitoring response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearVariableMonitoringResponseSerializer = null, CustomClearMonitoringResultResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearVariableMonitoringResponseSerializer">A delegate to serialize custom clear variable monitoring responses.</param>
        /// <param name="CustomStatusInfoResponseSerializer">A delegate to serialize a custom status info objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearVariableMonitoringResponse>?  CustomClearVariableMonitoringResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ClearMonitoringResult>?            CustomClearMonitoringResultResponseSerializer     = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                       CustomStatusInfoResponseSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataResponseSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("clearMonitoringResult",  new JArray(ClearMonitoringResults.Select(result => result.ToJSON(CustomClearMonitoringResultResponseSerializer,
                                                                                                                                          CustomStatusInfoResponseSerializer,
                                                                                                                                          CustomCustomDataResponseSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",             CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomClearVariableMonitoringResponseSerializer is not null
                       ? CustomClearVariableMonitoringResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The clear variable monitoring command failed.
        /// </summary>
        /// <param name="Request">The clear variable monitoring request leading to this response.</param>
        public static ClearVariableMonitoringResponse Failed(CS.ClearVariableMonitoringRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClearVariableMonitoringResponse1, ClearVariableMonitoringResponse2)

        /// <summary>
        /// Compares two clear variable monitoring responses for equality.
        /// </summary>
        /// <param name="ClearVariableMonitoringResponse1">A clear variable monitoring response.</param>
        /// <param name="ClearVariableMonitoringResponse2">Another clear variable monitoring response.</param>
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
        /// Compares two clear variable monitoring responses for inequality.
        /// </summary>
        /// <param name="ClearVariableMonitoringResponse1">A clear variable monitoring response.</param>
        /// <param name="ClearVariableMonitoringResponse2">Another clear variable monitoring response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearVariableMonitoringResponse? ClearVariableMonitoringResponse1,
                                           ClearVariableMonitoringResponse? ClearVariableMonitoringResponse2)

            => !(ClearVariableMonitoringResponse1 == ClearVariableMonitoringResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearVariableMonitoringResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear variable monitoring responses for equality.
        /// </summary>
        /// <param name="Object">A clear variable monitoring response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearVariableMonitoringResponse clearVariableMonitoringResponse &&
                   Equals(clearVariableMonitoringResponse);

        #endregion

        #region Equals(ClearVariableMonitoringResponse)

        /// <summary>
        /// Compares two clear variable monitoring responses for equality.
        /// </summary>
        /// <param name="ClearVariableMonitoringResponse">A clear variable monitoring response to compare with.</param>
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
