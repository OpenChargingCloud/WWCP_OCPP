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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The clear charging profile request.
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
        /// Create a new clear charging profile request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
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
        public ClearChargingProfileRequest(NetworkingNode_Id             NetworkingNodeId,
                                           ChargingProfile_Id?           ChargingProfileId        = null,
                                           Connector_Id?                 ConnectorId              = null,
                                           ChargingProfilePurposes?      ChargingProfilePurpose   = null,
                                           UInt32?                       StackLevel               = null,

                                           IEnumerable<KeyPair>?         SignKeys                 = null,
                                           IEnumerable<SignInfo>?        SignInfos                = null,
                                           IEnumerable<Signature>?       Signatures               = null,

                                           CustomData?                   CustomData               = null,

                                           Request_Id?                   RequestId                = null,
                                           DateTime?                     RequestTimestamp         = null,
                                           TimeSpan?                     RequestTimeout           = null,
                                           EventTracking_Id?             EventTrackingId          = null,
                                           NetworkPath?                  NetworkPath              = null,
                                           CancellationToken             CancellationToken        = default)

            : base(NetworkingNodeId,
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
                   CancellationToken)

        {

            this.ChargingProfileId       = ChargingProfileId;
            this.ConnectorId             = ConnectorId;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.StackLevel              = StackLevel;

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

        #region (static) Parse   (XML,  RequestId, NetworkingNodeId, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a clear charging profile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static ClearChargingProfileRequest Parse(XElement           XML,
                                                        Request_Id         RequestId,
                                                        NetworkingNode_Id  NetworkingNodeId,
                                                        NetworkPath        NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var clearChargingProfileRequest,
                         out var errorResponse) &&
                clearChargingProfileRequest is not null)
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given XML representation of a clear charging profile request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomClearChargingProfileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear charging profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomClearChargingProfileRequestParser">An optional delegate to parse custom clear charging profile requests.</param>
        public static ClearChargingProfileRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        NetworkingNode_Id                                          NetworkingNodeId,
                                                        NetworkPath                                                NetworkPath,
                                                        CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var clearChargingProfileRequest,
                         out var errorResponse,
                         CustomClearChargingProfileRequestParser) &&
                clearChargingProfileRequest is not null)
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a clear charging profile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, NetworkingNodeId, NetworkPath, out ClearChargingProfileRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a clear charging profile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                          XML,
                                       Request_Id                        RequestId,
                                       NetworkingNode_Id                 NetworkingNodeId,
                                       NetworkPath                       NetworkPath,
                                       out ClearChargingProfileRequest?  ClearChargingProfileRequest,
                                       out String?                       ErrorResponse)
        {

            try
            {

                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  NetworkingNodeId,

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
                ErrorResponse                = "The given XML representation of a clear charging profile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out ClearChargingProfileRequest, out ErrorResponse, CustomClearChargingProfileRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       Request_Id                        RequestId,
                                       NetworkingNode_Id                 NetworkingNodeId,
                                       NetworkPath                       NetworkPath,
                                       out ClearChargingProfileRequest?  ClearChargingProfileRequest,
                                       out String?                       ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out ClearChargingProfileRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the destination charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileRequestParser">An optional delegate to parse custom clear charging profile requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       NetworkingNode_Id                                          NetworkingNodeId,
                                       NetworkPath                                                NetworkPath,
                                       out ClearChargingProfileRequest?                           ClearChargingProfileRequest,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser)
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  NetworkingNodeId,
                                                  ChargingProfileId,
                                                  ConnectorId,
                                                  ChargingProfilePurpose,
                                                  StackLevel,

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

                if (CustomClearChargingProfileRequestParser is not null)
                    ClearChargingProfileRequest = CustomClearChargingProfileRequestParser(JSON,
                                                                                          ClearChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileRequest  = null;
                ErrorResponse                = "The given JSON representation of a clear charging profile request is invalid: " + e.Message;
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
        /// <param name="CustomClearChargingProfileRequestSerializer">A delegate to serialize custom clear charging profile requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?               CustomSignatureSerializer                     = null,
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
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A clear charging profile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another clear charging profile request.</param>
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
        /// Compares two clear charging profile requests for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A clear charging profile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another clear charging profile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileRequest? ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest? ClearChargingProfileRequest2)

            => !(ClearChargingProfileRequest1 == ClearChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="Object">A clear charging profile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileRequest clearChargingProfileRequest &&
                   Equals(clearChargingProfileRequest);

        #endregion

        #region Equals(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A clear charging profile request to compare with.</param>
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

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return (ChargingProfileId?.     GetHashCode() ?? 0) * 11 ^
                       (ConnectorId?.           GetHashCode() ?? 0) *  7 ^
                       (ChargingProfilePurpose?.GetHashCode() ?? 0) *  5 ^
                       (StackLevel?.            GetHashCode() ?? 0) *  3 ^
                       base.                    GetHashCode();

            }
        }

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
