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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The SetDefaultTariff response.
    /// </summary>
    public class SetDefaultTariffResponse : AResponse<SetDefaultTariffRequest,
                                                      SetDefaultTariffResponse>,
                                            IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setDefaultTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The tariff status.
        /// </summary>
        [Mandatory]
        public TariffStatus   Status        { get; }

        /// <summary>
        /// Optional information about the result of the SetDefaultTariff request.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDefaultTariff response.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">Optional information about the result of the SetDefaultTariff request.</param>
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
        public SetDefaultTariffResponse(SetDefaultTariffRequest  Request,
                                        TariffStatus             Status,
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

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomSetDefaultTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetDefaultTariff response.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetDefaultTariffResponseParser">A delegate to parse custom SetDefaultTariff responses.</param>
        public static SetDefaultTariffResponse Parse(SetDefaultTariffRequest                                 Request,
                                                     JObject                                                 JSON,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<SetDefaultTariffResponse>?  CustomSetDefaultTariffResponseParser   = null,
                                                     CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setDefaultTariffResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetDefaultTariffResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setDefaultTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetDefaultTariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetDefaultTariffResponse, out ErrorResponse, CustomSetDefaultTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDefaultTariff response.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetDefaultTariffResponse">The parsed SetDefaultTariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetDefaultTariffResponseParser">A delegate to parse custom SetDefaultTariff responses.</param>
        public static Boolean TryParse(SetDefaultTariffRequest                                 Request,
                                       JObject                                                 JSON,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out SetDefaultTariffResponse?      SetDefaultTariffResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<SetDefaultTariffResponse>?  CustomSetDefaultTariffResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                SetDefaultTariffResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "SetDefaultTariff status",
                                         TariffStatusExtensions.TryParse,
                                         out TariffStatus Status,
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


                SetDefaultTariffResponse = new SetDefaultTariffResponse(

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

                if (CustomSetDefaultTariffResponseParser is not null)
                    SetDefaultTariffResponse = CustomSetDefaultTariffResponseParser(JSON,
                                                                                    SetDefaultTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                SetDefaultTariffResponse  = null;
                ErrorResponse             = "The given JSON representation of a SetDefaultTariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDefaultTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDefaultTariffResponseSerializer">A delegate to serialize custom SetDefaultTariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDefaultTariffResponse>?  CustomSetDefaultTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                CustomStatusInfoSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
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

            return CustomSetDefaultTariffResponseSerializer is not null
                       ? CustomSetDefaultTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetDefaultTariff failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request.</param>
        public static SetDefaultTariffResponse RequestError(SetDefaultTariffRequest  Request,
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
        /// The SetDefaultTariff failed.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDefaultTariffResponse FormationViolation(SetDefaultTariffRequest  Request,
                                                                  String                   ErrorDescription)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDefaultTariff failed.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDefaultTariffResponse SignatureError(SetDefaultTariffRequest  Request,
                                                              String                   ErrorDescription)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDefaultTariff failed.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetDefaultTariffResponse Failed(SetDefaultTariffRequest  Request,
                                                      String?                  Description   = null)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SetDefaultTariff failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetDefaultTariff request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetDefaultTariffResponse ExceptionOccured(SetDefaultTariffRequest  Request,
                                                                Exception                Exception)

            => new (Request,
                    TariffStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetDefaultTariffResponse1, SetDefaultTariffResponse2)

        /// <summary>
        /// Compares two SetDefaultTariff responses for equality.
        /// </summary>
        /// <param name="SetDefaultTariffResponse1">A SetDefaultTariff response.</param>
        /// <param name="SetDefaultTariffResponse2">Another SetDefaultTariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDefaultTariffResponse? SetDefaultTariffResponse1,
                                           SetDefaultTariffResponse? SetDefaultTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDefaultTariffResponse1, SetDefaultTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetDefaultTariffResponse1 is null || SetDefaultTariffResponse2 is null)
                return false;

            return SetDefaultTariffResponse1.Equals(SetDefaultTariffResponse2);

        }

        #endregion

        #region Operator != (SetDefaultTariffResponse1, SetDefaultTariffResponse2)

        /// <summary>
        /// Compares two SetDefaultTariff responses for inequality.
        /// </summary>
        /// <param name="SetDefaultTariffResponse1">A SetDefaultTariff response.</param>
        /// <param name="SetDefaultTariffResponse2">Another SetDefaultTariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDefaultTariffResponse? SetDefaultTariffResponse1,
                                           SetDefaultTariffResponse? SetDefaultTariffResponse2)

            => !(SetDefaultTariffResponse1 == SetDefaultTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<SetDefaultTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDefaultTariff responses for equality.
        /// </summary>
        /// <param name="Object">A SetDefaultTariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDefaultTariffResponse setDefaultTariffResponse &&
                   Equals(setDefaultTariffResponse);

        #endregion

        #region Equals(SetDefaultTariffResponse)

        /// <summary>
        /// Compares two SetDefaultTariff responses for equality.
        /// </summary>
        /// <param name="SetDefaultTariffResponse">A SetDefaultTariff response to compare with.</param>
        public override Boolean Equals(SetDefaultTariffResponse? SetDefaultTariffResponse)

            => SetDefaultTariffResponse is not null &&

               Status.Equals(SetDefaultTariffResponse.Status) &&

             ((StatusInfo is     null && SetDefaultTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && SetDefaultTariffResponse.StatusInfo is not null && StatusInfo.Equals(SetDefaultTariffResponse.StatusInfo))) &&

               base.GenericEquals(SetDefaultTariffResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}";

        #endregion

    }

}