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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The SignedUpdateFirmware request.
    /// </summary>
    [SecurityExtensions]
    public class SignedUpdateFirmwareRequest : ARequest<SignedUpdateFirmwareRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/signedUpdateFirmwareRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The firmware image to be installed on the Charge Point.
        /// </summary>
        public FirmwareImage   Firmware           { get; }

        /// <summary>
        /// The unique identification of this SignedUpdateFirmware request
        /// </summary>
        public Int32           UpdateRequestId    { get; }

        /// <summary>
        /// The optional number of retries of a charge point for trying to
        /// download the firmware before giving up. If this field is not
        /// present, it is left to the charge point to decide how many times
        /// it wants to retry.
        /// </summary>
        public Byte?           Retries            { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to charge point to decide how long to
        /// wait between attempts.
        /// </summary>
        public TimeSpan?       RetryInterval      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SignedUpdateFirmware request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Firmware">The firmware image to be installed on the Charge Point.</param>
        /// <param name="UpdateRequestId">The unique identification of this SignedUpdateFirmware request</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
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
        public SignedUpdateFirmwareRequest(SourceRouting            Destination,
                                           FirmwareImage            Firmware,
                                           Int32                    UpdateRequestId,
                                           Byte?                    Retries               = null,
                                           TimeSpan?                RetryInterval         = null,

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
                   nameof(SignedUpdateFirmwareRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat,
                   CancellationToken)

        {

            this.Firmware         = Firmware;
            this.UpdateRequestId  = UpdateRequestId;
            this.Retries          = Retries;
            this.RetryInterval    = RetryInterval;

            unchecked
            {

                hashCode = this.Firmware.       GetHashCode()       * 11 ^
                           this.UpdateRequestId.GetHashCode()       *  7 ^
                          (this.Retries?.       GetHashCode() ?? 0) *  5 ^
                          (this.RetryInterval?. GetHashCode() ?? 0) *  3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignedUpdateFirmware.req",
        //   "definitions": {
        //     "FirmwareType": {
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "location": {
        //           "type": "string",
        //           "maxLength": 512
        //         },
        //         "retrieveDateTime": {
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "installDateTime": {
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "signingCertificate": {
        //           "type": "string",
        //           "maxLength": 5500
        //         },
        //         "signature": {
        //           "type": "string",
        //           "maxLength": 800
        //         }
        //       },
        //       "required": [
        //         "location",
        //         "retrieveDateTime",
        //         "signingCertificate",
        //         "signature"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "retries": {
        //       "type": "integer"
        //     },
        //     "retryInterval": {
        //       "type": "integer"
        //     },
        //     "requestId": {
        //       "type": "integer"
        //     },
        //     "firmware": {
        //       "$ref": "#/definitions/FirmwareType"
        //     }
        //   },
        //   "required": [
        //     "requestId",
        //     "firmware"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomSignedUpdateFirmwareRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SignedUpdateFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSignedUpdateFirmwareRequestParser">An optional delegate to parse custom SignedUpdateFirmware requests.</param>
        public static SignedUpdateFirmwareRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        SourceRouting                                              Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  RequestTimestamp                          = null,
                                                        TimeSpan?                                                  RequestTimeout                            = null,
                                                        EventTracking_Id?                                          EventTrackingId                           = null,
                                                        CustomJObjectParserDelegate<SignedUpdateFirmwareRequest>?  CustomSignedUpdateFirmwareRequestParser   = null,
                                                        CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                                        CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var signedUpdateFirmwareRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSignedUpdateFirmwareRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return signedUpdateFirmwareRequest;
            }

            throw new ArgumentException("The given JSON representation of a SignedUpdateFirmware request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SignedUpdateFirmwareRequest, out ErrorResponse, CustomSignedUpdateFirmwareRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SignedUpdateFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SignedUpdateFirmwareRequest">The parsed SignedUpdateFirmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSignedUpdateFirmwareRequestParser">An optional delegate to parse custom SignedUpdateFirmware requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       SourceRouting                                              Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out SignedUpdateFirmwareRequest?      SignedUpdateFirmwareRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  RequestTimestamp                          = null,
                                       TimeSpan?                                                  RequestTimeout                            = null,
                                       EventTracking_Id?                                          EventTrackingId                           = null,
                                       CustomJObjectParserDelegate<SignedUpdateFirmwareRequest>?  CustomSignedUpdateFirmwareRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                       CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            try
            {

                SignedUpdateFirmwareRequest = null;

                #region Firmware           [mandatory]

                if (!JSON.ParseMandatoryJSON("firmware",
                                             "firmware image",
                                             FirmwareImage.TryParse,
                                             out FirmwareImage? Firmware,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region UpdateRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int32 UpdateRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Retries            [optional]

                if (JSON.ParseOptional("retries",
                                       "retries",
                                       out Byte? Retries,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RetryInterval      [optional]

                if (JSON.ParseOptional("retryInterval",
                                       "retry interval",
                                       out TimeSpan? RetryInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures         [optional, OCPP_CSE]

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

                #region CustomData         [optional]

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


                SignedUpdateFirmwareRequest = new SignedUpdateFirmwareRequest(

                                                  Destination,
                                                  Firmware,
                                                  UpdateRequestId,
                                                  Retries,
                                                  RetryInterval,

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

                if (CustomSignedUpdateFirmwareRequestParser is not null)
                    SignedUpdateFirmwareRequest = CustomSignedUpdateFirmwareRequestParser(JSON,
                                                                                          SignedUpdateFirmwareRequest);

                return true;

            }
            catch (Exception e)
            {
                SignedUpdateFirmwareRequest  = null;
                ErrorResponse                = "The given JSON representation of a SignedUpdateFirmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedUpdateFirmwareRequestSerializer = null, CustomFirmwareImageSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedUpdateFirmwareRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomFirmwareImageSerializer">A delegate to serialize custom firmware images.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedUpdateFirmwareRequest>?  CustomSignedUpdateFirmwareRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<FirmwareImage>?                CustomFirmwareImageSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("firmware",        Firmware.ToJSON(CustomFirmwareImageSerializer)),
                                 new JProperty("requestId",       UpdateRequestId),

                           Retries.HasValue
                               ? new JProperty("retries",         Retries.Value)
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",   (UInt64) RetryInterval.Value.TotalSeconds)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignedUpdateFirmwareRequestSerializer is not null
                       ? CustomSignedUpdateFirmwareRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignedUpdateFirmwareRequest1, SignedUpdateFirmwareRequest2)

        /// <summary>
        /// Compares two SignedUpdateFirmware requests for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareRequest1">A SignedUpdateFirmware request.</param>
        /// <param name="SignedUpdateFirmwareRequest2">Another SignedUpdateFirmware request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignedUpdateFirmwareRequest? SignedUpdateFirmwareRequest1,
                                           SignedUpdateFirmwareRequest? SignedUpdateFirmwareRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignedUpdateFirmwareRequest1, SignedUpdateFirmwareRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SignedUpdateFirmwareRequest1 is null || SignedUpdateFirmwareRequest2 is null)
                return false;

            return SignedUpdateFirmwareRequest1.Equals(SignedUpdateFirmwareRequest2);

        }

        #endregion

        #region Operator != (SignedUpdateFirmwareRequest1, SignedUpdateFirmwareRequest2)

        /// <summary>
        /// Compares two SignedUpdateFirmware requests for inequality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareRequest1">A SignedUpdateFirmware request.</param>
        /// <param name="SignedUpdateFirmwareRequest2">Another SignedUpdateFirmware request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignedUpdateFirmwareRequest? SignedUpdateFirmwareRequest1,
                                           SignedUpdateFirmwareRequest? SignedUpdateFirmwareRequest2)

            => !(SignedUpdateFirmwareRequest1 == SignedUpdateFirmwareRequest2);

        #endregion

        #endregion

        #region IEquatable<SignedUpdateFirmwareRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SignedUpdateFirmware requests for equality.
        /// </summary>
        /// <param name="Object">A SignedUpdateFirmware request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedUpdateFirmwareRequest signedUpdateFirmwareRequest &&
                   Equals(signedUpdateFirmwareRequest);

        #endregion

        #region Equals(SignedUpdateFirmwareRequest)

        /// <summary>
        /// Compares two SignedUpdateFirmware requests for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareRequest">A SignedUpdateFirmware request to compare with.</param>
        public override Boolean Equals(SignedUpdateFirmwareRequest? SignedUpdateFirmwareRequest)

            => SignedUpdateFirmwareRequest is not null &&

               Firmware.       Equals(SignedUpdateFirmwareRequest.Firmware)        &&
               UpdateRequestId.Equals(SignedUpdateFirmwareRequest.UpdateRequestId) &&

               ((!Retries.      HasValue && !SignedUpdateFirmwareRequest.Retries.      HasValue) ||
                 (Retries.      HasValue &&  SignedUpdateFirmwareRequest.Retries.      HasValue && Retries.      Value.Equals(SignedUpdateFirmwareRequest.Retries.      Value))) &&

               ((!RetryInterval.HasValue && !SignedUpdateFirmwareRequest.RetryInterval.HasValue) ||
                 (RetryInterval.HasValue &&  SignedUpdateFirmwareRequest.RetryInterval.HasValue && RetryInterval.Value.Equals(SignedUpdateFirmwareRequest.RetryInterval.Value))) &&

               base.    GenericEquals(SignedUpdateFirmwareRequest);

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

                   $"{Firmware.RemoteLocation.ToString().SubstringMax(20)}, id: {UpdateRequestId}",

                   Retries.HasValue
                       ? $", {Retries.Value} retries"
                       : "",

                   RetryInterval.HasValue
                       ? $", retry interval {RetryInterval.Value.TotalSeconds} sec(s)"
                       : ""

               );

        #endregion

    }

}
