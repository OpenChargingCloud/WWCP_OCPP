/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The signed firmware status notification request.
    /// </summary>
    public class SignedFirmwareStatusNotificationRequest : ARequest<SignedFirmwareStatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The status of the firmware installation.
        /// </summary>
        public FirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed firmware status notification request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Status">The status of the firmware installation.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public SignedFirmwareStatusNotificationRequest(ChargeBox_Id        ChargeBoxId,
                                                       FirmwareStatus      Status,

                                                       Request_Id?         RequestId           = null,
                                                       DateTime?           RequestTimestamp    = null,
                                                       TimeSpan?           RequestTimeout      = null,
                                                       EventTracking_Id?   EventTrackingId     = null,
                                                       CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "SignedFirmwareStatusNotification",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
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

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSignedFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationRequestParser">A delegate to parse custom signed firmware status notification requests.</param>
        public static SignedFirmwareStatusNotificationRequest Parse(JObject                                                                JSON,
                                                                    Request_Id                                                             RequestId,
                                                                    ChargeBox_Id                                                           ChargeBoxId,
                                                                    CustomJObjectParserDelegate<SignedFirmwareStatusNotificationRequest>?  CustomSignedFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var signedFirmwareStatusNotificationRequest,
                         out var errorResponse,
                         CustomSignedFirmwareStatusNotificationRequestParser))
            {
                return signedFirmwareStatusNotificationRequest!;
            }

            throw new ArgumentException("The given JSON representation of a signed firmware status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SignedFirmwareStatusNotificationRequest, out ErrorResponse)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signed firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SignedFirmwareStatusNotificationRequest">The parsed signed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       Request_Id                                    RequestId,
                                       ChargeBox_Id                                  ChargeBoxId,
                                       out SignedFirmwareStatusNotificationRequest?  SignedFirmwareStatusNotificationRequest,
                                       out String?                                   ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SignedFirmwareStatusNotificationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signed firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SignedFirmwareStatusNotificationRequest">The parsed signed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedFirmwareStatusNotificationRequestParser">A delegate to parse custom signed firmware status notification requests.</param>
        public static Boolean TryParse(JObject                                                                JSON,
                                       Request_Id                                                             RequestId,
                                       ChargeBox_Id                                                           ChargeBoxId,
                                       out SignedFirmwareStatusNotificationRequest?                           SignedFirmwareStatusNotificationRequest,
                                       out String?                                                            ErrorResponse,
                                       CustomJObjectParserDelegate<SignedFirmwareStatusNotificationRequest>?  CustomSignedFirmwareStatusNotificationRequestParser)
        {

            try
            {

                SignedFirmwareStatusNotificationRequest = null;

                #region Status         [mandatory]

                if (!JSON.MapMandatory("status",
                                       "firmware status",
                                       FirmwareStatusExtentions.Parse,
                                       out FirmwareStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId    [optional, OCPP_CSE]

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


                SignedFirmwareStatusNotificationRequest = new SignedFirmwareStatusNotificationRequest(ChargeBoxId,
                                                                                                      Status,
                                                                                                      RequestId);

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

        #region ToJSON(CustomSignedFirmwareStatusNotificationRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedFirmwareStatusNotificationRequestSerializer">A delegate to serialize custom signed firmware status notification requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedFirmwareStatusNotificationRequest>? CustomSignedFirmwareStatusNotificationRequestSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
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
