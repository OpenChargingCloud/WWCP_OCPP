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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The get 15118 EV certificate response.
    /// </summary>
    public class Get15118EVCertificateResponse : AResponse<CS.Get15118EVCertificateRequest,
                                                           Get15118EVCertificateResponse>,
                                                 IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/get15118EVCertificateResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the EXI message processing.
        /// </summary>
        [Mandatory]
        public ISO15118EVCertificateStatus  Status                { get; }

        /// <summary>
        /// Base64 encoded certificate installation response to the electric vehicle.
        /// [max 5600]
        /// </summary>
        [Mandatory]
        public EXIData                      EXIResponse           { get; }

        /// <summary>
        /// The number of contracts that can be retrieved with additional requests.
        /// </summary>
        [Optional]
        public UInt32?                      RemainingContracts    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                  StatusInfo            { get; }

        #endregion

        #region Constructor(s)

        #region Get15118EVCertificateResponse(Request, Status, EXIResponse, RemainingContracts = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the EXI message processing.</param>
        /// <param name="EXIResponse">Base64 encoded certificate installation response to the electric vehicle.</param>
        /// <param name="RemainingContracts">The number of contracts that can be retrieved with additional requests.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Get15118EVCertificateResponse(CS.Get15118EVCertificateRequest  Request,
                                             ISO15118EVCertificateStatus      Status,
                                             EXIData                          EXIResponse,
                                             UInt32?                          RemainingContracts   = null,
                                             StatusInfo?                      StatusInfo           = null,
                                             DateTime?                        ResponseTimestamp    = null,

                                             IEnumerable<KeyPair>?            SignKeys             = null,
                                             IEnumerable<SignInfo>?           SignInfos            = null,
                                             IEnumerable<OCPP.Signature>?     Signatures           = null,

                                             CustomData?                      CustomData           = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status              = Status;
            this.EXIResponse         = EXIResponse;
            this.RemainingContracts  = RemainingContracts;
            this.StatusInfo          = StatusInfo;

        }

        #endregion

        #region Get15118EVCertificateResponse(Request, Result)

        /// <summary>
        /// Create a new get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public Get15118EVCertificateResponse(CS.Get15118EVCertificateRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:Get15118EVCertificateResponse",
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
        //     "Iso15118EVCertificateStatusEnumType": {
        //       "description": "Indicates whether the message was processed properly.\r\n",
        //       "javaType": "Iso15118EVCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
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
        //       "$ref": "#/definitions/Iso15118EVCertificateStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "exiResponse": {
        //       "description": "Raw CertificateInstallationRes response for the EV, Base64 encoded.\r\n",
        //       "type": "string",
        //       "maxLength": 5600
        //     }
        //   },
        //   "required": [
        //     "status",
        //     "exiResponse"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGet15118EVCertificateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGet15118EVCertificateResponseParser">A delegate to parse custom get 15118 EV certificate responses.</param>
        public static Get15118EVCertificateResponse Parse(CS.Get15118EVCertificateRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var get15118EVCertificateResponse,
                         out var errorResponse,
                         CustomGet15118EVCertificateResponseParser))
            {
                return get15118EVCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of a get 15118 EV certificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out Get15118EVCertificateResponse, out ErrorResponse, CustomGet15118EVCertificateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get 15118 EV certificate response.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Get15118EVCertificateResponse">The parsed get 15118 EV certificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGet15118EVCertificateResponseParser">A delegate to parse custom get 15118 EV certificate responses.</param>
        public static Boolean TryParse(CS.Get15118EVCertificateRequest                              Request,
                                       JObject                                                      JSON,
                                       [NotNullWhen(true)]  out Get15118EVCertificateResponse?      Get15118EVCertificateResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseParser   = null)
        {

            try
            {

                Get15118EVCertificateResponse = null;

                #region Status                [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "ISO 15118 EV certificate status",
                                         ISO15118EVCertificateStatusExtensions.TryParse,
                                         out ISO15118EVCertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EXIRequest            [mandatory]

                if (!JSON.ParseMandatory("exiResponse",
                                         "EXI response",
                                         EXIData.TryParse,
                                         out EXIData EXIRequest,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region RemainingContracts    [optional]

                if (JSON.ParseOptional("remainingContracts",
                                       "remaining contracts",
                                       out UInt32? RemainingContracts,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo            [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPP.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Get15118EVCertificateResponse = new Get15118EVCertificateResponse(
                                                    Request,
                                                    Status,
                                                    EXIRequest,
                                                    RemainingContracts,
                                                    StatusInfo,
                                                    null,
                                                    null,
                                                    null,
                                                    Signatures,
                                                    CustomData
                                                );

                if (CustomGet15118EVCertificateResponseParser is not null)
                    Get15118EVCertificateResponse = CustomGet15118EVCertificateResponseParser(JSON,
                                                                                              Get15118EVCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                Get15118EVCertificateResponse  = null;
                ErrorResponse                  = "The given JSON representation of a get 15118 EV certificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGet15118EVCertificateResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGet15118EVCertificateResponseSerializer">A delegate to serialize custom get 15118 EV certificate responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                 CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",               Status.     AsText()),
                                 new JProperty("exiResponse",          EXIResponse.ToString()),

                           RemainingContracts is not null
                               ? new JProperty("remainingContracts",   RemainingContracts.Value)
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",           StatusInfo. ToJSON(CustomStatusInfoSerializer,
                                                                                          CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGet15118EVCertificateResponseSerializer is not null
                       ? CustomGet15118EVCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get 15118 EV certificate failed.
        /// </summary>
        /// <param name="Request">The get 15118 EV certificate request leading to this response.</param>
        public static Get15118EVCertificateResponse Failed(CS.Get15118EVCertificateRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (Get15118EVCertificateResponse1, Get15118EVCertificateResponse2)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse1">A get 15118 EV certificate response.</param>
        /// <param name="Get15118EVCertificateResponse2">Another get 15118 EV certificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Get15118EVCertificateResponse? Get15118EVCertificateResponse1,
                                           Get15118EVCertificateResponse? Get15118EVCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Get15118EVCertificateResponse1, Get15118EVCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (Get15118EVCertificateResponse1 is null || Get15118EVCertificateResponse2 is null)
                return false;

            return Get15118EVCertificateResponse1.Equals(Get15118EVCertificateResponse2);

        }

        #endregion

        #region Operator != (Get15118EVCertificateResponse1, Get15118EVCertificateResponse2)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for inequality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse1">A get 15118 EV certificate response.</param>
        /// <param name="Get15118EVCertificateResponse2">Another get 15118 EV certificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Get15118EVCertificateResponse? Get15118EVCertificateResponse1,
                                           Get15118EVCertificateResponse? Get15118EVCertificateResponse2)

            => !(Get15118EVCertificateResponse1 == Get15118EVCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<Get15118EVCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for equality.
        /// </summary>
        /// <param name="Object">A get 15118 EV certificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Get15118EVCertificateResponse get15118EVCertificateResponse &&
                   Equals(get15118EVCertificateResponse);

        #endregion

        #region Equals(Get15118EVCertificateResponse)

        /// <summary>
        /// Compares two get 15118 EV certificate responses for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse">A get 15118 EV certificate response to compare with.</param>
        public override Boolean Equals(Get15118EVCertificateResponse? Get15118EVCertificateResponse)

            => Get15118EVCertificateResponse is not null &&

               Status.     Equals(Get15118EVCertificateResponse.Status)      &&
               EXIResponse.Equals(Get15118EVCertificateResponse.EXIResponse) &&

             ((RemainingContracts is     null && Get15118EVCertificateResponse.RemainingContracts is null) ||
               RemainingContracts is not null && Get15118EVCertificateResponse.RemainingContracts is not null && RemainingContracts.Value.Equals(Get15118EVCertificateResponse.RemainingContracts.Value)) &&

             ((StatusInfo         is     null && Get15118EVCertificateResponse.StatusInfo         is     null) ||
               StatusInfo         is not null && Get15118EVCertificateResponse.StatusInfo         is not null && StatusInfo.              Equals(Get15118EVCertificateResponse.StatusInfo)) &&

               base.GenericEquals(Get15118EVCertificateResponse);

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

                return Status.             GetHashCode()       * 11 ^
                       EXIResponse.        GetHashCode()       *  7 ^
                      (RemainingContracts?.GetHashCode() ?? 0) *  5 ^
                      (StatusInfo?.        GetHashCode() ?? 0) *  3 ^

                       base.               GetHashCode();

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
