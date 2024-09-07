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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    //Note: This command is a draft version of the OCPP 2.1 specification
    //       and might be subject to change in future versions of the specification!

    /// <summary>
    /// An set user charging tariff request.
    /// </summary>
    public class SetUserChargingTariffRequest : ARequest<SetUserChargingTariffRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/SetUserChargingTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification token to which this charging tariff applies.
        /// </summary>
        [Mandatory]
        public IdToken         IdToken           { get; }

        /// <summary>
        /// The charging tariff.
        /// </summary>
        [Mandatory]
        public Tariff  ChargingTariff    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set user charging tariff request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="IdToken">An identification token to which this charging tariff applies.</param>
        /// <param name="ChargingTariff">A charging tariff.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetUserChargingTariffRequest(SourceRouting            Destination,
                                            IdToken                  IdToken,
                                            Tariff           ChargingTariff,

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
                   nameof(SetUserChargingTariffRequest)[..^7],

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

            this.IdToken         = IdToken;
            this.ChargingTariff  = ChargingTariff;

            unchecked
            {

                hashCode = this.IdToken.       GetHashCode() * 5 ^
                           this.ChargingTariff.GetHashCode() * 3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomSetUserChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an SetUserChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSetUserChargingTariffRequestParser">A delegate to parse custom SetUserChargingTariff requests.</param>
        public static SetUserChargingTariffRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         CustomJObjectParserDelegate<SetUserChargingTariffRequest>?  CustomSetUserChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setUserChargingTariffRequest,
                         out var errorResponse,
                         CustomSetUserChargingTariffRequestParser) &&
                setUserChargingTariffRequest is not null)
            {
                return setUserChargingTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetUserChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out SetUserChargingTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SetUserChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetUserChargingTariffRequest">The parsed SetUserChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       Request_Id                         RequestId,
                                       SourceRouting                  Destination,
                                       NetworkPath                        NetworkPath,
                                       out SetUserChargingTariffRequest?  SetUserChargingTariffRequest,
                                       out String?                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        Destination,
                        NetworkPath,
                        out SetUserChargingTariffRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a SetUserChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetUserChargingTariffRequest">The parsed SetUserChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetUserChargingTariffRequestParser">A delegate to parse custom SetUserChargingTariff requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       out SetUserChargingTariffRequest?                           SetUserChargingTariffRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<SetUserChargingTariffRequest>?  CustomSetUserChargingTariffRequestParser)
        {

            try
            {

                SetUserChargingTariffRequest = null;

                #region IdToken              [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse) ||
                    IdToken is null)
                {
                    return false;
                }

                #endregion

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


                SetUserChargingTariffRequest = new SetUserChargingTariffRequest(

                                                   Destination,
                                                   IdToken,
                                                   ChargingTariff,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData,

                                                   RequestId,
                                                   null,
                                                   null,
                                                   null,
                                                   NetworkPath

                                               );

                if (CustomSetUserChargingTariffRequestParser is not null)
                    SetUserChargingTariffRequest = CustomSetUserChargingTariffRequestParser(JSON,
                                                                                            SetUserChargingTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                SetUserChargingTariffRequest  = null;
                ErrorResponse                 = "The given JSON representation of a SetUserChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetUserChargingTariffRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetUserChargingTariffRequestSerializer">A delegate to serialize custom SetUserChargingTariff requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomChargingTariffSerializer">A delegate to serialize custom charging tariff JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rate JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetUserChargingTariffRequest>?     CustomSetUserChargingTariffRequestSerializer      = null,
                              //CustomJObjectSerializerDelegate<IdToken>?                          CustomIdTokenSerializer                           = null,
                              //CustomJObjectSerializerDelegate<AdditionalInfo>?                   CustomAdditionalInfoSerializer                    = null,
                              //CustomJObjectSerializerDelegate<Tariff>?                   CustomChargingTariffSerializer                    = null,
                              //CustomJObjectSerializerDelegate<Price>?                            CustomPriceSerializer                             = null,
                              //CustomJObjectSerializerDelegate<TariffElement>?                    CustomTariffElementSerializer                     = null,
                              //CustomJObjectSerializerDelegate<PriceComponent>?                   CustomPriceComponentSerializer                    = null,
                              //CustomJObjectSerializerDelegate<TaxRate>?                          CustomTaxRateSerializer                           = null,
                              //CustomJObjectSerializerDelegate<TariffConditions>?               CustomTariffRestrictionsSerializer                = null,
                              //CustomJObjectSerializerDelegate<EnergyMix>?                        CustomEnergyMixSerializer                         = null,
                              //CustomJObjectSerializerDelegate<EnergySource>?                     CustomEnergySourceSerializer                      = null,
                              //CustomJObjectSerializerDelegate<EnvironmentalImpact>?              CustomEnvironmentalImpactSerializer               = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 //new JProperty("idToken",          IdToken.       ToJSON(CustomIdTokenSerializer,
                                 //                                                        CustomAdditionalInfoSerializer,
                                 //                                                        CustomCustomDataSerializer)),

                                 new JProperty("chargingTariff",   ChargingTariff.ToJSON(
                                                                                         //CustomChargingTariffSerializer,
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


                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetUserChargingTariffRequestSerializer is not null
                       ? CustomSetUserChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetUserChargingTariffRequest1, SetUserChargingTariffRequest2)

        /// <summary>
        /// Compares two SetUserChargingTariff requests for equality.
        /// </summary>
        /// <param name="SetUserChargingTariffRequest1">A SetUserChargingTariff request.</param>
        /// <param name="SetUserChargingTariffRequest2">Another setUserChargingTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetUserChargingTariffRequest? SetUserChargingTariffRequest1,
                                           SetUserChargingTariffRequest? SetUserChargingTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetUserChargingTariffRequest1, SetUserChargingTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetUserChargingTariffRequest1 is null || SetUserChargingTariffRequest2 is null)
                return false;

            return SetUserChargingTariffRequest1.Equals(SetUserChargingTariffRequest2);

        }

        #endregion

        #region Operator != (SetUserChargingTariffRequest1, SetUserChargingTariffRequest2)

        /// <summary>
        /// Compares two SetUserChargingTariff requests for inequality.
        /// </summary>
        /// <param name="SetUserChargingTariffRequest1">A SetUserChargingTariff request.</param>
        /// <param name="SetUserChargingTariffRequest2">Another setUserChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetUserChargingTariffRequest? SetUserChargingTariffRequest1,
                                           SetUserChargingTariffRequest? SetUserChargingTariffRequest2)

            => !(SetUserChargingTariffRequest1 == SetUserChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<SetUserChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetUserChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A SetUserChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetUserChargingTariffRequest SetUserChargingTariffRequest &&
                   Equals(SetUserChargingTariffRequest);

        #endregion

        #region Equals(SetUserChargingTariffRequest)

        /// <summary>
        /// Compares two SetUserChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="SetUserChargingTariffRequest">A SetUserChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(SetUserChargingTariffRequest? SetUserChargingTariffRequest)

            => SetUserChargingTariffRequest is not null &&

               IdToken.       Equals(SetUserChargingTariffRequest.IdToken)        &&
               ChargingTariff.Equals(SetUserChargingTariffRequest.ChargingTariff) &&

               base.   GenericEquals(SetUserChargingTariffRequest);

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

            => $"'{ChargingTariff.Id}' for '{IdToken}'";

        #endregion

    }

}
