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
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/securityEventNotificationRequest");

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
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Type">Type of the security event.</param>
        /// <param name="Timestamp">The timestamp of the security event.</param>
        /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
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
        public SecurityEventNotificationRequest(SourceRouting            Destination,
                                                SecurityEventType        Type,
                                                DateTime                 Timestamp,
                                                String?                  TechInfo              = null,

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
                   SerializationFormat ?? SerializationFormats.JSON,
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
        //       "description": "Type of the security event. This value should be taken from the Security events list.",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "timestamp": {
        //       "description": "Date and time at which the event occurred.",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "techInfo": {
        //       "description": "Additional information about the occurred security event.",
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

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomSecurityEventNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a security event notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSecurityEventNotificationRequestParser">A delegate to parse custom security event notification requests.</param>
        public static SecurityEventNotificationRequest Parse(JObject                                                         JSON,
                                                             Request_Id                                                      RequestId,
                                                             SourceRouting                                               Destination,
                                                             NetworkPath                                                     NetworkPath,
                                                             DateTime?                                                       RequestTimestamp                               = null,
                                                             TimeSpan?                                                       RequestTimeout                                 = null,
                                                             EventTracking_Id?                                               EventTrackingId                                = null,
                                                             CustomJObjectParserDelegate<SecurityEventNotificationRequest>?  CustomSecurityEventNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var securityEventNotificationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSecurityEventNotificationRequestParser))
            {
                return securityEventNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a security event notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out SecurityEventNotificationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a security event notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SecurityEventNotificationRequest">The parsed security event notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSecurityEventNotificationRequestParser">A delegate to parse custom security event notification requests.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       Request_Id                                                      RequestId,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out SecurityEventNotificationRequest?      SecurityEventNotificationRequest,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       DateTime?                                                       RequestTimestamp                               = null,
                                       TimeSpan?                                                       RequestTimeout                                 = null,
                                       EventTracking_Id?                                               EventTrackingId                                = null,
                                       CustomJObjectParserDelegate<SecurityEventNotificationRequest>?  CustomSecurityEventNotificationRequestParser   = null)
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SecurityEventNotificationRequest = new SecurityEventNotificationRequest(

                                                       Destination,
                                                       Type,
                                                       Timestamp,
                                                       TechInfo,

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
        public JObject ToJSON(Boolean                                                             IncludeJSONLDContext                               = false,
                              CustomJObjectSerializerDelegate<SecurityEventNotificationRequest>?  CustomSecurityEventNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("type",         Type.                ToString()),

                                 new JProperty("timestamp",    Timestamp.           ToIso8601()),

                           TechInfo is not null
                               ? new JProperty("techInfo",     TechInfo)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
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
