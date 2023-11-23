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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A cleared charging limit response.
    /// </summary>
    public class ClearedChargingLimitResponse : AResponse<CS.ClearedChargingLimitRequest,
                                                          ClearedChargingLimitResponse>,
                                                IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/clearedChargingLimitResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region ClearedChargingLimitResponse(Request, ...)

        /// <summary>
        /// Create a new cleared charging limit response.
        /// </summary>
        /// <param name="Request">The cleared charging limit request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ClearedChargingLimitResponse(CS.ClearedChargingLimitRequest  Request,
                                            DateTime?                       ResponseTimestamp   = null,

                                            IEnumerable<KeyPair>?           SignKeys            = null,
                                            IEnumerable<SignInfo>?          SignInfos           = null,
                                            IEnumerable<Signature>?         Signatures          = null,

                                            CustomData?                     CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region ClearedChargingLimitResponse(Request, Result)

        /// <summary>
        /// Create a new cleared charging limit response.
        /// </summary>
        /// <param name="Request">The cleared charging limit request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearedChargingLimitResponse(CS.ClearedChargingLimitRequest  Request,
                                          Result                        Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearedChargingLimitResponse",
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

        #region (static) Parse   (Request, JSON, CustomClearedChargingLimitResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cleared charging limit response.
        /// </summary>
        /// <param name="Request">The cleared charging limit request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearedChargingLimitResponseParser">A delegate to parse custom cleared charging limit responses.</param>
        public static ClearedChargingLimitResponse Parse(CS.ClearedChargingLimitRequest                              Request,
                                                         JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<ClearedChargingLimitResponse>?  CustomClearedChargingLimitResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var clearedChargingLimitResponse,
                         out var errorResponse,
                         CustomClearedChargingLimitResponseParser) &&
                clearedChargingLimitResponse is not null)
            {
                return clearedChargingLimitResponse;
            }

            throw new ArgumentException("The given JSON representation of a cleared charging limit response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearedChargingLimitResponse, out ErrorResponse, CustomClearedChargingLimitResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cleared charging limit response.
        /// </summary>
        /// <param name="Request">The cleared charging limit request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearedChargingLimitResponse">The parsed cleared charging limit response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearedChargingLimitResponseParser">A delegate to parse custom cleared charging limit responses.</param>
        public static Boolean TryParse(CS.ClearedChargingLimitRequest                              Request,
                                       JObject                                                     JSON,
                                       out ClearedChargingLimitResponse?                           ClearedChargingLimitResponse,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<ClearedChargingLimitResponse>?  CustomClearedChargingLimitResponseParser   = null)
        {

            try
            {

                ClearedChargingLimitResponse = null;

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


                ClearedChargingLimitResponse = new ClearedChargingLimitResponse(
                                                   Request,
                                                   null,
                                                   null,
                                                   null,
                                                   Signatures,
                                                   CustomData
                                               );

                if (CustomClearedChargingLimitResponseParser is not null)
                    ClearedChargingLimitResponse = CustomClearedChargingLimitResponseParser(JSON,
                                                                                            ClearedChargingLimitResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearedChargingLimitResponse  = null;
                ErrorResponse                 = "The given JSON representation of a cleared charging limit response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearedChargingLimitResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearedChargingLimitResponseSerializer">A delegate to serialize custom cleared charging limit responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearedChargingLimitResponse>?  CustomClearedChargingLimitResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearedChargingLimitResponseSerializer is not null
                       ? CustomClearedChargingLimitResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The cleared charging limit failed.
        /// </summary>
        public static ClearedChargingLimitResponse Failed(CS.ClearedChargingLimitRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClearedChargingLimitResponse1, ClearedChargingLimitResponse2)

        /// <summary>
        /// Compares two cleared charging limit responses for equality.
        /// </summary>
        /// <param name="ClearedChargingLimitResponse1">A cleared charging limit response.</param>
        /// <param name="ClearedChargingLimitResponse2">Another cleared charging limit response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearedChargingLimitResponse? ClearedChargingLimitResponse1,
                                           ClearedChargingLimitResponse? ClearedChargingLimitResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearedChargingLimitResponse1, ClearedChargingLimitResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearedChargingLimitResponse1 is null || ClearedChargingLimitResponse2 is null)
                return false;

            return ClearedChargingLimitResponse1.Equals(ClearedChargingLimitResponse2);

        }

        #endregion

        #region Operator != (ClearedChargingLimitResponse1, ClearedChargingLimitResponse2)

        /// <summary>
        /// Compares two cleared charging limit responses for inequality.
        /// </summary>
        /// <param name="ClearedChargingLimitResponse1">A cleared charging limit response.</param>
        /// <param name="ClearedChargingLimitResponse2">Another cleared charging limit response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearedChargingLimitResponse? ClearedChargingLimitResponse1,
                                           ClearedChargingLimitResponse? ClearedChargingLimitResponse2)

            => !(ClearedChargingLimitResponse1 == ClearedChargingLimitResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearedChargingLimitResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cleared charging limit responses for equality.
        /// </summary>
        /// <param name="Object">A cleared charging limit response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearedChargingLimitResponse clearedChargingLimitResponse &&
                   Equals(clearedChargingLimitResponse);

        #endregion

        #region Equals(ClearedChargingLimitResponse)

        /// <summary>
        /// Compares two cleared charging limit responses for equality.
        /// </summary>
        /// <param name="ClearedChargingLimitResponse">A cleared charging limit response to compare with.</param>
        public override Boolean Equals(ClearedChargingLimitResponse? ClearedChargingLimitResponse)

            => ClearedChargingLimitResponse is not null &&
                   base.GenericEquals(ClearedChargingLimitResponse);

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

                return base.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "ClearedChargingLimitResponse";

        #endregion

    }

}
