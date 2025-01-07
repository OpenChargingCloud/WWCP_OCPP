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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The ClearTariffs request.
    /// </summary>
    public class ClearTariffsRequest : ARequest<ClearTariffsRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/clearTariffsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional enumeration of tariff identifications to be cleared.
        /// When empty, clear all tariffs.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff_Id>  TariffIds     { get; }

        /// <summary>
        /// When present only clear tariffs of this kind.
        /// </summary>
        [Optional]
        public TariffKinds?            TariffKind    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearTariffs request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="TariffIds">An optional enumeration of tariff identifications to be cleared. When empty, clear all tariffs.</param>
        /// <param name="TariffKind">When present only clear tariffs of this kind.</param>
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
        public ClearTariffsRequest(SourceRouting            Destination,
                                   IEnumerable<Tariff_Id>?  TariffIds             = null,
                                   TariffKinds?             TariffKind            = null,

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
                   nameof(ClearTariffsRequest)[..^7],

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

            this.TariffIds   = TariffIds?.Distinct() ?? [];
            this.TariffKind  = TariffKind;

            unchecked
            {
                hashCode = this.TariffIds.  CalcHashCode()      * 5 ^
                          (this.TariffKind?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomClearTariffsRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearTariffsRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static ClearTariffsRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<ClearTariffsRequest>?  CustomClearTariffsRequestParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearTariffsRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomClearTariffsRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearTariffsRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearTariffs request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out ClearTariffsRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearTariffsRequest">The parsed setTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearTariffsRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out ClearTariffsRequest?      ClearTariffsRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp                  = null,
                                       TimeSpan?                                          RequestTimeout                    = null,
                                       EventTracking_Id?                                  EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<ClearTariffsRequest>?  CustomClearTariffsRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                ClearTariffsRequest = null;

                #region TariffIds     [optional]

                if (JSON.ParseOptionalHashSet("tariffIds",
                                              "tariff identifications",
                                              Tariff_Id.TryParse,
                                              out HashSet<Tariff_Id> TariffIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TariffKind    [optional]

                if (JSON.ParseOptional("tariffKind",
                                       "tariff kind",
                                       TariffKindsExtensions.TryParse,
                                       out TariffKinds? TariffKind,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                ClearTariffsRequest = new ClearTariffsRequest(

                                          Destination,
                                          TariffIds,
                                          TariffKind,

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

                if (CustomClearTariffsRequestParser is not null)
                    ClearTariffsRequest = CustomClearTariffsRequestParser(JSON,
                                                                          ClearTariffsRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearTariffsRequest  = null;
                ErrorResponse        = "The given JSON representation of a ClearTariffs request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearTariffsRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearTariffsRequestSerializer">A delegate to serialize custom setTariffs requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearTariffsRequest>?  CustomClearTariffsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           TariffIds.Any()
                               ? new JProperty("tariffIds",    new JArray(TariffIds. Select(tariffId  => tariffId. ToString())))
                               : null,

                           TariffKind.HasValue
                               ? new JProperty("tariffKind",   TariffKind.Value.AsText())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearTariffsRequestSerializer is not null
                       ? CustomClearTariffsRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearTariffsRequest1, ClearTariffsRequest2)

        /// <summary>
        /// Compares two ClearTariffs requests for equality.
        /// </summary>
        /// <param name="ClearTariffsRequest1">A ClearTariffs request.</param>
        /// <param name="ClearTariffsRequest2">Another setTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearTariffsRequest? ClearTariffsRequest1,
                                           ClearTariffsRequest? ClearTariffsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearTariffsRequest1, ClearTariffsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearTariffsRequest1 is null || ClearTariffsRequest2 is null)
                return false;

            return ClearTariffsRequest1.Equals(ClearTariffsRequest2);

        }

        #endregion

        #region Operator != (ClearTariffsRequest1, ClearTariffsRequest2)

        /// <summary>
        /// Compares two ClearTariffs requests for inequality.
        /// </summary>
        /// <param name="ClearTariffsRequest1">A ClearTariffs request.</param>
        /// <param name="ClearTariffsRequest2">Another setTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearTariffsRequest? ClearTariffsRequest1,
                                           ClearTariffsRequest? ClearTariffsRequest2)

            => !(ClearTariffsRequest1 == ClearTariffsRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearTariffsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearTariffsRequest requests for equality.
        /// </summary>
        /// <param name="Object">A ClearTariffsRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearTariffsRequest clearTariffsRequest &&
                   Equals(clearTariffsRequest);

        #endregion

        #region Equals(ClearTariffsRequest)

        /// <summary>
        /// Compares two ClearTariffsRequest requests for equality.
        /// </summary>
        /// <param name="ClearTariffsRequest">A ClearTariffsRequest request to compare with.</param>
        public override Boolean Equals(ClearTariffsRequest? ClearTariffsRequest)

            => ClearTariffsRequest is not null &&

               TariffIds.Count().Equals(ClearTariffsRequest.TariffIds.Count()) &&
               TariffIds.All(tariffId => ClearTariffsRequest.TariffIds.Contains(tariffId)) &&

            ((!TariffKind.HasValue && !ClearTariffsRequest.TariffKind.HasValue) ||
              (TariffKind.HasValue &&  ClearTariffsRequest.TariffKind.HasValue && TariffKind.Value.Equals(ClearTariffsRequest.TariffKind.Value))) &&

               base.GenericEquals(ClearTariffsRequest);

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

            => !TariffIds.Any() && !TariffKind.HasValue
                   ? "Clear all tariffs"
                   : String.Concat(

                         "Clear tariffs for",

                         TariffIds.Any()
                                ? $" tariffIds: '{TariffIds.AggregateWith(", ")}'"
                                : "",

                         TariffIds.Any() && TariffKind.HasValue
                                ? " and"
                                : "",

                         TariffKind.HasValue
                                ? $" tariff kind: '{TariffKind.Value}'"
                                : ""

                     );

        #endregion

    }

}
