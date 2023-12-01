/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The security event notification request.
    /// </summary>
    public class SecurityEventNotificationRequest : ARequest<SecurityEventNotificationRequest>,
                                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/securityEventNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Type of the security event.
        /// </summary>
        [Mandatory]
        public SecurityEventType  Type         { get; }

        /// <summary>
        /// The timestamp of the security event.
        /// </summary>
        [Mandatory]
        public DateTime           Timestamp    { get; }

        /// <summary>
        /// Optional additional information about the occurred security event.
        /// </summary>
        [Optional]
        public String?            TechInfo     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new security event notification request.
        /// </summary>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="Type">Type of the security event.</param>
        /// <param name="Timestamp">The timestamp of the security event.</param>
        /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
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
        public SecurityEventNotificationRequest(NetworkingNode_Id        NetworkingNodeId,
                                                SecurityEventType        Type,
                                                DateTime                 Timestamp,
                                                String?                  TechInfo            = null,

                                                IEnumerable<KeyPair>?    SignKeys            = null,
                                                IEnumerable<SignInfo>?   SignInfos           = null,
                                                IEnumerable<Signature>?  Signatures          = null,

                                                CustomData?              CustomData          = null,

                                                Request_Id?              RequestId           = null,
                                                DateTime?                RequestTimestamp    = null,
                                                TimeSpan?                RequestTimeout      = null,
                                                EventTracking_Id?        EventTrackingId     = null,
                                                NetworkPath?             NetworkPath         = null,
                                                CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(SecurityEventNotificationRequest)[..^7],

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

            this.Type       = Type;
            this.Timestamp  = Timestamp;
            this.TechInfo   = TechInfo;


            unchecked
            {

                hashCode = this.Type.     GetHashCode()       * 7 ^
                           this.Timestamp.GetHashCode()       * 5 ^
                          (this.TechInfo?.GetHashCode() ?? 0) * 3 ^
                           base.          GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SecurityEventNotificationRequest",
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
        //     "type": {
        //       "description": "Type of the security event. This value should be taken from the Security events list.\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "timestamp": {
        //       "description": "Date and time at which the event occurred.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "techInfo": {
        //       "description": "Additional information about the occurred security event.\r\n",
        //       "type": "string",
        //       "maxLength": 255
        //     }
        //   },
        //   "required": [
        //     "type",
        //     "timestamp"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomSecurityEventNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a security event notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSecurityEventNotificationRequestParser">A delegate to parse custom security event notification requests.</param>
        public static SecurityEventNotificationRequest Parse(JObject                                                         JSON,
                                                             Request_Id                                                      RequestId,
                                                             NetworkingNode_Id                                               NetworkingNodeId,
                                                             NetworkPath                                                     NetworkPath,
                                                             CustomJObjectParserDelegate<SecurityEventNotificationRequest>?  CustomSecurityEventNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var securityEventNotificationRequest,
                         out var errorResponse,
                         CustomSecurityEventNotificationRequestParser) &&
                securityEventNotificationRequest is not null)
            {
                return securityEventNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a security event notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out SecurityEventNotificationRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a security event notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SecurityEventNotificationRequest">The parsed security event notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       Request_Id                             RequestId,
                                       NetworkingNode_Id                      NetworkingNodeId,
                                       NetworkPath                            NetworkPath,
                                       out SecurityEventNotificationRequest?  SecurityEventNotificationRequest,
                                       out String?                            ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out SecurityEventNotificationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a security event notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SecurityEventNotificationRequest">The parsed security event notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSecurityEventNotificationRequestParser">A delegate to parse custom security event notification requests.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       Request_Id                                                      RequestId,
                                       NetworkingNode_Id                                               NetworkingNodeId,
                                       NetworkPath                                                     NetworkPath,
                                       out SecurityEventNotificationRequest?                           SecurityEventNotificationRequest,
                                       out String?                                                     ErrorResponse,
                                       CustomJObjectParserDelegate<SecurityEventNotificationRequest>?  CustomSecurityEventNotificationRequestParser)
        {

            try
            {

                SecurityEventNotificationRequest = null;

                #region Type                 [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "security event type",
                                         OCPPv2_1.SecurityEventType.TryParse,
                                         out SecurityEventType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp            [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TechInfo             [optional]

                var TechInfo = JSON.GetOptional("techInfo");

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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SecurityEventNotificationRequest = new SecurityEventNotificationRequest(

                                                       NetworkingNodeId,
                                                       Type,
                                                       Timestamp,
                                                       TechInfo,

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

                if (CustomSecurityEventNotificationRequestParser is not null)
                    SecurityEventNotificationRequest = CustomSecurityEventNotificationRequestParser(JSON,
                                                                                                    SecurityEventNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                SecurityEventNotificationRequest  = null;
                ErrorResponse                     = "The given JSON representation of a security event notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSecurityEventNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSecurityEventNotificationRequestSerializer">A delegate to serialize custom security event notification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SecurityEventNotificationRequest>?  CustomSecurityEventNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("type",         Type.      ToString()),

                                 new JProperty("timestamp",    Timestamp. ToIso8601()),

                           TechInfo is not null
                               ? new JProperty("techInfo",     TechInfo)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSecurityEventNotificationRequestSerializer is not null
                       ? CustomSecurityEventNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SecurityEventNotificationRequest1, SecurityEventNotificationRequest2)

        /// <summary>
        /// Compares two security event notification requests for equality.
        /// </summary>
        /// <param name="SecurityEventNotificationRequest1">A security event notification request.</param>
        /// <param name="SecurityEventNotificationRequest2">Another security event notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SecurityEventNotificationRequest? SecurityEventNotificationRequest1,
                                           SecurityEventNotificationRequest? SecurityEventNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SecurityEventNotificationRequest1, SecurityEventNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SecurityEventNotificationRequest1 is null || SecurityEventNotificationRequest2 is null)
                return false;

            return SecurityEventNotificationRequest1.Equals(SecurityEventNotificationRequest2);

        }

        #endregion

        #region Operator != (SecurityEventNotificationRequest1, SecurityEventNotificationRequest2)

        /// <summary>
        /// Compares two security event notification requests for inequality.
        /// </summary>
        /// <param name="SecurityEventNotificationRequest1">A security event notification request.</param>
        /// <param name="SecurityEventNotificationRequest2">Another security event notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SecurityEventNotificationRequest? SecurityEventNotificationRequest1,
                                           SecurityEventNotificationRequest? SecurityEventNotificationRequest2)

            => !(SecurityEventNotificationRequest1 == SecurityEventNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<SecurityEventNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two security event notification requests for equality.
        /// </summary>
        /// <param name="Object">A security event notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecurityEventNotificationRequest securityEventNotificationRequest &&
                   Equals(securityEventNotificationRequest);

        #endregion

        #region Equals(SecurityEventNotificationRequest)

        /// <summary>
        /// Compares two security event notification requests for equality.
        /// </summary>
        /// <param name="SecurityEventNotificationRequest">A security event notification request to compare with.</param>
        public override Boolean Equals(SecurityEventNotificationRequest? SecurityEventNotificationRequest)

            => SecurityEventNotificationRequest is not null &&

               Type.       Equals(SecurityEventNotificationRequest.Type)      &&
               Timestamp.  Equals(SecurityEventNotificationRequest.Timestamp) &&

             ((TechInfo is     null && SecurityEventNotificationRequest.TechInfo is     null) ||
              (TechInfo is not null && SecurityEventNotificationRequest.TechInfo is not null && TechInfo.Equals(SecurityEventNotificationRequest.TechInfo))) &&

               base.GenericEquals(SecurityEventNotificationRequest);

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

            => $"{Type} ({Timestamp})";

        #endregion

    }

}
