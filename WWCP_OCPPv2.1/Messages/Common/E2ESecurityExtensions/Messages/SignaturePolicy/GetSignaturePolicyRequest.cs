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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The GetSignaturePolicy request: the policy table a node has stored, as it received it.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4.
    /// </remarks>
    public class GetSignaturePolicyRequest : ARequest<GetSignaturePolicyRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getSignaturePolicyRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetSignaturePolicy request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
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
        public GetSignaturePolicyRequest(SourceRouting            Destination,

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
                   nameof(GetSignaturePolicyRequest)[..^7],

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
                hashCode = (0) * 3 ^
                           base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomGetSignaturePolicyRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetSignaturePolicy request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetSignaturePolicyRequestParser">An optional delegate to parse custom GetSignaturePolicy requests.</param>
        public static GetSignaturePolicyRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTimeOffset?                                             RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<GetSignaturePolicyRequest>?  CustomGetSignaturePolicyRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getSignaturePolicyRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetSignaturePolicyRequestParser))
            {
                return getSignaturePolicyRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetSignaturePolicy request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetSignaturePolicyRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetSignaturePolicy request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetSignaturePolicyRequest">The parsed GetSignaturePolicy request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetSignaturePolicyRequestParser">An optional delegate to parse custom GetSignaturePolicy requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out GetSignaturePolicyRequest?      GetSignaturePolicyRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTimeOffset?                                             RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<GetSignaturePolicyRequest>?  CustomGetSignaturePolicyRequestParser   = null)
        {

            try
            {

                GetSignaturePolicyRequest = null;


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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetSignaturePolicyRequest = new GetSignaturePolicyRequest(

                                                   Destination,

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

                if (CustomGetSignaturePolicyRequestParser is not null)
                    GetSignaturePolicyRequest = CustomGetSignaturePolicyRequestParser(JSON,
                                                                                            GetSignaturePolicyRequest);

                return true;

            }
            catch (Exception e)
            {
                GetSignaturePolicyRequest  = null;
                ErrorResponse                 = "The given JSON representation of a GetSignaturePolicy request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetSignaturePolicyRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetSignaturePolicyRequestSerializer">A delegate to serialize custom GetSignaturePolicy requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetSignaturePolicyRequest>?  CustomGetSignaturePolicyRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(


                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetSignaturePolicyRequestSerializer is not null
                       ? CustomGetSignaturePolicyRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out GetSignaturePolicyRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a GetSignaturePolicy request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="GetSignaturePolicyRequest">The GetSignaturePolicy request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetSignaturePolicyRequestParser">A delegate to read custom GetSignaturePolicy requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                                CBOR,
                                           Request_Id                                               RequestId,
                                           SourceRouting                                            Destination,
                                           NetworkPath                                              NetworkPath,
                                           [NotNullWhen(true)]  out GetSignaturePolicyRequest?   GetSignaturePolicyRequest,
                                           [NotNullWhen(false)] out String?                         ErrorResponse,
                                           DateTimeOffset?                                          RequestTimestamp                           = null,
                                           TimeSpan?                                                RequestTimeout                             = null,
                                           EventTracking_Id?                                        EventTrackingId                            = null,
                                           CustomCBORParserDelegate<GetSignaturePolicyRequest>?  CustomGetSignaturePolicyRequestParser   = null)
        {

            try
            {

                GetSignaturePolicyRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a GetSignaturePolicy request is not a map!";
                    return false;
                }


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


                GetSignaturePolicyRequest = new GetSignaturePolicyRequest(

                                                   Destination,

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

                if (CustomGetSignaturePolicyRequestParser is not null)
                    GetSignaturePolicyRequest = CustomGetSignaturePolicyRequestParser(CBOR,
                                                                                            GetSignaturePolicyRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetSignaturePolicyRequest = null;
                ErrorResponse = "The given CBOR representation of a GetSignaturePolicy request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomGetSignaturePolicyRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this GetSignaturePolicy request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomGetSignaturePolicyRequestSerializer">A delegate to serialize custom GetSignaturePolicy requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<GetSignaturePolicyRequest>? CustomGetSignaturePolicyRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomGetSignaturePolicyRequestSerializer is not null
                       ? CustomGetSignaturePolicyRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetSignaturePolicyRequest1, GetSignaturePolicyRequest2)

        /// <summary>
        /// Compares two GetSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="GetSignaturePolicyRequest1">A GetSignaturePolicy request.</param>
        /// <param name="GetSignaturePolicyRequest2">Another GetSignaturePolicy request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetSignaturePolicyRequest? GetSignaturePolicyRequest1,
                                           GetSignaturePolicyRequest? GetSignaturePolicyRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetSignaturePolicyRequest1, GetSignaturePolicyRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetSignaturePolicyRequest1 is null || GetSignaturePolicyRequest2 is null)
                return false;

            return GetSignaturePolicyRequest1.Equals(GetSignaturePolicyRequest2);

        }

        #endregion

        #region Operator != (GetSignaturePolicyRequest1, GetSignaturePolicyRequest2)

        /// <summary>
        /// Compares two GetSignaturePolicy requests for inequality.
        /// </summary>
        /// <param name="GetSignaturePolicyRequest1">A GetSignaturePolicy request.</param>
        /// <param name="GetSignaturePolicyRequest2">Another GetSignaturePolicy request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetSignaturePolicyRequest? GetSignaturePolicyRequest1,
                                           GetSignaturePolicyRequest? GetSignaturePolicyRequest2)

            => !(GetSignaturePolicyRequest1 == GetSignaturePolicyRequest2);

        #endregion

        #endregion

        #region IEquatable<GetSignaturePolicyRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="Object">A GetSignaturePolicy request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetSignaturePolicyRequest getSignaturePolicyRequest &&
                   Equals(getSignaturePolicyRequest);

        #endregion

        #region Equals(GetSignaturePolicyRequest)

        /// <summary>
        /// Compares two GetSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="GetSignaturePolicyRequest">A GetSignaturePolicy request to compare with.</param>
        public override Boolean Equals(GetSignaturePolicyRequest? GetSignaturePolicyRequest)

            => GetSignaturePolicyRequest is not null &&


               base.      GenericEquals(GetSignaturePolicyRequest);

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

            => "GetSignaturePolicy";

        #endregion

    }

}
