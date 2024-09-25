﻿/*
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
    /// The DiagnosticsStatusNotification request.
    /// </summary>
    public class DiagnosticsStatusNotificationRequest : ARequest<DiagnosticsStatusNotificationRequest>,
                                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/diagnosticsStatusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status of the diagnostics upload.
        /// </summary>
        public DiagnosticsStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DiagnosticsStatusNotification request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Status">The status of the diagnostics upload.</param>
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
        public DiagnosticsStatusNotificationRequest(SourceRouting            Destination,
                                                    DiagnosticsStatus        Status,

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
                   nameof(DiagnosticsStatusNotificationRequest)[..^7],

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

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

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
        //       <ns:diagnosticsStatusNotificationRequest>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:diagnosticsStatusNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:DiagnosticsStatusNotificationRequest",
        //     "title":   "DiagnosticsStatusNotificationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Idle",
        //                 "Uploaded",
        //                 "UploadFailed",
        //                 "Uploading"
        //             ]
        //     }
        // },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }
        // 

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a DiagnosticsStatusNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        public static DiagnosticsStatusNotificationRequest Parse(XElement       XML,
                                                                 Request_Id     RequestId,
                                                                 SourceRouting  Destination,
                                                                 NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var diagnosticsStatusNotificationRequest,
                         out var errorResponse))
            {
                return diagnosticsStatusNotificationRequest;
            }

            throw new ArgumentException("The given XML representation of a DiagnosticsStatusNotification request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DiagnosticsStatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDiagnosticsStatusNotificationRequestParser">A delegate to parse custom DiagnosticsStatusNotification requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static DiagnosticsStatusNotificationRequest Parse(JObject                                                             JSON,
                                                                 Request_Id                                                          RequestId,
                                                                 SourceRouting                                                       Destination,
                                                                 NetworkPath                                                         NetworkPath,
                                                                 DateTime?                                                           RequestTimestamp                                   = null,
                                                                 TimeSpan?                                                           RequestTimeout                                     = null,
                                                                 EventTracking_Id?                                                   EventTrackingId                                    = null,
                                                                 CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?  CustomDiagnosticsStatusNotificationRequestParser   = null,
                                                                 CustomJObjectParserDelegate<Signature>?                             CustomSignatureParser                              = null,
                                                                 CustomJObjectParserDelegate<CustomData>?                            CustomCustomDataParser                             = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var diagnosticsStatusNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDiagnosticsStatusNotificationRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return diagnosticsStatusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a DiagnosticsStatusNotification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out DiagnosticsStatusNotificationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a DiagnosticsStatusNotification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed DiagnosticsStatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                                        XML,
                                       Request_Id                                                      RequestId,
                                       SourceRouting                                                   Destination,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out DiagnosticsStatusNotificationRequest?  DiagnosticsStatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                                ErrorResponse)
        {

            try
            {

                DiagnosticsStatusNotificationRequest = new DiagnosticsStatusNotificationRequest(
                                                           Destination,
                                                           XML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                              DiagnosticsStatusExtensions.Parse),
                                                           RequestId: RequestId
                                                       );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationRequest  = null;
                ErrorResponse                         = "The given XML representation of a DiagnosticsStatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out DiagnosticsStatusNotificationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DiagnosticsStatusNotification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed DiagnosticsStatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDiagnosticsStatusNotificationRequestParser">A delegate to parse custom DiagnosticsStatusNotification requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                             JSON,
                                       Request_Id                                                          RequestId,
                                       SourceRouting                                                       Destination,
                                       NetworkPath                                                         NetworkPath,
                                       [NotNullWhen(true)]  out DiagnosticsStatusNotificationRequest?      DiagnosticsStatusNotificationRequest,
                                       [NotNullWhen(false)] out String?                                    ErrorResponse,
                                       DateTime?                                                           RequestTimestamp                                   = null,
                                       TimeSpan?                                                           RequestTimeout                                     = null,
                                       EventTracking_Id?                                                   EventTrackingId                                    = null,
                                       CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?  CustomDiagnosticsStatusNotificationRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                             CustomSignatureParser                              = null,
                                       CustomJObjectParserDelegate<CustomData>?                            CustomCustomDataParser                             = null)
        {

            try
            {

                DiagnosticsStatusNotificationRequest = null;

                #region DiagnosticsStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "diagnostics status",
                                       DiagnosticsStatusExtensions.Parse,
                                       out DiagnosticsStatus DiagnosticsStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                DiagnosticsStatusNotificationRequest = new DiagnosticsStatusNotificationRequest(

                                                           Destination,
                                                           DiagnosticsStatus,

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

                if (CustomDiagnosticsStatusNotificationRequestParser is not null)
                    DiagnosticsStatusNotificationRequest = CustomDiagnosticsStatusNotificationRequestParser(JSON,
                                                                                                            DiagnosticsStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationRequest  = null;
                ErrorResponse                         = "The given JSON representation of a DiagnosticsStatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationRequest",
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomDiagnosticsStatusNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDiagnosticsStatusNotificationRequestSerializer">A delegate to serialize custom DiagnosticsStatusNotification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationRequest>?  CustomDiagnosticsStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                             CustomSignatureSerializer                              = null,
                              CustomJObjectSerializerDelegate<CustomData>?                            CustomCustomDataSerializer                             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDiagnosticsStatusNotificationRequestSerializer is not null
                       ? CustomDiagnosticsStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A DiagnosticsStatusNotification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another DiagnosticsStatusNotification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationRequest? DiagnosticsStatusNotificationRequest1,
                                           DiagnosticsStatusNotificationRequest? DiagnosticsStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DiagnosticsStatusNotificationRequest1 is null || DiagnosticsStatusNotificationRequest2 is null)
                return false;

            return DiagnosticsStatusNotificationRequest1.Equals(DiagnosticsStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification requests for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A DiagnosticsStatusNotification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another DiagnosticsStatusNotification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationRequest? DiagnosticsStatusNotificationRequest1,
                                           DiagnosticsStatusNotificationRequest? DiagnosticsStatusNotificationRequest2)

            => !(DiagnosticsStatusNotificationRequest1 == DiagnosticsStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification requests for equality.
        /// </summary>
        /// <param name="Object">A DiagnosticsStatusNotification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DiagnosticsStatusNotificationRequest diagnosticsStatusNotificationRequest &&
                   Equals(diagnosticsStatusNotificationRequest);

        #endregion

        #region Equals(DiagnosticsStatusNotificationRequest)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest">A DiagnosticsStatusNotification request to compare with.</param>
        public override Boolean Equals(DiagnosticsStatusNotificationRequest? DiagnosticsStatusNotificationRequest)

            => DiagnosticsStatusNotificationRequest is not null &&

               Status.     Equals(DiagnosticsStatusNotificationRequest.Status) &&

               base.GenericEquals(DiagnosticsStatusNotificationRequest);

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

            => Status.ToString();

        #endregion

    }

}
