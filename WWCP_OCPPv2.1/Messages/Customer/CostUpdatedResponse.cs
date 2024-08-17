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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The CostUpdated response.
    /// </summary>
    public class CostUpdatedResponse : AResponse<CostUpdatedRequest,
                                                 CostUpdatedResponse>,
                                       IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/costUpdatedResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CostUpdated response.
        /// </summary>
        /// <param name="Request">The CostUpdated request leading to this response.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="SourceRouting">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public CostUpdatedResponse(CostUpdatedRequest       Request,

                                   Result?                  Result                = null,
                                   DateTime?                ResponseTimestamp     = null,

                                   SourceRouting?           SourceRouting         = null,
                                   NetworkPath?             NetworkPath           = null,

                                   IEnumerable<KeyPair>?    SignKeys              = null,
                                   IEnumerable<SignInfo>?   SignInfos             = null,
                                   IEnumerable<Signature>?  Signatures            = null,

                                   CustomData?              CustomData            = null,

                                   SerializationFormats?    SerializationFormat   = null,
                                   CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            unchecked
            {
                hashCode = base.GetHashCode();
            }

        }

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
        /// Parse the given JSON representation of a CostUpdated response.
        /// </summary>
        /// <param name="Request">The CostUpdated request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCostUpdatedResponseParser">A delegate to parse custom CostUpdated responses.</param>
        public static CostUpdatedResponse Parse(CostUpdatedRequest                                 Request,
                                                JObject                                            JSON,
                                                SourceRouting                                      SourceRouting,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          ResponseTimestamp                 = null,
                                                CustomJObjectParserDelegate<CostUpdatedResponse>?  CustomCostUpdatedResponseParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            if (TryParse(Request,
                         JSON,
                             SourceRouting,
                         NetworkPath,
                         out var costUpdatedResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomCostUpdatedResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return costUpdatedResponse;
            }

            throw new ArgumentException("The given JSON representation of a CostUpdated response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CostUpdatedResponse, out ErrorResponse, CustomCostUpdatedResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a CostUpdated response.
        /// </summary>
        /// <param name="Request">The CostUpdated request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CostUpdatedResponse">The parsed CostUpdated response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostUpdatedResponseParser">A delegate to parse custom CostUpdated responses.</param>
        public static Boolean TryParse(CostUpdatedRequest                                 Request,
                                       JObject                                            JSON,
                                       SourceRouting                                      SourceRouting,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out CostUpdatedResponse?      CostUpdatedResponse,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          ResponseTimestamp                 = null,
                                       CustomJObjectParserDelegate<CostUpdatedResponse>?  CustomCostUpdatedResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                CostUpdatedResponse = null;

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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                CostUpdatedResponse = new CostUpdatedResponse(

                                          Request,

                                          null,
                                          ResponseTimestamp,

                                              SourceRouting,
                                          NetworkPath,

                                          null,
                                          null,
                                          Signatures,

                                          CustomData

                                      );

                if (CustomCostUpdatedResponseParser is not null)
                    CostUpdatedResponse = CustomCostUpdatedResponseParser(JSON,
                                                                          CostUpdatedResponse);

                return true;

            }
            catch (Exception e)
            {
                CostUpdatedResponse  = null;
                ErrorResponse        = "The given JSON representation of a CostUpdated response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCostUpdatedResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCostUpdatedResponseSerializer">A delegate to serialize custom CostUpdated responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CostUpdatedResponse>?  CustomCostUpdatedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
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

            return CustomCostUpdatedResponseSerializer is not null
                       ? CustomCostUpdatedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The CostUpdated failed because of a request error.
        /// </summary>
        /// <param name="Request">The CostUpdated request.</param>
        public static CostUpdatedResponse RequestError(CostUpdatedRequest       Request,
                                                       EventTracking_Id         EventTrackingId,
                                                       ResultCode               ErrorCode,
                                                       String?                  ErrorDescription    = null,
                                                       JObject?                 ErrorDetails        = null,
                                                       DateTime?                ResponseTimestamp   = null,

                                                       SourceRouting?       SourceRouting       = null,
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

                       SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The CostUpdated failed.
        /// </summary>
        /// <param name="Request">The CostUpdated request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CostUpdatedResponse FormationViolation(CostUpdatedRequest  Request,
                                                             String              ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The CostUpdated failed.
        /// </summary>
        /// <param name="Request">The CostUpdated request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CostUpdatedResponse SignatureError(CostUpdatedRequest  Request,
                                                         String              ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The CostUpdated failed.
        /// </summary>
        /// <param name="Request">The CostUpdated request.</param>
        /// <param name="Description">An optional error description.</param>
        public static CostUpdatedResponse Failed(CostUpdatedRequest  Request,
                                                 String?             Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The CostUpdated failed because of an exception.
        /// </summary>
        /// <param name="Request">The CostUpdated request.</param>
        /// <param name="Exception">The exception.</param>
        public static CostUpdatedResponse ExceptionOccured(CostUpdatedRequest  Request,
                                                           Exception           Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (CostUpdatedResponse1, CostUpdatedResponse2)

        /// <summary>
        /// Compares two CostUpdated responses for equality.
        /// </summary>
        /// <param name="CostUpdatedResponse1">A CostUpdated response.</param>
        /// <param name="CostUpdatedResponse2">Another CostUpdated response.</param>
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
        /// Compares two CostUpdated responses for inequality.
        /// </summary>
        /// <param name="CostUpdatedResponse1">A CostUpdated response.</param>
        /// <param name="CostUpdatedResponse2">Another CostUpdated response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CostUpdatedResponse? CostUpdatedResponse1,
                                           CostUpdatedResponse? CostUpdatedResponse2)

            => !(CostUpdatedResponse1 == CostUpdatedResponse2);

        #endregion

        #endregion

        #region IEquatable<CostUpdatedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CostUpdated responses for equality.
        /// </summary>
        /// <param name="Object">A CostUpdated response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CostUpdatedResponse costUpdatedResponse &&
                   Equals(costUpdatedResponse);

        #endregion

        #region Equals(CostUpdatedResponse)

        /// <summary>
        /// Compares two CostUpdated responses for equality.
        /// </summary>
        /// <param name="CostUpdatedResponse">A CostUpdated response to compare with.</param>
        public override Boolean Equals(CostUpdatedResponse? CostUpdatedResponse)

            => CostUpdatedResponse is not null &&

               base.GenericEquals(CostUpdatedResponse);

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

            => "CostUpdatedResponse";

        #endregion

    }

}
