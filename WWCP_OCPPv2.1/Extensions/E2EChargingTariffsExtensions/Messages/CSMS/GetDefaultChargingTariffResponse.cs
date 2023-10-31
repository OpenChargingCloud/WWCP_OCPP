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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A get default charging tariff response.
    /// </summary>
    public class GetDefaultChargingTariffResponse : AResponse<CSMS.GetDefaultChargingTariffRequest,
                                                                   GetDefaultChargingTariffResponse>,
                                                    IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/getDefaultChargingTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                                                 Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The GetDefaultChargingTariff status.
        /// </summary>
        [Mandatory]
        public GenericStatus                                                 Status               { get; }

        /// <summary>
        /// An optional element providing more information about the GetDefaultChargingTariff status.
        /// </summary>
        [Optional]
        public StatusInfo?                                                   StatusInfo           { get; }

        /// <summary>
        /// An optional enuemration of default charging tariffs.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingTariff>                                   ChargingTariffs      { get; }

        /// <summary>
        /// The optional map of charging tariffs to EVSE identifications.
        /// </summary>
        [Optional]
        public IReadOnlyDictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>  ChargingTariffMap    { get; }

        #endregion

        #region Constructor(s)

        #region GetDefaultChargingTariffResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get default charging tariff response.
        /// </summary>
        /// <param name="Request">The get default charging tariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="StatusInfos">An optional enumeration of status infos for individual EVSEs.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetDefaultChargingTariffResponse(CSMS.GetDefaultChargingTariffRequest                           Request,
                                                GenericStatus                                                  Status,
                                                StatusInfo?                                                    StatusInfo          = null,
                                                IEnumerable<ChargingTariff>?                                   ChargingTariffs     = null,
                                                IReadOnlyDictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>?  ChargingTariffMap   = null,
                                                DateTime?                                                      ResponseTimestamp   = null,

                                                IEnumerable<KeyPair>?                                          SignKeys            = null,
                                                IEnumerable<SignInfo>?                                         SignInfos           = null,
                                                IEnumerable<Signature>?                                        Signatures          = null,

                                                CustomData?                                                    CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status             = Status;
            this.StatusInfo         = StatusInfo;
            this.ChargingTariffs    = ChargingTariffs?.Distinct() ?? Array.Empty<ChargingTariff>();
            this.ChargingTariffMap  = ChargingTariffMap ?? new ReadOnlyDictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>(
                                                               new Dictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>()
                                                           );

            unchecked
            {

                hashCode = this.Status.           GetHashCode()       * 7 ^
                          (this.StatusInfo?.      GetHashCode() ?? 0) * 5 ^
                           this.ChargingTariffMap.CalcHashCode()      * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion

        #region GetDefaultChargingTariffResponse(Request, Result)

        /// <summary>
        /// Create a new get default charging tariff response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public GetDefaultChargingTariffResponse(CSMS.GetDefaultChargingTariffRequest  Request,
                                                Result                                Result)

            : base(Request,
                   Result)

        {

            this.Status             = GenericStatus.Rejected;
            this.ChargingTariffs    = Array.Empty<ChargingTariff>();
            this.ChargingTariffMap  = ChargingTariffMap ?? new ReadOnlyDictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>(
                                                               new Dictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>()
                                                           );

            unchecked
            {

                hashCode = this.Status.           GetHashCode()       * 7 ^
                          (this.StatusInfo?.      GetHashCode() ?? 0) * 5 ^
                           this.ChargingTariffMap.CalcHashCode()      * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get default charging tariff response.
        /// </summary>
        /// <param name="Request">The get default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetDefaultChargingTariffResponseParser">A delegate to parse custom get default charging tariff responses.</param>
        public static GetDefaultChargingTariffResponse Parse(CSMS.GetDefaultChargingTariffRequest                            Request,
                                                             JObject                                                         JSON,
                                                             CustomJObjectParserDelegate<GetDefaultChargingTariffResponse>?  CustomGetDefaultChargingTariffResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var getDefaultChargingTariffResponse,
                         out var errorResponse,
                         CustomGetDefaultChargingTariffResponseParser))
            {
                return getDefaultChargingTariffResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get default charging tariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetDefaultChargingTariffResponse, out ErrorResponse, CustomGetDefaultChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get default charging tariff response.
        /// </summary>
        /// <param name="Request">The get default charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetDefaultChargingTariffResponse">The parsed get default charging tariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetDefaultChargingTariffResponseParser">A delegate to parse custom get default charging tariff responses.</param>
        public static Boolean TryParse(CSMS.GetDefaultChargingTariffRequest                            Request,
                                       JObject                                                         JSON,
                                       out GetDefaultChargingTariffResponse?                           GetDefaultChargingTariffResponse,
                                       out String?                                                     ErrorResponse,
                                       CustomJObjectParserDelegate<GetDefaultChargingTariffResponse>?  CustomGetDefaultChargingTariffResponseParser   = null)
        {

            try
            {

                GetDefaultChargingTariffResponse = null;

                #region Status               [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "get default charging tariff status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo           [optional]

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

                #region ChargingTariffs      [optional]

                if (JSON.ParseOptionalHashSet("chargingTariffs",
                                              "charging tariffs",
                                              ChargingTariff.TryParse,
                                              out HashSet<ChargingTariff> ChargingTariffs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingTariffMap    [optional]

                var ChargingTariffMap = new Dictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>();

                if (JSON.ParseOptional("chargingTariffMap",
                                       "charging tariff map",
                                       out JObject ChargingTariffMapJSON,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    foreach (var chargingTariffProperty in ChargingTariffMapJSON.Properties())
                    {

                        var chargingTariffId = ChargingTariff_Id.TryParse(chargingTariffProperty.Name);

                        if (!chargingTariffId.HasValue)
                            continue;

                        if (chargingTariffProperty.Value is not JArray evseIdArrayProperty)
                            continue;

                        var evseIds = new HashSet<EVSE_Id>();

                        foreach (var evseIdProperty in evseIdArrayProperty)
                        {

                            var evseIdString  = evseIdProperty?.Value<String>();
                            if (evseIdString is null)
                                continue;

                            var evseId        = EVSE_Id.TryParse(evseIdString);
                            if (!evseId.HasValue)
                                continue;

                            evseIds.Add(evseId.Value);

                        }

                        if (evseIds.Any())
                            ChargingTariffMap.Add(chargingTariffId.Value, evseIds);

                    }

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetDefaultChargingTariffResponse = new GetDefaultChargingTariffResponse(

                                                       Request,
                                                       Status,
                                                       StatusInfo,
                                                       ChargingTariffs,
                                                       new ReadOnlyDictionary<ChargingTariff_Id, IEnumerable<EVSE_Id>>(ChargingTariffMap),
                                                       null,

                                                       null,
                                                       null,
                                                       Signatures,

                                                       CustomData

                                                   );

                if (CustomGetDefaultChargingTariffResponseParser is not null)
                    GetDefaultChargingTariffResponse = CustomGetDefaultChargingTariffResponseParser(JSON,
                                                                                      GetDefaultChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                GetDefaultChargingTariffResponse  = null;
                ErrorResponse                     = "The given JSON representation of a get default charging tariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDefaultChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDefaultChargingTariffResponseSerializer">A delegate to serialize custom get default charging tariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomChargingTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDefaultChargingTariffResponse>?  CustomGetDefaultChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<ChargingTariff>?                    CustomChargingTariffSerializer                     = null,
                              CustomJObjectSerializerDelegate<Price>?                             CustomPriceSerializer                              = null,
                              CustomJObjectSerializerDelegate<TariffElement>?                     CustomTariffElementSerializer                      = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?                    CustomPriceComponentSerializer                     = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?                CustomTariffRestrictionsSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                         CustomEnergyMixSerializer                          = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                      CustomEnergySourceSerializer                       = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?               CustomEnvironmentalImpactSerializer                = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",              Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",          StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer))
                               : null,

                           ChargingTariffs.Any()
                               ? new JProperty("chargingTariffs",     new JArray (ChargingTariffs.  Select(chargingTariff => chargingTariff.ToJSON(CustomChargingTariffSerializer,
                                                                                                                                                   CustomPriceSerializer,
                                                                                                                                                   CustomTariffElementSerializer,
                                                                                                                                                   CustomPriceComponentSerializer,
                                                                                                                                                   CustomTariffRestrictionsSerializer,
                                                                                                                                                   CustomEnergyMixSerializer,
                                                                                                                                                   CustomEnergySourceSerializer,
                                                                                                                                                   CustomEnvironmentalImpactSerializer,
                                                                                                                                                   CustomSignatureSerializer,
                                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           ChargingTariffMap.Any()
                               ? new JProperty("chargingTariffMap",   new JObject(ChargingTariffMap.Select(kvp => new JProperty(kvp.Key.ToString(),
                                                                                                                  new JArray   (kvp.Value.Select(evseId => evseId.ToString()))))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray (Signatures.       Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                         CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetDefaultChargingTariffResponseSerializer is not null
                       ? CustomGetDefaultChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get default charging tariff failed.
        /// </summary>
        public static GetDefaultChargingTariffResponse Failed(CSMS.GetDefaultChargingTariffRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetDefaultChargingTariffResponse1, GetDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two get default charging tariff responses for equality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffResponse1">A get default charging tariff response.</param>
        /// <param name="GetDefaultChargingTariffResponse2">Another get default charging tariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse1,
                                           GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDefaultChargingTariffResponse1, GetDefaultChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetDefaultChargingTariffResponse1 is null || GetDefaultChargingTariffResponse2 is null)
                return false;

            return GetDefaultChargingTariffResponse1.Equals(GetDefaultChargingTariffResponse2);

        }

        #endregion

        #region Operator != (GetDefaultChargingTariffResponse1, GetDefaultChargingTariffResponse2)

        /// <summary>
        /// Compares two get default charging tariff responses for inequality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffResponse1">A get default charging tariff response.</param>
        /// <param name="GetDefaultChargingTariffResponse2">Another get default charging tariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse1,
                                           GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse2)

            => !(GetDefaultChargingTariffResponse1 == GetDefaultChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDefaultChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get default charging tariff responses for equality.
        /// </summary>
        /// <param name="Object">A get default charging tariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDefaultChargingTariffResponse getDefaultChargingTariffResponse &&
                   Equals(getDefaultChargingTariffResponse);

        #endregion

        #region Equals(GetDefaultChargingTariffResponse)

        /// <summary>
        /// Compares two get default charging tariff responses for equality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffResponse">A get default charging tariff response to compare with.</param>
        public override Boolean Equals(GetDefaultChargingTariffResponse? GetDefaultChargingTariffResponse)

            => GetDefaultChargingTariffResponse is not null &&

               Status.Equals(GetDefaultChargingTariffResponse.Status) &&

             ((StatusInfo is     null && GetDefaultChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && GetDefaultChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(GetDefaultChargingTariffResponse.StatusInfo))) &&

               ChargingTariffs.  Count().Equals(GetDefaultChargingTariffResponse.ChargingTariffs.  Count())       &&
               ChargingTariffs.  All(kvp =>     GetDefaultChargingTariffResponse.ChargingTariffs.  Contains(kvp)) &&

               ChargingTariffMap.Count().Equals(GetDefaultChargingTariffResponse.ChargingTariffMap.Count())       &&
               ChargingTariffMap.All(kvp => GetDefaultChargingTariffResponse.    ChargingTariffMap.Contains(kvp)) &&

               base.GenericEquals(GetDefaultChargingTariffResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}{(ChargingTariffMap.Any() ? $", {ChargingTariffMap.Select(kvp => $"'{kvp.Key}' => '{kvp.Value.AggregateWith(",")}'").AggregateWith("; ")}" : "")}";

        #endregion

    }

}
