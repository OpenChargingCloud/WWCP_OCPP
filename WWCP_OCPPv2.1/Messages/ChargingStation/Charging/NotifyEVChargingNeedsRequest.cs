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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A notify EV charging needs request.
    /// </summary>
    public class NotifyEVChargingNeedsRequest : ARequest<NotifyEVChargingNeedsRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/notifyEVChargingNeedsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE and connector to which the EV is connected to.
        /// </summary>
        [Mandatory]
        public EVSE_Id        EVSEId               { get; }

        /// <summary>
        /// The characteristics of the energy delivery required.
        /// </summary>
        [Mandatory]
        public ChargingNeeds  ChargingNeeds        { get; }

        /// <summary>
        /// An optional timestamp when the EV charging needs had been received,
        /// e.g. when the charging station was offline.
        /// </summary>
        [Optional]
        public DateTime?      ReceivedTimestamp    { get; }

        /// <summary>
        /// The optional maximum number of schedule tuples per schedule the car supports.
        /// </summary>
        [Optional]
        public UInt16?        MaxScheduleTuples    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify EV charging needs request.
        /// </summary>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="ReceivedTimestamp">An optional timestamp when the EV charging needs had been received, e.g. when the charging station was offline.</param>
        /// <param name="MaxScheduleTuples">An optional maximum number of schedule tuples per schedule the car supports.</param>
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
        public NotifyEVChargingNeedsRequest(NetworkingNode_Id        NetworkingNodeId,
                                            EVSE_Id                  EVSEId,
                                            ChargingNeeds            ChargingNeeds,
                                            DateTime?                ReceivedTimestamp   = null,
                                            UInt16?                  MaxScheduleTuples   = null,

                                            IEnumerable<KeyPair>?    SignKeys            = null,
                                            IEnumerable<SignInfo>?   SignInfos           = null,
                                            IEnumerable<Signature>?  Signatures          = null,

                                            CustomData?              CustomData          = null,

                                            Request_Id?              RequestId           = null,
                                            DateTime?                RequestTimestamp    = null,
                                            TimeSpan?                RequestTimeout      = null,
                                            EventTracking_Id?        EventTrackingId     = null,
                                            NetworkPath?             NetworkPath         = null,
                                            CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(NotifyEVChargingNeedsRequest)[..^7],

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

            this.EVSEId             = EVSEId;
            this.ChargingNeeds      = ChargingNeeds;
            this.ReceivedTimestamp  = ReceivedTimestamp;
            this.MaxScheduleTuples  = MaxScheduleTuples;


            unchecked
            {

                hashCode = this.EVSEId.            GetHashCode()       * 11 ^
                           this.ChargingNeeds.     GetHashCode()       *  7 ^
                          (this.ReceivedTimestamp?.GetHashCode() ?? 0) *  5 ^
                          (this.MaxScheduleTuples?.GetHashCode() ?? 0) *  3 ^
                           base.                   GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomNotifyEVChargingNeedsRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify EV charging needs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomNotifyEVChargingNeedsRequestParser">A delegate to parse custom notify EV charging needs requests.</param>
        public static NotifyEVChargingNeedsRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         NetworkingNode_Id                                           NetworkingNodeId,
                                                         NetworkPath                                                 NetworkPath,
                                                         CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var notifyEVChargingNeedsRequest,
                         out var errorResponse,
                         CustomNotifyEVChargingNeedsRequestParser) &&
                notifyEVChargingNeedsRequest is not null)
            {
                return notifyEVChargingNeedsRequest;
            }

            throw new ArgumentException("The given JSON representation of a notify EV charging needs request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out NotifyEVChargingNeedsRequest, out ErrorResponse, CustomNotifyEVChargingNeedsRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging needs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyEVChargingNeedsRequest">The parsed notify EV charging needs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       Request_Id                         RequestId,
                                       NetworkingNode_Id                  NetworkingNodeId,
                                       NetworkPath                        NetworkPath,
                                       out NotifyEVChargingNeedsRequest?  NotifyEVChargingNeedsRequest,
                                       out String?                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out NotifyEVChargingNeedsRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging needs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyEVChargingNeedsRequest">The parsed notify EV charging needs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingNeedsRequestParser">A delegate to parse custom notify EV charging needs requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       NetworkingNode_Id                                           NetworkingNodeId,
                                       NetworkPath                                                 NetworkPath,
                                       out NotifyEVChargingNeedsRequest?                           NotifyEVChargingNeedsRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestParser)
        {

            try
            {

                NotifyEVChargingNeedsRequest = null;

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
                                             "charging needs",
                                             OCPPv2_1.ChargingNeeds.TryParse,
                                             out ChargingNeeds? ChargingNeeds,
                                             out ErrorResponse) ||
                     ChargingNeeds is null)
                {
                    return false;
                }

                #endregion

                #region ReceivedTimestamp    [optional]

                if (JSON.ParseOptional("timestamp",
                                       "received timestamp",
                                       out DateTime? ReceivedTimestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                NotifyEVChargingNeedsRequest = new NotifyEVChargingNeedsRequest(

                                                   NetworkingNodeId,
                                                   EVSEId,
                                                   ChargingNeeds,
                                                   ReceivedTimestamp,
                                                   MaxScheduleTuples,

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

                if (CustomNotifyEVChargingNeedsRequestParser is not null)
                    NotifyEVChargingNeedsRequest = CustomNotifyEVChargingNeedsRequestParser(JSON,
                                                                                            NotifyEVChargingNeedsRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingNeedsRequest  = null;
                ErrorResponse                 = "The given JSON representation of a notify EV charging needs request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingNeedsRequestSerializer = null, CustomChargingNeedsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingNeedsRequestSerializer">A delegate to serialize custom NotifyEVChargingNeeds requests.</param>
        /// <param name="CustomChargingNeedsSerializer">A delegate to serialize custom charging needs.</param>
        /// <param name="CustomACChargingParametersSerializer">A delegate to serialize custom AC charging parameters.</param>
        /// <param name="CustomDCChargingParametersSerializer">A delegate to serialize custom DC charging parameters.</param>
        /// <param name="CustomV2XChargingParametersSerializer">A delegate to serialize custom V2X charging parameters.</param>
        /// <param name="CustomEVEnergyOfferSerializer">A delegate to serialize custom ev energy offers.</param>
        /// <param name="CustomEVPowerScheduleSerializer">A delegate to serialize custom ev power schedules.</param>
        /// <param name="CustomEVPowerScheduleEntrySerializer">A delegate to serialize custom ev power schedule entries.</param>
        /// <param name="CustomEVAbsolutePriceScheduleSerializer">A delegate to serialize custom ev absolute price schedules.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntrySerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom ev price rules.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestSerializer   = null,
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
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
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

                           ReceivedTimestamp.HasValue
                               ? new JProperty("timestamp",           ReceivedTimestamp.Value.ToIso8601())
                               : null,

                           MaxScheduleTuples.HasValue
                               ? new JProperty("maxScheduleTuples",   MaxScheduleTuples.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyEVChargingNeedsRequestSerializer is not null
                       ? CustomNotifyEVChargingNeedsRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest1">A notify EV charging needs request.</param>
        /// <param name="NotifyEVChargingNeedsRequest2">Another notify EV charging needs request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest1,
                                           NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingNeedsRequest1 is null || NotifyEVChargingNeedsRequest2 is null)
                return false;

            return NotifyEVChargingNeedsRequest1.Equals(NotifyEVChargingNeedsRequest2);

        }

        #endregion

        #region Operator != (NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2)

        /// <summary>
        /// Compares two notify EV charging needs requests for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest1">A notify EV charging needs request.</param>
        /// <param name="NotifyEVChargingNeedsRequest2">Another notify EV charging needs request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest1,
                                           NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest2)

            => !(NotifyEVChargingNeedsRequest1 == NotifyEVChargingNeedsRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingNeedsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="Object">A notify EV charging needs request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingNeedsRequest notifyEVChargingNeedsRequest &&
                   Equals(notifyEVChargingNeedsRequest);

        #endregion

        #region Equals(NotifyEVChargingNeedsRequest)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest">A notify EV charging needs request to compare with.</param>
        public override Boolean Equals(NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest)

            => NotifyEVChargingNeedsRequest is not null &&

               EVSEId.       Equals(NotifyEVChargingNeedsRequest.EVSEId)        &&
               ChargingNeeds.Equals(NotifyEVChargingNeedsRequest.ChargingNeeds) &&

            ((!ReceivedTimestamp.HasValue && !NotifyEVChargingNeedsRequest.ReceivedTimestamp.HasValue) ||
               ReceivedTimestamp.HasValue &&  NotifyEVChargingNeedsRequest.ReceivedTimestamp.HasValue && ReceivedTimestamp.Value.Equals(NotifyEVChargingNeedsRequest.ReceivedTimestamp.Value)) &&

            ((!MaxScheduleTuples.HasValue && !NotifyEVChargingNeedsRequest.MaxScheduleTuples.HasValue) ||
               MaxScheduleTuples.HasValue &&  NotifyEVChargingNeedsRequest.MaxScheduleTuples.HasValue && MaxScheduleTuples.Value.Equals(NotifyEVChargingNeedsRequest.MaxScheduleTuples.Value)) &&

               base.GenericEquals(NotifyEVChargingNeedsRequest);

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

            => $"EVSE Id: {EVSEId}: {ChargingNeeds}{(ReceivedTimestamp.HasValue ? ", received: " + ReceivedTimestamp : "")}{(MaxScheduleTuples.HasValue ? ", max schedule tuples: " + MaxScheduleTuples : "")}";

        #endregion

    }

}
