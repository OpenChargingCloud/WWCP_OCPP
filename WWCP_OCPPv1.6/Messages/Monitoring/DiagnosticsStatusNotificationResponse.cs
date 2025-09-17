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
    /// A DiagnosticsStatusNotification response.
    /// </summary>
    public class DiagnosticsStatusNotificationResponse : AResponse<DiagnosticsStatusNotificationRequest,
                                                                   DiagnosticsStatusNotificationResponse>,
                                                         IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/diagnosticsStatusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DiagnosticsStatusNotification response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
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
        public DiagnosticsStatusNotificationResponse(DiagnosticsStatusNotificationRequest  Request,

                                                     Result?                               Result                = null,
                                                     DateTimeOffset?                       ResponseTimestamp     = null,

                                                     SourceRouting?                        Destination           = null,
                                                     NetworkPath?                          NetworkPath           = null,

                                                     IEnumerable<KeyPair>?                 SignKeys              = null,
                                                     IEnumerable<SignInfo>?                SignInfos             = null,
                                                     IEnumerable<Signature>?               Signatures            = null,

                                                     CustomData?                           CustomData            = null,

                                                     SerializationFormats?                 SerializationFormat   = null,
                                                     CancellationToken                     CancellationToken     = default)

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

        { }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:diagnosticsStatusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:DiagnosticsStatusNotificationResponse",
        //     "title":   "DiagnosticsStatusNotificationResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a DiagnosticsStatusNotification response.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static DiagnosticsStatusNotificationResponse Parse(DiagnosticsStatusNotificationRequest  Request,
                                                                  XElement                              XML,
                                                                  SourceRouting                         Destination,
                                                                  NetworkPath                           NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var diagnosticsStatusNotificationResponse,
                         out var errorResponse))
            {
                return diagnosticsStatusNotificationResponse;
            }

            throw new ArgumentException("The given XML representation of a DiagnosticsStatusNotification response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DiagnosticsStatusNotification response.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomDiagnosticsStatusNotificationResponseParser">An optional delegate to parse custom DiagnosticsStatusNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static DiagnosticsStatusNotificationResponse Parse(DiagnosticsStatusNotificationRequest                                 Request,
                                                                  JObject                                                              JSON,
                                                                  SourceRouting                                                        Destination,
                                                                  NetworkPath                                                          NetworkPath,
                                                                  DateTimeOffset?                                                      ResponseTimestamp                                   = null,
                                                                  CustomJObjectParserDelegate<DiagnosticsStatusNotificationResponse>?  CustomDiagnosticsStatusNotificationResponseParser   = null,
                                                                  CustomJObjectParserDelegate<Signature>?                              CustomSignatureParser                               = null,
                                                                  CustomJObjectParserDelegate<CustomData>?                             CustomCustomDataParser                              = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var diagnosticsStatusNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomDiagnosticsStatusNotificationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return diagnosticsStatusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a DiagnosticsStatusNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out DiagnosticsStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a DiagnosticsStatusNotification response.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed DiagnosticsStatusNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(DiagnosticsStatusNotificationRequest                             Request,
                                       XElement                                                         XML,
                                       SourceRouting                                                    Destination,
                                       NetworkPath                                                      NetworkPath,
                                       [NotNullWhen(true)]  out DiagnosticsStatusNotificationResponse?  DiagnosticsStatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse)
        {

            try
            {

                ErrorResponse                          = null;
                DiagnosticsStatusNotificationResponse  = new DiagnosticsStatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationResponse  = null;
                ErrorResponse                          = "The given XML representation of a DiagnosticsStatusNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out DiagnosticsStatusNotificationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DiagnosticsStatusNotification response.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed DiagnosticsStatusNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomDiagnosticsStatusNotificationResponseParser">An optional delegate to parse custom DiagnosticsStatusNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(DiagnosticsStatusNotificationRequest                                 Request,
                                       JObject                                                              JSON,
                                       SourceRouting                                                        Destination,
                                       NetworkPath                                                          NetworkPath,
                                       [NotNullWhen(true)]  out DiagnosticsStatusNotificationResponse?      DiagnosticsStatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                                     ErrorResponse,
                                       DateTimeOffset?                                                      ResponseTimestamp                                   = null,
                                       CustomJObjectParserDelegate<DiagnosticsStatusNotificationResponse>?  CustomDiagnosticsStatusNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                              CustomSignatureParser                               = null,
                                       CustomJObjectParserDelegate<CustomData>?                             CustomCustomDataParser                              = null)
        {

            try
            {

                DiagnosticsStatusNotificationResponse  = null;

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


                DiagnosticsStatusNotificationResponse = new DiagnosticsStatusNotificationResponse(

                                                            Request,

                                                            null,
                                                            ResponseTimestamp,

                                                            Destination,
                                                            NetworkPath,

                                                            null,
                                                            null,
                                                            Signatures,

                                                            CustomData

                                                        );

                if (CustomDiagnosticsStatusNotificationResponseParser is not null)
                    DiagnosticsStatusNotificationResponse = CustomDiagnosticsStatusNotificationResponseParser(JSON,
                                                                                                              DiagnosticsStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationResponse  = null;
                ErrorResponse                          = "The given JSON representation of a DiagnosticsStatusNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationResponse");

        #endregion

        #region ToJSON(CustomDiagnosticsStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDiagnosticsStatusNotificationResponseSerializer">A delegate to serialize custom DiagnosticsStatusNotification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationResponse>?  CustomDiagnosticsStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                              CustomSignatureSerializer                               = null,
                              CustomJObjectSerializerDelegate<CustomData>?                             CustomCustomDataSerializer                              = null)
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

            return CustomDiagnosticsStatusNotificationResponseSerializer is not null
                       ? CustomDiagnosticsStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The DiagnosticsStatusNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request.</param>
        public static DiagnosticsStatusNotificationResponse RequestError(DiagnosticsStatusNotificationRequest  Request,
                                                                         EventTracking_Id                      EventTrackingId,
                                                                         ResultCode                            ErrorCode,
                                                                         String?                               ErrorDescription    = null,
                                                                         JObject?                              ErrorDetails        = null,
                                                                         DateTimeOffset?                       ResponseTimestamp   = null,

                                                                         SourceRouting?                        Destination         = null,
                                                                         NetworkPath?                          NetworkPath         = null,

                                                                         IEnumerable<KeyPair>?                 SignKeys            = null,
                                                                         IEnumerable<SignInfo>?                SignInfos           = null,
                                                                         IEnumerable<Signature>?               Signatures          = null,

                                                                         CustomData?                           CustomData          = null)

            => new (

                   Request,
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
        /// The DiagnosticsStatusNotification failed.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DiagnosticsStatusNotificationResponse FormationViolation(DiagnosticsStatusNotificationRequest  Request,
                                                                               String                                ErrorDescription)

            => new (Request,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The DiagnosticsStatusNotification failed.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DiagnosticsStatusNotificationResponse SignatureError(DiagnosticsStatusNotificationRequest  Request,
                                                                           String                                ErrorDescription)

            => new (Request,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The DiagnosticsStatusNotification failed.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request.</param>
        /// <param name="Description">An optional error description.</param>
        public static DiagnosticsStatusNotificationResponse Failed(DiagnosticsStatusNotificationRequest  Request,
                                                                   String?                               Description   = null)

            => new (Request,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The DiagnosticsStatusNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The DiagnosticsStatusNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static DiagnosticsStatusNotificationResponse ExceptionOccurred(DiagnosticsStatusNotificationRequest  Request,
                                                                             Exception                             Exception)

            => new (Request,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A DiagnosticsStatusNotification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another DiagnosticsStatusNotification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse1,
                                           DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DiagnosticsStatusNotificationResponse1 is null || DiagnosticsStatusNotificationResponse2 is null)
                return false;

            return DiagnosticsStatusNotificationResponse1.Equals(DiagnosticsStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification responses for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A DiagnosticsStatusNotification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another DiagnosticsStatusNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse1,
                                           DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse2)

            => !(DiagnosticsStatusNotificationResponse1 == DiagnosticsStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DiagnosticsStatusNotificationResponse diagnosticsStatusNotificationResponse &&
                   Equals(diagnosticsStatusNotificationResponse);

        #endregion

        #region Equals(DiagnosticsStatusNotificationResponse)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse">A DiagnosticsStatusNotification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse)

            => DiagnosticsStatusNotificationResponse is not null;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "DiagnosticsStatusNotificationResponse";

        #endregion

    }

}
