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
using System.Threading.Channels;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A notify EV charging schedule request.
    /// </summary>
    public class NotifyEVChargingScheduleRequest : ARequest<NotifyEVChargingScheduleRequest>
    {

        #region Properties

        /// <summary>
        /// The charging periods contained within the charging schedule
        /// are relative to this time base.
        /// </summary>
        [Mandatory]
        public DateTime          TimeBase                    { get; }

        /// <summary>
        /// The charging schedule applies to this EVSE.
        /// </summary>
        [Mandatory]
        public EVSE_Id           EVSEId                      { get; }

        /// <summary>
        /// Planned energy consumption of the EV over time.
        /// Always relative to the time base.
        /// </summary>
        [Mandatory]
        public ChargingSchedule  ChargingSchedule            { get; }

        /// <summary>
        /// The optional identification of the selected charging schedule
        /// from the provided charging profile.
        /// </summary>
        [Optional]
        public Byte?             SelectedScheduleTupleId     { get; }

        /// <summary>
        /// True when power tolerance is accepted.
        /// This value is taken from EVPowerProfile.PowerToleranceAcceptance in the ISO 15118-20 PowerDeliverReq message..
        /// </summary>
        public Boolean?          PowerToleranceAcceptance    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify EV charging schedule request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
        /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
        /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
        /// <param name="SelectedScheduleTupleId">The optional identification of the selected charging schedule from the provided charging profile.</param>
        /// <param name="PowerToleranceAcceptance">True when power tolerance is accepted.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyEVChargingScheduleRequest(ChargeBox_Id       ChargeBoxId,
                                               DateTime           TimeBase,
                                               EVSE_Id            EVSEId,
                                               ChargingSchedule   ChargingSchedule,
                                               Byte?              SelectedScheduleTupleId    = null,
                                               Boolean?           PowerToleranceAcceptance   = null,
                                               CustomData?        CustomData                 = null,

                                               Request_Id?        RequestId                  = null,
                                               DateTime?          RequestTimestamp           = null,
                                               TimeSpan?          RequestTimeout             = null,
                                               EventTracking_Id?  EventTrackingId            = null,
                                               CancellationToken  CancellationToken          = default)

            : base(ChargeBoxId,
                   "NotifyEVChargingNeeds",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.TimeBase                  = TimeBase;
            this.EVSEId                    = EVSEId;
            this.ChargingSchedule          = ChargingSchedule;
            this.SelectedScheduleTupleId   = SelectedScheduleTupleId;
            this.PowerToleranceAcceptance  = PowerToleranceAcceptance;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyEVChargingScheduleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify EV charging schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyEVChargingScheduleRequestParser">A delegate to parse custom notify EV charging schedule requests.</param>
        public static NotifyEVChargingScheduleRequest Parse(JObject                                                        JSON,
                                                            Request_Id                                                     RequestId,
                                                            ChargeBox_Id                                                   ChargeBoxId,
                                                            CustomJObjectParserDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var notifyEVChargingNeedsRequest,
                         out var errorResponse,
                         CustomNotifyEVChargingScheduleRequestParser))
            {
                return notifyEVChargingNeedsRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify EV charging schedule request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyEVChargingScheduleRequest, out ErrorResponse, CustomNotifyEVChargingScheduleRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyEVChargingScheduleRequest">The parsed notify EV charging schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingScheduleRequestParser">A delegate to parse custom notify EV charging schedule requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       ChargeBox_Id                                                ChargeBoxId,
                                       out NotifyEVChargingScheduleRequest?                           NotifyEVChargingScheduleRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestParser)
        {

            try
            {

                NotifyEVChargingScheduleRequest = null;

                #region TimeBase                    [mandatory]

                if (!JSON.ParseMandatory("timeBase",
                                         "time base",
                                         out DateTime TimeBase,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId                      [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedule            [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingSchedule",
                                             "charging schedule",
                                             OCPPv2_1.ChargingSchedule.TryParse,
                                             out ChargingSchedule? ChargingSchedule,
                                             out ErrorResponse) ||
                     ChargingSchedule is null)
                {
                    return false;
                }

                #endregion

                #region SelectedScheduleTupleId     [optional]

                if (JSON.ParseOptional("maxScheduleTuples",
                                       "max schedule tuples",
                                       out Byte? SelectedScheduleTupleId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PowerToleranceAcceptance    [optional]

                if (JSON.ParseOptional("powerToleranceAcceptance",
                                       "power tolerance acceptance",
                                       out Boolean? PowerToleranceAcceptance,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                  [optional]

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

                #region ChargeBoxId                 [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                NotifyEVChargingScheduleRequest = new NotifyEVChargingScheduleRequest(
                                                      ChargeBoxId,
                                                      TimeBase,
                                                      EVSEId,
                                                      ChargingSchedule,
                                                      SelectedScheduleTupleId,
                                                      PowerToleranceAcceptance,
                                                      CustomData,
                                                      RequestId
                                                  );

                if (CustomNotifyEVChargingScheduleRequestParser is not null)
                    NotifyEVChargingScheduleRequest = CustomNotifyEVChargingScheduleRequestParser(JSON,
                                                                                                  NotifyEVChargingScheduleRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingScheduleRequest  = null;
                ErrorResponse                    = "The given JSON representation of a notify EV charging schedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingScheduleRequestSerializer = null, CustomChargingScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingScheduleRequestSerializer">A delegate to serialize custom NotifyEVChargingNeeds requests.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom salesTariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                 CustomChargingScheduleSerializer                  = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?           CustomChargingSchedulePeriodSerializer            = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                      CustomSalesTariffSerializer                       = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?                 CustomSalesTariffEntrySerializer                  = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?             CustomRelativeTimeIntervalSerializer              = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?                  CustomConsumptionCostSerializer                   = null,
                              CustomJObjectSerializerDelegate<Cost>?                             CustomCostSerializer                              = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timeBase",                   TimeBase.        ToIso8601()),
                                 new JProperty("evseId",                     EVSEId.          Value),

                                 new JProperty("chargingSchedule",           ChargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                                     CustomChargingSchedulePeriodSerializer,
                                                                                                     CustomSalesTariffSerializer,
                                                                                                     CustomSalesTariffEntrySerializer,
                                                                                                     CustomRelativeTimeIntervalSerializer,
                                                                                                     CustomConsumptionCostSerializer,
                                                                                                     CustomCostSerializer,
                                                                                                     CustomCustomDataSerializer)),

                           SelectedScheduleTupleId.HasValue
                               ? new JProperty("selectedScheduleTupleId",    SelectedScheduleTupleId.Value)
                               : null,

                           PowerToleranceAcceptance.HasValue
                               ? new JProperty("powerToleranceAcceptance",   PowerToleranceAcceptance.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                 CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyEVChargingScheduleRequestSerializer is not null
                       ? CustomNotifyEVChargingScheduleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingScheduleRequest1, NotifyEVChargingScheduleRequest2)

        /// <summary>
        /// Compares two notify EV charging schedule requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequest1">A notify EV charging schedule request.</param>
        /// <param name="NotifyEVChargingScheduleRequest2">Another notify EV charging schedule request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest1,
                                           NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingScheduleRequest1, NotifyEVChargingScheduleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingScheduleRequest1 is null || NotifyEVChargingScheduleRequest2 is null)
                return false;

            return NotifyEVChargingScheduleRequest1.Equals(NotifyEVChargingScheduleRequest2);

        }

        #endregion

        #region Operator != (NotifyEVChargingScheduleRequest1, NotifyEVChargingScheduleRequest2)

        /// <summary>
        /// Compares two notify EV charging schedule requests for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequest1">A notify EV charging schedule request.</param>
        /// <param name="NotifyEVChargingScheduleRequest2">Another notify EV charging schedule request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest1,
                                           NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest2)

            => !(NotifyEVChargingScheduleRequest1 == NotifyEVChargingScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingScheduleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify EV charging schedule requests for equality.
        /// </summary>
        /// <param name="Object">A notify EV charging schedule request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingScheduleRequest notifyEVChargingNeedsRequest &&
                   Equals(notifyEVChargingNeedsRequest);

        #endregion

        #region Equals(NotifyEVChargingScheduleRequest)

        /// <summary>
        /// Compares two notify EV charging schedule requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequest">A notify EV charging schedule request to compare with.</param>
        public override Boolean Equals(NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest)

            => NotifyEVChargingScheduleRequest is not null &&

               TimeBase.        Equals(NotifyEVChargingScheduleRequest.TimeBase)         &&
               EVSEId.          Equals(NotifyEVChargingScheduleRequest.EVSEId)           &&
               ChargingSchedule.Equals(NotifyEVChargingScheduleRequest.ChargingSchedule) &&

            ((!SelectedScheduleTupleId. HasValue && !NotifyEVChargingScheduleRequest.SelectedScheduleTupleId. HasValue) ||
               SelectedScheduleTupleId. HasValue &&  NotifyEVChargingScheduleRequest.SelectedScheduleTupleId. HasValue && SelectedScheduleTupleId. Value.Equals(NotifyEVChargingScheduleRequest.SelectedScheduleTupleId. Value)) &&

            ((!PowerToleranceAcceptance.HasValue && !NotifyEVChargingScheduleRequest.PowerToleranceAcceptance.HasValue) ||
               PowerToleranceAcceptance.HasValue &&  NotifyEVChargingScheduleRequest.PowerToleranceAcceptance.HasValue && PowerToleranceAcceptance.Value.Equals(NotifyEVChargingScheduleRequest.PowerToleranceAcceptance.Value)) &&

               base.GenericEquals(NotifyEVChargingScheduleRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return TimeBase.                 GetHashCode()       * 13 ^
                       EVSEId.                   GetHashCode()       * 11 ^
                       ChargingSchedule.         GetHashCode()       *  7 ^
                      (SelectedScheduleTupleId?. GetHashCode() ?? 0) *  5 ^
                      (PowerToleranceAcceptance?.GetHashCode() ?? 0) *  3 ^

                       base.                     GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{TimeBase}, EVSE Id: {EVSEId}",

                   SelectedScheduleTupleId. HasValue
                       ? ", selected schedule tuple id: " + SelectedScheduleTupleId.Value
                       : "",

                   PowerToleranceAcceptance.HasValue
                       ? ", power tolerance acceptance: " + (PowerToleranceAcceptance.Value ? "yes" : "no")
                       : "",

                   ", charging schedule" + ChargingSchedule.ToString()

               );

        #endregion

    }

}
