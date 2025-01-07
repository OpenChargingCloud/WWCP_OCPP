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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    //Note: This command is a draft version of the OCPP v2.1 specification
    //       and might be subject to change in future versions of the specification!

    /// <summary>
    /// The SetDefaultE2EChargingTariff request.
    /// </summary>
    public class SetDefaultE2EChargingTariffRequest : ARequest<SetDefaultE2EChargingTariffRequest>,
                                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setDefaultE2EChargingTariffRequest");

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
        public Tariff        ChargingTariff    { get; }

        /// <summary>
        /// The optional enumeration of EVSEs the default charging tariff applies to.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_Id>  EVSEIds           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDefaultE2EChargingTariff request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ChargingTariff">A charging tariff.</param>
        /// <param name="EVSEIds">An optional enumeration of EVSEs the default charging tariff applies to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetDefaultE2EChargingTariffRequest(SourceRouting            Destination,
                                                  Tariff           ChargingTariff,
                                                  IEnumerable<EVSE_Id>?    EVSEIds               = null,

                                                  IEnumerable<KeyPair>?    SignKeys              = null,
                                                  IEnumerable<SignInfo>?   SignInfos             = null,
                                                  IEnumerable<Signature>?  Signatures            = null,

                                                  CustomData?              CustomData            = null,

                                                  Request_Id?              RequestId             = null,
                                                  DateTime?                RequestTimestamp      = null,
                                                  TimeSpan?                RequestTimeout        = null,
                                                  EventTracking_Id?        EventTrackingId       = null,
                                                  NetworkPath?             NetworkPath           = null,
                                                  SerializationFormats?    SerializationFormat   = null,
                                                  CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(SetDefaultE2EChargingTariffRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
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

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomSetDefaultE2EChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an setDefaultE2EChargingTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDefaultE2EChargingTariffRequestParser">A delegate to parse custom setDefaultE2EChargingTariffs requests.</param>
        public static SetDefaultE2EChargingTariffRequest Parse(JObject                                                           JSON,
                                                               Request_Id                                                        RequestId,
                                                               SourceRouting                                                     Destination,
                                                               NetworkPath                                                       NetworkPath,
                                                               DateTime?                                                         RequestTimestamp                                 = null,
                                                               TimeSpan?                                                         RequestTimeout                                   = null,
                                                               EventTracking_Id?                                                 EventTrackingId                                  = null,
                                                               CustomJObjectParserDelegate<SetDefaultE2EChargingTariffRequest>?  CustomSetDefaultE2EChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setDefaultE2EChargingTariffRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetDefaultE2EChargingTariffRequestParser))
            {
                return setDefaultE2EChargingTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetDefaultE2EChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out SetDefaultE2EChargingTariffRequest, out ErrorResponse, CustomSetDefaultE2EChargingTariffRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDefaultE2EChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetDefaultE2EChargingTariffRequest">The parsed setDefaultE2EChargingTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDefaultE2EChargingTariffRequestParser">A delegate to parse custom setDefaultE2EChargingTariffs requests.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       Request_Id                                                        RequestId,
                                       SourceRouting                                                     Destination,
                                       NetworkPath                                                       NetworkPath,
                                       [NotNullWhen(true)]  out SetDefaultE2EChargingTariffRequest?      SetDefaultE2EChargingTariffRequest,
                                       [NotNullWhen(false)] out String?                                  ErrorResponse,
                                       DateTime?                                                         RequestTimestamp                                 = null,
                                       TimeSpan?                                                         RequestTimeout                                   = null,
                                       EventTracking_Id?                                                 EventTrackingId                                  = null,
                                       CustomJObjectParserDelegate<SetDefaultE2EChargingTariffRequest>?  CustomSetDefaultE2EChargingTariffRequestParser   = null)
        {

            try
            {

                SetDefaultE2EChargingTariffRequest = null;

                #region ChargingTariff       [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingTariff",
                                             "charging tariff",
                                             OCPPv2_1.Tariff.TryParse,
                                             out Tariff? ChargingTariff,
                                             out ErrorResponse) ||
                     ChargingTariff is null)
                {
                    return false;
                }

                #endregion

                #region EVSEIds              [optional]

                if (JSON.ParseOptionalHashSet("evseIds",
                                              "EVSE identifications",
                                              EVSE_Id.TryParse,
                                              out HashSet<EVSE_Id> EVSEIds,
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


                SetDefaultE2EChargingTariffRequest = new SetDefaultE2EChargingTariffRequest(

                                                         Destination,
                                                         ChargingTariff,
                                                         EVSEIds,

                                                         null,
                                                         null,
                                                         Signatures,

                                                         CustomData,

                                                         RequestId,
                                                         RequestTimestamp,
                                                         RequestTimeout,
                                                         EventTrackingId,
                                                         NetworkPath

                                                     );

                if (CustomSetDefaultE2EChargingTariffRequestParser is not null)
                    SetDefaultE2EChargingTariffRequest = CustomSetDefaultE2EChargingTariffRequestParser(JSON,
                                                                                                        SetDefaultE2EChargingTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                SetDefaultE2EChargingTariffRequest  = null;
                ErrorResponse                       = "The given JSON representation of a SetDefaultE2EChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDefaultE2EChargingTariffRequestSerializer = null, CustomChargingTariffSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDefaultE2EChargingTariffRequestSerializer">A delegate to serialize custom setDefaultE2EChargingTariffs requests.</param>
        /// <param name="CustomChargingTariffSerializer">A delegate to serialize custom charging tariff JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rate JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDefaultE2EChargingTariffRequest>?  CustomSetDefaultE2EChargingTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Tariff>?                              CustomChargingTariffSerializer                    = null,
                              //CustomJObjectSerializerDelegate<Price>?                               CustomPriceSerializer                             = null,
                              //CustomJObjectSerializerDelegate<TariffElement>?                       CustomTariffElementSerializer                     = null,
                              //CustomJObjectSerializerDelegate<PriceComponent>?                      CustomPriceComponentSerializer                    = null,
                              //CustomJObjectSerializerDelegate<TaxRate>?                             CustomTaxRateSerializer                           = null,
                              //CustomJObjectSerializerDelegate<TariffConditions>?                    CustomTariffRestrictionsSerializer                = null,
                              //CustomJObjectSerializerDelegate<EnergyMix>?                           CustomEnergyMixSerializer                         = null,
                              //CustomJObjectSerializerDelegate<EnergySource>?                        CustomEnergySourceSerializer                      = null,
                              //CustomJObjectSerializerDelegate<EnvironmentalImpact>?                 CustomEnvironmentalImpactSerializer               = null,
                              //CustomJObjectSerializerDelegate<IdToken>?                             CustomIdTokenSerializer                           = null,
                              //CustomJObjectSerializerDelegate<AdditionalInfo>?                      CustomAdditionalInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<Signature>?                           CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingTariff",   ChargingTariff.ToJSON(CustomChargingTariffSerializer
                                                                                         //CustomPriceSerializer,
                                                                                         //CustomTariffElementSerializer,
                                                                                         //CustomPriceComponentSerializer,
                                                                                         //CustomTaxRateSerializer,
                                                                                         //CustomTariffRestrictionsSerializer,
                                                                                         //CustomEnergyMixSerializer,
                                                                                         //CustomEnergySourceSerializer,
                                                                                         //CustomEnvironmentalImpactSerializer,
                                                                                         //CustomIdTokenSerializer,
                                                                                         //CustomAdditionalInfoSerializer,
                                                                                         //CustomSignatureSerializer,
                                                                                         //CustomCustomDataSerializer
                                                                                         )),

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

            return CustomSetDefaultE2EChargingTariffRequestSerializer is not null
                       ? CustomSetDefaultE2EChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetDefaultE2EChargingTariffRequest1, SetDefaultE2EChargingTariffRequest2)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariff requests for equality.
        /// </summary>
        /// <param name="SetDefaultE2EChargingTariffRequest1">A SetDefaultE2EChargingTariff request.</param>
        /// <param name="SetDefaultE2EChargingTariffRequest2">Another setDefaultE2EChargingTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDefaultE2EChargingTariffRequest? SetDefaultE2EChargingTariffRequest1,
                                           SetDefaultE2EChargingTariffRequest? SetDefaultE2EChargingTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDefaultE2EChargingTariffRequest1, SetDefaultE2EChargingTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetDefaultE2EChargingTariffRequest1 is null || SetDefaultE2EChargingTariffRequest2 is null)
                return false;

            return SetDefaultE2EChargingTariffRequest1.Equals(SetDefaultE2EChargingTariffRequest2);

        }

        #endregion

        #region Operator != (SetDefaultE2EChargingTariffRequest1, SetDefaultE2EChargingTariffRequest2)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariff requests for inequality.
        /// </summary>
        /// <param name="SetDefaultE2EChargingTariffRequest1">A SetDefaultE2EChargingTariff request.</param>
        /// <param name="SetDefaultE2EChargingTariffRequest2">Another setDefaultE2EChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDefaultE2EChargingTariffRequest? SetDefaultE2EChargingTariffRequest1,
                                           SetDefaultE2EChargingTariffRequest? SetDefaultE2EChargingTariffRequest2)

            => !(SetDefaultE2EChargingTariffRequest1 == SetDefaultE2EChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<SetDefaultE2EChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A SetDefaultE2EChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDefaultE2EChargingTariffRequest setDefaultE2EChargingTariffRequest &&
                   Equals(setDefaultE2EChargingTariffRequest);

        #endregion

        #region Equals(SetDefaultE2EChargingTariffRequest)

        /// <summary>
        /// Compares two SetDefaultE2EChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="SetDefaultE2EChargingTariffRequest">A SetDefaultE2EChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(SetDefaultE2EChargingTariffRequest? SetDefaultE2EChargingTariffRequest)

            => SetDefaultE2EChargingTariffRequest is not null &&

               ChargingTariff.Equals(SetDefaultE2EChargingTariffRequest.ChargingTariff) &&

               EVSEIds.Count().Equals(SetDefaultE2EChargingTariffRequest.EVSEIds.Count())     &&
               EVSEIds.All(evseId => SetDefaultE2EChargingTariffRequest.EVSEIds.Contains(evseId)) &&

               base.   GenericEquals(SetDefaultE2EChargingTariffRequest);

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
