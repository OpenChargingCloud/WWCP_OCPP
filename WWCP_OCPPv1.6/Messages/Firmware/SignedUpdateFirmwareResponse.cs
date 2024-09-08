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

using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A signed update firmware response.
    /// </summary>
    [SecurityExtensions]
    public class SignedUpdateFirmwareResponse : AResponse<CS.SignedUpdateFirmwareRequest,
                                                             SignedUpdateFirmwareResponse>,
                                                IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/signedUpdateFirmwareResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the signed update firmware request.
        /// </summary>
        public UpdateFirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed update firmware response.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        /// <param name="Status">The success or failure of the signed update firmware request.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignedUpdateFirmwareResponse(SignedUpdateFirmwareRequest  Request,
                                            UpdateFirmwareStatus         Status,

                                            Result?                      Result                = null,
                                            DateTime?                    ResponseTimestamp     = null,

                                            SourceRouting?               Destination           = null,
                                            NetworkPath?                 NetworkPath           = null,

                                            IEnumerable<KeyPair>?        SignKeys              = null,
                                            IEnumerable<SignInfo>?       SignInfos             = null,
                                            IEnumerable<Signature>?      Signatures            = null,

                                            CustomData?                  CustomData            = null,

                                            SerializationFormats?        SerializationFormat   = null,
                                            CancellationToken            CancellationToken     = default)

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

        {

            this.Status = Status;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignedUpdateFirmware.conf",
        //   "definitions": {
        //     "UpdateFirmwareStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "AcceptedCanceled",
        //         "InvalidCertificate",
        //         "RevokedCertificate"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/UpdateFirmwareStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomSignedUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed update firmware response.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignedUpdateFirmwareResponseParser">An optional delegate to parse custom signed update firmware responses.</param>
        public static SignedUpdateFirmwareResponse Parse(SignedUpdateFirmwareRequest                                 Request,
                                                         JObject                                                     JSON,
                                                         SourceRouting                                               Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   ResponseTimestamp                          = null,
                                                         CustomJObjectParserDelegate<SignedUpdateFirmwareResponse>?  CustomSignedUpdateFirmwareResponseParser   = null,
                                                         CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                                         CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var signedUpdateFirmwareResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSignedUpdateFirmwareResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return signedUpdateFirmwareResponse;
            }

            throw new ArgumentException("The given JSON representation of a signed update firmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SignedUpdateFirmwareResponse, out ErrorResponse, CustomSignedUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a signed update firmware response.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignedUpdateFirmwareResponse">The parsed signed update firmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedUpdateFirmwareResponseParser">An optional delegate to parse custom signed update firmware responses.</param>
        public static Boolean TryParse(SignedUpdateFirmwareRequest                                 Request,
                                       JObject                                                     JSON,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out SignedUpdateFirmwareResponse?      SignedUpdateFirmwareResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   ResponseTimestamp                          = null,
                                       CustomJObjectParserDelegate<SignedUpdateFirmwareResponse>?  CustomSignedUpdateFirmwareResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                       CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            try
            {

                SignedUpdateFirmwareResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "signed update firmware status",
                                       UpdateFirmwareStatusExtensions.Parse,
                                       out UpdateFirmwareStatus Status,
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


                SignedUpdateFirmwareResponse = new SignedUpdateFirmwareResponse(

                                                   Request,
                                                   Status,

                                                   null,
                                                   ResponseTimestamp,

                                                   Destination,
                                                   NetworkPath,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData

                                               );


                if (CustomSignedUpdateFirmwareResponseParser is not null)
                    SignedUpdateFirmwareResponse = CustomSignedUpdateFirmwareResponseParser(JSON,
                                                                                            SignedUpdateFirmwareResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignedUpdateFirmwareResponse  = null;
                ErrorResponse                 = "The given JSON representation of a signed update firmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedUpdateFirmwareResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedUpdateFirmwareResponseSerializer">A delegate to serialize custom signed update firmware responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedUpdateFirmwareResponse>?  CustomSignedUpdateFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
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

            return CustomSignedUpdateFirmwareResponseSerializer is not null
                       ? CustomSignedUpdateFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SignedUpdateFirmware failed because of a request error.
        /// </summary>
        /// <param name="Request">The SignedUpdateFirmware request.</param>
        public static SignedUpdateFirmwareResponse RequestError(SignedUpdateFirmwareRequest  Request,
                                                                EventTracking_Id             EventTrackingId,
                                                                ResultCode                   ErrorCode,
                                                                String?                      ErrorDescription    = null,
                                                                JObject?                     ErrorDetails        = null,
                                                                DateTime?                    ResponseTimestamp   = null,

                                                                SourceRouting?               Destination         = null,
                                                                NetworkPath?                 NetworkPath         = null,

                                                                IEnumerable<KeyPair>?        SignKeys            = null,
                                                                IEnumerable<SignInfo>?       SignInfos           = null,
                                                                IEnumerable<Signature>?      Signatures          = null,

                                                                CustomData?                  CustomData          = null)

            => new (

                   Request,
                   UpdateFirmwareStatus.Rejected,
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
        /// The SignedUpdateFirmware failed.
        /// </summary>
        /// <param name="Request">The SignedUpdateFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SignedUpdateFirmwareResponse FormationViolation(SignedUpdateFirmwareRequest  Request,
                                                                      String                       ErrorDescription)

            => new (Request,
                    UpdateFirmwareStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SignedUpdateFirmware failed.
        /// </summary>
        /// <param name="Request">The SignedUpdateFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SignedUpdateFirmwareResponse SignatureError(SignedUpdateFirmwareRequest  Request,
                                                                  String                       ErrorDescription)

            => new (Request,
                    UpdateFirmwareStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SignedUpdateFirmware failed.
        /// </summary>
        /// <param name="Request">The SignedUpdateFirmware request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SignedUpdateFirmwareResponse Failed(SignedUpdateFirmwareRequest  Request,
                                                          String?                      Description   = null)

            => new (Request,
                    UpdateFirmwareStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The SignedUpdateFirmware failed because of an exception.
        /// </summary>
        /// <param name="Request">The SignedUpdateFirmware request.</param>
        /// <param name="Exception">The exception.</param>
        public static SignedUpdateFirmwareResponse ExceptionOccured(SignedUpdateFirmwareRequest  Request,
                                                                    Exception                    Exception)

            => new (Request,
                    UpdateFirmwareStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SignedUpdateFirmwareResponse1, SignedUpdateFirmwareResponse2)

        /// <summary>
        /// Compares two signed update firmware responses for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareResponse1">A signed update firmware response.</param>
        /// <param name="SignedUpdateFirmwareResponse2">Another signed update firmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse1,
                                           SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignedUpdateFirmwareResponse1, SignedUpdateFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SignedUpdateFirmwareResponse1 is null || SignedUpdateFirmwareResponse2 is null)
                return false;

            return SignedUpdateFirmwareResponse1.Equals(SignedUpdateFirmwareResponse2);

        }

        #endregion

        #region Operator != (SignedUpdateFirmwareResponse1, SignedUpdateFirmwareResponse2)

        /// <summary>
        /// Compares two signed update firmware responses for inequality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareResponse1">A signed update firmware response.</param>
        /// <param name="SignedUpdateFirmwareResponse2">Another signed update firmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse1,
                                           SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse2)

            => !(SignedUpdateFirmwareResponse1 == SignedUpdateFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<SignedUpdateFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed update firmware responses for equality.
        /// </summary>
        /// <param name="Object">A signed update firmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedUpdateFirmwareResponse signedUpdateFirmwareResponse &&
                   Equals(signedUpdateFirmwareResponse);

        #endregion

        #region Equals(SignedUpdateFirmwareResponse)

        /// <summary>
        /// Compares two signed update firmware responses for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareResponse">A signed update firmware response to compare with.</param>
        public override Boolean Equals(SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse)

            => SignedUpdateFirmwareResponse is not null &&
                   Status.Equals(SignedUpdateFirmwareResponse.Status);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Status.GetHashCode();

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
