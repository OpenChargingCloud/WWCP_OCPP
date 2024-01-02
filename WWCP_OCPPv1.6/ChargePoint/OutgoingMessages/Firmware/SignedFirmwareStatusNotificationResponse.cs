/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A signed signed firmware status notification response.
    /// </summary>
    [SecurityExtensions]
    public class SignedFirmwareStatusNotificationResponse : AResponse<CP.SignedFirmwareStatusNotificationRequest,
                                                                         SignedFirmwareStatusNotificationResponse>,
                                                            IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/signedFirmwareStatusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region SignedFirmwareStatusNotificationResponse(Request)

        /// <summary>
        /// Create a new signed firmware status notification response.
        /// </summary>
        /// <param name="Request">The signed firmware status notification request leading to this response.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignedFirmwareStatusNotificationResponse(CP.SignedFirmwareStatusNotificationRequest  Request,

                                                        DateTime?                                   ResponseTimestamp   = null,

                                                        IEnumerable<KeyPair>?                       SignKeys            = null,
                                                        IEnumerable<SignInfo>?                      SignInfos           = null,
                                                        IEnumerable<OCPP.Signature>?                Signatures          = null,

                                                        CustomData?                                 CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region SignedFirmwareStatusNotificationResponse(Result)

        /// <summary>
        /// Create a new signed firmware status notification response.
        /// </summary>
        /// <param name="Request">The signed firmware status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SignedFirmwareStatusNotificationResponse(CP.SignedFirmwareStatusNotificationRequest  Request,
                                                        Result                                      Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignedFirmwareStatusNotification.conf",
        //   "type": "object",
        //   "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomSignedFirmwareStatusNotificationResponseResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed firmware status notification response.
        /// </summary>
        /// <param name="Request">The signed firmware status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationResponseResponseParser">A delegate to parse custom signed firmware status notification responses.</param>
        public static SignedFirmwareStatusNotificationResponse Parse(CP.SignedFirmwareStatusNotificationRequest                              Request,
                                                                     JObject                                                                 JSON,
                                                                     CustomJObjectParserDelegate<SignedFirmwareStatusNotificationResponse>?  CustomSignedFirmwareStatusNotificationResponseResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var signedFirmwareStatusNotificationResponse,
                         out var errorResponse,
                         CustomSignedFirmwareStatusNotificationResponseResponseParser) &&
                signedFirmwareStatusNotificationResponse is not null)
            {
                return signedFirmwareStatusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a signed firmware status response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SignedFirmwareStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a signed firmware status notification response.
        /// </summary>
        /// <param name="Request">The signed firmware status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignedFirmwareStatusNotificationResponse">The parsed signed firmware status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationResponseResponseParser">A delegate to parse custom signed firmware status notification responses.</param>
        public static Boolean TryParse(CP.SignedFirmwareStatusNotificationRequest                              Request,
                                       JObject                                                                 JSON,
                                       out SignedFirmwareStatusNotificationResponse?                           SignedFirmwareStatusNotificationResponse,
                                       out String?                                                             ErrorResponse,
                                       CustomJObjectParserDelegate<SignedFirmwareStatusNotificationResponse>?  CustomSignedFirmwareStatusNotificationResponseResponseParser   = null)
        {

            try
            {

                SignedFirmwareStatusNotificationResponse = null;

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                SignedFirmwareStatusNotificationResponse = new SignedFirmwareStatusNotificationResponse(

                                                               Request,
                                                               null,

                                                               null,
                                                               null,
                                                               Signatures,

                                                               CustomData

                                                           );

                if (CustomSignedFirmwareStatusNotificationResponseResponseParser is not null)
                    SignedFirmwareStatusNotificationResponse = CustomSignedFirmwareStatusNotificationResponseResponseParser(JSON,
                                                                                                                            SignedFirmwareStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                SignedFirmwareStatusNotificationResponse  = null;
                ErrorResponse                             = "The given JSON representation of a signed firmware status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedFirmwareStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedFirmwareStatusNotificationResponseSerializer">A delegate to serialize custom signed firmware status notification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedFirmwareStatusNotificationResponse>?  CustomSignedFirmwareStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                            CustomSignatureSerializer                                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                CustomCustomDataSerializer                                 = null)
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

            return CustomSignedFirmwareStatusNotificationResponseSerializer is not null
                       ? CustomSignedFirmwareStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The signed firmware status notification request failed.
        /// </summary>
        /// <param name="Request">The signed firmware status notification request leading to this response.</param>
        public static SignedFirmwareStatusNotificationResponse Failed(CP.SignedFirmwareStatusNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SignedFirmwareStatusNotificationResponse1, SignedFirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two signed firmware status notification responses for equality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationResponse1">A signed firmware status notification response.</param>
        /// <param name="SignedFirmwareStatusNotificationResponse2">Another signed firmware status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse1,
                                           SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignedFirmwareStatusNotificationResponse1, SignedFirmwareStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SignedFirmwareStatusNotificationResponse1 is null || SignedFirmwareStatusNotificationResponse2 is null)
                return false;

            return SignedFirmwareStatusNotificationResponse1.Equals(SignedFirmwareStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (SignedFirmwareStatusNotificationResponse1, SignedFirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two signed firmware status notification responses for inequality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationResponse1">A signed firmware status notification response.</param>
        /// <param name="SignedFirmwareStatusNotificationResponse2">Another signed firmware status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse1,
                                           SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse2)

            => !(SignedFirmwareStatusNotificationResponse1 == SignedFirmwareStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<SignedFirmwareStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed firmware status notification responses for equality.
        /// </summary>
        /// <param name="Object">A signed firmware status notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedFirmwareStatusNotificationResponse signedFirmwareStatusNotificationResponse &&
                   Equals(signedFirmwareStatusNotificationResponse);

        #endregion

        #region Equals(SignedFirmwareStatusNotificationResponse)

        /// <summary>
        /// Compares two signed firmware status notification responses for equality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationResponse">A signed firmware status notification response to compare with.</param>
        public override Boolean Equals(SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse)

            => SignedFirmwareStatusNotificationResponse is not null;

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

            => "SignedFirmwareStatusNotificationResponse";

        #endregion

    }

}
