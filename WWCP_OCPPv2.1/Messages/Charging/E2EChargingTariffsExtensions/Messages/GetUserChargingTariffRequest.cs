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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    //Note: This command is a draft version of the OCPP 2.1 specification
    //       and might be subject to change in future versions of the specification!

    /// <summary>
    /// An get user charging tariff request.
    /// </summary>
    public class GetUserChargingTariffRequest : ARequest<GetUserChargingTariffRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getUserChargingTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification token for which the applicable charging tariff is requested.
        /// </summary>
        [Mandatory]
        public IdToken        IdToken    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get user charging tariff request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="IdToken">An identification token for which the applicable charging tariff is requested.</param>
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
        public GetUserChargingTariffRequest(SourceRouting            Destination,
                                            IdToken                  IdToken,

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
                   nameof(GetUserChargingTariffRequest)[..^7],

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

            this.IdToken = IdToken;

            unchecked
            {
                hashCode = this.IdToken.GetHashCode() * 3 ^
                           base.        GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetUserChargingTariffRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an GetUserChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomGetUserChargingTariffRequestParser">A delegate to parse custom GetUserChargingTariff requests.</param>
        public static GetUserChargingTariffRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         CustomJObjectParserDelegate<GetUserChargingTariffRequest>?  CustomGetUserChargingTariffRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getUserChargingTariffRequest,
                         out var errorResponse,
                         CustomGetUserChargingTariffRequestParser) &&
                getUserChargingTariffRequest is not null)
            {
                return getUserChargingTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetUserChargingTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out getUserChargingTariffRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a GetUserChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="getUserChargingTariffRequest">The parsed GetUserChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       Request_Id                         RequestId,
                                       SourceRouting                  Destination,
                                       NetworkPath                        NetworkPath,
                                       out GetUserChargingTariffRequest?  getUserChargingTariffRequest,
                                       out String?                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        Destination,
                        NetworkPath,
                        out getUserChargingTariffRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a GetUserChargingTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetUserChargingTariffRequest">The parsed GetUserChargingTariff request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetUserChargingTariffRequestParser">A delegate to parse custom GetUserChargingTariff requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       out GetUserChargingTariffRequest?                           GetUserChargingTariffRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<GetUserChargingTariffRequest>?  CustomGetUserChargingTariffRequestParser)
        {

            try
            {

                GetUserChargingTariffRequest = null;

                #region IdToken              [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse) ||
                    IdToken is null)
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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetUserChargingTariffRequest = new GetUserChargingTariffRequest(

                                                   Destination,
                                                   IdToken,

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

                if (CustomGetUserChargingTariffRequestParser is not null)
                    GetUserChargingTariffRequest = CustomGetUserChargingTariffRequestParser(JSON,
                                                                                            GetUserChargingTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                GetUserChargingTariffRequest  = null;
                ErrorResponse                 = "The given JSON representation of a GetUserChargingTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetUserChargingTariffRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetUserChargingTariffRequestSerializer">A delegate to serialize custom GetUserChargingTariff requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetUserChargingTariffRequest>?  CustomGetUserChargingTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?                       CustomIdTokenSerializer                        = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?                CustomAdditionalInfoSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idToken",      IdToken.   ToJSON(CustomIdTokenSerializer,
                                                                                 CustomAdditionalInfoSerializer,
                                                                                 CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetUserChargingTariffRequestSerializer is not null
                       ? CustomGetUserChargingTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetUserChargingTariffRequest1, GetUserChargingTariffRequest2)

        /// <summary>
        /// Compares two GetUserChargingTariff requests for equality.
        /// </summary>
        /// <param name="GetUserChargingTariffRequest1">A GetUserChargingTariff request.</param>
        /// <param name="GetUserChargingTariffRequest2">Another setUserChargingTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetUserChargingTariffRequest? GetUserChargingTariffRequest1,
                                           GetUserChargingTariffRequest? GetUserChargingTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetUserChargingTariffRequest1, GetUserChargingTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetUserChargingTariffRequest1 is null || GetUserChargingTariffRequest2 is null)
                return false;

            return GetUserChargingTariffRequest1.Equals(GetUserChargingTariffRequest2);

        }

        #endregion

        #region Operator != (GetUserChargingTariffRequest1, GetUserChargingTariffRequest2)

        /// <summary>
        /// Compares two GetUserChargingTariff requests for inequality.
        /// </summary>
        /// <param name="GetUserChargingTariffRequest1">A GetUserChargingTariff request.</param>
        /// <param name="GetUserChargingTariffRequest2">Another setUserChargingTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetUserChargingTariffRequest? GetUserChargingTariffRequest1,
                                           GetUserChargingTariffRequest? GetUserChargingTariffRequest2)

            => !(GetUserChargingTariffRequest1 == GetUserChargingTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<GetUserChargingTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetUserChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A GetUserChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetUserChargingTariffRequest getUserChargingTariffRequest &&
                   Equals(getUserChargingTariffRequest);

        #endregion

        #region Equals(GetUserChargingTariffRequest)

        /// <summary>
        /// Compares two GetUserChargingTariffRequest requests for equality.
        /// </summary>
        /// <param name="GetUserChargingTariffRequest">A GetUserChargingTariffRequest request to compare with.</param>
        public override Boolean Equals(GetUserChargingTariffRequest? GetUserChargingTariffRequest)

            => GetUserChargingTariffRequest is not null &&

               IdToken.       Equals(GetUserChargingTariffRequest.IdToken) &&

               base.   GenericEquals(GetUserChargingTariffRequest);

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

            => IdToken.ToString();

        #endregion

    }

}
