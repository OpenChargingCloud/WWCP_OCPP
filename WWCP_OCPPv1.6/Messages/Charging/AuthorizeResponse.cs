/*
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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// An Authorize response.
    /// </summary>
    public class AuthorizeResponse : AResponse<AuthorizeRequest,
                                               AuthorizeResponse>,
                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/AuthorizeResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification tag info.
        /// </summary>
        public IdTagInfo      IdTagInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an Authorize response.
        /// </summary>
        /// <param name="Request">The Authorize request leading to this response.</param>
        /// <param name="IdTagInfo">The identification tag info.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public AuthorizeResponse(AuthorizeRequest         Request,
                                 IdTagInfo                IdTagInfo,

                                 Result?                  Result                = null,
                                 DateTime?                ResponseTimestamp     = null,

                                 SourceRouting?           Destination           = null,
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

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.IdTagInfo = IdTagInfo;

            unchecked
            {

                hashCode = this.IdTagInfo.GetHashCode() * 3 ^
                           base.          GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:AuthorizeResponse>
        //
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
        //       </ns:AuthorizeResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:AuthorizeResponse",
        //     "title":    "AuthorizeResponse",
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
        //     "additionalProperties": false,
        //     "required": [
        //         "idTagInfo"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of an Authorize response.
        /// </summary>
        /// <param name="Request">The Authorize request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static AuthorizeResponse Parse(AuthorizeRequest   Request,
                                              XElement           XML,
                                              SourceRouting      Destination,
                                              NetworkPath        NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var authorizeResponse,
                         out var errorResponse))
            {
                return authorizeResponse;
            }

            throw new ArgumentException("The given XML representation of an Authorize request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an Authorize response.
        /// </summary>
        /// <param name="Request">The Authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomAuthorizeResponseParser">An optional delegate to parse custom Authorize responses.</param>
        /// <param name="CustomIdTagInfoParser">A delegate to parse custom IdTagInfos.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static AuthorizeResponse Parse(AuthorizeRequest                                 Request,
                                              JObject                                          JSON,
                                              SourceRouting                                    Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        ResponseTimestamp               = null,
                                              CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null,
                                              CustomJObjectParserDelegate<IdTagInfo>?          CustomIdTagInfoParser           = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var authorizeResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAuthorizeResponseParser,
                         CustomIdTagInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return authorizeResponse;
            }

            throw new ArgumentException("The given JSON representation of an Authorize response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out AuthorizeResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an Authorize response.
        /// </summary>
        /// <param name="Request">The Authorize request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="AuthorizeResponse">The parsed Authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(AuthorizeRequest                             Request,
                                       XElement                                     XML,
                                       SourceRouting                                Destination,
                                       NetworkPath                                  NetworkPath,
                                       [NotNullWhen(true)]  out AuthorizeResponse?  AuthorizeResponse,
                                       [NotNullWhen(false)] out String?             ErrorResponse)
        {

            try
            {

                AuthorizeResponse  = null;
                ErrorResponse      = null;

                if (IdTagInfo.TryParse(XML.ElementOrFail(OCPPNS.OCPPv1_6_CS + "idTagInfo"),
                                       out var idTagInfo,
                                       out ErrorResponse))
                {

                    AuthorizeResponse = new AuthorizeResponse(Request,
                                                              idTagInfo);

                    return true;

                }

                return false;

            }
            catch (Exception e)
            {
                AuthorizeResponse  = null;
                ErrorResponse      = "The given XML representation of an Authorize response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out AuthorizeResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an Authorize response.
        /// </summary>
        /// <param name="Request">The Authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="AuthorizeResponse">The parsed Authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomAuthorizeResponseParser">An optional delegate to parse custom Authorize responses.</param>
        /// <param name="CustomIdTagInfoParser">A delegate to parse custom IdTagInfos.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(AuthorizeRequest                                 Request,
                                       JObject                                          JSON,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out AuthorizeResponse?      AuthorizeResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        ResponseTimestamp               = null,
                                       CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null,
                                       CustomJObjectParserDelegate<IdTagInfo>?          CustomIdTagInfoParser           = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            try
            {

                AuthorizeResponse = null;

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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AuthorizeResponse = new AuthorizeResponse(

                                        Request,
                                        IdTagInfo,

                                        null,
                                        ResponseTimestamp,

                                        Destination,
                                        NetworkPath,

                                        null,
                                        null,
                                        Signatures,

                                        CustomData

                                    );

                if (CustomAuthorizeResponseParser is not null)
                    AuthorizeResponse = CustomAuthorizeResponseParser(JSON,
                                                                      AuthorizeResponse);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeResponse  = null;
                ErrorResponse      = "The given JSON representation of an Authorize response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "AuthorizeResponse",
                   IdTagInfo.ToXML()
               );

        #endregion

        #region ToJSON(CustomAuthorizeResponseSerializer = null, CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeResponseSerializer">A delegate to serialize custom Authorize responses.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>?          CustomIdTagInfoResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idTagInfo",   IdTagInfo.  ToJSON(CustomIdTagInfoResponseSerializer)),


                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizeResponseSerializer is not null
                       ? CustomAuthorizeResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The Authorize failed because of a request error.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        public static AuthorizeResponse RequestError(AuthorizeRequest         Request,
                                                     EventTracking_Id         EventTrackingId,
                                                     ResultCode               ErrorCode,
                                                     String?                  ErrorDescription    = null,
                                                     JObject?                 ErrorDetails        = null,
                                                     DateTime?                ResponseTimestamp   = null,

                                                     SourceRouting?           Destination         = null,
                                                     NetworkPath?             NetworkPath         = null,

                                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                                     IEnumerable<Signature>?  Signatures          = null,

                                                     CustomData?              CustomData          = null)

            => new (

                   Request,
                   new IdTagInfo(AuthorizationStatus.Error),
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AuthorizeResponse FormationViolation(AuthorizeRequest  Request,
                                                           String            ErrorDescription)

            => new (Request,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AuthorizeResponse SignatureError(AuthorizeRequest  Request,
                                                       String            ErrorDescription)

            => new (Request,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AuthorizeResponse Failed(AuthorizeRequest  Request,
                                               String?           Description   = null)

            => new (Request,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.Server(Description));


        /// <summary>
        /// The Authorize failed because of an exception.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="Exception">The exception.</param>
        public static AuthorizeResponse ExceptionOccured(AuthorizeRequest  Request,
                                                         Exception         Exception)

            => new (Request,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two Authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A Authorize response.</param>
        /// <param name="AuthorizeResponse2">Another Authorize response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeResponse1, AuthorizeResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AuthorizeResponse1 is null || AuthorizeResponse2 is null)
                return false;

            return AuthorizeResponse1.Equals(AuthorizeResponse2);

        }

        #endregion

        #region Operator != (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two Authorize responses for inequality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A Authorize response.</param>
        /// <param name="AuthorizeResponse2">Another Authorize response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)

            => !(AuthorizeResponse1 == AuthorizeResponse2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Authorize responses for equality.
        /// </summary>
        /// <param name="Object">An Authorize response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeResponse AuthorizeResponse &&
                   Equals(AuthorizeResponse);

        #endregion

        #region Equals(AuthorizeResponse)

        /// <summary>
        /// Compares two Authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse">An Authorize response to compare with.</param>
        public override Boolean Equals(AuthorizeResponse? AuthorizeResponse)

            => AuthorizeResponse is not null &&
                   IdTagInfo.Equals(AuthorizeResponse.IdTagInfo);

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

            => IdTagInfo.ToString();

        #endregion

    }

}
