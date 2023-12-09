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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The signed update firmware request.
    /// </summary>
    public class SignedUpdateFirmwareRequest : ARequest<SignedUpdateFirmwareRequest>
    {

        #region Properties

        /// <summary>
        /// The firmware image to be installed on the Charge Point.
        /// </summary>
        public FirmwareImage  Firmware           { get; }

        /// <summary>
        /// The unique identification of this signed update firmware request
        /// </summary>
        public Int32          UpdateRequestId    { get; }

        /// <summary>
        /// The optional number of retries of a charge point for trying to
        /// download the firmware before giving up. If this field is not
        /// present, it is left to the charge point to decide how many times
        /// it wants to retry.
        /// </summary>
        public Byte?          Retries            { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to charge point to decide how long to
        /// wait between attempts.
        /// </summary>
        public TimeSpan?      RetryInterval      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed update firmware request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Firmware">The firmware image to be installed on the Charge Point.</param>
        /// <param name="UpdateRequestId">The unique identification of this signed update firmware request</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public SignedUpdateFirmwareRequest(ChargeBox_Id       ChargeBoxId,
                                           FirmwareImage      Firmware,
                                           Int32              UpdateRequestId,
                                           Byte?              Retries             = null,
                                           TimeSpan?          RetryInterval       = null,

                                           Request_Id?        RequestId           = null,
                                           DateTime?          RequestTimestamp    = null,
                                           TimeSpan?          RequestTimeout      = null,
                                           EventTracking_Id?  EventTrackingId     = null,
                                           CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "SignedUpdateFirmware",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.Firmware         = Firmware;
            this.UpdateRequestId  = UpdateRequestId;
            this.Retries          = Retries;
            this.RetryInterval    = RetryInterval;

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

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSignedUpdateFirmwareRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed update firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSignedUpdateFirmwareRequestParser">A delegate to parse custom signed update firmware requests.</param>
        public static SignedUpdateFirmwareRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        ChargeBox_Id                                               ChargeBoxId,
                                                        CustomJObjectParserDelegate<SignedUpdateFirmwareRequest>?  CustomSignedUpdateFirmwareRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var signedUpdateFirmwareRequest,
                         out var errorResponse,
                         CustomSignedUpdateFirmwareRequestParser))
            {
                return signedUpdateFirmwareRequest!;
            }

            throw new ArgumentException("The given JSON representation of a signed update firmware request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SignedUpdateFirmwareRequest, out ErrorResponse, CustomSignedUpdateFirmwareRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signed update firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SignedUpdateFirmwareRequest">The parsed signed update firmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       Request_Id                        RequestId,
                                       ChargeBox_Id                      ChargeBoxId,
                                       out SignedUpdateFirmwareRequest?  SignedUpdateFirmwareRequest,
                                       out String?                       ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SignedUpdateFirmwareRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signed update firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SignedUpdateFirmwareRequest">The parsed signed update firmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedUpdateFirmwareRequestParser">A delegate to parse custom signed update firmware requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       ChargeBox_Id                                               ChargeBoxId,
                                       out SignedUpdateFirmwareRequest?                           SignedUpdateFirmwareRequest,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<SignedUpdateFirmwareRequest>?  CustomSignedUpdateFirmwareRequestParser)
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

                if (Firmware is null)
                    return false;

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

                #region ChargeBoxId        [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                SignedUpdateFirmwareRequest = new SignedUpdateFirmwareRequest(ChargeBoxId,
                                                                              Firmware,
                                                                              UpdateRequestId,
                                                                              Retries,
                                                                              RetryInterval,
                                                                              RequestId);

                if (CustomSignedUpdateFirmwareRequestParser is not null)
                    SignedUpdateFirmwareRequest = CustomSignedUpdateFirmwareRequestParser(JSON,
                                                                                          SignedUpdateFirmwareRequest);

                return true;

            }
            catch (Exception e)
            {
                SignedUpdateFirmwareRequest  = null;
                ErrorResponse                = "The given JSON representation of a signed update firmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedUpdateFirmwareRequestSerializer = null, CustomFirmwareImageSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedUpdateFirmwareRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomFirmwareImageSerializer">A delegate to serialize custom firmware images.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedUpdateFirmwareRequest>?  CustomSignedUpdateFirmwareRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<FirmwareImage>?                CustomFirmwareImageSerializer                 = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?               CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           new JProperty("firmware",             Firmware.ToJSON(CustomFirmwareImageSerializer)),
                           new JProperty("requestId",            UpdateRequestId),

                           Retries.HasValue
                               ? new JProperty("retries",        Retries.Value)
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",  (UInt64) RetryInterval.Value.TotalSeconds)
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
        /// Compares two signed update firmware requests for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareRequest1">A signed update firmware request.</param>
        /// <param name="SignedUpdateFirmwareRequest2">Another signed update firmware request.</param>
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
        /// Compares two signed update firmware requests for inequality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareRequest1">A signed update firmware request.</param>
        /// <param name="SignedUpdateFirmwareRequest2">Another signed update firmware request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignedUpdateFirmwareRequest? SignedUpdateFirmwareRequest1,
                                           SignedUpdateFirmwareRequest? SignedUpdateFirmwareRequest2)

            => !(SignedUpdateFirmwareRequest1 == SignedUpdateFirmwareRequest2);

        #endregion

        #endregion

        #region IEquatable<SignedUpdateFirmwareRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed update firmware requests for equality.
        /// </summary>
        /// <param name="Object">A signed update firmware request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedUpdateFirmwareRequest signedUpdateFirmwareRequest &&
                   Equals(signedUpdateFirmwareRequest);

        #endregion

        #region Equals(SignedUpdateFirmwareRequest)

        /// <summary>
        /// Compares two signed update firmware requests for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareRequest">A signed update firmware request to compare with.</param>
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

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Firmware.       GetHashCode()       * 11 ^
                       UpdateRequestId.GetHashCode()       *  7 ^

                      (Retries?.       GetHashCode() ?? 0) *  5 ^
                      (RetryInterval?. GetHashCode() ?? 0) *  3 ^

                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Firmware.RemoteLocation.ToString().SubstringMax(20),

                             ", id: ", UpdateRequestId,

                             Retries.HasValue
                                 ? ", " + Retries.Value + " retries"
                                 : "",

                             RetryInterval.HasValue
                                 ? ", retry interval " + RetryInterval.Value.TotalSeconds + " sec(s)"
                                 : "");

        #endregion

    }

}
