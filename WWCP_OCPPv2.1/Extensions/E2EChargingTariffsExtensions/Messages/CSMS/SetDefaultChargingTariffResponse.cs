/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using System.Collections.ObjectModel;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A set default charging tariff response.
    /// </summary>
    public class SetDefaultChargingTariffResponse : AResponse<CSMS.SetDefaultChargingTariffRequest,
                                                                   SetDefaultChargingTariffResponse>,
                                                    IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setDefaultChargingTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                                                Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The SetDefaultChargingTariff status.
        /// </summary>
        [Mandatory]
        public SetDefaultChargingTariffStatus                               Status             { get; }

        /// <summary>
        /// An optional element providing more information about the SetDefaultChargingTariff status.
        /// </summary>
        [Optional]
        public StatusInfo?                                                  StatusInfo         { get; }

        /// <summary>
        /// The optional enumeration of status infos for individual EVSEs.
        /// </summary>
        [Optional]
        public IEnumerable<EVSEStatusInfo<SetDefaultChargingTariffStatus>>  EVSEStatusInfos    { get; }

        #endregion

        #region Constructor(s)

        #region SetDefaultChargingTariffResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new set default charging tariff response.
        /// </summary>
        /// <param name="Request">The set default charging tariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="EVSEStatusInfos">An optional enumeration of status infos for individual EVSEs.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetDefaultChargingTariffResponse(CSMS.SetDefaultChargingTariffRequest                          Request,
                                                SetDefaultChargingTariffStatus                                Status,
                                                StatusInfo?                                                   StatusInfo          = null,
                                                IEnumerable<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?  EVSEStatusInfos     = null,
                                                DateTime?                                                     ResponseTimestamp   = null,

                                                IEnumerable<KeyPair>?                                         SignKeys            = null,
                                                IEnumerable<SignInfo>?                                        SignInfos           = null,
                                                IEnumerable<OCPP.Signature>?                                  Signatures          = null,

                                                CustomData?                                                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status           = Status;
            this.StatusInfo       = StatusInfo;
            this.EVSEStatusInfos  = EVSEStatusInfos ?? Array.Empty<EVSEStatusInfo<SetDefaultChargingTariffStatus>>();

            unchecked
            {

                hashCode = this.Status.         GetHashCode()       * 7 ^
                          (this.StatusInfo?.    GetHashCode() ?? 0) * 5 ^
                           this.EVSEStatusInfos.CalcHashCode()      * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion

        #region SetDefaultChargingTariffResponse(Request, Result)

        /// <summary>
        /// Create a new set default charging tariff response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public SetDefaultChargingTariffResponse(CSMS.SetDefaultChargingTariffRequest  Request,
                                                Result                                Result)

            : base(Request,
                   Result)

        {

            this.Status           = SetDefaultChargingTariffStatus.Rejected;
            this.EVSEStatusInfos  = Request.EVSEIds.Select(evseId => new EVSEStatusInfo<SetDefaultChargingTariffStatus>(
                                                                         evseId,
                                                                         SetDefaultChargingTariffStatus.Rejected
                                                                     ));

            unchecked
            {

                hashCode = this.Status.         GetHashCode()       * 7 ^
                          (this.StatusInfo?.    GetHashCode() ?? 0) * 5 ^
                           this.EVSEStatusInfos.CalcHashCode()      * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomSetDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set default charging tariff response.
        /// </summary>
        /// <param name="Request">The set default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetDefaultChargingTariffResponseParser">A delegate to parse custom set default charging tariff responses.</param>
        public static SetDefaultChargingTariffResponse Parse(CSMS.SetDefaultChargingTariffRequest                            Request,
                                                             JObject                                                         JSON,
                                                             CustomJObjectParserDelegate<SetDefaultChargingTariffResponse>?  CustomSetDefaultChargingTariffResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var setDefaultChargingTariffResponse,
                         out var errorResponse,
                         CustomSetDefaultChargingTariffResponseParser) &&
                setDefaultChargingTariffResponse is not null)
            {
                return setDefaultChargingTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a set default charging tariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetDefaultChargingTariffResponse, out ErrorResponse, CustomSetDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a set default charging tariff response.
        /// </summary>
        /// <param name="Request">The set default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetDefaultChargingTariffResponse">The parsed set default charging tariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetDefaultChargingTariffResponseParser">A delegate to parse custom set default charging tariff responses.</param>
        public static Boolean TryParse(CSMS.SetDefaultChargingTariffRequest                            Request,
                                       JObject                                                         JSON,
                                       out SetDefaultChargingTariffResponse?                           SetDefaultChargingTariffResponse,
                                       out String?                                                     ErrorResponse,
                                       CustomJObjectParserDelegate<SetDefaultChargingTariffResponse>?  CustomSetDefaultChargingTariffResponseParser   = null)
        {

            try
            {

                SetDefaultChargingTariffResponse = null;

                #region Status             [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "set default charging tariff status",
                                         SetDefaultChargingTariffStatusExtensions.TryParse,
                                         out SetDefaultChargingTariffStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo         [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPP.StatusInfo.TryParse,
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

                var EVSEStatusInfos = new List<EVSEStatusInfo<SetDefaultChargingTariffStatus>>();

                foreach (var evseStatusInfoProperty in EVSEStatusInfosJSON)
                {

                    if (evseStatusInfoProperty is not JObject evseStatusInfoJObject)
                    {
                        ErrorResponse = "Invalid evseStatusInfo JSON object!";
                        return false;
                    }

                    if (!EVSEStatusInfo<SetDefaultChargingTariffStatus>.TryParse(evseStatusInfoJObject,
                                                                                 out var evseStatusInfo,
                                                                                 out ErrorResponse,
                                                                                 SetDefaultChargingTariffStatusExtensions.TryParse,
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetDefaultChargingTariffResponse = new SetDefaultChargingTariffResponse(

                                                       Request,
                                                       Status,
                                                       StatusInfo,
                                                       EVSEStatusInfos,
                                                       null,

                                                       null,
                                                       null,
                                                       Signatures,

                                                       CustomData

                                                   );

                if (CustomSetDefaultChargingTariffResponseParser is not null)
                    SetDefaultChargingTariffResponse = CustomSetDefaultChargingTariffResponseParser(JSON,
                                                                                                    SetDefaultChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                SetDefaultChargingTariffResponse  = null;
                ErrorResponse                     = "The given JSON representation of a set default charging tariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDefaultChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDefaultChargingTariffResponseSerializer">A delegate to serialize custom set default charging tariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDefaultChargingTariffResponse>?                CustomSetDefaultChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                                      CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?  CustomEVSEStatusInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                                  CustomSignatureSerializer                          = null,
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

            return CustomSetDefaultChargingTariffResponseSerializer is not null
                       ? CustomSetDefaultChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The set default charging tariff failed.
        /// </summary>
        public static SetDefaultChargingTariffResponse Failed(CSMS.SetDefaultChargingTariffRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SetDefaultChargingTariffResponse1, SetDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two set default charging tariff responses for equality.
        /// </summary>
        /// <param name="SetDefaultChargingTariffResponse1">A set default charging tariff response.</param>
        /// <param name="SetDefaultChargingTariffResponse2">Another set default charging tariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDefaultChargingTariffResponse? SetDefaultChargingTariffResponse1,
                                           SetDefaultChargingTariffResponse? SetDefaultChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDefaultChargingTariffResponse1, SetDefaultChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetDefaultChargingTariffResponse1 is null || SetDefaultChargingTariffResponse2 is null)
                return false;

            return SetDefaultChargingTariffResponse1.Equals(SetDefaultChargingTariffResponse2);

        }

        #endregion

        #region Operator != (SetDefaultChargingTariffResponse1, SetDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two set default charging tariff responses for inequality.
        /// </summary>
        /// <param name="SetDefaultChargingTariffResponse1">A set default charging tariff response.</param>
        /// <param name="SetDefaultChargingTariffResponse2">Another set default charging tariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDefaultChargingTariffResponse? SetDefaultChargingTariffResponse1,
                                           SetDefaultChargingTariffResponse? SetDefaultChargingTariffResponse2)

            => !(SetDefaultChargingTariffResponse1 == SetDefaultChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<SetDefaultChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set default charging tariff responses for equality.
        /// </summary>
        /// <param name="Object">A set default charging tariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDefaultChargingTariffResponse setDefaultChargingTariffResponse &&
                   Equals(setDefaultChargingTariffResponse);

        #endregion

        #region Equals(SetDefaultChargingTariffResponse)

        /// <summary>
        /// Compares two set default charging tariff responses for equality.
        /// </summary>
        /// <param name="SetDefaultChargingTariffResponse">A set default charging tariff response to compare with.</param>
        public override Boolean Equals(SetDefaultChargingTariffResponse? SetDefaultChargingTariffResponse)

            => SetDefaultChargingTariffResponse is not null &&

               Status.Equals(SetDefaultChargingTariffResponse.Status) &&

             ((StatusInfo is     null && SetDefaultChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && SetDefaultChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(SetDefaultChargingTariffResponse.StatusInfo))) &&

               EVSEStatusInfos.Count().Equals(SetDefaultChargingTariffResponse.EVSEStatusInfos.Count()) &&
               EVSEStatusInfos.All(kvp => SetDefaultChargingTariffResponse.EVSEStatusInfos.Contains(kvp)) &&

               base.GenericEquals(SetDefaultChargingTariffResponse);

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
