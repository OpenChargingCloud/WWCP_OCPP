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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The ChangeTransactionTariff request.
    /// </summary>
    public class ChangeTransactionTariffRequest : ARequest<ChangeTransactionTariffRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/changeTransactionTariff");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The transaction identification of the transaction to change the tariff for.
        /// </summary>
        [Mandatory]
        public Transaction_Id  TransactionId    { get; }

        /// <summary>
        /// The tariff the be applied to the transaction.
        /// </summary>
        [Mandatory]
        public Tariff          Tariff           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChangeTransactionTariff request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="TransactionId">The transaction identification of the transaction to change the tariff for.</param>
        /// <param name="Tariff">The tariff the be applied to the transaction.</param>
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
        public ChangeTransactionTariffRequest(SourceRouting            Destination,
                                              Transaction_Id           TransactionId,
                                              Tariff                   Tariff,

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
                   nameof(ChangeTransactionTariffRequest)[..^7],

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

            this.TransactionId  = TransactionId;
            this.Tariff         = Tariff;

            unchecked
            {
                hashCode = this.TransactionId.GetHashCode() * 5 ^
                           this.Tariff.       GetHashCode() * 3 ^
                           base.              GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomChangeTransactionTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ChangeTransactionTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomChangeTransactionTariffRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static ChangeTransactionTariffRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<ChangeTransactionTariffRequest>?  CustomChangeTransactionTariffRequestParser   = null,
                                                           CustomJObjectParserDelegate<Tariff>?                          CustomTariffParser                           = null,
                                                           CustomJObjectParserDelegate<MessageContent>?                  CustomMessageContentParser                   = null,
                                                           CustomJObjectParserDelegate<Price>?                           CustomPriceParser                            = null,
                                                           CustomJObjectParserDelegate<TaxRate>?                         CustomTaxRateParser                          = null,
                                                           CustomJObjectParserDelegate<TariffConditions>?                CustomTariffConditionsParser                 = null,
                                                           CustomJObjectParserDelegate<TariffEnergy>?                    CustomTariffEnergyParser                     = null,
                                                           CustomJObjectParserDelegate<TariffEnergyPrice>?               CustomTariffEnergyPriceParser                = null,
                                                           CustomJObjectParserDelegate<TariffTime>?                      CustomTariffTimeParser                       = null,
                                                           CustomJObjectParserDelegate<TariffTimePrice>?                 CustomTariffTimePriceParser                  = null,
                                                           CustomJObjectParserDelegate<TariffFixed>?                     CustomTariffFixedParser                      = null,
                                                           CustomJObjectParserDelegate<TariffFixedPrice>?                CustomTariffFixedPriceParser                 = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var changeTransactionTariffRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomChangeTransactionTariffRequestParser,
                         CustomTariffParser,
                         CustomMessageContentParser,
                         CustomPriceParser,
                         CustomTaxRateParser,
                         CustomTariffConditionsParser,
                         CustomTariffEnergyParser,
                         CustomTariffEnergyPriceParser,
                         CustomTariffTimeParser,
                         CustomTariffTimePriceParser,
                         CustomTariffFixedParser,
                         CustomTariffFixedPriceParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return changeTransactionTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a ChangeTransactionTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out ChangeTransactionTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ChangeTransactionTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ChangeTransactionTariffRequest">The parsed setTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomChangeTransactionTariffRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out ChangeTransactionTariffRequest?      ChangeTransactionTariffRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<ChangeTransactionTariffRequest>?  CustomChangeTransactionTariffRequestParser   = null,
                                       CustomJObjectParserDelegate<Tariff>?                          CustomTariffParser                           = null,
                                       CustomJObjectParserDelegate<MessageContent>?                  CustomMessageContentParser                   = null,
                                       CustomJObjectParserDelegate<Price>?                           CustomPriceParser                            = null,
                                       CustomJObjectParserDelegate<TaxRate>?                         CustomTaxRateParser                          = null,
                                       CustomJObjectParserDelegate<TariffConditions>?                CustomTariffConditionsParser                 = null,
                                       CustomJObjectParserDelegate<TariffEnergy>?                    CustomTariffEnergyParser                     = null,
                                       CustomJObjectParserDelegate<TariffEnergyPrice>?               CustomTariffEnergyPriceParser                = null,
                                       CustomJObjectParserDelegate<TariffTime>?                      CustomTariffTimeParser                       = null,
                                       CustomJObjectParserDelegate<TariffTimePrice>?                 CustomTariffTimePriceParser                  = null,
                                       CustomJObjectParserDelegate<TariffFixed>?                     CustomTariffFixedParser                      = null,
                                       CustomJObjectParserDelegate<TariffFixedPrice>?                CustomTariffFixedPriceParser                 = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            try
            {

                ChangeTransactionTariffRequest = null;

                #region TransactionId    [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Tariff           [mandatory]

                if (!JSON.ParseMandatoryJSON("tariff",
                                             "tariff",
                                             OCPPv2_1.Tariff.TryParse,
                                             out Tariff? Tariff,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                ChangeTransactionTariffRequest = new ChangeTransactionTariffRequest(

                                                     Destination,
                                                     TransactionId,
                                                     Tariff,

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

                if (CustomChangeTransactionTariffRequestParser is not null)
                    ChangeTransactionTariffRequest = CustomChangeTransactionTariffRequestParser(JSON,
                                                                                                ChangeTransactionTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                ChangeTransactionTariffRequest  = null;
                ErrorResponse                   = "The given JSON representation of a ChangeTransactionTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChangeTransactionTariffRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeTransactionTariffRequestSerializer">A delegate to serialize custom setTariffs requests.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        /// <param name="CustomTariffEnergySerializer">A delegate to serialize custom TariffEnergy JSON objects.</param>
        /// <param name="CustomTariffEnergyPriceSerializer">A delegate to serialize custom TariffEnergyPrice JSON objects.</param>
        /// <param name="CustomTariffTimeSerializer">A delegate to serialize custom TariffTime JSON objects.</param>
        /// <param name="CustomTariffTimePriceSerializer">A delegate to serialize custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffFixedSerializer">A delegate to serialize custom TariffFixed JSON objects.</param>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeTransactionTariffRequest>?  CustomChangeTransactionTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Tariff>?                          CustomTariffSerializer                           = null,
                              CustomJObjectSerializerDelegate<MessageContent>?                  CustomMessageContentSerializer                   = null,
                              CustomJObjectSerializerDelegate<Price>?                           CustomPriceSerializer                            = null,
                              CustomJObjectSerializerDelegate<TaxRate>?                         CustomTaxRateSerializer                          = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?                CustomTariffConditionsSerializer                 = null,
                              CustomJObjectSerializerDelegate<TariffEnergy>?                    CustomTariffEnergySerializer                     = null,
                              CustomJObjectSerializerDelegate<TariffEnergyPrice>?               CustomTariffEnergyPriceSerializer                = null,
                              CustomJObjectSerializerDelegate<TariffTime>?                      CustomTariffTimeSerializer                       = null,
                              CustomJObjectSerializerDelegate<TariffTimePrice>?                 CustomTariffTimePriceSerializer                  = null,
                              CustomJObjectSerializerDelegate<TariffFixed>?                     CustomTariffFixedSerializer                      = null,
                              CustomJObjectSerializerDelegate<TariffFixedPrice>?                CustomTariffFixedPriceSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("transactionId",   TransactionId.ToString()),
                                 new JProperty("tariff",          Tariff.       ToJSON(CustomTariffSerializer,
                                                                                       CustomMessageContentSerializer,
                                                                                       CustomPriceSerializer,
                                                                                       CustomTaxRateSerializer,
                                                                                       CustomTariffConditionsSerializer,
                                                                                       CustomTariffEnergySerializer,
                                                                                       CustomTariffEnergyPriceSerializer,
                                                                                       CustomTariffTimeSerializer,
                                                                                       CustomTariffTimePriceSerializer,
                                                                                       CustomTariffFixedSerializer,
                                                                                       CustomTariffFixedPriceSerializer,
                                                                                       CustomSignatureSerializer,
                                                                                       CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.  ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChangeTransactionTariffRequestSerializer is not null
                       ? CustomChangeTransactionTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChangeTransactionTariffRequest1, ChangeTransactionTariffRequest2)

        /// <summary>
        /// Compares two ChangeTransactionTariff requests for equality.
        /// </summary>
        /// <param name="ChangeTransactionTariffRequest1">A ChangeTransactionTariff request.</param>
        /// <param name="ChangeTransactionTariffRequest2">Another setTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeTransactionTariffRequest? ChangeTransactionTariffRequest1,
                                           ChangeTransactionTariffRequest? ChangeTransactionTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeTransactionTariffRequest1, ChangeTransactionTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeTransactionTariffRequest1 is null || ChangeTransactionTariffRequest2 is null)
                return false;

            return ChangeTransactionTariffRequest1.Equals(ChangeTransactionTariffRequest2);

        }

        #endregion

        #region Operator != (ChangeTransactionTariffRequest1, ChangeTransactionTariffRequest2)

        /// <summary>
        /// Compares two ChangeTransactionTariff requests for inequality.
        /// </summary>
        /// <param name="ChangeTransactionTariffRequest1">A ChangeTransactionTariff request.</param>
        /// <param name="ChangeTransactionTariffRequest2">Another setTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeTransactionTariffRequest? ChangeTransactionTariffRequest1,
                                           ChangeTransactionTariffRequest? ChangeTransactionTariffRequest2)

            => !(ChangeTransactionTariffRequest1 == ChangeTransactionTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<ChangeTransactionTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChangeTransactionTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A ChangeTransactionTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeTransactionTariffRequest changeTransactionTariffRequest &&
                   Equals(changeTransactionTariffRequest);

        #endregion

        #region Equals(ChangeTransactionTariffRequest)

        /// <summary>
        /// Compares two ChangeTransactionTariffRequest requests for equality.
        /// </summary>
        /// <param name="ChangeTransactionTariffRequest">A ChangeTransactionTariffRequest request to compare with.</param>
        public override Boolean Equals(ChangeTransactionTariffRequest? ChangeTransactionTariffRequest)

            => ChangeTransactionTariffRequest is not null &&

               TransactionId.Equals(ChangeTransactionTariffRequest.TransactionId) &&
               Tariff.       Equals(ChangeTransactionTariffRequest.Tariff)        &&

               base.GenericEquals(ChangeTransactionTariffRequest);

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

            => $"Change tariff '{Tariff.Id}' for transaction '{TransactionId}'";

        #endregion

    }

}
