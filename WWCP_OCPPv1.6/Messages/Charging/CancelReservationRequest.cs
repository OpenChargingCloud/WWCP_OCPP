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
    /// The CancelReservation request.
    /// </summary>
    public class CancelReservationRequest : ARequest<CancelReservationRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/cancelReservationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the reservation to cancel.
        /// </summary>
        public Reservation_Id  ReservationId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a CancelReservation request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public CancelReservationRequest(SourceRouting            Destination,
                                        Reservation_Id           ReservationId,

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
                   nameof(CancelReservationRequest)[..^7],

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

            this.ReservationId = ReservationId;

            unchecked
            {

                hashCode = this.ReservationId.GetHashCode() * 3 ^
                           base.              GetHashCode();

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
        //       <ns:cancelReservationRequest>
        //
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:cancelReservationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:CancelReservationRequest",
        //     "title":   "CancelReservationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "reservationId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "reservationId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a CancelReservation request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static CancelReservationRequest Parse(XElement       XML,
                                                     Request_Id     RequestId,
                                                     SourceRouting  Destination,
                                                     NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var cancelReservationRequest,
                         out var errorResponse))
            {
                return cancelReservationRequest;
            }

            throw new ArgumentException("The given XMLs representation of a CancelReservation request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a CancelReservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomCancelReservationRequestParser">A delegate to parse custom CancelReservation requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static CancelReservationRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               RequestTimestamp                       = null,
                                                     TimeSpan?                                               RequestTimeout                         = null,
                                                     EventTracking_Id?                                       EventTrackingId                        = null,
                                                     CustomJObjectParserDelegate<CancelReservationRequest>?  CustomCancelReservationRequestParser   = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var cancelReservationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomCancelReservationRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return cancelReservationRequest;
            }

            throw new ArgumentException("The given JSON representation of a CancelReservation request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out CancelReservationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a CancelReservation request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancelReservationRequest">The parsed CancelReservation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                            XML,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out CancelReservationRequest?  CancelReservationRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse)
        {

            try
            {

                CancelReservationRequest = new CancelReservationRequest(

                                               Destination,

                                               XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "reservationId",
                                                                  Reservation_Id.Parse),

                                               RequestId:    RequestId,
                                               NetworkPath:  NetworkPath

                                           );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                CancelReservationRequest  = null;
                ErrorResponse             = "The given XML representation of a CancelReservation request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out CancelReservationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a CancelReservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancelReservationRequest">The parsed CancelReservation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomCancelReservationRequestParser">A delegate to parse custom CancelReservation requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out CancelReservationRequest?      CancelReservationRequest,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               RequestTimestamp                       = null,
                                       TimeSpan?                                               RequestTimeout                         = null,
                                       EventTracking_Id?                                       EventTrackingId                        = null,
                                       CustomJObjectParserDelegate<CancelReservationRequest>?  CustomCancelReservationRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                CancelReservationRequest = null;

                #region ReservationId    [mandatory]

                if (!JSON.ParseMandatory("reservationId",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id ReservationId,
                                         out ErrorResponse))
                {
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


                CancelReservationRequest = new CancelReservationRequest(

                                               Destination,
                                               ReservationId,

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

                if (CustomCancelReservationRequestParser is not null)
                    CancelReservationRequest = CustomCancelReservationRequestParser(JSON,
                                                                                    CancelReservationRequest);

                return true;

            }
            catch (Exception e)
            {
                CancelReservationRequest  = null;
                ErrorResponse             = "The given JSON representation of a CancelReservation request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "cancelReservationRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "reservationId",  ReservationId.ToString())

               );

        #endregion

        #region ToJSON(CustomCancelReservationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationRequestSerializer">A delegate to serialize custom CancelReservation requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationRequest>?  CustomCancelReservationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("reservationId",   ReservationId.Value),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCancelReservationRequestSerializer is not null
                       ? CustomCancelReservationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two CancelReservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A CancelReservation request.</param>
        /// <param name="CancelReservationRequest2">Another CancelReservation request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationRequest? CancelReservationRequest1,
                                           CancelReservationRequest? CancelReservationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CancelReservationRequest1, CancelReservationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (CancelReservationRequest1 is null || CancelReservationRequest2 is null)
                return false;

            return CancelReservationRequest1.Equals(CancelReservationRequest2);

        }

        #endregion

        #region Operator != (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two CancelReservation requests for inequality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A CancelReservation request.</param>
        /// <param name="CancelReservationRequest2">Another CancelReservation request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationRequest? CancelReservationRequest1,
                                           CancelReservationRequest? CancelReservationRequest2)

            => !(CancelReservationRequest1 == CancelReservationRequest2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CancelReservation requests for equality.
        /// </summary>
        /// <param name="Object">A CancelReservation request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CancelReservationRequest cancelReservationRequest &&
                   Equals(cancelReservationRequest);

        #endregion

        #region Equals(CancelReservationRequest)

        /// <summary>
        /// Compares two CancelReservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest">A CancelReservation request to compare with.</param>
        public override Boolean Equals(CancelReservationRequest? CancelReservationRequest)

            => CancelReservationRequest is not null &&

               ReservationId.Equals(CancelReservationRequest.ReservationId) &&

               base.  GenericEquals(CancelReservationRequest);

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

            => ReservationId.ToString();

        #endregion

    }

}
