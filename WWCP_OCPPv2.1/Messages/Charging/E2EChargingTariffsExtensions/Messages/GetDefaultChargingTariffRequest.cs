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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetDefaultChargingTariff request.
    /// </summary>
    public class GetDefaultChargingTariffRequest : ARequest<GetDefaultChargingTariffRequest>,
                                                   IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getDefaultChargingTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional enumeration of EVSEs the default charging tariff should be reported on.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_Id>  EVSEIds    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetDefaultChargingTariff request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="EVSEIds">An optional enumeration of EVSEs the default charging tariff should be reported on.</param>
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
        public GetDefaultChargingTariffRequest(NetworkingNode_Id        DestinationId,
                                               IEnumerable<EVSE_Id>?    EVSEIds             = null,

                                               IEnumerable<KeyPair>?    SignKeys            = null,
                                               IEnumerable<SignInfo>?   SignInfos           = null,
                                               IEnumerable<Signature>?       Signatures          = null,

                                               CustomData?              CustomData          = null,

                                               Request_Id?              RequestId           = null,
                                               DateTime?                RequestTimestamp    = null,
                                               TimeSpan?                RequestTimeout      = null,
                                               EventTracking_Id?        EventTrackingId     = null,
                                               NetworkPath?             NetworkPath         = null,
                                               CancellationToken        CancellationToken   = default)

            : base(DestinationId,
                   nameof(GetDefaultChargingTariffRequest)[..^7],

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

            this.EVSEIds = EVSEIds?.Distinct() ?? Array.Empty<EVSE_Id>();

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

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomGetDefaultChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an setDefaultChargingTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDefaultChargingTariffRequestParser">A delegate to parse custom setDefaultChargingTariffs requests.</param>
        public static GetDefaultChargingTariffRequest Parse(JObject                                                        JSON,
                                                            Request_Id                                                     RequestId,
                                                            NetworkingNode_Id                                              DestinationId,
                                                            NetworkPath                                                    NetworkPath,
                                                            DateTime?                                                      RequestTimestamp                              = null,
                                                            TimeSpan?                                                      RequestTimeout                                = null,
                                                            EventTracking_Id?                                              EventTrackingId                               = null,
                                                            CustomJObjectParserDelegate<GetDefaultChargingTariffRequest>?  CustomGetDefaultChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var getDefaultChargingTariffRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetDefaultChargingTariffRequestParser))
            {
                return getDefaultChargingTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetDefaultChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out getDefaultChargingTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetDefaultChargingTariffRequest">The parsed setDefaultChargingTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDefaultChargingTariffRequestParser">A delegate to parse custom setDefaultChargingTariffs requests.</param>
        public static Boolean TryParse(JObject                                                        JSON,
                                       Request_Id                                                     RequestId,
                                       NetworkingNode_Id                                              DestinationId,
                                       NetworkPath                                                    NetworkPath,
                                       [NotNullWhen(true)]  out GetDefaultChargingTariffRequest?      GetDefaultChargingTariffRequest,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       DateTime?                                                      RequestTimestamp                              = null,
                                       TimeSpan?                                                      RequestTimeout                                = null,
                                       EventTracking_Id?                                              EventTrackingId                               = null,
                                       CustomJObjectParserDelegate<GetDefaultChargingTariffRequest>?  CustomGetDefaultChargingTariffRequestParser   = null)
        {

            try
            {

                GetDefaultChargingTariffRequest = null;

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetDefaultChargingTariffRequest = new GetDefaultChargingTariffRequest(

                                                      DestinationId,
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

                if (CustomGetDefaultChargingTariffRequestParser is not null)
                    GetDefaultChargingTariffRequest = CustomGetDefaultChargingTariffRequestParser(JSON,
                                                                                                  GetDefaultChargingTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                GetDefaultChargingTariffRequest  = null;
                ErrorResponse                    = "The given JSON representation of a GetDefaultChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDefaultChargingTariffRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDefaultChargingTariffRequestSerializer">A delegate to serialize custom setDefaultChargingTariffs requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDefaultChargingTariffRequest>?  CustomGetDefaultChargingTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                           EVSEIds.Any()
                               ? new JProperty("evseIds",      new JArray(EVSEIds.   Select(evseId    => evseId.   ToString())))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetDefaultChargingTariffRequestSerializer is not null
                       ? CustomGetDefaultChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetDefaultChargingTariffRequest1, GetDefaultChargingTariffRequest2)

        /// <summary>
        /// Compares two GetDefaultChargingTariff requests for equality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffRequest1">A GetDefaultChargingTariff request.</param>
        /// <param name="GetDefaultChargingTariffRequest2">Another setDefaultChargingTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDefaultChargingTariffRequest? GetDefaultChargingTariffRequest1,
                                           GetDefaultChargingTariffRequest? GetDefaultChargingTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDefaultChargingTariffRequest1, GetDefaultChargingTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetDefaultChargingTariffRequest1 is null || GetDefaultChargingTariffRequest2 is null)
                return false;

            return GetDefaultChargingTariffRequest1.Equals(GetDefaultChargingTariffRequest2);

        }

        #endregion

        #region Operator != (GetDefaultChargingTariffRequest1, GetDefaultChargingTariffRequest2)

        /// <summary>
        /// Compares two GetDefaultChargingTariff requests for inequality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffRequest1">A GetDefaultChargingTariff request.</param>
        /// <param name="GetDefaultChargingTariffRequest2">Another setDefaultChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDefaultChargingTariffRequest? GetDefaultChargingTariffRequest1,
                                           GetDefaultChargingTariffRequest? GetDefaultChargingTariffRequest2)

            => !(GetDefaultChargingTariffRequest1 == GetDefaultChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<GetDefaultChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetDefaultChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A GetDefaultChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDefaultChargingTariffRequest getDefaultChargingTariffRequest &&
                   Equals(getDefaultChargingTariffRequest);

        #endregion

        #region Equals(GetDefaultChargingTariffRequest)

        /// <summary>
        /// Compares two GetDefaultChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="GetDefaultChargingTariffRequest">A GetDefaultChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(GetDefaultChargingTariffRequest? GetDefaultChargingTariffRequest)

            => GetDefaultChargingTariffRequest is not null &&

               EVSEIds.Count().Equals(GetDefaultChargingTariffRequest.EVSEIds.Count())         &&
               EVSEIds.All(evseId => GetDefaultChargingTariffRequest.EVSEIds.Contains(evseId)) &&

               base.   GenericEquals(GetDefaultChargingTariffRequest);

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

            => $"Get default charging tariff{(EVSEIds.Any() ? $" on EVSEs: {EVSEIds.AggregateWith(", ")}!" : "")}";

        #endregion

    }

}
