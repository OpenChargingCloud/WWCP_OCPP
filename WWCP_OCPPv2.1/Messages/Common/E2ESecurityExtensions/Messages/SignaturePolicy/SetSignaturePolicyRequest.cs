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
    /// The SetSignaturePolicy request: the policy table of a node, as a signed data object with a version.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4.
    /// </remarks>
    public class SetSignaturePolicyRequest : ARequest<SetSignaturePolicyRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/setSignaturePolicyRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The signed policy, as it is to be stored: its version and its entries.
        /// </summary>
        [Mandatory]
        public JObject             Policy               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetSignaturePolicy request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Policy">The signed policy, as it is to be stored.</param>
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
        public SetSignaturePolicyRequest(SourceRouting            Destination,
                                            JObject                  Policy,

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
                   nameof(SetSignaturePolicyRequest)[..^7],

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

            this.Policy = (JObject) Policy.DeepClone();

            unchecked
            {
                hashCode = (this.Policy.ToString(Newtonsoft.Json.Formatting.None).GetHashCode()) * 3 ^
                           base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomSetSignaturePolicyRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetSignaturePolicy request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetSignaturePolicyRequestParser">An optional delegate to parse custom SetSignaturePolicy requests.</param>
        public static SetSignaturePolicyRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTimeOffset?                                             RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<SetSignaturePolicyRequest>?  CustomSetSignaturePolicyRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setSignaturePolicyRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetSignaturePolicyRequestParser))
            {
                return setSignaturePolicyRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetSignaturePolicy request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetSignaturePolicyRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetSignaturePolicy request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetSignaturePolicyRequest">The parsed SetSignaturePolicy request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetSignaturePolicyRequestParser">An optional delegate to parse custom SetSignaturePolicy requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out SetSignaturePolicyRequest?      SetSignaturePolicyRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTimeOffset?                                             RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<SetSignaturePolicyRequest>?  CustomSetSignaturePolicyRequestParser   = null)
        {

            try
            {

                SetSignaturePolicyRequest = null;

                #region Policy               [mandatory]

                // As it was received: its signatures are over what it says, member for member.
                if (JSON["policy"] is not JObject Policy)
                {
                    ErrorResponse = "The policy is missing, or is not an object!";
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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetSignaturePolicyRequest = new SetSignaturePolicyRequest(

                                                   Destination,
                                                   Policy,

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

                if (CustomSetSignaturePolicyRequestParser is not null)
                    SetSignaturePolicyRequest = CustomSetSignaturePolicyRequestParser(JSON,
                                                                                            SetSignaturePolicyRequest);

                return true;

            }
            catch (Exception e)
            {
                SetSignaturePolicyRequest  = null;
                ErrorResponse                 = "The given JSON representation of a SetSignaturePolicy request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetSignaturePolicyRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetSignaturePolicyRequestSerializer">A delegate to serialize custom SetSignaturePolicy requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetSignaturePolicyRequest>?  CustomSetSignaturePolicyRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("policy",              Policy.DeepClone()),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetSignaturePolicyRequestSerializer is not null
                       ? CustomSetSignaturePolicyRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out SetSignaturePolicyRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a SetSignaturePolicy request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="SetSignaturePolicyRequest">The SetSignaturePolicy request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetSignaturePolicyRequestParser">A delegate to read custom SetSignaturePolicy requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                                CBOR,
                                           Request_Id                                               RequestId,
                                           SourceRouting                                            Destination,
                                           NetworkPath                                              NetworkPath,
                                           [NotNullWhen(true)]  out SetSignaturePolicyRequest?   SetSignaturePolicyRequest,
                                           [NotNullWhen(false)] out String?                         ErrorResponse,
                                           DateTimeOffset?                                          RequestTimestamp                           = null,
                                           TimeSpan?                                                RequestTimeout                             = null,
                                           EventTracking_Id?                                        EventTrackingId                            = null,
                                           CustomCBORParserDelegate<SetSignaturePolicyRequest>?  CustomSetSignaturePolicyRequestParser   = null)
        {

            try
            {

                SetSignaturePolicyRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a SetSignaturePolicy request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatory("policy",
                                         "policy",
                                         SignedObjectCBORExtensions.TryParseJSONObject,
                                         out JObject? Policy,
                                         out ErrorResponse))
                {
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


                SetSignaturePolicyRequest = new SetSignaturePolicyRequest(

                                                   Destination,
                                                   Policy,

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

                if (CustomSetSignaturePolicyRequestParser is not null)
                    SetSignaturePolicyRequest = CustomSetSignaturePolicyRequestParser(CBOR,
                                                                                            SetSignaturePolicyRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SetSignaturePolicyRequest = null;
                ErrorResponse = "The given CBOR representation of a SetSignaturePolicy request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomSetSignaturePolicyRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this SetSignaturePolicy request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomSetSignaturePolicyRequestSerializer">A delegate to serialize custom SetSignaturePolicy requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<SetSignaturePolicyRequest>? CustomSetSignaturePolicyRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("policy",                    SignedObjectCBORExtensions.ToCBOR(Policy)),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomSetSignaturePolicyRequestSerializer is not null
                       ? CustomSetSignaturePolicyRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetSignaturePolicyRequest1, SetSignaturePolicyRequest2)

        /// <summary>
        /// Compares two SetSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="SetSignaturePolicyRequest1">A SetSignaturePolicy request.</param>
        /// <param name="SetSignaturePolicyRequest2">Another SetSignaturePolicy request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetSignaturePolicyRequest? SetSignaturePolicyRequest1,
                                           SetSignaturePolicyRequest? SetSignaturePolicyRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetSignaturePolicyRequest1, SetSignaturePolicyRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetSignaturePolicyRequest1 is null || SetSignaturePolicyRequest2 is null)
                return false;

            return SetSignaturePolicyRequest1.Equals(SetSignaturePolicyRequest2);

        }

        #endregion

        #region Operator != (SetSignaturePolicyRequest1, SetSignaturePolicyRequest2)

        /// <summary>
        /// Compares two SetSignaturePolicy requests for inequality.
        /// </summary>
        /// <param name="SetSignaturePolicyRequest1">A SetSignaturePolicy request.</param>
        /// <param name="SetSignaturePolicyRequest2">Another SetSignaturePolicy request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetSignaturePolicyRequest? SetSignaturePolicyRequest1,
                                           SetSignaturePolicyRequest? SetSignaturePolicyRequest2)

            => !(SetSignaturePolicyRequest1 == SetSignaturePolicyRequest2);

        #endregion

        #endregion

        #region IEquatable<SetSignaturePolicyRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="Object">A SetSignaturePolicy request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetSignaturePolicyRequest setSignaturePolicyRequest &&
                   Equals(setSignaturePolicyRequest);

        #endregion

        #region Equals(SetSignaturePolicyRequest)

        /// <summary>
        /// Compares two SetSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="SetSignaturePolicyRequest">A SetSignaturePolicy request to compare with.</param>
        public override Boolean Equals(SetSignaturePolicyRequest? SetSignaturePolicyRequest)

            => SetSignaturePolicyRequest is not null &&

               JToken.DeepEquals(Policy, SetSignaturePolicyRequest.Policy) &&

               base.      GenericEquals(SetSignaturePolicyRequest);

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

            => $"Policy version {Policy["version"]}";

        #endregion

    }

}
