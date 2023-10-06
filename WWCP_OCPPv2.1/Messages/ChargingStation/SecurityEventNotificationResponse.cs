/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A security event notification response.
    /// </summary>
    public class SecurityEventNotificationResponse : AResponse<CS.SecurityEventNotificationRequest,
                                                               SecurityEventNotificationResponse>
    {

        #region Constructor(s)

        #region SecurityEventNotificationResponse(Request, ...)

        /// <summary>
        /// Create a new security event notification response.
        /// </summary>
        /// <param name="Request">The security event notification request leading to this response.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public SecurityEventNotificationResponse(CS.SecurityEventNotificationRequest  Request,

                                                 IEnumerable<KeyPair>?                SignKeys          = null,
                                                 IEnumerable<SignInfo>?               SignInfos         = null,
                                                 SignaturePolicy?                     SignaturePolicy   = null,
                                                 IEnumerable<Signature>?              Signatures        = null,

                                                 DateTime?                            Timestamp         = null,
                                                 CustomData?                          CustomData        = null)

            : base(Request,
                   Result.OK(),
                   SignKeys,
                   SignInfos,
                   SignaturePolicy,
                   Signatures,
                   Timestamp,
                   CustomData)

        { }

        #endregion

        #region SecurityEventNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new security event notification response.
        /// </summary>
        /// <param name="Request">The security event notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SecurityEventNotificationResponse(CS.SecurityEventNotificationRequest  Request,
                                                 Result                               Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SecurityEventNotificationResponse",
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
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (Request, SecurityEventNotificationResponseJSON, CustomSecurityEventNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a security event notification response.
        /// </summary>
        /// <param name="Request">The security event notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSecurityEventNotificationResponseParser">A delegate to parse custom security event notification responses.</param>
        public static SecurityEventNotificationResponse Parse(CS.SecurityEventNotificationRequest                              Request,
                                                              JObject                                                          JSON,
                                                              CustomJObjectParserDelegate<SecurityEventNotificationResponse>?  CustomSecurityEventNotificationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var securityEventNotificationResponse,
                         out var errorResponse,
                         CustomSecurityEventNotificationResponseParser))
            {
                return securityEventNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a security event notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SecurityEventNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a security event notification response.
        /// </summary
        /// <param name="Request">The security event notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SecurityEventNotificationResponse">The parsed security event notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSecurityEventNotificationResponseParser">A delegate to parse custom security event notification responses.</param>
        public static Boolean TryParse(CS.SecurityEventNotificationRequest                              Request,
                                       JObject                                                          JSON,
                                       out SecurityEventNotificationResponse?                           SecurityEventNotificationResponse,
                                       out String?                                                      ErrorResponse,
                                       CustomJObjectParserDelegate<SecurityEventNotificationResponse>?  CustomSecurityEventNotificationResponseParser   = null)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SecurityEventNotificationResponse = new SecurityEventNotificationResponse(
                                                        Request,
                                                        null,
                                                        null,
                                                        null,
                                                        Signatures,
                                                        null,
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
                ErrorResponse                      = "The given JSON representation of a security event notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSecurityEventNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSecurityEventNotificationResponseSerializer">A delegate to serialize custom security event notification responses.</param>
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
        /// The security event notification failed.
        /// </summary>
        /// <param name="Request">The security event notification request leading to this response.</param>
        public static SecurityEventNotificationResponse Failed(CS.SecurityEventNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SecurityEventNotificationResponse1, SecurityEventNotificationResponse2)

        /// <summary>
        /// Compares two security event notification responses for equality.
        /// </summary>
        /// <param name="SecurityEventNotificationResponse1">A security event notification response.</param>
        /// <param name="SecurityEventNotificationResponse2">Another security event notification response.</param>
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
        /// Compares two security event notification responses for inequality.
        /// </summary>
        /// <param name="SecurityEventNotificationResponse1">A security event notification response.</param>
        /// <param name="SecurityEventNotificationResponse2">Another security event notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SecurityEventNotificationResponse? SecurityEventNotificationResponse1,
                                           SecurityEventNotificationResponse? SecurityEventNotificationResponse2)

            => !(SecurityEventNotificationResponse1 == SecurityEventNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<SecurityEventNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two security event notification responses for equality.
        /// </summary>
        /// <param name="Object">A security event notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecurityEventNotificationResponse securityEventNotificationResponse &&
                   Equals(securityEventNotificationResponse);

        #endregion

        #region Equals(SecurityEventNotificationResponse)

        /// <summary>
        /// Compares two security event notification responses for equality.
        /// </summary>
        /// <param name="SecurityEventNotificationResponse">A security event notification response to compare with.</param>
        public override Boolean Equals(SecurityEventNotificationResponse? SecurityEventNotificationResponse)

            => SecurityEventNotificationResponse is not null &&
                   base.GenericEquals(SecurityEventNotificationResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
