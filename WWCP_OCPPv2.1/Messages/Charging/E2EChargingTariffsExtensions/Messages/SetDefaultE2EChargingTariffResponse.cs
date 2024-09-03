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
    /// The SetDefaultE2EChargingTariff response.
    /// </summary>
    public class SetDefaultE2EChargingTariffResponse : AResponse<SetDefaultE2EChargingTariffRequest,
                                                                 SetDefaultE2EChargingTariffResponse>,
                                                       IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setDefaultE2EChargingTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                                                Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The SetDefaultE2EChargingTariff status.
        /// </summary>
        [Mandatory]
        public SetDefaultE2EChargingTariffStatus                               Status             { get; }

        /// <summary>
        /// An optional element providing more information about the SetDefaultE2EChargingTariff status.
        /// </summary>
        [Optional]
        public StatusInfo?                                                  StatusInfo         { get; }

        /// <summary>
        /// The optional enumeration of status infos for individual EVSEs.
        /// </summary>
        [Optional]
        public IEnumerable<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>  EVSEStatusInfos    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDefaultE2EChargingTariff response.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="EVSEStatusInfos">An optional enumeration of status infos for individual EVSEs.</param>
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
        public SetDefaultE2EChargingTariffResponse(SetDefaultE2EChargingTariffRequest                               Request,
                                                SetDefaultE2EChargingTariffStatus                                Status,
                                                StatusInfo?                                                   StatusInfo            = null,
                                                IEnumerable<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>?  EVSEStatusInfos       = null,

                                                Result?                                                       Result                = null,
                                                DateTime?                                                     ResponseTimestamp     = null,

                                                SourceRouting?                                                Destination           = null,
                                                NetworkPath?                                                  NetworkPath           = null,

                                                IEnumerable<KeyPair>?                                         SignKeys              = null,
                                                IEnumerable<SignInfo>?                                        SignInfos             = null,
                                                IEnumerable<Signature>?                                       Signatures            = null,

                                                CustomData?                                                   CustomData            = null,

                                                SerializationFormats?                                         SerializationFormat   = null,
                                                CancellationToken                                             CancellationToken     = default)

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

            this.Status           = Status;
            this.StatusInfo       = StatusInfo;
            this.EVSEStatusInfos  = EVSEStatusInfos ?? Array.Empty<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>();

            unchecked
            {

                hashCode = this.Status.         GetHashCode()       * 7 ^
                          (this.StatusInfo?.    GetHashCode() ?? 0) * 5 ^
                           this.EVSEStatusInfos.CalcHashCode()      * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomSetDefaultE2EChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetDefaultE2EChargingTariff response.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetDefaultE2EChargingTariffResponseParser">A delegate to parse custom SetDefaultE2EChargingTariff responses.</param>
        public static SetDefaultE2EChargingTariffResponse Parse(SetDefaultE2EChargingTariffRequest                                               Request,
                                                             JObject                                                                       JSON,
                                                             SourceRouting                                                             Destination,
                                                             NetworkPath                                                                   NetworkPath,
                                                             DateTime?                                                                     ResponseTimestamp                              = null,
                                                             CustomJObjectParserDelegate<SetDefaultE2EChargingTariffResponse>?                CustomSetDefaultE2EChargingTariffResponseParser   = null,
                                                             CustomJObjectParserDelegate<StatusInfo>?                                      CustomStatusInfoParser                         = null,
                                                             CustomJObjectParserDelegate<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>?  CustomEVSEStatusInfoParser                     = null,
                                                             CustomJObjectParserDelegate<Signature>?                                       CustomSignatureParser                          = null,
                                                             CustomJObjectParserDelegate<CustomData>?                                      CustomCustomDataParser                         = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setDefaultE2EChargingTariffResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetDefaultE2EChargingTariffResponseParser,
                         CustomStatusInfoParser,
                         CustomEVSEStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setDefaultE2EChargingTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetDefaultE2EChargingTariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetDefaultE2EChargingTariffResponse, out ErrorResponse, CustomSetDefaultE2EChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDefaultE2EChargingTariff response.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetDefaultE2EChargingTariffResponse">The parsed SetDefaultE2EChargingTariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetDefaultE2EChargingTariffResponseParser">A delegate to parse custom SetDefaultE2EChargingTariff responses.</param>
        public static Boolean TryParse(SetDefaultE2EChargingTariffRequest                                               Request,
                                       JObject                                                                       JSON,
                                       SourceRouting                                                             Destination,
                                       NetworkPath                                                                   NetworkPath,
                                       [NotNullWhen(true)]  out SetDefaultE2EChargingTariffResponse?                    SetDefaultE2EChargingTariffResponse,
                                       [NotNullWhen(false)] out String?                                              ErrorResponse,
                                       DateTime?                                                                     ResponseTimestamp                              = null,
                                       CustomJObjectParserDelegate<SetDefaultE2EChargingTariffResponse>?                CustomSetDefaultE2EChargingTariffResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                                      CustomStatusInfoParser                         = null,
                                       CustomJObjectParserDelegate<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>?  CustomEVSEStatusInfoParser                     = null,
                                       CustomJObjectParserDelegate<Signature>?                                       CustomSignatureParser                          = null,
                                       CustomJObjectParserDelegate<CustomData>?                                      CustomCustomDataParser                         = null)
        {

            try
            {

                SetDefaultE2EChargingTariffResponse = null;

                #region Status             [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "SetDefaultE2EChargingTariff status",
                                         SetDefaultE2EChargingTariffStatusExtensions.TryParse,
                                         out SetDefaultE2EChargingTariffStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo         [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSEStatusInfos    [mandatory]

                if (!JSON.ParseMandatory("evseStatusInfos",
                                         "EVSE status infos",
                                         out JArray EVSEStatusInfosJSON,
                                         out ErrorResponse))
                {
                    return false;
                }

                var EVSEStatusInfos = new List<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>();

                foreach (var evseStatusInfoProperty in EVSEStatusInfosJSON)
                {

                    if (evseStatusInfoProperty is not JObject evseStatusInfoJObject)
                    {
                        ErrorResponse = "Invalid evseStatusInfo JSON object!";
                        return false;
                    }

                    if (!EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>.TryParse(evseStatusInfoJObject,
                                                                                 out var evseStatusInfo,
                                                                                 out ErrorResponse,
                                                                                 SetDefaultE2EChargingTariffStatusExtensions.TryParse,
                                                                                 null) ||
                        evseStatusInfo is null)
                    {
                        return false;
                    }

                    EVSEStatusInfos.Add(evseStatusInfo);

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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetDefaultE2EChargingTariffResponse = new SetDefaultE2EChargingTariffResponse(

                                                       Request,
                                                       Status,
                                                       StatusInfo,
                                                       EVSEStatusInfos,

                                                       null,
                                                       ResponseTimestamp,

                                                       Destination,
                                                       NetworkPath,

                                                       null,
                                                       null,
                                                       Signatures,

                                                       CustomData

                                                   );

                if (CustomSetDefaultE2EChargingTariffResponseParser is not null)
                    SetDefaultE2EChargingTariffResponse = CustomSetDefaultE2EChargingTariffResponseParser(JSON,
                                                                                                    SetDefaultE2EChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                SetDefaultE2EChargingTariffResponse  = null;
                ErrorResponse                     = "The given JSON representation of a SetDefaultE2EChargingTariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDefaultE2EChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDefaultE2EChargingTariffResponseSerializer">A delegate to serialize custom SetDefaultE2EChargingTariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDefaultE2EChargingTariffResponse>?                CustomSetDefaultE2EChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                                      CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>?  CustomEVSEStatusInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<Signature>?                                       CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                      CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",            Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",        StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                      CustomCustomDataSerializer))
                               : null,

                                 new JProperty("evseStatusInfos",   new JArray(EVSEStatusInfos.Select(evseStatusInfo => evseStatusInfo.ToJSON(status => status.AsText(),
                                                                                                                                              CustomEVSEStatusInfoSerializer,
                                                                                                                                              CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.     Select(signature      => signature.     ToJSON(CustomSignatureSerializer,
                                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetDefaultE2EChargingTariffResponseSerializer is not null
                       ? CustomSetDefaultE2EChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetDefaultE2EChargingTariff failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request.</param>
        public static SetDefaultE2EChargingTariffResponse RequestError(SetDefaultE2EChargingTariffRequest  Request,
                                                                       EventTracking_Id                    EventTrackingId,
                                                                       ResultCode                          ErrorCode,
                                                                       String?                             ErrorDescription    = null,
                                                                       JObject?                            ErrorDetails        = null,
                                                                       DateTime?                           ResponseTimestamp   = null,

                                                                       SourceRouting?                      Destination         = null,
                                                                       NetworkPath?                        NetworkPath         = null,

                                                                       IEnumerable<KeyPair>?               SignKeys            = null,
                                                                       IEnumerable<SignInfo>?              SignInfos           = null,
                                                                       IEnumerable<Signature>?             Signatures          = null,

                                                                       CustomData?                         CustomData          = null)

            => new (

                   Request,
                   SetDefaultE2EChargingTariffStatus.Rejected,
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
        /// The SetDefaultE2EChargingTariff failed.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDefaultE2EChargingTariffResponse FormationViolation(SetDefaultE2EChargingTariffRequest  Request,
                                                                          String                           ErrorDescription)

            => new (Request,
                    SetDefaultE2EChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDefaultE2EChargingTariff failed.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDefaultE2EChargingTariffResponse SignatureError(SetDefaultE2EChargingTariffRequest  Request,
                                                                      String                           ErrorDescription)

            => new (Request,
                    SetDefaultE2EChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDefaultE2EChargingTariff failed.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetDefaultE2EChargingTariffResponse Failed(SetDefaultE2EChargingTariffRequest  Request,
                                                              String?                          Description   = null)

            => new (Request,
                    SetDefaultE2EChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SetDefaultE2EChargingTariff failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetDefaultE2EChargingTariff request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetDefaultE2EChargingTariffResponse ExceptionOccured(SetDefaultE2EChargingTariffRequest  Request,
                                                                        Exception                        Exception)

            => new (Request,
                    SetDefaultE2EChargingTariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetDefaultE2EChargingTariffResponse1, SetDefaultE2EChargingTariffResponse2)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariff responses for equality.
        /// </summary>
        /// <param name="SetDefaultE2EChargingTariffResponse1">A SetDefaultE2EChargingTariff response.</param>
        /// <param name="SetDefaultE2EChargingTariffResponse2">Another SetDefaultE2EChargingTariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDefaultE2EChargingTariffResponse? SetDefaultE2EChargingTariffResponse1,
                                           SetDefaultE2EChargingTariffResponse? SetDefaultE2EChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDefaultE2EChargingTariffResponse1, SetDefaultE2EChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetDefaultE2EChargingTariffResponse1 is null || SetDefaultE2EChargingTariffResponse2 is null)
                return false;

            return SetDefaultE2EChargingTariffResponse1.Equals(SetDefaultE2EChargingTariffResponse2);

        }

        #endregion

        #region Operator != (SetDefaultE2EChargingTariffResponse1, SetDefaultE2EChargingTariffResponse2)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariff responses for inequality.
        /// </summary>
        /// <param name="SetDefaultE2EChargingTariffResponse1">A SetDefaultE2EChargingTariff response.</param>
        /// <param name="SetDefaultE2EChargingTariffResponse2">Another SetDefaultE2EChargingTariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDefaultE2EChargingTariffResponse? SetDefaultE2EChargingTariffResponse1,
                                           SetDefaultE2EChargingTariffResponse? SetDefaultE2EChargingTariffResponse2)

            => !(SetDefaultE2EChargingTariffResponse1 == SetDefaultE2EChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<SetDefaultE2EChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariff responses for equality.
        /// </summary>
        /// <param name="Object">A SetDefaultE2EChargingTariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDefaultE2EChargingTariffResponse setDefaultE2EChargingTariffResponse &&
                   Equals(setDefaultE2EChargingTariffResponse);

        #endregion

        #region Equals(SetDefaultE2EChargingTariffResponse)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariff responses for equality.
        /// </summary>
        /// <param name="SetDefaultE2EChargingTariffResponse">A SetDefaultE2EChargingTariff response to compare with.</param>
        public override Boolean Equals(SetDefaultE2EChargingTariffResponse? SetDefaultE2EChargingTariffResponse)

            => SetDefaultE2EChargingTariffResponse is not null &&

               Status.Equals(SetDefaultE2EChargingTariffResponse.Status) &&

             ((StatusInfo is     null && SetDefaultE2EChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && SetDefaultE2EChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(SetDefaultE2EChargingTariffResponse.StatusInfo))) &&

               EVSEStatusInfos.Count().Equals(SetDefaultE2EChargingTariffResponse.EVSEStatusInfos.Count()) &&
               EVSEStatusInfos.All(kvp => SetDefaultE2EChargingTariffResponse.EVSEStatusInfos.Contains(kvp)) &&

               base.GenericEquals(SetDefaultE2EChargingTariffResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}{(EVSEStatusInfos.Any() ? $", {EVSEStatusInfos.Select(evseStatusInfo => $"{evseStatusInfo.EVSEId} => '{evseStatusInfo.Status.AsText()}'").AggregateWith(", ")}" : "")}";

        #endregion

    }

}
