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

    /// <summary>
    /// The RemoveDefaultChargingTariff request.
    /// </summary>
    public class RemoveDefaultChargingTariffRequest : ARequest<RemoveDefaultChargingTariffRequest>,
                                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/removeDefaultChargingTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional unique charging tariff identification of the default charging tariff to be removed.
        /// </summary>
        [Optional]
        public Tariff_Id?    ChargingTariffId    { get; }

        /// <summary>
        /// The optional enumeration of EVSEs the default charging tariff should be removed from.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_Id>  EVSEIds             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RemoveDefaultChargingTariff request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ChargingTariffId">The optional unique charging tariff identification of the default charging tariff to be removed.</param>
        /// <param name="EVSEIds">An optional enumeration of EVSEs the default charging tariff should be removed from.</param>
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
        public RemoveDefaultChargingTariffRequest(SourceRouting            Destination,
                                                  Tariff_Id?       ChargingTariffId      = null,
                                                  IEnumerable<EVSE_Id>?    EVSEIds               = null,

                                                  IEnumerable<KeyPair>?    SignKeys              = null,
                                                  IEnumerable<SignInfo>?   SignInfos             = null,
                                                  IEnumerable<Signature>?  Signatures            = null,

                                                  CustomData?              CustomData            = null,

                                                  Request_Id?              RequestId             = null,
                                                  DateTimeOffset?          RequestTimestamp      = null,
                                                  TimeSpan?                RequestTimeout        = null,
                                                  EventTracking_Id?        EventTrackingId       = null,
                                                  NetworkPath?             NetworkPath           = null,
                                                  SerializationFormats?    SerializationFormat   = null,
                                                  CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(RemoveDefaultChargingTariffRequest)[..^7],

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

            this.ChargingTariffId  = ChargingTariffId;
            this.EVSEIds           = EVSEIds?.Distinct() ?? Array.Empty<EVSE_Id>();

            unchecked
            {
                hashCode = (this.ChargingTariffId?.GetHashCode() ?? 0) * 5 ^
                            this.EVSEIds.          CalcHashCode()      * 3 ^
                            base.                  GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomRemoveDefaultChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an removeDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRemoveDefaultChargingTariffRequestParser">A delegate to parse custom removeDefaultChargingTariff requests.</param>
        public static RemoveDefaultChargingTariffRequest Parse(JObject                                                           JSON,
                                                               Request_Id                                                        RequestId,
                                                               SourceRouting                                                 Destination,
                                                               NetworkPath                                                       NetworkPath,
                                                               DateTimeOffset?                                                   RequestTimestamp                                 = null,
                                                               TimeSpan?                                                         RequestTimeout                                   = null,
                                                               EventTracking_Id?                                                 EventTrackingId                                  = null,
                                                               CustomJObjectParserDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var removeDefaultChargingTariffRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomRemoveDefaultChargingTariffRequestParser))
            {
                return removeDefaultChargingTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a RemoveDefaultChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out removeDefaultChargingTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a RemoveDefaultChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RemoveDefaultChargingTariffRequest">The parsed removeDefaultChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoveDefaultChargingTariffRequestParser">A delegate to parse custom removeDefaultChargingTariff requests.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       Request_Id                                                        RequestId,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                       NetworkPath,
                                       [NotNullWhen(true)]  out RemoveDefaultChargingTariffRequest?      RemoveDefaultChargingTariffRequest,
                                       [NotNullWhen(false)] out String?                                  ErrorResponse,
                                       DateTimeOffset?                                                   RequestTimestamp                                 = null,
                                       TimeSpan?                                                         RequestTimeout                                   = null,
                                       EventTracking_Id?                                                 EventTrackingId                                  = null,
                                       CustomJObjectParserDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestParser   = null)
        {

            try
            {

                RemoveDefaultChargingTariffRequest = null;

                #region ChargingTariffId     [optional]

                if (JSON.ParseOptional("chargingTariffId",
                                       "charging tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? ChargingTariffId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                RemoveDefaultChargingTariffRequest = new RemoveDefaultChargingTariffRequest(

                                                         Destination,
                                                         ChargingTariffId,
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

                           ChargingTariffId.HasValue
                               ? new JProperty("chargingTariffId",   ChargingTariffId.Value.ToString())
                               : null,

                           EVSEIds.Any()
                               ? new JProperty("evseIds",            new JArray(EVSEIds.   Select(evseId    => evseId.   Value)))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.            ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRemoveDefaultChargingTariffRequestSerializer is not null
                       ? CustomRemoveDefaultChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out RemoveDefaultChargingTariffRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a RemoveDefaultChargingTariff request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="RemoveDefaultChargingTariffRequest">The RemoveDefaultChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomRemoveDefaultChargingTariffRequestParser">A delegate to read custom RemoveDefaultChargingTariff requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                                      CBOR,
                                           Request_Id                                                     RequestId,
                                           SourceRouting                                                  Destination,
                                           NetworkPath                                                    NetworkPath,
                                           [NotNullWhen(true)]  out RemoveDefaultChargingTariffRequest?   RemoveDefaultChargingTariffRequest,
                                           [NotNullWhen(false)] out String?                               ErrorResponse,
                                           DateTimeOffset?                                                RequestTimestamp                                 = null,
                                           TimeSpan?                                                      RequestTimeout                                   = null,
                                           EventTracking_Id?                                              EventTrackingId                                  = null,
                                           CustomCBORParserDelegate<RemoveDefaultChargingTariffRequest>?  CustomRemoveDefaultChargingTariffRequestParser   = null)
        {

            try
            {

                RemoveDefaultChargingTariffRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a RemoveDefaultChargingTariff request is not a map!";
                    return false;
                }

                Tariff_Id? ChargingTariffId = null;

                if (CBOR.ParseOptionalText("chargingTariffId",
                                           "charging tariff identification",
                                           out var ChargingTariffIdText,
                                           out ErrorResponse))
                {

                    if (!global::cloud.charging.open.protocols.OCPPv2_1.Tariff_Id.TryParse(ChargingTariffIdText!, out var ChargingTariffIdValue))
                    {
                        ErrorResponse = $"Invalid charging tariff identification '{ChargingTariffIdText}'!";
                        return false;
                    }

                    ChargingTariffId = ChargingTariffIdValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<EVSE_Id>("evseIds",
                                               "EVSE identifications",
                                               (CBORValue item, out EVSE_Id value, out String? errorResponse) => {
                                                   value         = default;
                                                   errorResponse = item.Kind == CBORValueKind.UnsignedInteger && item.AsUInt64() <= UInt16.MaxValue && global::cloud.charging.open.protocols.OCPPv2_1.EVSE_Id.TryParse((UInt16) item.AsUInt64(), out value)
                                                                       ? null
                                                                       : $"Invalid EVSE identification '{item}'!";
                                                   return errorResponse is null;
                                               },
                                               out var EVSEIds,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                RemoveDefaultChargingTariffRequest = new RemoveDefaultChargingTariffRequest(

                                                         Destination,
                                                         ChargingTariffId,
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

                if (CustomRemoveDefaultChargingTariffRequestParser is not null)
                    RemoveDefaultChargingTariffRequest = CustomRemoveDefaultChargingTariffRequestParser(CBOR,
                                                                                                       RemoveDefaultChargingTariffRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                RemoveDefaultChargingTariffRequest = null;
                ErrorResponse = "The given CBOR representation of a RemoveDefaultChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomRemoveDefaultChargingTariffRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this RemoveDefaultChargingTariff request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomRemoveDefaultChargingTariffRequestSerializer">A delegate to serialize custom RemoveDefaultChargingTariff requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<RemoveDefaultChargingTariffRequest>? CustomRemoveDefaultChargingTariffRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("chargingTariffId",          OCPPCBORExtensions.Text(ChargingTariffId?.ToString())),
                           ("evseIds",                   OCPPCBORExtensions.Array(EVSEIds, x => CBORValue.FromUInt64(x.Value))),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomRemoveDefaultChargingTariffRequestSerializer is not null
                       ? CustomRemoveDefaultChargingTariffRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoveDefaultChargingTariffRequest1, RemoveDefaultChargingTariffRequest2)

        /// <summary>
        /// Compares two RemoveDefaultChargingTariff requests for equality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffRequest1">A RemoveDefaultChargingTariff request.</param>
        /// <param name="RemoveDefaultChargingTariffRequest2">Another RemoveDefaultChargingTariff request.</param>
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
        /// Compares two RemoveDefaultChargingTariff requests for inequality.
        /// </summary>
        /// <param name="RemoveDefaultChargingTariffRequest1">A RemoveDefaultChargingTariff request.</param>
        /// <param name="RemoveDefaultChargingTariffRequest2">Another RemoveDefaultChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoveDefaultChargingTariffRequest? RemoveDefaultChargingTariffRequest1,
                                           RemoveDefaultChargingTariffRequest? RemoveDefaultChargingTariffRequest2)

            => !(RemoveDefaultChargingTariffRequest1 == RemoveDefaultChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<RemoveDefaultChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RemoveDefaultChargingTariff requests for equality.
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

             ((ChargingTariffId is     null && RemoveDefaultChargingTariffRequest.ChargingTariffId is     null) ||
              (ChargingTariffId is not null && RemoveDefaultChargingTariffRequest.ChargingTariffId is not null && ChargingTariffId.Value.Equals(RemoveDefaultChargingTariffRequest.ChargingTariffId.Value))) &&

               EVSEIds.Count().Equals(RemoveDefaultChargingTariffRequest.EVSEIds.Count())     &&
               EVSEIds.All(evseId => RemoveDefaultChargingTariffRequest.EVSEIds.Contains(evseId)) &&

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

            => $"Remove default charging tariff{(EVSEIds.Any() ? $" on EVSEs: {EVSEIds.AggregateWith(", ")}!" : "")}";

        #endregion

    }

}
