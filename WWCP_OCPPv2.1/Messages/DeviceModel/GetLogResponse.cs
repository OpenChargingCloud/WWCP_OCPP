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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetLog response.
    /// </summary>
    public class GetLogResponse : AResponse<GetLogRequest,
                                            GetLogResponse>,
                                  IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getLogResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the GetLog command.
        /// </summary>
        [Mandatory]
        public LogStatus      Status        { get; }

        /// <summary>
        /// The name of the log file that will be uploaded.
        /// This field is not present when no logging information is available.
        /// </summary>
        [Optional]
        public String?        Filename      { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetLog response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Status">The success or failure of the GetLog command.</param>
        /// <param name="Filename">The name of the log file that will be uploaded. This field is not present when no logging information is available.</param>
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
        public GetLogResponse(GetLogRequest            Request,
                              LogStatus                Status,
                              String?                  Filename              = null,
                              StatusInfo?              StatusInfo            = null,

                              Result?                  Result                = null,
                              DateTime?                ResponseTimestamp     = null,

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
            this.Filename    = Filename;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 7 ^
                          (this.Filename?.  GetHashCode() ?? 0) * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:GetLog.conf",
        //   "definitions": {
        //     "LogStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "AcceptedCanceled"
        //       ]
        //     }
        // },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //         "$ref": "#/definitions/LogStatusEnumType"
        //     },
        //     "filename": {
        //         "type": "string",
        //       "maxLength": 255
        //     }
        // },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetLogResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetLog response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetLogResponseParser">A delegate to parse custom GetLog responses.</param>
        public static GetLogResponse Parse(GetLogRequest                                 Request,
                                           JObject                                       JSON,
                                           SourceRouting                             Destination,
                                           NetworkPath                                   NetworkPath,
                                           DateTime?                                     ResponseTimestamp            = null,
                                           CustomJObjectParserDelegate<GetLogResponse>?  CustomGetLogResponseParser   = null,
                                           CustomJObjectParserDelegate<StatusInfo>?      CustomStatusInfoParser       = null,
                                           CustomJObjectParserDelegate<Signature>?       CustomSignatureParser        = null,
                                           CustomJObjectParserDelegate<CustomData>?      CustomCustomDataParser       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getLogResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetLogResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getLogResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetLog response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetLogResponse, out ErrorResponse, CustomGetLogResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetLog response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetLogResponse">The parsed GetLog response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLogResponseParser">A delegate to parse custom GetLog responses.</param>
        public static Boolean TryParse(GetLogRequest                                 Request,
                                       JObject                                       JSON,
                                       SourceRouting                             Destination,
                                       NetworkPath                                   NetworkPath,
                                       [NotNullWhen(true)]  out GetLogResponse?      GetLogResponse,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       DateTime?                                     ResponseTimestamp            = null,
                                       CustomJObjectParserDelegate<GetLogResponse>?  CustomGetLogResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?      CustomStatusInfoParser       = null,
                                       CustomJObjectParserDelegate<Signature>?       CustomSignatureParser        = null,
                                       CustomJObjectParserDelegate<CustomData>?      CustomCustomDataParser       = null)
        {

            try
            {

                GetLogResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "GetLog status",
                                         LogStatusExtensions.TryParse,
                                         out LogStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Filename      [optional]

                var Filename = JSON.GetOptional("filename");

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


                GetLogResponse = new GetLogResponse(

                                     Request,
                                     Status,
                                     Filename,
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

                if (CustomGetLogResponseParser is not null)
                    GetLogResponse = CustomGetLogResponseParser(JSON,
                                                                GetLogResponse);

                return true;

            }
            catch (Exception e)
            {
                GetLogResponse  = null;
                ErrorResponse   = "The given JSON representation of a GetLog response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetLogResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLogResponseSerializer">A delegate to serialize custom GetLog responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLogResponse>?  CustomGetLogResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?      CustomStatusInfoSerializer       = null,
                              CustomJObjectSerializerDelegate<Signature>?       CustomSignatureSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.AsText()),

                           Filename.IsNotNullOrEmpty()
                               ? new JProperty("filename",     Filename)
                               : null,

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

            return CustomGetLogResponseSerializer is not null
                       ? CustomGetLogResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetLog failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetLog request.</param>
        public static GetLogResponse RequestError(GetLogRequest            Request,
                                                  EventTracking_Id         EventTrackingId,
                                                  ResultCode               ErrorCode,
                                                  String?                  ErrorDescription    = null,
                                                  JObject?                 ErrorDetails        = null,
                                                  DateTime?                ResponseTimestamp   = null,

                                                  SourceRouting?           Destination         = null,
                                                  NetworkPath?             NetworkPath         = null,

                                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                                  IEnumerable<Signature>?  Signatures          = null,

                                                  CustomData?              CustomData          = null)

            => new (

                   Request,
                   LogStatus.Rejected,
                   null,
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
        /// The GetLog failed.
        /// </summary>
        /// <param name="Request">The GetLog request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetLogResponse FormationViolation(GetLogRequest  Request,
                                                        String         ErrorDescription)

            => new (Request,
                    LogStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetLog failed.
        /// </summary>
        /// <param name="Request">The GetLog request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetLogResponse SignatureError(GetLogRequest  Request,
                                                    String         ErrorDescription)

            => new (Request,
                    LogStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetLog failed.
        /// </summary>
        /// <param name="Request">The GetLog request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetLogResponse Failed(GetLogRequest  Request,
                                            String?        Description   = null)

            => new (Request,
                    LogStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetLog failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetLog request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetLogResponse ExceptionOccured(GetLogRequest  Request,
                                                      Exception      Exception)

            => new (Request,
                    LogStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetLogResponse1, GetLogResponse2)

        /// <summary>
        /// Compares two GetLog responses for equality.
        /// </summary>
        /// <param name="GetLogResponse1">A GetLog response.</param>
        /// <param name="GetLogResponse2">Another GetLog response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLogResponse? GetLogResponse1,
                                           GetLogResponse? GetLogResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLogResponse1, GetLogResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetLogResponse1 is null || GetLogResponse2 is null)
                return false;

            return GetLogResponse1.Equals(GetLogResponse2);

        }

        #endregion

        #region Operator != (GetLogResponse1, GetLogResponse2)

        /// <summary>
        /// Compares two GetLog responses for inequality.
        /// </summary>
        /// <param name="GetLogResponse1">A GetLog response.</param>
        /// <param name="GetLogResponse2">Another GetLog response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLogResponse? GetLogResponse1,
                                           GetLogResponse? GetLogResponse2)

            => !(GetLogResponse1 == GetLogResponse2);

        #endregion

        #endregion

        #region IEquatable<GetLogResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetLog responses for equality.
        /// </summary>
        /// <param name="Object">A GetLog response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLogResponse getLogResponse &&
                   Equals(getLogResponse);

        #endregion

        #region Equals(GetLogResponse)

        /// <summary>
        /// Compares two GetLog responses for equality.
        /// </summary>
        /// <param name="GetLogResponse">A GetLog response to compare with.</param>
        public override Boolean Equals(GetLogResponse? GetLogResponse)

            => GetLogResponse is not null &&

               Status.     Equals(GetLogResponse.Status) &&

             ((Filename   is     null && GetLogResponse.Filename   is     null) ||
              (Filename   is not null && GetLogResponse.Filename   is not null && Filename.  Equals(GetLogResponse.Filename)))  &&

             ((StatusInfo is     null && GetLogResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetLogResponse.StatusInfo is not null && StatusInfo.Equals(GetLogResponse.StatusInfo)) &&

               base.GenericEquals(GetLogResponse);

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
