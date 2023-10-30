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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// An set default charging tariff request.
    /// </summary>
    public class SetDefaultChargingTariffRequest : ARequest<SetDefaultChargingTariffRequest>,
                                                   IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/setDefaultChargingTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charging tariff.
        /// </summary>
        [Mandatory]
        public ChargingTariff        ChargingTariff    { get; }

        /// <summary>
        /// The optional enumeration of EVSEs the default charging tariff applies to.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_Id>  EVSEIds           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set default charging tariff request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="ChargingTariff">A charging tariff.</param>
        /// <param name="EVSEIds">An optional enumeration of EVSEs the default charging tariff applies to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetDefaultChargingTariffRequest(ChargingStation_Id       ChargingStationId,
                                               ChargingTariff           ChargingTariff,
                                               IEnumerable<EVSE_Id>?    EVSEIds             = null,

                                               IEnumerable<KeyPair>?    SignKeys            = null,
                                               IEnumerable<SignInfo>?   SignInfos           = null,
                                               IEnumerable<Signature>?  Signatures          = null,

                                               CustomData?              CustomData          = null,

                                               Request_Id?              RequestId           = null,
                                               DateTime?                RequestTimestamp    = null,
                                               TimeSpan?                RequestTimeout      = null,
                                               EventTracking_Id?        EventTrackingId     = null,
                                               CancellationToken        CancellationToken   = default)

            : base(ChargingStationId,
                   "SetDefaultChargingTariff",

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ChargingTariff  = ChargingTariff;
            this.EVSEIds         = EVSEIds?.Distinct() ?? Array.Empty<EVSE_Id>();

            unchecked
            {

                hashCode = this.EVSEIds.       CalcHashCode() * 5 ^
                           this.ChargingTariff.GetHashCode()  * 3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomSetDefaultChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an setDefaultChargingTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomSetDefaultChargingTariffRequestParser">A delegate to parse custom setDefaultChargingTariffs requests.</param>
        public static SetDefaultChargingTariffRequest Parse(JObject                                                        JSON,
                                                            Request_Id                                                     RequestId,
                                                            ChargingStation_Id                                             ChargingStationId,
                                                            CustomJObjectParserDelegate<SetDefaultChargingTariffRequest>?  CustomSetDefaultChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var setDefaultChargingTariffRequest,
                         out var errorResponse,
                         CustomSetDefaultChargingTariffRequestParser))
            {
                return setDefaultChargingTariffRequest!;
            }

            throw new ArgumentException("The given JSON representation of a SetDefaultChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out setDefaultChargingTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SetDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="setDefaultChargingTariffRequest">The parsed setDefaultChargingTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       Request_Id                            RequestId,
                                       ChargingStation_Id                    ChargingStationId,
                                       out SetDefaultChargingTariffRequest?  setDefaultChargingTariffRequest,
                                       out String?                           ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out setDefaultChargingTariffRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a SetDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="SetDefaultChargingTariffRequest">The parsed setDefaultChargingTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetDefaultChargingTariffRequestParser">A delegate to parse custom setDefaultChargingTariffs requests.</param>
        public static Boolean TryParse(JObject                                                        JSON,
                                       Request_Id                                                     RequestId,
                                       ChargingStation_Id                                             ChargingStationId,
                                       out SetDefaultChargingTariffRequest?                           SetDefaultChargingTariffRequest,
                                       out String?                                                    ErrorResponse,
                                       CustomJObjectParserDelegate<SetDefaultChargingTariffRequest>?  CustomSetDefaultChargingTariffRequestParser)
        {

            try
            {

                SetDefaultChargingTariffRequest = null;

                #region ChargingTariff       [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingTariff",
                                             "charging tariff",
                                             OCPPv2_1.ChargingTariff.TryParse,
                                             out ChargingTariff? ChargingTariff,
                                             out ErrorResponse) ||
                     ChargingTariff is null)
                {
                    return false;
                }

                #endregion

                #region EVSEIds              [optional]

                if (!JSON.ParseOptionalHashSet("evseIds",
                                               "EVSE identifications",
                                               EVSE_Id.TryParse,
                                               out HashSet<EVSE_Id> EVSEIds,
                                               out ErrorResponse))
                {
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingStationId    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargingStationId",
                                       "charging station identification",
                                       ChargingStation_Id.TryParse,
                                       out ChargingStation_Id? chargingStationId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargingStationId_PayLoad.HasValue)
                        ChargingStationId = chargingStationId_PayLoad.Value;

                }

                #endregion


                SetDefaultChargingTariffRequest = new SetDefaultChargingTariffRequest(
                                                      ChargingStationId,
                                                      ChargingTariff,
                                                      EVSEIds,
                                                      null,
                                                      null,
                                                      Signatures,
                                                      CustomData,
                                                      RequestId
                                                  );

                if (CustomSetDefaultChargingTariffRequestParser is not null)
                    SetDefaultChargingTariffRequest = CustomSetDefaultChargingTariffRequestParser(JSON,
                                                                                                  SetDefaultChargingTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                SetDefaultChargingTariffRequest  = null;
                ErrorResponse                    = "The given JSON representation of a SetDefaultChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDefaultChargingTariffRequestSerializer = null, CustomChargingTariffSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDefaultChargingTariffRequestSerializer">A delegate to serialize custom setDefaultChargingTariffs requests.</param>
        /// <param name="CustomChargingTariffSerializer">A delegate to serialize custom charging tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDefaultChargingTariffRequest>?  CustomSetDefaultChargingTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingTariff>?                   CustomChargingTariffSerializer                    = null,
                              CustomJObjectSerializerDelegate<DisplayText>?                      CustomDisplayTextSerializer                       = null,
                              CustomJObjectSerializerDelegate<Price>?                            CustomPriceSerializer                             = null,
                              CustomJObjectSerializerDelegate<TariffElement>?                    CustomTariffElementSerializer                     = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?                   CustomPriceComponentSerializer                    = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?               CustomTariffRestrictionsSerializer                = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                        CustomEnergyMixSerializer                         = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                     CustomEnergySourceSerializer                      = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?              CustomEnvironmentalImpactSerializer               = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingTariff",   ChargingTariff.ToJSON(CustomChargingTariffSerializer,
                                                                                         CustomDisplayTextSerializer,
                                                                                         CustomPriceSerializer,
                                                                                         CustomTariffElementSerializer,
                                                                                         CustomPriceComponentSerializer,
                                                                                         CustomTariffRestrictionsSerializer,
                                                                                         CustomEnergyMixSerializer,
                                                                                         CustomEnergySourceSerializer,
                                                                                         CustomEnvironmentalImpactSerializer,
                                                                                         CustomSignatureSerializer,
                                                                                         CustomCustomDataSerializer)),

                           EVSEIds.Any()
                               ? new JProperty("evseIds",          new JArray(EVSEIds.   Select(evseId    => evseId.   ToString())))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.    ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetDefaultChargingTariffRequestSerializer is not null
                       ? CustomSetDefaultChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetDefaultChargingTariffRequest1, SetDefaultChargingTariffRequest2)

        /// <summary>
        /// Compares two SetDefaultChargingTariff requests for equality.
        /// </summary>
        /// <param name="SetDefaultChargingTariffRequest1">A SetDefaultChargingTariff request.</param>
        /// <param name="SetDefaultChargingTariffRequest2">Another setDefaultChargingTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDefaultChargingTariffRequest? SetDefaultChargingTariffRequest1,
                                           SetDefaultChargingTariffRequest? SetDefaultChargingTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDefaultChargingTariffRequest1, SetDefaultChargingTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetDefaultChargingTariffRequest1 is null || SetDefaultChargingTariffRequest2 is null)
                return false;

            return SetDefaultChargingTariffRequest1.Equals(SetDefaultChargingTariffRequest2);

        }

        #endregion

        #region Operator != (SetDefaultChargingTariffRequest1, SetDefaultChargingTariffRequest2)

        /// <summary>
        /// Compares two SetDefaultChargingTariff requests for inequality.
        /// </summary>
        /// <param name="SetDefaultChargingTariffRequest1">A SetDefaultChargingTariff request.</param>
        /// <param name="SetDefaultChargingTariffRequest2">Another setDefaultChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDefaultChargingTariffRequest? SetDefaultChargingTariffRequest1,
                                           SetDefaultChargingTariffRequest? SetDefaultChargingTariffRequest2)

            => !(SetDefaultChargingTariffRequest1 == SetDefaultChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<SetDefaultChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDefaultChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A SetDefaultChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDefaultChargingTariffRequest setDefaultChargingTariffRequest &&
                   Equals(setDefaultChargingTariffRequest);

        #endregion

        #region Equals(SetDefaultChargingTariffRequest)

        /// <summary>
        /// Compares two SetDefaultChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="SetDefaultChargingTariffRequest">A SetDefaultChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(SetDefaultChargingTariffRequest? SetDefaultChargingTariffRequest)

            => SetDefaultChargingTariffRequest is not null &&

               ChargingTariff.Equals(SetDefaultChargingTariffRequest.ChargingTariff) &&

               EVSEIds.Count().Equals(SetDefaultChargingTariffRequest.EVSEIds.Count())     &&
               EVSEIds.All(evseId => SetDefaultChargingTariffRequest.EVSEIds.Contains(evseId)) &&

               base.   GenericEquals(SetDefaultChargingTariffRequest);

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

            => $"Set default charging tariff '{ChargingTariff.Id}'{(EVSEIds.Any() ? $" on EVSEs: {EVSEIds.AggregateWith(", ")}!" : "")}";

        #endregion

    }

}
