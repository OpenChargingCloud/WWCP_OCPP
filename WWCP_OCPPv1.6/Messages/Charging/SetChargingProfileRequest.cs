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
    /// The SetChargingProfile request.
    /// </summary>
    public class SetChargingProfileRequest : ARequest<SetChargingProfileRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/setChargingProfileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The connector to which the charging profile applies.
        /// If connectorId = 0, the message contains an overall
        /// limit for the charge point.
        /// </summary>
        public Connector_Id     ConnectorId        { get; }

        /// <summary>
        /// The charging profile to be set.
        /// </summary>
        public ChargingProfile  ChargingProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetChargingProfile request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
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
        public SetChargingProfileRequest(SourceRouting            Destination,
                                         Connector_Id             ConnectorId,
                                         ChargingProfile          ChargingProfile,

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
                   nameof(SetChargingProfileRequest)[..^7],

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

            this.ConnectorId      = ConnectorId;
            this.ChargingProfile  = ChargingProfile ?? throw new ArgumentNullException(nameof(ChargingProfile),  "The given charging profile must not be null!");

            unchecked
            {

                hashCode = this.ConnectorId.    GetHashCode() * 5 ^
                           this.ChargingProfile.GetHashCode() * 3 ^
                           base.                GetHashCode();

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
        //       <ns:setChargingProfileRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <ns:csChargingProfiles>
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
        //
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
        //
        //          </ns:csChargingProfiles>
        //
        //       </ns:setChargingProfileRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:SetChargingProfileRequest",
        //     "title":   "SetChargingProfileRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "csChargingProfiles": {
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
        //                                 "limit": {
        //                                     "type": "number",
        //                                     "multipleOf" : 0.1
        //                                 },
        //                                 "numberPhases": {
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
        //         "connectorId",
        //         "csChargingProfiles"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static SetChargingProfileRequest Parse(XElement       XML,
                                                      Request_Id     RequestId,
                                                      SourceRouting  Destination,
                                                      NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setChargingProfileRequest,
                         out var errorResponse))
            {
                return setChargingProfileRequest;
            }

            throw new ArgumentException("The given XML representation of a SetChargingProfile request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetChargingProfileRequestParser">A delegate to parse custom SetChargingProfile requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static SetChargingProfileRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<SetChargingProfileRequest>?  CustomSetChargingProfileRequestParser   = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setChargingProfileRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetChargingProfileRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetChargingProfile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out SetChargingProfileRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetChargingProfileRequest">The parsed SetChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                             XML,
                                       Request_Id                                           RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out SetChargingProfileRequest?  SetChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse)
        {

            try
            {

                SetChargingProfileRequest = new SetChargingProfileRequest(

                                                Destination,

                                                XML.MapValueOrFail   (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                      Connector_Id.Parse),

                                                XML.MapElementOrFail2(OCPPNS.OCPPv1_6_CP + "csChargingProfiles",
                                                                      ChargingProfile.Parse),

                                                RequestId:    RequestId,
                                                NetworkPath:  NetworkPath

                                            );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileRequest  = null;
                ErrorResponse              = "The given XML representation of a SetChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetChargingProfileRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetChargingProfileRequest">The parsed SetChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetChargingProfileRequestParser">A delegate to parse custom SetChargingProfile requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out SetChargingProfileRequest?      SetChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<SetChargingProfileRequest>?  CustomSetChargingProfileRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                SetChargingProfileRequest = null;

                #region ConnectorId        [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfile    [mandatory]

                if (!JSON.ParseMandatoryJSON("csChargingProfiles",
                                             "charging station charging profiles",
                                             OCPPv1_6.ChargingProfile.TryParse,
                                             out ChargingProfile? ChargingProfile,
                                             out ErrorResponse) ||
                     ChargingProfile is null)
                {
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


                SetChargingProfileRequest = new SetChargingProfileRequest(

                                                Destination,
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

                if (CustomSetChargingProfileRequestParser is not null)
                    SetChargingProfileRequest = CustomSetChargingProfileRequestParser(JSON,
                                                                                      SetChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileRequest  = null;
                ErrorResponse              = "The given JSON representation of a SetChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "setChargingProfileRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString()),
                   ChargingProfile.ToXML(OCPPNS.OCPPv1_6_CP + "csChargingProfiles")

               );

        #endregion

        #region ToJSON(CustomSetChargingProfileRequestSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfileRequest>?  CustomSetChargingProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?            CustomChargingProfileSerializer             = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?           CustomChargingScheduleSerializer            = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?     CustomChargingSchedulePeriodSerializer      = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("connectorId",          ConnectorId.    Value),

                                 new JProperty("csChargingProfiles",   ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                              CustomChargingScheduleSerializer,
                                                                                              CustomChargingSchedulePeriodSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetChargingProfileRequestSerializer is not null
                       ? CustomSetChargingProfileRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A SetChargingProfile request.</param>
        /// <param name="SetChargingProfileRequest2">Another SetChargingProfile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetChargingProfileRequest? SetChargingProfileRequest1,
                                           SetChargingProfileRequest? SetChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetChargingProfileRequest1, SetChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetChargingProfileRequest1 is null || SetChargingProfileRequest2 is null)
                return false;

            return SetChargingProfileRequest1.Equals(SetChargingProfileRequest2);

        }

        #endregion

        #region Operator != (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two SetChargingProfile requests for inequality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A SetChargingProfile request.</param>
        /// <param name="SetChargingProfileRequest2">Another SetChargingProfile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetChargingProfileRequest? SetChargingProfileRequest1,
                                           SetChargingProfileRequest? SetChargingProfileRequest2)

            => !(SetChargingProfileRequest1 == SetChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="Object">A SetChargingProfile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetChargingProfileRequest setChargingProfileRequest &&
                   Equals(setChargingProfileRequest);

        #endregion

        #region Equals(SetChargingProfileRequest)

        /// <summary>
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest">A SetChargingProfile request to compare with.</param>
        public override Boolean Equals(SetChargingProfileRequest? SetChargingProfileRequest)

            => SetChargingProfileRequest is not null &&

               ConnectorId.    Equals(SetChargingProfileRequest.ConnectorId)     &&
               ChargingProfile.Equals(SetChargingProfileRequest.ChargingProfile) &&

               base.    GenericEquals(SetChargingProfileRequest);

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

            => $"Set charging profile '{ChargingProfile.ChargingProfileId}' on connector '{ConnectorId}'";

        #endregion

    }

}
