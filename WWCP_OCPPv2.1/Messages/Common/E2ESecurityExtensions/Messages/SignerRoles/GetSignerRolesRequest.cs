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
    /// The GetSignerRoles request: the role assignments a node has stored, as it received them.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4.
    /// </remarks>
    public class GetSignerRolesRequest : ARequest<GetSignerRolesRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getSignerRolesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The roles that are asked for: all of them, when none is named.
        /// </summary>
        [Optional]
        public IEnumerable<String> Roles                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetSignerRoles request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Roles">The roles that are asked for: all of them, when none is named.</param>
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
        public GetSignerRolesRequest(SourceRouting            Destination,
                                            IEnumerable<String>?     Roles = null,

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
                   nameof(GetSignerRolesRequest)[..^7],

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

            this.Roles = Roles?.Distinct().ToArray() ?? [];

            unchecked
            {
                hashCode = (this.Roles.Aggregate(0, (hash, role) => hash * 31 ^ role.GetHashCode())) * 3 ^
                           base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomGetSignerRolesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetSignerRoles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetSignerRolesRequestParser">An optional delegate to parse custom GetSignerRoles requests.</param>
        public static GetSignerRolesRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTimeOffset?                                             RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<GetSignerRolesRequest>?  CustomGetSignerRolesRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getSignerRolesRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetSignerRolesRequestParser))
            {
                return getSignerRolesRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetSignerRoles request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetSignerRolesRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetSignerRoles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetSignerRolesRequest">The parsed GetSignerRoles request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetSignerRolesRequestParser">An optional delegate to parse custom GetSignerRoles requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out GetSignerRolesRequest?      GetSignerRolesRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTimeOffset?                                             RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<GetSignerRolesRequest>?  CustomGetSignerRolesRequestParser   = null)
        {

            try
            {

                GetSignerRolesRequest = null;

                #region Roles                [optional]

                var Roles = new List<String>();

                if (JSON["roles"] is JToken rolesToken)
                {

                    if (rolesToken is not JArray rolesArray ||
                        rolesArray.Any(role => role.Type != JTokenType.String || role.Value<String>()!.Length == 0))
                    {
                        ErrorResponse = "The roles are not an array of role names!";
                        return false;
                    }

                    Roles.AddRange(rolesArray.Select(role => role.Value<String>()!));

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


                GetSignerRolesRequest = new GetSignerRolesRequest(

                                                   Destination,
                                                   Roles,

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

                if (CustomGetSignerRolesRequestParser is not null)
                    GetSignerRolesRequest = CustomGetSignerRolesRequestParser(JSON,
                                                                                            GetSignerRolesRequest);

                return true;

            }
            catch (Exception e)
            {
                GetSignerRolesRequest  = null;
                ErrorResponse                 = "The given JSON representation of a GetSignerRoles request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetSignerRolesRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetSignerRolesRequestSerializer">A delegate to serialize custom GetSignerRoles requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetSignerRolesRequest>?  CustomGetSignerRolesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           Roles.Any()
                               ? new JProperty("roles",               new JArray(Roles))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetSignerRolesRequestSerializer is not null
                       ? CustomGetSignerRolesRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out GetSignerRolesRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a GetSignerRoles request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="GetSignerRolesRequest">The GetSignerRoles request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetSignerRolesRequestParser">A delegate to read custom GetSignerRoles requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                                CBOR,
                                           Request_Id                                               RequestId,
                                           SourceRouting                                            Destination,
                                           NetworkPath                                              NetworkPath,
                                           [NotNullWhen(true)]  out GetSignerRolesRequest?   GetSignerRolesRequest,
                                           [NotNullWhen(false)] out String?                         ErrorResponse,
                                           DateTimeOffset?                                          RequestTimestamp                           = null,
                                           TimeSpan?                                                RequestTimeout                             = null,
                                           EventTracking_Id?                                        EventTrackingId                            = null,
                                           CustomCBORParserDelegate<GetSignerRolesRequest>?  CustomGetSignerRolesRequestParser   = null)
        {

            try
            {

                GetSignerRolesRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a GetSignerRoles request is not a map!";
                    return false;
                }

                CBOR.ParseOptionalList<String>("roles",
                                               "roles",
                                               SignedObjectCBORExtensions.TryParseText,
                                               out var Roles,
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


                GetSignerRolesRequest = new GetSignerRolesRequest(

                                                   Destination,
                                                   Roles,

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

                if (CustomGetSignerRolesRequestParser is not null)
                    GetSignerRolesRequest = CustomGetSignerRolesRequestParser(CBOR,
                                                                                            GetSignerRolesRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetSignerRolesRequest = null;
                ErrorResponse = "The given CBOR representation of a GetSignerRoles request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomGetSignerRolesRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this GetSignerRoles request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomGetSignerRolesRequestSerializer">A delegate to serialize custom GetSignerRoles requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<GetSignerRolesRequest>? CustomGetSignerRolesRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("roles",                     OCPPCBORExtensions.Array(Roles, CBORValue.FromText)),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomGetSignerRolesRequestSerializer is not null
                       ? CustomGetSignerRolesRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetSignerRolesRequest1, GetSignerRolesRequest2)

        /// <summary>
        /// Compares two GetSignerRoles requests for equality.
        /// </summary>
        /// <param name="GetSignerRolesRequest1">A GetSignerRoles request.</param>
        /// <param name="GetSignerRolesRequest2">Another GetSignerRoles request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetSignerRolesRequest? GetSignerRolesRequest1,
                                           GetSignerRolesRequest? GetSignerRolesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetSignerRolesRequest1, GetSignerRolesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetSignerRolesRequest1 is null || GetSignerRolesRequest2 is null)
                return false;

            return GetSignerRolesRequest1.Equals(GetSignerRolesRequest2);

        }

        #endregion

        #region Operator != (GetSignerRolesRequest1, GetSignerRolesRequest2)

        /// <summary>
        /// Compares two GetSignerRoles requests for inequality.
        /// </summary>
        /// <param name="GetSignerRolesRequest1">A GetSignerRoles request.</param>
        /// <param name="GetSignerRolesRequest2">Another GetSignerRoles request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetSignerRolesRequest? GetSignerRolesRequest1,
                                           GetSignerRolesRequest? GetSignerRolesRequest2)

            => !(GetSignerRolesRequest1 == GetSignerRolesRequest2);

        #endregion

        #endregion

        #region IEquatable<GetSignerRolesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetSignerRoles requests for equality.
        /// </summary>
        /// <param name="Object">A GetSignerRoles request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetSignerRolesRequest getSignerRolesRequest &&
                   Equals(getSignerRolesRequest);

        #endregion

        #region Equals(GetSignerRolesRequest)

        /// <summary>
        /// Compares two GetSignerRoles requests for equality.
        /// </summary>
        /// <param name="GetSignerRolesRequest">A GetSignerRoles request to compare with.</param>
        public override Boolean Equals(GetSignerRolesRequest? GetSignerRolesRequest)

            => GetSignerRolesRequest is not null &&

               Roles.SequenceEqual(GetSignerRolesRequest.Roles) &&

               base.      GenericEquals(GetSignerRolesRequest);

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

            => Roles.Any() ? $"Roles: {String.Join(", ", Roles)}" : "All roles";

        #endregion

    }

}
