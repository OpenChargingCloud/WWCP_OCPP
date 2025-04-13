﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The ChangeTransactionTariff response.
    /// </summary>
    public class ChangeTransactionTariffResponse : AResponse<ChangeTransactionTariffRequest,
                                                             ChangeTransactionTariffResponse>,
                                                   IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getTariffsResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The ChangeTransactionTariff status.
        /// </summary>
        [Mandatory]
        public TariffChangeStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the ChangeTransactionTariff status.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChangeTransactionTariff response.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request leading to this response.</param>
        /// <param name="Status">A TariffChange status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the ChangeTransactionTariff status.</param>
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
        public ChangeTransactionTariffResponse(ChangeTransactionTariffRequest  Request,
                                               TariffChangeStatus              Status,
                                               StatusInfo?                     StatusInfo            = null,

                                               Result?                         Result                = null,
                                               DateTime?                       ResponseTimestamp     = null,

                                               SourceRouting?                  Destination           = null,
                                               NetworkPath?                    NetworkPath           = null,

                                               IEnumerable<KeyPair>?           SignKeys              = null,
                                               IEnumerable<SignInfo>?          SignInfos             = null,
                                               IEnumerable<Signature>?         Signatures            = null,

                                               CustomData?                     CustomData            = null,

                                               SerializationFormats?           SerializationFormat   = null,
                                               CancellationToken               CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ChangeTransactionTariffResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "TariffChangeStatusEnumType": {
        //             "description": "Status of the operation",
        //             "javaType": "TariffChangeStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "TooManyElements",
        //                 "ConditionNotSupported",
        //                 "TxNotFound",
        //                 "NoCurrencyChange"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "status": {
        //             "$ref": "#/definitions/TariffChangeStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ChangeTransactionTariff response.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChangeTransactionTariffResponseParser">A delegate to parse custom ChangeTransactionTariff responses.</param>
        public static ChangeTransactionTariffResponse Parse(ChangeTransactionTariffRequest                                 Request,
                                                            JObject                                                        JSON,
                                                            SourceRouting                                                  Destination,
                                                            NetworkPath                                                    NetworkPath,
                                                            DateTime?                                                      ResponseTimestamp                             = null,
                                                            CustomJObjectParserDelegate<ChangeTransactionTariffResponse>?  CustomChangeTransactionTariffResponseParser   = null,
                                                            CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                                            CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                                            CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getTariffResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomChangeTransactionTariffResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a ChangeTransactionTariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out ChangeTransactionTariffResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ChangeTransactionTariff response.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChangeTransactionTariffResponse">The parsed ChangeTransactionTariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChangeTransactionTariffResponseParser">A delegate to parse custom ChangeTransactionTariff responses.</param>
        public static Boolean TryParse(ChangeTransactionTariffRequest                                 Request,
                                       JObject                                                        JSON,
                                       SourceRouting                                                  Destination,
                                       NetworkPath                                                    NetworkPath,
                                       [NotNullWhen(true)]  out ChangeTransactionTariffResponse?      ChangeTransactionTariffResponse,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       DateTime?                                                      ResponseTimestamp                             = null,
                                       CustomJObjectParserDelegate<ChangeTransactionTariffResponse>?  CustomChangeTransactionTariffResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                       CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                       CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            try
            {

                ChangeTransactionTariffResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "tariff change status",
                                         TariffChangeStatus.TryParse,
                                         out TariffChangeStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChangeTransactionTariffResponse = new ChangeTransactionTariffResponse(

                                                      Request,
                                                      Status,
                                                      StatusInfo,

                                                      null,
                                                      ResponseTimestamp,

                                                      Destination,
                                                      NetworkPath,

                                                      null,
                                                      null,
                                                      Signatures,

                                                      CustomData

                                                  );

                if (CustomChangeTransactionTariffResponseParser is not null)
                    ChangeTransactionTariffResponse = CustomChangeTransactionTariffResponseParser(JSON,
                                                                                                  ChangeTransactionTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                ChangeTransactionTariffResponse  = null;
                ErrorResponse                    = "The given JSON representation of a ChangeTransactionTariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChangeTransactionTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeTransactionTariffResponseSerializer">A delegate to serialize custom ChangeTransactionTariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                            IncludeJSONLDContext                              = false,
                              CustomJObjectSerializerDelegate<ChangeTransactionTariffResponse>?  CustomChangeTransactionTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                       CustomStatusInfoSerializer                        = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              ToString()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray (Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChangeTransactionTariffResponseSerializer is not null
                       ? CustomChangeTransactionTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ChangeTransactionTariff failed because of a request error.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request.</param>
        public static ChangeTransactionTariffResponse RequestError(ChangeTransactionTariffRequest  Request,
                                                                   EventTracking_Id                EventTrackingId,
                                                                   ResultCode                      ErrorCode,
                                                                   String?                         ErrorDescription    = null,
                                                                   JObject?                        ErrorDetails        = null,
                                                                   DateTime?                       ResponseTimestamp   = null,

                                                                   SourceRouting?                  Destination         = null,
                                                                   NetworkPath?                    NetworkPath         = null,

                                                                   IEnumerable<KeyPair>?           SignKeys            = null,
                                                                   IEnumerable<SignInfo>?          SignInfos           = null,
                                                                   IEnumerable<Signature>?         Signatures          = null,

                                                                   CustomData?                     CustomData          = null)

            => new (

                   Request,
                   TariffChangeStatus.Rejected,
                   null,
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
        /// The ChangeTransactionTariff failed.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeTransactionTariffResponse FormationViolation(ChangeTransactionTariffRequest  Request,
                                                                         String                          ErrorDescription)

            => new (Request,
                    TariffChangeStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeTransactionTariff failed.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeTransactionTariffResponse SignatureError(ChangeTransactionTariffRequest  Request,
                                                                     String                          ErrorDescription)

            => new (Request,
                    TariffChangeStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeTransactionTariff failed.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ChangeTransactionTariffResponse Failed(ChangeTransactionTariffRequest  Request,
                                                             String?                         Description   = null)

            => new (Request,
                    TariffChangeStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ChangeTransactionTariff failed because of an exception.
        /// </summary>
        /// <param name="Request">The ChangeTransactionTariff request.</param>
        /// <param name="Exception">The exception.</param>
        public static ChangeTransactionTariffResponse ExceptionOccurred(ChangeTransactionTariffRequest  Request,
                                                                       Exception                       Exception)

            => new (Request,
                    TariffChangeStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ChangeTransactionTariffResponse1, ChangeTransactionTariffResponse2)

        /// <summary>
        /// Compares two ChangeTransactionTariff responses for equality.
        /// </summary>
        /// <param name="ChangeTransactionTariffResponse1">A ChangeTransactionTariff response.</param>
        /// <param name="ChangeTransactionTariffResponse2">Another ChangeTransactionTariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeTransactionTariffResponse? ChangeTransactionTariffResponse1,
                                           ChangeTransactionTariffResponse? ChangeTransactionTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeTransactionTariffResponse1, ChangeTransactionTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeTransactionTariffResponse1 is null || ChangeTransactionTariffResponse2 is null)
                return false;

            return ChangeTransactionTariffResponse1.Equals(ChangeTransactionTariffResponse2);

        }

        #endregion

        #region Operator != (ChangeTransactionTariffResponse1, ChangeTransactionTariffResponse2)

        /// <summary>
        /// Compares two ChangeTransactionTariff responses for inequality.
        /// </summary>
        /// <param name="ChangeTransactionTariffResponse1">A ChangeTransactionTariff response.</param>
        /// <param name="ChangeTransactionTariffResponse2">Another ChangeTransactionTariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeTransactionTariffResponse? ChangeTransactionTariffResponse1,
                                           ChangeTransactionTariffResponse? ChangeTransactionTariffResponse2)

            => !(ChangeTransactionTariffResponse1 == ChangeTransactionTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeTransactionTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChangeTransactionTariff responses for equality.
        /// </summary>
        /// <param name="Object">A ChangeTransactionTariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeTransactionTariffResponse getTariffResponse &&
                   Equals(getTariffResponse);

        #endregion

        #region Equals(ChangeTransactionTariffResponse)

        /// <summary>
        /// Compares two ChangeTransactionTariff responses for equality.
        /// </summary>
        /// <param name="ChangeTransactionTariffResponse">A ChangeTransactionTariff response to compare with.</param>
        public override Boolean Equals(ChangeTransactionTariffResponse? ChangeTransactionTariffResponse)

            => ChangeTransactionTariffResponse is not null &&

               Status.Equals(ChangeTransactionTariffResponse.Status) &&

             ((StatusInfo is     null && ChangeTransactionTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && ChangeTransactionTariffResponse.StatusInfo is not null && StatusInfo.Equals(ChangeTransactionTariffResponse.StatusInfo))) &&

               base.GenericEquals(ChangeTransactionTariffResponse);

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

            => $"{Status}{(StatusInfo is not null ? $", {StatusInfo}" : "")}";

        #endregion

    }

}
