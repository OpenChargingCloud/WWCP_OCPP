﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// An update firmware response.
    /// </summary>
    public class UpdateFirmwareResponse : AResponse<CSMS.UpdateFirmwareRequest,
                                                    UpdateFirmwareResponse>,
                                          IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/updateFirmwareResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the update firmware command.
        /// </summary>
        [Mandatory]
        public UpdateFirmwareStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?           StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region UpdateFirmwareResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="Status">The success or failure of the update firmware command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public UpdateFirmwareResponse(CSMS.UpdateFirmwareRequest    Request,
                                      UpdateFirmwareStatus          Status,
                                      StatusInfo?                   StatusInfo          = null,
                                      DateTime?                     ResponseTimestamp   = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

                                      CustomData?                   CustomData          = null)

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

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region UpdateFirmwareResponse(Request, Result)

        /// <summary>
        /// Create a new update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UpdateFirmwareResponse(CSMS.UpdateFirmwareRequest  Request,
                                      Result                      Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UpdateFirmwareResponse",
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
        //     "UpdateFirmwareStatusEnumType": {
        //       "description": "This field indicates whether the Charging Station was able to accept the request.\r\n\r\n",
        //       "javaType": "UpdateFirmwareStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "AcceptedCanceled",
        //         "InvalidCertificate",
        //         "RevokedCertificate"
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
        //       "$ref": "#/definitions/UpdateFirmwareStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUpdateFirmwareResponseParser">A delegate to parse custom update firmware responses.</param>
        public static UpdateFirmwareResponse Parse(CSMS.UpdateFirmwareRequest                            Request,
                                                   JObject                                               JSON,
                                                   CustomJObjectParserDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var updateFirmwareResponse,
                         out var errorResponse,
                         CustomUpdateFirmwareResponseParser))
            {
                return updateFirmwareResponse;
            }

            throw new ArgumentException("The given JSON representation of a update firmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UpdateFirmwareResponse, out ErrorResponse, CustomUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UpdateFirmwareResponse">The parsed update firmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateFirmwareResponseParser">A delegate to parse custom update firmware responses.</param>
        public static Boolean TryParse(CSMS.UpdateFirmwareRequest                            Request,
                                       JObject                                               JSON,
                                       [NotNullWhen(true)]  out UpdateFirmwareResponse?      UpdateFirmwareResponse,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseParser   = null)
        {

            try
            {

                UpdateFirmwareResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "update firmware status",
                                         UpdateFirmwareStatus.TryParse,
                                         out UpdateFirmwareStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UpdateFirmwareResponse = new UpdateFirmwareResponse(
                                             Request,
                                             Status,
                                             StatusInfo,
                                             null,
                                             null,
                                             null,
                                             Signatures,
                                             CustomData
                                         );

                if (CustomUpdateFirmwareResponseParser is not null)
                    UpdateFirmwareResponse = CustomUpdateFirmwareResponseParser(JSON,
                                                                                UpdateFirmwareResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareResponse  = null;
                ErrorResponse           = "The given JSON representation of an update firmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateFirmwareResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateFirmwareResponseSerializer">A delegate to serialize custom update firmware responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?              CustomStatusInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?          CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
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

            return CustomUpdateFirmwareResponseSerializer is not null
                       ? CustomUpdateFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The update firmware command failed.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        public static UpdateFirmwareResponse Failed(CSMS.UpdateFirmwareRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">An update firmware response.</param>
        /// <param name="UpdateFirmwareResponse2">Another update firmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateFirmwareResponse? UpdateFirmwareResponse1,
                                           UpdateFirmwareResponse? UpdateFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareResponse1, UpdateFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateFirmwareResponse1 is null || UpdateFirmwareResponse2 is null)
                return false;

            return UpdateFirmwareResponse1.Equals(UpdateFirmwareResponse2);

        }

        #endregion

        #region Operator != (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two update firmware responses for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">An update firmware response.</param>
        /// <param name="UpdateFirmwareResponse2">Another update firmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateFirmwareResponse? UpdateFirmwareResponse1,
                                           UpdateFirmwareResponse? UpdateFirmwareResponse2)

            => !(UpdateFirmwareResponse1 == UpdateFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="Object">An update firmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateFirmwareResponse updateFirmwareResponse &&
                   Equals(updateFirmwareResponse);

        #endregion

        #region Equals(UpdateFirmwareResponse)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse">An update firmware response to compare with.</param>
        public override Boolean Equals(UpdateFirmwareResponse? UpdateFirmwareResponse)

            => UpdateFirmwareResponse is not null &&

               Status.     Equals(UpdateFirmwareResponse.Status) &&

             ((StatusInfo is     null && UpdateFirmwareResponse.StatusInfo is     null) ||
               StatusInfo is not null && UpdateFirmwareResponse.StatusInfo is not null && StatusInfo.Equals(UpdateFirmwareResponse.StatusInfo)) &&

               base.GenericEquals(UpdateFirmwareResponse);

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

            => "UpdateFirmwareResponse";

        #endregion

    }

}
