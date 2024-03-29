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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The trigger message request.
    /// </summary>
    public class TriggerMessageRequest : ARequest<TriggerMessageRequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/triggerMessageRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The message to trigger.
        /// </summary>
        [Mandatory]
        public MessageTrigger  RequestedMessage    { get; }

        /// <summary>
        /// The optional EVSE (and connector) identification whenever the message
        /// applies to a specific EVSE and/or connector.
        /// </summary>
        [Optional]
        public EVSE?            EVSE                { get; }

        /// <summary>
        /// The optional custom trigger, when requestedMessage == "CustomTrigger".
        /// </summary>
        [Optional]
        public String?          CustomTrigger       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new trigger message request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="EVSE">An optional EVSE (and connector) identification whenever the message applies to a specific EVSE and/or connector.</param>
        /// <param name="CustomTrigger">An optional custom trigger, when requestedMessage == "CustomTrigger".</param>
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
        public TriggerMessageRequest(NetworkingNode_Id        NetworkingNodeId,
                                     MessageTrigger           RequestedMessage,
                                     EVSE?                    EVSE                = null,
                                     String?                  CustomTrigger       = null,

                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                                     CustomData?              CustomData          = null,

                                     Request_Id?              RequestId           = null,
                                     DateTime?                RequestTimestamp    = null,
                                     TimeSpan?                RequestTimeout      = null,
                                     EventTracking_Id?        EventTrackingId     = null,
                                     NetworkPath?             NetworkPath         = null,
                                     CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(TriggerMessageRequest)[..^7],

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

            this.RequestedMessage  = RequestedMessage;
            this.EVSE              = EVSE;
            this.CustomTrigger     = CustomTrigger;

            unchecked
            {

                hashCode = this.RequestedMessage.GetHashCode()       * 7 ^
                          (this.EVSE?.           GetHashCode() ?? 0) * 5 ^
                          (this.CustomTrigger?.  GetHashCode() ?? 0) * 3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:TriggerMessageRequest",
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
        //     "MessageTriggerEnumType": {
        //       "description": "Type of message to be triggered.\r\n",
        //       "javaType": "MessageTriggerEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "BootNotification",
        //         "LogStatusNotification",
        //         "FirmwareStatusNotification",
        //         "Heartbeat",
        //         "MeterValues",
        //         "SignChargingStationCertificate",
        //         "SignV2GCertificate",
        //         "StatusNotification",
        //         "TransactionEvent",
        //         "SignCombinedCertificate",
        //         "PublishFirmwareStatusNotification"
        //       ]
        //     },
        //     "EVSEType": {
        //       "description": "EVSE\r\nurn:x-oca:ocpp:uid:2:233123\r\nElectric Vehicle Supply Equipment\r\n",
        //       "javaType": "EVSE",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nEVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.\r\n",
        //           "type": "integer"
        //         },
        //         "connectorId": {
        //           "description": "An id to designate a specific connector (on an EVSE) by connector index number.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "id"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "evse": {
        //       "$ref": "#/definitions/EVSEType"
        //     },
        //     "requestedMessage": {
        //       "$ref": "#/definitions/MessageTriggerEnumType"
        //     }
        //   },
        //   "required": [
        //     "requestedMessage"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomTriggerMessageRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomTriggerMessageRequestParser">A delegate to parse custom trigger message requests.</param>
        public static TriggerMessageRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  NetworkingNode_Id                                    NetworkingNodeId,
                                                  NetworkPath                                          NetworkPath,
                                                  CustomJObjectParserDelegate<TriggerMessageRequest>?  CustomTriggerMessageRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var triggerMessageRequest,
                         out var errorResponse,
                         CustomTriggerMessageRequestParser))
            {
                return triggerMessageRequest;
            }

            throw new ArgumentException("The given JSON representation of a trigger message request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out TriggerMessageRequest, out ErrorResponse, CustomTriggerMessageRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a trigger message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="TriggerMessageRequest">The parsed trigger message request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTriggerMessageRequestParser">A delegate to parse custom trigger message requests.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       NetworkingNode_Id                                    NetworkingNodeId,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out TriggerMessageRequest?      TriggerMessageRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       CustomJObjectParserDelegate<TriggerMessageRequest>?  CustomTriggerMessageRequestParser)
        {

            try
            {

                TriggerMessageRequest = null;

                #region MessageTrigger       [mandatory]

                if (!JSON.ParseMandatory("requestedMessage",
                                         "requested message",
                                         OCPP.MessageTrigger.TryParse,
                                         out MessageTrigger MessageTrigger,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSE                 [optional]

                if (JSON.ParseOptionalJSON("evse",
                                           "evse",
                                           OCPPv2_1.EVSE.TryParse,
                                           out EVSE? EVSE,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomTrigger        [optional]

                var CustomTrigger = JSON["customTrigger"]?.Value<String>();

                #endregion


                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TriggerMessageRequest = new TriggerMessageRequest(

                                            NetworkingNodeId,
                                            MessageTrigger,
                                            EVSE,
                                            CustomTrigger,

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

                if (CustomTriggerMessageRequestParser is not null)
                    TriggerMessageRequest = CustomTriggerMessageRequestParser(JSON,
                                                                              TriggerMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                TriggerMessageRequest  = null;
                ErrorResponse          = "The given JSON representation of a trigger message request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTriggerMessageRequestSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTriggerMessageRequestSerializer">A delegate to serialize custom trigger message requests.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageRequest>?  CustomTriggerMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<EVSE>?                   CustomEVSESerializer                    = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?         CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestedMessage",   RequestedMessage.ToString()),

                           EVSE is not null
                               ? new JProperty("evse",               EVSE.            ToJSON(CustomEVSESerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTriggerMessageRequestSerializer is not null
                       ? CustomTriggerMessageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TriggerMessageRequest1, TriggerMessageRequest2)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest1">A trigger message request.</param>
        /// <param name="TriggerMessageRequest2">Another trigger message request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TriggerMessageRequest? TriggerMessageRequest1,
                                           TriggerMessageRequest? TriggerMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageRequest1, TriggerMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (TriggerMessageRequest1 is null || TriggerMessageRequest2 is null)
                return false;

            return TriggerMessageRequest1.Equals(TriggerMessageRequest2);

        }

        #endregion

        #region Operator != (TriggerMessageRequest1, TriggerMessageRequest2)

        /// <summary>
        /// Compares two trigger message requests for inequality.
        /// </summary>
        /// <param name="TriggerMessageRequest1">A trigger message request.</param>
        /// <param name="TriggerMessageRequest2">Another trigger message request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TriggerMessageRequest? TriggerMessageRequest1,
                                           TriggerMessageRequest? TriggerMessageRequest2)

            => !(TriggerMessageRequest1 == TriggerMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="Object">A trigger message request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TriggerMessageRequest triggerMessageRequest &&
                   Equals(triggerMessageRequest);

        #endregion

        #region Equals(TriggerMessageRequest)

        /// <summary>
        /// Compares two trigger message requests for equality.
        /// </summary>
        /// <param name="TriggerMessageRequest">A trigger message request to compare with.</param>
        public override Boolean Equals(TriggerMessageRequest? TriggerMessageRequest)

            => TriggerMessageRequest is not null &&

               RequestedMessage.Equals(TriggerMessageRequest.RequestedMessage) &&

             ((EVSE          is null     && TriggerMessageRequest.EVSE          is null) ||
              (EVSE          is not null && TriggerMessageRequest.EVSE          is not null && EVSE.         Equals(TriggerMessageRequest.EVSE)))          &&

             ((CustomTrigger is null     && TriggerMessageRequest.CustomTrigger is null) ||
              (CustomTrigger is not null && TriggerMessageRequest.CustomTrigger is not null && CustomTrigger.Equals(TriggerMessageRequest.CustomTrigger))) &&

               base.     GenericEquals(TriggerMessageRequest);

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

                   RequestedMessage,

                   EVSE is not null
                       ? $" at EVSE '{EVSE}'"
                       : ""

               );

        #endregion

    }

}
