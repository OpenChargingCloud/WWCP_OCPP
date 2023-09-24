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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// An install certificate response.
    /// </summary>
    public class InstallCertificateResponse : AResponse<CSMS.InstallCertificateRequest,
                                                        InstallCertificateResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the install certificate request.
        /// </summary>
        [Mandatory]
        public CertificateStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?        StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region InstallCertificateResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the install certificate request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public InstallCertificateResponse(CSMS.InstallCertificateRequest  Request,
                                          CertificateStatus               Status,
                                          StatusInfo?                     StatusInfo   = null,

                                          IEnumerable<Signature>?         Signatures   = null,
                                          CustomData?                     CustomData   = null)

            : base(Request,
                   Result.OK(),
                   Signatures,
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region InstallCertificateResponse(Request, Result)

        /// <summary>
        /// Create a new install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public InstallCertificateResponse(CSMS.InstallCertificateRequest  Request,
                                          Result                          Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:InstallCertificateResponse",
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
        //     },
        //     "InstallCertificateStatusEnumType": {
        //       "description": "Charging Station indicates if installation was successful.\r\n",
        //       "javaType": "InstallCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "Failed"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.\r\n",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "reasonCode"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/InstallCertificateStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomInstallCertificateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomInstallCertificateResponseParser">A delegate to parse custom install certificate responses.</param>
        public static InstallCertificateResponse Parse(CSMS.InstallCertificateRequest                            Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<InstallCertificateResponse>?  CustomInstallCertificateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var installCertificateResponse,
                         out var errorResponse,
                         CustomInstallCertificateResponseParser))
            {
                return installCertificateResponse!;
            }

            throw new ArgumentException("The given JSON representation of an install certificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out InstallCertificateResponse, out ErrorResponse, CustomInstallCertificateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="InstallCertificateResponse">The parsed install certificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomInstallCertificateResponseParser">A delegate to parse custom install certificate responses.</param>
        public static Boolean TryParse(CSMS.InstallCertificateRequest                            Request,
                                       JObject                                                   JSON,
                                       out InstallCertificateResponse?                           InstallCertificateResponse,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<InstallCertificateResponse>?  CustomInstallCertificateResponseParser   = null)
        {

            try
            {

                InstallCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "install certificate status",
                                         CertificateStatusExtensions.TryParse,
                                         out CertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                InstallCertificateResponse = new InstallCertificateResponse(
                                                 Request,
                                                 Status,
                                                 StatusInfo,
                                                 Signatures,
                                                 CustomData
                                             );

                if (CustomInstallCertificateResponseParser is not null)
                    InstallCertificateResponse = CustomInstallCertificateResponseParser(JSON,
                                                                                        InstallCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                InstallCertificateResponse  = null;
                ErrorResponse               = "The given JSON representation of an install certificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomInstallCertificateResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomInstallCertificateResponseSerializer">A delegate to serialize custom install certificate responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<InstallCertificateResponse>?  CustomInstallCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                  CustomStatusInfoSerializer                   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomInstallCertificateResponseSerializer is not null
                       ? CustomInstallCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The install certificate failed.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        public static InstallCertificateResponse Failed(CSMS.InstallCertificateRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (InstallCertificateResponse1, InstallCertificateResponse2)

        /// <summary>
        /// Compares two install certificate responses for equality.
        /// </summary>
        /// <param name="InstallCertificateResponse1">An install certificate response.</param>
        /// <param name="InstallCertificateResponse2">Another install certificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (InstallCertificateResponse? InstallCertificateResponse1,
                                           InstallCertificateResponse? InstallCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(InstallCertificateResponse1, InstallCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (InstallCertificateResponse1 is null || InstallCertificateResponse2 is null)
                return false;

            return InstallCertificateResponse1.Equals(InstallCertificateResponse2);

        }

        #endregion

        #region Operator != (InstallCertificateResponse1, InstallCertificateResponse2)

        /// <summary>
        /// Compares two install certificate responses for inequality.
        /// </summary>
        /// <param name="InstallCertificateResponse1">An install certificate response.</param>
        /// <param name="InstallCertificateResponse2">Another install certificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (InstallCertificateResponse? InstallCertificateResponse1,
                                           InstallCertificateResponse? InstallCertificateResponse2)

            => !(InstallCertificateResponse1 == InstallCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<InstallCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two install certificate responses for equality.
        /// </summary>
        /// <param name="Object">An install certificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InstallCertificateResponse installCertificateResponse &&
                   Equals(installCertificateResponse);

        #endregion

        #region Equals(InstallCertificateResponse)

        /// <summary>
        /// Compares two install certificate responses for equality.
        /// </summary>
        /// <param name="InstallCertificateResponse">An install certificate response to compare with.</param>
        public override Boolean Equals(InstallCertificateResponse? InstallCertificateResponse)

            => InstallCertificateResponse is not null &&

               Status.Equals(InstallCertificateResponse.Status) &&

             ((StatusInfo is     null && InstallCertificateResponse.StatusInfo is     null) ||
               StatusInfo is not null && InstallCertificateResponse.StatusInfo is not null && StatusInfo.Equals(InstallCertificateResponse.StatusInfo)) &&

               base.GenericEquals(InstallCertificateResponse);

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

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.AsText();

        #endregion

    }

}
