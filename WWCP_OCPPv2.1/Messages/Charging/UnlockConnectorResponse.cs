/*
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The UnlockConnector response.
    /// </summary>
    public class UnlockConnectorResponse : AResponse<UnlockConnectorRequest,
                                                     UnlockConnectorResponse>,
                                           IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/unlockConnectorResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the UnlockConnector request.
        /// </summary>
        public UnlockStatus   Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="Status">The success or failure of the UnlockConnector request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
        public UnlockConnectorResponse(UnlockConnectorRequest   Request,
                                       UnlockStatus             Status,
                                       StatusInfo?              StatusInfo            = null,

                                       Result?                  Result                = null,
                                       DateTimeOffset?          ResponseTimestamp     = null,

                                       SourceRouting?           Destination           = null,
                                       NetworkPath?             NetworkPath           = null,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default)

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
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:UnlockConnectorResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "UnlockStatusEnumType": {
        //             "description": "This indicates whether the Charging Station has unlocked the connector.",
        //             "javaType": "UnlockStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Unlocked",
        //                 "UnlockFailed",
        //                 "OngoingAuthorizedTransaction",
        //                 "UnknownConnector"
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
        //             "$ref": "#/definitions/UnlockStatusEnumType"
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
        /// Parse the given JSON representation of an UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUnlockConnectorResponseParser">A delegate to parse custom UnlockConnector responses.</param>
        public static UnlockConnectorResponse Parse(UnlockConnectorRequest                                 Request,
                                                    JObject                                                JSON,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTimeOffset?                                        ResponseTimestamp                     = null,
                                                    CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null,
                                                    CustomJObjectParserDelegate<StatusInfo>?               CustomStatusInfoParser                = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var unlockConnectorResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomUnlockConnectorResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return unlockConnectorResponse;
            }

            throw new ArgumentException("The given JSON representation of an UnlockConnector response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out UnlockConnectorResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed UnlockConnector response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorResponseParser">A delegate to parse custom UnlockConnector responses.</param>
        public static Boolean TryParse(UnlockConnectorRequest                                 Request,
                                       JObject                                                JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out UnlockConnectorResponse?      UnlockConnectorResponse,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTimeOffset?                                        ResponseTimestamp                     = null,
                                       CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?               CustomStatusInfoParser                = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                UnlockConnectorResponse = null;

                #region UnlockStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "unlock status",
                                         UnlockStatusExtensions.TryParse,
                                         out UnlockStatus UnlockStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo      [optional]

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

                #region Signatures      [optional, OCPP_CSE]

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

                #region CustomData      [optional]

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


                UnlockConnectorResponse = new UnlockConnectorResponse(

                                              Request,
                                              UnlockStatus,
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

                if (CustomUnlockConnectorResponseParser is not null)
                    UnlockConnectorResponse = CustomUnlockConnectorResponseParser(JSON,
                                                                                  UnlockConnectorResponse);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorResponse  = null;
                ErrorResponse            = "The given JSON representation of an UnlockConnector response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnlockConnectorResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorResponseSerializer">A delegate to serialize custom UnlockConnector responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                    IncludeJSONLDContext                      = false,
                              CustomJObjectSerializerDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?               CustomStatusInfoSerializer                = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnlockConnectorResponseSerializer is not null
                       ? CustomUnlockConnectorResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The UnlockConnector failed because of a request error.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        public static UnlockConnectorResponse RequestError(UnlockConnectorRequest   Request,
                                                           EventTracking_Id         EventTrackingId,
                                                           ResultCode               ErrorCode,
                                                           String?                  ErrorDescription    = null,
                                                           JObject?                 ErrorDetails        = null,
                                                           DateTimeOffset?          ResponseTimestamp   = null,

                                                           SourceRouting?           Destination         = null,
                                                           NetworkPath?             NetworkPath         = null,

                                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                                           IEnumerable<Signature>?  Signatures          = null,

                                                           CustomData?              CustomData          = null)

            => new (

                   Request,
                   UnlockStatus.UnlockFailed,
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
        /// The UnlockConnector failed.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UnlockConnectorResponse FormationViolation(UnlockConnectorRequest  Request,
                                                                 String                  ErrorDescription)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The UnlockConnector failed.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UnlockConnectorResponse SignatureError(UnlockConnectorRequest  Request,
                                                             String                  ErrorDescription)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The UnlockConnector failed.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="Description">An optional error description.</param>
        public static UnlockConnectorResponse Failed(UnlockConnectorRequest  Request,
                                                     String?                 Description   = null)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The UnlockConnector failed because of an exception.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="Exception">The exception.</param>
        public static UnlockConnectorResponse ExceptionOccurred(UnlockConnectorRequest  Request,
                                                               Exception               Exception)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two UnlockConnector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">An UnlockConnector response.</param>
        /// <param name="UnlockConnectorResponse2">Another UnlockConnector response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorResponse? UnlockConnectorResponse1,
                                           UnlockConnectorResponse? UnlockConnectorResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorResponse1, UnlockConnectorResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorResponse1 is null || UnlockConnectorResponse2 is null)
                return false;

            return UnlockConnectorResponse1.Equals(UnlockConnectorResponse2);

        }

        #endregion

        #region Operator != (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two UnlockConnector responses for inequality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">An UnlockConnector response.</param>
        /// <param name="UnlockConnectorResponse2">Another UnlockConnector response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorResponse? UnlockConnectorResponse1,
                                           UnlockConnectorResponse? UnlockConnectorResponse2)

            => !(UnlockConnectorResponse1 == UnlockConnectorResponse2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UnlockConnector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">An UnlockConnector response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorResponse unlockConnectorResponse &&
                   Equals(unlockConnectorResponse);

        #endregion

        #region Equals(UnlockConnectorResponse)

        /// <summary>
        /// Compares two UnlockConnector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">An UnlockConnector response to compare with.</param>
        public override Boolean Equals(UnlockConnectorResponse? UnlockConnectorResponse)

            => UnlockConnectorResponse is not null &&

               Status.     Equals(UnlockConnectorResponse.Status) &&

             ((StatusInfo is     null && UnlockConnectorResponse.StatusInfo is     null) ||
               StatusInfo is not null && UnlockConnectorResponse.StatusInfo is not null && StatusInfo.Equals(UnlockConnectorResponse.StatusInfo)) &&

               base.GenericEquals(UnlockConnectorResponse);

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

            => Status.ToString();

        #endregion

    }

}
