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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The ReserveNow request.
    /// </summary>
    public class ReserveNowRequest : ARequest<ReserveNowRequest>,
                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/reserveNowRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of the connector to be reserved.
        /// A value of 0 means that the reservation is not for
        /// a specific connector.
        /// </summary>
        [Mandatory]
        public Connector_Id    ConnectorId      { get; }

        /// <summary>
        /// The unique identification of this reservation.
        /// </summary>
        [Mandatory]
        public Reservation_Id  ReservationId    { get; }

        /// <summary>
        /// The timestamp when the reservation ends.
        /// </summary>
        [Mandatory]
        public DateTime        ExpiryDate       { get; }

        /// <summary>
        /// The unique token identification for which the reservation is being made.
        /// </summary>
        [Mandatory]
        public IdToken         IdTag            { get; }

        /// <summary>
        /// An optional parent idTag.
        /// </summary>
        [Optional]
        public IdToken?        ParentIdTag      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ReserveNow request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The unique token identification for which the reservation is being made.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
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
        public ReserveNowRequest(SourceRouting            Destination,
                                 Connector_Id             ConnectorId,
                                 Reservation_Id           ReservationId,
                                 DateTime                 ExpiryDate,
                                 IdToken                  IdTag,
                                 IdToken?                 ParentIdTag           = null,

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
                   nameof(ReserveNowRequest)[..^7],

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

            this.ConnectorId    = ConnectorId;
            this.ReservationId  = ReservationId;
            this.ExpiryDate     = ExpiryDate;
            this.IdTag          = IdTag;
            this.ParentIdTag    = ParentIdTag;

            unchecked
            {

                hashCode = this.ReservationId.GetHashCode()       * 13 ^
                           this.ConnectorId.  GetHashCode()       * 11 ^
                           this.ExpiryDate.   GetHashCode()       *  7 ^
                           this.IdTag.        GetHashCode()       *  5 ^
                          (this.ParentIdTag?. GetHashCode() ?? 0) *  3 ^
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
        //       <ns:reserveNowRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:expiryDate>?</ns:expiryDate>
        //          <ns:idTag>?</ns:idTag>
        //
        //          <!--Optional:-->
        //          <ns:parentIdTag>?</ns:parentIdTag>
        //
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:reserveNowRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ReserveNowRequest",
        //     "title":   "ReserveNowRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "expiryDate": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "idTag": {
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "parentIdTag": {
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "reservationId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "expiryDate",
        //         "idTag",
        //         "reservationId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a ReserveNow request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static ReserveNowRequest Parse(XElement       XML,
                                              Request_Id     RequestId,
                                              SourceRouting  Destination,
                                              NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var reserveNowRequest,
                         out var errorResponse))
            {
                return reserveNowRequest;
            }

            throw new ArgumentException("The given XML representation of a ReserveNow response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ReserveNow request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReserveNowRequestParser">A delegate to parse custom ReserveNow requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static ReserveNowRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              SourceRouting                                    Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        RequestTimestamp                = null,
                                              TimeSpan?                                        RequestTimeout                  = null,
                                              EventTracking_Id?                                EventTrackingId                 = null,
                                              CustomJObjectParserDelegate<ReserveNowRequest>?  CustomReserveNowRequestParser   = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var reserveNowRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomReserveNowRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return reserveNowRequest;
            }

            throw new ArgumentException("The given JSON representation of a ReserveNow response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out ReserveNowRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a ReserveNow request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ReserveNowRequest">The parsed ReserveNow request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                     XML,
                                       Request_Id                                   RequestId,
                                       SourceRouting                                Destination,
                                       NetworkPath                                  NetworkPath,
                                       [NotNullWhen(true)]  out ReserveNowRequest?  ReserveNowRequest,
                                       [NotNullWhen(false)] out String?             ErrorResponse)
        {

            try
            {

                ReserveNowRequest = new ReserveNowRequest(

                                        Destination,

                                        XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                               Connector_Id.Parse),

                                        XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "reservationId",
                                                               Reservation_Id.Parse),

                                        XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "expiryDate",
                                                               DateTime.Parse),

                                        XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "idTag",
                                                               IdToken.Parse),

                                        XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "parentIdTag",
                                                               IdToken.Parse),

                                        RequestId:    RequestId,
                                        NetworkPath:  NetworkPath

                                    );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ReserveNowRequest  = null;
                ErrorResponse      = "The given XML representation of a ReserveNow request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ReserveNowRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ReserveNow request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ReserveNowRequest">The parsed ReserveNow request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReserveNowRequestParser">A delegate to parse custom ReserveNow requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out ReserveNowRequest?      ReserveNowRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        RequestTimestamp                = null,
                                       TimeSpan?                                        RequestTimeout                  = null,
                                       EventTracking_Id?                                EventTrackingId                 = null,
                                       CustomJObjectParserDelegate<ReserveNowRequest>?  CustomReserveNowRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            try
            {

                ReserveNowRequest = null;

                #region ConnectorId      [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

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

                #region ExpiryDate       [mandatory]

                if (!JSON.ParseMandatory("expiryDate",
                                         "expiry date",
                                         out DateTime ExpiryDate,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTag            [mandatory]

                if (!JSON.ParseMandatory("idTag",
                                         "identification tag",
                                         IdToken.TryParse,
                                         out IdToken IdTag,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ParentIdTag      [optional]

                if (JSON.ParseOptional("parentIdTag",
                                       "parent identification tag",
                                       IdToken.TryParse,
                                       out IdToken ParentIdTag,
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


                ReserveNowRequest = new ReserveNowRequest(

                                        Destination,
                                        ConnectorId,
                                        ReservationId,
                                        ExpiryDate,
                                        IdTag,
                                        ParentIdTag,

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

                if (CustomReserveNowRequestParser is not null)
                    ReserveNowRequest = CustomReserveNowRequestParser(JSON,
                                                                      ReserveNowRequest);

                return true;

            }
            catch (Exception e)
            {
                ReserveNowRequest  = null;
                ErrorResponse      = "The given JSON representation of a ReserveNow request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "reserveNowRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",        ConnectorId.      ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "expiryDate",         ExpiryDate.       ToISO8601()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "idTag",              IdTag.            ToString()),

                   ParentIdTag.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "parentIdTag",  ParentIdTag.Value.ToString())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "reservationId",      ReservationId.    ToString())

               );

        #endregion

        #region ToJSON(CustomReserveNowRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReserveNowRequestSerializer">A delegate to serialize custom ReserveNow requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReserveNowRequest>?  CustomReserveNowRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("connectorId",     ConnectorId.Value),
                                 new JProperty("expiryDate",      ExpiryDate.       ToISO8601()),
                                 new JProperty("idTag",           IdTag.            ToString()),
                                 new JProperty("reservationId",   ReservationId.Value),

                           ParentIdTag.HasValue
                               ? new JProperty("parentIdTag",     ParentIdTag.Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomReserveNowRequestSerializer is not null
                       ? CustomReserveNowRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two ReserveNow requests for equality.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReserveNowRequest? ReserveNowRequest1,
                                           ReserveNowRequest? ReserveNowRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReserveNowRequest1, ReserveNowRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ReserveNowRequest1 is null || ReserveNowRequest2 is null)
                return false;

            return ReserveNowRequest1.Equals(ReserveNowRequest2);

        }

        #endregion

        #region Operator != (ReserveNowRequest1, ReserveNowRequest2)

        /// <summary>
        /// Compares two ReserveNow requests for inequality.
        /// </summary>
        /// <param name="ReserveNowRequest1">A ReserveNow request.</param>
        /// <param name="ReserveNowRequest2">Another ReserveNow request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReserveNowRequest? ReserveNowRequest1,
                                           ReserveNowRequest? ReserveNowRequest2)

            => !(ReserveNowRequest1 == ReserveNowRequest2);

        #endregion

        #endregion

        #region IEquatable<ReserveNowRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ReserveNow requests for equality.
        /// </summary>
        /// <param name="Object">A ReserveNow request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReserveNowRequest reserveNowRequest &&
                   Equals(reserveNowRequest);

        #endregion

        #region Equals(ReserveNowRequest)

        /// <summary>
        /// Compares two ReserveNow requests for equality.
        /// </summary>
        /// <param name="ReserveNowRequest">A ReserveNow request to compare with.</param>
        public override Boolean Equals(ReserveNowRequest? ReserveNowRequest)

            => ReserveNowRequest is not null &&

               ReservationId.Equals(ReserveNowRequest.ReservationId) &&
               ConnectorId.  Equals(ReserveNowRequest.ConnectorId)   &&
               ExpiryDate.   Equals(ReserveNowRequest.ExpiryDate)    &&
               IdTag.        Equals(ReserveNowRequest.IdTag)         &&

            ((!ParentIdTag.HasValue && !ReserveNowRequest.ParentIdTag.HasValue) ||
              (ParentIdTag.HasValue &&  ReserveNowRequest.ParentIdTag.HasValue && ParentIdTag.Equals(ReserveNowRequest.ParentIdTag))) &&

               base.  GenericEquals(ReserveNowRequest);

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

            => String.Concat(

                   $"{ConnectorId} / {ExpiryDate.ToISO8601()} / {IdTag}",

                   ParentIdTag.HasValue
                       ? $"/{ParentIdTag.Value}"
                       : "",

                   $" ({ReservationId})"

               );

        #endregion

    }

}
