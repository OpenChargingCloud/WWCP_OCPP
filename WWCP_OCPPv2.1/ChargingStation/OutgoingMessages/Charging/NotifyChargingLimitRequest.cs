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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A notify charging limit request.
    /// </summary>
    public class NotifyChargingLimitRequest : ARequest<NotifyChargingLimitRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyChargingLimitRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charging limit, its source and whether it is grid critical.
        /// </summary>
        [Mandatory]
        public ChargingLimit                  ChargingLimit        { get; }

        /// <summary>
        /// Limits for the available power or current over time, as set by the external source.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingSchedule>  ChargingSchedules    { get; }

        /// <summary>
        /// The optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.
        /// </summary>
        [Optional]
        public EVSE_Id?                       EVSEId               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify charging limit request.
        /// </summary>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="ChargingLimit">The charging limit, its source and whether it is grid critical.</param>
        /// <param name="ChargingSchedules">Optional limits for the available power or current over time, as set by the external source.</param>
        /// <param name="EVSEId">An optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.</param>
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
        public NotifyChargingLimitRequest(NetworkingNode_Id               NetworkingNodeId,
                                          ChargingLimit                   ChargingLimit,
                                          IEnumerable<ChargingSchedule>?  ChargingSchedules   = null,
                                          EVSE_Id?                        EVSEId              = null,

                                          IEnumerable<KeyPair>?           SignKeys            = null,
                                          IEnumerable<SignInfo>?          SignInfos           = null,
                                          IEnumerable<OCPP.Signature>?    Signatures          = null,
                                          CustomData?                     CustomData          = null,

                                          Request_Id?                     RequestId           = null,
                                          DateTime?                       RequestTimestamp    = null,
                                          TimeSpan?                       RequestTimeout      = null,
                                          EventTracking_Id?               EventTrackingId     = null,
                                          NetworkPath?                    NetworkPath         = null,
                                          CancellationToken               CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(NotifyChargingLimitRequest)[..^7],

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

            this.ChargingLimit      = ChargingLimit;
            this.ChargingSchedules  = ChargingSchedules?.Distinct() ?? Array.Empty<ChargingSchedule>();
            this.EVSEId             = EVSEId;


            unchecked
            {

                hashCode = this.ChargingLimit.    GetHashCode()       * 7 ^
                           this.ChargingSchedules.CalcHashCode()      * 5 ^
                          (this.EVSEId?.          GetHashCode() ?? 0) * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomNotifyChargingLimitRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomNotifyChargingLimitRequestParser">A delegate to parse custom notify charging limit requests.</param>
        public static NotifyChargingLimitRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       NetworkingNode_Id                                         NetworkingNodeId,
                                                       NetworkPath                                               NetworkPath,
                                                       CustomJObjectParserDelegate<NotifyChargingLimitRequest>?  CustomNotifyChargingLimitRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var notifyChargingLimitRequest,
                         out var errorResponse,
                         CustomNotifyChargingLimitRequestParser) &&
                notifyChargingLimitRequest is not null)
            {
                return notifyChargingLimitRequest;
            }

            throw new ArgumentException("The given JSON representation of a notify charging limit request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out NotifyChargingLimitRequest, out ErrorResponse, CustomNotifyChargingLimitRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a notify charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyChargingLimitRequest">The parsed notify charging limit request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       Request_Id                       RequestId,
                                       NetworkingNode_Id                NetworkingNodeId,
                                       NetworkPath                      NetworkPath,
                                       out NotifyChargingLimitRequest?  NotifyChargingLimitRequest,
                                       out String?                      ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out NotifyChargingLimitRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a notify charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyChargingLimitRequest">The parsed notify charging limit request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyChargingLimitRequestParser">A delegate to parse custom notify charging limit requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       NetworkingNode_Id                                         NetworkingNodeId,
                                       NetworkPath                                               NetworkPath,
                                       out NotifyChargingLimitRequest?                           NotifyChargingLimitRequest,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyChargingLimitRequest>?  CustomNotifyChargingLimitRequestParser)
        {

            try
            {

                NotifyChargingLimitRequest = null;

                #region ChargingLimit        [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingLimit",
                                             "charging limit",
                                             OCPPv2_1.ChargingLimit.TryParse,
                                             out ChargingLimit? ChargingLimit,
                                             out ErrorResponse) ||
                     ChargingLimit is null)
                {
                    return false;
                }

                #endregion

                #region ChargingSchedules    [optional]

                if (JSON.ParseOptionalHashSet("chargingSchedule",
                                              "charging schedule",
                                              ChargingSchedule.TryParse,
                                              out HashSet<ChargingSchedule> ChargingSchedules,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSEId               [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                NotifyChargingLimitRequest = new NotifyChargingLimitRequest(

                                                 NetworkingNodeId,
                                                 ChargingLimit,
                                                 ChargingSchedules,
                                                 EVSEId,

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

                if (CustomNotifyChargingLimitRequestParser is not null)
                    NotifyChargingLimitRequest = CustomNotifyChargingLimitRequestParser(JSON,
                                                                                        NotifyChargingLimitRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyChargingLimitRequest  = null;
                ErrorResponse               = "The given JSON representation of a notify charging limit request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyChargingLimitRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyChargingLimitRequestSerializer">A delegate to serialize custom NotifyChargingLimit requests.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomLimitBeyondSoCSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomV2XFreqWattEntrySerializer">A delegate to serialize custom V2X Frequency-Watt entrys.</param>
        /// <param name="CustomV2XSignalWattEntrySerializer">A delegate to serialize custom V2X Signal-Watt entrys.</param>
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom salesTariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// 
        /// <param name="CustomAbsolutePriceScheduleSerializer">A delegate to serialize custom absolute price schedules.</param>
        /// <param name="CustomPriceRuleStackSerializer">A delegate to serialize custom price rule stacks.</param>
        /// <param name="CustomPriceRuleSerializer">A delegate to serialize custom price rules.</param>
        /// <param name="CustomTaxRuleSerializer">A delegate to serialize custom tax rules.</param>
        /// <param name="CustomOverstayRuleListSerializer">A delegate to serialize custom overstay rule lists.</param>
        /// <param name="CustomOverstayRuleSerializer">A delegate to serialize custom overstay rules.</param>
        /// <param name="CustomAdditionalServiceSerializer">A delegate to serialize custom additional services.</param>
        /// 
        /// <param name="CustomPriceLevelScheduleSerializer">A delegate to serialize custom price level schedules.</param>
        /// <param name="CustomPriceLevelScheduleEntrySerializer">A delegate to serialize custom price level schedule entries.</param>
        /// 
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer             = null,
                              CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer               = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer       = null,
                              CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer             = null,
                              CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer           = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                  = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer             = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer         = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer              = null,
                              CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                         = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer               = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                      = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer             = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                 = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer            = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer           = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer      = null,

                              CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingLimit",      ChargingLimit.ToJSON()),

                           ChargingSchedules.Any()
                               ? new JProperty("chargingSchedule",   new JArray(ChargingSchedules.Select(chargingSchedule => chargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                                                                                     CustomLimitBeyondSoCSerializer,
                                                                                                                                                     CustomChargingSchedulePeriodSerializer,
                                                                                                                                                     CustomV2XFreqWattEntrySerializer,
                                                                                                                                                     CustomV2XSignalWattEntrySerializer,
                                                                                                                                                     CustomSalesTariffSerializer,
                                                                                                                                                     CustomSalesTariffEntrySerializer,
                                                                                                                                                     CustomRelativeTimeIntervalSerializer,
                                                                                                                                                     CustomConsumptionCostSerializer,
                                                                                                                                                     CustomCostSerializer,

                                                                                                                                                     CustomAbsolutePriceScheduleSerializer,
                                                                                                                                                     CustomPriceRuleStackSerializer,
                                                                                                                                                     CustomPriceRuleSerializer,
                                                                                                                                                     CustomTaxRuleSerializer,
                                                                                                                                                     CustomOverstayRuleListSerializer,
                                                                                                                                                     CustomOverstayRuleSerializer,
                                                                                                                                                     CustomAdditionalServiceSerializer,

                                                                                                                                                     CustomPriceLevelScheduleSerializer,
                                                                                                                                                     CustomPriceLevelScheduleEntrySerializer,

                                                                                                                                                     CustomCustomDataSerializer))))
                               : null,

                           EVSEId.HasValue
                               ? new JProperty("evseId",             EVSEId.       Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyChargingLimitRequestSerializer is not null
                       ? CustomNotifyChargingLimitRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyChargingLimitRequest1, NotifyChargingLimitRequest2)

        /// <summary>
        /// Compares two notify charging limit requests for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitRequest1">A notify charging limit request.</param>
        /// <param name="NotifyChargingLimitRequest2">Another notify charging limit request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyChargingLimitRequest? NotifyChargingLimitRequest1,
                                           NotifyChargingLimitRequest? NotifyChargingLimitRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyChargingLimitRequest1, NotifyChargingLimitRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyChargingLimitRequest1 is null || NotifyChargingLimitRequest2 is null)
                return false;

            return NotifyChargingLimitRequest1.Equals(NotifyChargingLimitRequest2);

        }

        #endregion

        #region Operator != (NotifyChargingLimitRequest1, NotifyChargingLimitRequest2)

        /// <summary>
        /// Compares two notify charging limit requests for inequality.
        /// </summary>
        /// <param name="NotifyChargingLimitRequest1">A notify charging limit request.</param>
        /// <param name="NotifyChargingLimitRequest2">Another notify charging limit request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyChargingLimitRequest? NotifyChargingLimitRequest1,
                                           NotifyChargingLimitRequest? NotifyChargingLimitRequest2)

            => !(NotifyChargingLimitRequest1 == NotifyChargingLimitRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyChargingLimitRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify charging limit requests for equality.
        /// </summary>
        /// <param name="Object">A notify charging limit request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyChargingLimitRequest notifyChargingLimitRequest &&
                   Equals(notifyChargingLimitRequest);

        #endregion

        #region Equals(NotifyChargingLimitRequest)

        /// <summary>
        /// Compares two notify charging limit requests for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitRequest">A notify charging limit request to compare with.</param>
        public override Boolean Equals(NotifyChargingLimitRequest? NotifyChargingLimitRequest)

            => NotifyChargingLimitRequest is not null &&

               ChargingLimit.Equals(NotifyChargingLimitRequest.ChargingLimit) &&

               ChargingSchedules.Count().Equals(NotifyChargingLimitRequest.ChargingSchedules.Count())     &&
               ChargingSchedules.All(data => NotifyChargingLimitRequest.ChargingSchedules.Contains(data)) &&

            ((!EVSEId.HasValue && !NotifyChargingLimitRequest.EVSEId.HasValue) ||
               EVSEId.HasValue &&  NotifyChargingLimitRequest.EVSEId.HasValue && EVSEId.Value.Equals(NotifyChargingLimitRequest.EVSEId.Value)) &&

               base.  GenericEquals(NotifyChargingLimitRequest);

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

            => $"'{ChargingLimit}'{(EVSEId.HasValue ? $" for EVSE Id {EVSEId}" : "")}";

        #endregion

    }

}
