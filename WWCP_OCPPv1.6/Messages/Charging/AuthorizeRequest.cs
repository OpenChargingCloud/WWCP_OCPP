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
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The Authorize request.
    /// </summary>
    public class AuthorizeRequest : ARequest<AuthorizeRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/AuthorizeRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identifier that needs to be Authorized.
        /// </summary>
        [Mandatory]
        public IdToken        IdTag    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Authorize request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="IdTag">The identifier that needs to be Authorized.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public AuthorizeRequest(SourceRouting            Destination,
                                IdToken                  IdTag,

                                IEnumerable<KeyPair>?    SignKeys              = null,
                                IEnumerable<SignInfo>?   SignInfos             = null,
                                IEnumerable<Signature>?  Signatures            = null,

                                CustomData?              CustomData            = null,

                                Request_Id?              RequestId             = null,
                                DateTime?                RequestTimestamp      = null,
                                TimeSpan?                RequestTimeout        = null,
                                EventTracking_Id?        EventTrackingId       = null,
                                NetworkPath?             NetworkPath           = null,
                                SerializationFormats?    SerializationFormat   = null,
                                CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(AuthorizeRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.IdTag = IdTag;

            unchecked
            {

                hashCode = this.IdTag.GetHashCode() * 3 ^
                           base.      GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:AuthorizeRequest>
        //
        //          <ns:idTag>?</ns:idTag>
        //
        //       </ns:AuthorizeRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:AuthorizeRequest",
        //     "title":    "AuthorizeRequest",
        //     "type":     "object",
        //     "properties": {
        //         "idTag": {
        //             "type":      "string",
        //             "maxLength":  20
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "idTag"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of an Authorize request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static AuthorizeRequest Parse(XElement       XML,
                                             Request_Id     RequestId,
                                             SourceRouting  Destination,
                                             NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var authorizeRequest,
                         out var errorResponse))
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given XML representation of an Authorize request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom Authorize requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static AuthorizeRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             SourceRouting                                   Destination,
                                             NetworkPath                                     NetworkPath,
                                             DateTime?                                       RequestTimestamp               = null,
                                             TimeSpan?                                       RequestTimeout                 = null,
                                             EventTracking_Id?                               EventTrackingId                = null,
                                             CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser   = null,
                                             CustomJObjectParserDelegate<Signature>?         CustomSignatureParser          = null,
                                             CustomJObjectParserDelegate<CustomData>?        CustomCustomDataParser         = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var authorizeRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomAuthorizeRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given JSON representation of an Authorize request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out AuthorizeRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an Authorize request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="AuthorizeRequest">The parsed Authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                    XML,
                                       Request_Id                                  RequestId,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out AuthorizeRequest?  AuthorizeRequest,
                                       [NotNullWhen(false)] out String?            ErrorResponse)
        {

            try
            {

                AuthorizeRequest = new AuthorizeRequest(

                                       Destination,

                                       XML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "idTag",
                                                          IdToken.Parse),

                                       RequestId: RequestId

                                   );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                AuthorizeRequest  = null;
                ErrorResponse     = "The given XML representation of an Authorize request is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out AuthorizeRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AuthorizeRequest">The parsed Authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom Authorize requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       SourceRouting                                   Destination,
                                       NetworkPath                                     NetworkPath,
                                       [NotNullWhen(true)]  out AuthorizeRequest?      AuthorizeRequest,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTime?                                       RequestTimestamp               = null,
                                       TimeSpan?                                       RequestTimeout                 = null,
                                       EventTracking_Id?                               EventTrackingId                = null,
                                       CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?         CustomSignatureParser          = null,
                                       CustomJObjectParserDelegate<CustomData>?        CustomCustomDataParser         = null)
        {

            try
            {

                AuthorizeRequest = null;

                #region IdTag         [mandatory]

                if (!JSON.ParseMandatory("idTag",
                                         "identification tag",
                                         IdToken.TryParse,
                                         out IdToken IdTag,
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


                AuthorizeRequest = new AuthorizeRequest(

                                       Destination,
                                       IdTag,

                                       null,
                                       null,
                                       Signatures,

                                       CustomData,

                                       RequestId,
                                       RequestTimestamp,
                                       RequestTimeout,
                                       EventTrackingId,
                                       NetworkPath

                                   );

                if (CustomAuthorizeRequestParser is not null)
                    AuthorizeRequest = CustomAuthorizeRequestParser(JSON,
                                                                    AuthorizeRequest);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeRequest  = null;
                ErrorResponse     = "The given JSON representation of an Authorize request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "AuthorizeRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "idTag",  IdTag.ToString())

               );

        #endregion

        #region ToJSON(CustomAuthorizeRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeRequestSerializer">A delegate to serialize custom Authorize requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeRequest>?  CustomAuthorizeRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?         CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idTag",        IdTag.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizeRequestSerializer is not null
                       ? CustomAuthorizeRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeRequest? AuthorizeRequest1,
                                           AuthorizeRequest? AuthorizeRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeRequest1, AuthorizeRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AuthorizeRequest1 is null || AuthorizeRequest2 is null)
                return false;

            return AuthorizeRequest1.Equals(AuthorizeRequest2);

        }

        #endregion

        #region Operator != (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two Authorize requests for inequality.
        /// </summary>
        /// <param name="AuthorizeRequest1">An Authorize request.</param>
        /// <param name="AuthorizeRequest2">Another Authorize request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeRequest? AuthorizeRequest1,
                                           AuthorizeRequest? AuthorizeRequest2)

            => !(AuthorizeRequest1 == AuthorizeRequest2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="Object">An Authorize request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeRequest AuthorizeRequest &&
                   Equals(AuthorizeRequest);

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two Authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">An Authorize request to compare with.</param>
        public override Boolean Equals(AuthorizeRequest? AuthorizeRequest)

            => AuthorizeRequest is not null &&

               IdTag.      Equals(AuthorizeRequest.IdTag) &&

               base.GenericEquals(AuthorizeRequest);

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

            => IdTag.ToString();

        #endregion

    }

}
