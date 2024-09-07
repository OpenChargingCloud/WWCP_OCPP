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
    /// The GetTariffs request.
    /// </summary>
    public class GetTariffsRequest : ARequest<GetTariffsRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getTariffsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional EVSE identification to get the tariff from.
        /// </summary>
        [Optional]
        public EVSE_Id?       EVSEId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetTariffs request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="EVSEId">An optional EVSE identification to get the tariff from.</param>
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
        public GetTariffsRequest(SourceRouting            Destination,
                                 EVSE_Id?                 EVSEId                = null,

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
                   nameof(GetTariffsRequest)[..^7],

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

            this.EVSEId = EVSEId;

            unchecked
            {
                hashCode = (this.EVSEId?.GetHashCode() ?? 0) * 3 ^
                            base.        GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetTariffsRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetTariffsRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static GetTariffsRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              SourceRouting                                    Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        RequestTimestamp                = null,
                                              TimeSpan?                                        RequestTimeout                  = null,
                                              EventTracking_Id?                                EventTrackingId                 = null,
                                              CustomJObjectParserDelegate<GetTariffsRequest>?  CustomGetTariffsRequestParser   = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getTariffsRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetTariffsRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getTariffsRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetTariffs request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetTariffsRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetTariffsRequest">The parsed setTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetTariffsRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out GetTariffsRequest?      GetTariffsRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        RequestTimestamp                = null,
                                       TimeSpan?                                        RequestTimeout                  = null,
                                       EventTracking_Id?                                EventTrackingId                 = null,
                                       CustomJObjectParserDelegate<GetTariffsRequest>?  CustomGetTariffsRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            try
            {

                GetTariffsRequest = null;

                #region EVSEId        [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
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


                GetTariffsRequest = new GetTariffsRequest(

                                        Destination,
                                        EVSEId,

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

                if (CustomGetTariffsRequestParser is not null)
                    GetTariffsRequest = CustomGetTariffsRequestParser(JSON,
                                                                      GetTariffsRequest);

                return true;

            }
            catch (Exception e)
            {
                GetTariffsRequest  = null;
                ErrorResponse     = "The given JSON representation of a GetTariffs request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetTariffsRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetTariffsRequestSerializer">A delegate to serialize custom setTariffs requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetTariffsRequest>?  CustomGetTariffsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           EVSEId.HasValue
                               ? new JProperty("evseId",       EVSEId.Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.  ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetTariffsRequestSerializer is not null
                       ? CustomGetTariffsRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetTariffsRequest1, GetTariffsRequest2)

        /// <summary>
        /// Compares two GetTariffs requests for equality.
        /// </summary>
        /// <param name="GetTariffsRequest1">A GetTariffs request.</param>
        /// <param name="GetTariffsRequest2">Another setTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetTariffsRequest? GetTariffsRequest1,
                                           GetTariffsRequest? GetTariffsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetTariffsRequest1, GetTariffsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetTariffsRequest1 is null || GetTariffsRequest2 is null)
                return false;

            return GetTariffsRequest1.Equals(GetTariffsRequest2);

        }

        #endregion

        #region Operator != (GetTariffsRequest1, GetTariffsRequest2)

        /// <summary>
        /// Compares two GetTariffs requests for inequality.
        /// </summary>
        /// <param name="GetTariffsRequest1">A GetTariffs request.</param>
        /// <param name="GetTariffsRequest2">Another setTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetTariffsRequest? GetTariffsRequest1,
                                           GetTariffsRequest? GetTariffsRequest2)

            => !(GetTariffsRequest1 == GetTariffsRequest2);

        #endregion

        #endregion

        #region IEquatable<GetTariffsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetTariffsRequest requests for equality.
        /// </summary>
        /// <param name="Object">A GetTariffsRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetTariffsRequest getTariffsRequest &&
                   Equals(getTariffsRequest);

        #endregion

        #region Equals(GetTariffsRequest)

        /// <summary>
        /// Compares two GetTariffsRequest requests for equality.
        /// </summary>
        /// <param name="GetTariffsRequest">A GetTariffsRequest request to compare with.</param>
        public override Boolean Equals(GetTariffsRequest? GetTariffsRequest)

            => GetTariffsRequest is not null &&

            ((!EVSEId.HasValue && !GetTariffsRequest.EVSEId.HasValue) ||
              (EVSEId.HasValue &&  GetTariffsRequest.EVSEId.HasValue && EVSEId.Value.Equals(GetTariffsRequest.EVSEId.Value))) &&

               base.GenericEquals(GetTariffsRequest);

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

            => $"Get tariff{(EVSEId.HasValue ? $" from EVSE: {EVSEId.Value}" : "")}";

        #endregion

    }

}
