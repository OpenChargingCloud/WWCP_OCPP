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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The authorize request.
    /// </summary>
    public class AuthorizeRequest : ARequest<AuthorizeRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/authorizeRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identifier that needs to be authorized.
        /// </summary>
        [Mandatory]
        public IdToken        IdTag    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorize request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="IdTag">The identifier that needs to be authorized.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public AuthorizeRequest(NetworkingNode_Id             NetworkingNodeId,
                                IdToken                       IdTag,

                                IEnumerable<WWCP.KeyPair>?    SignKeys            = null,
                                IEnumerable<WWCP.SignInfo>?   SignInfos           = null,
                                IEnumerable<Signature>?  Signatures          = null,

                                CustomData?                   CustomData          = null,

                                Request_Id?                   RequestId           = null,
                                DateTime?                     RequestTimestamp    = null,
                                TimeSpan?                     RequestTimeout      = null,
                                EventTracking_Id?             EventTrackingId     = null,
                                NetworkPath?                  NetworkPath         = null,
                                CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
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
                   CancellationToken)

        {
            this.IdTag = IdTag;
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
        //       <ns:authorizeRequest>
        //
        //          <ns:idTag>?</ns:idTag>
        //
        //       </ns:authorizeRequest>
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

        #region (static) Parse   (XML,  RequestId, NetworkingNodeId)

        /// <summary>
        /// Parse the given XML representation of an authorize request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        public static AuthorizeRequest Parse(XElement           XML,
                                             Request_Id         RequestId,
                                             NetworkingNode_Id  NetworkingNodeId)
        {

            if (TryParse(XML,
                         RequestId,
                         NetworkingNodeId,
                         out var authorizeRequest,
                         out var errorResponse) &&
                authorizeRequest is not null)
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given XML representation of an authorize request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomAuthorizeRequestParser">An optional delegate to parse custom authorize requests.</param>
        public static AuthorizeRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             NetworkingNode_Id                               NetworkingNodeId,
                                             NetworkPath                                     NetworkPath,
                                             CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var authorizeRequest,
                         out var errorResponse,
                         CustomAuthorizeRequestParser) &&
                authorizeRequest is not null)
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given JSON representation of an authorize request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, NetworkingNodeId, out AuthorizeRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an authorize request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement               XML,
                                       Request_Id             RequestId,
                                       NetworkingNode_Id      NetworkingNodeId,
                                       out AuthorizeRequest?  AuthorizeRequest,
                                       out String?            ErrorResponse)
        {

            try
            {

                AuthorizeRequest = new AuthorizeRequest(

                                       NetworkingNodeId,

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
                ErrorResponse     = "The given XML representation of an authorize request is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out AuthorizeRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       Request_Id             RequestId,
                                       NetworkingNode_Id      NetworkingNodeId,
                                       NetworkPath            NetworkPath,
                                       out AuthorizeRequest?  AuthorizeRequest,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out AuthorizeRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizeRequestParser">An optional delegate to parse custom Authorize requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       NetworkingNode_Id                               NetworkingNodeId,
                                       NetworkPath                                     NetworkPath,
                                       out AuthorizeRequest?                           AuthorizeRequest,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizeRequest>?  CustomAuthorizeRequestParser)
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

                                       NetworkingNodeId,
                                       IdTag,

                                       null,
                                       null,
                                       Signatures,

                                       CustomData,

                                       RequestId,
                                       null,
                                       null,
                                       null,
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
                ErrorResponse     = "The given JSON representation of an authorize request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "authorizeRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "idTag",  IdTag.ToString())

               );

        #endregion

        #region ToJSON(CustomAuthorizeRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeRequestSerializer">A delegate to serialize custom authorize requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeRequest>?  CustomAuthorizeRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?    CustomSignatureSerializer          = null,
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
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest1">An authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
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
        /// Compares two authorize requests for inequality.
        /// </summary>
        /// <param name="AuthorizeRequest1">An authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeRequest? AuthorizeRequest1,
                                           AuthorizeRequest? AuthorizeRequest2)

            => !(AuthorizeRequest1 == AuthorizeRequest2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="Object">An authorize request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeRequest authorizeRequest &&
                   Equals(authorizeRequest);

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">An authorize request to compare with.</param>
        public override Boolean Equals(AuthorizeRequest? AuthorizeRequest)

            => AuthorizeRequest is not null &&

               IdTag.      Equals(AuthorizeRequest.IdTag) &&

               base.GenericEquals(AuthorizeRequest);

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

                return IdTag.GetHashCode() * 3 ^
                       base. GetHashCode();

            }
        }

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
