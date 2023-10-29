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
    /// An remove default charging tariff request.
    /// </summary>
    public class RemoveDefaultChargingTariffRequest : ARequest<RemoveDefaultChargingTariffRequest>,
                                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/removeDefaultChargingTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of EVSEs the charging tariff applies to.
        /// </summary>
        [Mandatory]
        public IEnumerable<EVSE_Id>  EVSEIds    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remove default charging tariff request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="EVSEIds">The enumeration of EVSEs the charging tariff applies to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public RemoveDefaultChargingTariffRequest(ChargingStation_Id       ChargingStationId,
                                                  IEnumerable<EVSE_Id>     EVSEIds,

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
                   "RemoveDefaultChargingTariff",

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

            this.EVSEIds = EVSEIds.Distinct();

            unchecked
            {

                hashCode = this.EVSEIds.CalcHashCode() * 3 ^
                           base.        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomRemoveDefaultChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an removeDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomRemoveDefaultChargingTariffRequestParser">A delegate to parse custom removeDefaultChargingTariff requests.</param>
        public static RemoveDefaultChargingTariffRequest Parse(JObject                                                           JSON,
                                                               Request_Id                                                        RequestId,
                                                               ChargingStation_Id                                                ChargingStationId,
                                                               CustomJObjectParserDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var removeDefaultChargingTariffRequest,
                         out var errorResponse,
                         CustomRemoveDefaultChargingTariffRequestParser))
            {
                return removeDefaultChargingTariffRequest!;
            }

            throw new ArgumentException("The given JSON representation of a RemoveDefaultChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out removeDefaultChargingTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a RemoveDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="removeDefaultChargingTariffRequest">The parsed removeDefaultChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       Request_Id                               RequestId,
                                       ChargingStation_Id                       ChargingStationId,
                                       out RemoveDefaultChargingTariffRequest?  removeDefaultChargingTariffRequest,
                                       out String?                              ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out removeDefaultChargingTariffRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a RemoveDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="RemoveDefaultChargingTariffRequest">The parsed removeDefaultChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoveDefaultChargingTariffRequestParser">A delegate to parse custom removeDefaultChargingTariff requests.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       Request_Id                                                        RequestId,
                                       ChargingStation_Id                                                ChargingStationId,
                                       out RemoveDefaultChargingTariffRequest?                           RemoveDefaultChargingTariffRequest,
                                       out String?                                                       ErrorResponse,
                                       CustomJObjectParserDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestParser)
        {

            try
            {

                RemoveDefaultChargingTariffRequest = null;

                #region EVSEIds              [mandatory]

                if (!JSON.ParseMandatoryHashSet("evseIds",
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


                RemoveDefaultChargingTariffRequest = new RemoveDefaultChargingTariffRequest(
                                                         ChargingStationId,
                                                         EVSEIds,
                                                         null,
                                                         null,
                                                         Signatures,
                                                         CustomData,
                                                         RequestId
                                                     );

                if (CustomRemoveDefaultChargingTariffRequestParser is not null)
                    RemoveDefaultChargingTariffRequest = CustomRemoveDefaultChargingTariffRequestParser(JSON,
                                                                                                        RemoveDefaultChargingTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                RemoveDefaultChargingTariffRequest  = null;
                ErrorResponse                       = "The given JSON representation of a RemoveDefaultChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRemoveDefaultChargingTariffRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoveDefaultChargingTariffRequestSerializer">A delegate to serialize custom removeDefaultChargingTariff requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                           CustomSignatureSerializer                            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",       new JArray(EVSEIds.   Select(evseId    => evseId.   ToString()))),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.    ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRemoveDefaultChargingTariffRequestSerializer is not null
                       ? CustomRemoveDefaultChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoveDefaultChargingTariffRequest1, RemoveDefaultChargingTariffRequest2)

        /// <summary>
        /// Compares two removeDefaultChargingTariff requests for equality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffRequest1">A removeDefaultChargingTariff request.</param>
        /// <param name="RemoveDefaultChargingTariffRequest2">Another removeDefaultChargingTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoveDefaultChargingTariffRequest? RemoveDefaultChargingTariffRequest1,
                                           RemoveDefaultChargingTariffRequest? RemoveDefaultChargingTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoveDefaultChargingTariffRequest1, RemoveDefaultChargingTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (RemoveDefaultChargingTariffRequest1 is null || RemoveDefaultChargingTariffRequest2 is null)
                return false;

            return RemoveDefaultChargingTariffRequest1.Equals(RemoveDefaultChargingTariffRequest2);

        }

        #endregion

        #region Operator != (RemoveDefaultChargingTariffRequest1, RemoveDefaultChargingTariffRequest2)

        /// <summary>
        /// Compares two removeDefaultChargingTariff requests for inequality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffRequest1">A removeDefaultChargingTariff request.</param>
        /// <param name="RemoveDefaultChargingTariffRequest2">Another removeDefaultChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoveDefaultChargingTariffRequest? RemoveDefaultChargingTariffRequest1,
                                           RemoveDefaultChargingTariffRequest? RemoveDefaultChargingTariffRequest2)

            => !(RemoveDefaultChargingTariffRequest1 == RemoveDefaultChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<RemoveDefaultChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RemoveDefaultChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A RemoveDefaultChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoveDefaultChargingTariffRequest removeDefaultChargingTariffRequest &&
                   Equals(removeDefaultChargingTariffRequest);

        #endregion

        #region Equals(RemoveDefaultChargingTariffRequest)

        /// <summary>
        /// Compares two RemoveDefaultChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffRequest">A RemoveDefaultChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(RemoveDefaultChargingTariffRequest? RemoveDefaultChargingTariffRequest)

            => RemoveDefaultChargingTariffRequest is not null &&

               EVSEIds.Count().Equals(RemoveDefaultChargingTariffRequest.EVSEIds.Count())     &&
               EVSEIds.All(cost => RemoveDefaultChargingTariffRequest.EVSEIds.Contains(cost)) &&

               base.  GenericEquals(RemoveDefaultChargingTariffRequest);

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

            => $"Remove default charging tariff on EVSEs: {EVSEIds.AggregateWith(", ")}!";

        #endregion

    }

}
