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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
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
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/reserveNowRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of this reservation.
        /// </summary>
        [Mandatory]
        public Reservation_Id   Id               { get; }

        /// <summary>
        /// The timestamp when the reservation ends.
        /// </summary>
        [Mandatory]
        public DateTime         ExpiryDate       { get; }

        /// <summary>
        /// The unique token identification for which the reservation is being made.
        /// </summary>
        [Mandatory]
        public IdToken          IdToken          { get; }

        /// <summary>
        /// The optional connector type to be reserved.
        /// </summary>
        [Optional]
        public ConnectorType?  ConnectorType    { get; }

        /// <summary>
        /// The identification of the EVSE to be reserved.
        /// A value of 0 means that the reservation is not for
        /// a specific connector.
        /// </summary>
        [Optional]
        public EVSE_Id?         EVSEId           { get; }

        /// <summary>
        /// The optional group identifier for which the reservation is being made.
        /// </summary>
        [Optional]
        public IdToken?         GroupIdToken     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ReserveNow request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="Id">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdToken">The unique token identification for which the reservation is being made.</param>
        /// <param name="ConnectorType">An optional connector type to be reserved..</param>
        /// <param name="EVSEId">The identification of the EVSE to be reserved. A value of 0 means that the reservation is not for a specific EVSE.</param>
        /// <param name="GroupIdToken">An optional group identifier for which the reservation is being made.</param>
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
        public ReserveNowRequest(NetworkingNode_Id        DestinationId,
                                 Reservation_Id           Id,
                                 DateTime                 ExpiryDate,
                                 IdToken                  IdToken,
                                 ConnectorType?           ConnectorType       = null,
                                 EVSE_Id?                 EVSEId              = null,
                                 IdToken?                 GroupIdToken        = null,

                                 IEnumerable<KeyPair>?    SignKeys            = null,
                                 IEnumerable<SignInfo>?   SignInfos           = null,
                                 IEnumerable<Signature>?       Signatures          = null,

                                 CustomData?              CustomData          = null,

                                 Request_Id?              RequestId           = null,
                                 DateTime?                RequestTimestamp    = null,
                                 TimeSpan?                RequestTimeout      = null,
                                 EventTracking_Id?        EventTrackingId     = null,
                                 NetworkPath?             NetworkPath         = null,
                                 CancellationToken        CancellationToken   = default)

            : base(DestinationId,
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
                   CancellationToken)

        {

            this.Id             = Id;
            this.ExpiryDate     = ExpiryDate;
            this.IdToken        = IdToken;
            this.ConnectorType  = ConnectorType;
            this.EVSEId         = EVSEId;
            this.GroupIdToken   = GroupIdToken;


            unchecked
            {

                hashCode = this.Id.            GetHashCode()       * 17 ^
                           this.ExpiryDate.    GetHashCode()       * 13 ^
                           this.IdToken.       GetHashCode()       * 11 ^

                          (this.ConnectorType?.GetHashCode() ?? 0) *  7 ^
                          (this.EVSEId?.       GetHashCode() ?? 0) *  5 ^
                          (this.GroupIdToken?. GetHashCode() ?? 0) *  3 ^

                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ReserveNowRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "ConnectorEnumType": {
        //       "description": "This field specifies the connector type.",
        //       "javaType": "ConnectorEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "cCCS1",
        //         "cCCS2",
        //         "cG105",
        //         "cTesla",
        //         "cType1",
        //         "cType2",
        //         "s309-1P-16A",
        //         "s309-1P-32A",
        //         "s309-3P-16A",
        //         "s309-3P-32A",
        //         "sBS1361",
        //         "sCEE-7-7",
        //         "sType2",
        //         "sType3",
        //         "Other1PhMax16A",
        //         "Other1PhOver16A",
        //         "Other3Ph",
        //         "Pan",
        //         "wInductive",
        //         "wResonant",
        //         "Undetermined",
        //         "Unknown"
        //       ]
        //     },
        //     "IdTokenEnumType": {
        //       "description": "Enumeration of possible idToken types.",
        //       "javaType": "IdTokenEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Central",
        //         "eMAID",
        //         "ISO14443",
        //         "ISO15693",
        //         "KeyCode",
        //         "Local",
        //         "MacAddress",
        //         "NoAuthorization"
        //       ]
        //     },
        //     "AdditionalInfoType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //       "javaType": "AdditionalInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalIdToken": {
        //           "description": "This field specifies the additional IdToken.",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "description": "This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "additionalIdToken",
        //         "type"
        //       ]
        //     },
        //     "IdTokenType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //       "javaType": "IdToken",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalInfo": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/AdditionalInfoType"
        //           },
        //           "minItems": 1
        //         },
        //         "idToken": {
        //           "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "$ref": "#/definitions/IdTokenEnumType"
        //         }
        //       },
        //       "required": [
        //         "idToken",
        //         "type"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "id": {
        //       "description": "Id of reservation.",
        //       "type": "integer"
        //     },
        //     "expiryDateTime": {
        //       "description": "Date and time at which the reservation expires.",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "connectorType": {
        //       "$ref": "#/definitions/ConnectorEnumType"
        //     },
        //     "idToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     },
        //     "evseId": {
        //       "description": "This contains ID of the evse to be reserved.",
        //       "type": "integer"
        //     },
        //     "groupIdToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     }
        //   },
        //   "required": [
        //     "id",
        //     "expiryDateTime",
        //     "idToken"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomReserveNowRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ReserveNow request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReserveNowRequestParser">A delegate to parse custom ReserveNow requests.</param>
        public static ReserveNowRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              NetworkingNode_Id                                DestinationId,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        RequestTimestamp                = null,
                                              TimeSpan?                                        RequestTimeout                  = null,
                                              EventTracking_Id?                                EventTrackingId                 = null,
                                              CustomJObjectParserDelegate<ReserveNowRequest>?  CustomReserveNowRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var reserveNowRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomReserveNowRequestParser))
            {
                return reserveNowRequest;
            }

            throw new ArgumentException("The given JSON representation of a ReserveNow response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out ReserveNowRequest, out ErrorResponse, CustomReserveNowRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ReserveNow request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ReserveNowRequest">The parsed ReserveNow request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReserveNowRequestParser">A delegate to parse custom ReserveNowRequest requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       NetworkingNode_Id                                DestinationId,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out ReserveNowRequest?      ReserveNowRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        RequestTimestamp                = null,
                                       TimeSpan?                                        RequestTimeout                  = null,
                                       EventTracking_Id?                                EventTrackingId                 = null,
                                       CustomJObjectParserDelegate<ReserveNowRequest>?  CustomReserveNowRequestParser   = null)
        {

            try
            {

                ReserveNowRequest = null;

                #region Id                   [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ExpiryDateTime       [mandatory]

                if (!JSON.ParseMandatory("expiryDateTime",
                                         "expiry timestamp",
                                         out DateTime ExpiryDateTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdToken              [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse) ||
                     IdToken is null)
                {
                    return false;
                }

                #endregion

                #region ConnectorType        [optional]

                if (JSON.ParseOptional("connectorType",
                                       "connector type",
                                       OCPPv2_1.ConnectorType.TryParse,
                                       out ConnectorType? ConnectorType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSEId               [optional]

                if (JSON.ParseOptional("evseId",
                                       "evse identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region GroupIdToken         [optional]

                if (JSON.ParseOptionalJSON("groupIdToken",
                                           "group identification token",
                                           OCPPv2_1.IdToken.TryParse,
                                           out IdToken GroupIdToken,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ReserveNowRequest = new ReserveNowRequest(

                                        DestinationId,
                                        Id,
                                        ExpiryDateTime,
                                        IdToken,
                                        ConnectorType,
                                        EVSEId,
                                        GroupIdToken,

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

        #region ToJSON(CustomReserveNowRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReserveNowRequestSerializer">A delegate to serialize custom ReserveNow requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReserveNowRequest>?  CustomReserveNowRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?            CustomIdTokenSerializer             = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?     CustomAdditionalInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",               Id.Value),
                                 new JProperty("expiryDateTime",   ExpiryDate.         ToIso8601()),
                                 new JProperty("idToken",          IdToken.            ToJSON(CustomIdTokenSerializer,
                                                                                              CustomAdditionalInfoSerializer,
                                                                                              CustomCustomDataSerializer)),

                           ConnectorType.HasValue
                               ? new JProperty("connectorType",    ConnectorType.Value.ToString())
                               : null,

                           EVSEId.HasValue
                               ? new JProperty("evseId",           EVSEId.       Value.Value)
                               : null,

                           GroupIdToken is not null
                               ? new JProperty("groupIdToken",     GroupIdToken.       ToJSON(CustomIdTokenSerializer,
                                                                                              CustomAdditionalInfoSerializer,
                                                                                              CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.         ToJSON(CustomCustomDataSerializer))
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

               Id.Equals(ReserveNowRequest.Id) &&
               ExpiryDate.   Equals(ReserveNowRequest.ExpiryDate)    &&
               IdToken.      Equals(ReserveNowRequest.IdToken)       &&

            ((!ConnectorType.HasValue    && !ReserveNowRequest.ConnectorType.HasValue)    ||
              (ConnectorType.HasValue    &&  ReserveNowRequest.ConnectorType.HasValue    && ConnectorType.Equals(ReserveNowRequest.ConnectorType)))  &&

            ((!EVSEId.       HasValue    && !ReserveNowRequest.EVSEId.       HasValue)    ||
              (EVSEId.       HasValue    &&  ReserveNowRequest.EVSEId.       HasValue    && EVSEId.       Equals(ReserveNowRequest.EVSEId)))         &&

             ((GroupIdToken  is     null &&  ReserveNowRequest.GroupIdToken  is     null) ||
              (GroupIdToken  is not null &&  ReserveNowRequest.GroupIdToken  is not null && GroupIdToken. Equals(ReserveNowRequest.GroupIdToken)))   &&

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

                   Id,
                   " till ",
                   ExpiryDate.ToIso8601(),
                   " for ",
                   IdToken.ToString(),

                   ConnectorType.HasValue
                       ? ", connector type: " + ConnectorType.Value.ToString()
                       : "",

                   EVSEId.HasValue
                       ? ", EVSE Id: "        + EVSEId.       Value.ToString()
                       : "",

                   GroupIdToken is not null
                       ? "GroupIdToken: "     + GroupIdToken.       ToString()
                       : ""

               );

        #endregion

    }

}
