﻿/*
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
    /// The ClearChargingProfile request.
    /// </summary>
    public class ClearChargingProfileRequest : ARequest<ClearChargingProfileRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/clearChargingProfileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional identification of the charging profile to clear.
        /// </summary>
        public ChargingProfile_Id?       ChargingProfileId         { get; }

        /// <summary>
        /// The optional connector for which to clear the charging profiles.
        /// Connector identification 0 specifies the charging profile for the
        /// overall charge point. Absence of this parameter means the clearing
        /// applies to all charging profiles that match the other criteria in
        /// the request.
        /// </summary>
        public Connector_Id?             ConnectorId               { get; }

        /// <summary>
        /// The optional purpose of the charging profiles that will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public ChargingProfilePurposes?  ChargingProfilePurpose    { get; }

        /// <summary>
        /// The optional stack level for which charging profiles will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public UInt32?                   StackLevel                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearChargingProfile request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
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
        public ClearChargingProfileRequest(SourceRouting             Destination,
                                           ChargingProfile_Id?       ChargingProfileId        = null,
                                           Connector_Id?             ConnectorId              = null,
                                           ChargingProfilePurposes?  ChargingProfilePurpose   = null,
                                           UInt32?                   StackLevel               = null,

                                           IEnumerable<KeyPair>?     SignKeys                 = null,
                                           IEnumerable<SignInfo>?    SignInfos                = null,
                                           IEnumerable<Signature>?   Signatures               = null,

                                           CustomData?               CustomData               = null,

                                           Request_Id?               RequestId                = null,
                                           DateTime?                 RequestTimestamp         = null,
                                           TimeSpan?                 RequestTimeout           = null,
                                           EventTracking_Id?         EventTrackingId          = null,
                                           NetworkPath?              NetworkPath              = null,
                                           SerializationFormats?     SerializationFormat      = null,
                                           CancellationToken         CancellationToken        = default)

            : base(Destination,
                   nameof(ClearChargingProfileRequest)[..^7],

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

            this.ChargingProfileId       = ChargingProfileId;
            this.ConnectorId             = ConnectorId;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.StackLevel              = StackLevel;

            unchecked
            {

                hashCode = (this.ChargingProfileId?.     GetHashCode() ?? 0) * 11 ^
                           (this.ConnectorId?.           GetHashCode() ?? 0) *  7 ^
                           (this.ChargingProfilePurpose?.GetHashCode() ?? 0) *  5 ^
                           (this.StackLevel?.            GetHashCode() ?? 0) *  3 ^
                            base.                        GetHashCode();

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
        //       <ns:clearChargingProfileRequest>
        //
        //          <!--Optional:-->
        //          <ns:id>?</ns:id>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <!--Optional:-->
        //          <ns:chargingProfilePurpose>?</ns:chargingProfilePurpose>
        //
        //          <!--Optional:-->
        //          <ns:stackLevel>?</ns:stackLevel>
        //
        //       </ns:clearChargingProfileRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ClearChargingProfileRequest",
        //     "title":   "ClearChargingProfileRequest",
        //     "type":    "object",
        //     "properties": {
        //         "id": {
        //             "type": "integer"
        //         },
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "chargingProfilePurpose": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ChargePointMaxProfile",
        //                 "TxDefaultProfile",
        //                 "TxProfile"
        //             ]
        //         },
        //         "stackLevel": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static ClearChargingProfileRequest Parse(XElement       XML,
                                                        Request_Id     RequestId,
                                                        SourceRouting  Destination,
                                                        NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearChargingProfileRequest,
                         out var errorResponse))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given XML representation of a ClearChargingProfile request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static ClearChargingProfileRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        SourceRouting                                              Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  RequestTimestamp                          = null,
                                                        TimeSpan?                                                  RequestTimeout                            = null,
                                                        EventTracking_Id?                                          EventTrackingId                           = null,
                                                        CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser   = null,
                                                        CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                                        CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearChargingProfileRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomClearChargingProfileRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearChargingProfile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out ClearChargingProfileRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                               XML,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out ClearChargingProfileRequest?  ClearChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse)
        {

            try
            {

                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  Destination,

                                                  XML.MapValueOrNull    (OCPPNS.OCPPv1_6_CP + "id",
                                                                         ChargingProfile_Id.Parse),

                                                  XML.MapValueOrNull    (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                         Connector_Id.Parse),

                                                  XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",
                                                                         ChargingProfilePurposesExtensions.Parse),

                                                  XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "stackLevel",
                                                                         UInt32.Parse),

                                                  RequestId:    RequestId,
                                                  NetworkPath:  NetworkPath

                                              );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileRequest  = null;
                ErrorResponse                = "The given XML representation of a ClearChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ClearChargingProfileRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       SourceRouting                                              Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out ClearChargingProfileRequest?      ClearChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  RequestTimestamp                          = null,
                                       TimeSpan?                                                  RequestTimeout                            = null,
                                       EventTracking_Id?                                          EventTrackingId                           = null,
                                       CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                       CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            try
            {

                ClearChargingProfileRequest = null;

                #region ChargingProfileId         [optional]

                if (JSON.ParseOptional("chargingProfileId",
                                       "charging profile identification",
                                       ChargingProfile_Id.TryParse,
                                       out ChargingProfile_Id? ChargingProfileId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ConnectorId               [optional]

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

                #region ChargingProfilePurpose    [optional]

                if (JSON.ParseOptional("chargingProfilePurpose",
                                       "charging profile purpose",
                                       ChargingProfilePurposesExtensions.TryParse,
                                       out ChargingProfilePurposes? ChargingProfilePurpose,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StackLevel                [optional]

                if (JSON.ParseOptional("stackLevel",
                                       "stack level",
                                       out UInt32? StackLevel,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

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


                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  Destination,
                                                  ChargingProfileId,
                                                  ConnectorId,
                                                  ChargingProfilePurpose,
                                                  StackLevel,

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

                if (CustomClearChargingProfileRequestParser is not null)
                    ClearChargingProfileRequest = CustomClearChargingProfileRequestParser(JSON,
                                                                                          ClearChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileRequest  = null;
                ErrorResponse                = "The given JSON representation of a ClearChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "clearChargingProfileRequest",

                   ChargingProfileId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "id",                      ChargingProfileId.ToString())
                       : null,

                   ConnectorId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",             ConnectorId.      ToString())
                       : null,

                   ChargingProfilePurpose.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",  ChargingProfilePurpose.Value.AsText())
                       : null,

                   StackLevel.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "stackLevel",              StackLevel.Value)
                       : null

               );

        #endregion

        #region ToJSON(CustomClearChargingProfileRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileRequestSerializer">A delegate to serialize custom ClearChargingProfile requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           ChargingProfileId.HasValue
                               ? new JProperty("id",                       ChargingProfileId.     Value.Value)
                               : null,

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",              ConnectorId.           Value.Value)
                               : null,

                           ChargingProfilePurpose.HasValue
                               ? new JProperty("chargingProfilePurpose",   ChargingProfilePurpose.Value.ToString())
                               : null,

                           StackLevel.HasValue
                               ? new JProperty("stackLevel",               StackLevel.            Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",               new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                      CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.                  ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearChargingProfileRequestSerializer is not null
                       ? CustomClearChargingProfileRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileRequest? ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest? ClearChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfileRequest1, ClearChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearChargingProfileRequest1 is null || ClearChargingProfileRequest2 is null)
                return false;

            return ClearChargingProfileRequest1.Equals(ClearChargingProfileRequest2);

        }

        #endregion

        #region Operator != (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two ClearChargingProfile requests for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileRequest? ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest? ClearChargingProfileRequest2)

            => !(ClearChargingProfileRequest1 == ClearChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="Object">A ClearChargingProfile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileRequest clearChargingProfileRequest &&
                   Equals(clearChargingProfileRequest);

        #endregion

        #region Equals(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A ClearChargingProfile request to compare with.</param>
        public override Boolean Equals(ClearChargingProfileRequest? ClearChargingProfileRequest)

            => ClearChargingProfileRequest is not null &&

            ((!ChargingProfileId.     HasValue && !ClearChargingProfileRequest.ChargingProfileId.HasValue)      ||
              (ChargingProfileId.     HasValue &&  ClearChargingProfileRequest.ChargingProfileId.HasValue      && ChargingProfileId.           Equals(ClearChargingProfileRequest.ChargingProfileId)))            &&

            ((!ConnectorId.           HasValue && !ClearChargingProfileRequest.ConnectorId.HasValue)            ||
              (ConnectorId.           HasValue &&  ClearChargingProfileRequest.ConnectorId.HasValue            && ConnectorId.                 Equals(ClearChargingProfileRequest.ConnectorId)))                  &&

            ((!ChargingProfilePurpose.HasValue && !ClearChargingProfileRequest.ChargingProfilePurpose.HasValue) ||
              (ChargingProfilePurpose.HasValue &&  ClearChargingProfileRequest.ChargingProfilePurpose.HasValue && ChargingProfilePurpose.Value.Equals(ClearChargingProfileRequest.ChargingProfilePurpose.Value))) &&

            ((!StackLevel.            HasValue && !ClearChargingProfileRequest.StackLevel.            HasValue) ||
              (StackLevel.            HasValue &&  ClearChargingProfileRequest.StackLevel.            HasValue && StackLevel.            Value.Equals(ClearChargingProfileRequest.StackLevel.Value)))             &&

               base.GenericEquals(ClearChargingProfileRequest);

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

                   ChargingProfileId.HasValue
                       ? ChargingProfileId.ToString()
                       : "",

                   ConnectorId.HasValue
                       ? $" at {ConnectorId}"
                       : "",

                   ChargingProfilePurpose.HasValue
                       ? $" having {ChargingProfilePurpose.Value}"
                       : "",

                   StackLevel.HasValue
                       ? $" at {StackLevel.Value}"
                       : ""

               );

        #endregion

    }

}
