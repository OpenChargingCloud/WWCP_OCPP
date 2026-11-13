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
    /// The AddSignerRole request: keys for a signer role that has none yet, as a signed role assignment.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4.
    /// </remarks>
    public class AddSignerRoleRequest : ARequest<AddSignerRoleRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/addSignerRoleRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The signed role assignment, as it is to be stored: the role, its keys and its version.
        /// </summary>
        [Mandatory]
        public JObject             SignerRole           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AddSignerRole request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="SignerRole">The signed role assignment, as it is to be stored.</param>
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
        public AddSignerRoleRequest(SourceRouting            Destination,
                                            JObject                  SignerRole,

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
                   nameof(AddSignerRoleRequest)[..^7],

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

            this.SignerRole = (JObject) SignerRole.DeepClone();

            unchecked
            {
                hashCode = (this.SignerRole.ToString(Newtonsoft.Json.Formatting.None).GetHashCode()) * 3 ^
                           base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomAddSignerRoleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a AddSignerRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAddSignerRoleRequestParser">An optional delegate to parse custom AddSignerRole requests.</param>
        public static AddSignerRoleRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTimeOffset?                                             RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<AddSignerRoleRequest>?  CustomAddSignerRoleRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var addSignerRoleRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomAddSignerRoleRequestParser))
            {
                return addSignerRoleRequest;
            }

            throw new ArgumentException("The given JSON representation of a AddSignerRole request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out AddSignerRoleRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a AddSignerRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AddSignerRoleRequest">The parsed AddSignerRole request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAddSignerRoleRequestParser">An optional delegate to parse custom AddSignerRole requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out AddSignerRoleRequest?      AddSignerRoleRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTimeOffset?                                             RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<AddSignerRoleRequest>?  CustomAddSignerRoleRequestParser   = null)
        {

            try
            {

                AddSignerRoleRequest = null;

                #region SignerRole           [mandatory]

                // As it was received: its signatures are over what it says, member for member.
                if (JSON["signerRole"] is not JObject SignerRole)
                {
                    ErrorResponse = "The role assignment is missing, or is not an object!";
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


                AddSignerRoleRequest = new AddSignerRoleRequest(

                                                   Destination,
                                                   SignerRole,

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

                if (CustomAddSignerRoleRequestParser is not null)
                    AddSignerRoleRequest = CustomAddSignerRoleRequestParser(JSON,
                                                                                            AddSignerRoleRequest);

                return true;

            }
            catch (Exception e)
            {
                AddSignerRoleRequest  = null;
                ErrorResponse                 = "The given JSON representation of a AddSignerRole request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddSignerRoleRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddSignerRoleRequestSerializer">A delegate to serialize custom AddSignerRole requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AddSignerRoleRequest>?  CustomAddSignerRoleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("signerRole",          SignerRole.DeepClone()),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAddSignerRoleRequestSerializer is not null
                       ? CustomAddSignerRoleRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out AddSignerRoleRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a AddSignerRole request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="AddSignerRoleRequest">The AddSignerRole request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAddSignerRoleRequestParser">A delegate to read custom AddSignerRole requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                                CBOR,
                                           Request_Id                                               RequestId,
                                           SourceRouting                                            Destination,
                                           NetworkPath                                              NetworkPath,
                                           [NotNullWhen(true)]  out AddSignerRoleRequest?   AddSignerRoleRequest,
                                           [NotNullWhen(false)] out String?                         ErrorResponse,
                                           DateTimeOffset?                                          RequestTimestamp                           = null,
                                           TimeSpan?                                                RequestTimeout                             = null,
                                           EventTracking_Id?                                        EventTrackingId                            = null,
                                           CustomCBORParserDelegate<AddSignerRoleRequest>?  CustomAddSignerRoleRequestParser   = null)
        {

            try
            {

                AddSignerRoleRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a AddSignerRole request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatory("signerRole",
                                         "role assignment",
                                         SignedObjectCBORExtensions.TryParseJSONObject,
                                         out JObject? SignerRole,
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


                AddSignerRoleRequest = new AddSignerRoleRequest(

                                                   Destination,
                                                   SignerRole,

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

                if (CustomAddSignerRoleRequestParser is not null)
                    AddSignerRoleRequest = CustomAddSignerRoleRequestParser(CBOR,
                                                                                            AddSignerRoleRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                AddSignerRoleRequest = null;
                ErrorResponse = "The given CBOR representation of a AddSignerRole request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomAddSignerRoleRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this AddSignerRole request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomAddSignerRoleRequestSerializer">A delegate to serialize custom AddSignerRole requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<AddSignerRoleRequest>? CustomAddSignerRoleRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("signerRole",                SignedObjectCBORExtensions.ToCBOR(SignerRole)),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomAddSignerRoleRequestSerializer is not null
                       ? CustomAddSignerRoleRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AddSignerRoleRequest1, AddSignerRoleRequest2)

        /// <summary>
        /// Compares two AddSignerRole requests for equality.
        /// </summary>
        /// <param name="AddSignerRoleRequest1">A AddSignerRole request.</param>
        /// <param name="AddSignerRoleRequest2">Another AddSignerRole request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AddSignerRoleRequest? AddSignerRoleRequest1,
                                           AddSignerRoleRequest? AddSignerRoleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AddSignerRoleRequest1, AddSignerRoleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AddSignerRoleRequest1 is null || AddSignerRoleRequest2 is null)
                return false;

            return AddSignerRoleRequest1.Equals(AddSignerRoleRequest2);

        }

        #endregion

        #region Operator != (AddSignerRoleRequest1, AddSignerRoleRequest2)

        /// <summary>
        /// Compares two AddSignerRole requests for inequality.
        /// </summary>
        /// <param name="AddSignerRoleRequest1">A AddSignerRole request.</param>
        /// <param name="AddSignerRoleRequest2">Another AddSignerRole request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AddSignerRoleRequest? AddSignerRoleRequest1,
                                           AddSignerRoleRequest? AddSignerRoleRequest2)

            => !(AddSignerRoleRequest1 == AddSignerRoleRequest2);

        #endregion

        #endregion

        #region IEquatable<AddSignerRoleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AddSignerRole requests for equality.
        /// </summary>
        /// <param name="Object">A AddSignerRole request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddSignerRoleRequest addSignerRoleRequest &&
                   Equals(addSignerRoleRequest);

        #endregion

        #region Equals(AddSignerRoleRequest)

        /// <summary>
        /// Compares two AddSignerRole requests for equality.
        /// </summary>
        /// <param name="AddSignerRoleRequest">A AddSignerRole request to compare with.</param>
        public override Boolean Equals(AddSignerRoleRequest? AddSignerRoleRequest)

            => AddSignerRoleRequest is not null &&

               JToken.DeepEquals(SignerRole, AddSignerRoleRequest.SignerRole) &&

               base.      GenericEquals(AddSignerRoleRequest);

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

            => $"Role: {SignerRole["role"]}, version {SignerRole["version"]}";

        #endregion

    }

}
