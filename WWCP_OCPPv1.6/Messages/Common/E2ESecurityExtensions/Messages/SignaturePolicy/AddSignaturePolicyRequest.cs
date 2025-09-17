///*
// * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Affero GPL license, Version 3.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.gnu.org/licenses/agpl.html
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System.Diagnostics.CodeAnalysis;

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6.CSMS
//{

//    /// <summary>
//    /// An add signature policy request.
//    /// </summary>
//    public class AddSignaturePolicyRequest : ARequest<AddSignaturePolicyRequest>,
//                                             IRequest
//    {

//        #region Data

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/addSignaturePolicyRequest");

//        #endregion

//        #region Properties

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public JSONLDContext    Context
//            => DefaultJSONLDContext;

//        /// <summary>
//        /// The signature policy.
//        /// </summary>
//        [Mandatory]
//        public SignaturePolicy  SignaturePolicy    { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new add signature policy request.
//        /// </summary>
//        /// <param name="Destination">The destination networking node identification or source routing path.</param>
//        /// <param name="SignaturePolicy">A signature policy.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public AddSignaturePolicyRequest(NetworkingNode_Id        NetworkingNodeId,
//                                         SignaturePolicy          SignaturePolicy,

//                                         IEnumerable<KeyPair>?    SignKeys            = null,
//                                         IEnumerable<SignInfo>?   SignInfos           = null,
//                                         IEnumerable<Signature>?  Signatures          = null,

//                                         CustomData?              CustomData          = null,

//                                         Request_Id?              RequestId           = null,
//                                         DateTimeOffset?          RequestTimestamp    = null,
//                                         TimeSpan?                RequestTimeout      = null,
//                                         EventTracking_Id?        EventTrackingId     = null,
//                                         NetworkPath?             NetworkPath         = null,
//                                         CancellationToken        CancellationToken   = default)

//            : base(NetworkingNodeId,
//                   nameof(AddSignaturePolicyRequest)[..^7],

//                   SignKeys,
//                   SignInfos,
//                   Signatures,

//                   CustomData,

//                   RequestId,
//                   RequestTimestamp,
//                   RequestTimeout,
//                   EventTrackingId,
//                   NetworkPath,
//                   CancellationToken)

//        {

//            this.SignaturePolicy = SignaturePolicy;

//            unchecked
//            {
//                hashCode = this.SignaturePolicy.GetHashCode() * 3 ^
//                           base.                GetHashCode();
//            }

//        }

//        #endregion


//        #region Documentation

//        // tba.

//        #endregion

//        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomAddSignaturePolicyRequestParser = null)

//        /// <summary>
//        /// Parse the given JSON representation of an AddSignaturePolicy request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="Destination">The destination networking node identification or source routing path.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="CustomAddSignaturePolicyRequestParser">An optional delegate to parse custom AddSignaturePolicy requests.</param>
//        public static AddSignaturePolicyRequest Parse(JObject                                                  JSON,
//                                                      Request_Id                                               RequestId,
//                                                      NetworkingNode_Id                                        NetworkingNodeId,
//                                                      NetworkPath                                              NetworkPath,
//                                                      CustomJObjectParserDelegate<AddSignaturePolicyRequest>?  CustomAddSignaturePolicyRequestParser   = null)
//        {


//            if (TryParse(JSON,
//                         RequestId,
//                         NetworkingNodeId,
//                         NetworkPath,
//                         out var addSignaturePolicyRequest,
//                         out var errorResponse,
//                         CustomAddSignaturePolicyRequestParser))
//            {
//                return addSignaturePolicyRequest;
//            }

//            throw new ArgumentException("The given JSON representation of a AddSignaturePolicy request is invalid: " + errorResponse,
//                                        nameof(JSON));

//        }

//        #endregion

//        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out AddSignaturePolicyRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

//        /// <summary>
//        /// Try to parse the given JSON representation of a AddSignaturePolicy request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="Destination">The destination networking node identification or source routing path.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="AddSignaturePolicyRequest">The parsed AddSignaturePolicy request.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        /// <param name="CustomAddSignaturePolicyRequestParser">An optional delegate to parse custom AddSignaturePolicy requests.</param>
//        public static Boolean TryParse(JObject                                                  JSON,
//                                       Request_Id                                               RequestId,
//                                       NetworkingNode_Id                                        NetworkingNodeId,
//                                       NetworkPath                                              NetworkPath,
//                                       [NotNullWhen(true)]  out AddSignaturePolicyRequest?      AddSignaturePolicyRequest,
//                                       [NotNullWhen(false)] out String?                         ErrorResponse,
//                                       CustomJObjectParserDelegate<AddSignaturePolicyRequest>?  CustomAddSignaturePolicyRequestParser)
//        {

//            try
//            {

//                AddSignaturePolicyRequest = null;

//                #region SignaturePolicy      [mandatory]

//                if (!JSON.ParseMandatoryJSON("signaturePolicy",
//                                             "signature policy",
//                                             WWCP.SignaturePolicy.TryParse,
//                                             out SignaturePolicy? SignaturePolicy,
//                                             out ErrorResponse) ||
//                     SignaturePolicy is null)
//                {
//                    return false;
//                }

//                #endregion

//                #region Signatures           [optional, OCPP_CSE]

//                if (JSON.ParseOptionalHashSet("signatures",
//                                              "cryptographic signatures",
//                                              Signature.TryParse,
//                                              out HashSet<Signature> Signatures,
//                                              out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region CustomData           [optional]

//                if (JSON.ParseOptionalJSON("customData",
//                                           "custom data",
//                                           WWCP.CustomData.TryParse,
//                                           out CustomData? CustomData,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion


//                AddSignaturePolicyRequest = new AddSignaturePolicyRequest(

//                                                NetworkingNodeId,
//                                                SignaturePolicy,

//                                                null,
//                                                null,
//                                                Signatures,

//                                                CustomData,

//                                                RequestId,
//                                                null,
//                                                null,
//                                                null,
//                                                NetworkPath

//                                            );

//                if (CustomAddSignaturePolicyRequestParser is not null)
//                    AddSignaturePolicyRequest = CustomAddSignaturePolicyRequestParser(JSON,
//                                                                                      AddSignaturePolicyRequest);

//                return true;

//            }
//            catch (Exception e)
//            {
//                AddSignaturePolicyRequest  = null;
//                ErrorResponse              = "The given JSON representation of a AddSignaturePolicy request is invalid: " + e.Message;
//                return false;
//            }

//        }

//        #endregion

//        #region ToJSON(CustomAddSignaturePolicyRequestSerializer = null, CustomSignaturePolicySerializer = null, ...)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomAddSignaturePolicyRequestSerializer">A delegate to serialize custom AddSignaturePolicy requests.</param>
//        /// <param name="CustomSignaturePolicySerializer">A delegate to serialize custom signature policies.</param>
//        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
//        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
//        public JObject ToJSON(CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?  CustomAddSignaturePolicyRequestSerializer   = null,
//                              CustomJObjectSerializerDelegate<SignaturePolicy>?            CustomSignaturePolicySerializer             = null,
//                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer                   = null,
//                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
//        {

//            var json = JSONObject.Create(

//                                 new JProperty("signaturePolicy",   SignaturePolicy.ToJSON(CustomSignaturePolicySerializer)),

//                           Signatures.Any()
//                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
//                                                                                                                               CustomCustomDataSerializer))))
//                               : null,

//                           CustomData is not null
//                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
//                               : null

//                       );

//            return CustomAddSignaturePolicyRequestSerializer is not null
//                       ? CustomAddSignaturePolicyRequestSerializer(this, json)
//                       : json;

//        }

//        #endregion


//        #region Operator overloading

//        #region Operator == (AddSignaturePolicyRequest1, AddSignaturePolicyRequest2)

//        /// <summary>
//        /// Compares two AddSignaturePolicy requests for equality.
//        /// </summary>
//        /// <param name="AddSignaturePolicyRequest1">A AddSignaturePolicy request.</param>
//        /// <param name="AddSignaturePolicyRequest2">Another AddSignaturePolicy request.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public static Boolean operator == (AddSignaturePolicyRequest? AddSignaturePolicyRequest1,
//                                           AddSignaturePolicyRequest? AddSignaturePolicyRequest2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(AddSignaturePolicyRequest1, AddSignaturePolicyRequest2))
//                return true;

//            // If one is null, but not both, return false.
//            if (AddSignaturePolicyRequest1 is null || AddSignaturePolicyRequest2 is null)
//                return false;

//            return AddSignaturePolicyRequest1.Equals(AddSignaturePolicyRequest2);

//        }

//        #endregion

//        #region Operator != (AddSignaturePolicyRequest1, AddSignaturePolicyRequest2)

//        /// <summary>
//        /// Compares two AddSignaturePolicy requests for inequality.
//        /// </summary>
//        /// <param name="AddSignaturePolicyRequest1">A AddSignaturePolicy request.</param>
//        /// <param name="AddSignaturePolicyRequest2">Another AddSignaturePolicy request.</param>
//        /// <returns>False if both match; True otherwise.</returns>
//        public static Boolean operator != (AddSignaturePolicyRequest? AddSignaturePolicyRequest1,
//                                           AddSignaturePolicyRequest? AddSignaturePolicyRequest2)

//            => !(AddSignaturePolicyRequest1 == AddSignaturePolicyRequest2);

//        #endregion

//        #endregion

//        #region IEquatable<AddSignaturePolicyRequest> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two AddSignaturePolicy requests for equality.
//        /// </summary>
//        /// <param name="Object">A AddSignaturePolicy request to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is AddSignaturePolicyRequest addSignaturePolicyRequest &&
//                   Equals(addSignaturePolicyRequest);

//        #endregion

//        #region Equals(AddSignaturePolicyRequest)

//        /// <summary>
//        /// Compares two AddSignaturePolicy requests for equality.
//        /// </summary>
//        /// <param name="AddSignaturePolicyRequest">A AddSignaturePolicy request to compare with.</param>
//        public override Boolean Equals(AddSignaturePolicyRequest? AddSignaturePolicyRequest)

//            => AddSignaturePolicyRequest is not null &&

//               SignaturePolicy.Equals(AddSignaturePolicyRequest.SignaturePolicy) &&

//               base.    GenericEquals(AddSignaturePolicyRequest);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        private readonly Int32 hashCode;

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => hashCode;

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => SignaturePolicy.ToString();

//        #endregion

//    }

//}
