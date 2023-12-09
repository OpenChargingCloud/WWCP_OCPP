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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// An set transaction charging tariff request.
    /// </summary>
    public class SetTransactionChargingTariffRequest : ARequest<SetTransactionChargingTariffRequest>,
                                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext TransactionJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/SetTransactionChargingTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => TransactionJSONLDContext;

        /// <summary>
        /// The transaction identification for which the applicable charging tariff is requested.
        /// </summary>
        [Mandatory]
        public Transaction_Id  TransactionId     { get; }

        /// <summary>
        /// The charging tariff.
        /// </summary>
        [Mandatory]
        public ChargingTariff  ChargingTariff    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set transaction charging tariff request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="TransactionId">An transaction identification for which the applicable charging tariff is requested.</param>
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
        public SetTransactionChargingTariffRequest(NetworkingNode_Id        NetworkingNodeId,
                                                   Transaction_Id           TransactionId,
                                                   ChargingTariff           ChargingTariff,

                                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                                   IEnumerable<OCPP.Signature>?  Signatures          = null,

                                                   CustomData?              CustomData          = null,

                                                   Request_Id?              RequestId           = null,
                                                   DateTime?                RequestTimestamp    = null,
                                                   TimeSpan?                RequestTimeout      = null,
                                                   EventTracking_Id?        EventTrackingId     = null,
                                                   NetworkPath?             NetworkPath         = null,
                                                   CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(SetTransactionChargingTariffRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.TransactionId   = TransactionId;
            this.ChargingTariff  = ChargingTariff;

            unchecked
            {

                hashCode = this.TransactionId. GetHashCode() * 5 ^
                           this.ChargingTariff.GetHashCode() * 3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomSetTransactionChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an SetTransactionChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSetTransactionChargingTariffRequestParser">A delegate to parse custom SetTransactionChargingTariff requests.</param>
        public static SetTransactionChargingTariffRequest Parse(JObject                                                            JSON,
                                                                Request_Id                                                         RequestId,
                                                                NetworkingNode_Id                                                  NetworkingNodeId,
                                                                NetworkPath                                                        NetworkPath,
                                                                CustomJObjectParserDelegate<SetTransactionChargingTariffRequest>?  CustomSetTransactionChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var setTransactionChargingTariffRequest,
                         out var errorResponse,
                         CustomSetTransactionChargingTariffRequestParser) &&
                setTransactionChargingTariffRequest is not null)
            {
                return setTransactionChargingTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetTransactionChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out SetTransactionChargingTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SetTransactionChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetTransactionChargingTariffRequest">The parsed SetTransactionChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       Request_Id                                RequestId,
                                       NetworkingNode_Id                         NetworkingNodeId,
                                       NetworkPath                               NetworkPath,
                                       out SetTransactionChargingTariffRequest?  SetTransactionChargingTariffRequest,
                                       out String?                               ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out SetTransactionChargingTariffRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a SetTransactionChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetTransactionChargingTariffRequest">The parsed SetTransactionChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetTransactionChargingTariffRequestParser">A delegate to parse custom SetTransactionChargingTariff requests.</param>
        public static Boolean TryParse(JObject                                                            JSON,
                                       Request_Id                                                         RequestId,
                                       NetworkingNode_Id                                                  NetworkingNodeId,
                                       NetworkPath                                                        NetworkPath,
                                       out SetTransactionChargingTariffRequest?                           SetTransactionChargingTariffRequest,
                                       out String?                                                        ErrorResponse,
                                       CustomJObjectParserDelegate<SetTransactionChargingTariffRequest>?  CustomSetTransactionChargingTariffRequestParser)
        {

            try
            {

                SetTransactionChargingTariffRequest = null;

                #region TransactionId        [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

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


                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                SetTransactionChargingTariffRequest = new SetTransactionChargingTariffRequest(

                                                          NetworkingNodeId,
                                                          TransactionId,
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

                if (CustomSetTransactionChargingTariffRequestParser is not null)
                    SetTransactionChargingTariffRequest = CustomSetTransactionChargingTariffRequestParser(JSON,
                                                                                                          SetTransactionChargingTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                SetTransactionChargingTariffRequest  = null;
                ErrorResponse                        = "The given JSON representation of a SetTransactionChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetTransactionChargingTariffRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetTransactionChargingTariffRequestSerializer">A delegate to serialize custom SetTransactionChargingTariff requests.</param>
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetTransactionChargingTariffRequest>?     CustomSetTransactionChargingTariffRequestSerializer      = null,
                              CustomJObjectSerializerDelegate<IdToken>?                          CustomIdTokenSerializer                           = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?                   CustomAdditionalInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<ChargingTariff>?                   CustomChargingTariffSerializer                    = null,
                              CustomJObjectSerializerDelegate<Price>?                            CustomPriceSerializer                             = null,
                              CustomJObjectSerializerDelegate<TariffElement>?                    CustomTariffElementSerializer                     = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?                   CustomPriceComponentSerializer                    = null,
                              CustomJObjectSerializerDelegate<TaxRate>?                          CustomTaxRateSerializer                           = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?               CustomTariffRestrictionsSerializer                = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?                        CustomEnergyMixSerializer                         = null,
                              CustomJObjectSerializerDelegate<EnergySource>?                     CustomEnergySourceSerializer                      = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?              CustomEnvironmentalImpactSerializer               = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                   CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("transactionId",    TransactionId.       ToString()),

                                 new JProperty("chargingTariff",   ChargingTariff.ToJSON(CustomChargingTariffSerializer,
                                                                                         CustomPriceSerializer,
                                                                                         CustomTariffElementSerializer,
                                                                                         CustomPriceComponentSerializer,
                                                                                         CustomTaxRateSerializer,
                                                                                         CustomTariffRestrictionsSerializer,
                                                                                         CustomEnergyMixSerializer,
                                                                                         CustomEnergySourceSerializer,
                                                                                         CustomEnvironmentalImpactSerializer,
                                                                                         CustomIdTokenSerializer,
                                                                                         CustomAdditionalInfoSerializer,
                                                                                         CustomSignatureSerializer,
                                                                                         CustomCustomDataSerializer)),


                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetTransactionChargingTariffRequestSerializer is not null
                       ? CustomSetTransactionChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetTransactionChargingTariffRequest1, SetTransactionChargingTariffRequest2)

        /// <summary>
        /// Compares two SetTransactionChargingTariff requests for equality.
        /// </summary>
        /// <param name="SetTransactionChargingTariffRequest1">A SetTransactionChargingTariff request.</param>
        /// <param name="SetTransactionChargingTariffRequest2">Another setTransactionChargingTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetTransactionChargingTariffRequest? SetTransactionChargingTariffRequest1,
                                           SetTransactionChargingTariffRequest? SetTransactionChargingTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetTransactionChargingTariffRequest1, SetTransactionChargingTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetTransactionChargingTariffRequest1 is null || SetTransactionChargingTariffRequest2 is null)
                return false;

            return SetTransactionChargingTariffRequest1.Equals(SetTransactionChargingTariffRequest2);

        }

        #endregion

        #region Operator != (SetTransactionChargingTariffRequest1, SetTransactionChargingTariffRequest2)

        /// <summary>
        /// Compares two SetTransactionChargingTariff requests for inequality.
        /// </summary>
        /// <param name="SetTransactionChargingTariffRequest1">A SetTransactionChargingTariff request.</param>
        /// <param name="SetTransactionChargingTariffRequest2">Another setTransactionChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetTransactionChargingTariffRequest? SetTransactionChargingTariffRequest1,
                                           SetTransactionChargingTariffRequest? SetTransactionChargingTariffRequest2)

            => !(SetTransactionChargingTariffRequest1 == SetTransactionChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<SetTransactionChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetTransactionChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A SetTransactionChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetTransactionChargingTariffRequest SetTransactionChargingTariffRequest &&
                   Equals(SetTransactionChargingTariffRequest);

        #endregion

        #region Equals(SetTransactionChargingTariffRequest)

        /// <summary>
        /// Compares two SetTransactionChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="SetTransactionChargingTariffRequest">A SetTransactionChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(SetTransactionChargingTariffRequest? SetTransactionChargingTariffRequest)

            => SetTransactionChargingTariffRequest is not null &&

               TransactionId. Equals(SetTransactionChargingTariffRequest.TransactionId)  &&
               ChargingTariff.Equals(SetTransactionChargingTariffRequest.ChargingTariff) &&

               base.   GenericEquals(SetTransactionChargingTariffRequest);

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

            => $"'{ChargingTariff.Id}' for '{TransactionId}'";

        #endregion

    }

}
