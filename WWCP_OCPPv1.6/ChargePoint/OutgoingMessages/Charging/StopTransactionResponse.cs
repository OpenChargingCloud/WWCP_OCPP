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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A stop transaction response.
    /// </summary>
    public class StopTransactionResponse : AResponse<CP.StopTransactionRequest,
                                                        StopTransactionResponse>,
                                           IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/stopTransactionResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// It is optional, because a transaction may have been stopped
        /// without an identifier.
        /// </summary>
        public IdTagInfo?     IdTagInfo    { get; }

        #endregion

        #region Constructor(s)

        #region StopTransactionResponse(Request, IdTagInfo = null, ...)

        /// <summary>
        /// Create a new stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public StopTransactionResponse(CP.StopTransactionRequest     Request,
                                       IdTagInfo?                    IdTagInfo           = null,

                                       DateTime?                     ResponseTimestamp   = null,

                                       IEnumerable<KeyPair>?         SignKeys            = null,
                                       IEnumerable<SignInfo>?        SignInfos           = null,
                                       IEnumerable<OCPP.Signature>?  Signatures          = null,

                                       CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.IdTagInfo  = IdTagInfo;

        }

        #endregion

        #region StopTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public StopTransactionResponse(CP.StopTransactionRequest  Request,
                                       Result                     Result)

            : base(Request,
                   Result)

        {

            this.IdTagInfo = new IdTagInfo(AuthorizationStatus.Unknown);

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:stopTransactionResponse>
        //
        //          <!--Optional:-->
        //          <ns:idTagInfo>
        //
        //             <ns:status>?</ns:status>
        //
        //             <!--Optional:-->
        //             <ns:expiryDate>?</ns:expiryDate>
        //
        //             <!--Optional:-->
        //             <ns:parentIdTag>?</ns:parentIdTag>
        //
        //          </ns:idTagInfo>
        //
        //       </ns:stopTransactionResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:StopTransactionResponse",
        //     "title":    "StopTransactionResponse",
        //     "type":     "object",
        //     "properties": {
        //         "idTagInfo": {
        //             "type": "object",
        //             "properties": {
        //                 "expiryDate": {
        //                     "type":      "string",
        //                     "format":    "date-time"
        //                 },
        //                 "parentIdTag": {
        //                     "type":      "string",
        //                     "maxLength":  20
        //                 },
        //                 "status": {
        //                     "type":      "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Accepted",
        //                         "Blocked",
        //                         "Expired",
        //                         "Invalid",
        //                         "ConcurrentTx"
        //                     ]
        //                 }
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "status"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static StopTransactionResponse Parse(CP.StopTransactionRequest  Request,
                                                    XElement                   XML)
        {

            if (TryParse(Request,
                         XML,
                         out var stopTransactionResponse,
                         out var errorResponse) &&
                stopTransactionResponse is not null)
            {
                return stopTransactionResponse;
            }

            throw new ArgumentException("The given XML representation of a stop transaction response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomStopTransactionResponseParser = null)

        /// <summary>
        /// Parse the given text representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="CustomStopTransactionResponseParser">A delegate to parse custom stop transaction responses.</param>
        public static StopTransactionResponse Parse(CP.StopTransactionRequest                              Request,
                                                    JObject                                                JSON,
                                                    CustomJObjectParserDelegate<StopTransactionResponse>?  CustomStopTransactionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var stopTransactionResponse,
                         out var errorResponse,
                         CustomStopTransactionResponseParser) &&
                stopTransactionResponse is not null)
            {
                return stopTransactionResponse;
            }

            throw new ArgumentException("The given JSON representation of a stop transaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out StopTransactionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="StopTransactionResponse">The parsed stop transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CP.StopTransactionRequest     Request,
                                       XElement                      XML,
                                       out StopTransactionResponse?  StopTransactionResponse,
                                       out String?                   ErrorResponse)
        {

            try
            {

                StopTransactionResponse = new StopTransactionResponse(

                                              Request,

                                              XML.MapElementOrNullable(OCPPNS.OCPPv1_6_CS + "idTagInfo",
                                                                       OCPPv1_6.IdTagInfo.Parse)

                                          );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                StopTransactionResponse  = null;
                ErrorResponse            = "The given XML representation of a stop transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out StopTransactionResponse, out ErrorResponse, CustomStopTransactionResponseParser = null)

        /// <summary>
        /// Try to parse the given text representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="StopTransactionResponse">The parsed stop transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStopTransactionResponseParser">A delegate to parse custom stop transaction responses.</param>
        public static Boolean TryParse(CP.StopTransactionRequest                              Request,
                                       JObject                                                JSON,
                                       out StopTransactionResponse?                           StopTransactionResponse,
                                       out String?                                            ErrorResponse,
                                       CustomJObjectParserDelegate<StopTransactionResponse>?  CustomStopTransactionResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                StopTransactionResponse = null;

                #region IdTagInfo     [mandatory]

                if (!JSON.ParseMandatoryJSONStruct("idTagInfo",
                                                   "identification tag information",
                                                   OCPPv1_6.IdTagInfo.TryParse,
                                                   out IdTagInfo IdTagInfo,
                                                   out ErrorResponse))
                {
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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                StopTransactionResponse = new StopTransactionResponse(

                                              Request,
                                              IdTagInfo,
                                              null,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData

                                          );

                if (CustomStopTransactionResponseParser is not null)
                    StopTransactionResponse = CustomStopTransactionResponseParser(JSON,
                                                                                  StopTransactionResponse);

                return true;

            }
            catch (Exception e)
            {
                StopTransactionResponse  = null;
                ErrorResponse            = "The given JSON representation of a stop transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "stopTransactionResponse",

                   IdTagInfo.HasValue
                       ? IdTagInfo.Value.ToXML()
                       : null

               );

        #endregion

        #region ToJSON(CustomStopTransactionResponseSerializer = null, CustomIdTagInfoResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStopTransactionResponseSerializer">A delegate to serialize custom start transaction responses.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StopTransactionResponse>?  CustomStopTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>?                CustomIdTagInfoResponseSerializer         = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?           CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IdTagInfo.HasValue
                               ? new JProperty("idTagInfo",   IdTagInfo.Value.ToJSON(CustomIdTagInfoResponseSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.    ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomStopTransactionResponseSerializer is not null
                       ? CustomStopTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        public static StopTransactionResponse Failed(CP.StopTransactionRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (StopTransactionResponse1, StopTransactionResponse2)

        /// <summary>
        /// Compares two stop transaction responses for equality.
        /// </summary>
        /// <param name="StopTransactionResponse1">A stop transaction response.</param>
        /// <param name="StopTransactionResponse2">Another stop transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StopTransactionResponse? StopTransactionResponse1,
                                           StopTransactionResponse? StopTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StopTransactionResponse1, StopTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (StopTransactionResponse1 is null || StopTransactionResponse2 is null)
                return false;

            return StopTransactionResponse1.Equals(StopTransactionResponse2);

        }

        #endregion

        #region Operator != (StopTransactionResponse1, StopTransactionResponse2)

        /// <summary>
        /// Compares two stop transaction responses for inequality.
        /// </summary>
        /// <param name="StopTransactionResponse1">A stop transaction response.</param>
        /// <param name="StopTransactionResponse2">Another stop transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StopTransactionResponse? StopTransactionResponse1,
                                           StopTransactionResponse? StopTransactionResponse2)

            => !(StopTransactionResponse1 == StopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<StopTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two stop transaction responses for equality.
        /// </summary>
        /// <param name="Object">A stop transaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StopTransactionResponse stopTransactionResponse &&
                   Equals(stopTransactionResponse);

        #endregion

        #region Equals(StopTransactionResponse)

        /// <summary>
        /// Compares two stop transaction responses for equality.
        /// </summary>
        /// <param name="StopTransactionResponse">A stop transaction response to compare with.</param>
        public override Boolean Equals(StopTransactionResponse? StopTransactionResponse)

            => StopTransactionResponse is not null &&

               IdTagInfo is not null
                   ? IdTagInfo.Equals(StopTransactionResponse.IdTagInfo)
                   : true;

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

                return IdTagInfo?.GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => IdTagInfo.HasValue
                   ? IdTagInfo.Value.ToString()
                   : "-";

        #endregion

    }

}
