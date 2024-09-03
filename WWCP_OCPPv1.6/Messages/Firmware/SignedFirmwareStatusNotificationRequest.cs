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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The signed firmware status notification request.
    /// </summary>
    [SecurityExtensions]
    public class SignedFirmwareStatusNotificationRequest : ARequest<SignedFirmwareStatusNotificationRequest>,
                                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/signedFirmwareStatusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status of the firmware installation.
        /// </summary>
        public FirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed firmware status notification request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="Status">The status of the firmware installation.</param>
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
        public SignedFirmwareStatusNotificationRequest(NetworkingNode_Id              NetworkingNodeId,
                                                       FirmwareStatus                 Status,

                                                       IEnumerable<WWCP.KeyPair>?     SignKeys            = null,
                                                       IEnumerable<WWCP.SignInfo>?    SignInfos           = null,
                                                       IEnumerable<Signature>?   Signatures          = null,

                                                       CustomData?                    CustomData          = null,

                                                       Request_Id?                    RequestId           = null,
                                                       DateTime?                      RequestTimestamp    = null,
                                                       TimeSpan?                      RequestTimeout      = null,
                                                       EventTracking_Id?              EventTrackingId     = null,
                                                       NetworkPath?                   NetworkPath         = null,
                                                       CancellationToken              CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(SignedFirmwareStatusNotificationRequest)[..^7],

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

            this.Status = Status;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignedFirmwareStatusNotification.req",
        //   "definitions": {
        //     "FirmwareStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Downloaded",
        //         "DownloadFailed",
        //         "Downloading",
        //         "DownloadScheduled",
        //         "DownloadPaused",
        //         "Idle",
        //         "InstallationFailed",
        //         "Installing",
        //         "Installed",
        //         "InstallRebooting",
        //         "InstallScheduled",
        //         "InstallVerificationFailed",
        //         "InvalidSignature",
        //         "SignatureVerified"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/FirmwareStatusEnumType"
        //     },
        //     "requestId": {
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomSignedFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationRequestParser">An optional delegate to parse custom signed firmware status notification requests.</param>
        public static SignedFirmwareStatusNotificationRequest Parse(JObject                                                                JSON,
                                                                    Request_Id                                                             RequestId,
                                                                    NetworkingNode_Id                                                      NetworkingNodeId,
                                                                    NetworkPath                                                            NetworkPath,
                                                                    CustomJObjectParserDelegate<SignedFirmwareStatusNotificationRequest>?  CustomSignedFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var signedFirmwareStatusNotificationRequest,
                         out var errorResponse,
                         CustomSignedFirmwareStatusNotificationRequestParser) &&
                signedFirmwareStatusNotificationRequest is not null)
            {
                return signedFirmwareStatusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a signed firmware status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out SignedFirmwareStatusNotificationRequest, out ErrorResponse)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signed firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SignedFirmwareStatusNotificationRequest">The parsed signed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       Request_Id                                    RequestId,
                                       NetworkingNode_Id                             NetworkingNodeId,
                                       NetworkPath                                   NetworkPath,
                                       out SignedFirmwareStatusNotificationRequest?  SignedFirmwareStatusNotificationRequest,
                                       out String?                                   ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out SignedFirmwareStatusNotificationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signed firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SignedFirmwareStatusNotificationRequest">The parsed signed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationRequestParser">An optional delegate to parse custom signed firmware status notification requests.</param>
        public static Boolean TryParse(JObject                                                                JSON,
                                       Request_Id                                                             RequestId,
                                       NetworkingNode_Id                                                      NetworkingNodeId,
                                       NetworkPath                                                            NetworkPath,
                                       out SignedFirmwareStatusNotificationRequest?                           SignedFirmwareStatusNotificationRequest,
                                       out String?                                                            ErrorResponse,
                                       CustomJObjectParserDelegate<SignedFirmwareStatusNotificationRequest>?  CustomSignedFirmwareStatusNotificationRequestParser)
        {

            try
            {

                SignedFirmwareStatusNotificationRequest = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "firmware status",
                                       FirmwareStatusExtensions.Parse,
                                       out FirmwareStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

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


                SignedFirmwareStatusNotificationRequest = new SignedFirmwareStatusNotificationRequest(

                                                              NetworkingNodeId,
                                                              Status,

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

                if (CustomSignedFirmwareStatusNotificationRequestParser is not null)
                    SignedFirmwareStatusNotificationRequest = CustomSignedFirmwareStatusNotificationRequestParser(JSON,
                                                                                                                  SignedFirmwareStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                SignedFirmwareStatusNotificationRequest  = null;
                ErrorResponse                            = "The given JSON representation of a signed firmware status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedFirmwareStatusNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedFirmwareStatusNotificationRequestSerializer">A delegate to serialize custom signed firmware status notification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedFirmwareStatusNotificationRequest>?  CustomSignedFirmwareStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                           CustomSignatureSerializer                                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?                               CustomCustomDataSerializer                                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignedFirmwareStatusNotificationRequestSerializer is not null
                       ? CustomSignedFirmwareStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignedFirmwareStatusNotificationRequest1, SignedFirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two signed firmware status notification requests for equality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationRequest1">A signed firmware status notification request.</param>
        /// <param name="SignedFirmwareStatusNotificationRequest2">Another signed firmware status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignedFirmwareStatusNotificationRequest? SignedFirmwareStatusNotificationRequest1,
                                           SignedFirmwareStatusNotificationRequest? SignedFirmwareStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignedFirmwareStatusNotificationRequest1, SignedFirmwareStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SignedFirmwareStatusNotificationRequest1 is null || SignedFirmwareStatusNotificationRequest2 is null)
                return false;

            return SignedFirmwareStatusNotificationRequest1.Equals(SignedFirmwareStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (SignedFirmwareStatusNotificationRequest1, SignedFirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two signed firmware status notification requests for inequality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationRequest1">A signed firmware status notification request.</param>
        /// <param name="SignedFirmwareStatusNotificationRequest2">Another signed firmware status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignedFirmwareStatusNotificationRequest? SignedFirmwareStatusNotificationRequest1,
                                           SignedFirmwareStatusNotificationRequest? SignedFirmwareStatusNotificationRequest2)

            => !(SignedFirmwareStatusNotificationRequest1 == SignedFirmwareStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<SignedFirmwareStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed firmware status notification requests for equality.
        /// </summary>
        /// <param name="Object">A signed firmware status notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedFirmwareStatusNotificationRequest signedFirmwareStatusNotificationRequest &&
                   Equals(signedFirmwareStatusNotificationRequest);


        #endregion

        #region Equals(SignedFirmwareStatusNotificationRequest)

        /// <summary>
        /// Compares two signed firmware status notification requests for equality.
        /// </summary>
        /// <param name="SignedFirmwareStatusNotificationRequest">A signed firmware status notification request to compare with.</param>
        public override Boolean Equals(SignedFirmwareStatusNotificationRequest? SignedFirmwareStatusNotificationRequest)

            => SignedFirmwareStatusNotificationRequest is not null &&

               Status.     Equals(SignedFirmwareStatusNotificationRequest.Status) &&

               base.GenericEquals(SignedFirmwareStatusNotificationRequest);

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

                return Status.GetHashCode() * 3 ^
                       base.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
