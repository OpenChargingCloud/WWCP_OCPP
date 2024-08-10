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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetCRL response.
    /// </summary>
    public class GetCRLResponse : AResponse<CS.GetCRLRequest,
                                               GetCRLResponse>,
                                  IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getCRLResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The GetCRL request identification.
        /// </summary>
        [Mandatory]
        public UInt32         GetCRLRequestId    { get; }

        /// <summary>
        /// The success or failure of the EXI message processing.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status             { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo         { get; }

        #endregion

        #region Constructor(s)

        #region GetCRLResponse(Request, GetCRLRequestId, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new GetCRL response.
        /// </summary>
        /// <param name="Request">The GetCRL request leading to this response.</param>
        /// <param name="GetCRLRequestId">The GetCRL request identification.</param>
        /// <param name="Status">The success or failure of the EXI message processing.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetCRLResponse(CS.GetCRLRequest         Request,
                              UInt32                   GetCRLRequestId,
                              GenericStatus            Status,
                              StatusInfo?              StatusInfo          = null,
                              DateTime?                ResponseTimestamp   = null,

                              NetworkingNode_Id?       DestinationId       = null,
                              NetworkPath?             NetworkPath         = null,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.GetCRLRequestId  = GetCRLRequestId;
            this.Status           = Status;
            this.StatusInfo       = StatusInfo;

        }

        #endregion

        #region GetCRLResponse(Request, Result)

        /// <summary>
        /// Create a new GetCRL response.
        /// </summary>
        /// <param name="Request">The GetCRL request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetCRLResponse(CS.GetCRLRequest         Request,
                              Result                   Result,
                              DateTime?                ResponseTimestamp   = null,

                              NetworkingNode_Id?       DestinationId       = null,
                              NetworkPath?             NetworkPath         = null,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomGetCRLResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetCRL response.
        /// </summary>
        /// <param name="Request">The GetCRL request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetCRLResponseParser">A delegate to parse custom GetCRL responses.</param>
        public static GetCRLResponse Parse(CS.GetCRLRequest                              Request,
                                           JObject                                       JSON,
                                           NetworkingNode_Id                             DestinationId,
                                           NetworkPath                                   NetworkPath,
                                           DateTime?                                     ResponseTimestamp            = null,
                                           CustomJObjectParserDelegate<GetCRLResponse>?  CustomGetCRLResponseParser   = null,
                                           CustomJObjectParserDelegate<StatusInfo>?      CustomStatusInfoParser       = null,
                                           CustomJObjectParserDelegate<Signature>?       CustomSignatureParser        = null,
                                           CustomJObjectParserDelegate<CustomData>?      CustomCustomDataParser       = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var getCRLResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetCRLResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getCRLResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetCRL response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetCRLResponse, out ErrorResponse, CustomGetCRLResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetCRL response.
        /// </summary>
        /// <param name="Request">The GetCRL request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetCRLResponse">The parsed GetCRL response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCRLResponseParser">A delegate to parse custom GetCRL responses.</param>
        public static Boolean TryParse(CS.GetCRLRequest                              Request,
                                       JObject                                       JSON,
                                       NetworkingNode_Id                             DestinationId,
                                       NetworkPath                                   NetworkPath,
                                       [NotNullWhen(true)]  out GetCRLResponse?      GetCRLResponse,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       DateTime?                                     ResponseTimestamp            = null,
                                       CustomJObjectParserDelegate<GetCRLResponse>?  CustomGetCRLResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?      CustomStatusInfoParser       = null,
                                       CustomJObjectParserDelegate<Signature>?       CustomSignatureParser        = null,
                                       CustomJObjectParserDelegate<CustomData>?      CustomCustomDataParser       = null)
        {

            try
            {

                GetCRLResponse = null;

                #region GetCRLRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "GetCRL request identification",
                                         out UInt32 GetCRLRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Status             [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatus.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo         [optional]

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

                #region Signatures         [optional, OCPP_CSE]

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

                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetCRLResponse = new GetCRLResponse(

                                     Request,
                                     GetCRLRequestId,
                                     Status,
                                     StatusInfo,
                                     ResponseTimestamp,

                                     DestinationId,
                                     NetworkPath,

                                     null,
                                     null,
                                     Signatures,

                                     CustomData

                                 );

                if (CustomGetCRLResponseParser is not null)
                    GetCRLResponse = CustomGetCRLResponseParser(JSON,
                                                                GetCRLResponse);

                return true;

            }
            catch (Exception e)
            {
                GetCRLResponse  = null;
                ErrorResponse   = "The given JSON representation of a GetCRL response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCRLResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCRLResponseSerializer">A delegate to serialize custom GetCRL responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCRLResponse>?  CustomGetCRLResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?      CustomStatusInfoSerializer       = null,
                              CustomJObjectSerializerDelegate<Signature>?       CustomSignatureSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.         AsText()),
                                 new JProperty("requestId",    GetCRLRequestId.ToString()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.     ToJSON(CustomStatusInfoSerializer,
                                                                                      CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCRLResponseSerializer is not null
                       ? CustomGetCRLResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetCRL failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetCRL request.</param>
        public static GetCRLResponse RequestError(CS.GetCRLRequest         Request,
                                                  EventTracking_Id         EventTrackingId,
                                                  ResultCode               ErrorCode,
                                                  String?                  ErrorDescription    = null,
                                                  JObject?                 ErrorDetails        = null,
                                                  DateTime?                ResponseTimestamp   = null,

                                                  NetworkingNode_Id?       DestinationId       = null,
                                                  NetworkPath?             NetworkPath         = null,

                                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                                  IEnumerable<Signature>?  Signatures          = null,

                                                  CustomData?              CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The GetCRL failed.
        /// </summary>
        /// <param name="Request">The GetCRL request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCRLResponse FormationViolation(CS.GetCRLRequest  Request,
                                                        String            ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetCRL failed.
        /// </summary>
        /// <param name="Request">The GetCRL request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCRLResponse SignatureError(CS.GetCRLRequest  Request,
                                                    String            ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetCRL failed.
        /// </summary>
        /// <param name="Request">The GetCRL request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetCRLResponse Failed(CS.GetCRLRequest  Request,
                                            String?           Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The GetCRL failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetCRL request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetCRLResponse ExceptionOccured(CS.GetCRLRequest  Request,
                                                      Exception         Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetCRLResponse1, GetCRLResponse2)

        /// <summary>
        /// Compares two GetCRL responses for equality.
        /// </summary>
        /// <param name="GetCRLResponse1">A GetCRL response.</param>
        /// <param name="GetCRLResponse2">Another GetCRL response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCRLResponse? GetCRLResponse1,
                                           GetCRLResponse? GetCRLResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCRLResponse1, GetCRLResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetCRLResponse1 is null || GetCRLResponse2 is null)
                return false;

            return GetCRLResponse1.Equals(GetCRLResponse2);

        }

        #endregion

        #region Operator != (GetCRLResponse1, GetCRLResponse2)

        /// <summary>
        /// Compares two GetCRL responses for inequality.
        /// </summary>
        /// <param name="GetCRLResponse1">A GetCRL response.</param>
        /// <param name="GetCRLResponse2">Another GetCRL response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCRLResponse? GetCRLResponse1,
                                           GetCRLResponse? GetCRLResponse2)

            => !(GetCRLResponse1 == GetCRLResponse2);

        #endregion

        #endregion

        #region IEquatable<GetCRLResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetCRL responses for equality.
        /// </summary>
        /// <param name="Object">A GetCRL response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCRLResponse getCRLResponse &&
                   Equals(getCRLResponse);

        #endregion

        #region Equals(GetCRLResponse)

        /// <summary>
        /// Compares two GetCRL responses for equality.
        /// </summary>
        /// <param name="GetCRLResponse">A GetCRL response to compare with.</param>
        public override Boolean Equals(GetCRLResponse? GetCRLResponse)

            => GetCRLResponse is not null &&

               Status.         Equals(GetCRLResponse.Status)          &&
               GetCRLRequestId.Equals(GetCRLResponse.GetCRLRequestId) &&

             ((StatusInfo is     null && GetCRLResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetCRLResponse.StatusInfo is not null && StatusInfo.Equals(GetCRLResponse.StatusInfo)) &&

               base.GenericEquals(GetCRLResponse);

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

                return Status.         GetHashCode()       * 7 ^
                       GetCRLRequestId.GetHashCode()       * 5 ^
                      (StatusInfo?.    GetHashCode() ?? 0) * 3 ^

                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Status} ({GetCRLRequestId})";

        #endregion


    }

}
