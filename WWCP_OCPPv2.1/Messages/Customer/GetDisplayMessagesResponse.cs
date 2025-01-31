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
    /// The GetDisplayMessages response.
    /// </summary>
    public class GetDisplayMessagesResponse : AResponse<GetDisplayMessagesRequest,
                                                        GetDisplayMessagesResponse>,
                                              IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getDisplayMessagesResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charging station will indicate whether it has display messages
        /// that match the request criteria.
        /// </summary>
        [Mandatory]
        public GetDisplayMessagesStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetDisplayMessages response.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request leading to this response.</param>
        /// <param name="Status">The charging station will indicate whether it has display messages that match the request criteria.</param>
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
        public GetDisplayMessagesResponse(GetDisplayMessagesRequest  Request,
                                          GetDisplayMessagesStatus   Status,
                                          StatusInfo?                StatusInfo            = null,

                                          Result?                    Result                = null,
                                          DateTime?                  ResponseTimestamp     = null,

                                          SourceRouting?             Destination           = null,
                                          NetworkPath?               NetworkPath           = null,

                                          IEnumerable<KeyPair>?      SignKeys              = null,
                                          IEnumerable<SignInfo>?     SignInfos             = null,
                                          IEnumerable<Signature>?    Signatures            = null,

                                          CustomData?                CustomData            = null,

                                          SerializationFormats?      SerializationFormat   = null,
                                          CancellationToken          CancellationToken     = default)

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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetDisplayMessagesResponse",
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
        //     "GetDisplayMessagesStatusEnumType": {
        //       "description": "Indicates if the Charging Station has Display Messages that match the request criteria in the &lt;&lt;getdisplaymessagesrequest,GetDisplayMessagesRequest&gt;&gt;",
        //       "javaType": "GetDisplayMessagesStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Unknown"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.",
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
        //       "$ref": "#/definitions/GetDisplayMessagesStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomGetDisplayMessagesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetDisplayMessages response.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetDisplayMessagesResponseParser">A delegate to parse custom GetDisplayMessages responses.</param>
        public static GetDisplayMessagesResponse Parse(GetDisplayMessagesRequest                                 Request,
                                                       JObject                                                   JSON,
                                                       SourceRouting                                         Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 ResponseTimestamp                        = null,
                                                       CustomJObjectParserDelegate<GetDisplayMessagesResponse>?  CustomGetDisplayMessagesResponseParser   = null,
                                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getDisplayMessagesResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetDisplayMessagesResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getDisplayMessagesResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetDisplayMessages response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetDisplayMessagesResponse, out ErrorResponse, CustomGetDisplayMessagesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetDisplayMessages response.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetDisplayMessagesResponse">The parsed GetDisplayMessages response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetDisplayMessagesResponseParser">A delegate to parse custom GetDisplayMessages responses.</param>
        public static Boolean TryParse(GetDisplayMessagesRequest                                 Request,
                                       JObject                                                   JSON,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out GetDisplayMessagesResponse?      GetDisplayMessagesResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 ResponseTimestamp                        = null,
                                       CustomJObjectParserDelegate<GetDisplayMessagesResponse>?  CustomGetDisplayMessagesResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            try
            {

                GetDisplayMessagesResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "GetDisplayMessages status",
                                         GetDisplayMessagesStatusExtensions.TryParse,
                                         out GetDisplayMessagesStatus Status,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetDisplayMessagesResponse = new GetDisplayMessagesResponse(

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

                if (CustomGetDisplayMessagesResponseParser is not null)
                    GetDisplayMessagesResponse = CustomGetDisplayMessagesResponseParser(JSON,
                                                                                        GetDisplayMessagesResponse);

                return true;

            }
            catch (Exception e)
            {
                GetDisplayMessagesResponse  = null;
                ErrorResponse               = "The given JSON representation of a GetDisplayMessages response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDisplayMessagesResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDisplayMessagesResponseSerializer">A delegate to serialize custom GetDisplayMessages responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<GetDisplayMessagesResponse>?  CustomGetDisplayMessagesResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                  CustomStatusInfoSerializer                   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomGetDisplayMessagesResponseSerializer is not null
                       ? CustomGetDisplayMessagesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetDisplayMessages failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request.</param>
        public static GetDisplayMessagesResponse RequestError(GetDisplayMessagesRequest  Request,
                                                              EventTracking_Id           EventTrackingId,
                                                              ResultCode                 ErrorCode,
                                                              String?                    ErrorDescription    = null,
                                                              JObject?                   ErrorDetails        = null,
                                                              DateTime?                  ResponseTimestamp   = null,

                                                              SourceRouting?             Destination         = null,
                                                              NetworkPath?               NetworkPath         = null,

                                                              IEnumerable<KeyPair>?      SignKeys            = null,
                                                              IEnumerable<SignInfo>?     SignInfos           = null,
                                                              IEnumerable<Signature>?    Signatures          = null,

                                                              CustomData?                CustomData          = null)

            => new (

                   Request,
                   GetDisplayMessagesStatus.Unknown,
                   null,
                  OCPPv2_1.Result.FromErrorResponse(
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
        /// The GetDisplayMessages failed.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetDisplayMessagesResponse FormationViolation(GetDisplayMessagesRequest  Request,
                                                                    String                          ErrorDescription)

            => new (Request,
                    GetDisplayMessagesStatus.Unknown,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDisplayMessages failed.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetDisplayMessagesResponse SignatureError(GetDisplayMessagesRequest  Request,
                                                                String                          ErrorDescription)

            => new (Request,
                    GetDisplayMessagesStatus.Unknown,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDisplayMessages failed.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetDisplayMessagesResponse Failed(GetDisplayMessagesRequest  Request,
                                                        String?                         Description   = null)

            => new (Request,
                    GetDisplayMessagesStatus.Unknown,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetDisplayMessages failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetDisplayMessages request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetDisplayMessagesResponse ExceptionOccured(GetDisplayMessagesRequest  Request,
                                                                  Exception                       Exception)

            => new (Request,
                    GetDisplayMessagesStatus.Unknown,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetDisplayMessagesResponse1, GetDisplayMessagesResponse2)

        /// <summary>
        /// Compares two GetDisplayMessages responses for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesResponse1">A GetDisplayMessages response.</param>
        /// <param name="GetDisplayMessagesResponse2">Another GetDisplayMessages response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDisplayMessagesResponse? GetDisplayMessagesResponse1,
                                           GetDisplayMessagesResponse? GetDisplayMessagesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDisplayMessagesResponse1, GetDisplayMessagesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetDisplayMessagesResponse1 is null || GetDisplayMessagesResponse2 is null)
                return false;

            return GetDisplayMessagesResponse1.Equals(GetDisplayMessagesResponse2);

        }

        #endregion

        #region Operator != (GetDisplayMessagesResponse1, GetDisplayMessagesResponse2)

        /// <summary>
        /// Compares two GetDisplayMessages responses for inequality.
        /// </summary>
        /// <param name="GetDisplayMessagesResponse1">A GetDisplayMessages response.</param>
        /// <param name="GetDisplayMessagesResponse2">Another GetDisplayMessages response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDisplayMessagesResponse? GetDisplayMessagesResponse1,
                                           GetDisplayMessagesResponse? GetDisplayMessagesResponse2)

            => !(GetDisplayMessagesResponse1 == GetDisplayMessagesResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDisplayMessagesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetDisplayMessages responses for equality.
        /// </summary>
        /// <param name="Object">A GetDisplayMessages response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDisplayMessagesResponse getDisplayMessagesResponse &&
                   Equals(getDisplayMessagesResponse);

        #endregion

        #region Equals(GetDisplayMessagesResponse)

        /// <summary>
        /// Compares two GetDisplayMessages responses for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesResponse">A GetDisplayMessages response to compare with.</param>
        public override Boolean Equals(GetDisplayMessagesResponse? GetDisplayMessagesResponse)

            => GetDisplayMessagesResponse is not null &&

               Status.     Equals(GetDisplayMessagesResponse.Status) &&

             ((StatusInfo is     null && GetDisplayMessagesResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetDisplayMessagesResponse.StatusInfo is not null && StatusInfo.Equals(GetDisplayMessagesResponse.StatusInfo)) &&

               base.GenericEquals(GetDisplayMessagesResponse);

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

            => Status.AsText();

        #endregion

    }

}
