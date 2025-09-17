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
    /// A signed SignedFirmwareStatusNotification response.
    /// </summary>
    [SecurityExtensions]
    public class SignedFirmwareStatusNotificationResponse : AResponse<SignedFirmwareStatusNotificationRequest,
                                                                      SignedFirmwareStatusNotificationResponse>,
                                                            IResponse<Result>
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

        /// <summary>
        /// Create a new SignedFirmwareStatusNotification response.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request request leading to this response.</param>
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
        public SignedFirmwareStatusNotificationResponse(SignedFirmwareStatusNotificationRequest  Request,

                                                        Result?                                  Result                = null,
                                                        DateTimeOffset?                          ResponseTimestamp     = null,

                                                        SourceRouting?                           Destination           = null,
                                                        NetworkPath?                             NetworkPath           = null,

                                                        IEnumerable<KeyPair>?                    SignKeys              = null,
                                                        IEnumerable<SignInfo>?                   SignInfos             = null,
                                                        IEnumerable<Signature>?                  Signatures            = null,

                                                        CustomData?                              CustomData            = null,

                                                        SerializationFormats?                    SerializationFormat   = null,
                                                        CancellationToken                        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        { }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignedFirmwareStatusNotification.conf",
        //   "type": "object",
        //   "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SignedFirmwareStatusNotification response.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationResponseParser">An optional delegate to parse custom SignedFirmwareStatusNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static SignedFirmwareStatusNotificationResponse Parse(SignedFirmwareStatusNotificationRequest                                 Request,
                                                                     JObject                                                                 JSON,
                                                                     SourceRouting                                                           Destination,
                                                                     NetworkPath                                                             NetworkPath,
                                                                     DateTimeOffset?                                                         ResponseTimestamp                                              = null,
                                                                     CustomJObjectParserDelegate<SignedFirmwareStatusNotificationResponse>?  CustomSignedFirmwareStatusNotificationResponseResponseParser   = null,
                                                                     CustomJObjectParserDelegate<Signature>?                                 CustomSignatureParser                                          = null,
                                                                     CustomJObjectParserDelegate<CustomData>?                                CustomCustomDataParser                                         = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var signedFirmwareStatusNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSignedFirmwareStatusNotificationResponseResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return signedFirmwareStatusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a SignedFirmwareStatusNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SignedFirmwareStatusNotificationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SignedFirmwareStatusNotification response.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="SignedFirmwareStatusNotificationResponse">The parsed SignedFirmwareStatusNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationResponseParser">An optional delegate to parse custom SignedFirmwareStatusNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(SignedFirmwareStatusNotificationRequest                                 Request,
                                       JObject                                                                 JSON,
                                       SourceRouting                                                           Destination,
                                       NetworkPath                                                             NetworkPath,
                                       [NotNullWhen(true)]  out SignedFirmwareStatusNotificationResponse?      SignedFirmwareStatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                                        ErrorResponse,
                                       DateTimeOffset?                                                         ResponseTimestamp                                              = null,
                                       CustomJObjectParserDelegate<SignedFirmwareStatusNotificationResponse>?  CustomSignedFirmwareStatusNotificationResponseResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                                 CustomSignatureParser                                          = null,
                                       CustomJObjectParserDelegate<CustomData>?                                CustomCustomDataParser                                         = null)
        {

            try
            {

                SignedFirmwareStatusNotificationResponse = null;

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


                SignedFirmwareStatusNotificationResponse = new SignedFirmwareStatusNotificationResponse(

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

                if (CustomSignedFirmwareStatusNotificationResponseResponseParser is not null)
                    SignedFirmwareStatusNotificationResponse = CustomSignedFirmwareStatusNotificationResponseResponseParser(JSON,
                                                                                                                            SignedFirmwareStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                SignedFirmwareStatusNotificationResponse  = null;
                ErrorResponse                             = "The given JSON representation of a SignedFirmwareStatusNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedFirmwareStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedFirmwareStatusNotificationResponseSerializer">A delegate to serialize custom SignedFirmwareStatusNotification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedFirmwareStatusNotificationResponse>?  CustomSignedFirmwareStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                                 CustomSignatureSerializer                                  = null,
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
        /// The SignedFirmwareStatusNotification request failed because of a request error.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request request.</param>
        public static SignedFirmwareStatusNotificationResponse RequestError(SignedFirmwareStatusNotificationRequest  Request,
                                                                            EventTracking_Id                         EventTrackingId,
                                                                            ResultCode                               ErrorCode,
                                                                            String?                                  ErrorDescription    = null,
                                                                            JObject?                                 ErrorDetails        = null,
                                                                            DateTimeOffset?                          ResponseTimestamp   = null,

                                                                            SourceRouting?                           Destination         = null,
                                                                            NetworkPath?                             NetworkPath         = null,

                                                                            IEnumerable<KeyPair>?                    SignKeys            = null,
                                                                            IEnumerable<SignInfo>?                   SignInfos           = null,
                                                                            IEnumerable<Signature>?                  Signatures          = null,

                                                                            CustomData?                              CustomData          = null)

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
        /// The SignedFirmwareStatusNotification request failed.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SignedFirmwareStatusNotificationResponse FormationViolation(SignedFirmwareStatusNotificationRequest Request,
                                                                                  String                                  ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The SignedFirmwareStatusNotification request failed.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SignedFirmwareStatusNotificationResponse SignatureError(SignedFirmwareStatusNotificationRequest  Request,
                                                                              String                                   ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The SignedFirmwareStatusNotification request failed.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SignedFirmwareStatusNotificationResponse Failed(SignedFirmwareStatusNotificationRequest  Request,
                                                                      String?                                  Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The SignedFirmwareStatusNotification request failed because of an exception.
        /// </summary>
        /// <param name="Request">The SignedFirmwareStatusNotification request request.</param>
        /// <param name="Exception">The exception.</param>
        public static SignedFirmwareStatusNotificationResponse ExceptionOccurred(SignedFirmwareStatusNotificationRequest  Request,
                                                                                Exception                                Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SignedFirmwareStatusNotificationResponse1, SignedFirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two SignedFirmwareStatusNotification responses for equality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationResponse1">A SignedFirmwareStatusNotification response.</param>
        /// <param name="SignedFirmwareStatusNotificationResponse2">Another SignedFirmwareStatusNotification response.</param>
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
        /// Compares two SignedFirmwareStatusNotification responses for inequality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationResponse1">A SignedFirmwareStatusNotification response.</param>
        /// <param name="SignedFirmwareStatusNotificationResponse2">Another SignedFirmwareStatusNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse1,
                                           SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse2)

            => !(SignedFirmwareStatusNotificationResponse1 == SignedFirmwareStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<SignedFirmwareStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SignedFirmwareStatusNotification responses for equality.
        /// </summary>
        /// <param name="Object">A SignedFirmwareStatusNotification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedFirmwareStatusNotificationResponse signedFirmwareStatusNotificationResponse &&
                   Equals(signedFirmwareStatusNotificationResponse);

        #endregion

        #region Equals(SignedFirmwareStatusNotificationResponse)

        /// <summary>
        /// Compares two SignedFirmwareStatusNotification responses for equality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationResponse">A SignedFirmwareStatusNotification response to compare with.</param>
        public override Boolean Equals(SignedFirmwareStatusNotificationResponse? SignedFirmwareStatusNotificationResponse)

            => SignedFirmwareStatusNotificationResponse is not null;

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

            => "SignedFirmwareStatusNotificationResponse";

        #endregion

    }

}
