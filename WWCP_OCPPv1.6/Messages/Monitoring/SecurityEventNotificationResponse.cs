/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A SecurityEventNotification response.
    /// </summary>
    [SecurityExtensions]
    public class SecurityEventNotificationResponse : AResponse<SecurityEventNotificationRequest,
                                                               SecurityEventNotificationResponse>,
                                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/securityEventNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SecurityEventNotification response.
        /// </summary>
        /// <param name="Request">The SecurityEventNotification request leading to this response.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SecurityEventNotificationResponse(SecurityEventNotificationRequest  Request,

                                                 Result?                           Result                = null,
                                                 DateTime?                         ResponseTimestamp     = null,

                                                 SourceRouting?                    Destination           = null,
                                                 NetworkPath?                      NetworkPath           = null,

                                                 IEnumerable<KeyPair>?             SignKeys              = null,
                                                 IEnumerable<SignInfo>?            SignInfos             = null,
                                                 IEnumerable<Signature>?           Signatures            = null,

                                                 CustomData?                       CustomData            = null,

                                                 SerializationFormats?             SerializationFormat   = null,
                                                 CancellationToken                 CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        { }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SecurityEventNotification.conf",
        //   "type": "object",
        //   "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SecurityEventNotification response.
        /// </summary>
        /// <param name="Request">The SecurityEventNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSecurityEventNotificationResponseParser">An optional delegate to parse custom SecurityEventNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static SecurityEventNotificationResponse Parse(SecurityEventNotificationRequest                                 Request,
                                                              JObject                                                          JSON,
                                                              SourceRouting                                                    Destination,
                                                              NetworkPath                                                      NetworkPath,
                                                              DateTime?                                                        ResponseTimestamp                               = null,
                                                              CustomJObjectParserDelegate<SecurityEventNotificationResponse>?  CustomSecurityEventNotificationResponseParser   = null,
                                                              CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                                              CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var securityEventNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSecurityEventNotificationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return securityEventNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a SecurityEventNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SecurityEventNotificationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SecurityEventNotification response.
        /// </summary
        /// <param name="Request">The SecurityEventNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="SecurityEventNotificationResponse">The parsed SecurityEventNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSecurityEventNotificationResponseParser">An optional delegate to parse custom SecurityEventNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(SecurityEventNotificationRequest                                 Request,
                                       JObject                                                          JSON,
                                       SourceRouting                                                    Destination,
                                       NetworkPath                                                      NetworkPath,
                                       [NotNullWhen(true)]  out SecurityEventNotificationResponse?      SecurityEventNotificationResponse,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse,
                                       DateTime?                                                        ResponseTimestamp                               = null,
                                       CustomJObjectParserDelegate<SecurityEventNotificationResponse>?  CustomSecurityEventNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                       CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {

            try
            {

                SecurityEventNotificationResponse = null;

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                SecurityEventNotificationResponse = new SecurityEventNotificationResponse(

                                                        Request,

                                                        null,
                                                        ResponseTimestamp,

                                                        Destination,
                                                        NetworkPath,

                                                        null,
                                                        null,
                                                        Signatures,

                                                        CustomData

                                                    );

                if (CustomSecurityEventNotificationResponseParser is not null)
                    SecurityEventNotificationResponse = CustomSecurityEventNotificationResponseParser(JSON,
                                                                                                      SecurityEventNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                SecurityEventNotificationResponse  = null;
                ErrorResponse                      = "The given JSON representation of a SecurityEventNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSecurityEventNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSecurityEventNotificationResponseSerializer">A delegate to serialize custom SecurityEventNotification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SecurityEventNotificationResponse>?  CustomSecurityEventNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                          CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSecurityEventNotificationResponseSerializer is not null
                       ? CustomSecurityEventNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SecurityEventNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The SecurityEventNotification request.</param>
        public static SecurityEventNotificationResponse RequestError(SecurityEventNotificationRequest  Request,
                                                                     EventTracking_Id                  EventTrackingId,
                                                                     ResultCode                        ErrorCode,
                                                                     String?                           ErrorDescription    = null,
                                                                     JObject?                          ErrorDetails        = null,
                                                                     DateTime?                         ResponseTimestamp   = null,

                                                                     SourceRouting?                    Destination         = null,
                                                                     NetworkPath?                      NetworkPath         = null,

                                                                     IEnumerable<KeyPair>?             SignKeys            = null,
                                                                     IEnumerable<SignInfo>?            SignInfos           = null,
                                                                     IEnumerable<Signature>?           Signatures          = null,

                                                                     CustomData?                       CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The SecurityEventNotification failed.
        /// </summary>
        /// <param name="Request">The SecurityEventNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SecurityEventNotificationResponse FormationViolation(SecurityEventNotificationRequest  Request,
                                                                           String                            ErrorDescription)

            => new (Request,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SecurityEventNotification failed.
        /// </summary>
        /// <param name="Request">The SecurityEventNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SecurityEventNotificationResponse SignatureError(SecurityEventNotificationRequest  Request,
                                                                       String                            ErrorDescription)

            => new (Request,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SecurityEventNotification failed.
        /// </summary>
        /// <param name="Request">The SecurityEventNotification request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SecurityEventNotificationResponse Failed(SecurityEventNotificationRequest  Request,
                                                               String?                           Description   = null)

            => new (Request,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The SecurityEventNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The SecurityEventNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static SecurityEventNotificationResponse ExceptionOccured(SecurityEventNotificationRequest  Request,
                                                                         Exception                         Exception)

            => new (Request,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SecurityEventNotificationResponse1, SecurityEventNotificationResponse2)

        /// <summary>
        /// Compares two SecurityEventNotification responses for equality.
        /// </summary>
        /// <param name="SecurityEventNotificationResponse1">A SecurityEventNotification response.</param>
        /// <param name="SecurityEventNotificationResponse2">Another SecurityEventNotification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SecurityEventNotificationResponse? SecurityEventNotificationResponse1,
                                           SecurityEventNotificationResponse? SecurityEventNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SecurityEventNotificationResponse1, SecurityEventNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SecurityEventNotificationResponse1 is null || SecurityEventNotificationResponse2 is null)
                return false;

            return SecurityEventNotificationResponse1.Equals(SecurityEventNotificationResponse2);

        }

        #endregion

        #region Operator != (SecurityEventNotificationResponse1, SecurityEventNotificationResponse2)

        /// <summary>
        /// Compares two SecurityEventNotification responses for inequality.
        /// </summary>
        /// <param name="SecurityEventNotificationResponse1">A SecurityEventNotification response.</param>
        /// <param name="SecurityEventNotificationResponse2">Another SecurityEventNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SecurityEventNotificationResponse? SecurityEventNotificationResponse1,
                                           SecurityEventNotificationResponse? SecurityEventNotificationResponse2)

            => !(SecurityEventNotificationResponse1 == SecurityEventNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<SecurityEventNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SecurityEventNotification responses for equality.
        /// </summary>
        /// <param name="Object">A SecurityEventNotification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecurityEventNotificationResponse securityEventNotificationResponse &&
                   Equals(securityEventNotificationResponse);

        #endregion

        #region Equals(SecurityEventNotificationResponse)

        /// <summary>
        /// Compares two SecurityEventNotification responses for equality.
        /// </summary>
        /// <param name="SecurityEventNotificationResponse">A SecurityEventNotification response to compare with.</param>
        public override Boolean Equals(SecurityEventNotificationResponse? SecurityEventNotificationResponse)

            => SecurityEventNotificationResponse is not null;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "SecurityEventNotificationResponse";

        #endregion

    }

}
