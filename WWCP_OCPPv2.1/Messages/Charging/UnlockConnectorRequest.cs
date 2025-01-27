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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The UnlockConnector request.
    /// </summary>
    public class UnlockConnectorRequest : ARequest<UnlockConnectorRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/unlockConnectorRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identifier of the EVSE to be unlocked.
        /// </summary>
        [Mandatory]
        public EVSE_Id        EVSEId         { get; }

        /// <summary>
        /// The identifier of the connector to be unlocked.
        /// </summary>
        [Mandatory]
        public Connector_Id   ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UnlockConnector request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="EVSEId">The identifier of the EVSE to be unlocked.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
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
        public UnlockConnectorRequest(SourceRouting            Destination,
                                      EVSE_Id                  EVSEId,
                                      Connector_Id             ConnectorId,

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
                   nameof(UnlockConnectorRequest)[..^7],

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

            this.EVSEId       = EVSEId;
            this.ConnectorId  = ConnectorId;

            unchecked
            {
                hashCode = this.EVSEId.     GetHashCode() * 5 ^
                           this.ConnectorId.GetHashCode() * 3 ^
                           base.            GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UnlockConnectorRequest",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "evseId": {
        //       "description": "This contains the identifier of the EVSE for which a connector needs to be unlocked.",
        //       "type": "integer"
        //     },
        //     "connectorId": {
        //       "description": "This contains the identifier of the connector that needs to be unlocked.",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "evseId",
        //     "connectorId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomUnlockConnectorRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UnlockConnector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomUnlockConnectorRequestParser">A delegate to parse custom UnlockConnector requests.</param>
        public static UnlockConnectorRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   SourceRouting                                     Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             RequestTimestamp                     = null,
                                                   TimeSpan?                                             RequestTimeout                       = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var unlockConnectorRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomUnlockConnectorRequestParser))
            {
                return unlockConnectorRequest;
            }

            throw new ArgumentException("The given JSON representation of an UnlockConnector request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON,  RequestId, SourceRouting, NetworkPath, out UnlockConnectorRequest, out ErrorResponse, CustomUnlockConnectorRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an UnlockConnector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UnlockConnectorRequest">The parsed UnlockConnector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorRequestParser">A delegate to parse custom UnlockConnector requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out UnlockConnectorRequest?      UnlockConnectorRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             RequestTimestamp                     = null,
                                       TimeSpan?                                             RequestTimeout                       = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser   = null)
        {

            try
            {

                UnlockConnectorRequest = null;

                #region EVSEId               [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId          [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
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


                UnlockConnectorRequest = new UnlockConnectorRequest(

                                             Destination,
                                             EVSEId,
                                             ConnectorId,

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

                if (CustomUnlockConnectorRequestParser is not null)
                    UnlockConnectorRequest = CustomUnlockConnectorRequestParser(JSON,
                                                                                UnlockConnectorRequest);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorRequest  = null;
                ErrorResponse           = "The given JSON representation of an UnlockConnector request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnlockConnectorRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorRequestSerializer">A delegate to serialize custom UnlockConnector requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                   IncludeJSONLDContext                     = false,
                              CustomJObjectSerializerDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("evseId",        EVSEId.     Value),

                                 new JProperty("connectorId",   ConnectorId.Value),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnlockConnectorRequestSerializer is not null
                       ? CustomUnlockConnectorRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two UnlockConnector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorRequest1, UnlockConnectorRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorRequest1 is null || UnlockConnectorRequest2 is null)
                return false;

            return UnlockConnectorRequest1.Equals(UnlockConnectorRequest2);

        }

        #endregion

        #region Operator != (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two UnlockConnector requests for inequality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">An UnlockConnector request.</param>
        /// <param name="UnlockConnectorRequest2">Another UnlockConnector request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)

            => !(UnlockConnectorRequest1 == UnlockConnectorRequest2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UnlockConnector requests for equality.
        /// </summary>
        /// <param name="Object">An UnlockConnector request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorRequest unlockConnectorRequest &&
                   Equals(unlockConnectorRequest);

        #endregion

        #region Equals(UnlockConnectorRequest)

        /// <summary>
        /// Compares two UnlockConnector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest">An UnlockConnector request to compare with.</param>
        public override Boolean Equals(UnlockConnectorRequest? UnlockConnectorRequest)

            => UnlockConnectorRequest is not null &&

               EVSEId.     Equals(UnlockConnectorRequest.EVSEId)      &&
               ConnectorId.Equals(UnlockConnectorRequest.ConnectorId) &&

               base.GenericEquals(UnlockConnectorRequest);

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

            => $"{EVSEId} / {ConnectorId}";

        #endregion

    }

}
