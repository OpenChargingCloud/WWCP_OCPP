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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A GetConfiguration request.
    /// </summary>
    public class GetConfigurationRequest : ARequest<GetConfigurationRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/getConfigurationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// An optional enumeration of keys for which the configuration is requested.
        /// Return all keys if empty.
        /// </summary>
        public IEnumerable<String>  Keys    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a GetConfiguration request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
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
        public GetConfigurationRequest(SourceRouting            Destination,
                                       IEnumerable<String>?     Keys                  = null,

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
                   nameof(GetConfigurationRequest)[..^7],

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

            this.Keys = Keys is not null
                            ? Keys.Where (key => key is not null).
                                   Select(key => key.Trim().SubstringMax(50)).
                                   Where (key => key.IsNotNullOrEmpty()).
                                   Distinct().
                                   ToArray()
                            : [];

            unchecked
            {

                hashCode = this.Keys.CalcHashCode() * 3 ^
                           base.     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:getConfigurationRequest>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:key>?</ns:key>
        //
        //       </ns:getConfigurationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetConfigurationRequest",
        //     "title":   "GetConfigurationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "key": {
        //             "type": "array",
        //             "items": {
        //                 "type": "string",
        //                 "maxLength": 50
        //             }
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a GetConfiguration request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static GetConfigurationRequest Parse(XElement       XML,
                                                    Request_Id     RequestId,
                                                    SourceRouting  Destination,
                                                    NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getConfigurationRequest,
                         out var errorResponse))
            {
                return getConfigurationRequest;
            }

            throw new ArgumentException("The given XML representation of a GetConfiguration request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetConfiguration request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetConfigurationRequestParser">A delegate to parse custom GetConfiguration requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static GetConfigurationRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<GetConfigurationRequest>?  CustomGetConfigurationRequestParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getConfigurationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetConfigurationRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getConfigurationRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetConfiguration request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out GetConfigurationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a GetConfiguration request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetConfigurationRequest">The parsed GetConfiguration request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                           XML,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out GetConfigurationRequest?  GetConfigurationRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse)
        {

            try
            {

                GetConfigurationRequest = new GetConfigurationRequest(

                                              Destination,

                                              XML.ElementValues(OCPPNS.OCPPv1_6_CP + "key"),

                                              RequestId:    RequestId,
                                              NetworkPath:  NetworkPath

                                          );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetConfigurationRequest  = null;
                ErrorResponse            = "The given JSON representation of a GetConfiguration request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetConfigurationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetConfiguration request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetConfigurationRequest">The parsed GetConfiguration request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetConfigurationRequestParser">A delegate to parse custom GetConfiguration requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out GetConfigurationRequest?      GetConfigurationRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<GetConfigurationRequest>?  CustomGetConfigurationRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                GetConfigurationRequest = null;

                #region Keys             [optional]

                if (JSON.GetOptional("key",
                                     "configuration keys",
                                     out IEnumerable<String> Keys,
                                     out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                GetConfigurationRequest = new GetConfigurationRequest(

                                              Destination,
                                              Keys,

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

                if (CustomGetConfigurationRequestParser is not null)
                    GetConfigurationRequest = CustomGetConfigurationRequestParser(JSON,
                                                                                  GetConfigurationRequest);

                return true;

            }
            catch (Exception e)
            {
                GetConfigurationRequest  = null;
                ErrorResponse            = "The given JSON representation of a GetConfiguration request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getConfigurationRequest",

                       Keys.Select(key => new XElement(OCPPNS.OCPPv1_6_CP + "key",  key))

                   );

        #endregion

        #region ToJSON(CustomGetConfigurationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetConfigurationRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetConfigurationRequest>?  CustomGetConfigurationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           Keys.Any()
                               ? new JProperty("key",          new JArray(Keys))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetConfigurationRequestSerializer is not null
                       ? CustomGetConfigurationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetConfigurationRequest1, GetConfigurationRequest2)

        /// <summary>
        /// Compares two GetConfiguration requests for equality.
        /// </summary>
        /// <param name="GetConfigurationRequest1">A GetConfiguration request.</param>
        /// <param name="GetConfigurationRequest2">Another GetConfiguration request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetConfigurationRequest? GetConfigurationRequest1,
                                           GetConfigurationRequest? GetConfigurationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetConfigurationRequest1, GetConfigurationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetConfigurationRequest1 is null || GetConfigurationRequest2 is null)
                return false;

            return GetConfigurationRequest1.Equals(GetConfigurationRequest2);

        }

        #endregion

        #region Operator != (GetConfigurationRequest1, GetConfigurationRequest2)

        /// <summary>
        /// Compares two GetConfiguration requests for inequality.
        /// </summary>
        /// <param name="GetConfigurationRequest1">A GetConfiguration request.</param>
        /// <param name="GetConfigurationRequest2">Another GetConfiguration request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetConfigurationRequest? GetConfigurationRequest1,
                                           GetConfigurationRequest? GetConfigurationRequest2)

            => !(GetConfigurationRequest1 == GetConfigurationRequest2);

        #endregion

        #endregion

        #region IEquatable<GetConfigurationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetConfiguration requests for equality.
        /// </summary>
        /// <param name="Object">A GetConfiguration request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetConfigurationRequest getConfigurationRequest &&
                   Equals(getConfigurationRequest);

        #endregion

        #region Equals(GetConfigurationRequest)

        /// <summary>
        /// Compares two GetConfiguration requests for equality.
        /// </summary>
        /// <param name="GetConfigurationRequest">A GetConfiguration request to compare with.</param>
        public override Boolean Equals(GetConfigurationRequest? GetConfigurationRequest)

            => GetConfigurationRequest is not null &&

               Keys.Count().Equals(GetConfigurationRequest.Keys.Count())   &&
               Keys.All(key =>     GetConfigurationRequest.Keys.Contains(key)) &&

               base. GenericEquals(GetConfigurationRequest);

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

            => Keys.Any()
                   ? $"{Keys.Count()} configuration key(s)"
                   : "all configuration keys";

        #endregion


    }

}
