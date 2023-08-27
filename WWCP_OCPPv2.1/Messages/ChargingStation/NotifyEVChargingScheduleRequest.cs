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
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="MaxScheduleTuples">The optional maximum schedule tuples the car supports per schedule.</param>
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
                                               CustomData?        CustomData          = null,

                                               Request_Id?        RequestId           = null,
                                               DateTime?          RequestTimestamp    = null,
                                               TimeSpan?          RequestTimeout      = null,
                                               EventTracking_Id?  EventTrackingId     = null,
                                               CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "NotifyEVChargingNeeds",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.EVSEId             = EVSEId;
            this.ChargingNeeds      = ChargingNeeds;
            this.MaxScheduleTuples  = MaxScheduleTuples;

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
        public static NotifyEVChargingScheduleRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         ChargeBox_Id                                                ChargeBoxId,
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

                #region EVSEId               [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingNeeds        [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingNeeds",
                                             "charging schedule",
                                             OCPPv2_1.ChargingNeeds.TryParse,
                                             out ChargingNeeds? ChargingNeeds,
                                             out ErrorResponse) ||
                     ChargingNeeds is null)
                {
                    return false;
                }

                #endregion

                #region MaxScheduleTuples    [optional]

                if (JSON.ParseOptional("maxScheduleTuples",
                                       "max schedule tuples",
                                       out UInt16? MaxScheduleTuples,
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

                #region ChargeBoxId          [optional, OCPP_CSE]

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
                                                   EVSEId,
                                                   ChargingNeeds,
                                                   MaxScheduleTuples,
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
                ErrorResponse                 = "The given JSON representation of a notify EV charging schedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingScheduleRequestSerializer = null, CustomChargingNeedsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingScheduleRequestSerializer">A delegate to serialize custom NotifyEVChargingNeeds requests.</param>
        /// <param name="CustomChargingNeedsSerializer">A delegate to serialize custom charging schedule.</param>
        /// <param name="CustomACChargingParametersSerializer">A delegate to serialize custom AC charging parameters.</param>
        /// <param name="CustomDCChargingParametersSerializer">A delegate to serialize custom DC charging parameters.</param>
        /// <param name="CustomV2XChargingParametersSerializer">A delegate to serialize custom V2X charging parameters.</param>
        /// <param name="CustomEVEnergyOfferSerializer">A delegate to serialize custom ev energy offers.</param>
        /// <param name="CustomEVPowerScheduleSerializer">A delegate to serialize custom ev power schedules.</param>
        /// <param name="CustomEVPowerScheduleEntrySerializer">A delegate to serialize custom ev power schedule entries.</param>
        /// <param name="CustomEVAbsolutePriceScheduleSerializer">A delegate to serialize custom ev absolute price schedules.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntrySerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom ev price rules.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingNeeds>?                 CustomChargingNeedsSerializer                  = null,
                              CustomJObjectSerializerDelegate<ACChargingParameters>?          CustomACChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<DCChargingParameters>?          CustomDCChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<V2XChargingParameters>?         CustomV2XChargingParametersSerializer          = null,
                              CustomJObjectSerializerDelegate<EVEnergyOffer>?                 CustomEVEnergyOfferSerializer                  = null,
                              CustomJObjectSerializerDelegate<EVPowerSchedule>?               CustomEVPowerScheduleSerializer                = null,
                              CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?          CustomEVPowerScheduleEntrySerializer           = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?       CustomEVAbsolutePriceScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<EVPriceRule>?                   CustomEVPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",              EVSEId.       Value),

                                 new JProperty("chargingNeeds",       ChargingNeeds.ToJSON(CustomChargingNeedsSerializer,
                                                                                           CustomACChargingParametersSerializer,
                                                                                           CustomDCChargingParametersSerializer,
                                                                                           CustomV2XChargingParametersSerializer,
                                                                                           CustomEVEnergyOfferSerializer,
                                                                                           CustomEVPowerScheduleSerializer,
                                                                                           CustomEVPowerScheduleEntrySerializer,
                                                                                           CustomEVAbsolutePriceScheduleSerializer,
                                                                                           CustomEVAbsolutePriceScheduleEntrySerializer,
                                                                                           CustomEVPriceRuleSerializer,
                                                                                           CustomCustomDataSerializer)),

                           MaxScheduleTuples.HasValue
                               ? new JProperty("maxScheduleTuples",   MaxScheduleTuples.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.   ToJSON(CustomCustomDataSerializer))
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

               EVSEId.       Equals(NotifyEVChargingScheduleRequest.EVSEId)        &&
               ChargingNeeds.Equals(NotifyEVChargingScheduleRequest.ChargingNeeds) &&

            ((!MaxScheduleTuples.HasValue && !NotifyEVChargingScheduleRequest.MaxScheduleTuples.HasValue) ||
               MaxScheduleTuples.HasValue &&  NotifyEVChargingScheduleRequest.MaxScheduleTuples.HasValue && MaxScheduleTuples.Value.Equals(NotifyEVChargingScheduleRequest.MaxScheduleTuples.Value)) &&

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

                return EVSEId.            GetHashCode()       * 7 ^
                       ChargingNeeds.     GetHashCode()       * 5 ^
                      (MaxScheduleTuples?.GetHashCode() ?? 0) * 3 ^

                       base.              GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"EVSE Id: {EVSEId}: {ChargingNeeds} {(MaxScheduleTuples.HasValue ? ", max schedule tuples: " + MaxScheduleTuples : "")}";

        #endregion

    }

}
