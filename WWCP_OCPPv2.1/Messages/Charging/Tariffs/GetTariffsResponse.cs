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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetTariffs response.
    /// </summary>
    public class GetTariffsResponse : AResponse<GetTariffsRequest,
                                                GetTariffsResponse>,
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
        public JSONLDContext                  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The GetTariffs status.
        /// </summary>
        [Mandatory]
        public TariffStatus                   Status              { get; }

        /// <summary>
        /// The optional map of installed default and user-specific tariffs per EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<TariffAssignment>  TariffAssignments    { get; }

        /// <summary>
        /// An optional element providing more information about the GetTariffs status.
        /// </summary>
        [Optional]
        public StatusInfo?                    StatusInfo          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetTariffs response.
        /// </summary>
        /// <param name="Request">The GetTariffs request leading to this response.</param>
        /// <param name="Status">The GetTariffs status.</param>
        /// <param name="TariffAssignments">An optional map of installed default and user-specific tariffs per EVSE.</param>
        /// <param name="StatusInfo">An optional element providing more information about the GetTariffs status.</param>
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetTariffsResponse(GetTariffsRequest               Request,
                                  TariffStatus                    Status,
                                  IEnumerable<TariffAssignment>?  TariffAssignments     = null,
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

            this.Status             = Status;
            this.TariffAssignments  = TariffAssignments?.Distinct() ?? [];
            this.StatusInfo         = StatusInfo;

            unchecked
            {

                hashCode = this.Status.           GetHashCode()       * 7 ^
                           this.TariffAssignments.CalcHashCode()      * 5 ^
                          (this.StatusInfo?.      GetHashCode() ?? 0) * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetTariffsResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetTariffs response.
        /// </summary>
        /// <param name="Request">The GetTariffs request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetTariffsResponseParser">A delegate to parse custom GetTariffs responses.</param>
        public static GetTariffsResponse Parse(GetTariffsRequest                                 Request,
                                               JObject                                           JSON,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<GetTariffsResponse>?  CustomGetTariffsResponseParser   = null,
                                               CustomJObjectParserDelegate<TariffAssignment>?    CustomTariffAssignmentParser     = null,
                                               CustomJObjectParserDelegate<IdToken>?             CustomIdTokenParser              = null,
                                               CustomJObjectParserDelegate<AdditionalInfo>?      CustomAdditionalInfoParser       = null,
                                               CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getTariffsResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetTariffsResponseParser,
                         CustomTariffAssignmentParser,
                         CustomIdTokenParser,
                         CustomAdditionalInfoParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getTariffsResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetTariffs response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetTariffsResponse, out ErrorResponse, CustomGetTariffsResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetTariffs response.
        /// </summary>
        /// <param name="Request">The GetTariffs request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetTariffsResponse">The parsed GetTariffs response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetTariffsResponseParser">A delegate to parse custom GetTariffs responses.</param>
        public static Boolean TryParse(GetTariffsRequest                                 Request,
                                       JObject                                           JSON,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out GetTariffsResponse?      GetTariffsResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<GetTariffsResponse>?  CustomGetTariffsResponseParser   = null,
                                       CustomJObjectParserDelegate<TariffAssignment>?    CustomTariffAssignmentParser     = null,
                                       CustomJObjectParserDelegate<IdToken>?             CustomIdTokenParser              = null,
                                       CustomJObjectParserDelegate<AdditionalInfo>?      CustomAdditionalInfoParser       = null,
                                       CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                GetTariffsResponse = null;

                #region Parse Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "GetTariffs status",
                                         TariffStatus.TryParse,
                                         out TariffStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TariffAssignments    [optional]

                if (JSON.ParseOptionalHashSet("tariffAssignments",
                                              "tariff assignments",
                                              TariffAssignment.TryParse,
                                              out HashSet<TariffAssignment> TariffAssignments,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo           [optional]

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

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                GetTariffsResponse = new GetTariffsResponse(

                                         Request,
                                         Status,
                                         TariffAssignments,
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

                if (CustomGetTariffsResponseParser is not null)
                    GetTariffsResponse = CustomGetTariffsResponseParser(JSON,
                                                                        GetTariffsResponse);

                return true;

            }
            catch (Exception e)
            {
                GetTariffsResponse  = null;
                ErrorResponse       = "The given JSON representation of a GetTariffs response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetTariffsResponseSerializer = null, CustomTariffAssignmentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetTariffsResponseSerializer">A delegate to serialize custom GetTariffs responses.</param>
        /// <param name="CustomTariffAssignmentSerializer">A delegate to serialize custom TariffAssignment JSON objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetTariffsResponse>?  CustomGetTariffsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffAssignment>?    CustomTariffAssignmentSerializer     = null,
                              CustomJObjectSerializerDelegate<IdToken>?             CustomIdTokenSerializer              = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?      CustomAdditionalInfoSerializer       = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?          CustomStatusInfoSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",              Status.    AsText()),

                           TariffAssignments.Any()
                               ? new JProperty("tariffAssignments",   new JArray (TariffAssignments.Select(tariffAssignment => tariffAssignment.ToJSON(CustomTariffAssignmentSerializer,
                                                                                                                                                       CustomIdTokenSerializer,
                                                                                                                                                       CustomAdditionalInfoSerializer,
                                                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",          StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray (Signatures.       Select(signature        => signature.       ToJSON(CustomSignatureSerializer,
                                                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetTariffsResponseSerializer is not null
                       ? CustomGetTariffsResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetTariffs failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetTariffs request.</param>
        public static GetTariffsResponse RequestError(GetTariffsRequest        Request,
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
                   TariffStatus.Rejected,
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
        /// The GetTariffs failed.
        /// </summary>
        /// <param name="Request">The GetTariffs request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetTariffsResponse FormationViolation(GetTariffsRequest  Request,
                                                            String             ErrorDescription)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetTariffs failed.
        /// </summary>
        /// <param name="Request">The GetTariffs request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetTariffsResponse SignatureError(GetTariffsRequest  Request,
                                                        String             ErrorDescription)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetTariffs failed.
        /// </summary>
        /// <param name="Request">The GetTariffs request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetTariffsResponse Failed(GetTariffsRequest  Request,
                                                String?            Description   = null)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetTariffs failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetTariffs request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetTariffsResponse ExceptionOccured(GetTariffsRequest  Request,
                                                          Exception          Exception)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetTariffsResponse1, GetTariffsResponse2)

        /// <summary>
        /// Compares two GetTariffs responses for equality.
        /// </summary>
        /// <param name="GetTariffsResponse1">A GetTariffs response.</param>
        /// <param name="GetTariffsResponse2">Another GetTariffs response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetTariffsResponse? GetTariffsResponse1,
                                           GetTariffsResponse? GetTariffsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetTariffsResponse1, GetTariffsResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetTariffsResponse1 is null || GetTariffsResponse2 is null)
                return false;

            return GetTariffsResponse1.Equals(GetTariffsResponse2);

        }

        #endregion

        #region Operator != (GetTariffsResponse1, GetTariffsResponse2)

        /// <summary>
        /// Compares two GetTariffs responses for inequality.
        /// </summary>
        /// <param name="GetTariffsResponse1">A GetTariffs response.</param>
        /// <param name="GetTariffsResponse2">Another GetTariffs response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetTariffsResponse? GetTariffsResponse1,
                                           GetTariffsResponse? GetTariffsResponse2)

            => !(GetTariffsResponse1 == GetTariffsResponse2);

        #endregion

        #endregion

        #region IEquatable<GetTariffsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetTariffs responses for equality.
        /// </summary>
        /// <param name="Object">A GetTariffs response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetTariffsResponse getTariffsResponse &&
                   Equals(getTariffsResponse);

        #endregion

        #region Equals(GetTariffsResponse)

        /// <summary>
        /// Compares two GetTariffs responses for equality.
        /// </summary>
        /// <param name="GetTariffsResponse">A GetTariffs response to compare with.</param>
        public override Boolean Equals(GetTariffsResponse? GetTariffsResponse)

            => GetTariffsResponse is not null &&

               Status.Equals(GetTariffsResponse.Status) &&

               TariffAssignments.Count().Equals(GetTariffsResponse.TariffAssignments.Count())       &&
               TariffAssignments.All(kvp =>     GetTariffsResponse.TariffAssignments.Contains(kvp)) &&

             ((StatusInfo is     null && GetTariffsResponse.StatusInfo is     null) ||
              (StatusInfo is not null && GetTariffsResponse.StatusInfo is not null && StatusInfo.Equals(GetTariffsResponse.StatusInfo))) &&

               base.GenericEquals(GetTariffsResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}{(TariffAssignments.Any() ? $", {TariffAssignments.Select(tariffAssignment => $"'{tariffAssignment.TariffId}' ({tariffAssignment.TariffKind.AsText()}) => '{tariffAssignment.EVSEIds.AggregateWith(",")}', '{tariffAssignment.IdTokens.AggregateWith(",")}'")}" : "")}";

        #endregion

    }

}
