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
    /// The RemoteStartTransaction request.
    /// </summary>
    public class RemoteStartTransactionRequest : ARequest<RemoteStartTransactionRequest>,
                                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/remoteStartTransactionRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification tag to start the charging transaction.
        /// </summary>
        public IdToken           IdTag              { get; }

        /// <summary>
        /// An optional connector identification on which the charging
        /// transaction should be started (SHALL be > 0).
        /// </summary>
        public Connector_Id?     ConnectorId        { get; }

        /// <summary>
        /// An optional charging profile to be used by the charge point
        /// for the requested charging transaction.
        /// The 'ChargingProfilePurpose' MUST be set to 'TxProfile'.
        /// </summary>
        public ChargingProfile?  ChargingProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RemoteStartTransaction request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
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
        public RemoteStartTransactionRequest(SourceRouting            Destination,
                                             IdToken                  IdTag,
                                             Connector_Id?            ConnectorId           = null,
                                             ChargingProfile?         ChargingProfile       = null,

                                             IEnumerable<KeyPair>?    SignKeys              = null,
                                             IEnumerable<SignInfo>?   SignInfos             = null,
                                             IEnumerable<Signature>?  Signatures            = null,

                                             CustomData?              CustomData            = null,

                                             Request_Id?              RequestId             = null,
                                             DateTimeOffset?          RequestTimestamp      = null,
                                             TimeSpan?                RequestTimeout        = null,
                                             EventTracking_Id?        EventTrackingId       = null,
                                             NetworkPath?             NetworkPath           = null,
                                             SerializationFormats?    SerializationFormat   = null,
                                             CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(RemoteStartTransactionRequest)[..^7],

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

            this.IdTag            = IdTag;
            this.ConnectorId      = ConnectorId;
            this.ChargingProfile  = ChargingProfile;

            unchecked
            {

                hashCode = this.IdTag.           GetHashCode()       * 7 ^
                          (this.ConnectorId?.    GetHashCode() ?? 0) * 5 ^
                          (this.ChargingProfile?.GetHashCode() ?? 0) * 3 ^
                           base.                 GetHashCode();

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
        //       <ns:remoteStartTransactionRequest>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <ns:idTag>?</ns:idTag>
        //
        //          <!--Optional:-->
        //          <ns:chargingProfile>
        //
        //             <ns:chargingProfileId>?</ns:chargingProfileId>
        //
        //             <!--Optional:-->
        //             <ns:transactionId>?</ns:transactionId>
        //
        //             <ns:stackLevel>?</ns:stackLevel>
        //             <ns:chargingProfilePurpose>?</ns:chargingProfilePurpose>
        //             <ns:chargingProfileKind>?</ns:chargingProfileKind>
        //
        //             <!--Optional:-->
        //             <ns:recurrencyKind>?</ns:recurrencyKind>
        //
        //             <!--Optional:-->
        //             <ns:validFrom>?</ns:validFrom>
        //
        //             <!--Optional:-->
        //             <ns:validTo>?</ns:validTo>
        //
        //             <ns:chargingSchedule>
        //
        //                <!--Optional:-->
        //                <ns:duration>?</ns:duration>
        //
        //                <!--Optional:-->
        //                <ns:startSchedule>?</ns:startSchedule>
        //                <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //                <!--1 or more repetitions:-->
        //                <ns:chargingSchedulePeriod>
        //
        //                   <ns:startPeriod>?</ns:startPeriod>
        //                   <ns:limit>?</ns:limit>
        //
        //                   <!--Optional:-->
        //                   <ns:numberPhases>?</ns:numberPhases>
        //
        //                </ns:chargingSchedulePeriod>
        //
        //                <!--Optional:-->
        //                <ns:minChargingRate>?</ns:minChargingRate>
        //
        //             </ns:chargingSchedule>
        //          </ns:chargingProfile>
        //
        //       </ns:remoteStartTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStartTransactionRequest",
        //     "title":   "RemoteStartTransactionRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "idTag": {
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "chargingProfile": {
        //             "type": "object",
        //             "properties": {
        //                 "chargingProfileId": {
        //                     "type": "integer"
        //                 },
        //                 "transactionId": {
        //                     "type": "integer"
        //                 },
        //                 "stackLevel": {
        //                     "type": "integer"
        //                 },
        //                 "chargingProfilePurpose": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "ChargePointMaxProfile",
        //                         "TxDefaultProfile",
        //                         "TxProfile"
        //                     ]
        //                 },
        //                 "chargingProfileKind": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Absolute",
        //                         "Recurring",
        //                         "Relative"
        //                     ]
        //                 },
        //                 "recurrencyKind": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Daily",
        //                         "Weekly"
        //                     ]
        //                 },
        //                 "validFrom": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "validTo": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "chargingSchedule": {
        //                     "type": "object",
        //                     "properties": {
        //                         "duration": {
        //                             "type": "integer"
        //                         },
        //                         "startSchedule": {
        //                             "type": "string",
        //                             "format": "date-time"
        //                         },
        //                         "chargingRateUnit": {
        //                             "type": "string",
        //                             "additionalProperties": false,
        //                             "enum": [
        //                                 "A",
        //                                 "W"
        //                             ]
        //                         },
        //                         "chargingSchedulePeriod": {
        //                             "type": "array",
        //                             "items": {
        //                                 "type": "object",
        //                                 "properties": {
        //                                     "startPeriod": {
        //                                         "type": "integer"
        //                                     },
        //                                     "limit": {
        //                                         "type": "number",
        //                                         "multipleOf" : 0.1
        //                                     },
        //                                     "numberPhases": {
        //                                         "type": "integer"
        //                                     }
        //                                 },
        //                                 "additionalProperties": false,
        //                                 "required": [
        //                                     "startPeriod",
        //                                     "limit"
        //                                 ]
        //                             }
        //                         },
        //                         "minChargingRate": {
        //                             "type": "number",
        //                             "multipleOf" : 0.1
        //                         }
        //                     },
        //                     "additionalProperties": false,
        //                     "required": [
        //                         "chargingRateUnit",
        //                         "chargingSchedulePeriod"
        //                     ]
        //                 }
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "chargingProfileId",
        //                 "stackLevel",
        //                 "chargingProfilePurpose",
        //                 "chargingProfileKind",
        //                 "chargingSchedule"
        //             ]
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
        /// Parse the given XML representation of a RemoteStartTransaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static RemoteStartTransactionRequest Parse(XElement       XML,
                                                          Request_Id     RequestId,
                                                          SourceRouting  Destination,
                                                          NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var remoteStartTransactionRequest,
                         out var errorResponse))
            {
                return remoteStartTransactionRequest;
            }

            throw new ArgumentException("The given XML representation of a RemoteStartTransaction request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a RemoteStartTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRemoteStartTransactionRequestParser">A delegate to parse custom RemoteStartTransaction requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static RemoteStartTransactionRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          SourceRouting                                                Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTimeOffset?                                              RequestTimestamp                            = null,
                                                          TimeSpan?                                                    RequestTimeout                              = null,
                                                          EventTracking_Id?                                            EventTrackingId                             = null,
                                                          CustomJObjectParserDelegate<RemoteStartTransactionRequest>?  CustomRemoteStartTransactionRequestParser   = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var remoteStartTransactionRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomRemoteStartTransactionRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return remoteStartTransactionRequest;
            }

            throw new ArgumentException("The given JSON representation of a RemoteStartTransaction request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out RemoteStartTransactionRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a RemoteStartTransaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RemoteStartTransactionRequest">The parsed RemoteStartTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                                 XML,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out RemoteStartTransactionRequest?  RemoteStartTransactionRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse)
        {

            try
            {

                RemoteStartTransactionRequest = new RemoteStartTransactionRequest(

                                                    Destination,

                                                    XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "idTag",
                                                                           IdToken.Parse),

                                                    XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                           Connector_Id.Parse),

                                                    XML.MapElement        (OCPPNS.OCPPv1_6_CP + "chargingProfile",
                                                                           ChargingProfile.Parse),

                                                    RequestId:    RequestId,
                                                    NetworkPath:  NetworkPath

                                                );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                RemoteStartTransactionRequest  = null;
                ErrorResponse                  = "The given XML representation of a RemoteStartTransaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out RemoteStartTransactionRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a RemoteStartTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RemoteStartTransactionRequest">The parsed RemoteStartTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRemoteStartTransactionRequestParser">A delegate to parse custom RemoteStartTransaction requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out RemoteStartTransactionRequest?      RemoteStartTransactionRequest,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTimeOffset?                                              RequestTimestamp                            = null,
                                       TimeSpan?                                                    RequestTimeout                              = null,
                                       EventTracking_Id?                                            EventTrackingId                             = null,
                                       CustomJObjectParserDelegate<RemoteStartTransactionRequest>?  CustomRemoteStartTransactionRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                RemoteStartTransactionRequest = null;

                #region IdTag              [mandatory]

                if (!JSON.ParseMandatory("idTag",
                                         "identification tag",
                                         IdToken.TryParse,
                                         out IdToken IdTag,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId        [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? ConnectorId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingProfile    [optional]

                if (JSON.ParseOptionalJSON("chargingProfile",
                                           "charging profile",
                                           OCPPv1_6.ChargingProfile.TryParse,
                                           out ChargingProfile ChargingProfile,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures         [optional, OCPP_CSE]

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

                #region CustomData         [optional]

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


                RemoteStartTransactionRequest = new RemoteStartTransactionRequest(

                                                    Destination,
                                                    IdTag,
                                                    ConnectorId,
                                                    ChargingProfile,

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

                if (CustomRemoteStartTransactionRequestParser is not null)
                    RemoteStartTransactionRequest = CustomRemoteStartTransactionRequestParser(JSON,
                                                                                              RemoteStartTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                RemoteStartTransactionRequest  = null;
                ErrorResponse                  = "The given JSON representation of a RemoteStartTransaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "remoteStartTransactionRequest",

                   ConnectorId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.Value.ToString())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "idTag",              IdTag.ToString()),

                   ChargingProfile?.ToXML()

               );

        #endregion

        #region ToJSON(CustomRemoteStartTransactionRequestSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStartTransactionRequestSerializer">A delegate to serialize custom RemoteStartTransaction requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStartTransactionRequest>?  CustomRemoteStartTransactionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?                CustomChargingProfileSerializer                 = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?               CustomChargingScheduleSerializer                = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?         CustomChargingSchedulePeriodSerializer          = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",       ConnectorId.Value.Value)
                               : null,

                                 new JProperty("idTag",             IdTag.          ToString()),

                           ChargingProfile is not null
                               ? new JProperty("chargingProfile",   ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                           CustomChargingScheduleSerializer,
                                                                                           CustomChargingSchedulePeriodSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRemoteStartTransactionRequestSerializer is not null
                       ? CustomRemoteStartTransactionRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStartTransactionRequest1, RemoteStartTransactionRequest2)

        /// <summary>
        /// Compares two RemoteStartTransaction requests for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest1">A RemoteStartTransaction request.</param>
        /// <param name="RemoteStartTransactionRequest2">Another RemoteStartTransaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStartTransactionRequest? RemoteStartTransactionRequest1,
                                           RemoteStartTransactionRequest? RemoteStartTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStartTransactionRequest1, RemoteStartTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (RemoteStartTransactionRequest1 is null || RemoteStartTransactionRequest2 is null)
                return false;

            return RemoteStartTransactionRequest1.Equals(RemoteStartTransactionRequest2);

        }

        #endregion

        #region Operator != (RemoteStartTransactionRequest1, RemoteStartTransactionRequest2)

        /// <summary>
        /// Compares two RemoteStartTransaction requests for inequality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest1">A RemoteStartTransaction request.</param>
        /// <param name="RemoteStartTransactionRequest2">Another RemoteStartTransaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStartTransactionRequest? RemoteStartTransactionRequest1,
                                           RemoteStartTransactionRequest? RemoteStartTransactionRequest2)

            => !(RemoteStartTransactionRequest1 == RemoteStartTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<RemoteStartTransactionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RemoteStartTransaction requests for equality.
        /// </summary>
        /// <param name="Object">A RemoteStartTransaction request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteStartTransactionRequest remoteStartTransactionRequest &&
                   Equals(remoteStartTransactionRequest);

        #endregion

        #region Equals(RemoteStartTransactionRequest)

        /// <summary>
        /// Compares two RemoteStartTransaction requests for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest">A RemoteStartTransaction request to compare with.</param>
        public override Boolean Equals(RemoteStartTransactionRequest? RemoteStartTransactionRequest)

            => RemoteStartTransactionRequest is not null &&

               IdTag.      Equals(RemoteStartTransactionRequest.IdTag) &&

            ((!ConnectorId.HasValue       && !RemoteStartTransactionRequest.ConnectorId.HasValue) ||
              (ConnectorId.HasValue       &&  RemoteStartTransactionRequest.ConnectorId.HasValue        && ConnectorId.Value.Equals(RemoteStartTransactionRequest.ConnectorId.Value))) &&

             ((ChargingProfile is     null && RemoteStartTransactionRequest. ChargingProfile is     null) ||
              (ChargingProfile is not null && RemoteStartTransactionRequest. ChargingProfile is not null && ChargingProfile.  Equals(RemoteStartTransactionRequest.ChargingProfile)))  &&

               base.GenericEquals(RemoteStartTransactionRequest);

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

            => String.Concat(IdTag,

                             ConnectorId.HasValue
                                 ? " at " + IdTag : "",

                             ChargingProfile is not null
                                 ? " with profile"
                                 : "");

        #endregion

    }

}
