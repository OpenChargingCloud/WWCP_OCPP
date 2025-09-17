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
    /// The SetDERControl response.
    /// </summary>
    public class SetDERControlResponse : AResponse<SetDERControlRequest,
                                                   SetDERControlResponse>,
                                         IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getDERControlResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext               Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the SetDERControl request.
        /// </summary>
        public DERControlStatus            Status           { get; }

        /// <summary>
        /// The optional enumeration of controlIds that are superseded as a result of setting this control.
        /// </summary>
        [Optional]
        public IEnumerable<DERControl_Id>  SupersededIds    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                 StatusInfo       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDERControl response.
        /// </summary>
        /// <param name="Request">The SetDERControl request leading to this response.</param>
        /// <param name="Status">The success or failure of the SetDERControl request.</param>
        /// <param name="SupersededIds">An optional enumeration of controlIds that are superseded as a result of setting this control.</param>
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
        public SetDERControlResponse(SetDERControlRequest         Request,
                                     DERControlStatus             Status,
                                     IEnumerable<DERControl_Id>?  SupersededIds         = null,
                                     StatusInfo?                  StatusInfo            = null,

                                     Result?                      Result                = null,
                                     DateTimeOffset?              ResponseTimestamp     = null,

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

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status         = Status;
            this.SupersededIds  = SupersededIds?.Distinct() ?? [];
            this.StatusInfo     = StatusInfo;

            unchecked
            {

                hashCode = this.Status.       GetHashCode()        * 7 ^
                           this.SupersededIds.CalcHashCode()       * 5 ^
                          (this.StatusInfo?.  GetHashCode()  ?? 0) * 3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:SetDERControlResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "DERControlStatusEnumType": {
        //             "description": "Result of operation.",
        //             "javaType": "DERControlStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "NotSupported",
        //                 "NotFound"
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
        //             "$ref": "#/definitions/DERControlStatusEnumType"
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
        /// Parse the given JSON representation of a SetDERControl response.
        /// </summary>
        /// <param name="Request">The SetDERControl request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetDERControlResponseParser">A delegate to parse custom SetDERControl responses.</param>
        public static SetDERControlResponse Parse(SetDERControlRequest                                 Request,
                                                  JObject                                              JSON,
                                                  SourceRouting                                        Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTimeOffset?                                      ResponseTimestamp                   = null,
                                                  CustomJObjectParserDelegate<SetDERControlResponse>?  CustomSetDERControlResponseParser   = null,
                                                  CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getDERControlResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetDERControlResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getDERControlResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetDERControl response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SetDERControlResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDERControl response.
        /// </summary>
        /// <param name="Request">The SetDERControl request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetDERControlResponse">The parsed SetDERControl response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetDERControlResponseParser">A delegate to parse custom SetDERControl responses.</param>
        public static Boolean TryParse(SetDERControlRequest                                 Request,
                                       JObject                                              JSON,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out SetDERControlResponse?      SetDERControlResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTimeOffset?                                      ResponseTimestamp                   = null,
                                       CustomJObjectParserDelegate<SetDERControlResponse>?  CustomSetDERControlResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                SetDERControlResponse = null;

                #region Status           [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         DERControlStatus.TryParse,
                                         out DERControlStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SupersededIds    [optional]

                if (JSON.ParseOptionalHashSet("supersededIds",
                                              "superseded identifications",
                                              DERControl_Id.TryParse,
                                              out HashSet<DERControl_Id> SupersededIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo       [optional]

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

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                SetDERControlResponse = new SetDERControlResponse(

                                            Request,
                                            Status,
                                            SupersededIds,
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

                if (CustomSetDERControlResponseParser is not null)
                    SetDERControlResponse = CustomSetDERControlResponseParser(JSON,
                                                                              SetDERControlResponse);

                return true;

            }
            catch (Exception e)
            {
                SetDERControlResponse  = null;
                ErrorResponse          = "The given JSON representation of a SetDERControl response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDERControlResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDERControlResponseSerializer">A delegate to serialize custom SetDERControl responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                  IncludeJSONLDContext                    = false,
                              CustomJObjectSerializerDelegate<SetDERControlResponse>?  CustomSetDERControlResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?             CustomStatusInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",        DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",          Status.              ToString()),

                           SupersededIds.Any()
                               ? new JProperty("supersededIds",   new JArray(SupersededIds.Select(supersededId => supersededId.ToString())))
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",      StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                              CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetDERControlResponseSerializer is not null
                       ? CustomSetDERControlResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetDERControl failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetDERControl request.</param>
        public static SetDERControlResponse RequestError(SetDERControlRequest     Request,
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
                   DERControlStatus.Rejected,
                   null,
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
        /// The SetDERControl failed.
        /// </summary>
        /// <param name="Request">The SetDERControl request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDERControlResponse FormationViolation(SetDERControlRequest  Request,
                                                               String                ErrorDescription)

            => new (Request,
                    DERControlStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDERControl failed.
        /// </summary>
        /// <param name="Request">The SetDERControl request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDERControlResponse SignatureError(SetDERControlRequest  Request,
                                                           String                ErrorDescription)

            => new (Request,
                    DERControlStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDERControl failed.
        /// </summary>
        /// <param name="Request">The SetDERControl request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetDERControlResponse Failed(SetDERControlRequest  Request,
                                                   String?               Description   = null)

            => new (Request,
                    DERControlStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The SetDERControl failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetDERControl request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetDERControlResponse ExceptionOccurred(SetDERControlRequest  Request,
                                                             Exception             Exception)

            => new (Request,
                    DERControlStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetDERControlResponse1, SetDERControlResponse2)

        /// <summary>
        /// Compares two SetDERControl responses for equality.
        /// </summary>
        /// <param name="SetDERControlResponse1">A SetDERControl response.</param>
        /// <param name="SetDERControlResponse2">Another SetDERControl response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDERControlResponse? SetDERControlResponse1,
                                           SetDERControlResponse? SetDERControlResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDERControlResponse1, SetDERControlResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetDERControlResponse1 is null || SetDERControlResponse2 is null)
                return false;

            return SetDERControlResponse1.Equals(SetDERControlResponse2);

        }

        #endregion

        #region Operator != (SetDERControlResponse1, SetDERControlResponse2)

        /// <summary>
        /// Compares two SetDERControl responses for inequality.
        /// </summary>
        /// <param name="SetDERControlResponse1">A SetDERControl response.</param>
        /// <param name="SetDERControlResponse2">Another SetDERControl response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDERControlResponse? SetDERControlResponse1,
                                           SetDERControlResponse? SetDERControlResponse2)

            => !(SetDERControlResponse1 == SetDERControlResponse2);

        #endregion

        #endregion

        #region IEquatable<SetDERControlResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDERControl responses for equality.
        /// </summary>
        /// <param name="SetDERControlResponse">A SetDERControl response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDERControlResponse getDERControlResponse &&
                   Equals(getDERControlResponse);

        #endregion

        #region Equals(SetDERControlResponse)

        /// <summary>
        /// Compares two SetDERControl responses for equality.
        /// </summary>
        /// <param name="SetDERControlResponse">A SetDERControl response to compare with.</param>
        public override Boolean Equals(SetDERControlResponse? SetDERControlResponse)

            => SetDERControlResponse is not null &&

               Status.                      Equals(SetDERControlResponse.Status)        &&
               SupersededIds.ToHashSet().SetEquals(SetDERControlResponse.SupersededIds) &&

             ((StatusInfo is     null && SetDERControlResponse.StatusInfo is     null) ||
               StatusInfo is not null && SetDERControlResponse.StatusInfo is not null && StatusInfo.Equals(SetDERControlResponse.StatusInfo)) &&

               base.GenericEquals(SetDERControlResponse);

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

                   Status.ToString(),

                   SupersededIds.Any()
                       ? $", superseded Ids: '{SupersededIds.AggregateWith(", ")}'"
                       : ""

               );

        #endregion

    }

}
