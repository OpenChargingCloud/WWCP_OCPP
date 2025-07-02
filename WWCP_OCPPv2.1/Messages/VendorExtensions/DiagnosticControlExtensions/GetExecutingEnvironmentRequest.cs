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
    /// The GetExecutingEnvironment request.
    /// </summary>
    public class GetExecutingEnvironmentRequest : ARequest<GetExecutingEnvironmentRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/attachCableRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetExecutingEnvironment request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetExecutingEnvironmentRequest(SourceRouting            Destination,

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
                   nameof(GetExecutingEnvironmentRequest)[..^7],

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

            unchecked
            {

                hashCode = base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetExecutingEnvironment request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetExecutingEnvironmentRequestParser">An optional delegate to parse custom GetExecutingEnvironment requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static GetExecutingEnvironmentRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<GetExecutingEnvironmentRequest>?  CustomGetExecutingEnvironmentRequestParser   = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var attachCableRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetExecutingEnvironmentRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return attachCableRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetExecutingEnvironment request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetExecutingEnvironmentRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetExecutingEnvironment request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetExecutingEnvironmentRequest">The parsed GetExecutingEnvironment request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetExecutingEnvironmentRequestParser">An optional delegate to parse custom GetExecutingEnvironment requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out GetExecutingEnvironmentRequest?      GetExecutingEnvironmentRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<GetExecutingEnvironmentRequest>?  CustomGetExecutingEnvironmentRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            try
            {

                GetExecutingEnvironmentRequest = null;

                #region Signatures       [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData       [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? customData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetExecutingEnvironmentRequest = new GetExecutingEnvironmentRequest(

                                                     Destination,

                                                     null,
                                                     null,
                                                     signatures,

                                                     customData,

                                                     RequestId,
                                                     RequestTimestamp,
                                                     RequestTimeout,
                                                     EventTrackingId,
                                                     NetworkPath

                                                 );

                if (CustomGetExecutingEnvironmentRequestParser is not null)
                    GetExecutingEnvironmentRequest = CustomGetExecutingEnvironmentRequestParser(JSON,
                                                                                                GetExecutingEnvironmentRequest);

                return true;

            }
            catch (Exception e)
            {
                GetExecutingEnvironmentRequest  = null;
                ErrorResponse                   = "The given JSON representation of a GetExecutingEnvironment request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetExecutingEnvironmentRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetExecutingEnvironmentRequestSerializer">A delegate to serialize custom GetExecutingEnvironment requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetExecutingEnvironmentRequest>?  CustomGetExecutingEnvironmentRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetExecutingEnvironmentRequestSerializer is not null
                       ? CustomGetExecutingEnvironmentRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetExecutingEnvironmentRequest1, GetExecutingEnvironmentRequest2)

        /// <summary>
        /// Compares two GetExecutingEnvironment requests for equality.
        /// </summary>
        /// <param name="GetExecutingEnvironmentRequest1">A GetExecutingEnvironment request.</param>
        /// <param name="GetExecutingEnvironmentRequest2">Another GetExecutingEnvironment request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetExecutingEnvironmentRequest? GetExecutingEnvironmentRequest1,
                                           GetExecutingEnvironmentRequest? GetExecutingEnvironmentRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetExecutingEnvironmentRequest1, GetExecutingEnvironmentRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetExecutingEnvironmentRequest1 is null || GetExecutingEnvironmentRequest2 is null)
                return false;

            return GetExecutingEnvironmentRequest1.Equals(GetExecutingEnvironmentRequest2);

        }

        #endregion

        #region Operator != (GetExecutingEnvironmentRequest1, GetExecutingEnvironmentRequest2)

        /// <summary>
        /// Compares two GetExecutingEnvironment requests for inequality.
        /// </summary>
        /// <param name="GetExecutingEnvironmentRequest1">A GetExecutingEnvironment request.</param>
        /// <param name="GetExecutingEnvironmentRequest2">Another GetExecutingEnvironment request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetExecutingEnvironmentRequest? GetExecutingEnvironmentRequest1,
                                           GetExecutingEnvironmentRequest? GetExecutingEnvironmentRequest2)

            => !(GetExecutingEnvironmentRequest1 == GetExecutingEnvironmentRequest2);

        #endregion

        #endregion

        #region IEquatable<GetExecutingEnvironmentRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetExecutingEnvironment requests for equality.
        /// </summary>
        /// <param name="Object">A GetExecutingEnvironment request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetExecutingEnvironmentRequest attachCableRequest &&
                   Equals(attachCableRequest);

        #endregion

        #region Equals(GetExecutingEnvironmentRequest)

        /// <summary>
        /// Compares two GetExecutingEnvironment requests for equality.
        /// </summary>
        /// <param name="GetExecutingEnvironmentRequest">A GetExecutingEnvironment request to compare with.</param>
        public override Boolean Equals(GetExecutingEnvironmentRequest? GetExecutingEnvironmentRequest)

            => GetExecutingEnvironmentRequest is not null &&
               base.GenericEquals(GetExecutingEnvironmentRequest);

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

            => base.ToString();

        #endregion

    }

}
