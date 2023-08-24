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
    /// A cost updated response.
    /// </summary>
    public class CostUpdatedResponse : AResponse<CSMS.CostUpdatedRequest,
                                                 CostUpdatedResponse>
    {

        #region Constructor(s)

        #region CostUpdatedResponse(Request, CustomData = null)

        /// <summary>
        /// Create a new cost updated response.
        /// </summary>
        /// <param name="Request">The cost updated request leading to this response.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public CostUpdatedResponse(CSMS.CostUpdatedRequest  Request,
                                   CustomData?              CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        { }

        #endregion

        #region CostUpdatedResponse(Request, Result)

        /// <summary>
        /// Create a new cost updated response.
        /// </summary>
        /// <param name="Request">The cost updated request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public CostUpdatedResponse(CSMS.CostUpdatedRequest  Request,
                                   Result                   Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:CostUpdatedResponse",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomCostUpdatedResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cost updated response.
        /// </summary>
        /// <param name="Request">The cost updated request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCostUpdatedResponseParser">A delegate to parse custom cost updated responses.</param>
        public static CostUpdatedResponse Parse(CSMS.CostUpdatedRequest                            Request,
                                                JObject                                            JSON,
                                                CustomJObjectParserDelegate<CostUpdatedResponse>?  CustomCostUpdatedResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var costUpdatedResponse,
                         out var errorResponse,
                         CustomCostUpdatedResponseParser))
            {
                return costUpdatedResponse!;
            }

            throw new ArgumentException("The given JSON representation of a cost updated response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CostUpdatedResponse, out ErrorResponse, CustomCostUpdatedResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cost updated response.
        /// </summary>
        /// <param name="Request">The cost updated request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CostUpdatedResponse">The parsed cost updated response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostUpdatedResponseParser">A delegate to parse custom cost updated responses.</param>
        public static Boolean TryParse(CSMS.CostUpdatedRequest                            Request,
                                       JObject                                            JSON,
                                       out CostUpdatedResponse?                           CostUpdatedResponse,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<CostUpdatedResponse>?  CustomCostUpdatedResponseParser   = null)
        {

            try
            {

                CostUpdatedResponse = null;

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


                CostUpdatedResponse = new CostUpdatedResponse(Request,
                                                              CustomData);

                if (CustomCostUpdatedResponseParser is not null)
                    CostUpdatedResponse = CustomCostUpdatedResponseParser(JSON,
                                                                          CostUpdatedResponse);

                return true;

            }
            catch (Exception e)
            {
                CostUpdatedResponse  = null;
                ErrorResponse        = "The given JSON representation of a cost updated response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCostUpdatedResponseSerializer = null, CustomCustomDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCostUpdatedResponseSerializer">A delegate to serialize custom cost updated responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CostUpdatedResponse>?  CustomCostUpdatedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCostUpdatedResponseSerializer is not null
                       ? CustomCostUpdatedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The cost updated command failed.
        /// </summary>
        /// <param name="Request">The cost updated request leading to this response.</param>
        public static CostUpdatedResponse Failed(CSMS.CostUpdatedRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (CostUpdatedResponse1, CostUpdatedResponse2)

        /// <summary>
        /// Compares two cost updated responses for equality.
        /// </summary>
        /// <param name="CostUpdatedResponse1">A cost updated response.</param>
        /// <param name="CostUpdatedResponse2">Another cost updated response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CostUpdatedResponse? CostUpdatedResponse1,
                                           CostUpdatedResponse? CostUpdatedResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CostUpdatedResponse1, CostUpdatedResponse2))
                return true;

            // If one is null, but not both, return false.
            if (CostUpdatedResponse1 is null || CostUpdatedResponse2 is null)
                return false;

            return CostUpdatedResponse1.Equals(CostUpdatedResponse2);

        }

        #endregion

        #region Operator != (CostUpdatedResponse1, CostUpdatedResponse2)

        /// <summary>
        /// Compares two cost updated responses for inequality.
        /// </summary>
        /// <param name="CostUpdatedResponse1">A cost updated response.</param>
        /// <param name="CostUpdatedResponse2">Another cost updated response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CostUpdatedResponse? CostUpdatedResponse1,
                                           CostUpdatedResponse? CostUpdatedResponse2)

            => !(CostUpdatedResponse1 == CostUpdatedResponse2);

        #endregion

        #endregion

        #region IEquatable<CostUpdatedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cost updated responses for equality.
        /// </summary>
        /// <param name="Object">A cost updated response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CostUpdatedResponse costUpdatedResponse &&
                   Equals(costUpdatedResponse);

        #endregion

        #region Equals(CostUpdatedResponse)

        /// <summary>
        /// Compares two cost updated responses for equality.
        /// </summary>
        /// <param name="CostUpdatedResponse">A cost updated response to compare with.</param>
        public override Boolean Equals(CostUpdatedResponse? CostUpdatedResponse)

            => CostUpdatedResponse is not null &&

               base.GenericEquals(CostUpdatedResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "CostUpdatedResponse";

        #endregion

    }

}
