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
    /// The ChangeConfiguration request.
    /// </summary>
    public class ChangeConfigurationRequest : ARequest<ChangeConfigurationRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/changeConfigurationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The name of the configuration setting to change.
        /// </summary>
        public String         Key      { get; }

        /// <summary>
        /// The new value as string for the setting.
        /// </summary>
        public String         Value    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChangeConfiguration request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
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
        public ChangeConfigurationRequest(SourceRouting            Destination,
                                          String                   Key,
                                          String                   Value,

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
                   nameof(ChangeConfigurationRequest)[..^7],

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

            Key = Key.Trim();

            this.Key    = Key ?? throw new ArgumentNullException(nameof(Key), "The given configuration key must not be null or empty!");
            this.Value  = Value;

            unchecked
            {

                hashCode = this.Key.  GetHashCode() * 5 ^
                           this.Value.GetHashCode() * 3 ^
                           base.      GetHashCode();

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
        //       <ns:changeConfigurationRequest>
        //
        //          <ns:key>?</ns:key>
        //          <ns:value>?</ns:value>
        //
        //       </ns:changeConfigurationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeConfigurationRequest",
        //     "title":   "ChangeConfigurationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "key": {
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "value": {
        //             "type": "string",
        //             "maxLength": 500
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "key",
        //         "value"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a ChangeConfiguration request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static ChangeConfigurationRequest Parse(XElement       XML,
                                                       Request_Id     RequestId,
                                                       SourceRouting  Destination,
                                                       NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var changeConfigurationRequest,
                         out var errorResponse))
            {
                return changeConfigurationRequest;
            }

            throw new ArgumentException("The given XML representation of a ChangeConfiguration request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ChangeConfiguration request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomChangeConfigurationRequestParser">A delegate to parse custom ChangeConfiguration requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static ChangeConfigurationRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                             Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<ChangeConfigurationRequest>?  CustomChangeConfigurationRequestParser   = null,
                                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var changeConfigurationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomChangeConfigurationRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return changeConfigurationRequest;
            }

            throw new ArgumentException("The given JSON representation of a ChangeConfiguration request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out ChangeConfigurationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a ChangeConfiguration request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ChangeConfigurationRequest">The parsed ChangeConfiguration request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                              XML,
                                       Request_Id                                            RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out ChangeConfigurationRequest?  ChangeConfigurationRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse)
        {

            try
            {

                ChangeConfigurationRequest = new ChangeConfigurationRequest(

                                                 Destination,

                                                 XML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "key"),
                                                 XML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "value"),

                                                 RequestId:    RequestId,
                                                 NetworkPath:  NetworkPath

                                             );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ChangeConfigurationRequest  = null;
                ErrorResponse               = "The given XML representation of a ChangeConfiguration request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ChangeConfigurationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ChangeConfiguration request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ChangeConfigurationRequest">The parsed ChangeConfiguration request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomChangeConfigurationRequestParser">A delegate to parse custom ChangeConfiguration requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out ChangeConfigurationRequest?      ChangeConfigurationRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<ChangeConfigurationRequest>?  CustomChangeConfigurationRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            try
            {

                ChangeConfigurationRequest = null;

                #region Key            [mandatory]

                if (!JSON.ParseMandatoryText("key",
                                             "configuration key",
                                             out String? Key,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Value          [mandatory]

                if (!JSON.ParseMandatoryText("value",
                                             "configuration value",
                                             out String? Value,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

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


                ChangeConfigurationRequest = new ChangeConfigurationRequest(

                                                 Destination,
                                                 Key,
                                                 Value,

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

                if (CustomChangeConfigurationRequestParser is not null)
                    ChangeConfigurationRequest = CustomChangeConfigurationRequestParser(JSON,
                                                                                        ChangeConfigurationRequest);

                return true;

            }
            catch (Exception e)
            {
                ChangeConfigurationRequest  = null;
                ErrorResponse               = "The given JSON representation of a ChangeConfiguration request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "changeConfigurationRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "key",   Key),
                   new XElement(OCPPNS.OCPPv1_6_CP + "value", Value)

               );

        #endregion

        #region ToJSON(CustomChangeConfigurationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeConfigurationRequestSerializer">A delegate to serialize custom ChangeConfiguration requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeConfigurationRequest>?  CustomChangeConfigurationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("key",          Key),
                                 new JProperty("value",        Value),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChangeConfigurationRequestSerializer is not null
                       ? CustomChangeConfigurationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChangeConfigurationRequest1, ChangeConfigurationRequest2)

        /// <summary>
        /// Compares two ChangeConfiguration requests for equality.
        /// </summary>
        /// <param name="ChangeConfigurationRequest1">A ChangeConfiguration request.</param>
        /// <param name="ChangeConfigurationRequest2">Another ChangeConfiguration request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeConfigurationRequest? ChangeConfigurationRequest1,
                                           ChangeConfigurationRequest? ChangeConfigurationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeConfigurationRequest1, ChangeConfigurationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeConfigurationRequest1 is null || ChangeConfigurationRequest2 is null)
                return false;

            return ChangeConfigurationRequest1.Equals(ChangeConfigurationRequest2);

        }

        #endregion

        #region Operator != (ChangeConfigurationRequest1, ChangeConfigurationRequest2)

        /// <summary>
        /// Compares two ChangeConfiguration requests for inequality.
        /// </summary>
        /// <param name="ChangeConfigurationRequest1">A ChangeConfiguration request.</param>
        /// <param name="ChangeConfigurationRequest2">Another ChangeConfiguration request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeConfigurationRequest? ChangeConfigurationRequest1,
                                           ChangeConfigurationRequest? ChangeConfigurationRequest2)

            => !(ChangeConfigurationRequest1 == ChangeConfigurationRequest2);

        #endregion

        #endregion

        #region IEquatable<ChangeConfigurationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChangeConfiguration requests for equality.
        /// </summary>
        /// <param name="Object">A ChangeConfiguration request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeConfigurationRequest changeConfigurationRequest &&
                   Equals(changeConfigurationRequest);

        #endregion

        #region Equals(ChangeConfigurationRequest)

        /// <summary>
        /// Compares two ChangeConfiguration requests for equality.
        /// </summary>
        /// <param name="ChangeConfigurationRequest">A ChangeConfiguration request to compare with.</param>
        public override Boolean Equals(ChangeConfigurationRequest? ChangeConfigurationRequest)

            => ChangeConfigurationRequest is not null &&

               Key.        Equals(ChangeConfigurationRequest.Key)   &&
               Value.      Equals(ChangeConfigurationRequest.Value) &&

               base.GenericEquals(ChangeConfigurationRequest);

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

            => String.Concat(Key,
                             " = ",
                             Value);

        #endregion

    }

}
